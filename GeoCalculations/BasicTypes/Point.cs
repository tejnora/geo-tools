using System;

namespace GeoCalculations.BasicTypes
{
    public struct Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Height = 0;
        }

        public Point(double x, double y, double height)
        {
            X = x;
            Y = y;
            Height = height;
        }

        public double X;
        public double Y;
        public double Height;

        public double GetLength()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double GetSquareLength()
        {
            return X * X + Y * Y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator +(Point p1, double p2)
        {
            return new Point(p1.X + p2, p1.Y + p2);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator -(Point p1, double p2)
        {
            return new Point(p1.X - p2, p1.Y - p2);
        }

        public static Point operator *(Point p1, Point p2)
        {
            return new Point(p1.X * p2.X, p1.Y * p2.Y);
        }

        public static Point operator *(Point p1, double p2)
        {
            return new Point(p1.X * p2, p1.Y * p2);
        }

        public static Point operator *(int value, Point p1)
        {
            return new Point(p1.X * value, p1.Y * value);
        }

        public static Point operator /(Point p1, Point p2)
        {
            return new Point(p1.X / p2.X, p1.Y / p2.Y);
        }

        public static Point operator /(Point p1, double p2)
        {
            return new Point(p1.X / p2, p1.Y / p2);
        }

        public static Point operator -(Point p1)
        {
            return new Point(-p1.X, -p1.Y);
        }

        public override string ToString()
        {
            return string.Format("X:{0},Y:{1},Height:{2}", X, Y, Height);
        }
    }
}
