using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmPodrobnyBodZPSElement
    : DtmElement
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            exporter.AddElement("atr", "UrovenUmisteniObjektuZPS", SpolecneAtributyZPS.UrovenUmisteniObjektuZPS);
            exporter.AddElement("atr", "TridaPresnostiPoloha", SpolecneAtributyZPS.TridaPresnostiPoloha);
            exporter.AddElement("atr", "TridaPresnostiVyska", SpolecneAtributyZPS.TridaPresnostiVyska);
            exporter.AddElement("atr", "ZpusobPorizeniPB_ZPS", SpolecneAtributyZPS.ZpusobPorizeniZPS);
            exporter.AddElement("atr", "CisloBodu", CisloBodu);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            if (SpolecneAtributyZPS == null)
                SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "ZpusobPorizeniPB_ZP":
                        SpolecneAtributyZPS.UrovenUmisteniObjektuZPS = int.Parse(x.InnerText);
                        break;
                    case "CisloBodu":
                        CisloBodu = x.InnerText;
                        break;
                    case "TridaPresnostiPoloha":
                        SpolecneAtributyZPS.TridaPresnostiPoloha = int.Parse(x.InnerText);
                        break;
                    case "TridaPresnostiVyska":
                        SpolecneAtributyZPS.TridaPresnostiVyska = int.Parse(x.InnerText);
                        break;
                    case "UrovenUmisteniObjektuZPS":
                        SpolecneAtributyZPS.UrovenUmisteniObjektuZPS = int.Parse(x.InnerText);
                        break;
                }
            }
        }

    }
}
