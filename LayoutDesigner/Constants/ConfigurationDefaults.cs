using System;
using System.IO;

namespace LayoutDesigner.Constants
{
    /// <summary>
    /// Configuration defaults for file paths and application settings
    /// Best Practice: Centralize configuration constants
    /// </summary>
    public static class ConfigurationDefaults
    {
        /// <summary>
        /// Application data directory name
        /// </summary>
        public const string AppDataDirectoryName = "LayoutDesigner";

        /// <summary>
        /// Templates subdirectory name
        /// </summary>
        public const string TemplatesDirectoryName = "Templates";

        /// <summary>
        /// Settings file name
        /// </summary>
        public const string SettingsFileName = "settings.json";

        /// <summary>
        /// Maximum number of recent files to track
        /// </summary>
        public const int MaxRecentFiles = 10;

        /// <summary>
        /// Recent files settings key
        /// </summary>
        public const string RecentFilesKey = "RecentFiles";

        /// <summary>
        /// Gets the application data directory path
        /// </summary>
        public static string AppDataPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppDataDirectoryName);

        /// <summary>
        /// Gets the templates directory path
        /// </summary>
        public static string TemplatesPath => Path.Combine(
            AppDataPath,
            TemplatesDirectoryName);

        /// <summary>
        /// Gets the settings file path
        /// </summary>
        public static string SettingsFilePath => Path.Combine(
            AppDataPath,
            SettingsFileName);
    }
}
