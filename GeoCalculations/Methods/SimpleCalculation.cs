using System;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class SimpleCalculation
    {
        public static double CalculateDistance(Point bod1, Point bod2)
        {
            var result = Math.Pow(bod2.X - bod1.X, 2) + Math.Pow(bod2.Y - bod1.Y, 2) + Math.Pow(bod2.Height - bod1.Height, 2);
            result = Math.Sqrt(result);
            return result;
        }

        public static void CalculateOrientationMovement(PolarContex polarMethod)
        {
            BalancedDirectionAzimut.Calculate(polarMethod.PointOfView, polarMethod.Orientation, polarMethod.Deviations);
        }

        public static void CalculateOrientationMovement(PointBaseEx stanovisko, OrientationContext orientace)
        {
            BalancedDirectionAzimut.Calculate(stanovisko, orientace, orientace.Deviations);
        }


        public static double CalculateDirectionAzimut(Point bod1, Point bod2)
        {
            var result = Math.Atan2(bod2.Y - bod1.Y, bod2.X - bod1.X) * 200 / Math.PI;
            if (result < 0)
                result += 400;
            return result;
        }

        public static double CalculateDifferenceBetweenDirectionAzimutAndMeasurePoint(double smernik, double merenySmer)
        {
            var result = smernik - merenySmer;
            if (result < 0)
                result += 400;
            return result;
        }

        public static double ApplayDistanceScale(double distance)
        {
            return distance / 1000.0;
        }

        public static double ApplyVerticalDistanceScale(double height)
        {
            return 1 / (height * height);
        }

        public static double CalculateAngle(double p1, double p2)
        {
            var ret = p2 - p1;
            if (ret < 0) ret += 400;
            return ret;
        }

        public static double InverseAngle(double angle)
        {
            angle += 200;
            return ValidateDirectionAzimut(angle);
        }

        public static double ValidateHeight(double value)
        {
            while (value > 400)
                value -= 400;
            if (value > 200 && value <= 300)
                value = 100 + 300 - value;
            else if (value > 300)
                value = 400 - value;
            return value;
        }

        public static double ValidateDirectionAzimut(double smernik)
        {
            if (smernik >= 400)
                smernik -= 400;
            else if (smernik < 0)
                smernik += 400;
            return smernik;
        }

        public static double CalculateHeightDistance(Point bod1, Point bod2)
        {
            var z1 = bod1.Height;
            var z2 = bod2.Height;
            if (double.IsNaN(z1) || double.IsNaN(z2))
                throw new ArgumentNullException();
            if (z1 >= 0)
            {
                if (z2 >= 0)
                    return z2 - z1;
                return -(z1 + Math.Abs(z2));
            }
            if (z2 >= 0)
                return z2 + Math.Abs(z1);
            return z2 - z1;
        }

        public static double CalculateHeightByDistanceAndAngle(double angle, double horizontaLength)
        {
            if (double.IsNaN(angle) || double.IsNaN(horizontaLength))
                throw new HeightCalculationException("E14");
            angle = ValidateHeight(angle);
            var calcAngle = 100 - angle;
            if (calcAngle == 0 || calcAngle == 200)
                throw new HeightCalculationException("E15");
            var vyska = Math.Tan(calcAngle * Math.PI / 200.0) * horizontaLength;
            return vyska;
        }

        static public double CalculateHorizontalLength(Point bod1, Point bod2)
        {
            var result = Math.Pow(bod2.X - bod1.X, 2) + Math.Pow(bod2.Y - bod1.Y, 2);
            result = Math.Sqrt(result);
            return result;
        }


        static public double CalculateSlope(Point bod1, Point bod2)
        {//return grady
            var delka = CalculateHorizontalLength(bod1, bod2);
            var prevyseni = CalculateHeightDistance(bod1, bod2);
            return ToGrady(Math.Atan2(prevyseni, delka) * 180 / Math.PI);
        }

        static public double CalculateGradient(Point bod1, Point bod2)
        {
            var delka = CalculateHorizontalLength(bod1, bod2);
            var prevyseni = CalculateHeightDistance(bod1, bod2);
            return prevyseni / delka * 100;
        }

        static private double ToGrady(double value)
        {
            return value * 10 / 9;
        }


        static public double CalculateRotation(double a1, double a2)
        {
            return Math.Atan(a2 / a1) * 200 / Math.PI;
        }

        static public double CalculateScale(double a1, double a2)
        {
            return Math.Sqrt(Math.Pow(a1, 2) + Math.Pow(a2, 2));
        }


        static public Point CalculatePointOnLineAtSpecificDistance(Point point1, Point point2, double distanceFromPoint1)
        {
            /*
            If your points are (x1, y1) and (x2, y2), and you want to find the point (x3, y3) that is n units away from point 2:
            d = sqrt((x2-x1)^2 + (y2 - y1)^2) #distance
            r = n / d #segment ratio
            x3 = r * x2 + (1 - r) * x1 #find point that divides the segment
            y3 = r * y2 + (1 - r) * y1 #into the ratio (1-r):r*/
            var distance = CalculateDistance(point1, point2);
            var ratio = distanceFromPoint1 / distance;
            var x = ratio * point2.X + (1 - ratio) * point1.X;
            var y = ratio * point2.Y + (1 - ratio) * point1.Y;
            return new Point(x, y);
        }

        static public double CalculateM0Red(double dX, double dY)
        {
            return Math.Sqrt(0.5 * (Math.Pow(dX, 2) + Math.Pow(dY, 2)));
        }
    }
}
