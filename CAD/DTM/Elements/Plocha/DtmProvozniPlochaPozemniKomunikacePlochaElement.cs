using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmProvozniPlochaPozemniKomunikacePlochaElement
        : DtmPlochaBaseElement
    {
        public DtmTypPozemniKomunikaceEnum TypPozemniKomunikace { get; set; }
        public DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }

        public override string GetInfoAsString()
        {
            return $"Typ pozemni komunikace:{TypPozemniKomunikace}, Prevazujici povrch:{PrevazujiciPovrch}";
        }
    }
}
