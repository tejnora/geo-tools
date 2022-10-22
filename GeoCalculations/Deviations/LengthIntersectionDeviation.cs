using GeoCalculations.MethodPoints;

namespace GeoCalculations.Deviations
{
    public class LengthIntersectionDeviation
        : DeviationBase
    {
        public enum Types
        {
            MinimalniUhleProtnuti,
            KratsiUrcovanaVzdalenost
        } ;

        Types TypeP { get; set; }

        public LengthIntersectionDeviation(LengthIntersectionCalculatedPoint owner, Types type)
            : base(owner)
        {
            TypeP = type;
            if (type == Types.MinimalniUhleProtnuti)
            {
                LimitDeviation = 170.00000;
                CalculationDeviation = owner.MinimumIntersectionAngle;
            }
            else
            {
                LimitDeviation = 1500.0000;
                CalculationDeviation = owner.ShorterLength;
            }
        }
        public override string ToString()
        {
            if (TypeP == Types.MinimalniUhleProtnuti)
                return GetStringFromId(6);
            return GetStringFromId(7);
        }
    }
}
