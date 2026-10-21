using System.Collections.Generic;
using System;
using System.Globalization;
using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using CAD.DTM.Elements.GUI;

namespace CAD.DTM.Elements
{
    class DtmZarizeniKanalizacniPripojkyBodElement
        : DtmBodBaseElement
    {
        public DtmZarizeniKanalizacniPripojkyBodElement()
        {

        }
        public override DtmGraphicElement GetGraphicElement(DtmElementOption dtmElement, DtmGraphicElementScaleEnum scale)
        {
            return dtmElement.GetGraphicElement(TypZarizeniKanalizacniPripojky.ToString(), scale);
        }
        public DtmTypZarizeniKanalizacniPripojkyEnum TypZarizeniKanalizacniPripojky { get; set; }

        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<DtmTypZarizeniKanalizacniPripojkyEnum>
                ("Typ zařízení kanalizační přípojky:", TypZarizeniKanalizacniPripojky, (cv) => TypZarizeniKanalizacniPripojky = cv));
        }

        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
            SpolecneAtributyObjektuZPS_TI.ZpusobPorizeniTI = 1;
            SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI = -1;
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
