using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
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

            // Add mouse wheel zoom support
            DesignCanvas.MouseWheel += OnCanvasMouseWheel;

            // Add drag & drop support for images
            DesignCanvas.AllowDrop = true;
            DesignCanvas.DragEnter += OnCanvasDragEnter;
            DesignCanvas.Drop += OnCanvasDrop;

            // Add keyboard shortcut support
            PreviewKeyDown += OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is not MainViewModel viewModel)
                return;

            // Let CanvasViewModel handle the key
            if (viewModel.CanvasViewModel.HandleKeyDown(e.Key, Keyboard.Modifiers))
            {
                e.Handled = true;
            }
        }

        private void OnCanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is not MainViewModel viewModel) return;

            // Only zoom when Ctrl is held
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0)
                {
                    viewModel.ZoomInCommand.Execute(null);
                }
                else if (e.Delta < 0)
                {
                    viewModel.ZoomOutCommand.Execute(null);
                }

                e.Handled = true;
            }
        }

        private void OnCanvasDragEnter(object sender, DragEventArgs e)
        {
            // Check if the data contains file(s)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    // Check if any file is an image
                    var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                    var hasImage = files.Any(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()));

                    if (hasImage)
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void OnCanvasDrop(object sender, DragEventArgs e)
        {
            if (DataContext is not MainViewModel viewModel) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files == null || files.Length == 0) return;

                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                // Get drop position on canvas
                var dropPosition = e.GetPosition(DesignCanvas);

                // Account for zoom
                var zoom = viewModel.CanvasViewModel.Zoom;
                var x = dropPosition.X / zoom;
                var y = dropPosition.Y / zoom;

                // Add each image file to the canvas
                foreach (var file in files)
                {
                    if (imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                    {
                        var imageElement = new LayoutDesigner.Models.ImageElement
                        {
                            Name = $"Image {viewModel.CanvasViewModel.Elements.Count + 1}",
                            X = x,
                            Y = y,
                            Width = 200,
                            Height = 200,
                            ImagePath = file,
                            StretchMode = "Uniform"
                        };

                        viewModel.CanvasViewModel.Elements.Add(imageElement);

                        // Offset next image position
                        x += 20;
                        y += 20;
                    }
                }

                e.Handled = true;
            }
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
