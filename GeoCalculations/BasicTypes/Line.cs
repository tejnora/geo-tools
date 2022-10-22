using System;

namespace GeoCalculations.BasicTypes
{
    public class Line
    {
        public Line(Point pointA, Point pointB)
        {
            IsInvalid = true;
            if (double.IsNaN(pointA.X) || double.IsNaN(pointA.Y) || double.IsNaN(pointB.X) || double.IsNaN(pointB.Y))
                return;
            if (pointA.X == pointB.X && pointA.Y == pointB.Y)
                return;
            IsInvalid = false;
            A = pointB.Y - pointA.Y;
            B = pointA.X - pointB.X;
            C = -A * pointA.X - B * pointA.Y;

        }
        public Line(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public bool IsInvalid { get; set; }

        public bool AreParallel(Line primka)
        {
            var ka = A / primka.A;
            var kb = B / primka.B;
            return ka == kb || (B == 0 && primka.B == 0) || (A == 0 && primka.A == 0);
        }
        public bool ArePerpendicular(Line primka)
        {
            var pom = new Line(-primka.B, primka.A, primka.C);
            return AreParallel(pom);
        }
        public bool Intersection(Line line, out double retx, out double rety)
        {
            retx = 0;
            rety = 0;
            if (this == line)
                return false;
            if (AreParallel(line))
                return false;
            retx = (B * line.C - C * line.B) / (A * line.B - line.A * B);
            rety = -(A * line.C - line.A * C) / (A * line.B - line.A * B);
            return true;
        }

        public double Angle(Line line)
        {
            return Math.Acos((A * line.A + B * line.B) / (Math.Sqrt((A * A + B * B) * Math.Sqrt(line.A * line.A + line.B * line.B))));
        }

        public double DisctancePointFromLine(double x, double y)
        {
            var distance = (A * x + B * y + C) / Math.Sqrt(A * A + B * B);
            if (distance < 0.0)
            {
                distance = -distance;
            }
            return distance;
        }
        public static bool operator ==(Line lineA, Line lineB)
        {
            return lineA.A == lineB.A && lineA.B == lineB.B && lineA.C == lineB.C;
            /*double ka = primkaA.A / primkaB.A;
            double kb = primkaA.B / primkaB.B;
            double kc = primkaA.C / primkaB.C;
            if (ka == kb && (ka == kc ))
            {
                return true;// Splývající přímky
            }
            return false;// Dvě různé přímky*/
        }
        public static bool operator !=(Line lineA, Line lineB)
        {
            return !(lineB == lineA);
        }
        public override bool Equals(object obj)
        {
            return this == (Line)obj;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + A.GetHashCode();
                hash = hash * 23 + B.GetHashCode();
                hash = hash * 23 + C.GetHashCode();
                hash = hash * 23 + IsInvalid.GetHashCode();
                return hash;
            }
        }
    }
}
