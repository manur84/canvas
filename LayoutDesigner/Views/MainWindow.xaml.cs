using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
using LayoutDesigner.ViewModels;
using LayoutDesigner.Events;
using LayoutDesigner.Commands;
using LayoutDesigner.Constants;

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

            // Clean up event handlers when window is unloaded
            Unloaded += OnWindowUnloaded;

            // Bind RoutedUICommands to ViewModel commands
            BindRoutedCommands();
        }

        private void BindRoutedCommands()
        {
            // Bind LayoutCommands to CanvasViewModel commands
            CommandBindings.Add(new CommandBinding(LayoutCommands.Duplicate,
                (s, e) => ExecuteCanvasCommand(vm => vm.DuplicateSelectedCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.DuplicateSelectedCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.Delete,
                (s, e) => ExecuteCanvasCommand(vm => vm.DeleteSelectedCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.DeleteSelectedCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.BringToFront,
                (s, e) => ExecuteCanvasCommand(vm => vm.BringToFrontCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.BringToFrontCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.SendToBack,
                (s, e) => ExecuteCanvasCommand(vm => vm.SendToBackCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.SendToBackCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.BringForward,
                (s, e) => ExecuteCanvasCommand(vm => vm.BringForwardCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.BringForwardCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.SendBackward,
                (s, e) => ExecuteCanvasCommand(vm => vm.SendBackwardCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.SendBackwardCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.AlignLeft,
                (s, e) => ExecuteCanvasCommand(vm => vm.AlignLeftCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.AlignLeftCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.AlignCenter,
                (s, e) => ExecuteCanvasCommand(vm => vm.AlignCenterCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.AlignCenterCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.AlignRight,
                (s, e) => ExecuteCanvasCommand(vm => vm.AlignRightCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.AlignRightCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.AlignTop,
                (s, e) => ExecuteCanvasCommand(vm => vm.AlignTopCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.AlignTopCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.AlignMiddle,
                (s, e) => ExecuteCanvasCommand(vm => vm.AlignMiddleCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.AlignMiddleCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.AlignBottom,
                (s, e) => ExecuteCanvasCommand(vm => vm.AlignBottomCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.AlignBottomCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.DistributeHorizontally,
                (s, e) => ExecuteCanvasCommand(vm => vm.DistributeHorizontallyCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.DistributeHorizontallyCommand)));

            CommandBindings.Add(new CommandBinding(LayoutCommands.DistributeVertically,
                (s, e) => ExecuteCanvasCommand(vm => vm.DistributeVerticallyCommand),
                (s, e) => e.CanExecute = CanExecuteCanvasCommand(vm => vm.DistributeVerticallyCommand)));
        }

        private void ExecuteCanvasCommand(Func<CanvasViewModel, ICommand> commandSelector)
        {
            if (DataContext is MainViewModel viewModel)
            {
                var command = commandSelector(viewModel.CanvasViewModel);
                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }

        private bool CanExecuteCanvasCommand(Func<CanvasViewModel, ICommand> commandSelector)
        {
            if (DataContext is MainViewModel viewModel)
            {
                var command = commandSelector(viewModel.CanvasViewModel);
                return command.CanExecute(null);
            }
            return false;
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from all event handlers to prevent memory leaks
            DesignCanvas.MouseWheel -= OnCanvasMouseWheel;
            DesignCanvas.DragEnter -= OnCanvasDragEnter;
            DesignCanvas.Drop -= OnCanvasDrop;
            PreviewKeyDown -= OnPreviewKeyDown;
            Unloaded -= OnWindowUnloaded;

            // DataContext change handler will automatically clean up ExportRequested
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
                        x += UIConstants.DropOffsetPixels;
                        y += UIConstants.DropOffsetPixels;
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
                var errorService = ServiceContainer.GetService<Services.Interfaces.IErrorHandlingService>();
                errorService?.HandleError(ex, "Export failed", showMessageBox: false);
                e.Success = false;
            }
        }
    }
}
