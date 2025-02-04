using System;
using System.ComponentModel;
using System.Windows;
using CAD.Utils;
using CAD.Canvas;

using Registry = GeoBase.Utils.Registry;
using ProgramOption = GeoBase.Utils.ProgramOption;

namespace CAD.GUI
{
    public class PpWindow
        : Window, INotifyPropertyChanged
    {
                public PpWindow()
        {
        }
        public PpWindow(String aRegistryValue)
        {
            _registryValue = aRegistryValue;
            ProgramOption po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, aRegistryValue+"/Pos");
            if (po.isPoint())
            {
                Left = po.getPoint().X;
                Top = po.getPoint().Y;
            }
            po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, aRegistryValue + "/Size");
            if (po.isPoint())
            {
                Width = po.getPoint().X;
                Height = po.getPoint().Y;
            }

            Closing += OnClosing;
        }
                        private string _registryValue;
                        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
                        public virtual void SetOwner(IDrawObject aOwner)
        {
        }
        private void OnClosing(object sender, EventArgs arg)
        {
            Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, _registryValue + "/Pos", new ProgramOption(new Point(Left, Top)));
            Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, _registryValue + "/Size", new ProgramOption(new Point(Width, Height)));
        }
        
    }
}
