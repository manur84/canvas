using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace LayoutDesigner.Models.Base
{
    /// <summary>
    /// Base class for all layout elements with common properties
    /// </summary>
    public abstract class LayoutElementBase : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        private string _name = string.Empty;
        private double _x;
        private double _y;
        private double _width = 100;
        private double _height = 100;
        private double _rotation;
        private int _zIndex;
        private bool _isLocked;
        private bool _isVisible = true;
        private double _opacity = 1.0;
        private bool _enableBitmapCache = false; // Performance: BitmapCache for complex elements
        private bool _useLayoutRounding = true; // Performance: Pixel-perfect rendering

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Unique identifier for this element
        /// </summary>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Display name of the element
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// X coordinate on the canvas
        /// </summary>
        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        /// <summary>
        /// Y coordinate on the canvas
        /// </summary>
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        /// <summary>
        /// Width of the element
        /// </summary>
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        /// <summary>
        /// Height of the element
        /// </summary>
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        /// <summary>
        /// Rotation angle in degrees
        /// </summary>
        public double Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        /// <summary>
        /// Z-Index for layering (higher values are on top)
        /// </summary>
        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
        }

        /// <summary>
        /// Whether the element is locked (cannot be moved/edited)
        /// </summary>
        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        /// <summary>
        /// Whether the element is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        /// <summary>
        /// Opacity of the element (0.0 to 1.0)
        /// </summary>
        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, Math.Clamp(value, 0.0, 1.0));
        }

        /// <summary>
        /// Enable BitmapCache for better performance on complex elements
        /// Best Practice: Enable for elements that are rendered frequently but change infrequently
        /// </summary>
        public bool EnableBitmapCache
        {
            get => _enableBitmapCache;
            set => SetProperty(ref _enableBitmapCache, value);
        }

        /// <summary>
        /// Use layout rounding for pixel-perfect rendering
        /// Best Practice: Prevents blurry rendering on non-integer pixel positions
        /// </summary>
        public bool UseLayoutRounding
        {
            get => _useLayoutRounding;
            set => SetProperty(ref _useLayoutRounding, value);
        }

        /// <summary>
        /// Type of the element (used for deserialization)
        /// </summary>
        [JsonIgnore]
        public abstract string ElementType { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Creates a deep copy of the element
        /// </summary>
        public abstract LayoutElementBase Clone();
    }
}
