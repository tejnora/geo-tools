using System;
using System.IO;
using CAD.Utils;
using GeoBase.Gui;
using GeoBase.Utils;
using VFK;

namespace CAD.VFK.GUI
{
    public partial class VFKExportDialog : DialogBase
    {
        public VFKExportDialog(VFKDataContext vfkDataContext)
            : base("VFKExportDialog")
        {
            InitializeComponent();
            _mFileName.DataContext = this;
            var po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, "Vfk/FileName");
            if (!po.isString()) return;
            var directory = Path.GetDirectoryName(po.getString());
            FileName = Path.Combine(directory, string.Format("{0:d6}_ZPMZ_{1:d5}_vfk.vfk", vfkDataContext.FSU, vfkDataContext.CisloZPMZ));
        }

        private string _mFileNamee = string.Empty;

        public string FileName
        {
            get
            {
                return _mFileNamee;
            }
            set
            {
                _mFileNamee = value;
                OnPropertyChanged("FileName");
            }
        }

        private void OnFileChooser(object sender, EventArgs aArgs)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Vymenny format KN (*.vfk)|*.vfk";
            dlg.FileName = FileName;
            var result = dlg.ShowDialog();
            if (result == true)
            {
                FileName = dlg.FileName;
            }
        }
    }
}
