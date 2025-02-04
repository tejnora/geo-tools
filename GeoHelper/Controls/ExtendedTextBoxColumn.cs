using System;
using System.Windows;
using System.Windows.Controls;

namespace GeoHelper.Tabulky
{

    
    
    public class ExtendedTextBoxColumn : DataGridTextColumn
    {
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalAlignment",
                typeof (HorizontalAlignment),
                typeof (ExtendedTextBoxColumn),
                new UIPropertyMetadata(HorizontalAlignment.Stretch));

        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment) GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(
                "VerticalAlignment",
                typeof (VerticalAlignment),
                typeof (ExtendedTextBoxColumn),
                new UIPropertyMetadata(VerticalAlignment.Stretch));

        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment) GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        TextAlignment GetTextAlignment()
        {
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    return TextAlignment.Center;
                case HorizontalAlignment.Left:
                    return TextAlignment.Left;
                case HorizontalAlignment.Right:
                    return TextAlignment.Right;
                case HorizontalAlignment.Stretch:
                    return TextAlignment.Justify;
                default:
                    throw new ArgumentOutOfRangeException("HorizontalAlignment", "Unsupported alignment type!");
            }
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            FrameworkElement element = base.GenerateElement(cell, dataItem);

            element.HorizontalAlignment = HorizontalAlignment;
            element.VerticalAlignment = VerticalAlignment;

            return element;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var textBox = (TextBox) base.GenerateEditingElement(cell, dataItem);

            textBox.TextAlignment = GetTextAlignment();
            textBox.VerticalContentAlignment = VerticalAlignment;

            return textBox;
        }
    }
}