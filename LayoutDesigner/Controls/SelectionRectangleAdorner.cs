using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// Adorner that displays a selection rectangle for multi-select
    /// Best Practice: Rectangle selection is essential for professional design tools
    /// </summary>
    public class SelectionRectangleAdorner : Adorner
    {
        private Point _startPoint;
        private Point _currentPoint;
        private Pen? _rectanglePen;
        private Brush? _rectangleFill;
        private bool _isSelecting;

        public SelectionRectangleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false;

            // Create pen for rectangle border
            var borderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 215)); // Windows blue
            borderBrush.Freeze();
            _rectanglePen = new Pen(borderBrush, 1);
            _rectanglePen.Freeze();

            // Create semi-transparent fill
            var fillBrush = new SolidColorBrush(Color.FromArgb(40, 0, 120, 215));
            fillBrush.Freeze();
            _rectangleFill = fillBrush;
        }

        /// <summary>
        /// Starts a new selection
        /// </summary>
        public void StartSelection(Point startPoint)
        {
            _startPoint = startPoint;
            _currentPoint = startPoint;
            _isSelecting = true;
            InvalidateVisual();
        }

        /// <summary>
        /// Updates the current selection point
        /// </summary>
        public void UpdateSelection(Point currentPoint)
        {
            if (!_isSelecting)
                return;

            _currentPoint = currentPoint;
            InvalidateVisual();
        }

        /// <summary>
        /// Ends the selection and returns the selection rectangle
        /// </summary>
        public Rect EndSelection()
        {
            _isSelecting = false;
            var rect = GetSelectionRectangle();
            InvalidateVisual();
            return rect;
        }

        /// <summary>
        /// Cancels the current selection
        /// </summary>
        public void CancelSelection()
        {
            _isSelecting = false;
            InvalidateVisual();
        }

        /// <summary>
        /// Gets the current selection rectangle
        /// </summary>
        public Rect GetSelectionRectangle()
        {
            var x = Math.Min(_startPoint.X, _currentPoint.X);
            var y = Math.Min(_startPoint.Y, _currentPoint.Y);
            var width = Math.Abs(_currentPoint.X - _startPoint.X);
            var height = Math.Abs(_currentPoint.Y - _startPoint.Y);

            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// Checks if selection is active
        /// </summary>
        public bool IsSelecting => _isSelecting;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!_isSelecting || _rectanglePen == null || _rectangleFill == null)
                return;

            var rect = GetSelectionRectangle();

            // Only draw if rectangle has area
            if (rect.Width > 1 && rect.Height > 1)
            {
                drawingContext.DrawRectangle(_rectangleFill, _rectanglePen, rect);
            }
        }
    }
}
