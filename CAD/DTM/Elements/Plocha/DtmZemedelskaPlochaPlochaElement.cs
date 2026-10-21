using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmZemedelskaPlochaPlochaElement
        : DtmPlochaBaseElement
    {
        public DtmZemedelskaPlochaPlochaElement()
        {
        }
        public DtmTypZemedelskePlochyEnum TypZemedelskePlochy { get; set; }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            TypZemedelskePlochy = DtmTypZemedelskePlochyEnum.Nezjisteno;
        }

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmTypZemedelskePlochyEnum>("Typ zemědělské plochy:", TypZemedelskePlochy, cv => TypZemedelskePlochy = cv));
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypZemedelskePlochyEnum));
        public override void SelectedSetting(string value)
        {
            TypZemedelskePlochy = (DtmTypZemedelskePlochyEnum)Enum.Parse(typeof(DtmTypZemedelskePlochyEnum), value);
        }
        public override string GetInfoAsString()
        {
            return $"Typ zemědělské plochy: {TypZemedelskePlochy}";
        }
    }
}
