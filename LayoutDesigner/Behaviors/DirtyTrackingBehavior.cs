using System.ComponentModel;
using System.Windows;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner.Behaviors
{
    /// <summary>
    /// Attached behavior for automatic dirty tracking of elements
    /// Best Practice: Automatically track property changes for incremental saves
    /// </summary>
    public static class DirtyTrackingBehavior
    {
        private static IDirtyTrackingService? _dirtyTrackingService;

        public static readonly DependencyProperty EnableDirtyTrackingProperty =
            DependencyProperty.RegisterAttached(
                "EnableDirtyTracking",
                typeof(bool),
                typeof(DirtyTrackingBehavior),
                new PropertyMetadata(false, OnEnableDirtyTrackingChanged));

        public static bool GetEnableDirtyTracking(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableDirtyTrackingProperty);
        }

        public static void SetEnableDirtyTracking(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableDirtyTrackingProperty, value);
        }

        private static void OnEnableDirtyTrackingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            // Get the service lazily
            _dirtyTrackingService ??= App.Services.Resolve<IDirtyTrackingService>();

            if ((bool)e.NewValue)
            {
                element.DataContextChanged += OnDataContextChanged;
            }
            else
            {
                element.DataContextChanged -= OnDataContextChanged;

                if (element.DataContext is LayoutElementBase layoutElement)
                {
                    layoutElement.PropertyChanged -= OnElementPropertyChanged;
                }
            }
        }

        private static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FrameworkElement element)
                return;

            // Unsubscribe from old DataContext
            if (e.OldValue is LayoutElementBase oldElement)
            {
                oldElement.PropertyChanged -= OnElementPropertyChanged;
            }

            // Subscribe to new DataContext
            if (e.NewValue is LayoutElementBase newElement)
            {
                newElement.PropertyChanged -= OnElementPropertyChanged; // Prevent double subscription
                newElement.PropertyChanged += OnElementPropertyChanged;
            }
        }

        private static void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not LayoutElementBase element)
                return;

            // Mark the element as dirty
            _dirtyTrackingService?.MarkDirty(element);
        }
    }
}
