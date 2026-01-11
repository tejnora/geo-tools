using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmZedLinieElement
        : DtmLinieElementBase
    {
        public DtmZedLinieElement()
        {

        }
        public DtmTypZdiEnum TypZdi { get; set; }
        public bool HraniceJinehoObjektu { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypZdi", (int)TypZdi);
            exporter.AddElement("atr", "HraniceJinehoObjektu", HraniceJinehoObjektu ? "1" : "0");
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
                    case "HraniceJinehoObjektu":
                        HraniceJinehoObjektu = x.InnerText != "0";
                        break;
                }
            }
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmTypZdiEnum>("Typ zdi:", TypZdi, cv => TypZdi = cv));
            properties.AddProperty(new DtmBoolCustomProperty("Hranice jiného objektu?", HraniceJinehoObjektu, cv => HraniceJinehoObjektu = cv));
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            TypZdi = DtmTypZdiEnum.Nezjisteno;
            HraniceJinehoObjektu = false;
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypZdiEnum));
        public override void SelectedSetting(string value)
        {
            TypZdi = (DtmTypZdiEnum)Enum.Parse(typeof(DtmTypZdiEnum), value);
        }
        public override string GetInfoAsString()
        {
            return $"Type stavby: {TypZdi}, HraniceJinehoObjektu:{HraniceJinehoObjektu}";
        }

    }
}
