using System;
using System.Runtime.Serialization;
using GeoBase.Utils;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class ConstructionDistancePoint : PointBaseEx
    {
        public ConstructionDistancePoint()
        {

        }
        public ConstructionDistancePoint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        readonly PropertyData _xLocalProperty = RegisterProperty("XLocal", typeof(double), double.NaN);
        public double XLocal
        {
            get { return GetValue<double>(_xLocalProperty); }
            set { SetValue(_xLocalProperty, value); }
        }

        readonly PropertyData _yLocalProperty = RegisterProperty("YLocal", typeof(double), double.NaN);
        public double YLocal
        {
            get { return GetValue<double>(_yLocalProperty); }
            set { SetValue(_yLocalProperty, value); }
        }

        readonly PropertyData _directionProperty = RegisterProperty("Direction", typeof(double), double.NaN);
        public double Direction
        {
            get { return GetValue<double>(_directionProperty); }
            set { SetValue(_directionProperty, value); }
        }
    }
}
