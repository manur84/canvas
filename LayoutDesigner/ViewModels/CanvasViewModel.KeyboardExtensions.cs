using LayoutDesigner.Helpers;
using LayoutDesigner.Models.Base;
using System.Windows.Input;

namespace LayoutDesigner.ViewModels
{
    /// <summary>
    /// Partial class extension for keyboard handling functionality
    /// </summary>
    public partial class CanvasViewModel
    {
        #region Keyboard Handling

        /// <summary>
        /// Handles keyboard input for element manipulation
        /// Returns true if the key was handled
        /// </summary>
        public bool HandleKeyDown(Key key, ModifierKeys modifiers)
        {
            // Check for grouping shortcuts first
            if (modifiers == ModifierKeys.Control && key == Key.G)
            {
                if (GroupCommand?.CanExecute(null) == true)
                {
                    GroupCommand.Execute(null);
                    return true;
                }
            }

            if (modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && key == Key.G)
            {
                if (UngroupCommand?.CanExecute(null) == true)
                {
                    UngroupCommand.Execute(null);
                    return true;
                }
            }

            // Check for delete key
            if (key == Key.Delete)
            {
                if (DeleteSelectedCommand.CanExecute(null))
                {
                    DeleteSelectedCommand.Execute(null);
                    return true;
                }
            }

            // Use KeyboardHandler for arrow keys and other element manipulation
            if (KeyboardHandler.HandleKeyDown(key, modifiers, SelectedElements, out var action))
            {
                if (action != null)
                {
                    ApplyActionToSelected(action);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies an action to all selected elements with undo/redo support
        /// </summary>
        private void ApplyActionToSelected(Action<LayoutElementBase> action)
        {
            if (SelectedElements.Count == 0)
                return;

            // Store original values for undo
            var originalValues = SelectedElements
                .Where(e => !e.IsLocked)
                .Select(e => new
                {
                    Element = e,
                    X = e.X,
                    Y = e.Y,
                    Width = e.Width,
                    Height = e.Height,
                    Rotation = e.Rotation,
                    Opacity = e.Opacity,
                    ZIndex = e.ZIndex
                })
                .ToList();

            if (originalValues.Count == 0)
                return;

            // Apply action
            foreach (var item in originalValues)
            {
                action(item.Element);
            }

            // Add undo/redo
            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var item in originalValues)
                    {
                        item.Element.X = item.X;
                        item.Element.Y = item.Y;
                        item.Element.Width = item.Width;
                        item.Element.Height = item.Height;
                        item.Element.Rotation = item.Rotation;
                        item.Element.Opacity = item.Opacity;
                        item.Element.ZIndex = item.ZIndex;
                    }
                },
                redoAction: () =>
                {
                    foreach (var item in originalValues)
                    {
                        action(item.Element);
                    }
                },
                description: "Keyboard manipulation"
            ));
        }

        /// <summary>
        /// Gets description for keyboard shortcut
        /// </summary>
        public string? GetKeyboardShortcutDescription(Key key, ModifierKeys modifiers)
        {
            return KeyboardHandler.GetActionDescription(key, modifiers);
        }

        /// <summary>
        /// Gets all available keyboard shortcuts
        /// </summary>
        public Dictionary<string, string> GetAllKeyboardShortcuts()
        {
            return KeyboardHandler.GetAllShortcuts();
        }

        #endregion
    }
}
