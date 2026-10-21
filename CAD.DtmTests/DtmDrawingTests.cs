using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.DTM;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Gui;
using CAD.DrawTools;
using GeoBase.Utils;
using NUnit.Framework;

namespace CAD.DtmTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class DtmDrawingTests
{
    static IDrawObject Drawing(DtmElement element)
    {
        var drawing = element.CreateDrawObject();
        new DtmDrawingGroup(element.Options.Name, drawing);
        return drawing;
    }
    static List<DtmPoint> Ring(double x, double y, double width, double height) => new()
    {
        new() { X=x, Y=y, Z=100 }, new() { X=x+width, Y=y, Z=101 }, new() { X=x+width, Y=y+height, Z=102 },
        new() { X=x, Y=y+height, Z=103 }, new() { X=x, Y=y, Z=100 }
    };

    [Test]
    public void RendersZpsDiTiSymbolsAndPreservesPolygonHoles()
    {
        using var canvas = new DrawingCanvas();
        using var font = new Font("Arial", 10);
        var label = 0;
        foreach (var name in new[] { "PodrobnyBodZPS", "HeliportBod", "PodperneZarizeni", "PodperneZarizeni" })
        {
            var element = DtmConfigurationSingleton.Instance.CreateType(name);
            if (element.Options.GraphicAttribute != null) element.SchemaAttributes.SetText(element.Options.GraphicAttribute, label == 3 ? "2" : "1");
            var x = 65 + label * 150;
            element.Geometry = new DtmGeometryGroup { Geometries = new[] { new DtmPointGeometry { Point = new DtmPoint { X=x, Y=60 } } } };
            Drawing(element).Draw(canvas, Rect.Empty);
            canvas.Graphics.DrawString(label == 0 ? "ZPS / podrobný bod" : label == 1 ? "DI / heliport" : "TI / podpěrné zařízení " + (label - 1), font, Brushes.Black, x - 55, 95);
            label++;
        }
        var fence = DtmConfigurationSingleton.Instance.CreateType("Plot");
        fence.Geometry = new DtmGeometryGroup { Geometries = new[] { new DtmCurveGeometry { Points = new List<DtmPoint> { new() { X=35, Y=200 }, new() { X=150, Y=230 }, new() { X=235, Y=180 } } } } };
        Drawing(fence).Draw(canvas, Rect.Empty);
        canvas.Graphics.DrawString("ZPS / plot – základní čára", font, Brushes.Black, 35, 255);
        var building = DtmConfigurationSingleton.Instance.CreateType("BudovaPlocha");
        var polygon = new DtmSurfaceGeometry { Points = Ring(300, 145, 290, 160), InteriorRings = new() { Ring(390, 185, 90, 70) } };
        building.Geometry = new DtmGeometryGroup { Geometries = new[] { polygon } };
        var area = Drawing(building);
        area.Draw(canvas, Rect.Empty);
        canvas.Graphics.DrawString("ZPS / budova s otvorem", font, Brushes.Black, 350, 320);
        Assert.That(canvas.Bitmap.GetPixel(350, 210).ToArgb(), Is.Not.EqualTo(Color.White.ToArgb()));
        Assert.That(canvas.Bitmap.GetPixel(420, 210).ToArgb(), Is.EqualTo(Color.White.ToArgb()));
        Assert.That(area.PointInObject(canvas, new UnitPoint(350, 210)), Is.True);
        Assert.That(area.PointInObject(canvas, new UnitPoint(420, 210)), Is.False);
        canvas.Bitmap.Save(Path.Combine(TestContext.CurrentContext.WorkDirectory, "dtm-rendering.png"));
    }

    [Test]
    public void AreaToolCreatesBothSurfaceAndRequiredThreeDimensionalOutline()
    {
        using var canvas = new DrawingCanvas();
        var layer = new DtmDrawingLayerMain(new DtmMain()) { DtmAreaSelected = "BudovaPlocha" };
        DtmPodrobnyBodSnapPoint Snap(DtmPoint point)
        {
            var survey = DtmConfigurationSingleton.Instance.CreateType("PodrobnyBodZPS");
            survey.Geometry = new DtmGeometryGroup { Geometries = new[] { new DtmPointGeometry { Point = point } } };
            var drawing = Drawing(survey);
            return new DtmPodrobnyBodSnapPoint(canvas, drawing, new UnitPoint(point.X, point.Y));
        }
        var points = Ring(100, 100, 40, 40);
        var tool = new DtmDrawingPlochaElement();
        tool.InitializeFromModel(new UnitPoint(100, 100), layer, Snap(points[0]));
        foreach (var point in points.Skip(1).Take(3)) tool.OnMouseDown(canvas, new UnitPoint(point.X, point.Y), Snap(point));
        Assert.That(tool.OnFinish(), Is.EqualTo(DrawObjectState.Done));
        var geometries = tool.GetDtmElement.Geometry.Geometries;
        Assert.That(geometries.OfType<DtmSurfaceGeometry>().Single().SrsDimension, Is.EqualTo(2));
        var outline = geometries.OfType<DtmMultiCurveGeometry>().Single();
        Assert.That(outline.SrsDimension, Is.EqualTo(3));
        Assert.That(outline.Members[0].Points[2].Z, Is.EqualTo(points[2].Z));
    }

    [Test]
    public void PropertyPanelRendersAndLoadingItDoesNotChangeValues()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("PodperneZarizeni");
        foreach (var field in element.SchemaAttributes.Fields()) element.SchemaAttributes.SetText(field.Path, DtmCoverageTests.Example(field.Attribute));
        element.Geometry = new DtmGeometryGroup { Geometries = new[] { DtmCoverageTests.ExampleGeometry("pointProperty") } };
        var before = element.SchemaAttributes.Values.ToDictionary(p => p.Key, p => p.Value);
        var page = new DtmPropPage();
        page.Load(Drawing(element));
        var tabs = (TabControl)page.FindName("_tabControl"); tabs.SelectedIndex = 4;
        page.Measure(new System.Windows.Size(980, 760)); page.Arrange(new Rect(0, 0, 980, 760)); page.UpdateLayout();
        var bitmap = new RenderTargetBitmap(980, 760, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
        bitmap.Render(page);
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var file = File.Create(Path.Combine(TestContext.CurrentContext.WorkDirectory, "dtm-properties.png")); encoder.Save(file);
        Assert.That(element.SchemaAttributes.Values, Is.EquivalentTo(before));
    }

    [Test]
    public void EmptyRegionEditorAllowsAddingVerticesWithoutChangingTheOriginal()
    {
        var element = DtmConfigurationSingleton.Instance.CreateType("O6");
        var original = new DtmSurfaceGeometry { Points = new List<DtmPoint>() };
        var target = new List<IDtmGeometry> { original };
        var dialog = new DtmGeometryEditorDialog(element, target, new[] { "surfaceProperty" });
        var grid = ((DockPanel)dialog.Content).Children.OfType<DataGrid>().Single();
        var view = (System.ComponentModel.IEditableCollectionView)CollectionViewSource.GetDefaultView(grid.ItemsSource);
        Assert.That(view.CanAddNew, Is.True);
        var vertex = (DtmPoint)view.AddNew(); vertex.X = 10; vertex.Y = 20; view.CommitNew();
        Assert.That(original.Points, Is.Empty);
        dialog.Close();
    }
}
