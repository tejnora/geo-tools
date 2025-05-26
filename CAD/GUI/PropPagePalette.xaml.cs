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
        UIElement _lastControl;
        public PropPagePalette()
        {
            InitializeComponent();
        }
        public void Reset()
        {

        }
        public void Load(IDrawObject drawObject)
        {
            var propPageType = drawObject?.PropPageType;
            if (propPageType == null)
                return;
            if (!_propPageCache.TryGetValue(propPageType, out var guiControl))
            {
                guiControl = (UIElement)Activator.CreateInstance(propPageType);
                _propPageCache[propPageType] = guiControl;
            }

            if (_lastControl != guiControl)
            {
                if (_lastControl != null)
                {
                    _stack.Children.Remove(_lastControl);
                }
                _stack.Children.Add(guiControl);
                _lastControl = guiControl;
            }
            ((IPropPage)guiControl).Load(drawObject);
        }
    }
}
