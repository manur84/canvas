using System;
using System.Globalization;
using System.Windows.Data;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts boolean values to "Yes" or "No" strings
    /// </summary>
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Yes" : "No";
            }
            return "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return strValue.Equals("Yes", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
