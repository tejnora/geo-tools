using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    public class PolygonTraverseContext : CalculationContextBase
    {
        readonly PropertyData _beginPointProperty = RegisterProperty("BeginPoint", typeof(PointBaseEx), new PointBaseEx());
        [ProtocolPropertyData("BeginPoint")]
        public PointBaseEx BeginPoint
        {
            get { return GetValue<PointBaseEx>(_beginPointProperty); }
            set { SetValue(_beginPointProperty, value); }
        }

        readonly PropertyData _endOrientationContext = RegisterProperty("EndOrientationContext", typeof(OrientationContext), new OrientationContext());
        [ProtocolPropertyData("EndOrientationContext")]
        public OrientationContext EndOrientationContext
        {
            get { return GetValue<OrientationContext>(_endOrientationContext); }
            set { SetValue(_endOrientationContext, value); }
        }

        readonly PropertyData _endPointProperty = RegisterProperty("EndPoint", typeof(PointBaseEx), new PointBaseEx());
        [ProtocolPropertyData("EndPoint")]
        public PointBaseEx EndPoint
        {
            get { return GetValue<PointBaseEx>(_endPointProperty); }
            set { SetValue(_endPointProperty, value); }
        }

        readonly PropertyData _beginOrientationContext = RegisterProperty("BeginOrientationContext", typeof(OrientationContext), new OrientationContext());
        [ProtocolPropertyData("BeginOrientationContext")]
        public OrientationContext BeginOrientationContext
        {
            get { return GetValue<OrientationContext>(_beginOrientationContext); }
            set { SetValue(_beginOrientationContext, value); }
        }

        readonly PropertyData _measuredContext = RegisterProperty("MeasuredContext", typeof(MeasuredPointsContext), new MeasuredPointsContext());
        [ProtocolPropertyData("MeasuredContext")]
        public MeasuredPointsContext MeasuredContext
        {
            get { return GetValue<MeasuredPointsContext>(_measuredContext); }
            set { SetValue(_measuredContext, value); }
        }

    }
}
