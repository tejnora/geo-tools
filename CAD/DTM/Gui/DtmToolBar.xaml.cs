using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using CAD.Canvas;
using CAD.DTM.Configuration;
using CAD.UITools;
using GeoBase;
using GeoBase.Utils;
using System.Windows.Input;

namespace CAD.DTM.Gui
{
    class DtmToolBarCtx : DataObjectBase<DtmToolBarCtx>
    {
        public DtmToolBarCtx() : base(null, new StreamingContext())
        {
            DtmLineSetting = new ObservableCollection<string>();
            DtmPointSetting = new ObservableCollection<string>();
        }
        public readonly PropertyData _dtmLineElements = RegisterProperty("DtmLineElements", typeof(ListCollectionView), null);
        public ListCollectionView DtmLineElements
        {
            get => GetValue<ListCollectionView>(_dtmLineElements);
            set => SetValue(_dtmLineElements, value);
        }
        public readonly PropertyData _dtmLineElementSelected = RegisterProperty("DtmLineElementSelected", typeof(DtmLineElement), null);
        public DtmLineElement DtmLineElementSelected
        {
            get => GetValue<DtmLineElement>(_dtmLineElementSelected);
            set
            {
                SetValue(_dtmLineElementSelected, value);
                _updateToolSettingLock = true;
                UpdateLineSetting();
                _updateToolSettingLock = false;
                UpdateToolSetting();
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
                UpdateToolSetting();
            }
        }

        public bool DtmLineSettingEnabled => DtmLineSetting.Count > 0;


        public readonly PropertyData _dtmPointElements = RegisterProperty("DtmPointElements", typeof(ListCollectionView), null);
        public ListCollectionView DtmPointElements
        {
            get => GetValue<ListCollectionView>(_dtmPointElements);
            set => SetValue(_dtmPointElements, value);
        }
        public readonly PropertyData _dtmPointElementSelected = RegisterProperty("DtmPointElementSelected", typeof(DtmPointElement), null);
        public DtmPointElement DtmPointElementSelected
        {
            get => GetValue<DtmPointElement>(_dtmPointElementSelected);
            set
            {
                SetValue(_dtmPointElementSelected, value);
                _updateToolSettingLock = true;
                UpdatePointSetting();
                _updateToolSettingLock = false;
                UpdateToolSetting();
            }
        }
        public readonly PropertyData _dtmPointSettingSelected = RegisterProperty("DtmPointSettingSelected", typeof(string), null);
        public string DtmPointSettingSelected
        {
            get => GetValue<string>(_dtmPointSettingSelected);
            set
            {
                SetValue(_dtmPointSettingSelected, value);
                UpdateToolSetting();
            }
        }

        public readonly PropertyData _dtmPointSettings = RegisterProperty("DtmPointSetting", typeof(ObservableCollection<string>), null);
        public ObservableCollection<string> DtmPointSetting
        {
            get => GetValue<ObservableCollection<string>>(_dtmPointSettings);
            set => SetValue(_dtmPointSettings, value);
        }

        ICommand _currentPointCommand = DtmToolBar.DtmPoint;
        public ICommand CurrentPointCommand
        {
            get => _currentPointCommand;
        }

        public bool DtmPointSettingEnabled => DtmPointSetting.Count > 0;

        Document _document;
        DataModel _dataModel;
        ICanvasCommand _canvasCommand;
        public void SetDocument(Document document)
        {
            if (_document == document)
                return;
            _canvasCommand = null;
            _document = document;
            _dataModel = document?.DataModel;
            if (_document == null)
                return;
            UpdateLineSetting();
            UpdatePointSetting();
            UpdateToolSetting();
            _canvasCommand = _document.CanvasCommand;
        }
        void UpdateLineSetting()
        {
            DtmLineSetting.Clear();
            DtmLineSettingSelected = "";
            var element = DtmConfigurationSingleton.Instance.CreateType(DtmLineElementSelected.Name);
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

        void UpdatePointSetting()
        {
            DtmPointSetting.Clear();
            DtmPointSettingSelected = "";
            var element = DtmConfigurationSingleton.Instance.CreateType(DtmPointElementSelected.Name);
            if (element != null)
            {
                foreach (var setting in element.Settings)
                {
                    DtmPointSetting.Add(setting);
                }
                if (DtmPointSetting.Count > 0)
                {
                    DtmPointSettingSelected = DtmPointSetting[0];
                }
            }
            OnPropertyChanged("DtmPointSettingEnabled");
        }

        bool _updateToolSettingLock = false;
        void UpdateToolSetting()
        {
            if (_updateToolSettingLock)
                return;
            if (!(_dataModel?.ActiveLayer is DtmDrawingLayerMain dtmLayout)) return;
            dtmLayout.DtmLineElementSelected = new Tuple<string, string>(DtmLineElementSelected.Name, DtmLineSettingSelected);
            dtmLayout.DtmPointSelected = new Tuple<string, string>(DtmPointElementSelected.Name, DtmPointSettingSelected);
            UpdateCurrentPointCommand();
            _canvasCommand?.UpadateDrawToolProperties();
        }

        public ToggleButton PointToggleButton { get; set; }
        void UpdateCurrentPointCommand()
        {
            var currentCommand = DtmPointElementSelected.Category == DtmElementType.Bod
                ? DtmToolBar.DtmPoint
                : DtmToolBar.DtmDefinitionPoint;
            if (currentCommand == _currentPointCommand)
                return;
            var isChecked = PointToggleButton.IsChecked;
            _currentPointCommand = currentCommand;
            _currentPointCommand.Execute(null);
            PointToggleButton.IsChecked = isChecked;
            OnPropertyChanged("CurrentPointCommand");
        }
    }

    class DtmPointElement
    {
        string _displayName;
        public string Name { get; set; }
        public string ObjektovyTypNazev { get; set; }
        public string KategorieObjektu { get; set; }
        public string DisplayName
        {
            get
            {
                if (_displayName != null)
                    return _displayName;
                _displayName = !string.IsNullOrWhiteSpace(ObjektovyTypNazev)
                    ? ObjektovyTypNazev
                    : DtmConfigurationSingleton.ToNiceName(Name, Category);
                return _displayName;
            }
        }
        public DtmElementType Category { get; set; }
    }

    class DtmLineElement
    {
        string _displayName;
        public string Name { get; set; }
        public string Category { get; set; }
        public string ObjektovyTypNazev { get; set; }
        public string DisplayName
        {
            get
            {
                if (_displayName != null)
                    return _displayName;
                _displayName = !string.IsNullOrWhiteSpace(ObjektovyTypNazev)
                    ? ObjektovyTypNazev
                    : DtmConfigurationSingleton.ToNiceName(Name, DtmElementType.Linie);
                return _displayName;
            }
        }
    }

    public partial class DtmToolBar : GeoCadToolBar
    {
        DtmToolBarCtx _ctx;
        public static GeoCadRoutedCommand DtmMultiLine = new GeoCadRoutedCommand("DtmMultiLine", typeof(DtmToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand DtmPoint = new GeoCadRoutedCommand("DtmPoint", typeof(DtmToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand DtmDefinitionPoint = new GeoCadRoutedCommand("DtmDefinitionPoint", typeof(DtmToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public DtmToolBar()
        {
            InitializeComponent();
            var dtmPointsElement = new List<DtmPointElement>();
            var dtmLinesElement = new List<DtmLineElement>();
            _ctx = new DtmToolBarCtx
            {
                PointToggleButton = _pointToggleButton
            };
            foreach (var element in DtmConfigurationSingleton.Instance.ElementSetting)
            {
                switch (element.Value.ElementType)
                {
                    case DtmElementType.Linie:
                        dtmLinesElement.Add(new DtmLineElement { Name = element.Key, Category = element.Value.KategorieObjektu, ObjektovyTypNazev = element.Value.ObjektovyTypNazev });
                        break;
                    case DtmElementType.Bod:
                    case DtmElementType.DefinicniBod:
                        dtmPointsElement.Add(new DtmPointElement
                        {
                            Name = element.Key,
                            Category = element.Value.ElementType,
                            ObjektovyTypNazev = element.Value.ObjektovyTypNazev,
                            KategorieObjektu = element.Value.KategorieObjektu
                        });
                        break;
                }
            }
            _ctx.DtmPointElements = new ListCollectionView(dtmPointsElement);
            _ctx.DtmPointElements.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            _ctx.DtmPointElements.GroupDescriptions.Add(new PropertyGroupDescription("KategorieObjektu"));
            _ctx.DtmLineElements = new ListCollectionView(dtmLinesElement);
            _ctx.DtmLineElements.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            _ctx.DtmLineElements.MoveCurrentToFirst();
            _ctx.DtmLineElementSelected = (DtmLineElement)_ctx.DtmLineElements.CurrentItem;
            _ctx.DtmPointElements.MoveCurrentToFirst();
            _ctx.DtmPointElementSelected = (DtmPointElement)_ctx.DtmPointElements.CurrentItem;
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
                            _ctx.SetDocument(ToolBarManager.Document);
                        }
                    }
                    break;
            }

        }
        static bool IsGroupHeader(object sender, MouseButtonEventArgs e) =>
            e.OriginalSource is FrameworkElement fe && fe.Name == "GroupHeader" && fe.TemplatedParent == sender;

        void OnGroupHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsGroupHeader(sender, e))
                e.Handled = true;
        }

        const string CollapsedGroupsKey = "Dtm/CollapsedGroups";
        HashSet<string> _collapsedGroups;

        HashSet<string> CollapsedGroups
        {
            get
            {
                if (_collapsedGroups == null)
                {
                    var value = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, CollapsedGroupsKey).getString(string.Empty);
                    _collapsedGroups = new HashSet<string>(value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                }
                return _collapsedGroups;
            }
        }

        static string GetGroupPath(FrameworkElement groupItem)
        {
            var names = new List<string>();
            DependencyObject current = groupItem;
            while (current != null)
            {
                if (current is System.Windows.Controls.GroupItem item && item.DataContext is CollectionViewGroup group)
                    names.Insert(0, group.Name?.ToString() ?? string.Empty);
                current = VisualTreeHelper.GetParent(current);
            }
            var owner = System.Windows.Controls.ItemsControl.GetItemsOwner(groupItem) as FrameworkElement;
            return (owner?.Name ?? string.Empty) + ">" + string.Join(">", names);
        }

        void OnGroupLoaded(object sender, RoutedEventArgs e)
        {
            var group = (FrameworkElement)sender;
            group.Tag = CollapsedGroups.Contains(GetGroupPath(group)) ? "Collapsed" : null;
        }

        void OnGroupHeaderMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsGroupHeader(sender, e))
                return;
            var group = (FrameworkElement)sender;
            var path = GetGroupPath(group);
            if ((string)group.Tag == "Collapsed")
            {
                group.Tag = null;
                CollapsedGroups.Remove(path);
            }
            else
            {
                group.Tag = "Collapsed";
                CollapsedGroups.Add(path);
            }
            SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, CollapsedGroupsKey,
                new ProgramOption(string.Join("|", CollapsedGroups)));
            e.Handled = true;
        }

        void OnDtmValidate(object sender, RoutedEventArgs e)
        {
            ToolBarManager.Document.ValidateDtm();
        }
        void OnDtmLayerManager(object sender, RoutedEventArgs e)
        {
            if (ToolBarManager.Document.DataModel.ShowDtmLayerManager())
                ToolBarManager.Document.CanvasCommand.InvalidateAll();
        }
    }
}
