using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    public class DtmZedDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmTypZdiEnum TypZdi { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
            exporter.AddElement("atr", "TypZdi", (int)TypZdi);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypZdi":
                        TypZdi = (DtmTypZdiEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            TypZdi = DtmTypZdiEnum.Nezjisteno;
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
        public override string GetInfoAsString()
        {
            return $"Type stavby: {TypZdi}";
        }
    }
}
