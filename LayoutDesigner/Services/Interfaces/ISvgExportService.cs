using System.Threading.Tasks;
using LayoutDesigner.Models;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for exporting layouts to SVG format
    /// </summary>
    public interface ISvgExportService
    {
        /// <summary>
        /// Exports a layout document to SVG file
        /// </summary>
        /// <param name="document">The layout document to export</param>
        /// <param name="filePath">The output file path</param>
        /// <returns>True if export was successful</returns>
        Task<bool> ExportToSvgAsync(LayoutDocument document, string filePath);

        /// <summary>
        /// Generates SVG content from a layout document
        /// </summary>
        /// <param name="document">The layout document</param>
        /// <returns>SVG content as string</returns>
        string GenerateSvgContent(LayoutDocument document);
    }
}
