namespace LayoutDesigner.Models
{
    /// <summary>
    /// Represents an action that can be undone/redone
    /// </summary>
    public class UndoRedoAction
    {
        public UndoRedoAction(Action undoAction, Action redoAction, string description = "")
        {
            UndoAction = undoAction ?? throw new ArgumentNullException(nameof(undoAction));
            RedoAction = redoAction ?? throw new ArgumentNullException(nameof(redoAction));
            Description = description;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Action to execute when undoing
        /// </summary>
        public Action UndoAction { get; }

        /// <summary>
        /// Action to execute when redoing
        /// </summary>
        public Action RedoAction { get; }

        /// <summary>
        /// Description of this action
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// When this action was created
        /// </summary>
        public DateTime Timestamp { get; }
    }
}
