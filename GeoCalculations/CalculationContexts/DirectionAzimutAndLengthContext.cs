using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;

namespace GeoCalculations.CalculationContexts
{
    public class DirectionAzimutAndLengthContext : CalculationContextBase
    {
        public DirectionAzimutAndLengthContext()
            : base(null, new StreamingContext())
        { }

        public DirectionAzimutAndLengthContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _pointOfViewProperty = RegisterProperty("PointOfView", typeof(PointBase), new PointBase());
        public PointBase PointOfView
        {
            get { return GetValue<PointBase>(_pointOfViewProperty); }
            set { SetValue(_pointOfViewProperty, value); }
        }

        readonly PropertyData _orientationProperty = RegisterProperty("Orientation", typeof(PointBase), new PointBase());
        public PointBase Orientation
        {
            get { return GetValue<PointBase>(_orientationProperty); }
            set { SetValue(_orientationProperty, value); }
        }


    }
}
