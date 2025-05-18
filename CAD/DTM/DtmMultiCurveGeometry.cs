using CAD.DTM.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAD.DTM
{
    class DtmMultiCurveGeometry
    : DtmCurveGeometry
    {
        public override void ExportToDtm(IDtmExporter exporter)
        {
            var posListData = new StringBuilder();
            foreach (var p in Points)
            {
                posListData.Append(p.ExportToDtm(3) + " ");
                exporter.MarkPoint(p);
            }
            exporter.BeginElement("gml", "multiCurveProperty", false);
            exporter.BeginElement("gml", "MultiCurve");
            exporter.AddAttribute("gml", "id", Id);
            exporter.AddAttribute("srsName", SrsName);
            exporter.AddAttribute("srsDimension", SrsDimension);
            exporter.BeginElement("gml", "curveMember");
            exporter.BeginElement("gml", "LineString");
            exporter.AddElement("gml", "posList", posListData.ToString(0, posListData.Length - 1));
            exporter.EndElement();
            exporter.EndElement();
            exporter.EndElement();
            exporter.EndElement();
        }
    }
}
