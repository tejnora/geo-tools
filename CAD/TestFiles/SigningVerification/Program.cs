using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CAD.DTM;
using CAD.DTM.Gui;
using CAD.DTM.Signing;
using CAD.DTM.Validation;

internal static class Program
{
    static int _checks;
    static string _work;
    static void Check(bool value, string message)
    {
        if (!value) throw new Exception(message);
        _checks++;
    }
    static async Task Throws(Func<Task> action, string message)
    {
        try { await action(); }
        catch { _checks++; return; }
        throw new Exception(message);
    }
    static string DirectoryFor(string name)
    {
        var directory = Path.Combine(_work, name);
        Directory.CreateDirectory(directory);
        return directory;
    }
    static DtmSigningSettings Settings() => new()
    {
        CertificateThumbprint = "test-certificate", UserName = "test-user", Password = "test-password"
    };

    [STAThread]
    static int Main()
    {
        var result = 1;
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        app.Startup += async (_, _) =>
        {
            try
            {
                GeoBase.SingletonsBase.Registry.SuppressRegistry = true;
                _work = DirectoryForRun();
                await VerifySettings();
                await VerifySnapshot();
                await VerifyPublication();
                await VerifyTransport();
                using var certificates = new Certificates();
                await VerifyCryptography(certificates);
                await VerifySteps();
                await VerifyLiveProgress();
                VerifyWindows();
                Console.WriteLine($"PASS: {_checks} signing checks. Artifacts: {_work}");
                result = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine(ex); }
            finally { app.Shutdown(); }
        };
        app.Run();
        return result;
    }

    static string DirectoryForRun()
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "Verification", DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    static async Task VerifySettings()
    {
        var password = "heslo-žluťoučký-123";
        var protectedPassword = DtmSigningSettings.ProtectPassword(password);
        Check(!protectedPassword.Contains(password) && DtmSigningSettings.UnprotectPassword(protectedPassword) == password, "DPAPI round trip");
        await Throws(() => Task.Run(() => DtmSigningSettings.UnprotectPassword("invalid")), "Invalid DPAPI secret must fail");
        var settings = Settings();
        settings.RememberPassword = true;
        settings.Save();
        Check(DtmSigningSettings.Load().Password == settings.Password, "Remembered password loads");
        settings.RememberPassword = false;
        settings.Save();
        Check(DtmSigningSettings.Load().Password == "", "Turning remembering off removes saved secret");
        foreach (var url in new[] { "http://tsa.example", "https://user:secret@tsa.example", "invalid" })
        {
            settings.TsaUrl = url;
            await Throws(() => Task.Run(settings.Validate), "Invalid TSA URL accepted");
        }
        settings = Settings();
        settings.Password = "";
        await Throws(() => Task.Run(settings.Validate), "Empty TSA password accepted");
        await Throws(() => Task.Run(() => DtmDirectorySigner.ValidateVerification("", DateTime.Today)), "Empty verification number accepted");
        await Throws(() => Task.Run(() => DtmDirectorySigner.ValidateVerification("1/2026", DateTime.Today.AddDays(1))), "Future date accepted");
        var copy = Settings();
        var independent = copy.Copy();
        independent.Password = "changed";
        Check(copy.Password == "test-password", "Cancelling settings can preserve original credentials");
    }

    static async Task VerifySnapshot()
    {
        var directory = DirectoryFor("snapshot");
        var nested = Path.Combine(directory, "Přílohy");
        Directory.CreateDirectory(nested);
        var file = Path.Combine(nested, "náčrt.pdf");
        File.WriteAllText(file, "žluťoučký");
        File.WriteAllText(Path.Combine(directory, "a.txt"), "first");
        File.WriteAllText(Path.Combine(directory, "other.p7s"), "independent signature");
        foreach (var name in DtmSignaturePublisher.ArtifactNames) File.WriteAllText(Path.Combine(directory, name), "old");
        using (var snapshot = await DtmSigningSnapshot.CreateAsync(directory))
        {
            var manifest = snapshot.Manifest("8/2026", new DateTime(2026, 10, 20), "Ing. Žaneta Nová");
            var text = Encoding.UTF8.GetString(manifest);
            Check(text.StartsWith("Náležitostmi a přesností odpovídá právním předpisům.\r\n8/2026\r\n20. 10. 2026\r\nIng. Žaneta Nová\r\n----\r\n"), "Manifest header, date, UTF8 and CRLF");
            Check(text.Contains("Přílohy\\náčrt.pdf;" + Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes("žluťoučký")))), "SHA512 and relative Unicode path");
            Check(!text.Contains("Overeni") && text.Contains("other.p7s;"), "Only own root artifacts excluded");
            var lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Skip(5).ToArray();
            Check(lines.SequenceEqual(lines.OrderBy(l => l, StringComparer.Ordinal)), "Stable ordering");
            await Throws(() => Task.Run(() => File.WriteAllText(file, "changed")), "Inputs should be locked against writes");
            File.WriteAllText(Path.Combine(directory, "new.txt"), "new");
            await Throws(() => snapshot.VerifyAsync(), "New file during signing not detected");
        }
        File.WriteAllText(file, "unlocked");
        Check(File.ReadAllText(file) == "unlocked", "Input locks released");
        var invalid = DirectoryFor("invalid-names");
        File.WriteAllText(Path.Combine(invalid, "bad;name.txt"), "bad");
        await Throws(async () => { using var snapshot = await DtmSigningSnapshot.CreateAsync(invalid); }, "Ambiguous manifest filename accepted");
        await Throws(async () => { using var snapshot = await DtmSigningSnapshot.CreateAsync(DirectoryFor("empty")); }, "Empty directory accepted");
    }

    static async Task VerifyPublication()
    {
        var directory = DirectoryFor("publication");
        var originals = DtmSignaturePublisher.ArtifactNames.ToDictionary(n => n, n => Encoding.UTF8.GetBytes("old-" + n));
        foreach (var pair in originals) File.WriteAllBytes(Path.Combine(directory, pair.Key), pair.Value);
        var contents = new[] { new byte[] { 1 }, new byte[] { 2 }, new byte[] { 3 } };
        await Throws(() => DtmSignaturePublisher.PublishAsync(directory, contents, false, _ => Task.CompletedTask), "Unconfirmed replacement accepted");
        int moves = 0;
        await Throws(() => DtmSignaturePublisher.PublishAsync(directory, contents, true, _ => Task.CompletedTask, (source, target) =>
        {
            if (++moves == originals.Count + 2) throw new IOException("Injected write failure");
            File.Move(source, target);
        }), "Injected publication failure ignored");
        Check(originals.All(p => File.ReadAllBytes(Path.Combine(directory, p.Key)).SequenceEqual(p.Value)), "Old and legacy artifacts restored after partial publication");
        int verifications = 0;
        await Throws(() => DtmSignaturePublisher.PublishAsync(directory, contents, true, _ =>
        {
            if (++verifications == 2) throw new IOException("Input changed during publication");
            return Task.CompletedTask;
        }), "Final input verification not applied");
        Check(originals.All(p => File.ReadAllBytes(Path.Combine(directory, p.Key)).SequenceEqual(p.Value)), "Rollback on final verification failure");
        await DtmSignaturePublisher.PublishAsync(directory, contents, true, _ => Task.CompletedTask);
        Check(DtmSignaturePublisher.OutputNames.Select((n, i) => File.ReadAllBytes(Path.Combine(directory, n)).SequenceEqual(contents[i])).All(v => v), "New trio published");
        Check(DtmSignaturePublisher.ExistingArtifacts(directory).Length == 3 && !Directory.EnumerateDirectories(directory).Any(), "Legacy files and staging removed on success");
    }

    sealed class Handler : HttpMessageHandler
    {
        readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _send;
        internal Handler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) { _send = send; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => _send(request, token);
    }

    static async Task VerifyTransport()
    {
        var settings = Settings();
        var client = new DtmTimestampClient(() => new Handler(async (message, _) =>
        {
            Check(message.RequestUri.Scheme == "https" && message.Method == HttpMethod.Post, "TSA HTTPS POST");
            Check(message.Content.Headers.ContentType.MediaType == "application/timestamp-query", "TSA request MIME");
            Check(message.Headers.Authorization.Scheme == "Basic" && Encoding.UTF8.GetString(Convert.FromBase64String(message.Headers.Authorization.Parameter)) == "test-user:test-password", "TSA authentication");
            Check((await message.Content.ReadAsByteArrayAsync()).SequenceEqual(new byte[] { 1, 2 }), "TSA request body");
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(new byte[] { 3, 4 }) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/timestamp-reply");
            return response;
        }));
        Check((await client.SendAsync(new byte[] { 1, 2 }, settings)).SequenceEqual(new byte[] { 3, 4 }), "TSA reply");
        foreach (var status in new[] { HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden, HttpStatusCode.InternalServerError, HttpStatusCode.Redirect })
        {
            var failed = new DtmTimestampClient(() => new Handler((_, _) => Task.FromResult(new HttpResponseMessage(status))));
            await Throws(() => failed.SendAsync(new byte[] { 1 }, settings), "HTTP failure accepted");
        }
        var timeout = new DtmTimestampClient(() => new Handler(async (_, token) =>
        {
            await Task.Delay(10000, token);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }), TimeSpan.FromMilliseconds(50));
        await Throws(() => timeout.SendAsync(new byte[] { 1 }, settings), "TSA timeout ignored");
        var html = new DtmTimestampClient(() => new Handler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("login") })));
        await Throws(() => html.SendAsync(new byte[] { 1 }, settings), "Non-TSA reply accepted");
    }

    sealed class Certificates : IDisposable
    {
        internal X509Certificate2 Root { get; }
        internal X509Certificate2 Signer { get; }
        internal X509Certificate2 Tsa { get; }
        internal X509Certificate2Collection Roots => new(Root);
        internal Certificates()
        {
            using var rootKey = RSA.Create(2048);
            var rootRequest = new CertificateRequest("CN=GeoCAD offline test root", rootKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            rootRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            rootRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
            Root = rootRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddYears(12));
            Signer = Issue("CN=Ing. Žaneta Nová", false);
            Tsa = Issue("CN=GeoCAD offline TSA", true);
        }
        internal X509Certificate2 Issue(string name, bool tsa, int? years = null)
        {
            using var key = RSA.Create(2048);
            var request = new CertificateRequest(name, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, true));
            if (tsa) request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.8") }, true));
            using var certificate = request.Create(Root, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(years ?? (tsa ? 9 : 2)), RandomNumberGenerator.GetBytes(16));
            return certificate.CopyWithPrivateKey(key);
        }
        public void Dispose() { Signer.Dispose(); Tsa.Dispose(); Root.Dispose(); }
    }

    sealed class FakeTsa : IDtmTimestampClient
    {
        readonly X509Certificate2 _certificate;
        internal string Failure;
        internal Action BeforeReply;
        internal byte[] LastResponse;
        internal byte[] LastRequest;
        internal FakeTsa(X509Certificate2 certificate) { _certificate = certificate; }
        public async Task<byte[]> SendAsync(byte[] bytes, DtmSigningSettings settings)
        {
            await Task.Yield();
            BeforeReply?.Invoke();
            LastRequest = bytes;
            if (Failure == "timeout") throw new InvalidOperationException("Vypršel časový limit.");
            if (!Rfc3161TimestampRequest.TryDecode(bytes, out var request, out _)) throw new Exception("Invalid TSA request");
            var hash = request.GetMessageHash().ToArray();
            if (Failure == "hash") hash[0] ^= 1;
            var nonce = request.GetNonce().Value.ToArray();
            if (Failure == "nonce") nonce[^1] ^= 1;
            var info = new Rfc3161TimestampTokenInfo(new Oid("1.2.3.4.5"), request.HashAlgorithmId, hash,
                new byte[] { 1 }, DateTimeOffset.UtcNow, nonce: nonce);
            var cms = new SignedCms(new ContentInfo(new Oid("1.2.840.113549.1.9.16.1.4"), info.Encode()), false);
            var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, _certificate)
            {
                IncludeOption = X509IncludeOption.EndCertOnly,
                DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1")
            };
            var certificateId = new AsnWriter(AsnEncodingRules.DER);
            certificateId.PushSequence();
            certificateId.PushSequence();
            certificateId.PushSequence();
            certificateId.WriteOctetString(SHA256.HashData(_certificate.RawData));
            certificateId.PopSequence();
            certificateId.PopSequence();
            certificateId.PopSequence();
            signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), certificateId.Encode()));
            cms.ComputeSignature(signer);
            var signed = cms.Encode();
            if (Failure == "signature") signed[^1] ^= 1;
            var writer = new AsnWriter(AsnEncodingRules.DER);
            writer.PushSequence();
            writer.PushSequence();
            writer.WriteInteger(Failure == "rejected" ? 2 : 0);
            writer.PopSequence();
            if (Failure != "rejected") writer.WriteEncodedValue(signed);
            writer.PopSequence();
            return LastResponse = writer.Encode();
        }
    }

    static async Task VerifyCryptography(Certificates certificates)
    {
        var directory = DirectoryFor("signing");
        File.WriteAllText(Path.Combine(directory, "data.xml"), "<data/>");
        var tsa = new FakeTsa(certificates.Tsa);
        var signer = new DtmDirectorySigner(tsa, certificates.Roots);
        var settings = Settings();
        var good = await signer.SignWithCertificateAsync(directory, settings, "8/2026", DateTime.Today, false, certificates.Signer);
        Check(good.Status == DtmExportActionStatus.Passed, "Full signing flow: " + good.Detail);
        var manifest = File.ReadAllBytes(Path.Combine(directory, "Overeni.txt"));
        var signature = File.ReadAllBytes(Path.Combine(directory, "Overeni.txt.p7s"));
        var cms = new SignedCms(new ContentInfo(manifest), true);
        cms.Decode(signature);
        cms.CheckSignature(true);
        Check(cms.Detached && cms.SignerInfos[0].DigestAlgorithm.Value == "2.16.840.1.101.3.4.2.1", "Detached SHA256 signature");
        Check(cms.SignerInfos[0].SignedAttributes.Cast<CryptographicAttributeObject>().Any(a => a.Oid.Value == "1.2.840.113549.1.9.16.2.47"), "Certificate binding signed attribute");
        var altered = manifest.ToArray();
        altered[0] ^= 1;
        await Throws(() => Task.Run(() => { var bad = new SignedCms(new ContentInfo(altered), true); bad.Decode(signature); bad.CheckSignature(true); }), "Altered manifest verifies");
        Rfc3161TimestampRequest.TryDecode(tsa.LastRequest, out var timestampRequest, out _);
        Check(DtmTimestampClient.Verify(signature, tsa.LastResponse, timestampRequest, certificates.Roots) <= DateTimeOffset.UtcNow, "Timestamp validates");
        var previous = DtmSignaturePublisher.OutputNames.ToDictionary(n => n, n => File.ReadAllBytes(Path.Combine(directory, n)));
        foreach (var failure in new[] { "hash", "nonce", "signature", "rejected", "timeout" })
        {
            tsa.Failure = failure;
            var result = await signer.SignWithCertificateAsync(directory, settings, "9/2026", DateTime.Today, true, certificates.Signer);
            Check(result.Status == DtmExportActionStatus.Failed, "Invalid timestamp rejected: " + failure);
            Check(previous.All(p => File.ReadAllBytes(Path.Combine(directory, p.Key)).SequenceEqual(p.Value)), "Old signature preserved on failure: " + failure);
            Check(result.Detail.Contains("Dosavadní podpis"), "Stale signature notice");
        }
        tsa.Failure = null;
        tsa.BeforeReply = () => File.WriteAllText(Path.Combine(directory, "added.txt"), "concurrent change");
        Check((await signer.SignWithCertificateAsync(directory, settings, "9/2026", DateTime.Today, true, certificates.Signer)).Status == DtmExportActionStatus.Failed, "Concurrent directory changes rejected");
        tsa.BeforeReply = null;
        Check((await signer.SignWithCertificateAsync(directory, settings, "9/2026", DateTime.Today, true, certificates.Signer)).Status == DtmExportActionStatus.Passed, "Repeated signing succeeds");
        Check((await signer.SignWithCertificateAsync(Path.GetRelativePath(Environment.CurrentDirectory, directory), settings,
            "10/2026", DateTime.Today, true, certificates.Signer)).Status == DtmExportActionStatus.Passed, "Relative output directory supported");
        var noTrust = new DtmDirectorySigner(tsa);
        Check((await noTrust.SignWithCertificateAsync(directory, settings, "9/2026", DateTime.Today, true, certificates.Signer)).Status == DtmExportActionStatus.Failed, "Untrusted signer rejected");
        using var publicOnly = X509CertificateLoader.LoadCertificate(certificates.Signer.RawData);
        await Throws(() => Task.Run(() => DtmSigningCertificates.Validate(publicOnly)), "Certificate without key accepted");
        using var shortTsa = certificates.Issue("CN=Short-lived TSA", true, 1);
        var shortResult = await new DtmDirectorySigner(new FakeTsa(shortTsa), certificates.Roots)
            .SignWithCertificateAsync(directory, settings, "9/2026", DateTime.Today, true, certificates.Signer);
        Check(shortResult.Status == DtmExportActionStatus.Failed && shortResult.Detail.Contains("pět let"), "TSA must have five years remaining validity");
        using var otherAuthority = new Certificates();
        var untrustedTsa = await new DtmDirectorySigner(new FakeTsa(otherAuthority.Tsa), certificates.Roots)
            .SignWithCertificateAsync(directory, settings, "9/2026", DateTime.Today, true, certificates.Signer);
        Check(untrustedTsa.Status == DtmExportActionStatus.Failed && untrustedTsa.Detail.Contains("důvěryhodnost"), "Untrusted TSA rejected");
        using var expiredKey = RSA.Create(2048);
        var expiredRequest = new CertificateRequest("CN=Expired", expiredKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var expired = expiredRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-3), DateTimeOffset.UtcNow.AddDays(-1));
        await Throws(() => Task.Run(() => DtmSigningCertificates.Validate(expired)), "Expired certificate accepted");
    }

    static async Task VerifySteps()
    {
        int signatures = 0;
        var order = new List<string>();
        var valid = new DtmValidationResult(new[] { new DtmRuleResult("1", "Test") });
        var flow = new DtmExportProgress("output.xml", () => { order.Add("validation"); return valid; }, () => order.Add("write"),
            () => { order.Add("GAD"); return new DtmExportActionResult(DtmExportActionStatus.Passed, "OK"); }, async () =>
            {
                await Task.Delay(10);
                signatures++;
                order.Add("sign");
                return new DtmExportActionResult(DtmExportActionStatus.Passed, "OK");
            });
        await flow.RunAsync(() => Task.CompletedTask);
        Check(order.SequenceEqual(new[] { "validation", "write", "GAD", "sign" }), "Four-step ordering and async completion");
        await flow.RunAsync(() => Task.CompletedTask);
        Check(signatures == 1, "No duplicate signing");
        var failed = new DtmExportProgress("output.xml", () => valid, () => { },
            () => new DtmExportActionResult(DtmExportActionStatus.Failed, "Missing MN"),
            () => { signatures++; return Task.FromResult(new DtmExportActionResult(DtmExportActionStatus.Passed, "OK")); });
        await failed.RunAsync(() => Task.CompletedTask);
        Check(failed.Actions[3].Status == DtmExportActionStatus.Skipped && signatures == 1, "Missing GAD attachments skip signing");
        var off = new DtmExportProgress("output.xml", () => valid, () => { }, () => new DtmExportActionResult(DtmExportActionStatus.Passed, "OK"));
        Check(off.Actions.Count == 3, "Unchecked checkbox has no signing step");
        var warning = new DtmRuleResult("1", "Warning");
        warning.AddWarning("Object", "Warning");
        var warned = new DtmExportProgress("output.xml", () => new DtmValidationResult(new[] { warning }), () => { },
            () => new DtmExportActionResult(DtmExportActionStatus.Passed, "OK"),
            () => { signatures++; return Task.FromResult(new DtmExportActionResult(DtmExportActionStatus.Passed, "OK")); });
        await warned.RunAsync(() => Task.CompletedTask);
        Check(warned.Actions[3].Status == DtmExportActionStatus.Passed && signatures == 2, "Validation warnings allow signing");
    }

    static async Task VerifyLiveProgress()
    {
        DtmExportProgressDialog dialog = null;
        bool signingStarted = false;
        var progress = new DtmExportProgress("output.xml", () => new DtmValidationResult(new[] { new DtmRuleResult("1", "OK") }),
            () => { }, () => new DtmExportActionResult(DtmExportActionStatus.Passed, "OK"), async () =>
            {
                signingStarted = true;
                Check(dialog.IsVisible && !((Button)dialog.FindName("_close")).IsEnabled, "Live dialog stays open while signing");
                Check(progressActionStatus(dialog) == DtmExportActionStatus.Running, "Live signing status rendered");
                dialog.Close();
                Check(dialog.IsVisible, "Close blocked during asynchronous signing");
                await Task.Delay(50);
                return new DtmExportActionResult(DtmExportActionStatus.Passed, "Test signature");
            });
        dialog = new DtmExportProgressDialog(progress);
        dialog.Show();
        for (int attempt = 0; attempt < 200 && !((Button)dialog.FindName("_close")).IsEnabled; attempt++) await Task.Delay(10);
        Check(signingStarted && ((Button)dialog.FindName("_close")).IsEnabled && progress.Actions[3].Status == DtmExportActionStatus.Passed, "Live async signing completed and close enabled");
        Render(dialog, "progress-four-steps", 760, 400);
        dialog.Close();
        Check(!dialog.IsVisible, "Live dialog closes after signing");

        static DtmExportActionStatus progressActionStatus(Window window) => ((DtmExportProgress)window.DataContext).Actions[3].Status;
    }

    static void VerifyWindows()
    {
        var settings = new DtmSigningSettingsDialog(Settings());
        Render(settings, "settings", 540, 480);
        Render(settings, "settings-wide", 760, 660);
        Check(((PasswordBox)settings.FindName("_password")).Password == "test-password", "PasswordBox populated");
        settings.Close();
        Check(settings.Settings.Password == "", "Cancel clears settings dialog secret");
        var culture = System.Globalization.CultureInfo.GetCultureInfo("cs-CZ");
        GeoBase.Localization.LanguageDictionary.RegisterDictionary(culture, new GeoBase.Localization.XmlLanguageDictionary(
            Path.Combine(AppContext.BaseDirectory, "Languages", "cs-CZ.xml"), ""));
        GeoBase.Localization.LanguageContext.Instance.Culture = culture;
        var owner = new Window();
        _ = new System.Windows.Interop.WindowInteropHelper(owner).EnsureHandle();
        Application.Current.MainWindow = owner;
        var ctx = new DtmExportCtx();
        Check(!ctx.SignOutputDirectory, "Signing is opt-in per export");
        var export = new DtmExportDialog(ctx);
        ctx.SignOutputDirectory = true;
        export.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() => { }));
        Render(export, "export-signing", 540, 500);
        Render(export, "export-signing-wide", 760, 850);
        export.Close();
        owner.Close();
    }

    static void Render(Window window, string name, double width, double height)
    {
        var content = (FrameworkElement)window.Content;
        content.Measure(new Size(width, height));
        content.Arrange(new Rect(0, 0, width, height));
        content.UpdateLayout();
        var visual = new DrawingVisual();
        using (var drawing = visual.RenderOpen())
        {
            drawing.DrawRectangle(window.Background, null, new Rect(0, 0, width, height));
            var brush = new VisualBrush(content) { Stretch = Stretch.Fill };
            drawing.DrawRectangle(brush, null, new Rect(content.Margin.Left, content.Margin.Top, content.ActualWidth, content.ActualHeight));
        }
        var bitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(visual);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = File.Create(Path.Combine(_work, name + ".png"));
        encoder.Save(stream);
        Check(content.ActualWidth > 0 && content.ActualHeight > 0, "Rendered " + name);
    }
}
