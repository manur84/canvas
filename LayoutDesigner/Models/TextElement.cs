using System;
using LayoutDesigner.Models.Base;
using System.Windows;
using System.Windows.Media;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Text element with font, color, and alignment properties
    /// </summary>
    public class TextElement : LayoutElementBase
    {
        private string _text = "Text";
        private string _fontFamily = "Arial";
        private double _fontSize = 16;
        private bool _isBold;
        private bool _isItalic;
        private bool _isUnderline;
        private string _foregroundColor = "#FF000000";
        private string _backgroundColor = "#00FFFFFF";
        private TextAlignment _textAlignment = TextAlignment.Left;
        private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
        private double _lineHeight = 1.2;
        private double _letterSpacing = 0;
        private bool _hasShadow = false;
        private string _shadowColor = "#80000000";
        private double _shadowBlur = 5;
        private double _shadowOffsetX = 2;
        private double _shadowOffsetY = 2;
        private bool _hasBorder = false;
        private string _borderColor = "#FF000000";
        private double _borderThickness = 1;
        private double _padding = 4;
        private double _cornerRadius = 0;

        public override string ElementType => "Text";

        /// <summary>
        /// The text content
        /// </summary>
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        /// <summary>
        /// Font family name
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
        /// Underlined text
        /// </summary>
        public bool IsUnderline
        {
            get => _isUnderline;
            set => SetProperty(ref _isUnderline, value);
        }

        /// <summary>
        /// Foreground color (text color) as hex string
        /// </summary>
        public string ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, value);
        }

        /// <summary>
        /// Background color as hex string
        /// </summary>
        public string BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        /// <summary>
        /// Horizontal text alignment
        /// </summary>
        public TextAlignment TextAlignment
        {
            get => _textAlignment;
            set => SetProperty(ref _textAlignment, value);
        }

        /// <summary>
        /// Vertical text alignment
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set => SetProperty(ref _verticalAlignment, value);
        }

        /// <summary>
        /// Line height multiplier (1.0 = normal, 1.5 = 150%, etc.)
        /// </summary>
        public double LineHeight
        {
            get => _lineHeight;
            set => SetProperty(ref _lineHeight, value);
        }

        /// <summary>
        /// Letter spacing in pixels
        /// </summary>
        public double LetterSpacing
        {
            get => _letterSpacing;
            set => SetProperty(ref _letterSpacing, value);
        }

        /// <summary>
        /// Enable text shadow
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
        /// Internal padding in pixels
        /// </summary>
        public double Padding
        {
            get => _padding;
            set => SetProperty(ref _padding, value);
        }

        /// <summary>
        /// Corner radius for rounded borders
        /// </summary>
        public double CornerRadius
        {
            get => _cornerRadius;
            set => SetProperty(ref _cornerRadius, value);
        }

        public override LayoutElementBase Clone()
        {
            return new TextElement
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
                Text = Text,
                FontFamily = FontFamily,
                FontSize = FontSize,
                IsBold = IsBold,
                IsItalic = IsItalic,
                IsUnderline = IsUnderline,
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                TextAlignment = TextAlignment,
                VerticalAlignment = VerticalAlignment,
                LineHeight = LineHeight,
                LetterSpacing = LetterSpacing,
                HasShadow = HasShadow,
                ShadowColor = ShadowColor,
                ShadowBlur = ShadowBlur,
                ShadowOffsetX = ShadowOffsetX,
                ShadowOffsetY = ShadowOffsetY,
                HasBorder = HasBorder,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness,
                Padding = Padding,
                CornerRadius = CornerRadius
            };
        }
    }
}
