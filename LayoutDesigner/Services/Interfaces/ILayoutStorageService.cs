using LayoutDesigner.Models;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for saving and loading layouts
    /// </summary>
    public interface ILayoutStorageService
    {
        /// <summary>
        /// Saves a layout document to a file
        /// </summary>
        Task<bool> SaveLayoutAsync(LayoutDocument document, string filePath);

        /// <summary>
        /// Loads a layout document from a file
        /// </summary>
        Task<LayoutDocument?> LoadLayoutAsync(string filePath);

        /// <summary>
        /// Gets the most recently used files
        /// </summary>
        List<string> GetRecentFiles();

        /// <summary>
        /// Adds a file to the recent files list
        /// </summary>
        void AddRecentFile(string filePath);
    }
}
