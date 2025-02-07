using CAD.Canvas;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    public interface IDtmElement
    {
        IDrawObject CreateDrawObject();
        bool IsDeleted { get; set; }
        bool ExportToOutput { get; }
        void ExportToDtm(IDtmExporter exporter);
    }
}
