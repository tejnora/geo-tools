namespace CAD.DTM.Configuration
{
    public enum ElementType
    {
        Point,
        Line
    }
    public class DtmElementOption
    {
        public float Width { get; set; }
        public System.Drawing.Color Color { get; set; }
        public ElementType ElementType { get; set; }
        public string CodeBase { get; set; }
        public string CodeSuffix { get; set; }
        public string XmlNamespace { get; set; }
        public string ObjektovyTypNazev { get; set; }
        public string KategorieObjektu { get; set; }
        public string SkupinaObjektu { get; set; }
        public string ObsahovaCast { get; set; }
    }
}
