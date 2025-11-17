using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LayoutDesigner.Events;
using LayoutDesigner.Helpers;
using LayoutDesigner.Services;
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
        private readonly IErrorHandlingService _errorHandler;
        private readonly IDirtyTrackingService _dirtyTrackingService;
        private string? _currentFilePath;
        private bool _isDirty;
        private List<string> _recentFiles;

        /// <summary>
        /// Event raised when export is requested
        /// </summary>
        public event EventHandler<ExportRequestedEventArgs>? ExportRequested;

        public MainViewModel()
        {
            _storageService = App.Services.Resolve<ILayoutStorageService>();
            _undoRedoService = App.Services.Resolve<IUndoRedoService>();
            _errorHandler = App.Services.Resolve<IErrorHandlingService>();
            _dirtyTrackingService = App.Services.Resolve<IDirtyTrackingService>();
            _recentFiles = new List<string>();

            CanvasViewModel = new CanvasViewModel();
            PropertyPanelViewModel = new PropertyPanelViewModel(CanvasViewModel);

            // Subscribe to changes
            CanvasViewModel.Document.Elements.CollectionChanged += (s, e) => IsDirty = true;
            _undoRedoService.StateChanged += OnUndoRedoStateChanged;
            _dirtyTrackingService.DirtyStateChanged += (s, e) => OnPropertyChanged(nameof(DirtyElementsCount));

            // Commands
            NewCommand = new RelayCommand(async () => await NewAsync());
            OpenCommand = new RelayCommand(async () => await OpenAsync());
            SaveCommand = new RelayCommand(async () => await SaveAsync(), () => CanvasViewModel.Elements.Count > 0);
            SaveAsCommand = new RelayCommand(async () => await SaveAsAsync(), () => CanvasViewModel.Elements.Count > 0);
            ExitCommand = new RelayCommand(async () => await ExitAsync());
            OpenRecentCommand = new RelayCommand<string>(async (path) => await OpenRecentAsync(path));

            UndoCommand = new RelayCommand(() => _undoRedoService.Undo(), () => _undoRedoService.CanUndo);
            RedoCommand = new RelayCommand(() => _undoRedoService.Redo(), () => _undoRedoService.CanRedo);

            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ZoomResetCommand = new RelayCommand(ZoomReset);

            ExportPngCommand = new RelayCommand(async () => await ExportPngAsync(), () => CanvasViewModel.Elements.Count > 0);
            ExportJpgCommand = new RelayCommand(async () => await ExportJpgAsync(), () => CanvasViewModel.Elements.Count > 0);

            // Load recent files
            LoadRecentFiles();
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

        public int DirtyElementsCount => _dirtyTrackingService.DirtyCount;

        public List<string> RecentFiles
        {
            get => _recentFiles;
            private set => SetProperty(ref _recentFiles, value);
        }

        #endregion

        #region Commands

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand OpenRecentCommand { get; }

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

            try
            {
                var document = await _storageService.LoadLayoutAsync(filePath);
                if (document != null)
                {
                    CanvasViewModel.LoadDocument(document);
                    CurrentFilePath = filePath;
                    IsDirty = false;

                    // Refresh recent files list
                    LoadRecentFiles();
                }
                else
                {
                    _errorHandler.HandleError(
                        new InvalidOperationException("Document loaded as null"),
                        "Failed to load layout file.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, "Failed to load layout file.");
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
            try
            {
                var success = await _storageService.SaveLayoutAsync(CanvasViewModel.Document, filePath);
                if (success)
                {
                    CurrentFilePath = filePath;
                    IsDirty = false;

                    // Clear dirty tracking after successful save
                    _dirtyTrackingService.ClearAll();

                    // Refresh recent files list
                    LoadRecentFiles();
                }
                else
                {
                    _errorHandler.HandleError(
                        new InvalidOperationException("Save operation returned false"),
                        "Failed to save layout file.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, "Failed to save layout file.");
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

            // Note: Confirm returns true for Yes, false for No
            // We need 3-way logic, so we use the error handler's Confirm as Yes/No only
            var shouldSave = _errorHandler.Confirm("Do you want to save changes?", "Unsaved Changes");

            if (shouldSave)
            {
                await SaveAsync();
                return !IsDirty; // Return false if save failed
            }

            return true; // User chose "No" - continue without saving
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
                _errorHandler.HandleInfo($"Layout exported successfully to:\n{filePath}", "Export Successful");
            }
            else
            {
                _errorHandler.HandleError(
                    new InvalidOperationException("Export operation failed"),
                    "Failed to export layout. Please try again.");
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
                _errorHandler.HandleInfo($"Layout exported successfully to:\n{filePath}", "Export Successful");
            }
            else
            {
                _errorHandler.HandleError(
                    new InvalidOperationException("Export operation failed"),
                    "Failed to export layout. Please try again.");
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

        private async void LoadRecentFiles()
        {
            var files = await _storageService.GetRecentFilesAsync();
            RecentFiles = files;
        }

        private async Task OpenRecentAsync(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            if (!await CheckSaveChangesAsync())
                return;

            try
            {
                var document = await _storageService.LoadLayoutAsync(filePath);
                if (document != null)
                {
                    CanvasViewModel.LoadDocument(document);
                    CurrentFilePath = filePath;
                    IsDirty = false;

                    // Refresh recent files list
                    LoadRecentFiles();
                }
                else
                {
                    _errorHandler.HandleError(
                        new InvalidOperationException("Document loaded as null"),
                        "Failed to load recent file.");

                    // Remove invalid file from recent list
                    var updatedFiles = RecentFiles.Where(f => f != filePath).ToList();
                    RecentFiles = updatedFiles;
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex, "Failed to load recent file.");

                // Remove invalid file from recent list
                var updatedFiles = RecentFiles.Where(f => f != filePath).ToList();
                RecentFiles = updatedFiles;
            }
        }

        #endregion
    }
}
