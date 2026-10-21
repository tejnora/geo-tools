using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using CAD.DTM;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using GeoBase;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmFileExportTests
{
    [OneTimeSetUp]
    public void DisableUserSettings() => SingletonsBase.Registry.SuppressRegistry = true;

    [Test]
    public void ActualExporterProducesValidCompleteJvfAndReimportsIt()
    {
        var main = new DtmMain();
        foreach (var name in new[] { "Plot", "O6", "OPElektrickeSite", "BudovaPlocha" })
        {
            var element = DtmConfigurationSingleton.Instance.CreateType(name);
            foreach (var field in element.SchemaAttributes.Fields())
                element.SchemaAttributes.SetText(field.Path, DtmCoverageTests.Example(field.Attribute).Replace("KI1", "KI_" + name));
            element.Geometry = new DtmGeometryGroup { Geometries = element.SchemaAttributes.Definition.Geometries.Select(DtmCoverageTests.ExampleGeometry).ToList() };
            foreach (var geometry in element.Geometry.Geometries) geometry.Id = null;
            main.AddElementIfNotExist(name, element);
        }
        var filename = Path.Combine(TestContext.CurrentContext.WorkDirectory, "complete-jvf.xml");
        var context = new DtmExportCtx
        {
            FileName = filename, NazevZakazky = "Test DTM", Zpracovatel = "Test", OrganizaceZpracovatele = "Test",
            AZI = "1234", CisloOvereni = "1/2026", DatumMereni = new DateTime(2026, 10, 21),
            DatumZpracovani = new DateTime(2026, 10, 21), DatumOvereni = new DateTime(2026, 10, 21)
        };
        new DtmExporter(main).CreateFile(context);
        var document = XDocument.Load(filename);
        var errors = new List<string>();
        var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        schemas.Add("objtyp", Path.Combine(DtmSchema.XsdDirectory, "index", "index_data.xsd"));
        document.Validate(schemas, (_, e) => errors.Add(e.Message));
        Assert.That(errors, Is.Empty, string.Join("\n", errors));
        var validation = new CAD.DTM.Validation.Rules.DtmXsdValidationRule().Validate(new CAD.DTM.Validation.DtmValidationContext(main, context, CAD.DTM.Validation.DtmValidationSettings.Default));
        Assert.That(validation.Issues, Is.Empty, string.Join("\n", validation.Issues.Select(i => i.Message)));
        var copy = new DtmMain();
        new DtmImporter(copy).ParseFile(filename);
        Assert.That(copy.GetElementGroups().Count(), Is.EqualTo(4));
        Assert.That(copy.getElementsGroup("O6"), Is.Not.Null);
        Assert.That(((DtmElement)copy.getElementsGroup("OPElektrickeSite").GetElementGroups().Single()).SchemaAttributes.RecordXml().Count(), Is.EqualTo(1));
    }

    [Test]
    public void InvalidAttributesDoNotOverwriteAnExistingOutputFile()
    {
        var main = new DtmMain();
        var element = DtmConfigurationSingleton.Instance.CreateType("Plot");
        element.Geometry = new DtmGeometryGroup { Geometries = new[] { DtmCoverageTests.ExampleGeometry("curveProperty") } };
        element.Geometry.Geometries[0].Id = null;
        main.AddElementIfNotExist("Plot", element);
        var filename = Path.Combine(TestContext.CurrentContext.WorkDirectory, "unchanged-output.xml");
        File.WriteAllText(filename, "original");
        Assert.Throws<InvalidOperationException>(() => new DtmExporter(main).CreateFile(new DtmExportCtx { FileName = filename }));
        Assert.That(File.ReadAllText(filename), Is.EqualTo("original"));
    }
}
