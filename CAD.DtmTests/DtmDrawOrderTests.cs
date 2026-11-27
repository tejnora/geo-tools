using System.Linq;
using CAD.DTM;
using CAD.DTM.Configuration;
using NUnit.Framework;

namespace CAD.DtmTests;

/// <summary>Opaque area fills must not cover points: areas are drawn first, points last.</summary>
[TestFixture]
public class DtmDrawOrderTests
{
    [Test]
    public void DefinitionPointIsDrawnAfterAreaRegardlessOfGroupOrder()
    {
        var main = new DtmMain();
        foreach (var name in new[] { "BudovaDefinicniBod", "PodrobnyBodZPS", "BudovaPlocha" })
        {
            var element = DtmConfigurationSingleton.Instance.CreateType(name);
            element.Geometry = new DtmGeometryGroup { Geometries = new System.Collections.Generic.List<IDtmGeometry> { name == "BudovaPlocha"
                ? new DtmSurfaceGeometry { Points = new() { new() { X = 0 }, new() { X = 10 }, new() { X = 10, Y = 10 }, new() { X = 0 } }, SrsDimension = 3 }
                : new DtmPointGeometry { Point = new DtmPoint { X = 5, Y = 5 }, SrsDimension = 3 } } };
            main.AddElementIfNotExist(name, element);
        }

        var order = new DtmDrawingLayerMain(main).LayersInDrawOrder.Select(l => l.Name).ToList();

        Assert.That(order, Is.EqualTo(new[] { "BudovaPlocha", "BudovaDefinicniBod", "PodrobnyBodZPS" }));
    }
}
