using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using CAD.DTM.Configuration;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    /// <summary>Attribute values with explicit presence, shared by XML, typed models and the editor.</summary>
    public sealed class DtmSchemaAttributes
    {
        readonly DtmElement _owner;
        readonly Dictionary<string, string> _values = new();
        readonly Dictionary<string, string> _legacySnapshot = new();
        static readonly Lazy<XmlSchemaSet> AttributeSchemas = new(() =>
        {
            var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
            schemas.Add("atr", System.IO.Path.Combine(DtmSchema.XsdDirectory, "common", "atributy.xsd"));
            schemas.Compile();
            return schemas;
        });

        public DtmObjectDefinition Definition { get; }
        internal DtmElement Owner => _owner;
        public IReadOnlyDictionary<string, string> Values => _values;

        public DtmSchemaAttributes(DtmElement owner, DtmObjectDefinition definition)
        {
            _owner = owner;
            Definition = definition;
        }

        public T Get<T>(string path)
        {
            if (!_values.TryGetValue(path, out var value)) return default;
            var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (type == typeof(string)) return (T)(object)value;
            if (type.IsEnum) return (T)Enum.ToObject(type, long.Parse(value, CultureInfo.InvariantCulture));
            if (type == typeof(bool)) return (T)(object)XmlConvert.ToBoolean(value);
            if (type == typeof(DateTime)) return (T)(object)XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        public void Set<T>(string path, T value)
        {
            var name = path.Split('/').Last();
            var text = value == null ? null : Format(value);
            if (value is DateTime date && DtmSchema.Instance.Attributes.TryGetValue(name, out var definition) && definition.Type == "date")
                text = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            SetText(path, text);
        }

        public void SetText(string path, string value)
        {
            SynchronizeLegacyValues();
            if (value == null) _values.Remove(path);
            else _values[path] = value;
            // Existing typed properties remain available to topology rules and symbol selectors.
            var accessor = FindLegacyMember(path);
            if (accessor != null)
            {
                try { accessor.Value.Set(value); }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is ArgumentException) { }
            }
            CaptureLegacyValues();
        }

        public void Import(XmlElement attributes)
        {
            _values.Clear();
            foreach (XmlElement child in attributes.ChildNodes.OfType<XmlElement>())
                Read(child, child.LocalName);
        }

        public void ApplyToLegacyValues()
        {
            foreach (var pair in _values)
            {
                var accessor = FindLegacyMember(pair.Key);
                if (accessor == null) continue;
                try { accessor.Value.Set(pair.Value); }
                catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException) { }
            }
            CaptureLegacyValues();
        }

        public void ImportRecordAttribute(XmlElement element) => Read(element, element.LocalName);

        void Read(XmlElement element, string path)
        {
            if (element.ChildNodes.OfType<XmlElement>().Any())
            {
                if (element.ChildNodes.OfType<XmlElement>().Any(e => e.NamespaceURI == "http://www.opengis.net/gml/3.2"))
                    _values[path] = element.OuterXml;
                else
                    foreach (XmlElement child in element.ChildNodes.OfType<XmlElement>()) Read(child, path + "/" + child.LocalName);
            }
            else _values[path] = element.InnerText;
        }

        public IEnumerable<(string Path, DtmAttributeDefinition Attribute, bool Required)> Fields()
        {
            foreach (var reference in Definition.Attributes.Concat(Definition.RecordAttributes))
                foreach (var field in Fields(reference, reference.Name, true)) yield return field;
        }

        IEnumerable<(string, DtmAttributeDefinition, bool)> Fields(DtmAttributeReference reference, string path, bool parentRequired)
        {
            if (!DtmSchema.Instance.Attributes.TryGetValue(reference.Name, out var attribute)) yield break;
            if (attribute.Type != "complex" || attribute.Children.Any(c => c.Namespace == "gml"))
                yield return (path, attribute, parentRequired && reference.Required);
            else
                foreach (var child in attribute.Children)
                    foreach (var field in Fields(child, path + "/" + child.Name, parentRequired && reference.Required)) yield return field;
        }

        public void CaptureLegacyValues()
        {
            foreach (var field in Fields())
            {
                var accessor = FindLegacyMember(field.Path);
                if (accessor != null) _legacySnapshot[field.Path] = accessor.Value.Get();
            }
        }

        public void SynchronizeLegacyValues()
        {
            foreach (var field in Fields())
            {
                var accessor = FindLegacyMember(field.Path);
                if (accessor == null) continue;
                var current = accessor.Value.Get();
                if (_legacySnapshot.TryGetValue(field.Path, out var previous) && previous != current)
                {
                    if (current == null) _values.Remove(field.Path);
                    else _values[field.Path] = current;
                }
            }
            CaptureLegacyValues();
        }

        public IReadOnlyList<string> Validate()
        {
            SynchronizeLegacyValues();
            var errors = new List<string>();
            foreach (var field in Fields())
                if (field.Required && !_values.ContainsKey(field.Path)) errors.Add(field.Path + ": povinný údaj není vyplněn.");
            foreach (var reference in Definition.Attributes.Concat(Definition.RecordAttributes))
            {
                try
                {
                    var node = Build(reference, reference.Name);
                    if (node == null) continue;
                    new XDocument(node).Validate(AttributeSchemas.Value, (_, e) => errors.Add(reference.Name + ": " + e.Message));
                }
                catch (XmlException ex) { errors.Add(reference.Name + ": " + ex.Message); }
            }
            return errors;
        }

        public static string ValidateValue(DtmAttributeDefinition definition, string text)
        {
            string error = null;
            try
            {
                var node = definition.Type == "complex" ? XElement.Parse(text) : new XElement(XName.Get(definition.Name, "atr"), text);
                if (node.Name != XName.Get(definition.Name, "atr")) return "Očekáván element atr:" + definition.Name;
                new XDocument(node).Validate(AttributeSchemas.Value, (_, e) => error ??= e.Message);
            }
            catch (XmlException ex) { error = ex.Message; }
            return error;
        }

        public IEnumerable<XElement> ToXml(bool includeCommon = true)
        {
            SynchronizeLegacyValues();
            foreach (var reference in Definition.Attributes)
            {
                if (!includeCommon && reference.Name == "SpolecneAtributyVsechObjektu") continue;
                var element = Build(reference, reference.Name);
                if (element != null) yield return element;
            }
        }

        public IEnumerable<XElement> RecordXml()
        {
            foreach (var reference in Definition.RecordAttributes)
            {
                var element = Build(reference, reference.Name);
                if (element != null) yield return element;
            }
        }

        XElement Build(DtmAttributeReference reference, string path)
        {
            var present = _values.ContainsKey(path) || _values.Keys.Any(p => p.StartsWith(path + "/", StringComparison.Ordinal));
            if (!reference.Required && !present) return null;
            var attribute = DtmSchema.Instance.Attributes[reference.Name];
            var element = new XElement(XName.Get(reference.Name, "atr"));
            if (attribute.Type == "complex")
            {
                if (attribute.Children.Any(c => c.Namespace == "gml"))
                {
                    if (_values.TryGetValue(path, out var raw))
                    {
                        var parsed = XElement.Parse(raw);
                        if (parsed.Name != element.Name) throw new XmlException("Očekáván element " + element.Name);
                        return parsed;
                    }
                    return element;
                }
                foreach (var child in attribute.Children)
                {
                    var nested = Build(child, path + "/" + child.Name);
                    if (nested != null) element.Add(nested);
                }
            }
            else if (_values.TryGetValue(path, out var text)) element.Value = text;
            return element;
        }

        public void Export(IDtmExporter exporter, bool includeCommon = true)
        {
            foreach (var element in ToXml(includeCommon)) WriteElement(exporter, element);
        }

        internal static void WriteElement(IDtmExporter exporter, XElement element)
        {
            var ns = element.Name.NamespaceName == "http://www.opengis.net/gml/3.2" ? "gml" : element.Name.NamespaceName;
            exporter.BeginElement(ns, element.Name.LocalName);
            foreach (var attribute in element.Attributes().Where(a => !a.IsNamespaceDeclaration))
            {
                if (attribute.Name.NamespaceName.Length == 0) exporter.AddAttribute(attribute.Name.LocalName, attribute.Value);
                else exporter.AddAttribute(attribute.Name.NamespaceName switch
                {
                    "http://www.opengis.net/gml/3.2" => "gml",
                    "http://www.w3.org/1999/xlink" => "xlink",
                    "http://www.w3.org/2001/XMLSchema-instance" => "xsi",
                    "http://www.w3.org/XML/1998/namespace" => "xml",
                    _ => attribute.Name.NamespaceName
                }, attribute.Name.LocalName, attribute.Value);
            }
            if (element.HasElements)
                foreach (var child in element.Elements()) WriteElement(exporter, child);
            else exporter.AddStringData(element.Value);
            exporter.EndElement();
        }

        static string Format(object value) => value switch
        {
            bool b => XmlConvert.ToString(b),
            DateTime d => XmlConvert.ToString(d, XmlDateTimeSerializationMode.RoundtripKind),
            Enum e => Convert.ToInt64(e, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };

        (Func<string> Get, Action<string> Set)? FindLegacyMember(string path)
        {
            object target = _owner;
            var parts = path.Split('/');
            for (var i = 0; i < parts.Length; i++)
            {
                if (target == null) return null;
                var name = parts[i] == "SpolecneAtributyVsechObjektu" ? "SpolecneAtributy" : parts[i];
                var type = target.GetType();
                var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
                if (property == null && field == null && parts.Length == 1 && _owner.SpolecneAtributyZPS != null)
                {
                    target = _owner.SpolecneAtributyZPS;
                    type = target.GetType();
                    field = type.GetField(name == "ZpusobPorizeniPB_ZPS" ? "ZpusobPorizeniZPS" : name);
                }
                if (property == null && field == null) return null;
                // Generated properties already read/write this store, never reflect them back into it.
                if (property?.DeclaringType == type && type.GetCustomAttribute<DtmGeneratedElementAttribute>() != null) return null;
                if (i < parts.Length - 1) { target = property != null ? property.GetValue(target) : field.GetValue(target); continue; }
                var memberType = property?.PropertyType ?? field.FieldType;
                if (memberType != typeof(string) && !memberType.IsValueType) return null;
                var owner = target;
                return (() => { var value = property != null ? property.GetValue(owner) : field.GetValue(owner); return value == null ? null : Format(value); }, text =>
                {
                    var scalar = Nullable.GetUnderlyingType(memberType) ?? memberType;
                    object value = text == null ? (memberType.IsValueType && Nullable.GetUnderlyingType(memberType) == null ? Activator.CreateInstance(memberType) : null)
                        : scalar.IsEnum ? Enum.ToObject(scalar, long.Parse(text, CultureInfo.InvariantCulture))
                        : scalar == typeof(bool) ? XmlConvert.ToBoolean(text)
                        : scalar == typeof(DateTime) ? XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind)
                        : Convert.ChangeType(text, scalar, CultureInfo.InvariantCulture);
                    if (property?.CanWrite == true) property.SetValue(owner, value);
                    else if (field != null) field.SetValue(owner, value);
                });
            }
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DtmGeneratedElementAttribute : Attribute { }
}
