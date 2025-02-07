using System;
using System.Windows;
using CAD.Utils;
using CAD.VFK;
using GeoBase.Utils;
using VFK;
using VFK.Tables;
using Pen = System.Drawing.Pen;
using Color = System.Drawing.Color;
using SizeF = System.Drawing.SizeF;

namespace CAD.Canvas.DrawTools
{
    public class DrawUtils
    {
        static Pen _selectedPen;
        static public Pen SelectedPen
        {
            get
            {
                if (_selectedPen == null)
                {
                    _selectedPen = new Pen(Color.Magenta, 1);
                    _selectedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                }
                return _selectedPen;
            }
        }
        static public void DrawNode(ICanvas canvas, UnitPoint nodepoint, Size inflate)
        {
            System.Drawing.RectangleF r = new System.Drawing.RectangleF(canvas.ToScreen(nodepoint).FromWpfPoint(), new System.Drawing.Size(0, 0));
            r.Inflate(new SizeF((float)inflate.Width,(float)inflate.Height));
            if (r.Right < 0 || r.Left > canvas.ClientRectangle.Width)
                return;
            if (r.Top < 0 || r.Bottom > canvas.ClientRectangle.Height)
                return;
            canvas.Graphics.FillRectangle(System.Drawing.Brushes.White, r);
            r.Inflate(1, 1);
            canvas.Graphics.DrawRectangle(System.Drawing.Pens.Black, ScreenUtils.ConvertRect(r));
        }
        static public void DrawNode(ICanvas canvas, UnitPoint nodepoint)
        {
            DrawNode(canvas, nodepoint, new Size(4, 4));
        }
        static public void DrawTriangleNode(ICanvas canvas, UnitPoint nodepoint)
        {
            Point screenpoint = canvas.ToScreen(nodepoint);
            float size = 4;
            System.Drawing.Point[] p =
                            {
                                new System.Drawing.Point((int)(screenpoint.X - size), (int)screenpoint.Y),
                                new System.Drawing.Point((int)screenpoint.X, (int)(screenpoint.Y + size)),
                                new System.Drawing.Point((int)(screenpoint.X + size), (int)screenpoint.Y),
                                new System.Drawing.Point((int)screenpoint.X, (int)(screenpoint.Y - size)),
                            };
            canvas.Graphics.FillPolygon(System.Drawing.Brushes.White, p);
        }
    }


    public interface IObjectEditInstance
    {
        IDrawObject GetDrawObject();
        GUI.PpWindow GetPropPage(IModel aDataMode);
        bool HasPropPage();
        bool ValidateObjectContent();
    }
    [Serializable]
    public abstract class DrawObjectBase
    {
        double _width;
        Color _color;
        ICanvasLayer _layer;
        [Flags]
        enum EFlags
        {
            Selected = 0x00000001,
            Highlighted = 0x00000002,
            UseLayerWidth = 0x00000004,
            UseLayerColor = 0x00000008,
        }
        int _flag;//(int)(EFlags.UseLayerWidth | EFlags.UseLayerColor);
        bool GetFlag(EFlags flag)
        {
            return (_flag & (int)flag) > 0;
        }
        void SetFlag(EFlags flag, bool enable)
        {
            if (enable)
                _flag |= (int)flag;
            else
                _flag &= ~(int)flag;
        }

        public bool UseLayerWidth
        {
            get { return GetFlag(EFlags.UseLayerWidth); }
            set { SetFlag(EFlags.UseLayerWidth, value); }
        }
        public bool UseLayerColor
        {
            get { return GetFlag(EFlags.UseLayerColor); }
            set { SetFlag(EFlags.UseLayerColor, value); }
        }
        public double Width
        {
            set { _width = value; }
            get
            {
                if (Layer != null && UseLayerWidth)
                    return Layer.Width;
                return _width;
            }
        }
        public Color Color
        {
            set { _color = value; }
            get
            {
                if (Layer != null && UseLayerColor)
                    return Layer.Color;
                return _color;
            }
        }
        public ICanvasLayer Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        abstract public void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap);
        public bool Selected
        {
            get { return GetFlag(EFlags.Selected); }
            set { SetFlag(EFlags.Selected, value); }
        }
        public virtual bool Highlighted
        {
            get { return GetFlag(EFlags.Highlighted); }
            set { SetFlag(EFlags.Highlighted, value); }
        }
        public virtual void Copy(DrawObjectBase acopy)
        {
            UseLayerColor = acopy.UseLayerColor;
            UseLayerWidth = acopy.UseLayerWidth;
            Width = acopy.Width;
            Color = acopy.Color;
        }
    }
}
