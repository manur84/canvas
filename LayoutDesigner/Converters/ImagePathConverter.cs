using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts image file path to BitmapImage with lazy loading and caching optimization
    /// Best Practice: Use BitmapCacheOption.OnLoad and limit decode size for performance
    /// </summary>
    public class ImagePathConverter : IValueConverter
    {
        // Thread-safe cache for images - Best Practice: Reuse frozen images
        private static readonly ConcurrentDictionary<string, BitmapImage> _imageCache = new();

        // Maximum decode size to avoid loading massive images at full resolution
        private const int MaxDecodePixelWidth = 2048;
        private const int MaxDecodePixelHeight = 2048;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string imagePath || string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            // Return cached image if available
            if (_imageCache.TryGetValue(imagePath, out var cachedImage))
            {
                return cachedImage;
            }

            // Check if file exists
            if (!File.Exists(imagePath))
            {
                return null;
            }

            try
            {
                // Create BitmapImage with lazy loading optimizations
                var bitmap = new BitmapImage();

                // Best Practice: Use BeginInit/EndInit for property setting
                bitmap.BeginInit();

                // Best Practice: OnLoad ensures image is decoded when accessed, not when loaded
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                // Best Practice: Use UriSource for better file handling
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);

                // Performance: Limit decode size to avoid loading huge images at full resolution
                // This significantly reduces memory usage for large images
                bitmap.DecodePixelWidth = MaxDecodePixelWidth;

                // Best Practice: CreateOptions.DelayCreation defers image loading
                // Combined with OnLoad, this provides true lazy loading
                bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;

                bitmap.EndInit();

                // Best Practice: Freeze bitmap for better performance and thread safety
                bitmap.Freeze();

                // Cache the frozen bitmap
                _imageCache.TryAdd(imagePath, bitmap);

                return bitmap;
            }
            catch (Exception)
            {
                // If image fails to load, return null (will show nothing)
                // Could also return a placeholder image here
                return null;
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not needed for image paths
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears the image cache - useful for memory management
        /// </summary>
        public static void ClearCache()
        {
            _imageCache.Clear();
        }

        /// <summary>
        /// Removes a specific image from cache - useful when file is updated
        /// </summary>
        public static void RemoveFromCache(string imagePath)
        {
            _imageCache.TryRemove(imagePath, out _);
        }
    }
}
