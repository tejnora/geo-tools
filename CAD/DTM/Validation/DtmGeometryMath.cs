using System;
using System.Collections.Generic;
using System.Linq;

namespace CAD.DTM.Validation
{
    public enum DtmSegmentRelationKind
    {
        None,
        Overlap,
        Point
    }

    public struct DtmSegmentRelation
    {
        public DtmSegmentRelationKind Kind;
        public double X;
        public double Y;
    }

    public static class DtmGeometryMath
    {
        public static double Distance2D(DtmPoint a, DtmPoint b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double Distance3D(DtmPoint a, DtmPoint b)
        {
            var dz = a.Z - b.Z;
            var d = Distance2D(a, b);
            return Math.Sqrt(d * d + dz * dz);
        }

        public static string Key2D(DtmPoint p) => $"{Math.Round(p.X * 1000):0}|{Math.Round(p.Y * 1000):0}";
        public static string Key3D(DtmPoint p) => $"{Key2D(p)}|{Math.Round(p.Z * 1000):0}";

        public static double DistanceToSegment2D(DtmPoint p, DtmPoint a, DtmPoint b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var len2 = dx * dx + dy * dy;
            var t = len2 == 0 ? 0 : Math.Max(0, Math.Min(1, ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / len2));
            var cx = a.X + t * dx;
            var cy = a.Y + t * dy;
            return Math.Sqrt((p.X - cx) * (p.X - cx) + (p.Y - cy) * (p.Y - cy));
        }

        public static double DistanceToSegment3D(DtmPoint p, DtmPoint a, DtmPoint b)
        {
            double dx = b.X - a.X, dy = b.Y - a.Y, dz = b.Z - a.Z;
            var len2 = dx * dx + dy * dy + dz * dz;
            var t = len2 == 0 ? 0 : Math.Max(0, Math.Min(1, ((p.X - a.X) * dx + (p.Y - a.Y) * dy + (p.Z - a.Z) * dz) / len2));
            double cx = a.X + t * dx - p.X, cy = a.Y + t * dy - p.Y, cz = a.Z + t * dz - p.Z;
            return Math.Sqrt(cx * cx + cy * cy + cz * cz);
        }

        public static bool IsInsideOrOnBoundary(DtmPoint p, IList<DtmPoint> ring, double tolerance)
        {
            var inside = false;
            for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
            {
                var a = ring[i];
                var b = ring[j];
                if (DistanceToSegment2D(p, a, b) <= tolerance)
                    return true;
                if ((a.Y > p.Y) != (b.Y > p.Y) && p.X < (b.X - a.X) * (p.Y - a.Y) / (b.Y - a.Y) + a.X)
                    inside = !inside;
            }
            return inside;
        }

        public static bool IsStrictlyInside(DtmPoint p, IList<DtmPoint> ring, double tolerance)
        {
            for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
                if (DistanceToSegment2D(p, ring[i], ring[j]) <= tolerance)
                    return false;
            return IsInsideOrOnBoundary(p, ring, 0);
        }

        public static double Area2D(IList<DtmPoint> ring)
        {
            double sum = 0;
            for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
                sum += ring[j].X * ring[i].Y - ring[i].X * ring[j].Y;
            return Math.Abs(sum) / 2;
        }

        // Vztah dvou useciek ve 2D; Point vraci bod pruniku/dotyku, Overlap kolinearni prekryv delsi nez tolerance.
        public static DtmSegmentRelation Relate(DtmPoint a1, DtmPoint a2, DtmPoint b1, DtmPoint b2, double eps)
        {
            double d1x = a2.X - a1.X, d1y = a2.Y - a1.Y, d2x = b2.X - b1.X, d2y = b2.Y - b1.Y;
            var len1 = Math.Sqrt(d1x * d1x + d1y * d1y);
            var len2 = Math.Sqrt(d2x * d2x + d2y * d2y);
            var none = new DtmSegmentRelation { Kind = DtmSegmentRelationKind.None };
            if (len1 < eps || len2 < eps)
                return none;
            var cross = d1x * d2y - d1y * d2x;
            if (Math.Abs(cross) <= 1e-9 * len1 * len2)
            {
                if (DistanceToSegment2D(b1, a1, a2) > eps && DistanceToSegment2D(b2, a1, a2) > eps
                    && DistanceToSegment2D(a1, b1, b2) > eps && DistanceToSegment2D(a2, b1, b2) > eps)
                    return none;
                // projekce na osu prvni useciky
                double ux = d1x / len1, uy = d1y / len1;
                var t1 = (b1.X - a1.X) * ux + (b1.Y - a1.Y) * uy;
                var t2 = (b2.X - a1.X) * ux + (b2.Y - a1.Y) * uy;
                var lo = Math.Max(0, Math.Min(t1, t2));
                var hi = Math.Min(len1, Math.Max(t1, t2));
                if (hi - lo > eps)
                    return new DtmSegmentRelation { Kind = DtmSegmentRelationKind.Overlap, X = a1.X + ux * lo, Y = a1.Y + uy * lo };
                var t = (lo + hi) / 2;
                return new DtmSegmentRelation { Kind = DtmSegmentRelationKind.Point, X = a1.X + ux * t, Y = a1.Y + uy * t };
            }
            var s = ((b1.X - a1.X) * d2y - (b1.Y - a1.Y) * d2x) / cross;
            var u = ((b1.X - a1.X) * d1y - (b1.Y - a1.Y) * d1x) / cross;
            double tolS = eps / len1, tolU = eps / len2;
            if (s < -tolS || s > 1 + tolS || u < -tolU || u > 1 + tolU)
                return none;
            return new DtmSegmentRelation { Kind = DtmSegmentRelationKind.Point, X = a1.X + s * d1x, Y = a1.Y + s * d1y };
        }

        public static bool BoxesOverlap(double[] a, double[] b, double eps) =>
            a[0] <= b[2] + eps && b[0] <= a[2] + eps && a[1] <= b[3] + eps && b[1] <= a[3] + eps;

        public static double[] Box(IEnumerable<DtmPoint> pts)
        {
            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
            foreach (var p in pts)
            {
                minX = Math.Min(minX, p.X); maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y); maxY = Math.Max(maxY, p.Y);
            }
            return new[] { minX, minY, maxX, maxY };
        }
    }
}
