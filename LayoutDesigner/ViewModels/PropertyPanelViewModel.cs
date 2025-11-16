using LayoutDesigner.Models.Base;
using LayoutDesigner.ViewModels.Base;
using System.Collections.Specialized;

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
        }

        public LayoutElementBase? SelectedElement
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
        }

        public bool HasSelection => SelectedElement != null;

        public bool HasMultipleSelection => _canvasViewModel.SelectedElements.Count > 1;

        // Canvas properties (exposed for editing when no element is selected)
        public double CanvasWidth
        {
            get => _canvasViewModel.CanvasWidth;
            set
            {
                _canvasViewModel.CanvasWidth = value;
                OnPropertyChanged();
            }
        }

        public double CanvasHeight
        {
            get => _canvasViewModel.CanvasHeight;
            set
            {
                _canvasViewModel.CanvasHeight = value;
                OnPropertyChanged();
            }
        }

        public double GridSize
        {
            get => _canvasViewModel.GridSize;
            set
            {
                _canvasViewModel.GridSize = value;
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
        }
    }
}
