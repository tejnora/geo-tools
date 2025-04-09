using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmHraniceBudovyElement
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
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
    }
}
