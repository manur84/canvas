using System;
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
                _errorHandlingService.HandleError(ex, "Error generating QR code", showDialog: false);

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
                _errorHandlingService.HandleError(ex, $"Invalid color format: {hex}", showDialog: false);
            }

            // Fallback to black for invalid colors
            return Colors.Black;
        }

        private BitmapSource CreateErrorBitmap()
        {
            const int width = 100;
            const int height = 100;
            const int bytesPerPixel = 4;
            int stride = width * bytesPerPixel;

            // Create red error bitmap using byte array (safe alternative to unsafe code)
            byte[] pixelData = new byte[height * stride];
            for (int i = 0; i < pixelData.Length; i += bytesPerPixel)
            {
                pixelData[i] = 0;      // Blue
                pixelData[i + 1] = 0;  // Green
                pixelData[i + 2] = 255; // Red
                pixelData[i + 3] = 255; // Alpha
            }

            var bitmap = BitmapSource.Create(
                width, height,
                96, 96,
                PixelFormats.Bgra32,
                null,
                pixelData,
                stride);

            bitmap.Freeze();
            return bitmap;
        }
    }
}
