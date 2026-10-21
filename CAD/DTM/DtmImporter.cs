using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;

namespace CAD.DTM
{
    class DtmImporter
    {
        readonly IDtmMain _main;

        public DtmImporter(IDtmMain main)
        {
            _main = main;
        }

        public void ParseFile(string location)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(location);
            ParseDocument(xmlDocument);
        }

        public void ParseDocument(XmlDocument xmlDocument)
        {
            var JVFDTM = xmlDocument["JVFDTM", "objtyp"];
            var DataJVFDTM = FindElement(JVFDTM, "DataJVFDTM");
            var data = FindElement(DataJVFDTM, "Data");
            if (data == null) throw new FormatException("Soubor neobsahuje JVFDTM / DataJVFDTM / Data.");
            ParseDataNode(data);
            ParseDoprovodneInformace(DataJVFDTM);
        }

        void ParseDataNode(XmlElement dataElement)
        {
            foreach (XmlElement element in dataElement)
            {
                var group = new DtmElementsGroup(DtmSchema.CanonicalName(element.LocalName));
                _main.AddElementGroup(group.Name, group);
                ParseElementsGroup(element, group);
            }
        }

        void ParseElementsGroup(XmlElement xmlGroupReader, DtmElementsGroup group)
        {

            foreach (XmlElement xmlGroup in xmlGroupReader)
            {
                switch (xmlGroup.LocalName)
                {
                    case "ObjektovyTypNazev":
                        {
                            group.ObjektovyTypNazev = xmlGroup.InnerText;
                            group.CodeBase = xmlGroup.GetAttribute("code_base");
                            group.CodeSuffix = xmlGroup.GetAttribute("code_suffix");
                        }
                        break;
                    case "KategorieObjektu":
                        group.KategorieObjektu = xmlGroup.InnerText;
                        break;
                    case "SkupinaObjektu":
                        group.SkupinaObjektu = xmlGroup.InnerText;
                        break;
                    case "ObsahovaCast":
                        group.ObsahovaCast = xmlGroup.InnerText;
                        break;
                    case "ZaznamyObjektu":
                        {
                            foreach (XmlElement xmlElement in xmlGroup)
                            {
                                var element = DtmConfigurationSingleton.Instance.CreateType(group.Name);
                                ParseZaznamObjektu(xmlElement, element);
                                group.AddElement(element);
                            }
                        }
                        break;
                }
            }
        }

        void ParseZaznamObjektu(XmlElement xmlElement, DtmElement element)
        {
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "ZapisObjektu":
                        element.ZapisObjektu = e.InnerText.Trim(' ')[0];
                        break;
                    case "AtributyObjektu":
                        ParseAtributyObjektu(e, element);
                        break;
                    case "GeometrieObjektu":
                        element.Geometry = ParseGeometrieObjektu(e);
                        break;
                    default:
                        element.SchemaAttributes?.ImportRecordAttribute(e);
                        break;
                }
            }
            element.SchemaAttributes?.ApplyToLegacyValues();
        }

        IDtmGeometryGroup ParseGeometrieObjektu(XmlElement xmlElement)
        {
            var geometryGroup = new DtmGeometryGroup { Geometries = new List<IDtmGeometry>() };
            foreach (XmlElement e in xmlElement)
            {
                var geometry = DtmGeometryXml.Read(e);
                geometryGroup.Geometries.Add(geometry);
            }
            return geometryGroup;
        }

        DtmPolygonGeometry ParsePolygonGeometry<T>(XmlElement xmlElement) where T : DtmPolygonGeometry, new()
        {
            return DtmGeometryXml.Polygon<T>(FindElement(xmlElement, "Polygon"));
        }

        void ParseAtributyObjektu(XmlElement xmlElement, DtmElement element)
        {
            element.ImportDtmAttributes(xmlElement);
        }

        public static XmlElement FindElement(XmlElement parent, string name)
        {
            if (parent == null) return null;
            foreach (XmlElement c in parent)
            {
                if (c.LocalName == name)
                    return c;
            }
            return null;
        }

        void ParseDoprovodneInformace(XmlElement xmlElement)
        {
            var doprovodneInformace = FindElement(xmlElement, "DoprovodneInformace");
            var udajeOVydejiXml = FindElement(doprovodneInformace, "UdajeOVydeji");
            if (udajeOVydejiXml != null)
            {
                _main.UdajeOVydeji = ParseUdajeOVydeji(udajeOVydejiXml);
                return;
            }
            var udajeOZmenach = FindElement(doprovodneInformace, "UdajeOZmenach");
            if (udajeOZmenach != null)
            {
                _main.UdajeOZmenach = ParseUdajeOZmenach(udajeOZmenach);
            }
        }
        DtmUdajeOVydeji ParseUdajeOVydeji(XmlElement udajeOVydejiXml)
        {
            var vydej = new DtmUdajeOVydeji();
            foreach (XmlElement xn in udajeOVydejiXml)
            {
                switch (xn.LocalName)
                {
                    case "DatumPlatnosti":
                        vydej.DatumPlatnosti = DateTime.Parse(xn.InnerText);
                        break;
                    case "TypDatoveSady":
                        vydej.TypDatoveSady = int.Parse(xn.InnerText);
                        break;
                    case "ObvodDatoveSady":
                        vydej.Polygon = ParsePolygonGeometry<DtmPolygonGeometry>(xn);
                        break;
                }
            }
            return vydej;
        }

        UdajeOZmenach ParseUdajeOZmenach(XmlElement udajeOZmenach)
        {
            var zaznamZmeny = FindElement(udajeOZmenach, "ZaznamZmeny");
            if (zaznamZmeny == null)
                return null;
            var zmena = new UdajeOZmenach();
            foreach (XmlElement xn in zaznamZmeny)
            {
                switch (xn.LocalName)
                {
                    case "OblastZmeny":
                        var surfaceProperty = FindElement(xn, "surfaceProperty");
                        zmena.Polygon = ParsePolygonGeometry<DtmPolygonGeometry>(surfaceProperty);
                        break;
                }
            }
            return zmena;

        }
    }
}
