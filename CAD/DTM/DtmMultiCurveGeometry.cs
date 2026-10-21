using System.Collections.Generic;
using System.Linq;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    public class DtmMultiCurveGeometry : DtmCurveGeometry
    {
        public List<DtmCurveGeometry> Members { get; set; } = new();
        public IEnumerable<DtmCurveGeometry> Curves => Members.Count > 0 ? Members : new[] { new DtmCurveGeometry { Points = Points, SrsDimension = SrsDimension, SrsName = SrsName } };

        public override void ExportToDtm(IDtmExporter exporter)
        {
            exporter.BeginElement("gml", "multiCurveProperty");
            exporter.BeginElement("gml", "MultiCurve");
            exporter.AddAttribute("gml", "id", Id);
            exporter.AddAttribute("srsName", SrsName);
            exporter.AddAttribute("srsDimension", SrsDimension);
            foreach (var member in Curves)
            {
                exporter.BeginElement("gml", "curveMember");
                exporter.BeginElement("gml", "LineString");
                if (!string.IsNullOrEmpty(member.Id)) exporter.AddAttribute("gml", "id", member.Id);
                exporter.AddAttribute("srsName", member.SrsName);
                exporter.AddAttribute("srsDimension", member.SrsDimension);
                exporter.AddElement("gml", "posList", string.Join(" ", member.Points.Select(p => p.ExportToDtm(member.SrsDimension))));
                foreach (var point in member.Points) exporter.MarkPoint(point);
                exporter.EndElement();
                exporter.EndElement();
            }
            exporter.EndElement();
            exporter.EndElement();
        }
    }
}
