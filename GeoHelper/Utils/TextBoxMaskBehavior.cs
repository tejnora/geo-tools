using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace CAD.Utils
{
    #region Documentation Tags
    /// <summary>
    ///     WPF Maskable TextBox class. Just specify the TextBoxMaskBehavior.Mask attached property to a TextBox. 
    ///     It protect your TextBox from unwanted non numeric symbols and make it easy to modify your numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Class Information:
    ///	    <list type="bullet">
    ///         <item name="authors">Authors: Ruben Hakopian</item>
    ///         <item name="date">February 2009</item>
    ///         <item name="originalURL">http://www.rubenhak.com/?p=8</item>
    ///     </list>
    /// </para>
    /// </remarks>
    #endregion
    public class TextBoxMaskBehavior
    {
        #region MinimumValue Property

        public static double GetMinimumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MinimumValueProperty);
        }

        public static void SetMinimumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MinimumValueProperty, value);
        }

        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.RegisterAttached(
                "MinimumValue",
                typeof(double),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(double.NaN, MinimumValueChangedCallback)
                );

        private static void MinimumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            ValidateTextBox(_this);
        }
        #endregion

        #region MaximumValue Property

        public static double GetMaximumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MaximumValueProperty);
        }

        public static void SetMaximumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MaximumValueProperty, value);
        }

        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.RegisterAttached(
                "MaximumValue",
                typeof(double),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(double.NaN, MaximumValueChangedCallback)
                );

        private static void MaximumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            ValidateTextBox(_this);
        }
        #endregion

        #region Mask Property

        public static MaskType GetMask(DependencyObject obj)
        {
            return (MaskType)obj.GetValue(MaskProperty);
        }

        public static void SetMask(DependencyObject obj, MaskType value)
        {
            obj.SetValue(MaskProperty, value);
        }

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.RegisterAttached(
                "Mask",
                typeof(MaskType),
                typeof(TextBoxMaskBehavior),
                new FrameworkPropertyMetadata(MaskChangedCallback)
                );

        private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TextBox)
            {
                (e.OldValue as TextBox).PreviewTextInput -= TextBox_PreviewTextInput;
                DataObject.RemovePastingHandler((e.OldValue as TextBox), (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            TextBox _this = (d as TextBox);
            if (_this == null)
                return;

            if ((MaskType)e.NewValue != MaskType.Text)
            {
                _this.PreviewTextInput += TextBox_PreviewTextInput;
                DataObject.AddPastingHandler(_this, (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            ValidateTextBox(_this);
        }

        #endregion

        #region Private Static Methods

        private static void ValidateTextBox(TextBox _this)
        {
            if (GetMask(_this) != MaskType.Text)
            {
                _this.Text = ValidateValue(GetMask(_this), _this.Text, GetMinimumValue(_this), GetMaximumValue(_this));
            }
        }

        private static void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            string clipboard = e.DataObject.GetData(typeof(string)) as string;
            clipboard = ValidateValue(GetMask(_this), clipboard, GetMinimumValue(_this), GetMaximumValue(_this));
            if (!string.IsNullOrEmpty(clipboard))
            {
                _this.Text = clipboard;
            }
            e.CancelCommand();
            e.Handled = true;
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            string insertedtext = e.Text;
            if (insertedtext == ",")
                insertedtext = ".";
            if(GetMask(_this)==MaskType.Text)
                return;
            bool isValid = IsSymbolValid(GetMask(_this), insertedtext);
            e.Handled = !isValid;
            if (isValid)
            {
                int caret = _this.CaretIndex;
                string text = _this.Text;
                bool textInserted = false;
                int selectionLength = 0;

                if (_this.SelectionLength > 0)
                {
                    text = text.Substring(0, _this.SelectionStart) +
                            text.Substring(_this.SelectionStart + _this.SelectionLength);
                    caret = _this.SelectionStart;
                }

                if (insertedtext == ".")
                {
                    while (true)
                    {
                        int ind = text.IndexOf(".");
                        if (ind == -1)
                            break;

                        text = text.Substring(0, ind) + text.Substring(ind + 1);
                        if (caret > ind)
                            caret--;
                    }

                    if (caret == 0)
                    {
                        text = "0" + text;
                        caret++;
                    }
                    else
                    {
                        if (caret == 1 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign)
                        {
                            text = NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1);
                            caret++;
                        }
                    }

                    if (caret == text.Length)
                    {
                        selectionLength = 1;
                        textInserted = true;
                        text = text + "." + "0";
                        caret++;
                    }
                }
                else if (insertedtext == NumberFormatInfo.CurrentInfo.NegativeSign)
                {
                    textInserted = true;
                    if (_this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                    {
                        text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);
                        if (caret != 0)
                            caret--;
                    }
                    else
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + _this.Text;
                        caret++;
                    }
                }

                if (!textInserted)
                {
                    text = text.Substring(0, caret) + insertedtext +
                        ((caret < _this.Text.Length) ? text.Substring(caret) : string.Empty);

                    caret++;
                }
                if (text != NumberFormatInfo.CurrentInfo.NegativeSign)
                {
                    try
                    {
                        double val = Convert.ToDouble(text, CultureInfo.InvariantCulture);
                        double newVal = ValidateLimits(GetMinimumValue(_this), GetMaximumValue(_this), val);
                        if (val != newVal)
                        {
                            text = newVal.ToString();
                        }
                        else if (val == 0)
                        {
                            if (!text.Contains(".") && !text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                                text = "0";
                        }
                    }
                    catch
                    {
                        text = "0";
                    }
                }
                while (text.Length > 1 && text[0] == '0' && string.Empty + text[1] != ".")
                {
                    text = text.Substring(1);
                    if (caret > 0)
                        caret--;
                }

                while (text.Length > 2 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign && text[1] == '0' && string.Empty + text[2] != ".")
                {
                    text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2);
                    if (caret > 1)
                        caret--;
                }

                if (caret > text.Length)
                    caret = text.Length;

                _this.Text = text;
                _this.CaretIndex = caret;
                _this.SelectionStart = caret;
                _this.SelectionLength = selectionLength;
                e.Handled = true;
            }
        }

        private static string ValidateValue(MaskType mask, string value, double min, double max)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.Trim();
            switch (mask)
            {
                case MaskType.Integer:
                    try
                    {
                        Convert.ToInt64(value);
                        return value;
                    }
                    catch
                    {
                    }
                    return string.Empty;

                case MaskType.Decimal:
                    try
                    {
                        Convert.ToDouble(value,CultureInfo.InvariantCulture);

                        return value;
                    }
                    catch
                    {
                    }
                    return string.Empty;
            }

            return value;
        }

        private static double ValidateLimits(double min, double max, double value)
        {
            if (!min.Equals(double.NaN))
            {
                if (value < min)
                    return min;
            }

            if (!max.Equals(double.NaN))
            {
                if (value > max)
                {
                    value = Math.Round(value/10-0.5,0);
                    return value;
                }
            }

            return value;
        }

        private static bool IsSymbolValid(MaskType mask, string str)
        {
            switch (mask)
            {
                case MaskType.Text:
                    return true;

                case MaskType.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                case MaskType.Decimal:
                    if (str == "." ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;
            }

            if (mask.Equals(MaskType.Integer) || mask.Equals(MaskType.Decimal))
            {
                foreach (char ch in str)
                {
                    if (!Char.IsDigit(ch))
                        return false;
                }

                return true;
            }

            return false;
        }

        #endregion
    }

    public enum MaskType
    {
        Text,
        Integer,
        Decimal
    }
}