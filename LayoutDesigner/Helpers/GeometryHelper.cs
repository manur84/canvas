using System;
using System.Windows;

namespace LayoutDesigner.Helpers
{
    /// <summary>
    /// Helper methods for geometric calculations
    /// </summary>
    public static class GeometryHelper
    {
        /// <summary>
        /// Checks if two rectangles intersect
        /// </summary>
        public static bool Intersects(Rect rect1, Rect rect2)
        {
            return rect1.IntersectsWith(rect2);
        }

        /// <summary>
        /// Gets the center point of a rectangle
        /// </summary>
        public static Point GetCenter(Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Calculates distance between two points
        /// </summary>
        public static double Distance(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Constrains a rectangle within bounds
        /// </summary>
        public static Rect ConstrainToBounds(Rect rect, Rect bounds)
        {
            var x = Math.Max(bounds.Left, Math.Min(rect.Left, bounds.Right - rect.Width));
            var y = Math.Max(bounds.Top, Math.Min(rect.Top, bounds.Bottom - rect.Height));
            var width = Math.Min(rect.Width, bounds.Width);
            var height = Math.Min(rect.Height, bounds.Height);

            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// Maintains aspect ratio when resizing
        /// </summary>
        public static Size MaintainAspectRatio(Size originalSize, Size newSize, bool lockWidth = false)
        {
            if (originalSize.Width == 0 || originalSize.Height == 0)
                return newSize;

            double aspectRatio = originalSize.Width / originalSize.Height;

            if (lockWidth)
            {
                return new Size(newSize.Width, newSize.Width / aspectRatio);
            }
            else
            {
                return new Size(newSize.Height * aspectRatio, newSize.Height);
            }
        }
    }
}
