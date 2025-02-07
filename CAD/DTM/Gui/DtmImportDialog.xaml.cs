using GeoBase.Gui;
using GeoBase.Utils;
using System;
using System.Windows.Input;
using GeoBase;

namespace CAD.DTM.Gui
{
    public partial class DtmImportDialog : DialogBase
    {
        readonly DtmImportCtx _ctx;

        public DtmImportDialog(DtmImportCtx ctx)
        : base("DtmImportDialog", false, true)
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

        void OnImportButtonClick(object sender, EventArgs aArgs)
        {
            DialogResult = true;
            SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, "Dtm/FileName", new ProgramOption(_ctx.FileName));
            SavePosAndSize();
            Close();
        }
        void OnCanImportButtonClick(object sender, CanExecuteRoutedEventArgs args)
        {
            _ctx.Validate();
            args.CanExecute = !_ctx.HasErrors;
        }

        void OnFileChooser(object sender, EventArgs aArgs)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
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
