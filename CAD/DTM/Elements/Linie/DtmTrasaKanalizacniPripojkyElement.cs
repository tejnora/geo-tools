using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmTrasaKanalizacniPripojkyElement
        : DtmLinieElementBase
    {
        public string Material { get; set; }
        public uint Dimenze { get; set; }
        public DtmDruhStokoveSiteEnum DtmDruhStokoveSite { get; set; }

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS_TI(exporter);
            exporter.AddElement("atr", "DruhStokoveSite", (int)DtmDruhStokoveSite);
            exporter.AddElement("atr", "Dimenze", (int)Dimenze);
            exporter.AddElement("atr", "Material", Material);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
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
                    case "DruhStokoveSite":
                        DtmDruhStokoveSite = (DtmDruhStokoveSiteEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Material: {Material}, Dimenze:{Dimenze}, Druh stokove site:{DtmDruhStokoveSite}";
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            Material = "PVC";
            Dimenze = 150;
            DtmDruhStokoveSite = DtmDruhStokoveSiteEnum.Gravitacni;
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
        }
        static Dictionary<string, Tuple<string, uint>> _values = new Dictionary<string, Tuple<string, uint>>()
        {
            {"PVC, 150",new Tuple<string, uint>("PVC",150)}
        };
        public override IEnumerable<string> Settings => _values.Select((n) => n.Key);

        public override void SelectedSetting(string value)
        {
            Material = _values[value].Item1;
            Dimenze = _values[value].Item2;
        }
    }
}
