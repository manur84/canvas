using System;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// QR Code element that generates a QR code from text
    /// </summary>
    public class QrCodeElement : LayoutElementBase
    {
        private string _content = "https://example.com";
        private string _foregroundColor = "#FF000000";
        private string _backgroundColor = "#FFFFFFFF";
        private int _errorCorrectionLevel = 1; // 0=Low, 1=Medium, 2=Quartile, 3=High

        public override string ElementType => "QrCode";

        /// <summary>
        /// Content to encode in the QR code
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        /// <summary>
        /// Foreground color (QR code color) as hex string
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
        /// Error correction level (0-3: Low, Medium, Quartile, High)
        /// </summary>
        public int ErrorCorrectionLevel
        {
            get => _errorCorrectionLevel;
            set => SetProperty(ref _errorCorrectionLevel, Math.Clamp(value, 0, 3));
        }

        public override LayoutElementBase Clone()
        {
            return new QrCodeElement
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
                Content = Content,
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                ErrorCorrectionLevel = ErrorCorrectionLevel
            };
        }
    }
}
