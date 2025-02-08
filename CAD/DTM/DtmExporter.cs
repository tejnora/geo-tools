using System;
using System.Collections.Generic;
using CAD.DTM.Gui;
using System.Xml;
using CAD.DTM.Configuration;

namespace CAD.DTM
{
    class DtmExporter
    : IDtmExporter
    {
        readonly IDtmMain _main;
        XmlWriter _xmlWriter;
        HashSet<string> _namespaces;
        DtmExportCtx _exportCtx;
        public DtmExporter(IDtmMain main)
        {
            _main = main;
        }
        public void CreateFile(DtmExportCtx ctx)
        {
            _exportCtx = ctx;
            using (var writer = XmlWriter.Create(ctx.FileName, new XmlWriterSettings { Indent = true }))
            {
                try
                {
                    _xmlWriter = writer;
                    writer.WriteStartDocument();
                    BeginElement(null, "JVFDTM");
                    AddNamespaces();
                    BeginElement("objtyp", "DataJVFDTM");
                    AddHead();
                    BeginElement("objtyp", "Data");
                    AddData();
                    EndElement();
                    ExportDoprovodneInformace();
                    EndElement();
                    EndElement();
                    writer.WriteEndDocument();
                }
                finally
                {
                    _xmlWriter = null;
                }
            }
        }
        void AddNamespaces()
        {
            _namespaces = new HashSet<string>
            {
                "objtyp", "cmn", "atr", "dopinf"
            };
            foreach (var group in _main.GetElementGroups())
            {
                if (!group.Value.HasSameElementsForExport())
                    continue;
                _namespaces.Add(DtmConfigurationSingleton.Instance.ElementSetting[group.Key].XmlNamespace);
            }
            _xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            _xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            _xmlWriter.WriteAttributeString("xmlns", "gml", null, "http://www.opengis.net/gml/3.2");
            foreach (var ns in _namespaces)
            {
                _xmlWriter.WriteAttributeString("xmlns", ns, null, ns);
            }

            _namespaces.Add("gml");
        }
        void AddHead()
        {
            AddElement("cmn", "VerzeJVFDTM", "1.4.3");
            AddElement("cmn", "DatumZapisu", DateTime.Now);
            AddElement("cmn", "TypZapisu", "změnové věty");
        }
        void AddData()
        {
            BeginElement("objtyp", "Data");
            foreach (var elementGroup in _main.GetElementGroups())
            {
                elementGroup.Value.ExportToDtm(this);
            }
            EndElement();
        }
        public void AddAttribute(string name, string value)
        {
            _xmlWriter.WriteAttributeString(name, value);
        }
        public void AddAttribute(string name, int value)
        {
            _xmlWriter.WriteAttributeString(name, value.ToString());
        }
        public void AddStringData(string value)
        {
            _xmlWriter.WriteString(value);
        }
        public void AddElement(string ns, string name, string text)
        {
            if (!string.IsNullOrEmpty(ns) && !_namespaces.Contains(ns))
                throw new Exception($"Namespace '{ns}' does not exist.");
            _xmlWriter.WriteStartElement(name, ns);
            _xmlWriter.WriteString(text);
            _xmlWriter.WriteEndElement();
        }
        public void AddElement(string ns, string name, int value)
        {
            AddElement(ns, name, value.ToString());
        }
        public void AddElement(string ns, string name, DateTime value)
        {
            AddElement(ns, name, value.ToString("ddd"));
        }
        public void AddElement(string ns, string name, double value)
        {
            AddElement(ns, name, value.ToString("##.00"));
        }
        public void AddElement(string ns, string name, bool value)
        {
            AddElement(ns, name, value ? "true" : "false");
        }
        public void BeginElement(string ns, string name)
        {
            if (!string.IsNullOrEmpty(ns) && !_namespaces.Contains(ns))
                throw new Exception($"Namespace '{ns}' does not exist.");
            _xmlWriter.WriteStartElement(name, ns);
        }
        public void EndElement()
        {
            _xmlWriter.WriteEndElement();
        }

        void ExportDoprovodneInformace()
        {
            BeginElement("dopinf", "DoprovodneInformace");
            BeginElement(null, "UdajeOZmenach");
            BeginElement(null, "ZaznamZmeny");
            ExportZaznamZmeny();
            EndElement();
            EndElement();
            EndElement();
        }

        void ExportZaznamZmeny()
        {
            AddEmptyElement("atr", "IDPodani");
            AddEmptyElement("atr", "PopisObjektu");
            AddEmptyElement("atr", "IDEditora");
            AddEmptyElement("atr", "DatumVkladu");
            AddEmptyElement("atr", "VkladOsoba");
            AddElement(null, "NazevZakazky", _exportCtx.NazevZakazky);
            AddEmptyElement(null, "CisloStavbyZakazky");
            AddEmptyElement(null, "PartnerInvestor");
            AddElement(null, "Zpracovatel", _exportCtx.Zpracovatel);
            AddElement(null, "OrganizaceZpracovatele",_exportCtx.OrganizaceZpracovatele);
            AddElement(null, "DatumMereni", _exportCtx.DatumMereni.ToString("yyyy-MM-dd"));
            AddElement(null, "DatumZpracovani", _exportCtx.DatumZpracovani.ToString("yyyy-MM-dd"));
            AddElement(null, "AZI",_exportCtx.AZI);
            AddElement(null, "DatumOvereni",_exportCtx.DatumOvereni.ToString("yyyy-MM-dd"));
            AddElement(null, "CisloOvereni",_exportCtx.CisloOvereni);

        }

        void AddEmptyElement(string ns, string name)
        {
            AddElement(ns, name, null);
        }
    }
}
