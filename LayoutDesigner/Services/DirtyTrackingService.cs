using System;
using System.Collections.Generic;
using System.Linq;
using LayoutDesigner.Models.Base;
using LayoutDesigner.Services.Interfaces;
using System.Collections.Concurrent;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Service for tracking element changes for incremental saves
    /// Best Practice: Track changes to avoid full document serialization
    /// </summary>
    public class DirtyTrackingService : IDirtyTrackingService
    {
        private readonly ConcurrentDictionary<string, LayoutElementBase> _dirtyElements = new();
        private readonly object _lockObject = new();

        public event EventHandler? DirtyStateChanged;

        public int DirtyCount => _dirtyElements.Count;

        public void MarkDirty(LayoutElementBase element)
        {
            if (element == null)
                return;

            var wasEmpty = _dirtyElements.IsEmpty;

            _dirtyElements.TryAdd(element.Id, element);

            if (wasEmpty && !_dirtyElements.IsEmpty)
            {
                OnDirtyStateChanged();
            }
        }

        public void MarkClean(LayoutElementBase element)
        {
            if (element == null)
                return;

            var hadItems = !_dirtyElements.IsEmpty;

            _dirtyElements.TryRemove(element.Id, out _);

            if (hadItems && _dirtyElements.IsEmpty)
            {
                OnDirtyStateChanged();
            }
        }

        public bool IsDirty(LayoutElementBase element)
        {
            if (element == null)
                return false;

            return _dirtyElements.ContainsKey(element.Id);
        }

        public IEnumerable<LayoutElementBase> GetDirtyElements()
        {
            return _dirtyElements.Values.ToList();
        }

        public void ClearAll()
        {
            var hadItems = !_dirtyElements.IsEmpty;

            _dirtyElements.Clear();

            if (hadItems)
            {
                OnDirtyStateChanged();
            }
        }

        private void OnDirtyStateChanged()
        {
            DirtyStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
