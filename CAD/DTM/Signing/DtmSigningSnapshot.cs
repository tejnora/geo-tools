using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CAD.DTM.Signing
{
    internal sealed class DtmSigningSnapshot : IDisposable
    {
        readonly string _directory;
        readonly List<(string Name, FileStream Stream, string Hash)> _files = new();

        DtmSigningSnapshot(string directory) { _directory = Path.GetFullPath(directory); }

        internal static async Task<DtmSigningSnapshot> CreateAsync(string directory)
        {
            var snapshot = new DtmSigningSnapshot(directory);
            try
            {
                foreach (var path in snapshot.ListFiles())
                {
                    var name = Path.GetRelativePath(snapshot._directory, path);
                    if (name.IndexOfAny(new[] { ';', '\r', '\n' }) >= 0)
                        throw new IOException("Název souboru nelze zapsat do soupisu ověření: " + name);
                    var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    // Register the stream before hashing so exceptions cannot leave an input locked.
                    snapshot._files.Add((name, stream, ""));
                    var hash = Convert.ToHexString(await SHA512.HashDataAsync(stream));
                    snapshot._files[^1] = (name, stream, hash);
                }
                if (snapshot._files.Count == 0) throw new IOException("Adresář neobsahuje žádné soubory k podpisu.");
                await snapshot.VerifyAsync();
                return snapshot;
            }
            catch { snapshot.Dispose(); throw; }
        }

        internal byte[] Manifest(string verificationNumber, DateTime verificationDate, string name)
        {
            foreach (var value in new[] { verificationNumber, name })
                if (string.IsNullOrWhiteSpace(value) || value.IndexOfAny(new[] { '\r', '\n' }) >= 0)
                    throw new InvalidOperationException("Vyplňte platné číslo ověření a jméno ověřovatele.");
            var lines = new List<string>
            {
                "Náležitostmi a přesností odpovídá právním předpisům.",
                verificationNumber,
                verificationDate.ToString("d. M. yyyy", CultureInfo.InvariantCulture),
                name,
                "----"
            };
            lines.AddRange(_files.Select(f => f.Name + ";" + f.Hash));
            return new UTF8Encoding(false, true).GetBytes(string.Join("\r\n", lines) + "\r\n");
        }

        internal async Task VerifyAsync(string stagingDirectory = null)
        {
            var names = ListFiles(stagingDirectory).Select(p => Path.GetRelativePath(_directory, p));
            if (!names.SequenceEqual(_files.Select(f => f.Name), StringComparer.Ordinal))
                throw new IOException("Obsah adresáře se během podepisování změnil. Spusťte export znovu.");
            foreach (var file in _files)
            {
                file.Stream.Position = 0;
                if (Convert.ToHexString(await SHA512.HashDataAsync(file.Stream)) != file.Hash)
                    throw new IOException("Soubor se během podepisování změnil: " + file.Name);
            }
        }

        IEnumerable<string> ListFiles(string stagingDirectory = null)
        {
            var files = new List<string>();
            Collect(_directory, files, stagingDirectory);
            return files.OrderBy(path => Path.GetRelativePath(_directory, path), StringComparer.Ordinal);
        }

        void Collect(string directory, List<string> files, string stagingDirectory)
        {
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("Podepisovaný adresář nesmí obsahovat odkazy na jiné adresáře: " + directory);
            foreach (var path in Directory.EnumerateFileSystemEntries(directory))
            {
                if (string.Equals(path, stagingDirectory, StringComparison.OrdinalIgnoreCase)) continue;
                var attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("Podepisovaný adresář obsahuje odkaz: " + path);
                if (directory == _directory && DtmSignaturePublisher.ArtifactNames.Contains(Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
                {
                    if ((attributes & FileAttributes.Directory) != 0)
                        throw new IOException("Název podpisového souboru je obsazen adresářem: " + path);
                    continue;
                }
                if ((attributes & FileAttributes.Directory) != 0) Collect(path, files, stagingDirectory);
                else files.Add(path);
            }
        }

        public void Dispose() { foreach (var file in _files) file.Stream.Dispose(); }
    }
}
