using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public enum TypUdrzovaneZeleneEnum
    {
        MestkaParkovaZelen = 1,
        UdrzovanaTravnataAOkrasnaPlocha = 2,
        SkupinaStromuAKeru = 3,
        SilnicniVegetace = 4,
        Nezjisteno = 99
    }
    public class DtmUdrzovanaPlochaZeleneDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public TypUdrzovaneZeleneEnum TypUdrzovaneZelene { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
            exporter.AddElement("atr", "TypUdrzovaneZelene", (int)TypUdrzovaneZelene);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypUdrzovaneZelene":
                        TypUdrzovaneZelene = (TypUdrzovaneZeleneEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
    }
}
