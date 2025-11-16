using System.Collections.Generic;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Extensions
{
    /// <summary>
    /// Extension methods for collections
    /// Best Practice: Extension methods improve code readability
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Finds elements by name (case-insensitive)
        /// </summary>
        public static IEnumerable<LayoutElementBase> FindByName(
            this IEnumerable<LayoutElementBase> elements,
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Enumerable.Empty<LayoutElementBase>();

            return elements.Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Finds elements by type
        /// </summary>
        public static IEnumerable<T> FindByType<T>(
            this IEnumerable<LayoutElementBase> elements)
            where T : LayoutElementBase
        {
            return elements.OfType<T>();
        }

        /// <summary>
        /// Finds elements within a rectangular area
        /// </summary>
        public static IEnumerable<LayoutElementBase> FindInArea(
            this IEnumerable<LayoutElementBase> elements,
            double x, double y, double width, double height)
        {
            var searchRect = new System.Windows.Rect(x, y, width, height);

            return elements.Where(e =>
            {
                var elementRect = new System.Windows.Rect(e.X, e.Y, e.Width, e.Height);
                return searchRect.IntersectsWith(elementRect);
            });
        }

        /// <summary>
        /// Finds elements that intersect with a point
        /// </summary>
        public static IEnumerable<LayoutElementBase> FindAtPoint(
            this IEnumerable<LayoutElementBase> elements,
            double x, double y)
        {
            return elements.Where(e =>
                x >= e.X && x <= (e.X + e.Width) &&
                y >= e.Y && y <= (e.Y + e.Height));
        }

        /// <summary>
        /// Gets all visible elements
        /// </summary>
        public static IEnumerable<LayoutElementBase> WhereVisible(
            this IEnumerable<LayoutElementBase> elements)
        {
            return elements.Where(e => e.IsVisible);
        }

        /// <summary>
        /// Gets all locked elements
        /// </summary>
        public static IEnumerable<LayoutElementBase> WhereLocked(
            this IEnumerable<LayoutElementBase> elements)
        {
            return elements.Where(e => e.IsLocked);
        }

        /// <summary>
        /// Gets all unlocked elements
        /// </summary>
        public static IEnumerable<LayoutElementBase> WhereUnlocked(
            this IEnumerable<LayoutElementBase> elements)
        {
            return elements.Where(e => !e.IsLocked);
        }

        /// <summary>
        /// Orders elements by Z-Index (ascending)
        /// </summary>
        public static IEnumerable<LayoutElementBase> OrderByZIndex(
            this IEnumerable<LayoutElementBase> elements)
        {
            return elements.OrderBy(e => e.ZIndex);
        }

        /// <summary>
        /// Orders elements by Z-Index (descending)
        /// </summary>
        public static IEnumerable<LayoutElementBase> OrderByZIndexDescending(
            this IEnumerable<LayoutElementBase> elements)
        {
            return elements.OrderByDescending(e => e.ZIndex);
        }

        /// <summary>
        /// Gets the bounding box that contains all elements
        /// </summary>
        public static System.Windows.Rect GetBoundingBox(
            this IEnumerable<LayoutElementBase> elements)
        {
            var elementsList = elements.ToList();
            if (elementsList.Count == 0)
                return System.Windows.Rect.Empty;

            var minX = elementsList.Min(e => e.X);
            var minY = elementsList.Min(e => e.Y);
            var maxX = elementsList.Max(e => e.X + e.Width);
            var maxY = elementsList.Max(e => e.Y + e.Height);

            return new System.Windows.Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Checks if any elements overlap
        /// </summary>
        public static bool HasOverlaps(this IEnumerable<LayoutElementBase> elements)
        {
            var elementsList = elements.ToList();

            for (int i = 0; i < elementsList.Count; i++)
            {
                for (int j = i + 1; j < elementsList.Count; j++)
                {
                    var rect1 = new System.Windows.Rect(
                        elementsList[i].X,
                        elementsList[i].Y,
                        elementsList[i].Width,
                        elementsList[i].Height);

                    var rect2 = new System.Windows.Rect(
                        elementsList[j].X,
                        elementsList[j].Y,
                        elementsList[j].Width,
                        elementsList[j].Height);

                    if (rect1.IntersectsWith(rect2))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Centers elements horizontally within a container
        /// </summary>
        public static void CenterHorizontally(
            this IEnumerable<LayoutElementBase> elements,
            double containerWidth)
        {
            foreach (var element in elements)
            {
                element.X = (containerWidth - element.Width) / 2;
            }
        }

        /// <summary>
        /// Centers elements vertically within a container
        /// </summary>
        public static void CenterVertically(
            this IEnumerable<LayoutElementBase> elements,
            double containerHeight)
        {
            foreach (var element in elements)
            {
                element.Y = (containerHeight - element.Height) / 2;
            }
        }

        /// <summary>
        /// Spaces elements evenly along X-axis
        /// </summary>
        public static void SpaceEvenlyHorizontal(
            this IEnumerable<LayoutElementBase> elements,
            double startX, double endX)
        {
            var elementsList = elements.OrderBy(e => e.X).ToList();
            if (elementsList.Count < 2)
                return;

            var totalWidth = elementsList.Sum(e => e.Width);
            var availableSpace = endX - startX - totalWidth;
            var spacing = availableSpace / (elementsList.Count - 1);

            var currentX = startX;
            foreach (var element in elementsList)
            {
                element.X = currentX;
                currentX += element.Width + spacing;
            }
        }

        /// <summary>
        /// Spaces elements evenly along Y-axis
        /// </summary>
        public static void SpaceEvenlyVertical(
            this IEnumerable<LayoutElementBase> elements,
            double startY, double endY)
        {
            var elementsList = elements.OrderBy(e => e.Y).ToList();
            if (elementsList.Count < 2)
                return;

            var totalHeight = elementsList.Sum(e => e.Height);
            var availableSpace = endY - startY - totalHeight;
            var spacing = availableSpace / (elementsList.Count - 1);

            var currentY = startY;
            foreach (var element in elementsList)
            {
                element.Y = currentY;
                currentY += element.Height + spacing;
            }
        }
    }
}
