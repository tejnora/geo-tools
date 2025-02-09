using CAD.DTM.Configuration;
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

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS
            {
                TridaPresnostiPoloha = 3,
                TridaPresnostiVyska = 3,
                UrovenUmisteniObjektuZPS = 0,
                ZpusobPorizeniZPS = 1
            };
        }
    }
}
