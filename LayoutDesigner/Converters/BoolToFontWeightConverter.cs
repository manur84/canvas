using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts bool to FontWeight (Bold or Normal)
    /// </summary>
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBold = value is bool b && b;
            return isBold ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is FontWeight weight && weight == FontWeights.Bold;
        }
    }
}
