using CAD.DTM.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmHraniceDopravniStavbyPlochyElement
        : DtmElement
    {
        public int TypDopravniStavbyNeboPlochy { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypDopravniStavbyNeboPlochy", TypDopravniStavbyNeboPlochy);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement xe in xmlElement)
            {
                switch (xe.LocalName)
                {
                    case "TypDopravniStavbyNeboPlochy":
                        TypDopravniStavbyNeboPlochy = int.Parse(xe.InnerText);
                        break;
                }
            }
        }

    }
}
