using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CAD.DTM
{
    public static class DtmGeometryXml
    {
        public static IDtmGeometry Read(XmlElement property)
        {
            var value = property.ChildNodes.OfType<XmlElement>().FirstOrDefault()
                ?? throw new FormatException("Chybí geometrie " + property.LocalName);
            return property.LocalName switch
            {
                "pointProperty" => Point(value),
                "curveProperty" => Curve(value),
                "surfaceProperty" => Polygon<DtmSurfaceGeometry>(value),
                "multiCurveProperty" => MultiCurve(value),
                _ => throw new FormatException("Nepodporovaná geometrie: " + property.LocalName)
            };
        }

        public static void Attributes(IDtmGeometry geometry, XmlElement element, IDtmGeometry inherited = null)
        {
            geometry.Id = element.GetAttribute("id", "http://www.opengis.net/gml/3.2");
            geometry.SrsName = element.HasAttribute("srsName") ? element.GetAttribute("srsName") : inherited?.SrsName ?? "EPSG:5514";
            geometry.SrsDimension = element.HasAttribute("srsDimension") ? XmlConvert.ToInt32(element.GetAttribute("srsDimension")) : inherited?.SrsDimension ?? 2;
            if (geometry.SrsDimension is not (2 or 3)) throw new FormatException("Rozměr souřadnic musí být 2 nebo 3.");
        }

        public static List<DtmPoint> Coordinates(XmlElement element, int dimension)
        {
            if (element == null) throw new FormatException("Chybí souřadnice geometrie.");
            if (element.HasAttribute("srsDimension")) dimension = XmlConvert.ToInt32(element.GetAttribute("srsDimension"));
            if (dimension is not (2 or 3)) throw new FormatException("Rozměr souřadnic musí být 2 nebo 3.");
            var values = element.InnerText.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length == 0 || values.Length % dimension != 0) throw new FormatException("Neplatný počet souřadnic geometrie.");
            var result = new List<DtmPoint>();
            for (var i = 0; i < values.Length; i += dimension)
                result.Add(new DtmPoint(values[i], values[i + 1], dimension == 3 ? values[i + 2] : "0"));
            return result;
        }

        static DtmPointGeometry Point(XmlElement element)
        {
            if (element.LocalName != "Point") throw new FormatException("Očekávána geometrie Point.");
            var result = new DtmPointGeometry();
            Attributes(result, element);
            var points = Coordinates(Child(element, "pos"), result.SrsDimension);
            if (points.Count != 1) throw new FormatException("Bod musí obsahovat právě jednu souřadnici.");
            result.Point = points[0];
            return result;
        }

        static DtmCurveGeometry Curve(XmlElement element, IDtmGeometry inherited = null)
        {
            if (element.LocalName != "LineString") throw new FormatException("Očekávána geometrie LineString.");
            var result = new DtmCurveGeometry();
            Attributes(result, element, inherited);
            result.Points = Coordinates(Child(element, "posList"), result.SrsDimension);
            return result;
        }

        static DtmMultiCurveGeometry MultiCurve(XmlElement element)
        {
            if (element.LocalName != "MultiCurve") throw new FormatException("Očekávána geometrie MultiCurve.");
            var result = new DtmMultiCurveGeometry();
            Attributes(result, element);
            foreach (var member in element.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "curveMember"))
                result.Members.Add(Curve(member.ChildNodes.OfType<XmlElement>().Single(), result));
            if (result.Members.Count == 0) throw new FormatException("MultiCurve neobsahuje žádný člen.");
            result.Points = result.Members[0].Points;
            return result;
        }

        public static T Polygon<T>(XmlElement element) where T : DtmPolygonGeometry, new()
        {
            if (element.LocalName != "Polygon") throw new FormatException("Očekávána geometrie Polygon.");
            var result = new T();
            Attributes(result, element);
            result.Points = Ring(Child(element, "exterior"), result.SrsDimension);
            foreach (var interior in element.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "interior"))
                result.InteriorRings.Add(Ring(interior, result.SrsDimension));
            return result;
        }

        static List<DtmPoint> Ring(XmlElement element, int dimension) => Coordinates(Child(Child(element, "LinearRing"), "posList"), dimension);
        static XmlElement Child(XmlElement element, string name) => element?.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.LocalName == name);
    }
}
