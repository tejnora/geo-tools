using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using CAD.DTM;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Elements.GUI;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmRegressionTests
{
    [TestCase("SUBJ-00006299_CZ053_00003.jvf.xml")]
    [TestCase("vydej_zps_ref.jvf.xml")]
    public void SampleFilesImportAndPreserveAllAttributeValues(string filename)
    {
        var main = new DtmMain();
        new DtmImporter(main).ParseFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", filename));
        var count = 0;
        foreach (var group in main.GetElementGroups())
            foreach (var element in group.Value.GetElementGroups().Cast<DtmElement>())
            {
                var attributes = new XElement("AtributyObjektu", element.SchemaAttributes.ToXml());
                var copy = DtmConfigurationSingleton.Instance.CreateType(group.Key);
                var doc = new XmlDocument(); doc.LoadXml(attributes.ToString());
                copy.ImportDtmAttributes(doc.DocumentElement);
                copy.SchemaAttributes.ApplyToLegacyValues();
                Assert.That(XNode.DeepEquals(attributes, new XElement("AtributyObjektu", copy.SchemaAttributes.ToXml())), Is.True, group.Key);
                Assert.That(element.CreateDrawObject(), Is.Not.Null, group.Key);
                count++;
            }
        Assert.That(count, Is.GreaterThan(0));
    }

    [Test]
    public void LegacyPropertyEditsAndSchemaEditsStayInSync()
    {
        var fence = (DtmPlotElement)DtmConfigurationSingleton.Instance.CreateType("Plot");
        fence.SchemaAttributes.SetText("HraniceJinehoObjektu", "false");
        fence.HraniceJinehoObjektu = true;
        fence.SchemaAttributes.SetText("SpolecneAtributyVsechObjektu/PopisObjektu", "changed");
        Assert.That(fence.SchemaAttributes.Get<bool?>("HraniceJinehoObjektu"), Is.True);
        fence.SchemaAttributes.SetText("HraniceJinehoObjektu", "false");
        Assert.That(fence.HraniceJinehoObjektu, Is.False);
        var point = DtmConfigurationSingleton.Instance.CreateType("PodrobnyBodZPS");
        point.SchemaAttributes.SetText("TridaPresnostiPoloha", "5");
        Assert.That(point.SpolecneAtributyZPS.TridaPresnostiPoloha, Is.EqualTo(5));
        fence.ApplySelectedSetting("Nezjisteno");
        Assert.That(fence.SchemaAttributes.Get<string>("DruhPlotu"), Is.EqualTo("99"));
    }

    [Test]
    public void LoadingEditorsDoesNotChangeDefinitionPointAttributes()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("BudovaDefinicniBod");
        foreach (var field in element.SchemaAttributes.Fields()) element.SchemaAttributes.SetText(field.Path, DtmCoverageTests.Example(field.Attribute));
        element.SchemaAttributes.SetText("SpolecneAtributyObjektuDefinicnichBodu/UrovenUmisteniObjektuZPS", "2");
        var before = new XElement("Attributes", element.SchemaAttributes.ToXml());
        element.InitGUICustomProperties(new DtmCustomElementProperties());
        foreach (var field in element.SchemaAttributes.Fields())
            new DtmSchemaCustomProperty(element.SchemaAttributes, field.Path, field.Attribute, field.Required).GetEditControl();
        Assert.That(XNode.DeepEquals(before, new XElement("Attributes", element.SchemaAttributes.ToXml())), Is.True);
    }

    [Test]
    public void PointSymbolChangesWithItsEnumerationValue()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("PodperneZarizeni");
        var option = element.Options;
        Assert.That(option.GraphicAttribute, Is.EqualTo("TypPodpernehoZarizeni"));
        element.SchemaAttributes.SetText(option.GraphicAttribute, "1");
        var first = element.ResolveGraphicElement(option, DtmGraphicElementScaleEnum._500);
        element.SchemaAttributes.SetText(option.GraphicAttribute, "2");
        var second = element.ResolveGraphicElement(option, DtmGraphicElementScaleEnum._500);
        Assert.That(first.Symbol, Is.Not.SameAs(second.Symbol));
    }

    [Test]
    public void ReferenceAndDeletionSemanticsArePreserved()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("Plot");
        element.ZapisObjektu = 'r';
        Assert.That(element.ExportToOutput, Is.False);
        element.IsDeleted = true;
        Assert.That(element.ExportToOutput, Is.True);
        Assert.That(element.EvaluateZapisObjektuForExportToDtm(), Is.EqualTo("d"));
    }
}
