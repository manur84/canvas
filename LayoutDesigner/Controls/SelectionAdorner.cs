using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        private readonly List<Thumb> _resizeHandles;
        private readonly Thumb _rotateHandle;

        public SelectionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);
            _resizeHandles = new List<Thumb>();

            // Border
            _border = new Rectangle
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 4, 2 },
                Fill = Brushes.Transparent
            };
            _visualChildren.Add(_border);

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

            // Arrange border
            _border.Arrange(rect);

            // Arrange resize handles
            double handleSize = 8;
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
                Width = 8,
                Height = 8,
                Background = Brushes.White,
                BorderBrush = Brushes.DodgerBlue,
                BorderThickness = new Thickness(2),
                Tag = position,
                Cursor = GetCursor(position)
            };

            thumb.DragDelta += OnResizeHandleDragDelta;
            return thumb;
        }

        private Thumb CreateRotateHandle()
        {
            var thumb = new Thumb
            {
                Width = 8,
                Height = 8,
                Background = Brushes.LightGreen,
                BorderBrush = Brushes.Green,
                BorderThickness = new Thickness(2),
                Cursor = Cursors.Hand
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
