using System.ComponentModel;
using System.Windows.Controls;
using GeoBase.Localization;

namespace GeoBase.Gui
{
    public class UserControlBase : UserControl, INotifyPropertyChanged
    {
        public string LanguageNamespace
        {
            get { return (string)GetValue(LanguageNamespaceDependencyProperty.LanguageNamespaceProperty); }
            set { SetValue(LanguageNamespaceDependencyProperty.LanguageNamespaceProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
