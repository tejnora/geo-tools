using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace CAD.UITools
{
    #region Enums
    public enum NotificationType
    {
        DocumentChanged,
        ToolChanged,
        MergeCommandBindings,
        MergeInputBindings,
    }
    #endregion
    public class GeoCadToolBarManager
    {
        #region Constructor
        public GeoCadToolBarManager(IMainWinInterface owner)
        {
            Owner = owner;
            _toolBars = new List<GeoCadToolBar>();
        }
        #endregion
        #region Property & Fields
        private readonly List<GeoCadToolBar> _toolBars;
        private Document _document;
        public Document Document
        {
            get { return _document; }
            set { _document = value; Notify(NotificationType.DocumentChanged, null); }
        }

        private GeoCadRoutedCommand _commnad;

        public delegate void CurrentCommand(GeoCadRoutedCommand command);
        public event CurrentCommand ToolChanged;
        public GeoCadRoutedCommand Command
        {
            get { return _commnad; }
            set
            {
                if (value != null && !value.HasState) return;
                if (_commnad != value)
                {
                    _commnad = value;
                    Notify(NotificationType.ToolChanged, value);
                    if (ToolChanged != null)
                        ToolChanged(value);
                }
            }
        }
        public IMainWinInterface Owner
        {
            get; private set;
        }
        #endregion
        #region Methods
        public void ForceToolBarChange()
        {
            if (ToolChanged != null)
                ToolChanged(Command);
        }
        public void NotifyDocumentChanged()
        {
            Notify(NotificationType.DocumentChanged, null);
        }
        public void MergeCommandBindings(CommandBindingCollection commands)
        {
            Notify(NotificationType.MergeCommandBindings, commands);
        }
        public void MergeInputBindings(InputBindingCollection commands)
        {
            Notify(NotificationType.MergeInputBindings, commands);
        }
        public void RegisterToolBar(ToolBarTray toolBarTray, GeoCadToolBar toolBar)
        {
            _toolBars.Add(toolBar);
            toolBar.ToolBarManager = this;
            toolBar.BandIndex = _toolBars.Count - 1;
            if (toolBarTray != null)
                toolBarTray.ToolBars.Add(toolBar);
        }
        public void UnRegisterToolBar(GeoCadToolBar toolBar)
        {
            if (_toolBars.Contains(toolBar))
            {
                toolBar.ToolBarManager = null;
                _toolBars.Remove(toolBar);
            }
        }
        private void Notify(NotificationType type, object additionData)
        {
            foreach (var toolBar in _toolBars)
            {
                toolBar.Notify(type, additionData);
            }
        }
        #endregion
    }
}
