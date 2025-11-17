using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LayoutDesigner.Constants;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Application settings with persistence
    /// Best Practice: Centralized settings management
    /// </summary>
    public class ApplicationSettings : INotifyPropertyChanged
    {
        private static ApplicationSettings? _instance;

        #region Singleton

        public static ApplicationSettings Instance => _instance ??= Load();

        #endregion

        #region Properties

        // Grid settings
        private bool _showGrid = true;
        private double _gridSize = 20;
        private bool _snapToGrid = true;
        private double _snapDistance = 8;

        // UI settings
        private double _defaultZoom = 1.0;
        private bool _showSnapLines = true;
        private bool _showStatusBar = true;
        private bool _showToolbox = true;
        private bool _showPropertyPanel = true;

        // Auto-save settings
        private bool _autoSaveEnabled = true;
        private int _autoSaveIntervalMinutes = 5;

        // Recent files
        private List<string> _recentFiles = new();
        private int _maxRecentFiles = 10;

        // Canvas defaults
        private double _defaultCanvasWidth = 800;
        private double _defaultCanvasHeight = 600;
        private string _defaultBackgroundColor = "#FFFFFFFF";

        // Keyboard settings
        private double _nudgeDistance = 1;
        private double _largeNudgeDistance = 10;

        // Export settings
        private string _lastExportPath = "";
        private string _defaultExportFormat = "PNG";
        private int _exportQuality = 95;

        // Grid Settings
        public bool ShowGrid
        {
            get => _showGrid;
            set => SetProperty(ref _showGrid, value);
        }

        public double GridSize
        {
            get => _gridSize;
            set => SetProperty(ref _gridSize, value);
        }

        public bool SnapToGrid
        {
            get => _snapToGrid;
            set => SetProperty(ref _snapToGrid, value);
        }

        public double SnapDistance
        {
            get => _snapDistance;
            set => SetProperty(ref _snapDistance, value);
        }

        // UI Settings
        public double DefaultZoom
        {
            get => _defaultZoom;
            set => SetProperty(ref _defaultZoom, value);
        }

        public bool ShowSnapLines
        {
            get => _showSnapLines;
            set => SetProperty(ref _showSnapLines, value);
        }

        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set => SetProperty(ref _showStatusBar, value);
        }

        public bool ShowToolbox
        {
            get => _showToolbox;
            set => SetProperty(ref _showToolbox, value);
        }

        public bool ShowPropertyPanel
        {
            get => _showPropertyPanel;
            set => SetProperty(ref _showPropertyPanel, value);
        }

        // Auto-save Settings
        public bool AutoSaveEnabled
        {
            get => _autoSaveEnabled;
            set => SetProperty(ref _autoSaveEnabled, value);
        }

        public int AutoSaveIntervalMinutes
        {
            get => _autoSaveIntervalMinutes;
            set => SetProperty(ref _autoSaveIntervalMinutes, Math.Max(1, value));
        }

        // Recent Files
        public List<string> RecentFiles
        {
            get => _recentFiles;
            set => SetProperty(ref _recentFiles, value);
        }

        public int MaxRecentFiles
        {
            get => _maxRecentFiles;
            set => SetProperty(ref _maxRecentFiles, Math.Max(1, Math.Min(20, value)));
        }

        // Canvas Defaults
        public double DefaultCanvasWidth
        {
            get => _defaultCanvasWidth;
            set => SetProperty(ref _defaultCanvasWidth, value);
        }

        public double DefaultCanvasHeight
        {
            get => _defaultCanvasHeight;
            set => SetProperty(ref _defaultCanvasHeight, value);
        }

        public string DefaultBackgroundColor
        {
            get => _defaultBackgroundColor;
            set => SetProperty(ref _defaultBackgroundColor, value);
        }

        // Keyboard Settings
        public double NudgeDistance
        {
            get => _nudgeDistance;
            set => SetProperty(ref _nudgeDistance, value);
        }

        public double LargeNudgeDistance
        {
            get => _largeNudgeDistance;
            set => SetProperty(ref _largeNudgeDistance, value);
        }

        // Export Settings
        public string LastExportPath
        {
            get => _lastExportPath;
            set => SetProperty(ref _lastExportPath, value);
        }

        public string DefaultExportFormat
        {
            get => _defaultExportFormat;
            set => SetProperty(ref _defaultExportFormat, value);
        }

        public int ExportQuality
        {
            get => _exportQuality;
            set => SetProperty(ref _exportQuality, Math.Max(1, Math.Min(100, value)));
        }

        #endregion

        #region Recent Files Management

        /// <summary>
        /// Adds a file to recent files list
        /// </summary>
        public void AddRecentFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            // Remove if already exists
            RecentFiles.Remove(filePath);

            // Add to beginning
            RecentFiles.Insert(0, filePath);

            // Limit to max count
            if (RecentFiles.Count > MaxRecentFiles)
            {
                RecentFiles.RemoveRange(MaxRecentFiles, RecentFiles.Count - MaxRecentFiles);
            }

            OnPropertyChanged(nameof(RecentFiles));
        }

        /// <summary>
        /// Removes a file from recent files list
        /// </summary>
        public void RemoveRecentFile(string filePath)
        {
            if (RecentFiles.Remove(filePath))
            {
                OnPropertyChanged(nameof(RecentFiles));
            }
        }

        /// <summary>
        /// Clears all recent files
        /// </summary>
        public void ClearRecentFiles()
        {
            RecentFiles.Clear();
            OnPropertyChanged(nameof(RecentFiles));
        }

        #endregion

        #region Persistence

        /// <summary>
        /// Saves settings to file asynchronously
        /// </summary>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            var logger = ServiceContainer.GetService<IAppLogger>();
            try
            {
                var directory = Path.GetDirectoryName(ConfigurationDefaults.SettingsFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(ConfigurationDefaults.SettingsFilePath, json, cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to save application settings");
                var errorService = ServiceContainer.GetService<Services.Interfaces.IErrorHandlingService>();
                errorService?.HandleError(ex, "Failed to save settings", showDialog: false);
            }
        }

        /// <summary>
        /// Saves settings to file synchronously (for backward compatibility)
        /// </summary>
        public void Save()
        {
            // Use async version but wait synchronously (not ideal but maintains compatibility)
            SaveAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Loads settings from file
        /// </summary>
        public static ApplicationSettings Load()
        {
            var logger = ServiceContainer.GetService<IAppLogger>();
            try
            {
                if (File.Exists(ConfigurationDefaults.SettingsFilePath))
                {
                    var json = File.ReadAllText(ConfigurationDefaults.SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<ApplicationSettings>(json);
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to load application settings");
                var errorService = ServiceContainer.GetService<Services.Interfaces.IErrorHandlingService>();
                errorService?.HandleError(ex, "Failed to load settings", showDialog: false);
            }

            return new ApplicationSettings();
        }

        /// <summary>
        /// Resets all settings to defaults
        /// </summary>
        public void ResetToDefaults()
        {
            ShowGrid = true;
            GridSize = 20;
            SnapToGrid = true;
            SnapDistance = 8;
            DefaultZoom = 1.0;
            ShowSnapLines = true;
            ShowStatusBar = true;
            ShowToolbox = true;
            ShowPropertyPanel = true;
            AutoSaveEnabled = true;
            AutoSaveIntervalMinutes = 5;
            MaxRecentFiles = 10;
            DefaultCanvasWidth = 800;
            DefaultCanvasHeight = 600;
            DefaultBackgroundColor = "#FFFFFFFF";
            NudgeDistance = 1;
            LargeNudgeDistance = 10;
            DefaultExportFormat = "PNG";
            ExportQuality = 95;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);

            // Auto-save on change
            Save();

            return true;
        }

        #endregion
    }
}
