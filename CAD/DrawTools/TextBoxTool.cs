using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Forms;
using CAD.Export;
using CAD.UITools;
using CAD.Utils;
using CAD.GUI;
using GeoBase.Utils;
using FontStyle = System.Drawing.FontStyle;
using Color = System.Drawing.Color;
using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
using StringFormat = System.Drawing.StringFormat;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Font = System.Drawing.Font;
using RectangleF = System.Drawing.RectangleF;

namespace CAD.Canvas.DrawTools
{
        public enum PointPositions
    {
        TopLeft,
        TopRight,
        Center,
        CenterLeft,
        CenterRight,
        CenterTop,
        CenterBottom,
        BottomLeft,
        BottomRight,
    };
        [Serializable]
    public class TextBox : DrawObjectBase, IDrawObject, IDeserializationCallback
    {
                public TextBox()
        {
            PointPosition = PointPositions.TopLeft;
            FontFamily = new System.Drawing.FontFamily("Arial");
            FontStyle = FontStyle.Regular;
            FontColor = Color.White;
            FontSize = 11.0;
            FontScale = 100;
            AngleOfRotation = 0.0;
            Text = string.Empty;
            Reset();
        }
                        private Size _size = new Size(); //pixel size
        protected UnitPoint _p1;
        public UnitPoint P1
        {
            get { return _p1; }
            set { _p1 = value; }
        }

        private bool ReloadBBValues { get; set; }
        public void Reset()
        {
            ReloadBBValues = true;
        }

        private string _text;
        public string Text
        {
            set { _text = value; }
            get { return _text; }
        }

        private string _fontFamyliString;
        [NonSerialized]
        private System.Drawing.FontFamily _fontFamily;
        public System.Drawing.FontFamily FontFamily
        { 
            get { return _fontFamily; }
            set { _fontFamily = value;
                _fontFamyliString = value.Name;
                }
        }

        private FontStyle _fontStyle; 
        public FontStyle FontStyle 
        { 
            get{ return _fontStyle;}
            set { _fontStyle = value; }
        }

        private Color _fontColor;
        public Color FontColor
        {
            get{ return _fontColor;}
            set { _fontColor = value; }
        }

        private double _fontSize;
        public Double FontSize
        {
            get{ return _fontSize;}
            set { _fontSize = value; }
        }

        private double _angleOfRotation;
        public Double AngleOfRotation
        {
            get{ return _angleOfRotation;}
            set { _angleOfRotation = value; }
        }

        private PointPositions _pointPosition;
        public PointPositions PointPosition
        {
            get{ return _pointPosition;}
            set { _pointPosition = value; }
        }

        private float _fontScale;
        public float FontScale
        {
            get{ return _fontScale;}
            set { _fontScale = value; }
        }
        [NonSerialized]
        GraphicsPath _mGP = new GraphicsPath();
        [NonSerialized]
        Rect _mBoundingBox;
                        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            Reset();
        }
        private Size getPointTransformation()
        {
            switch (PointPosition)
            {
                case PointPositions.TopLeft:
                    return new Size(0, 0);
                case PointPositions.TopRight:
                    return new Size(-_size.Width, 0);
                case PointPositions.Center:
                    return new Size(-_size.Width / 2, -_size.Height / 2);
                case PointPositions.BottomLeft:
                    return new Size(0, -_size.Height);
                case PointPositions.BottomRight:
                    return new Size(-_size.Width, -_size.Height);
                case PointPositions.CenterBottom:
                    return new Size(-_size.Width / 2, -_size.Height);
                case PointPositions.CenterTop:
                    return new Size(-_size.Width / 2, 0);
                case PointPositions.CenterLeft:
                    return new Size(0, -_size.Height / 2);
                case PointPositions.CenterRight:
                    return new Size(-_size.Width, -_size.Height / 2);
            }
            System.Diagnostics.Debug.Assert(false);
            return new Size();
        }

                        public virtual string Id
        {
            get { return DrawToolBar.TextBox.Name; }
        }
        public virtual IDrawObject Clone()
        {
            TextBox e = new TextBox();
            e.Copy(this);
            return e;
        }
        public virtual void Copy(TextBox acopy)
        {
            base.Copy(acopy);
            _p1 = acopy._p1;
            Text = acopy.Text;
            FontFamily = acopy.FontFamily;
            FontStyle = acopy.FontStyle;
            FontColor = acopy.FontColor;
            FontSize = acopy.FontSize;
            AngleOfRotation = acopy.AngleOfRotation;
            PointPosition = acopy.PointPosition;
            FontScale = acopy.FontScale;
        }

        public virtual bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            Rect rect = GetBoundingRect(canvas);
            return rect.Contains(new Point(point.X, point.Y));
        }

        public virtual bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            Rect boundingrect = GetBoundingRect(canvas);
            return rect.Contains(boundingrect) || boundingrect.Contains(rect);
        }
        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            if (Text == null || FontFamily == null) return;
            /*
            GraphicsPath gpDebug = new GraphicsPath();
            RectangleF invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(canvas, GetBoundingRect(canvas)));
            gpDebug.AddRectangle(invalidaterect);
            canvas.DrawPath(canvas, new Pen(Brushes.Red), gpDebug);
            */
            Point pointPos = canvas.ToScreen(P1);
            Size pointTr = new Size();
            pointTr.Width = canvas.ToScreen(getPointTransformation().Width);
            pointTr.Height = canvas.ToScreen(getPointTransformation().Height);
            StringFormat sf = new StringFormat();
            _mGP.Reset();
            _mGP.AddString(Text, FontFamily, (int)FontStyle, (float)FontSize, new System.Drawing.Point(0, 0), sf);
            Matrix matrix = new Matrix();
            matrix.Translate((float)(pointPos.X + pointTr.Width),(float)(pointPos.Y + pointTr.Height));
            matrix.Rotate((float)AngleOfRotation);
            float zoom = (float)canvas.getZoom();
            matrix.Scale(zoom * FontScale, zoom * FontScale);
            _mGP.Transform(matrix);
            canvas.FillPath(canvas, new System.Drawing.SolidBrush(FontColor), _mGP);
            if (Selected)
            {
                Rect bb = GetBoundingRect(canvas);
                canvas.Graphics.DrawRectangle(DrawUtils.SelectedPen, ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(canvas, bb)));
                DrawUtils.DrawNode(canvas, _p1);
                DrawUtils.DrawNode(canvas, _p1 + new UnitPoint(bb.Width, 0));
                DrawUtils.DrawNode(canvas, _p1 + new UnitPoint(bb.Width, -bb.Height));
                DrawUtils.DrawNode(canvas, _p1 + new UnitPoint(0, -bb.Height));
            }
        }

        public virtual Rect GetBoundingRect(ICanvas canvas)
        {
            if (ReloadBBValues)
            {
                Font font = new Font(FontFamily, (float)FontSize, FontStyle);
                Size layoutSize = new Size(10000.0F, 20000.0F);
                int charactersFitted;
                int linesFilled;
                _size = canvas.MeasureString(canvas, Text, font, layoutSize, new StringFormat(), out charactersFitted, out linesFilled);
                _size.Width = _size.Width / 72 * 0.0254f * FontScale;
                _size.Height = _size.Height / 72 * 0.0254f * FontScale;
                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(new RectangleF(0, 0, (float)_size.Width, (float)_size.Height));
                Matrix mx = new Matrix();
                mx.Rotate((float)AngleOfRotation);
                gp.Transform(mx);

                _mBoundingBox = gp.GetBounds().FromRectangleF();
                _size = _mBoundingBox.Size;
                mx.Reset();
                ReloadBBValues = false;
            }
            Matrix mx2 = new Matrix();
            Size size = getPointTransformation();
            mx2.Translate((float)(P1.X + size.Width), (float)(P1.Y - _mBoundingBox.Height - _mBoundingBox.Top * 2 - size.Height));
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(_mBoundingBox.ToRectangleF());
            path.Transform(mx2);
            return path.GetBounds().FromRectangleF();
        }

        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            _p1 = point;
        }

        public virtual DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            return DrawObjectState.DoneRepeat;
        }

        public DrawObjectState OnFinish()
        {
            throw new NotImplementedException();
        }

        public virtual void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        public virtual void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }

        public virtual UnitPoint RepeatStartingPoint
        {
            get { return _p1; }
        }

        public virtual INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            return null;
        }

        public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
            return null;
        }

        public virtual void Move(UnitPoint offset)
        {
            _p1.X += offset.X;
            _p1.Y += offset.Y;
        }
        public bool getSelectDrawToolCreate()
        {
            return true;
        }
        public virtual string GetInfoAsString()
        {
            return string.Format("TextBox@{0},{1}", _p1, Text);
        }
        public void Export(IExport export)
        {

            Font font = new Font(FontFamily, (float)FontSize, FontStyle);
            var height = FontSize / 72.0 * 0.0254f * FontScale;
            string styleName=export.AddStyle(false, 0.0, 1, 0.0, false, false, 0.2, FontFamily.Name);
            export.AddText(Text, P1.X, P1.Y, height, styleName, Color, 0);
        }
        public void OnDeserialization(object sender)
        {
            try
            {
                FontFamily = new System.Drawing.FontFamily(_fontFamyliString);
            }
            catch (Exception)
            {
                FontFamily = new System.Drawing.FontFamily("Arial");                
            }
            _mGP = new GraphicsPath();
            Reset();
        }

            }
    public class TextBoxEdit : TextBox, IObjectEditInstance
    {
                public void Copy(TextBoxEdit acopy)
        {
            base.Copy(acopy);
        }
        public override IDrawObject Clone()
        {
            TextBoxEdit l = new TextBoxEdit();
            l.Copy(this);
            return l;
        }
                        public IDrawObject GetDrawObject()
        {
            TextBox textBox = new TextBox();
            textBox.Copy(this);
            return textBox.Clone();
        }
        public PpWindow GetPropPage(IModel aDataMode)
        {
            return new TextBoxPropPage(aDataMode, this);
        }
        public bool HasPropPage()
        {
            return true;
        }
        public bool ValidateObjectContent()
        {
            return true;
        }
            }

}
