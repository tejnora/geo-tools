using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CAD.Export;
using CAD.UITools;
using CAD.Utils;
using CAD.GUI;
using GeoBase.Utils;

namespace CAD.Canvas.DrawTools
{
    public class NodePointAcitePoint : INodePoint
    {
                public enum EPoint
        {
            P1,
        }
                        static bool _angleLocked = false;
        ActivePoint _owner;
        ActivePoint _clone;
        UnitPoint _originalPoint;
        UnitPoint _endPoint;
        EPoint _pointId;
                        public NodePointAcitePoint(ActivePoint owner, EPoint id)
        {
            _owner = owner;
            _clone = _owner.Clone() as ActivePoint;
            _pointId = id;
            _originalPoint = GetPoint(_pointId);
        }
                        public IDrawObject GetClone()
        {
            return _clone;
        }
        public IDrawObject GetOriginal()
        {
            return _owner;
        }
        public void SetPosition(UnitPoint pos)
        {
            SetPoint(_pointId, pos, _clone);
        }
        public void Finish()
        {
            _endPoint = GetPoint(_pointId);
            _owner.P1 = _clone.P1;
            _clone = null;
        }
        public void Cancel()
        {
        }
        public void Undo()
        {
            SetPoint(_pointId, _originalPoint, _owner);
        }
        public void Redo()
        {
            SetPoint(_pointId, _endPoint, _owner);
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.L)
            {
                _angleLocked = !_angleLocked;
                e.Handled = true;
            }
        }
                        protected UnitPoint GetPoint(EPoint pointid)
        {
            if (pointid == EPoint.P1)
                return _clone.P1;
            return _owner.P1;
        }
        protected UnitPoint OtherPoint(EPoint currentpointid)
        {
            return GetPoint(EPoint.P1);
        }
        protected void SetPoint(EPoint pointid, UnitPoint point, ActivePoint aActivePoint)
        {
            if (pointid == EPoint.P1)
                aActivePoint.P1 = point;
        }
            }
    [Serializable]
    public class ActivePoint : DrawObjectBase, IDrawObject
    {
                public virtual UnitPoint P1
        {
            get;
            set;
        }
        public double Radius
        {
            get;
            set;
        }
        public bool DrawCircle
        {
            get;
            set;
        }
        public virtual string Id
        {
            get { return DrawToolBar.ActivePoint.Name; }
        }
                        public ActivePoint()
        {
        }
        public ActivePoint(UnitPoint point, double width, Color color, double aRadius)
        {
            P1 = point;
            Width = width;
            Color = color;
            Selected = false;
            Radius = aRadius;
            DrawCircle = true;
        }
                
        public virtual IDrawObject Clone()
        {
            ActivePoint l = new ActivePoint();
            l.Copy(this);
            return l;
        }

        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            P1 = point;
            Width = layer.Width;
            Color = layer.Color;
            Selected = true;
            Radius = 1.0f;
            DrawCircle = true;
        }

        public Rect GetBoundingRect(ICanvas canvas)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            double delta = Radius + Radius / 3.0;
            return ScreenUtils.GetRect(new UnitPoint(P1.X - delta, P1.Y - delta), new UnitPoint(P1.X + delta, P1.Y + delta), thWidth);
        }

        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            Rect rect = GetBoundingRect(canvas);
            return rect.Contains(point.Point);
        }

        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
//            Rect boundingrect = GetBoundingRect(canvas);
            return rect.Contains(P1.Point);
        }
        
        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            Color color = Color;
            Pen pen = canvas.CreatePen(color, (float)Width);
            pen.EndCap = LineCap.Flat;
            pen.StartCap = LineCap.Flat;
            double delta = Radius + Radius / 3.0;
            UnitPoint left = P1;
            left.X -= delta;
            UnitPoint right = P1;
            right.X += delta;
            UnitPoint top = P1;
            top.Y -= delta;
            UnitPoint bottom = P1;
            bottom.Y += delta;
            canvas.DrawLine(canvas, pen, left, right);
            canvas.DrawLine(canvas, pen, top, bottom);
            if(DrawCircle)
                canvas.DrawArc(canvas, pen, P1, (float)Radius, 0.0f, 360.0f);
            if (Selected)
            {
                if (P1.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, P1);
            }
             
        }

        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            P1 = point;
        }

        public virtual eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            return eDrawObjectMouseDown.DoneRepeat;
        }

        public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        public virtual void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }

        public UnitPoint RepeatStartingPoint
        {
            get { return new UnitPoint(); }
        }

        public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            if (HitUtil.CircleHitPoint(P1, thWidth, point))
                return new NodePointAcitePoint(this, NodePointAcitePoint.EPoint.P1);
            return null;
        }

        public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobjs, Type[] runningsnaptypes, Type usersnaptype)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            if (runningsnaptypes != null)
            {
                foreach (Type snaptype in runningsnaptypes)
                {
                    if (snaptype == typeof(VertextSnapPoint))
                    {
                        if (HitUtil.CircleHitPoint(P1, thWidth, point))
                            return new VertextSnapPoint(canvas, this, P1);
                    }
                }
                return null;
            }
            if (usersnaptype == typeof(VertextSnapPoint))
            {
                return new VertextSnapPoint(canvas, this, P1);
            }
            return null;
        }

        public void Move(UnitPoint offset)
        {
            P1 = new UnitPoint(P1.X + offset.X, P1.Y + offset.Y);
        }

        public string GetInfoAsString()
        {
            return string.Format("Activepoint@{0}",
                P1.PosAsString());
        }
        public bool getSelectDrawToolCreate()
        {
            return true;
        }
        public virtual void Copy(ActivePoint acopy)
        {
            base.Copy(acopy);
            P1 = acopy.P1;
            Radius = acopy.Radius;
            Selected = acopy.Selected;
            DrawCircle = acopy.DrawCircle;
        }
        public virtual void Export(IExport export)
        {
        }
                        static int ThresholdPixel = 6;
        public static double ThresholdWidth(ICanvas canvas, double objectwidth)
        {
            return ThresholdWidth(canvas, objectwidth, ThresholdPixel);
        }
        public static double ThresholdWidth(ICanvas canvas, double objectwidth, double pixelwidth)
        {
            double minWidth = canvas.ToUnit(pixelwidth);
            double width = Math.Max(objectwidth / 2, minWidth);
            return (float)width;
        }
    }
    public class ActivePointEdit : ActivePoint, IObjectEditInstance
    {
                public override string Id
        {
            get
            {
                return "activePoint";
            }
        }
                        public void Copy(ActivePointEdit acopy)
        {
            base.Copy(acopy);
        }
        public override IDrawObject Clone()
        {
            ActivePointEdit l = new ActivePointEdit();
            l.Copy(this);
            return l;
        }
                        public IDrawObject GetDrawObject()
        {
            return new ActivePoint(P1, Width, Color, Radius);
        }
        public PpWindow GetPropPage(IModel aDataMode)
        {
            return new ActivePointPropPage(this);
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
