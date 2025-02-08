using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
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

        public void AddElementIfNotExist(IDtmElement dtmElementGetDtmElement, IDtmMain dtmMain)
        {
            if (_elements.Contains(dtmElementGetDtmElement))
                return;
            Debug.Assert(string.IsNullOrEmpty(dtmElementGetDtmElement.ID));
            dtmElementGetDtmElement.ID = dtmMain.AllocateUniqueId(Name);
            _elements.Add(dtmElementGetDtmElement);
        }

        public bool HasSameElementsForExport()
        {
            return _elements.Any(n => n.ExportToOutput);
        }

        public void ExportToDtm(IDtmExporter exporter)
        {
            var ns = DtmConfigurationSingleton.Instance.ElementSetting[Name].XmlNamespace;
            exporter.BeginElement("objtyp", Name);
            ExportToDtmGroupData(exporter, ns);
            exporter.BeginElement(ns, "ZaznamyObjektu");
            foreach (var element in _elements.Where(element => element.ExportToOutput))
            {
                exporter.BeginElement(ns, "ZaznamObjektu");
                exporter.AddElement("cmn", "ZapisObjektu", element.EvaluateZapisObjektuForExportToDtm());
                exporter.BeginElement(null, "AtributyObjektu");
                element.ExportSpolecneAtributyVsechObjektu(exporter);
                element.ExportAttributesToDtm(exporter);
                exporter.EndElement();
                element.Geometry.ExportToDtm(exporter);
                exporter.EndElement();
            }
            exporter.EndElement();
            exporter.EndElement();
        }
        void ExportToDtmGroupData(IDtmExporter dtmExporter, string ns)
        {
            dtmExporter.BeginElement(ns, "ObjektovyTypNazev");
            dtmExporter.AddAttribute("code_base", CodeBase);
            dtmExporter.AddAttribute("code_suffix", CodeSuffix);
            dtmExporter.AddStringData(ObjektovyTypNazev);
            dtmExporter.EndElement();
            dtmExporter.AddElement(ns, "KategorieObjektu", KategorieObjektu);
            dtmExporter.AddElement(ns, "SkupinaObjektu", SkupinaObjektu);
            dtmExporter.AddElement(ns, "ObsahovaCast", ObsahovaCast);
        }

    }
}
