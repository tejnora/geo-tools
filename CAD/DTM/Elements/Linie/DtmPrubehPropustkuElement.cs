using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmPrubehPropustkuElement
        : DtmLinieElementBase
    {
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
