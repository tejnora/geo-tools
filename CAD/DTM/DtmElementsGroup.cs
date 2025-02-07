using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CAD.DTM.Configuration;
using CAD.DTM.Elements;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    public class DtmElementsGroup
    : IDtmElementsGroup
    {
        public DtmElementsGroup(string name)
        {
            Name = name;
        }
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
        public string Name { get; }

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

        public bool HasSameElementsToExport()
        {
            return _elements.Any(n => n.ExportToOutput);
        }

        public void ExportToDtm(IDtmExporter exporter, DtmElementOption option)
        {
            ExportToDtmGroupData(exporter, option);
            exporter.BeginElement(option.XmlNamespace, "ZaznamyObjektu");
            foreach (var element in _elements)
            {
                if (!element.ExportToOutput)
                    continue;
                exporter.BeginElement(option.XmlNamespace, "ZaznamObjektu");
                element.ExportToDtm(exporter);
                exporter.EndElement();
            }
            exporter.EndElement();
        }
        static void ExportToDtmGroupData(IDtmExporter dtmExporter, DtmElementOption option)
        {
            var ns = option.XmlNamespace;
            dtmExporter.BeginElement(ns, "ObjektovyTypNazev");
            dtmExporter.AddAttribute("code_base", option.CodeBase);
            dtmExporter.AddAttribute("code_suffix", option.CodeSuffix);
            dtmExporter.AddPCData(option.ObjektovyTypNazev);
            dtmExporter.EndElement();
            dtmExporter.AddElement(ns, "KategorieObjektu", option.KategorieObjektu);
            dtmExporter.AddElement(ns, "SkupinaObjektu", option.SkupinaObjektu);
            dtmExporter.AddElement(ns, "ObsahovaCast", option.ObsahovaCast);
        }

    }
}
