using System;
using System.Collections.Generic;
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
            var JVFDTM = xmlDocument["JVFDTM", "objtyp"];
            var DataJVFDTM = FindElement(JVFDTM, "DataJVFDTM");
            var data = FindElement(DataJVFDTM, "Data");
            ParseDataNode(data);
            _main.UdajeOVydeji = ParseUdajeOVydeji(FindElement(DataJVFDTM, "DoprovodneInformace"));
        }

        void ParseDataNode(XmlElement dataElement)
        {
            foreach (XmlElement element in dataElement)
            {
                var group = new DtmElementsGroup(element.LocalName);
                _main.AddElementGroup(element.LocalName, group);
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
                }
            }
        }

        IDtmGeometry ParseGeometrieObjektu(XmlElement xmlElement)
        {
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "curveProperty":
                        return ParseCurveGeometry(e);
                    case "pointProperty":
                        return ParsePointGeometry(e);
                    default:
                        throw new Exception("Invalid geometry.");
                }
            }
            throw new Exception("Invalid geometry.");
        }

        IDtmGeometry ParsePointGeometry(XmlElement xmlElement)
        {
            var geometry = new DtmPointGeometry();
            var point = (XmlElement)xmlElement.ChildNodes[0];
            foreach (XmlAttribute attribute in point.Attributes)
            {
                switch (attribute.LocalName)
                {
                    case "id":
                        geometry.Id = attribute.InnerText;
                        break;
                    case "SrsName":
                        geometry.SrsName = attribute.InnerText;
                        break;
                    case "SrsDimension":
                        geometry.SrsDimension = int.Parse(attribute.InnerText);
                        break;
                }
            }
            if (point.ChildNodes.Count != 1 && point.ChildNodes[0].LocalName != "pos")
                throw new Exception("Invalid curve geometry.");
            var values = point.ChildNodes[0].InnerText.Split(' ');
            if (values.Length != 3)
                throw new Exception("Invalid Point Geometry.");
            geometry.Point = new DtmPoint(values[0], values[1], values[2]);
            return geometry;
        }

        IDtmGeometry ParseCurveGeometry(XmlElement xmlElement)
        {
            var geometry = new DtmCurveGeometry();
            var lineString = (XmlElement)xmlElement.ChildNodes[0];
            foreach (XmlAttribute attribute in lineString.Attributes)
            {
                switch (attribute.LocalName)
                {
                    case "id":
                        geometry.Id = attribute.InnerText;
                        break;
                    case "SrsName":
                        geometry.SrsName = attribute.InnerText;
                        break;
                    case "SrsDimension":
                        geometry.SrsDimension = int.Parse(attribute.InnerText);
                        break;
                }
            }
            if (lineString.ChildNodes.Count != 1 && lineString.ChildNodes[0].LocalName != "LineString")
                throw new Exception("Invalid curve geometry.");
            var posList = lineString.ChildNodes[0];
            var coordinates = posList.InnerText.Split(' ');
            if (coordinates.Length % 3 != 0)
            {
                throw new Exception($"Coordinates are not in correct format.");
            }
            var count = coordinates.Length / 3;
            geometry.Points = new List<DtmPoint>(count);
            for (var i = 0; i < count; i++)
            {
                var beginIdx = i * 3;
                geometry.Points.Add(new DtmPoint(coordinates[beginIdx], coordinates[beginIdx + 1], coordinates[beginIdx + 2]));
            }
            return geometry;
        }
        DtmPolygonGeometry ParsePolygonGeometry(XmlElement xmlElement)
        {
            var geometry = new DtmPolygonGeometry();
            var polygon = FindElement(xmlElement, "Polygon");
            foreach (XmlAttribute attribute in polygon.Attributes)
            {
                switch (attribute.LocalName)
                {
                    case "srsName":
                        geometry.SrsName = attribute.InnerText;
                        break;
                    case "srsDimension":
                        geometry.SrsDimension = int.Parse(attribute.InnerText);
                        break;
                }
            }

            var exterior = FindElement(polygon, "exterior");
            var linearRing = FindElement(exterior, "LinearRing");
            var posList = FindElement(linearRing, "posList");
            var coordinates = posList.InnerText.Split(' ');
            if (coordinates.Length % 2 != 0)
            {
                throw new Exception($"Coordinates are not in correct format.");
            }
            var count = coordinates.Length / 2;
            geometry.Points = new List<DtmPoint>(count);
            for (var i = 0; i < count; i++)
            {
                var beginIdx = i * 2;
                geometry.Points.Add(new DtmPoint(coordinates[beginIdx], coordinates[beginIdx + 1], "0"));
            }
            return geometry;
        }

        void ParseAtributyObjektu(XmlElement xmlElement, DtmElement element)
        {
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "SpolecneAtributyVsechObjektu":
                        element.SpolecneAtributy = ParseSpolecneAtributyVsechObjektu(e);
                        break;
                    case "SpolecneAtributyObjektuZPS":
                        element.SpolecneAtributyZPS = ParseDtmSpolecneAtributyZPS(e);
                        break;
                    case "CisloBodu":
                        element.CisloBodu = e.InnerText;
                        break;
                }
            }
            element.ImportDtmAttributes(xmlElement);
        }

        DtmElementSpolecneAtributy ParseSpolecneAtributyVsechObjektu(XmlElement xmlElement)
        {
            var atributy = new DtmElementSpolecneAtributy();
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "DatumVkladu":
                        atributy.DatumVkladu = DateTime.Parse(e.InnerText);
                        break;
                    case "DatumZmeny":
                        atributy.DatumZmeny = DateTime.Parse(e.InnerText);
                        break;

                }
            }
            return atributy;
        }

        DtmSpolecneAtributyZPS ParseDtmSpolecneAtributyZPS(XmlElement xmlElement)
        {
            var atributy = new DtmSpolecneAtributyZPS();
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "UrovenUmisteniObjektuZPS":
                        atributy.UrovenUmisteniObjektuZPS = int.Parse(e.InnerText);
                        break;
                    case "TridaPresnostiPoloha":
                        atributy.TridaPresnostiPoloha = int.Parse(e.InnerText);
                        break;
                    case "TridaPresnostiVyska":
                        atributy.TridaPresnostiVyska = int.Parse(e.InnerText);
                        break;
                    case "ZpusobPorizeniZPS":
                        atributy.ZpusobPorizeniZPS = int.Parse(e.InnerText);
                        break;
                }
            }
            return atributy;
        }

        XmlElement FindElement(XmlElement parent, string name)
        {
            foreach (XmlElement c in parent)
            {
                if (c.LocalName == name)
                    return c;
            }
            return null;
        }
        DtmUdajeOVydeji ParseUdajeOVydeji(XmlElement xmlElement)
        {
            var udajeOVydejiXml = FindElement(xmlElement, "UdajeOVydeji");
            if (udajeOVydejiXml == null)
                return null;
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
                        vydej.Polygon = ParsePolygonGeometry(xn);
                        break;
                }
            }
            return vydej;
        }
    }
}
