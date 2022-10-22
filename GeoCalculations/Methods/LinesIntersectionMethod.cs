using GeoCalculations.CalculationContexts;
using GeoCalculations.Exceptions;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Methods
{
    public static class LinesIntersectionMethod
    {
        public static void Calculated(LinesIntersectionContext calculationContext, CalculatedPointBase calculatedPoint)
        {
            var firstLine = calculationContext.FirstLine;
            var secondLine = calculationContext.SecondLine;
            var firstParametricLine = new BasicTypes.Line(new BasicTypes.Point(firstLine.StartPoint.X, firstLine.StartPoint.Y), new BasicTypes.Point(firstLine.EndPoint.X, firstLine.EndPoint.Y));
            var secondParametricLine = new BasicTypes.Line(new BasicTypes.Point(secondLine.StartPoint.X, secondLine.StartPoint.Y), new BasicTypes.Point(secondLine.EndPoint.X, secondLine.EndPoint.Y));
            if (firstParametricLine.IsInvalid || secondParametricLine.IsInvalid)
                return;
            double x;
            double y;
            if (!firstParametricLine.Intersection(secondParametricLine, out x, out y))
            {
                throw new LinesIntersectionException("E38");
            }
            calculatedPoint.X = x;
            calculatedPoint.Y = y;
        }

    }
}
