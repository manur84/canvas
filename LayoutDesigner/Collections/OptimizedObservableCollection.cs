using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace LayoutDesigner.Collections
{
    /// <summary>
    /// Optimized ObservableCollection that supports batch updates
    /// Best Practice: Suppress notifications during bulk operations to improve performance
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    public class OptimizedObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification;

        /// <summary>
        /// Adds a range of items to the collection with a single notification
        /// Best Practice: Batch collection changes to reduce UI updates
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _suppressNotification = true;

            try
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
            finally
            {
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            }
        }

        /// <summary>
        /// Removes a range of items from the collection with a single notification
        /// Best Practice: Batch collection changes to reduce UI updates
        /// </summary>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _suppressNotification = true;

            try
            {
                var itemsList = items.ToList();
                foreach (var item in itemsList)
                {
                    Remove(item);
                }
            }
            finally
            {
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            }
        }

        /// <summary>
        /// Executes an action with collection change notifications suspended
        /// Best Practice: Use for complex multi-step operations
        /// </summary>
        public void ExecuteBatch(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _suppressNotification = true;

            try
            {
                action();
            }
            finally
            {
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Performance: Suppress notifications during batch operations
            if (!_suppressNotification)
            {
                base.OnCollectionChanged(e);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            // Performance: Suppress notifications during batch operations
            if (!_suppressNotification)
            {
                base.OnPropertyChanged(e);
            }
        }
    }
}
