using CAD.Canvas;
using System.Collections.Generic;
using System.Windows;
using CAD.Export;
using GeoBase.Utils;
using Color = System.Drawing.Color;
using CAD.Utils;
using System;
using System.Xml.Linq;

namespace CAD.DTM
{
    class DtmDrawingLayerMain : ICanvasLayer
    {
        readonly IDtmMain _dtmMain;
        readonly Dictionary<string, ICanvasLayer> _layers = new Dictionary<string, ICanvasLayer>();

        public DtmDrawingLayerMain(IDtmMain dtmMain)
        {
            Name = "Dtm";
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
            throw new NotImplementedException();
        }

        public void Export(IExport export)
        {
            foreach (var layer in _layers)
            {
                layer.Value.Export(export);
            }
        }
    }
}
