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

        public override string GetInfoAsString()
        {
            return $"Typ nádrže, zdrže se vzdouvacím objektem: {TypNadrzeZdrzeSeVzdouvacimObjektem}";
        }
    }
}
