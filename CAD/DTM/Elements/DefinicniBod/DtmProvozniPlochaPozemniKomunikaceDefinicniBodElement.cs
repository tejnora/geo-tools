using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmProvozniPlochaPozemniKomunikaceDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmTypPozemniKomunikaceEnum TypPozemniKomunikace { get; set; }
        public DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }

        public override string GetInfoAsString()
        {
            return $"Typ pozemni komunikace: {TypPozemniKomunikace}, Prevazujici povrch:{PrevazujiciPovrch}";
        }
    }

}
