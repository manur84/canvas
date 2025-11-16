using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// Adorner that provides resize and rotate handles for selected elements
    /// </summary>
    public class SelectionAdorner : Adorner
    {
        private readonly VisualCollection _visualChildren;
        private readonly Rectangle _border;
        private readonly Rectangle _shadowBorder;
        private readonly List<Thumb> _resizeHandles;
        private readonly Thumb _rotateHandle;
        private readonly TextBlock _dimensionLabel;

        public SelectionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);
            _resizeHandles = new List<Thumb>();

            // Shadow border (for depth effect)
            _shadowBorder = new Rectangle
            {
                Stroke = new SolidColorBrush(Color.FromArgb(60, 0, 120, 215)),
                StrokeThickness = 4,
                Fill = Brushes.Transparent,
                Effect = new BlurEffect { Radius = 3 }
            };
            _visualChildren.Add(_shadowBorder);

            // Border
            _border = new Rectangle
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 5, 3 },
                Fill = Brushes.Transparent
            };
            _visualChildren.Add(_border);

            // Dimension label
            _dimensionLabel = new TextBlock
            {
                Background = new SolidColorBrush(Color.FromArgb(220, 0, 120, 215)),
                Foreground = Brushes.White,
                Padding = new Thickness(4, 2, 4, 2),
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };
            _visualChildren.Add(_dimensionLabel);

            // Resize handles (8 positions)
            var positions = new[]
            {
                HandlePosition.TopLeft,
                HandlePosition.TopCenter,
                HandlePosition.TopRight,
                HandlePosition.MiddleLeft,
                HandlePosition.MiddleRight,
                HandlePosition.BottomLeft,
                HandlePosition.BottomCenter,
                HandlePosition.BottomRight
            };

            foreach (var position in positions)
            {
                var handle = CreateResizeHandle(position);
                _resizeHandles.Add(handle);
                _visualChildren.Add(handle);
            }

            // Rotate handle
            _rotateHandle = CreateRotateHandle();
            _visualChildren.Add(_rotateHandle);
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        protected override Visual GetVisualChild(int index) => _visualChildren[index];

        protected override Size ArrangeOverride(Size finalSize)
        {
            var element = AdornedElement as FrameworkElement;
            if (element == null) return finalSize;

            var rect = new Rect(0, 0, element.ActualWidth, element.ActualHeight);

            // Arrange shadow border
            _shadowBorder.Arrange(rect);

            // Arrange border
            _border.Arrange(rect);

            // Update and arrange dimension label
            if (element.DataContext is LayoutElementBase layoutElement)
            {
                _dimensionLabel.Text = $"{layoutElement.Width:F0} × {layoutElement.Height:F0}";
                _dimensionLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                var labelRect = new Rect(
                    rect.Right + 5,
                    rect.Bottom + 5,
                    _dimensionLabel.DesiredSize.Width,
                    _dimensionLabel.DesiredSize.Height);
                _dimensionLabel.Arrange(labelRect);
            }

            // Arrange resize handles
            double handleSize = 10; // Increased from 8 for better visibility
            double halfSize = handleSize / 2;

            for (int i = 0; i < _resizeHandles.Count; i++)
            {
                var handle = _resizeHandles[i];
                var position = (HandlePosition)handle.Tag;

                Point point = position switch
                {
                    HandlePosition.TopLeft => new Point(-halfSize, -halfSize),
                    HandlePosition.TopCenter => new Point(rect.Width / 2 - halfSize, -halfSize),
                    HandlePosition.TopRight => new Point(rect.Width - halfSize, -halfSize),
                    HandlePosition.MiddleLeft => new Point(-halfSize, rect.Height / 2 - halfSize),
                    HandlePosition.MiddleRight => new Point(rect.Width - halfSize, rect.Height / 2 - halfSize),
                    HandlePosition.BottomLeft => new Point(-halfSize, rect.Height - halfSize),
                    HandlePosition.BottomCenter => new Point(rect.Width / 2 - halfSize, rect.Height - halfSize),
                    HandlePosition.BottomRight => new Point(rect.Width - halfSize, rect.Height - halfSize),
                    _ => new Point()
                };

                handle.Arrange(new Rect(point, new Size(handleSize, handleSize)));
            }

            // Arrange rotate handle (above the element)
            _rotateHandle.Arrange(new Rect(
                rect.Width / 2 - halfSize,
                -30,
                handleSize,
                handleSize));

            return finalSize;
        }

        private Thumb CreateResizeHandle(HandlePosition position)
        {
            var thumb = new Thumb
            {
                Width = 10,
                Height = 10,
                Background = Brushes.White,
                BorderBrush = Brushes.DodgerBlue,
                BorderThickness = new Thickness(2),
                Tag = position,
                Cursor = GetCursor(position),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    ShadowDepth = 2,
                    Opacity = 0.4
                }
            };

            // Add hover effect
            thumb.MouseEnter += (s, e) =>
            {
                thumb.Background = new SolidColorBrush(Color.FromRgb(135, 206, 250)); // LightSkyBlue
                thumb.Width = 12;
                thumb.Height = 12;
            };
            thumb.MouseLeave += (s, e) =>
            {
                thumb.Background = Brushes.White;
                thumb.Width = 10;
                thumb.Height = 10;
            };

            thumb.DragDelta += OnResizeHandleDragDelta;
            return thumb;
        }

        private Thumb CreateRotateHandle()
        {
            var thumb = new Thumb
            {
                Width = 12,
                Height = 12,
                Background = new SolidColorBrush(Color.FromRgb(144, 238, 144)), // LightGreen
                BorderBrush = new SolidColorBrush(Color.FromRgb(34, 139, 34)), // ForestGreen
                BorderThickness = new Thickness(2),
                Cursor = Cursors.Hand,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    ShadowDepth = 2,
                    Opacity = 0.4
                }
            };

            // Add hover effect
            thumb.MouseEnter += (s, e) =>
            {
                thumb.Background = new SolidColorBrush(Color.FromRgb(50, 205, 50)); // LimeGreen
                thumb.Width = 14;
                thumb.Height = 14;
            };
            thumb.MouseLeave += (s, e) =>
            {
                thumb.Background = new SolidColorBrush(Color.FromRgb(144, 238, 144)); // LightGreen
                thumb.Width = 12;
                thumb.Height = 12;
            };

            thumb.DragDelta += OnRotateHandleDragDelta;
            return thumb;
        }

        private Cursor GetCursor(HandlePosition position)
        {
            return position switch
            {
                HandlePosition.TopLeft or HandlePosition.BottomRight => Cursors.SizeNWSE,
                HandlePosition.TopRight or HandlePosition.BottomLeft => Cursors.SizeNESW,
                HandlePosition.TopCenter or HandlePosition.BottomCenter => Cursors.SizeNS,
                HandlePosition.MiddleLeft or HandlePosition.MiddleRight => Cursors.SizeWE,
                _ => Cursors.Arrow
            };
        }

        private void OnResizeHandleDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (AdornedElement is FrameworkElement element && element.DataContext is LayoutElementBase layoutElement)
            {
                var handle = (Thumb)sender;
                var position = (HandlePosition)handle.Tag;

                double deltaX = e.HorizontalChange;
                double deltaY = e.VerticalChange;

                switch (position)
                {
                    case HandlePosition.TopLeft:
                        layoutElement.X += deltaX;
                        layoutElement.Y += deltaY;
                        layoutElement.Width = Math.Max(10, layoutElement.Width - deltaX);
                        layoutElement.Height = Math.Max(10, layoutElement.Height - deltaY);
                        break;

                    case HandlePosition.TopRight:
                        layoutElement.Y += deltaY;
                        layoutElement.Width = Math.Max(10, layoutElement.Width + deltaX);
                        layoutElement.Height = Math.Max(10, layoutElement.Height - deltaY);
                        break;

                    case HandlePosition.BottomLeft:
                        layoutElement.X += deltaX;
                        layoutElement.Width = Math.Max(10, layoutElement.Width - deltaX);
                        layoutElement.Height = Math.Max(10, layoutElement.Height + deltaY);
                        break;

                    case HandlePosition.BottomRight:
                        layoutElement.Width = Math.Max(10, layoutElement.Width + deltaX);
                        layoutElement.Height = Math.Max(10, layoutElement.Height + deltaY);
                        break;

                    case HandlePosition.TopCenter:
                        layoutElement.Y += deltaY;
                        layoutElement.Height = Math.Max(10, layoutElement.Height - deltaY);
                        break;

                    case HandlePosition.BottomCenter:
                        layoutElement.Height = Math.Max(10, layoutElement.Height + deltaY);
                        break;

                    case HandlePosition.MiddleLeft:
                        layoutElement.X += deltaX;
                        layoutElement.Width = Math.Max(10, layoutElement.Width - deltaX);
                        break;

                    case HandlePosition.MiddleRight:
                        layoutElement.Width = Math.Max(10, layoutElement.Width + deltaX);
                        break;
                }
            }
        }

        private void OnRotateHandleDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (AdornedElement is FrameworkElement element && element.DataContext is LayoutElementBase layoutElement)
            {
                // Simple rotation based on horizontal drag
                layoutElement.Rotation += e.HorizontalChange;

                // Normalize angle to 0-360
                layoutElement.Rotation = layoutElement.Rotation % 360;
                if (layoutElement.Rotation < 0) layoutElement.Rotation += 360;
            }
        }

        private enum HandlePosition
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        private class Thumb : System.Windows.Controls.Primitives.Thumb
        {
            static Thumb()
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(Thumb),
                    new FrameworkPropertyMetadata(typeof(Thumb)));
            }
        }
    }
}
