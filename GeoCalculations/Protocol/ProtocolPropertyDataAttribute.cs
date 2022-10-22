using System;

namespace GeoCalculations.Protocol
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ProtocolPropertyDataAttribute : Attribute
    {
        public ProtocolPropertyDataAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }

    }
}
