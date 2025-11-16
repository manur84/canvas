using LayoutDesigner.Models;
using LayoutDesigner.Services.Interfaces;
using System.Collections.ObjectModel;

namespace LayoutDesigner.Services
{
    /// <summary>
    /// Service for managing undo/redo operations
    /// </summary>
    public class UndoRedoService : IUndoRedoService
    {
        private readonly Stack<UndoRedoAction> _undoStack = new();
        private readonly Stack<UndoRedoAction> _redoStack = new();
        private const int MaxHistorySize = 100;

        public event EventHandler? StateChanged;

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public IReadOnlyList<UndoRedoAction> UndoHistory => _undoStack.ToList();
        public IReadOnlyList<UndoRedoAction> RedoHistory => _redoStack.ToList();

        public void AddAction(UndoRedoAction action)
        {
            _undoStack.Push(action);

            // Clear redo stack when new action is added
            _redoStack.Clear();

            // Limit history size
            if (_undoStack.Count > MaxHistorySize)
            {
                var temp = _undoStack.Take(MaxHistorySize).ToList();
                _undoStack.Clear();
                temp.Reverse();
                foreach (var item in temp)
                {
                    _undoStack.Push(item);
                }
            }

            OnStateChanged();
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            var action = _undoStack.Pop();
            action.UndoAction();
            _redoStack.Push(action);

            OnStateChanged();
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            var action = _redoStack.Pop();
            action.RedoAction();
            _undoStack.Push(action);

            OnStateChanged();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
