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
            var po = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, "Dtm/ExportFileName");
            FileName = po.getString("");
        }
        public readonly PropertyData FileNameProperty = RegisterProperty("FileName", typeof(string), string.Empty);
        public string FileName
        {
            get => GetValue<string>(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        protected override void ValidateFields()
        {
            var dictionary = LanguageConverter.ResolveDictionary();
            var msg = (string)dictionary.Translate("81", "Text", "PolozkaNemuzeBytPrazdna", typeof(string));
            if (string.IsNullOrEmpty(FileName)) SetFieldError(FileNameProperty, msg);
        }

    }
}
