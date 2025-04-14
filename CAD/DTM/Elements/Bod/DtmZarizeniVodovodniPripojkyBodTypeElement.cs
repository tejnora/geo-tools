using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Xml;

namespace CAD.DTM.Elements
{
    public class DtmZarizeniVodovodniPripojkyBodTypeElement
        : DtmBodBaseElement
    {
        public DtmTypZarizeniVodovodniPripojkyEnum TypZarizeniVodovodniPripojky { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS_TI(exporter);
            exporter.AddElement("atr", "TypZarizeniVodovodniPripojky", (int)TypZarizeniVodovodniPripojky);
        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypZarizeniVodovodniPripojky":
                        TypZarizeniVodovodniPripojky = (DtmTypZarizeniVodovodniPripojkyEnum)int.Parse(x.InnerText, CultureInfo.InvariantCulture);
                        break;
                }
            }
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
            TypZarizeniVodovodniPripojky = DtmTypZarizeniVodovodniPripojkyEnum.Jine;
        }
        public override string GetInfoAsString()
        {
            return $"Type zarizeni vodovodni pripojky: {TypZarizeniVodovodniPripojky}";
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(DtmTypZarizeniVodovodniPripojkyEnum));
        public override void SelectedSetting(string value)
        {
            TypZarizeniVodovodniPripojky = (DtmTypZarizeniVodovodniPripojkyEnum)Enum.Parse(typeof(DtmTypZarizeniVodovodniPripojkyEnum), value);
        }

    }
}
