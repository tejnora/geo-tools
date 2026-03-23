using System.IO;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using CAD.DTM.Elements;
using CAD.VFK;
using GeoBase.Utils;
using Image = System.Drawing.Image;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CAD.DTM.Configuration
{
    public class DtmConfigurationSingleton
    {
        static readonly DtmConfigurationSingleton _instance = new DtmConfigurationSingleton();
        DtmConfigurationSingleton()
        {
            ElementSetting = new Dictionary<string, DtmElementOption>();
            try
            {
                var jsonString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Data\\DtmConfiguration.json");
                using (var document = JsonDocument.Parse(jsonString))
                {
                    var root = document.RootElement;
                    foreach (var field in root.EnumerateObject())
                    {
                        var value = field.Value;
                        var elementOption = new DtmElementOption()
                        {
                            ElementType = GetElementType(value.GetProperty("CodeSuffix").GetString()),
                            CodeBase = value.GetProperty("CodeBase").GetString(),
                            CodeSuffix = value.GetProperty("CodeSuffix").GetString(),
                            XmlNamespace = value.GetProperty("Xmlns").GetString(),
                            ObjektovyTypNazev = value.GetProperty("ObjektovyTypNazev").GetString(),
                            KategorieObjektu = value.GetProperty("KategorieObjektu").GetString(),
                            SkupinaObjektu = value.GetProperty("SkupinaObjektu").GetString(),
                            ObsahovaCast = value.GetProperty("ObsahovaCast").GetString(),
                            ClassType = Type.GetType($"CAD.DTM.Elements.Dtm{field.Name}Element")
                        };
                        ParseGraphicElement(elementOption, value);
                        try
                        {
                            var width = (float)value.GetProperty("Width").GetDouble();
                            var color = value.GetProperty("Color");
                            var r = color[0].GetInt32();
                            var g = color[1].GetInt32();
                            var b = color[2].GetInt32();
                            var a = color[3].GetInt32();
                            elementOption.Color = Color.FromArgb(a, r, g, b);
                            elementOption.Width = width;
                        }
                        catch
                        {

                        }

                        ElementSetting[field.Name] = elementOption;
                    }
                }
            }
            catch { }
        }

        void ParseGraphicElement(DtmElementOption dtmElement, JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("Graphic", out var graphic))
                return;
            IList<DtmGraphicElement> definitionByScales;
            switch (graphic.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var field in graphic.EnumerateObject())
                    {
                        definitionByScales = ParseGraphicElementArray(field.Value);
                        dtmElement.RegisterGraphicElementByType(field.Name, definitionByScales);
                    }
                    break;
                case JsonValueKind.Array:
                    definitionByScales = ParseGraphicElementArray(graphic);
                    dtmElement.RegisterGraphicElementByType("_", definitionByScales);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        IList<DtmGraphicElement> ParseGraphicElementArray(JsonElement jsonScaleElement)
        {
            var elements = new List<DtmGraphicElement>();
            foreach (var field in jsonScaleElement.EnumerateArray())
            {
                var width = (float)field.GetProperty("Width").GetDouble();
                var color = field.GetProperty("Color");
                var r = color[0].GetInt32();
                var g = color[1].GetInt32();
                var b = color[2].GetInt32();
                var a = color[3].GetInt32();
                var symbol = field.GetProperty("Symbol").GetString();
                elements.Add(new DtmGraphicElement()
                {
                    Color = Color.FromArgb(a, r, g, b),
                    Size = new UnitPoint(width, width),
                    Symbol = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DtmSymbols", symbol))
                });
            }

            return elements;
        }

        DtmElementType GetElementType(string value)
        {
            switch (int.Parse(value))
            {
                case 1: return DtmElementType.Bod;
                case 2: return DtmElementType.Linie;
                case 3: return DtmElementType.Plocha;
                case 4: return DtmElementType.DefinicniBod;
                case 5: return DtmElementType.Obvod;
                default:
                    throw new UnExpectException();
            }
        }

        public static DtmConfigurationSingleton Instance => _instance;
        public Dictionary<string, DtmElementOption> ElementSetting { get; }

        public DtmElement CreateType(string elementName)
        {
            DtmElement element;
            if (ElementSetting.TryGetValue(elementName, out var option) && option.ClassType != null)
                element = (DtmElement)Activator.CreateInstance(option.ClassType);
            else
                element = (DtmElement)Activator.CreateInstance(typeof(DtmElementGeneric));
            element.Init(ElementSetting[elementName]);
            return element;
        }

        public DtmElementsGroup CreateGroup(string elementName)
        {
            var element = ElementSetting[elementName];
            return new DtmElementsGroup(elementName)
            {
                CodeBase = element.CodeBase,
                CodeSuffix = element.CodeSuffix,
                KategorieObjektu = element.KategorieObjektu,
                ObjektovyTypNazev = element.ObjektovyTypNazev,
                ObsahovaCast = element.ObsahovaCast,
                SkupinaObjektu = element.SkupinaObjektu
            };
        }

        public static string ToNiceName(string name, DtmElementType category)
        {
            var splittedValues = Regex.Split(name, @"(?<!^)(?=[A-Z])");
            if (category == DtmElementType.Bod)
            {
                if (splittedValues.Last() == "Bod")
                    Array.Resize(ref splittedValues, splittedValues.Length - 1);
            }
            else if (category == DtmElementType.DefinicniBod)
                Array.Resize(ref splittedValues, splittedValues.Length - 2);
            for (var i = 1; i < splittedValues.Length; i++)
            {
                splittedValues[i] = splittedValues[i].ToLowerInvariant();
            }
            var res = string.Join(" ", splittedValues);
            return res;
        }
    }
}
