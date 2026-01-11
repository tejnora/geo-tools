using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmTrasaOdbernehoPlynovehoZarizeniElement
        : DtmLinieElementBase
    {
        public DtmTlakovaHladinaPlynovodniSiteEnum TlakovaHladinaPlynovodniSite { get; set; }
        public uint Dimenze { get; set; }

        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS_TI(exporter);
            exporter.AddElement("atr", "TlakovaHladinaPlynovodniSite", (int)TlakovaHladinaPlynovodniSite);
            exporter.AddElement("atr", "Dimenze", (int)Dimenze);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TlakovaHladinaPlynovodniSite":
                        TlakovaHladinaPlynovodniSite = (DtmTlakovaHladinaPlynovodniSiteEnum)int.Parse(x.InnerText);
                        break;
                    case "Dimenze":
                        Dimenze = uint.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override string GetInfoAsString()
        {
            return $"Dimenze:{Dimenze}";
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            Dimenze = 25;
            TlakovaHladinaPlynovodniSite = DtmTlakovaHladinaPlynovodniSiteEnum.Nezjisteno;
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
            SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI = -1;
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmTlakovaHladinaPlynovodniSiteEnum>("Tlaková hladina plynovodní sítě:", TlakovaHladinaPlynovodniSite, cv => TlakovaHladinaPlynovodniSite = cv));
            properties.AddProperty(new DtmUIntCustomProperty("Dimenze:", Dimenze, cv => Dimenze = cv));
        }
    }
}
