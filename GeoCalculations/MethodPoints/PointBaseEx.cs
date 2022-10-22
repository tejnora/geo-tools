using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Methods;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class PointBaseEx : PointBase
    {
        public PointBaseEx()
        {

        }

        public PointBaseEx(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        readonly PropertyData _hzProperty = RegisterProperty("Hz", typeof(double), double.NaN);
        [ProtocolPropertyData("Hz"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double Hz
        {
            get { return GetValue<double>(_hzProperty); }
            set { SetValue(_hzProperty, value); }
        }

        readonly PropertyData _distanceProperty = RegisterProperty("Distance", typeof(double), double.NaN);
        [ProtocolPropertyData("Distance"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double Distance
        {
            get { return GetValue<double>(_distanceProperty); }
            set { SetValue(_distanceProperty, value); }
        }

        readonly PropertyData _zenitAgnleProperty = RegisterProperty("ZenitAngle", typeof(double), double.NaN);
        [ProtocolPropertyData("ZenitAngle"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double ZenitAngle
        {
            get { return GetValue<double>(_zenitAgnleProperty); }
            set { SetValue(_zenitAgnleProperty, value); }
        }

        readonly PropertyData _targetHeightProperty = RegisterProperty("TargetHeight", typeof(double), double.NaN);
        [ProtocolPropertyData("TargetHeight"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Heigth)]
        public double TargetHeight
        {
            get { return GetValue<double>(_targetHeightProperty); }
            set { SetValue(_targetHeightProperty, value); }
        }

        readonly PropertyData _descriptionProperty = RegisterProperty("Description", typeof(string), string.Empty);
        [ProtocolPropertyData("Description"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Text)]
        public string Description
        {
            get { return GetValue<string>(_descriptionProperty); }
            set { SetValue(_descriptionProperty, value); }
        }

        readonly PropertyData _signalHeightProperty = RegisterProperty("SignalHeight", typeof(double), double.NaN);
        [ProtocolPropertyData("SignalHeight"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Heigth)]
        public double SignalHeight
        {
            get { return GetValue<double>(_signalHeightProperty); }
            set { SetValue(_signalHeightProperty, value); }
        }

        readonly PropertyData _stationingProperty = RegisterProperty("Stationing", typeof(double), double.NaN);
        [ProtocolPropertyData("Stationing"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double Stationing
        {
            get { return GetValue<double>(_stationingProperty); }
            set { SetValue(_stationingProperty, value); }
        }

        readonly PropertyData _verticalProperty = RegisterProperty("Vertical", typeof(double), double.NaN);
        [ProtocolPropertyData("Vertical"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double Vertical
        {
            get { return GetValue<double>(_verticalProperty); }
            set { SetValue(_verticalProperty, value); }
        }

        readonly PropertyData _verticalOrientationProperty = RegisterProperty("VerticalOrientation", typeof(double), double.NaN);
        [ProtocolPropertyData("VerticalOrientation"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double VerticalOrientation //oprava orientace
        {
            get { return GetValue<double>(_verticalOrientationProperty); }
            set { SetValue(_verticalOrientationProperty, value); }
        }

        readonly PropertyData _verticalDistanceProperty = RegisterProperty("VerticalDistance", typeof(double), double.NaN);
        [ProtocolPropertyData("VerticalDistance"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double VerticalDistance //oprava delka
        {
            get { return GetValue<double>(_verticalDistanceProperty); }
            set { SetValue(_verticalDistanceProperty, value); }
        }


        readonly PropertyData _dYProperty = RegisterProperty("dY", typeof(double), double.NaN);
        [ProtocolPropertyData("dY"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double dY
        {
            get { return GetValue<double>(_dYProperty); }
            set { SetValue(_dYProperty, value); }
        }

        readonly PropertyData _dXProperty = RegisterProperty("dX", typeof(double), double.NaN);
        [ProtocolPropertyData("dX"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double dX
        {
            get { return GetValue<double>(_dXProperty); }
            set { SetValue(_dXProperty, value); }
        }

        readonly PropertyData _vYProperty = RegisterProperty("vY", typeof(double), double.NaN);
        [ProtocolPropertyData("vY"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double vY
        {
            get { return GetValue<double>(_vYProperty); }
            set { SetValue(_vYProperty, value); }
        }

        readonly PropertyData _vXProperty = RegisterProperty("vX", typeof(double), double.NaN);
        [ProtocolPropertyData("vX"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double vX
        {
            get { return GetValue<double>(_vXProperty); }
            set { SetValue(_vXProperty, value); }
        }

        [ProtocolPropertyData("m0"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double m0
        {
            get { return SimpleCalculation.CalculateM0Red(vX, vY); }
        }

        readonly PropertyData _directionAzimutProperty = RegisterProperty("DirectionAzimut", typeof(double), double.NaN);
        [ProtocolPropertyData("DirectionAzimut"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double DirectionAzimut
        {
            get { return GetValue<double>(_directionAzimutProperty); }
            set { SetValue(_directionAzimutProperty, value); }
        }

        readonly PropertyData _elevationDifferenceProperty = RegisterProperty("ElevationDifference", typeof(double), double.NaN);
        [ProtocolPropertyData("ElevationDifference"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double ElevationDifference
        {
            get { return GetValue<double>(_elevationDifferenceProperty); }
            set { SetValue(_elevationDifferenceProperty, value); }
        }


        readonly PropertyData _scaleProperty = RegisterProperty("Scale", typeof(double), double.NaN);
        [ProtocolPropertyData("Scale"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Scale)]
        public double Scale
        {
            get { return GetValue<double>(_scaleProperty); }
            set { SetValue(_scaleProperty, value); }
        }

        public override void LoadValues(IPointLoadAdapter adapter)
        {
            base.LoadValues(adapter);
            Description = adapter.Description;
            if ((adapter.Properties & LoadAdapterProperties.MeasureData) == 0) return;
            Hz = adapter.Hz;
            ZenitAngle = adapter.ZenitAgnel;
            Distance = adapter.HorizontalDistance;
            ElevationDifference = adapter.ElevationDifference;
            SignalHeight = adapter.Signal;
        }
    }
}
