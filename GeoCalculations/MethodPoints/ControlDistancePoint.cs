using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class ControlDistancePoint : CalculatedPointBase
    {
        public ControlDistancePoint()
        {
        }
        public ControlDistancePoint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _measuringLengthProperty = RegisterProperty("MeasuringLength", typeof(double), double.NaN);
        [ProtocolPropertyData("MeasureLength"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double MeasuringLength
        {
            get { return GetValue<double>(_measuringLengthProperty); }
            set { SetValue(_measuringLengthProperty, value); } 
        }

        readonly PropertyData _coordinateLengthProperty = RegisterProperty("CoordinateLength", typeof(double), double.NaN);
        [ProtocolPropertyData("CoordinateLength"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double CoordinateLength
        {
            get { return GetValue<double>(_coordinateLengthProperty); }
            set { SetValue(_coordinateLengthProperty, value); }
        }

        readonly PropertyData _lengthDifferenceProperty = RegisterProperty("LengthDifference", typeof(double), double.NaN);
        [ProtocolPropertyData("LengthDifference"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double LengthDifference
        {
            get { return GetValue<double>(_lengthDifferenceProperty); }
            set { SetValue(_lengthDifferenceProperty, value); }
        }

        readonly PropertyData _limitDeviationProperty = RegisterProperty("LimitDeviation", typeof(double), double.NaN);
        [ProtocolPropertyData("LimitDeviation"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double LimitDeviation
        {
            get { return GetValue<double>(_limitDeviationProperty); }
            set { SetValue(_limitDeviationProperty, value); }
        }
    }
}
