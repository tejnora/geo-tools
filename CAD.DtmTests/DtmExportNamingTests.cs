using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using CAD.DTM;
using GeoBase;
using GeoBase.Localization;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA), NonParallelizable]
public class DtmExportNamingTests
{
    [OneTimeSetUp]
    public void Initialize()
    {
        SingletonsBase.Registry.SuppressRegistry = true;
        var culture = System.Globalization.CultureInfo.GetCultureInfo("cs-CZ");
        LanguageDictionary.RegisterDictionary(culture, new XmlLanguageDictionary(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Languages", "cs-CZ.xml"), string.Empty));
        LanguageContext.Instance.Culture = culture;
    }

    static DtmExportCtx ValidContext() => new DtmExportCtx
    {
        OutputDirectory = TestContext.CurrentContext.WorkDirectory,
        UidSubjektu = "SUBJ-00000001", PrefixKraje = "CZ010", Cgad = "8"
    };

    [TestCase("8", "00008")]
    [TestCase("00008", "00008")]
    [TestCase("12345", "12345")]
    [TestCase("0", "00000")]
    public void PreparesExactOutputPath(string input, string expected)
    {
        var ctx = ValidContext();
        ctx.Cgad = input;
        Assert.That(ctx.PrepareOutputFile(), Is.True);
        Assert.That(ctx.GeneratedFileName, Is.EqualTo($"SUBJ-00000001_CZ010_{expected}_JVF.jvf.xml"));
        Assert.That(ctx.FileName, Is.EqualTo(Path.Combine(ctx.OutputDirectory, ctx.GeneratedFileName)));
    }

    [Test]
    public void RegionsMatchMethodology()
    {
        Assert.That(DtmExportCtx.Regions.Select(r => r.Key), Is.EqualTo(new[]
        {
            "CZ010", "CZ020", "CZ031", "CZ032", "CZ041", "CZ042", "CZ051",
            "CZ052", "CZ053", "CZ063", "CZ064", "CZ071", "CZ072", "CZ080"
        }));
        foreach (var region in DtmExportCtx.Regions)
        {
            var ctx = ValidContext();
            ctx.PrefixKraje = region.Key;
            Assert.That(ctx.PrepareOutputFile(), Is.True);
            Assert.That(ctx.GeneratedFileName, Does.Contain("_" + region.Key + "_"));
            Assert.That(region.Value, Does.Contain(region.Key));
        }
    }

    [TestCase("UidSubjektu", "")]
    [TestCase("UidSubjektu", "SUBJ-1234567")]
    [TestCase("UidSubjektu", "SUBJ-123456789")]
    [TestCase("UidSubjektu", "SUBJ-00000001\n")]
    [TestCase("PrefixKraje", "")]
    [TestCase("PrefixKraje", "CZ999")]
    [TestCase("Cgad", "")]
    [TestCase("Cgad", "123456")]
    [TestCase("Cgad", "-1")]
    [TestCase("Cgad", "abc")]
    [TestCase("Cgad", "8\n")]
    [TestCase("OutputDirectory", "")]
    public void InvalidInputPreventsConfirmation(string property, string value)
    {
        var ctx = ValidContext();
        ctx.FileName = "unchanged.xml";
        typeof(DtmExportCtx).GetProperty(property).SetValue(ctx, value);
        Assert.That(ctx.PrepareOutputFile(), Is.False);
        Assert.That(((IDataErrorInfo)ctx)[property], Is.Not.Empty);
        Assert.That(ctx.FileName, Is.EqualTo("unchanged.xml"));
    }

    [Test]
    public void MissingDirectoryAndFilePathsAreRejected()
    {
        var ctx = ValidContext();
        ctx.OutputDirectory = Path.Combine(ctx.OutputDirectory, Guid.NewGuid().ToString("N"));
        Assert.That(ctx.PrepareOutputFile(), Is.False);
        ctx.OutputDirectory = typeof(DtmExportCtx).Assembly.Location;
        Assert.That(ctx.PrepareOutputFile(), Is.False);
    }

    [Test]
    public void PreviewNotifiesAndValidationRecovers()
    {
        var ctx = ValidContext();
        var changes = new List<string>();
        ctx.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
        ctx.Cgad = "invalid";
        Assert.That(ctx.GeneratedFileName, Is.Empty);
        Assert.That(ctx.PrepareOutputFile(), Is.False);
        ctx.Cgad = "9";
        Assert.That(ctx.PrepareOutputFile(), Is.True);
        Assert.That(ctx.GeneratedFileName, Does.Contain("_00009_"));
        Assert.That(changes, Does.Contain(nameof(ctx.GeneratedFileName)));
        Assert.That(changes, Does.Contain(nameof(ctx.OutputPath)));
    }
}
