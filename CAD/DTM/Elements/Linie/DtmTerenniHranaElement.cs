using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;
using System.Collections.Generic;
using System;
using System.Xml;

namespace CAD.DTM.Elements
{
    enum TypTerenniHranyEnum
    {
        Hrana = 1,
        Pata = 2,
        Nezjisteno = 99
    }
    class DtmTerenniHranaElement
        : DtmLinieElementBase
    {
        public TypTerenniHranyEnum TypTerenniHrany { get; set; }
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS(exporter);
            exporter.AddElement("atr", "TypTerenniHrany", (int)TypTerenniHrany);

        }
        public override void ImportDtmAttributes(XmlElement xmlElement)
        {
            base.ImportDtmAttributes(xmlElement);
            foreach (XmlElement x in xmlElement)
            {
                switch (x.LocalName)
                {
                    case "TypTerenniHrany":
                        TypTerenniHrany = (TypTerenniHranyEnum)int.Parse(x.InnerText);
                        break;
                }
            }
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
            properties.AddProperty(new DtmEnumCustomProperty<TypTerenniHranyEnum>("Typ terénní hrany:", TypTerenniHrany, cv => TypTerenniHrany = cv));
        }
        public override IEnumerable<string> Settings => Enum.GetNames(typeof(TypTerenniHranyEnum));
        public override void SelectedSetting(string value)
        {
            TypTerenniHrany = (TypTerenniHranyEnum)Enum.Parse(typeof(TypTerenniHranyEnum), value);
        }
        public override string GetInfoAsString()
        {
            return $"Typ terenni hrany: {TypTerenniHrany}";
        }
    }
}
