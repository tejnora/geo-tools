
using System.Collections.Generic;
using System.Text;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    class DtmCurveGeometry
    : IDtmGeometry
    {
        public DtmCurveGeometry()
        {
            SrsName = "EPSG:5514";
            SrsDimension = 3;
        }
        public string Id { get; set; }
        public string SrsName { get; set; }
        public int SrsDimension { get; set; }
        public List<DtmPoint> Points { get; set; }

        public void ExportToDtm(IDtmExporter exporter)
        {
            var posListData = new StringBuilder();
            foreach (var p in Points)
            {
                posListData.Append(p.ExportToDtm(3) + " ");
            }
            exporter.BeginElement(null, "GeometrieObjektu");
            exporter.BeginElement("gml", "curveProperty", false);
            exporter.BeginElement("gml", "LineString");
            exporter.AddAttribute("gml", "id", Id);
            exporter.AddAttribute("srsName", SrsName);
            exporter.AddAttribute("srsDimension", SrsDimension);
            exporter.AddElement("gml", "posList", posListData.ToString(0, posListData.Length - 1));
            exporter.EndElement();
            exporter.EndElement();
            exporter.EndElement();
        }
    }
}
