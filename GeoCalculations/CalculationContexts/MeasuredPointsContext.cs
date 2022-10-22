using System;
using System.Runtime.Serialization;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.CalculationContexts
{
    [Serializable]
    public class MeasuredPointsContext : CalculationContextWithSimpleTable<MeasuredPoint>
    {
        public MeasuredPointsContext()
        {
            
        }

        public MeasuredPointsContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
