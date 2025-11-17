using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Premium quality SVG export service
    /// Exports layouts to scalable vector graphics with high fidelity
    /// </summary>
    public class SvgExportService : ISvgExportService
    {
        private readonly IErrorHandlingService _errorHandlingService;

        public SvgExportService(IErrorHandlingService errorHandlingService)
        {
            _errorHandlingService = errorHandlingService;
        }

        public async Task<bool> ExportToSvgAsync(LayoutDocument document, string filePath)
        {
            try
            {
                var svgContent = GenerateSvgContent(document);
                await File.WriteAllTextAsync(filePath, svgContent, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleError(ex, "Failed to export SVG", showDialog: true);
                return false;
            }
        }

        public string GenerateSvgContent(LayoutDocument document)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();

                // SVG root element
                writer.WriteStartElement("svg", "http://www.w3.org/2000/svg");
                writer.WriteAttributeString("width", FormatNumber(document.Width));
                writer.WriteAttributeString("height", FormatNumber(document.Height));
                writer.WriteAttributeString("viewBox", $"0 0 {FormatNumber(document.Width)} {FormatNumber(document.Height)}");
                writer.WriteAttributeString("version", "1.1");

                // Premium quality rendering hints
                writer.WriteAttributeString("shape-rendering", "geometricPrecision");
                writer.WriteAttributeString("text-rendering", "geometricPrecision");
                writer.WriteAttributeString("image-rendering", "optimizeQuality");

                // Background
                if (!string.IsNullOrEmpty(document.BackgroundColor))
                {
                    writer.WriteStartElement("rect");
                    writer.WriteAttributeString("width", "100%");
                    writer.WriteAttributeString("height", "100%");
                    writer.WriteAttributeString("fill", document.BackgroundColor);
                    writer.WriteEndElement();
                }

                // Export elements sorted by Z-Index
                var sortedElements = document.Elements.OrderBy(e => e.ZIndex).ToList();

                foreach (var element in sortedElements)
                {
                    if (!element.IsVisible) continue;

                    ExportElement(writer, element);
                }

                writer.WriteEndElement(); // svg
                writer.WriteEndDocument();
            }

            return sb.ToString();
        }

        private void ExportElement(XmlWriter writer, LayoutElementBase element)
        {
            // Apply transform for position, rotation, and opacity
            writer.WriteStartElement("g");

            var transforms = new StringBuilder();
            transforms.Append($"translate({FormatNumber(element.X)},{FormatNumber(element.Y)})");

            if (Math.Abs(element.Rotation) > 0.001)
            {
                // Rotate around center of element
                var centerX = element.Width / 2;
                var centerY = element.Height / 2;
                transforms.Append($" rotate({FormatNumber(element.Rotation)},{FormatNumber(centerX)},{FormatNumber(centerY)})");
            }

            writer.WriteAttributeString("transform", transforms.ToString());

            if (Math.Abs(element.Opacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("opacity", FormatNumber(element.Opacity));
            }

            // Export based on element type
            switch (element)
            {
                case TextElement textElement:
                    ExportTextElement(writer, textElement);
                    break;
                case ImageElement imageElement:
                    ExportImageElement(writer, imageElement);
                    break;
                case ShapeElement shapeElement:
                    ExportShapeElement(writer, shapeElement);
                    break;
                case LineElement lineElement:
                    ExportLineElement(writer, lineElement);
                    break;
                case QrCodeElement qrCodeElement:
                    ExportQrCodeElement(writer, qrCodeElement);
                    break;
                case TableElement tableElement:
                    ExportTableElement(writer, tableElement);
                    break;
                case ButtonElement buttonElement:
                    ExportButtonElement(writer, buttonElement);
                    break;
            }

            writer.WriteEndElement(); // g
        }

        private void ExportTextElement(XmlWriter writer, TextElement element)
        {
            // Background rect
            if (!string.IsNullOrEmpty(element.BackgroundColor))
            {
                writer.WriteStartElement("rect");
                writer.WriteAttributeString("width", FormatNumber(element.Width));
                writer.WriteAttributeString("height", FormatNumber(element.Height));
                writer.WriteAttributeString("fill", element.BackgroundColor);
                writer.WriteAttributeString("rx", FormatNumber(element.CornerRadius));
                writer.WriteEndElement();
            }

            // Text
            writer.WriteStartElement("text");
            writer.WriteAttributeString("x", FormatNumber(element.Width / 2));
            writer.WriteAttributeString("y", FormatNumber(element.Height / 2));
            writer.WriteAttributeString("font-family", element.FontFamily);
            writer.WriteAttributeString("font-size", FormatNumber(element.FontSize));
            writer.WriteAttributeString("fill", element.ForegroundColor);
            writer.WriteAttributeString("text-anchor", "middle");
            writer.WriteAttributeString("dominant-baseline", "middle");

            if (element.IsBold)
                writer.WriteAttributeString("font-weight", "bold");
            if (element.IsItalic)
                writer.WriteAttributeString("font-style", "italic");
            if (element.IsUnderline)
                writer.WriteAttributeString("text-decoration", "underline");

            writer.WriteString(element.Text);
            writer.WriteEndElement();
        }

        private void ExportImageElement(XmlWriter writer, ImageElement element)
        {
            writer.WriteStartElement("image");
            writer.WriteAttributeString("width", FormatNumber(element.Width));
            writer.WriteAttributeString("height", FormatNumber(element.Height));

            // Use embedded data if available, otherwise path
            if (!string.IsNullOrEmpty(element.ImageData))
            {
                writer.WriteAttributeString("href", $"data:image/png;base64,{element.ImageData}");
            }
            else if (!string.IsNullOrEmpty(element.ImagePath))
            {
                writer.WriteAttributeString("href", element.ImagePath);
            }

            writer.WriteAttributeString("preserveAspectRatio", element.MaintainAspectRatio ? "xMidYMid meet" : "none");
            writer.WriteEndElement();
        }

        private void ExportShapeElement(XmlWriter writer, ShapeElement element)
        {
            switch (element.ShapeType)
            {
                case ShapeType.Rectangle:
                case ShapeType.RoundedRectangle:
                    writer.WriteStartElement("rect");
                    writer.WriteAttributeString("width", FormatNumber(element.Width));
                    writer.WriteAttributeString("height", FormatNumber(element.Height));
                    writer.WriteAttributeString("fill", element.FillColor);
                    writer.WriteAttributeString("stroke", element.StrokeColor);
                    writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));
                    if (element.ShapeType == ShapeType.RoundedRectangle)
                    {
                        writer.WriteAttributeString("rx", FormatNumber(element.CornerRadius));
                        writer.WriteAttributeString("ry", FormatNumber(element.CornerRadius));
                    }
                    writer.WriteEndElement();
                    break;

                case ShapeType.Ellipse:
                    writer.WriteStartElement("ellipse");
                    writer.WriteAttributeString("cx", FormatNumber(element.Width / 2));
                    writer.WriteAttributeString("cy", FormatNumber(element.Height / 2));
                    writer.WriteAttributeString("rx", FormatNumber(element.Width / 2));
                    writer.WriteAttributeString("ry", FormatNumber(element.Height / 2));
                    writer.WriteAttributeString("fill", element.FillColor);
                    writer.WriteAttributeString("stroke", element.StrokeColor);
                    writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));
                    writer.WriteEndElement();
                    break;

                case ShapeType.Line:
                    writer.WriteStartElement("line");
                    writer.WriteAttributeString("x1", "0");
                    writer.WriteAttributeString("y1", "0");
                    writer.WriteAttributeString("x2", FormatNumber(element.Width));
                    writer.WriteAttributeString("y2", FormatNumber(element.Height));
                    writer.WriteAttributeString("stroke", element.StrokeColor);
                    writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));
                    writer.WriteAttributeString("stroke-linecap", "round");
                    writer.WriteEndElement();
                    break;
            }
        }

        private void ExportLineElement(XmlWriter writer, LineElement element)
        {
            writer.WriteStartElement("line");
            writer.WriteAttributeString("x1", "0");
            writer.WriteAttributeString("y1", "0");
            writer.WriteAttributeString("x2", FormatNumber(element.X2));
            writer.WriteAttributeString("y2", FormatNumber(element.Y2));
            writer.WriteAttributeString("stroke", element.StrokeColor);
            writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));
            writer.WriteAttributeString("stroke-linecap", "round");
            writer.WriteEndElement();
        }

        private void ExportQrCodeElement(XmlWriter writer, QrCodeElement element)
        {
            // QR Code as placeholder rectangle
            writer.WriteStartElement("rect");
            writer.WriteAttributeString("width", FormatNumber(element.Width));
            writer.WriteAttributeString("height", FormatNumber(element.Height));
            writer.WriteAttributeString("fill", element.BackgroundColor);
            writer.WriteEndElement();

            // Add text label
            writer.WriteStartElement("text");
            writer.WriteAttributeString("x", FormatNumber(element.Width / 2));
            writer.WriteAttributeString("y", FormatNumber(element.Height / 2));
            writer.WriteAttributeString("text-anchor", "middle");
            writer.WriteAttributeString("dominant-baseline", "middle");
            writer.WriteAttributeString("font-size", "12");
            writer.WriteString("[QR Code]");
            writer.WriteEndElement();
        }

        private void ExportTableElement(XmlWriter writer, TableElement element)
        {
            var cellWidth = element.Width / element.Columns;
            var cellHeight = element.Height / element.Rows;

            // Table border
            if (element.ShowBorder)
            {
                writer.WriteStartElement("rect");
                writer.WriteAttributeString("width", FormatNumber(element.Width));
                writer.WriteAttributeString("height", FormatNumber(element.Height));
                writer.WriteAttributeString("fill", "none");
                writer.WriteAttributeString("stroke", element.BorderColor);
                writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));
                writer.WriteEndElement();
            }

            // Draw cells
            for (int row = 0; row < element.Rows; row++)
            {
                for (int col = 0; col < element.Columns; col++)
                {
                    var x = col * cellWidth;
                    var y = row * cellHeight;

                    // Determine cell background
                    string bgColor = element.RowBackgroundColor;
                    string textColor = element.ForegroundColor;
                    double fontSize = element.FontSize;
                    bool isBold = false;

                    if (row == 0 && element.ShowHeader)
                    {
                        bgColor = element.HeaderBackgroundColor;
                        textColor = element.HeaderForegroundColor;
                        fontSize = element.HeaderFontSize;
                        isBold = element.HeaderBold;
                    }
                    else if (element.AlternateRows && row % 2 == 1)
                    {
                        bgColor = element.AlternateRowBackgroundColor;
                    }

                    // Cell background
                    writer.WriteStartElement("rect");
                    writer.WriteAttributeString("x", FormatNumber(x));
                    writer.WriteAttributeString("y", FormatNumber(y));
                    writer.WriteAttributeString("width", FormatNumber(cellWidth));
                    writer.WriteAttributeString("height", FormatNumber(cellHeight));
                    writer.WriteAttributeString("fill", bgColor);

                    if (element.ShowBorder)
                    {
                        writer.WriteAttributeString("stroke", element.BorderColor);
                        writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));
                    }
                    writer.WriteEndElement();

                    // Cell text
                    if (row < element.Cells.Count && col < element.Cells[row].Count)
                    {
                        var cellValue = element.Cells[row][col].Value;

                        writer.WriteStartElement("text");
                        writer.WriteAttributeString("x", FormatNumber(x + cellWidth / 2));
                        writer.WriteAttributeString("y", FormatNumber(y + cellHeight / 2));
                        writer.WriteAttributeString("text-anchor", "middle");
                        writer.WriteAttributeString("dominant-baseline", "middle");
                        writer.WriteAttributeString("font-family", element.FontFamily);
                        writer.WriteAttributeString("font-size", FormatNumber(fontSize));
                        writer.WriteAttributeString("fill", textColor);

                        if (isBold)
                            writer.WriteAttributeString("font-weight", "bold");

                        writer.WriteString(cellValue ?? "");
                        writer.WriteEndElement();
                    }
                }
            }
        }

        private void ExportButtonElement(XmlWriter writer, ButtonElement element)
        {
            // Button background
            writer.WriteStartElement("rect");
            writer.WriteAttributeString("width", FormatNumber(element.Width));
            writer.WriteAttributeString("height", FormatNumber(element.Height));
            writer.WriteAttributeString("fill", element.BackgroundColor);
            writer.WriteAttributeString("stroke", element.BorderColor);
            writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));
            writer.WriteAttributeString("rx", FormatNumber(element.CornerRadius));
            writer.WriteEndElement();

            // Button text
            writer.WriteStartElement("text");
            writer.WriteAttributeString("x", FormatNumber(element.Width / 2));
            writer.WriteAttributeString("y", FormatNumber(element.Height / 2));
            writer.WriteAttributeString("font-family", element.FontFamily);
            writer.WriteAttributeString("font-size", FormatNumber(element.FontSize));
            writer.WriteAttributeString("fill", element.ForegroundColor);
            writer.WriteAttributeString("text-anchor", "middle");
            writer.WriteAttributeString("dominant-baseline", "middle");

            if (element.IsBold)
                writer.WriteAttributeString("font-weight", "bold");
            if (element.IsItalic)
                writer.WriteAttributeString("font-style", "italic");

            writer.WriteString(element.Text);
            writer.WriteEndElement();
        }

        private string FormatNumber(double value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
