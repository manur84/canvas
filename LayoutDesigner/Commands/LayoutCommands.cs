using System.Windows.Input;

namespace LayoutDesigner.Commands
{
    /// <summary>
    /// Static class containing application-wide routed commands
    /// </summary>
    public static class LayoutCommands
    {
        // Element manipulation commands
        public static readonly RoutedUICommand Duplicate = new RoutedUICommand(
            "Duplicate",
            "Duplicate",
            typeof(LayoutCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            });

        public static readonly RoutedUICommand Delete = new RoutedUICommand(
            "Delete",
            "Delete",
            typeof(LayoutCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Delete)
            });

        public static readonly RoutedUICommand BringToFront = new RoutedUICommand(
            "Bring to Front",
            "BringToFront",
            typeof(LayoutCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemCloseBrackets, ModifierKeys.Control)
            });

        public static readonly RoutedUICommand SendToBack = new RoutedUICommand(
            "Send to Back",
            "SendToBack",
            typeof(LayoutCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemOpenBrackets, ModifierKeys.Control)
            });

        public static readonly RoutedUICommand BringForward = new RoutedUICommand(
            "Bring Forward",
            "BringForward",
            typeof(LayoutCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemCloseBrackets, ModifierKeys.Control | ModifierKeys.Shift)
            });

        public static readonly RoutedUICommand SendBackward = new RoutedUICommand(
            "Send Backward",
            "SendBackward",
            typeof(LayoutCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.OemOpenBrackets, ModifierKeys.Control | ModifierKeys.Shift)
            });

        // Alignment commands
        public static readonly RoutedUICommand AlignLeft = new RoutedUICommand(
            "Align Left",
            "AlignLeft",
            typeof(LayoutCommands));

        public static readonly RoutedUICommand AlignCenter = new RoutedUICommand(
            "Align Center",
            "AlignCenter",
            typeof(LayoutCommands));

        public static readonly RoutedUICommand AlignRight = new RoutedUICommand(
            "Align Right",
            "AlignRight",
            typeof(LayoutCommands));

        public static readonly RoutedUICommand AlignTop = new RoutedUICommand(
            "Align Top",
            "AlignTop",
            typeof(LayoutCommands));

        public static readonly RoutedUICommand AlignMiddle = new RoutedUICommand(
            "Align Middle",
            "AlignMiddle",
            typeof(LayoutCommands));

        public static readonly RoutedUICommand AlignBottom = new RoutedUICommand(
            "Align Bottom",
            "AlignBottom",
            typeof(LayoutCommands));

        public static readonly RoutedUICommand DistributeHorizontally = new RoutedUICommand(
            "Distribute Horizontally",
            "DistributeHorizontally",
            typeof(LayoutCommands));

        public static readonly RoutedUICommand DistributeVertically = new RoutedUICommand(
            "Distribute Vertically",
            "DistributeVertically",
            typeof(LayoutCommands));
    }
}
