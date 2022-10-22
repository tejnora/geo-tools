using System;
using GeoCalculations.CalculationContexts;

namespace GeoCalculations.Deviations
{
    public class OrthogonalMethodDeviation
        : DeviationBase
    {
        public enum Types
        {
            LengthDiffirence,
            Length
        } ;

        public OrthogonalMethodDeviation(OrtogonalContext owner, Types type)
            : base(owner)
        {
            TypeP = type;
            switch (type)
            {
                case Types.LengthDiffirence:
                    LimitDeviation = (owner.LengthFromCoordinates + 12) / (owner.LengthFromCoordinates + 20) * 2 * Math.Sqrt(2) * 0.14;
                    CalculationDeviation = Math.Abs(owner.LengthFromCoordinates - owner.MeasuringLength);
                    break;
                case Types.Length:
                    LimitDeviation = 2000;
                    CalculationDeviation = owner.MeasuringLength;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
        Types TypeP { get; set; }
        public override string ToString()
        {
            if (TypeP == Types.LengthDiffirence)
                return GetStringFromId(8);
            return GetStringFromId(9);
        }
    }

}
