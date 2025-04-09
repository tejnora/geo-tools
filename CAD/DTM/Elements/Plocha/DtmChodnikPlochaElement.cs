using CAD.DTM.Elements.Plocha;
using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmChodnikPlochaElement
        : DtmPlochaBaseElement
    {
        public DtmChodnikPlochaElement()
        {
        }
        public DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "PrevazujiciPovrch", (int)PrevazujiciPovrch);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "PrevazujiciPovrch":
                        PrevazujiciPovrch = (DtmPrevazujiciPovrchEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }

    }
}
