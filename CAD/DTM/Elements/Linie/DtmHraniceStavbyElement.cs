using System;
using System.Collections.Generic;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmHraniceStavbyElement
        : DtmLinieElementBase
    {
        public DtmHraniceStavbyElement()
        {
            TypStavby = DtmTypStavbyEnum.OstatniZastresenaStavba;
        }
        public DtmTypStavbyEnum TypStavby { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypStavby", (int)TypStavby);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypStavby":
                        TypStavby = (DtmTypStavbyEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            TypStavby = DtmTypStavbyEnum.Nezjisteno;
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypStavbyEnum));
        public override void SelectedSetting(string value)
        {
            TypStavby = (DtmTypStavbyEnum)Enum.Parse(typeof(DtmTypStavbyEnum), value);
        }
        public override string GetInfoAsString()
        {
            return $"Type stavby: {TypStavby}";
        }
    }
}
