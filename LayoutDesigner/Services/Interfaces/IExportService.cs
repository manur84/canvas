using System.Windows;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for exporting layouts to images
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Exports a UIElement to a PNG file
        /// </summary>
        Task<bool> ExportToPngAsync(UIElement element, string filePath, double width, double height);

        /// <summary>
        /// Exports a UIElement to a JPG file
        /// </summary>
        Task<bool> ExportToJpgAsync(UIElement element, string filePath, double width, double height, int quality = 95);
    }
}
