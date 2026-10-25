using System;
using System.Security.Cryptography;
using System.Text;
using GeoBase;
using GeoBase.Utils;

namespace CAD.DTM.Signing
{
    internal sealed class DtmSigningSettings
    {
        internal const string DefaultTsaUrl = "https://www3.postsignum.cz/TSS/TSS_user/";
        static readonly byte[] Entropy = Encoding.UTF8.GetBytes("GeoCAD.DTM.Signing.v1");
        internal string CertificateThumbprint { get; set; } = "";
        internal string TsaUrl { get; set; } = DefaultTsaUrl;
        internal string UserName { get; set; } = "";
        internal string Password { get; set; } = "";
        internal bool RememberPassword { get; set; }
        internal string CredentialError { get; private set; }

        internal DtmSigningSettings Copy() => (DtmSigningSettings)MemberwiseClone();

        static string Read(string name) => SingletonsBase.Registry
            .getEntry(Registry.SubKey.kCurrentUser, "Dtm/Signing/" + name).getString("");
        static void Write(string name, string value) => SingletonsBase.Registry
            .setEntry(Registry.SubKey.kCurrentUser, "Dtm/Signing/" + name, new ProgramOption(value));

        internal static DtmSigningSettings Load()
        {
            var settings = new DtmSigningSettings
            {
                CertificateThumbprint = Read("Certificate"),
                UserName = Read("UserName")
            };
            var url = Read("TsaUrl");
            if (!string.IsNullOrWhiteSpace(url)) settings.TsaUrl = url;
            var protectedPassword = Read("Password");
            if (!string.IsNullOrEmpty(protectedPassword))
            {
                try
                {
                    settings.Password = UnprotectPassword(protectedPassword);
                    settings.RememberPassword = true;
                }
                catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
                {
                    settings.CredentialError = "Uložené heslo nelze načíst. Zadejte je znovu.";
                }
            }
            return settings;
        }

        internal void Save()
        {
            // Encrypt before changing settings so a DPAPI failure does not partially save a new identity.
            var secret = RememberPassword && !string.IsNullOrEmpty(Password) ? ProtectPassword(Password) : "";
            Write("Certificate", CertificateThumbprint);
            Write("TsaUrl", TsaUrl);
            Write("UserName", UserName);
            Write("Password", secret);
            CredentialError = null;
        }

        internal static string ProtectPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            try { return Convert.ToBase64String(ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser)); }
            finally { CryptographicOperations.ZeroMemory(bytes); }
        }

        internal static string UnprotectPassword(string encoded)
        {
            var bytes = ProtectedData.Unprotect(Convert.FromBase64String(encoded), Entropy, DataProtectionScope.CurrentUser);
            try { return Encoding.UTF8.GetString(bytes); }
            finally { CryptographicOperations.ZeroMemory(bytes); }
        }

        internal void Validate()
        {
            if (!Uri.TryCreate(TsaUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
                !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Fragment))
                throw new InvalidOperationException("Zadejte HTTPS adresu serveru časových razítek bez přihlašovacích údajů v adrese.");
            if (string.IsNullOrWhiteSpace(UserName) || UserName.Contains(':') || string.IsNullOrEmpty(Password))
                throw new InvalidOperationException("Vyplňte přihlašovací jméno a heslo pro časová razítka.");
            if (string.IsNullOrWhiteSpace(CertificateThumbprint))
                throw new InvalidOperationException("Vyberte podpisový certifikát.");
        }
    }
}
