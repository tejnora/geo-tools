using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using System.Globalization;
using System.Windows.Controls;
using System.Xml;
using CAD.DTM.Elements.Bod.Gui;
using CAD.DTM.Elements.Linie;
using System.Collections.Generic;
using System;

namespace CAD.DTM.Elements
{
    public class DtmZarizeniVodovodniPripojkyBodElement
        : DtmBodBaseElement
        , IAdditionalPropertiesGui
    {
        public DtmZarizeniVodovodniPripojkyBodElement()
        {

        }
        public override DtmBodDrawingMarkEnum DrawingMark => DtmBodDrawingMarkEnum.Circle;
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
        public override IAdditionalPropertiesGui AdditionalPropertiesGui => this;
        static readonly DtmZarizeniVodovodniPripojkyBodPP GUIControl = new DtmZarizeniVodovodniPripojkyBodPP();
        public void InitGui(ContentControl additionalProperties)
        {
            additionalProperties.Content = GUIControl;
            GUIControl.SetElement(this);
        }
    }
}
