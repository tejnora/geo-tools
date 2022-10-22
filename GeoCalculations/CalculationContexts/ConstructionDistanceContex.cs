using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.CalculationContexts
{
    public class ConstructionDistanceContex : CalculationContextWithSimpleTable<ConstructionDistancePoint>
    {
        public ConstructionDistanceContex()
        {

        }

        public ConstructionDistanceContex(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public readonly PropertyData _moveProperty = RegisterProperty("Move", typeof(double), double.NaN);
        public double Move
        {
            get { return GetValue<double>(_moveProperty); }
            set { SetValue(_moveProperty, value); }
        }
    }
}
