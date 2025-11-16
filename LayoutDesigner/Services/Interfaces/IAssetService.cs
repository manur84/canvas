using System.Collections.Generic;
using System.Threading.Tasks;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for managing image assets
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Gets the full path to an asset
        /// </summary>
        string GetAssetPath(string relativePath);

        /// <summary>
        /// Copies an external image to the assets folder
        /// </summary>
        Task<string?> ImportAssetAsync(string sourceFilePath);

        /// <summary>
        /// Checks if an asset exists
        /// </summary>
        bool AssetExists(string relativePath);

        /// <summary>
        /// Gets all available assets
        /// </summary>
        List<string> GetAllAssets();
    }
}
