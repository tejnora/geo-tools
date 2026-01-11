using CAD.DTM.Configuration;
using CAD.DTM.Elements.Linie;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Xml;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    class DtmHranicePrirodnihoPoloprirodnihoObjektuElement
        : DtmLinieElementBase
    {
        public DtmTypPrirodnihoPoloprirodnihoObjektuEnum TypPrirodnihoPoloprirodnihoObjektu { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypPrirodnihoPoloprirodnihoObjektu", (int)TypPrirodnihoPoloprirodnihoObjektu);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypPrirodnihoPoloprirodnihoObjektu":
                        TypPrirodnihoPoloprirodnihoObjektu = (DtmTypPrirodnihoPoloprirodnihoObjektuEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmTypPrirodnihoPoloprirodnihoObjektuEnum>
                ("Typ přírodního a polopřírodního objektu:", TypPrirodnihoPoloprirodnihoObjektu, cv => TypPrirodnihoPoloprirodnihoObjektu = cv));
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
            TypPrirodnihoPoloprirodnihoObjektu = DtmTypPrirodnihoPoloprirodnihoObjektuEnum.Nezjisteno;
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypPrirodnihoPoloprirodnihoObjektuEnum));
        public override void SelectedSetting(string value)
        {
            TypPrirodnihoPoloprirodnihoObjektu = (DtmTypPrirodnihoPoloprirodnihoObjektuEnum)Enum.Parse(typeof(DtmTypPrirodnihoPoloprirodnihoObjektuEnum), value);
        }
        public override string GetInfoAsString()
        {
            return $"Typ prirodniho ploprirodniho objektu:{TypPrirodnihoPoloprirodnihoObjektu}";
        }

    }
}
