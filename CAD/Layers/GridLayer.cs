using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using CAD.Export;
using CAD.Utils;
using CAD.Canvas.DrawTools;
using GeoBase.Utils;
using Color = System.Drawing.Color;
using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
using Pen = System.Drawing.Pen;

namespace CAD.Canvas.Layers
{
    public class GridLayer : ICanvasLayer, ISerialize
    {
                public enum eStyle
        {
            Dots,
            Lines,
        }
        public Size m_spacing = new Size(1, 1); // 12"
        private bool m_enabled = false;
        private int m_minSize = 15;
        private eStyle m_gridStyle = eStyle.Lines;
        private Color m_color = Color.FromArgb(50, Color.Gray);
        [XmlSerializable]
        public Size Spacing
        {
            get { return m_spacing; }
            set { m_spacing = value; }
        }
        [XmlSerializable]
        public int MinSize
        {
            get { return m_minSize; }
            set { m_minSize = value; }
        }
        [XmlSerializable]
        public eStyle GridStyle
        {
            get { return m_gridStyle; }
            set { m_gridStyle = value; }
        }
        [XmlSerializable]
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }
                        public void Copy(GridLayer acopy)
        {
            m_enabled = acopy.m_enabled;
            m_spacing = acopy.m_spacing;
            m_minSize = acopy.m_minSize;
            m_gridStyle = acopy.m_gridStyle;
            m_color = acopy.m_color;
        }
        public void Draw(ICanvas canvas, Rect unitrect)
        {
            if (Enabled == false)
                return;
            double gridX = Spacing.Width;
            double gridY = Spacing.Height;
            double gridscreensizeX = canvas.ToScreen(gridX);
            double gridscreensizeY = canvas.ToScreen(gridY);
            if (gridscreensizeX < MinSize || gridscreensizeY < MinSize)
                return;

            Point leftpoint = unitrect.Location;
            Point rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

            double left = (float)Math.Round(leftpoint.X / gridX) * gridX;
            double top = unitrect.Height + unitrect.Y;
            double right = rightpoint.X;
            double bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;

            if (GridStyle == eStyle.Dots)
            {
                GDI gdi = new GDI();
                gdi.BeginGDI(canvas.Graphics);
                for (double x = left; x <= right; x += gridX)
                {
                    for (double y = bottom; y <= top; y += gridY)
                    {
                        Point p1 = canvas.ToScreen(new UnitPoint(x, y));
                        gdi.SetPixel((int)p1.X, (int)p1.Y, m_color.ToArgb());
                    }
                }
                gdi.EndGDI();
            }
            if (GridStyle == eStyle.Lines)
            {
                Pen pen = new Pen(m_color);
                GraphicsPath path = new GraphicsPath();

                // draw vertical lines
                while (left < right)
                {
                    Point p1 = canvas.ToScreen(new UnitPoint(left, leftpoint.Y));
                    Point p2 = canvas.ToScreen(new UnitPoint(left, rightpoint.Y));
                    path.AddLine(p1.FromWpfPoint(), p2.FromWpfPoint());
                    path.CloseFigure();
                    left += gridX;
                }

                // draw horizontal lines
                while (bottom < top)
                {
                    Point p1 = canvas.ToScreen(new UnitPoint(leftpoint.X, bottom));
                    Point p2 = canvas.ToScreen(new UnitPoint(rightpoint.X, bottom));
                    path.AddLine(p1.FromWpfPoint(), p2.FromWpfPoint());
                    path.CloseFigure();
                    bottom += gridY;
                }
                canvas.Graphics.DrawPath(pen, path);
            }
        }
        public string Id
        {
            get { return "grid"; }
        }
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            if (Enabled == false)
                return null;
            UnitPoint snappoint = new UnitPoint();
            UnitPoint mousepoint = point;
            double gridX = Spacing.Width;
            double gridY = Spacing.Height;
            if (canvas.ToScreen(gridX) < MinSize || canvas.ToScreen(gridY) < MinSize)
                return null;
            snappoint.X = (float)(Math.Round(mousepoint.X / gridX)) * gridX;
            snappoint.Y = (float)(Math.Round(mousepoint.Y / gridY)) * gridY;
            double threshold = canvas.ToUnit(/*ThresholdPixel*/6);
            if ((snappoint.X < point.X - threshold) || (snappoint.X > point.X + threshold))
                return null;
            if ((snappoint.Y < point.Y - threshold) || (snappoint.Y > point.Y + threshold))
                return null;
            return new GridSnapPoint(canvas, snappoint);
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
        public double Width
        {
            get { return 0; }
            set { }
        }
        public string Name
        {
            get { return "GridLayer"; }
            set { }
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            return Rect.Empty;
        }
        public IEnumerable<IDrawObject> Objects
        {
            get { return null; }
        }
        [XmlSerializable]
        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }
        public bool Visible
        {
            get { return true; }
            set { }
        }
        public void AddObject(IDrawObject drawobject)
        {
        }
        public void Export(IExport export)
        {
        }
                        public void GetObjectData(XmlWriter wr)
        {
            wr.WriteStartElement("gridlayer");
            XmlUtil.WriteProperties(this, wr);
            wr.WriteEndElement();
        }
        public void AfterSerializedIn()
        {
        }
            }
}
