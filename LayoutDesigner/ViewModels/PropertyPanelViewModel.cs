using LayoutDesigner.Models.Base;
using LayoutDesigner.Models;
using LayoutDesigner.ViewModels.Base;
using LayoutDesigner.Helpers;
using System.Collections.Specialized;
using System.Windows.Input;

namespace LayoutDesigner.ViewModels
{
    /// <summary>
    /// ViewModel for the property panel showing selected element properties
    /// </summary>
    public class PropertyPanelViewModel : ViewModelBase
    {
        private readonly CanvasViewModel _canvasViewModel;
        private LayoutElementBase? _selectedElement;

        public PropertyPanelViewModel(CanvasViewModel canvasViewModel)
        {
            _canvasViewModel = canvasViewModel;
            _canvasViewModel.SelectedElements.CollectionChanged += OnSelectionChanged;

            // Commands
            BrowseImageCommand = new RelayCommand(BrowseImage, () => SelectedElement is ImageElement);
        }

        public LayoutElementBase? SelectedElement
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
        }

        public bool HasSelection => SelectedElement != null;

        public bool HasMultipleSelection => _canvasViewModel.SelectedElements.Count > 1;

        // Commands
        public ICommand BrowseImageCommand { get; }

        // Canvas properties (exposed for editing when no element is selected)
        public double CanvasWidth
        {
            get => _canvasViewModel.CanvasWidth;
            set
            {
                // Validation: Canvas width must be between 100 and 10000 pixels
                var validatedValue = Math.Clamp(value, 100, 10000);
                _canvasViewModel.CanvasWidth = validatedValue;
                OnPropertyChanged();
            }
        }

        public double CanvasHeight
        {
            get => _canvasViewModel.CanvasHeight;
            set
            {
                // Validation: Canvas height must be between 100 and 10000 pixels
                var validatedValue = Math.Clamp(value, 100, 10000);
                _canvasViewModel.CanvasHeight = validatedValue;
                OnPropertyChanged();
            }
        }

        public double GridSize
        {
            get => _canvasViewModel.GridSize;
            set
            {
                // Validation: Grid size must be between 5 and 100 pixels
                var validatedValue = Math.Clamp(value, 5, 100);
                _canvasViewModel.GridSize = validatedValue;
                OnPropertyChanged();
            }
        }

        public bool ShowGrid
        {
            get => _canvasViewModel.ShowGrid;
            set
            {
                _canvasViewModel.ShowGrid = value;
                OnPropertyChanged();
            }
        }

        public bool SnapToGrid
        {
            get => _canvasViewModel.SnapToGrid;
            set
            {
                _canvasViewModel.SnapToGrid = value;
                OnPropertyChanged();
            }
        }

        public string BackgroundColor
        {
            get => _canvasViewModel.Document.BackgroundColor;
            set
            {
                _canvasViewModel.Document.BackgroundColor = value;
                OnPropertyChanged();
            }
        }

        private void OnSelectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedElement = _canvasViewModel.SelectedElements.Count == 1
                ? _canvasViewModel.SelectedElements[0]
                : null;

            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(HasMultipleSelection));

            // Update command can execute state
            OnPropertyChanged(nameof(BrowseImageCommand));
        }

        private void BrowseImage()
        {
            if (SelectedElement is not ImageElement imageElement)
                return;

            var filePath = FileDialogHelper.ShowOpenImageDialog();
            if (filePath != null)
            {
                imageElement.ImagePath = filePath;
            }
        }
    }
}
