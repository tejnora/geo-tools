using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmChodnikPlochaElement
        : DtmElement
    {
        public DtmChodnikPlochaElement()
        {
        }

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
        }
    }
}
