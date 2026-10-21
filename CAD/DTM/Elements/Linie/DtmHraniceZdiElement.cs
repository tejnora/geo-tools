using CAD.DTM.Configuration;
using System.Collections.Generic;
using System;
using CAD.DTM.Gui;
using System.Xml;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    class DtmHraniceZdiElement
        : DtmLinieElementBase
    {

        public DtmTypZdiEnum TypZdi { get; set; }

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            TypZdi = DtmTypZdiEnum.Nezjisteno;
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmTypZdiEnum>("Typ zdi:", TypZdi, cv => TypZdi = cv));
        }

        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypZdiEnum));
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
