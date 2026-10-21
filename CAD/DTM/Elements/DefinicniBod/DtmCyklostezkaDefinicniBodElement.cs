using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmCyklostezkaDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmCyklostezkaDefinicniBodElement()
        {
            PrevazujiciPovrch = DtmPrevazujiciPovrchEnum.Nezjisteno;
        }
        DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }
        string OznaceniCyklostezky { get; set; }

        public override string GetInfoAsString()
        {
            return $"Prevazujici provrch: {PrevazujiciPovrch}, Oznaceni cyklostesky:{OznaceniCyklostezky}";
        }

    }
}
