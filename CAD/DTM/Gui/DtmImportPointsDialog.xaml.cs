using GeoBase;
using GeoBase.Gui;
using GeoBase.Utils;
using System;

namespace CAD.DTM.Gui
{
    public partial class DtmImportPointsDialog : DialogBase
    {
        readonly DtmImportPointsCtx _ctx;

        public DtmImportPointsDialog(DtmImportPointsCtx ctx)
            : base("DtmImportPointsDialog", false, true)
        {
            _ctx = ctx;
            DataContext = ctx;
            InitializeComponent();
        }
        public Nullable<bool> DoModal()
        {
            return ShowDialog();
        }
        void OnCancel(object sender, EventArgs aArgs)
        {
            Close();
        }
        void OnImportButtonClick(object sender, EventArgs aArgs)
        {
            DialogResult = true;
            _ctx.SaveToRegistry();
            SavePosAndSize();
            Close();
        }
        void OnFileChooser(object sender, EventArgs aArgs)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "DTM (*.txt)|*.txt",
                FileName = _ctx.FileName
            };
            var result = dlg.ShowDialog();
            if (result != true) return;
            _ctx.FileName = dlg.FileName;
        }
    }
}
