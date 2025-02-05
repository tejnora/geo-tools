using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;
using static CAD.Canvas.DefaultColors;

namespace CAD.DTM
{
    class DtmDrawingPointElement
        : IDrawObject
    {
        readonly DtmElement _element;
        readonly DtmPointGeometry _pointGeometry;
        readonly float _width = 0.001f;
        readonly System.Drawing.Color _color = System.Drawing.Color.White;
        readonly UnitPoint _point;
        const int ThresholdPixel = 6;
        readonly Font _font = new Font("Arial", 12F, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel, 0);

        public DtmDrawingPointElement(DtmElement element)
        {
            _element = element;
            _pointGeometry = (DtmPointGeometry)element.Geometry;
            _point = new UnitPoint(_pointGeometry.Point.X, _pointGeometry.Point.Y);
        }
        public string Id { get; }
        public IDrawObject Clone()
        {
            throw new NotImplementedException();
        }

        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            var rect = GetBoundingRect(canvas);
            return rect.Contains(point.Point);

        }

        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            var bbox = GetBoundingRect(canvas);
            return rect.Contains(bbox);

        }

        public void Draw(ICanvas canvas, Rect unitrect)
        {
            var pen = canvas.CreatePen(_color, _width);
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
            var _brush = new SolidBrush(System.Drawing.Color.Yellow);
            canvas.Graphics.DrawString(_element.CisloBodu, _font, _brush, p1.FromWpfPoint(), f);

            /*if (Selected)
                {
                if (P1.IsEmpty == false)
                DrawUtils.DrawNode(canvas, P1);
                }
            */
        }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            return new Rect(new System.Windows.Point(_point.X - 3.0f, _point.Y - 3.0f), new System.Windows.Size(6.0f, 6.0f));
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
            var thWidth = ThresholdWidth(canvas, _width);
            if (runningsnaptypes == null)
            {
                return usersnaptype == typeof(VertextSnapPoint) ? new VertextSnapPoint(canvas, this, _point) : null;
            }

            foreach (var snaptype in runningsnaptypes)
            {
                if (snaptype != typeof(VertextSnapPoint)) continue;
                if (HitUtil.CircleHitPoint(_point, thWidth, point))
                    return new VertextSnapPoint(canvas, this, _point);
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
            return "";
        }

        public void Export(IExport export)
        {
            export.AddPoint(_point.X, _point.Y, _color);
            if (!string.IsNullOrEmpty(_element.CisloBodu))
            {
                export.AddText(_element.CisloBodu, _point.X, _point.Y, 0.5, string.Empty, _color, 0);
            }
        }
    }
}
