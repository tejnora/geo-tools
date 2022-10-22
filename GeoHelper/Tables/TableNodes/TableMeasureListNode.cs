using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using GeoBase.Utils;
using GeoHelper.FileParses;

namespace GeoHelper.Tables.TableNodes
{
    [Serializable]
    public class TableMeasureListNode : TableNodesBase
    {
        public enum PointTypes
        {
            PointOfView,
            Orientation,
            MeasuringPoint
        }

        public TableMeasureListNode()
        {
            Description = string.Empty;
        }

        public TableMeasureListNode(IStationRecord station)
        {
            AssignPredcisliAndCislo(station.StationPointNumber);
            Hz = 0;
            ZenitAngle = 0;
            Signal = station.HeightOfInstrument;
            PointType = PointTypes.PointOfView;
            Description = string.Empty;
        }

        public TableMeasureListNode(INodeSideshot sideshot)
        {
            AssignPredcisliAndCislo(sideshot.PointNumber);
            Hz = sideshot.Hz;
            ZenitAngle = sideshot.Z;
            Signal = sideshot.HeightOfTarget;
            PointType = PointTypes.Orientation;
            if (sideshot.SlopeDistance != 0)
            {
                var val = Math.PI * sideshot.Z * 0.9 / 180.0;
                HorizontalDistance = Math.Round(sideshot.SlopeDistance * Math.Sin(val), 4);
            }
            Description = string.Empty;
        }

        public TableMeasureListNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public void AssignNode(TableMeasureListNode aTableMeasureListNode)
        {
            base.AssignNode(aTableMeasureListNode);
            Hz = aTableMeasureListNode.Hz;
            HorizontalDistance = aTableMeasureListNode.HorizontalDistance;
            ZenitAngle = aTableMeasureListNode.ZenitAngle;
            ElevationDefference = aTableMeasureListNode.ElevationDefference;
            Signal = aTableMeasureListNode.Signal;
            Scale = aTableMeasureListNode.Scale;
            PointType = aTableMeasureListNode.PointType;
        }

        public override void Serialize(BinaryWriter w)
        {
            w.Write((UInt32)1); //version
            base.Serialize(w);
            w.Write(Hz);
            w.Write(HorizontalDistance);
            w.Write(ZenitAngle);
            w.Write(ElevationDefference);
            w.Write(Signal);
            w.Write(Scale);
            w.Write((UInt32)PointType);
        }

        public override void Deserialize(BinaryReader r)
        {
            r.ReadUInt32();
            base.Deserialize(r);
            Hz = r.ReadDouble();
            HorizontalDistance = r.ReadDouble();
            ZenitAngle = r.ReadDouble();
            ElevationDefference = r.ReadDouble();
            Signal = r.ReadDouble();
            Scale = r.ReadDouble();
            PointType = (PointTypes)r.ReadUInt32();
        }

        public override bool SetValue(SymbolToken propertyToken, string propertyValue)
        {
            if (base.SetValue(propertyToken, propertyValue))
                return true;
            switch (propertyToken.Symbol)
            {
                case "Ha":
                    {
                        if (propertyValue.Length == 0)
                        {
                            Hz = double.NaN;
                            return true;
                        }
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        Hz = temp;
                        return true;
                    }
                case "Hl":
                    {
                        if (propertyValue.Length == 0)
                        {
                            HorizontalDistance = double.NaN;
                            return true;
                        }
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        HorizontalDistance = temp;
                        return true;
                    }
                case "Za":
                    {
                        if (propertyValue.Length == 0)
                        {
                            ZenitAngle = double.NaN;
                            return true;
                        }
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        ZenitAngle = temp;
                        return true;
                    }
                case "Su":
                    {
                        if (propertyValue.Length == 0)
                        {
                            ElevationDefference = double.NaN;
                            return true;
                        }
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        ElevationDefference = temp;
                        return true;
                    }
                case "Hp":
                    {
                        if (propertyValue.Length == 0)
                        {
                            Signal = double.NaN;
                            return true;
                        }
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        Signal = temp;
                        return true;
                    }
                case "W":
                    {
                        if (propertyValue.Length == 0)
                        {
                            Scale = double.NaN;
                            return true;
                        }
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        Scale = temp;
                        return true;
                    }
            }
            return false;
        }

        public override bool GetValue(SymbolToken propertyToken, out string value)
        {
            if (base.GetValue(propertyToken, out value))
                return true;
            switch (propertyToken.Symbol)
            {
                case "Ha":
                    {
                        value = propertyToken.ToString(Hz);
                        return true;
                    }
                case "Hl":
                    {
                        value = propertyToken.ToString(HorizontalDistance);
                        return true;
                    }
                case "Za":
                    {
                        value = propertyToken.ToString(ZenitAngle);
                        return true;
                    }
                case "Su":
                    {
                        value = propertyToken.ToString(ElevationDefference);
                        return true;
                    }
                case "Hp":
                    {
                        value = propertyToken.ToString(Signal);
                        return true;
                    }
                case "W":
                    {
                        value = propertyToken.ToString(Scale);
                        return true;
                    }
            }
            value = string.Empty;
            return false;
        }




        public bool IsPointOfView
        {
            get { return PointType == PointTypes.PointOfView; }
        }

        readonly PropertyData _hzProperty = RegisterProperty("Hz", typeof(Double), double.NaN);
        public Double Hz
        {
            get { return GetValue<Double>(_hzProperty); }
            set { SetValue(_hzProperty, value); }
        }

        readonly PropertyData _horizontalDistanceProperty = RegisterProperty("HorizontalDistance", typeof(Double), double.NaN);
        public Double HorizontalDistance
        {
            get { return GetValue<Double>(_horizontalDistanceProperty); }
            set { SetValue(_horizontalDistanceProperty, value); }
        }

        readonly PropertyData _zenitAngleProperty = RegisterProperty("ZenitAngle", typeof(Double), double.NaN);
        public Double ZenitAngle
        {
            get { return GetValue<Double>(_zenitAngleProperty); }
            set { SetValue(_zenitAngleProperty, value); }
        }

        readonly PropertyData _elevationDifference = RegisterProperty("ElevationDefference", typeof(Double), double.NaN);
        public Double ElevationDefference
        {
            get { return GetValue<Double>(_elevationDifference); }
            set { SetValue(_elevationDifference, value); }
        }

        readonly PropertyData _signalProperty = RegisterProperty("Signal", typeof(Double), double.NaN);
        public Double Signal
        {
            get { return GetValue<Double>(_signalProperty); }
            set { SetValue(_signalProperty, value); }
        }

        readonly PropertyData _scaleProperty = RegisterProperty("Scale", typeof(Double), double.NaN);
        public Double Scale
        {
            get { return GetValue<Double>(_scaleProperty); }
            set { SetValue(_scaleProperty, value); }
        }

        readonly PropertyData _pointTypeProperty = RegisterProperty("PointType", typeof(PointTypes), PointTypes.MeasuringPoint);
        public PointTypes PointType
        {
            get { return GetValue<PointTypes>(_pointTypeProperty); }
            set
            {
                SetValue(_pointTypeProperty, value);
                OnPropertyChanged("FontWeight");
                OnPropertyChanged("FontColor");
                OnPropertyChanged("IsPointOfView");
            }
        }

        public Brush FontColor
        {
            get { return PointType == PointTypes.PointOfView ? Brushes.Red : Brushes.Black; }
        }

        public FontWeight FontWeight
        {
            get
            {
                if (PointType == PointTypes.Orientation || PointType == PointTypes.PointOfView)
                    return FontWeights.Bold;
                return FontWeights.Normal;
            }
        }

        void AssignPredcisliAndCislo(string value)
        {
            if (value.Length < 5)
            {
                Prefix = string.Empty;
                Number = value;
            }
            else
            {
                Prefix = value.Substring(0, value.Length - 4);
                Number = value.Substring(value.Length - 4);
            }
        }
    }

}
