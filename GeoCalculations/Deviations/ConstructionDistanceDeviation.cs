using System;
using GeoCalculations.MethodPoints;
using GeoCalculations.Points;

namespace GeoCalculations.Deviations
{
    public class ConstructionDistanceDeviation
    : DeviationBase
    {
        public ConstructionDistanceDeviation(ConstructionDistancePoint lastPoint, double deviation, double length)
            : base(lastPoint)
        {
            LimitDeviation = (length + 12) / (length + 20) * (2 * Math.Sqrt(2) * 0.14);
            CalculationDeviation = deviation;
        }
        public override string ToString()
        {
            return GetStringFromId(11);
        }
    }
}
