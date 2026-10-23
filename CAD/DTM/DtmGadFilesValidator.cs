using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CAD.DTM
{
    internal static class DtmGadFilesValidator
    {
        // CUZK methodology 2.1, section 5.2:
        // https://cuzk.gov.cz/DMVS/Metodika/Metodika_pro_geodety_k_aktualizaci_DTM_v2-1_final.aspx
        internal static DtmExportActionResult Validate(string directory, string fileNamePrefix, bool descriptionInSketch)
        {
            if (string.IsNullOrWhiteSpace(fileNamePrefix))
                return new DtmExportActionResult(DtmExportActionStatus.Failed, "Nelze sestavit názvy příloh: chybí platné označení GAD DTM.");
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return new DtmExportActionResult(DtmExportActionStatus.Failed, "Výstupní adresář neexistuje: " + directory);

            // Enumerate files, not directories or files in nested folders. Windows file names are case insensitive.
            var files = Directory.GetFiles(directory).Select(Path.GetFileName).ToArray();
            var requirements = new[]
            {
                (Code: "PP", Extension: ".pdf", Name: "Popisové pole"),
                (Code: "MN", Extension: ".pdf", Name: "Měřický náčrt"),
                (Code: "TZ", Extension: ".pdf", Name: "Technická zpráva"),
                (Code: "SS", Extension: ".txt", Name: "Seznam souřadnic")
            };
            var details = new List<string> { "Kontrolovaný adresář: " + directory };
            bool failed = false;
            foreach (var requirement in requirements)
            {
                var expected = fileNamePrefix + "_" + requirement.Code + requirement.Extension;
                if (files.Contains(expected, StringComparer.OrdinalIgnoreCase))
                {
                    details.Add($"V pořádku — {requirement.Name}: {expected}");
                    continue;
                }
                if (requirement.Code == "PP" && descriptionInSketch)
                {
                    details.Add("Popisové pole je podle nastavení součástí měřického náčrtu; samostatný soubor PP.pdf se nevyžaduje.");
                    continue;
                }

                failed = true;
                var candidates = files.Where(name => Regex.IsMatch(name,
                    @"(?:\A|_)" + requirement.Code + @"(?:\.|_|\z)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).ToArray();
                details.Add(candidates.Length == 0
                    ? $"Chybí — {requirement.Name}. Očekávaný soubor: {expected}"
                    : $"Nesprávný název — {requirement.Name}. Očekáváno: {expected}. Nalezeno: {string.Join(", ", candidates)}");
            }

            return new DtmExportActionResult(failed ? DtmExportActionStatus.Failed : DtmExportActionStatus.Passed,
                string.Join("\n\n", details));
        }
    }
}
