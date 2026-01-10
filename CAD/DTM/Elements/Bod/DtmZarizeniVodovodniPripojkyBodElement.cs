using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Globalization;
using System.Xml;
using System.Collections.Generic;
using System;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    public class DtmZarizeniVodovodniPripojkyBodElement
        : DtmBodBaseElement
    {
        public DtmZarizeniVodovodniPripojkyBodElement()
        {

        }
        public override DtmGraphicElement GetGraphicElement(DtmElementOption dtmElement, DtmGraphicElementScaleEnum scale)
        {
            return dtmElement.GetGraphicElement(TypZarizeniVodovodniPripojky.ToString(), scale);
        }
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
            SpolecneAtributyObjektuZPS_TI.ZpusobPorizeniTI = 1;
            SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI = -1;
            TypZarizeniVodovodniPripojky = DtmTypZarizeniVodovodniPripojkyEnum.SachtaMernaAKontrolní;
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
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmReadonlyCustomProperty("Typ zařízení vodovodní přípojky:", TypZarizeniVodovodniPripojky.ToString()));
        }
    }
}
