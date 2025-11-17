using System;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Central logging abstraction for the application
    /// </summary>
    public interface IAppLogger
    {
        /// <summary>
        /// Logs an informational message
        /// </summary>
        void LogInfo(string message);

        /// <summary>
        /// Logs an error with exception details
        /// </summary>
        void LogError(Exception ex, string message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        void LogWarning(string message);
    }
}
