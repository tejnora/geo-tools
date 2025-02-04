using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using CAD.Canvas;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;
using Point = System.Drawing.Point;


namespace CAD.Canvas.Layers
{
    interface IImageBackgrounLayer
    {
        void AddImageObject(IDrawObject drawobject);
        IDrawObject DeleteImageObject(IDrawObject drawobject);
        IDrawObject getImageObject(int aIndex);
        int getImageObjectCount();
    }

    class ImageBackgroundLayer : IImageBackgrounLayer, ICanvasLayer, ISerialize
    {
        public ImageBackgroundLayer()
        {
            //            AddImageObject(new CAD.BackgrounImages.BackgrounImage(@"c:\test.cit"));
        }

        List<IDrawObject> _images = new List<IDrawObject>();
        Dictionary<IDrawObject, bool> _imagesMap = new Dictionary<IDrawObject, bool>();

                public void AddImageObject(IDrawObject drawobject)
        {
            if (_imagesMap.ContainsKey(drawobject))
                return; // this should never happen
            _images.Add(drawobject);
            _imagesMap[drawobject] = true;
        }

        public IDrawObject DeleteImageObject(IDrawObject drawobject)
        {
            if (_imagesMap.ContainsKey(drawobject))
            {
                _imagesMap.Remove(drawobject);
                _images.Remove(drawobject);
                return drawobject;
            }
            System.Diagnostics.Debug.Assert(false);
            return null;
        }

        public int getImageObjectCount()
        {
            return _images.Count;
        }

        public IDrawObject getImageObject(int aIndex)
        {
            if (_images.Count <= aIndex)
                return null;
            return _images[aIndex];
        }

                        public Color Color
        {
            get;
            set;
        }
        List<IDrawObject> _imageMap = new List<IDrawObject>();
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            Tools.Tracing.StartTrack(App.TracePaint);
            int cnt = 0;
            foreach (IDrawObject drawobject in _images)
            {
                DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
                drawobject.Draw(canvas, unitrect);
                cnt++;
            }
            Tools.Tracing.EndTrack(App.TracePaint, "Draw Layer {0}, ObjCount {1}, Painted ObjCount {2}", Id, _imageMap.Count, cnt);
        }
        public Point SnapPoint(Point unitmousepoint)
        {
            return new Point();
        }
        public string Id
        {
            get { return "imagebackground"; }
        }
        ISnapPoint ICanvasLayer.SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public IEnumerable<IDrawObject> Objects
        {
            get { return null; }
        }
        public bool Enabled
        {
            get { return true; }
            set { ;}
        }
        public bool Visible
        {
            get { return true; }
            set { ;}
        }

        public double Width
        {
            get { return 0; }
            set { }
        }
        public string Name
        {
            get { return "Backgroundlayer"; }
            set { }
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            Rect bb = Rect.Empty;
            foreach (IDrawObject drawobject in _images)
            {
                bb = Rect.Union(bb, drawobject.GetBoundingRect(canvas));
            }
            return bb;
        }
        public void GetHitObjects(List<IDrawObject> aHitObjects, ICanvas canvas, UnitPoint point)
        {
        }
        public void GetHitObjects(List<IDrawObject> selected, ICanvas canvas, Rect selection, bool anyPoint)
        {
        }
        public void AddObject(IDrawObject drawobject)
        {
        }
        public void Export(IExport export)
        {
        }
                        public void GetObjectData(XmlWriter wr)
        {
            wr.WriteStartElement("backgroundlayer");
            XmlUtil.WriteProperties(this, wr);
            wr.WriteEndElement();
        }
        public void AfterSerializedIn()
        {
        }
            }
}
