using System.IO;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Drawing;
using CAD.VFK;

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
                        var width = (float)value.GetProperty("Width").GetDouble();
                        var color = value.GetProperty("Color");
                        var r = color[0].GetInt32();
                        var g = color[1].GetInt32();
                        var b = color[2].GetInt32();
                        var a = color[3].GetInt32();
                        var elementOption = new DtmElementOption
                        {
                            Color = Color.FromArgb(a, r, g, b),
                            Width = width,
                            ElementType = GetElementType(value.GetProperty("Type").GetString()),
                            CodeBase = value.GetProperty("CodeBase").GetString(),
                            CodeSuffix = value.GetProperty("CodeSuffix").GetString(),
                            XmlNamespace = value.GetProperty("Xmlns").GetString(),
                            ObjektovyTypNazev = value.GetProperty("ObjektovyTypNazev").GetString(),
                            KategorieObjektu = value.GetProperty("KategorieObjektu").GetString(),
                            SkupinaObjektu = value.GetProperty("SkupinaObjektu").GetString(),
                            ObsahovaCast = value.GetProperty("ObsahovaCast").GetString()
                        };
                        ElementSetting[field.Name] = elementOption;
                    }
                }
            }
            catch { }
        }

        ElementType GetElementType(string value)
        {
            switch (value)
            {
                case "point": return ElementType.Point;
                case "line": return ElementType.Line;
            }

            throw new UnExpectException();
        }

        public static DtmConfigurationSingleton Instance => _instance;
        public Dictionary<string, DtmElementOption> ElementSetting { get; }
    }
}
