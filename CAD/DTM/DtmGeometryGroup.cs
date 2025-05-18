using CAD.DTM.Gui;
using System.Collections.Generic;

namespace CAD.DTM
{
    class DtmGeometryGroup
        : IDtmGeometryGroup
    {
        public IList<IDtmGeometry> Geometries { get; set; }

        T IDtmGeometryGroup.GetDrawGeometry<T>()
        {
            foreach (var geometry in Geometries)
            {
                if (geometry is T geometryRes)
                    return geometryRes;
            }
            throw new System.NotImplementedException();
        }

        public void ExportToDtm(IDtmExporter exporter)
        {
            foreach (var geometry in Geometries)
            {
                geometry.ExportToDtm(exporter);
            }
        }
    }
}
