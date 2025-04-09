using CAD.DTM.Configuration;
using CAD.DTM.Elements.Linie;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmPlotElement
        : DtmLinieElementBase
    {
        public DtmDruhPlotuEnum DruhPlotu { get; set; }
        public bool HraniceJinehoObjektu { get; set; }

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "DruhPlotu", (int)DruhPlotu);
            exporter.AddElement("atr", "HraniceJinehoObjektu", HraniceJinehoObjektu);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "DruhPlotu":
                        DruhPlotu = (DtmDruhPlotuEnum)int.Parse(x.InnerText);
                        break;
                    case "HraniceJinehoObjektu":
                        {
                            if (bool.TryParse(x.InnerText, out var value))
                            {
                                HraniceJinehoObjektu = value;
                                continue;
                            }
                            HraniceJinehoObjektu = int.Parse(x.InnerText) == 1;
                        }
                        break;
                }
            }
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            DruhPlotu = DtmDruhPlotuEnum.Nezjisteno;
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmDruhPlotuEnum));
        public override void SelectedSetting(string value)
        {
            DruhPlotu = (DtmDruhPlotuEnum)Enum.Parse(typeof(DtmDruhPlotuEnum), value);
        }
        public override string GetInfoAsString()
        {
            return $"Druh plotu: {DruhPlotu}";
        }

    }
}
