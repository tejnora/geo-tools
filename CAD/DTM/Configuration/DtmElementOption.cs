using System;
using System.Collections.Generic;

namespace CAD.DTM.Configuration
{
    public class DtmElementOption
    {
        public string Name { get; set; }
        public string GraphicSource { get; set; }
        public string GraphicAttribute { get; set; }
        readonly Dictionary<string, IList<DtmGraphicElement>> _graphicElements = new Dictionary<string, IList<DtmGraphicElement>>();

        public DtmElementOption()
        {
        }
        public void RegisterGraphicElementByType(string @type, IList<DtmGraphicElement> graphicElements)
        {
            _graphicElements.Add(@type, graphicElements);
        }

        public DtmGraphicElement GetGraphicElement(string @type, DtmGraphicElementScaleEnum scale)
        {
            if (!_graphicElements.TryGetValue(@type, out var graphicElements))
                _graphicElements.TryGetValue("_", out graphicElements);
            if (graphicElements != null && graphicElements.Count > 0)
            {
                return graphicElements[(int)scale >= 0 && (int)scale < graphicElements.Count ? (int)scale : 0];
            }
            return null;
        }
        public float Width { get; set; }
        public System.Drawing.Color Color { get; set; }
        public DtmElementType ElementType { get; set; }
        public string CodeBase { get; set; }
        public string CodeSuffix { get; set; }
        public string XmlNamespace { get; set; }
        public string ObjektovyTypNazev { get; set; }
        public string KategorieObjektu { get; set; }
        public string SkupinaObjektu { get; set; }
        public string ObsahovaCast { get; set; }
        public Type ClassType { get; set; }
    }
}
