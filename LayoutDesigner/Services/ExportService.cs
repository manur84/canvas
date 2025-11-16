using LayoutDesigner.Services.Interfaces;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Service for exporting layouts to image files
    /// </summary>
    public class ExportService : IExportService
    {
        public async Task<bool> ExportToPngAsync(UIElement element, string filePath, double width, double height)
        {
            try
            {
                var bitmap = RenderElement(element, width, height);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                await using var stream = new FileStream(filePath, FileMode.Create);
                encoder.Save(stream);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to PNG: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ExportToJpgAsync(UIElement element, string filePath, double width, double height, int quality = 95)
        {
            try
            {
                var bitmap = RenderElement(element, width, height);

                var encoder = new JpegBitmapEncoder
                {
                    QualityLevel = quality
                };
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                await using var stream = new FileStream(filePath, FileMode.Create);
                encoder.Save(stream);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to JPG: {ex.Message}");
                return false;
            }
        }

        private RenderTargetBitmap RenderElement(UIElement element, double width, double height)
        {
            var size = new Size(width, height);
            element.Measure(size);
            element.Arrange(new Rect(size));

            var renderBitmap = new RenderTargetBitmap(
                (int)width,
                (int)height,
                96,
                96,
                PixelFormats.Pbgra32);

            renderBitmap.Render(element);

            return renderBitmap;
        }
    }
}
