using System;
using System.Runtime.Serialization;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.Points
{
    [Serializable]
    public class SimplePoint : PointBase
    {
        public SimplePoint()
        {
        }
        public SimplePoint(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
