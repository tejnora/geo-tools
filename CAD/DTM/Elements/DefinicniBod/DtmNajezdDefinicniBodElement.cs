using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmNajezdDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }

        public override string GetInfoAsString()
        {
            return $"Prevazujici povrch: {PrevazujiciPovrch}";
        }
    }
}
