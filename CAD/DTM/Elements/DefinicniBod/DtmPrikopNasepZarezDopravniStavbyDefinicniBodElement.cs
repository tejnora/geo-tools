using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmPrikopNasepZarezDopravniStavbyDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
        }
    }

}
