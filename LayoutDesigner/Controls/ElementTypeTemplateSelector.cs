using System.Windows;
using System.Windows.Controls;
using LayoutDesigner.Models;

namespace LayoutDesigner.Controls
{
    /// <summary>
    /// DataTemplateSelector that selects the appropriate template based on element type
    /// This prevents binding errors by only creating bindings for properties that exist on the selected element type
    /// </summary>
    public class ElementTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextElementTemplate { get; set; }
        public DataTemplate? ImageElementTemplate { get; set; }
        public DataTemplate? ShapeElementTemplate { get; set; }
        public DataTemplate? QrCodeElementTemplate { get; set; }
        public DataTemplate? DynamicFieldElementTemplate { get; set; }
        public DataTemplate? ButtonElementTemplate { get; set; }
        public DataTemplate? TableElementTemplate { get; set; }
        public DataTemplate? LineElementTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                TextElement => TextElementTemplate,
                ImageElement => ImageElementTemplate,
                ShapeElement => ShapeElementTemplate,
                QrCodeElement => QrCodeElementTemplate,
                DynamicFieldElement => DynamicFieldElementTemplate,
                ButtonElement => ButtonElementTemplate,
                TableElement => TableElementTemplate,
                LineElement => LineElementTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
