using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public enum TypNadrzeZdrzeSeVzdouvacimObjektemEnum
    {
        VodniNadrz = 1,
        PrumyslovaNadrz = 2,
        DestovaUsazovaciNadrz = 3,
        DestovaPrusakovaNadrz = 4,
        PozarniNadrz = 5,
        VyrovnavaciNadrz = 6,
        JezováZdrz = 7,
        Jina = 98,
        Nezjisteno = 99
    }
    public class DtmNadrzZdrzSeVzdouvacimObjektemDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public TypNadrzeZdrzeSeVzdouvacimObjektemEnum TypNadrzeZdrzeSeVzdouvacimObjektem { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuDefinicnichBodu(exporter);
            exporter.AddElement("atr", "TypNadrzeZdrzeSeVzdouvacimObjektem", (int)TypNadrzeZdrzeSeVzdouvacimObjektem);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypNadrzeZdrzeSeVzdouvacimObjektem":
                        TypNadrzeZdrzeSeVzdouvacimObjektem = (TypNadrzeZdrzeSeVzdouvacimObjektemEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Typ nádrže, zdrže se vzdouvacím objektem: {TypNadrzeZdrzeSeVzdouvacimObjektem}";
        }
    }
}
