using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System;
using System.Linq;
using System.Xml;

namespace CAD.DTM.Elements
{
    class DtmStavebneUpravenyVjezdNaPozemekElement
        : DtmElement
    {
        public uint PrujezdnaSirka { get; set; }
        public uint PrujezdnaVyska { get; set; }
        public bool HraniceJinehoObjektu { get; set; }

        public DtmStavebneUpravenyVjezdNaPozemekElement()
        {
        }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "PrujezdnaSirka", (int)PrujezdnaSirka);
            exporter.AddElement("atr", "PrujezdnaVyska", (int)PrujezdnaVyska);
            exporter.AddElement("atr", "HraniceJinehoObjektu", HraniceJinehoObjektu ? "1" : "0");
        }

        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "PrujezdnaSirka":
                        PrujezdnaSirka = uint.Parse(x.InnerText);
                        break;
                    case "PrujezdnaVyska":
                        PrujezdnaVyska = uint.Parse(x.InnerText);
                        break;
                    case "HraniceJinehoObjektu":
                        HraniceJinehoObjektu = x.InnerText != "0";
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"PrujezdnaSirka: {PrujezdnaSirka}, PrujezdnaVyska:{PrujezdnaVyska}, HraniceJinehoObjektu:{HraniceJinehoObjektu}";
        }

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);

            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS
            {
                TridaPresnostiPoloha = 3,
                TridaPresnostiVyska = 3,
                UrovenUmisteniObjektuZPS = 0,
                ZpusobPorizeniZPS = 1
            };
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
