using System.Windows;
using GeoBase.Localization;

namespace GeoBase.Gui
{
    public class WindowBase : Window
    {
        public string LanguageNamespace
        {
            get { return (string)GetValue(LanguageNamespaceDependencyProperty.LanguageNamespaceProperty); }
            set { SetValue(LanguageNamespaceDependencyProperty.LanguageNamespaceProperty, value); }
        }
    }
}
