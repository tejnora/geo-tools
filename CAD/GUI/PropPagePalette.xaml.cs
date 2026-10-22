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
        GridLength _validationHeight = new GridLength(1, GridUnitType.Star);
        public void SetValidationDocument(Document document)
        {
            _validationPanel.SetDocument(document);
            var visible = document?.IsDtmValidationVisible == true;
            _validationPanel.Visibility = _validationSplitter.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (visible && _validationRow.Height.Value == 0)
                _validationRow.Height = _validationHeight;
            else if (!visible)
            {
                if (_validationRow.Height.Value > 0) _validationHeight = _validationRow.Height;
                _validationRow.Height = new GridLength(0);
            }
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
