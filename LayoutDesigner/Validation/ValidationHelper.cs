using System.Text.RegularExpressions;

namespace LayoutDesigner.Validation
{
    /// <summary>
    /// Helper class for input validation
    /// Best Practice: Centralized validation logic for consistency
    /// </summary>
    public static partial class ValidationHelper
    {
        #region Regex Patterns (Compiled for performance)

        // Best Practice: Use source generators for regex in .NET 7+
        [GeneratedRegex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$", RegexOptions.Compiled)]
        private static partial Regex HexColorRegex();

        [GeneratedRegex(@"^[a-zA-Z0-9_\-\s]+$", RegexOptions.Compiled)]
        private static partial Regex ElementNameRegex();

        #endregion

        #region Color Validation

        /// <summary>
        /// Validates hex color string format
        /// </summary>
        /// <param name="color">Color string to validate</param>
        /// <returns>True if valid hex color</returns>
        public static bool IsValidHexColor(string? color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return false;

            return HexColorRegex().IsMatch(color);
        }

        /// <summary>
        /// Normalizes color string to 8-character hex format (#AARRGGBB)
        /// </summary>
        public static string NormalizeHexColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return "#FF000000";

            color = color.Trim();

            // Add # if missing
            if (!color.StartsWith('#'))
                color = "#" + color;

            // Add alpha channel if missing (6 chars -> 8 chars)
            if (color.Length == 7)
                color = "#FF" + color[1..];

            return IsValidHexColor(color) ? color : "#FF000000";
        }

        #endregion

        #region Numeric Validation

        /// <summary>
        /// Validates and clamps numeric value within range
        /// </summary>
        public static double ClampValue(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Validates that value is positive
        /// </summary>
        public static bool IsPositive(double value)
        {
            return value > 0;
        }

        /// <summary>
        /// Validates that value is non-negative
        /// </summary>
        public static bool IsNonNegative(double value)
        {
            return value >= 0;
        }

        /// <summary>
        /// Validates that value is within range
        /// </summary>
        public static bool IsInRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        #endregion

        #region String Validation

        /// <summary>
        /// Validates element name (alphanumeric, underscore, hyphen, space)
        /// </summary>
        public static bool IsValidElementName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.Length > 100)
                return false;

            return ElementNameRegex().IsMatch(name);
        }

        /// <summary>
        /// Sanitizes element name by removing invalid characters
        /// </summary>
        public static string SanitizeElementName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Unnamed Element";

            name = name.Trim();

            // Remove invalid characters
            name = Regex.Replace(name, @"[^a-zA-Z0-9_\-\s]", "");

            // Limit length
            if (name.Length > 100)
                name = name[..100];

            return string.IsNullOrWhiteSpace(name) ? "Unnamed Element" : name;
        }

        /// <summary>
        /// Validates file path
        /// </summary>
        public static bool IsValidFilePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                // Check for invalid characters
                var invalidChars = Path.GetInvalidPathChars();
                if (path.Any(c => invalidChars.Contains(c)))
                    return false;

                // Try to get full path (will throw if invalid)
                _ = Path.GetFullPath(path);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Canvas Validation

        /// <summary>
        /// Validates canvas dimensions
        /// </summary>
        public static bool IsValidCanvasDimension(double dimension)
        {
            return IsInRange(dimension, 100, 10000);
        }

        /// <summary>
        /// Validates grid size
        /// </summary>
        public static bool IsValidGridSize(double gridSize)
        {
            return IsInRange(gridSize, 5, 100);
        }

        /// <summary>
        /// Validates zoom level
        /// </summary>
        public static bool IsValidZoom(double zoom)
        {
            return IsInRange(zoom, 0.1, 10.0);
        }

        /// <summary>
        /// Validates opacity value (0.0 to 1.0)
        /// </summary>
        public static bool IsValidOpacity(double opacity)
        {
            return IsInRange(opacity, 0.0, 1.0);
        }

        /// <summary>
        /// Validates rotation angle (0 to 360)
        /// </summary>
        public static bool IsValidRotation(double rotation)
        {
            return IsInRange(rotation, 0, 360);
        }

        #endregion

        #region Font Validation

        /// <summary>
        /// Validates font size
        /// </summary>
        public static bool IsValidFontSize(double fontSize)
        {
            return IsInRange(fontSize, 1, 500);
        }

        /// <summary>
        /// Validates font family name
        /// </summary>
        public static bool IsValidFontFamily(string? fontFamily)
        {
            if (string.IsNullOrWhiteSpace(fontFamily))
                return false;

            // Check if font exists in system
            try
            {
                var fonts = System.Drawing.FontFamily.Families;
                return fonts.Any(f => f.Name.Equals(fontFamily, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Element Position/Size Validation

        /// <summary>
        /// Validates element position within canvas bounds
        /// </summary>
        public static bool IsWithinCanvasBounds(double x, double y, double width, double height,
            double canvasWidth, double canvasHeight)
        {
            return x >= 0 && y >= 0 &&
                   (x + width) <= canvasWidth &&
                   (y + height) <= canvasHeight;
        }

        /// <summary>
        /// Clamps element position to canvas bounds
        /// </summary>
        public static (double x, double y) ClampToCanvasBounds(double x, double y, double width, double height,
            double canvasWidth, double canvasHeight)
        {
            var clampedX = Math.Max(0, Math.Min(x, canvasWidth - width));
            var clampedY = Math.Max(0, Math.Min(y, canvasHeight - height));

            return (clampedX, clampedY);
        }

        /// <summary>
        /// Validates element dimensions
        /// </summary>
        public static bool IsValidElementDimension(double dimension)
        {
            return IsInRange(dimension, 1, 5000);
        }

        #endregion
    }
}
