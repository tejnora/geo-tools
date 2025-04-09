using CAD.DTM.Configuration;
using CAD.DTM.Elements.Linie;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmHraniceDopravniStavbyPlochyElement
        : DtmLinieElementBase
    {
        public DtmTypDopravniStavbyNeboPlochyEnum TypDopravniStavbyNeboPlochy { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypDopravniStavbyNeboPlochy", (int)TypDopravniStavbyNeboPlochy);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement xe in xmlElement)
            {
                switch (xe.LocalName)
                {
                    case "TypDopravniStavbyNeboPlochy":
                        TypDopravniStavbyNeboPlochy = (DtmTypDopravniStavbyNeboPlochyEnum)int.Parse(xe.InnerText);
                        break;
                }
            }
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
