using System.Collections.Generic;
using System.Linq;
using CAD.DTM;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Validation;
using CAD.DTM.Validation.Rules;
using NUnit.Framework;

namespace CAD.DtmTests;

/// <summary>Areas are derived from closed ZPS construction lines; a definition point inside them lies in an area (rule 4.7).</summary>
[TestFixture]
public class DtmDerivedAreaTests
{
    DtmMain _main;
    [SetUp] public void SetUp() => _main = new DtmMain();
    static DtmPoint P(double x, double y) => new() { X = x, Y = y, Z = 300 };

    void Add(string group, IDtmGeometry geometry)
    {
        var element = DtmConfigurationSingleton.Instance.CreateType(group);
        element.ZapisObjektu = 'i';
        element.Geometry = new DtmGeometryGroup { Geometries = new List<IDtmGeometry> { geometry } };
        _main.AddElementIfNotExist(group, element);
    }

    void Line(params DtmPoint[] points) => Add("HraniceBudovy", new DtmCurveGeometry { Points = points.ToList(), SrsDimension = 3 });
    void DefinitionPoint(double x, double y) => Add("BudovaDefinicniBod", new DtmPointGeometry { Point = P(x, y), SrsDimension = 3 });
    List<string> Issues() => new DtmDefinitionPointWithoutAreaRule()
        .Validate(new DtmValidationContext(_main, null, DtmValidationSettings.Default)).Issues.Select(i => i.Message).ToList();

    [Test]
    public void PointInsideOneClosedLineLiesInArea()
    {
        Line(P(0, 0), P(10, 0), P(10, 10), P(0, 10), P(0, 0));
        DefinitionPoint(5, 5);
        Assert.That(Issues(), Is.Empty);
    }

    [Test]
    public void PointInsideFourSeparateLinesLiesInArea()
    {
        Line(P(0, 0), P(10, 0));
        Line(P(10, 10), P(10, 0));
        Line(P(10, 10), P(0, 10));
        Line(P(0, 10), P(0, 0));
        DefinitionPoint(5, 5);
        Assert.That(Issues(), Is.Empty);
    }

    [Test]
    public void PointOutsideOrInOpenLinesHasNoArea()
    {
        Line(P(0, 0), P(10, 0), P(10, 10), P(0, 10), P(0, 0));
        Line(P(20, 0), P(30, 0), P(30, 10), P(20, 10));
        DefinitionPoint(15, 5);
        DefinitionPoint(25, 5);
        Assert.That(Issues(), Has.Count.EqualTo(2));
    }

    [Test]
    public void DanglingLineInsideDoesNotBreakArea()
    {
        Line(P(0, 0), P(10, 0), P(10, 10), P(0, 10), P(0, 0));
        Line(P(10, 10), P(5, 8));
        Assert.That(DtmDerivedAreas.Faces(new DtmValidationContext(_main, null, DtmValidationSettings.Default).Lines), Has.Count.EqualTo(1));
    }
}
