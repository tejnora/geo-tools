using CAD.Canvas;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using CAD.Export;
using GeoBase.Utils;
using CAD.Canvas.DrawTools;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Pen = System.Drawing.Pen;
using CAD.Utils;
using System.Drawing;
using CAD.VFK.DrawTools;

namespace CAD.DTM
{
    class DtmDrawingCurveElement
        : IDrawObject
    {
        readonly DtmElement _element;
        readonly float _width = 0.001f;
        readonly System.Drawing.Color _color = System.Drawing.Color.White;
        DtmCurveGeometry _curveGeometry;
        public DtmDrawingCurveElement(DtmElement element)
        {
            _element = element;
            _curveGeometry = (DtmCurveGeometry)element.Geometry;
        }
        public string Id { get; }
        public IDrawObject Clone()
        {
            throw new NotImplementedException();
        }

        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            return false;
        }

        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            if (!anyPoint)
            {
                var bBox = GetBoundingRect(canvas);
                return rect.Contains(bBox);
            }
            var p1 = new UnitPoint(_curveGeometry.Points[0].X, _curveGeometry.Points[0].Y);
            var p2 = new UnitPoint();
            for (var i = 1; i < _curveGeometry.Points.Count; i++)
            {
                p2.X = _curveGeometry.Points[i].X;
                p2.Y = _curveGeometry.Points[i].Y;
                if (HitUtil.LineIntersectWithRect(p1, p2, rect))
                    return true;
                (p1, p2) = (p2, p1);
            }
            p2.X = _curveGeometry.Points[0].X;
            p2.Y = _curveGeometry.Points[0].Y;
            return HitUtil.LineIntersectWithRect(p1, p2, rect);
        }

        public void Draw(ICanvas canvas, Rect unitrect)
        {
            if (_curveGeometry.Points.Count <= 1)
                return;
            var pen = canvas.CreatePen(_color, _width);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;

            var p1 = new UnitPoint(_curveGeometry.Points[0].X, _curveGeometry.Points[0].Y);
            var p2 = new UnitPoint();
            for (var i = 1; i < _curveGeometry.Points.Count; i++)
            {
                p2.X = _curveGeometry.Points[i].X;
                p2.Y = _curveGeometry.Points[i].Y;
                canvas.DrawLine(canvas, pen, p1, p2);
                (p1, p2) = (p2, p1);
            }
        }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            if (_curveGeometry.Points.Count <= 1)
                return Rect.Empty;
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            foreach (var lineSegment in _curveGeometry.Points)
            {
                minX = Math.Min(lineSegment.X, minX);
                minY = Math.Min(lineSegment.Y, minY);
                maxX = Math.Max(lineSegment.X, maxX);
                maxY = Math.Max(lineSegment.Y, maxY);
            }
            var p1 = new UnitPoint(minX, minY);
            var p2 = new UnitPoint(maxX, maxY);
            return ScreenUtils.GetRect(p1, p2, _width);
        }

        public void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
        }

        public eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            return eDrawObjectMouseDown.DoneRepeat;
        }

        public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }

        public UnitPoint RepeatStartingPoint { get; }
        public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            return null;
        }

        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
            return null;
        }

        public void Move(UnitPoint offset)
        {
        }

        public bool getSelectDrawToolCreate()
        {
            return false;
        }

        public string GetInfoAsString()
        {
            return "";
        }

        public void Export(IExport export)
        {
            if (_curveGeometry.Points.Count <= 1)
                return;
            var points = new UnitPoint[_curveGeometry.Points.Count];
            for (var i = 0; i < _curveGeometry.Points.Count; i++)
                points[i] = new UnitPoint(_curveGeometry.Points[i].X, _curveGeometry.Points[i].Y);
            export.AddPolyline(ref points, _color, _width);

        }
    }
}
