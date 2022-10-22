using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables
{
    internal interface ITableBase
    {
        TableNodesBase GetNode(string uplneCislo);
    }
}