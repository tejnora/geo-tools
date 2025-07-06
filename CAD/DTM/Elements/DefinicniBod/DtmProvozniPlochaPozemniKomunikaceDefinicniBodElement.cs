using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmProvozniPlochaPozemniKomunikaceDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmTypPozemniKomunikaceEnum TypPozemniKomunikace { get; set; }
        public DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
            exporter.AddElement("atr", "TypPozemniKomunikace", (int)TypPozemniKomunikace);
            exporter.AddElement("atr", "PrevazujiciPovrch", (int)PrevazujiciPovrch);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypPozemniKomunikace":
                        TypPozemniKomunikace = (DtmTypPozemniKomunikaceEnum)int.Parse(x.InnerText);
                        break;
                    case "PrevazujiciPovrch":
                        PrevazujiciPovrch = (DtmPrevazujiciPovrchEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Typ pozemni komunikace: {TypPozemniKomunikace}, Prevazujici povrch:{PrevazujiciPovrch}";
        }
    }

}
