using System;
using System.Globalization;
using System.IO;
using System.Xml;
using CAD.DTM.Gui;

namespace CAD.DTM
{
    internal sealed class DtmXmlFragmentWriter : IDtmExporter, IDisposable
    {
        readonly StringWriter _output = new(CultureInfo.InvariantCulture);
        readonly XmlWriter _writer;
        public DtmXmlFragmentWriter() => _writer = XmlWriter.Create(_output, new XmlWriterSettings { OmitXmlDeclaration = true });
        static string Namespace(string value) => value switch
        {
            "gml" => "http://www.opengis.net/gml/3.2",
            "xlink" => "http://www.w3.org/1999/xlink",
            "xsi" => "http://www.w3.org/2001/XMLSchema-instance",
            "xml" => "http://www.w3.org/XML/1998/namespace",
            _ => value
        };
        public void BeginElement(string ns, string name, bool addNsToAttribute = false) => _writer.WriteStartElement(ns, name, Namespace(ns));
        public void EndElement() => _writer.WriteEndElement();
        public void AddElement(string ns, string name, string text) { BeginElement(ns, name); AddStringData(text); EndElement(); }
        public void AddElement(string ns, string name, int value) => AddElement(ns, name, XmlConvert.ToString(value));
        public void AddElement(string ns, string name, double value) => AddElement(ns, name, XmlConvert.ToString(value));
        public void AddElement(string ns, string name, bool value) => AddElement(ns, name, XmlConvert.ToString(value));
        public void AddElement(string ns, string name, DateTime value) => AddElement(ns, name, XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind));
        public void AddAttribute(string ns, string name, string value) => _writer.WriteAttributeString(ns, name, Namespace(ns), value);
        public void AddAttribute(string name, string value) => _writer.WriteAttributeString(name, value);
        public void AddAttribute(string name, int value) => AddAttribute(name, XmlConvert.ToString(value));
        public void AddStringData(string value) => _writer.WriteString(value ?? "");
        public void MarkPoint(DtmPoint point) { }
        public override string ToString() { _writer.Flush(); return _output.ToString(); }
        public void Dispose() { _writer.Dispose(); _output.Dispose(); }
    }
}
