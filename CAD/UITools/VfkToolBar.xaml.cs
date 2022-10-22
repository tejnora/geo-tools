using System;
using System.Windows.Controls;
using CAD.Canvas.Layers;
using CAD.Utils;

namespace CAD.UITools
{
    public partial class VfkToolBar : GeoCadToolBar
    {
        #region RoutedCommands
        public static GeoCadRoutedCommand VfkMultiLine = new GeoCadRoutedCommand("VfkMultiLine", typeof(VfkToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand VfkMark = new GeoCadRoutedCommand("VfkMark", typeof(VfkToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand VfkText = new GeoCadRoutedCommand("VfkText", typeof(VfkToolBar), GeoCadRoutedCommand.CommandTypes.DrawTool);
        public static GeoCadRoutedCommand VfkActivePoint = new GeoCadRoutedCommand("VfkActivePoint", typeof(VfkToolBar), GeoCadRoutedCommand.CommandTypes.None);
        public static GeoCadRoutedCommand VfkLayerManger = new GeoCadRoutedCommand("VfkLayerManager", typeof(VfkToolBar), GeoCadRoutedCommand.CommandTypes.None);
        public static GeoCadRoutedCommand VfkMeasureArea = new GeoCadRoutedCommand("VfkMeasureArea", typeof(VfkToolBar), GeoCadRoutedCommand.CommandTypes.InfoTool);
        #endregion
        #region Constructor
        public VfkToolBar()
        {
            InitializeComponent();
            DataContext = this;
            _elementsGroup.DataContext = Singletons.VFKElements;
            _elementsGroup.IsEnabled = false;
            _elementGroupSubCombo.IsEnabled = false;

        }
        #endregion
        #region Fields & Properties
        public bool VfkShowPointsIsChecked
        {
            get { return ToolBarManager.Document != null && ToolBarManager.Document.DataModel.ViewPointEnable; }
            set
            {
                ToolBarManager.Document.DataModel.ViewPointEnable = value;
                ToolBarManager.Document.CanvasCommand.InvalidateAll();
                OnPropertyChanged("VfkShowPointsIsChecked");
            }
        }
        private bool _lockToolBarChanged;
        #endregion
        #region GeoCadToolBar
        public override void Notify(NotificationType type, object additionData)
        {
            base.Notify(type,additionData);
            bool updatePropertyValues = false;
            switch (type)
            {
                case NotificationType.DocumentChanged:
                    {
                        bool enable = ToolBarManager.Document != null &&
                                      ToolBarManager.Document.DataModel.ActiveLayer is VFKDrawingLayerMain;
                        IsEnabled = enable;
                        updatePropertyValues = true;
                    } break;
                case NotificationType.ToolChanged:
                    {
                        _lockToolBarChanged = true;
                        bool isEditable = false;
                        var elements = Singletons.VFKElements;
                        if(additionData==VfkMultiLine)
                        {
                            isEditable = true;
                            elements.SubGroups = elements.VfkLineElementGroup.SubGroupItems;
                            elements.SelectedSubGroup = elements.VfkLineElementGroup.DefaultElementSubGroup;
                        }
                        else if(additionData==VfkText)
                        {
                            isEditable = true;
                            elements.SubGroups = elements.VfkNumberElementGroup.SubGroupItems;
                            elements.SelectedSubGroup = elements.VfkNumberElementGroup.DefaultElementSubGroup;
                        }
                        else if(additionData==VfkMark)
                        {
                            isEditable = true;
                            elements.SubGroups = elements.VfkMarkElementGroup.SubGroupItems;
                            elements.SelectedSubGroup = elements.VfkMarkElementGroup.DefaultElementSubGroup;
                        }
                        if (elements.SelectedSubGroup == null)
                            elements.SelectedSubGroup = elements.VfkLineElementGroup.DefaultElementSubGroup;
                        if (elements.SelectedSubGroup.SelectedElement!=null)
                            elements.SelectedSubGroup.SelectedElement = elements.SelectedSubGroup.DefaultElement;
                        _elementsGroup.IsEnabled = isEditable;
                        _elementGroupSubCombo.IsEnabled = isEditable;
                        _lockToolBarChanged = false;
                    }break;
            }
            if (updatePropertyValues)
            {
                OnPropertyChanged("VfkShowPointsIsChecked");
            }
        }
        private void OnVfkLayerManger(object sender, EventArgs args)
        {
            if (ToolBarManager.Document.DataModel.ShowVfkLayerManager())
                ToolBarManager.Document.CanvasCommand.InvalidateAll();
        }
        #endregion
        #region Events
        private void OnReloadTool(object sender, SelectionChangedEventArgs e)
        {
            if (_lockToolBarChanged) return;
            ToolBarManager.ForceToolBarChange();
        }
        #endregion
    }
}
