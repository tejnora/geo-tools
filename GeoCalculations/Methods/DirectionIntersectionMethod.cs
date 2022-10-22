using System;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class DirectionIntersectionMethod
    {
        public static void Calculate(DirectionIntersectionContext calculationContext, DirectionIntersectionCalculatedPoint calculatedPoint)
        {
            var pointOfViewA = calculationContext.PointOfViewA;
            var pointOfViewB = calculationContext.PointOfViewB;
            foreach (var orientaceNode in pointOfViewA.Orientation.TableNodes)
            {
                orientaceNode.Distance = SimpleCalculation.CalculateDistance(new Point(pointOfViewA.PointOfView.X, pointOfViewA.PointOfView.Y),
                                                          new Point(orientaceNode.X, orientaceNode.Y));
            }
            foreach (var orientaceNode in pointOfViewB.Orientation.TableNodes)
            {
                orientaceNode.Distance = SimpleCalculation.CalculateDistance(new Point(pointOfViewB.PointOfView.X, pointOfViewB.PointOfView.Y),
                                                          new Point(orientaceNode.X, orientaceNode.Y));
            }
            SimpleCalculation.CalculateOrientationMovement(pointOfViewA.PointOfView, pointOfViewA.Orientation);
            SimpleCalculation.CalculateOrientationMovement(pointOfViewB.PointOfView, pointOfViewB.Orientation);
            var sigmaB = SimpleCalculation.ValidateDirectionAzimut(pointOfViewB.Orientation.OrientationMovement + calculatedPoint.DirectionFromB);
            var sigmaA = SimpleCalculation.ValidateDirectionAzimut(pointOfViewA.Orientation.OrientationMovement + calculatedPoint.DirectionFromA);
            double dA, dB;
            try
            {
                if (sigmaB != 0)
                {
                    //dA = {XB - XA + [YA - YB] / tan(sigmaB)} / {cos(sigmaA) - sin(sigmaA) / tan(sigmaB)}
                    dA = (pointOfViewB.PointOfView.X - pointOfViewA.PointOfView.X +
                                 (pointOfViewA.PointOfView.Y - pointOfViewB.PointOfView.Y) / Math.Tan(sigmaB * Math.PI / 200.0)) /
                                (Math.Cos(sigmaA * Math.PI / 200.0) - Math.Sin(sigmaA * Math.PI / 200.0) / Math.Tan(sigmaB * Math.PI / 200.0));
                    //dB = [YA + dA * sin(sigmaA) - YB] / sin(sigmaB)
                    dB = (pointOfViewA.PointOfView.Y + dA * Math.Sin(sigmaA * Math.PI / 200.0) - pointOfViewB.PointOfView.Y) / Math.Sin(sigmaB * Math.PI / 200.0);
                }
                else
                {
                    //dA = (YB - YA) / sin(sigmaA)
                    dA = (pointOfViewB.PointOfView.Y - pointOfViewA.PointOfView.Y) / Math.Sin(sigmaA * Math.PI / 200.0);
                    //dB = XA  - XB + (YB - YA) / tan(sigmaA)
                    dB = pointOfViewA.PointOfView.X - pointOfViewB.PointOfView.X +
                         (pointOfViewB.PointOfView.Y - pointOfViewA.PointOfView.Y) / Math.Tan(sigmaA * Math.PI / 200.0);
                }
                var d = Math.Sqrt(Math.Pow(pointOfViewA.PointOfView.Y - pointOfViewB.PointOfView.Y, 2) +
                          Math.Pow(pointOfViewA.PointOfView.X - pointOfViewB.PointOfView.X, 2));
                //gama = arccos((dA^2 +dB^2 - d^2) / (2 * dA * dB))
                calculatedPoint.AngleOfIntersection = Math.Acos((dA * dA + dB * dB - d * d) / (2 * dA * dB));
                calculatedPoint.AngleOfIntersection = calculatedPoint.AngleOfIntersection * 200 / Math.PI;

                //Y1 = YA + dA * sin(sigmaA)
                calculatedPoint.Y = pointOfViewA.PointOfView.Y + dA * Math.Sin(sigmaA * Math.PI / 200.0);
                //X1 = XA + dA * cos(sigmaA)
                calculatedPoint.X = pointOfViewA.PointOfView.X + dA * Math.Cos(sigmaA * Math.PI / 200.0);
                //d = sqrt((YA - YB)^2 + (XA - XB)^2)
                calculatedPoint.MinimumIntersectionAngle = Math.Max(sigmaB, sigmaA);
                calculatedPoint.ShorterLength = Math.Min(dA, dB);
                calculationContext.Deviations.Deviations.Add(new DirectionIntersectionDeviation(calculatedPoint, DirectionIntersectionDeviation.Types.MinimumLength));
                calculationContext.Deviations.Deviations.Add(new DirectionIntersectionDeviation(calculatedPoint, DirectionIntersectionDeviation.Types.MinimumAngleOfIntersection));
            }
            catch (Exception)
            {
                throw new LengthIntersectionCalculationException("E22");
            }
        }
    }
}
