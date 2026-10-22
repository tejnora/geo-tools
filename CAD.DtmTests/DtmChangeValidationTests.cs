using System.Collections.Generic;
using System.Linq;
using CAD.DTM;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Validation;
using CAD.DTM.Validation.Rules;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture]
public class DtmChangeValidationTests
{
    DtmMain _main;
    [SetUp] public void SetUp() => _main = new DtmMain();
    static DtmPoint P(double x, double y = 0) => new() { X = x, Y = y, Z = 100 };
    static DtmPointGeometry Point(double x, double y = 0) => new() { Point = P(x, y), SrsDimension = 3 };
    static DtmCurveGeometry Line(params DtmPoint[] points) => new() { Points = points.ToList(), SrsDimension = 3 };
    static DtmSurfaceGeometry Area() => new() { Points = new() { P(0), P(10), P(10, 10), P(0, 10), P(0) }, SrsDimension = 3 };

    DtmElement Add(string group, char mode, IDtmGeometry geometry)
    {
        var element = DtmConfigurationSingleton.Instance.CreateType(group);
        element.ZapisObjektu = mode;
        element.Geometry = new DtmGeometryGroup { Geometries = new List<IDtmGeometry> { geometry } };
        if (_main.getElementsGroup(group) == null) _main.AddElementGroup(group, new DtmElementsGroup(group));
        ((DtmElementsGroup)_main.getElementsGroup(group)).AddElement(element);
        return element;
    }

    DtmValidationContext Context() => new(_main, null, DtmValidationSettings.Default);
    DtmRuleResult Check(DtmValidationRule rule) => rule.Validate(Context());

    [Test]
    public void UnchangedReferenceDefectsAreIgnored()
    {
        Add("PodrobnyBodZPS", 'r', Point(50));
        Add("PodrobnyBodZPS", 'r', Point(50));
        Add("Plot", 'r', Line(P(0), P(0), P(0.01)));
        Add("Plot", 'r', Line(P(0), P(0), P(0.01)));
        Add("BudovaPlocha", 'r', Area());
        Add("BudovaDefinicniBod", 'r', Point(20, 20));
        Assert.That(Check(new DtmTopologyGroupRule()).Status, Is.EqualTo(DtmRuleStatus.Passed));
        Assert.That(Check(new DtmDerivedObjectsGroupRule()).Status, Is.EqualTo(DtmRuleStatus.Passed));
    }

    [TestCase('i')]
    [TestCase('u')]
    public void ChangedLinesAreCheckedAgainstReferences(char mode)
    {
        Add("Plot", 'r', Line(P(0), P(10, 10)));
        Add("Plot", mode, Line(P(0, 10), P(10)));
        Assert.That(Check(new DtmLineCrossingRule()).Status, Is.EqualTo(DtmRuleStatus.Failed));
    }

    [Test]
    public void ThirdDuplicateIsNotHiddenByTwoReferences()
    {
        Add("PodrobnyBodZPS", 'r', Point(0));
        Add("PodrobnyBodZPS", 'r', Point(0));
        Add("PodrobnyBodZPS", 'i', Point(0));
        Assert.That(Check(new DtmPointDuplicityRule()).Issues.Single().ObjectName, Does.EndWith("#3"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void DeletingLastLineReportsOnlyNewlyOrphanedPoints(bool explicitDelete)
    {
        Add("PodrobnyBodZPS", 'r', Point(0));
        Add("PodrobnyBodZPS", 'r', Point(99)); // Already orphaned on import.
        var line = Add("Plot", explicitDelete ? 'd' : 'r', Line(P(0), P(10)));
        if (!explicitDelete) line.IsDeleted = true;
        Assert.That(Check(new DtmSolitaryPodrobnyBodRule()).Issues, Has.Count.EqualTo(1));
        Assert.That(Context().Lines, Is.Empty);
        if (!explicitDelete)
        {
            line.IsDeleted = false;
            Assert.That(Check(new DtmSolitaryPodrobnyBodRule()).Issues, Is.Empty);
        }
    }

    [Test]
    public void AnotherSurvivingLinePreservesPointDependency()
    {
        Add("PodrobnyBodZPS", 'r', Point(0));
        Add("Plot", 'r', Line(P(0), P(10))).IsDeleted = true;
        Add("Plot", 'r', Line(P(0), P(20)));
        Assert.That(Check(new DtmSolitaryPodrobnyBodRule()).Issues, Is.Empty);
    }

    [Test]
    public void DeletingPointReportsOnlyPreviouslySupportedVertices()
    {
        Add("PodrobnyBodZPS", 'r', Point(0)).IsDeleted = true;
        Add("Plot", 'r', Line(P(0), P(10)));
        Assert.That(Check(new DtmVertexPodrobnyBodRule()).Issues, Has.Count.EqualTo(1));
    }

    [Test]
    public void UpdateUsesDeepOriginalGeometryForLostDependencies()
    {
        Add("PodrobnyBodZPS", 'r', Point(0));
        var geometry = Line(P(0), P(10));
        var line = Add("Plot", 'r', geometry);
        line.ZapisObjektu = 'u';
        geometry.Points[0].X = 20;
        Assert.That(Check(new DtmSolitaryPodrobnyBodRule()).Issues, Has.Count.EqualTo(1));
        Assert.That(Check(new DtmOriginalStateAvailableRule()).Issues, Is.Empty);
        Assert.That(Context().OriginalOf(Context().Lines.Single()).Points.First().X, Is.Zero);
        geometry.Points[0].X = 0;
        Assert.That(Check(new DtmSolitaryPodrobnyBodRule()).Issues, Is.Empty);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void DeletingOrMovingDefinitionPointInvalidatesReferenceArea(bool move)
    {
        Add("BudovaPlocha", 'r', Area());
        var geometry = Point(5, 5);
        var definition = Add("BudovaDefinicniBod", 'r', geometry);
        if (move) { definition.ZapisObjektu = 'u'; geometry.Point.X = 20; }
        else definition.IsDeleted = true;
        Assert.That(Check(new DtmAreaDefinitionPointRule()).Issues, Has.Count.EqualTo(1));
    }

    [Test]
    public void RemovingAreaOrphansItsSurvivingDefinitionPoint()
    {
        Add("BudovaPlocha", 'r', Area()).IsDeleted = true;
        Add("BudovaDefinicniBod", 'r', Point(5, 5));
        Add("BudovaDefinicniBod", 'r', Point(20, 20));
        Assert.That(Check(new DtmDefinitionPointWithoutAreaRule()).Issues, Has.Count.EqualTo(1));
    }

    [Test]
    public void AddedDefinitionPointCanInvalidateReferenceArea()
    {
        Add("BudovaPlocha", 'r', Area());
        Add("BudovaDefinicniBod", 'r', Point(5, 5));
        var added = Add("BudovaDefinicniBod", 'i', Point(6, 6));
        Assert.That(Check(new DtmAreaDefinitionPointRule()).Issues, Has.Count.EqualTo(1));
        added.IsDeleted = true;
        Assert.That(Check(new DtmAreaDefinitionPointRule()).Issues, Is.Empty);
    }

    [Test]
    public void UnknownUpdateOriginalProducesWarningAndStillChecksNewState()
    {
        Add("Plot", 'u', Line(P(0), P(0)));
        Assert.That(Check(new DtmOriginalStateAvailableRule()).Status, Is.EqualTo(DtmRuleStatus.Warning));
        Assert.That(Check(new DtmZeroSegmentLengthRule()).Status, Is.EqualTo(DtmRuleStatus.Failed));
    }

    [Test]
    public void CancelledInsertDoesNotChangeTopologyOrExport()
    {
        var inserted = Add("PodrobnyBodZPS", 'i', Point(0));
        inserted.IsDeleted = true;
        Assert.That(Context().Items, Is.Empty);
        Assert.That(Context().TargetItems, Is.Empty);
        Assert.That(Check(new DtmTopologyGroupRule()).Status, Is.EqualTo(DtmRuleStatus.Passed));
    }

    [Test]
    public void ValidationIsRepeatableAndDoesNotChangeReferenceState()
    {
        var line = Add("Plot", 'r', Line(P(0), P(10)));
        var validator = new DtmValidator(_main);
        var original = Context().OriginalItems.Single().Points.Select(p => p.X).ToArray();
        var first = validator.Validate().Rules.Select(r => r.Status).ToArray();
        Assert.That(validator.Validate().Rules.Select(r => r.Status), Is.EqualTo(first));
        Assert.That(line.ZapisObjektu, Is.EqualTo('r'));
        Assert.That(line.IsDeleted, Is.False);
        Assert.That(Context().OriginalItems.Single().Points.Select(p => p.X), Is.EqualTo(original));
        line.IsDeleted = true;
        Assert.That(Context().TargetItems, Is.Empty);
    }

    [Test]
    public void CoordinateChecksStillValidateExportedDeletesAndIgnoreReferences()
    {
        Add("PodrobnyBodZPS", 'r', Point(0));
        Assert.That(Check(new DtmCoordinatesRule()).Issues, Is.Empty);
        Add("PodrobnyBodZPS", 'd', Point(1));
        Assert.That(Check(new DtmCoordinatesRule()).Status, Is.EqualTo(DtmRuleStatus.Failed));
    }

    [Test]
    public void OldDefectsDoNotConsumeTheIssueLimit()
    {
        for (var i = 0; i < 510; i++) Add("Plot", 'r', Line(P(i * 10), P(i * 10 + 1)));
        Add("Plot", 'i', Line(P(10000), P(10001)));
        var issues = Check(new DtmVertexPodrobnyBodRule()).Issues;
        Assert.That(issues, Has.Count.EqualTo(2));
        Assert.That(issues.All(i => i.ObjectName.EndsWith("#511")), Is.True);
    }

    [Test]
    public void MovingDetailedPointToAnotherLevelBreaksItsOldDependency()
    {
        var point = Add("PodrobnyBodZPS", 'r', Point(0));
        Add("Plot", 'r', Line(P(0), P(10)));
        point.ZapisObjektu = 'u';
        point.SpolecneAtributyZPS.UrovenUmisteniObjektuZPS = 1;
        Assert.That(Check(new DtmVertexPodrobnyBodRule()).Issues, Has.Count.EqualTo(1));
        Assert.That(Context().OriginalItems.First().Level, Is.Zero);
    }

    [Test]
    public void RepairingAnOriginallyInvalidAreaDoesNotProduceAnIssue()
    {
        Add("BudovaPlocha", 'r', Area());
        Add("BudovaDefinicniBod", 'i', Point(5, 5));
        Assert.That(Check(new DtmAreaDefinitionPointRule()).Issues, Is.Empty);
    }

    [Test]
    public void AnExistingExcessOfDefinitionPointsIsReportedOnlyWhenWorsened()
    {
        Add("BudovaPlocha", 'r', Area());
        Add("BudovaDefinicniBod", 'r', Point(4, 4));
        Add("BudovaDefinicniBod", 'r', Point(5, 5));
        Assert.That(Check(new DtmAreaDefinitionPointRule()).Issues, Is.Empty);
        Add("BudovaDefinicniBod", 'i', Point(6, 6));
        Assert.That(Check(new DtmAreaDefinitionPointRule()).Issues, Has.Count.EqualTo(1));
    }

    [Test]
    public void ThirdDefinitionPointDuplicateIncludesTheChangedObject()
    {
        Add("BudovaDefinicniBod", 'r', Point(5, 5));
        Add("BudovaDefinicniBod", 'r', Point(5, 5));
        Add("BudovaDefinicniBod", 'u', Point(5, 5));
        Assert.That(Check(new DtmPointDuplicityRule()).Issues.Single().ObjectName, Does.EndWith("#3"));
    }

    [Test]
    public void ChangedAreaIsCheckedAgainstReferenceArea()
    {
        Add("BudovaPlocha", 'r', Area());
        var overlapping = Area();
        foreach (var p in overlapping.Points) { p.X += 5; p.Y += 5; }
        Add("BudovaPlocha", 'i', overlapping);
        Assert.That(Check(new DtmAreaCollisionRule()).Status, Is.EqualTo(DtmRuleStatus.Failed));
    }

    [Test]
    public void ValidationRefreshesTargetCollectionsAfterDeletionAndRestoration()
    {
        Add("PodrobnyBodZPS", 'r', Point(0));
        var line = Add("Plot", 'r', Line(P(0), P(10)));
        var validator = new DtmValidator(_main);
        DtmRuleResult Solitary() => validator.Validate().Rules.SelectMany(r => r.SubResults).Single(r => r.Code == "3.11");
        Assert.That(Solitary().Issues, Is.Empty);
        line.IsDeleted = true;
        Assert.That(Solitary().Issues, Has.Count.EqualTo(1));
        line.IsDeleted = false;
        Assert.That(Solitary().Issues, Is.Empty);
    }
}
