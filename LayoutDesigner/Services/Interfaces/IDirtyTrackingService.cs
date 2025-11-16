using System;
using System.Collections.Generic;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for tracking changes to elements for incremental saves
    /// Performance: Only save elements that have changed since last save
    /// </summary>
    public interface IDirtyTrackingService
    {
        /// <summary>
        /// Marks an element as modified
        /// </summary>
        void MarkDirty(LayoutElementBase element);

        /// <summary>
        /// Marks an element as clean (unchanged)
        /// </summary>
        void MarkClean(LayoutElementBase element);

        /// <summary>
        /// Checks if an element has been modified
        /// </summary>
        bool IsDirty(LayoutElementBase element);

        /// <summary>
        /// Gets all dirty elements
        /// </summary>
        IEnumerable<LayoutElementBase> GetDirtyElements();

        /// <summary>
        /// Clears all dirty flags (call after successful save)
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Gets the number of dirty elements
        /// </summary>
        int DirtyCount { get; }

        /// <summary>
        /// Event raised when dirty state changes
        /// </summary>
        event EventHandler? DirtyStateChanged;
    }
}
