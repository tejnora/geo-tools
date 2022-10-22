using GeoCalculations.Points;

namespace GeoCalculations.Deviations
{
    public class PolygonLengthDeviation
    : DeviationBase
    {
        public PolygonLengthDeviation(PolygonCalculatedPoints porad)
            : base(porad)
        {
            LimitDeviation = 2000;
            CalculationDeviation = porad.LenghtOfTraverse;
        }
        public override string ToString()
        {
            return GetStringFromId(3);
        }
    }
}
