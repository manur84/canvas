using System;
using System.IO;
using System.Windows;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Implementation of error handling service
    /// </summary>
    public class ErrorHandlingService : IErrorHandlingService
    {
        /// <summary>
        /// Handles exceptions with logging and optional user notification
        /// </summary>
        public void HandleError(Exception exception, string? userMessage = null, bool showDialog = true)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            // Log error (in production, use proper logging framework like Serilog)
            LogError(exception);

            if (showDialog)
            {
                var message = userMessage ?? GetUserFriendlyMessage(exception);
                ShowErrorDialog(message, "Error");
            }
        }

        /// <summary>
        /// Shows a warning message to the user
        /// </summary>
        public void HandleWarning(string message, string? title = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            MessageBox.Show(
                message,
                title ?? "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        /// <summary>
        /// Shows an information message to the user
        /// </summary>
        public void HandleInfo(string message, string? title = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            MessageBox.Show(
                message,
                title ?? "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Shows a confirmation dialog
        /// </summary>
        public bool Confirm(string message, string? title = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            var result = MessageBox.Show(
                message,
                title ?? "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            return result == MessageBoxResult.Yes;
        }

        #region Private Methods

        private static void LogError(Exception exception)
        {
            // In production, use a proper logging framework
            // For now, write to debug output and trace
            System.Diagnostics.Debug.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"Type: {exception.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"Message: {exception.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {exception.StackTrace}");

            if (exception.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {exception.InnerException.Message}");
            }
        }

        private static void ShowErrorDialog(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private static string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                FileNotFoundException => "The requested file could not be found.",
                UnauthorizedAccessException => "Access to the file was denied. Please check permissions.",
                IOException => "An error occurred while accessing the file.",
                ArgumentNullException => "Invalid input: A required value was not provided.",
                ArgumentException => "Invalid input: " + exception.Message,
                InvalidOperationException => "The operation could not be completed: " + exception.Message,
                NotSupportedException => "This operation is not supported.",
                OutOfMemoryException => "The system is running low on memory. Please save your work and restart the application.",
                _ => $"An unexpected error occurred: {exception.Message}"
            };
        }

        #endregion
    }
}
