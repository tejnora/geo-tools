using System.Windows;
using System.Windows.Controls;

namespace GeoBase.Gui.Buttons
{
    public class ButtonBase : Button
    {
        public ButtonBase()
        {
            Margin = new Thickness(2);
            Padding = new Thickness(0, 1, 0, 1);
            MinWidth = 80;
        }
    }
}
