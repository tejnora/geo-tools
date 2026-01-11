using System.Windows.Controls;
using CAD.GUI;

namespace CAD.DTM.Elements.GUI
{
    public class DtmReadonlyCustomProperty
    : IDtmCustomProperty
    {
        string _value;
        public DtmReadonlyCustomProperty(string name, string value)
        {
            Name = name;
            _value = value;
        }
        public string Name { get; }
        public Control GetEditControl()
        {
            return new Label { Content = _value };
        }
        public IPropPage PropPage { get; set; }
    }
}
