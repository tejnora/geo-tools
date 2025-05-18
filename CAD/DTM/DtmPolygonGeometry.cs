using System.Collections.Generic;
using System.Text;
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

        public virtual void ExportToDtm(IDtmExporter exporter)
        {
            ExportToDtmInternal(exporter, 3);
        }
        public virtual void ExportToDtmInternal(IDtmExporter exporter, int srcDimension)
        {
            var posListData = new StringBuilder();
            foreach (var p in Points)
            {
                posListData.Append(p.ExportToDtm(srcDimension) + " ");
                exporter.MarkPoint(p);
            }
            exporter.BeginElement("gml", "Polygon", false);
            exporter.AddAttribute("gml", "id", Id);
            exporter.AddAttribute("srsName", SrsName);
            exporter.AddAttribute("srsDimension", SrsDimension);
            exporter.BeginElement("gml", "exterior");
            exporter.BeginElement("gml", "LinearRing");
            exporter.AddElement("gml", "posList", posListData.ToString(0, posListData.Length - 1));
            exporter.EndElement();
            exporter.EndElement();
            exporter.EndElement();
        }
    }
}
