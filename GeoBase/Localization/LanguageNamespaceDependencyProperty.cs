using System.Windows;

namespace GeoBase.Localization
{
    public static class LanguageNamespaceDependencyProperty
    {
        public static readonly DependencyProperty LanguageNamespaceProperty =

            DependencyProperty.Register("LanguageNamespace", typeof(string), typeof(FrameworkElement), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));

    }
}
