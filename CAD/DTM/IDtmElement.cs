using CAD.Canvas;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    public interface IDtmElement
    {
        IDrawObject CreateDrawObject();
        string ID { get; set; }
        bool IsDeleted { get; set; }
        bool ExportToOutput { get; }
        IDtmGeometry Geometry { get; set; }
        void ExportAttributesToDtm(IDtmExporter exporter);
        char EvaluateZapisObjektuForExportToDtm();
        void ExportSpolecneAtributyVsechObjektu(IDtmExporter exporter);
    }
}
