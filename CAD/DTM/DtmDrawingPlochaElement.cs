using CAD.Canvas.DrawTools;
using CAD.Canvas;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Gui;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;
using GeoHelper.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Forms;
using CAD.DrawTools;
using CAD.VFK;

namespace CAD.DTM
{
    class DtmDrawingPlochaElement
        : DrawObjectBase
        , IDrawObject
        , IDtmDrawingElement
        , ISnapList
    {
        DtmElement _element;
        DtmSurfaceGeometry _curveGeometry;
        const int ThresholdPixel = 6;
        public DtmDrawingPlochaElement(DtmElement element)
        {
            _element = element;
            _curveGeometry = element.Geometry.GetDrawGeometry<DtmSurfaceGeometry>();
        }

        public string Id => "";

        public IDrawObject Clone()
        {
            var l = new DtmDrawingLineElement();
            l.Copy(this);
            return l;
        }

        public virtual void Copy(DtmDrawingPlochaElement origin)
        {
            base.Copy(origin);
            _element = origin._element;
            _curveGeometry = origin._curveGeometry;
        }

        bool ProcessLines(Func<UnitPoint, UnitPoint, bool> doAction)
        {
            if (_curveGeometry == null)
                return false;
            var p1 = new UnitPoint(_curveGeometry.Points[0].X, _curveGeometry.Points[0].Y);
            var p2 = new UnitPoint();
            for (var i = 1; i < _curveGeometry.Points.Count; i++)
            {
                p2.X = _curveGeometry.Points[i].X;
                p2.Y = _curveGeometry.Points[i].Y;
                if (doAction(p1, p2))
                    return true;
                (p1, p2) = (p2, p1);
            }

            return false;
        }

        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            double thWidth = ThresholdWidth(canvas, Group.Options.Width);
            return ProcessLines((p1, p2) => HitUtil.IsPointInLine(p1, p2, point, thWidth));
            /*            var fillPath = new GraphicsPath();
                        ProcessLines((p1, p2) =>
                        {
                            fillPath.AddLine(canvas.ToScreen(p1).FromWpfPoint(), canvas.ToScreen(p2).FromWpfPoint());
                            return false;
                        });
                        return fillPath.IsVisible(canvas.ToScreen(point).FromWpfPoint());
            */
        }

        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            var bBox = GetBoundingRect(canvas);
            return rect.IntersectsWith(bBox);

        }

        public void Draw(ICanvas canvas, Rect unitrect)
        {
            if (_curveGeometry == null || _curveGeometry.Points.Count <= 1)
                return;
            var fillPath = new GraphicsPath();
            ProcessLines((p1, p2) =>
            {
                fillPath.AddLine(canvas.ToScreen(p1).FromWpfPoint(), canvas.ToScreen(p2).FromWpfPoint());
                return false;
            });
            canvas.FillPath(canvas, new System.Drawing.SolidBrush(Group.Options.Color), fillPath);
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

        public Rect GetBoundingRect(ICanvas canvas)
        {
            if (_curveGeometry == null || _curveGeometry.Points.Count <= 1)
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
            double thWidth = ThresholdWidth(canvas, Group.Options.Width);
            return ScreenUtils.GetRect(p1, p2, thWidth);
        }

        public void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
        }

        public DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            return DrawObjectState.Drop;
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
            if (Group == null)
                return "";
            return $"{Group.Name}({_element.ZapisObjektu}), {_element.GetInfoAsString()}";
        }

        public void Export(IExport export)
        {
            if (_curveGeometry.Points.Count <= 1)
                return;
            var points = new UnitPoint[_curveGeometry.Points.Count];
            for (var i = 0; i < _curveGeometry.Points.Count; i++)
                points[i] = new UnitPoint(_curveGeometry.Points[i].X, _curveGeometry.Points[i].Y);
            export.AddPolyline(ref points, Group.Options.Color, Group.Options.Width);

        }

        public Type PropPageType => typeof(DtmPropPage);

        public IDtmDrawingGroup Group { get; set; }
        public IDtmElement GetDtmElement => _element;

        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            throw new NotImplemented();
        }

        public Type[] RunningSnaps
        {
            get { return new[] { typeof(DtmPodrobnyBodSnapPoint) }; }
        }
    }
}
