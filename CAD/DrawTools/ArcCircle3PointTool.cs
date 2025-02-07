using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Forms;
using CAD.Export;
using CAD.UITools;
using CAD.Utils;
using GeoBase.Utils;
using Pen = System.Drawing.Pen;

namespace CAD.Canvas.DrawTools
{
    class NodePointArc3PointPoint : INodePoint
    {
                public NodePointArc3PointPoint(Arc3Point owner, Arc3Point.ECurrentPoint curpoint)
        {
            Owner = owner;
            Clone = Owner.Clone() as Arc3Point;
            if(Clone!=null)
                Clone.Selected = true;
            Owner.Selected = false;
            OriginalPoints[0] = Owner.P1;
            OriginalPoints[1] = Owner.P2;
            OriginalPoints[2] = Owner.P3;
            CurPoint = curpoint;
        }
                        protected Arc3Point Owner;
        protected Arc3Point Clone;
        protected UnitPoint[] OriginalPoints = new UnitPoint[3];
        protected UnitPoint[] EndPoints = new UnitPoint[3];
        protected Arc3Point.ECurrentPoint CurPoint;
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
            SetPoint(Clone, pos);
        }
        /*
        UnitPoint GetPoint()
        {
            if (m_curPoint == Arc3Point.eCurrentPoint.p1)
                return m_clone.P1;
            if (m_curPoint == Arc3Point.eCurrentPoint.p2)
                return m_clone.P2;
            if (m_curPoint == Arc3Point.eCurrentPoint.p3)
                return m_clone.P3;
            if (m_curPoint == Arc3Point.eCurrentPoint.startangle)
                return m_clone.P1;
            if (m_curPoint == Arc3Point.eCurrentPoint.endangle)
                return m_clone.P3;
            if (m_curPoint == Arc3Point.eCurrentPoint.radius)
                return m_clone.P2;
            return UnitPoint.Empty;
        }
         * */
        void SetPoint(Arc3Point arc, UnitPoint pos)
        {
            if (CurPoint == Arc3Point.ECurrentPoint.P1)
                arc.P1 = pos;
            if (CurPoint == Arc3Point.ECurrentPoint.P2)
                arc.P2 = pos;
            if (CurPoint == Arc3Point.ECurrentPoint.P3)
                arc.P3 = pos;

            double angleToRound = 0;
            if (Control.ModifierKeys == Keys.Control)
                angleToRound = HitUtil.DegressToRadians(45);
            double angleR = HitUtil.LineAngleR(arc.Center, pos, angleToRound);

            if (CurPoint == Arc3Point.ECurrentPoint.Startangle)
                arc.P1 = HitUtil.PointOncircle(arc.Center, arc.Radius, angleR);
            if (CurPoint == Arc3Point.ECurrentPoint.Endangle)
                arc.P3 = HitUtil.PointOncircle(arc.Center, arc.Radius, angleR);
            if (CurPoint == Arc3Point.ECurrentPoint.Radius)
            {
                double radius = HitUtil.Distance(arc.Center, pos);
                arc.P1 = HitUtil.PointOncircle(arc.Center, radius, HitUtil.DegressToRadians(arc.StartAngle));
                arc.P2 = pos;
                arc.P3 = HitUtil.PointOncircle(arc.Center, radius, HitUtil.DegressToRadians(arc.EndAngle));
            }

            arc.UpdateArcFrom3Points();

            if ((CurPoint == Arc3Point.ECurrentPoint.Startangle) || (CurPoint == Arc3Point.ECurrentPoint.Endangle))
                arc.UpdateCenterNodeFromAngles();
        }
        public virtual void Finish()
        {
            EndPoints[0] = Clone.P1;
            EndPoints[1] = Clone.P2;
            EndPoints[2] = Clone.P3;
            Owner.Copy(Clone);
            Owner.Selected = true;
            Clone = null;
        }
        public void Cancel()
        {
            Owner.Selected = true;
        }
        public virtual void Undo()
        {
            Owner.P1 = OriginalPoints[0];
            Owner.P2 = OriginalPoints[1];
            Owner.P3 = OriginalPoints[2];
            Owner.UpdateArcFrom3Points();
        }
        public virtual void Redo()
        {
            Owner.P1 = EndPoints[0];
            Owner.P2 = EndPoints[1];
            Owner.P3 = EndPoints[2];
            Owner.UpdateArcFrom3Points();
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
            }
    [Serializable]
    class Arc3Point : DrawObjectBase, IArc, IDrawObject, IDeserializationCallback
    {
                public enum EArcType
        {
            KArc3P132,
            KArc3P123,
        }
        public enum EDirection
        {
            KCw,
            KCcw,
        }
        public enum ECurrentPoint
        {
            P1,
            P2,
            P3,
            Startangle,
            Endangle,
            Radius,
            Done,
        }
        public ECurrentPoint CurrentPoint
        {
            get { return CurPoint; }
            set { CurPoint = value; }
        }
                        EArcType _type = EArcType.KArc3P132;
        UnitPoint _p1 = UnitPoint.Empty;
        UnitPoint _p2 = UnitPoint.Empty;
        UnitPoint _p3 = UnitPoint.Empty;
        public UnitPoint P1
        {
            get { return _p1; }
            set { _p1 = value; }
        }
        public UnitPoint P2
        {
            get { return _p2; }
            set { _p2 = value; }
        }
        public UnitPoint P3
        {
            get { return _p3; }
            set { _p3 = value; }
        }

        UnitPoint _center;
        float _radius;
        float _startAngle;
        float _endAngle;
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
        EDirection _direction = EDirection.KCcw;
        public EDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        protected static int ThresholdPixel = 6;
        protected ECurrentPoint CurPoint = ECurrentPoint.Done;
        protected UnitPoint LastPoint = UnitPoint.Empty;
                        public Arc3Point()
        {
        }
        public Arc3Point(EArcType type)
        {
            _type = type;
            CurPoint = ECurrentPoint.P1;
        }
                        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            Width = layer.Width;
            Color = layer.Color;
            Selected = true;
            OnMouseDown(null, point, snap);
        }
                        public void Copy(Arc3Point acopy)
        {
            base.Copy(acopy);
            _p1 = acopy._p1;
            _p2 = acopy._p2;
            _p3 = acopy._p3;
            _center = acopy._center;
            _radius = acopy._radius;
            _startAngle = acopy._startAngle;
            _endAngle = acopy._endAngle;
            _direction = acopy._direction;
            CurPoint = acopy.CurPoint;
        }
        double GetSweep(double start, double end, EDirection direction)
        {
            double sweep = 360;
            if (start == end)
                return sweep;
            if (direction == EDirection.KCcw)
            {
                if (end >= start)
                    sweep = end - start;
                else
                    sweep = 360 - (start - end);
            }
            if (direction == EDirection.KCw)
            {
                if (end >= start)
                    sweep = -(360 - (end - start));
                else
                    sweep = -(start - end);
            }
            return sweep;
        }
        public float SweepAngle
        {
            get { return (float)GetSweep(StartAngle, EndAngle, Direction); }
        }
                        public virtual string Id
        {
            get { return DrawToolBar.Arc3Points.Name; }
        }
        public virtual IDrawObject Clone()
        {
            Arc3Point a = new Arc3Point(_type);
            a.Copy(this);
            return a;
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            double thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (thWidth < Width)
                thWidth = Width;

            Rect rect = Rect.Empty;

            // if one of the points is empty, then a straight line is drawn, so return bounding rect for a line
            if (_p2.IsEmpty || _p3.IsEmpty)
                rect = ScreenUtils.GetRect(_p1, LastPoint, thWidth);

            if (rect.IsEmpty)
            {
                double r = _radius + thWidth / 2;
                rect = HitUtil.CircleBoundingRect(_center, r);
                if (Selected)
                {
                    float w = (float)canvas.ToUnit(20); // include space for the 'extern' nodes
                    rect.Inflate(w, w);
                }
            }
            // if drawing either angle then include the mouse point in the rectangle - this is to redraw (erase) the line drawn
            // from center point to mouse point
            //if (m_curPoint == eCurrentPoint.startAngle || m_curPoint == eCurrentPoint.endAngle)
            if (LastPoint.IsEmpty == false)
                rect = Rect.Union(rect, new Rect(LastPoint.Point, new Size(0, 0)));
            return rect;
        }
        void DrawArc(ICanvas canvas)
        {
            Pen pen = canvas.CreatePen(Color, (float)Width);
            bool inline = PointsInLine();
            double sweep = GetSweep(StartAngle, EndAngle, Direction);

            if (inline == false)
                canvas.DrawArc(canvas, pen, _center, _radius, StartAngle, (float)sweep);
            else
            {
                canvas.DrawLine(canvas, pen, P1, P2);
                canvas.DrawLine(canvas, pen, P1, P3);
            }

            if (Selected)
            {
                if (inline == false)
                    canvas.DrawArc(canvas, DrawUtils.SelectedPen, _center, _radius, StartAngle, (float)sweep);
                else
                {
                    canvas.DrawLine(canvas, DrawUtils.SelectedPen, P1, P2);
                    canvas.DrawLine(canvas, DrawUtils.SelectedPen, P1, P3);
                }
                if (_p1.IsEmpty == false)
                {
                    DrawUtils.DrawNode(canvas, P1);
                    UnitPoint anglepoint = StartAngleNodePoint(canvas);
                    if (!anglepoint.IsEmpty)
                        DrawUtils.DrawTriangleNode(canvas, anglepoint);
                    anglepoint = EndAngleNodePoint(canvas);
                    if (!anglepoint.IsEmpty)
                        DrawUtils.DrawTriangleNode(canvas, anglepoint);
                    anglepoint = RadiusNodePoint(canvas);
                    if (!anglepoint.IsEmpty)
                        DrawUtils.DrawTriangleNode(canvas, anglepoint);
                }
                if (_p2.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, P2);
                if (_p3.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, P3);
            }
        }
        bool PointsInLine()
        {
            double slope1 = HitUtil.LineSlope(P1, P2);
            double slope2 = HitUtil.LineSlope(P1, P3);
            return slope1 == slope2;
        }
        void Draw3P132(ICanvas canvas)
        {
            if (CurPoint == ECurrentPoint.P3)
            {
                Pen pen = canvas.CreatePen(Color, (float)Width);
                canvas.DrawLine(canvas, pen, _p1, _p3);
            }
            if (CurPoint == ECurrentPoint.P2 || CurPoint == ECurrentPoint.Done)
            {
                DrawArc(canvas);
            }
        }
        void Draw3P123(ICanvas canvas)
        {
            if (CurPoint == ECurrentPoint.P2)
            {
                Pen pen = canvas.CreatePen(Color, (float)Width);
                canvas.DrawLine(canvas, pen, _p1, _p2);
            }
            if (CurPoint == ECurrentPoint.P3 || CurPoint == ECurrentPoint.Done)
            {
                DrawArc(canvas);
            }
        }
        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            if (_type == EArcType.KArc3P132)
                Draw3P132(canvas);
            if (_type == EArcType.KArc3P123)
                Draw3P123(canvas);
        }

        public void UpdateCenterNodeFromAngles()
        {
            float angle = StartAngle + SweepAngle / 2;
            P2 = HitUtil.PointOncircle(_center, _radius, HitUtil.DegressToRadians(angle));
        }
        public void UpdateArcFrom3Points()
        {
            _center = HitUtil.CenterPointFrom3Points(_p1, _p2, _p3);
            _radius = (float)HitUtil.Distance(_center, _p1);
            StartAngle = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(_center, _p1, 0));
            EndAngle = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(_center, _p3, 0));
            // find angle from P1 on line P1|P3 to P2. If this angle is 0-180 the direction is CCW else it is CW

            double p1P3Angle = HitUtil.RadiansToDegrees(HitUtil.LineAngleR(_p1, _p3, 0));
            double p1P2Angle = HitUtil.RadiansToDegrees(HitUtil.LineAngleR(_p1, _p2, 0));
            double diff = p1P3Angle - p1P2Angle;
            // I know there is a problem with this logic, in some cases the arc does not follow the mouse,
            // but it will work for now.
            Direction = EDirection.KCcw;
            if (diff < 0 || diff > 180)
                Direction = EDirection.KCw;

            if (p1P3Angle == 0)
            {
                if (diff < -180)
                    Direction = EDirection.KCcw;
                else
                    Direction = EDirection.KCw;
            }
            if (p1P3Angle == 90)
            {
                if (diff < -180)
                    Direction = EDirection.KCcw;
            }
        }
        void MoveMouse3P132(UnitPoint point)
        {
            if (CurPoint == ECurrentPoint.P1)
            {
                _p1 = point;
                return;
            }
            if (CurPoint == ECurrentPoint.P3)
            {
                _p3 = point;
                return;
            }
            if (CurPoint == ECurrentPoint.P2)
            {
                _p2 = point;
                UpdateArcFrom3Points();
                return;
            }
        }
        void MoveMouse3P123(UnitPoint point)
        {
            if (CurPoint == ECurrentPoint.P1)
            {
                _p1 = point;
                return;
            }
            if (CurPoint == ECurrentPoint.P2)
            {
                _p2 = point;
                return;
            }
            if (CurPoint == ECurrentPoint.P3)
            {
                _p3 = point;
                UpdateArcFrom3Points();
                return;
            }
        }
        DrawObjectState MouseDown3P132(ICanvas canvas, UnitPoint point)
        {
            OnMouseMove(canvas, point);
            if (CurPoint == ECurrentPoint.P1)
            {
                CurPoint = ECurrentPoint.P3;
                return DrawObjectState.Continue;
            }
            if (CurPoint == ECurrentPoint.P3)
            {
                CurPoint = ECurrentPoint.P2;
                return DrawObjectState.Continue;
            }
            if (CurPoint == ECurrentPoint.P2)
            {
                CurPoint = ECurrentPoint.Done;
                Selected = false;
                return DrawObjectState.Done;
            }
            return DrawObjectState.Done;
        }
        DrawObjectState MouseDown3P123(ICanvas canvas, UnitPoint point)
        {
            OnMouseMove(canvas, point);
            if (CurPoint == ECurrentPoint.P1)
            {
                CurPoint = ECurrentPoint.P2;
                return DrawObjectState.Continue;
            }
            if (CurPoint == ECurrentPoint.P2)
            {
                CurPoint = ECurrentPoint.P3;
                return DrawObjectState.Continue;
            }
            if (CurPoint == ECurrentPoint.P3)
            {
                CurPoint = ECurrentPoint.Done;
                Selected = false;
                return DrawObjectState.Done;
            }
            return DrawObjectState.Done;
        }

        UnitPoint StartAngleNodePoint(ICanvas canvas)
        {
            double r = Radius + canvas.ToUnit(8);
            return HitUtil.PointOncircle(_center, r, HitUtil.DegressToRadians(StartAngle));
        }
        UnitPoint EndAngleNodePoint(ICanvas canvas)
        {
            double r = Radius + canvas.ToUnit(8);
            return HitUtil.PointOncircle(_center, r, HitUtil.DegressToRadians(EndAngle));
        }
        UnitPoint RadiusNodePoint(ICanvas canvas)
        {
            double r = Radius + canvas.ToUnit(8);
            float angle = StartAngle + SweepAngle / 2;
            return HitUtil.PointOncircle(_center, r, HitUtil.DegressToRadians(angle));
        }
        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            LastPoint = point;
            if (_type == EArcType.KArc3P132)
                MoveMouse3P132(point);
            if (_type == EArcType.KArc3P123)
                MoveMouse3P123(point);
        }
        public virtual DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if (_type == EArcType.KArc3P132)
                return MouseDown3P132(canvas, point);
            if (_type == EArcType.KArc3P123)
                return MouseDown3P123(canvas, point);
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
            if (e.KeyCode == Keys.D)
            {
                if (Direction == EDirection.KCw)
                    Direction = EDirection.KCcw;
                else
                    Direction = EDirection.KCw;
                e.Handled = true;
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
            if (HitUtil.PointInPoint(P1, point, thWidth))
            {
                LastPoint = P1;
                return new NodePointArc3PointPoint(this, ECurrentPoint.P1);
            }
            if (HitUtil.PointInPoint(P2, point, thWidth))
            {
                LastPoint = P2;
                return new NodePointArc3PointPoint(this, ECurrentPoint.P2);
            }
            if (HitUtil.PointInPoint(P3, point, thWidth))
            {
                LastPoint = P3;
                return new NodePointArc3PointPoint(this, ECurrentPoint.P3);
            }
            UnitPoint p = StartAngleNodePoint(canvas);
            if (HitUtil.PointInPoint(p, point, thWidth))
            {
                LastPoint = p;
                return new NodePointArc3PointPoint(this, ECurrentPoint.Startangle);
            }
            p = EndAngleNodePoint(canvas);
            if (HitUtil.PointInPoint(p, point, thWidth))
            {
                LastPoint = p;
                return new NodePointArc3PointPoint(this, ECurrentPoint.Endangle);
            }
            p = RadiusNodePoint(canvas);
            if (HitUtil.PointInPoint(p, point, thWidth))
            {
                LastPoint = p;
                return new NodePointArc3PointPoint(this, ECurrentPoint.Radius);
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
                    /*
                    if (snaptype == typeof(QuadrantSnapPoint))
                    {
                        UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 90);
                        if (p != UnitPoint.Empty && HitUtil.PointInPoint(p, point, thWidth))
                            return new QuadrantSnapPoint(canvas, this, p);
                    }
                    if (snaptype == typeof(CenterSnapPoint))
                    {
                        if (HitUtil.PointInPoint(m_center, point, thWidth))
                            return new CenterSnapPoint(canvas, this, m_center);
                    }
                     * */
                    if (snaptype == typeof(VertextSnapPoint))
                    {
                        if (HitUtil.PointInPoint(P1, point, thWidth))
                            return new VertextSnapPoint(canvas, this, P1);
                        if (HitUtil.PointInPoint(P3, point, thWidth))
                            return new VertextSnapPoint(canvas, this, P3);
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
            P1 += offset;
            P2 += offset;
            P3 += offset;
            UpdateArcFrom3Points();
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
        public void OnDeserialization(object sender)
        {
            UpdateArcFrom3Points();

        }
            }
}
