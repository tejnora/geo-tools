using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoHelper.FileParses;

namespace GeoHelper.Tables.TableNodes
{
    [Serializable]
    public class TableDoubleCoordinateListNode : TableCoordinateListNode
    {
        public TableDoubleCoordinateListNode()
        {
            Type = string.Empty;
        }

        public TableDoubleCoordinateListNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public void AssignNode(TableDoubleCoordinateListNode a)
        {
            base.AssignNode(a);
            SpolX = a.SpolX;
            SpolY = a.SpolY;
            SpolQuality = a.SpolQuality;
        }

        public override void Serialize(BinaryWriter w)
        {
            w.Write(0);
            base.Serialize(w);
            w.Write(SpolX);
            w.Write(SpolY);
            w.Write(SpolQuality);
        }

        public override void Deserialize(BinaryReader r)
        {
            r.ReadInt32();
            base.Deserialize(r);
            SpolX = r.ReadDouble();
            SpolY = r.ReadDouble();
            SpolQuality = r.ReadUInt32();
        }

        public override bool SetValue(SymbolToken propertyToken, string propertyValue)
        {
            if (base.SetValue(propertyToken, propertyValue))
                return true;
            switch (propertyToken.Symbol)
            {
                case "SpolX":
                    {
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        SpolX = temp;
                        return true;
                    }
                case "SpolY":
                    {
                        double temp;
                        if (!double.TryParse(propertyValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            return false;
                        SpolY = temp;
                        return true;
                    }
                case "SpolPrec":
                    {
                        uint temp;
                        if (!UInt32.TryParse(propertyValue, out temp))
                            return false;
                        SpolQuality = temp;
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
                case "SpolX":
                    {
                        value = propertyToken.ToString(SpolX);
                        return true;
                    }
                case "SpolY":
                    {
                        value = propertyToken.ToString(SpolY);
                        return true;
                    }
                case "SpolPrec":
                    {
                        value = propertyToken.ToString(SpolQuality);
                        return true;
                    }
            }
            value = string.Empty;
            return false;
        }


        readonly PropertyData _spolXProperty = RegisterProperty("SpolX", typeof(Double), double.NaN);
        public Double SpolX
        {
            get { return GetValue<Double>(_spolXProperty); }
            set { SetValue(_spolXProperty, value); }
        }

        readonly PropertyData _spolYProperty = RegisterProperty("SpolY", typeof(Double), double.NaN);
        public Double SpolY
        {
            get { return GetValue<Double>(_spolYProperty); }
            set { SetValue(_spolYProperty, value); }
        }

        readonly PropertyData _spolQualityProperty = RegisterProperty("SpolQuality", typeof(uint), 0);
        public uint SpolQuality
        {
            get { return GetValue<uint>(_spolQualityProperty); }
            set { SetValue(_spolQualityProperty, value); }
        }

        public UInt32 SpolecnaKvalita //for dialog
        {
            get
            {
                return Quality != 0 ? Quality : SpolQuality;
            }
            set
            {
                if (value <= 3)
                {
                    SpolQuality = value;
                    Quality = 0;
                    return;
                }
                Quality = value;
                SpolQuality = 0;
            }
        }
    }

}
