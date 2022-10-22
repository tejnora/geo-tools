using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class DirectionAzimutAndLengthCalculatedPoint : CalculatedPointBase
    {
        public DirectionAzimutAndLengthCalculatedPoint()
            : base(null, new StreamingContext())
        {

        }
        
        public DirectionAzimutAndLengthCalculatedPoint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

        readonly PropertyData _slopeProperty = RegisterProperty("Slope", typeof(double), double.NaN);
        [ProtocolPropertyData("Slope"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double Slope
        {
            get { return GetValue<double>(_slopeProperty); }
            set { SetValue(_slopeProperty, value); }
        }

        readonly PropertyData _sikmaDelkaProperty = RegisterProperty("ObliqueLength", typeof(double), double.NaN);
        [ProtocolPropertyData("ObliqueLength"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double ObliqueLength
        {
            get { return GetValue<double>(_sikmaDelkaProperty); }
            set { SetValue(_sikmaDelkaProperty, value); }
        }

        readonly PropertyData _gradientProperty = RegisterProperty("Gradient", typeof(double), double.NaN);
        [ProtocolPropertyData("Gradient"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double Gradient
        {
            get { return GetValue<double>(_gradientProperty); }
            set { SetValue(_gradientProperty, value); }
        }
    }
}
