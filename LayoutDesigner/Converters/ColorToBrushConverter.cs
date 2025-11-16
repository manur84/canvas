using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts hex color string to SolidColorBrush with caching for performance
    /// Best Practice: Cache and freeze brushes to avoid creating new objects on every binding update
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        // Thread-safe cache for brushes - Best Practice: Reuse frozen brushes
        private static readonly ConcurrentDictionary<string, SolidColorBrush> _brushCache = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                // Return cached brush if available
                if (_brushCache.TryGetValue(colorString, out var cachedBrush))
                {
                    return cachedBrush;
                }

                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(colorString);
                    var brush = new SolidColorBrush(color);

                    // Best Practice: Freeze brush for better performance
                    brush.Freeze();

                    // Cache the frozen brush
                    _brushCache.TryAdd(colorString, brush);

                    return brush;
                }
                catch
                {
                    return Brushes.Black;
                }
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color.ToString();
            }

            return "#FF000000";
        }
    }
}
