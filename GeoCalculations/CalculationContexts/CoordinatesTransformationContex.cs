using System;
using System.Linq;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Methods;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    public enum TransformationTypes
    {
        Similar,
        HelmertSimilar,
        Identity,
        Affine
    }

    public class CoordinatesTransformationContex : CalculationContextWithSimpleTable<CordinateTransformationPoint>
    {
        public CoordinatesTransformationContex()
        {

        }

        public CoordinatesTransformationContex(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        readonly PropertyData _transformationTypesProperty = RegisterProperty("TransformationType", typeof(TransformationTypes), TransformationTypes.HelmertSimilar);
        public TransformationTypes TransformationType
        {
            get { return GetValue<TransformationTypes>(_transformationTypesProperty); }
            set { SetValue(_transformationTypesProperty, value); }
        }

        readonly PropertyData _rotationProperty = RegisterProperty("Rotation", typeof(double), double.NaN);
        [ProtocolPropertyData("Rotation"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double Rotation
        {
            get { return GetValue<double>(_rotationProperty); }
            set { SetValue(_rotationProperty, value); }
        }

        readonly PropertyData _scaleProperty = RegisterProperty("Scale", typeof(double), double.NaN);
        [ProtocolPropertyData("Scale"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Scale)]
        public double Scale
        {
            get { return GetValue<double>(_scaleProperty); }
            set { SetValue(_scaleProperty, value); }
        }

        [ProtocolPropertyData("ResiduesMiddleDeviationX"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double ResiduesMiddleDeviationX
        {
            get
            {
                var sum = TableNodes.Sum(residuesPoint => Math.Pow(residuesPoint.vX, 2));
                return Math.Sqrt(sum / (TableNodes.Count - 1));
            }
        }

        [ProtocolPropertyData("ResiduesMiddleDeviationY"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double ResiduesMiddleDeviationY
        {
            get
            {
                var sum = TableNodes.Sum(residuesPoint => Math.Pow(residuesPoint.vY, 2));
                return Math.Sqrt(sum / (TableNodes.Count - 1));
            }
        }

        [ProtocolPropertyData("ResiduesMiddleDeviation"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
        public double MiddleDeviation
        {
            get { return SimpleCalculation.CalculateM0Red(ResiduesMiddleDeviationX, ResiduesMiddleDeviationY); }
        }


        readonly PropertyData _inputFileNameProperty = RegisterProperty("InputFileName", typeof(string), string.Empty);
        public string InputFileName
        {
            get { return GetValue<string>(_inputFileNameProperty); }
            set { SetValue(_inputFileNameProperty, value); }
        }

        readonly PropertyData _outputFileNameProperty = RegisterProperty("OutputFileName", typeof(string), string.Empty);
        public string OutputFileName
        {
            get { return GetValue<string>(_outputFileNameProperty); }
            set { SetValue(_outputFileNameProperty, value); }
        }

        public override void ResetBeforeCalculation()
        {
            base.ResetBeforeCalculation();
            foreach (var node in TableNodes)
            {
                node.dX = double.NaN;
                node.dY = double.NaN;
            }
        }

    }
}
