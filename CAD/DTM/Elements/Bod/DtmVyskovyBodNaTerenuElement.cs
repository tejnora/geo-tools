using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Globalization;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmVyskovyBodNaTerenuElement
        : DtmBodBaseElement
    {
        public double VyskaNaTerenu { get; set; }
        public override DtmElementType ElementType => DtmElementType.Linie;

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "VyskaNaTerenu", VyskaNaTerenu);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
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
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }

        public override string GetInfoAsString()
        {
            return $"Vyska na terenu: {VyskaNaTerenu}";
        }
    }
}
