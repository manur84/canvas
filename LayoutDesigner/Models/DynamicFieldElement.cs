using System;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Dynamic field element for date/time and other runtime values
    /// </summary>
    public class DynamicFieldElement : LayoutElementBase
    {
        private DynamicFieldType _fieldType = DynamicFieldType.DateTime;
        private string _format = "dd.MM.yyyy HH:mm:ss";
        private string _fontFamily = "Arial";
        private double _fontSize = 16;
        private bool _isBold;
        private bool _isItalic;
        private string _foregroundColor = "#FF000000";
        private string _backgroundColor = "#00FFFFFF";

        public override string ElementType => "DynamicField";

        /// <summary>
        /// Type of dynamic field
        /// </summary>
        public DynamicFieldType FieldType
        {
            get => _fieldType;
            set => SetProperty(ref _fieldType, value);
        }

        /// <summary>
        /// Format string (e.g., for DateTime: "dd.MM.yyyy HH:mm:ss")
        /// </summary>
        public string Format
        {
            get => _format;
            set => SetProperty(ref _format, value);
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
        /// Foreground color as hex string
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

        public override LayoutElementBase Clone()
        {
            return new DynamicFieldElement
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
                FieldType = FieldType,
                Format = Format,
                FontFamily = FontFamily,
                FontSize = FontSize,
                IsBold = IsBold,
                IsItalic = IsItalic,
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor
            };
        }
    }

    /// <summary>
    /// Types of dynamic fields
    /// </summary>
    public enum DynamicFieldType
    {
        DateTime,
        Date,
        Time,
        Custom
    }
}
