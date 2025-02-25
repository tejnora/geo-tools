using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Xml;

namespace CAD.DTM.Elements
{
    enum DtmTypStavby
    {
        Podezdivka = 1,
        Rampa = 2,
        Terasa = 3,
        Komin = 4,
        Sklenik = 5,
        ZahradniBazen = 6,
        PatkaDdeskaMonolitPilir = 7,
        StavbaProZpevneniPovrchu = 8,
        CeloPropustku = 9,
        DrobnaSakralniStavba = 10,
        DrobnaKulturniStavba = 11,
        OstatniZastresenaStavba = 12,
        Zastreseni = 13,
        Nezjisteno = 99
    }

    class DtmHraniceStavbyElement
        : DtmElement
    {
        public DtmHraniceStavbyElement()
        {
            TypStavby = DtmTypStavby.OstatniZastresenaStavba;
        }
        public DtmTypStavby TypStavby { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypStavby", (int)TypStavby);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypStavby":
                        TypStavby = (DtmTypStavby)int.Parse(x.InnerText);
                        break;
                }
            }
        }

        public override string GetInfoAsString()
        {
            return $"Type stavby: {TypStavby}";
        }

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            TypStavby = DtmTypStavby.OstatniZastresenaStavba;
            SpolecneAtributyZPS = new DtmSpolecneAtributyZPS
            {
                TridaPresnostiPoloha = 3,
                TridaPresnostiVyska = 3,
                UrovenUmisteniObjektuZPS = 0,
                ZpusobPorizeniZPS = 1
            };

        }
    }
}
