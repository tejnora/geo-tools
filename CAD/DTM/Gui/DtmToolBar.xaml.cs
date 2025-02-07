using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using CAD.Canvas;
using CAD.DTM.Configuration;
using CAD.UITools;
using GeoBase.Utils;

namespace CAD.DTM.Gui
{
    class DtmToolBarCtx : DataObjectBase<DtmToolBarCtx>
    {
        public DtmToolBarCtx() : base(null, new StreamingContext())
        {
        }
        public readonly PropertyData _dtmLineElements = RegisterProperty("DtmLineElements", typeof(ObservableCollection<string>), null);
        public ObservableCollection<string> DtmLineElements
        {
            get => GetValue<ObservableCollection<string>>(_dtmLineElements);
            set => SetValue(_dtmLineElements, value);
        }
        public readonly PropertyData _dtmLineElementSelected = RegisterProperty("DtmLineElementSelected", typeof(string), null);
        public string DtmLineElementSelected
        {
            get => GetValue<string>(_dtmLineElementSelected);
            set
            {
                SetValue(_dtmLineElementSelected, value);
                UpdateDrawingLayer();
            }
        }
        DataModel _dataModel;
        public DataModel DataModel
        {
            get => _dataModel;
            set
            {
                _dataModel = value;
                UpdateDrawingLayer();
            }
        }
        void UpdateDrawingLayer()
        {
            if (_dataModel?.ActiveLayer is DtmDrawingLayerMain dtmLayout)
            {
                dtmLayout.DtmLineElementSelected = DtmLineElementSelected;
            }
        }
    }

    public partial class DtmToolBar : GeoCadToolBar
    {
        DtmToolBarCtx _ctx;
        public static GeoCadRoutedCommand DtmMultiLine = new GeoCadRoutedCommand("DtmMultiLine", typeof(DtmToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public DtmToolBar()
        {
            InitializeComponent();
            _ctx = new DtmToolBarCtx
            {
                DtmLineElements = new ObservableCollection<string>(),
            };
            foreach (var element in DtmConfigurationSingleton.Instance.ElementSetting)
            {
                if (element.Value.ElementType != ElementType.Line)
                    continue;
                _ctx.DtmLineElements.Add(element.Key);
            }

            _ctx.DtmLineElementSelected = _ctx.DtmLineElements[0];
            DataContext = _ctx;
        }
        public override void Notify(NotificationType type, object additionData)
        {
            base.Notify(type, additionData);
            switch (type)
            {
                case NotificationType.DocumentChanged:
                    {
                        IsEnabled = ToolBarManager.Document?.DataModel.ActiveLayer is DtmDrawingLayerMain;
                        if (IsEnabled)
                        {
                            _ctx.DataModel = ToolBarManager.Document.DataModel;
                        }
                    }
                    break;
            }

        }
    }
}
