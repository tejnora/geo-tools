using CAD.DTM.Configuration;
using System.Collections.Generic;
using System;
using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmHraniceZdiElement
        : DtmLinieElementBase
    {

        public DtmTypZdiEnum TypZdi { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
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
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypStavbyEnum));
        public override void SelectedSetting(string value)
        {
            TypZdi = (DtmTypZdiEnum)Enum.Parse(typeof(DtmTypZdiEnum), value);
        }
        public override string GetInfoAsString()
        {
            return $"Type stavby: {TypZdi}";
        }
    }
}
