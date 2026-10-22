using System.Threading;
using System.Xml;
using System.Xml.Linq;
using CAD.DTM.Configuration;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmNewObjectDefaultsTests
{
    [Test]
    public void NewObjectsExportSurveyDefaults()
    {
        foreach (var name in DtmConfigurationSingleton.Instance.ElementSetting.Keys)
        {
            var element = DtmConfigurationSingleton.Instance.CreateType(name);
            var attributes = new XElement("Attributes", element.SchemaAttributes.ToXml());
            foreach (var node in attributes.Descendants())
            {
                var expected = node.Name.LocalName switch
                {
                    "UrovenUmisteniObjektuZPS" or "UrovenUmisteniObjektuTI" or "UrovenUmisteniObjektuDI" => "0",
                    "TridaPresnostiPoloha" or "TridaPresnostiVyska" => "3",
                    "ZpusobPorizeniZPS" or "ZpusobPorizeniPB_ZPS" or "ZpusobPorizeniTI" or "ZpusobPorizeniDI" => "1",
                    _ => null
                };
                if (expected != null)
                    Assert.That(node.Value, Is.EqualTo(expected), name + ": " + node.Name.LocalName);
            }
        }
    }

    [Test]
    public void ImportReplacesDefaultsWithSourceValuesAndPresence()
    {
        var point = DtmConfigurationSingleton.Instance.CreateType("PodrobnyBodZPS");
        var doc = new XmlDocument();
        doc.LoadXml("<Attributes><TridaPresnostiPoloha>5</TridaPresnostiPoloha></Attributes>");
        point.ImportDtmAttributes(doc.DocumentElement);
        Assert.That(point.SchemaAttributes.Get<int?>("TridaPresnostiPoloha"), Is.EqualTo(5));
        Assert.That(point.SchemaAttributes.Values.Keys, Is.EquivalentTo(new[] { "TridaPresnostiPoloha" }));
    }
}