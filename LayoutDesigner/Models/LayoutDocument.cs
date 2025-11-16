using System;
using LayoutDesigner.Models.Base;
using System.Collections.ObjectModel;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Container for a complete layout with all elements and settings
    /// </summary>
    public class LayoutDocument
    {
        public LayoutDocument()
        {
            Elements = new ObservableCollection<LayoutElementBase>();
        }

        /// <summary>
        /// Unique identifier for this layout
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Name of the layout
        /// </summary>
        public string Name { get; set; } = "Untitled Layout";

        /// <summary>
        /// Canvas width
        /// </summary>
        public double CanvasWidth { get; set; } = 1920;

        /// <summary>
        /// Canvas height
        /// </summary>
        public double CanvasHeight { get; set; } = 1080;

        /// <summary>
        /// Background color of the canvas
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFFFF";

        /// <summary>
        /// Show grid on canvas
        /// </summary>
        public bool ShowGrid { get; set; } = true;

        /// <summary>
        /// Grid size in pixels
        /// </summary>
        public double GridSize { get; set; } = 10;

        /// <summary>
        /// Enable snap to grid
        /// </summary>
        public bool SnapToGrid { get; set; } = true;

        /// <summary>
        /// All elements in this layout
        /// </summary>
        public ObservableCollection<LayoutElementBase> Elements { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Last modified date
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Version of the layout format
        /// </summary>
        public string Version { get; set; } = "1.0";
    }
}
