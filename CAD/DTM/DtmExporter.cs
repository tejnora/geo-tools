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
        DateTime _now = DateTime.Now;

        public DtmExporter(IDtmMain main)
        {
            _main = main;
        }

        public void CreateFile(string location)
        {
            using (var writer = XmlWriter.Create(location))
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
                "objtyp", "cmn", "atr"
            };
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
                BeginElement("objtyp", elementGroup.Key);
                elementGroup.Value.ExportToDtm(this, DtmConfigurationSingleton.Instance.ElementSetting[elementGroup.Key]);
                EndElement();
            }
            EndElement();
        }

        public void AddAttribute(string name, string value)
        {
            _xmlWriter.WriteAttributeString(name, value);
        }

        public void AddPCData(string value)
        {
            _xmlWriter.WriteCData(value);
        }

        public void AddSpolecneAtributyVsechObjektu()
        {
            BeginElement("atr", "SpolecneAtributyVsechObjektu");
            AddElement(null, "DatumVkladu", _now);
            AddElement(null, "DatumZmeny", _now);
            EndElement();
        }

        public void AddSpolecneAtributyObjektuZPS()
        {
            BeginElement("atr", "SpolecneAtributyObjektuZPS");
            AddElement(null, "UrovenUmisteniObjektuZPS", "0");
            AddElement(null, "TridaPresnostiPoloha", "3");
            AddElement(null, "TridaPresnostiVyska", "3");
            AddElement(null, "ZpusobPorizeniZPS", "3");
            EndElement();
        }

        public void AddElement(string ns, string name, string text)
        {
            if (!string.IsNullOrEmpty(ns) && !_namespaces.Contains(ns))
                throw new Exception($"Namespace '{ns}' does not exist.");
            _xmlWriter.WriteStartElement(name, ns);
            _xmlWriter.WriteCData(text);
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
    }
}
