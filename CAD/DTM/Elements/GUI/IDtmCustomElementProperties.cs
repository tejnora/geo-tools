using System;
using System.Collections.Generic;

namespace CAD.DTM.Elements.GUI
{
    public interface IDtmCustomElementProperties
    {
        IList<IDtmCustomProperty> Properties { get; }
        void AddProperty(IDtmCustomProperty property);
    }
}
