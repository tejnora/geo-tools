using System;
using System.Runtime.Serialization;
using DotNetMatrix;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    [Serializable]
    public class OrientationContext : CalculationContextWithSimpleTable<OrientationPoint>
    {
        public OrientationContext()
        {
        }

        public OrientationContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _orientationMovementProperty = RegisterProperty("OrientationMovement", typeof(double), double.NaN);
        [ProtocolPropertyData("OrientationMovement"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double OrientationMovement
        {
            get { return GetValue<double>(_orientationMovementProperty); }
            set { SetValue(_orientationMovementProperty, value); }
        }

        readonly PropertyData _calculateHeighsAlsoProperty = RegisterProperty("CalculateHeightsAlso", typeof(bool), false);
        public bool CalculateHeightsAlso
        {
            get { return GetValue<bool>(_calculateHeighsAlsoProperty); }
            set { SetValue(_calculateHeighsAlsoProperty, value); }
        }

        readonly PropertyData _m0Property = RegisterProperty("m0", typeof(double), double.NaN);
        [ProtocolPropertyData("m0"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double m0
        {
            get { return GetValue<double>(_m0Property); }
            set { SetValue(_m0Property, value); }
        }

        readonly PropertyData _m1Property = RegisterProperty("m1", typeof(double), double.NaN);
        [ProtocolPropertyData("m1"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double m1
        {
            get { return GetValue<double>(_m1Property); }
            set { SetValue(_m1Property, value); }
        }

        readonly PropertyData _scaleProperty = RegisterProperty("Scale", typeof(double), double.NaN);
        [ProtocolPropertyData("Scale"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Scale)]
        public double Scale
        {
            get { return GetValue<double>(_scaleProperty); }
            set { SetValue(_scaleProperty, value); }
        }

        readonly PropertyData _rotationProperty = RegisterProperty("Rotation", typeof(double), double.NaN);
        [ProtocolPropertyData("Rotation"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double Rotation
        {
            get { return GetValue<double>(_rotationProperty); }
            set { SetValue(_rotationProperty, value); }
        }

        readonly PropertyData _xProperty = RegisterProperty("X", typeof(double), double.NaN);
        [ProtocolPropertyDataAttribute("X"), ProtocolPropertyValueTypeAttribute(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double X
        {
            get { return GetValue<double>(_xProperty); }
            set { SetValue(_xProperty, value); }
        }

        readonly PropertyData _yProperty = RegisterProperty("Y", typeof(double), double.NaN);
        [ProtocolPropertyDataAttribute("Y"), ProtocolPropertyValueTypeAttribute(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double Y
        {
            get { return GetValue<double>(_yProperty); }
            set { SetValue(_yProperty, value); }
        }

        public GeneralMatrix Matrix { get; set; }
    }
}
