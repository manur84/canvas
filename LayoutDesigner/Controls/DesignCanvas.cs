using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LayoutDesigner.Helpers;
using LayoutDesigner.Models.Base;

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
        private readonly List<UIElement> _selectedElements = new();
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
                _dragStartPoint = e.GetPosition(this);
                _isDragging = false;
                element.CaptureMouse();
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

            // Start dragging if moved more than 3 pixels (dead zone to avoid accidental drags)
            if (!_isDragging)
            {
                if (Math.Abs(delta.X) > 3 || Math.Abs(delta.Y) > 3)
                {
                    _isDragging = true;
                }
                else
                {
                    return; // Still in dead zone
                }
            }

            // Performance: Only process if we have valid element
            if (e.Source is not FrameworkElement element || element.DataContext is not LayoutElementBase layoutElement)
                return;

            // Calculate new position
            var newX = layoutElement.X + delta.X;
            var newY = layoutElement.Y + delta.Y;

            // Snap to grid if enabled (cached property access)
            if (SnapToGrid)
            {
                newX = SnapHelper.SnapToGrid(newX, GridSize);
                newY = SnapHelper.SnapToGrid(newY, GridSize);
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
                if (e.Source is FrameworkElement element)
                {
                    element.ReleaseMouseCapture();
                }

                // Clear snap lines when drag ends
                _snapLinesAdorner?.Clear();

                _dragStartPoint = null;
                _isDragging = false;
                e.Handled = true;
            }
        }

        #endregion

        #region Public Methods

        public void ClearSelection()
        {
            _selectedElements.Clear();
        }

        public void SelectElement(UIElement element)
        {
            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);
            }
        }

        public void DeselectElement(UIElement element)
        {
            _selectedElements.Remove(element);
        }

        #endregion
    }
}
