using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmTrasaVodovodniPripojkyElement
        : DtmElement
    {
        public string Material { get; set; }
        public uint Dimenze { get; set; }

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "Dimenze", (int)Dimenze);
            exporter.AddElement("atr", "Material", Material);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "Material":
                        Material = x.InnerText;
                        break;
                    case "Dimenze":
                        Dimenze = uint.Parse(x.InnerText);
                        break;
                }
            }
        }

        public override string GetInfoAsString()
        {
            return $"Material: {Material}, Dimenze:{Dimenze}";
        }

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            Material = "PE";
            Dimenze = 25;

            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS
            {
                TridaPresnostiPoloha = 3,
                TridaPresnostiVyska = 3,
                UrovenUmisteniObjektuZPS = 0,
                ZpusobPorizeniZPS = 1
            };
        }
        static Dictionary<string, Tuple<string, uint>> _values = new Dictionary<string, Tuple<string, uint>>()
        {
            {"PE, DN25",new Tuple<string, uint>("PE",25)}
        };
        public override IEnumerable<string> Settings => _values.Select((n) => n.Key);

        public override void SelectedSetting(string value)
        {
            Material = _values[value].Item1;
            Dimenze = _values[value].Item2;
        }

    }
}
