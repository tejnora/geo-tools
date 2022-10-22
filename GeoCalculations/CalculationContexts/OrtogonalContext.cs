using System.Windows;
using DotNetMatrix;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoCalculations.Protocol;

namespace GeoCalculations.CalculationContexts
{
    public class OrtogonalContext : CalculationContextWithSimpleTable<PointBaseEx>
    {
        readonly PropertyData _scaleProperty = RegisterProperty("Scale", typeof(double), double.NaN);
        [ProtocolPropertyData("Scale"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Distance)]
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

        readonly PropertyData _lengthFromCoordinatesProperty = RegisterProperty("LengthFromCoordinates", typeof(double), double.NaN);
        public double LengthFromCoordinates
        {
            get { return GetValue<double>(_lengthFromCoordinatesProperty); }
            set { SetValue(_lengthFromCoordinatesProperty, value); }
        }

        readonly PropertyData _measuringLengthProperty = RegisterProperty("MeasuringLength", typeof(double), double.NaN);
        public double MeasuringLength
        {
            get { return GetValue<double>(_measuringLengthProperty); }
            set { SetValue(_measuringLengthProperty, value); }
        }

        readonly PropertyData _stationingCorrectionProperty = RegisterProperty("StationingCorrection", typeof(double), double.NaN);
        public double StationingCorrection
        {
            get { return GetValue<double>(_stationingCorrectionProperty); }
            set { SetValue(_stationingCorrectionProperty, value); }
        }


        public GeneralMatrix Matrix { get; set; }

        public override bool AddNode()
        {
            if (Node.Stationing > 2000)
            {
                LanguageConverter.ResolveDictionary().ShowMessageBox("OrtogonalMethodDialog.26", null, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return base.AddNode();
        }
    }
}
