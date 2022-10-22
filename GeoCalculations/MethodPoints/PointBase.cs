using System;
using System.Globalization;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Points;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class PointBase : DataObjectBase<PointBase>, IPointImport, IPointExport
    {
        public PointBase()
            : base(null, new StreamingContext())
        {
        }
        public PointBase(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public virtual void Init()
        {
        }

        readonly PropertyData _prefixProperty = RegisterProperty("Prefix", typeof(string), string.Empty);
        [ProtocolPropertyData("Prefix"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Text)]
        public string Prefix
        {
            get { return GetValue<string>(_prefixProperty); }
            set { SetValue(_prefixProperty, value); }
        }

        readonly PropertyData _numberProperty = RegisterProperty("Number", typeof(string), string.Empty);
        [ProtocolPropertyData("Number"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Text)]
        public string Number
        {
            get { return GetValue<string>(_numberProperty); }
            set { SetValue(_numberProperty, value); }
        }

        readonly PropertyData _xProperty = RegisterProperty("X", typeof(double), double.NaN);
        [ProtocolPropertyData("X"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double X
        {
            get { return GetValue<double>(_xProperty); }
            set { SetValue(_xProperty, value); }
        }

        readonly PropertyData _yProperty = RegisterProperty("Y", typeof(double), double.NaN);
        [ProtocolPropertyData("Y"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double Y
        {
            get { return GetValue<double>(_yProperty); }
            set { SetValue(_yProperty, value); }
        }

        readonly PropertyData _zProperty = RegisterProperty("Z", typeof(double), double.NaN);
        [ProtocolPropertyData("Z"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Cordinate)]
        public double Z
        {
            get { return GetValue<double>(_zProperty); }
            set { SetValue(_zProperty, value); }
        }

        [ProtocolPropertyData("NumberWithPrefix"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Text)]
        public string NumberWithPrefix
        {
            get { return string.Format("{0:00000000}{1:0000}", Prefix, Number); }
        }

        public virtual void SetElement(ImportExportElementType type, object value)
        {
            switch (type)
            {
                case ImportExportElementType.NUM:
                    {
                        var number = (string)value;
                        if (string.IsNullOrEmpty(number)) throw new ArgumentOutOfRangeException("InvalidNumber");
                        number = number.PadLeft(12, '0');
                        Prefix = number.Substring(0, 8);
                        Number = number.Substring(8, 4);
                    }
                    break;
                case ImportExportElementType.X:
                    X = (double)value;
                    break;
                case ImportExportElementType.Y:
                    Y = (double)value;
                    break;
                case ImportExportElementType.Z:
                    Z = (double)value;
                    break;
                case ImportExportElementType.MEAS:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ImportElementType");
            }
        }
        public virtual void Export(IPointExporter pointExporter)
        {
            pointExporter.AddValue(ImportExportElementType.NUM, Prefix.PadLeft(8, '0') + Number.PadLeft(4, '0'));
            pointExporter.AddValue(ImportExportElementType.X, X);
            pointExporter.AddValue(ImportExportElementType.Y, Y);
            pointExporter.AddValue(ImportExportElementType.Z, Z);
            pointExporter.AddValue(ImportExportElementType.MEAS, Prefix.PadLeft(8, '0') + Number.PadLeft(4, '0'));
        }

        public virtual void LoadValues(IPointLoadAdapter adapter)
        {
            if ((adapter.Properties & LoadAdapterProperties.CordinateData) != 0)
            {
                X = adapter.X;
                Y = adapter.Y;
                Z = adapter.Z;
            }
            Prefix = adapter.Prefix;
            Number = adapter.Number;
        }

        public void IncrementNumber()
        {
            if (string.IsNullOrEmpty(Number))
                Number = "1";
            int value;
            if (!int.TryParse(Number, out value)) return;
            value++;
            Number = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
