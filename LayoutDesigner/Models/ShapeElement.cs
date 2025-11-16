using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Shape element (Rectangle, Ellipse, Line) with fill and stroke
    /// </summary>
    public class ShapeElement : LayoutElementBase
    {
        private ShapeType _shapeType = ShapeType.Rectangle;
        private string _fillColor = "#FF808080";
        private string _strokeColor = "#FF000000";
        private double _strokeThickness = 2;
        private double _cornerRadius;
        private bool _useGradient = false;
        private string _gradientStartColor = "#FF808080";
        private string _gradientEndColor = "#FF404040";
        private string _gradientDirection = "Vertical";
        private bool _hasShadow = false;
        private string _shadowColor = "#80000000";
        private double _shadowBlur = 10;
        private double _shadowOffsetX = 5;
        private double _shadowOffsetY = 5;
        private string _strokeDashStyle = "Solid";

        public override string ElementType => "Shape";

        /// <summary>
        /// Type of shape
        /// </summary>
        public ShapeType ShapeType
        {
            get => _shapeType;
            set => SetProperty(ref _shapeType, value);
        }

        /// <summary>
        /// Fill color as hex string
        /// </summary>
        public string FillColor
        {
            get => _fillColor;
            set => SetProperty(ref _fillColor, value);
        }

        /// <summary>
        /// Stroke (border) color as hex string
        /// </summary>
        public string StrokeColor
        {
            get => _strokeColor;
            set => SetProperty(ref _strokeColor, value);
        }

        /// <summary>
        /// Stroke thickness in pixels
        /// </summary>
        public double StrokeThickness
        {
            get => _strokeThickness;
            set => SetProperty(ref _strokeThickness, value);
        }

        /// <summary>
        /// Corner radius for rounded rectangles
        /// </summary>
        public double CornerRadius
        {
            get => _cornerRadius;
            set => SetProperty(ref _cornerRadius, value);
        }

        /// <summary>
        /// Use gradient fill instead of solid color
        /// </summary>
        public bool UseGradient
        {
            get => _useGradient;
            set => SetProperty(ref _useGradient, value);
        }

        /// <summary>
        /// Gradient start color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string GradientStartColor
        {
            get => _gradientStartColor;
            set => SetProperty(ref _gradientStartColor, value);
        }

        /// <summary>
        /// Gradient end color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string GradientEndColor
        {
            get => _gradientEndColor;
            set => SetProperty(ref _gradientEndColor, value);
        }

        /// <summary>
        /// Gradient direction: Vertical, Horizontal, Diagonal, Radial
        /// </summary>
        public string GradientDirection
        {
            get => _gradientDirection;
            set => SetProperty(ref _gradientDirection, value);
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
        /// Stroke dash style: Solid, Dash, Dot, DashDot
        /// </summary>
        public string StrokeDashStyle
        {
            get => _strokeDashStyle;
            set => SetProperty(ref _strokeDashStyle, value);
        }

        public override LayoutElementBase Clone()
        {
            return new ShapeElement
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
                ShapeType = ShapeType,
                FillColor = FillColor,
                StrokeColor = StrokeColor,
                StrokeThickness = StrokeThickness,
                CornerRadius = CornerRadius,
                UseGradient = UseGradient,
                GradientStartColor = GradientStartColor,
                GradientEndColor = GradientEndColor,
                GradientDirection = GradientDirection,
                HasShadow = HasShadow,
                ShadowColor = ShadowColor,
                ShadowBlur = ShadowBlur,
                ShadowOffsetX = ShadowOffsetX,
                ShadowOffsetY = ShadowOffsetY,
                StrokeDashStyle = StrokeDashStyle
            };
        }
    }

    /// <summary>
    /// Available shape types
    /// </summary>
    public enum ShapeType
    {
        Rectangle,
        Ellipse,
        Line,
        RoundedRectangle
    }
}
