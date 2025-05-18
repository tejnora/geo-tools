using System;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    class DtmSurfaceGeometry
    : DtmPolygonGeometry
    {
        public override void ExportToDtm(IDtmExporter exporter)
        {
            exporter.BeginElement("gml", "surfaceProperty", false);
            base.ExportToDtmInternal(exporter, 2);
            exporter.EndElement();
        }
    }
}
