using System;
using System.Windows;
using System.Xml;

namespace GeoBase.Utils
{
    [Serializable]
    public struct UnitPoint
    {
        public static UnitPoint Empty;
        public static bool operator !=(UnitPoint left, UnitPoint right)
        {
            return !(left == right);
        }
        public static bool operator ==(UnitPoint left, UnitPoint right)
        {
            if (left.X == right.X)
                return left.Y == right.Y;
            if (left.IsEmpty && right.IsEmpty) // after changing Empty to use NaN this extra check is required
                return true;
            return false;
        }
        public static UnitPoint operator +(UnitPoint left, UnitPoint right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }
        double _x;
        double _y;
        static UnitPoint()
        {
            Empty = new UnitPoint();
            Empty._x = double.NaN;
            Empty._y = double.NaN;
        }
        public UnitPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }
        public UnitPoint(Point p)
        {
            _x = p.X;
            _y = p.Y;
        }
        public bool IsEmpty
        {
            get
            {
                return double.IsNaN(X) && double.IsNaN(Y);
            }
        }
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public Point Point
        {
            get { return new Point(_x, _y); }
        }
        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}}}", XmlConvert.ToString(Math.Round(X, 8)), XmlConvert.ToString(Math.Round(Y, 8)));
        }
        public override bool Equals(object obj)
        {
            if (obj is UnitPoint)
                return (this == (UnitPoint)obj);
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public string PosAsString()
        {
            return string.Format("[{0:f4}, {1:f4}]", X, Y);
        }
    }
}
