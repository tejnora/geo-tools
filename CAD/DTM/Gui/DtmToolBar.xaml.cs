using System;
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
            DtmLineSetting = new ObservableCollection<string>();
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
                UpdateLineSetting();
                UpdateDrawingLayer();
            }
        }
        public readonly PropertyData _dtmLineSettings = RegisterProperty("DtmLineSetting", typeof(ObservableCollection<string>), null);
        public ObservableCollection<string> DtmLineSetting
        {
            get => GetValue<ObservableCollection<string>>(_dtmLineSettings);
            set => SetValue(_dtmLineSettings, value);
        }

        public readonly PropertyData _dtmLineSettingSelected = RegisterProperty("DtmLineSettingSelected", typeof(string), null);
        public string DtmLineSettingSelected
        {
            get => GetValue<string>(_dtmLineSettingSelected);
            set
            {
                SetValue(_dtmLineSettingSelected, value);
                UpdateDrawingLayer();
            }
        }

        public bool DtmLineSettingEnabled => DtmLineSetting.Count > 0;

        public readonly PropertyData _dtmPointElements = RegisterProperty("DtmPointElements", typeof(ObservableCollection<string>), null);
        public ObservableCollection<string> DtmPointElements
        {
            get => GetValue<ObservableCollection<string>>(_dtmPointElements);
            set => SetValue(_dtmPointElements, value);
        }

        public readonly PropertyData _dtmPointElementSelected = RegisterProperty("DtmPointElementSelected", typeof(string), null);
        public string DtmPointElementSelected
        {
            get => GetValue<string>(_dtmPointElementSelected);
            set
            {
                SetValue(_dtmPointElementSelected, value);
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
                UpdateLineSetting();
                UpdateDrawingLayer();
            }
        }
        void UpdateLineSetting()
        {
            DtmLineSetting.Clear();
            DtmLineSettingSelected = "";
            var element = DtmConfigurationSingleton.Instance.CreateType(DtmLineElementSelected);
            if (element != null)
            {
                foreach (var setting in element.Settings)
                {
                    DtmLineSetting.Add(setting);
                }
                if (DtmLineSetting.Count > 0)
                {
                    DtmLineSettingSelected = DtmLineSetting[0];
                }
            }
            OnPropertyChanged("DtmLineSettingEnabled");
        }
        void UpdateDrawingLayer()
        {
            if (!(_dataModel?.ActiveLayer is DtmDrawingLayerMain dtmLayout)) return;
            dtmLayout.DtmLineElementSelected = new Tuple<string, string>(DtmLineElementSelected, DtmLineSettingSelected);
            dtmLayout.DtmPointSelected = new Tuple<string, string>(DtmPointElementSelected, "");
        }
    }

    public partial class DtmToolBar : GeoCadToolBar
    {
        DtmToolBarCtx _ctx;
        public static GeoCadRoutedCommand DtmMultiLine = new GeoCadRoutedCommand("DtmMultiLine", typeof(DtmToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand DtmPoint = new GeoCadRoutedCommand("DtmPoint", typeof(DtmToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public DtmToolBar()
        {
            InitializeComponent();
            _ctx = new DtmToolBarCtx
            {
                DtmLineElements = new ObservableCollection<string>(),
                DtmPointElements = new ObservableCollection<string>()
            };
            foreach (var element in DtmConfigurationSingleton.Instance.ElementSetting)
            {
                switch (element.Value.ElementType)
                {
                    case DtmElementType.Linie:
                        _ctx.DtmLineElements.Add(element.Key);
                        break;
                    case DtmElementType.Bod:
                        _ctx.DtmPointElements.Add(element.Key);
                        break;
                }
            }

            _ctx.DtmLineElementSelected = _ctx.DtmLineElements[0];
            _ctx.DtmPointElementSelected = _ctx.DtmPointElements[1];
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
