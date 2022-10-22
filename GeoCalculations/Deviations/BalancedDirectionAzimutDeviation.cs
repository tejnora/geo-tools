using GeoCalculations.MethodPoints;
using GeoCalculations.Points;

namespace GeoCalculations.Deviations
{
    public class BalancedDirectionAzimutDeviation
    : DeviationBase
    {
        public BalancedDirectionAzimutDeviation(OrientationPoint orientace)
            : base(orientace)
        {
            LimitDeviation = 0.08;
            CalculationDeviation = orientace.VerticalOrientation;
            Owner = orientace;
        }
        public new OrientationPoint Owner
        { get; private set; }
        public override string ToString()
        {
            return GetStringFromId(4);
        }
    }
}
