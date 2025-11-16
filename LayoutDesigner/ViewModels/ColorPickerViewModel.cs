using System;
using System.Windows.Media;
using LayoutDesigner.ViewModels.Base;

namespace LayoutDesigner.ViewModels
{
    /// <summary>
    /// ViewModel for Color Picker Dialog
    /// Handles RGB/HSV conversion and color preview
    /// </summary>
    public class ColorPickerViewModel : ViewModelBase
    {
        private int _redValue;
        private int _greenValue;
        private int _blueValue;
        private int _alphaValue = 255;
        private double _hueValue;
        private double _saturationValue;
        private double _brightnessValue;
        private string _hexColor = "#FFFFFFFF";
        private bool _isUpdatingFromRgb;
        private bool _isUpdatingFromHsv;
        private bool _isUpdatingFromHex;
        private readonly string _originalColor;

        public ColorPickerViewModel(string initialColor)
        {
            _originalColor = initialColor ?? "#FFFFFFFF";
            ParseHexColor(_originalColor);
        }

        #region Properties

        public int RedValue
        {
            get => _redValue;
            set
            {
                if (SetProperty(ref _redValue, Math.Clamp(value, 0, 255)))
                {
                    if (!_isUpdatingFromHsv && !_isUpdatingFromHex)
                    {
                        UpdateFromRgb();
                    }
                }
            }
        }

        public int GreenValue
        {
            get => _greenValue;
            set
            {
                if (SetProperty(ref _greenValue, Math.Clamp(value, 0, 255)))
                {
                    if (!_isUpdatingFromHsv && !_isUpdatingFromHex)
                    {
                        UpdateFromRgb();
                    }
                }
            }
        }

        public int BlueValue
        {
            get => _blueValue;
            set
            {
                if (SetProperty(ref _blueValue, Math.Clamp(value, 0, 255)))
                {
                    if (!_isUpdatingFromHsv && !_isUpdatingFromHex)
                    {
                        UpdateFromRgb();
                    }
                }
            }
        }

        public int AlphaValue
        {
            get => _alphaValue;
            set
            {
                if (SetProperty(ref _alphaValue, Math.Clamp(value, 0, 255)))
                {
                    if (!_isUpdatingFromHsv && !_isUpdatingFromHex)
                    {
                        UpdateFromRgb();
                    }
                }
            }
        }

        public double HueValue
        {
            get => _hueValue;
            set
            {
                if (SetProperty(ref _hueValue, Math.Clamp(value, 0, 360)))
                {
                    if (!_isUpdatingFromRgb && !_isUpdatingFromHex)
                    {
                        UpdateFromHsv();
                    }
                }
            }
        }

        public double SaturationValue
        {
            get => _saturationValue;
            set
            {
                if (SetProperty(ref _saturationValue, Math.Clamp(value, 0, 100)))
                {
                    if (!_isUpdatingFromRgb && !_isUpdatingFromHex)
                    {
                        UpdateFromHsv();
                    }
                }
            }
        }

        public double BrightnessValue
        {
            get => _brightnessValue;
            set
            {
                if (SetProperty(ref _brightnessValue, Math.Clamp(value, 0, 100)))
                {
                    if (!_isUpdatingFromRgb && !_isUpdatingFromHex)
                    {
                        UpdateFromHsv();
                    }
                }
            }
        }

        public string HexColor
        {
            get => _hexColor;
            set
            {
                if (SetProperty(ref _hexColor, value))
                {
                    if (!_isUpdatingFromRgb && !_isUpdatingFromHsv)
                    {
                        ParseHexColor(value);
                    }
                }
            }
        }

        public SolidColorBrush NewColorBrush
        {
            get
            {
                var color = Color.FromArgb((byte)_alphaValue, (byte)_redValue, (byte)_greenValue, (byte)_blueValue);
                return new SolidColorBrush(color);
            }
        }

        public SolidColorBrush OriginalColorBrush
        {
            get
            {
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(_originalColor);
                    return new SolidColorBrush(color);
                }
                catch
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
        }

        #endregion

        #region Color Conversion

        private void UpdateFromRgb()
        {
            _isUpdatingFromRgb = true;

            // Update HSV
            var (h, s, v) = RgbToHsv(_redValue, _greenValue, _blueValue);
            _hueValue = h;
            _saturationValue = s;
            _brightnessValue = v;

            OnPropertyChanged(nameof(HueValue));
            OnPropertyChanged(nameof(SaturationValue));
            OnPropertyChanged(nameof(BrightnessValue));

            // Update Hex
            _hexColor = $"#{_alphaValue:X2}{_redValue:X2}{_greenValue:X2}{_blueValue:X2}";
            OnPropertyChanged(nameof(HexColor));

            // Update preview
            OnPropertyChanged(nameof(NewColorBrush));

            _isUpdatingFromRgb = false;
        }

        private void UpdateFromHsv()
        {
            _isUpdatingFromHsv = true;

            // Update RGB
            var (r, g, b) = HsvToRgb(_hueValue, _saturationValue, _brightnessValue);
            _redValue = r;
            _greenValue = g;
            _blueValue = b;

            OnPropertyChanged(nameof(RedValue));
            OnPropertyChanged(nameof(GreenValue));
            OnPropertyChanged(nameof(BlueValue));

            // Update Hex
            _hexColor = $"#{_alphaValue:X2}{_redValue:X2}{_greenValue:X2}{_blueValue:X2}";
            OnPropertyChanged(nameof(HexColor));

            // Update preview
            OnPropertyChanged(nameof(NewColorBrush));

            _isUpdatingFromHsv = false;
        }

        private void ParseHexColor(string hex)
        {
            _isUpdatingFromHex = true;

            try
            {
                // Remove # if present
                hex = hex.TrimStart('#');

                // Handle different formats
                if (hex.Length == 6)
                {
                    // RGB format - add alpha
                    hex = "FF" + hex;
                }
                else if (hex.Length == 8)
                {
                    // ARGB format - already complete
                }
                else if (hex.Length == 3)
                {
                    // Short RGB format (e.g., "F0A" -> "FFF00AA")
                    hex = "FF" + hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
                }
                else if (hex.Length == 4)
                {
                    // Short ARGB format (e.g., "8F0A" -> "88FF00AA")
                    hex = hex[0].ToString() + hex[0] + hex[1] + hex[1] + hex[2] + hex[2] + hex[3] + hex[3];
                }
                else
                {
                    // Invalid format - reset to white
                    hex = "FFFFFFFF";
                }

                _alphaValue = Convert.ToInt32(hex.Substring(0, 2), 16);
                _redValue = Convert.ToInt32(hex.Substring(2, 2), 16);
                _greenValue = Convert.ToInt32(hex.Substring(4, 2), 16);
                _blueValue = Convert.ToInt32(hex.Substring(6, 2), 16);

                OnPropertyChanged(nameof(AlphaValue));
                OnPropertyChanged(nameof(RedValue));
                OnPropertyChanged(nameof(GreenValue));
                OnPropertyChanged(nameof(BlueValue));

                // Update HSV
                var (h, s, v) = RgbToHsv(_redValue, _greenValue, _blueValue);
                _hueValue = h;
                _saturationValue = s;
                _brightnessValue = v;

                OnPropertyChanged(nameof(HueValue));
                OnPropertyChanged(nameof(SaturationValue));
                OnPropertyChanged(nameof(BrightnessValue));

                // Ensure hex is formatted correctly
                _hexColor = $"#{_alphaValue:X2}{_redValue:X2}{_greenValue:X2}{_blueValue:X2}";
                OnPropertyChanged(nameof(HexColor));

                // Update preview
                OnPropertyChanged(nameof(NewColorBrush));
            }
            catch
            {
                // Invalid hex color - keep current values
            }

            _isUpdatingFromHex = false;
        }

        /// <summary>
        /// Converts RGB to HSV
        /// </summary>
        private static (double hue, double saturation, double value) RgbToHsv(int r, int g, int b)
        {
            double rNorm = r / 255.0;
            double gNorm = g / 255.0;
            double bNorm = b / 255.0;

            double max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
            double min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
            double delta = max - min;

            // Hue calculation
            double hue = 0;
            if (delta != 0)
            {
                if (max == rNorm)
                {
                    hue = 60 * (((gNorm - bNorm) / delta) % 6);
                }
                else if (max == gNorm)
                {
                    hue = 60 * (((bNorm - rNorm) / delta) + 2);
                }
                else if (max == bNorm)
                {
                    hue = 60 * (((rNorm - gNorm) / delta) + 4);
                }

                if (hue < 0)
                {
                    hue += 360;
                }
            }

            // Saturation calculation
            double saturation = max == 0 ? 0 : (delta / max) * 100;

            // Value calculation
            double value = max * 100;

            return (hue, saturation, value);
        }

        /// <summary>
        /// Converts HSV to RGB
        /// </summary>
        private static (int r, int g, int b) HsvToRgb(double hue, double saturation, double value)
        {
            double s = saturation / 100.0;
            double v = value / 100.0;

            double c = v * s;
            double x = c * (1 - Math.Abs((hue / 60.0) % 2 - 1));
            double m = v - c;

            double r1, g1, b1;

            if (hue >= 0 && hue < 60)
            {
                r1 = c; g1 = x; b1 = 0;
            }
            else if (hue >= 60 && hue < 120)
            {
                r1 = x; g1 = c; b1 = 0;
            }
            else if (hue >= 120 && hue < 180)
            {
                r1 = 0; g1 = c; b1 = x;
            }
            else if (hue >= 180 && hue < 240)
            {
                r1 = 0; g1 = x; b1 = c;
            }
            else if (hue >= 240 && hue < 300)
            {
                r1 = x; g1 = 0; b1 = c;
            }
            else
            {
                r1 = c; g1 = 0; b1 = x;
            }

            int r = (int)Math.Round((r1 + m) * 255);
            int g = (int)Math.Round((g1 + m) * 255);
            int b = (int)Math.Round((b1 + m) * 255);

            return (r, g, b);
        }

        #endregion
    }
}
