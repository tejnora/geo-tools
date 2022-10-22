using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeoCalculations.Utils;

namespace GeoCalculations.BasicTypes
{
    public class Path<TPoint>
    {
        public enum PathTypes
        {
            Move = 0,
            Line = 1,
            Bezier1 = 2,
            Bezier2 = 3,
            Bezier3 = 4,
            Conic1 = 5,
            Conic2 = 6
        };

        protected class PointPath
        {
            public TPoint Point;
            public PathTypes Type;
        };


        protected List<PointPath> Points = new List<PointPath>();

        public bool LineOnly { get; private set; }
        public int Count { get { return Points.Count; } }

        public Path()
        {
            LineOnly = true;
        }

        public TPoint this[int idx]
        {
            get { return Points[idx].Point; }
            set { Points[idx].Point = value; }
        }

        public PathTypes GetPointType(int idx)
        {
            return Points[idx].Type;
        }

        public bool SetPointType(int idx, PathTypes type)
        {
            switch (Points[idx].Type)
            {
                case PathTypes.Move:
                case PathTypes.Line:
                    {
                        if (type == PathTypes.Move || type == PathTypes.Line)
                        {
                            Points[idx].Type = type;
                            return true;
                        }
                        return false;
                    }
                default:
                    return false;
            }
        }

        public void MoveTo(TPoint point)
        {
            Points.Add(new PointPath { Point = point, Type = PathTypes.Move });
        }

        public void LineTo(TPoint point)
        {
            Debug.Assert(Points.Count > 0);
            Points.Add(new PointPath { Point = point, Type = PathTypes.Line });
        }

        public void ConicTo(TPoint c1, TPoint c2)
        {
            Debug.Assert(Points.Count > 0);
            LineOnly = false;
            Points.Add(new PointPath { Point = c1, Type = PathTypes.Conic1 });
            Points.Add(new PointPath { Point = c2, Type = PathTypes.Conic2 });
        }

        public void BezierTo(TPoint b1, TPoint b2, TPoint b3)
        {
            Debug.Assert(Points.Count > 0);
            LineOnly = false;
            Points.Add(new PointPath { Point = b1, Type = PathTypes.Bezier1 });
            Points.Add(new PointPath { Point = b2, Type = PathTypes.Bezier2 });
            Points.Add(new PointPath { Point = b3, Type = PathTypes.Bezier3 });
        }

        public void Clear()
        {
            Points.Clear();
            LineOnly = true;
        }

        public void AddPath(Path<TPoint> path)
        {
            Points.AddRange(path.Points);
        }

        public void ReversePath()
        {
            var current = PathTypes.Move;
            var size = Points.Count;
            for (var i = size / 2; i-- > 0; )
            {
                Swap.Do(Points, i, size - 1 - i);
            }
            for (var idx = 0; idx < size; idx++)
            {
                Swap.Do(ref current, ref Points[idx].Type);
                switch (current)
                {
                    case PathTypes.Conic1:
                        current = PathTypes.Conic2;
                        break;
                    case PathTypes.Conic2:
                        current = PathTypes.Conic1;
                        break;
                    case PathTypes.Bezier1:
                        current = PathTypes.Bezier3;
                        break;
                    case PathTypes.Bezier3:
                        current = PathTypes.Bezier1;
                        break;
                }
            }
        }

        public TPoint GetLastMove()
        {
            for (var i = Points.Count - 1; i >= 0; i--)
            {
                if (Points[i].Type == PathTypes.Move)
                {
                    return Points[i].Point;
                }
            }
            Debug.Assert(false);
            return default(TPoint);
        }

        public void Close()
        {
            if (Points.Count == 0) return;
            var lastMove = GetLastMove();
            /*if(_points[_points.Count-1].Type!=PathTypes.Move)
            {
                if (RPoint(lastMove-mPoints.back().mP).getManhattanLength() < 1e-6) return;
            }*/
            LineTo(lastMove);
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            for (var i = 0; i < Points.Count; i++)
            {
                switch (Points[i].Type)
                {
                    case PathTypes.Move:
                        str.Append("p.moveTo(");
                        break;
                    case PathTypes.Line:
                        str.Append("p.lineTo(");
                        break;
                    case PathTypes.Bezier1:
                        str.Append(string.Format("p.bezierTo({0},{1}", Points[i++].Point, Points[i++].Point));
                        break;
                    case PathTypes.Conic1:
                        str.Append(string.Format("p.conicTo({0}", Points[i].Point));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                str.Append(string.Format("{0})\n", Points[i].Point));
            }
            return str.ToString();
        }

        public int GetSubpathCount()
        {
            return Points.Count(point => point.Type == PathTypes.Move);
        }

        public Path<TPoint> GetSubpath(int idx)
        {
            var subPathIdx = 0;
            var from = 0;
            for (; from < Points.Count; from++)
            {
                if (Points[from].Type != PathTypes.Move) continue;
                if (subPathIdx == idx) break;
                subPathIdx++;
            }
            if (from == Points.Count) return default(Path<TPoint>);
            var to = from + 1;
            for (; to < Points.Count; to++)
            {
                if (Points[to].Type != PathTypes.Move) continue;
                if (subPathIdx == idx) break;
                subPathIdx++;
            }
            if (from == 0 && to == Points.Count + 1)
                return this;
            var path = Points.GetRange(from, to - from);
            return new Path<TPoint>
                {
                    Points = path,
                    LineOnly = LineOnly
                };
        }
    }
}
