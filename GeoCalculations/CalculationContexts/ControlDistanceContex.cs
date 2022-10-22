using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.CalculationContexts
{
    public class ControlDistanceContex : CalculationContextWithSimpleTable<ControlDistancePoint>
    {
        public enum CodeQualities
        {
            _3,
            _4,
            _5,
            _6,
            _7,
            _8
        }

        public ControlDistanceContex()
        {
        }

        public ControlDistanceContex(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public readonly PropertyData _qualityCodeProperty = RegisterProperty("QualityCode", typeof(CodeQualities), CodeQualities._3);
        public CodeQualities QualityCode
        {
            get { return GetValue<CodeQualities>(_qualityCodeProperty); }
            set { SetValue(_qualityCodeProperty, value); }
        }

    }
}
