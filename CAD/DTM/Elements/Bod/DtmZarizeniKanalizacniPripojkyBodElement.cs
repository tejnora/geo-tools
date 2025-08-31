using System.Collections.Generic;
using System;
using System.Globalization;
using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmZarizeniKanalizacniPripojkyBodElement
        : DtmBodBaseElement
    {
        public DtmZarizeniKanalizacniPripojkyBodElement()
        {

        }

        public DtmTypZarizeniKanalizacniPripojkyEnum TypZarizeniKanalizacniPripojky { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS_TI(exporter);
            exporter.AddElement("atr", "TypZarizeniKanalizacniPripojky", (int)TypZarizeniKanalizacniPripojky);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypZarizeniKanalizacniPripojky":
                        TypZarizeniKanalizacniPripojky = (DtmTypZarizeniKanalizacniPripojkyEnum)int.Parse(x.InnerText, CultureInfo.InvariantCulture);
                        break;
                }
            }
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
            TypZarizeniKanalizacniPripojky = DtmTypZarizeniKanalizacniPripojkyEnum.RevizniSachta;
        }
        public override string GetInfoAsString()
        {
            return $"Type zarizeni kanalizacni pripojky: {TypZarizeniKanalizacniPripojky}";
        }

        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypZarizeniKanalizacniPripojkyEnum));
        public override void SelectedSetting(string value)
        {
            TypZarizeniKanalizacniPripojky = (DtmTypZarizeniKanalizacniPripojkyEnum)Enum.Parse(typeof(DtmTypZarizeniKanalizacniPripojkyEnum), value);
        }
    }
}
