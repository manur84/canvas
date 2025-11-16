using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts bool to Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;

            // Check if we should invert
            bool invert = parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            if (invert)
                boolValue = !boolValue;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value is Visibility visibility && visibility == Visibility.Visible;

            // Check if we should invert
            bool invert = parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            return invert ? !result : result;
        }
    }
}
