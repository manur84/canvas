using LayoutDesigner.Services.Interfaces;
using QRCoder;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Service for generating QR codes using QRCoder library
    /// </summary>
    public class QrCodeService : IQrCodeService
    {
        private readonly IErrorHandlingService _errorHandlingService;

        public QrCodeService(IErrorHandlingService errorHandlingService)
        {
            _errorHandlingService = errorHandlingService;
        }
        public BitmapSource GenerateQrCode(
            string content,
            int pixelsPerModule = 10,
            string foregroundColor = "#000000",
            string backgroundColor = "#FFFFFF",
            int errorCorrectionLevel = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = "Empty";
                }

                // Create QR code
                using var qrGenerator = new QRCodeGenerator();
                var eccLevel = errorCorrectionLevel switch
                {
                    0 => QRCodeGenerator.ECCLevel.L,
                    1 => QRCodeGenerator.ECCLevel.M,
                    2 => QRCodeGenerator.ECCLevel.Q,
                    3 => QRCodeGenerator.ECCLevel.H,
                    _ => QRCodeGenerator.ECCLevel.M
                };

                using var qrCodeData = qrGenerator.CreateQrCode(content, eccLevel);
                using var qrCode = new PngByteQRCode(qrCodeData);

                // Parse colors
                var fgColor = ColorFromHex(foregroundColor);
                var bgColor = ColorFromHex(backgroundColor);

                // Generate PNG bytes
                var pngBytes = qrCode.GetGraphic(
                    pixelsPerModule,
                    new byte[] { fgColor.R, fgColor.G, fgColor.B },
                    new byte[] { bgColor.R, bgColor.G, bgColor.B });

                // Convert to BitmapSource
                using var stream = new System.IO.MemoryStream(pngBytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleError(ex, "Error generating QR code", showMessageBox: false);

                // Return a simple error placeholder
                return CreateErrorBitmap();
            }
        }

        private Color ColorFromHex(string hex)
        {
            try
            {
                hex = hex.TrimStart('#');

                if (hex.Length == 6)
                {
                    hex = "FF" + hex; // Add full opacity
                }

                if (hex.Length == 8)
                {
                    byte a = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte r = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(4, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(6, 2), 16);
                    return Color.FromArgb(a, r, g, b);
                }
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleError(ex, $"Invalid color format: {hex}", showMessageBox: false);
            }

            // Fallback to black for invalid colors
            return Colors.Black;
        }

        private BitmapSource CreateErrorBitmap()
        {
            var bitmap = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Bgra32, null);
            bitmap.Lock();

            unsafe
            {
                var backBuffer = (int*)bitmap.BackBuffer.ToPointer();
                int stride = bitmap.BackBufferStride / 4;

                for (int y = 0; y < 100; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        backBuffer[y * stride + x] = unchecked((int)0xFFFF0000); // Red
                    }
                }
            }

            bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, 100, 100));
            bitmap.Unlock();
            bitmap.Freeze();

            return bitmap;
        }
    }
}
