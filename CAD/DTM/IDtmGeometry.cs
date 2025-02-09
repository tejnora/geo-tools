using CAD.DTM.Gui;

namespace CAD.DTM
{
    public interface IDtmGeometry
    {
        string Id { get; set; }
        void ExportToDtm(IDtmExporter exporter);
    }
}
