using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using CAD.Export;
using CAD.Utils;
using CAD.Tools;
using GeoBase.Utils;
using Color = System.Drawing.Color;

namespace CAD.Canvas.Layers
{
    [Serializable]
    public class DrawingLayer : ICanvasLayer, ISerializable
    {
        public DrawingLayer(string id, string name, Color color, float width)
        {
            _id = id;
            _name = name;
            _color = color;
            m_width = width;
        }
        string _id;
        string _name = "<Layer>";
        Color _color;
        double m_width = 0.00f;
        bool _enabled = true;
        bool _visible = true;
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public double Width
        {
            get { return m_width; }
            set { m_width = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public bool Enabled
        {
            get { return _enabled && _visible; }
            set { _enabled = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public string Id
        {
            get { return _id; }
        }
        List<IDrawObject> _objects = new List<IDrawObject>();
        Dictionary<IDrawObject, bool> _objectMap = new Dictionary<IDrawObject, bool>();
        public void AddObject(IDrawObject drawobject)
        {
            if (_objectMap.ContainsKey(drawobject))
                return; // this should never happen
            if (drawobject is DrawTools.DrawObjectBase)
                ((DrawTools.DrawObjectBase)drawobject).Layer = this;
            _objects.Add(drawobject);
            _objectMap[drawobject] = true;
        }
        public int Count
        {
            get { return _objects.Count; }
        }
        public void Copy(DrawingLayer acopy, bool includeDrawObjects)
        {
            if (includeDrawObjects)
                throw new Exception("not supported yet");
            _id = acopy._id;
            _name = acopy._name;
            _color = acopy._color;
            m_width = acopy.m_width;
            _enabled = acopy._enabled;
            _visible = acopy._visible;
        }
        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            Tracing.StartTrack(App.TracePaint);
            int cnt = 0;
            foreach (IDrawObject drawobject in _objects)
            {
                DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
                if (obj is IDrawObject && ((IDrawObject)obj).ObjectInRectangle(canvas, unitrect, true) == false)
                    continue;
                bool sel = obj.Selected;
                bool high = obj.Highlighted;
                obj.Selected = false;
                drawobject.Draw(canvas, unitrect);
                obj.Selected = sel;
                obj.Highlighted = high;
                cnt++;
            }
            Tracing.EndTrack(App.TracePaint, "Draw Layer {0}, ObjCount {1}, Painted ObjCount {2}", Id, _objects.Count, cnt);
        }
        public virtual Point SnapPoint(Point unitmousepoint)
        {
            return new Point();
        }
        public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            foreach (IDrawObject obj in _objects)
            {
                ISnapPoint sp = obj.SnapPoint(canvas, point, otherobj, null, null);
                if (sp != null)
                    return sp;
            }
            return null;
        }
        public virtual void GetHitObjects(List<IDrawObject> aHitObjects, ICanvas canvas, UnitPoint point)
        {
            foreach (IDrawObject drawobject in Objects)
            {
                if (drawobject.PointInObject(canvas, point))
                    aHitObjects.Add(drawobject);
            }
        }
        public void GetHitObjects(List<IDrawObject> selected, ICanvas canvas, Rect selection, bool anyPoint)
        {
            foreach (IDrawObject drawobject in Objects)
            {
                if (drawobject.ObjectInRectangle(canvas, selection, anyPoint))
                    selected.Add(drawobject);
            }
        }
        public virtual Rect GetBoundingRect(ICanvas canvas)
        {
            Rect bb = Rect.Empty;
            foreach (IDrawObject drawobject in _objects)
            {
                bb = WPFToFormConverter.unionRect(bb, drawobject.GetBoundingRect(canvas));
            }
            return bb;
        }
        public IEnumerable<IDrawObject> Objects
        {
            get { return _objects; }
        }
        public void Export(IExport export)
        {
            export.SinkLayer(this);
            foreach (IDrawObject drawobject in Objects)
                drawobject.Export(export);
            export.RiseLayer();
        }

        public void DeleteObjects(IEnumerable<IDrawObject> objects, List<Tuple<ICanvasLayer, IDrawObject>> deletedObjects)
        {
            if (Enabled == false)
                return;
            var removedobjects = new List<IDrawObject>();
            // first remove from map only
            foreach (IDrawObject obj in objects)
            {
                if (!_objectMap.ContainsKey(obj)) continue;
                _objectMap.Remove(obj);
                removedobjects.Add(obj);
            }
            // need some smart algorithm here to either remove from existing list or build a new list
            // for now I will just ise the removed count;
            if (removedobjects.Count == 0)
                return;
            if (removedobjects.Count < 10) // remove from existing list
            {
                foreach (IDrawObject obj in removedobjects)
                    _objects.Remove(obj);
            }
            else // else build new list;
            {
                var newlist = new List<IDrawObject>();
                foreach (IDrawObject obj in _objects)
                {
                    if (_objectMap.ContainsKey(obj))
                        newlist.Add(obj);
                }
                _objects.Clear();
                _objects = newlist;
            }
            deletedObjects.AddRange(removedobjects.Select(obj => new Tuple<ICanvasLayer, IDrawObject>(this, obj)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Color", Color);
            info.AddValue("Width", Width);
            info.AddValue("Name", Name);
            info.AddValue("Enabled", Enabled);
            info.AddValue("Visible", Visible);
            info.AddValue("Id", Id);
            info.AddValue("ObjectCount", _objects.Count);
            Int32 counter = 0;
            foreach (IDrawObject drawobj in _objects)
            {
                info.AddValue("Object " + counter, drawobj, drawobj.GetType());
                counter++;
            }

        }
        public DrawingLayer(SerializationInfo info, StreamingContext ctxt)
        {
            Color = (Color)info.GetValue("Color", typeof(Color));
            Width = (float)info.GetValue("Width", typeof(float));
            Name = info.GetString("Name");
            Enabled = info.GetBoolean("Enabled");
            Visible = info.GetBoolean("Visible");
            _id = info.GetString("Id");
            int objectCount = info.GetInt32("ObjectCount");
            for (int i = 0; i < objectCount; i++)
                _objects.Add(info.GetValue("Object " + i, typeof(IDrawObject)) as IDrawObject);
        }
    }
}
