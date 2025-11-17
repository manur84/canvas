using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;
using LayoutDesigner.Models;
using LayoutDesigner.Constants;

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
        private readonly IUndoRedoService? _undoRedoService;

        // Store original values for undo/redo
        private double _originalX;
        private double _originalY;
        private double _originalWidth;
        private double _originalHeight;
        private double _originalRotation;

        public SelectionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);
            _undoRedoService = ServiceContainer.GetService<IUndoRedoService>();
            _resizeHandles = new List<Thumb>();

            // Shadow border (for depth effect)
            _shadowBorder = new Rectangle
            {
                Stroke = new SolidColorBrush(Color.FromArgb(60, 0, 120, 215)),
                StrokeThickness = UIConstants.SelectionShadowThickness,
                Fill = Brushes.Transparent,
                Effect = new BlurEffect { Radius = UIConstants.ShadowBlurRadius },
                IsHitTestVisible = false  // Don't block mouse events for dragging
            };
            _visualChildren.Add(_shadowBorder);

            // Border
            _border = new Rectangle
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = UIConstants.SelectionBorderThickness,
                StrokeDashArray = new DoubleCollection { 5, 3 },
                Fill = Brushes.Transparent,
                IsHitTestVisible = false  // Don't block mouse events for dragging
            };
            _visualChildren.Add(_border);

            // Dimension label
            _dimensionLabel = new TextBlock
            {
                Background = new SolidColorBrush(Color.FromArgb(220, 0, 120, 215)),
                Foreground = Brushes.White,
                Padding = new Thickness(4, 2, 4, 2),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                IsHitTestVisible = false  // Don't block mouse events for dragging
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
            double handleSize = UIConstants.ResizeHandleSize;
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
                -UIConstants.RotateHandleDistance,
                handleSize,
                handleSize));

            return finalSize;
        }

        private Thumb CreateResizeHandle(HandlePosition position)
        {
            var thumb = new Thumb
            {
                Width = UIConstants.ResizeHandleSize,
                Height = UIConstants.ResizeHandleSize,
                Background = Brushes.White,
                BorderBrush = Brushes.DodgerBlue,
                BorderThickness = new Thickness(UIConstants.SelectionBorderThickness),
                Tag = position,
                Cursor = GetCursor(position),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = UIConstants.HandleShadowBlurRadius,
                    ShadowDepth = UIConstants.HandleShadowDepth,
                    Opacity = UIConstants.HandleShadowOpacity
                }
            };

            // Add hover effect using named methods to avoid memory leaks
            thumb.MouseEnter += OnResizeHandleMouseEnter;
            thumb.MouseLeave += OnResizeHandleMouseLeave;

            thumb.DragStarted += OnResizeHandleDragStarted;
            thumb.DragDelta += OnResizeHandleDragDelta;
            thumb.DragCompleted += OnResizeHandleDragCompleted;
            return thumb;
        }

        private Thumb CreateRotateHandle()
        {
            var thumb = new Thumb
            {
                Width = UIConstants.RotateHandleSize,
                Height = UIConstants.RotateHandleSize,
                Background = new SolidColorBrush(Color.FromRgb(144, 238, 144)), // LightGreen
                BorderBrush = new SolidColorBrush(Color.FromRgb(34, 139, 34)), // ForestGreen
                BorderThickness = new Thickness(UIConstants.SelectionBorderThickness),
                Cursor = Cursors.Hand,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = UIConstants.HandleShadowBlurRadius,
                    ShadowDepth = UIConstants.HandleShadowDepth,
                    Opacity = UIConstants.HandleShadowOpacity
                }
            };

            // Add hover effect using named methods to avoid memory leaks
            thumb.MouseEnter += OnRotateHandleMouseEnter;
            thumb.MouseLeave += OnRotateHandleMouseLeave;

            thumb.DragStarted += OnRotateHandleDragStarted;
            thumb.DragDelta += OnRotateHandleDragDelta;
            thumb.DragCompleted += OnRotateHandleDragCompleted;
            return thumb;
        }

        /// <summary>
        /// Named event handler for resize handle mouse enter to avoid memory leaks
        /// </summary>
        private void OnResizeHandleMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.Background = new SolidColorBrush(Color.FromRgb(135, 206, 250)); // LightSkyBlue
                thumb.Width = UIConstants.ResizeHandleHoverSize;
                thumb.Height = UIConstants.ResizeHandleHoverSize;
            }
        }

        /// <summary>
        /// Named event handler for resize handle mouse leave to avoid memory leaks
        /// </summary>
        private void OnResizeHandleMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.Background = Brushes.White;
                thumb.Width = UIConstants.ResizeHandleSize;
                thumb.Height = UIConstants.ResizeHandleSize;
            }
        }

        /// <summary>
        /// Named event handler for rotate handle mouse enter to avoid memory leaks
        /// </summary>
        private void OnRotateHandleMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.Background = new SolidColorBrush(Color.FromRgb(50, 205, 50)); // LimeGreen
                thumb.Width = UIConstants.RotateHandleHoverSize;
                thumb.Height = UIConstants.RotateHandleHoverSize;
            }
        }

        /// <summary>
        /// Named event handler for rotate handle mouse leave to avoid memory leaks
        /// </summary>
        private void OnRotateHandleMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.Background = new SolidColorBrush(Color.FromRgb(144, 238, 144)); // LightGreen
                thumb.Width = UIConstants.RotateHandleSize;
                thumb.Height = UIConstants.RotateHandleSize;
            }
        }

        /// <summary>
        /// Detaches all event handlers to prevent memory leaks
        /// Call this when the adorner is being removed
        /// </summary>
        public void Detach()
        {
            if (_resizeHandles != null)
            {
                foreach (var handle in _resizeHandles)
                {
                    if (handle != null)
                    {
                        handle.MouseEnter -= OnResizeHandleMouseEnter;
                        handle.MouseLeave -= OnResizeHandleMouseLeave;
                        handle.DragStarted -= OnResizeHandleDragStarted;
                        handle.DragDelta -= OnResizeHandleDragDelta;
                        handle.DragCompleted -= OnResizeHandleDragCompleted;
                    }
                }
            }

            if (_rotateHandle != null)
            {
                _rotateHandle.MouseEnter -= OnRotateHandleMouseEnter;
                _rotateHandle.MouseLeave -= OnRotateHandleMouseLeave;
                _rotateHandle.DragStarted -= OnRotateHandleDragStarted;
                _rotateHandle.DragDelta -= OnRotateHandleDragDelta;
                _rotateHandle.DragCompleted -= OnRotateHandleDragCompleted;
            }
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

        private void OnResizeHandleDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            // Store original values for undo/redo
            if (AdornedElement is FrameworkElement element && element.DataContext is LayoutElementBase layoutElement)
            {
                _originalX = layoutElement.X;
                _originalY = layoutElement.Y;
                _originalWidth = layoutElement.Width;
                _originalHeight = layoutElement.Height;
            }
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
                        layoutElement.Width = Math.Max(UIConstants.MinElementSize, layoutElement.Width - deltaX);
                        layoutElement.Height = Math.Max(UIConstants.MinElementSize, layoutElement.Height - deltaY);
                        break;

                    case HandlePosition.TopRight:
                        layoutElement.Y += deltaY;
                        layoutElement.Width = Math.Max(UIConstants.MinElementSize, layoutElement.Width + deltaX);
                        layoutElement.Height = Math.Max(UIConstants.MinElementSize, layoutElement.Height - deltaY);
                        break;

                    case HandlePosition.BottomLeft:
                        layoutElement.X += deltaX;
                        layoutElement.Width = Math.Max(UIConstants.MinElementSize, layoutElement.Width - deltaX);
                        layoutElement.Height = Math.Max(UIConstants.MinElementSize, layoutElement.Height + deltaY);
                        break;

                    case HandlePosition.BottomRight:
                        layoutElement.Width = Math.Max(UIConstants.MinElementSize, layoutElement.Width + deltaX);
                        layoutElement.Height = Math.Max(UIConstants.MinElementSize, layoutElement.Height + deltaY);
                        break;

                    case HandlePosition.TopCenter:
                        layoutElement.Y += deltaY;
                        layoutElement.Height = Math.Max(UIConstants.MinElementSize, layoutElement.Height - deltaY);
                        break;

                    case HandlePosition.BottomCenter:
                        layoutElement.Height = Math.Max(UIConstants.MinElementSize, layoutElement.Height + deltaY);
                        break;

                    case HandlePosition.MiddleLeft:
                        layoutElement.X += deltaX;
                        layoutElement.Width = Math.Max(UIConstants.MinElementSize, layoutElement.Width - deltaX);
                        break;

                    case HandlePosition.MiddleRight:
                        layoutElement.Width = Math.Max(UIConstants.MinElementSize, layoutElement.Width + deltaX);
                        break;
                }
            }
        }

        private void OnResizeHandleDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (_undoRedoService == null) return;
            if (AdornedElement is not FrameworkElement element || element.DataContext is not LayoutElementBase layoutElement)
                return;

            // Only add undo action if something actually changed
            if (layoutElement.X != _originalX || layoutElement.Y != _originalY ||
                layoutElement.Width != _originalWidth || layoutElement.Height != _originalHeight)
            {
                var newX = layoutElement.X;
                var newY = layoutElement.Y;
                var newWidth = layoutElement.Width;
                var newHeight = layoutElement.Height;

                _undoRedoService.AddAction(new UndoRedoAction(
                    undoAction: () =>
                    {
                        layoutElement.X = _originalX;
                        layoutElement.Y = _originalY;
                        layoutElement.Width = _originalWidth;
                        layoutElement.Height = _originalHeight;
                    },
                    redoAction: () =>
                    {
                        layoutElement.X = newX;
                        layoutElement.Y = newY;
                        layoutElement.Width = newWidth;
                        layoutElement.Height = newHeight;
                    },
                    description: $"Resize {layoutElement.Name}"
                ));
            }
        }

        private void OnRotateHandleDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            // Store original rotation for undo/redo
            if (AdornedElement is FrameworkElement element && element.DataContext is LayoutElementBase layoutElement)
            {
                _originalRotation = layoutElement.Rotation;
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

        private void OnRotateHandleDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (_undoRedoService == null) return;
            if (AdornedElement is not FrameworkElement element || element.DataContext is not LayoutElementBase layoutElement)
                return;

            // Only add undo action if rotation actually changed
            if (layoutElement.Rotation != _originalRotation)
            {
                var newRotation = layoutElement.Rotation;

                _undoRedoService.AddAction(new UndoRedoAction(
                    undoAction: () =>
                    {
                        layoutElement.Rotation = _originalRotation;
                    },
                    redoAction: () =>
                    {
                        layoutElement.Rotation = newRotation;
                    },
                    description: $"Rotate {layoutElement.Name}"
                ));
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
