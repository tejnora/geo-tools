using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using CAD.Export;
using CAD.UITools;
using CAD.Utils;
using GeoBase.Utils;

namespace CAD.Canvas.DrawTools
{
    interface IArc
    {
        UnitPoint Center { get; }
        float Radius { get; }
        float StartAngle { get; }
        float EndAngle { get; }
    }
    class NodePointArcCenter : INodePoint
    {
        public NodePointArcCenter(Arc owner)
        {
            Owner = owner;
            Clone = Owner.Clone() as Arc;
            OriginalPoint = Owner.Center;
        }
        protected Arc Owner;
        protected Arc Clone;
        protected UnitPoint OriginalPoint;
        protected UnitPoint EndPoint;
        public IDrawObject GetClone()
        {
            return Clone;
        }
        public IDrawObject GetOriginal()
        {
            return Owner;
        }
        public virtual void SetPosition(UnitPoint pos)
        {
            Clone.Center = pos;
        }
        public virtual void Finish()
        {
            EndPoint = Clone.Center;
            Owner.Center = Clone.Center;
            Owner.Radius = Clone.Radius;
            Owner.Selected = true;
            Clone = null;
        }
        public void Cancel()
        {
            Owner.Selected = true;
        }
        public virtual void Undo()
        {
            Owner.Center = OriginalPoint;
        }
        public virtual void Redo()
        {
            Owner.Center = EndPoint;
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
    }
    class NodePointArcRadius : INodePoint
    {
        public NodePointArcRadius(Arc owner)
        {
            Owner = owner;
            Clone = Owner.Clone() as Arc;
            if (Clone != null)
                Clone.CurrentPoint = Owner.CurrentPoint;
            OriginalValue = Owner.Radius;
        }
        protected Arc Owner;
        protected Arc Clone;
        protected float OriginalValue;
        protected float EndValue;
        public IDrawObject GetClone()
        {
            return Clone;
        }
        public IDrawObject GetOriginal()
        {
            return Owner;
        }
        public virtual void SetPosition(UnitPoint pos)
        {
            Clone.OnMouseMove(null, pos);
        }
        public virtual void Finish()
        {
            EndValue = Clone.Radius;
            Owner.Radius = Clone.Radius;
            Owner.Selected = true;
            Clone = null;
        }
        public void Cancel()
        {
            Owner.Selected = true;
        }
        public virtual void Undo()
        {
            Owner.Radius = OriginalValue;
        }
        public virtual void Redo()
        {
            Owner.Radius = EndValue;
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
    }
    class NodePointArcAngle : INodePoint
    {
        public NodePointArcAngle(Arc owner)
        {
            Owner = owner;
            Clone = Owner.Clone() as Arc;
            if (Clone != null)
                Clone.CurrentPoint = Owner.CurrentPoint;
            OriginalA1 = Owner.StartAngle;
            OriginalA2 = Owner.EndAngle;
            Owner.Selected = false;
        }
        protected Arc Owner;
        protected Arc Clone;
        protected float OriginalA1;
        protected float EndA1;
        protected float OriginalA2;
        protected float EndA2;
        public IDrawObject GetClone()
        {
            return Clone;
        }
        public IDrawObject GetOriginal()
        {
            return Owner;
        }
        public virtual void SetPosition(UnitPoint pos)
        {
            Clone.OnMouseMove(null, pos);
        }
        public virtual void Finish()
        {
            EndA1 = Clone.StartAngle;
            EndA2 = Clone.EndAngle;
            Owner.Copy(Clone);
            Clone = null;
        }
        public void Cancel()
        {
            Owner.Selected = true;
        }
        public virtual void Undo()
        {
            Owner.StartAngle = OriginalA1;
            Owner.EndAngle = OriginalA2;
        }
        public virtual void Redo()
        {
            Owner.StartAngle = EndA1;
            Owner.EndAngle = EndA2;
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
    }

    [Serializable]
    class Arc : DrawObjectBase, IArc, IDrawObject
    {
        public enum EDirection
        {
            KCw,
            KCcw,
        }
        public enum ECurrentPoint
        {
            P1,
            P2,
            Center,
            Radius,
            StartAngle,
            EndAngle,
            Done,
        }
        public enum EArcType
        {
            TypeCenterRadius,
            Type2Point,
        }
        UnitPoint _center;
        float _radius;
        float _startAngle;
        float _endAngle;
        EDirection _direction = EDirection.KCcw;
        public UnitPoint Center
        {
            get { return _center; }
            set { _center = value; }
        }
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public float StartAngle
        {
            get { return _startAngle; }
            set { _startAngle = value; }
        }
        public float EndAngle
        {
            get { return _endAngle; }
            set { _endAngle = value; }
        }
        public EDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        protected static int ThresholdPixel = 6;
        protected static int Divisions = 8;
        public ECurrentPoint CurrentPoint
        {
            get { return CurPoint; }
            set { CurPoint = value; }
        }
        protected UnitPoint RadiusPoint
        {
            get
            {
                float angle = StartAngle + SweepAngle / 2;
                return AnglePoint(angle);
            }
        }
        protected UnitPoint StartAnglePoint
        {
            get { return AnglePoint(StartAngle); }
        }
        protected UnitPoint EndAnglePoint
        {
            get { return AnglePoint(EndAngle); }
        }
        protected EArcType ArcType = EArcType.Type2Point;
        protected ECurrentPoint CurPoint = ECurrentPoint.P1;
        protected UnitPoint LastPoint;
        protected UnitPoint P1;
        public Arc()
        {
        }
        public Arc(EArcType type)
        {
            ArcType = type;
            CurPoint = ECurrentPoint.P1;
            if (ArcType == EArcType.TypeCenterRadius)
                CurPoint = ECurrentPoint.Center;
        }
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            Width = layer.Width;
            Color = layer.Color;
            OnMouseDown(null, point, snap);
            Selected = true;
        }
        public void Copy(Arc acopy)
        {
            base.Copy(acopy);
            Center = acopy.Center;
            Radius = acopy.Radius;
            StartAngle = acopy.StartAngle;
            EndAngle = acopy.EndAngle;
            Selected = acopy.Selected;
            Direction = acopy.Direction;
            ArcType = acopy.ArcType;
            CurPoint = acopy.CurPoint;
        }
        public virtual string Id
        {
            get { return DrawToolBar.Arc.Name; }
        }
        public virtual IDrawObject Clone()
        {
            Arc a = new Arc();
            a.Copy(this);
            return a;
        }
        public float SweepAngle
        {
            get
            {
                float sweep = 360;
                if (StartAngle == EndAngle)
                    return sweep;
                if (Direction == EDirection.KCcw)
                {
                    if (EndAngle >= StartAngle)
                        sweep = EndAngle - StartAngle;
                    else
                        sweep = 360 - (StartAngle - EndAngle);
                }
                if (Direction == EDirection.KCw)
                {
                    if (EndAngle >= StartAngle)
                        sweep = -(360 - (EndAngle - StartAngle));
                    else
                        sweep = -(StartAngle - EndAngle);
                }
                return sweep;
            }
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            double thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (thWidth < Width)
                thWidth = Width;
            double r = _radius + thWidth / 2;
            Rect rect = HitUtil.CircleBoundingRect(_center, r);
            // if drawing either angle then include the mouse point in the ractangle - this is to redraw (erase) the line drawn
            // from center point to mouse point
            if (CurPoint == ECurrentPoint.StartAngle || CurPoint == ECurrentPoint.EndAngle)
                rect = Rect.Union(rect, new Rect(LastPoint.Point, new System.Windows.Size()));
            return rect;
        }
        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            float sweep = SweepAngle;
            Pen pen = canvas.CreatePen(Color, (float)Width);
            canvas.DrawArc(canvas, pen, _center, _radius, StartAngle, sweep);
            if (Selected)
            {
                canvas.DrawArc(canvas, DrawUtils.SelectedPen, _center, _radius, StartAngle, sweep);
                DrawUtils.DrawNode(canvas, _center);
                DrawNodes(canvas);
            }
        }
        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            LastPoint = point;
            if (CurPoint == ECurrentPoint.P1)
            {
                P1 = point;
                return;
            }
            if (CurPoint == ECurrentPoint.P2)
            {
                StartAngle = 0;
                EndAngle = 360;
                _center = HitUtil.LineMidpoint(P1, point);
                _radius = (float)HitUtil.Distance(_center, point);
                return;
            }
            if (CurPoint == ECurrentPoint.Center)
            {
                _center = point;
            }
            if (CurPoint == ECurrentPoint.Radius)
            {
                //StartAngle = 0;
                //EndAngle = 360;
                _radius = (float)HitUtil.Distance(_center, point);
            }

            double angleToRound = 0;
            if (Control.ModifierKeys == Keys.Control)
                angleToRound = HitUtil.DegressToRadians(45);

            if (CurPoint == ECurrentPoint.StartAngle)
            {
                StartAngle = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(_center, point, angleToRound));
            }
            if (CurPoint == ECurrentPoint.EndAngle)
            {
                EndAngle = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(_center, point, angleToRound));
            }
        }
        public virtual DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            OnMouseMove(canvas, point);
            if (ArcType == EArcType.Type2Point)
            {
                if (CurPoint == ECurrentPoint.P1)
                {
                    CurPoint = ECurrentPoint.P2;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.P2)
                {
                    CurPoint = ECurrentPoint.StartAngle;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.StartAngle)
                {
                    CurPoint = ECurrentPoint.EndAngle;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.EndAngle)
                {
                    CurPoint = ECurrentPoint.Done;
                    OnMouseMove(canvas, point);
                    Selected = false;
                    return DrawObjectState.Done;
                }
            }
            if (ArcType == EArcType.TypeCenterRadius)
            {
                if (CurPoint == ECurrentPoint.Center)
                {
                    CurPoint = ECurrentPoint.Radius;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.Radius)
                {
                    CurPoint = ECurrentPoint.StartAngle;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.StartAngle)
                {
                    CurPoint = ECurrentPoint.EndAngle;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.EndAngle)
                {
                    CurPoint = ECurrentPoint.Done;
                    OnMouseMove(canvas, point);
                    Selected = false;
                    return DrawObjectState.Done;
                }
            }
            return DrawObjectState.Done;
        }

        public DrawObjectState OnFinish()
        {
            return DrawObjectState.Drop;
        }

        public virtual void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }
        public virtual void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && (CurPoint == ECurrentPoint.StartAngle || CurPoint == ECurrentPoint.EndAngle))
            {
                if (Direction == EDirection.KCw)
                    Direction = EDirection.KCcw;
                else
                    Direction = EDirection.KCw;
                e.Handled = true;
            }
            if (e.KeyCode == Keys.C) // switch to centerRadius mode
            {
                _center = UnitPoint.Empty;
                _radius = 0;
                _startAngle = 0;
                _endAngle = 0;
                ArcType = EArcType.TypeCenterRadius;
                CurPoint = ECurrentPoint.Center;
                e.Handled = true;
                canvas.Invalidate();
            }
            if (e.KeyCode == Keys.D2) // switch to 2 pointmode
            {
                _center = UnitPoint.Empty;
                _radius = 0;
                _startAngle = 0;
                _endAngle = 0;
                ArcType = EArcType.Type2Point;
                CurPoint = ECurrentPoint.P1;
                e.Handled = true;
                canvas.Invalidate();
            }
        }
        public virtual bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            Rect boundingrect = GetBoundingRect(canvas);
            if (boundingrect.Contains(point.Point) == false)
                return false;
            double thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (HitUtil.PointInPoint(_center, point, thWidth))
                return true;
            return HitUtil.IsPointInCircle(_center, _radius, point, thWidth / 2);
        }
        public virtual bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            double r = _radius + Width / 2;
            Rect boundingrect = HitUtil.CircleBoundingRect(_center, r);
            if (anyPoint)
            {
                UnitPoint lp1 = new UnitPoint(rect.Left, rect.Top);
                UnitPoint lp2 = new UnitPoint(rect.Left, rect.Bottom);
                if (HitUtil.CircleIntersectWithLine(_center, _radius, lp1, lp2))
                    return true;
                lp1 = new UnitPoint(rect.Left, rect.Bottom);
                lp2 = new UnitPoint(rect.Right, rect.Bottom);
                if (HitUtil.CircleIntersectWithLine(_center, _radius, lp1, lp2))
                    return true;
                lp1 = new UnitPoint(rect.Left, rect.Top);
                lp2 = new UnitPoint(rect.Right, rect.Top);
                if (HitUtil.CircleIntersectWithLine(_center, _radius, lp1, lp2))
                    return true;
                lp1 = new UnitPoint(rect.Left, rect.Top);
                lp2 = new UnitPoint(rect.Right, rect.Top);
                if (HitUtil.CircleIntersectWithLine(_center, _radius, lp1, lp2))
                    return true;

                lp1 = new UnitPoint(rect.Right, rect.Top);
                lp2 = new UnitPoint(rect.Right, rect.Bottom);
                if (HitUtil.CircleIntersectWithLine(_center, _radius, lp1, lp2))
                    return true;
            }
            return rect.Contains(boundingrect);
        }
        public virtual UnitPoint RepeatStartingPoint
        {
            get { return UnitPoint.Empty; }
        }
        public virtual INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            double thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (HitUtil.PointInPoint(_center, point, thWidth))
                return new NodePointArcCenter(this);
            if (HitUtil.PointInPoint(StartAnglePoint, point, thWidth))
            {
                CurPoint = ECurrentPoint.StartAngle;
                LastPoint = _center;
                return new NodePointArcAngle(this);
            }
            if (HitUtil.PointInPoint(EndAnglePoint, point, thWidth))
            {
                CurPoint = ECurrentPoint.EndAngle;
                LastPoint = _center;
                return new NodePointArcAngle(this);
            }
            if (HitUtil.PointInPoint(RadiusPoint, point, thWidth))
            {
                CurPoint = ECurrentPoint.Radius;
                LastPoint = _center;
                return new NodePointArcRadius(this);
            }
            return null;
        }
        public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
            double thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (runningsnaptypes != null)
            {
                foreach (Type snaptype in runningsnaptypes)
                {
                    if (snaptype == typeof(QuadrantSnapPoint))
                    {
                        UnitPoint p = HitUtil.NearestPointOnCircle(_center, _radius, point, 90);
                        if (p != UnitPoint.Empty && HitUtil.PointInPoint(p, point, thWidth))
                            return new QuadrantSnapPoint(canvas, this, p);
                    }
                    if (snaptype == typeof(DivisionSnapPoint))
                    {
                        double angle = 360.0 / Divisions;
                        UnitPoint p = HitUtil.NearestPointOnCircle(_center, _radius, point, angle);
                        if (p != UnitPoint.Empty && HitUtil.PointInPoint(p, point, thWidth))
                            return new DivisionSnapPoint(canvas, this, p);
                    }
                    if (snaptype == typeof(CenterSnapPoint))
                    {
                        if (HitUtil.PointInPoint(_center, point, thWidth))
                            return new CenterSnapPoint(canvas, this, _center);
                    }
                    if (snaptype == typeof(VertextSnapPoint))
                    {
                        if (HitUtil.PointInPoint(StartAnglePoint, point, thWidth))
                            return new VertextSnapPoint(canvas, this, StartAnglePoint);
                        if (HitUtil.PointInPoint(EndAnglePoint, point, thWidth))
                            return new VertextSnapPoint(canvas, this, EndAnglePoint);
                    }
                }
                return null;
            }
            if (usersnaptype == typeof(NearestSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnCircle(_center, _radius, point, 0);
                if (p != UnitPoint.Empty)
                    return new NearestSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(PerpendicularSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnCircle(_center, _radius, point, 0);
                if (p != UnitPoint.Empty)
                    return new PerpendicularSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(QuadrantSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnCircle(_center, _radius, point, 90);
                if (p != UnitPoint.Empty)
                    return new QuadrantSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(DivisionSnapPoint))
            {
                double angle = 360.0 / Divisions;
                UnitPoint p = HitUtil.NearestPointOnCircle(_center, _radius, point, angle);
                if (p != UnitPoint.Empty)
                    return new DivisionSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(TangentSnapPoint))
            {
                IDrawObject drawingObject = canvas.CurrentObject;
                UnitPoint p = UnitPoint.Empty;
                if (drawingObject is LineEdit)
                {
                    UnitPoint mousepoint = point;
                    point = ((LineEdit)drawingObject).P1;
                    UnitPoint p1 = HitUtil.TangentPointOnCircle(_center, _radius, point, false);
                    UnitPoint p2 = HitUtil.TangentPointOnCircle(_center, _radius, point, true);
                    double d1 = HitUtil.Distance(mousepoint, p1);
                    double d2 = HitUtil.Distance(mousepoint, p2);
                    if (d1 <= d2)
                        return new TangentSnapPoint(canvas, this, p1);
                    return new TangentSnapPoint(canvas, this, p2);
                }
                //if (p != PointF.Empty)
                return new TangentSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(CenterSnapPoint))
            {
                return new CenterSnapPoint(canvas, this, _center);
            }
            return null;
        }
        public virtual void Move(UnitPoint offset)
        {
            _center.X += offset.X;
            _center.Y += offset.Y;
            LastPoint = _center;
        }
        public bool getSelectDrawToolCreate()
        {
            return false;
        }
        public virtual string GetInfoAsString()
        {
            return string.Format("Arc@{0}, r={1:f4}, A1={2:f4}, A2={3:f4}", Center.PosAsString(), Radius, StartAngle, EndAngle);
        }
        public void Export(IExport export)
        {
        }
        protected virtual void DrawNodes(ICanvas canvas)
        {
            if (CurPoint == ECurrentPoint.StartAngle && LastPoint != UnitPoint.Empty)
                canvas.DrawLine(canvas, DrawUtils.SelectedPen, _center, LastPoint);
            if (CurPoint == ECurrentPoint.EndAngle && LastPoint != UnitPoint.Empty)
                canvas.DrawLine(canvas, DrawUtils.SelectedPen, _center, LastPoint);

            DrawUtils.DrawNode(canvas, StartAnglePoint);
            DrawUtils.DrawNode(canvas, EndAnglePoint);
            DrawUtils.DrawNode(canvas, RadiusPoint);
        }
        protected UnitPoint AnglePoint(float angle)
        {
            return HitUtil.PointOncircle(_center, _radius, HitUtil.DegressToRadians(angle));
        }
    }

    [Serializable]
    class Circle : Arc
    {
        public Circle()
        {
        }
        public Circle(EArcType type)
            : base(type)
        {
        }
        public static string ObjectType
        {
            get { return "circle"; }
        }
        public override string Id
        {
            get { return ObjectType; }
        }
        public override IDrawObject Clone()
        {
            Circle a = new Circle();
            a.Copy(this);
            return a;
        }
        public override DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            OnMouseMove(canvas, point);
            if (ArcType == EArcType.Type2Point)
            {
                if (CurPoint == ECurrentPoint.P1)
                {
                    CurPoint = ECurrentPoint.P2;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.P2)
                {
                    CurPoint = ECurrentPoint.Done;
                    OnMouseMove(canvas, point);
                    Selected = false;
                    return DrawObjectState.Done;
                }
            }
            if (ArcType == EArcType.TypeCenterRadius)
            {
                if (CurPoint == ECurrentPoint.Center)
                {
                    CurPoint = ECurrentPoint.Radius;
                    return DrawObjectState.Continue;
                }
                if (CurPoint == ECurrentPoint.Radius)
                {
                    CurPoint = ECurrentPoint.Done;
                    OnMouseMove(canvas, point);
                    Selected = false;
                    return DrawObjectState.Done;
                }
            }
            return DrawObjectState.Done;
        }
        protected override void DrawNodes(ICanvas canvas)
        {
            DrawUtils.DrawNode(canvas, AnglePoint(0));
            DrawUtils.DrawNode(canvas, AnglePoint(90));
            DrawUtils.DrawNode(canvas, AnglePoint(180));
            DrawUtils.DrawNode(canvas, AnglePoint(270));
        }
        public override INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            double thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (HitUtil.PointInPoint(Center, point, thWidth))
                return new NodePointArcCenter(this);
            bool radiushit = HitUtil.PointInPoint(AnglePoint(0), point, thWidth);
            if (radiushit == false)
                radiushit = HitUtil.PointInPoint(AnglePoint(90), point, thWidth);
            if (radiushit == false)
                radiushit = HitUtil.PointInPoint(AnglePoint(180), point, thWidth);
            if (radiushit == false)
                radiushit = HitUtil.PointInPoint(AnglePoint(270), point, thWidth);
            if (radiushit)
            {
                CurPoint = ECurrentPoint.Radius;
                LastPoint = Center;
                return new NodePointArcRadius(this);
            }
            return null;
        }
        public override string GetInfoAsString()
        {
            return string.Format("Circle@{0}, r={1:f4}", Center.PosAsString(), Radius);
        }
    }
}
