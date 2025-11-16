using System;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Represents a line element for connections and dividers
    /// </summary>
    public class LineElement : LayoutElementBase
    {
        private double _x2 = 100;
        private double _y2 = 100;
        private string _strokeColor = "#FF000000";
        private double _strokeThickness = 2;
        private string _strokeDashStyle = "Solid";
        private string _startCap = "None";
        private string _endCap = "None";

        public override string ElementType => "Line";

        /// <summary>
        /// End X coordinate
        /// </summary>
        public double X2
        {
            get => _x2;
            set => SetProperty(ref _x2, value);
        }

        /// <summary>
        /// End Y coordinate
        /// </summary>
        public double Y2
        {
            get => _y2;
            set => SetProperty(ref _y2, value);
        }

        /// <summary>
        /// Line color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string StrokeColor
        {
            get => _strokeColor;
            set => SetProperty(ref _strokeColor, value);
        }

        /// <summary>
        /// Line thickness in pixels
        /// </summary>
        public double StrokeThickness
        {
            get => _strokeThickness;
            set => SetProperty(ref _strokeThickness, value);
        }

        /// <summary>
        /// Dash style: Solid, Dash, Dot, DashDot
        /// </summary>
        public string StrokeDashStyle
        {
            get => _strokeDashStyle;
            set => SetProperty(ref _strokeDashStyle, value);
        }

        /// <summary>
        /// Start cap style: None, Arrow, Circle, Square
        /// </summary>
        public string StartCap
        {
            get => _startCap;
            set => SetProperty(ref _startCap, value);
        }

        /// <summary>
        /// End cap style: None, Arrow, Circle, Square
        /// </summary>
        public string EndCap
        {
            get => _endCap;
            set => SetProperty(ref _endCap, value);
        }

        public override LayoutElementBase Clone()
        {
            return new LineElement
            {
                Name = Name + " (Copy)",
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                Rotation = Rotation,
                ZIndex = ZIndex,
                Opacity = Opacity,
                IsLocked = IsLocked,
                IsVisible = IsVisible,
                X2 = X2,
                Y2 = Y2,
                StrokeColor = StrokeColor,
                StrokeThickness = StrokeThickness,
                StrokeDashStyle = StrokeDashStyle,
                StartCap = StartCap,
                EndCap = EndCap
            };
        }
    }
}
