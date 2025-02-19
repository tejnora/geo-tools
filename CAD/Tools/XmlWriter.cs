using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GeoHelper.Utils;

namespace CAD.Tools
{
    class XmlWriter
    {
        StringBuilder _output = new StringBuilder();
        List<Tuple<string, bool>> _elementStack = new List<Tuple<string, bool>>();
        List<string> _indents = new List<string>();
        bool _humanReadable = true;
        bool _shouldWriteNL = false;
        bool _inElement = false;
        bool _wasEndElement = true;
        bool _preserveSpaces = false;

        public XmlWriter()
        {
            Init();
        }

        public void WriteTo(string path)
        {
            Debug.Assert(_elementStack.Count == 0);
            File.WriteAllText(path, _output.ToString());
        }

        public void Init()
        {
            _inElement = false;
            _wasEndElement = true;
            _shouldWriteNL = false;
            _humanReadable = true;
            CreateIndents();
        }
        void CreateIndents()
        {
            if (!_humanReadable)
                return;
            _indents = new List<string>();
            for (var i = 0; i < 20; i++)
            {
                _indents.Add(" ".PadLeft(i * 2));
            }
        }
        void Indent(int level)
        {
            if (level < 20)
            {
                _output.Append(_indents[level]);
            }
            else
            {
                _output.Append(_indents.Last());
                for (var i = 20; i < level; i++)
                {
                    _output.Append(" ");
                }
            }
        }

        public void WriteStartElement(string localName, string ns)
        {
            WriteStartElement(string.IsNullOrEmpty(ns) ? localName : $"{ns}:{localName}");
        }
        public void WriteStartElement(string localName)
        {
            if (_shouldWriteNL)
            {
                _shouldWriteNL = false;
                if (_humanReadable)
                    _output.Append("\r\n");
            }
            if (_inElement)
            {
                _output.Append(">");
            }

            if (!_preserveSpaces && _humanReadable)
            {
                if (_inElement || !_wasEndElement)
                {
                    _output.Append("\r\n");
                }
                Indent(_elementStack.Count);
            }
            _inElement = true;
            _wasEndElement = false;
            _elementStack.Add(new Tuple<string, bool>(localName, _preserveSpaces));
            _output.Append("<" + localName);
        }
        public void WriteEndElement()
        {
            if (_inElement)
            {
                _output.Append("/>");
                _inElement = false;
                _wasEndElement = true;
                goto stackpop;
            }
            if (_shouldWriteNL && (_wasEndElement || _inElement))
            {
                _shouldWriteNL = false;
                if (_humanReadable)
                    _output.Append("\r\n");
            }
            if (_wasEndElement)
            {
                if (!_preserveSpaces && _humanReadable) Indent(_elementStack.Count - 1);
            }
            else
            {
                _wasEndElement = true;
            }
            _output.Append("</" + _elementStack.Last().Item1 + ">");
        stackpop:
            _elementStack.Pop();
            if (_elementStack.Count == 0 && !_preserveSpaces)
            {
                _shouldWriteNL = false;
                if (_humanReadable)
                    _output.Append("\r\n");
            }
            else
            {
                _preserveSpaces = _elementStack.Count == 0 ? false : _elementStack.Back().Item2;
                _shouldWriteNL = !_preserveSpaces;
            }

        }

        public void WriteAttributeString(string localName, string ns, string value)
        {
            WriteAttributeString(string.IsNullOrEmpty(ns) ? localName : $"{ns}:{localName}", value);
        }
        public void WriteAttributeString(string localName, string value)
        {
            if (localName == "xml:space")
            {
                if (value == "preserve")
                {
                    if (_preserveSpaces)
                        return;
                    _preserveSpaces = true;
                }
                else if (value == "default")
                {
                    if (!_preserveSpaces)
                        return;
                    _preserveSpaces = false;
                }
            }
            _output.Append(" " + localName + "=\"");
            TranslateString(value, false);
            _output.Append("\"");

        }
        public void WriteString(string data, bool fillSpaceAtr = true)
        {
            if (_shouldWriteNL)
            {
                _shouldWriteNL = false;
            }
            _wasEndElement = false;
            if (string.IsNullOrEmpty(data)) return;
            if (_inElement)
            {
                /* if (_handleInvalidChars)
                     {
                         String replacedInvalidChars = replaceInvalidXMLChars(aData);
                         if (replacedInvalidChars != aData)
                         {
                             addAttribute("Base64PCData", LocaleUtils::fromStringToUtf8(aData).toBase64String());
                             data = replacedInvalidChars;
                         }
                     }*/
                if (fillSpaceAtr && !_preserveSpaces && (data[0] == ' ' || data[data.Length - 1] == ' '))
                {
                    _preserveSpaces = true;
                    _elementStack[_elementStack.Count - 1] = new Tuple<string, bool>(_elementStack.Back().Item1, true);
                    _output.Append(" xml:space=\"preserve\"");
                }
                _inElement = false;
                _output.Append(">");
            }
            TranslateString(data, true, !fillSpaceAtr);
        }
        void TranslateString(string input, bool spaceBeginEnd, bool writeSpace = false)
        {

            var len = input.Length;
            var lastIndex = 0;
            var i = 0;
            for (i = 0; i < len; i++)
            {
                var c = input[i];
                if (c < 0x20 || (c == 0x20 && !writeSpace && spaceBeginEnd && (i == 0 || i == len - 1)))
                {
                    var str = Convert.ToByte(c).ToString("x2");
                    if (i - lastIndex > 0)
                    {
                        _output.Append(input.Substring(lastIndex, i - lastIndex));
                    }
                    _output.Append("&#x" + str + ";");
                    lastIndex = i + 1;
                }
                else switch (c)
                    {
                        case '<':
                            {
                                if (i - lastIndex > 0)
                                {
                                    _output.Append(input.Substring(lastIndex, i - lastIndex));
                                }
                                _output.Append("&lt;");
                                lastIndex = i + 1;
                                break;
                            }
                        case '>':
                            {
                                if (i - lastIndex > 0)
                                {
                                    _output.Append(input.Substring(lastIndex, i - lastIndex));
                                }
                                _output.Append("&gt;");
                                lastIndex = i + 1;
                                break;
                            }
                        case '&':
                            {
                                if (i - lastIndex > 0)
                                {
                                    _output.Append(input.Substring(lastIndex, i - lastIndex));
                                }
                                _output.Append("&amp;");
                                lastIndex = i + 1;
                                break;
                            }
                        case '\'':
                            {
                                if (i - lastIndex > 0)
                                {
                                    _output.Append(input.Substring(lastIndex, i - lastIndex));
                                }
                                _output.Append("&#39;"); // IE does not support &apos;
                                lastIndex = i + 1;
                                break;
                            }
                        case '\"':
                            {
                                if (i - lastIndex > 0)
                                {
                                    _output.Append(input.Substring(lastIndex, i - lastIndex));
                                }
                                _output.Append("&quot;");
                                lastIndex = i + 1;
                                break;
                            }
                    }
            }
            if (lastIndex == 0) _output.Append(input);
            else if (i - lastIndex > 0)
            {
                _output.Append(input.Substring(lastIndex));
            }
        }

        public void WriteStartDocument()
        {
            _output.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
        }
        public void WriteEndDocument()
        {

        }
    }
}
