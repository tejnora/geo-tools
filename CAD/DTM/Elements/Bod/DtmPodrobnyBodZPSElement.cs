using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;
using System.Text.RegularExpressions;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmPodrobnyBodZPSElement
    : DtmBodBaseElement
    {

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmReadonlyCustomProperty("Cislo bodu:", CisloBodu));
        }

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }

        public override string GetInfoAsString()
        {
            return $"Cislo bodu: {SpolecneAtributy.ID}";
        }
    }
}
