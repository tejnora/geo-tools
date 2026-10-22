using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using CAD.Canvas;
using CAD.DTM.Configuration;
using CAD.UITools;
using GeoBase;
using GeoBase.Utils;
using System.Windows.Input;
using System.Linq;

namespace CAD.DTM.Gui
{
    class DtmToolBarCtx : DataObjectBase<DtmToolBarCtx>
    {
        public DtmToolBarCtx() : base(null, new StreamingContext())
        {
        }
        public readonly PropertyData _dtmLineElements = RegisterProperty("DtmLineElements", typeof(ListCollectionView), null);
        public ListCollectionView DtmAreaElements { get; set; }
        DtmLineElement _areaSelected;
        public DtmLineElement DtmAreaElementSelected
        {
            get => _areaSelected;
            set { _areaSelected = value; UpdateToolSetting(); OnPropertyChanged("DtmAreaElementSelected"); }
        }
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
                UpdateToolSetting();
            }
        }
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
                UpdateToolSetting();
            }
        }
        ICommand _currentPointCommand = DtmToolBar.DtmPoint;
        public ICommand CurrentPointCommand
        {
            get => _currentPointCommand;
        }

        Document _document;
        public bool IsValidationVisible => _document?.IsDtmValidationVisible ?? SingletonsBase.Registry
            .getEntry(Registry.SubKey.kCurrentUser, DtmValidationPanel.VisibilitySettingKey).getBool(false);
        DataModel _dataModel;
        ICanvasCommand _canvasCommand;
        public void SetDocument(Document document)
        {
            if (_document == document)
            {
                OnPropertyChanged(nameof(IsValidationVisible));
                return;
            }
            _canvasCommand = null;
            _document = document;
            OnPropertyChanged(nameof(IsValidationVisible));
            _dataModel = document?.DataModel;
            if (_document == null)
                return;
            UpdateToolSetting();
            _canvasCommand = _document.CanvasCommand;
        }
        void UpdateToolSetting()
        {
            if (!(_dataModel?.ActiveLayer is DtmDrawingLayerMain dtmLayout)) return;
            dtmLayout.DtmLineElementSelected = DtmLineElementSelected.Name;
            dtmLayout.DtmPointSelected = DtmPointElementSelected.Name;
            dtmLayout.DtmAreaSelected = DtmAreaElementSelected?.Name;
            UpdateCurrentPointCommand();
            _canvasCommand?.UpadateDrawToolProperties();
        }

        void UpdateCurrentPointCommand()
        {
            var currentCommand = DtmPointElementSelected.Category == DtmElementType.Bod
                ? DtmToolBar.DtmPoint
                : DtmToolBar.DtmDefinitionPoint;
            if (currentCommand == _currentPointCommand)
                return;
            _currentPointCommand = currentCommand;
            OnPropertyChanged("CurrentPointCommand");
        }
    }

    class DtmPointElement
    {
        string _displayName;
        public string Name { get; set; }
        public string ObjektovyTypNazev { get; set; }
        public string ObsahovaCast { get; set; }
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
        public string ObsahovaCast { get; set; }
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
        public static GeoCadRoutedCommand DtmArea = new GeoCadRoutedCommand("DtmArea", typeof(DtmToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public DtmToolBar()
        {
            InitializeComponent();
            var dtmPointsElement = new List<DtmPointElement>();
            var dtmLinesElement = new List<DtmLineElement>();
            var dtmAreaElements = new List<DtmLineElement>();
            _ctx = new DtmToolBarCtx();
            foreach (var element in DtmConfigurationSingleton.Instance.ElementSetting)
            {
                switch (element.Value.ElementType)
                {
                    case DtmElementType.Plocha:
                        dtmAreaElements.Add(new DtmLineElement { Name = element.Key, ObsahovaCast = element.Value.ObsahovaCast, Category = element.Value.KategorieObjektu, ObjektovyTypNazev = element.Value.ObjektovyTypNazev });
                        break;
                    case DtmElementType.Linie:
                        dtmLinesElement.Add(new DtmLineElement { Name = element.Key, ObsahovaCast = element.Value.ObsahovaCast, Category = element.Value.KategorieObjektu, ObjektovyTypNazev = element.Value.ObjektovyTypNazev });
                        break;
                    case DtmElementType.Bod:
                    case DtmElementType.DefinicniBod:
                        dtmPointsElement.Add(new DtmPointElement
                        {
                            Name = element.Key,
                            Category = element.Value.ElementType,
                            ObsahovaCast = element.Value.ObsahovaCast,
                            ObjektovyTypNazev = element.Value.ObjektovyTypNazev,
                            KategorieObjektu = element.Value.KategorieObjektu
                        });
                        break;
                }
            }
            _ctx.DtmPointElements = new ListCollectionView(dtmPointsElement);
            _ctx.DtmAreaElements = new ListCollectionView(dtmAreaElements);
            _ctx.DtmAreaElements.GroupDescriptions.Add(new PropertyGroupDescription("ObsahovaCast"));
            _ctx.DtmAreaElements.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            _ctx.DtmAreaElementSelected = dtmAreaElements.FirstOrDefault();
            _ctx.DtmPointElements.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            _ctx.DtmPointElements.GroupDescriptions.Add(new PropertyGroupDescription("ObsahovaCast"));
            _ctx.DtmPointElements.GroupDescriptions.Add(new PropertyGroupDescription("KategorieObjektu"));
            _ctx.DtmLineElements = new ListCollectionView(dtmLinesElement);
            _ctx.DtmLineElements.GroupDescriptions.Add(new PropertyGroupDescription("ObsahovaCast"));
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
        void OnElementSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender;
            if (!combo.IsLoaded || !combo.IsEnabled || combo.SelectedItem == null ||
                (!combo.IsDropDownOpen && !combo.IsKeyboardFocusWithin) ||
                !(ToolBarManager?.Document?.DataModel.ActiveLayer is DtmDrawingLayerMain))
                return;

            combo.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
            var command = combo == _areaCombo ? DtmArea
                : combo == _lineCombo ? DtmMultiLine
                : (GeoCadRoutedCommand)_ctx.CurrentPointCommand;
            if (command.CanExecute(null, this))
                command.Execute(null, this);
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
            ToolBarManager.Document.ToggleDtmValidationPanel();
        }
        void OnDtmLayerManager(object sender, RoutedEventArgs e)
        {
            if (ToolBarManager.Document.DataModel.ShowDtmLayerManager())
                ToolBarManager.Document.CanvasCommand.InvalidateAll();
        }
    }
}
