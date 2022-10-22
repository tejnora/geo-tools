using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeoHelper
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Select the text in a TextBox when it recieves focus.
            EventManager.RegisterClassHandler(typeof (TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
                                              new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
            EventManager.RegisterClassHandler(typeof (TextBox), UIElement.GotKeyboardFocusEvent,
                                              new RoutedEventHandler(SelectAllText));
            EventManager.RegisterClassHandler(typeof (TextBox), Control.MouseDoubleClickEvent,
                                              new RoutedEventHandler(SelectAllText));
            SuppressFalseDataBindingTraceEvents();
            base.OnStartup(e);
        }

        void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox) parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }

        [Conditional("DEBUG")]
        internal static void SuppressFalseDataBindingTraceEvents()
        {
            //todo!!
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
            //dynamic menu throw binding error => http://blog.jtango.net/building-composable-wpf-menus-with-compositecollection
        }
    }
}