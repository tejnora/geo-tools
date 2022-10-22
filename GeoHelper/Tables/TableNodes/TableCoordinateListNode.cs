using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using GeoBase.Utils;
using GeoCalculations.MethodPoints;
using GeoHelper.FileParses;

namespace GeoHelper.Tables.TableNodes
{
    [Serializable]
    public class TableCoordinateListNode : TableNodesBase
    {
        public TableCoordinateListNode()
        {
        }

        public TableCoordinateListNode(PointBase points)
        {
            X = points.X;
            Y = points.Y;
            Z = points.Z;
            Number = points.Number;
            Prefix = points.Prefix;
        }

        public TableCoordinateListNode(ISouradnice souradnice)
        {
            X = souradnice.X;
            Y = souradnice.Y;
            Z = souradnice.Z;
            NumberWithPrefix = souradnice.PointNumber;
        }

        public TableCoordinateListNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public void AssignNode(TableCoordinateListNode a)
        {
            base.AssignNode(a);
            Y = a.Y;
            X = a.X;
            Z = a.Z;
            Quality = a.Quality;
            Type = a.Type;
        }

        public override void Serialize(BinaryWriter w)
        {
            w.Write((UInt32)1); //version
            base.Serialize(w);
            w.Write(Y);
            w.Write(X);
            w.Write(Z);
            w.Write(Quality);
            w.Write(Type);
        }

        public override void Deserialize(BinaryReader r)
        {
            r.ReadUInt32();
            base.Deserialize(r);
            Y = r.ReadDouble();
            X = r.ReadDouble();
            Z = r.ReadDouble();
            Quality = r.ReadUInt32();
            Type = r.ReadString();
        }

        public override bool SetValue(SymbolToken propertyToken, string propertyValue)
        {
            if (base.SetValue(propertyToken, propertyValue))
                return true;
            switch (propertyToken.Symbol)
            {
                case "SobrX":
                    {
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        X = temp;
                        return true;
                    }
                case "SobrY":
                    {
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        Y = temp;
                        return true;
                    }

                case "SobrZ":
                    {
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        Z = temp;
                        return true;
                    }
                case "SobrPrec":
                    {
                        if (propertyValue == " ")
                        {
                            Quality = 0;
                            return true;
                        }
                        uint temp;
                        if (!UInt32.TryParse(propertyValue, out temp))
                            return false;
                        Quality = temp;
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
                case "SobrX":
                    {
                        value = propertyToken.ToString(X);
                        return true;
                    }
                case "SobrY":
                    {
                        value = propertyToken.ToString(Y);
                        return true;
                    }

                case "SobrZ":
                    {
                        value = propertyToken.ToString(Z);
                        return true;
                    }
                case "SobrPrec":
                    {
                        value = propertyToken.ToString(Quality);
                        return true;
                    }
            }
            value = string.Empty;
            return false;
        }

        readonly PropertyData _yProperty = RegisterProperty("Y", typeof(double), double.NaN);
        public double Y
        {
            get { return GetValue<double>(_yProperty); }
            set { SetValue(_yProperty, value); }
        }

        readonly PropertyData _xProperty = RegisterProperty("X", typeof(double), double.NaN);
        public double X
        {
            get { return GetValue<double>(_xProperty); }
            set { SetValue(_xProperty, value); }
        }

        readonly PropertyData _zProperty = RegisterProperty("Z", typeof(double), double.NaN);
        public double Z
        {
            get { return GetValue<double>(_zProperty); }
            set { SetValue(_zProperty, value); }
        }

        readonly PropertyData _qualityProperty = RegisterProperty("Quality", typeof(uint), double.NaN);
        public uint Quality
        {
            get { return GetValue<uint>(_qualityProperty); }
            set { SetValue(_qualityProperty, value); }
        }

        public readonly PropertyData _typeProperty = RegisterProperty("Type", typeof(string), string.Empty);
        public string Type
        {
            get { return GetValue<string>(_typeProperty); }
            set { SetValue(_typeProperty, value); }
        }

        public Brush FontColor
        {
            get { return Brushes.Black; }
        }

        public FontWeight FontWeight
        {
            get { return FontWeights.Normal; }
        }
    }
}
