using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Elements.Linie;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmStavebneUpravenyVjezdNaPozemekElement
        : DtmLinieElementBase
    {
        public uint PrujezdnaSirka { get; set; }
        public uint PrujezdnaVyska { get; set; }
        public bool HraniceJinehoObjektu { get; set; }

        public DtmStavebneUpravenyVjezdNaPozemekElement()
        {
        }

        public override string GetInfoAsString()
        {
            return $"PrujezdnaSirka: {PrujezdnaSirka}, PrujezdnaVyska:{PrujezdnaVyska}, HraniceJinehoObjektu:{HraniceJinehoObjektu}";
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS();
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmBoolCustomProperty("Hranice jiného objektu?", HraniceJinehoObjektu, cv => HraniceJinehoObjektu = cv));
            properties.AddProperty(new DtmUIntCustomProperty("Průjezdná šířka:", PrujezdnaSirka, cv => PrujezdnaSirka = cv));
            properties.AddProperty(new DtmUIntCustomProperty("Průjezdná výška:", PrujezdnaVyska, cv => PrujezdnaVyska = cv));
        }

        static Dictionary<string, bool> _values = new Dictionary<string, bool>()
        {
            {"HraniceJinehoObjektu (Ano)",true },
            {"HraniceJinehoObjektu (Ne)",false },
        };
        public override IEnumerable<string> Settings => _values.Select((n) => n.Key);
        public override void SelectedSetting(string value)
        {
            HraniceJinehoObjektu = _values[value];
        }
    }
}
