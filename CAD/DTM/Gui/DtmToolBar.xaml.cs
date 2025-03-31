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
            if (_dataModel?.ActiveLayer is DtmDrawingLayerMain dtmLayout)
            {
                dtmLayout.DtmLineElementSelected = new Tuple<string, string>(DtmLineElementSelected, DtmLineSettingSelected);
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
