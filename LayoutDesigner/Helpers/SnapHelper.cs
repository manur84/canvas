using System.Windows;

namespace LayoutDesigner.Helpers
{
    /// <summary>
    /// Helper for snapping elements to grid
    /// </summary>
    public static class SnapHelper
    {
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
    }
}
