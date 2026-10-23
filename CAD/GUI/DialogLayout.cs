using System.Windows;
using System.Windows.Controls;
using GeoBase.Gui;

namespace CAD.GUI;

internal static class DialogLayout
{
    internal static void ApplyButtonBar(DialogBase dialog)
    {
        foreach (var child in dialog.CustomButtons)
        {
            if (child is not Button button) continue;
            button.MinWidth = 88;
            button.Padding = new Thickness(12, 4, 12, 4);
            button.Margin = new Thickness(8, 0, 0, 0);
            if (button.Parent is FrameworkElement bar)
                bar.Margin = new Thickness(16, 8, 16, 16);
        }
    }
}