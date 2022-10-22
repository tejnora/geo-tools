using System;

namespace GeoCalculations.Protocol
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ProtocolPropertyValueTypeAttribute : Attribute
    {
        public enum Types
        {
            Unknow,
            Heigth,
            Text,
            Cordinate,
            Distance,
            Scale,
            Angle,
            Area
        }

        public Types Type { get; private set; }

        public ProtocolPropertyValueTypeAttribute(Types type)
        {
            Type = type;
        }
    }
}
