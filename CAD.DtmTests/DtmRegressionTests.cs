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
using CAD.DTM.Elements.GUI;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmRegressionTests
{
    [TestCase("SUBJ-00006299_CZ053_00003.jvf.xml")]
    [TestCase("vydej_zps_ref.jvf.xml")]
    [TestCase("vydej_zps_ref_jvf_1_5.jvf.xml")]
    [TestCase("jvf1502_ukazkaZPS.jvf.xml")]
    [TestCase("jvf1502_ukazka_DI.jvf.xml")]
    [TestCase("jvf1502_ukazka_TI.jvf.xml")]
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
    public void Jvf15IssueCanBeEditedAndExportedAsValidGad()
    {
        // Real JVF DTM 1.5.0.2 issue of ZPS: delete one reference object, add a new point, export GAD DTM.
        var main = new DtmMain();
        new DtmImporter(main).ParseFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", "vydej_zps_ref_jvf_1_5.jvf.xml"));
        var lines = main.getElementsGroup("HraniceStavby").GetElementGroups().Cast<DtmElement>().ToList();
        Assert.That(lines.All(e => e.ZapisObjektu == 'r'), Is.True);
        lines[0].IsDeleted = true;
        var definitionPoint = main.GetElementGroups().SelectMany(g => g.Value.GetElementGroups()).Cast<DtmElement>().First(e => e.ElementType == DtmElementType.DefinicniBod);
        definitionPoint.IsDeleted = true;
        var point = DtmConfigurationSingleton.Instance.CreateType("PodrobnyBodZPS");
        foreach (var (path, value) in new[] { ("UrovenUmisteniObjektuZPS", "0"), ("TridaPresnostiPoloha", "3"), ("TridaPresnostiVyska", "3"), ("ZpusobPorizeniPB_ZPS", "1"), ("CisloBodu", "1001") })
            point.SchemaAttributes.SetText(path, value);
        point.Geometry = new DtmGeometryGroup { Geometries = new List<IDtmGeometry> { new DtmPointGeometry { Point = new DtmPoint { X = -1034180, Y = -628690, Z = 268.5 } } } };
        main.AddElementIfNotExist("PodrobnyBodZPS", point);

        var filename = Path.Combine(TestContext.CurrentContext.WorkDirectory, "gad-from-jvf15.xml");
        new DtmExporter(main).CreateFile(new DtmExportCtx
        {
            FileName = filename, NazevZakazky = "Test", Zpracovatel = "Test", OrganizaceZpracovatele = "Test", AZI = "1234",
            CisloOvereni = "1/2026", DatumMereni = new DateTime(2026, 11, 1), DatumZpracovani = new DateTime(2026, 11, 2), DatumOvereni = new DateTime(2026, 11, 3)
        });
        var document = XDocument.Load(filename);
        var schemas = new System.Xml.Schema.XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        schemas.Add("objtyp", Path.Combine(DtmSchema.XsdDirectory, "index", "index_data.xsd"));
        var errors = new System.Collections.Generic.List<string>();
        document.Validate(schemas, (_, e) => errors.Add(e.Message));
        Assert.That(errors, Is.Empty, string.Join(Environment.NewLine, errors));
        var records = document.Descendants().Where(e => e.Name.LocalName.StartsWith("ZaznamObjektu") && e.Name.LocalName != "ZaznamyObjektu").Select(e => e.Name.LocalName).ToList();
        Assert.That(records, Is.EquivalentTo(new[] { "ZaznamObjektuDel", "ZaznamObjektuDel", "ZaznamObjektuIns" }));
        Assert.That(document.Descendants(XName.Get("DoprovodneInformaceGAD", "dopinf")).Count(), Is.EqualTo(1));

        var copy = new DtmMain();
        new DtmImporter(copy).ParseFile(filename);
        var all = copy.GetElementGroups().SelectMany(g => g.Value.GetElementGroups()).Cast<DtmElement>().ToList();
        Assert.That(all.Select(e => e.ZapisObjektu), Is.EquivalentTo(new[] { 'd', 'd', 'i' }));
    }

    [TestCase("jvf1502_ukazkaZPS.jvf.xml")]
    [TestCase("jvf1502_ukazka_DI.jvf.xml")]
    [TestCase("jvf1502_ukazka_TI.jvf.xml")]
    public void Jvf15SampleObjectsCanBeExportedAsValidGad(string filename)
    {
        // Official JVF DTM 1.5.0.2 samples (ZPS area with hole and outline, DI and TI objects): every object exported as ZaznamObjektuDel.
        var main = new DtmMain();
        new DtmImporter(main).ParseFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", filename));
        var elements = main.GetElementGroups().SelectMany(g => g.Value.GetElementGroups()).Cast<DtmElement>().ToList();
        Assert.That(elements, Is.Not.Empty);
        // Reference objects are deleted (ZaznamObjektuDel); objects of an update file (ZaznamObjektuIns) are exported again.
        foreach (var element in elements.Where(e => e.ZapisObjektu == 'r')) element.IsDeleted = true;
        var output = Path.Combine(TestContext.CurrentContext.WorkDirectory, "gad-delete-" + filename);
        new DtmExporter(main).CreateFile(new DtmExportCtx
        {
            FileName = output, NazevZakazky = "Test", Zpracovatel = "Test", OrganizaceZpracovatele = "Test", AZI = "1234",
            CisloOvereni = "1/2026", DatumMereni = new DateTime(2026, 11, 1), DatumZpracovani = new DateTime(2026, 11, 2), DatumOvereni = new DateTime(2026, 11, 3)
        });
        var document = XDocument.Load(output);
        var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        schemas.Add("objtyp", Path.Combine(DtmSchema.XsdDirectory, "index", "index_data.xsd"));
        var errors = new List<string>();
        document.Validate(schemas, (_, e) => errors.Add(e.Message));
        Assert.That(errors, Is.Empty, string.Join(Environment.NewLine, errors));
        var copy = new DtmMain();
        new DtmImporter(copy).ParseFile(output);
        var reimported = copy.GetElementGroups().SelectMany(g => g.Value.GetElementGroups()).Cast<DtmElement>().ToList();
        Assert.That(reimported.Count, Is.EqualTo(elements.Count));
        Assert.That(reimported.SelectMany(e => e.Geometry.Geometries).Count(), Is.EqualTo(elements.SelectMany(e => e.Geometry.Geometries).Count()));
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
