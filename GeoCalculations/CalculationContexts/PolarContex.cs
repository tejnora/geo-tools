using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    [Serializable]
    public class PolarContex : CalculationContextBase
    {
        public PolarContex()
            : base(null, new StreamingContext())
        {
        }

        public PolarContex(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _orientationProperty = RegisterProperty("Orientation", typeof(OrientationContext), new OrientationContext());
        [ProtocolPropertyData("Orientation")]
        public OrientationContext Orientation
        {
            get { return GetValue<OrientationContext>(_orientationProperty); }
            set { SetValue(_orientationProperty, value); }
        }

        readonly PropertyData _pointOfViewProperty = RegisterProperty("PointOfView", typeof(PointBaseEx), new PointBaseEx());
        [ProtocolPropertyData("PointOfView")]
        public PointBaseEx PointOfView
        {
            get { return GetValue<PointBaseEx>(_pointOfViewProperty); }
            set { SetValue(_pointOfViewProperty, value); }
        }
    }
}
