using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.DTM.Elements;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;

namespace CAD.DTM
{
    class DtmDrawingPointElement
        : DrawObjectBase
        , IDrawObject
        , IDtmDrawingElement
    {
        readonly DtmElement _element;
        public DtmPointGeometry PointGeometry { get; }
        readonly UnitPoint _point;
        const int ThresholdPixel = 6;
        readonly Font _font = new Font("Arial", 12F, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel, 0);

        public DtmDrawingPointElement(DtmElement element)
        {
            _element = element;
            PointGeometry = (DtmPointGeometry)element.Geometry;
            _point = new UnitPoint(PointGeometry.Point.X, PointGeometry.Point.Y);
        }
        public string Id { get; }
        public IDrawObject Clone()
        {
            throw new NotImplementedException();
        }

        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            var rect = GetBoundingRect(canvas);
            var res = rect.Contains(point.Point);
            return res;
        }

        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            return rect.Contains(_point.X, _point.Y);
        }

        public void Draw(ICanvas canvas, Rect unitrect)
        {
            var pen = canvas.CreatePen(Group.Options.Color, Group.Options.Width);
            pen.EndCap = LineCap.Flat;
            pen.StartCap = LineCap.Flat;
            var p1 = canvas.ToScreen(_point);
            var p2 = p1;
            p1.X -= 5;
            p2.X += 5;
            canvas.DrawLine(canvas, pen, p1, p2);
            p1.X += 5;
            p2.X -= 5;
            p1.Y -= 5;
            p2.Y += 5;
            canvas.DrawLine(canvas, pen, p1, p2);

            var f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            p1.X += 10;
            p1.Y -= 10;
            var brush = new SolidBrush(Group.Options.Color);
            canvas.Graphics.DrawString(_element.CisloBodu, _font, brush, p1.FromWpfPoint(), f);

            if (Selected && !_point.IsEmpty)
            {
                DrawUtils.DrawNode(canvas, _point);
            }
        }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            var thWidth = ThresholdWidth(canvas, Group.Options.Width);
            var delta = canvas.ToUnit(2);
            return ScreenUtils.GetRect(new UnitPoint(_point.X - delta, _point.Y - delta), new UnitPoint(_point.X + delta, _point.Y + delta), thWidth);
        }

        public void OnMouseMove(ICanvas canvas, UnitPoint point)
        {

        }

        public DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            return DrawObjectState.DoneRepeat;
        }

        public DrawObjectState OnFinish()
        {
            return DrawObjectState.Drop;
        }

        public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }

        public UnitPoint RepeatStartingPoint => new UnitPoint();
        public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            return null;
        }
        public static float ThresholdWidth(ICanvas canvas, float objectwidth)
        {
            return ThresholdWidth(canvas, objectwidth, ThresholdPixel);
        }
        public static float ThresholdWidth(ICanvas canvas, float objectwidth, float pixelwidth)
        {
            var minWidth = canvas.ToUnit(pixelwidth);
            var width = Math.Max(objectwidth / 2, minWidth);
            return (float)width;
        }

        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
            var thWidth = ThresholdWidth(canvas, Group.Options.Width);
            foreach (var snaptype in runningsnaptypes)
            {
                if (snaptype == typeof(PodrobnyBodZPS) && Group.Name != "PodrobnyBodZPS" && snaptype != typeof(VertextSnapPoint))
                    return null;
                if (HitUtil.CircleHitPoint(_point, thWidth, point))
                    return new PodrobnyBodZPS(canvas, this, _point);
            }
            return null;
        }

        public void Move(UnitPoint offset)
        {
        }

        public bool getSelectDrawToolCreate()
        {
            return true;
        }

        public string GetInfoAsString()
        {
            return $"Group name: {Group.Name}, Cislo bodu: {_element.CisloBodu}, Stav: {_element.ZapisObjektuPopis}, " +
                   $"[Y,X,Z]=[{PointGeometry.Point.X:##.00},{PointGeometry.Point.Y:##.00},{PointGeometry.Point.Z:##.00}]";
        }

        public void Export(IExport export)
        {
            export.AddPoint(_point.X, _point.Y, Group.Options.Color);
            if (!string.IsNullOrEmpty(_element.CisloBodu))
            {
                export.AddText(_element.CisloBodu, _point.X, _point.Y, 0.5, string.Empty, Group.Options.Color, 0);
            }
        }
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            throw new NotImplementedException();
        }
        public IDtmDrawingGroup Group { get; set; }
        public IDtmElement GetDtmElement => _element;
    }
}
