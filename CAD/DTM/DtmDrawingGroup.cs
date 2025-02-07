using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using CAD.Canvas;
using CAD.DTM.Configuration;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;
using Color = System.Drawing.Color;

namespace CAD.DTM
{
    class DtmDrawingGroup
    : ICanvasLayer
    , IDtmDrawingGroup
    {
        readonly List<IDrawObject> _objects = new List<IDrawObject>();

        public DtmDrawingGroup(string groupName, IDtmElementsGroup group)
        {
            Visible = true;
            Name = groupName;
            Id = groupName;
            InitOptions();
            CreateDrawingObjects(group);
        }

        public DtmDrawingGroup(string groupName, IDrawObject drawObject)
        {
            Visible = true;
            Name = groupName;
            Id = groupName;
            InitOptions();
            ((IDtmDrawingElement)drawObject).Group = this;
            _objects = new List<IDrawObject> { drawObject };
        }

        void InitOptions()
        {
            var setting = DtmConfigurationSingleton.Instance.ElementSetting;
            Options = new DtmElementOption { Width = 0.003f, Color = Color.White };
            if (!setting.ContainsKey(Name))
                return;
            Options = setting[Name];
        }

        void CreateDrawingObjects(IDtmElementsGroup group)
        {
            foreach (var g in group.GetElementGroups())
            {
                var obj = g.CreateDrawObject();
                ((IDtmDrawingElement)obj).Group = this;
                _objects.Add(obj);
            }
        }

        public string Id { get; }
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            foreach (var obj in _objects)
            {
                if (obj.ObjectInRectangle(canvas, unitrect, true) == false)
                    continue;
                var sel = obj.Selected;
                var high = obj.Highlighted;
                obj.Selected = false;
                obj.Highlighted = false;
                obj.Draw(canvas, unitrect);
                obj.Selected = sel;
                obj.Highlighted = high;
            }
        }

        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            foreach (var obj in _objects)
            {
                var sp = obj.SnapPoint(canvas, point, otherobj, null, null);
                if (sp != null)
                    return sp;
            }
            return null;
        }

        public IEnumerable<IDrawObject> Objects => _objects;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public double Width { get; set; }
        public DtmElementOption Options { get; set; }
        public string Name { get; set; }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            var bbox = Rect.Empty;
            foreach (var obj in _objects)
            {
                bbox = WPFToFormConverter.unionRect(bbox, obj.GetBoundingRect(canvas));
            }
            return bbox;
        }

        public void GetHitObjects(List<IDrawObject> selected, ICanvas canvas, Rect selection, bool anyPoint)
        {
            foreach (var drawobject in Objects)
            {
                if (drawobject.ObjectInRectangle(canvas, selection, anyPoint))
                    selected.Add(drawobject);
            }

        }

        public void GetHitObjects(List<IDrawObject> aHitObjects, ICanvas canvas, UnitPoint point)
        {
            foreach (var drawobject in Objects)
            {
                if (drawobject.PointInObject(canvas, point))
                    aHitObjects.Add(drawobject);
            }
        }

        public void Export(IExport export)
        {
            export.SinkLayer(this);
            foreach (var obj in Objects)
            {
                obj.Export(export);
            }
            export.RiseLayer();
        }
        public void AddObject(IDrawObject drawobject)
        {
            ((IDtmDrawingElement)drawobject).Group = this;
            _objects.Add(drawobject);
        }

        public void DeleteObjects(IEnumerable<IDrawObject> objects, List<Tuple<ICanvasLayer, IDrawObject>> deletedObjects)
        {
            foreach (var obj in objects)
            {
                var deletedObj = _objects.Find((o => obj == o));
                if (deletedObj == null)
                    continue;
                _objects.Remove(deletedObj);
                deletedObjects.Add(new Tuple<ICanvasLayer, IDrawObject>(this, deletedObj));
            }
        }
    }
}
