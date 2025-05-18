using CAD.Canvas;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CAD.GUI
{
    public partial class PropPagePalette
    : IDockableContent
    {
        Dictionary<Type, UIElement> _propPageCache = new Dictionary<Type, UIElement>();
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
            if (!_propPageCache.TryGetValue(propPageType, out var guiControl))
            {
                guiControl = (UIElement)Activator.CreateInstance(propPageType);
                _propPageCache[propPageType] = guiControl;
            }
            ((IPropPage)guiControl).Load(drawObject);
            _stack.Children.Add(guiControl);
        }
    }
}
