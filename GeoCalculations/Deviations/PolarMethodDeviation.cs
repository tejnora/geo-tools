using System;
using GeoCalculations.CalculationContexts;

namespace GeoCalculations.Deviations
{
    public class PolarMethodDeviation
    : DeviationBase
    {
        public enum Types
        {
            MaxVOrientace
        } ;
        Types TypeP { get; set; }

        public PolarMethodDeviation(OrientationContext owner, Types type, double maxVOrientace)
            : base(owner)
        {
            TypeP = type;
            switch (type)
            {
                case Types.MaxVOrientace:
                    LimitDeviation = 0.08000;
                    CalculationDeviation = maxVOrientace;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public override string ToString()
        {
            if (TypeP == Types.MaxVOrientace)
                return GetStringFromId(10);
            throw new ArgumentOutOfRangeException();
        }
    }

}
