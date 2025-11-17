namespace LayoutDesigner.Constants
{
    /// <summary>
    /// UI-related constants for the Layout Designer
    /// Best Practice: Centralize magic numbers for maintainability
    /// </summary>
    public static class UIConstants
    {
        /// <summary>
        /// Dead zone threshold in pixels for drag operations
        /// Prevents accidental drags from small mouse movements
        /// </summary>
        public const double DragDeadZonePixels = 3.0;

        /// <summary>
        /// Zoom factor for zoom in/out operations
        /// 1.2 = 20% zoom increment
        /// </summary>
        public const double ZoomFactor = 1.2;

        /// <summary>
        /// Offset in pixels when pasting or duplicating elements
        /// Prevents pasted elements from appearing exactly on top of originals
        /// </summary>
        public const double PasteOffsetPixels = 20.0;

        /// <summary>
        /// Offset in pixels when dropping images from drag and drop
        /// Used to position subsequent dropped images
        /// </summary>
        public const double DropOffsetPixels = 20.0;

        #region Snapping

        /// <summary>
        /// Default snap distance in pixels for element-to-element snapping
        /// Elements snap when within this distance of each other
        /// </summary>
        public const double DefaultSnapDistance = 8.0;

        #endregion

        #region Canvas Limits

        /// <summary>
        /// Minimum canvas width or height in pixels
        /// </summary>
        public const double MinCanvasSize = 100;

        /// <summary>
        /// Maximum canvas width or height in pixels
        /// </summary>
        public const double MaxCanvasSize = 10000;

        #endregion

        #region Grid Limits

        /// <summary>
        /// Minimum grid size in pixels
        /// </summary>
        public const double MinGridSize = 5;

        /// <summary>
        /// Maximum grid size in pixels
        /// </summary>
        public const double MaxGridSize = 100;

        #endregion

        #region Element Size Limits

        /// <summary>
        /// Minimum element width or height in pixels
        /// </summary>
        public const double MinElementSize = 10;

        /// <summary>
        /// Maximum element width or height in pixels
        /// </summary>
        public const double MaxElementSize = 5000;

        #endregion

        #region SelectionAdorner Constants

        /// <summary>
        /// Size of resize handles in pixels
        /// </summary>
        public const double ResizeHandleSize = 10;

        /// <summary>
        /// Size of resize handles when hovered (in pixels)
        /// </summary>
        public const double ResizeHandleHoverSize = 12;

        /// <summary>
        /// Size of rotate handle in pixels
        /// </summary>
        public const double RotateHandleSize = 12;

        /// <summary>
        /// Size of rotate handle when hovered (in pixels)
        /// </summary>
        public const double RotateHandleHoverSize = 14;

        /// <summary>
        /// Vertical distance of rotate handle from element (in pixels)
        /// </summary>
        public const double RotateHandleDistance = 30;

        /// <summary>
        /// Border thickness for selection rectangle
        /// </summary>
        public const double SelectionBorderThickness = 2;

        /// <summary>
        /// Shadow border thickness for selection (depth effect)
        /// </summary>
        public const double SelectionShadowThickness = 4;

        /// <summary>
        /// Blur radius for shadow effect
        /// </summary>
        public const double ShadowBlurRadius = 3;

        /// <summary>
        /// Blur radius for drop shadow on handles
        /// </summary>
        public const double HandleShadowBlurRadius = 4;

        /// <summary>
        /// Shadow depth for drop shadow on handles
        /// </summary>
        public const double HandleShadowDepth = 2;

        /// <summary>
        /// Shadow opacity for drop shadow on handles
        /// </summary>
        public const double HandleShadowOpacity = 0.4;

        #endregion
    }
}
