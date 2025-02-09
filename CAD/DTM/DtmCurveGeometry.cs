
using System.Collections.Generic;
using System.Text;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    class DtmCurveGeometry
    : IDtmGeometry
    {
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
            exporter.BeginElement("gml", "curveProperty", true);
            exporter.BeginElement("gml", "LineString");
            exporter.AddAttribute("id", Id);
            exporter.AddAttribute("srsName", SrsName);
            exporter.AddAttribute("srsDimension", SrsDimension);
            exporter.AddElement("gml", "posList", posListData.ToString(0, posListData.Length - 1));
            exporter.EndElement();
            exporter.EndElement();
            exporter.EndElement();
        }
    }
}
