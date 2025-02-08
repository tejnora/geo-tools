using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmUdrzovanaPlochaZeleneDefinicniBodElement
        : DtmElement
    {
        public int TypUdrzovaneZelene { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
            exporter.AddElement("atr", "TypUdrzovaneZelene", TypUdrzovaneZelene);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            ImportSpolecneAtributyObjektuDefinicnichBodu(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypUdrzovaneZelene":
                        TypUdrzovaneZelene = int.Parse(x.InnerText);
                        break;
                }
            }
        }
    }
}
