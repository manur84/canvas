using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LayoutDesigner.Helpers;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Constants;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// Custom Canvas control for the layout designer with selection, drag, and resize capabilities
    /// Optimized for performance following WPF best practices
    /// </summary>
    public class DesignCanvas : Canvas
    {
        private Point? _dragStartPoint;
        private bool _isDragging;
        private bool _isRectangleSelecting;
        private Pen? _gridPen; // Cached pen for grid rendering

        // Adorners for visual feedback
        private SnapLinesAdorner? _snapLinesAdorner;
        private SelectionRectangleAdorner? _selectionAdorner;
        private AdornerLayer? _adornerLayer;

        static DesignCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignCanvas),
                new FrameworkPropertyMetadata(typeof(DesignCanvas)));
        }

        public DesignCanvas()
        {
            Background = Brushes.White;
            ClipToBounds = true;

            // Performance optimizations - WPF Best Practices
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased); // Faster rendering for grid lines
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality); // Better image quality

            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;

            // Initialize adorners when loaded
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Get or create adorner layer
            _adornerLayer = AdornerLayer.GetAdornerLayer(this);

            if (_adornerLayer != null)
            {
                // Create and add snap lines adorner
                _snapLinesAdorner = new SnapLinesAdorner(this);
                _adornerLayer.Add(_snapLinesAdorner);

                // Create and add selection rectangle adorner
                _selectionAdorner = new SelectionRectangleAdorner(this);
                _adornerLayer.Add(_selectionAdorner);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Clean up adorners to prevent resource leaks
            if (_adornerLayer != null)
            {
                if (_snapLinesAdorner != null)
                {
                    _adornerLayer.Remove(_snapLinesAdorner);
                    _snapLinesAdorner = null;
                }

                if (_selectionAdorner != null)
                {
                    _adornerLayer.Remove(_selectionAdorner);
                    _selectionAdorner = null;
                }

                _adornerLayer = null;
            }
        }

        #region Dependency Properties

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(DesignCanvas),
                new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowGridProperty =
            DependencyProperty.Register(nameof(ShowGrid), typeof(bool), typeof(DesignCanvas),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SnapToGridProperty =
            DependencyProperty.Register(nameof(SnapToGrid), typeof(bool), typeof(DesignCanvas),
                new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty SnapToElementsProperty =
            DependencyProperty.Register(nameof(SnapToElements), typeof(bool), typeof(DesignCanvas),
                new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ElementsProperty =
            DependencyProperty.Register(nameof(Elements), typeof(System.Collections.IEnumerable), typeof(DesignCanvas),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty CanvasViewModelProperty =
            DependencyProperty.Register(nameof(CanvasViewModel), typeof(object), typeof(DesignCanvas),
                new FrameworkPropertyMetadata(null));

        public double GridSize
        {
            get => (double)GetValue(GridSizeProperty);
            set => SetValue(GridSizeProperty, value);
        }

        public bool ShowGrid
        {
            get => (bool)GetValue(ShowGridProperty);
            set => SetValue(ShowGridProperty, value);
        }

        public bool SnapToGrid
        {
            get => (bool)GetValue(SnapToGridProperty);
            set => SetValue(SnapToGridProperty, value);
        }

        public bool SnapToElements
        {
            get => (bool)GetValue(SnapToElementsProperty);
            set => SetValue(SnapToElementsProperty, value);
        }

        public System.Collections.IEnumerable? Elements
        {
            get => (System.Collections.IEnumerable?)GetValue(ElementsProperty);
            set => SetValue(ElementsProperty, value);
        }

        public object? CanvasViewModel
        {
            get => GetValue(CanvasViewModelProperty);
            set => SetValue(CanvasViewModelProperty, value);
        }

        #endregion

        #region Rendering

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (ShowGrid && GridSize > 0)
            {
                DrawGrid(dc);
            }
        }

        private void DrawGrid(DrawingContext dc)
        {
            // Cache the pen for better performance (Best Practice: Reuse frozen objects)
            if (_gridPen == null)
            {
                var brush = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128));
                brush.Freeze(); // Best Practice: Freeze unchanging brushes
                _gridPen = new Pen(brush, 1);
                _gridPen.Freeze(); // Best Practice: Freeze unchanging pens
            }

            double width = ActualWidth;
            double height = ActualHeight;

            // Best Practice: Use GuidelineSet for pixel-perfect rendering
            var guidelines = new GuidelineSet();

            // Vertical lines
            for (double x = 0; x < width; x += GridSize)
            {
                guidelines.GuidelinesX.Add(x);
                dc.DrawLine(_gridPen, new Point(x, 0), new Point(x, height));
            }

            // Horizontal lines
            for (double y = 0; y < height; y += GridSize)
            {
                guidelines.GuidelinesY.Add(y);
                dc.DrawLine(_gridPen, new Point(0, y), new Point(width, y));
            }

            dc.PushGuidelineSet(guidelines);
            dc.Pop();
        }

        #endregion

        #region Mouse Handling

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement element && element.DataContext is LayoutElementBase)
            {
                // Dragging an element
                _dragStartPoint = e.GetPosition(this);
                _isDragging = false;
                _isRectangleSelecting = false;
                element.CaptureMouse();
                e.Handled = true;
            }
            else if (e.Source == this)
            {
                // Click on empty canvas - start rectangle selection
                _dragStartPoint = e.GetPosition(this);
                _isDragging = false;
                _isRectangleSelecting = false;
                CaptureMouse();
                e.Handled = true;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Performance optimization: Early return if not in drag mode
            if (!_dragStartPoint.HasValue || e.LeftButton != MouseButtonState.Pressed)
                return;

            var currentPoint = e.GetPosition(this);
            var delta = currentPoint - _dragStartPoint.Value;

            // Start dragging/selecting if moved more than dead zone threshold
            if (!_isDragging && !_isRectangleSelecting)
            {
                if (Math.Abs(delta.X) > UIConstants.DragDeadZonePixels || Math.Abs(delta.Y) > UIConstants.DragDeadZonePixels)
                {
                    // Check if we're dragging an element or doing rectangle selection
                    if (e.Source is FrameworkElement elem && elem.DataContext is LayoutElementBase)
                    {
                        _isDragging = true;
                    }
                    else
                    {
                        _isRectangleSelecting = true;
                        _selectionAdorner?.StartSelection(_dragStartPoint.Value);
                    }
                }
                else
                {
                    return; // Still in dead zone
                }
            }

            // Handle rectangle selection
            if (_isRectangleSelecting)
            {
                _selectionAdorner?.UpdateSelection(currentPoint);
                e.Handled = true;
                return;
            }

            // Performance: Only process if we have valid element for dragging
            if (e.Source is not FrameworkElement element || element.DataContext is not LayoutElementBase layoutElement)
                return;

            // Calculate new position
            var newX = layoutElement.X + delta.X;
            var newY = layoutElement.Y + delta.Y;

            // Clear previous snap lines
            _snapLinesAdorner?.Clear();

            // Snap to grid if enabled (cached property access)
            if (SnapToGrid)
            {
                newX = SnapHelper.SnapToGrid(newX, GridSize);
                newY = SnapHelper.SnapToGrid(newY, GridSize);
            }

            // Snap to elements if enabled
            if (SnapToElements && Elements != null)
            {
                // Temporarily update position for snap calculation
                var originalX = layoutElement.X;
                var originalY = layoutElement.Y;
                layoutElement.X = newX;
                layoutElement.Y = newY;

                var otherElements = Elements.OfType<LayoutElementBase>().Where(e => e != layoutElement);
                var (snapX, snapY, snapped) = SnapHelper.SnapToElements(layoutElement, otherElements);

                if (snapped)
                {
                    newX = snapX;
                    newY = snapY;

                    // Show snap lines
                    ShowSnapLines(layoutElement, otherElements);
                }

                // Restore original position
                layoutElement.X = originalX;
                layoutElement.Y = originalY;
            }

            // Update position with bounds check
            layoutElement.X = Math.Max(0, newX);
            layoutElement.Y = Math.Max(0, newY);

            _dragStartPoint = currentPoint;
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragStartPoint.HasValue)
            {
                // Handle rectangle selection end
                if (_isRectangleSelecting && _selectionAdorner != null)
                {
                    var selectionRect = _selectionAdorner.EndSelection();
                    SelectElementsInRectangle(selectionRect);
                    ReleaseMouseCapture();
                }
                else if (e.Source is FrameworkElement element)
                {
                    // Handle simple click (not a drag) - select the element
                    if (!_isDragging && element.DataContext is LayoutElementBase layoutElement)
                    {
                        SelectElement(layoutElement, Keyboard.Modifiers.HasFlag(ModifierKeys.Control));
                    }

                    element.ReleaseMouseCapture();
                }
                else if (e.Source == this)
                {
                    // Click on empty canvas - clear selection
                    if (!_isDragging && !_isRectangleSelecting)
                    {
                        ClearSelection();
                    }
                    ReleaseMouseCapture();
                }

                // Clear snap lines when drag ends
                _snapLinesAdorner?.Clear();

                _dragStartPoint = null;
                _isDragging = false;
                _isRectangleSelecting = false;
                e.Handled = true;
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Selects all elements within the given rectangle
        /// </summary>
        private void SelectElementsInRectangle(Rect selectionRect)
        {
            if (Elements == null || CanvasViewModel == null)
                return;

            // Use reflection to access SelectedElements
            var canvasVmType = CanvasViewModel.GetType();
            var selectedElementsProperty = canvasVmType.GetProperty("SelectedElements");
            if (selectedElementsProperty == null)
                return;

            var selectedElements = selectedElementsProperty.GetValue(CanvasViewModel) as System.Collections.IList;
            if (selectedElements == null)
                return;

            // Clear current selection (unless Ctrl is held for additive selection)
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                selectedElements.Clear();
            }

            // Select elements that intersect with selection rectangle
            foreach (var element in Elements.OfType<LayoutElementBase>())
            {
                var elementRect = new Rect(element.X, element.Y, element.Width, element.Height);

                if (selectionRect.IntersectsWith(elementRect))
                {
                    if (!selectedElements.Contains(element))
                    {
                        selectedElements.Add(element);
                    }
                }
            }
        }

        /// <summary>
        /// Shows snap lines for the currently dragged element
        /// </summary>
        private void ShowSnapLines(LayoutElementBase movingElement, IEnumerable<LayoutElementBase> otherElements)
        {
            if (_snapLinesAdorner == null)
                return;

            var snapDistance = SnapHelper.DefaultSnapDistance;
            var movingX = movingElement.X;
            var movingY = movingElement.Y;
            var movingWidth = movingElement.Width;
            var movingHeight = movingElement.Height;
            var movingRight = movingX + movingWidth;
            var movingBottom = movingY + movingHeight;
            var movingCenterX = movingX + movingWidth / 2;
            var movingCenterY = movingY + movingHeight / 2;

            foreach (var other in otherElements)
            {
                var otherX = other.X;
                var otherY = other.Y;
                var otherWidth = other.Width;
                var otherHeight = other.Height;
                var otherRight = otherX + otherWidth;
                var otherBottom = otherY + otherHeight;
                var otherCenterX = otherX + otherWidth / 2;
                var otherCenterY = otherY + otherHeight / 2;

                // Check for horizontal alignment (vertical lines)
                if (Math.Abs(movingX - otherX) < snapDistance) // Left to Left
                    _snapLinesAdorner.ShowVerticalLine(otherX, 0, ActualHeight);

                if (Math.Abs(movingRight - otherRight) < snapDistance) // Right to Right
                    _snapLinesAdorner.ShowVerticalLine(otherRight, 0, ActualHeight);

                if (Math.Abs(movingCenterX - otherCenterX) < snapDistance) // Center to Center
                    _snapLinesAdorner.ShowVerticalLine(otherCenterX, 0, ActualHeight);

                if (Math.Abs(movingX - otherRight) < snapDistance) // Left to Right
                    _snapLinesAdorner.ShowVerticalLine(otherRight, 0, ActualHeight);

                if (Math.Abs(movingRight - otherX) < snapDistance) // Right to Left
                    _snapLinesAdorner.ShowVerticalLine(otherX, 0, ActualHeight);

                // Check for vertical alignment (horizontal lines)
                if (Math.Abs(movingY - otherY) < snapDistance) // Top to Top
                    _snapLinesAdorner.ShowHorizontalLine(otherY, 0, ActualWidth);

                if (Math.Abs(movingBottom - otherBottom) < snapDistance) // Bottom to Bottom
                    _snapLinesAdorner.ShowHorizontalLine(otherBottom, 0, ActualWidth);

                if (Math.Abs(movingCenterY - otherCenterY) < snapDistance) // Center to Center
                    _snapLinesAdorner.ShowHorizontalLine(otherCenterY, 0, ActualWidth);

                if (Math.Abs(movingY - otherBottom) < snapDistance) // Top to Bottom
                    _snapLinesAdorner.ShowHorizontalLine(otherBottom, 0, ActualWidth);

                if (Math.Abs(movingBottom - otherY) < snapDistance) // Bottom to Top
                    _snapLinesAdorner.ShowHorizontalLine(otherY, 0, ActualWidth);
            }
        }

        /// <summary>
        /// Selects a single element (or toggles selection if Ctrl is held)
        /// </summary>
        private void SelectElement(LayoutElementBase element, bool isCtrlHeld)
        {
            if (CanvasViewModel == null)
                return;

            // Use reflection to access SelectedElements
            var canvasVmType = CanvasViewModel.GetType();
            var selectedElementsProperty = canvasVmType.GetProperty("SelectedElements");
            if (selectedElementsProperty == null)
                return;

            var selectedElements = selectedElementsProperty.GetValue(CanvasViewModel) as System.Collections.IList;
            if (selectedElements == null)
                return;

            if (isCtrlHeld)
            {
                // Toggle selection
                if (selectedElements.Contains(element))
                {
                    selectedElements.Remove(element);
                }
                else
                {
                    selectedElements.Add(element);
                }
            }
            else
            {
                // Single selection
                selectedElements.Clear();
                selectedElements.Add(element);
            }
        }

        /// <summary>
        /// Clears all element selection
        /// </summary>
        private void ClearSelection()
        {
            if (CanvasViewModel == null)
                return;

            // Use reflection to access SelectedElements
            var canvasVmType = CanvasViewModel.GetType();
            var selectedElementsProperty = canvasVmType.GetProperty("SelectedElements");
            if (selectedElementsProperty == null)
                return;

            var selectedElements = selectedElementsProperty.GetValue(CanvasViewModel) as System.Collections.IList;
            selectedElements?.Clear();
        }

        #endregion
    }
}
