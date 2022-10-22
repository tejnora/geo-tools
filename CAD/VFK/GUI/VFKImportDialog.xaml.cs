using System;
using System.Windows.Input;
using CAD.Utils;
using GeoBase.Gui;
using Registry = GeoBase.Utils.Registry;
using ProgramOption = GeoBase.Utils.ProgramOption;

namespace VFK.GUI
{
    public partial class VFKImportDialog : DialogBase
    {
        #region Constructors
        private VFKDataContext _vfkDataContext;
        public VFKImportDialog(VFKDataContext aDataContext)
            : base("VFKImportDialog")
        {
            InitializeComponent();
            _vfkDataContext = aDataContext;
            if(_vfkDataContext.AuthorName.Length==0)
            {
                ProgramOption po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser,
                                                                "VFPImportDialog/AuthorName");
                if (po.isString())
                    _vfkDataContext.AuthorName= po.getString();
            }
            if(_vfkDataContext.FileName.Length==0)
            {
                ProgramOption po = Singletons.Registry.getEntry(Registry.SubKey.kCurrentUser, "Vfk/FileName");
                if (po.isString())
                    _vfkDataContext.FileName = po.getString();
            }
            DataContext = aDataContext;
            _vztazneMeritko.ItemsSource = Singletons.ScaleList.ScaleListItems;
            _vztazneMeritko.DisplayMemberPath = "ComboString";
            _vfkDataContext.OnReloadProperties();
            _cilImportuEntriesCB.DataContext = _vfkDataContext;
        }
        #endregion
        #region Events
        private void OnFileChooser(object sender, EventArgs aArgs)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Vymenny format KN (*.vfk)|*.vfk";
            dlg.FileName = _vfkDataContext.FileName;
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                _vfkDataContext.FileName = dlg.FileName;
                _mFileName.Text = _vfkDataContext.FileName;
            }
        }

        private void OnOk(object sender, EventArgs aArgs)
        {
            DialogResult = true;
            Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, "VFPImportDialog/AuthorName", new ProgramOption(_vfkDataContext.AuthorName));
            Singletons.Registry.setEntry(Registry.SubKey.kCurrentUser, "Vfk/FileName", new ProgramOption(_vfkDataContext.FileName));
            Close();
        }

        private void OnCanOk(object sender, CanExecuteRoutedEventArgs args)
        {
            if (_vfkDataContext != null)
            {
                _vfkDataContext.Validate();
#if DEBUG
                args.CanExecute = true;
#else
                args.CanExecute = !_vfkDataContext.HasErrors;
#endif
            }
        }

        private void OnCancel(object sender, EventArgs aArgs)
        {
            Close();
        }

        private void onLoaded(object sender, EventArgs args)
        {
            if(_vfkDataContext!=null)
                _vfkDataContext.OnReloadProperties();
        }

        public Nullable<bool> DoModal()
        {
            return ShowDialog();
        }

        public void onSelectKU(object sender, EventArgs args)
        {
            KatastralniUzemiDialog dialog = new KatastralniUzemiDialog();
            var res=dialog.ShowDialog();
            if(res.HasValue && res.Value && dialog.SelectedNode!=null)
            {
                _vfkDataContext.FSU = System.Convert.ToUInt32(dialog.SelectedNode.KU_KOD);
                _vfkDataContext.KatastralniPracoviste = dialog.SelectedNode.OKRES_NAZEV;
                _vfkDataContext.Obec = dialog.SelectedNode.KU_NAZEV;
                _vfkDataContext.KatastralniUzemi = dialog.SelectedNode.KU_NAZEV;
                _vfkDataContext.PoradoveCisloKU = dialog.SelectedNode.KU_PRAC;
                _vfkDataContext.CiselnaRada = System.Convert.ToUInt32(dialog.SelectedNode.CISELNA_RADA);
                if (dialog.SelectedNode.MAPA == "D" || dialog.SelectedNode.MAPA == "C")
                    _vfkDataContext.TypGP = TypGP.DKM;
                else
                    _vfkDataContext.TypGP = TypGP.Ostatni;
            }
        }
        #endregion
    }
}
