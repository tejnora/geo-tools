using System.IO;

namespace GeoHelper.Tabulky
{
    internal interface ILoadSaveMdi
    {
        FileInfo FileName { get; set; }
        Stream FileStream { get; set; }
        string SaveDialogFilter { get; }
        void Serialize(Stream aWriter);
        void Deserialize(Stream aReader);
        bool GetIsModifed();
        void RestModifedFlag();
    }
}