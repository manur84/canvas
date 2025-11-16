using LayoutDesigner.Models;
using LayoutDesigner.Models.Base;
using System.Windows.Input;

namespace LayoutDesigner.ViewModels
{
    /// <summary>
    /// Partial class extension for grouping functionality
    /// </summary>
    public partial class CanvasViewModel
    {
        #region Grouping Commands

        private ICommand? _groupCommand;
        private ICommand? _ungroupCommand;

        public ICommand? GroupCommand => _groupCommand;
        public ICommand? UngroupCommand => _ungroupCommand;

        /// <summary>
        /// Initializes grouping commands (call from constructor)
        /// </summary>
        partial void InitializeGroupingCommands()
        {
            _groupCommand = new RelayCommand(GroupSelected, () => SelectedElements.Count >= 2);
            _ungroupCommand = new RelayCommand(UngroupSelected, () => SelectedElements.Count == 1 && SelectedElements[0] is ElementGroup);
        }

        #endregion

        #region Grouping Methods

        /// <summary>
        /// Groups selected elements
        /// </summary>
        private void GroupSelected()
        {
            if (SelectedElements.Count < 2)
                return;

            var elementsToGroup = SelectedElements.ToList();

            // Create new group
            var group = new ElementGroup
            {
                Name = $"Group {Elements.Count + 1}"
            };

            // Add elements to group
            foreach (var element in elementsToGroup)
            {
                group.AddChild(element);
                Elements.Remove(element);
            }

            // Update group bounds
            group.UpdateBounds();

            // Add group to canvas
            Elements.Add(group);

            // Add undo/redo
            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    Elements.Remove(group);
                    foreach (var element in elementsToGroup)
                    {
                        Elements.Add(element);
                    }
                },
                redoAction: () =>
                {
                    foreach (var element in elementsToGroup)
                    {
                        Elements.Remove(element);
                    }
                    Elements.Add(group);
                },
                description: $"Group {elementsToGroup.Count} elements"
            ));

            // Select the new group
            SelectedElements.Clear();
            SelectedElements.Add(group);
        }

        /// <summary>
        /// Ungroups selected group
        /// </summary>
        private void UngroupSelected()
        {
            if (SelectedElements.Count != 1 || SelectedElements[0] is not ElementGroup group)
                return;

            var children = group.Children.ToList();

            // Remove group
            Elements.Remove(group);

            // Add children back to canvas
            foreach (var child in children)
            {
                Elements.Add(child);
            }

            // Add undo/redo
            _undoRedoService.AddAction(new UndoRedoAction(
                undoAction: () =>
                {
                    foreach (var child in children)
                    {
                        Elements.Remove(child);
                    }
                    Elements.Add(group);
                },
                redoAction: () =>
                {
                    Elements.Remove(group);
                    foreach (var child in children)
                    {
                        Elements.Add(child);
                    }
                },
                description: "Ungroup elements"
            ));

            // Select the ungrouped elements
            SelectedElements.Clear();
            foreach (var child in children)
            {
                SelectedElements.Add(child);
            }
        }

        /// <summary>
        /// Checks if a group contains the specified element
        /// </summary>
        private bool IsElementInGroup(LayoutElementBase element, ElementGroup group)
        {
            return group.ContainsElement(element);
        }

        /// <summary>
        /// Gets the top-level group containing the element (if any)
        /// </summary>
        private ElementGroup? GetParentGroup(LayoutElementBase element)
        {
            foreach (var item in Elements)
            {
                if (item is ElementGroup group && group.ContainsElement(element))
                {
                    return group;
                }
            }

            return null;
        }

        #endregion
    }
}
