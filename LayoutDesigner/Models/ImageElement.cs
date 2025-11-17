using System;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Image element with source path and scaling options
    /// </summary>
    public class ImageElement : LayoutElementBase
    {
        private string _imagePath = string.Empty;
        private string? _imageData = null; // Base64 encoded image data
        private ImageStretchMode _stretchMode = ImageStretchMode.Uniform;
        private bool _maintainAspectRatio = true;
        private bool _hasBorder = false;
        private string _borderColor = "#FF000000";
        private double _borderThickness = 1;
        private double _cornerRadius = 0;
        private bool _hasShadow = false;
        private string _shadowColor = "#80000000";
        private double _shadowBlur = 10;
        private double _shadowOffsetX = 5;
        private double _shadowOffsetY = 5;
        private double _brightness = 1.0;
        private double _contrast = 1.0;
        private double _saturation = 1.0;
        private double _blur = 0;
        private bool _grayscale = false;

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
        /// Base64 encoded image data (used for embedding images in the layout file)
        /// When this is set, it takes priority over ImagePath
        /// </summary>
        public string? ImageData
        {
            get => _imageData;
            set => SetProperty(ref _imageData, value);
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

        /// <summary>
        /// Enable border
        /// </summary>
        public bool HasBorder
        {
            get => _hasBorder;
            set => SetProperty(ref _hasBorder, value);
        }

        /// <summary>
        /// Border color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, value);
        }

        /// <summary>
        /// Border thickness in pixels
        /// </summary>
        public double BorderThickness
        {
            get => _borderThickness;
            set => SetProperty(ref _borderThickness, value);
        }

        /// <summary>
        /// Corner radius for rounded corners
        /// </summary>
        public double CornerRadius
        {
            get => _cornerRadius;
            set => SetProperty(ref _cornerRadius, value);
        }

        /// <summary>
        /// Enable drop shadow
        /// </summary>
        public bool HasShadow
        {
            get => _hasShadow;
            set => SetProperty(ref _hasShadow, value);
        }

        /// <summary>
        /// Shadow color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string ShadowColor
        {
            get => _shadowColor;
            set => SetProperty(ref _shadowColor, value);
        }

        /// <summary>
        /// Shadow blur radius
        /// </summary>
        public double ShadowBlur
        {
            get => _shadowBlur;
            set => SetProperty(ref _shadowBlur, value);
        }

        /// <summary>
        /// Shadow horizontal offset
        /// </summary>
        public double ShadowOffsetX
        {
            get => _shadowOffsetX;
            set => SetProperty(ref _shadowOffsetX, value);
        }

        /// <summary>
        /// Shadow vertical offset
        /// </summary>
        public double ShadowOffsetY
        {
            get => _shadowOffsetY;
            set => SetProperty(ref _shadowOffsetY, value);
        }

        /// <summary>
        /// Brightness adjustment (1.0 = normal, 0.0 = black, 2.0 = double bright)
        /// </summary>
        public double Brightness
        {
            get => _brightness;
            set => SetProperty(ref _brightness, value);
        }

        /// <summary>
        /// Contrast adjustment (1.0 = normal, 0.0 = gray, 2.0 = high contrast)
        /// </summary>
        public double Contrast
        {
            get => _contrast;
            set => SetProperty(ref _contrast, value);
        }

        /// <summary>
        /// Saturation adjustment (1.0 = normal, 0.0 = grayscale, 2.0 = vibrant)
        /// </summary>
        public double Saturation
        {
            get => _saturation;
            set => SetProperty(ref _saturation, value);
        }

        /// <summary>
        /// Blur radius in pixels
        /// </summary>
        public double Blur
        {
            get => _blur;
            set => SetProperty(ref _blur, value);
        }

        /// <summary>
        /// Convert image to grayscale
        /// </summary>
        public bool Grayscale
        {
            get => _grayscale;
            set => SetProperty(ref _grayscale, value);
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
                ImageData = ImageData,
                StretchMode = StretchMode,
                MaintainAspectRatio = MaintainAspectRatio,
                HasBorder = HasBorder,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness,
                CornerRadius = CornerRadius,
                HasShadow = HasShadow,
                ShadowColor = ShadowColor,
                ShadowBlur = ShadowBlur,
                ShadowOffsetX = ShadowOffsetX,
                ShadowOffsetY = ShadowOffsetY,
                Brightness = Brightness,
                Contrast = Contrast,
                Saturation = Saturation,
                Blur = Blur,
                Grayscale = Grayscale
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
