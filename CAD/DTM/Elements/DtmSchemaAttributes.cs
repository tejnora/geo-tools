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
            RelocateUnknownPaths();
        }

        /// <summary>
        /// JVF DTM 1.5 moved some attributes (e.g. ZpusobPorizeniZPS) out of their 1.4.3 parent group.
        /// Values whose path is unknown to the schema are moved to the single known path with the same attribute name.
        /// </summary>
        void RelocateUnknownPaths()
        {
            var known = Fields().Select(f => f.Path).ToList();
            foreach (var path in _values.Keys.Where(p => !known.Contains(p)).ToList())
            {
                var name = path.Split('/').Last();
                var targets = known.Where(p => p.Split('/').Last() == name).ToList();
                if (targets.Count != 1 || _values.ContainsKey(targets[0])) continue;
                _values[targets[0]] = _values[path];
                _values.Remove(path);
            }
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

        public void ImportRecordAttribute(XmlElement element)
        {
            Read(element, element.LocalName);
            RelocateUnknownPaths();
        }

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

        /// <summary>
        /// Validates the values against the XSD. Without a context the complete (reference) record is checked;
        /// with a JVF DTM 1.5 context (Ins, Upd, Del) only the attributes of that record variant are checked.
        /// </summary>
        public IReadOnlyList<string> Validate(string context = null)
        {
            SynchronizeLegacyValues();
            var errors = new List<string>();
            var (attributes, recordAttributes) = References(context);
            foreach (var reference in attributes.Concat(recordAttributes))
                Validate(reference, reference.Name, errors);
            return errors;
        }

        void Validate(DtmAttributeReference reference, string path, List<string> errors)
        {
            var attribute = DtmSchema.Instance.Attributes[reference.Name];
            if (IsGroup(attribute))
            {
                if (!reference.Required && !IsPresent(path)) return;
                foreach (var child in Children(reference, attribute))
                    Validate(child, path + "/" + child.Name, errors);
                return;
            }
            if (!IsPresent(path))
            {
                if (reference.Required) errors.Add(path + ": povinný údaj není vyplněn.");
                return;
            }
            if (!AttributeSchemas.Value.GlobalElements.Contains(new XmlQualifiedName(reference.Name, "atr"))) return;
            try
            {
                var node = Build(reference, path);
                if (node != null) new XDocument(node).Validate(AttributeSchemas.Value, (_, e) => errors.Add(path + ": " + e.Message));
            }
            catch (XmlException ex) { errors.Add(path + ": " + ex.Message); }
        }

        (IReadOnlyList<DtmAttributeReference> Attributes, IReadOnlyList<DtmAttributeReference> RecordAttributes) References(string context)
        {
            if (context == null) return (Definition.Attributes, Definition.RecordAttributes);
            if (!Definition.Contexts.TryGetValue(context, out var record))
                throw new InvalidOperationException($"{Definition.Name}: kontext záznamu {context} není ve schématu JVF DTM definován.");
            return (record.Attributes, record.RecordAttributes);
        }

        static bool IsGroup(DtmAttributeDefinition attribute) => attribute.Type == "complex" && !attribute.Children.Any(c => c.Namespace == "gml");

        static IEnumerable<DtmAttributeReference> Children(DtmAttributeReference reference, DtmAttributeDefinition attribute) =>
            reference.Type != null && DtmSchema.Instance.Types.TryGetValue(reference.Type, out var typed) ? typed : attribute.Children;

        bool IsPresent(string path) => _values.ContainsKey(path) || _values.Keys.Any(p => p.StartsWith(path + "/", StringComparison.Ordinal));

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

        public IEnumerable<XElement> ToXml(bool includeCommon = true) => ToXml(null, includeCommon);

        /// <summary>Attribute elements of the record; with a context (Ins, Upd, Del) in the layout of that JVF DTM 1.5 record variant.</summary>
        public IEnumerable<XElement> ToXml(string context, bool includeCommon = true)
        {
            SynchronizeLegacyValues();
            foreach (var reference in References(context).Attributes)
            {
                if (!includeCommon && reference.Name == "SpolecneAtributyVsechObjektu") continue;
                var element = Build(reference, reference.Name);
                if (element != null) yield return element;
            }
        }

        public IEnumerable<XElement> RecordXml(string context = null)
        {
            foreach (var reference in References(context).RecordAttributes)
            {
                var element = Build(reference, reference.Name);
                if (element != null) yield return element;
            }
        }

        XElement Build(DtmAttributeReference reference, string path)
        {
            var attribute = DtmSchema.Instance.Attributes[reference.Name];
            var group = IsGroup(attribute);
            var children = group ? Children(reference, attribute).ToList() : null;
            var present = group ? children.Any(c => IsPresent(path + "/" + c.Name)) : IsPresent(path);
            if (!reference.Required && !present) return null;
            // Groups declared locally in the object schema (JVF DTM 1.5) are qualified by the object's namespace.
            var ns = reference.Namespace == "local" ? _owner.Options?.XmlNamespace ?? "atr" : "atr";
            var element = new XElement(XName.Get(reference.Name, ns));
            if (attribute.Type == "complex")
            {
                if (!group)
                {
                    if (_values.TryGetValue(path, out var raw))
                    {
                        var parsed = XElement.Parse(raw);
                        if (parsed.Name != element.Name) throw new XmlException("Očekáván element " + element.Name);
                        return parsed;
                    }
                    return element;
                }
                foreach (var child in children)
                {
                    var nested = Build(child, path + "/" + child.Name);
                    if (nested != null) element.Add(nested);
                }
            }
            else if (_values.TryGetValue(path, out var text) && text.Length > 0) element.Value = text;
            // An empty value of a nillable non-string attribute (JVF DTM 1.5, e.g. TridaPresnostiVyska of DTI) is written as xsi:nil.
            else if (attribute.Nillable && attribute.Type != "string")
                element.SetAttributeValue(XName.Get("nil", "http://www.w3.org/2001/XMLSchema-instance"), "true");
            return element;
        }

        public void Export(IDtmExporter exporter, bool includeCommon = true) => Export(exporter, null, includeCommon);

        public void Export(IDtmExporter exporter, string context, bool includeCommon = true)
        {
            foreach (var element in ToXml(context, includeCommon)) WriteElement(exporter, element);
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
