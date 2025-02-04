using System;
using System.Diagnostics;
using System.Globalization;

namespace VFK.GUI
{
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    
    public class DatabindingDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }


    public class LabelTextBoxColumn : ExtendedTextBoxColumn
    {
        
        private void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
        {
            var style = PickStyle(isEditing, defaultToElementStyle);
            if (style != null)
                element.Style = style;
        }

        private Style PickStyle(bool isEditing, bool defaultToElementStyle)
        {
            var style = isEditing ? EditingElementStyle : ElementStyle;
            if (isEditing && defaultToElementStyle && (style == null))
                style = ElementStyle;
            return style;
        }

        private void ApplyBinding(DependencyObject target, DependencyProperty property)
        {
            var binding = Binding;
            if (binding != null)
                BindingOperations.SetBinding(target, property, binding);
            else
                BindingOperations.ClearBinding(target, property);
        }

        
        
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var label = new Label
                        {
                            HorizontalAlignment = this.HorizontalAlignment,
                            VerticalAlignment = this.VerticalAlignment
                        };

            ApplyStyle(false, false, label);
            ApplyBinding(label, ContentControl.ContentProperty);

            return label;
        }

            }
}
