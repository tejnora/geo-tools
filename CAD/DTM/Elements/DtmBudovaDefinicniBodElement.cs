using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmBudovaDefinicniBodElement
        : DtmBodBaseElement
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            ImportSpolecneAtributyObjektuDefinicnichBodu(xmlElement);
        }
    }
}
