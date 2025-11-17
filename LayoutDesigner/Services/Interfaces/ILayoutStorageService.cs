using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        Task<bool> SaveLayoutAsync(LayoutDocument document, string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads a layout document from a file
        /// </summary>
        Task<LayoutDocument?> LoadLayoutAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the most recently used files
        /// </summary>
        Task<List<string>> GetRecentFilesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a file to the recent files list
        /// </summary>
        Task AddRecentFileAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
