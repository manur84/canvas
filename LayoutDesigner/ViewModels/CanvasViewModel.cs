using System;
using System.Collections.Generic;
using System.Linq;
using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;
using LayoutDesigner.ViewModels.Base;
using LayoutDesigner.Constants;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

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
            AddHorizontalLineCommand = new RelayCommand(AddHorizontalLine);
            AddVerticalLineCommand = new RelayCommand(AddVerticalLine);
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
            ExportToSvgCommand = new RelayCommand(async () => await ExportToSvg());

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

        // Safe properties for selected element position and size (avoid index out of range)
        public double? SelectedElementX => SelectedElements.Count > 0 ? SelectedElements[0].X : null;
        public double? SelectedElementY => SelectedElements.Count > 0 ? SelectedElements[0].Y : null;
        public double? SelectedElementWidth => SelectedElements.Count > 0 ? SelectedElements[0].Width : null;
        public double? SelectedElementHeight => SelectedElements.Count > 0 ? SelectedElements[0].Height : null;

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
        public ICommand AddHorizontalLineCommand { get; }
        public ICommand AddVerticalLineCommand { get; }
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
        public ICommand ExportToSvgCommand { get; }

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

        private void AddHorizontalLine()
        {
            var element = new LineElement
            {
                Name = $"Horizontal Line {Elements.Count + 1}",
                X = 50,
                Y = 100,
                Width = 200,
                Height = 2,
                X2 = 200,  // Horizontal: X2 is different, Y2 is same
                Y2 = 0
            };

            AddElement(element, "Add Horizontal Line");
        }

        private void AddVerticalLine()
        {
            var element = new LineElement
            {
                Name = $"Vertical Line {Elements.Count + 1}",
                X = 100,
                Y = 50,
                Width = 2,
                Height = 200,
                X2 = 0,  // Vertical: X2 is same, Y2 is different
                Y2 = 200
            };

            AddElement(element, "Add Vertical Line");
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

            // Notify UI that Elements collection has changed
            OnPropertyChanged(nameof(Elements));
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

            // Cache selected elements to avoid multiple enumerations
            var elements = SelectedElements.ToList();

            // Save original positions and find min in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(elements.Count);
            var minX = double.MaxValue;

            foreach (var element in elements)
            {
                originalPositions[element] = element.X;
                if (element.X < minX)
                    minX = element.X;
            }

            // Apply alignment
            foreach (var element in elements)
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
                description: $"Align Left {elements.Count} element(s)"
            ));
        }

        public void AlignCenter()
        {
            if (SelectedElements.Count < 2) return;

            // Cache selected elements to avoid multiple enumerations
            var elements = SelectedElements.ToList();

            // Save original positions and calculate center in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(elements.Count);
            var sumCenterX = 0.0;

            foreach (var element in elements)
            {
                originalPositions[element] = element.X;
                sumCenterX += element.X + element.Width / 2;
            }

            var centerX = sumCenterX / elements.Count;

            // Apply alignment
            foreach (var element in elements)
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
                description: $"Align Center {elements.Count} element(s)"
            ));
        }

        public void AlignRight()
        {
            if (SelectedElements.Count < 2) return;

            // Cache selected elements to avoid multiple enumerations
            var elements = SelectedElements.ToList();

            // Save original positions and find max right edge in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(elements.Count);
            var maxX = double.MinValue;

            foreach (var element in elements)
            {
                originalPositions[element] = element.X;
                var rightEdge = element.X + element.Width;
                if (rightEdge > maxX)
                    maxX = rightEdge;
            }

            // Apply alignment
            foreach (var element in elements)
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
                description: $"Align Right {elements.Count} element(s)"
            ));
        }

        public void AlignTop()
        {
            if (SelectedElements.Count < 2) return;

            // Cache selected elements to avoid multiple enumerations
            var elements = SelectedElements.ToList();

            // Save original positions and find min in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(elements.Count);
            var minY = double.MaxValue;

            foreach (var element in elements)
            {
                originalPositions[element] = element.Y;
                if (element.Y < minY)
                    minY = element.Y;
            }

            // Apply alignment
            foreach (var element in elements)
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
                description: $"Align Top {elements.Count} element(s)"
            ));
        }

        public void AlignMiddle()
        {
            if (SelectedElements.Count < 2) return;

            // Cache selected elements to avoid multiple enumerations
            var elements = SelectedElements.ToList();

            // Save original positions and calculate center in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(elements.Count);
            var sumCenterY = 0.0;

            foreach (var element in elements)
            {
                originalPositions[element] = element.Y;
                sumCenterY += element.Y + element.Height / 2;
            }

            var centerY = sumCenterY / elements.Count;

            // Apply alignment
            foreach (var element in elements)
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
                    foreach (var element in originalPositions.Keys)
                    {
                        element.Y = centerY - element.Height / 2;
                    }
                },
                description: $"Align Middle {elements.Count} element(s)"
            ));
        }

        public void AlignBottom()
        {
            if (SelectedElements.Count < 2) return;

            // Cache selected elements to avoid multiple enumerations
            var elements = SelectedElements.ToList();

            // Save original positions and find max bottom edge in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(elements.Count);
            var maxY = double.MinValue;

            foreach (var element in elements)
            {
                originalPositions[element] = element.Y;
                var bottomEdge = element.Y + element.Height;
                if (bottomEdge > maxY)
                    maxY = bottomEdge;
            }

            // Apply alignment
            foreach (var element in elements)
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
                    foreach (var element in originalPositions.Keys)
                    {
                        element.Y = maxY - element.Height;
                    }
                },
                description: $"Align Bottom {elements.Count} element(s)"
            ));
        }

        public void DistributeHorizontally()
        {
            if (SelectedElements.Count < 3) return;

            // Cache and sort selected elements by X position
            var sorted = SelectedElements.OrderBy(e => e.X).ToList();

            // Save original positions and calculate distribution in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(sorted.Count);
            var totalWidth = 0.0;

            foreach (var element in sorted)
            {
                originalPositions[element] = element.X;
                totalWidth += element.Width;
            }

            var leftMost = sorted[0].X;
            var rightMost = sorted[^1].X + sorted[^1].Width;
            var spacing = (rightMost - leftMost - totalWidth) / (sorted.Count - 1);

            // Store new positions for redo
            var newPositions = new Dictionary<LayoutElementBase, double>(sorted.Count);
            var currentX = leftMost;

            foreach (var element in sorted)
            {
                newPositions[element] = currentX;
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
                    foreach (var kvp in newPositions)
                    {
                        kvp.Key.X = kvp.Value;
                    }
                },
                description: $"Distribute Horizontally {sorted.Count} element(s)"
            ));
        }

        public void DistributeVertically()
        {
            if (SelectedElements.Count < 3) return;

            // Cache and sort selected elements by Y position
            var sorted = SelectedElements.OrderBy(e => e.Y).ToList();

            // Save original positions and calculate distribution in single pass
            var originalPositions = new Dictionary<LayoutElementBase, double>(sorted.Count);
            var totalHeight = 0.0;

            foreach (var element in sorted)
            {
                originalPositions[element] = element.Y;
                totalHeight += element.Height;
            }

            var topMost = sorted[0].Y;
            var bottomMost = sorted[^1].Y + sorted[^1].Height;
            var spacing = (bottomMost - topMost - totalHeight) / (sorted.Count - 1);

            // Store new positions for redo
            var newPositions = new Dictionary<LayoutElementBase, double>(sorted.Count);
            var currentY = topMost;

            foreach (var element in sorted)
            {
                newPositions[element] = currentY;
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
                    foreach (var kvp in newPositions)
                    {
                        kvp.Key.Y = kvp.Value;
                    }
                },
                description: $"Distribute Vertically {sorted.Count} element(s)"
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

        private async System.Threading.Tasks.Task ExportToSvg()
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Export Layout to SVG",
                Filter = "SVG Files (*.svg)|*.svg|All Files (*.*)|*.*",
                DefaultExt = "svg",
                FileName = "layout.svg"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var svgService = App.Services.Resolve<ISvgExportService>();
                var success = await svgService.ExportToSvgAsync(Document, saveDialog.FileName);

                if (success)
                {
                    MessageBox.Show("SVG exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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

            // Notify changes to selected element properties for status bar
            OnPropertyChanged(nameof(SelectedElementX));
            OnPropertyChanged(nameof(SelectedElementY));
            OnPropertyChanged(nameof(SelectedElementWidth));
            OnPropertyChanged(nameof(SelectedElementHeight));
        }

        #endregion

        #endregion
    }
}
