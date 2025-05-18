using CAD.DTM.Gui;
using System.Collections.Generic;
namespace CAD.DTM
{
    public interface IDtmGeometryGroup
    {
        IList<IDtmGeometry> Geometries { get; }
        T GetDrawGeometry<T>();
        void ExportToDtm(IDtmExporter exporter);
    }
}
