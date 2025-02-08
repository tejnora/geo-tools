using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmOstatniZastresenaStavbaDefinicniBodElement
    : DtmElement
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
