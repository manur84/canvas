using LayoutDesigner.Models.Base;
using System.Collections.ObjectModel;

namespace LayoutDesigner.Models
{
    /// <summary>
    /// Represents a group of layout elements that can be moved and manipulated together
    /// Best Practice: Grouping improves workflow efficiency
    /// </summary>
    public class ElementGroup : LayoutElementBase
    {
        private ObservableCollection<LayoutElementBase> _children = new();
        private bool _isExpanded = true;

        public ElementGroup()
        {
            ElementType = "Group";
            Name = "Group";
        }

        /// <summary>
        /// Child elements in this group
        /// </summary>
        public ObservableCollection<LayoutElementBase> Children
        {
            get => _children;
            set => SetProperty(ref _children, value);
        }

        /// <summary>
        /// Whether the group is expanded in the UI
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        /// <summary>
        /// Updates group bounds based on children positions and sizes
        /// </summary>
        public void UpdateBounds()
        {
            if (Children.Count == 0)
                return;

            var minX = Children.Min(e => e.X);
            var minY = Children.Min(e => e.Y);
            var maxX = Children.Max(e => e.X + e.Width);
            var maxY = Children.Max(e => e.Y + e.Height);

            X = minX;
            Y = minY;
            Width = maxX - minX;
            Height = maxY - minY;
        }

        /// <summary>
        /// Adds an element to the group
        /// </summary>
        public void AddChild(LayoutElementBase element)
        {
            if (element == null || element == this)
                return;

            // Don't add if already in group
            if (Children.Contains(element))
                return;

            Children.Add(element);
            UpdateBounds();
        }

        /// <summary>
        /// Removes an element from the group
        /// </summary>
        public void RemoveChild(LayoutElementBase element)
        {
            if (element == null)
                return;

            Children.Remove(element);

            if (Children.Count > 0)
                UpdateBounds();
        }

        /// <summary>
        /// Moves all children by the specified delta
        /// </summary>
        public void MoveChildren(double deltaX, double deltaY)
        {
            foreach (var child in Children)
            {
                child.X += deltaX;
                child.Y += deltaY;
            }

            X += deltaX;
            Y += deltaY;
        }

        /// <summary>
        /// Applies rotation to all children
        /// </summary>
        public void RotateChildren(double deltaRotation)
        {
            foreach (var child in Children)
            {
                child.Rotation += deltaRotation;
            }

            Rotation += deltaRotation;
        }

        /// <summary>
        /// Applies opacity to all children
        /// </summary>
        public void SetChildrenOpacity(double opacity)
        {
            foreach (var child in Children)
            {
                child.Opacity = opacity;
            }

            Opacity = opacity;
        }

        /// <summary>
        /// Locks/unlocks all children
        /// </summary>
        public void SetChildrenLocked(bool locked)
        {
            foreach (var child in Children)
            {
                child.IsLocked = locked;
            }

            IsLocked = locked;
        }

        /// <summary>
        /// Creates a clone of this group and all children
        /// </summary>
        public override LayoutElementBase Clone()
        {
            var clone = new ElementGroup
            {
                Name = Name + " Copy",
                X = X + 20,
                Y = Y + 20,
                Width = Width,
                Height = Height,
                Rotation = Rotation,
                Opacity = Opacity,
                IsVisible = IsVisible,
                IsLocked = IsLocked,
                ZIndex = ZIndex,
                IsExpanded = IsExpanded
            };

            // Clone all children
            foreach (var child in Children)
            {
                var childClone = child.Clone();
                // Adjust position relative to group
                childClone.X += 20;
                childClone.Y += 20;
                clone.Children.Add(childClone);
            }

            return clone;
        }

        /// <summary>
        /// Gets all elements in the group (flattened, including nested groups)
        /// </summary>
        public IEnumerable<LayoutElementBase> GetAllElements()
        {
            foreach (var child in Children)
            {
                yield return child;

                if (child is ElementGroup group)
                {
                    foreach (var nested in group.GetAllElements())
                    {
                        yield return nested;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if this group contains the specified element (recursively)
        /// </summary>
        public bool ContainsElement(LayoutElementBase element)
        {
            if (element == null)
                return false;

            return GetAllElements().Contains(element);
        }
    }
}
