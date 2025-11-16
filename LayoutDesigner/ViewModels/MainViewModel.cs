using LayoutDesigner.Events;
using LayoutDesigner.Helpers;
using LayoutDesigner.Services.Interfaces;
using LayoutDesigner.ViewModels.Base;
using LayoutDesigner.Constants;
using System.Windows;
using System.Windows.Input;

namespace LayoutDesigner.ViewModels
{
    /// <summary>
    /// Main ViewModel for the application window
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly ILayoutStorageService _storageService;
        private readonly IUndoRedoService _undoRedoService;
        private string? _currentFilePath;
        private bool _isDirty;

        /// <summary>
        /// Event raised when export is requested
        /// </summary>
        public event EventHandler<ExportRequestedEventArgs>? ExportRequested;

        public MainViewModel()
        {
            _storageService = App.Services.Resolve<ILayoutStorageService>();
            _undoRedoService = App.Services.Resolve<IUndoRedoService>();

            CanvasViewModel = new CanvasViewModel();
            PropertyPanelViewModel = new PropertyPanelViewModel(CanvasViewModel);

            // Subscribe to changes
            CanvasViewModel.Document.Elements.CollectionChanged += (s, e) => IsDirty = true;
            _undoRedoService.StateChanged += OnUndoRedoStateChanged;

            // Commands
            NewCommand = new RelayCommand(async () => await NewAsync());
            OpenCommand = new RelayCommand(async () => await OpenAsync());
            SaveCommand = new RelayCommand(async () => await SaveAsync(), () => CanvasViewModel.Elements.Count > 0);
            SaveAsCommand = new RelayCommand(async () => await SaveAsAsync(), () => CanvasViewModel.Elements.Count > 0);
            ExitCommand = new RelayCommand(async () => await ExitAsync());

            UndoCommand = new RelayCommand(() => _undoRedoService.Undo(), () => _undoRedoService.CanUndo);
            RedoCommand = new RelayCommand(() => _undoRedoService.Redo(), () => _undoRedoService.CanRedo);

            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ZoomResetCommand = new RelayCommand(ZoomReset);

            ExportPngCommand = new RelayCommand(async () => await ExportPngAsync(), () => CanvasViewModel.Elements.Count > 0);
            ExportJpgCommand = new RelayCommand(async () => await ExportJpgAsync(), () => CanvasViewModel.Elements.Count > 0);
        }

        #region Properties

        public CanvasViewModel CanvasViewModel { get; }
        public PropertyPanelViewModel PropertyPanelViewModel { get; }

        public string? CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                if (SetProperty(ref _currentFilePath, value))
                {
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (SetProperty(ref _isDirty, value))
                {
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Title
        {
            get
            {
                var fileName = string.IsNullOrEmpty(CurrentFilePath)
                    ? "Untitled"
                    : System.IO.Path.GetFileNameWithoutExtension(CurrentFilePath);

                var dirtyMark = IsDirty ? "*" : "";
                return $"{fileName}{dirtyMark} - Layout Designer";
            }
        }

        public int UndoCount => _undoRedoService.UndoHistory.Count;
        public int RedoCount => _undoRedoService.RedoHistory.Count;
        public string UndoRedoStatus => $"Undo: {UndoCount} | Redo: {RedoCount}";

        #endregion

        #region Commands

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand ExitCommand { get; }

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ZoomResetCommand { get; }

        public ICommand ExportPngCommand { get; }
        public ICommand ExportJpgCommand { get; }

        #endregion

        #region Methods

        private async Task NewAsync()
        {
            if (!await CheckSaveChangesAsync())
                return;

            CanvasViewModel.NewDocument();
            CurrentFilePath = null;
            IsDirty = false;
        }

        private async Task OpenAsync()
        {
            if (!await CheckSaveChangesAsync())
                return;

            var filePath = FileDialogHelper.ShowOpenLayoutDialog();
            if (filePath == null)
                return;

            var document = await _storageService.LoadLayoutAsync(filePath);
            if (document != null)
            {
                CanvasViewModel.LoadDocument(document);
                CurrentFilePath = filePath;
                IsDirty = false;
            }
            else
            {
                MessageBox.Show("Failed to load layout file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                await SaveAsAsync();
            }
            else
            {
                await SaveToFileAsync(CurrentFilePath);
            }
        }

        private async Task SaveAsAsync()
        {
            var filePath = FileDialogHelper.ShowSaveLayoutDialog(CanvasViewModel.Document.Name);
            if (filePath == null)
                return;

            await SaveToFileAsync(filePath);
        }

        private async Task SaveToFileAsync(string filePath)
        {
            var success = await _storageService.SaveLayoutAsync(CanvasViewModel.Document, filePath);
            if (success)
            {
                CurrentFilePath = filePath;
                IsDirty = false;
            }
            else
            {
                MessageBox.Show("Failed to save layout file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExitAsync()
        {
            if (await CheckSaveChangesAsync())
            {
                Application.Current.Shutdown();
            }
        }

        private async Task<bool> CheckSaveChangesAsync()
        {
            if (!IsDirty)
                return true;

            var result = MessageBox.Show(
                "Do you want to save changes?",
                "Unsaved Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await SaveAsync();
                return !IsDirty; // Return false if save failed
            }

            return result != MessageBoxResult.Cancel;
        }

        private void ZoomIn()
        {
            CanvasViewModel.Zoom *= UIConstants.ZoomFactor;
        }

        private void ZoomOut()
        {
            CanvasViewModel.Zoom /= UIConstants.ZoomFactor;
        }

        private void ZoomReset()
        {
            CanvasViewModel.Zoom = 1.0;
            CanvasViewModel.PanX = 0;
            CanvasViewModel.PanY = 0;
        }

        private async Task ExportPngAsync()
        {
            var filePath = FileDialogHelper.ShowExportImageDialog(CanvasViewModel.Document.Name);
            if (filePath == null)
                return;

            // Ensure .png extension
            if (!filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                filePath += ".png";
            }

            // Raise event for view to handle
            var args = new ExportRequestedEventArgs(filePath, ExportFormat.Png);
            ExportRequested?.Invoke(this, args);

            if (args.Success)
            {
                MessageBox.Show($"Layout exported successfully to:\n{filePath}", "Export Successful",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to export layout. Please try again.", "Export Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExportJpgAsync()
        {
            var filePath = FileDialogHelper.ShowExportImageDialog(CanvasViewModel.Document.Name);
            if (filePath == null)
                return;

            // Ensure .jpg extension
            if (!filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                filePath += ".jpg";
            }

            // Raise event for view to handle
            var args = new ExportRequestedEventArgs(filePath, ExportFormat.Jpg);
            ExportRequested?.Invoke(this, args);

            if (args.Success)
            {
                MessageBox.Show($"Layout exported successfully to:\n{filePath}", "Export Successful",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to export layout. Please try again.", "Export Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnUndoRedoStateChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(UndoCommand));
            OnPropertyChanged(nameof(RedoCommand));
            OnPropertyChanged(nameof(UndoCount));
            OnPropertyChanged(nameof(RedoCount));
            OnPropertyChanged(nameof(UndoRedoStatus));
        }

        #endregion
    }
}
