using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Globalization;
using System.Xml;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    class DtmVyskovyBodNaTerenuElement
        : DtmBodBaseElement
    {
        public string VyskaNaTerenu { get; set; }
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
                        VyskaNaTerenu = x.InnerText;
                        break;
                }

            }
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmStringCustomProperty("Výška na terénu:", VyskaNaTerenu, cv => VyskaNaTerenu = cv));
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
