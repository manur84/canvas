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
                VerticalAlignment = VerticalAlignment
            };
        }
    }
}
