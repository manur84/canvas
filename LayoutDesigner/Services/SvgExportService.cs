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
        private readonly IQrCodeService _qrCodeService;

        public SvgExportService(IErrorHandlingService errorHandlingService, IQrCodeService qrCodeService)
        {
            _errorHandlingService = errorHandlingService;
            _qrCodeService = qrCodeService;
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
                writer.WriteAttributeString("width", FormatNumber(document.CanvasWidth));
                writer.WriteAttributeString("height", FormatNumber(document.CanvasHeight));
                writer.WriteAttributeString("viewBox", $"0 0 {FormatNumber(document.CanvasWidth)} {FormatNumber(document.CanvasHeight)}");
                writer.WriteAttributeString("version", "1.1");

                // Premium quality rendering hints
                writer.WriteAttributeString("shape-rendering", "geometricPrecision");
                writer.WriteAttributeString("text-rendering", "geometricPrecision");
                writer.WriteAttributeString("image-rendering", "optimizeQuality");

                // Definitions section for filters, gradients, and markers
                writer.WriteStartElement("defs");
                int filterId = 0;
                int gradientId = 0;

                // Pre-create filters and gradients for all elements
                var sortedElements = document.Elements.OrderBy(e => e.ZIndex).ToList();
                foreach (var element in sortedElements)
                {
                    if (!element.IsVisible) continue;

                    switch (element)
                    {
                        case TextElement textElement:
                            if (textElement.HasShadow)
                            {
                                textElement.Name = textElement.Name ?? "text";
                                CreateShadowFilter(writer, $"shadow_{element.Id}", textElement.ShadowColor,
                                    textElement.ShadowBlur, textElement.ShadowOffsetX, textElement.ShadowOffsetY);
                            }
                            break;
                        case ShapeElement shapeElement:
                            if (shapeElement.UseGradient)
                            {
                                CreateGradient(writer, $"gradient_{element.Id}", shapeElement.GradientStartColor,
                                    shapeElement.GradientEndColor, shapeElement.GradientDirection, element.Width, element.Height);
                            }
                            if (shapeElement.HasShadow)
                            {
                                CreateShadowFilter(writer, $"shadow_{element.Id}", shapeElement.ShadowColor,
                                    shapeElement.ShadowBlur, shapeElement.ShadowOffsetX, shapeElement.ShadowOffsetY);
                            }
                            break;
                        case ImageElement imageElement:
                            if (imageElement.HasShadow || imageElement.Blur > 0 || imageElement.Grayscale ||
                                Math.Abs(imageElement.Brightness - 1.0) > 0.001 ||
                                Math.Abs(imageElement.Contrast - 1.0) > 0.001 ||
                                Math.Abs(imageElement.Saturation - 1.0) > 0.001)
                            {
                                CreateImageFilter(writer, $"filter_{element.Id}", imageElement);
                            }
                            if (imageElement.CornerRadius > 0)
                            {
                                CreateRoundedClipPath(writer, $"clip_{element.Id}", element.Width, element.Height, imageElement.CornerRadius);
                            }
                            break;
                    }
                }

                // Add arrow markers for lines
                CreateArrowMarker(writer, "arrowStart", "start");
                CreateArrowMarker(writer, "arrowEnd", "end");
                CreateCircleMarker(writer, "circleMarker");
                CreateSquareMarker(writer, "squareMarker");

                writer.WriteEndElement(); // defs

                // Background
                if (!string.IsNullOrEmpty(document.BackgroundColor))
                {
                    var bgOpacity = GetColorOpacity(document.BackgroundColor);

                    // Only render background if not fully transparent
                    if (bgOpacity > 0.001)
                    {
                        writer.WriteStartElement("rect");
                        writer.WriteAttributeString("width", "100%");
                        writer.WriteAttributeString("height", "100%");
                        writer.WriteAttributeString("fill", ConvertColorToSvg(document.BackgroundColor));

                        if (Math.Abs(bgOpacity - 1.0) > 0.001)
                        {
                            writer.WriteAttributeString("fill-opacity", FormatNumber(bgOpacity));
                        }

                        writer.WriteEndElement();
                    }
                }

                // Export elements sorted by Z-Index
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
            // Background rect with border if enabled
            var bgOpacity = GetColorOpacity(element.BackgroundColor);
            bool hasVisibleBackground = !string.IsNullOrEmpty(element.BackgroundColor) && bgOpacity > 0.001;

            if (hasVisibleBackground || element.HasBorder)
            {
                writer.WriteStartElement("rect");
                writer.WriteAttributeString("width", FormatNumber(element.Width));
                writer.WriteAttributeString("height", FormatNumber(element.Height));
                writer.WriteAttributeString("fill", hasVisibleBackground ? ConvertColorToSvg(element.BackgroundColor) : "none");
                writer.WriteAttributeString("rx", FormatNumber(element.CornerRadius));

                if (hasVisibleBackground && Math.Abs(bgOpacity - 1.0) > 0.001)
                {
                    writer.WriteAttributeString("fill-opacity", FormatNumber(bgOpacity));
                }

                if (element.HasBorder)
                {
                    writer.WriteAttributeString("stroke", ConvertColorToSvg(element.BorderColor));
                    writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));

                    var borderOpacity = GetColorOpacity(element.BorderColor);
                    if (Math.Abs(borderOpacity - 1.0) > 0.001)
                    {
                        writer.WriteAttributeString("stroke-opacity", FormatNumber(borderOpacity));
                    }
                }

                writer.WriteEndElement();
            }

            // Calculate text position based on alignment
            double textX = element.Padding;
            string textAnchor = "start";

            switch (element.TextAlignment)
            {
                case System.Windows.TextAlignment.Left:
                    textX = element.Padding;
                    textAnchor = "start";
                    break;
                case System.Windows.TextAlignment.Center:
                    textX = element.Width / 2;
                    textAnchor = "middle";
                    break;
                case System.Windows.TextAlignment.Right:
                    textX = element.Width - element.Padding;
                    textAnchor = "end";
                    break;
            }

            double textY = element.Padding;
            string dominantBaseline = "text-before-edge";

            switch (element.VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Top:
                    textY = element.Padding + element.FontSize * 0.8;
                    dominantBaseline = "text-before-edge";
                    break;
                case System.Windows.VerticalAlignment.Center:
                    textY = element.Height / 2;
                    dominantBaseline = "middle";
                    break;
                case System.Windows.VerticalAlignment.Bottom:
                    textY = element.Height - element.Padding;
                    dominantBaseline = "text-after-edge";
                    break;
            }

            // Text with all styling
            writer.WriteStartElement("text");
            writer.WriteAttributeString("x", FormatNumber(textX));
            writer.WriteAttributeString("y", FormatNumber(textY));
            writer.WriteAttributeString("font-family", element.FontFamily);
            writer.WriteAttributeString("font-size", FormatNumber(element.FontSize));
            writer.WriteAttributeString("fill", ConvertColorToSvg(element.ForegroundColor));
            writer.WriteAttributeString("text-anchor", textAnchor);
            writer.WriteAttributeString("dominant-baseline", dominantBaseline);

            var textOpacity = GetColorOpacity(element.ForegroundColor);
            if (Math.Abs(textOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("fill-opacity", FormatNumber(textOpacity));
            }

            if (element.IsBold)
                writer.WriteAttributeString("font-weight", "bold");
            if (element.IsItalic)
                writer.WriteAttributeString("font-style", "italic");
            if (element.IsUnderline)
                writer.WriteAttributeString("text-decoration", "underline");

            if (Math.Abs(element.LetterSpacing) > 0.001)
                writer.WriteAttributeString("letter-spacing", FormatNumber(element.LetterSpacing));

            if (element.HasShadow)
                writer.WriteAttributeString("filter", $"url(#shadow_{element.Id})");

            writer.WriteString(element.Text);
            writer.WriteEndElement();
        }

        private void ExportImageElement(XmlWriter writer, ImageElement element)
        {
            // Border if enabled
            if (element.HasBorder)
            {
                writer.WriteStartElement("rect");
                writer.WriteAttributeString("width", FormatNumber(element.Width));
                writer.WriteAttributeString("height", FormatNumber(element.Height));
                writer.WriteAttributeString("fill", "none");
                writer.WriteAttributeString("stroke", ConvertColorToSvg(element.BorderColor));
                writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));
                writer.WriteAttributeString("rx", FormatNumber(element.CornerRadius));

                var borderOpacity = GetColorOpacity(element.BorderColor);
                if (Math.Abs(borderOpacity - 1.0) > 0.001)
                {
                    writer.WriteAttributeString("stroke-opacity", FormatNumber(borderOpacity));
                }

                writer.WriteEndElement();
            }

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

            // Apply filter if any image effects are enabled
            if (element.HasShadow || element.Blur > 0 || element.Grayscale ||
                Math.Abs(element.Brightness - 1.0) > 0.001 ||
                Math.Abs(element.Contrast - 1.0) > 0.001 ||
                Math.Abs(element.Saturation - 1.0) > 0.001)
            {
                writer.WriteAttributeString("filter", $"url(#filter_{element.Id})");
            }

            // Apply clip path for rounded corners
            if (element.CornerRadius > 0)
            {
                writer.WriteAttributeString("clip-path", $"url(#clip_{element.Id})");
            }

            writer.WriteEndElement();
        }

        private void ExportShapeElement(XmlWriter writer, ShapeElement element)
        {
            var fillValue = element.UseGradient ? $"url(#gradient_{element.Id})" : ConvertColorToSvg(element.FillColor);
            var fillOpacity = element.UseGradient ? 1.0 : GetColorOpacity(element.FillColor);
            var strokeOpacity = GetColorOpacity(element.StrokeColor);

            switch (element.ShapeType)
            {
                case ShapeType.Rectangle:
                case ShapeType.RoundedRectangle:
                    writer.WriteStartElement("rect");
                    writer.WriteAttributeString("width", FormatNumber(element.Width));
                    writer.WriteAttributeString("height", FormatNumber(element.Height));
                    writer.WriteAttributeString("fill", fillValue);
                    writer.WriteAttributeString("stroke", ConvertColorToSvg(element.StrokeColor));
                    writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));

                    if (!element.UseGradient && Math.Abs(fillOpacity - 1.0) > 0.001)
                    {
                        writer.WriteAttributeString("fill-opacity", FormatNumber(fillOpacity));
                    }

                    if (Math.Abs(strokeOpacity - 1.0) > 0.001)
                    {
                        writer.WriteAttributeString("stroke-opacity", FormatNumber(strokeOpacity));
                    }

                    if (element.ShapeType == ShapeType.RoundedRectangle)
                    {
                        writer.WriteAttributeString("rx", FormatNumber(element.CornerRadius));
                        writer.WriteAttributeString("ry", FormatNumber(element.CornerRadius));
                    }

                    ApplyStrokeDashStyle(writer, element.StrokeDashStyle);

                    if (element.HasShadow)
                        writer.WriteAttributeString("filter", $"url(#shadow_{element.Id})");

                    writer.WriteEndElement();
                    break;

                case ShapeType.Ellipse:
                    writer.WriteStartElement("ellipse");
                    writer.WriteAttributeString("cx", FormatNumber(element.Width / 2));
                    writer.WriteAttributeString("cy", FormatNumber(element.Height / 2));
                    writer.WriteAttributeString("rx", FormatNumber(element.Width / 2));
                    writer.WriteAttributeString("ry", FormatNumber(element.Height / 2));
                    writer.WriteAttributeString("fill", fillValue);
                    writer.WriteAttributeString("stroke", ConvertColorToSvg(element.StrokeColor));
                    writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));

                    if (!element.UseGradient && Math.Abs(fillOpacity - 1.0) > 0.001)
                    {
                        writer.WriteAttributeString("fill-opacity", FormatNumber(fillOpacity));
                    }

                    if (Math.Abs(strokeOpacity - 1.0) > 0.001)
                    {
                        writer.WriteAttributeString("stroke-opacity", FormatNumber(strokeOpacity));
                    }

                    ApplyStrokeDashStyle(writer, element.StrokeDashStyle);

                    if (element.HasShadow)
                        writer.WriteAttributeString("filter", $"url(#shadow_{element.Id})");

                    writer.WriteEndElement();
                    break;

                case ShapeType.Line:
                    writer.WriteStartElement("line");
                    writer.WriteAttributeString("x1", "0");
                    writer.WriteAttributeString("y1", "0");
                    writer.WriteAttributeString("x2", FormatNumber(element.Width));
                    writer.WriteAttributeString("y2", FormatNumber(element.Height));
                    writer.WriteAttributeString("stroke", ConvertColorToSvg(element.StrokeColor));
                    writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));
                    writer.WriteAttributeString("stroke-linecap", "round");

                    if (Math.Abs(strokeOpacity - 1.0) > 0.001)
                    {
                        writer.WriteAttributeString("stroke-opacity", FormatNumber(strokeOpacity));
                    }

                    ApplyStrokeDashStyle(writer, element.StrokeDashStyle);

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
            writer.WriteAttributeString("stroke", ConvertColorToSvg(element.StrokeColor));
            writer.WriteAttributeString("stroke-width", FormatNumber(element.StrokeThickness));
            writer.WriteAttributeString("stroke-linecap", "round");

            var strokeOpacity = GetColorOpacity(element.StrokeColor);
            if (Math.Abs(strokeOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("stroke-opacity", FormatNumber(strokeOpacity));
            }

            // Apply dash style
            ApplyStrokeDashStyle(writer, element.StrokeDashStyle);

            // Apply line caps (markers)
            if (!string.IsNullOrEmpty(element.StartCap) && element.StartCap != "None")
            {
                var markerRef = GetMarkerReference(element.StartCap);
                if (!string.IsNullOrEmpty(markerRef))
                    writer.WriteAttributeString("marker-start", markerRef);
            }

            if (!string.IsNullOrEmpty(element.EndCap) && element.EndCap != "None")
            {
                var markerRef = GetMarkerReference(element.EndCap);
                if (!string.IsNullOrEmpty(markerRef))
                    writer.WriteAttributeString("marker-end", markerRef);
            }

            writer.WriteEndElement();
        }

        private void ExportQrCodeElement(XmlWriter writer, QrCodeElement element)
        {
            try
            {
                // Generate actual QR code using the QR code service
                var qrBitmap = _qrCodeService.GenerateQrCode(
                    element.Content,
                    pixelsPerModule: 10,
                    foregroundColor: element.ForegroundColor,
                    backgroundColor: element.BackgroundColor,
                    errorCorrectionLevel: element.ErrorCorrectionLevel
                );

                // Convert BitmapSource to Base64 PNG
                string base64Image = ConvertBitmapToBase64(qrBitmap);

                // Export as SVG image element
                writer.WriteStartElement("image");
                writer.WriteAttributeString("width", FormatNumber(element.Width));
                writer.WriteAttributeString("height", FormatNumber(element.Height));
                writer.WriteAttributeString("href", $"data:image/png;base64,{base64Image}");
                writer.WriteAttributeString("preserveAspectRatio", "xMidYMid meet");
                writer.WriteEndElement();
            }
            catch (Exception ex)
            {
                // Fallback: render a placeholder rectangle if QR code generation fails
                _errorHandlingService.HandleError(ex, "Failed to generate QR code for SVG export", showDialog: false);

                writer.WriteStartElement("rect");
                writer.WriteAttributeString("width", FormatNumber(element.Width));
                writer.WriteAttributeString("height", FormatNumber(element.Height));
                writer.WriteAttributeString("fill", ConvertColorToSvg(element.BackgroundColor));
                writer.WriteAttributeString("stroke", ConvertColorToSvg(element.BorderColor));
                writer.WriteAttributeString("stroke-width", "2");
                writer.WriteEndElement();

                // Error text
                writer.WriteStartElement("text");
                writer.WriteAttributeString("x", FormatNumber(element.Width / 2));
                writer.WriteAttributeString("y", FormatNumber(element.Height / 2));
                writer.WriteAttributeString("text-anchor", "middle");
                writer.WriteAttributeString("dominant-baseline", "middle");
                writer.WriteAttributeString("fill", "#FF0000");
                writer.WriteAttributeString("font-size", "12");
                writer.WriteString("[QR Error]");
                writer.WriteEndElement();
            }
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
                writer.WriteAttributeString("stroke", ConvertColorToSvg(element.BorderColor));
                writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));

                var borderOpacity = GetColorOpacity(element.BorderColor);
                if (Math.Abs(borderOpacity - 1.0) > 0.001)
                {
                    writer.WriteAttributeString("stroke-opacity", FormatNumber(borderOpacity));
                }

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
                    writer.WriteAttributeString("fill", ConvertColorToSvg(bgColor));

                    var bgOpacity = GetColorOpacity(bgColor);
                    if (Math.Abs(bgOpacity - 1.0) > 0.001)
                    {
                        writer.WriteAttributeString("fill-opacity", FormatNumber(bgOpacity));
                    }

                    if (element.ShowBorder)
                    {
                        writer.WriteAttributeString("stroke", ConvertColorToSvg(element.BorderColor));
                        writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));

                        var borderOpacity = GetColorOpacity(element.BorderColor);
                        if (Math.Abs(borderOpacity - 1.0) > 0.001)
                        {
                            writer.WriteAttributeString("stroke-opacity", FormatNumber(borderOpacity));
                        }
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
                        writer.WriteAttributeString("fill", ConvertColorToSvg(textColor));

                        var textOpacity = GetColorOpacity(textColor);
                        if (Math.Abs(textOpacity - 1.0) > 0.001)
                        {
                            writer.WriteAttributeString("fill-opacity", FormatNumber(textOpacity));
                        }

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
            writer.WriteAttributeString("fill", ConvertColorToSvg(element.BackgroundColor));
            writer.WriteAttributeString("stroke", ConvertColorToSvg(element.BorderColor));
            writer.WriteAttributeString("stroke-width", FormatNumber(element.BorderThickness));
            writer.WriteAttributeString("rx", FormatNumber(element.CornerRadius));

            var bgOpacity = GetColorOpacity(element.BackgroundColor);
            if (Math.Abs(bgOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("fill-opacity", FormatNumber(bgOpacity));
            }

            var borderOpacity = GetColorOpacity(element.BorderColor);
            if (Math.Abs(borderOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("stroke-opacity", FormatNumber(borderOpacity));
            }

            writer.WriteEndElement();

            // Calculate positions based on icon presence and position
            double iconSize = element.FontSize * 1.2;
            double spacing = 6;
            bool hasIcon = !string.IsNullOrEmpty(element.Icon);

            double textX = element.Width / 2;
            double textY = element.Height / 2;
            double iconX = element.Width / 2;
            double iconY = element.Height / 2;

            var textOpacity = GetColorOpacity(element.ForegroundColor);

            if (hasIcon)
            {
                switch (element.IconPosition)
                {
                    case "Left":
                        iconX = element.Width / 2 - spacing;
                        textX = element.Width / 2 + iconSize / 2 + spacing;
                        break;
                    case "Right":
                        textX = element.Width / 2 - iconSize / 2 - spacing;
                        iconX = element.Width / 2 + spacing;
                        break;
                    case "Top":
                        iconY = element.Height / 2 - spacing;
                        textY = element.Height / 2 + iconSize / 2 + spacing;
                        break;
                    case "Bottom":
                        textY = element.Height / 2 - iconSize / 2 - spacing;
                        iconY = element.Height / 2 + spacing;
                        break;
                }

                // Icon text
                writer.WriteStartElement("text");
                writer.WriteAttributeString("x", FormatNumber(iconX));
                writer.WriteAttributeString("y", FormatNumber(iconY));
                writer.WriteAttributeString("font-family", element.FontFamily);
                writer.WriteAttributeString("font-size", FormatNumber(iconSize));
                writer.WriteAttributeString("fill", ConvertColorToSvg(element.ForegroundColor));
                writer.WriteAttributeString("text-anchor", "middle");
                writer.WriteAttributeString("dominant-baseline", "middle");

                if (Math.Abs(textOpacity - 1.0) > 0.001)
                {
                    writer.WriteAttributeString("fill-opacity", FormatNumber(textOpacity));
                }

                writer.WriteString(element.Icon);
                writer.WriteEndElement();
            }

            // Button text
            writer.WriteStartElement("text");
            writer.WriteAttributeString("x", FormatNumber(textX));
            writer.WriteAttributeString("y", FormatNumber(textY));
            writer.WriteAttributeString("font-family", element.FontFamily);
            writer.WriteAttributeString("font-size", FormatNumber(element.FontSize));
            writer.WriteAttributeString("fill", ConvertColorToSvg(element.ForegroundColor));
            writer.WriteAttributeString("text-anchor", "middle");
            writer.WriteAttributeString("dominant-baseline", "middle");

            if (Math.Abs(textOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("fill-opacity", FormatNumber(textOpacity));
            }

            if (element.IsBold)
                writer.WriteAttributeString("font-weight", "bold");
            if (element.IsItalic)
                writer.WriteAttributeString("font-style", "italic");

            writer.WriteString(element.Text);
            writer.WriteEndElement();
        }

        private void CreateShadowFilter(XmlWriter writer, string id, string color, double blur, double offsetX, double offsetY)
        {
            writer.WriteStartElement("filter");
            writer.WriteAttributeString("id", id);
            writer.WriteAttributeString("x", "-50%");
            writer.WriteAttributeString("y", "-50%");
            writer.WriteAttributeString("width", "200%");
            writer.WriteAttributeString("height", "200%");

            // Flood fill with shadow color
            writer.WriteStartElement("feFlood");
            writer.WriteAttributeString("flood-color", ConvertColorToSvg(color));

            var shadowOpacity = GetColorOpacity(color);
            if (Math.Abs(shadowOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("flood-opacity", FormatNumber(shadowOpacity));
            }

            writer.WriteAttributeString("result", "flood");
            writer.WriteEndElement();

            // Composite with alpha
            writer.WriteStartElement("feComposite");
            writer.WriteAttributeString("in", "flood");
            writer.WriteAttributeString("in2", "SourceAlpha");
            writer.WriteAttributeString("operator", "in");
            writer.WriteAttributeString("result", "shadow");
            writer.WriteEndElement();

            // Blur
            writer.WriteStartElement("feGaussianBlur");
            writer.WriteAttributeString("in", "shadow");
            writer.WriteAttributeString("stdDeviation", FormatNumber(blur / 2));
            writer.WriteAttributeString("result", "blurred");
            writer.WriteEndElement();

            // Offset
            writer.WriteStartElement("feOffset");
            writer.WriteAttributeString("in", "blurred");
            writer.WriteAttributeString("dx", FormatNumber(offsetX));
            writer.WriteAttributeString("dy", FormatNumber(offsetY));
            writer.WriteAttributeString("result", "offsetShadow");
            writer.WriteEndElement();

            // Merge shadow with source
            writer.WriteStartElement("feMerge");
            writer.WriteStartElement("feMergeNode");
            writer.WriteAttributeString("in", "offsetShadow");
            writer.WriteEndElement();
            writer.WriteStartElement("feMergeNode");
            writer.WriteAttributeString("in", "SourceGraphic");
            writer.WriteEndElement();
            writer.WriteEndElement(); // feMerge

            writer.WriteEndElement(); // filter
        }

        private void CreateGradient(XmlWriter writer, string id, string startColor, string endColor, string direction, double width, double height)
        {
            bool isLinear = direction != "Radial";

            if (isLinear)
            {
                writer.WriteStartElement("linearGradient");
                writer.WriteAttributeString("id", id);

                switch (direction)
                {
                    case "Horizontal":
                        writer.WriteAttributeString("x1", "0%");
                        writer.WriteAttributeString("y1", "0%");
                        writer.WriteAttributeString("x2", "100%");
                        writer.WriteAttributeString("y2", "0%");
                        break;
                    case "Vertical":
                        writer.WriteAttributeString("x1", "0%");
                        writer.WriteAttributeString("y1", "0%");
                        writer.WriteAttributeString("x2", "0%");
                        writer.WriteAttributeString("y2", "100%");
                        break;
                    case "Diagonal":
                        writer.WriteAttributeString("x1", "0%");
                        writer.WriteAttributeString("y1", "0%");
                        writer.WriteAttributeString("x2", "100%");
                        writer.WriteAttributeString("y2", "100%");
                        break;
                }
            }
            else
            {
                writer.WriteStartElement("radialGradient");
                writer.WriteAttributeString("id", id);
                writer.WriteAttributeString("cx", "50%");
                writer.WriteAttributeString("cy", "50%");
                writer.WriteAttributeString("r", "50%");
            }

            writer.WriteStartElement("stop");
            writer.WriteAttributeString("offset", "0%");
            writer.WriteAttributeString("stop-color", ConvertColorToSvg(startColor));

            var startOpacity = GetColorOpacity(startColor);
            if (Math.Abs(startOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("stop-opacity", FormatNumber(startOpacity));
            }

            writer.WriteEndElement();

            writer.WriteStartElement("stop");
            writer.WriteAttributeString("offset", "100%");
            writer.WriteAttributeString("stop-color", ConvertColorToSvg(endColor));

            var endOpacity = GetColorOpacity(endColor);
            if (Math.Abs(endOpacity - 1.0) > 0.001)
            {
                writer.WriteAttributeString("stop-opacity", FormatNumber(endOpacity));
            }

            writer.WriteEndElement();

            writer.WriteEndElement(); // linearGradient or radialGradient
        }

        private void CreateImageFilter(XmlWriter writer, string id, ImageElement element)
        {
            writer.WriteStartElement("filter");
            writer.WriteAttributeString("id", id);
            writer.WriteAttributeString("x", "-50%");
            writer.WriteAttributeString("y", "-50%");
            writer.WriteAttributeString("width", "200%");
            writer.WriteAttributeString("height", "200%");

            string currentResult = "SourceGraphic";

            // Grayscale
            if (element.Grayscale)
            {
                writer.WriteStartElement("feColorMatrix");
                writer.WriteAttributeString("in", currentResult);
                writer.WriteAttributeString("type", "saturate");
                writer.WriteAttributeString("values", "0");
                writer.WriteAttributeString("result", "grayscale");
                writer.WriteEndElement();
                currentResult = "grayscale";
            }

            // Saturation
            if (Math.Abs(element.Saturation - 1.0) > 0.001 && !element.Grayscale)
            {
                writer.WriteStartElement("feColorMatrix");
                writer.WriteAttributeString("in", currentResult);
                writer.WriteAttributeString("type", "saturate");
                writer.WriteAttributeString("values", FormatNumber(element.Saturation));
                writer.WriteAttributeString("result", "saturated");
                writer.WriteEndElement();
                currentResult = "saturated";
            }

            // Brightness & Contrast (using component transfer)
            if (Math.Abs(element.Brightness - 1.0) > 0.001 || Math.Abs(element.Contrast - 1.0) > 0.001)
            {
                writer.WriteStartElement("feComponentTransfer");
                writer.WriteAttributeString("in", currentResult);
                writer.WriteAttributeString("result", "adjusted");

                var slope = element.Contrast;
                var intercept = element.Brightness - 1.0;

                writer.WriteStartElement("feFuncR");
                writer.WriteAttributeString("type", "linear");
                writer.WriteAttributeString("slope", FormatNumber(slope));
                writer.WriteAttributeString("intercept", FormatNumber(intercept));
                writer.WriteEndElement();

                writer.WriteStartElement("feFuncG");
                writer.WriteAttributeString("type", "linear");
                writer.WriteAttributeString("slope", FormatNumber(slope));
                writer.WriteAttributeString("intercept", FormatNumber(intercept));
                writer.WriteEndElement();

                writer.WriteStartElement("feFuncB");
                writer.WriteAttributeString("type", "linear");
                writer.WriteAttributeString("slope", FormatNumber(slope));
                writer.WriteAttributeString("intercept", FormatNumber(intercept));
                writer.WriteEndElement();

                writer.WriteEndElement(); // feComponentTransfer
                currentResult = "adjusted";
            }

            // Blur
            if (element.Blur > 0)
            {
                writer.WriteStartElement("feGaussianBlur");
                writer.WriteAttributeString("in", currentResult);
                writer.WriteAttributeString("stdDeviation", FormatNumber(element.Blur));
                writer.WriteAttributeString("result", "blurred");
                writer.WriteEndElement();
                currentResult = "blurred";
            }

            // Shadow (if enabled)
            if (element.HasShadow)
            {
                // Create shadow
                writer.WriteStartElement("feFlood");
                writer.WriteAttributeString("flood-color", ConvertColorToSvg(element.ShadowColor));

                var shadowOpacity = GetColorOpacity(element.ShadowColor);
                if (Math.Abs(shadowOpacity - 1.0) > 0.001)
                {
                    writer.WriteAttributeString("flood-opacity", FormatNumber(shadowOpacity));
                }

                writer.WriteAttributeString("result", "shadowColor");
                writer.WriteEndElement();

                writer.WriteStartElement("feComposite");
                writer.WriteAttributeString("in", "shadowColor");
                writer.WriteAttributeString("in2", "SourceAlpha");
                writer.WriteAttributeString("operator", "in");
                writer.WriteAttributeString("result", "shadowAlpha");
                writer.WriteEndElement();

                writer.WriteStartElement("feGaussianBlur");
                writer.WriteAttributeString("in", "shadowAlpha");
                writer.WriteAttributeString("stdDeviation", FormatNumber(element.ShadowBlur / 2));
                writer.WriteAttributeString("result", "shadowBlurred");
                writer.WriteEndElement();

                writer.WriteStartElement("feOffset");
                writer.WriteAttributeString("in", "shadowBlurred");
                writer.WriteAttributeString("dx", FormatNumber(element.ShadowOffsetX));
                writer.WriteAttributeString("dy", FormatNumber(element.ShadowOffsetY));
                writer.WriteAttributeString("result", "shadowOffset");
                writer.WriteEndElement();

                // Merge shadow with filtered image
                writer.WriteStartElement("feMerge");
                writer.WriteStartElement("feMergeNode");
                writer.WriteAttributeString("in", "shadowOffset");
                writer.WriteEndElement();
                writer.WriteStartElement("feMergeNode");
                writer.WriteAttributeString("in", currentResult);
                writer.WriteEndElement();
                writer.WriteEndElement(); // feMerge
            }

            writer.WriteEndElement(); // filter
        }

        private void CreateRoundedClipPath(XmlWriter writer, string id, double width, double height, double radius)
        {
            writer.WriteStartElement("clipPath");
            writer.WriteAttributeString("id", id);

            writer.WriteStartElement("rect");
            writer.WriteAttributeString("width", FormatNumber(width));
            writer.WriteAttributeString("height", FormatNumber(height));
            writer.WriteAttributeString("rx", FormatNumber(radius));
            writer.WriteAttributeString("ry", FormatNumber(radius));
            writer.WriteEndElement();

            writer.WriteEndElement(); // clipPath
        }

        private void CreateArrowMarker(XmlWriter writer, string id, string position)
        {
            writer.WriteStartElement("marker");
            writer.WriteAttributeString("id", id);
            writer.WriteAttributeString("markerWidth", "10");
            writer.WriteAttributeString("markerHeight", "10");
            writer.WriteAttributeString("refX", position == "start" ? "0" : "10");
            writer.WriteAttributeString("refY", "5");
            writer.WriteAttributeString("orient", "auto");
            writer.WriteAttributeString("markerUnits", "strokeWidth");

            writer.WriteStartElement("path");
            if (position == "start")
                writer.WriteAttributeString("d", "M 10 0 L 0 5 L 10 10 z");
            else
                writer.WriteAttributeString("d", "M 0 0 L 10 5 L 0 10 z");
            writer.WriteAttributeString("fill", "context-stroke");
            writer.WriteEndElement();

            writer.WriteEndElement(); // marker
        }

        private void CreateCircleMarker(XmlWriter writer, string id)
        {
            writer.WriteStartElement("marker");
            writer.WriteAttributeString("id", id);
            writer.WriteAttributeString("markerWidth", "8");
            writer.WriteAttributeString("markerHeight", "8");
            writer.WriteAttributeString("refX", "4");
            writer.WriteAttributeString("refY", "4");
            writer.WriteAttributeString("markerUnits", "strokeWidth");

            writer.WriteStartElement("circle");
            writer.WriteAttributeString("cx", "4");
            writer.WriteAttributeString("cy", "4");
            writer.WriteAttributeString("r", "3");
            writer.WriteAttributeString("fill", "context-stroke");
            writer.WriteEndElement();

            writer.WriteEndElement(); // marker
        }

        private void CreateSquareMarker(XmlWriter writer, string id)
        {
            writer.WriteStartElement("marker");
            writer.WriteAttributeString("id", id);
            writer.WriteAttributeString("markerWidth", "8");
            writer.WriteAttributeString("markerHeight", "8");
            writer.WriteAttributeString("refX", "4");
            writer.WriteAttributeString("refY", "4");
            writer.WriteAttributeString("markerUnits", "strokeWidth");

            writer.WriteStartElement("rect");
            writer.WriteAttributeString("x", "1");
            writer.WriteAttributeString("y", "1");
            writer.WriteAttributeString("width", "6");
            writer.WriteAttributeString("height", "6");
            writer.WriteAttributeString("fill", "context-stroke");
            writer.WriteEndElement();

            writer.WriteEndElement(); // marker
        }

        private void ApplyStrokeDashStyle(XmlWriter writer, string dashStyle)
        {
            if (string.IsNullOrEmpty(dashStyle) || dashStyle == "Solid")
                return;

            string dashArray = dashStyle switch
            {
                "Dash" => "8,4",
                "Dot" => "2,2",
                "DashDot" => "8,4,2,4",
                "DashDotDot" => "8,4,2,4,2,4",
                _ => null
            };

            if (!string.IsNullOrEmpty(dashArray))
                writer.WriteAttributeString("stroke-dasharray", dashArray);
        }

        private string GetMarkerReference(string capStyle)
        {
            return capStyle switch
            {
                "Arrow" => "url(#arrowEnd)",
                "Circle" => "url(#circleMarker)",
                "Square" => "url(#squareMarker)",
                _ => null
            };
        }

        private string FormatNumber(double value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a BitmapSource to Base64-encoded PNG string
        /// </summary>
        private string ConvertBitmapToBase64(System.Windows.Media.Imaging.BitmapSource bitmap)
        {
            using var stream = new System.IO.MemoryStream();
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));
            encoder.Save(stream);
            return Convert.ToBase64String(stream.ToArray());
        }

        /// <summary>
        /// Converts WPF color format (#AARRGGBB) to SVG format (#RRGGBB)
        /// </summary>
        private string ConvertColorToSvg(string wpfColor)
        {
            if (string.IsNullOrEmpty(wpfColor))
                return wpfColor;

            // If color has alpha channel (#AARRGGBB), extract RGB only
            if (wpfColor.Length == 9 && wpfColor.StartsWith("#"))
            {
                // Extract RRGGBB (positions 3-8)
                return "#" + wpfColor.Substring(3, 6);
            }

            return wpfColor;
        }

        /// <summary>
        /// Extracts opacity from WPF color format (#AARRGGBB)
        /// Returns 1.0 if no alpha channel or if fully opaque
        /// </summary>
        private double GetColorOpacity(string wpfColor)
        {
            if (string.IsNullOrEmpty(wpfColor))
                return 1.0;

            // If color has alpha channel (#AARRGGBB)
            if (wpfColor.Length == 9 && wpfColor.StartsWith("#"))
            {
                var alphaHex = wpfColor.Substring(1, 2);
                if (int.TryParse(alphaHex, System.Globalization.NumberStyles.HexNumber, null, out int alpha))
                {
                    return alpha / 255.0;
                }
            }

            return 1.0;
        }
    }
}
