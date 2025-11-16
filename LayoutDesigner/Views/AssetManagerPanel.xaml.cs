using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LayoutDesigner.Helpers;
using LayoutDesigner.Models;
using LayoutDesigner.Services;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Views
{
    /// <summary>
    /// Asset Manager Panel with thumbnail view
    /// </summary>
    public partial class AssetManagerPanel : UserControl
    {
        private readonly IAssetService _assetService;
        private readonly IErrorHandlingService _errorHandler;

        public AssetManagerPanel()
        {
            InitializeComponent();

            _assetService = App.Services.Resolve<IAssetService>();
            _errorHandler = App.Services.Resolve<IErrorHandlingService>();

            LoadAssets();
        }

        /// <summary>
        /// Event raised when an asset is selected for insertion
        /// </summary>
        public event EventHandler<AssetSelectedEventArgs>? AssetSelected;

        private void LoadAssets()
        {
            try
            {
                var assetFileNames = _assetService.GetAllAssets();
                var assetItems = new List<AssetItem>();

                foreach (var fileName in assetFileNames)
                {
                    var fullPath = _assetService.GetAssetPath(fileName);

                    if (File.Exists(fullPath))
                    {
                        var fileInfo = new FileInfo(fullPath);

                        assetItems.Add(new AssetItem
                        {
                            FileName = fileName,
                            FullPath = fullPath,
                            FileSize = fileInfo.Length,
                            DateModified = fileInfo.LastWriteTime
                        });
                    }
                }

                // Sort by date modified (newest first)
                assetItems = assetItems.OrderByDescending(a => a.DateModified).ToList();

                AssetsList.ItemsSource = assetItems;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, "Failed to load assets", showDialog: false);
            }
        }

        private async void OnImportClick(object sender, RoutedEventArgs e)
        {
            var filePath = FileDialogHelper.ShowOpenImageDialog();
            if (filePath == null)
                return;

            try
            {
                var relativePath = await _assetService.ImportAssetAsync(filePath);

                if (relativePath != null)
                {
                    LoadAssets(); // Refresh the list
                    _errorHandler.HandleInfo($"Asset imported successfully:\n{relativePath}", "Import Complete");
                }
                else
                {
                    _errorHandler.HandleError(
                        new InvalidOperationException("Import returned null"),
                        "Failed to import asset");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, "Failed to import asset");
            }
        }

        private void OnAssetClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border { DataContext: AssetItem asset })
            {
                AssetSelected?.Invoke(this, new AssetSelectedEventArgs(asset.FullPath));
            }
        }

        private void OnInsertAssetClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: AssetItem asset })
            {
                AssetSelected?.Invoke(this, new AssetSelectedEventArgs(asset.FullPath));
            }
        }

        private void OnDeleteAssetClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem { DataContext: AssetItem asset })
                return;

            var confirm = _errorHandler.Confirm(
                $"Are you sure you want to delete this asset?\n\n{asset.FileName}",
                "Delete Asset");

            if (!confirm)
                return;

            try
            {
                if (File.Exists(asset.FullPath))
                {
                    File.Delete(asset.FullPath);
                    LoadAssets(); // Refresh the list
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, "Failed to delete asset");
            }
        }

        /// <summary>
        /// Refresh the asset list (called when assets change externally)
        /// </summary>
        public void RefreshAssets()
        {
            LoadAssets();
        }
    }

    /// <summary>
    /// Event args for asset selection
    /// </summary>
    public class AssetSelectedEventArgs : EventArgs
    {
        public string AssetPath { get; }

        public AssetSelectedEventArgs(string assetPath)
        {
            AssetPath = assetPath;
        }
    }
}
