using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmPrikopNasepZarezDopravniStavbyPlochaElement
        : DtmPlochaBaseElement
    {
        public DtmDruhSchodisteEnum DruhSchodiste { get; set; }

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
        }

        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
        }
    }
}
