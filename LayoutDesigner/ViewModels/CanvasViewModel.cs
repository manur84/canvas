using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;
using LayoutDesigner.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace LayoutDesigner.ViewModels
{
    /// <summary>
    /// ViewModel for the canvas containing all layout elements
    /// </summary>
    public class CanvasViewModel : ViewModelBase
    {
        private readonly IUndoRedoService _undoRedoService;
        private LayoutDocument _document;
        private ObservableCollection<LayoutElementBase> _selectedElements;
        private double _zoom = 1.0;
        private double _panX;
        private double _panY;
        private bool _showGrid = true;
        private double _gridSize = 20;
        private bool _snapToGrid = true;

        public CanvasViewModel()
        {
            _undoRedoService = App.Services.Resolve<IUndoRedoService>();
            _document = new LayoutDocument();
            _selectedElements = new ObservableCollection<LayoutElementBase>();

            // Commands
            AddTextCommand = new RelayCommand(AddText);
            AddImageCommand = new RelayCommand(AddImage);
            AddShapeCommand = new RelayCommand<string>(AddShape);
            AddQrCodeCommand = new RelayCommand(AddQrCode);
            AddDynamicFieldCommand = new RelayCommand(AddDynamicField);
            DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => SelectedElements.Count > 0);
            DuplicateSelectedCommand = new RelayCommand(DuplicateSelected, () => SelectedElements.Count > 0);
            SelectAllCommand = new RelayCommand(SelectAll);
            BringToFrontCommand = new RelayCommand(BringToFront, () => SelectedElements.Count > 0);
            SendToBackCommand = new RelayCommand(SendToBack, () => SelectedElements.Count > 0);
            BringForwardCommand = new RelayCommand(BringForward, () => SelectedElements.Count > 0);
            SendBackwardCommand = new RelayCommand(SendBackward, () => SelectedElements.Count > 0);
        }

        #region Properties

        public LayoutDocument Document
        {
            get => _document;
            set => SetProperty(ref _document, value);
        }

        public ObservableCollection<LayoutElementBase> Elements => Document.Elements;

        public ObservableCollection<LayoutElementBase> SelectedElements
        {
            get => _selectedElements;
            set => SetProperty(ref _selectedElements, value);
        }

        public double Zoom
        {
            get => _zoom;
            set => SetProperty(ref _zoom, Math.Clamp(value, 0.1, 10.0));
        }

        public double PanX
        {
            get => _panX;
            set => SetProperty(ref _panX, value);
        }

        public double PanY
        {
            get => _panY;
            set => SetProperty(ref _panY, value);
        }

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                if (SetProperty(ref _showGrid, value))
                {
                    Document.ShowGrid = value;
                }
            }
        }

        public double GridSize
        {
            get => _gridSize;
            set
            {
                if (SetProperty(ref _gridSize, value))
                {
                    Document.GridSize = value;
                }
            }
        }

        public bool SnapToGrid
        {
            get => _snapToGrid;
            set
            {
                if (SetProperty(ref _snapToGrid, value))
                {
                    Document.SnapToGrid = value;
                }
            }
        }

        public double CanvasWidth
        {
            get => Document.CanvasWidth;
            set
            {
                if (Document.CanvasWidth != value)
                {
                    Document.CanvasWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double CanvasHeight
        {
            get => Document.CanvasHeight;
            set
            {
                if (Document.CanvasHeight != value)
                {
                    Document.CanvasHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand AddTextCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand AddShapeCommand { get; }
        public ICommand AddQrCodeCommand { get; }
        public ICommand AddDynamicFieldCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand DuplicateSelectedCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand BringToFrontCommand { get; }
        public ICommand SendToBackCommand { get; }
        public ICommand BringForwardCommand { get; }
        public ICommand SendBackwardCommand { get; }

        #endregion

        #region Methods

        private void AddText()
        {
            var element = new TextElement
            {
                Name = $"Text {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 200,
                Height = 50,
                Text = "New Text"
            };

            AddElement(element, "Add Text");
        }

        private void AddImage()
        {
            var element = new ImageElement
            {
                Name = $"Image {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 200,
                Height = 200
            };

            AddElement(element, "Add Image");
        }

        private void AddShape(string? shapeType)
        {
            var shape = shapeType switch
            {
                "Rectangle" => ShapeType.Rectangle,
                "Ellipse" => ShapeType.Ellipse,
                "Line" => ShapeType.Line,
                "RoundedRectangle" => ShapeType.RoundedRectangle,
                _ => ShapeType.Rectangle
            };

            var element = new ShapeElement
            {
                Name = $"{shape} {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 150,
                Height = 150,
                ShapeType = shape
            };

            AddElement(element, $"Add {shape}");
        }

        private void AddQrCode()
        {
            var element = new QrCodeElement
            {
                Name = $"QR Code {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 150,
                Height = 150,
                Content = "https://example.com"
            };

            AddElement(element, "Add QR Code");
        }

        private void AddDynamicField()
        {
            var element = new DynamicFieldElement
            {
                Name = $"Dynamic Field {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 250,
                Height = 40,
                FieldType = DynamicFieldType.DateTime
            };

            AddElement(element, "Add Dynamic Field");
        }

        private void AddElement(LayoutElementBase element, string actionName)
        {
            Elements.Add(element);

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () => Elements.Remove(element),
                redoAction: () => Elements.Add(element),
                description: actionName
            ));

            SelectedElements.Clear();
            SelectedElements.Add(element);
        }

        private void DeleteSelected()
        {
            if (SelectedElements.Count == 0) return;

            var elementsToDelete = SelectedElements.ToList();

            foreach (var element in elementsToDelete)
            {
                Elements.Remove(element);
            }

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var element in elementsToDelete)
                    {
                        Elements.Add(element);
                    }
                },
                redoAction: () =>
                {
                    foreach (var element in elementsToDelete)
                    {
                        Elements.Remove(element);
                    }
                },
                description: $"Delete {elementsToDelete.Count} element(s)"
            ));

            SelectedElements.Clear();
        }

        private void DuplicateSelected()
        {
            if (SelectedElements.Count == 0) return;

            var newElements = new List<LayoutElementBase>();

            foreach (var element in SelectedElements)
            {
                var clone = element.Clone();
                newElements.Add(clone);
                Elements.Add(clone);
            }

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var element in newElements)
                    {
                        Elements.Remove(element);
                    }
                },
                redoAction: () =>
                {
                    foreach (var element in newElements)
                    {
                        Elements.Add(element);
                    }
                },
                description: $"Duplicate {newElements.Count} element(s)"
            ));

            SelectedElements.Clear();
            foreach (var element in newElements)
            {
                SelectedElements.Add(element);
            }
        }

        private void SelectAll()
        {
            SelectedElements.Clear();
            foreach (var element in Elements)
            {
                SelectedElements.Add(element);
            }
        }

        private void BringToFront()
        {
            if (SelectedElements.Count == 0) return;

            int maxZ = Elements.Count > 0 ? Elements.Max(e => e.ZIndex) : 0;

            foreach (var element in SelectedElements)
            {
                element.ZIndex = ++maxZ;
            }
        }

        private void SendToBack()
        {
            if (SelectedElements.Count == 0) return;

            int minZ = Elements.Count > 0 ? Elements.Min(e => e.ZIndex) : 0;

            foreach (var element in SelectedElements)
            {
                element.ZIndex = --minZ;
            }
        }

        private void BringForward()
        {
            if (SelectedElements.Count == 0) return;

            foreach (var element in SelectedElements.OrderByDescending(e => e.ZIndex))
            {
                element.ZIndex++;
            }
        }

        private void SendBackward()
        {
            if (SelectedElements.Count == 0) return;

            foreach (var element in SelectedElements.OrderBy(e => e.ZIndex))
            {
                element.ZIndex--;
            }
        }

        public void LoadDocument(LayoutDocument document)
        {
            Document = document;
            SelectedElements.Clear();
            Zoom = 1.0;
            PanX = 0;
            PanY = 0;
            ShowGrid = document.ShowGrid;
            GridSize = document.GridSize;
            SnapToGrid = document.SnapToGrid;

            OnPropertyChanged(nameof(CanvasWidth));
            OnPropertyChanged(nameof(CanvasHeight));
        }

        public void NewDocument()
        {
            LoadDocument(new LayoutDocument());
            _undoRedoService.Clear();
        }

        #region Alignment Methods

        public void AlignLeft()
        {
            if (SelectedElements.Count < 2) return;

            var minX = SelectedElements.Min(e => e.X);
            foreach (var element in SelectedElements)
            {
                element.X = minX;
            }
        }

        public void AlignCenter()
        {
            if (SelectedElements.Count < 2) return;

            var centerX = SelectedElements.Average(e => e.X + e.Width / 2);
            foreach (var element in SelectedElements)
            {
                element.X = centerX - element.Width / 2;
            }
        }

        public void AlignRight()
        {
            if (SelectedElements.Count < 2) return;

            var maxX = SelectedElements.Max(e => e.X + e.Width);
            foreach (var element in SelectedElements)
            {
                element.X = maxX - element.Width;
            }
        }

        public void AlignTop()
        {
            if (SelectedElements.Count < 2) return;

            var minY = SelectedElements.Min(e => e.Y);
            foreach (var element in SelectedElements)
            {
                element.Y = minY;
            }
        }

        public void AlignMiddle()
        {
            if (SelectedElements.Count < 2) return;

            var centerY = SelectedElements.Average(e => e.Y + e.Height / 2);
            foreach (var element in SelectedElements)
            {
                element.Y = centerY - element.Height / 2;
            }
        }

        public void AlignBottom()
        {
            if (SelectedElements.Count < 2) return;

            var maxY = SelectedElements.Max(e => e.Y + e.Height);
            foreach (var element in SelectedElements)
            {
                element.Y = maxY - element.Height;
            }
        }

        public void DistributeHorizontally()
        {
            if (SelectedElements.Count < 3) return;

            var sorted = SelectedElements.OrderBy(e => e.X).ToList();
            var leftMost = sorted.First().X;
            var rightMost = sorted.Last().X + sorted.Last().Width;
            var totalWidth = sorted.Sum(e => e.Width);
            var spacing = (rightMost - leftMost - totalWidth) / (sorted.Count - 1);

            double currentX = leftMost;
            foreach (var element in sorted)
            {
                element.X = currentX;
                currentX += element.Width + spacing;
            }
        }

        public void DistributeVertically()
        {
            if (SelectedElements.Count < 3) return;

            var sorted = SelectedElements.OrderBy(e => e.Y).ToList();
            var topMost = sorted.First().Y;
            var bottomMost = sorted.Last().Y + sorted.Last().Height;
            var totalHeight = sorted.Sum(e => e.Height);
            var spacing = (bottomMost - topMost - totalHeight) / (sorted.Count - 1);

            double currentY = topMost;
            foreach (var element in sorted)
            {
                element.Y = currentY;
                currentY += element.Height + spacing;
            }
        }

        #endregion

        #endregion
    }
}
