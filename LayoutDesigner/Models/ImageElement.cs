using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Image element with source path and scaling options
    /// </summary>
    public class ImageElement : LayoutElementBase
    {
        private string _imagePath = string.Empty;
        private ImageStretchMode _stretchMode = ImageStretchMode.Uniform;
        private bool _maintainAspectRatio = true;

        public override string ElementType => "Image";

        /// <summary>
        /// Path to the image file (can be absolute or relative)
        /// </summary>
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        /// <summary>
        /// How the image should be stretched
        /// </summary>
        public ImageStretchMode StretchMode
        {
            get => _stretchMode;
            set => SetProperty(ref _stretchMode, value);
        }

        /// <summary>
        /// Whether to maintain the image's aspect ratio
        /// </summary>
        public bool MaintainAspectRatio
        {
            get => _maintainAspectRatio;
            set => SetProperty(ref _maintainAspectRatio, value);
        }

        public override LayoutElementBase Clone()
        {
            return new ImageElement
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name + " (Copy)",
                X = X + 20,
                Y = Y + 20,
                Width = Width,
                Height = Height,
                Rotation = Rotation,
                ZIndex = ZIndex,
                IsVisible = IsVisible,
                Opacity = Opacity,
                ImagePath = ImagePath,
                StretchMode = StretchMode,
                MaintainAspectRatio = MaintainAspectRatio
            };
        }
    }

    /// <summary>
    /// Image stretch modes
    /// </summary>
    public enum ImageStretchMode
    {
        None,       // No stretching
        Fill,       // Fill the entire area (may distort)
        Uniform,    // Fit within area maintaining aspect ratio
        UniformToFill // Fill area maintaining aspect ratio (may crop)
    }
}
