using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts bool to FontStyle (Italic or Normal)
    /// </summary>
    public class BoolToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isItalic = value is bool b && b;
            return isItalic ? FontStyles.Italic : FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is FontStyle style && style == FontStyles.Italic;
        }
    }
}
