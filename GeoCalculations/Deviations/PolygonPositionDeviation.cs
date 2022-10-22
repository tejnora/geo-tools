using System;
using GeoCalculations.Points;

namespace GeoCalculations.Deviations
{
    public class PolygonPositionDeviation
        : DeviationBase
    {
        public PolygonPositionDeviation(PolygonCalculatedPoints porad)
            : base(porad)
        {
            LimitDeviation = Math.Abs(0.012 * Math.Sqrt(porad.LenghtOfTraverse) + 0.01);
            CalculationDeviation = porad.LocationDeviation;
        }
        public override string ToString()
        {
            return GetStringFromId(1);
        }
    }
}
