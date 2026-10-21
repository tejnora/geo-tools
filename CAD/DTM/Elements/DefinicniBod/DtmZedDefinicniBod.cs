using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    public class DtmZedDefinicniBodElement
        : DtmDefinicniBodBaseElement
    {
        public DtmTypZdiEnum TypZdi { get; set; }

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            TypZdi = DtmTypZdiEnum.Nezjisteno;
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
        public override string GetInfoAsString()
        {
            return $"Type stavby: {TypZdi}";
        }
    }
}
