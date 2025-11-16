using Microsoft.Win32;

namespace LayoutDesigner.Helpers
{
    /// <summary>
    /// Helper for file dialogs
    /// </summary>
    public static class FileDialogHelper
    {
        private const string LayoutFilter = "Layout Files (*.layout)|*.layout|All Files (*.*)|*.*";
        private const string ImageFilter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|PNG Files (*.png)|*.png|JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All Files (*.*)|*.*";
        private const string ExportImageFilter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg";

        /// <summary>
        /// Shows a dialog to open a layout file
        /// </summary>
        public static string? ShowOpenLayoutDialog()
        {
            var dialog = new OpenFileDialog
            {
                Filter = LayoutFilter,
                Title = "Open Layout"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        /// <summary>
        /// Shows a dialog to save a layout file
        /// </summary>
        public static string? ShowSaveLayoutDialog(string? defaultFileName = null)
        {
            var dialog = new SaveFileDialog
            {
                Filter = LayoutFilter,
                Title = "Save Layout",
                DefaultExt = ".layout",
                FileName = defaultFileName ?? "Untitled"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        /// <summary>
        /// Shows a dialog to select an image file
        /// </summary>
        public static string? ShowOpenImageDialog()
        {
            var dialog = new OpenFileDialog
            {
                Filter = ImageFilter,
                Title = "Select Image"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        /// <summary>
        /// Shows a dialog to export to image
        /// </summary>
        public static string? ShowExportImageDialog(string defaultFileName = "Export")
        {
            var dialog = new SaveFileDialog
            {
                Filter = ExportImageFilter,
                Title = "Export Image",
                FileName = defaultFileName
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
