using System;

namespace GeoCalculations.Protocol
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProtocolMethodAttribute : Attribute
    {
        public ProtocolMethodAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }

    }
}
