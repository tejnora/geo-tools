using System;
namespace GeoCalculations.BasicTypes
{
    public class Path2D : Path<Point>
    {
        public Path2D ToLineOnly(double flatness = 6e-005)
        {
            if (LineOnly) return this;
            var result = new Path2D();
            flatness = flatness * flatness;
            for (var i = 0; i < Points.Count; i++)
            {
                var point = Points[i];
                switch (point.Type)
                {
                    case PathTypes.Move:
                        result.MoveTo(point.Point);
                        break;
                    case PathTypes.Line:
                        result.LineTo(point.Point);
                        break;
                    case PathTypes.Bezier1:
                        result.AddRecursiveBezier(Points[i - 1].Point, Points[i].Point, Points[i + 1].Point, Points[i + 2].Point, flatness);
                        i += 2;
                        break;
                    case PathTypes.Conic1:
                        result.AddRecursiveConic(Points[i - 1].Point, Points[i].Point, Points[i + 1].Point, flatness);
                        ++i;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return result;
        }

        void AddRecursiveConic(Point a0, Point a1, Point a2, double flatness)
        {
            var d = a2 - a0;
            var d0 = a1 - a0;
            var dot = d0.X * d.X + d0.Y * d.Y;
            if (dot < 0)
            {
                if ((a1 - a2).GetSquareLength() > flatness) goto subdivide;
            }
            else
            {
                var rr = d.GetSquareLength();
                if (dot > rr)
                {
                    if (d0.GetSquareLength() > flatness) goto subdivide;
                }
                else
                {
                    var perp = d0.Y * d.X - d0.X * d.Y;
                    if (perp * perp > rr * flatness) goto subdivide;
                }
            }
            // don't subdivide
            LineTo(a2);
            return;

        subdivide:
            var pa = (a0 + a1) * 0.5;
            var pb = (a1 + a2) * 0.5;
            var pm = (pa + pb) * 0.5;
            AddRecursiveConic(a0, pa, pm, flatness);
            AddRecursiveConic(pm, pb, a2, flatness);
        }

        void AddRecursiveBezier(Point a0, Point a1, Point a2, Point a3, double flatness)
        {
            // Flatness is already squared
            /* 
           It's possible to optimize this routine a fair amount.
   
           First, once the _dot conditions are met, they will also be met in
           all further subdivisions. So we might recurse to a different
           routine that only checks the _perp conditions.
     
           Second, the distance _should_ decrease according to fairly
           predictable rules (a factor of 4 with each subdivision). So it might
           be possible to note that the distance is within a factor of 4 of
           acceptable, and subdivide once. But proving this might be hard.
       
           Third, at the last subdivision, x_m and y_m can be computed more
           expeditiously.
         
           Finally, if we were able to subdivide by, say 2 or 3, this would
           allow considerably finer-grain control, i.e. fewer points for the
           same flatness tolerance. This would speed things up downstream.

           In any case, this routine is unlikely to be the bottleneck. It's
           just that I have this undying quest for more speed...
           */
            var _3_0 = a3 - a0;
            // z3_0_dot is dist z0-z3 squared
            var z3_0_dot = _3_0.GetSquareLength();

            if (z3_0_dot < flatness)
            {
                if ((a1 - a0).GetSquareLength() < flatness * 0.25)
                {
                    if ((a2 - a3).GetSquareLength() < flatness * 0.25) goto nosubdivide;
                }
                if (z3_0_dot < 1e-15)
                    goto subdivide; //bug #39682
            }

            // todo: this test is far from satisfactory.
            // Bobris change: next line commented ...
            // if (z3_0_dot < 0.001) goto nosubdivide;

            // we can avoid subdivision if:
            // z1 has distance no more than flatness from the z0-z3 line
            // z1 is no more z0'ward than flatness past z0-z3
            // z1 is more z0'ward than z3'ward on the line traversing z0-z3
            // and correspondingly for z2

            // perp is distance from line, multiplied by dist z0-z3
            var max_perp_sq = flatness * z3_0_dot;

            var temp = (a1.Y - a0.Y) * _3_0.X - (a1.X - a0.X) * _3_0.Y;
            if (temp * temp > max_perp_sq) goto subdivide;

            temp = (a3.Y - a2.Y) * _3_0.X - (a3.X - a2.X) * _3_0.Y;
            if (temp * temp > max_perp_sq) goto subdivide;

            temp = (a1.X - a0.X) * _3_0.X + (a1.Y - a0.Y) * _3_0.Y;
            if (temp < 0 && temp * temp > max_perp_sq)
                goto subdivide;
            if (temp + temp > z3_0_dot) goto subdivide;

            temp = (a3.X - a2.X) * _3_0.X + (a3.Y - a2.Y) * _3_0.Y;
            if (temp < 0 && temp * temp > max_perp_sq)
                goto subdivide;
            if (temp + temp > z3_0_dot) goto subdivide;

        nosubdivide: // don't subdivide
            LineTo(a3);
            return;

        subdivide:

            var pa1 = (a0 + a1) * 0.5;
            var pa2 = (a0 + 2 * a1 + a2) * 0.25;
            var pb1 = (a1 + 2 * a2 + a3) * 0.25;
            var pb2 = (a2 + a3) * 0.5;
            var pm = (pa2 + pb1) * 0.5;

            AddRecursiveBezier(a0, pa1, pa2, pm, flatness);
            AddRecursiveBezier(pm, pb1, pb2, a3, flatness);
        }
    }
}
