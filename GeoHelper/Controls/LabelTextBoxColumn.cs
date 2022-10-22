using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GeoHelper.Tabulky;

namespace GeoHelper.Controls
{
    public class LabelTextBoxColumn : ExtendedTextBoxColumn
    {
        void ApplyStyle(bool isEditing, bool defaultToElementStyle,
                        FrameworkElement element)
        {
            Style style = PickStyle(isEditing, defaultToElementStyle);
            if (style != null)
                element.Style = style;
        }

        Style PickStyle(bool isEditing, bool defaultToElementStyle)
        {
            Style style = isEditing ? EditingElementStyle : ElementStyle;
            if (isEditing && defaultToElementStyle && (style == null))
                style = ElementStyle;
            return style;
        }

        void ApplyBinding(DependencyObject target,
                          DependencyProperty property)
        {
            BindingBase binding = Binding;
            if (binding != null)
                BindingOperations.SetBinding(target, property, binding);
            else
                BindingOperations.ClearBinding(target, property);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell,
                                                            object dataItem)
        {
            var label = new Label {HorizontalAlignment = HorizontalAlignment, VerticalAlignment = VerticalAlignment};

            ApplyStyle(false, false, label);
            ApplyBinding(label, ContentControl.ContentProperty);

            return label;
        }
    }
}