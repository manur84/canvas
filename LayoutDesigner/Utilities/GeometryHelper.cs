using System;
using System.Linq;
using System.Windows;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Utilities
{
    /// <summary>
    /// Helper methods for geometric calculations
    /// Best Practice: Centralize geometric calculations for reuse
    /// </summary>
    public static class GeometryHelper
    {
        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        public static double Distance(Point p1, Point p2)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calculates the distance between two elements (center to center)
        /// </summary>
        public static double Distance(LayoutElementBase e1, LayoutElementBase e2)
        {
            var center1 = GetCenter(e1);
            var center2 = GetCenter(e2);
            return Distance(center1, center2);
        }

        /// <summary>
        /// Gets the center point of an element
        /// </summary>
        public static Point GetCenter(LayoutElementBase element)
        {
            return new Point(
                element.X + element.Width / 2,
                element.Y + element.Height / 2);
        }

        /// <summary>
        /// Gets the center point of a rectangle
        /// </summary>
        public static Point GetCenter(Rect rect)
        {
            return new Point(
                rect.X + rect.Width / 2,
                rect.Y + rect.Height / 2);
        }

        /// <summary>
        /// Rotates a point around another point
        /// </summary>
        public static Point RotatePoint(Point point, Point center, double angleInDegrees)
        {
            var angleInRadians = angleInDegrees * Math.PI / 180.0;
            var cos = Math.Cos(angleInRadians);
            var sin = Math.Sin(angleInRadians);

            var translatedX = point.X - center.X;
            var translatedY = point.Y - center.Y;

            var rotatedX = translatedX * cos - translatedY * sin;
            var rotatedY = translatedX * sin + translatedY * cos;

            return new Point(
                rotatedX + center.X,
                rotatedY + center.Y);
        }

        /// <summary>
        /// Calculates the angle between two points in degrees
        /// </summary>
        public static double AngleBetween(Point p1, Point p2)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            var radians = Math.Atan2(dy, dx);
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Checks if a point is inside an element (considering rotation)
        /// </summary>
        public static bool IsPointInElement(Point point, LayoutElementBase element)
        {
            if (element.Rotation == 0)
            {
                // Simple rect check
                return point.X >= element.X && point.X <= element.X + element.Width &&
                       point.Y >= element.Y && point.Y <= element.Y + element.Height;
            }

            // Rotate point back and check
            var center = GetCenter(element);
            var rotatedPoint = RotatePoint(point, center, -element.Rotation);

            return rotatedPoint.X >= element.X && rotatedPoint.X <= element.X + element.Width &&
                   rotatedPoint.Y >= element.Y && rotatedPoint.Y <= element.Y + element.Height;
        }

        /// <summary>
        /// Gets the four corner points of an element (considering rotation)
        /// </summary>
        public static Point[] GetCornerPoints(LayoutElementBase element)
        {
            var corners = new[]
            {
                new Point(element.X, element.Y),
                new Point(element.X + element.Width, element.Y),
                new Point(element.X + element.Width, element.Y + element.Height),
                new Point(element.X, element.Y + element.Height)
            };

            if (element.Rotation != 0)
            {
                var center = GetCenter(element);
                for (int i = 0; i < corners.Length; i++)
                {
                    corners[i] = RotatePoint(corners[i], center, element.Rotation);
                }
            }

            return corners;
        }

        /// <summary>
        /// Gets the bounding rectangle of an element (considering rotation)
        /// </summary>
        public static Rect GetRotatedBounds(LayoutElementBase element)
        {
            if (element.Rotation == 0)
            {
                return new Rect(element.X, element.Y, element.Width, element.Height);
            }

            var corners = GetCornerPoints(element);
            var minX = corners.Min(p => p.X);
            var minY = corners.Min(p => p.Y);
            var maxX = corners.Max(p => p.X);
            var maxY = corners.Max(p => p.Y);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Constrains a point to a rectangle
        /// </summary>
        public static Point ConstrainToRect(Point point, Rect bounds)
        {
            return new Point(
                Math.Max(bounds.Left, Math.Min(bounds.Right, point.X)),
                Math.Max(bounds.Top, Math.Min(bounds.Bottom, point.Y)));
        }

        /// <summary>
        /// Constrains an element to canvas bounds
        /// </summary>
        public static void ConstrainToCanvas(LayoutElementBase element, double canvasWidth, double canvasHeight)
        {
            element.X = Math.Max(0, Math.Min(canvasWidth - element.Width, element.X));
            element.Y = Math.Max(0, Math.Min(canvasHeight - element.Height, element.Y));
        }

        /// <summary>
        /// Calculates the area of intersection between two rectangles
        /// </summary>
        public static double IntersectionArea(Rect rect1, Rect rect2)
        {
            rect1.Intersect(rect2);
            return rect1.Width * rect1.Height;
        }

        /// <summary>
        /// Calculates the area of union between two rectangles
        /// </summary>
        public static double UnionArea(Rect rect1, Rect rect2)
        {
            rect1.Union(rect2);
            return rect1.Width * rect1.Height;
        }

        /// <summary>
        /// Calculates the overlap percentage between two elements
        /// </summary>
        public static double OverlapPercentage(LayoutElementBase e1, LayoutElementBase e2)
        {
            var rect1 = new Rect(e1.X, e1.Y, e1.Width, e1.Height);
            var rect2 = new Rect(e2.X, e2.Y, e2.Width, e2.Height);

            var intersectionArea = IntersectionArea(rect1, rect2);
            var minArea = Math.Min(rect1.Width * rect1.Height, rect2.Width * rect2.Height);

            return minArea > 0 ? intersectionArea / minArea : 0;
        }

        /// <summary>
        /// Expands a rectangle by a margin on all sides
        /// </summary>
        public static Rect Expand(Rect rect, double margin)
        {
            return new Rect(
                rect.X - margin,
                rect.Y - margin,
                rect.Width + 2 * margin,
                rect.Height + 2 * margin);
        }

        /// <summary>
        /// Shrinks a rectangle by a margin on all sides
        /// </summary>
        public static Rect Shrink(Rect rect, double margin)
        {
            return Expand(rect, -margin);
        }

        /// <summary>
        /// Calculates the aspect ratio of a rectangle
        /// </summary>
        public static double AspectRatio(Rect rect)
        {
            return rect.Height > 0 ? rect.Width / rect.Height : 0;
        }

        /// <summary>
        /// Calculates the aspect ratio of an element
        /// </summary>
        public static double AspectRatio(LayoutElementBase element)
        {
            return element.Height > 0 ? element.Width / element.Height : 0;
        }

        /// <summary>
        /// Scales a size while maintaining aspect ratio
        /// </summary>
        public static Size ScaleToFit(Size original, Size target, bool scaleUp = false)
        {
            if (original.Width == 0 || original.Height == 0)
                return target;

            var aspectRatio = original.Width / original.Height;
            var targetWidth = target.Width;
            var targetHeight = target.Height;

            if (targetWidth / aspectRatio <= targetHeight)
            {
                targetHeight = targetWidth / aspectRatio;
            }
            else
            {
                targetWidth = targetHeight * aspectRatio;
            }

            // Only scale down unless scaleUp is true
            if (!scaleUp)
            {
                targetWidth = Math.Min(targetWidth, original.Width);
                targetHeight = Math.Min(targetHeight, original.Height);
            }

            return new Size(targetWidth, targetHeight);
        }

        /// <summary>
        /// Interpolates between two points
        /// </summary>
        public static Point Lerp(Point p1, Point p2, double t)
        {
            t = Math.Max(0, Math.Min(1, t)); // Clamp to [0,1]
            return new Point(
                p1.X + (p2.X - p1.X) * t,
                p1.Y + (p2.Y - p1.Y) * t);
        }

        /// <summary>
        /// Interpolates between two values
        /// </summary>
        public static double Lerp(double a, double b, double t)
        {
            t = Math.Max(0, Math.Min(1, t)); // Clamp to [0,1]
            return a + (b - a) * t;
        }
    }
}
