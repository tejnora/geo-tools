using CAD.Canvas;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using CAD.Export;
using GeoBase.Utils;
using CAD.Utils;
using CAD.Canvas.DrawTools;
using CAD.DrawTools;
using GeoHelper.Utils;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;

namespace CAD.DTM
{
    class DtmDrawingCurveElement
        : DrawObjectBase
            , IDrawObject
            , IDtmDrawingElement
            , ISnapList
    {
        DtmElement _element;
        DtmCurveGeometry _curveGeometry;
        const int ThresholdPixel = 6;

        public DtmDrawingCurveElement()
        {

        }

        public DtmDrawingCurveElement(DtmElement element)
        {
            _element = element;
            _curveGeometry = (DtmCurveGeometry)element.Geometry;
        }

        public string Id { get; }

        public IDrawObject Clone()
        {
            var l = new DtmDrawingCurveElement();
            l.Copy(this);
            return l;
        }

        public virtual void Copy(DtmDrawingCurveElement origin)
        {
            base.Copy(origin);
            _element = origin._element;
            _curveGeometry = origin._curveGeometry;
        }

        bool ProcessLines(Func<UnitPoint, UnitPoint, bool> doAction)
        {
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
        }

        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            if (!anyPoint)
            {
                var bBox = GetBoundingRect(canvas);
                return rect.Contains(bBox);
            }

            return ProcessLines((p1, p2) => HitUtil.LineIntersectWithRect(p1, p2, rect));
        }

        public void Draw(ICanvas canvas, Rect unitrect)
        {
            if (_curveGeometry == null || _curveGeometry.Points.Count <= 1)
                return;
            var pen = canvas.CreatePen(Group.Options.Color, Group.Options.Width);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            ProcessLines((p1, p2) =>
            {
                canvas.DrawLine(canvas, pen, p1, p2);
                if (!Selected) return false;
                canvas.DrawLine(canvas, DrawUtils.SelectedPen, p1, p2);
                DrawUtils.DrawNode(canvas, p1);
                DrawUtils.DrawNode(canvas, p2);
                return false;
            });
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
            var lastPoint = _curveGeometry.Points.Back();
            lastPoint.X = point.X;
            lastPoint.Y = point.Y;
        }

        public DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if (!(snappoint is PodrobnyBodZPS zpz))
                return DrawObjectState.Continue;
            var pointGeometry = ((DtmDrawingPointElement)zpz.Owner).PointGeometry;
            _curveGeometry.Points.Add((DtmPoint)pointGeometry.Point.Clone());
            return DrawObjectState.Continue;
        }

        public DrawObjectState OnFinish()
        {
            if (_curveGeometry.Points.Count > 2)
            {
                _curveGeometry.Points.Pop();
                return DrawObjectState.Done;
            }
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
            return $"Group name: {Group.Name}";
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

        public IDtmDrawingGroup Group { get; set; }
        public IDtmElement GetDtmElement => _element;

        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            if (!(snap is PodrobnyBodZPS zpz))
                return;
            var pointGeometry = ((DtmDrawingPointElement)zpz.Owner).PointGeometry;
            _curveGeometry = new DtmCurveGeometry
            {
                Points = new List<DtmPoint> { (DtmPoint)pointGeometry.Point.Clone(), (DtmPoint)pointGeometry.Point }
            };
            var dtmLayer = (DtmDrawingLayerMain)layer;
            _element = DtmConfigurationSingleton.Instance.CreateType(dtmLayer.DtmLineElementSelected);
            _element.Geometry = _curveGeometry;
            new DtmDrawingGroup(dtmLayer.DtmLineElementSelected, this);
            Selected = true;
        }

        public Type[] RunningSnaps
        {
            get { return new[] { typeof(PodrobnyBodZPS) }; }
        }
    }
}
