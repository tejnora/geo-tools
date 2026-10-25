using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Interop;
using CAD.DTM.Signing;

namespace CAD.DTM.Gui
{
    internal partial class DtmSigningSettingsDialog : Window
    {
        internal DtmSigningSettings Settings { get; }

        internal DtmSigningSettingsDialog(DtmSigningSettings settings)
        {
            Settings = settings.Copy();
            InitializeComponent();
            _url.Text = Settings.TsaUrl;
            _userName.Text = Settings.UserName;
            _password.Password = Settings.Password;
            _rememberPassword.IsChecked = Settings.RememberPassword;
            _error.Text = Settings.CredentialError;
            RefreshCertificate();
        }

        void RefreshCertificate()
        {
            try
            {
                using var certificate = DtmSigningCertificates.Find(Settings.CertificateThumbprint);
                _certificate.Text = DtmSigningCertificates.Describe(certificate);
            }
            catch (Exception) { _certificate.Text = "Certifikát není vybrán nebo není dostupný."; }
        }

        void OnChooseCertificate(object sender, RoutedEventArgs e)
        {
            try
            {
                using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                var certificates = store.Certificates;
                try
                {
                    var available = new X509Certificate2Collection(certificates.Cast<X509Certificate2>()
                        .Where(c => c.HasPrivateKey && c.NotBefore <= DateTime.Now && c.NotAfter >= DateTime.Now).ToArray());
                    if (available.Count == 0) throw new InvalidOperationException("V osobním úložišti Windows nejsou platné certifikáty s privátním klíčem. Připojte podpisový token.");
                    var selected = X509Certificate2UI.SelectFromCollection(available, "Podpisový certifikát",
                        "Vyberte certifikát ověřovatele AZI.", X509SelectionFlag.SingleSelection, new WindowInteropHelper(this).Handle);
                    try
                    {
                        if (selected.Count == 0) return;
                        DtmSigningCertificates.Validate(selected[0]);
                        Settings.CertificateThumbprint = selected[0].Thumbprint;
                        _error.Text = "";
                    }
                    finally { foreach (var certificate in selected) certificate.Dispose(); }
                }
                finally { foreach (var certificate in certificates) certificate.Dispose(); }
                RefreshCertificate();
            }
            catch (Exception ex) { _error.Text = ex.Message; }
        }

        void OnSave(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.TsaUrl = _url.Text.Trim();
                Settings.UserName = _userName.Text.Trim();
                Settings.Password = _password.Password;
                Settings.RememberPassword = _rememberPassword.IsChecked == true;
                Settings.Validate();
                using var certificate = DtmSigningCertificates.Find(Settings.CertificateThumbprint);
                Settings.Save();
                DialogResult = true;
            }
            catch (Exception ex) { _error.Text = ex.Message; }
        }

        protected override void OnClosed(EventArgs e)
        {
            _password.Clear();
            if (DialogResult != true) Settings.Password = "";
            base.OnClosed(e);
        }
    }
}
