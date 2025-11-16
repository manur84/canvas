using System.Windows.Input;
using LayoutDesigner.Models.Base;

namespace LayoutDesigner.Helpers
{
    /// <summary>
    /// Handles keyboard shortcuts for element manipulation
    /// Best Practice: Keyboard shortcuts improve power user productivity
    /// </summary>
    public static class KeyboardHandler
    {
        /// <summary>
        /// Default nudge distance in pixels
        /// </summary>
        public const double DefaultNudgeDistance = 1;

        /// <summary>
        /// Large nudge distance (with Shift modifier)
        /// </summary>
        public const double LargeNudgeDistance = 10;

        /// <summary>
        /// Processes keyboard input for element manipulation
        /// Returns true if the key was handled
        /// </summary>
        public static bool HandleKeyDown(Key key, ModifierKeys modifiers, IEnumerable<LayoutElementBase>? selectedElements,
            out Action<LayoutElementBase>? action)
        {
            action = null;

            if (selectedElements is null || !selectedElements.Any())
                return false;

            var shift = modifiers.HasFlag(ModifierKeys.Shift);
            var ctrl = modifiers.HasFlag(ModifierKeys.Control);
            var alt = modifiers.HasFlag(ModifierKeys.Alt);

            // Calculate nudge distance
            var nudgeDistance = shift ? LargeNudgeDistance : DefaultNudgeDistance;

            switch (key)
            {
                // Arrow keys for movement
                case Key.Left:
                    action = element => element.X -= nudgeDistance;
                    return true;

                case Key.Right:
                    action = element => element.X += nudgeDistance;
                    return true;

                case Key.Up:
                    action = element => element.Y -= nudgeDistance;
                    return true;

                case Key.Down:
                    action = element => element.Y += nudgeDistance;
                    return true;

                // Size manipulation (Ctrl + Arrow)
                case Key.Left when ctrl:
                    action = element => element.Width = Math.Max(1, element.Width - nudgeDistance);
                    return true;

                case Key.Right when ctrl:
                    action = element => element.Width += nudgeDistance;
                    return true;

                case Key.Up when ctrl:
                    action = element => element.Height = Math.Max(1, element.Height - nudgeDistance);
                    return true;

                case Key.Down when ctrl:
                    action = element => element.Height += nudgeDistance;
                    return true;

                // Rotation (Alt + Arrow)
                case Key.Left when alt:
                    action = element => element.Rotation = (element.Rotation - 1 + 360) % 360;
                    return true;

                case Key.Right when alt:
                    action = element => element.Rotation = (element.Rotation + 1) % 360;
                    return true;

                // Opacity (Ctrl + Shift + Arrow)
                case Key.Up when ctrl && shift:
                    action = element => element.Opacity = Math.Min(1.0, element.Opacity + 0.05);
                    return true;

                case Key.Down when ctrl && shift:
                    action = element => element.Opacity = Math.Max(0.0, element.Opacity - 0.05);
                    return true;

                // Z-Index (Page Up/Down)
                case Key.PageUp:
                    action = element => element.ZIndex++;
                    return true;

                case Key.PageDown:
                    action = element => element.ZIndex--;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a human-readable description of the action for the given key combination
        /// </summary>
        public static string? GetActionDescription(Key key, ModifierKeys modifiers)
        {
            var shift = modifiers.HasFlag(ModifierKeys.Shift);
            var ctrl = modifiers.HasFlag(ModifierKeys.Control);
            var alt = modifiers.HasFlag(ModifierKeys.Alt);

            return (key, ctrl, shift, alt) switch
            {
                // Movement
                (Key.Left, false, false, false) => "Move Left 1px",
                (Key.Right, false, false, false) => "Move Right 1px",
                (Key.Up, false, false, false) => "Move Up 1px",
                (Key.Down, false, false, false) => "Move Down 1px",

                (Key.Left, false, true, false) => "Move Left 10px",
                (Key.Right, false, true, false) => "Move Right 10px",
                (Key.Up, false, true, false) => "Move Up 10px",
                (Key.Down, false, true, false) => "Move Down 10px",

                // Size
                (Key.Left, true, false, false) => "Decrease Width",
                (Key.Right, true, false, false) => "Increase Width",
                (Key.Up, true, false, false) => "Decrease Height",
                (Key.Down, true, false, false) => "Increase Height",

                // Rotation
                (Key.Left, false, false, true) => "Rotate Left 1°",
                (Key.Right, false, false, true) => "Rotate Right 1°",

                // Opacity
                (Key.Up, true, true, false) => "Increase Opacity",
                (Key.Down, true, true, false) => "Decrease Opacity",

                // Z-Index
                (Key.PageUp, _, _, _) => "Bring Forward",
                (Key.PageDown, _, _, _) => "Send Backward",

                _ => null
            };
        }

        /// <summary>
        /// Applies the action to all selected elements
        /// </summary>
        public static void ApplyToSelected(Action<LayoutElementBase>? action, IEnumerable<LayoutElementBase>? selectedElements)
        {
            if (action is null || selectedElements is null)
                return;

            foreach (var element in selectedElements.Where(e => !e.IsLocked))
            {
                action(element);
            }
        }

        /// <summary>
        /// Gets all available keyboard shortcuts
        /// </summary>
        public static Dictionary<string, string> GetAllShortcuts()
        {
            return new Dictionary<string, string>
            {
                // File operations
                { "Ctrl+N", "New Document" },
                { "Ctrl+O", "Open Document" },
                { "Ctrl+S", "Save Document" },
                { "Ctrl+Shift+S", "Save As..." },

                // Edit operations
                { "Ctrl+Z", "Undo" },
                { "Ctrl+Y", "Redo" },
                { "Ctrl+C", "Copy" },
                { "Ctrl+X", "Cut" },
                { "Ctrl+V", "Paste" },
                { "Ctrl+D", "Duplicate" },
                { "Ctrl+A", "Select All" },
                { "Delete", "Delete Selected" },

                // View operations
                { "Ctrl+Plus", "Zoom In" },
                { "Ctrl+Minus", "Zoom Out" },
                { "Ctrl+0", "Reset Zoom" },

                // Alignment
                { "Ctrl+Shift+L", "Align Left" },
                { "Ctrl+Shift+C", "Align Center" },
                { "Ctrl+Shift+R", "Align Right" },
                { "Ctrl+Shift+T", "Align Top" },
                { "Ctrl+Shift+M", "Align Middle" },
                { "Ctrl+Shift+B", "Align Bottom" },
                { "Ctrl+Shift+H", "Distribute Horizontally" },
                { "Ctrl+Shift+V", "Distribute Vertically" },

                // Movement
                { "Arrow Keys", "Move 1px" },
                { "Shift+Arrow Keys", "Move 10px" },

                // Size
                { "Ctrl+Arrow Left/Right", "Adjust Width" },
                { "Ctrl+Arrow Up/Down", "Adjust Height" },

                // Rotation
                { "Alt+Arrow Left/Right", "Rotate 1°" },

                // Opacity
                { "Ctrl+Shift+Arrow Up/Down", "Adjust Opacity" },

                // Z-Index
                { "Page Up", "Bring Forward" },
                { "Page Down", "Send Backward" },

                // Grouping
                { "Ctrl+G", "Group Elements" },
                { "Ctrl+Shift+G", "Ungroup Elements" }
            };
        }
    }
}
