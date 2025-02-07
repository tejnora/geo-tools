using GeoBase;
using GeoBase.Gui;
using GeoBase.Utils;
using System;
using System.Windows.Input;

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
            DialogResult = true;
            SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, "Dtm/ExportFileName", new ProgramOption(_ctx.FileName));
            SavePosAndSize();
            Close();
        }
        void OnCanExportButtonClick(object sender, CanExecuteRoutedEventArgs args)
        {
            _ctx.Validate();
            args.CanExecute = !_ctx.HasErrors;
        }

        void OnFileChooser(object sender, EventArgs aArgs)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "DTM (*.xml)|*.xml",
                FileName = _ctx.FileName
            };
            var result = dlg.ShowDialog();
            if (result != true) return;
            _ctx.FileName = dlg.FileName;
        }

    }
}
