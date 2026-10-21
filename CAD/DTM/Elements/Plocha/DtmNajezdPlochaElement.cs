using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmNajezdPlochaElement
        : DtmPlochaBaseElement
    {
        public DtmNajezdPlochaElement()
        {
            PrevazujiciPovrch = DtmPrevazujiciPovrchEnum.Nezjisteno;
        }
        DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }

        public override string GetInfoAsString()
        {
            return $"Prevazujici provrch: {PrevazujiciPovrch}";
        }
    }
}
