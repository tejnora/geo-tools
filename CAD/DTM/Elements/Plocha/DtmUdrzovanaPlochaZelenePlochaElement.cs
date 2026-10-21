using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmUdrzovanaPlochaZelenePlochaElement
        : DtmPlochaBaseElement
    {
        public DtmUdrzovanaPlochaZelenePlochaElement()
        {
            TypUdrzovaneZelene = DtmTypUdrzovaneZeleneEnum.Nezjisteno;
        }
        DtmTypUdrzovaneZeleneEnum TypUdrzovaneZelene { get; set; }

        public override string GetInfoAsString()
        {
            return $"Typ udrzovaci zelene: {TypUdrzovaneZelene}";
        }
    }
}
