using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Behaviors
{
    /// <summary>
    /// Attached behavior for viewport culling optimization
    /// Best Practice: Hide elements outside viewport to reduce render overhead
    /// Lighter alternative to full canvas virtualization
    ///
    /// NOTE: This is an aggressive optimization for very large canvases (1000+ elements).
    /// It directly manipulates container visibility and may conflict with element.IsVisible bindings.
    /// Only enable if experiencing performance issues with many elements.
    /// </summary>
    public static class ViewportCullingBehavior
    {
        private static ScrollViewer? _scrollViewer;
        private static ItemsControl? _itemsControl;

        public static readonly DependencyProperty EnableViewportCullingProperty =
            DependencyProperty.RegisterAttached(
                "EnableViewportCulling",
                typeof(bool),
                typeof(ViewportCullingBehavior),
                new PropertyMetadata(false, OnEnableViewportCullingChanged));

        public static bool GetEnableViewportCulling(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableViewportCullingProperty);
        }

        public static void SetEnableViewportCulling(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableViewportCullingProperty, value);
        }

        private static void OnEnableViewportCullingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ItemsControl itemsControl)
                return;

            if ((bool)e.NewValue)
            {
                _itemsControl = itemsControl;
                itemsControl.Loaded += OnItemsControlLoaded;
            }
            else
            {
                itemsControl.Loaded -= OnItemsControlLoaded;
                if (_scrollViewer != null)
                {
                    _scrollViewer.ScrollChanged -= OnScrollChanged;
                }
            }
        }

        private static void OnItemsControlLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ItemsControl itemsControl)
                return;

            // Find the parent ScrollViewer
            _scrollViewer = FindParent<ScrollViewer>(itemsControl);

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += OnScrollChanged;
                UpdateVisibleElements(); // Initial update
            }
        }

        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateVisibleElements();
        }

        private static void UpdateVisibleElements()
        {
            if (_scrollViewer == null || _itemsControl == null)
                return;

            // Get viewport bounds
            var viewportWidth = _scrollViewer.ViewportWidth;
            var viewportHeight = _scrollViewer.ViewportHeight;
            var horizontalOffset = _scrollViewer.HorizontalOffset;
            var verticalOffset = _scrollViewer.VerticalOffset;

            // Add buffer zone for smoother scrolling (10% of viewport)
            var bufferX = viewportWidth * 0.1;
            var bufferY = viewportHeight * 0.1;

            var viewportRect = new Rect(
                horizontalOffset - bufferX,
                verticalOffset - bufferY,
                viewportWidth + (2 * bufferX),
                viewportHeight + (2 * bufferY));

            // Update visibility for each element
            for (int i = 0; i < _itemsControl.Items.Count; i++)
            {
                var item = _itemsControl.Items[i];

                if (item is not LayoutElementBase element)
                    continue;

                // Get the container (ContentPresenter) for this item
                var container = _itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as UIElement;

                if (container == null)
                    continue;

                var elementRect = new Rect(element.X, element.Y, element.Width, element.Height);

                // Check if element intersects with viewport
                var isInViewport = viewportRect.IntersectsWith(elementRect);

                // Combine viewport culling with element's IsVisible property
                // Only show if both in viewport AND element is marked visible
                var shouldBeVisible = isInViewport && element.IsVisible;
                var targetVisibility = shouldBeVisible ? Visibility.Visible : Visibility.Collapsed;

                // Only update if changed to avoid unnecessary updates
                if (container.Visibility != targetVisibility)
                {
                    container.Visibility = targetVisibility;
                }
            }
        }

        /// <summary>
        /// Helper method to find parent of specific type
        /// </summary>
        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
                return null;

            if (parent is T typedParent)
                return typedParent;

            return FindParent<T>(parent);
        }
    }
}
