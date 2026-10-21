using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using CAD.Canvas;
using GeoBase.Utils;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace CAD.DtmTests;

internal sealed class DrawingCanvas : ICanvas, IDisposable
{
    public Bitmap Bitmap { get; } = new(640, 360);
    public Graphics Graphics { get; }
    public DrawingCanvas() { Graphics = Graphics.FromImage(Bitmap); Graphics.Clear(Color.White); Graphics.SmoothingMode = SmoothingMode.AntiAlias; }
    public IModel DataModel => null;
    public IDrawObject CurrentObject => null;
    public Rectangle ClientRectangle => new(0, 0, Bitmap.Width, Bitmap.Height);
    public double SymbolScaleCoefficient => 1;
    public UnitPoint ScreenTopLeftToUnitPoint() => new(0, 0);
    public UnitPoint ScreenBottomRightToUnitPoint() => new(Bitmap.Width, Bitmap.Height);
    public Point ToScreen(UnitPoint point) => new(point.X, point.Y);
    public GraphicsPath ToScreen(GraphicsPath path) => (GraphicsPath)path.Clone();
    public double ToScreen(double value) => value;
    public double ToScreenWithoutZoom(double value) => value * 24;
    public double ToUnit(double value) => value;
    public UnitPoint ToUnit(Point point) => new(point.X, point.Y);
    public double getZoom() => 0.002;
    public void Invalidate() { }
    public Pen CreatePen(Color color, float width) => new(color, Math.Max(1, width * 3000));
    public Font CreateFont(string name, float size, FontStyle style) => new(name, size, style);
    public void DrawLine(ICanvas canvas, Pen pen, UnitPoint a, UnitPoint b) => Graphics.DrawLine(pen, (float)a.X, (float)a.Y, (float)b.X, (float)b.Y);
    public void DrawLine(ICanvas canvas, Pen pen, Point a, Point b) => Graphics.DrawLine(pen, (float)a.X, (float)a.Y, (float)b.X, (float)b.Y);
    public void DrawArc(ICanvas canvas, Pen pen, UnitPoint center, float radius, float begin, float angle) => Graphics.DrawArc(pen, (float)center.X - radius, (float)center.Y - radius, radius * 2, radius * 2, begin, angle);
    public void DrawPath(ICanvas canvas, Pen pen, GraphicsPath path) => Graphics.DrawPath(pen, path);
    public void FillPath(ICanvas canvas, Brush brush, GraphicsPath path) => Graphics.FillPath(brush, path);
    public Size MeasureString(ICanvas canvas, string text, Font font, Size size, StringFormat format, out int characters, out int lines)
    {
        var measured = Graphics.MeasureString(text, font);
        characters = text.Length; lines = 1; return new Size(measured.Width, measured.Height);
    }
    public void DrawPath(ICanvas canvas, Pen pen, PathImpl path, UnitPoint offset) => throw new NotSupportedException();
    public void DrawCircle(ICanvas canvas, Pen pen, Point point, float radius) => Graphics.DrawEllipse(pen, (float)point.X - radius, (float)point.Y - radius, radius * 2, radius * 2);
    public void DrawSymbol(ICanvas canvas, Image image, UnitPoint point, UnitPoint size)
    {
        var width = (float)(size.X * 24); var height = (float)(size.Y * 24);
        Graphics.DrawImage(image, (float)point.X - width / 2, (float)point.Y - height / 2, width, height);
    }
    public void Dispose() { Graphics.Dispose(); Bitmap.Dispose(); }
}
