using GeoCalculations.BasicTypes;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class DirectionAzimutAndLengthMethod
    {
        public static void Calculate(PointBase pointOfView, PointBase orientation, DirectionAzimutAndLengthCalculatedPoint calculatedPoint)
        {
            var p1 = new Point { X = pointOfView.X, Y = pointOfView.Y, Height = pointOfView.Z };
            var p2 = new Point { X = orientation.X, Y = orientation.Y, Height = orientation.Z };
            calculatedPoint.DirectionAzimut = SimpleCalculation.CalculateDirectionAzimut(p1, p2);
            calculatedPoint.Distance = SimpleCalculation.CalculateHorizontalLength(p1, p2);
            calculatedPoint.ObliqueLength = SimpleCalculation.CalculateDistance(p1, p2);
            calculatedPoint.ElevationDifference = SimpleCalculation.CalculateHeightDistance(p1, p2);
            calculatedPoint.Slope = SimpleCalculation.CalculateSlope(p1, p2);
            calculatedPoint.Gradient = SimpleCalculation.CalculateGradient(p1, p2);
        }
    }
}
