using System.Windows;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Constants;

namespace LayoutDesigner.Helpers
{
    /// <summary>
    /// Helper for snapping elements to grid and other elements
    /// Best Practice: Smart snapping improves user experience
    /// </summary>
    public static class SnapHelper
    {
        /// <summary>
        /// Default snap distance in pixels for element-to-element snapping
        /// </summary>
        public const double DefaultSnapDistance = UIConstants.DefaultSnapDistance;

        #region Grid Snapping

        /// <summary>
        /// Snaps a value to the nearest grid line
        /// </summary>
        public static double SnapToGrid(double value, double gridSize)
        {
            if (gridSize <= 0)
                return value;

            return Math.Round(value / gridSize) * gridSize;
        }

        /// <summary>
        /// Snaps a point to the nearest grid intersection
        /// </summary>
        public static Point SnapToGrid(Point point, double gridSize)
        {
            return new Point(
                SnapToGrid(point.X, gridSize),
                SnapToGrid(point.Y, gridSize));
        }

        /// <summary>
        /// Snaps a rect to the grid
        /// </summary>
        public static Rect SnapToGrid(Rect rect, double gridSize)
        {
            return new Rect(
                SnapToGrid(rect.X, gridSize),
                SnapToGrid(rect.Y, gridSize),
                SnapToGrid(rect.Width, gridSize),
                SnapToGrid(rect.Height, gridSize));
        }

        #endregion

        #region Element-to-Element Snapping

        /// <summary>
        /// Snaps element position to nearby elements
        /// Returns adjusted position if snap occurred, original position otherwise
        /// </summary>
        public static (double x, double y, bool snapped) SnapToElements(
            LayoutElementBase movingElement,
            IEnumerable<LayoutElementBase> otherElements,
            double snapDistance = DefaultSnapDistance)
        {
            if (movingElement == null || otherElements == null)
                return (movingElement?.X ?? 0, movingElement?.Y ?? 0, false);

            var x = movingElement.X;
            var y = movingElement.Y;
            var width = movingElement.Width;
            var height = movingElement.Height;

            double? snapX = null;
            double? snapY = null;
            var minDistX = snapDistance;
            var minDistY = snapDistance;

            foreach (var other in otherElements)
            {
                // Skip self
                if (other == movingElement)
                    continue;

                var otherRight = other.X + other.Width;
                var otherBottom = other.Y + other.Height;
                var movingRight = x + width;
                var movingBottom = y + height;

                // Horizontal snapping (left, center, right alignment)
                CheckHorizontalSnap(x, width, other.X, other.Width, snapDistance, ref snapX, ref minDistX);

                // Vertical snapping (top, center, bottom alignment)
                CheckVerticalSnap(y, height, other.Y, other.Height, snapDistance, ref snapY, ref minDistY);
            }

            var snapped = snapX.HasValue || snapY.HasValue;
            return (snapX ?? x, snapY ?? y, snapped);
        }

        private static void CheckHorizontalSnap(double x, double width, double otherX, double otherWidth,
            double snapDistance, ref double? snapX, ref double minDistX)
        {
            var right = x + width;
            var otherRight = otherX + otherWidth;
            var centerX = x + width / 2;
            var otherCenterX = otherX + otherWidth / 2;

            // Left to Left
            var distLeftToLeft = Math.Abs(x - otherX);
            if (distLeftToLeft < minDistX)
            {
                snapX = otherX;
                minDistX = distLeftToLeft;
            }

            // Right to Right
            var distRightToRight = Math.Abs(right - otherRight);
            if (distRightToRight < minDistX)
            {
                snapX = otherRight - width;
                minDistX = distRightToRight;
            }

            // Left to Right
            var distLeftToRight = Math.Abs(x - otherRight);
            if (distLeftToRight < minDistX)
            {
                snapX = otherRight;
                minDistX = distLeftToRight;
            }

            // Right to Left
            var distRightToLeft = Math.Abs(right - otherX);
            if (distRightToLeft < minDistX)
            {
                snapX = otherX - width;
                minDistX = distRightToLeft;
            }

            // Center to Center
            var distCenterToCenter = Math.Abs(centerX - otherCenterX);
            if (distCenterToCenter < minDistX)
            {
                snapX = otherCenterX - width / 2;
                minDistX = distCenterToCenter;
            }
        }

        private static void CheckVerticalSnap(double y, double height, double otherY, double otherHeight,
            double snapDistance, ref double? snapY, ref double minDistY)
        {
            var bottom = y + height;
            var otherBottom = otherY + otherHeight;
            var centerY = y + height / 2;
            var otherCenterY = otherY + otherHeight / 2;

            // Top to Top
            var distTopToTop = Math.Abs(y - otherY);
            if (distTopToTop < minDistY)
            {
                snapY = otherY;
                minDistY = distTopToTop;
            }

            // Bottom to Bottom
            var distBottomToBottom = Math.Abs(bottom - otherBottom);
            if (distBottomToBottom < minDistY)
            {
                snapY = otherBottom - height;
                minDistY = distBottomToBottom;
            }

            // Top to Bottom
            var distTopToBottom = Math.Abs(y - otherBottom);
            if (distTopToBottom < minDistY)
            {
                snapY = otherBottom;
                minDistY = distTopToBottom;
            }

            // Bottom to Top
            var distBottomToTop = Math.Abs(bottom - otherY);
            if (distBottomToTop < minDistY)
            {
                snapY = otherY - height;
                minDistY = distBottomToTop;
            }

            // Center to Center
            var distCenterToCenter = Math.Abs(centerY - otherCenterY);
            if (distCenterToCenter < minDistY)
            {
                snapY = otherCenterY - height / 2;
                minDistY = distCenterToCenter;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets bounding rectangle for an element
        /// </summary>
        public static Rect GetBounds(LayoutElementBase element)
        {
            return new Rect(element.X, element.Y, element.Width, element.Height);
        }

        /// <summary>
        /// Checks if two rectangles are close to each other (for snap detection)
        /// </summary>
        public static bool AreClose(Rect rect1, Rect rect2, double distance)
        {
            // Check if rectangles are within snap distance
            var horizontalGap = Math.Max(0,
                Math.Max(rect1.Left, rect2.Left) - Math.Min(rect1.Right, rect2.Right));
            var verticalGap = Math.Max(0,
                Math.Max(rect1.Top, rect2.Top) - Math.Min(rect1.Bottom, rect2.Bottom));

            return horizontalGap <= distance || verticalGap <= distance;
        }

        #endregion
    }
}
