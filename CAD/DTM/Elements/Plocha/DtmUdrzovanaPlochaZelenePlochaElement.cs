using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmUdrzovanaPlochaZelenePlochaElement
        : DtmPlochaBaseElement
    {
        public DtmUdrzovanaPlochaZelenePlochaElement()
        {
            TypUdrzovaneZelene = DtmTypUdrzovaneZeleneEnum.Nezjisteno;
        }
        DtmTypUdrzovaneZeleneEnum TypUdrzovaneZelene { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
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
                        TypUdrzovaneZelene = (DtmTypUdrzovaneZeleneEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Typ udrzovaci zelene: {TypUdrzovaneZelene}";
        }
    }
}
