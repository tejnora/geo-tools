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
using CAD.DTM.Gui;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmCoverageTests
{
    static IEnumerable<string> ObjectNames => DtmSchema.Instance.Objects.Keys;
    static readonly Lazy<XmlSchemaSet> Schemas = new(() =>
    {
        var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        schemas.Add("objtyp", Path.Combine(DtmSchema.XsdDirectory, "index", "index_data.xsd"));
        schemas.Compile();
        return schemas;
    });

    [Test]
    public void InventoryMatchesEveryXsdAndAllSymbolsAreDeployed()
    {
        var config = DtmConfigurationSingleton.Instance;
        var files = Directory.GetFiles(Path.Combine(DtmSchema.XsdDirectory, "objekty"), "*.xsd");
        Assert.That(files, Has.Length.EqualTo(358));
        Assert.That(config.ElementSetting.Keys, Is.EquivalentTo(ObjectNames));
        Assert.That(config.Diagnostics, Is.Empty, string.Join("\n", config.Diagnostics));
        foreach (var file in files)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var schema = XDocument.Load(file);
            var name = (string)schema.Root.Element(xs + "element").Attribute("name");
            Assert.That(config.ElementSetting[name].XmlNamespace, Is.EqualTo((string)schema.Root.Attribute("targetNamespace")));
        }
    }

    [TestCaseSource(nameof(ObjectNames))]
    public void EveryObjectCreatesImportsAndExportsValidAttributesAndGeometry(string name)
    {
        var config = DtmConfigurationSingleton.Instance;
        var element = config.CreateType(name);
        Assert.That(element, Is.Not.TypeOf<DtmElementGeneric>());
        var attrs = element.SchemaAttributes;
        Assert.That(attrs, Is.Not.Null);
        foreach (var field in attrs.Fields()) attrs.SetText(field.Path, Example(field.Attribute));
        using (var attributesWriter = new XmlExporter())
        {
            attributesWriter.BeginElement("atr", "Attributes");
            element.ExportAttributesToDtm(attributesWriter);
            attributesWriter.EndElement();
            var directAttributes = XElement.Parse(attributesWriter.ToString());
            Assert.That(directAttributes.Elements().Select(e => e.Name.LocalName), Is.EqualTo(attrs.ToXml(false).Select(e => e.Name.LocalName)));
        }
        Assert.That(attrs.Validate(), Is.Empty, name + "\n" + string.Join("\n", attrs.Validate()));
        element.Geometry = new DtmGeometryGroup { Geometries = attrs.Definition.Geometries.Select(ExampleGeometry).ToList() };
        var group = config.CreateGroup(name);
        group.AddElement(element);
        using var writer = new XmlExporter();
        group.ExportToDtm(writer);
        var xml = XDocument.Parse(writer.ToString());
        var errors = new List<string>();
        xml.Validate(Schemas.Value, (_, e) => errors.Add(e.Message));
        Assert.That(errors, Is.Empty, name + "\n" + string.Join("\n", errors));
        var main = new DtmMain();
        XNamespace objtyp = "objtyp";
        var envelope = new XDocument(new XElement(objtyp + "JVFDTM", new XElement(objtyp + "DataJVFDTM", new XElement(objtyp + "Data", xml.Root))));
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(envelope.ToString());
        new DtmImporter(main).ParseDocument(xmlDoc);
        var imported = (DtmElement)main.getElementsGroup(name).GetElementGroups().Single();
        var before = new XElement("Attributes", attrs.ToXml(), attrs.RecordXml());
        var after = new XElement("Attributes", imported.SchemaAttributes.ToXml(), imported.SchemaAttributes.RecordXml());
        // Namespace declarations may move during serialization; compare expanded XML names and values.
        foreach (var root in new[] { before, after })
            foreach (var node in root.DescendantsAndSelf()) node.Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
        Assert.That(XNode.DeepEquals(before, after), Is.True, name + "\n" + after);
        Assert.That(element.CreateDrawObject(), Is.Not.Null);
        Assert.That(new CAD.DTM.Validation.Rules.DtmGeometryMatchesElementRule().Validate(new CAD.DTM.Validation.DtmValidationContext(main, null, CAD.DTM.Validation.DtmValidationSettings.Default)).Issues, Is.Empty);
    }

    internal static string Example(DtmAttributeDefinition definition)
    {
        if (definition.Values.Count > 0) return definition.Values.Keys.First();
        return definition.Type switch
        {
            "boolean" => "false",
            "date" => "2026-10-21",
            "dateTime" => "2026-10-21T10:00:00Z",
            "integer" or "int" or "long" or "nonNegativeInteger" or "positiveInteger" or "unsignedInt" => "1",
            "decimal" or "double" or "float" => "1.25",
            "complex" => "<atr:OblastObjektuKI xmlns:atr='atr' xmlns:gml='http://www.opengis.net/gml/3.2'><gml:surfaceProperty><gml:Polygon gml:id='KI1' srsName='EPSG:5514' srsDimension='2'><gml:exterior><gml:LinearRing><gml:posList>0 0 10 0 0 10 0 0</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceProperty></atr:OblastObjektuKI>",
            _ => "test"
        };
    }

    internal static IDtmGeometry ExampleGeometry(string property)
    {
        var points = new List<DtmPoint> { new() { X = -500000, Y = -1000000, Z = 100 }, new() { X = -500010, Y = -1000000, Z = 101 }, new() { X = -500010, Y = -1000010, Z = 102 }, new() { X = -500000, Y = -1000000, Z = 100 } };
        return property switch
        {
            "pointProperty" => new DtmPointGeometry { Id = "P1", Point = points[0] },
            "curveProperty" => new DtmCurveGeometry { Id = "L1", Points = points },
            "multiCurveProperty" => new DtmMultiCurveGeometry { Id = "M1", Points = points },
            "surfaceProperty" => new DtmSurfaceGeometry { Id = "S1", Points = points },
            _ => throw new ArgumentException(property)
        };
    }

    [Test]
    public void MissingRequiredAttributesAndInvalidValuesAreReported()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("Plot");
        Assert.That(element.SchemaAttributes.Validate(), Is.Not.Empty);
        foreach (var field in element.SchemaAttributes.Fields()) element.SchemaAttributes.SetText(field.Path, Example(field.Attribute));
        Assert.That(element.SchemaAttributes.Validate(), Is.Empty);
        element.SchemaAttributes.SetText("DruhPlotu", "99999");
        element.SchemaAttributes.SetText("HraniceJinehoObjektu", "perhaps");
        Assert.That(element.SchemaAttributes.Validate().Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void OptionalZeroFalseAndEmptyStringRemainDistinctFromMissing()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("Plot");
        var attributes = element.SchemaAttributes;
        attributes.SetText("SpolecneAtributyVsechObjektu/IDExterni", null);
        attributes.SetText("SpolecneAtributyVsechObjektu/PopisObjektu", "");
        attributes.SetText("HraniceJinehoObjektu", "false");
        Assert.That(attributes.Values.ContainsKey("SpolecneAtributyVsechObjektu/IDExterni"), Is.False);
        Assert.That(attributes.Values["SpolecneAtributyVsechObjektu/PopisObjektu"], Is.Empty);
        Assert.That(attributes.Values["HraniceJinehoObjektu"], Is.EqualTo("false"));
    }

    [TestCase("ParkovisteOdstavnaPlocha", "ParkovisteOdstavnaPlochaPlocha")]
    [TestCase("OPObjektuElektrickeSite", "OPElektrickeSite")]
    public void LegacyNamesResolveToCanonicalObjects(string alias, string canonical)
    {
        Assert.That(DtmConfigurationSingleton.Instance.CreateGroup(alias).Name, Is.EqualTo(canonical));
        Assert.That(DtmConfigurationSingleton.Instance.CreateType(alias).Options.Name, Is.EqualTo(canonical));
    }
}
