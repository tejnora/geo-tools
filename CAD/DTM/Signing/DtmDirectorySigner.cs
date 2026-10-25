using System;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CAD.DTM.Signing
{
    internal sealed class DtmDirectorySigner
    {
        readonly IDtmTimestampClient _timestampClient;
        readonly X509Certificate2Collection _trustedRoots;

        internal DtmDirectorySigner(IDtmTimestampClient timestampClient = null, X509Certificate2Collection trustedRoots = null)
        {
            _timestampClient = timestampClient ?? new DtmTimestampClient();
            _trustedRoots = trustedRoots;
        }

        internal async Task<DtmExportActionResult> SignAsync(string directory, DtmSigningSettings settings,
            string verificationNumber, DateTime verificationDate, bool replaceConfirmed)
        {
            try
            {
                settings.Validate();
                using var certificate = DtmSigningCertificates.Find(settings.CertificateThumbprint);
                return await SignWithCertificateAsync(directory, settings, verificationNumber, verificationDate, replaceConfirmed, certificate);
            }
            catch (Exception ex) { return Failure(directory, ex); }
        }

        internal async Task<DtmExportActionResult> SignWithCertificateAsync(string directory, DtmSigningSettings settings,
            string verificationNumber, DateTime verificationDate, bool replaceConfirmed, X509Certificate2 certificate)
        {
            try
            {
                directory = Path.GetFullPath(directory);
                settings.Validate();
                DtmSigningCertificates.Validate(certificate);
                ValidateVerification(verificationNumber, verificationDate);
                await Task.Run(() => DtmSigningCertificates.ValidateTrust(certificate, DateTime.Now, trustedRoots: _trustedRoots));
                using var snapshot = await DtmSigningSnapshot.CreateAsync(directory);
                var manifest = snapshot.Manifest(verificationNumber, verificationDate, DtmSigningCertificates.Name(certificate));
                // ComputeSignature remains on the UI thread so Windows can display a token/PIN dialog.
                var signature = CreateSignature(manifest, certificate);
                var request = Rfc3161TimestampRequest.CreateFromData(signature, HashAlgorithmName.SHA256,
                    nonce: RandomNumberGenerator.GetBytes(16), requestSignerCertificates: true);
                var response = await _timestampClient.SendAsync(request.Encode(), settings);
                var timestamp = await Task.Run(() => DtmTimestampClient.Verify(signature, response, request, _trustedRoots));
                await snapshot.VerifyAsync();
                await DtmSignaturePublisher.PublishAsync(directory, new[] { manifest, signature, response }, replaceConfirmed, snapshot.VerifyAsync);
                return new DtmExportActionResult(DtmExportActionStatus.Passed,
                    $"Certifikát: {DtmSigningCertificates.Describe(certificate)}\nOtisk certifikátu: {certificate.Thumbprint}\n" +
                    $"Časové razítko: {timestamp.ToLocalTime():G}\n" + string.Join("\n", DtmSignaturePublisher.OutputNames.Select(n => Path.Combine(directory, n))));
            }
            catch (Exception ex)
            {
                return Failure(directory, ex);
            }
        }

        static DtmExportActionResult Failure(string directory, Exception exception)
        {
            var previous = DtmSignaturePublisher.ExistingArtifacts(directory).Length > 0
                ? "\nDosavadní podpis nemusí odpovídat nově exportovaným souborům." : "";
            return new DtmExportActionResult(DtmExportActionStatus.Failed, exception.Message + previous);
        }

        internal static void ValidateVerification(string number, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(number) || number.IndexOfAny(new[] { '\r', '\n' }) >= 0)
                throw new InvalidOperationException("Vyplňte číslo z evidence ověřovaných výsledků.");
            if (date.Date > DateTime.Today || date.Year < 1900)
                throw new InvalidOperationException("Datum ověření nesmí být v budoucnosti a musí být platné.");
        }

        internal static byte[] CreateSignature(byte[] manifest, X509Certificate2 certificate)
        {
            var cms = new SignedCms(new ContentInfo(manifest), detached: true);
            var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificate)
            {
                DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1"),
                IncludeOption = X509IncludeOption.ExcludeRoot
            };
            signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
            // SigningCertificateV2 binds the signer's certificate into the signed attributes.
            var writer = new AsnWriter(AsnEncodingRules.DER);
            writer.PushSequence();
            writer.PushSequence();
            writer.PushSequence();
            writer.WriteOctetString(SHA256.HashData(certificate.RawData));
            writer.PopSequence();
            writer.PopSequence();
            writer.PopSequence();
            signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), writer.Encode()));
            cms.ComputeSignature(signer, silent: false);
            var encoded = cms.Encode();
            var verified = new SignedCms(new ContentInfo(manifest), detached: true);
            verified.Decode(encoded);
            verified.CheckSignature(verifySignatureOnly: true);
            return encoded;
        }
    }
}
