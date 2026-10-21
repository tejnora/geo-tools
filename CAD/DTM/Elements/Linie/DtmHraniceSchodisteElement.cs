using System.Collections.Generic;
using System;
using System.Xml;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmHraniceSchodisteElement
        : DtmLinieElementBase
    {
        public DtmHraniceSchodisteElement()
        {
            DruhSchodiste = DtmDruhSchodisteEnum.Nezjisteno;
        }
        public DtmDruhSchodisteEnum DruhSchodiste { get; set; }

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmDruhSchodisteEnum>("Druh schodiště:", DruhSchodiste, cv => DruhSchodiste = cv));
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmDruhSchodisteEnum));
        public override void SelectedSetting(string value)
        {
            DruhSchodiste = (DtmDruhSchodisteEnum)Enum.Parse(typeof(DtmDruhSchodisteEnum), value);
        }

        public override string GetInfoAsString()
        {
            return $"Druh schodiste: {DruhSchodiste}";
        }
    }

}
