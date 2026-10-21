using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    class DtmTrasaVodovodniPripojkyElement
        : DtmLinieElementBase
    {
        public string Material { get; set; }
        public uint Dimenze { get; set; }

        public override string GetInfoAsString()
        {
            return $"Material: {Material}, Dimenze:{Dimenze}";
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            Material = "PE";
            Dimenze = 25;
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
            SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI = -1;
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmStringCustomProperty("Materiál:", Material, cv => Material = cv));
            properties.AddProperty(new DtmUIntCustomProperty("Dimenze:", Dimenze, cv => Dimenze = cv));
        }
        static Dictionary<string, Tuple<string, uint>> _values = new Dictionary<string, Tuple<string, uint>>()
        {
            {"PE, DN25",new Tuple<string, uint>("PE",25)},
            {"PE, DN32",new Tuple<string, uint>("PE",32)}
        };
        public override IEnumerable<string> Settings => _values.Select((n) => n.Key);
        public override void SelectedSetting(string value)
        {
            Material = _values[value].Item1;
            Dimenze = _values[value].Item2;
        }
    }
}
