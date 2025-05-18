using CAD.Canvas;
using CAD.DTM.Elements;
using CAD.DTM.Gui;
using System.Collections.Generic;

namespace CAD.DTM
{
    public enum DtmElementType
    {
        Bod,
        Linie,
        Plocha,
        DefinicniBod,
        Obvod
    }

    public interface IDtmElement
    {
        IDrawObject CreateDrawObject();
        //string ID { get; set; }
        DtmElementType ElementType { get; }
        bool IsDeleted { get; set; }
        bool ExportToOutput { get; }
        IDtmGeometryGroup Geometry { get; set; }
        IEnumerable<string> Settings { get; }
        void SelectedSetting(string value);
        void ExportAttributesToDtm(IDtmExporter exporter);
        string EvaluateZapisObjektuForExportToDtm();
        char EvaluateZapisObjektuForGui();
        void ExportSpolecneAtributyVsechObjektu(IDtmExporter exporter);
        DtmElementSpolecneAtributy SpolecneAtributy { get; }
    }
}
