using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts object type to Visibility based on type name
    /// </summary>
    public class TypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            var typeName = value.GetType().Name;
            var expectedType = parameter.ToString();

            return typeName == expectedType ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
