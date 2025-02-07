using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using GeoHelper.Utils;

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
            using (var reader = XmlReader.Create(location))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (reader.LocalName == "Data")
                                {
                                    ParseDataNode(reader);
                                }
                            }
                            break;
                        case XmlNodeType.EndElement:
                            break;
                    }
                }
            }
        }

        void ParseDataNode(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            var elements = new DtmElementsGroup(reader.LocalName);
                            _main.AddElementGroup(reader.LocalName, elements);
                            ParseElementsGroup(reader, elements);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        void ParseElementsGroup(XmlReader reader, DtmElementsGroup group)
        {
            var currentElement = "";
            var endElementName = reader.LocalName;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            Debug.Assert(currentElement == "");
                            currentElement = reader.LocalName;
                            if (currentElement == "ObjektovyTypNazev")
                            {
                                group.CodeBase = reader.GetAttribute("code_base");
                                group.CodeSuffix = reader.GetAttribute("code_suffix");
                            }
                            else if (currentElement == "ZaznamyObjektu")
                            {
                                ParseZaznamObjektu(reader, group);
                            }
                        }
                        break;
                    case XmlNodeType.Text:
                        {
                            switch (currentElement)
                            {
                                case "ObjektovyTypNazev":
                                    group.ObjektovyTypNazev = reader.Value;
                                    break;
                                case "KategorieObjektu":
                                    group.KategorieObjektu = reader.Value;
                                    break;
                                case "SkupinaObjektu":
                                    group.SkupinaObjektu = reader.Value;
                                    break;
                                case "ObsahovaCast":
                                    group.ObsahovaCast = reader.Value;
                                    break;
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        {
                            if (reader.LocalName == endElementName)
                            {
                                reader.Read();
                                return;
                            }
                            currentElement = "";
                        }
                        break;
                }

            }
        }

        void ParseZaznamObjektu(XmlReader reader, DtmElementsGroup group)
        {
            DtmElement element = null;
            var elementStack = new List<string>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.IsEmptyElement)
                                continue;
                            elementStack.Add(reader.LocalName);
                            switch (reader.LocalName)
                            {
                                case "ZaznamObjektu":
                                    {
                                        element = DtmConfigurationSingleton.Instance.CreateType(group.Name);
                                    }
                                    break;
                                case "GeometrieObjektu":
                                    ParseGeometrieObjektu(reader, element);
                                    break;
                            }
                        }
                        break;
                    case XmlNodeType.Text:
                        {
                            switch (elementStack.Back())
                            {
                                case "ZapisObjektu":
                                    element.ZapisObjektu = char.Parse(reader.Value);
                                    break;
                                case "UrovenUmisteniObjektuZPS":
                                    element.UrovenUmisteniObjektuZPS = int.Parse(reader.Value);
                                    break;
                                case "TridaPresnostiPoloha":
                                    element.TridaPresnostiPoloha = int.Parse(reader.Value);
                                    break;
                                case "TridaPresnostiVyska":
                                    element.TridaPresnostiVyska = int.Parse(reader.Value);
                                    break;
                                case "ZpusobPorizeniZPS":
                                    element.ZpusobPorizeniZPS = int.Parse(reader.Value);
                                    break;
                                case "VyskaNaTerenu":
                                    element.VyskaNaTerenu = double.Parse(reader.Value, CultureInfo.InvariantCulture);
                                    break;
                                case "ID":
                                    element.ID = reader.Value;
                                    break;
                                case "CisloBodu":
                                    element.CisloBodu = reader.Value;
                                    break;

                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        {
                            if (reader.LocalName == "ZaznamyObjektu")
                            {
                                Debug.Assert(elementStack.Count == 0);
                                reader.Read();
                                return;
                            }
                            Debug.Assert(reader.LocalName == elementStack.Back());
                            elementStack.Pop();
                            if (elementStack.Count == 0)
                            {
                                group.AddElement(element);
                                element = null;
                            }
                        }
                        break;
                }
            }
        }

        void ParseGeometrieObjektu(XmlReader reader, DtmElement dtmElement)
        {
            var elementStack = new List<string>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.IsEmptyElement)
                                continue;
                            elementStack.Add(reader.LocalName);
                            switch (reader.LocalName)
                            {
                                case "pointProperty":
                                    dtmElement.Geometry = ParsePointGeometry(reader);
                                    break;
                                case "curveProperty":
                                    dtmElement.Geometry = ParseCurveGeometry(reader);
                                    break;
                                default:
                                    throw new Exception($"Geometry type is not supported {reader.LocalName}");
                            }

                        }
                        break;
                    case XmlNodeType.EndElement:
                        {
                            Debug.Assert(reader.LocalName == elementStack.Back());
                            elementStack.Pop();
                            if (elementStack.Count == 0)
                            {
                                reader.Read();
                                return;
                            }
                        }
                        break;
                }

            }
        }

        IDtmGeometry ParseCurveGeometry(XmlReader reader)
        {
            var geometry = new DtmCurveGeometry();
            var elementStack = new List<string>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.IsEmptyElement)
                                continue;
                            elementStack.Add(reader.LocalName);
                            if (reader.LocalName == "LineString")
                            {
                                geometry.Id = reader.GetAttribute("gml:id");
                                geometry.SrsName = reader.GetAttribute("srsName");
                                geometry.SrsDimension = int.Parse(reader.GetAttribute("srsDimension"));
                            }
                            break;
                        }
                    case XmlNodeType.Text:
                        {
                            switch (elementStack.Back())
                            {
                                case "posList":
                                    {
                                        var coordinates = reader.Value.Split(' ');
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

                                    }
                                    break;
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        {
                            elementStack.Pop();
                            if (elementStack.Count == 0)
                            {
                                reader.Read();
                                return geometry;
                            }
                        }
                        break;

                }
            }
            throw new Exception("Invalid Curve Geometry.");
        }

        IDtmGeometry ParsePointGeometry(XmlReader reader)
        {
            var geometry = new DtmPointGeometry();
            var elementStack = new List<string>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.IsEmptyElement)
                                continue;
                            elementStack.Add(reader.LocalName);
                            if (reader.LocalName == "Point")
                            {
                                geometry.Id = reader.GetAttribute("gml:id");
                                geometry.SrsName = reader.GetAttribute("srsName");
                                geometry.SrsDimension = int.Parse(reader.GetAttribute("srsDimension"));
                            }
                        }
                        break;
                    case XmlNodeType.Text:
                        {
                            if (elementStack.Back() == "pos")
                            {
                                var values = reader.Value.Split(' ');
                                if (values.Length != 3)
                                    throw new Exception("Invalid Point Geometry.");
                                geometry.Point = new DtmPoint(values[0], values[1], values[2]);
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        {
                            elementStack.Pop();
                            if (elementStack.Count == 0)
                            {
                                reader.Read();
                                return geometry;
                            }
                        }
                        break;
                }

            }
            throw new Exception("Invalid Point Geometry.");
        }
    }
}
