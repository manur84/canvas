using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using LayoutDesigner.ViewModels;
using LayoutDesigner.Events;

namespace LayoutDesigner.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Subscribe to export events
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ExportRequested += OnExportRequested;
            }

            // Handle DataContext changes
            DataContextChanged += (s, e) =>
            {
                if (e.OldValue is MainViewModel oldVm)
                {
                    oldVm.ExportRequested -= OnExportRequested;
                }

                if (e.NewValue is MainViewModel newVm)
                {
                    newVm.ExportRequested += OnExportRequested;
                }
            };
        }

        private async void OnExportRequested(object? sender, ExportRequestedEventArgs e)
        {
            try
            {
                // Get the canvas element
                var canvas = DesignCanvas;
                if (canvas == null)
                {
                    e.Success = false;
                    return;
                }

                // Create a visual brush to render the canvas
                var size = new Size(canvas.Width, canvas.Height);
                canvas.Measure(size);
                canvas.Arrange(new Rect(size));

                // Create render bitmap
                var renderBitmap = new RenderTargetBitmap(
                    (int)canvas.Width,
                    (int)canvas.Height,
                    96,
                    96,
                    PixelFormats.Pbgra32);

                renderBitmap.Render(canvas);

                // Encode and save
                BitmapEncoder encoder = e.Format switch
                {
                    ExportFormat.Png => new PngBitmapEncoder(),
                    ExportFormat.Jpg => new JpegBitmapEncoder { QualityLevel = 95 },
                    _ => new PngBitmapEncoder()
                };

                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                await using (var stream = new FileStream(e.FilePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                e.Success = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Export failed: {ex.Message}");
                e.Success = false;
            }
        }
    }
}
