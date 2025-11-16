using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;
using LayoutDesigner.ViewModels.Base;
using LayoutDesigner.Constants;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace LayoutDesigner.ViewModels
{
    /// <summary>
    /// ViewModel for the canvas containing all layout elements
    /// </summary>
    public partial class CanvasViewModel : ViewModelBase
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
        private List<LayoutElementBase> _clipboard = new();

        // Partial method for grouping commands initialization
        partial void InitializeGroupingCommands();

        public CanvasViewModel()
        {
            _undoRedoService = App.Services.Resolve<IUndoRedoService>();
            _document = new LayoutDocument();
            _selectedElements = new ObservableCollection<LayoutElementBase>();

            // Subscribe to selection changes to update IsSelected property
            _selectedElements.CollectionChanged += OnSelectedElementsChanged;

            // Commands
            AddTextCommand = new RelayCommand(AddText);
            AddImageCommand = new RelayCommand(AddImage);
            AddShapeCommand = new RelayCommand<string>(AddShape);
            AddQrCodeCommand = new RelayCommand(AddQrCode);
            AddDynamicFieldCommand = new RelayCommand(AddDynamicField);
            AddLineCommand = new RelayCommand(AddLine);
            AddButtonCommand = new RelayCommand(AddButton);
            AddTableCommand = new RelayCommand(AddTable);
            DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => SelectedElements.Count > 0);
            DuplicateSelectedCommand = new RelayCommand(DuplicateSelected, () => SelectedElements.Count > 0);
            CopyCommand = new RelayCommand(Copy, () => SelectedElements.Count > 0);
            CutCommand = new RelayCommand(Cut, () => SelectedElements.Count > 0);
            PasteCommand = new RelayCommand(Paste, () => _clipboard.Count > 0);
            SelectAllCommand = new RelayCommand(SelectAll);
            BringToFrontCommand = new RelayCommand(BringToFront, () => SelectedElements.Count > 0);
            SendToBackCommand = new RelayCommand(SendToBack, () => SelectedElements.Count > 0);
            BringForwardCommand = new RelayCommand(BringForward, () => SelectedElements.Count > 0);
            SendBackwardCommand = new RelayCommand(SendBackward, () => SelectedElements.Count > 0);
            AlignLeftCommand = new RelayCommand(AlignLeft, () => SelectedElements.Count >= 2);
            AlignCenterCommand = new RelayCommand(AlignCenter, () => SelectedElements.Count >= 2);
            AlignRightCommand = new RelayCommand(AlignRight, () => SelectedElements.Count >= 2);
            AlignTopCommand = new RelayCommand(AlignTop, () => SelectedElements.Count >= 2);
            AlignMiddleCommand = new RelayCommand(AlignMiddle, () => SelectedElements.Count >= 2);
            AlignBottomCommand = new RelayCommand(AlignBottom, () => SelectedElements.Count >= 2);
            DistributeHorizontallyCommand = new RelayCommand(DistributeHorizontally, () => SelectedElements.Count >= 3);
            DistributeVerticallyCommand = new RelayCommand(DistributeVertically, () => SelectedElements.Count >= 3);
            LockCommand = new RelayCommand(LockElements, () => SelectedElements.Count > 0);
            UnlockCommand = new RelayCommand(UnlockElements, () => SelectedElements.Count > 0);

            // Initialize grouping commands from partial class
            InitializeGroupingCommands();
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
        public ICommand AddLineCommand { get; }
        public ICommand AddButtonCommand { get; }
        public ICommand AddTableCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand DuplicateSelectedCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand BringToFrontCommand { get; }
        public ICommand SendToBackCommand { get; }
        public ICommand BringForwardCommand { get; }
        public ICommand SendBackwardCommand { get; }
        public ICommand AlignLeftCommand { get; }
        public ICommand AlignCenterCommand { get; }
        public ICommand AlignRightCommand { get; }
        public ICommand AlignTopCommand { get; }
        public ICommand AlignMiddleCommand { get; }
        public ICommand AlignBottomCommand { get; }
        public ICommand DistributeHorizontallyCommand { get; }
        public ICommand DistributeVerticallyCommand { get; }
        public ICommand LockCommand { get; }
        public ICommand UnlockCommand { get; }

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

        private void AddLine()
        {
            var element = new LineElement
            {
                Name = $"Line {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 200,
                Height = 2,
                X2 = 250,
                Y2 = 50
            };

            AddElement(element, "Add Line");
        }

        private void AddButton()
        {
            var element = new ButtonElement
            {
                Name = $"Button {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 120,
                Height = 40,
                Text = "Click Me"
            };

            AddElement(element, "Add Button");
        }

        private void AddTable()
        {
            var element = new TableElement
            {
                Name = $"Table {Elements.Count + 1}",
                X = 50,
                Y = 50,
                Width = 400,
                Height = 200,
                Rows = 3,
                Columns = 3
            };

            AddElement(element, "Add Table");
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

        private void Copy()
        {
            if (SelectedElements.Count == 0) return;

            _clipboard.Clear();
            foreach (var element in SelectedElements)
            {
                _clipboard.Add(element.Clone());
            }
        }

        private void Cut()
        {
            if (SelectedElements.Count == 0) return;

            // Copy to clipboard
            _clipboard.Clear();
            foreach (var element in SelectedElements)
            {
                _clipboard.Add(element.Clone());
            }

            // Delete selected elements
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
                description: $"Cut {elementsToDelete.Count} element(s)"
            ));

            SelectedElements.Clear();
        }

        private void Paste()
        {
            if (_clipboard.Count == 0) return;

            var newElements = new List<LayoutElementBase>();

            foreach (var element in _clipboard)
            {
                var clone = element.Clone();
                // Offset the pasted element to make it visible
                clone.X += UIConstants.PasteOffsetPixels;
                clone.Y += UIConstants.PasteOffsetPixels;
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
                description: $"Paste {newElements.Count} element(s)"
            ));

            // Select the newly pasted elements
            SelectedElements.Clear();
            foreach (var element in newElements)
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

            // Save original positions for undo
            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.X);

            var minX = SelectedElements.Min(e => e.X);
            foreach (var element in SelectedElements)
            {
                element.X = minX;
            }

            // Add undo/redo action
            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.X = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    foreach (var element in originalPositions.Keys)
                    {
                        element.X = minX;
                    }
                },
                description: $"Align Left {SelectedElements.Count} element(s)"
            ));
        }

        public void AlignCenter()
        {
            if (SelectedElements.Count < 2) return;

            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.X);
            var centerX = SelectedElements.Average(e => e.X + e.Width / 2);

            foreach (var element in SelectedElements)
            {
                element.X = centerX - element.Width / 2;
            }

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.X = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    foreach (var element in originalPositions.Keys)
                    {
                        element.X = centerX - element.Width / 2;
                    }
                },
                description: $"Align Center {SelectedElements.Count} element(s)"
            ));
        }

        public void AlignRight()
        {
            if (SelectedElements.Count < 2) return;

            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.X);
            var maxX = SelectedElements.Max(e => e.X + e.Width);

            foreach (var element in SelectedElements)
            {
                element.X = maxX - element.Width;
            }

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.X = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    foreach (var element in originalPositions.Keys)
                    {
                        element.X = maxX - element.Width;
                    }
                },
                description: $"Align Right {SelectedElements.Count} element(s)"
            ));
        }

        public void AlignTop()
        {
            if (SelectedElements.Count < 2) return;

            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.Y);
            var minY = SelectedElements.Min(e => e.Y);

            foreach (var element in SelectedElements)
            {
                element.Y = minY;
            }

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.Y = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    foreach (var element in originalPositions.Keys)
                    {
                        element.Y = minY;
                    }
                },
                description: $"Align Top {SelectedElements.Count} element(s)"
            ));
        }

        public void AlignMiddle()
        {
            if (SelectedElements.Count < 2) return;

            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.Y);
            var centerY = SelectedElements.Average(e => e.Y + e.Height / 2);

            foreach (var element in SelectedElements)
            {
                element.Y = centerY - element.Height / 2;
            }

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.Y = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    var redoCenterY = originalPositions.Keys.Average(e => e.Y + e.Height / 2);
                    foreach (var element in originalPositions.Keys)
                    {
                        element.Y = redoCenterY - element.Height / 2;
                    }
                },
                description: $"Align Middle {SelectedElements.Count} element(s)"
            ));
        }

        public void AlignBottom()
        {
            if (SelectedElements.Count < 2) return;

            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.Y);
            var maxY = SelectedElements.Max(e => e.Y + e.Height);

            foreach (var element in SelectedElements)
            {
                element.Y = maxY - element.Height;
            }

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.Y = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    var redoMaxY = originalPositions.Keys.Max(e => e.Y + e.Height);
                    foreach (var element in originalPositions.Keys)
                    {
                        element.Y = redoMaxY - element.Height;
                    }
                },
                description: $"Align Bottom {SelectedElements.Count} element(s)"
            ));
        }

        public void DistributeHorizontally()
        {
            if (SelectedElements.Count < 3) return;

            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.X);
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

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.X = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    var redoSorted = originalPositions.Keys.OrderBy(e => originalPositions[e]).ToList();
                    var redoLeftMost = redoSorted.First().X;
                    var redoRightMost = redoSorted.Last().X + redoSorted.Last().Width;
                    var redoTotalWidth = redoSorted.Sum(e => e.Width);
                    var redoSpacing = (redoRightMost - redoLeftMost - redoTotalWidth) / (redoSorted.Count - 1);

                    double redoCurrentX = redoLeftMost;
                    foreach (var element in redoSorted)
                    {
                        element.X = redoCurrentX;
                        redoCurrentX += element.Width + redoSpacing;
                    }
                },
                description: $"Distribute Horizontally {SelectedElements.Count} element(s)"
            ));
        }

        public void DistributeVertically()
        {
            if (SelectedElements.Count < 3) return;

            var originalPositions = SelectedElements.ToDictionary(e => e, e => e.Y);
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

            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var kvp in originalPositions)
                    {
                        kvp.Key.Y = kvp.Value;
                    }
                },
                redoAction: () =>
                {
                    var redoSorted = originalPositions.Keys.OrderBy(e => originalPositions[e]).ToList();
                    var redoTopMost = redoSorted.First().Y;
                    var redoBottomMost = redoSorted.Last().Y + redoSorted.Last().Height;
                    var redoTotalHeight = redoSorted.Sum(e => e.Height);
                    var redoSpacing = (redoBottomMost - redoTopMost - redoTotalHeight) / (redoSorted.Count - 1);

                    double redoCurrentY = redoTopMost;
                    foreach (var element in redoSorted)
                    {
                        element.Y = redoCurrentY;
                        redoCurrentY += element.Height + redoSpacing;
                    }
                },
                description: $"Distribute Vertically {SelectedElements.Count} element(s)"
            ));
        }

        private void LockElements()
        {
            if (SelectedElements.Count == 0) return;

            foreach (var element in SelectedElements)
            {
                element.IsLocked = true;
            }
        }

        private void UnlockElements()
        {
            if (SelectedElements.Count == 0) return;

            foreach (var element in SelectedElements)
            {
                element.IsLocked = false;
            }
        }

        /// <summary>
        /// Handles changes to the SelectedElements collection
        /// Updates IsSelected property on elements
        /// </summary>
        private void OnSelectedElementsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Mark removed elements as not selected
            if (e.OldItems != null)
            {
                foreach (LayoutElementBase element in e.OldItems)
                {
                    element.IsSelected = false;
                }
            }

            // Mark added elements as selected
            if (e.NewItems != null)
            {
                foreach (LayoutElementBase element in e.NewItems)
                {
                    element.IsSelected = true;
                }
            }

            // Handle Reset action (e.g., when Clear() is called)
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                // Deselect all elements
                foreach (var element in Elements)
                {
                    element.IsSelected = false;
                }
            }
        }

        #endregion

        #endregion
    }
}
