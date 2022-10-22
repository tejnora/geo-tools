using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Points;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class Line : DataObjectBase<Line>
    {
        public Line()
            : base(null, new StreamingContext())
        { }

        public Line(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public readonly PropertyData _startPointProperty = RegisterProperty("StartPoint", typeof(SimplePoint), new SimplePoint());
        [ProtocolPropertyData("StartPoint")]
        public virtual SimplePoint StartPoint
        {
            get { return GetValue<SimplePoint>(_startPointProperty); }
            set { SetValue(_startPointProperty, value); }
        }
        public readonly PropertyData _endPointBProperty = RegisterProperty("EndPoint", typeof(SimplePoint), new SimplePoint());
        [ProtocolPropertyData("EndPoint")]
        public virtual SimplePoint EndPoint
        {
            get { return GetValue<SimplePoint>(_endPointBProperty); }
            set { SetValue(_endPointBProperty, value); }
        }
    }
}
