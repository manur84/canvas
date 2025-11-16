using System.Windows;
using System.Windows.Documents;
using LayoutDesigner.Controls;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Behaviors
{
    /// <summary>
    /// Attached behavior that automatically shows SelectionAdorner on selected elements
    /// </summary>
    public static class SelectionBehavior
    {
        /// <summary>
        /// Attached property to enable selection adorner behavior
        /// </summary>
        public static readonly DependencyProperty EnableSelectionAdornerProperty =
            DependencyProperty.RegisterAttached(
                "EnableSelectionAdorner",
                typeof(bool),
                typeof(SelectionBehavior),
                new PropertyMetadata(false, OnEnableSelectionAdornerChanged));

        public static bool GetEnableSelectionAdorner(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableSelectionAdornerProperty);
        }

        public static void SetEnableSelectionAdorner(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableSelectionAdornerProperty, value);
        }

        private static void OnEnableSelectionAdornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            if ((bool)e.NewValue)
            {
                // Subscribe to DataContext changes
                element.DataContextChanged += OnDataContextChanged;
                element.Loaded += OnElementLoaded;
            }
            else
            {
                // Unsubscribe
                element.DataContextChanged -= OnDataContextChanged;
                element.Loaded -= OnElementLoaded;
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is LayoutElementBase layoutElement)
            {
                // Subscribe to IsSelected changes
                layoutElement.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(LayoutElementBase.IsSelected))
                    {
                        UpdateAdorner(element, layoutElement.IsSelected);
                    }
                };

                // Update adorner based on current selection state
                UpdateAdorner(element, layoutElement.IsSelected);
            }
        }

        private static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FrameworkElement element)
                return;

            // When DataContext changes, update adorner
            if (e.NewValue is LayoutElementBase layoutElement)
            {
                UpdateAdorner(element, layoutElement.IsSelected);

                // Subscribe to IsSelected changes
                layoutElement.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(LayoutElementBase.IsSelected))
                    {
                        UpdateAdorner(element, layoutElement.IsSelected);
                    }
                };
            }
            else
            {
                // Remove adorner if DataContext is not a LayoutElementBase
                RemoveAdorner(element);
            }
        }

        private static void UpdateAdorner(FrameworkElement element, bool isSelected)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer == null)
                return;

            // Remove existing adorner
            var existingAdorners = adornerLayer.GetAdorners(element);
            if (existingAdorners != null)
            {
                foreach (var adorner in existingAdorners.OfType<SelectionAdorner>())
                {
                    adornerLayer.Remove(adorner);
                }
            }

            // Add new adorner if selected
            if (isSelected)
            {
                var selectionAdorner = new SelectionAdorner(element);
                adornerLayer.Add(selectionAdorner);
            }
        }

        private static void RemoveAdorner(FrameworkElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer == null)
                return;

            var existingAdorners = adornerLayer.GetAdorners(element);
            if (existingAdorners != null)
            {
                foreach (var adorner in existingAdorners.OfType<SelectionAdorner>())
                {
                    adornerLayer.Remove(adorner);
                }
            }
        }
    }
}
