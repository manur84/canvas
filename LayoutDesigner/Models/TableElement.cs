using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Represents a table element for displaying structured data
    /// </summary>
    public class TableElement : LayoutElementBase
    {
        private int _rows = 3;
        private int _columns = 3;
        private string _headerBackgroundColor = "#FF0078D7";
        private string _headerForegroundColor = "#FFFFFFFF";
        private string _rowBackgroundColor = "#FFFFFFFF";
        private string _alternateRowBackgroundColor = "#FFF0F0F0";
        private string _foregroundColor = "#FF000000";
        private string _borderColor = "#FFCCCCCC";
        private double _borderThickness = 1;
        private string _fontFamily = "Segoe UI";
        private double _fontSize = 14;
        private double _headerFontSize = 14;
        private bool _headerBold = true;
        private bool _showHeader = true;
        private bool _showBorder = true;
        private bool _alternateRows = true;

        public TableElement()
        {
            ElementType = "Table";
            Width = 400;
            Height = 200;
        }

        /// <summary>
        /// Number of rows
        /// </summary>
        public int Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }

        /// <summary>
        /// Number of columns
        /// </summary>
        public int Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        /// <summary>
        /// Header background color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string HeaderBackgroundColor
        {
            get => _headerBackgroundColor;
            set => SetProperty(ref _headerBackgroundColor, value);
        }

        /// <summary>
        /// Header text color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string HeaderForegroundColor
        {
            get => _headerForegroundColor;
            set => SetProperty(ref _headerForegroundColor, value);
        }

        /// <summary>
        /// Row background color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string RowBackgroundColor
        {
            get => _rowBackgroundColor;
            set => SetProperty(ref _rowBackgroundColor, value);
        }

        /// <summary>
        /// Alternate row background color (#RRGGBB or #AARRGGBB)
        /// </summary>
        public string AlternateRowBackgroundColor
        {
            get => _alternateRowBackgroundColor;
            set => SetProperty(ref _alternateRowBackgroundColor, value);
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
        /// Font family
        /// </summary>
        public string FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        /// <summary>
        /// Font size for content
        /// </summary>
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        /// <summary>
        /// Font size for header
        /// </summary>
        public double HeaderFontSize
        {
            get => _headerFontSize;
            set => SetProperty(ref _headerFontSize, value);
        }

        /// <summary>
        /// Bold header text
        /// </summary>
        public bool HeaderBold
        {
            get => _headerBold;
            set => SetProperty(ref _headerBold, value);
        }

        /// <summary>
        /// Show header row
        /// </summary>
        public bool ShowHeader
        {
            get => _showHeader;
            set => SetProperty(ref _showHeader, value);
        }

        /// <summary>
        /// Show table borders
        /// </summary>
        public bool ShowBorder
        {
            get => _showBorder;
            set => SetProperty(ref _showBorder, value);
        }

        /// <summary>
        /// Alternate row colors
        /// </summary>
        public bool AlternateRows
        {
            get => _alternateRows;
            set => SetProperty(ref _alternateRows, value);
        }

        public override LayoutElementBase Clone()
        {
            return new TableElement
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
                Rows = Rows,
                Columns = Columns,
                HeaderBackgroundColor = HeaderBackgroundColor,
                HeaderForegroundColor = HeaderForegroundColor,
                RowBackgroundColor = RowBackgroundColor,
                AlternateRowBackgroundColor = AlternateRowBackgroundColor,
                ForegroundColor = ForegroundColor,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness,
                FontFamily = FontFamily,
                FontSize = FontSize,
                HeaderFontSize = HeaderFontSize,
                HeaderBold = HeaderBold,
                ShowHeader = ShowHeader,
                ShowBorder = ShowBorder,
                AlternateRows = AlternateRows
            };
        }
    }
}
