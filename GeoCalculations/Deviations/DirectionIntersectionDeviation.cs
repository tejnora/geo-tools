using System;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Deviations
{
    class DirectionIntersectionDeviation
                : DeviationBase
    {
        public enum Types
        {
            MinimumAngleOfIntersection,
            MinimumLength
        } ;

        Types TypeP { get; set; }

        public DirectionIntersectionDeviation(DirectionIntersectionCalculatedPoint owner, Types type)
            : base(owner)
        {
            TypeP = type;
            if (type == Types.MinimumAngleOfIntersection)
            {
                LimitDeviation = 30.00000;
                CalculationDeviation = owner.MinimumIntersectionAngle;
            }
            else
            {
                LimitDeviation = 1500.0000;
                CalculationDeviation = owner.ShorterLength;
            }
        }

        public override bool Exceeded
        {
            get
            {
                switch (TypeP)
                {
                    case Types.MinimumAngleOfIntersection:
                        return Math.Abs(CalculationDeviation) < Math.Abs(LimitDeviation);
                    case Types.MinimumLength:
                        return Math.Abs(CalculationDeviation) > Math.Abs(LimitDeviation);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        public override string ToString()
        {
            if (TypeP == Types.MinimumAngleOfIntersection)
                return GetStringFromId(13);
            return GetStringFromId(14);
        }
    }
}
