using GeoCalculations.Points;

namespace GeoCalculations.Deviations
{
    public class PolygonHeightClosureDeviation
        : DeviationBase
    {
        public PolygonHeightClosureDeviation(PolygonCalculatedPoints porad)
            : base(porad)
        {
            LimitDeviation = 0;//todo
            CalculationDeviation = porad.HeightClosure;
        }
        public override string ToString()
        {
            return GetStringFromId(5);
        }
    }

}
