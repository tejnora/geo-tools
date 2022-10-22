using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using CAD.Export;
using CAD.Utils;
using CAD.Tools;
using GeoBase.Utils;
using VFK;

namespace CAD.Canvas.Layers
{
    class VfkDrawinLayer : ICanvasLayer
    {
        #region Constructor
        public VfkDrawinLayer(VfkElement vfkElement)
        {
            Enabled = true;
            Visible = true;
            _id = vfkElement.TYPPPD_KOD.ToString();
            Name = _id;
            Name = vfkElement.PritomnyStav.LayerName.ToString();
        }
        #endregion
        #region Fields
        private List<IDrawObject> _objects = new List<IDrawObject>();
        readonly string _id;
        public string Id { get { return _id; } }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public double Width { get; set; }
        public string Name { get; set; }
        #endregion
        #region ICanvasLayer
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            Tracing.StartTrack(App.TracePaint);
            int cnt = 0;
            foreach (IDrawObject drawobject in _objects)
            {
                DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
                if (obj is IDrawObject && ((IDrawObject)obj).ObjectInRectangle(canvas, unitrect, true) == false)
                    continue;
                if (obj != null)
                {
                    bool sel = obj.Selected;
                    bool high = obj.Highlighted;
                    obj.Selected = false;
                    drawobject.Draw(canvas, unitrect);
                    obj.Selected = sel;
                    obj.Highlighted = high;
                }
                cnt++;
            }
            Tracing.EndTrack(App.TracePaint, "Draw Layer {0}, ObjCount {1}, Painted ObjCount {2}", Id, _objects.Count, cnt);
        }
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            foreach (IDrawObject obj in _objects)
            {
                ISnapPoint sp = obj.SnapPoint(canvas, point, otherobj, null, null);
                if (sp != null)
                    return sp;
            }
            return null;
        }
        public IEnumerable<IDrawObject> Objects 
        {
            get
            {
                return _objects;
            }
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            Rect bb = Rect.Empty;
            foreach (IDrawObject drawobject in _objects)
            {
                bb = WPFToFormConverter.unionRect(bb, drawobject.GetBoundingRect(canvas));
                Debug.Assert(!double.IsInfinity(bb.Bottom));
            }
            return bb;
        }
        public void GetHitObjects(List<IDrawObject> selected, ICanvas canvas, Rect selection, bool anyPoint)
        {
            foreach (IDrawObject drawobject in Objects)
            {
                if (drawobject.ObjectInRectangle(canvas, selection, anyPoint))
                    selected.Add(drawobject);
            }
        }
        public void GetHitObjects(List<IDrawObject> aHitObjects, ICanvas canvas, UnitPoint point)
        {
            foreach (IDrawObject drawobject in Objects)
            {
                if (drawobject.PointInObject(canvas, point))
                    aHitObjects.Add(drawobject);
            }
        }
        public void AddObject(IDrawObject drawobject)
        {
            System.Diagnostics.Debug.Assert(false);
        }
        public void Export(IExport export)
        {
            export.SinkLayer(this);
            foreach (IDrawObject drawobject in Objects)
                drawobject.Export(export);
            export.RiseLayer();
        }
        #endregion
        #region Methods
        public void DeleteObjects(IDrawObject objects)
        {
            _objects.Remove(objects);
        }
        public void AddVfkObjectFast(IDrawObject aObject)
        {
            _objects.Add(aObject);
        }
        #endregion
    }
}
