using System;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Basic console-based logger implementation
    /// </summary>
    public class ConsoleLogger : IAppLogger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void LogError(Exception ex, string message)
        {
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.WriteLine($"        Exception: {ex.GetType().Name}");
            Console.WriteLine($"        Message: {ex.Message}");
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                Console.WriteLine($"        StackTrace: {ex.StackTrace}");
            }
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"[WARN] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
}
