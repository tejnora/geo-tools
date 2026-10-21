using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmChodnikPlochaElement
        : DtmPlochaBaseElement
    {
        public DtmChodnikPlochaElement()
        {
        }
        public DtmPrevazujiciPovrchEnum PrevazujiciPovrch { get; set; }

    }
}
