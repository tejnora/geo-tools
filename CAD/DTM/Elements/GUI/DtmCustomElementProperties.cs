using System.Collections.Generic;
namespace CAD.DTM.Elements.GUI
{
    public class DtmCustomElementProperties
    : IDtmCustomElementProperties
    {
        List<IDtmCustomProperty> _properties = new List<IDtmCustomProperty>();
        public DtmCustomElementProperties()
        {
        }
        public IList<IDtmCustomProperty> Properties => _properties;
        public void AddProperty(IDtmCustomProperty property)
        {
            _properties.Add(property);
        }

        public void Clear()
        {
            _properties.Clear();
        }
    }
}
