using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    [Serializable]
    public class LinesIntersectionContext : CalculationContextBase
    {
        public LinesIntersectionContext()
            : base(null, new StreamingContext())
        { }

        public LinesIntersectionContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _firstLineProperty = RegisterProperty("FirstLine", typeof(Line), new Line());
        [ProtocolPropertyData("FirstLine")]
        public Line FirstLine
        {
            get { return GetValue<Line>(_firstLineProperty); }
            set { SetValue(_firstLineProperty, value); }
        }

        readonly PropertyData _secondLineProperty = RegisterProperty("SecondLine", typeof(Line), new Line());
        [ProtocolPropertyData("SecondLine")]
        public Line SecondLine
        {
            get { return GetValue<Line>(_secondLineProperty); }
            set { SetValue(_secondLineProperty, value); }
        }
    }
}
