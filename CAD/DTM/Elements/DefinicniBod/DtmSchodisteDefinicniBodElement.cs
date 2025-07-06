using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    public class DtmSchodisteDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmDruhSchodisteEnum DruhSchodiste { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
            exporter.AddElement("atr", "DruhSchodiste", (int)DruhSchodiste);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "DruhSchodiste":
                        DruhSchodiste = (DtmDruhSchodisteEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Druh schodiste: {DruhSchodiste}";
        }
    }
}
