using GeoBase.Utils;

namespace GeoHelper.FileExport
{
    public class ExportException : LanguageDictionaryException
    {
        public ExportException(string id)
            : base(id)
        {
        }

        public ExportException(string id, ResourceParams @params)
            : base(id, @params)
        {
        }
    }
}