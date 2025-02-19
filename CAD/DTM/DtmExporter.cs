using System;
using System.Collections.Generic;
using System.Text;
using CAD.DTM.Gui;
using System.Xml;
using CAD.DTM.Configuration;

namespace CAD.DTM
{
    class DtmExporter
    : IDtmExporter
    {
        readonly IDtmMain _main;
        CAD.Tools.XmlWriter _xmlWriter;
        HashSet<string> _namespaces;
        DtmExportCtx _exportCtx;
        public DtmExporter(IDtmMain main)
        {
            _main = main;
        }
        public void CreateFile(DtmExportCtx ctx)
        {
            _exportCtx = ctx;

            try
            {
                _xmlWriter = new Tools.XmlWriter();
                _xmlWriter.WriteStartDocument();
                _xmlWriter.WriteStartElement("JVFDTM", "");
                _xmlWriter.WriteAttributeString("xmlns", "objtyp");
                AddNamespaces();
                BeginElement("objtyp", "DataJVFDTM");
                AddHead();
                AddData();
                ExportDoprovodneInformace();
                EndElement();
                EndElement();
                _xmlWriter.WriteEndDocument();
                _xmlWriter.WriteTo(ctx.FileName);
            }
            finally
            {
                _xmlWriter = null;
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
            _xmlWriter.WriteAttributeString("xsd", "xmlns", "http://www.w3.org/2001/XMLSchema");
            _xmlWriter.WriteAttributeString("xsi", "xmlns", "http://www.w3.org/2001/XMLSchema-instance");
            _xmlWriter.WriteAttributeString("gml", "xmlns", "http://www.opengis.net/gml/3.2");
            foreach (var ns in _namespaces)
            {
                _xmlWriter.WriteAttributeString(ns, "xmlns", ns);
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

        public void AddAttribute(string ns, string name, string value)
        {
            _xmlWriter.WriteAttributeString(name, ns, value);
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
            BeginElement(ns, name);
            _xmlWriter.WriteString(text);
            EndElement();
        }
        public void AddElement(string ns, string name, int value)
        {
            AddElement(ns, name, value.ToString());
        }
        public void AddElement(string ns, string name, DateTime value)
        {
            AddElement(ns, name, value.ToString("O"));
        }
        public void AddElement(string ns, string name, double value)
        {
            AddElement(ns, name, value.ToString("##.00"));
        }
        public void AddElement(string ns, string name, bool value)
        {
            AddElement(ns, name, value ? "true" : "false");
        }
        public void BeginElement(string ns, string name, bool addNsToAttribute = false)
        {
            if (!string.IsNullOrEmpty(ns) && !_namespaces.Contains(ns))
                throw new Exception($"Namespace '{ns}' does not exist.");
            _xmlWriter.WriteStartElement(name, ns);
            if (addNsToAttribute)
                _xmlWriter.WriteAttributeString("xmlns", ns);
        }
        public void EndElement()
        {
            _xmlWriter.WriteEndElement();
        }

        void ExportDoprovodneInformace()
        {
            BeginElement("dopinf", "DoprovodneInformace", true);
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
            AddElement("atr", "DatumVkladu", DateTime.Now);
            AddEmptyElement("atr", "VkladOsoba");
            AddElement(null, "NazevZakazky", _exportCtx.NazevZakazky);
            AddEmptyElement(null, "CisloStavbyZakazky");
            AddEmptyElement(null, "PartnerInvestor");
            AddElement(null, "Zpracovatel", _exportCtx.Zpracovatel);
            AddElement(null, "OrganizaceZpracovatele", _exportCtx.OrganizaceZpracovatele);
            AddElement(null, "DatumMereni", _exportCtx.DatumMereni.ToString("yyyy-MM-dd"));
            AddElement(null, "DatumZpracovani", _exportCtx.DatumZpracovani.ToString("yyyy-MM-dd"));
            AddElement(null, "AZI", _exportCtx.AZI);
            AddElement(null, "DatumOvereni", _exportCtx.DatumOvereni.ToString("yyyy-MM-dd"));
            AddElement(null, "CisloOvereni", _exportCtx.CisloOvereni);
            if (_main.UdajeOVydeji != null)
            {
                var polygon = _main.UdajeOVydeji.Polygon;
                BeginElement(null, "OblastZmeny");
                BeginElement("gml", "surfaceProperty", false);
                BeginElement("gml", "Polygon");
                AddAttribute("gml", "id", _main.AllocateUniqueId("03"));
                AddAttribute("srsName", polygon.SrsName);
                AddAttribute("srsDimension", polygon.SrsDimension);
                BeginElement("gml", "exterior");
                BeginElement("gml", "LinearRing");
                var posListData = new StringBuilder();
                foreach (var p in polygon.Points)
                {
                    posListData.Append(p.ExportToDtm(2) + " ");
                }
                AddElement("gml", "posList", posListData.ToString(0, posListData.Length - 1));
                EndElement();
                EndElement();
                EndElement();
                EndElement();
                EndElement();
            }
            AddElement(null, "Konsolidace", false);
        }
        void AddEmptyElement(string ns, string name)
        {
            AddElement(ns, name, null);
        }
    }
}
