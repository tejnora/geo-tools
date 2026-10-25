using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CAD.DTM.Signing
{
    internal static class DtmSigningCertificates
    {
        internal static X509Certificate2 Find(string thumbprint)
        {
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var certificates = store.Certificates;
            try
            {
                var certificate = certificates.Cast<X509Certificate2>().FirstOrDefault(c =>
                    string.Equals(c.Thumbprint, thumbprint, StringComparison.OrdinalIgnoreCase));
                if (certificate == null) throw new InvalidOperationException("Vybraný certifikát není dostupný v osobním úložišti Windows.");
                Validate(certificate);
                return new X509Certificate2(certificate);
            }
            finally { foreach (var certificate in certificates) certificate.Dispose(); }
        }

        internal static void Validate(X509Certificate2 certificate)
        {
            if (!certificate.HasPrivateKey) throw new InvalidOperationException("Certifikát nemá dostupný privátní klíč. Připojte podpisový token.");
            if (DateTime.Now < certificate.NotBefore || DateTime.Now > certificate.NotAfter)
                throw new InvalidOperationException("Podpisový certifikát není platný k dnešnímu dni.");
            var usage = certificate.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
            if (usage != null && (usage.KeyUsages & (X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation)) == 0)
                throw new InvalidOperationException("Certifikát není určen k elektronickému podepisování.");
            if (string.IsNullOrWhiteSpace(Name(certificate))) throw new InvalidOperationException("Certifikát neobsahuje jméno ověřovatele.");
        }

        internal static string Name(X509Certificate2 certificate) => certificate.GetNameInfo(X509NameType.SimpleName, false);
        internal static string Describe(X509Certificate2 certificate) =>
            $"{Name(certificate)}\nVydavatel: {certificate.GetNameInfo(X509NameType.SimpleName, true)}\nPlatnost do: {certificate.NotAfter:d}";

        internal static void ValidateTrust(X509Certificate2 certificate, DateTime verificationTime,
            X509Certificate2Collection extraCertificates = null, X509Certificate2Collection trustedRoots = null)
        {
            using var chain = new X509Chain();
            chain.ChainPolicy.VerificationTime = verificationTime;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            chain.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromSeconds(15);
            if (extraCertificates != null) chain.ChainPolicy.ExtraStore.AddRange(extraCertificates);
            // Custom roots are supplied only by the offline test harness, never by the signing UI.
            if (trustedRoots != null)
            {
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.CustomTrustStore.AddRange(trustedRoots);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.DisableCertificateDownloads = true;
            }
            if (!chain.Build(certificate))
                throw new CryptographicException("Nelze ověřit důvěryhodnost certifikátu: " +
                    string.Join(", ", chain.ChainStatus.Select(s => s.StatusInformation.Trim())));
        }
    }
}
