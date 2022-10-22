using System;
using System.Runtime.Serialization;
using GeoCalculations.Deviations;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class CalculatedPointBase : PointBaseEx
    {
        public CalculatedPointBase()
        {
            Deviations = new CalculationDeviation();
        }
        public CalculatedPointBase(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public override void Init()
        {
            base.Init();
            Deviations = new CalculationDeviation();
        }

        public CalculationDeviation Deviations { get; set; }
    }
}
