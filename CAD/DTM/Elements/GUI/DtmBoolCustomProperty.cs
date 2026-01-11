using System;
using System.Windows;
using System.Windows.Controls;
using CAD.GUI;

namespace CAD.DTM.Elements.GUI
{
    public class DtmBoolCustomProperty
        : IDtmCustomProperty
    {
        bool _value;
        readonly Action<bool> _updateAction;
        public DtmBoolCustomProperty(string name, bool value, Action<bool> updateAction)
        {
            _value = value;
            _updateAction = updateAction;
            Name = name;
        }
        public string Name { get; }
        public Control GetEditControl()
        {
            var control = new CheckBox { VerticalAlignment = VerticalAlignment.Center };
            control.IsChecked = _value;
            control.Checked += Control_Checked;
            control.Unchecked += Control_Checked;
            return control;
        }

        void Control_Checked(object sender, RoutedEventArgs e)
        {
            var control = (CheckBox)sender;
            _value = control.IsChecked.Value;
            _updateAction(_value);
        }

        public IPropPage PropPage { get; set; }
    }
}
