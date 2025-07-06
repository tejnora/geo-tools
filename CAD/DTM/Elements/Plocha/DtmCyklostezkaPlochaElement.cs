using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmCyklostezkaPlochaElement
        : DtmPlochaBaseElement
    {
        public DtmCyklostezkaPlochaElement()
        {
            PrevazujiciPovrch = DtmPrevazujiciPovrchEnum.Nezjisteno;
        }
        DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }
        string OznaceniCyklostezky { get; set; }

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "PrevazujiciPovrch", (int)PrevazujiciPovrch);
            exporter.AddElement("atr", "OznaceniCyklostezky", OznaceniCyklostezky);
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
                    case "OznaceniCyklostezky":
                        OznaceniCyklostezky = x.InnerText;
                        break;
                }
            }
        }

        public override string GetInfoAsString()
        {
            return $"Prevazujici provrch: {PrevazujiciPovrch}, Oznaceni cyklostesky:{OznaceniCyklostezky}";
        }
    }
}
