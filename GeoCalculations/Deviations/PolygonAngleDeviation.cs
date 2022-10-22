using System;
using GeoCalculations.Methods;
using GeoCalculations.Points;

namespace GeoCalculations.Deviations
{
    public class PolygonAngleDeviation
        : DeviationBase
    {
        public PolygonAngleDeviation(PolygonCalculatedPoints porad)
            : base(porad)
        {
            var pocetBodu = porad.Nodes.Count;
            if (porad.PolygonTraverseType == PolygonTraverseTypes.Closure)
                pocetBodu--;
            LimitDeviation = Math.Abs(0.02 * Math.Sqrt(pocetBodu + 2));
            CalculationDeviation = porad.AngleDeviation;
        }
        public override string ToString()
        {
            return GetStringFromId(2);
        }
    }
}
