using CAD.DTM.Gui;
using System.Globalization;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmVyskovyBodNaTerenuElement
        : DtmElement
    {
        public double VyskaNaTerenu { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "VyskaNaTerenu", VyskaNaTerenu);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "VyskaNaTerenu":
                        VyskaNaTerenu = double.Parse(x.InnerText, CultureInfo.InvariantCulture);
                        break;
                }

            }
        }
    }
}
