using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public enum TypUdrzovaneZeleneEnum
    {
        MestkaParkovaZelen = 1,
        UdrzovanaTravnataAOkrasnaPlocha = 2,
        SkupinaStromuAKeru = 3,
        SilnicniVegetace = 4,
        Nezjisteno = 99
    }
    public class DtmUdrzovanaPlochaZeleneDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public TypUdrzovaneZeleneEnum TypUdrzovaneZelene { get; set; }

    }
}
