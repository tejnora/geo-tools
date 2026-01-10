namespace CAD.DTM.Elements.GUI
{
    public class DtmReadonlyCustomProperty
    : IDtmCustomProperty
    {
        public DtmReadonlyCustomProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public string Value { get; }
    }
}
