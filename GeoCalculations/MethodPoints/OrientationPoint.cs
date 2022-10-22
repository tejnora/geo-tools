using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.MethodPoints
{
    [Serializable]
    public class OrientationPoint : PointBaseEx
    {
        public OrientationPoint()
        {

        }
        public OrientationPoint(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        readonly PropertyData _dHCalculatedProperty = RegisterProperty("dhCalculated", typeof(double), double.NaN);
        [ProtocolPropertyData("dhCalculated"), ProtocolPropertyValueType(ProtocolPropertyValueTypeAttribute.Types.Angle)]
        public double dhCalculated
        {
            get { return GetValue<double>(_dHCalculatedProperty); }
            set { SetValue(_dHCalculatedProperty, value); }
        }

        readonly PropertyData _isEnabled = RegisterProperty("IsEnabled", typeof(bool), true);
        public bool IsEnabled
        {
            get { return GetValue<bool>(_isEnabled); }
            set { SetValue(_isEnabled, value); }
        }

        readonly PropertyData _ZpProperty = RegisterProperty("Zp", typeof(double), double.NaN);
        public double Zp
        {
            get { return GetValue<double>(_ZpProperty); }
            set { SetValue(_ZpProperty, value); }
        }

        readonly PropertyData _vZProperty = RegisterProperty("vZ", typeof(double), double.NaN);
        public double vZ
        {
            get { return GetValue<double>(_vZProperty); }
            set { SetValue(_vZProperty, value); }
        }

        readonly PropertyData _elevationDifferenceScale = RegisterProperty("ElevationDifferenceScale", typeof(double), double.NaN);
        public double ElevationDifferenceScale
        {
            get { return GetValue<double>(_elevationDifferenceScale); }
            set { SetValue(_elevationDifferenceScale, value); }
        }

        public double Temp1
        {
            get;
            set;
        }

        public double Temp2
        {
            get;
            set;
        }

        /*        public override void LoadValues(IPointLoadAdapter adapter)
                {
                    base.LoadValues(adapter);
                    if ((adapter.Properties & LoadAdapterProperties.MeasureData) == 0) return;
                    Hz = adapter.Hz;
                    dH = adapter.dH;
                }

                public override void SetElement(ImportExportElementType type, object value)
                {
                    switch (type)
                    {
                        case ImportExportElementType.HZ:
                            Hz = (double)value * 200 / Math.PI;
                            return;
                        case ImportExportElementType.DIST:
                            Delka = (double)value;
                            return;
                        case ImportExportElementType.VERT:
                            ZenitAgnel = (double)value * 200 / Math.PI; ;
                            return;
                        case ImportExportElementType.SIGNAL:
                            Signal = (double)value;
                            return;
                        case ImportExportElementType.DH:
                            dH = (double)value;
                            return;
                    }
                    base.SetElement(type, value);
                }*/

        /*  public override void Export(IPointExporter pointExporter)
          {
              base.Export(pointExporter);
              pointExporter.AddValue(ImportExportElementType.HZ, Hz / 200 * Math.PI);
              pointExporter.AddValue(ImportExportElementType.DIST, Delka);
              pointExporter.AddValue(ImportExportElementType.VERT, ZenitAgnel / 200 * Math.PI);
              pointExporter.AddValue(ImportExportElementType.DH, dH);
              pointExporter.AddValue(ImportExportElementType.SIGNAL, Signal);
          }*/

    }
}
