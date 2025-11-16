using LayoutDesigner.Services.Interfaces;
using System.IO;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Service for managing image and other assets
    /// </summary>
    public class AssetService : IAssetService
    {
        private readonly string _assetsFolderPath;

        public AssetService()
        {
            _assetsFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LayoutDesigner",
                "Assets");

            Directory.CreateDirectory(_assetsFolderPath);
        }

        public string GetAssetPath(string relativePath)
        {
            return Path.Combine(_assetsFolderPath, relativePath);
        }

        public async Task<string?> ImportAssetAsync(string sourceFilePath)
        {
            try
            {
                if (!File.Exists(sourceFilePath))
                    return null;

                var fileName = Path.GetFileName(sourceFilePath);
                var destPath = Path.Combine(_assetsFolderPath, fileName);

                // If file exists, add a number suffix
                int counter = 1;
                while (File.Exists(destPath))
                {
                    var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    var ext = Path.GetExtension(fileName);
                    destPath = Path.Combine(_assetsFolderPath, $"{nameWithoutExt}_{counter}{ext}");
                    counter++;
                }

                await Task.Run(() => File.Copy(sourceFilePath, destPath));

                return Path.GetFileName(destPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error importing asset: {ex.Message}");
                return null;
            }
        }

        public bool AssetExists(string relativePath)
        {
            var fullPath = GetAssetPath(relativePath);
            return File.Exists(fullPath);
        }

        public List<string> GetAllAssets()
        {
            try
            {
                var extensions = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" };
                var files = new List<string>();

                foreach (var ext in extensions)
                {
                    files.AddRange(Directory.GetFiles(_assetsFolderPath, ext));
                }

                return files.Select(Path.GetFileName).Where(f => f != null).Cast<string>().ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting assets: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
