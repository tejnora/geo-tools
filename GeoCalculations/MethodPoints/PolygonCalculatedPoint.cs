using System;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoCalculations.Points
{
    public class PolygonCalculatedPoint : DataObjectBase<PolygonCalculatedPoint>
    {
        public PolygonCalculatedPoint()
            : base(null, new StreamingContext())
        {
        }
        public PolygonCalculatedPoint(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
        public readonly PropertyData _predcisliProperty = RegisterProperty("Prefix", typeof(string), string.Empty);
        public string Predcisli
        {
            get { return GetValue<string>(_predcisliProperty); }
            set { SetValue(_predcisliProperty, value); }
        }

        public readonly PropertyData _cisloProperty = RegisterProperty("Number", typeof(string), string.Empty);
        public string Cislo
        {
            get { return GetValue<string>(_cisloProperty); }
            set { SetValue(_cisloProperty, value); }
        }

        public readonly PropertyData _predcisli2Property = RegisterProperty("Predcisli2", typeof(string), string.Empty);
        public string Predcisli2
        {
            get { return GetValue<string>(_predcisli2Property); }
            set { SetValue(_predcisli2Property, value); }
        }

        public readonly PropertyData _cislo2Property = RegisterProperty("Cislo2", typeof(string), string.Empty);
        public string Cislo2
        {
            get { return GetValue<string>(_cislo2Property); }
            set { SetValue(_cislo2Property, value); }
        }
        public readonly PropertyData _xProperty = RegisterProperty("X", typeof(double), double.NaN);
        public double X
        {
            get { return GetValue<double>(_xProperty); }
            set { SetValue(_xProperty, value); }
        }

        public readonly PropertyData _yProperty = RegisterProperty("Y", typeof(double), double.NaN);
        public double Y
        {
            get { return GetValue<double>(_yProperty); }
            set { SetValue(_yProperty, value); }
        }

        public readonly PropertyData _zProperty = RegisterProperty("Height", typeof(double), double.NaN);
        public double Z
        {
            get { return GetValue<double>(_zProperty); }
            set { SetValue(_zProperty, value); }
        }
        public readonly PropertyData _zTamProperty = RegisterProperty("ZTam", typeof(double), double.NaN);
        public double ZTam
        {
            get { return GetValue<double>(_zTamProperty); }
            set { SetValue(_zTamProperty, value); }
        }
        public readonly PropertyData _zZpetProperty = RegisterProperty("ZZpet", typeof(double), double.NaN);
        public double ZZpet
        {
            get { return GetValue<double>(_zZpetProperty); }
            set { SetValue(_zZpetProperty, value); }
        }
        public readonly PropertyData _dHTamProperty = RegisterProperty("dhTam", typeof(double), double.NaN);
        public double dhTam
        {
            get { return GetValue<double>(_dHTamProperty); }
            set { SetValue(_dHTamProperty, value); }
        }
        public readonly PropertyData _dhZpetProperty = RegisterProperty("dhZpet", typeof(double), double.NaN);
        public double dhZpet
        {
            get { return GetValue<double>(_dhZpetProperty); }
            set { SetValue(_dhZpetProperty, value); }
        }

        public readonly PropertyData _dHProperty = RegisterProperty("dH", typeof(double), double.NaN);
        public double dH
        {
            get { return GetValue<double>(_dHProperty); }
            set { SetValue(_dHProperty, value); }
        }

        public readonly PropertyData _VdHProperty = RegisterProperty("VdH", typeof(double), double.NaN);
        public double VdH
        {
            get { return GetValue<double>(_VdHProperty); }
            set { SetValue(_VdHProperty, value); }
        }

        public readonly PropertyData _VdHVyrovnaniProperty = RegisterProperty("VdHVyrovnani", typeof(double), double.NaN);
        public double VdHVyrovnani
        {
            get { return GetValue<double>(_VdHVyrovnaniProperty); }
            set { SetValue(_VdHVyrovnaniProperty, value); }
        }

        public readonly PropertyData _dhVyrovnaniProperty = RegisterProperty("dhVyrovanini", typeof(double), double.NaN);
        public double dhVyrovanini
        {
            get { return GetValue<double>(_dhVyrovnaniProperty); }
            set { SetValue(_dhVyrovnaniProperty, value); }
        }
        public string UplneCislo
        {
            get
            {
                return string.Format("{0:00000000}{1:0000}", Predcisli, Cislo);
            }
            set
            {
                throw new ArgumentException("NEVER_GET_HERE");
            }
        }
        public string UplneCislo2
        {
            get
            {
                return string.Format("{0:00000000}{1:0000}", Predcisli2, Cislo2);
            }
            set
            {
                throw new ArgumentException("NEVER_GET_HERE");
            }
        }

    }
}
