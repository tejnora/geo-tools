using System.Drawing;
using System.Threading;
using System.Windows;
using CAD.DTM;
using GeoBase.Utils;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmDefinitionPointToolDrawTests
{
    [Test]
    public void InsertedDefinitionPointIsDrawnByLayer()
    {
        using var canvas = new DrawingCanvas();
        var layer = new DtmDrawingLayerMain(new DtmMain()) { DtmPointSelected = "BudovaDefinicniBod" };
        var tool = new DtmDrawingDefinitionPointElement();
        var drawing = (DtmDrawingDefinitionPointElement)tool.Clone();
        drawing.InitializeFromModel(new UnitPoint(0, 0), layer, null);
        drawing.OnMouseMove(canvas, new UnitPoint(200, 150));
        drawing.OnMouseDown(canvas, new UnitPoint(200, 150), null);
        layer.AddObject(drawing);

        layer.Draw(canvas, new Rect(0, 0, 640, 360));

        Assert.That(canvas.Bitmap.GetPixel(200, 150).ToArgb(), Is.Not.EqualTo(Color.White.ToArgb()));
    }
}
