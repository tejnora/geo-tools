using CAD.DTM.Gui;
using System.Globalization;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmChodnikDefinicniBodElement
        : DtmElement
    {
        public double PrevazujiciPovrch { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
            exporter.AddElement("atr", "PrevazujiciPovrch", PrevazujiciPovrch);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            ImportSpolecneAtributyObjektuDefinicnichBodu(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "PrevazujiciPovrch":
                        PrevazujiciPovrch = double.Parse(x.InnerText, CultureInfo.InvariantCulture);
                        break;
                }
            }
        }
    }
}
