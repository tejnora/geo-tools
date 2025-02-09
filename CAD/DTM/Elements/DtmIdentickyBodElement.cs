using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmIdentickyBodElement
        : DtmElement
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "CisloBodu", CisloBodu);
        }

        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "CisloBodu":
                        CisloBodu = x.InnerText;
                        break;
                }
            }
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
