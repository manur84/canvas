using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// Control for rendering grid lines on the canvas
    /// </summary>
    public class GridLines : Control
    {
        static GridLines()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridLines),
                new FrameworkPropertyMetadata(typeof(GridLines)));
        }

        #region Dependency Properties

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(GridLines),
                new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GridColorProperty =
            DependencyProperty.Register(nameof(GridColor), typeof(Brush), typeof(GridLines),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowGridProperty =
            DependencyProperty.Register(nameof(ShowGrid), typeof(bool), typeof(GridLines),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public double GridSize
        {
            get => (double)GetValue(GridSizeProperty);
            set => SetValue(GridSizeProperty, value);
        }

        public Brush GridColor
        {
            get => (Brush)GetValue(GridColorProperty);
            set => SetValue(GridColorProperty, value);
        }

        public bool ShowGrid
        {
            get => (bool)GetValue(ShowGridProperty);
            set => SetValue(ShowGridProperty, value);
        }

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (!ShowGrid || GridSize <= 0)
                return;

            var pen = new Pen(GridColor, 1);
            pen.Freeze();

            double width = ActualWidth;
            double height = ActualHeight;

            // Vertical lines
            for (double x = 0; x < width; x += GridSize)
            {
                dc.DrawLine(pen, new Point(x, 0), new Point(x, height));
            }

            // Horizontal lines
            for (double y = 0; y < height; y += GridSize)
            {
                dc.DrawLine(pen, new Point(0, y), new Point(width, y));
            }
        }
    }
}
