using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LayoutDesigner.Views;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// Color input control with preview and picker button
    /// </summary>
    public partial class ColorInputControl : UserControl
    {
        public static readonly DependencyProperty ColorValueProperty =
            DependencyProperty.Register(
                nameof(ColorValue),
                typeof(string),
                typeof(ColorInputControl),
                new FrameworkPropertyMetadata(
                    "#FFFFFFFF",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnColorValueChanged));

        public static readonly DependencyProperty ShowTransparencyPatternProperty =
            DependencyProperty.Register(
                nameof(ShowTransparencyPattern),
                typeof(bool),
                typeof(ColorInputControl),
                new PropertyMetadata(false));

        private static readonly DependencyPropertyKey ColorBrushPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ColorBrush),
                typeof(SolidColorBrush),
                typeof(ColorInputControl),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty ColorBrushProperty = ColorBrushPropertyKey.DependencyProperty;

        public ColorInputControl()
        {
            InitializeComponent();
            UpdateColorBrush();
        }

        /// <summary>
        /// Gets or sets the color value as hex string
        /// </summary>
        public string ColorValue
        {
            get => (string)GetValue(ColorValueProperty);
            set => SetValue(ColorValueProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show the transparency checkered pattern
        /// </summary>
        public bool ShowTransparencyPattern
        {
            get => (bool)GetValue(ShowTransparencyPatternProperty);
            set => SetValue(ShowTransparencyPatternProperty, value);
        }

        /// <summary>
        /// Gets the color as a SolidColorBrush for preview
        /// </summary>
        public SolidColorBrush ColorBrush
        {
            get => (SolidColorBrush)GetValue(ColorBrushProperty);
            private set => SetValue(ColorBrushPropertyKey, value);
        }

        private static void OnColorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorInputControl control)
            {
                control.UpdateColorBrush();
            }
        }

        private void UpdateColorBrush()
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(ColorValue ?? "#FFFFFFFF");
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                ColorBrush = brush;
            }
            catch
            {
                // Invalid color - use white
                ColorBrush = new SolidColorBrush(Colors.White);
            }
        }

        private void OnPickColorClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var selectedColor = ColorPickerDialog.ShowDialog(window, ColorValue);

            if (selectedColor != null)
            {
                ColorValue = selectedColor;
            }
        }
    }
}
