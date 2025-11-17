using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LayoutDesigner.Behaviors
{
    /// <summary>
    /// Attached behavior that restricts TextBox input to numeric values only
    /// Supports integers and decimals with optional negative values
    /// </summary>
    public static class NumericInputBehavior
    {
        // Regex for validating numeric input (integers and decimals, positive and negative)
        private static readonly Regex _numericRegex = new Regex(@"^-?\d*\.?\d*$", RegexOptions.Compiled);

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(NumericInputBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.PreviewTextInput += OnPreviewTextInput;
                    DataObject.AddPastingHandler(textBox, OnPasting);
                }
                else
                {
                    textBox.PreviewTextInput -= OnPreviewTextInput;
                    DataObject.RemovePastingHandler(textBox, OnPasting);
                }
            }
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Get the text that would result from this input
            var fullText = GetProposedText(textBox, e.Text);

            // Allow empty string (user can delete all text)
            if (string.IsNullOrEmpty(fullText))
            {
                return;
            }

            // Check if the resulting text would be valid
            e.Handled = !IsValidNumericInput(fullText);
        }

        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = e.DataObject.GetData(typeof(string)) as string;
                if (!string.IsNullOrEmpty(pastedText))
                {
                    var fullText = GetProposedText(textBox, pastedText);
                    if (!IsValidNumericInput(fullText))
                    {
                        e.CancelCommand();
                    }
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static string GetProposedText(TextBox textBox, string newText)
        {
            var text = textBox.Text;
            if (textBox.SelectionLength > 0)
            {
                // Replace selected text
                text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);
            }

            // Insert new text at caret position
            var caretIndex = textBox.SelectionStart;
            return text.Insert(caretIndex, newText);
        }

        private static bool IsValidNumericInput(string text)
        {
            // Empty or just a minus sign is valid (intermediate state)
            if (string.IsNullOrEmpty(text) || text == "-" || text == ".")
            {
                return true;
            }

            // Check against regex
            return _numericRegex.IsMatch(text);
        }
    }
}
