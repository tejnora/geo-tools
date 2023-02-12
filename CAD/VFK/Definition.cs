using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

using VFK.Tables;
using System.Globalization;

namespace VFK
{
    enum DefinitionFieldType
    {
        Number,
        Text,
        Date,
        Bool,
        None,
    }

    class DefinitionItem
    {
        public DefinitionItem(string aFieldName, DefinitionFieldType aFieldType, string aFieldLength, string aFieldLengthAfterDot)
        {
            _mFieldName = aFieldName;
            _mFieldType = aFieldType;
            if (aFieldLength.Length > 0)
            {
                _mFieldLength = Convert.ToUInt16(aFieldLength);
            }
            else
            {
                _mFieldLength = UInt32.MaxValue;
            }
            if (aFieldLengthAfterDot.Length > 0)
            {
                _mFieldLengthAfterDot = Convert.ToUInt16(aFieldLengthAfterDot);
            }
            else
            {
                _mFieldLengthAfterDot = UInt32.MaxValue;
            }
            Due = false;
        }
        public DefinitionItem(string aFieldName, DefinitionFieldType aFieldType)
        {
            _mFieldName = aFieldName;
            _mFieldType = aFieldType;
            _mFieldLength = 0;
            _mFieldLengthAfterDot = 0;
            Due = false;
        }
        public string _mFieldName;
        public DefinitionFieldType _mFieldType;
        public UInt32 _mFieldLength;
        public UInt32 _mFieldLengthAfterDot;
        public bool Due
        {
            get;
            set;
        }
    }

    class Definition
    {
        public Definition(String aDefinitionType, string aParseText)
        {
            DefintionType = aDefinitionType;
            CreateDefintionItems(aParseText);
        }
        public Definition(String aDefinitionType)
        {
            DefintionType = aDefinitionType; ;
        }
        public String DefintionType
        {
            get;
            private set;
        }
        List<DefinitionItem> _mDefinitionItems = new List<DefinitionItem>();
        private void CreateDefintionItems(string aText)
        {
            string fieldName = string.Empty;
            DefinitionFieldType fieldtype = DefinitionFieldType.None;
            string fieldLength = string.Empty;
            string fieldLengthAfterDot = string.Empty;
            bool firstStep = true;
            bool wasDot = false;

            for (int i = 0; i < aText.Length; i++)
            {
                char c = aText[i];
                if (firstStep)
                {
                    if (c == ' ')
                    {
                        firstStep = false;
                        continue;
                    }
                    fieldName += c;
                }
                else
                {
                    if (c == ';')
                    {
                        if (fieldName.Length == 0 || fieldtype == DefinitionFieldType.None)
                        {
                            throw new Exception("Invalid item in dataBlock.");
                        }
                        _mDefinitionItems.Add(new DefinitionItem(fieldName, fieldtype, fieldLength, fieldLengthAfterDot));
                        fieldName = string.Empty;
                        fieldtype = DefinitionFieldType.None;
                        fieldLength = string.Empty;
                        fieldLengthAfterDot = string.Empty;
                        firstStep = true;
                        wasDot = false;
                        continue;
                    }
                    else if (c == 'N')
                    {
                        fieldtype = DefinitionFieldType.Number;
                    }
                    else if (c == 'T')
                    {
                        fieldtype = DefinitionFieldType.Text;
                    }
                    else if (c == 'D')
                    {
                        fieldtype = DefinitionFieldType.Date;
                    }
                    else if (c == '.')
                    {
                        wasDot = true;
                    }
                    else if (char.IsDigit(c))
                    {
                        if (wasDot)
                        {
                            fieldLengthAfterDot += c;
                        }
                        else
                        {
                            fieldLength += c;
                        }
                    }
                }
            }
            if (fieldName.Length > 0)
            {
                _mDefinitionItems.Add(new DefinitionItem(fieldName, fieldtype, fieldLength, fieldLengthAfterDot));
            }
        }
        private void CreateDefinitionFromTableItem()
        {
            if (DefintionType.Length == 0)
            {
                throw new Exception("Can not create from empty string.");
            }
            string className = "VFK.Tables.VFK" + DefintionType + "TableItem";
            Type type = Type.GetType(className);
            PropertyInfo[] properties = type.GetProperties();
            Array.Sort(properties, (PropertyInfo aP1, PropertyInfo aP2) =>
            {
                if (Attribute.IsDefined(aP1, typeof(VFKDefinitionAttribute)) && Attribute.IsDefined(aP2, typeof(VFKDefinitionAttribute)))
                {
                    VFKDefinitionAttribute p1 = Attribute.GetCustomAttribute(aP1, typeof(VFKDefinitionAttribute)) as VFKDefinitionAttribute;
                    VFKDefinitionAttribute p2 = Attribute.GetCustomAttribute(aP2, typeof(VFKDefinitionAttribute)) as VFKDefinitionAttribute;
                    if (p1.IdOrder < p2.IdOrder)
                        return -1;
                    else
                        return 1;
                }
                else if (Attribute.IsDefined(aP1, typeof(VFKDefinitionAttribute)))
                {
                    return -1;
                }
                else if (Attribute.IsDefined(aP2, typeof(VFKDefinitionAttribute)))
                {
                    return 1;
                }
                else
                {
                    return aP1.Name.CompareTo(aP2.Name);
                }
            });

            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(VFKDefinitionAttribute)))
                {
                    VFKDefinitionAttribute vfkDef = Attribute.GetCustomAttribute(property, typeof(VFKDefinitionAttribute)) as VFKDefinitionAttribute;
                    switch (vfkDef.DefinitionFieldType)
                    {
                        case DefinitionFieldType.Date:
                            {
                                _mDefinitionItems.Add(new DefinitionItem(property.Name, DefinitionFieldType.Date));
                            }
                            break;
                        case DefinitionFieldType.Text:
                        case DefinitionFieldType.Number:
                            {
                                _mDefinitionItems.Add(new DefinitionItem(property.Name, vfkDef.DefinitionFieldType, vfkDef.Lenght.ToString(), vfkDef.LengthAfterDot.ToString()));
                            }
                            break;
                        default:
                            {
                                System.Diagnostics.Debug.Assert(false);
                                throw new Exception("Unexpect exception.");
                            }
                    }
                    _mDefinitionItems.Last().Due = vfkDef.Due;
                }
            }
        }
        public void SaveDefinitionToStream(StreamWriter aWriter)
        {
            if (_mDefinitionItems.Count == 0)
            {
                CreateDefinitionFromTableItem();
            }
            string def = "&B" + DefintionType;
            foreach (var item in _mDefinitionItems)
            {
                switch (item._mFieldType)
                {
                    case DefinitionFieldType.Number:
                        {
                            if (item._mFieldLengthAfterDot == 0)
                                def += string.Format(";{0} N{1}", item._mFieldName, item._mFieldLength);
                            else
                                def += string.Format(";{0} N{1}.{2}", item._mFieldName, item._mFieldLength, item._mFieldLengthAfterDot);
                        }
                        break;
                    case DefinitionFieldType.Text:
                        {
                            def += string.Format(";{0} T{1}", item._mFieldName, item._mFieldLength);
                        }
                        break;
                    case DefinitionFieldType.Date:
                        {
                            def += string.Format(";{0} D", item._mFieldName);
                        }
                        break;
                    default:
                        {
                            System.Diagnostics.Debug.Assert(false);
                            throw new Exception("Unexpect exception.");
                        };
                }
            }
            def = ToDebug(def);
            aWriter.WriteLine(def);
        }

        private string ToDebug(string aString)
        {
            return aString;
            /*string[] splited = aString.Split(';');
            aString = string.Empty;
            foreach (var s in splited)
            {
                aString += s.Replace(';','|').PadLeft(25);
            }
            return aString;*/
        }
        public string GetItemDataString(object aAnyData)
        {
            if (_mDefinitionItems.Count == 0)
            {
                System.Diagnostics.Debug.Assert(false);
                throw new Exception("Unexpect exception.");
            }
            string className = "VFK.Tables.VFK" + DefintionType + "TableItem";
            Type classType = Type.GetType(className);
            string data = "&D" + DefintionType;
            foreach (var item in _mDefinitionItems)
            {
                PropertyInfo property = classType.GetProperty(item._mFieldName);
                if (property == null)
                    throw new Exception(string.Format("Field {0} does not exist.", item._mFieldName));
                switch (item._mFieldType)
                {
                    case DefinitionFieldType.Date:
                        {
                            DateTime date = (DateTime)property.GetValue(aAnyData, null);
                            if (date.Kind == DateTimeKind.Unspecified)
                            {
                                if (item.Due)
                                {
                                    throw new Exception("Attribute is empty, but must be set.");
                                }
                                data += ";\"\"";
                            }
                            else
                            {
                                data += string.Format(";\"{0:00}.{1:00}.{2:0000} {3:00}:{4:00}:{5:00}\"", date.Day, date.Month, date.Year, date.Hour,
                                    date.Minute, date.Second);
                            }

                        }
                        break;
                    case DefinitionFieldType.Text:
                        {
                            string text = (string)property.GetValue(aAnyData, null);
                            if (text == null || text.Length == 0)
                            {
                                if (item.Due)
                                {
                                    throw new Exception("Attribute is empty, but must be set.");
                                }
                                data += ";\"\"";
                            }
                            else if (text.Length > item._mFieldLength)
                            {
                                throw new Exception("Icorrect format.");
                            }
                            else
                            {
                                data += string.Format(";\"{0}\"", text);
                            }
                        }
                        break;
                    case DefinitionFieldType.Number:
                        {
                            if (property.PropertyType.FullName == "System.Int32")
                            {
                                var number = (int)property.GetValue(aAnyData, null);
                                if (number == int.MaxValue)
                                {
                                    if (item.Due)
                                    {
                                        throw new Exception("Attribute is empty, but must be set.");
                                    }
                                    data += ";";
                                }
                                else if (number.ToString().Length > item._mFieldLength)
                                {
                                    throw new Exception("Icorrect format.");
                                }
                                else
                                {
                                    data += $";{number}";
                                }
                            }
                            else if (property.PropertyType.FullName == "System.UInt64")
                            {
                                UInt64 number = (UInt64)property.GetValue(aAnyData, null);
                                if (number == UInt64.MaxValue)
                                {
                                    if (item.Due)
                                    {
                                        throw new Exception("Attribute is empty, but must be set.");
                                    }
                                    data += ";";
                                }
                                else if (number.ToString().Length > item._mFieldLength)
                                {
                                    throw new Exception("Icorrect format.");
                                }
                                else
                                {
                                    data += string.Format(";{0}", number);
                                }
                            }
                            else if (property.PropertyType.FullName == "System.UInt32")
                            {
                                UInt32 number = (UInt32)property.GetValue(aAnyData, null);
                                if (number == UInt32.MaxValue)
                                {
                                    if (item.Due)
                                    {
                                        throw new Exception("Attribute is empty, but must be set.");
                                    }
                                    data += ";";
                                }
                                else if (number.ToString().Length > item._mFieldLength)
                                {
                                    throw new Exception("Icorrect format.");
                                }
                                else
                                {
                                    data += string.Format(";{0}", number);
                                }
                            }
                            else if (property.PropertyType.FullName == "System.Double")
                            {
                                Double number = (Double)property.GetValue(aAnyData, null);
                                if (number == double.MaxValue)
                                {
                                    if (item.Due)
                                    {
                                        throw new Exception("Attribute is empty, but must be set.");
                                    }
                                    data += ";";
                                }
                                else if (((Int64)number + 0.5).ToString().Length > item._mFieldLength)
                                {
                                    throw new Exception("Icorrect format.");
                                }
                                else
                                {
                                    if (number == 0)
                                        data += ";0";
                                    else
                                    {
                                        string formater = "0.";
                                        formater = formater.PadLeft((int)(formater.Length + item._mFieldLength - 1), '#');
                                        formater =
                                            formater.PadRight((int)(formater.Length + item._mFieldLengthAfterDot), '#');
                                        formater = formater.Insert(0, "{0:");
                                        formater += "}";
                                        string doubleData = string.Format(";" + formater, number);
                                        data += doubleData.Replace(',', '.');
                                    }
                                }
                            }
                            else if (property.PropertyType.FullName == "System.String")
                            {
                                string subString = (string)property.GetValue(aAnyData, null);
                                if (subString == null)
                                {
                                    if (item.Due)
                                    {
                                        throw new Exception("Attribute is empty, but must be set.");
                                    }
                                    data += ";";
                                }
                                else if (subString.Length > item._mFieldLength)
                                {
                                    throw new Exception("Icorrect format.");
                                }
                                else
                                {
                                    for (int i = 0; i < subString.Length; i++)
                                    {
                                        if (!char.IsDigit(subString[i]))
                                        {
                                            throw new Exception(string.Format("Invalid number.{0}", subString));
                                        }
                                    }
                                    data += ";" + subString;
                                }
                            }
                            else
                            {
                                throw new Exception("Unexpect value type.");
                            }

                        }
                        break;
                    default:
                        {
                            System.Diagnostics.Debug.Assert(false);
                            throw new Exception("Unexpect exception.");
                        }
                };
            }
            data = ToDebug(data);
            return data;
        }
        public IVFKDataTableItem GetVFKTable(string aDataRow)
        {
            string className = "VFK.Tables.VFK" + DefintionType + "TableItem";
            Type type = Type.GetType(className);
            if (type == null)
                throw new Exception(string.Format("Class {0} does not exist.", className));
            IVFKDataTableItem vfkTable = (IVFKDataTableItem)System.Activator.CreateInstance(type);
            int parseIndex = 0;
            foreach (var item in _mDefinitionItems)
            {
                int backupParseIndex = parseIndex;
                parseIndex = aDataRow.IndexOf(';', parseIndex);
                string subString = string.Empty;
                if (parseIndex == -1)
                    subString = aDataRow.Substring(backupParseIndex);
                else
                    subString = aDataRow.Substring(backupParseIndex, parseIndex - backupParseIndex);
                parseIndex++;
                PropertyInfo property = type.GetProperty(item._mFieldName);
                if (property == null)
                    throw new Exception(string.Format("Field {0} does not exist.", item._mFieldName));
                switch (item._mFieldType)
                {
                    case DefinitionFieldType.Date:
                        {
                            System.Diagnostics.Debug.Assert(subString[0] == '\"' && subString[subString.Length - 1] == '\"');
                            if (subString.Length == 2)
                                continue;
                            //"24.05.2001 14:21:11"
                            var dateTime = DateTime.ParseExact(subString.Substring(1, subString.Length - 2), "dd.MM.yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture);
                            property.SetValue(vfkTable, dateTime, null);
                        }
                        break;
                    case DefinitionFieldType.Text:
                        {
                            if (subString[0] == '\"' && subString[subString.Length - 1] == '\"')
                            {
                                if (subString.Length == 2)
                                    continue;
                                property.SetValue(vfkTable, subString.Substring(1, subString.Length - 2), null);
                            }
                            else
                            {//probably vfk export bug, text without quotation marks 
                                property.SetValue(vfkTable, subString, null);
                            }
                        }
                        break;
                    case DefinitionFieldType.Number:
                        {
                            if (property.PropertyType.FullName == "System.UInt64")
                            {
                                if (subString.Length == 0)
                                    property.SetValue(vfkTable, UInt64.MaxValue, null);
                                else
                                    property.SetValue(vfkTable, Convert.ToUInt64(subString), null);
                            }
                            else if (property.PropertyType.FullName == "System.Int32")
                            {
                                if (subString.Length == 0)
                                    property.SetValue(vfkTable, Int32.MaxValue, null);
                                else
                                    property.SetValue(vfkTable, Convert.ToInt32(subString), null);
                            }
                            else if (property.PropertyType.FullName == "System.UInt32")
                            {
                                if (subString.Length == 0)
                                    property.SetValue(vfkTable, UInt32.MaxValue, null);
                                else
                                    property.SetValue(vfkTable, Convert.ToUInt32(subString), null);
                            }
                            else if (property.PropertyType.FullName == "System.Double")
                            {
                                if (subString.Length == 0)
                                    property.SetValue(vfkTable, Double.MaxValue, null);
                                else
                                {
                                    //subString = subString.Replace('.', ',');
                                    property.SetValue(vfkTable, Convert.ToDouble(subString,CultureInfo.InvariantCulture), null);
                                }
                            }
                            else if (property.PropertyType.FullName == "System.String")
                            {
                                for (int i = 0; i < subString.Length; i++)
                                {
                                    if (!char.IsDigit(subString[i]))
                                    {
                                        throw new Exception(string.Format("Invalid number.{0}", subString));
                                    }
                                }
                                property.SetValue(vfkTable, subString, null);
                            }
                            else
                            {
                                throw new Exception("Unexpect value type.");
                            }

                        }
                        break;
                    case DefinitionFieldType.Bool:
                        {
                            subString = subString.Trim(' ');
                            if (subString.Length != 3)
                                throw new Exception("Invalid bool value.");
                            if (subString[1] == 'a')
                                property.SetValue(vfkTable, true, null);
                            else if (subString[1] == 'n')
                                property.SetValue(vfkTable, false, null);
                            else
                                throw new Exception("Invalid bool value.");
                        }
                        break;
                    default:
                        {
                            System.Diagnostics.Debug.Assert(false);
                            throw new Exception("Unexpect exception.");
                        }
                }
            }
            return vfkTable;
        }
    };

    [AttributeUsage(AttributeTargets.Property)]
    class VFKDefinitionAttribute : Attribute
    {
        public VFKDefinitionAttribute(DefinitionFieldType aType, Int32 aLenght, Int32 aLengthAfterDot, bool aDue, UInt32 aIdOrder)
        {
            DefinitionFieldType = aType;
            Lenght = aLenght;
            LengthAfterDot = aLengthAfterDot;
            Due = aDue;
            IdOrder = aIdOrder;
        }

        public VFKDefinitionAttribute(DefinitionFieldType aType, bool aDue, UInt32 aIdOrder)
        {
            DefinitionFieldType = aType;
            Lenght = Int32.MaxValue;
            LengthAfterDot = 0;
            Due = aDue;
            IdOrder = aIdOrder;
        }

        public DefinitionFieldType DefinitionFieldType
        {
            get;
            set;
        }
        public Int32 Lenght
        {
            get;
            set;
        }
        public Int32 LengthAfterDot
        {
            get;
            set;
        }
        public bool Due
        {
            get;
            set;
        }
        public UInt32 IdOrder
        {
            get;
            set;
        }
    }
}
