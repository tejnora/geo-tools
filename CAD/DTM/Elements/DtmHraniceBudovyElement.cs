using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmHraniceBudovyElement
        : DtmElement
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
        }

        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
        }
    }
}
