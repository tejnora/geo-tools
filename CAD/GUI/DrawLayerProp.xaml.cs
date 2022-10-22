using System;
using System.Windows;
using System.Windows.Controls;
using CAD.Canvas;

namespace CAD.GUI
{
    /// <summary>
    /// Interaction logic for DrawLayerProp.xaml
    /// </summary>
    public partial class DrawLayerProp : Window
    {
        private ICanvasLayer _drawingLayer;
        public DrawLayerProp(ICanvasLayer aDrawingLayer)
        {
            _drawingLayer=aDrawingLayer;
            InitializeComponent();
            _layerName.Text = _drawingLayer.Name;
            _enable.IsChecked = _drawingLayer.Enabled;
            _visble.IsChecked = _drawingLayer.Visible;
        }

        string _resultAction = string.Empty;
        private void onClick(object aObject, EventArgs e)
        {
            Button bt = (Button)aObject;
            _resultAction = (string)bt.Tag;
            if (_resultAction == "OK")
            {
                _drawingLayer.Name = _layerName.Text;
                if(_enable.IsChecked.HasValue)
                    _drawingLayer.Enabled =(bool) _enable.IsChecked;
                if (_visble.IsChecked.HasValue)
                    _drawingLayer.Visible = (bool)_visble.IsChecked;
            }
            Close();
        }

        public string ResultAction
        {
            get { return _resultAction; }
        }
    }
}
