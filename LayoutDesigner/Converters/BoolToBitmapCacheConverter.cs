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
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool enableCache && enableCache)
            {
                // Best Practice: RenderAtScale = 1.0 for crisp rendering
                // Can be adjusted to 2.0 for high-DPI displays
                return new BitmapCache
                {
                    RenderAtScale = 1.0,
                    SnapsToDevicePixels = true
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is BitmapCache;
        }
    }
}
