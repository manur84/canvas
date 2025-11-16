using System.Windows;
using LayoutDesigner.ViewModels;

namespace LayoutDesigner.Views
{
    /// <summary>
    /// Color Picker Dialog
    /// </summary>
    public partial class ColorPickerDialog : Window
    {
        public ColorPickerDialog(string initialColor)
        {
            InitializeComponent();

            var viewModel = new ColorPickerViewModel(initialColor);
            DataContext = viewModel;
        }

        /// <summary>
        /// Gets the selected color as hex string
        /// </summary>
        public string SelectedColor => ((ColorPickerViewModel)DataContext).HexColor;

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Shows the color picker dialog and returns the selected color
        /// </summary>
        public static string? ShowDialog(Window owner, string initialColor)
        {
            var dialog = new ColorPickerDialog(initialColor)
            {
                Owner = owner
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedColor;
            }

            return null;
        }
    }
}
