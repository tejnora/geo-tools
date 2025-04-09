using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmBudovaDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            ImportSpolecneAtributyObjektuDefinicnichBodu(xmlElement);
        }
    }
}
