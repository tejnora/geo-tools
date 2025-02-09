using CAD.Canvas;
using System.Collections.Generic;
using System.Windows;
using CAD.Export;
using GeoBase.Utils;
using Color = System.Drawing.Color;
using CAD.Utils;
using System;
using System.Linq;
using CAD.Canvas.DrawTools;
using System.Windows.Controls.Primitives;

namespace CAD.DTM
{
    class DtmDrawingLayerMain : ICanvasLayer
    {
        readonly IDtmMain _dtmMain;
        readonly Dictionary<string, ICanvasLayer> _layers = new Dictionary<string, ICanvasLayer>();

        public DtmDrawingLayerMain(IDtmMain dtmMain)
        {
            Name = "Dtm";
            Visible = true;
            Enabled = true;
            _dtmMain = dtmMain;
            CreateDrawingObjects();
        }

        void CreateDrawingObjects()
        {
            foreach (var group in _dtmMain.GetElementGroups())
            {
                _layers[group.Key] = new DtmDrawingGroup(group.Key, group.Value);
            }
        }

        public string Id => "Dtm";
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                layer.Value.Draw(canvas, unitrect);
            }

            if (_dtmMain.UdajeOVydeji == null) return;
            var pen = canvas.CreatePen(Color.FromArgb(100, 209, 231, 235), 0.001f);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            ProcessLines((p1, p2) =>
            {
                canvas.DrawLine(canvas, pen, p1, p2);
                return false;
            });
        }

        bool ProcessLines(Func<UnitPoint, UnitPoint, bool> doAction)
        {
            var points = _dtmMain.UdajeOVydeji.Polygon.Points;
            var p1 = new UnitPoint(points[0].X, points[0].Y);
            var p2 = new UnitPoint();
            for (var i = 1; i < points.Count; i++)
            {
                p2.X = points[i].X;
                p2.Y = points[i].Y;
                if (doAction(p1, p2))
                    return true;
                (p1, p2) = (p2, p1);
            }
            return false;
        }


        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                var snap = layer.Value.SnapPoint(canvas, point, otherobj);
                if (snap != null)
                    return snap;
            }
            return null;

        }

        public IEnumerable<IDrawObject> Objects => null;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public double Width { get; set; }
        public string Name { get; set; }
        public string DtmLineElementSelected { get; set; }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            var bb = Rect.Empty;
            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                bb = WPFToFormConverter.unionRect(bb, layer.Value.GetBoundingRect(canvas));
            }
            return bb;
        }

        public void GetHitObjects(List<IDrawObject> selected, ICanvas canvas, Rect selection, bool anyPoint)
        {
            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                layer.Value.GetHitObjects(selected, canvas, selection, anyPoint);
            }

        }

        public void GetHitObjects(List<IDrawObject> aHitObjects, ICanvas canvas, UnitPoint point)
        {
            foreach (var layer in _layers)
            {
                if (!layer.Value.Visible) continue;
                layer.Value.GetHitObjects(aHitObjects, canvas, point);
            }
        }

        public void AddObject(IDrawObject drawobject)
        {
            var dtmElement = (IDtmDrawingElement)drawobject;
            dtmElement.GetDtmElement.IsDeleted = false;
            _dtmMain.AddElementIfNotExist(dtmElement.Group.Name, dtmElement.GetDtmElement);
            if (_layers[dtmElement.Group.Name] == null)
            {
                _layers[dtmElement.Group.Name] = new DtmDrawingGroup(dtmElement.Group.Name, drawobject);
            }
            else
            {
                _layers[dtmElement.Group.Name].AddObject(drawobject);
            }
        }

        public void Export(IExport export)
        {
            foreach (var layer in _layers)
            {
                layer.Value.Export(export);
            }
        }

        public void DeleteObjects(IEnumerable<IDrawObject> objects, List<Tuple<ICanvasLayer, IDrawObject>> deletedObjects)
        {
            var localDeletedObjects = new List<Tuple<ICanvasLayer, IDrawObject>>();
            foreach (var layer in _layers.Where(layer => layer.Value.Visible))
            {
                layer.Value.DeleteObjects(objects, localDeletedObjects);
            }

            foreach (var delObj in localDeletedObjects)
            {
                var dtmElement = (IDtmDrawingElement)delObj.Item2;
                dtmElement.GetDtmElement.IsDeleted = true;
                deletedObjects.Add(new Tuple<ICanvasLayer, IDrawObject>(this, delObj.Item2));
            }
        }
    }
}
