using CAD.Canvas;
using System;
using System.Windows;

namespace CAD.GUI
{
    public partial class PropPagePalette
    : IDockableContent
    {
        public PropPagePalette()
        {
            InitializeComponent();
        }
        public void Reset()
        {

        }
        public void Load(IDrawObject drawObject)
        {
            _stack.Children.Clear();
            var propPageType = drawObject?.PropPageType;
            if (propPageType == null)
                return;
            var guiControl = (UIElement)Activator.CreateInstance(propPageType);
            ((IPropPage)guiControl).Load(drawObject);
            _stack.Children.Add(guiControl);
        }
    }
}
