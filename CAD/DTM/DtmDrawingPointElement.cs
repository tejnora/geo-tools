using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Gui;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;
using Size = System.Drawing.Size;

namespace CAD.DTM
{
    class DtmDrawingPointElement
        : DrawObjectBase
        , IDrawObject
        , IDtmDrawingElement
    {
        DtmBodBaseElement _element;
        public DtmPointGeometry PointGeometry { get; private set; }
        UnitPoint _point;
        const int ThresholdPixel = 6;
        static readonly Font Font = new Font("Arial", 12F, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel, 0);
        public DtmDrawingPointElement()
        {

        }
        public DtmDrawingPointElement(DtmElement element)
        {
            _element = (DtmBodBaseElement)element;
            PointGeometry = element.Geometry.GetDrawGeometry<DtmPointGeometry>();
            UpdatePoint();
        }
        void UpdatePoint()
        {
            _point = new UnitPoint(PointGeometry.Point.X, PointGeometry.Point.Y);
        }
        public string Id => DtmToolBar.DtmPoint.Name;
        public IDrawObject Clone()
        {
            var l = new DtmDrawingPointElement();
            l.Copy(this);
            return l;
        }
        public virtual void Copy(DtmDrawingPointElement origin)
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
            if (PointGeometry == null)
                return;
            var scale = DtmUtils.GetScale(canvas);
            var graphicElement = _element.GetGraphicElement(Group.Options, scale);
            if (graphicElement == null)
                return;
            canvas.DrawSymbol(canvas, graphicElement.Symbol, _point, graphicElement.Size);
            if (!string.IsNullOrEmpty(_element.CisloBodu))
            {
                var textPoint = _point + graphicElement.Size;
                var f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                var p1 = canvas.ToScreen(textPoint);
                var brush = new SolidBrush(graphicElement.Color);
                canvas.Graphics.DrawString(_element.CisloBodu, Font, brush, p1.FromWpfPoint(), f);
            }
            if (Selected && !_point.IsEmpty)
            {
                DrawUtils.DrawNode(canvas, _point);
            }
            /*                var p1 = canvas.ToScreen(_point);
                            var pen = canvas.CreatePen(Group.Options.Color, Group.Options.Width);
                            pen.EndCap = LineCap.Flat;
                            pen.StartCap = LineCap.Flat;
                            switch (_element.DrawingMark)
                            {
                                case DtmBodDrawingMarkEnum.Cross:
                                    var p2 = p1;
                                    p1.X -= 5;
                                    p2.X += 5;
                                    canvas.DrawLine(canvas, pen, p1, p2);
                                    p1.X += 5;
                                    p2.X -= 5;
                                    p1.Y -= 5;
                                    p2.Y += 5;
                                    canvas.DrawLine(canvas, pen, p1, p2);
                                    break;
                                case DtmBodDrawingMarkEnum.Circle:
                                    canvas.DrawCircle(canvas, pen, p1, 5);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            var f = new StringFormat();
                            f.Alignment = StringAlignment.Center;
                            p1.X += 10;
                            p1.Y -= 10;
                            var brush = new SolidBrush(Group.Options.Color);
                            canvas.Graphics.DrawString(_element.CisloBodu, Font, brush, p1.FromWpfPoint(), f);

                            if (Selected && !_point.IsEmpty)
                            {
                                DrawUtils.DrawNode(canvas, _point);
                            }

                        }*/
        }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            if (PointGeometry == null)
                return Rect.Empty;
            var thWidth = ThresholdWidth(canvas, Group.Options.Width);
            var scale = DtmUtils.GetScale(canvas);
            var graphicElement = _element.GetGraphicElement(Group.Options, scale);
            if (graphicElement == null)
                return Rect.Empty;
            var deltaX = graphicElement.Size.X / 2.0;
            var deltaY = graphicElement.Size.Y / 2.0;
            return ScreenUtils.GetRect(new UnitPoint(_point.X - deltaX, _point.Y - deltaY), new UnitPoint(_point.X + deltaX, _point.Y + deltaY), thWidth);
            /*var delta = canvas.ToUnit(2);
            return ScreenUtils.GetRect(new UnitPoint(_point.X - delta, _point.Y - delta), new UnitPoint(_point.X + delta, _point.Y + delta), thWidth);
            */
        }

        public void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            if (PointGeometry == null)
                return;
            PointGeometry.Point.X = point.X;
            PointGeometry.Point.Y = point.Y;
            UpdatePoint();
        }

        public DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if (!(snappoint is DtmPodrobnyBodSnapPoint zpz))
                return DrawObjectState.Continue;
            var pointGeometry = ((DtmDrawingPointElement)zpz.Owner).PointGeometry;
            PointGeometry.Point.X = pointGeometry.Point.X;
            PointGeometry.Point.Y = pointGeometry.Point.Y;
            PointGeometry.Point.Z = pointGeometry.Point.Z;
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
            if (PointGeometry == null)
                return null;
            var thWidth = ThresholdWidth(canvas, Group.Options.Width);
            foreach (var snaptype in runningsnaptypes)
            {
                if (snaptype == typeof(DtmPodrobnyBodSnapPoint) && Group.Name != "PodrobnyBodZPS" && snaptype != typeof(VertextSnapPoint))
                    return null;
                if (HitUtil.CircleHitPoint(_point, thWidth, point))
                    return new DtmPodrobnyBodSnapPoint(canvas, this, _point);
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
            if (PointGeometry == null)
                return "";
            return $"{Group.Name}({_element.ZapisObjektu}),{_element.GetInfoAsString()}, " +
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

        public Type PropPageType => typeof(DtmPropPage);

        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            if (snap is DtmPodrobnyBodSnapPoint bodSnap)
            {
                var pointGeometry = ((DtmDrawingPointElement)bodSnap.Owner).PointGeometry;
                PointGeometry = new DtmPointGeometry() { Point = (DtmPoint)pointGeometry.Point.Clone() };
            }
            else
            {
                PointGeometry = new DtmPointGeometry() { Point = new DtmPoint() { X = point.X, Y = point.Y } };
            }
            UpdatePoint();

            var dtmLayer = (DtmDrawingLayerMain)layer;
            _element = (DtmBodBaseElement)DtmConfigurationSingleton.Instance.CreateType(dtmLayer.DtmPointSelected.Item1);
            _element.SelectedSetting(dtmLayer.DtmPointSelected.Item2);
            _element.Geometry = new DtmGeometryGroup { Geometries = new List<IDtmGeometry> { PointGeometry } };
            new DtmDrawingGroup(dtmLayer.DtmPointSelected.Item1, this);
            Selected = true;
        }
        public IDtmDrawingGroup Group { get; set; }
        public IDtmElement GetDtmElement => _element;
    }
}
