using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using LayoutDesigner.Models;

namespace LayoutDesigner.Converters
{
    /// <summary>
    /// Converts ImageStretchMode to WPF Stretch
    /// </summary>
    public class ImageStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ImageStretchMode mode)
            {
                return mode switch
                {
                    ImageStretchMode.None => Stretch.None,
                    ImageStretchMode.Fill => Stretch.Fill,
                    ImageStretchMode.Uniform => Stretch.Uniform,
                    ImageStretchMode.UniformToFill => Stretch.UniformToFill,
                    _ => Stretch.Uniform
                };
            }

            return Stretch.Uniform;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Stretch stretch)
            {
                return stretch switch
                {
                    Stretch.None => ImageStretchMode.None,
                    Stretch.Fill => ImageStretchMode.Fill,
                    Stretch.Uniform => ImageStretchMode.Uniform,
                    Stretch.UniformToFill => ImageStretchMode.UniformToFill,
                    _ => ImageStretchMode.Uniform
                };
            }

            return ImageStretchMode.Uniform;
        }
    }
}
