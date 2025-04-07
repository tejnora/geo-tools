using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmParkovisteOdstavnaPlochaDefinicniBod
        : DtmBodBaseElement
    {
        public DtmParkovisteOdstavnaPlochaDefinicniBod()
        {
            PrevazujiciPovrch = DtmPrevazujiciPovrch.Nezjisteno;
        }
        DtmPrevazujiciPovrch PrevazujiciPovrch { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "PrevazujiciPovrch", (int)PrevazujiciPovrch);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "PrevazujiciPovrch":
                        PrevazujiciPovrch = (DtmPrevazujiciPovrch)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Prevazujici provrch: {PrevazujiciPovrch}";
        }

    }
}
