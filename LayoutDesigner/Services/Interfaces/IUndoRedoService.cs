using LayoutDesigner.Models;

namespace LayoutDesigner.Services.Interfaces
{
    /// <summary>
    /// Service for managing undo/redo operations
    /// </summary>
    public interface IUndoRedoService
    {
        /// <summary>
        /// Adds an action to the undo stack
        /// </summary>
        void AddAction(UndoRedoAction action);

        /// <summary>
        /// Undoes the last action
        /// </summary>
        void Undo();

        /// <summary>
        /// Redoes the last undone action
        /// </summary>
        void Redo();

        /// <summary>
        /// Clears all undo/redo history
        /// </summary>
        void Clear();

        /// <summary>
        /// Whether there are actions to undo
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Whether there are actions to redo
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Gets the undo history
        /// </summary>
        IReadOnlyList<UndoRedoAction> UndoHistory { get; }

        /// <summary>
        /// Gets the redo history
        /// </summary>
        IReadOnlyList<UndoRedoAction> RedoHistory { get; }

        /// <summary>
        /// Event raised when undo/redo state changes
        /// </summary>
        event EventHandler? StateChanged;
    }
}
