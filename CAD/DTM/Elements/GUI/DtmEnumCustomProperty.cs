using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using CAD.GUI;

namespace CAD.DTM.Elements.GUI
{
    public class DtmEnumCustomProperty<T>
        : IDtmCustomProperty
    {
        T _selectedValue;
        readonly Action<T> _updateValue;

        public DtmEnumCustomProperty(string name, T selectedValue, Action<T> updateValue)
        {
            _selectedValue = selectedValue;
            _updateValue = updateValue;
            Name = name;
        }
        public string Name { get; }
        public Control GetEditControl()
        {
            var comboBox = new ComboBox();
            var values = Enum.GetValues(typeof(T));
            foreach (var value in values)
            {
                var stringValue = value.ToString();
                var spittedValues = Regex.Split(stringValue, @"(?<!^)(?=[A-Z])");
                stringValue = string.Join(" ", spittedValues);
                comboBox.Items.Add(new ComboBoxItem { Tag = value, Content = stringValue });
            }
            comboBox.SelectedValuePath = "Tag";
            comboBox.SelectedValue = _selectedValue;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            return comboBox;
        }
        public IPropPage PropPage { get; set; }
        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedValue = (T)((ComboBox)sender).SelectedValue;
            _updateValue(_selectedValue);
            PropPage.InvalidateCanvas();
        }
    }
}
