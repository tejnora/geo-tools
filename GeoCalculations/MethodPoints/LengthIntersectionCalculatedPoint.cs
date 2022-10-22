using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class LengthIntersectionCalculatedPoint : CalculatedPointBase
    {
        public LengthIntersectionCalculatedPoint() { }

        public LengthIntersectionCalculatedPoint(SerializationInfo info, StreamingContext context)
            : base(info, context) { }


        readonly PropertyData _distanceFromLeftProperty = RegisterProperty("DistanceFromLeft", typeof(double), double.NaN);
        [ProtocolPropertyData("Sa"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double DistanceFromLeft
        {
            get { return GetValue<double>(_distanceFromLeftProperty); }
            set { SetValue(_distanceFromLeftProperty, value); }
        }

        readonly PropertyData _distanceFromRightProperty = RegisterProperty("DistanceFromRight", typeof(double), double.NaN);
        [ProtocolPropertyData("Sb"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double DistanceFromRight
        {
            get { return GetValue<double>(_distanceFromRightProperty); }
            set { SetValue(_distanceFromRightProperty, value); }
        }

        readonly PropertyData _minimutIntersectionAgnleProperty = RegisterProperty("MinimumIntersectionAngle", typeof(double), double.NaN);
        public double MinimumIntersectionAngle
        {
            get { return GetValue<double>(_minimutIntersectionAgnleProperty); }
            set { SetValue(_minimutIntersectionAgnleProperty, value); }
        }

        readonly PropertyData _shorterLengthProperty = RegisterProperty("ShorterLength", typeof(double), double.NaN);
        public double ShorterLength
        {
            get { return GetValue<double>(_shorterLengthProperty); }
            set { SetValue(_shorterLengthProperty, value); }
        }
    }
}
