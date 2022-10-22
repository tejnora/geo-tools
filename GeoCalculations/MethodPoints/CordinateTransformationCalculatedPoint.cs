using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class CordinateTransformationCalculatedPoint : CalculatedPointBase
    {
        public CordinateTransformationCalculatedPoint()
            : base(null, new StreamingContext())
        { }
        public CordinateTransformationCalculatedPoint(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
        public readonly PropertyData _xLocalProperty = RegisterProperty("XLocal", typeof(double), double.NaN);
        [ProtocolPropertyData("XLocal"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double XLocal
        {
            get { return GetValue<double>(_xLocalProperty); }
            set { SetValue(_xLocalProperty, value); }
        }
        public readonly PropertyData _yLocalProperty = RegisterProperty("YLocal", typeof(double), double.NaN);
        [ProtocolPropertyData("YLocal"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double YLocal
        {
            get { return GetValue<double>(_yLocalProperty); }
            set { SetValue(_yLocalProperty, value); }
        }
    }
}
