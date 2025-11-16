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
    }
}
