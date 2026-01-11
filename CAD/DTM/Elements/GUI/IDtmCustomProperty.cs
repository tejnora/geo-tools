
using System.Windows.Controls;
using CAD.GUI;

namespace CAD.DTM.Elements.GUI
{
    public interface IDtmCustomProperty
    {
        string Name { get; }
        Control GetEditControl();
        IPropPage PropPage { get; set; }
    }
}
