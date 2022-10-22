using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.Points;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class MeasuredPoint : PointBaseEx
    {
        public MeasuredPoint()
            : base(null, new StreamingContext())
        {
            MeasuringBack = new OrientationPoint();
            MeasuringForward = new OrientationPoint();
            PointOfView = new PointBaseEx();
        }
        public MeasuredPoint(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        readonly PropertyData _measuringBackProperty = RegisterProperty("MeasuringBack", typeof(OrientationPoint), new OrientationPoint());
        [ProtocolPropertyData("MeasuringBack"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public OrientationPoint MeasuringBack
        {
            get { return GetValue<OrientationPoint>(_measuringBackProperty); }
            set { SetValue(_measuringBackProperty, value); }
        }
        readonly PropertyData _measuringForwardProperty = RegisterProperty("MeasuringForward", typeof(OrientationPoint), new OrientationPoint());
        [ProtocolPropertyData("MeasuringForward"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public OrientationPoint MeasuringForward
        {
            get { return GetValue<OrientationPoint>(_measuringForwardProperty); }
            set { SetValue(_measuringForwardProperty, value); }
        }
        readonly PropertyData _pointOfView = RegisterProperty("PointOfView", typeof(PointBaseEx), new PointBaseEx());
        [ProtocolPropertyData("PointOfView")]
        public PointBaseEx PointOfView
        {
            get { return GetValue<PointBaseEx>(_pointOfView); }
            set { SetValue(_pointOfView, value); }
        }
        readonly PropertyData _smerZpet = RegisterProperty("SmerZpet", typeof(double), double.NaN);
        public double SmerZpet
        {
            get { return GetValue<double>(_smerZpet); }
            set { SetValue(_smerZpet, value); }
        }
        readonly PropertyData _smerVpred = RegisterProperty("SmerVpred", typeof(double), double.NaN);
        public double SmerVpred
        {
            get { return GetValue<double>(_smerVpred); }
            set { SetValue(_smerVpred, value); }
        }
        readonly PropertyData _delkaVpredProperty = RegisterProperty("DelkaVpred", typeof(double), double.NaN);
        public double DelkaVpred
        {
            get { return GetValue<double>(_delkaVpredProperty); }
            set { SetValue(_delkaVpredProperty, value); }
        }
        readonly PropertyData _delkaZpetProperty = RegisterProperty("DelkaVzpet", typeof(double), double.NaN);
        public double DelkaZpet
        {
            get { return GetValue<double>(_delkaZpetProperty); }
            set { SetValue(_delkaZpetProperty, value); }
        }
        readonly PropertyData _rozdilDelekProperty = RegisterProperty("RozdilDelek", typeof(double), double.NaN);
        public double RozdilDelek
        {
            get { return GetValue<double>(_rozdilDelekProperty); }
            set { SetValue(_rozdilDelekProperty, value); }
        }
        readonly PropertyData _uhelProperty = RegisterProperty("Uhel", typeof(double), double.NaN);
        public double Uhel
        {
            get { return GetValue<double>(_uhelProperty); }
            set { SetValue(_uhelProperty, value); }
        }
        readonly PropertyData _uhlovaOdchylkaProperty = RegisterProperty("AngleDeviation", typeof(double), double.NaN);
        public double UhlovaOdchylka
        {
            get { return GetValue<double>(_uhlovaOdchylkaProperty); }
            set { SetValue(_uhlovaOdchylkaProperty, value); }
        }
        readonly PropertyData _zprumerovanaDelkaProperty = RegisterProperty("ZprumerovanaDelka", typeof(double), double.NaN);
        public double ZprumerovanaDelka
        {
            get { return GetValue<double>(_zprumerovanaDelkaProperty); }
            set { SetValue(_zprumerovanaDelkaProperty, value); }
        }
        readonly PropertyData _souradnicovyRozdilXProperty = RegisterProperty("SouradnicovyRozdilX", typeof(double), double.NaN);
        public double SouradnicovyRozdilX
        {
            get { return GetValue<double>(_souradnicovyRozdilXProperty); }
            set { SetValue(_souradnicovyRozdilXProperty, value); }
        }
        readonly PropertyData _souradnicovyRozdilYProperty = RegisterProperty("SouradnicovyRozdilY", typeof(double), double.NaN);
        public double SouradnicovyRozdilY
        {
            get { return GetValue<double>(_souradnicovyRozdilYProperty); }
            set { SetValue(_souradnicovyRozdilYProperty, value); }
        }
        readonly PropertyData _opravaVyrovnaniXProperty = RegisterProperty("OpravaVyrovnaniX", typeof(double), double.NaN);
        public double OpravaVyrovnaniX
        {
            get { return GetValue<double>(_opravaVyrovnaniXProperty); }
            set { SetValue(_opravaVyrovnaniXProperty, value); }
        }
        readonly PropertyData _opravaVyrovnaniYProperty = RegisterProperty("OpravaVyrovnaniY", typeof(double), double.NaN);
        public double OpravaVyrovnaniY
        {
            get { return GetValue<double>(_opravaVyrovnaniYProperty); }
            set { SetValue(_opravaVyrovnaniYProperty, value); }
        }

        public void SetNotification(bool add, PropertyChangedEventHandler notifier)
        {
            if (add)
            {
                MeasuringBack.PropertyChanged += notifier;
                MeasuringForward.PropertyChanged += notifier;
                PointOfView.PropertyChanged += notifier;
            }
            else
            {
                MeasuringBack.PropertyChanged -= notifier;
                MeasuringForward.PropertyChanged -= notifier;
                PointOfView.PropertyChanged -= notifier;
            }
        }

        protected override void ValidateFields()
        {
            var dictionary = LanguageConverter.ResolveDictionary();
            var errorMesg = (string)dictionary.Translate("235", "ErrorText", "Nespravna hodnota.", typeof(string));
            if (string.IsNullOrEmpty(PointOfView.Number)) SetFieldError(_pointOfView, errorMesg);
        }

        public void SetElement(ImportExportElementType type, object value)
        {
            switch (type)
            {
                case ImportExportElementType.NUM:
                case ImportExportElementType.X:
                case ImportExportElementType.Y:
                case ImportExportElementType.Z:
                case ImportExportElementType.MEAS:
                case ImportExportElementType.IH:
                    PointOfView.SetElement(type, value);
                    return;
                case ImportExportElementType.HZ1:
                    MeasuringBack.SetElement(ImportExportElementType.HZ, value);
                    return;
                case ImportExportElementType.HZ2:
                    MeasuringForward.SetElement(ImportExportElementType.HZ, value);
                    return;
                case ImportExportElementType.VERT1:
                    MeasuringBack.SetElement(ImportExportElementType.VERT, value);
                    break;
                case ImportExportElementType.VERT2:
                    MeasuringForward.SetElement(ImportExportElementType.VERT, value);
                    break;
                case ImportExportElementType.DIST1:
                    MeasuringBack.SetElement(ImportExportElementType.DIST, value);
                    return;
                case ImportExportElementType.DIST2:
                    MeasuringForward.SetElement(ImportExportElementType.DIST, value);
                    return;
                case ImportExportElementType.SIGNAL1:
                    MeasuringBack.SetElement(ImportExportElementType.SIGNAL, value);
                    return;
                case ImportExportElementType.SIGNAL2:
                    MeasuringForward.SetElement(ImportExportElementType.SIGNAL, value);
                    return;
                case ImportExportElementType.DH1:
                    MeasuringBack.SetElement(ImportExportElementType.DH, value);
                    return;
                case ImportExportElementType.DH2:
                    MeasuringForward.SetElement(ImportExportElementType.DH, value);
                    return;
                case ImportExportElementType.DISABLED:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
        private enum ExportStructureType
        {
            None,
            PointOfView,
            MeasuringBack,
            MeasuringForward
        }
        private IPointExporter _pointExporter;
        private ExportStructureType _exportStructureType;
        public override void Export(IPointExporter pointExporter)
        {
            _pointExporter = pointExporter;
            _exportStructureType = ExportStructureType.PointOfView;
            PointOfView.Export(pointExporter);
            _exportStructureType = ExportStructureType.MeasuringBack;
            MeasuringBack.Export(pointExporter);
            _exportStructureType = ExportStructureType.MeasuringForward;
            MeasuringForward.Export(pointExporter);
            _exportStructureType = ExportStructureType.None;
        }

        public void AddEmptyLine()
        {
            _pointExporter.AddEmptyLine();
        }

        public void AddValue(ImportExportElementType type, object value)
        {
            switch (_exportStructureType)
            {
                case ExportStructureType.None:
                    throw new ArgumentOutOfRangeException();
                case ExportStructureType.MeasuringBack:
                    {
                        switch (type)
                        {
                            case ImportExportElementType.HZ:
                                _pointExporter.AddValue(ImportExportElementType.HZ1, value);
                                break;
                            case ImportExportElementType.DIST:
                                _pointExporter.AddValue(ImportExportElementType.DIST1, value);
                                break;
                            case ImportExportElementType.VERT:
                                _pointExporter.AddValue(ImportExportElementType.VERT1, value);
                                break;
                            case ImportExportElementType.SIGNAL:
                                _pointExporter.AddValue(ImportExportElementType.SIGNAL1, value);
                                break;
                            case ImportExportElementType.DH:
                                _pointExporter.AddValue(ImportExportElementType.DH1, value);
                                break;
                            case ImportExportElementType.NUM:
                            case ImportExportElementType.X:
                            case ImportExportElementType.Y:
                            case ImportExportElementType.Z:
                            case ImportExportElementType.MEAS:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;
                case ExportStructureType.MeasuringForward:
                    switch (type)
                    {
                        case ImportExportElementType.HZ:
                            _pointExporter.AddValue(ImportExportElementType.HZ2, value);
                            break;
                        case ImportExportElementType.DIST:
                            _pointExporter.AddValue(ImportExportElementType.DIST2, value);
                            break;
                        case ImportExportElementType.VERT:
                            _pointExporter.AddValue(ImportExportElementType.VERT2, value);
                            break;
                        case ImportExportElementType.SIGNAL:
                            _pointExporter.AddValue(ImportExportElementType.SIGNAL2, value);
                            break;
                        case ImportExportElementType.DH:
                            _pointExporter.AddValue(ImportExportElementType.DH2, value);
                            break;
                        case ImportExportElementType.NUM:
                        case ImportExportElementType.X:
                        case ImportExportElementType.Y:
                        case ImportExportElementType.Z:
                        case ImportExportElementType.MEAS:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ExportStructureType.PointOfView:
                    {
                        switch (type)
                        {
                            case ImportExportElementType.NUM:
                                _pointExporter.AddValue(type, value);
                                break;
                            default:
                                return;

                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void FlushValues()
        {
            _pointExporter.FlushValues();
        }

        public void AddSettingValue(string name, string value)
        {
            _pointExporter.AddSettingValue(name, value);
        }
    }
}
