using System.Linq;
using CAD.DTM.Configuration;
using NUnit.Framework;

namespace CAD.DtmTests;

/// <summary>SpolecneAtributyVsechObjektu are hidden for new objects and never validated by the application (the server checks them).</summary>
[TestFixture]
public class NewElementCommonAttributesTests
{
    [TestCase("IdentickyBod")]
    [TestCase("PodrobnyBodZPS")]
    public void NewElementHidesAndSkipsExportFilledCommonAttributes(string group)
    {
        var element = DtmConfigurationSingleton.Instance.CreateType(group);
        element.ZapisObjektu = 'i';
        var attributes = element.SchemaAttributes;

        Assert.That(attributes.EditingContext, Is.EqualTo("Ins"));
        var paths = attributes.Fields(attributes.EditingContext).Select(f => f.Path).ToList();
        Assert.That(paths, Has.None.EqualTo("SpolecneAtributyVsechObjektu/ID"));
        Assert.That(paths, Has.None.EqualTo("SpolecneAtributyVsechObjektu/IDZmeny"));
        Assert.That(paths, Has.None.EqualTo("SpolecneAtributyVsechObjektu/VkladOsoba"));
        Assert.That(attributes.Validate(attributes.EditingContext), Has.None.StartsWith("SpolecneAtributyVsechObjektu/"));
    }

    [Test]
    public void ReferenceElementKeepsCompleteCommonAttributes()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("IdentickyBod");
        element.ZapisObjektu = 'r';
        var attributes = element.SchemaAttributes;

        Assert.That(attributes.EditingContext, Is.Null);
        Assert.That(attributes.Fields(attributes.EditingContext).Select(f => f.Path), Has.Member("SpolecneAtributyVsechObjektu/ID"));
        Assert.That(attributes.Validate(attributes.EditingContext), Has.None.StartsWith("SpolecneAtributyVsechObjektu/"));
        Assert.That(attributes.Validate(attributes.EditingContext, includeCommon: true), Has.Some.StartsWith("SpolecneAtributyVsechObjektu/ID"));
    }
}
