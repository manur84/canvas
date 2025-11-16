using System;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Centralized error handling service
    /// Best Practice: Consistent error handling and user feedback
    /// </summary>
    public interface IErrorHandlingService
    {
        void HandleError(Exception exception, string? userMessage = null, bool showDialog = true);
        void HandleWarning(string message, string? title = null);
        void HandleInfo(string message, string? title = null);
        bool Confirm(string message, string? title = null);
    }
}
