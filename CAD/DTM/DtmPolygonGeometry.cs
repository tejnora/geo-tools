using System;
using System.Collections.Generic;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    public class DtmPolygonGeometry
    : IDtmGeometry
    {
        public string Id { get; set; }
        public string SrsName { get; set; }
        public int SrsDimension { get; set; }
        public List<DtmPoint> Points { get; set; }

        public void ExportToDtm(IDtmExporter exporter)
        {
            throw new NotImplementedException();
        }
    }
}
