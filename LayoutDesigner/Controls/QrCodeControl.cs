using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LayoutDesigner.Models;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// Custom control for rendering QR codes
    /// Uses QrCodeService to generate QR code images
    /// </summary>
    public class QrCodeControl : Control
    {
        private readonly Image _image;
        private IQrCodeService? _qrCodeService;

        static QrCodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QrCodeControl),
                new FrameworkPropertyMetadata(typeof(QrCodeControl)));
        }

        public QrCodeControl()
        {
            _image = new Image
            {
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            AddVisualChild(_image);
            AddLogicalChild(_image);

            Loaded += OnLoaded;
            DataContextChanged += OnDataContextChanged;
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? _image : throw new ArgumentOutOfRangeException(nameof(index));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _image.Measure(constraint);
            return _image.DesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _image.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Get QR code service from DI container
            _qrCodeService = ServiceContainer.GetService<IQrCodeService>();
            UpdateQrCode();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Unsubscribe from old element
            if (e.OldValue is QrCodeElement oldElement)
            {
                oldElement.PropertyChanged -= OnElementPropertyChanged;
            }

            // Subscribe to new element
            if (e.NewValue is QrCodeElement newElement)
            {
                newElement.PropertyChanged += OnElementPropertyChanged;
                UpdateQrCode();
            }
        }

        private void OnElementPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Update QR code when relevant properties change
            if (e.PropertyName is nameof(QrCodeElement.Content) or
                nameof(QrCodeElement.ForegroundColor) or
                nameof(QrCodeElement.BackgroundColor) or
                nameof(QrCodeElement.ErrorCorrectionLevel) or
                nameof(QrCodeElement.Width) or
                nameof(QrCodeElement.Height))
            {
                UpdateQrCode();
            }
        }

        private void UpdateQrCode()
        {
            if (DataContext is not QrCodeElement element || _qrCodeService == null)
            {
                _image.Source = null;
                return;
            }

            try
            {
                // Calculate pixels per module based on element size
                // Aim for approximately 200-300 pixels for good quality
                var size = Math.Min(element.Width, element.Height);
                var pixelsPerModule = Math.Max(1, (int)(size / 30)); // Approximately 30 modules

                // Generate QR code using the service
                var qrCodeBitmap = _qrCodeService.GenerateQrCode(
                    element.Content,
                    pixelsPerModule,
                    element.ForegroundColor,
                    element.BackgroundColor,
                    element.ErrorCorrectionLevel);

                _image.Source = qrCodeBitmap;
            }
            catch
            {
                // On error, clear the image (error placeholder is handled by QrCodeService)
                _image.Source = null;
            }
        }
    }
}
