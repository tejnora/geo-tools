using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CAD.GUI;

namespace CAD.DTM.Elements.GUI
{
    public class DtmUIntCustomProperty
        : IDtmCustomProperty
    {
        uint _value;
        readonly Action<uint> _updateValue;

        public DtmUIntCustomProperty(string name, uint value, Action<uint> updateValue)
        {
            _value = value;
            _updateValue = updateValue;
            Name = name;
        }
        public string Name { get; }
        public Control GetEditControl()
        {
            var control = new IntegerTextBox
            {
                Text = _value.ToString(),
                VerticalAlignment = VerticalAlignment.Center
            };
            control.TextChanged += Control_TextChanged;
            return control;
        }

        void Control_TextChanged(object sender, TextChangedEventArgs e)
        {
            var control = (IntegerTextBox)sender;
            if (!uint.TryParse(control.Text, out var value))
            {
                value = 0;
            }
            if (_value == value)
                return;
            _value = value;
            _updateValue(_value);
        }
        public IPropPage PropPage { get; set; }
    }
}
