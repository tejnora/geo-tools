using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace GeoBase.Localization
{
    public class XmlLanguageDictionary : LanguageDictionary
    {
        readonly Dictionary<string, Dictionary<string, string>> _data = new Dictionary<string, Dictionary<string, string>>();
        public string Path { get; private set; }
        public string LocalizationSettingPath { get; private set; }

        public XmlLanguageDictionary(string path, string localizationSettings)
        {
            Path = path;
            LocalizationSettingPath = localizationSettings;
        }

        protected override void OnLoad()
        {
            if (!string.IsNullOrEmpty(LocalizationSettingPath))
                LoadFromFile();
            else
                LoadFromFileOld();
        }

        void LoadFromFile()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(LocalizationSettingPath);
            if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.Name != "LocalizationSettings")
            {
                throw new XmlException("Invalid localization settings.");
            }
            var rootElement = xmlDocument["LocalizationSettings"];
            if (rootElement["CultureName"] != null)
                CultureName = rootElement["CultureName"].InnerText;
            if (rootElement["EnglishName"] != null)
                EnglishName = rootElement["EnglishName"].InnerText;
            var guiElements = rootElement["GuiElements"];
            if (guiElements != null)
            {
                foreach (XmlElement guiElement in guiElements.ChildNodes)
                {
                    ParseCurrentFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(LocalizationSettingPath), guiElement.InnerText));
                }
            }
        }

        void ParseCurrentFile(string fileName)
        {
            if (!File.Exists(fileName))
                throw new XmlException("Invalid file name.");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.Name != "Localization")
                throw new XmlException("Invalid localization root element.");
            foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Gui":
                        ParseGuiNode(node);
                        break;
                    case "Protocols":
                        ParseProtocolsNode(node);
                        break;
                    case "Deviations":
                        ParseDeviationsNode(node);
                        break;
                    case "Exceptions":
                        ParseExceptionsNode(node);
                        break;
                    default:
                        throw new XmlException(string.Format("Invalid localization node {0}", node.Name));
                }

            }
        }

        void ParseExceptionsNode(XmlNode exceptionsNodes)
        {
            foreach (XmlNode node in exceptionsNodes.ChildNodes)
            {
                if (node.Name != "Value") continue;
                var innerData = new Dictionary<string, string>();
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name == "Id")
                    {
                        if (!_data.ContainsKey(attribute.Value))
                            _data["Exception." + attribute.Value] = innerData;
                    }
                    else
                        innerData[attribute.Name] = attribute.Value;
                }
            }
        }

        void ParseDeviationsNode(XmlNode deviationsNode)
        {
            foreach (XmlNode node in deviationsNode.ChildNodes)
            {
                if (node.Name != "Value") continue;
                var innerData = new Dictionary<string, string>();
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name == "Id")
                    {
                        if (!_data.ContainsKey(attribute.Value))
                            _data["Deviations." + attribute.Value] = innerData;
                    }
                    else
                        innerData[attribute.Name] = attribute.Value;
                }
            }
        }

        void ParseProtocolsNode(XmlNode protocolsNode)
        {
            foreach (XmlNode protocol in protocolsNode.ChildNodes)
            {
                var protocolName = "Protocols." + protocol.Name + "." + protocol.Attributes["MethodName"].Value;
                var innerData = new Dictionary<string, string>();
                foreach (XmlNode node in protocol.ChildNodes)
                {
                    var text = node.InnerXml;
                    text = text.Replace("&gt;", ">");
                    text = text.Replace("&lt;", "<");
                    text = text.Replace("\r\n", "\n");
                    var lines = text.Split('\n');
                    var correctionString = new StringBuilder();
                    foreach (var line in lines)
                    {
                        correctionString.Append(line.TrimStart(' '));
                    }
                    innerData[node.Name] = correctionString.ToString();
                }
                _data[protocolName] = innerData;
            }
        }

        void ParseGuiNode(XmlNode guiNode)
        {
            foreach (XmlNode nameSpaceElement in guiNode.ChildNodes)
            {
                var nameSpace = string.Empty;
                if (nameSpaceElement.Attributes["Value"] != null)
                {
                    nameSpace = nameSpaceElement.Attributes["Value"].Value;
                }
                foreach (XmlNode node in nameSpaceElement.ChildNodes)
                {
                    if (node.Name == "Value")
                    {
                        var innerData = new Dictionary<string, string>();
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.Name == "Id")
                            {
                                if (!_data.ContainsKey(attribute.Value))
                                    _data[nameSpace + "." + attribute.Value] = innerData;
                            }
                            else
                                innerData[attribute.Name] = attribute.Value;
                        }
                    }
                }
            }

        }

        protected void LoadFromFileOld()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(Path);
            if (xmlDocument.DocumentElement.Name != "Dictionary")
            {
                throw new XmlException("Invalid root element. Must be Dictionary");
            }
            var englishNameAttribute = xmlDocument.DocumentElement.Attributes["EnglishName"];
            if (englishNameAttribute != null)
            {
                EnglishName = englishNameAttribute.Value;
            }
            var cultureNameAttribute = xmlDocument.DocumentElement.Attributes["CultureName"];
            if (cultureNameAttribute != null)
            {
                CultureName = cultureNameAttribute.Value;
            }
            foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                if (node.Name == "Value")
                {
                    var innerData = new Dictionary<string, string>();
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (attribute.Name == "Id")
                        {
                            if (!_data.ContainsKey(attribute.Value))
                            {
                                _data[attribute.Value] = innerData;
                            }
                        }
                        else
                        {
                            innerData[attribute.Name] = attribute.Value;
                        }
                    }
                }
            }
        }

        protected override void OnUnload()
        {
            _data.Clear();
        }

        protected override object OnTranslate(string uid, string vid, object defaultValue, Type type)
        {
            if (string.IsNullOrEmpty(uid))
            {
                Debug.WriteLine(string.Format("Uid must not be null or empty"));
                return defaultValue;
            }
            if (string.IsNullOrEmpty(vid))
            {
                Debug.WriteLine(string.Format("Vid must not be null or empty"));
                return defaultValue;
            }
            if (!_data.ContainsKey(uid))
            {
                Debug.WriteLine(string.Format("Uid {0} was not found in the {1} dictionary", uid, EnglishName));
                return defaultValue;
            }
            var innerData = _data[uid];
            if (!innerData.ContainsKey(vid))
            {
                Debug.WriteLine(string.Format("Vid {0} was not found for Uid {1}, in the {2} dictionary", vid, uid, EnglishName));
                return defaultValue;
            }
            var textValue = innerData[vid];
            try
            {
                if (type == typeof(object))
                {
                    return textValue;
                }
                var typeConverter = TypeDescriptor.GetConverter(type);
                var translation = typeConverter.ConvertFromString(textValue);
                return translation;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Failed to translate text {0} in dictionary {1}:\n{2}", textValue, EnglishName, ex.Message));
                return null;
            }
        }
    }
}
