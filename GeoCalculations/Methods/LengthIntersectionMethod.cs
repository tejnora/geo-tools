using System;
using GeoCalculations.BasicTypes;
using GeoCalculations.CalculationContexts;
using GeoCalculations.Deviations;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class LengthIntersectionMethod
    {
        public static void CalculatePoint(LengthIntersectionContext calculationContext, LengthIntersectionCalculatedPoint calculatedPoint)
        {
            var leftPoint = new Point(calculationContext.LeftPointOfView.X, calculationContext.LeftPointOfView.Y);
            var rightPoint = new Point(calculationContext.RightPointOfView.X, calculationContext.RightPointOfView.Y);
            var azimut = SimpleCalculation.CalculateDirectionAzimut(leftPoint, rightPoint);
            if (double.IsNaN(calculationContext.LeftPointOfView.Distance) || calculationContext.LeftPointOfView.Distance == 0 || double.IsNaN(calculationContext.RightPointOfView.Distance) || calculationContext.RightPointOfView.Distance == 0)
                throw new LengthIntersectionCalculationException("E20");
            var dL = calculationContext.LeftPointOfView.Distance;
            var d = SimpleCalculation.CalculateDistance(leftPoint, rightPoint);
            var dR = calculationContext.RightPointOfView.Distance;
            calculatedPoint.DistanceFromLeft = dL;
            calculatedPoint.DistanceFromRight = dR;
            //alfa = arccos[(d^2 + dL^2 - dP^2) / (2 * d * dL)]
            var numerator = Math.Pow(d, 2) + Math.Pow(dL, 2) - Math.Pow(dR, 2);
            var denominator = 2 * d * dL;
            var fractionValue = numerator / denominator;
            if (fractionValue > 1 || fractionValue < -1)
                throw new LengthIntersectionCalculationException("E21");
            var alfa = Math.Acos(fractionValue) * 200 / Math.PI;

            //beta = arccos[(dL^2 + dP^2 - d^2) / (2 * dL * dP)]
            numerator = Math.Pow(dL, 2) + Math.Pow(dR, 2) - Math.Pow(d, 2);
            denominator = 2 * dL * dR;
            fractionValue = numerator / denominator;
            if (fractionValue > 1 || fractionValue < -1)
                throw new LengthIntersectionCalculationException("E21");
            var beta = Math.Acos(fractionValue) * 200 / Math.PI;
            if (beta < 30 || beta > 170)
                throw new LengthIntersectionCalculationException("E21");
            var sL = azimut - alfa;
            if(string.IsNullOrEmpty(calculatedPoint.Number))
                throw new LengthIntersectionCalculationException("E37");
            calculatedPoint.Y = calculationContext.LeftPointOfView.Y + dL * Math.Sin(sL * Math.PI / 200.0);
            calculatedPoint.X = calculationContext.LeftPointOfView.X + dL * Math.Cos(sL * Math.PI / 200.0);
            calculatedPoint.MinimumIntersectionAngle = Math.Max(beta, alfa);
            calculatedPoint.ShorterLength = Math.Min(dL, dR);
            calculationContext.Deviations.Deviations.Add(new LengthIntersectionDeviation(calculatedPoint, LengthIntersectionDeviation.Types.KratsiUrcovanaVzdalenost));
            calculationContext.Deviations.Deviations.Add(new LengthIntersectionDeviation(calculatedPoint, LengthIntersectionDeviation.Types.MinimalniUhleProtnuti));
        }

    }
}
