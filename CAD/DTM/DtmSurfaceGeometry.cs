using System;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    public class DtmSurfaceGeometry
    : DtmPolygonGeometry
    {
        public override void ExportToDtm(IDtmExporter exporter)
        {
            exporter.BeginElement("gml", "surfaceProperty", false);
            base.ExportToDtmInternal(exporter, SrsDimension);
            exporter.EndElement();
        }
    }
}
