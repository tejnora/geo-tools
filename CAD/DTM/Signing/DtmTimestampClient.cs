using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CAD.DTM.Signing
{
    internal interface IDtmTimestampClient
    {
        Task<byte[]> SendAsync(byte[] request, DtmSigningSettings settings);
    }

    internal sealed class DtmTimestampClient : IDtmTimestampClient
    {
        readonly Func<HttpMessageHandler> _handlerFactory;
        readonly TimeSpan _timeout;

        internal DtmTimestampClient(Func<HttpMessageHandler> handlerFactory = null, TimeSpan? timeout = null)
        {
            _handlerFactory = handlerFactory ?? (() => new HttpClientHandler { AllowAutoRedirect = false });
            _timeout = timeout ?? TimeSpan.FromSeconds(45);
        }

        public async Task<byte[]> SendAsync(byte[] request, DtmSigningSettings settings)
        {
            settings.Validate();
            using var handler = _handlerFactory();
            using var client = new HttpClient(handler) { Timeout = _timeout };
            using var message = new HttpRequestMessage(HttpMethod.Post, settings.TsaUrl);
            var credentials = Encoding.UTF8.GetBytes(settings.UserName + ":" + settings.Password);
            try { message.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials)); }
            finally { CryptographicOperations.ZeroMemory(credentials); }
            message.Content = new ByteArrayContent(request);
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/timestamp-query");
            try
            {
                using var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    throw new InvalidOperationException("PostSignum odmítlo přihlášení. Zkontrolujte účet pro časová razítka.");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Server časových razítek vrátil HTTP {(int)response.StatusCode}.");
                if (!string.Equals(response.Content.Headers.ContentType?.MediaType, "application/timestamp-reply", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Server nevrátil odpověď ve formátu časového razítka.");
                if (response.Content.Headers.ContentLength > 1024 * 1024)
                    throw new InvalidOperationException("Odpověď serveru časových razítek je příliš velká.");
                using var timeout = new System.Threading.CancellationTokenSource(_timeout);
                using var stream = await response.Content.ReadAsStreamAsync(timeout.Token);
                using var buffer = new MemoryStream();
                var chunk = new byte[8192];
                int read;
                while ((read = await stream.ReadAsync(chunk.AsMemory(), timeout.Token)) > 0)
                {
                    if (buffer.Length + read > 1024 * 1024) throw new InvalidOperationException("Odpověď serveru časových razítek je příliš velká.");
                    buffer.Write(chunk, 0, read);
                }
                return buffer.ToArray();
            }
            catch (OperationCanceledException) { throw new InvalidOperationException("Vypršel časový limit pro získání časového razítka."); }
            catch (HttpRequestException) { throw new InvalidOperationException("Nelze se spojit se serverem časových razítek. Zkontrolujte připojení a adresu TSA."); }
        }

        internal static DateTimeOffset Verify(byte[] signature, byte[] response, Rfc3161TimestampRequest request,
            X509Certificate2Collection trustedRoots = null)
        {
            var token = request.ProcessResponse(response, out var consumed);
            if (consumed != response.Length || !token.VerifySignatureForData(signature, out var certificate))
                throw new CryptographicException("Časové razítko nemá platný podpis nebo neodpovídá podpisovému souboru.");
            var timestamp = token.TokenInfo.Timestamp;
            if (timestamp > DateTimeOffset.UtcNow.AddMinutes(5) || timestamp < DateTimeOffset.UtcNow.AddMinutes(-10))
                throw new CryptographicException("Čas časového razítka neodpovídá aktuálnímu času.");
            if (certificate.NotAfter.ToUniversalTime() < timestamp.UtcDateTime.AddYears(5))
                throw new CryptographicException("Certifikát časové autority nemá požadovanou zbývající platnost alespoň pět let.");
            DtmSigningCertificates.ValidateTrust(certificate, timestamp.UtcDateTime, token.AsSignedCms().Certificates, trustedRoots);
            return timestamp;
        }
    }
}
