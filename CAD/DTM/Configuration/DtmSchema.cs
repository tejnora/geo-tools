using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CAD.DTM.Configuration
{
    public sealed class DtmAttributeReference
    {
        public string Name { get; set; }
        public string Namespace { get; set; } = "atr";
        public bool Required { get; set; }
    }

    public sealed class DtmAttributeDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Values { get; set; } = new();
        public List<DtmAttributeReference> Children { get; set; } = new();
        public Dictionary<string, string> Facets { get; set; } = new();
    }

    public sealed class DtmObjectDefinition
    {
        public string Name { get; set; }
        public string File { get; set; }
        public string ExchangeName { get; set; }
        public string ElementType { get; set; }
        public List<string> Geometries { get; set; } = new();
        public List<DtmAttributeReference> Attributes { get; set; } = new();
        public List<DtmAttributeReference> RecordAttributes { get; set; } = new();
    }

    public sealed class DtmSchema
    {
        static readonly Lazy<DtmSchema> Schema = new(() => ReadResource<DtmSchema>("DtmSchema.json"));
        static readonly Lazy<Dictionary<string, string>> Aliases = new(() => ReadResource<Dictionary<string, string>>("DtmAliases.json"));
        public static DtmSchema Instance => Schema.Value;
        public Dictionary<string, DtmAttributeDefinition> Attributes { get; set; }
        public Dictionary<string, DtmObjectDefinition> Objects { get; set; }

        public static string CanonicalName(string name) => Aliases.Value.TryGetValue(name, out var canonical) ? canonical : name;

        internal static T ReadResource<T>(string filename)
        {
            var assembly = typeof(DtmSchema).Assembly;
            var resource = assembly.GetManifestResourceNames().Single(n => n.EndsWith("." + filename, StringComparison.Ordinal));
            using var stream = assembly.GetManifestResourceStream(resource);
            return JsonSerializer.Deserialize<T>(stream);
        }

        public static string XsdDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DTM", "Doc", "xsd");
    }
}
