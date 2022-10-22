using System.Windows.Controls;

namespace GeoHelper.Tables
{
    public interface IEditMenu
    {
        void FillMenuItems(MenuItem menu);
        string GetMenuCaption();
    }
}