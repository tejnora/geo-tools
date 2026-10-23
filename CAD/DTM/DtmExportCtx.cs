using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeoBase;
using GeoBase.Localization;
using GeoBase.Utils;
using System.Runtime.Serialization;
namespace CAD.DTM
{
    public class DtmExportCtx : DataObjectBase<DtmExportCtx>
    {
        public DtmExportCtx() : base(null, new StreamingContext())
        {
            FileName = ReadFromRegistry("Dtm/ExportFileName");
            OutputDirectory = ReadFromRegistry("Dtm/ExportDirectory");
            if (string.IsNullOrWhiteSpace(OutputDirectory) && !string.IsNullOrWhiteSpace(FileName))
            {
                try { OutputDirectory = Path.GetDirectoryName(FileName); }
                catch (ArgumentException) { }
            }
            UidSubjektu = ReadFromRegistry("Dtm/ExportUidSubjektu");
            PrefixKraje = ReadFromRegistry("Dtm/ExportPrefixKraje");
            Cgad = ReadFromRegistry("Dtm/ExportCgad");
            NazevZakazky = ReadFromRegistry("Dtm/ExportNazevZakazky");
            Zpracovatel = ReadFromRegistry("Dtm/ExportZpracovatel");
            OrganizaceZpracovatele = ReadFromRegistry("Dtm/ExportOrganizaceZpracovatele");
            DatumMereni = ReadFromRegistryDateTime("Dtm/ExportDatumMereni");
            DatumZpracovani = ReadFromRegistryDateTime("Dtm/ExportDatumZpracovani");
            AZI = ReadFromRegistry("Dtm/ExportAZI");
            DatumOvereni = ReadFromRegistryDateTime("Dtm/ExportDatumOvereni");
            CisloOvereni = ReadFromRegistry("Dtm/ExportCisloOvereni");
        }

        public void SaveValuesToRegistry()
        {
            SaveToRegistry(OutputDirectory, "Dtm/ExportDirectory");
            SaveToRegistry(UidSubjektu, "Dtm/ExportUidSubjektu");
            SaveToRegistry(PrefixKraje, "Dtm/ExportPrefixKraje");
            SaveToRegistry(Cgad, "Dtm/ExportCgad");
            SaveToRegistry(FileName, "Dtm/ExportFileName");
            SaveToRegistry(NazevZakazky, "Dtm/ExportNazevZakazky");
            SaveToRegistry(Zpracovatel, "Dtm/ExportZpracovatel");
            SaveToRegistry(OrganizaceZpracovatele, "Dtm/ExportOrganizaceZpracovatele");
            SaveToRegistry(DatumMereni.ToLongDateString(), "Dtm/ExportDatumMereni");
            SaveToRegistry(DatumZpracovani.ToLongDateString(), "Dtm/ExportDatumZpracovani");
            SaveToRegistry(AZI, "Dtm/ExportAZI");
            SaveToRegistry(DatumOvereni.ToLongDateString(), "Dtm/ExportDatumOvereni");
            SaveToRegistry(CisloOvereni, "Dtm/ExportCisloOvereni");
        }

        void SaveToRegistry(string value, string key)
        {
            SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, key, new ProgramOption(value));
        }
        DateTime ReadFromRegistryDateTime(string key)
        {
            var po = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, key);
            var value = po.getString("");
            return DateTime.TryParse(value, out var date) ? date : DateTime.Now;
        }

        string ReadFromRegistry(string key)
        {
            var po = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, key);
            return po.getString("");
        }
        public readonly PropertyData NazevZakazkyProperty = RegisterProperty("NazevZakazky", typeof(string), string.Empty);
        public string NazevZakazky
        {
            get => GetValue<string>(NazevZakazkyProperty);
            set => SetValue(NazevZakazkyProperty, value);
        }
        public readonly PropertyData FileNameProperty = RegisterProperty("FileName", typeof(string), string.Empty);
        public string FileName
        {
            get => GetValue<string>(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        public readonly PropertyData ZpracovatelProperty = RegisterProperty("Zpracovatel", typeof(string), string.Empty);
        public string Zpracovatel
        {
            get => GetValue<string>(ZpracovatelProperty);
            set => SetValue(ZpracovatelProperty, value);
        }
        public readonly PropertyData OrganizaceZpracovateleProperty = RegisterProperty("OrganizaceZpracovatele", typeof(string), string.Empty);
        public string OrganizaceZpracovatele
        {
            get => GetValue<string>(OrganizaceZpracovateleProperty);
            set => SetValue(OrganizaceZpracovateleProperty, value);
        }
        public readonly PropertyData DatumMereniProperty = RegisterProperty("DatumMereni", typeof(DateTime), DateTime.Now);
        public DateTime DatumMereni
        {
            get => GetValue<DateTime>(DatumMereniProperty);
            set => SetValue(DatumMereniProperty, value);
        }
        public readonly PropertyData DatumZpracovaniProperty = RegisterProperty("DatumZpracovani", typeof(DateTime), DateTime.Now);
        public DateTime DatumZpracovani
        {
            get => GetValue<DateTime>(DatumZpracovaniProperty);
            set => SetValue(DatumZpracovaniProperty, value);
        }
        public readonly PropertyData AZIProperty = RegisterProperty("AZI", typeof(string), string.Empty);
        public string AZI
        {
            get => GetValue<string>(AZIProperty);
            set => SetValue(AZIProperty, value);
        }
        public readonly PropertyData DatumOvereniProperty = RegisterProperty("DatumOvereni", typeof(DateTime), DateTime.Now);
        public DateTime DatumOvereni
        {
            get => GetValue<DateTime>(DatumOvereniProperty);
            set => SetValue(DatumOvereniProperty, value);
        }
        public readonly PropertyData CisloOvereniProperty = RegisterProperty("CisloOvereni", typeof(string), string.Empty);
        public string CisloOvereni
        {
            get => GetValue<string>(CisloOvereniProperty);
            set => SetValue(CisloOvereniProperty, value);
        }

        // FileName remains writable for internal XSD validation exports.
        public static IReadOnlyList<KeyValuePair<string, string>> Regions { get; } = Array.AsReadOnly(new[]
        {
            new KeyValuePair<string, string>("CZ010", "Hlavní město Praha (CZ010)"),
            new KeyValuePair<string, string>("CZ020", "Středočeský kraj (CZ020)"),
            new KeyValuePair<string, string>("CZ031", "Jihočeský kraj (CZ031)"),
            new KeyValuePair<string, string>("CZ032", "Plzeňský kraj (CZ032)"),
            new KeyValuePair<string, string>("CZ041", "Karlovarský kraj (CZ041)"),
            new KeyValuePair<string, string>("CZ042", "Ústecký kraj (CZ042)"),
            new KeyValuePair<string, string>("CZ051", "Liberecký kraj (CZ051)"),
            new KeyValuePair<string, string>("CZ052", "Královéhradecký kraj (CZ052)"),
            new KeyValuePair<string, string>("CZ053", "Pardubický kraj (CZ053)"),
            new KeyValuePair<string, string>("CZ063", "Kraj Vysočina (CZ063)"),
            new KeyValuePair<string, string>("CZ064", "Jihomoravský kraj (CZ064)"),
            new KeyValuePair<string, string>("CZ071", "Olomoucký kraj (CZ071)"),
            new KeyValuePair<string, string>("CZ072", "Zlínský kraj (CZ072)"),
            new KeyValuePair<string, string>("CZ080", "Moravskoslezský kraj (CZ080)"),
        });

        public readonly PropertyData OutputDirectoryProperty = RegisterProperty("OutputDirectory", typeof(string), string.Empty);
        public string OutputDirectory
        {
            get => GetValue<string>(OutputDirectoryProperty);
            set
            {
                SetValue(OutputDirectoryProperty, value);
                OnPropertyChanged(nameof(GeneratedFileName));
                OnPropertyChanged(nameof(OutputPath));
            }
        }
        public readonly PropertyData UidSubjektuProperty = RegisterProperty("UidSubjektu", typeof(string), string.Empty);
        public string UidSubjektu
        {
            get => GetValue<string>(UidSubjektuProperty);
            set
            {
                SetValue(UidSubjektuProperty, value);
                OnPropertyChanged(nameof(GeneratedFileName));
                OnPropertyChanged(nameof(OutputPath));
            }
        }
        public readonly PropertyData PrefixKrajeProperty = RegisterProperty("PrefixKraje", typeof(string), string.Empty);
        public string PrefixKraje
        {
            get => GetValue<string>(PrefixKrajeProperty);
            set
            {
                SetValue(PrefixKrajeProperty, value);
                OnPropertyChanged(nameof(GeneratedFileName));
                OnPropertyChanged(nameof(OutputPath));
            }
        }
        public readonly PropertyData CgadProperty = RegisterProperty("Cgad", typeof(string), string.Empty);
        public string Cgad
        {
            get => GetValue<string>(CgadProperty);
            set
            {
                SetValue(CgadProperty, value);
                OnPropertyChanged(nameof(GeneratedFileName));
                OnPropertyChanged(nameof(OutputPath));
            }
        }

        bool HasValidNaming => Regex.IsMatch(UidSubjektu ?? "", @"\ASUBJ-[0-9]{8}\z")
            && Regions.Any(r => r.Key == PrefixKraje)
            && Regex.IsMatch(Cgad ?? "", @"\A[0-9]{1,5}\z");

        public string GeneratedFileName => HasValidNaming
            ? $"{UidSubjektu}_{PrefixKraje}_{Cgad.PadLeft(5, '0')}_JVF.jvf.xml" : string.Empty;

        public string OutputPath => HasValidNaming && Directory.Exists(OutputDirectory)
            ? Path.Combine(OutputDirectory, GeneratedFileName) : string.Empty;

        public bool PrepareOutputFile()
        {
            Validate();
            if (HasErrors) return false;
            FileName = OutputPath;
            return true;
        }

        protected override void ValidateFields()
        {
            if (!Directory.Exists(OutputDirectory))
                SetFieldError(OutputDirectoryProperty, ExportText("DtmExportDirectoryError", "Vyberte existující výstupní adresář."));
            if (!Regex.IsMatch(UidSubjektu ?? "", @"\ASUBJ-[0-9]{8}\z"))
                SetFieldError(UidSubjektuProperty, ExportText("DtmExportUidError", "UID musí mít tvar SUBJ-00000001 (osm číslic)."));
            if (!Regions.Any(r => r.Key == PrefixKraje))
                SetFieldError(PrefixKrajeProperty, ExportText("DtmExportRegionError", "Vyberte kraj."));
            if (!Regex.IsMatch(Cgad ?? "", @"\A[0-9]{1,5}\z"))
                SetFieldError(CgadProperty, ExportText("DtmExportCgadError", "CGAD musí obsahovat 1 až 5 číslic."));
        }
        internal static string ExportText(string id, string fallback) =>
            (string)LanguageConverter.ResolveDictionary().Translate(id, "Text", fallback, typeof(string));
    }
}
