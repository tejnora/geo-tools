using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using CAD.DTM.Elements;
using GeoBase.Utils;

namespace CAD.DTM.Configuration
{
    public class DtmConfigurationSingleton
    {
        static readonly Lazy<DtmConfigurationSingleton> Singleton = new(() => Load(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DtmConfiguration.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DtmSymbols")));
        public static DtmConfigurationSingleton Instance => Singleton.Value;
        public Dictionary<string, DtmElementOption> ElementSetting { get; } = new(StringComparer.Ordinal);
        public List<string> Diagnostics { get; } = new();
        readonly Dictionary<string, Image> _symbols = new(StringComparer.OrdinalIgnoreCase);

        DtmConfigurationSingleton() { }

        public static DtmConfigurationSingleton Load(string filename, string symbolDirectory)
        {
            var result = new DtmConfigurationSingleton();
            using var document = JsonDocument.Parse(File.ReadAllText(filename));
            foreach (var field in document.RootElement.EnumerateObject())
            {
                try
                {
                    var value = field.Value;
                    var name = DtmSchema.CanonicalName(field.Name);
                    if (!DtmSchema.Instance.Objects.TryGetValue(name, out var schema))
                        throw new InvalidDataException("Chybí definice XSD.");
                    var option = new DtmElementOption
                    {
                        Name = name,
                        ElementType = Enum.Parse<DtmElementType>(schema.ElementType),
                        CodeBase = String(value, "CodeBase"), CodeSuffix = String(value, "CodeSuffix"),
                        XmlNamespace = String(value, "Xmlns"), ObjektovyTypNazev = String(value, "ObjektovyTypNazev"),
                        KategorieObjektu = String(value, "KategorieObjektu"), SkupinaObjektu = String(value, "SkupinaObjektu"),
                        ObsahovaCast = String(value, "ObsahovaCast"), GraphicSource = String(value, "GraphicSource"),
                        GraphicAttribute = String(value, "GraphicAttribute"),
                        ClassType = typeof(DtmElement).Assembly.GetType($"CAD.DTM.Elements.Dtm{name}Element"),
                        Width = value.TryGetProperty("Width", out var width) ? (float)width.GetDouble() : 0.00018f,
                        Color = value.TryGetProperty("Color", out var color) ? ColorValue(color) : Color.Gray
                    };
                    if (option.ClassType == null || option.ClassType.IsAbstract)
                        throw new InvalidDataException("Chybí implementace objektu.");
                    if (value.TryGetProperty("Graphic", out var graphic))
                    {
                        if (graphic.ValueKind == JsonValueKind.Array)
                            option.RegisterGraphicElementByType("_", result.ReadGraphics(name, graphic, symbolDirectory));
                        else
                            foreach (var variant in graphic.EnumerateObject())
                                option.RegisterGraphicElementByType(variant.Name, result.ReadGraphics(name, variant.Value, symbolDirectory));
                    }
                    if (option.GetGraphicElement("_", DtmGraphicElementScaleEnum._500) == null
                        && option.ElementType is DtmElementType.Bod or DtmElementType.DefinicniBod)
                        option.RegisterGraphicElementByType("_", new[] { result.NeutralGraphic(option.Color) });
                    if (!result.ElementSetting.TryAdd(name, option))
                        throw new InvalidDataException("Duplicitní název nebo alias objektu.");
                }
                catch (Exception ex) when (ex is not OutOfMemoryException)
                {
                    throw new InvalidDataException($"DTM konfigurace: {field.Name}: {ex.Message}", ex);
                }
            }
            var missing = DtmSchema.Instance.Objects.Keys.Except(result.ElementSetting.Keys).ToArray();
            if (missing.Length > 0) throw new InvalidDataException("DTM konfigurace neobsahuje: " + string.Join(", ", missing));
            return result;
        }

        static string String(JsonElement value, string property) =>
            value.TryGetProperty(property, out var field) && field.ValueKind == JsonValueKind.String ? field.GetString() : null;

        static Color ColorValue(JsonElement value) => Color.FromArgb(value[3].GetInt32(), value[0].GetInt32(), value[1].GetInt32(), value[2].GetInt32());

        IList<DtmGraphicElement> ReadGraphics(string name, JsonElement array, string directory)
        {
            var result = new List<DtmGraphicElement>();
            foreach (var value in array.EnumerateArray())
            {
                var filename = value.GetProperty("Symbol").GetString();
                var color = ColorValue(value.GetProperty("Color"));
                Image image;
                try
                {
                    if (!_symbols.TryGetValue(filename, out image))
                    {
                        using var source = Image.FromFile(Path.Combine(directory, filename));
                        image = new Bitmap(source);
                        _symbols.Add(filename, image);
                    }
                }
                catch (Exception ex) when (ex is FileNotFoundException or ArgumentException or OutOfMemoryException)
                {
                    Diagnostics.Add($"{name}: symbol {filename}: {ex.Message}; použita neutrální značka.");
                    image = NeutralGraphic(color).Symbol;
                }
                var size = value.GetProperty("Width").GetDouble();
                result.Add(new DtmGraphicElement { Color = color, Size = new UnitPoint(size, size), Symbol = image });
            }
            return result;
        }

        DtmGraphicElement NeutralGraphic(Color color)
        {
            var bitmap = new Bitmap(32, 32);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var pen = new Pen(color, 3))
            {
                graphics.Clear(Color.Transparent);
                graphics.DrawEllipse(pen, 5, 5, 22, 22);
                graphics.DrawLine(pen, 16, 2, 16, 30);
                graphics.DrawLine(pen, 2, 16, 30, 16);
            }
            return new DtmGraphicElement { Color = color, Size = new UnitPoint(0.8, 0.8), Symbol = bitmap };
        }

        public DtmElement CreateType(string elementName)
        {
            elementName = DtmSchema.CanonicalName(elementName);
            if (!ElementSetting.TryGetValue(elementName, out var option))
                throw new InvalidDataException($"Neznámý typ objektu DTM: {elementName}.");
            var element = (DtmElement)Activator.CreateInstance(option.ClassType);
            element.Init(option);
            if (element.SchemaAttributes != null)
            {
                foreach (var field in element.SchemaAttributes.Fields())
                {
                    int? value = field.Attribute.Name switch
                    {
                        "UrovenUmisteniObjektuZPS" or "UrovenUmisteniObjektuTI" or "UrovenUmisteniObjektuDI" => 0,
                        "TridaPresnostiPoloha" or "TridaPresnostiVyska" => 3,
                        "ZpusobPorizeniZPS" or "ZpusobPorizeniPB_ZPS" or "ZpusobPorizeniTI" or "ZpusobPorizeniDI" => 1,
                        _ => null
                    };
                    if (value.HasValue)
                        element.SchemaAttributes.Set(field.Path, value.Value);
                }
                element.SchemaAttributes.CaptureLegacyValues();
            }
            return element;
        }

        public DtmElementsGroup CreateGroup(string elementName)
        {
            elementName = DtmSchema.CanonicalName(elementName);
            var option = ElementSetting[elementName];
            return new DtmElementsGroup(elementName)
            {
                CodeBase = option.CodeBase, CodeSuffix = option.CodeSuffix, KategorieObjektu = option.KategorieObjektu,
                ObjektovyTypNazev = option.ObjektovyTypNazev, ObsahovaCast = option.ObsahovaCast, SkupinaObjektu = option.SkupinaObjektu
            };
        }

        public static string ToNiceName(string name, DtmElementType category)
        {
            name = Regex.Replace(name, "(DefinicniBod|Plocha|Bod)$", "");
            return Regex.Replace(name, @"(?<!^)(?=[A-Z][a-z])", " ");
        }
    }
}
