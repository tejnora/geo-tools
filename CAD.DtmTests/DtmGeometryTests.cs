using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using CAD.DTM;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;
using NUnit.Framework;

namespace CAD.DtmTests;

public class DtmGeometryTests
{
    const string Gml = "http://www.opengis.net/gml/3.2";
    static IDtmGeometry Read(string text) { var doc = new XmlDocument(); doc.LoadXml(text); return DtmGeometryXml.Read(doc.DocumentElement); }
    static string Export(IDtmGeometry geometry) { using var writer = new XmlExporter(); geometry.ExportToDtm(writer); return writer.ToString(); }

    [TestCase(2)]
    [TestCase(3)]
    public void MultiCurvePreservesAllMembersAndDimension(int dimension)
    {
        var coordinates = dimension == 2 ? "0 0\n10 5" : "0 0 100\n10 5 105";
        var xml = $"<gml:multiCurveProperty xmlns:gml='{Gml}'><gml:MultiCurve gml:id='multi' srsName='EPSG:5514' srsDimension='{dimension}'><gml:curveMember><gml:LineString><gml:posList>{coordinates}</gml:posList></gml:LineString></gml:curveMember><gml:curveMember><gml:LineString gml:id='second'><gml:posList>{coordinates}</gml:posList></gml:LineString></gml:curveMember></gml:MultiCurve></gml:multiCurveProperty>";
        var geometry = (DtmMultiCurveGeometry)Read(xml);
        Assert.That(geometry.Members, Has.Count.EqualTo(2));
        var roundtrip = (DtmMultiCurveGeometry)Read(Export(geometry));
        Assert.That(roundtrip.Members, Has.Count.EqualTo(2));
        Assert.That(roundtrip.Members[1].Id, Is.EqualTo("second"));
        Assert.That(roundtrip.Members[1].SrsDimension, Is.EqualTo(dimension));
        Assert.That(roundtrip.Members[1].Points[1].Z, Is.EqualTo(dimension == 3 ? 105 : 0));
    }

    [TestCase(2)]
    [TestCase(3)]
    public void PolygonPreservesInteriorRingsAndDimension(int dimension)
    {
        var exterior = dimension == 2 ? "0 0 10 0 10 10 0 0" : "0 0 5 10 0 6 10 10 7 0 0 5";
        var interior = dimension == 2 ? "2 2 3 2 3 3 2 2" : "2 2 5 3 2 6 3 3 7 2 2 5";
        var xml = $"<gml:surfaceProperty xmlns:gml='{Gml}'><gml:Polygon gml:id='area' srsName='EPSG:5514' srsDimension='{dimension}'><gml:exterior><gml:LinearRing><gml:posList>{exterior}</gml:posList></gml:LinearRing></gml:exterior><gml:interior><gml:LinearRing><gml:posList>{interior}</gml:posList></gml:LinearRing></gml:interior></gml:Polygon></gml:surfaceProperty>";
        var geometry = (DtmSurfaceGeometry)Read(xml);
        var roundtrip = (DtmSurfaceGeometry)Read(Export(geometry));
        Assert.That(roundtrip.InteriorRings, Has.Count.EqualTo(1));
        Assert.That(roundtrip.InteriorRings[0][2].X, Is.EqualTo(3));
        Assert.That(roundtrip.InteriorRings[0][2].Z, Is.EqualTo(dimension == 3 ? 7 : 0));
        Assert.That(roundtrip.SrsDimension, Is.EqualTo(dimension));
    }

    [Test]
    public void CurveExportsTheDeclaredCoordinateDimension()
    {
        var curve = new DtmCurveGeometry { SrsDimension = 2, Id = "L", Points = new List<DtmPoint> { new() { X = 1, Y = 2, Z = 77 }, new() { X = 3, Y = 4, Z = 88 } } };
        var xml = XDocument.Parse(Export(curve));
        Assert.That(xml.Descendants(XName.Get("posList", Gml)).Single().Value.Split(' '), Has.Length.EqualTo(4));
    }

    [Test]
    public void ExportDoesNotRoundCoordinatesToCentimeters()
    {
        var point = new DtmPointGeometry { Id = "precise", Point = new DtmPoint { X = -500000.123456, Y = -1000000.654321, Z = 123.123456 } };
        var roundtrip = (DtmPointGeometry)Read(Export(point));
        Assert.That(roundtrip.Point.X, Is.EqualTo(point.Point.X).Within(1e-9));
        Assert.That(roundtrip.Point.Y, Is.EqualTo(point.Point.Y).Within(1e-9));
        Assert.That(roundtrip.Point.Z, Is.EqualTo(point.Point.Z).Within(1e-9));
    }

    [Test]
    public void GeometryEditorCloneIsIsolatedFromOriginal()
    {
        var source = (DtmSurfaceGeometry)DtmCoverageTests.ExampleGeometry("surfaceProperty");
        source.InteriorRings.Add(source.Points.Select(p => (DtmPoint)p.Clone()).ToList());
        var clone = (DtmSurfaceGeometry)DtmGeometryEditorDialog.Clone(source);
        clone.Points[0].X = 123;
        clone.InteriorRings[0][0].Z = 999;
        Assert.That(source.Points[0].X, Is.Not.EqualTo(123));
        Assert.That(source.InteriorRings[0][0].Z, Is.Not.EqualTo(999));
    }

    [TestCase("1 2 3 4 5")]
    [TestCase("")]
    public void MalformedCoordinatesFailExplicitly(string coordinates)
    {
        Assert.Throws<FormatException>(() => Read($"<gml:curveProperty xmlns:gml='{Gml}'><gml:LineString srsDimension='3'><gml:posList>{coordinates}</gml:posList></gml:LineString></gml:curveProperty>"));
    }
}
