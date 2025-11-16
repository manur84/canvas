using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Represents an interactive button element
    /// </summary>
    public class ButtonElement : LayoutElementBase
    {
        private string _text = "Button";
        private string _fontFamily = "Segoe UI";
        private double _fontSize = 16;
        private bool _isBold = false;
        private bool _isItalic = false;
        private string _foregroundColor = "#FFFFFFFF";
        private string _backgroundColor = "#FF0078D7";
        private string _borderColor = "#FF0078D7";
        private double _borderThickness = 0;
        private double _cornerRadius = 4;
        private string _hoverBackgroundColor = "#FF005A9E";
        private string _pressedBackgroundColor = "#FF004275";
        private string _icon = "";
        private string _iconPosition = "Left";

        public ButtonElement()
        {
            ElementType = "Button";
            Width = 120;
            Height = 40;
        }

        /// <summary>
        /// Button text
        /// </summary>
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        /// <summary>
        /// Font family
        /// </summary>
        public string FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        /// <summary>
        /// Font size in points
        /// </summary>
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        /// <summary>
        /// Bold text
        /// </summary>
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        /// <summary>
        /// Italic text
        /// </summary>
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }

        /// <summary>
        /// Text color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, value);
        }

        /// <summary>
        /// Background color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
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
        /// Background color when hovering (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string HoverBackgroundColor
        {
            get => _hoverBackgroundColor;
            set => SetProperty(ref _hoverBackgroundColor, value);
        }

        /// <summary>
        /// Background color when pressed (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string PressedBackgroundColor
        {
            get => _pressedBackgroundColor;
            set => SetProperty(ref _pressedBackgroundColor, value);
        }

        /// <summary>
        /// Icon text/character (e.g., Unicode symbol)
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        /// <summary>
        /// Icon position: Left, Right, Top, Bottom
        /// </summary>
        public string IconPosition
        {
            get => _iconPosition;
            set => SetProperty(ref _iconPosition, value);
        }

        public override LayoutElementBase Clone()
        {
            return new ButtonElement
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
                Text = Text,
                FontFamily = FontFamily,
                FontSize = FontSize,
                IsBold = IsBold,
                IsItalic = IsItalic,
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness,
                CornerRadius = CornerRadius,
                HoverBackgroundColor = HoverBackgroundColor,
                PressedBackgroundColor = PressedBackgroundColor,
                Icon = Icon,
                IconPosition = IconPosition
            };
        }
    }
}
