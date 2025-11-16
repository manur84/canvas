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
                CornerRadius = CornerRadius
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
