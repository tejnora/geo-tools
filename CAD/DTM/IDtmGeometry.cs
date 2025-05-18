using CAD.DTM.Gui;

namespace CAD.DTM
{
    public interface IDtmGeometry
    {
        string Id { get; set; }
        string SrsName { get; set; }
        int SrsDimension { get; set; }
        void ExportToDtm(IDtmExporter exporter);
    }
}
