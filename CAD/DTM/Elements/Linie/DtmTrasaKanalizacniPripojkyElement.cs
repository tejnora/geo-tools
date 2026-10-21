using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmTrasaKanalizacniPripojkyElement
        : DtmLinieElementBase
    {
        public string Material { get; set; }
        public uint Dimenze { get; set; }
        public DtmDruhStokoveSiteEnum DruhStokoveSite { get; set; }
        public DtmUceloveZarazeniStokoveSiteEnum UceloveZarazeniStokoveSite { get; set; }

        public override string GetInfoAsString()
        {
            return $"Material: {Material}, Dimenze:{Dimenze}, Druh stokove site:{DruhStokoveSite}, Ucelove zarazeni stokove site:{UceloveZarazeniStokoveSite}";
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            Material = "PVC";
            Dimenze = 150;
            DruhStokoveSite = DtmDruhStokoveSiteEnum.Gravitacni;
            UceloveZarazeniStokoveSite = DtmUceloveZarazeniStokoveSiteEnum.Splazkova;
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
            SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI = -1;
        }

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmStringCustomProperty("Materiál:", Material, cv => Material = cv));
            properties.AddProperty(new DtmUIntCustomProperty("Dimenze:", Dimenze, cv => Dimenze = cv));
            properties.AddProperty(new DtmEnumCustomProperty<DtmDruhStokoveSiteEnum>("Druh stokové sitě:", DruhStokoveSite, (cv) => DruhStokoveSite = cv));
            properties.AddProperty(new DtmEnumCustomProperty<DtmUceloveZarazeniStokoveSiteEnum>("Učlove zařízení stokove sitě:", UceloveZarazeniStokoveSite, (cv) => UceloveZarazeniStokoveSite = cv));
        }

        static Dictionary<string, Tuple<string, uint>> _values = new Dictionary<string, Tuple<string, uint>>()
        {
            {"PVC, 150, Gravitacni, Splazkova",new Tuple<string, uint>("PVC",150)}
        };
        public override IEnumerable<string> Settings => _values.Select((n) => n.Key);

        public override void SelectedSetting(string value)
        {
            Material = _values[value].Item1;
            Dimenze = _values[value].Item2;
        }
    }
}
