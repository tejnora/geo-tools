using CAD.DTM.Configuration;
using CAD.DTM.Elements.Linie;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Xml;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    public class DtmPlotElement
        : DtmLinieElementBase
    {
        public DtmDruhPlotuEnum DruhPlotu { get; set; }
        public bool HraniceJinehoObjektu { get; set; }

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmDruhPlotuEnum>("Druh plotu:", DruhPlotu, cv => DruhPlotu = cv));
            properties.AddProperty(new DtmBoolCustomProperty("Hranice jiného objektu?", HraniceJinehoObjektu, cv => HraniceJinehoObjektu = cv));
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
