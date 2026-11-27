using System.Drawing;
using CAD.Canvas;
using NUnit.Framework;

namespace CAD.DtmTests;

/// <summary>Black catalogue symbols (e.g. definition point ZPS_db.png) must stay visible on the black canvas background.</summary>
[TestFixture]
public class SymbolContrastTests
{
    [Test]
    public void DarkPixelsAreLightenedOnDarkBackgroundOnly()
    {
        using var symbol = new Bitmap(3, 1);
        symbol.SetPixel(0, 0, Color.Black);
        symbol.SetPixel(1, 0, Color.FromArgb(255, 200, 0, 0));
        symbol.SetPixel(2, 0, Color.Transparent);

        Assert.That(SymbolContrast.ForBackground(symbol, Color.White), Is.SameAs(symbol));
        var dark = (Bitmap)SymbolContrast.ForBackground(symbol, Color.Black);
        Assert.That(dark.GetPixel(0, 0).ToArgb(), Is.EqualTo(Color.White.ToArgb()));
        Assert.That(dark.GetPixel(1, 0).ToArgb(), Is.EqualTo(Color.FromArgb(255, 200, 0, 0).ToArgb()));
        Assert.That(dark.GetPixel(2, 0).A, Is.EqualTo(0));
        Assert.That(SymbolContrast.ForBackground(symbol, Color.Black), Is.SameAs(dark));
    }
}
