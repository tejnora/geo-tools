using CAD.DTM.Gui;

namespace CAD.DTM
{
    class DtmPointGeometry
        : IDtmGeometry
    {
        public DtmPointGeometry()
        {
            SrsDimension = 3;
            SrsName = "EPSG:5514";
        }
        public string Id { get; set; }
        public string SrsName { get; set; }
        public int SrsDimension { get; set; }
        public DtmPoint Point { get; set; }
        public void ExportToDtm(IDtmExporter exporter)
        {
            exporter.BeginElement(null, "GeometrieObjektu");
            exporter.BeginElement("gml", "pointProperty");
            exporter.BeginElement("gml", "Point");
            exporter.AddAttribute("id", Id);
            exporter.AddAttribute("srsName", SrsName);
            exporter.AddAttribute("srsDimension", SrsDimension);
            exporter.AddElement("gml", "pos", Point.ExportToDtm());
            exporter.EndElement();
            exporter.EndElement();
            exporter.EndElement();
        }
    }
}
