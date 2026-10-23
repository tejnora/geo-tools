using GeoBase.Gui;
using System;
using System.IO;
using System.Windows;

namespace CAD.DTM.Gui
{
    public partial class DtmExportDialog : DialogBase
    {
        readonly DtmExportCtx _ctx;
        public DtmExportDialog(DtmExportCtx ctx)
            : base("DtmExportDialog", false, true)
        {
            _ctx = ctx;
            DataContext = _ctx;
            InitializeComponent();
            CAD.GUI.DialogLayout.ApplyButtonBar(this);
        }
        public Nullable<bool> DoModal()
        {
            return ShowDialog();
        }
        void OnCancel(object sender, EventArgs aArgs)
        {
            Close();
        }

        void OnExportButtonClick(object sender, EventArgs aArgs)
        {
            if (!_ctx.PrepareOutputFile()) return;
            if (File.Exists(_ctx.FileName) && MessageBox.Show(this,
                DtmExportCtx.ExportText("DtmExportOverwrite", "Soubor již existuje. Chcete jej přepsat?") + "\n" + _ctx.FileName,
                DtmExportCtx.ExportText("DtmExportTitle", "Export DTM"),
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            _ctx.SaveValuesToRegistry();
            DialogResult = true;
            SavePosAndSize();
            Close();
        }
        void OnDirectoryChooser(object sender, EventArgs aArgs)
        {
            var dlg = new Microsoft.Win32.OpenFolderDialog
            {
                Title = DtmExportCtx.ExportText("DtmExportDirectoryTitle", "Vyberte výstupní adresář"),
                InitialDirectory = Directory.Exists(_ctx.OutputDirectory) ? _ctx.OutputDirectory : string.Empty,
                Multiselect = false
            };
            if (dlg.ShowDialog(this) == true) _ctx.OutputDirectory = dlg.FolderName;
        }
    }
}
