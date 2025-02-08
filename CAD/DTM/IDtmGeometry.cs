using CAD.DTM.Gui;

namespace CAD.DTM
{
    public interface IDtmGeometry
    {
        void ExportToDtm(IDtmExporter exporter);
    }
}
