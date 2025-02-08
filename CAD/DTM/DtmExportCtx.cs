using System;
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
            FileName = GetSavedValue("Dtm/ExportFileName");
            NazevZakazky = GetSavedValue("Dtm/ExportNazevZakazky");
            Zpracovatel = GetSavedValue("Dtm/ExportZpracovatel");
            OrganizaceZpracovatele = GetSavedValue("Dtm/ExportOrganizaceZpracovatele");
        }
        string GetSavedValue(string key)
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

        protected override void ValidateFields()
        {
            var dictionary = LanguageConverter.ResolveDictionary();
            var msg = (string)dictionary.Translate("81", "Text", "PolozkaNemuzeBytPrazdna", typeof(string));
            if (string.IsNullOrEmpty(FileName)) SetFieldError(FileNameProperty, msg);
        }

    }
}
