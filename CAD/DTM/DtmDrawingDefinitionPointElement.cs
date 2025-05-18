using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.DTM.Elements;
using CAD.Export;
using GeoBase.Utils;
using CAD.Utils;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    public class DtmDrawingDefinitionPointElement
        : DrawObjectBase
        , IDrawObject
        , IDtmDrawingElement
    {
        DtmElement _element;
        readonly UnitPoint _point;
        const int ThresholdPixel = 6;

        public DtmDrawingDefinitionPointElement()
        {

        }
        public DtmDrawingDefinitionPointElement(DtmElement element)
        {
            _element = element;
            PointGeometry = element.Geometry.GetDrawGeometry<DtmPointGeometry>();
            _point = new UnitPoint(PointGeometry.Point.X, PointGeometry.Point.Y);

        }
        public DtmPointGeometry PointGeometry { get; private set; }
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            throw new NotImplementedException();
        }
        public string Id { get; }
        public IDrawObject Clone()
        {
            var l = new DtmDrawingDefinitionPointElement();
            l.Copy(this);
            return l;
        }
        public virtual void Copy(DtmDrawingDefinitionPointElement origin)
        {
            base.Copy(origin);
            _element = origin._element;
            PointGeometry = origin.PointGeometry;
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
            var p = canvas.ToScreen(_point).FromWpfPoint();
            canvas.Graphics.DrawRectangle(pen, p.X - 5f, p.Y - 5f, 10f, 10f);
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

        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
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
            return $"Group name: {Group.Name},{_element.GetInfoAsString()}, " +
                   $"[Y,X,Z]=[{PointGeometry.Point.X:##.00},{PointGeometry.Point.Y:##.00},{PointGeometry.Point.Z:##.00}]";

        }
        public void Export(IExport export)
        {
            export.AddPoint(_point.X, _point.Y, Group.Options.Color);
        }

        public Type PropPageType => typeof(DtmPropPage);

        public IDtmDrawingGroup Group { get; set; }
        public IDtmElement GetDtmElement => _element;
    }
}
