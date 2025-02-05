using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using CAD.Canvas;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;
using Color = System.Drawing.Color;

namespace CAD.DTM
{
    class DtmDrawingGroup
    : ICanvasLayer
    {
        readonly IDtmElementsGroup _group;
        readonly List<IDrawObject> _objects = new List<IDrawObject>();

        public DtmDrawingGroup(string groupName, IDtmElementsGroup group)
        {
            Visible = true;
            Name = groupName;
            Id = groupName;
            _group = group;
            CreateDrawingObjects();
        }

        void CreateDrawingObjects()
        {
            foreach (var group in _group.GetElementGroups())
            {
                _objects.Add(group.CreateDrawObject());
            }
        }

        public string Id { get; private set; }
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            foreach (var obj in _objects)
            {
                if (obj.ObjectInRectangle(canvas, unitrect, true) == false)
                    continue;
                obj.Draw(canvas, unitrect);
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

        public void AddObject(IDrawObject drawobject)
        {
            throw new NotImplementedException();
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
    }
}
