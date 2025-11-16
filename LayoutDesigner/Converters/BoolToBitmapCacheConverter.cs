using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts boolean to BitmapCache for performance optimization
    /// Best Practice: Use BitmapCache for elements that are rendered frequently
    /// but change infrequently to improve performance
    /// </summary>
    public class BoolToBitmapCacheConverter : IValueConverter
    {
        // Static instance to avoid creating new BitmapCache objects repeatedly
        // Best Practice: Reuse BitmapCache instances for better memory efficiency
        private static readonly BitmapCache _cachedInstance = new()
        {
            RenderAtScale = 1.0,
            SnapsToDevicePixels = true,
            EnableClearType = true  // Better text rendering
        };

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool enableCache && enableCache)
            {
                return _cachedInstance;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is BitmapCache;
        }
    }
}
