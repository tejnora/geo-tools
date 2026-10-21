using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using CAD.Canvas;
using CAD.Canvas.DrawTools;
using CAD.DrawTools;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Gui;
using CAD.Export;
using CAD.Utils;
using CAD.VFK;
using GeoBase.Utils;
using GeoHelper.Utils;

namespace CAD.DTM
{
    class DtmDrawingPlochaElement : DrawObjectBase, IDrawObject, IDtmDrawingElement, ISnapList
    {
        DtmElement _element;
        DtmSurfaceGeometry _geometry;
        List<DtmPoint> _activeRing;
        public DtmDrawingPlochaElement() { }
        public DtmDrawingPlochaElement(DtmElement element)
        {
            _element = element;
            _geometry = element.Geometry.GetDrawGeometry<DtmSurfaceGeometry>();
        }
        public string Id => DtmToolBar.DtmArea.Name;
        public IDrawObject Clone()
        {
            var copy = new DtmDrawingPlochaElement();
            copy.Copy(this);
            return copy;
        }
        public void Copy(DtmDrawingPlochaElement origin)
        {
            base.Copy(origin);
            _element = origin._element;
            _geometry = origin._geometry;
            _activeRing = origin._activeRing;
            Group = origin.Group;
        }
        IEnumerable<List<DtmPoint>> Rings => _geometry == null ? Enumerable.Empty<List<DtmPoint>>() : new[] { _geometry.Points }.Concat(_geometry.InteriorRings);

        GraphicsPath Path(ICanvas canvas)
        {
            var path = new GraphicsPath(FillMode.Alternate);
            foreach (var ring in Rings.Where(r => r?.Count >= 3))
                path.AddPolygon(ring.Select(p => canvas.ToScreen(new UnitPoint(p.X, p.Y)).FromWpfPoint()).ToArray());
            return path;
        }

        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            using var path = Path(canvas);
            using var pen = new System.Drawing.Pen(System.Drawing.Color.Black, 12);
            var screen = canvas.ToScreen(point).FromWpfPoint();
            return path.IsVisible(screen) || path.IsOutlineVisible(screen, pen);
        }
        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint) => anyPoint ? rect.IntersectsWith(GetBoundingRect(canvas)) : rect.Contains(GetBoundingRect(canvas));

        public void Draw(ICanvas canvas, Rect unitrect)
        {
            if (_geometry?.Points == null) return;
            using var path = Path(canvas);
            using var brush = new System.Drawing.SolidBrush(Group.Options.Color);
            canvas.FillPath(canvas, brush, path);
            var pen = canvas.CreatePen(Group.Options.Color, Group.Options.Width);
            foreach (var ring in Rings.Where(r => r?.Count > 1))
                for (var i = 1; i < ring.Count; i++)
                {
                    var a = new UnitPoint(ring[i - 1].X, ring[i - 1].Y);
                    var b = new UnitPoint(ring[i].X, ring[i].Y);
                    canvas.DrawLine(canvas, pen, a, b);
                    if (Selected) { canvas.DrawLine(canvas, DrawUtils.SelectedPen, a, b); DrawUtils.DrawNode(canvas, a); }
                }
        }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            var points = Rings.Where(r => r != null).SelectMany(r => r).ToArray();
            if (points.Length == 0) return Rect.Empty;
            return ScreenUtils.GetRect(new UnitPoint(points.Min(p => p.X), points.Min(p => p.Y)),
                new UnitPoint(points.Max(p => p.X), points.Max(p => p.Y)), DtmDrawingLineElement.ThresholdWidth(canvas, Group.Options.Width));
        }
        public void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            if (_activeRing?.Count > 0) { _activeRing[^1].X = point.X; _activeRing[^1].Y = point.Y; }
        }
        public DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if (_activeRing == null) return DrawObjectState.Drop;
            if (snappoint is not DtmPodrobnyBodSnapPoint snap) return DrawObjectState.Continue;
            var vertex = (DtmPoint)((DtmDrawingPointElement)snap.Owner).PointGeometry.Point.Clone();
            if (_activeRing.Count == 0) _activeRing.Add((DtmPoint)vertex.Clone());
            _activeRing[^1] = vertex;
            _activeRing.Add((DtmPoint)vertex.Clone());
            return DrawObjectState.Continue;
        }
        bool CloseRing()
        {
            if (_activeRing == null || _activeRing.Count < 4) return false;
            _activeRing.RemoveAt(_activeRing.Count - 1); // discard the moving preview vertex
            if (_activeRing[^1].X != _activeRing[0].X || _activeRing[^1].Y != _activeRing[0].Y || _activeRing[^1].Z != _activeRing[0].Z)
                _activeRing.Add((DtmPoint)_activeRing[0].Clone());
            return _activeRing.Count >= 4;
        }
        public DrawObjectState OnFinish()
        {
            if (_geometry == null) return DrawObjectState.Drop;
            if (_activeRing?.Count == 0 && _geometry.InteriorRings.Contains(_activeRing))
                _geometry.InteriorRings.Remove(_activeRing);
            else if (!CloseRing()) return DrawObjectState.Drop;
            _activeRing = null;
            if (_element.SchemaAttributes.Definition.Geometries.Contains("multiCurveProperty"))
            {
                var outline = new DtmMultiCurveGeometry { Points = _geometry.Points.Select(p => (DtmPoint)p.Clone()).ToList() };
                outline.Members = Rings.Select(r => new DtmCurveGeometry { Points = r.Select(p => (DtmPoint)p.Clone()).ToList() }).ToList();
                _element.Geometry.Geometries.Add(outline);
                _geometry.SrsDimension = 2;
            }
            return DrawObjectState.Done;
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.H || !CloseRing()) return;
            _activeRing = new List<DtmPoint>();
            _geometry.InteriorRings.Add(_activeRing);
            e.Handled = true;
        }
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            if (snap is not DtmPodrobnyBodSnapPoint survey) return;
            var dtmLayer = (DtmDrawingLayerMain)layer;
            if (dtmLayer.DtmAreaSelected == null) return;
            var vertex = ((DtmDrawingPointElement)survey.Owner).PointGeometry.Point;
            _activeRing = new List<DtmPoint> { (DtmPoint)vertex.Clone(), (DtmPoint)vertex.Clone() };
            _geometry = new DtmSurfaceGeometry { Points = _activeRing };
            _element = DtmConfigurationSingleton.Instance.CreateType(dtmLayer.DtmAreaSelected);
            _element.Geometry = new DtmGeometryGroup { Geometries = new List<IDtmGeometry> { _geometry } };
            new DtmDrawingGroup(dtmLayer.DtmAreaSelected, this);
            Selected = true;
        }
        public void Export(IExport export)
        {
            foreach (var ring in Rings.Where(r => r?.Count > 1))
            {
                var points = ring.Select(p => new UnitPoint(p.X, p.Y)).ToArray();
                export.AddPolyline(ref points, Group.Options.Color, Group.Options.Width);
            }
        }
        public void Move(UnitPoint offset) { }
        public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint) { }
        public UnitPoint RepeatStartingPoint => new();
        public INodePoint NodePoint(ICanvas canvas, UnitPoint point) => null;
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype) => null;
        public bool getSelectDrawToolCreate() => false;
        public string GetInfoAsString() => Group == null ? "" : $"{Group.Name}({_element.ZapisObjektu}), {_element.GetInfoAsString()}";
        public Type PropPageType => typeof(DtmPropPage);
        public IDtmDrawingGroup Group { get; set; }
        public IDtmElement GetDtmElement => _element;
        public Type[] RunningSnaps => new[] { typeof(DtmPodrobnyBodSnapPoint) };
    }
}
