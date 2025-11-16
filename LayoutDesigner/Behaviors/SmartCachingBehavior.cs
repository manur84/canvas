using System.Windows;
using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Behaviors
{
    /// <summary>
    /// Attached behavior for intelligent render caching
    /// Best Practice: Automatically enable BitmapCache for complex elements
    /// </summary>
    public static class SmartCachingBehavior
    {
        public static readonly DependencyProperty EnableSmartCachingProperty =
            DependencyProperty.RegisterAttached(
                "EnableSmartCaching",
                typeof(bool),
                typeof(SmartCachingBehavior),
                new PropertyMetadata(false, OnEnableSmartCachingChanged));

        public static bool GetEnableSmartCaching(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableSmartCachingProperty);
        }

        public static void SetEnableSmartCaching(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableSmartCachingProperty, value);
        }

        private static void OnEnableSmartCachingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            if ((bool)e.NewValue)
            {
                element.DataContextChanged += OnDataContextChanged;
                UpdateCacheMode(element);
            }
            else
            {
                element.DataContextChanged -= OnDataContextChanged;
            }
        }

        private static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                UpdateCacheMode(element);
            }
        }

        private static void UpdateCacheMode(FrameworkElement element)
        {
            if (element.DataContext is not LayoutElementBase layoutElement)
                return;

            // Determine if this element should be cached based on type
            var shouldCache = ShouldCacheElement(layoutElement);

            // Set the EnableBitmapCache property on the element
            layoutElement.EnableBitmapCache = shouldCache;
        }

        /// <summary>
        /// Determines if an element should be cached based on complexity
        /// </summary>
        private static bool ShouldCacheElement(LayoutElementBase element)
        {
            return element switch
            {
                // Complex elements that benefit from caching
                QrCodeElement => true,          // Dynamic QR generation
                TableElement => true,           // Many sub-elements
                ElementGroup => true,           // Multiple children
                ButtonElement => true,          // Border + Text composition

                // Image elements - cache large images
                ImageElement img => img.Width * img.Height > 40000, // > 200x200px

                // Shape elements - cache complex shapes
                ShapeElement shape => shape.HasShadow || shape.CornerRadius > 0,

                // Text elements - cache if has shadow or border
                TextElement text => text.HasShadow || text.HasBorder,

                // Default: don't cache simple elements
                _ => false
            };
        }
    }
}
