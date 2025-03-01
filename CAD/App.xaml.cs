﻿using System.Data.SqlTypes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GeoBase.Utils;
using System.Globalization;

namespace CAD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        public static int TracePaint = 1;
        private CrashReporter _crashReporter;
        protected override void OnStartup(StartupEventArgs e)
        {
            // Select the text in a TextBox when it recieves focus.
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent,
                new RoutedEventHandler(SelectAllText));
            _crashReporter = new CrashReporter(Current);
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
                var textBox = (TextBox)parent;
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
    }
}
