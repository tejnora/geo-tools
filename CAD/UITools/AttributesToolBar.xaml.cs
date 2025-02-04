using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using CAD.Canvas;
using CAD.Tools;
using CAD.Utils;
using CAD.Canvas.Layers;

namespace CAD.UITools
{
    public partial class AttributesToolBar : GeoCadToolBar
    {
                public static GeoCadRoutedCommand AddLayer = new GeoCadRoutedCommand("AddLayer", typeof(AttributesToolBar), false);
        public static GeoCadRoutedCommand RemoveLayer = new GeoCadRoutedCommand("RemoveLayer", typeof(AttributesToolBar), false);
        public static GeoCadRoutedCommand EditLayer = new GeoCadRoutedCommand("EditLayer", typeof(AttributesToolBar), false);
                        public AttributesToolBar()
        {
            InitializeComponent();
            DataContext = this;
        }
                        public override void Notify(NotificationType type, object additionData)
        {
            base.Notify(type, additionData);
            bool updatePropertyValues = false;
            switch (type)
            {
                case NotificationType.DocumentChanged:
                    {
                        IsEnabled = ToolBarManager.Document != null;
                        if (ToolBarManager.Document != null)
                        {
                            bool enable = !(ToolBarManager.Document.DataModel.ActiveLayer is VFKDrawingLayerMain);
                            _widthComboBox.IsEnabled = enable;
                            _colorComboBox.IsEnabled = enable;
                        }
                        updatePropertyValues = true;
                    } break;
            }
            if (updatePropertyValues)
            {
                RefillDrawingLayers();
                RefillColorCombo();
                RefillWidthCombo();
            }
        }
                        private ObservableCollection<NameObject<ICanvasLayer>> _layers;
        public ObservableCollection<NameObject<ICanvasLayer>> Layers
        {
            get { return _layers; }
            set { _layers = value; OnPropertyChanged("Layers"); }
        }
        private NameObject<ICanvasLayer> _selectedLayer;
        public NameObject<ICanvasLayer> SelectedLayer
        {
            get { return _selectedLayer; }
            set { _selectedLayer = value; OnPropertyChanged("SelectedLayer"); }
        }

        private ObservableCollection<NameObject<double>> _linesWidth;
        public ObservableCollection<NameObject<double>> LinesWidth
        {
            get { return _linesWidth; }
            set { _linesWidth = value; OnPropertyChanged("LinesWidth"); }
        }
        private NameObject<double> _selectedLineWidth;
        public NameObject<double> SelectedLineWidth
        {
            get { return _selectedLineWidth; }
            set
            {
                _selectedLineWidth = value;
                OnPropertyChanged("SelectedLineWidth");
                if (ToolBarManager.Document != null && value != null)
                    ToolBarManager.Document.DataModel.ActiveLayer.Width = value.Object * 0.001;
            }
        }

        private NameObjectTwo<System.Drawing.Color, Brush> _selectedColor;
        public NameObjectTwo<System.Drawing.Color, Brush> SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                OnPropertyChanged("SelectedColor");
                if (ToolBarManager.Document != null && value != null)
                    ToolBarManager.Document.DataModel.ActiveLayer.Color = value.Object;
            }
        }

        private ObservableCollection<NameObjectTwo<System.Drawing.Color, Brush>> _colors;
        public ObservableCollection<NameObjectTwo<System.Drawing.Color, Brush>> Colors
        {
            get { return _colors; }
            set { _colors = value; OnPropertyChanged("Colors"); }
        }
                        private void RefillColorCombo()
        {
            Colors = new ObservableCollection<NameObjectTwo<System.Drawing.Color, Brush>>();
            if (ToolBarManager.Document != null && IsEnabled)
            {
                foreach (var c in ToolBarManager.Document.DataModel.getColors())
                {
                    Colors.Add(new NameObjectTwo<System.Drawing.Color, Brush>(c.Name, c, new SolidColorBrush(WPFToFormConverter.getWPFColor(c))));
                }
            }
            UpdateSelectedColor();
        }
        private void UpdateSelectedColor()
        {
            SelectedColor = null;
            if (ToolBarManager.Document != null && IsEnabled)
            {
                foreach (NameObjectTwo<System.Drawing.Color, Brush> c in _colorComboBox.Items)
                {
                    if (ToolBarManager.Document.DataModel.ActiveLayer.Color == c.Object)
                    {
                        SelectedColor = c;
                        break;
                    }
                }
            }
        }
        private void RefillWidthCombo()
        {
            LinesWidth = new ObservableCollection<NameObject<double>>();
            if (ToolBarManager.Document != null && IsEnabled)
            {
                foreach (var w in ToolBarManager.Document.DataModel.getWidths())
                {
                    LinesWidth.Add(new NameObject<double>(" " + w + " mm", w * 1000));
                }
            }
            UpdateSelectedWidth();
        }
        private void UpdateSelectedWidth()
        {
            SelectedLineWidth = null;
            if (ToolBarManager.Document != null && IsEnabled)
            {
                foreach (var width in _linesWidth)
                {
                    var layerWidth = Math.Round(ToolBarManager.Document.DataModel.ActiveLayer.Width, 6);
                    var w = Math.Round(width.Object * 0.001, 6);
                    if (layerWidth == w)
                    {
                        SelectedLineWidth = width;
                        break;
                    }
                }
            }
        }
        private void RefillDrawingLayers()
        {
            Layers = new ObservableCollection<NameObject<ICanvasLayer>>();
            if (ToolBarManager.Document != null && IsEnabled)
            {
                Int32 index = 0;
                foreach (ICanvasLayer layer in ToolBarManager.Document.DataModel.Layers)
                {
                    Layers.Add(new NameObject<ICanvasLayer>(string.Format("({0}) - {1}", index, layer.Name), layer));
                    if (layer == ToolBarManager.Document.DataModel.ActiveLayer)
                        SelectedLayer = Layers[index];
                    index++;
                }
            }
        }
                        private void OnAddLayer(object sender, ExecutedRoutedEventArgs e)
        {
            string id = ToolBarManager.Document.DataModel.FindUniqueId();
            ICanvasLayer dl = new DrawingLayer(id, id, SelectedColor.Object,(float)SelectedLineWidth.Object);
            GUI.DrawLayerProp dlp = new GUI.DrawLayerProp(dl);
            dlp.ShowDialog();
            if (dlp.ResultAction == "Ok")
            {
                ToolBarManager.Document.DataModel.AddLayer(dl);
                Layers.Add(new NameObject<ICanvasLayer>(string.Format("({0}) - {1}", Layers.Count - 1, dl.Name), dl));
                SelectedLayer = Layers[Layers.Count - 1];
                UpdateSelectedWidth();
                UpdateSelectedColor();
            }
        }
        private void OnCanRemove(object sender, CanExecuteRoutedEventArgs e)
        {
            OnCanExecute(sender, e);
            if (e.CanExecute && SelectedLayer != null)
            {
                if (SelectedLayer.Object is VFKDrawingLayerMain)
                    e.CanExecute = false;
                else
                {
                    e.CanExecute = false;
                    int counter = 0;
                    foreach (var layer in ToolBarManager.Document.DataModel.Layers)
                    {
                        if (!(layer is VFKDrawingLayerMain))
                            counter++;
                        if (counter == 2)
                        {
                            e.CanExecute = true;
                            break;
                        }
                    }
                }
            }
        }
        private void OnRemoveLayer(object sender, ExecutedRoutedEventArgs e)
        {
            ToolBarManager.Document.DataModel.RemoveLayer(SelectedLayer.Object);
            Layers.Remove(SelectedLayer);
            SelectedLayer = Layers[0];
        }
        private void OnEditLayer(object sender, ExecutedRoutedEventArgs e)
        {
            GUI.DrawLayerProp dlp = new GUI.DrawLayerProp(SelectedLayer.Object);
            dlp.ShowDialog();
            if (dlp.ResultAction == "Ok")
            {
                RefillDrawingLayers();
                ToolBarManager.Document.CanvasCommand.InvalidateAll();
            }
        }
            }

}
