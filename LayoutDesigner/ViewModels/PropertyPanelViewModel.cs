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
