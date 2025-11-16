using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// Adorner that displays snap alignment guides during element dragging
    /// Best Practice: Visual feedback improves user experience
    /// </summary>
    public class SnapLinesAdorner : Adorner
    {
        private readonly List<Line> _snapLines = new();
        private Pen? _snapLinePen;

        /// <summary>
        /// Represents a snap line
        /// </summary>
        public class Line
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            public LineType Type { get; set; }
        }

        /// <summary>
        /// Type of snap line
        /// </summary>
        public enum LineType
        {
            Horizontal,
            Vertical
        }

        public SnapLinesAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false; // Don't interfere with mouse events

            // Create pen for snap lines
            var brush = new SolidColorBrush(Color.FromRgb(255, 0, 128)); // Magenta
            brush.Freeze();
            _snapLinePen = new Pen(brush, 1.5)
            {
                DashStyle = DashStyles.Dash
            };
            _snapLinePen.Freeze();
        }

        /// <summary>
        /// Shows a horizontal snap line
        /// </summary>
        public void ShowHorizontalLine(double y, double startX, double endX)
        {
            _snapLines.Add(new Line
            {
                Start = new Point(startX, y),
                End = new Point(endX, y),
                Type = LineType.Horizontal
            });
            InvalidateVisual();
        }

        /// <summary>
        /// Shows a vertical snap line
        /// </summary>
        public void ShowVerticalLine(double x, double startY, double endY)
        {
            _snapLines.Add(new Line
            {
                Start = new Point(x, startY),
                End = new Point(x, endY),
                Type = LineType.Vertical
            });
            InvalidateVisual();
        }

        /// <summary>
        /// Shows snap lines at element edges
        /// </summary>
        public void ShowSnapLines(double x, double y, double width, double height,
            bool snapLeft = false, bool snapRight = false, bool snapTop = false, bool snapBottom = false,
            bool snapCenterH = false, bool snapCenterV = false)
        {
            var canvasWidth = ((FrameworkElement)AdornedElement).ActualWidth;
            var canvasHeight = ((FrameworkElement)AdornedElement).ActualHeight;

            // Horizontal lines (top, bottom, center)
            if (snapTop)
                ShowHorizontalLine(y, 0, canvasWidth);

            if (snapBottom)
                ShowHorizontalLine(y + height, 0, canvasWidth);

            if (snapCenterV)
            {
                var centerY = y + height / 2;
                ShowHorizontalLine(centerY, 0, canvasWidth);
            }

            // Vertical lines (left, right, center)
            if (snapLeft)
                ShowVerticalLine(x, 0, canvasHeight);

            if (snapRight)
                ShowVerticalLine(x + width, 0, canvasHeight);

            if (snapCenterH)
            {
                var centerX = x + width / 2;
                ShowVerticalLine(centerX, 0, canvasHeight);
            }
        }

        /// <summary>
        /// Clears all snap lines
        /// </summary>
        public void Clear()
        {
            if (_snapLines.Count > 0)
            {
                _snapLines.Clear();
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_snapLinePen == null || _snapLines.Count == 0)
                return;

            // Draw all snap lines
            foreach (var line in _snapLines)
            {
                drawingContext.DrawLine(_snapLinePen, line.Start, line.End);
            }
        }
    }
}
