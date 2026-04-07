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
            var JVFDTM = xmlDocument["JVFDTM", "objtyp"];
            var DataJVFDTM = FindElement(JVFDTM, "DataJVFDTM");
            var data = FindElement(DataJVFDTM, "Data");
            ParseDataNode(data);
            ParseDoprovodneInformace(DataJVFDTM);
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

        IDtmGeometryGroup ParseGeometrieObjektu(XmlElement xmlElement)
        {
            var geometryGroup = new DtmGeometryGroup { Geometries = new List<IDtmGeometry>() };
            foreach (XmlElement e in xmlElement)
            {
                IDtmGeometry geometry;
                switch (e.LocalName)
                {
                    case "curveProperty":
                        geometry = ParseCurveGeometry<DtmCurveGeometry>(e, false);
                        break;
                    case "pointProperty":
                        geometry = ParsePointGeometry(e);
                        break;
                    case "surfaceProperty":
                        geometry = ParseSurfaceProperty(e);
                        break;
                    case "multiCurveProperty":
                        geometry = ParseMultiCurveProperty(e);
                        break;
                    default:
                        throw new Exception("Invalid geometry.");
                }
                geometryGroup.Geometries.Add(geometry);
            }
            return geometryGroup;
        }

        void ParseGeometryAttributes(IDtmGeometry geometry, XmlElement xmlElement)
        {
            foreach (XmlAttribute attribute in xmlElement.Attributes)
            {
                switch (attribute.LocalName)
                {
                    case "id":
                        geometry.Id = attribute.InnerText;
                        break;
                    case "srsName":
                        geometry.SrsName = attribute.InnerText;
                        break;
                    case "srsDimension":
                        geometry.SrsDimension = int.Parse(attribute.InnerText);
                        break;
                }
            }
        }

        IDtmGeometry ParsePointGeometry(XmlElement xmlElement)
        {
            var geometry = new DtmPointGeometry();
            var point = (XmlElement)xmlElement.ChildNodes[0];
            ParseGeometryAttributes(geometry, point);
            if (point.ChildNodes.Count != 1 && point.ChildNodes[0].LocalName != "pos")
                throw new Exception("Invalid curve geometry.");
            var values = point.ChildNodes[0].InnerText.Split(' ');
            switch (geometry.SrsDimension)
            {
                case 2:
                    {
                        if (values.Length != 2)
                            throw new Exception("Invalid Point Geometry.");
                        geometry.Point = new DtmPoint(values[0], values[1]);
                    }
                    break;
                case 3:
                    {
                        if (values.Length != 3)
                            throw new Exception("Invalid Point Geometry.");
                        geometry.Point = new DtmPoint(values[0], values[1], values[2]);
                    }
                    break;
                default:
                    throw new Exception($"Dimension '{geometry.SrsDimension}' is not valid.");
            }
            return geometry;
        }

        IDtmGeometry ParseSurfaceProperty(XmlElement xmlElement)
        {
            var polygon = (XmlElement)xmlElement.ChildNodes[0];
            if (polygon.LocalName == "Polygon")
                return ParsePolygonGeometry<DtmSurfaceGeometry>(xmlElement);
            throw new Exception("Invalid surface geometry.");
        }
        IDtmGeometry ParseMultiCurveProperty(XmlElement xmlElement)
        {
            var multiCurveElement = FindElement(xmlElement, "MultiCurve");
            if (multiCurveElement == null)
                throw new Exception("Invalid multi-curve geometry.");
            var curveMemberElement = FindElement(multiCurveElement, "curveMember");
            var dtmGeometry = ParseCurveGeometry<DtmMultiCurveGeometry>(curveMemberElement, true);
            ParseGeometryAttributes(dtmGeometry, multiCurveElement);
            //todo
            return dtmGeometry;
        }

        IDtmGeometry ParseCurveGeometry<T>(XmlElement xmlElement, bool skipAttributes) where T : DtmCurveGeometry, new()
        {
            var geometry = new T();
            var lineString = (XmlElement)xmlElement.ChildNodes[0];
            if (!skipAttributes)
            {
                ParseGeometryAttributes(geometry, lineString);
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
        DtmPolygonGeometry ParsePolygonGeometry<T>(XmlElement xmlElement) where T : DtmPolygonGeometry, new()
        {
            var geometry = new T();
            var polygon = FindElement(xmlElement, "Polygon");
            ParseGeometryAttributes(geometry, polygon);
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
            element.ImportDtmAttributes(xmlElement);
        }

        public static XmlElement FindElement(XmlElement parent, string name)
        {
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
