using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmHranicePrirodnihoPoloprirodnihoObjektuElement
        : DtmElement
    {
        public int TypPrirodnihoPoloprirodnihoObjektu { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypPrirodnihoPoloprirodnihoObjektu", TypPrirodnihoPoloprirodnihoObjektu);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypPrirodnihoPoloprirodnihoObjektu":
                        TypPrirodnihoPoloprirodnihoObjektu = int.Parse(x.InnerText);
                        break;
                }
            }
        }
    }
}
