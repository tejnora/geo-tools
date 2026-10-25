using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CAD.DTM.Signing
{
    internal static class DtmSignaturePublisher
    {
        internal static readonly string[] OutputNames = { "Overeni.txt", "Overeni.txt.p7s", "Overeni.txt.p7s.tsr" };
        internal static readonly string[] ArtifactNames = OutputNames.Concat(new[]
        {
            "Overeni.txt.p7s.tsq", "Overeni_UOZI.txt", "Overeni_UOZI.txt.p7s",
            "Overeni_UOZI.txt.p7s.tsr", "Overeni_UOZI.txt.p7s.tsq"
        }).ToArray();

        internal static string[] ExistingArtifacts(string directory) => ArtifactNames
            .Select(n => Path.Combine(directory, n)).Where(File.Exists).ToArray();

        internal static async Task PublishAsync(string directory, byte[][] contents, bool replaceConfirmed,
            Func<string, Task> verifyInputs, Action<string, string> move = null)
        {
            if (contents.Length != OutputNames.Length) throw new ArgumentException("Neúplný podpis.", nameof(contents));
            move ??= (source, target) => File.Move(source, target);
            // Staging is on the target volume so each move and rollback stays on one filesystem.
            var staging = Path.Combine(directory, ".geocad-sign-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(staging);
            var backups = new List<(string Original, string Backup)>();
            var installed = new List<string>();
            bool keepRecoveryFiles = false;
            try
            {
                for (int i = 0; i < contents.Length; i++)
                    await File.WriteAllBytesAsync(Path.Combine(staging, OutputNames[i]), contents[i]);
                var existing = ExistingArtifacts(directory);
                if (existing.Length > 0 && !replaceConfirmed)
                    throw new IOException("Adresář již obsahuje podpis. Jeho nahrazení nebylo potvrzeno.");
                await verifyInputs(staging);
                foreach (var target in existing)
                {
                    var backup = Path.Combine(staging, Path.GetFileName(target) + ".backup");
                    move(target, backup);
                    backups.Add((target, backup));
                }
                foreach (var name in OutputNames)
                {
                    var target = Path.Combine(directory, name);
                    move(Path.Combine(staging, name), target);
                    installed.Add(target);
                }
                await verifyInputs(staging);
            }
            catch (Exception failure)
            {
                try
                {
                    foreach (var target in installed) File.Delete(target);
                    foreach (var backup in backups.AsEnumerable().Reverse()) File.Move(backup.Backup, backup.Original);
                }
                catch (Exception recoveryFailure)
                {
                    keepRecoveryFiles = true;
                    throw new IOException("Podpis se nepodařilo uložit ani plně obnovit. Záložní soubory zůstaly v: " + staging,
                        new AggregateException(failure, recoveryFailure));
                }
                throw;
            }
            finally
            {
                // Remove only the uniquely named directory created by this operation.
                // If recovery failed, retain backups for manual restoration.
                if (!keepRecoveryFiles)
                {
                    try { Directory.Delete(staging, true); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
        }
    }
}
