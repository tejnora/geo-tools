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

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS_TI(exporter);
            exporter.AddElement("atr", "UceloveZarazeniStokoveSite", (int)UceloveZarazeniStokoveSite);
            exporter.AddElement("atr", "DruhStokoveSite", (int)DruhStokoveSite);
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
                        DruhStokoveSite = (DtmDruhStokoveSiteEnum)int.Parse(x.InnerText);
                        break;
                    case "UceloveZarazeniStokoveSite":
                        UceloveZarazeniStokoveSite = (DtmUceloveZarazeniStokoveSiteEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
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
            properties.AddProperty(new DtmReadonlyCustomProperty("Materiál:", Material));
            properties.AddProperty(new DtmReadonlyCustomProperty("Dimenze:", Dimenze.ToString()));
            properties.AddProperty(new DtmReadonlyCustomProperty("Druh stokové sitě:", DruhStokoveSite.ToString()));
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
