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
    class NodePointLine : INodePoint
    {
        public enum EPoint
        {
            P1,
            P2,
        }
        static bool _angleLocked;
        Line _owner;
        Line _clone;
        UnitPoint _originalPoint;
        UnitPoint _endPoint;
        EPoint _pointId;
        public NodePointLine(Line owner, EPoint id)
        {
            _owner = owner;
            _clone = _owner.Clone() as Line;
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
            if (Control.ModifierKeys == Keys.Control)
                pos = HitUtil.OrthoPointD(OtherPoint(_pointId), pos, 45);
            if (_angleLocked || Control.ModifierKeys == (Keys.Control | Keys.Shift))
                pos = HitUtil.NearestPointOnLine(_owner.P1, _owner.P2, pos, true);
            SetPoint(_pointId, pos, _clone);
        }
        public void Finish()
        {
            _endPoint = GetPoint(_pointId);
            _owner.P1 = _clone.P1;
            _owner.P2 = _clone.P2;
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
            if (pointid == EPoint.P2)
                return _clone.P2;
            return _owner.P1;
        }
        protected UnitPoint OtherPoint(EPoint currentpointid)
        {
            if (currentpointid == EPoint.P1)
                return GetPoint(EPoint.P2);
            return GetPoint(EPoint.P1);
        }
        protected void SetPoint(EPoint pointid, UnitPoint point, Line line)
        {
            if (pointid == EPoint.P1)
                line.P1 = point;
            if (pointid == EPoint.P2)
                line.P2 = point;
        }
    }
    [Serializable]
    class Line : DrawObjectBase, IDrawObject
    {
        protected UnitPoint _p1, _p2;

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
        public Line()
        {
        }
        public Line(UnitPoint point, UnitPoint endpoint, float width, Color color)
        {
            P1 = point;
            P2 = endpoint;
            Width = width;
            Color = color;
            Selected = false;
        }
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            P1 = P2 = point;
            Width = layer.Width;
            Color = layer.Color;
            Selected = true;
        }
        public virtual void Copy(Line acopy)
        {
            base.Copy(acopy);
            _p1 = acopy._p1;
            _p2 = acopy._p2;
            Selected = acopy.Selected;
        }
        public virtual string Id
        {
            get { return DrawToolBar.Line.Name; }
        }
        public virtual IDrawObject Clone()
        {
            Line l = new Line();
            l.Copy(this);
            return l;
        }
        public Rect GetBoundingRect(ICanvas canvas)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            return ScreenUtils.GetRect(_p1, _p2, thWidth);
        }

        UnitPoint MidPoint(ICanvas canvas, UnitPoint p1, UnitPoint p2, UnitPoint hitpoint)
        {
            UnitPoint mid = HitUtil.LineMidpoint(p1, p2);
            double thWidth = ThresholdWidth(canvas, Width);
            if (HitUtil.CircleHitPoint(mid, thWidth, hitpoint))
                return mid;
            return UnitPoint.Empty;
        }
        public bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            return HitUtil.IsPointInLine(_p1, _p2, point, thWidth);
        }
        public bool ObjectInRectangle(ICanvas canvas, Rect rect, bool anyPoint)
        {
            Rect boundingrect = GetBoundingRect(canvas);
            if (anyPoint)
                return HitUtil.LineIntersectWithRect(_p1, _p2, rect);
            return rect.Contains(boundingrect);
        }
        public virtual void Draw(ICanvas canvas, Rect unitrect)
        {
            Color color = Color;
            Pen pen = canvas.CreatePen(color, (float)Width);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            canvas.DrawLine(canvas, pen, _p1, _p2);
            if (Highlighted)
                canvas.DrawLine(canvas, DrawUtils.SelectedPen, _p1, _p2);
            if (Selected)
            {
                canvas.DrawLine(canvas, DrawUtils.SelectedPen, _p1, _p2);
                if (_p1.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, _p1);
                if (_p2.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, _p2);
            }
        }
        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            if (Control.ModifierKeys == Keys.Control)
                point = HitUtil.OrthoPointD(_p1, point, 45);
            _p2 = point;
        }
        public virtual DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            Selected = false;
            if (snappoint is PerpendicularSnapPoint && snappoint.Owner is Line)
            {
                Line src = snappoint.Owner as Line;
                _p2 = HitUtil.NearestPointOnLine(src.P1, src.P2, _p1, true);
                return DrawObjectState.DoneRepeat;
            }
            if (snappoint is PerpendicularSnapPoint && snappoint.Owner is Arc)
            {
                Arc src = snappoint.Owner as Arc;
                _p2 = HitUtil.NearestPointOnCircle(src.Center, src.Radius, _p1, 0);
                return DrawObjectState.DoneRepeat;
            }
            if (Control.ModifierKeys == Keys.Control)
                point = HitUtil.OrthoPointD(_p1, point, 45);
            _p2 = point;
            return DrawObjectState.DoneRepeat;
        }

        public DrawObjectState OnFinish()
        {
            return DrawObjectState.Drop;
        }

        public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }
        public virtual void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
        public UnitPoint RepeatStartingPoint
        {
            get { return _p2; }
        }
        public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            double thWidth = ThresholdWidth(canvas, Width);
            if (HitUtil.CircleHitPoint(_p1, thWidth, point))
                return new NodePointLine(this, NodePointLine.EPoint.P1);
            if (HitUtil.CircleHitPoint(_p2, thWidth, point))
                return new NodePointLine(this, NodePointLine.EPoint.P2);
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
                        if (HitUtil.CircleHitPoint(_p1, thWidth, point))
                            return new VertextSnapPoint(canvas, this, _p1);
                        if (HitUtil.CircleHitPoint(_p2, thWidth, point))
                            return new VertextSnapPoint(canvas, this, _p2);
                    }
                    if (snaptype == typeof(MidpointSnapPoint))
                    {
                        UnitPoint p = MidPoint(canvas, _p1, _p2, point);
                        if (p != UnitPoint.Empty)
                            return new MidpointSnapPoint(canvas, this, p);
                    }
                    if (snaptype == typeof(IntersectSnapPoint))
                    {
                        Line otherline = CAD.Utils.Utils.FindObjectTypeInList(this, otherobjs, typeof(Line)) as Line;
                        if (otherline == null)
                            continue;
                        UnitPoint p = HitUtil.LinesIntersectPoint(_p1, _p2, otherline._p1, otherline._p2);
                        if (p != UnitPoint.Empty)
                            return new IntersectSnapPoint(canvas, this, p);
                    }
                }
                return null;
            }

            if (usersnaptype == typeof(MidpointSnapPoint))
                return new MidpointSnapPoint(canvas, this, HitUtil.LineMidpoint(_p1, _p2));
            if (usersnaptype == typeof(IntersectSnapPoint))
            {
                Line otherline = CAD.Utils.Utils.FindObjectTypeInList(this, otherobjs, typeof(Line)) as Line;
                if (otherline == null)
                    return null;
                UnitPoint p = HitUtil.LinesIntersectPoint(_p1, _p2, otherline._p1, otherline._p2);
                if (p != UnitPoint.Empty)
                    return new IntersectSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(VertextSnapPoint))
            {
                double d1 = HitUtil.Distance(point, _p1);
                double d2 = HitUtil.Distance(point, _p2);
                if (d1 <= d2)
                    return new VertextSnapPoint(canvas, this, _p1);
                return new VertextSnapPoint(canvas, this, _p2);
            }
            if (usersnaptype == typeof(NearestSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnLine(_p1, _p2, point);
                if (p != UnitPoint.Empty)
                    return new NearestSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(PerpendicularSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnLine(_p1, _p2, point);
                if (p != UnitPoint.Empty)
                    return new PerpendicularSnapPoint(canvas, this, p);
            }
            return null;
        }
        public void Move(UnitPoint offset)
        {
            _p1.X += offset.X;
            _p1.Y += offset.Y;
            _p2.X += offset.X;
            _p2.Y += offset.Y;
        }
        public bool getSelectDrawToolCreate()
        {
            return false;
        }
        public virtual string GetInfoAsString()
        {
            return string.Format("Line@{0},{1} - L={2:f4}<{3:f4}",
                P1.PosAsString(),
                P2.PosAsString(),
                HitUtil.Distance(P1, P2),
                HitUtil.RadiansToDegrees(HitUtil.LineAngleR(P1, P2, 0)));
        }
        public void Export(IExport export)
        {
            export.AddLine(P1, P2, Color, Width);
        }

        public void ExtendLineToPoint(UnitPoint newpoint)
        {
            UnitPoint newlinepoint = HitUtil.NearestPointOnLine(P1, P2, newpoint, true);
            if (HitUtil.Distance(newlinepoint, P1) < HitUtil.Distance(newlinepoint, P2))
                P1 = newlinepoint;
            else
                P2 = newlinepoint;
        }
        static int ThresholdPixel = 6;
        static double ThresholdWidth(ICanvas canvas, double objectwidth)
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
    class LineEdit : Line, IObjectEditInstance
    {
        public LineEdit(bool singleLine)
        {
            _singleLineSegment = singleLine;
        }
        PerpendicularSnapPoint _perSnap;
        TangentSnapPoint _tanSnap;
        bool _tanReverse;
        bool _singleLineSegment = true;
        public override string Id
        {
            get
            {
                if (_singleLineSegment)
                    return DrawToolBar.Line.Name;
                return DrawToolBar.MultiLine.Name;
            }
        }
        public override void InitializeFromModel(UnitPoint point, ICanvasLayer layer, ISnapPoint snap)
        {
            base.InitializeFromModel(point, layer, snap);
            _perSnap = snap as PerpendicularSnapPoint;
            _tanSnap = snap as TangentSnapPoint;
        }
        public override void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            if (_perSnap != null)
            {
                MouseMovePerpendicular(canvas, point);
                return;
            }
            if (_tanSnap != null)
            {
                MouseMoveTangent(canvas, point);
                return;
            }
            base.OnMouseMove(canvas, point);
        }
        public override DrawObjectState OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if (_tanSnap != null && Control.MouseButtons == MouseButtons.Right)
            {
                ReverseTangent(canvas);
                return DrawObjectState.Continue;
            }

            if (_perSnap != null || _tanSnap != null)
            {
                if (snappoint != null)
                    point = snappoint.SnapPoint;
                OnMouseMove(canvas, point);
                if (_singleLineSegment)
                    return DrawObjectState.Done;
                return DrawObjectState.DoneRepeat;
            }
            DrawObjectState result = base.OnMouseDown(canvas, point, snappoint);
            if (_singleLineSegment)
                return DrawObjectState.Done;
            return DrawObjectState.DoneRepeat;
        }
        public override void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && _tanSnap != null)
            {
                ReverseTangent(canvas);
                e.Handled = true;
            }
        }
        protected virtual void MouseMovePerpendicular(ICanvas canvas, UnitPoint point)
        {
            if (_perSnap.Owner is Line)
            {
                Line src = _perSnap.Owner as Line;
                _p1 = HitUtil.NearestPointOnLine(src.P1, src.P2, point, true);
                _p2 = point;
            }
            if (_perSnap.Owner is IArc)
            {
                IArc src = _perSnap.Owner as IArc;
                _p1 = HitUtil.NearestPointOnCircle(src.Center, src.Radius, point, 0);
                _p2 = point;
            }
        }
        protected virtual void MouseMoveTangent(ICanvas canvas, UnitPoint point)
        {
            if (_tanSnap.Owner is IArc)
            {
                IArc src = _tanSnap.Owner as IArc;
                _p1 = HitUtil.TangentPointOnCircle(src.Center, src.Radius, point, _tanReverse);
                _p2 = point;
                if (_p1 == UnitPoint.Empty)
                    _p2 = _p1 = src.Center;
            }
        }
        protected virtual void ReverseTangent(ICanvas canvas)
        {
            _tanReverse = !_tanReverse;
            MouseMoveTangent(canvas, _p2);
            canvas.Invalidate();
        }

        public void Copy(LineEdit acopy)
        {
            base.Copy(acopy);
            _perSnap = acopy._perSnap;
            _tanSnap = acopy._tanSnap;
            _tanReverse = acopy._tanReverse;
            _singleLineSegment = acopy._singleLineSegment;
        }
        public override IDrawObject Clone()
        {
            LineEdit l = new LineEdit(false);
            l.Copy(this);
            return l;
        }
        public IDrawObject GetDrawObject()
        {
            return new Line(P1, P2, (float)Width, Color);
        }
        public GUI.PpWindow GetPropPage(IModel aDataMode)
        {
            return null;
        }
        public bool HasPropPage()
        {
            return false;
        }
        public bool ValidateObjectContent()
        {
            return true;
        }
    }
}
