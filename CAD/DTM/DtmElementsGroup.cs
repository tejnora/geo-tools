using CAD.DTM.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CAD.DTM
{
    public class DtmElementsGroup
    : IDtmElementsGroup
    {
        List<IDtmElement> _elements = new List<IDtmElement>();
        public void AddElement(IDtmElement element)
        {
            _elements.Add(element);
        }

        public void Save()
        {
            var output = new StringBuilder();
            foreach (var element in _elements)
            {
                var ddd = element as DtmElement;
                var geometryPoint = ddd.Geometry as DtmPointGeometry;
                if (string.IsNullOrEmpty(ddd.CisloBodu))
                    continue;
                output.Append($"{ddd.CisloBodu} {geometryPoint.Point.Y:##.00} {geometryPoint.Point.X:##.00} {geometryPoint.Point.Z:##.00}\r\n");
            }
            File.WriteAllText(@"c:\Temp\JVF DTM\PodrobnyBodZPS.txt", output.ToString());
        }

        public string ObjektovyTypNazev { get; set; }
        public string CodeBase { get; set; }
        public string CodeSuffix { get; set; }
        public string KategorieObjektu { get; set; }
        public string SkupinaObjektu { get; set; }
        public string ObsahovaCast { get; set; }
        public IEnumerable<IDtmElement> GetElementGroups()
        {
            return _elements;
        }

        public void AddElementIfNotExist(IDtmElement dtmElementGetDtmElement)
        {
            if (_elements.Contains(dtmElementGetDtmElement))
                return;
            _elements.Add(dtmElementGetDtmElement);
        }
    }
}
