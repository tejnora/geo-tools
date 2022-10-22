using System.IO;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;

namespace GeoHelper.FileParses
{
    public interface IUserPattern
    {
        void ParseFile(FileInfo location);
        TableNodesBase GetNewNode();
    };
}