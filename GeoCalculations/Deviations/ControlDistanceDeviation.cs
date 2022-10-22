using GeoCalculations.MethodPoints;
using GeoCalculations.Points;

namespace GeoCalculations.Deviations
{
    class ControlDistanceDeviation
        : DeviationBase
    {
        public ControlDistanceDeviation(ControlDistancePoint controlDistancePoint, double difference, double limitDeviation)
            : base(controlDistancePoint)
        {
            LimitDeviation = limitDeviation;
            CalculationDeviation = difference;

        }
        public override string ToString()
        {
            return GetStringFromId(12);
        }

    }
}
