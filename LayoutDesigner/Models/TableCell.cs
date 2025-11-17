using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Represents a single cell in a table with Excel-like properties
    /// </summary>
    public class TableCell : INotifyPropertyChanged
    {
        private string _value = "";
        private string _formula = "";
        private string _backgroundColor = "#FFFFFFFF";
        private string _foregroundColor = "#FF000000";
        private string _fontFamily = "Segoe UI";
        private double _fontSize = 12;
        private bool _isBold = false;
        private bool _isItalic = false;
        private bool _isUnderline = false;
        private string _horizontalAlignment = "Left"; // Left, Center, Right
        private string _verticalAlignment = "Center"; // Top, Center, Bottom
        private string _borderColor = "#FFCCCCCC";
        private double _borderThickness = 1;
        private string _format = ""; // Number format (e.g., "0.00", "0%", etc.)
        private bool _isReadOnly = false;
        private int _rowSpan = 1;
        private int _columnSpan = 1;

        /// <summary>
        /// Cell value (displayed value)
        /// </summary>
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        /// <summary>
        /// Cell formula (e.g., "=SUM(A1:A10)")
        /// </summary>
        public string Formula
        {
            get => _formula;
            set => SetProperty(ref _formula, value);
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
        /// Text color as hex string
        /// </summary>
        public string ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, value);
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
        /// Font size
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
        /// Underline text
        /// </summary>
        public bool IsUnderline
        {
            get => _isUnderline;
            set => SetProperty(ref _isUnderline, value);
        }

        /// <summary>
        /// Horizontal alignment (Left, Center, Right)
        /// </summary>
        public string HorizontalAlignment
        {
            get => _horizontalAlignment;
            set => SetProperty(ref _horizontalAlignment, value);
        }

        /// <summary>
        /// Vertical alignment (Top, Center, Bottom)
        /// </summary>
        public string VerticalAlignment
        {
            get => _verticalAlignment;
            set => SetProperty(ref _verticalAlignment, value);
        }

        /// <summary>
        /// Border color as hex string
        /// </summary>
        public string BorderColor
        {
            get => _borderColor;
            set => SetProperty(ref _borderColor, value);
        }

        /// <summary>
        /// Border thickness
        /// </summary>
        public double BorderThickness
        {
            get => _borderThickness;
            set => SetProperty(ref _borderThickness, value);
        }

        /// <summary>
        /// Number format string (e.g., "0.00", "0%", "#,##0", etc.)
        /// </summary>
        public string Format
        {
            get => _format;
            set => SetProperty(ref _format, value);
        }

        /// <summary>
        /// Whether the cell is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        /// <summary>
        /// Number of rows this cell spans (for merged cells)
        /// </summary>
        public int RowSpan
        {
            get => _rowSpan;
            set => SetProperty(ref _rowSpan, value);
        }

        /// <summary>
        /// Number of columns this cell spans (for merged cells)
        /// </summary>
        public int ColumnSpan
        {
            get => _columnSpan;
            set => SetProperty(ref _columnSpan, value);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        public TableCell Clone()
        {
            return new TableCell
            {
                Value = Value,
                Formula = Formula,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                FontFamily = FontFamily,
                FontSize = FontSize,
                IsBold = IsBold,
                IsItalic = IsItalic,
                IsUnderline = IsUnderline,
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness,
                Format = Format,
                IsReadOnly = IsReadOnly,
                RowSpan = RowSpan,
                ColumnSpan = ColumnSpan
            };
        }
    }
}
