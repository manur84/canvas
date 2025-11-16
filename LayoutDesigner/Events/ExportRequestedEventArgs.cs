using System;

namespace LayoutDesigner.Events
{
    /// <summary>
    /// Event arguments for export requests
    /// </summary>
    public class ExportRequestedEventArgs : EventArgs
    {
        public ExportRequestedEventArgs(string filePath, ExportFormat format)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Format = format;
        }

        /// <summary>
        /// Path where to save the exported file
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Export format
        /// </summary>
        public ExportFormat Format { get; }

        /// <summary>
        /// Set to true by the event handler if export was successful
        /// </summary>
        public bool Success { get; set; }
    }

    /// <summary>
    /// Export format types
    /// </summary>
    public enum ExportFormat
    {
        Png,
        Jpg
    }
}
