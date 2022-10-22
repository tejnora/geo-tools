using System;

namespace GeoCalculations.Protocol
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtocolRootDataAttribute : Attribute
    {
        public ProtocolRootDataAttribute(string templateName)
        {
            TemplateName = templateName;
        }
        public string TemplateName { get; private set; }
    }
}
