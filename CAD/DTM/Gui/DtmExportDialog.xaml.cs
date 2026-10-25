using GeoBase.Gui;
using System;
using System.IO;
using System.Windows;
using CAD.DTM.Signing;

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
            if (_ctx.SignOutputDirectory)
            {
                try
                {
                    _ctx.SigningSettings ??= DtmSigningSettings.Load();
                    _ctx.SigningSettings.Validate();
                    using var certificate = DtmSigningCertificates.Find(_ctx.SigningSettings.CertificateThumbprint);
                    DtmDirectorySigner.ValidateVerification(_ctx.CisloOvereni, _ctx.DatumOvereni);
                    var existing = DtmSignaturePublisher.ExistingArtifacts(_ctx.OutputDirectory);
                    _ctx.ReplaceSignatureConfirmed = existing.Length > 0;
                    if (existing.Length > 0 && MessageBox.Show(this,
                        "Adresář již obsahuje podpisové soubory. Chcete je po úspěšném podpisu nahradit?\n" + string.Join("\n", existing),
                        "Podepsání adresáře", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) return;
                    _signingError.Text = "";
                }
                catch (Exception ex) { _signingError.Text = ex.Message; return; }
            }
            if (File.Exists(_ctx.FileName) && MessageBox.Show(this,
                DtmExportCtx.ExportText("DtmExportOverwrite", "Soubor již existuje. Chcete jej přepsat?") + "\n" + _ctx.FileName,
                DtmExportCtx.ExportText("DtmExportTitle", "Export DTM"),
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            _ctx.SaveValuesToRegistry();
            DialogResult = true;
            SavePosAndSize();
            Close();
        }

        void OnSigningChanged(object sender, RoutedEventArgs e)
        {
            if (_signingCertificate == null) return;
            _signingError.Text = "";
            if ((sender as System.Windows.Controls.CheckBox)?.IsChecked != true) return;
            _ctx.SigningSettings ??= DtmSigningSettings.Load();
            RefreshSigningCertificate();
        }

        void RefreshSigningCertificate()
        {
            try
            {
                using var certificate = DtmSigningCertificates.Find(_ctx.SigningSettings.CertificateThumbprint);
                _signingCertificate.Text = DtmSigningCertificates.Describe(certificate);
            }
            catch (Exception) { _signingCertificate.Text = "Vyberte podpisový certifikát v nastavení podpisu."; }
        }

        void OnSigningSettings(object sender, RoutedEventArgs e)
        {
            _ctx.SigningSettings ??= DtmSigningSettings.Load();
            var dialog = new DtmSigningSettingsDialog(_ctx.SigningSettings) { Owner = this };
            if (dialog.ShowDialog() != true) return;
            _ctx.SigningSettings.Password = "";
            _ctx.SigningSettings = dialog.Settings;
            _signingError.Text = "";
            RefreshSigningCertificate();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DialogResult != true && _ctx.SigningSettings != null) _ctx.SigningSettings.Password = "";
            base.OnClosed(e);
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
