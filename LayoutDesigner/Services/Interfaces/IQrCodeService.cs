using System.Windows.Media.Imaging;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for generating QR codes
    /// </summary>
    public interface IQrCodeService
    {
        /// <summary>
        /// Generates a QR code image from content
        /// </summary>
        /// <param name="content">Content to encode</param>
        /// <param name="pixelsPerModule">Size per module</param>
        /// <param name="foregroundColor">Foreground color (hex)</param>
        /// <param name="backgroundColor">Background color (hex)</param>
        /// <param name="errorCorrectionLevel">Error correction level (0-3)</param>
        /// <returns>BitmapSource of the QR code</returns>
        BitmapSource GenerateQrCode(
            string content,
            int pixelsPerModule = 10,
            string foregroundColor = "#000000",
            string backgroundColor = "#FFFFFF",
            int errorCorrectionLevel = 1);
    }
}
