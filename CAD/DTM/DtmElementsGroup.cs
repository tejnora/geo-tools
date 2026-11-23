using System.Collections.Generic;
using System.Diagnostics;
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
            (element as DtmElement)?.CaptureValidationOriginal();
            _elements.Add(element);
        }

        public void Save()
        {
            var output = new StringBuilder();
            foreach (var element in _elements)
            {
                var ddd = element as DtmBodBaseElement;
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
            foreach (var geometry in dtmElementGetDtmElement.Geometry.Geometries)
            {
                Debug.Assert(string.IsNullOrEmpty(geometry.Id));
                geometry.Id = dtmMain.AllocateUniqueId(Name);
            }
            (dtmElementGetDtmElement as DtmElement)?.CaptureValidationOriginal();
            _elements.Add(dtmElementGetDtmElement);
        }

        public bool HasSameElementsForExport()
        {
            return _elements.Any(n => n.ExportToOutput);
        }

        public void ExportToDtm(IDtmExporter exporter)
        {
            if (!HasSameElementsForExport())
                return;
            var ns = DtmConfigurationSingleton.Instance.ElementSetting[Name].XmlNamespace;
            exporter.BeginElement("objtyp", DtmSchema.Instance.Objects[Name].ExchangeName);
            ExportToDtmGroupData(exporter, ns);
            exporter.BeginElement(ns, "ZaznamyObjektu");
            foreach (var element in _elements.Where(element => element.ExportToOutput))
            {
                if (element is not DtmElement record || record.SchemaAttributes == null)
                    throw new System.InvalidOperationException(Name + ": objekt nemá definici XSD, nelze jej exportovat.");
                // JVF DTM 1.5: the kind of change is the record element itself (ZaznamObjektuIns/Upd/Del).
                var context = record.EvaluateExportContext();
                var layout = record.SchemaAttributes.Definition.Contexts[context];
                exporter.BeginElement(ns, "ZaznamObjektu" + context, true);
                exporter.BeginElement(ns, "AtributyObjektu");
                record.SchemaAttributes.Export(exporter, context);
                exporter.EndElement();
                if (layout.GeometryType != null && (layout.GeometryRequired || element.Geometry?.Geometries.Count > 0))
                {
                    exporter.BeginElement(ns, "GeometrieObjektu");
                    DtmGeometryXml.Write(exporter, layout.GeometryType, element.Geometry.Geometries);
                    exporter.EndElement();
                }
                foreach (var field in record.SchemaAttributes.RecordXml(context)) DtmSchemaAttributes.WriteElement(exporter, field);
                exporter.EndElement();
            }
            exporter.EndElement();
            exporter.EndElement();
        }
        void ExportToDtmGroupData(IDtmExporter dtmExporter, string ns)
        {
            dtmExporter.BeginElement(ns, "ObjektovyTypNazev");
            if (CodeBase != null) dtmExporter.AddAttribute("code_base", CodeBase);
            if (CodeSuffix != null) dtmExporter.AddAttribute("code_suffix", CodeSuffix);
            dtmExporter.AddStringData(ObjektovyTypNazev);
            dtmExporter.EndElement();
            if (KategorieObjektu != null) dtmExporter.AddElement(ns, "KategorieObjektu", KategorieObjektu);
            if (SkupinaObjektu != null) dtmExporter.AddElement(ns, "SkupinaObjektu", SkupinaObjektu);
            if (ObsahovaCast != null) dtmExporter.AddElement(ns, "ObsahovaCast", ObsahovaCast);
        }

    }
}
