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
        public List<List<DtmPoint>> InteriorRings { get; set; } = new();

        public DtmPolygonGeometry()
        {
            SrsDimension = 3;
            SrsName = "EPSG:5514";
        }

        public virtual void ExportToDtm(IDtmExporter exporter)
        {
            ExportToDtmInternal(exporter, SrsDimension);
        }
        public virtual void ExportToDtmInternal(IDtmExporter exporter, int srcDimension)
        {
            exporter.BeginElement("gml", "Polygon", false);
            exporter.AddAttribute("gml", "id", Id);
            exporter.AddAttribute("srsName", SrsName);
            exporter.AddAttribute("srsDimension", SrsDimension);
            ExportRing(exporter, "exterior", Points, srcDimension);
            foreach (var ring in InteriorRings) ExportRing(exporter, "interior", ring, srcDimension);
            exporter.EndElement();
        }

        static void ExportRing(IDtmExporter exporter, string role, List<DtmPoint> points, int dimension)
        {
            exporter.BeginElement("gml", role);
            exporter.BeginElement("gml", "LinearRing");
            exporter.AddElement("gml", "posList", string.Join(" ", points.ConvertAll(p => p.ExportToDtm(dimension))));
            foreach (var point in points) exporter.MarkPoint(point);
            exporter.EndElement();
            exporter.EndElement();
        }
    }
}
