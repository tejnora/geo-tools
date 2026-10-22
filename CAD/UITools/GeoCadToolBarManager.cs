using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using GeoBase.Utils;

namespace CAD.UITools
{

    public enum NotificationType
    {
        DocumentChanged,
        ToolChanged,
        MergeCommandBindings,
        MergeInputBindings,
    }


    public class GeoCadToolBarManager
    {

        public GeoCadToolBarManager(IMainWinInterface owner)
        {
            Owner = owner;
            _toolBars = new List<GeoCadToolBar>();
        }



        private readonly List<GeoCadToolBar> _toolBars;
        private Document _document;

        public Document Document
        {
            get { return _document; }
            set
            {
                _document = value;
                Notify(NotificationType.DocumentChanged, null);
            }
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

        public IMainWinInterface Owner { get; private set; }



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
            var key = "ToolBars/" + toolBar.GetType().Name;
            var band = GeoBase.SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, key + "/Band");
            var index = GeoBase.SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, key + "/BandIndex");
            if (band.isInt() && band.getInt() >= 0)
                toolBar.Band = band.getInt();
            if (index.isInt() && index.getInt() >= 0)
                toolBar.BandIndex = index.getInt();
            if (toolBarTray != null)
                toolBarTray.ToolBars.Add(toolBar);
        }

        public void SaveLayout()
        {
            foreach (var toolBar in _toolBars)
            {
                var key = "ToolBars/" + toolBar.GetType().Name;
                GeoBase.SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, key + "/Band", new ProgramOption(toolBar.Band));
                GeoBase.SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, key + "/BandIndex", new ProgramOption(toolBar.BandIndex));
            }
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

    }
}
