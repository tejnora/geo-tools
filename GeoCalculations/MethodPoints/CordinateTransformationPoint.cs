using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class CordinateTransformationPoint : PointBaseEx
    {
        public CordinateTransformationPoint()
            : base(null, new StreamingContext())
        {

        }

        public CordinateTransformationPoint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        readonly PropertyData _xLocalProperty = RegisterProperty("XLocal", typeof(double), double.NaN);
        [ProtocolPropertyData("XLocal"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double XLocal
        {
            get { return GetValue<double>(_xLocalProperty); }
            set { SetValue(_xLocalProperty, value); }
        }

        readonly PropertyData _yLocalProperty = RegisterProperty("YLocal", typeof(double), double.NaN);
        [ProtocolPropertyData("YLocal"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double YLocal
        {
            get { return GetValue<double>(_yLocalProperty); }
            set { SetValue(_yLocalProperty, value); }
        }
    }
}
