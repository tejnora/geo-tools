using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using CAD.Export;
using CAD.Utils;
using GeoBase.Utils;
using SolidBrush = System.Drawing.SolidBrush;
using Color = System.Drawing.Color;

namespace CAD.Canvas.Layers
{
    public class BackgroundLayer : ICanvasLayer, ISerialize
    {
        #region Constructors
        public BackgroundLayer()
        {
            _backgroundBrush = new SolidBrush(_color);
        }
        #endregion
        #region Properties & Fields
        SolidBrush _backgroundBrush;
        Color _color = Color.Black;
        [XmlSerializable]
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                _backgroundBrush = new SolidBrush(_color);
            }
        }
        #endregion
        #region ICanvasLayer Members
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            Rect r = ScreenUtils.ToScreenNormalized(canvas, unitrect);
            canvas.Graphics.FillRectangle(_backgroundBrush, r.ToRectangleF());
        }
        public Point SnapPoint(Point unitmousepoint)
        {
            return new Point();
        }
        public string Id
        {
            get { return "background"; }
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
            get { return false; }
            set { }
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
            return Rect.Empty;
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
        public void AddObject(IDrawObject drawobject)
        {
        }
        public void Export(IExport export)
        {
        }
        #endregion
        #region ISerialize
        public void GetObjectData(XmlWriter wr)
        {
            wr.WriteStartElement("backgroundlayer");
            XmlUtil.WriteProperties(this, wr);
            wr.WriteEndElement();
        }
        public void AfterSerializedIn()
        {
        }
        #endregion
    }
}
