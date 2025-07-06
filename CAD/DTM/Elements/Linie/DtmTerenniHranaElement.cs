using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    enum TypTerenniHranyEnum
    {

    }
    class DtmTerenniHranaElement
        : DtmLinieElementBase
    {
        public TypTerenniHranyEnum TypTerenniHrany { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypTerenniHrany", (int)TypTerenniHrany);

        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypTerenniHrany":
                        TypTerenniHrany = (TypTerenniHranyEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Typ terenni hrany: {TypTerenniHrany}";
        }
    }
}
