using CAD.DTM.Configuration;
using CAD.DTM.Elements.Linie;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Xml;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    class DtmHraniceDopravniStavbyPlochyElement
        : DtmLinieElementBase
    {
        public DtmTypDopravniStavbyNeboPlochyEnum TypDopravniStavbyNeboPlochy { get; set; }

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmTypDopravniStavbyNeboPlochyEnum>
                ("Typ dopravní stavby nebo plochy:", TypDopravniStavbyNeboPlochy, cv => TypDopravniStavbyNeboPlochy = cv));
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
            TypDopravniStavbyNeboPlochy = DtmTypDopravniStavbyNeboPlochyEnum.Nezjisteno;
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypDopravniStavbyNeboPlochyEnum));
        public override void SelectedSetting(string value)
        {
            TypDopravniStavbyNeboPlochy = (DtmTypDopravniStavbyNeboPlochyEnum)Enum.Parse(typeof(DtmTypDopravniStavbyNeboPlochyEnum), value);
        }

        public override string GetInfoAsString()
        {
            return $"Typ dopravni stavby nebo plochy:{TypDopravniStavbyNeboPlochy}";
        }
    }
}
