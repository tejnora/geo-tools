using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmHraniceStavbyElement
        : DtmElement
    {
        public double TypStavby { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypStavby", TypStavby);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypStavby":
                        TypStavby = double.Parse(x.InnerText);
                        break;
                }
            }
        }
    }
}
