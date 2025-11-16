using System.Globalization;
using System.Windows;
using System.Windows.Data;
using LayoutDesigner.Models;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts ShapeType enum to Visibility based on expected shape type
    /// </summary>
    public class ShapeTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            if (value is ShapeType shapeType && Enum.TryParse<ShapeType>(parameter.ToString(), out var expectedType))
            {
                return shapeType == expectedType ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-way converter, ConvertBack not supported
            return Binding.DoNothing;
        }
    }
}
