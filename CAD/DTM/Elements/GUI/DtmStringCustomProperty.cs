using System;
using System.Windows;
using System.Windows.Controls;
using CAD.GUI;

namespace CAD.DTM.Elements.GUI
{
    public class DtmStringCustomProperty
        : IDtmCustomProperty
    {
        string _value;
        readonly Action<string> _updateValue;
        public DtmStringCustomProperty(string name, string value, Action<string> updateValue)
        {
            _value = value;
            _updateValue = updateValue;
            Name = name;
        }
        public string Name { get; }
        public Control GetEditControl()
        {
            var control = new TextBox
            {
                Text = _value,
                VerticalAlignment = VerticalAlignment.Center
            };
            control.TextChanged += Control_TextChanged;
            return control;
        }
        void Control_TextChanged(object sender, TextChangedEventArgs e)
        {
            var control = (TextBox)sender;
            _value = control.Text;
            _updateValue(_value);
        }
        public IPropPage PropPage { get; set; }
    }
}
