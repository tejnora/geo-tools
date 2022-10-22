using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class DirectionIntersectionCalculatedPoint
        : CalculatedPointBase
    {
        public DirectionIntersectionCalculatedPoint()
        {
            
        }

        public DirectionIntersectionCalculatedPoint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
        
        readonly PropertyData _directionFromAProperty = RegisterProperty("DirectionFromA", typeof(double), double.NaN);
        [ProtocolPropertyData("DirectionFromA"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double DirectionFromA
        {
            get { return GetValue<double>(_directionFromAProperty); }
            set { SetValue(_directionFromAProperty, value); }
        }

        public readonly PropertyData _directionFromBProperty = RegisterProperty("DirectionFromB", typeof(double), double.NaN);
        [ProtocolPropertyData("DirectionFromB"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double DirectionFromB
        {
            get { return GetValue<double>(_directionFromBProperty); }
            set { SetValue(_directionFromBProperty, value); }
        }

        public readonly PropertyData _AngleOfIntersectionProperty = RegisterProperty("AngleOfIntersection", typeof(double), double.NaN);
        public double AngleOfIntersection
        {
            get { return GetValue<double>(_AngleOfIntersectionProperty); }
            set { SetValue(_AngleOfIntersectionProperty, value); }
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
