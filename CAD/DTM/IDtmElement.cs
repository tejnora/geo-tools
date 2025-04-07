using CAD.Canvas;
using CAD.DTM.Gui;
using System.Collections.Generic;

namespace CAD.DTM
{
    public enum DtmElementType
    {
        Point,
        Line,
        Plocha
   }

    public interface IDtmElement
    {
        IDrawObject CreateDrawObject();
        //string ID { get; set; }
        DtmElementType ElementType { get; }
        bool IsDeleted { get; set; }
        bool ExportToOutput { get; }
        IDtmGeometry Geometry { get; set; }
        IEnumerable<string> Settings { get; }
        void SelectedSetting(string value);
        void ExportAttributesToDtm(IDtmExporter exporter);
        string EvaluateZapisObjektuForExportToDtm();
        void ExportSpolecneAtributyVsechObjektu(IDtmExporter exporter);
    }
}
