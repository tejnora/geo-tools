using System;
using GeoCalculations.BasicTypes;

namespace GeoCalculations.Curves
{
    public static class PathTo
    {
        static public Path<Point> ToBezierSegments(Path<Point> pathPoints)
        {
            var p = new Path<Point>();
            var subPathCount = pathPoints.GetSubpathCount();
            if (subPathCount == 0) return default(Path<Point>);
            if (subPathCount == 1) return ToBezierLinesSegments(pathPoints, -1);
            for (var i = 0; i < subPathCount; i++)
            {
                p.AddPath(ToBezierLinesSegments(pathPoints.GetSubpath(i), -1));
            }
            return p;
        }
        static Path<Point> ToBezierLinesSegments(Path<Point> points, int idx/*=-1 selected segment*/)
        {
            var p = new Path<Point>();
            if (points.Count == 0) return p;
            int i0, i1, i2, i3;
            i0 = i1 = i2 = i3 = 0;
            if (points.Count > 1)
            {
                i2++;
                i3++;
            }
            if (points.Count > 2)
            {
                i3++;
            }

            var bezierSegment = 0;
            if (idx == -1) p.MoveTo(points[0]);
            var idxLimit = (uint)points.Count - 1;
            while (i2 < points.Count)
            {
                Point bz0, bz1, bz2, bz3;
                EvaluateBezier(points, i0, i1, i2, i3, out bz0, out bz1, out bz2, out bz3);
                if (idx == -1 || bezierSegment == idx)
                {
                    if (bezierSegment == idx)
                    {
                        p.MoveTo(points[bezierSegment]);
                    }
                    p.BezierTo(bz1, bz2, bz3);
                }
                if (i1 != i0) i0++;
                i1++;
                i2++;
                if (i3 != idxLimit) i3++;
                bezierSegment++;
            }
            return p;
        }

        static void EvaluateBezier(Path<Point> points, int aI0, int aI1, int aI2, int aI3, out Point aBz0, out Point aBz1, out Point aBz2, out Point aBz3)
        {
            var p0 = points[aI0];
            var p1 = points[aI1];
            var p2 = points[aI2];
            var p3 = points[aI3];

            // var d01 = (p0 - p1).GetLength();
            var d12 = (p1 - p2).GetLength();
            // var d23 = (p2 - p3).GetLength();
            var d02 = (p0 - p2).GetLength();
            var d13 = (p1 - p3).GetLength();

            aBz0 = p1;
            if ((d02 / 6.0 < d12 / 2.0) && (d13 / 6.0 < d12 / 2.0))
            {
                Double f;
                if (aI0 != aI1)
                {
                    f = 1 / 6.0;
                }
                else
                {
                    f = 1 / 3.0;
                }
                aBz1 = p1 + (p2 - p0) * f;
                if (aI2 != aI3)
                {
                    f = 1 / 6.0;
                }
                else
                {
                    f = 1 / 3.0;
                }
                aBz2 = p2 + (p1 - p3) * f;
            }
            else
                if ((d02 / 6.0 >= d12 / 2.0) && (d13 / 6.0 >= d12 / 2.0))
                {
                    aBz1 = p1;
                    if (!DoubleEqual(d02, 0.0))
                    {
                        aBz1 += (p2 - p0) * d12 / 2.0 / d02;
                    }
                    aBz2 = p2;
                    if (!DoubleEqual(d13, 0.0))
                    {
                        aBz2 += (p1 - p3) * d12 / 2.0 / d13;
                    }
                }
                else
                    if (d02 / 6.0 >= d12 / 2.0)
                    {
                        aBz1 = p1;
                        if (!DoubleEqual(d02, 0.0))
                        {
                            aBz1 += (p2 - p0) * d12 / 2.0 / d02;
                        }
                        aBz2 = p2;
                        if (!DoubleEqual(d13, 0.0) && !DoubleEqual(d02, 0.0))
                        {
                            aBz2 += (p1 - p3) * d12 / 2.0 / d13 * (d13 / d02);
                        }
                    }
                    else
                    {
                        aBz1 = p1;
                        if (!DoubleEqual(d13, 0.0) && !DoubleEqual(d02, 0.0))
                        {
                            aBz1 += (p2 - p0) * d12 / 2.0 / d02 * (d02 / d13);
                        }
                        aBz2 = p2;
                        if (!DoubleEqual(d13, 0.0))
                        {
                            aBz2 += (p1 - p3) * d12 / 2.0 / d13;
                        }
                    }
            aBz3 = p2;
        }

        static bool DoubleEqual(double x, double y)
        {
            return Math.Abs(x - y) < 1e-15;
        }

    }
}
