//////////////////////////////////////////////////////////
//
// Copyright notice
//
// This class is originally developed by CatenaLogic
// 
// Copyright (c) CatenaLogic, 2008 - 2009
// 
// For more information, visit http://www.catenalogic.com
//
//////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////
//
// Usage example
//
// Below is a class that shows how to use the DataObjectBase class.
// The example is a simple object that represents an ini-file entry. 
//
//[Serializable]
//internal class IniData : DataObjectBase<IniData>
//{
//    //    
//    //    /// <summary>
//    /// Initializes a new object from scratch.
//    /// </summary>
//    public IniData()
//        : base(null, new StreamingContext()) { }

//    /// <summary>
//    /// Initializes a new object based on <see cref="SerializationInfo"/>.
//    /// </summary>
//    /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
//    /// <param name="context"><see cref="StreamingContext"/>.</param>
//    public IniData(SerializationInfo info, StreamingContext context)
//        : base(info, context) { }
//    
//    //    /// <summary>
//    /// Gets or sets the filename of the ini file to modify.
//    /// </summary>
//    public string FileName
//    {
//        get { return GetValue<string>(FileNameProperty); }
//        set { SetValue(FileNameProperty, value); }
//    }

//    /// <summary>
//    /// Register the property so it is known in the class.
//    /// </summary>
//    public readonly PropertyData FileNameProperty = RegisterProperty("FileName", typeof(string), string.Empty);

//    /// <summary>
//    /// Gets or sets the group inside the ini file to modify.
//    /// </summary>
//    public string Group
//    {
//        get { return GetValue<string>(GroupProperty); }
//        set { SetValue(GroupProperty, value); }
//    }

//    /// <summary>
//    /// Register the property so it is known in the class.
//    /// </summary>
//    public readonly PropertyData GroupProperty = RegisterProperty("Group", typeof(string), string.Empty);

//    /// <summary>
//    /// Gets or sets the key to modify.
//    /// </summary>
//    public string Key
//    {
//        get { return GetValue<string>(KeyProperty); }
//        set { SetValue(KeyProperty, value); }
//    }

//    /// <summary>
//    /// Register the property so it is known in the class.
//    /// </summary>
//    public readonly PropertyData KeyProperty = RegisterProperty("Key", typeof(string), string.Empty);

//    /// <summary>
//    /// Gets or sets the new value of the key.
//    /// </summary>
//    public string Value
//    {
//        get { return GetValue<string>(ValueProperty); }
//        set { SetValue(ValueProperty, value); }
//    }

//    /// <summary>
//    /// Register the property so it is known in the class.
//    /// </summary>
//    public readonly PropertyData ValueProperty = RegisterProperty("Value", typeof(string), string.Empty);
//    
//    //    
//    //    /// <summary>
//    /// Retrieves the actual data from the serialization info.
//    /// </summary>
//    /// <param name="info"><see cref="SerializationInfo"/>.</param>
//    protected override void GetDataFromSerializationInfo(SerializationInfo info)
//    {
//        // Check if deserialization succeeded
//        if (DeserializationSucceeded) return;

//        // Perform any custom serialization here, or if you wish to 
//        // support older style serialization, you can do it here
//    }
//    //}
//
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Collections;

namespace GeoBase.Utils
{
    public class InvalidPropertyException : Exception
    {
        public InvalidPropertyException(string name)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' is invalid (not serializable?)",
                string.IsNullOrEmpty(name) ? "null reference property" : name)) { }
    }

    public class InvalidPropertyValueException : Exception
    {
        public InvalidPropertyValueException(string name, Type expectedType, Type actualType)
            : base(string.Format(CultureInfo.InvariantCulture, "Expected a value of type '{0} instead of '{1}' for property '{2}'",
                expectedType, actualType, name)) { }
    }

    public class PropertyAlreadyRegisteredException : Exception
    {
        public PropertyAlreadyRegisteredException(string name, Type type)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' is already registered on type '{1}'", name, type)) { }
    }

    public class PropertyNotRegisteredException : Exception
    {
        public PropertyNotRegisteredException(string name, Type type)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' is not registered on type '{1}'", name, type)) { }
    }

    public class PropertyNotNullableException : Exception
    {
        public PropertyNotNullableException(string name, Type type)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' on type '{1}' does not support null-values", name, type)) { }
    }

    public class PropertyData
    {

        internal PropertyData(string name, Type type, object defaultValue)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }

        private Type _type;
        public string Name { get; private set; }
        public Type Type
        {
            get { return (_type != null) ? _type : typeof(object); }
            private set { _type = value; }
        }
        private object DefaultValue { get; set; }

        public object GetDefaultValue()
        {
            return DefaultValue;
        }

        public T GetDefaultValue<T>()
        {
            return ((DefaultValue != null) && (DefaultValue is T)) ? (T)DefaultValue : default(T);
        }

        public object GetDefaultClonedValue()
        {
            if (DefaultValue is ICloneable)
                return (DefaultValue as ICloneable).Clone();
            return DefaultValue;
        }
    }

    public static class SerializationHelper
    {
        public static string GetString(SerializationInfo info, string name, string defaultValue)
        {
            return GetObject<string>(info, name, defaultValue);
        }

        public static int GetInt(SerializationInfo info, string name, int defaultValue)
        {
            return GetObject(info, name, defaultValue);
        }

        public static bool GetBool(SerializationInfo info, string name, bool defaultValue)
        {
            return GetObject(info, name, defaultValue);
        }

        public static T GetObject<T>(SerializationInfo info, string name, T defaultValue)
        {
            var type = typeof(T);
            var value = GetObject(info, name, type, defaultValue);
            return ((value != null) && (value is T)) ? (T)value : defaultValue;
        }

        public static object GetObject(SerializationInfo info, string name, Type type, object defaultValue)
        {
            try
            {
                var obj = info.GetValue(name, type);
                return (obj != null) ? obj : defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class RedirectTypeAttribute : Attribute
    {
        public RedirectTypeAttribute(string originalAssemblyName, string originalTypeName)
        {
            OriginalAssemblyName = originalAssemblyName;
            OriginalTypeName = originalTypeName;
        }

        public string OriginalAssemblyName { get; private set; }
        public string NewAssemblyName { get; set; }
        public string OriginalTypeName { get; private set; }
        public string NewTypeName { get; set; }
        public string OriginalType
        {
            get { return TypeHelper.FormatType(OriginalAssemblyName, OriginalTypeName); }
        }
        public string TypeToLoad
        {
            get
            {
                var assemblyToLoad = string.IsNullOrEmpty(NewAssemblyName) ? OriginalAssemblyName : NewAssemblyName;
                var typeToLoad = string.IsNullOrEmpty(NewTypeName) ? OriginalTypeName : NewTypeName;
                return TypeHelper.FormatType(assemblyToLoad, typeToLoad);
            }
        }
    }

    public static class TypeHelper
    {
        public static string FormatType(string assembly, string type)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", type, assembly);
        }

        public static string FormatInnerTypes(string[] innerTypes)
        {
            var result = string.Empty;
            for (var i = 0; i < innerTypes.Length; i++)
            {
                result += string.Format(CultureInfo.InvariantCulture, "[{0}]", innerTypes[i]);
                if (i < innerTypes.Length - 1) result += ",";
            }
            return result;
        }
    }

    public sealed class RedirectDeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        private readonly Dictionary<string, RedirectTypeAttribute> _redirectAttributes = new Dictionary<string, RedirectTypeAttribute>();

        public RedirectDeserializationBinder()
            : this(AppDomain.CurrentDomain) { }

        public RedirectDeserializationBinder(AppDomain appDomain)
        {
            Initialize(appDomain.GetAssemblies().Select(assembly => assembly.Location).ToArray());
        }

        public RedirectDeserializationBinder(string[] assemblies)
        {
            Initialize(assemblies);
        }

        private void Initialize(IEnumerable<string> assemblies)
        {
            var attributeType = typeof(RedirectTypeAttribute);
            foreach (string assemblyLocation in assemblies)
            {
                if (!File.Exists(assemblyLocation)) continue;
                var assembly = Assembly.LoadFile(assemblyLocation);
                if (assembly == null) continue;
                foreach (var type in assembly.GetTypes())
                {
                    InitializeAttributes(type, (RedirectTypeAttribute[])type.GetCustomAttributes(attributeType, true));
                    foreach (var member in type.GetMembers())
                    {
                        InitializeAttributes(member, (RedirectTypeAttribute[])member.GetCustomAttributes(attributeType, true));
                    }
                }
            }
        }

        private void InitializeAttributes(object decoratedObject, RedirectTypeAttribute[] attributes)
        {
            Type type = null;
            if (attributes.Length == 0) return;
            if (decoratedObject is Type)
            {
                type = decoratedObject as Type;
            }
            else if (decoratedObject is MemberInfo)
            {
            }
            foreach (var attribute in attributes)
            {
                if (type != null)
                {
                    var typeName = TypeHelper.FormatType(type.Assembly.FullName, type.FullName);
                    typeName = ConvertTypeToVersionIndependentType(typeName);
                    string finalTypeName, finalAssemblyName = string.Empty;
                    SplitType(typeName, out finalAssemblyName, out finalTypeName);
                    attribute.NewTypeName = finalTypeName;
                    attribute.NewAssemblyName = finalAssemblyName;
                }
                if (_redirectAttributes.ContainsKey(attribute.OriginalType))
                {
                    Trace.TraceWarning("A redirect for type '{0}' is already added to '{1}'. The redirect to '{2}' will not be added.",
                        attribute.OriginalType, _redirectAttributes[attribute.OriginalType].TypeToLoad, attribute.TypeToLoad);
                }
                else
                {
                    _redirectAttributes.Add(attribute.OriginalType, attribute);
                }
            }
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var currentType = TypeHelper.FormatType(assemblyName, typeName);
            var currentTypeVersionIndependent = ConvertTypeToVersionIndependentType(currentType);
            var newType = ConvertTypeToNewType(currentTypeVersionIndependent);
            var typeToDeserialize = LoadType(newType) ?? (LoadType(currentTypeVersionIndependent) ?? LoadType(currentType));
            return typeToDeserialize;
        }

        private static Type LoadType(string type)
        {
            try
            {
                return Type.GetType(type);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed to load type '{1}'.{0}{0}Additional details:{0}{2}", Environment.NewLine, type, ex.ToString());
            }
            return null;
        }

        private static void SplitType(string type, out string assemblyName, out string typeName)
        {
            var splitterPos = type.IndexOf(", ");
            typeName = (splitterPos != -1) ? type.Substring(0, splitterPos).Trim() : type;
            assemblyName = (splitterPos != -1) ? type.Substring(splitterPos + 1).Trim() : type;
        }

        private static string ConvertTypeToVersionIndependentType(string type)
        {
            const string InnerTypesEnd = ",";
            var newType = type;
            var innerTypes = GetInnerTypes(newType);
            if (innerTypes.Length > 0)
            {
                newType = newType.Replace(string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes)), string.Empty);
                for (var i = 0; i < innerTypes.Length; i++)
                {
                    innerTypes[i] = ConvertTypeToVersionIndependentType(innerTypes[i]);
                }
            }

            string typeName, assemblyName = string.Empty;
            SplitType(newType, out assemblyName, out typeName);
            var splitterPos = assemblyName.IndexOf(", ");
            if (splitterPos != -1) assemblyName = assemblyName.Substring(0, splitterPos);
            newType = TypeHelper.FormatType(assemblyName, typeName);
            if (innerTypes.Length > 0)
            {
                var innerTypesIndex = newType.IndexOf(InnerTypesEnd);
                if (innerTypesIndex >= 0)
                {
                    newType = newType.Insert(innerTypesIndex, string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes)));
                }
            }
            return newType;
        }

        private string ConvertTypeToNewType(string type)
        {
            const string InnerTypesEnd = ",";
            var newType = type;
            var innerTypes = GetInnerTypes(newType);
            if (innerTypes.Length > 0)
            {
                newType = newType.Replace(string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes)), string.Empty);
                for (var i = 0; i < innerTypes.Length; i++)
                {
                    innerTypes[i] = ConvertTypeToNewType(innerTypes[i]);
                }
            }
            if (_redirectAttributes.ContainsKey(newType))
            {
                newType = _redirectAttributes[newType].TypeToLoad;
            }
            if (innerTypes.Length > 0)
            {
                var innerTypesIndex = newType.IndexOf(InnerTypesEnd);
                if (innerTypesIndex >= 0)
                {
                    newType = newType.Insert(innerTypesIndex, string.Format(CultureInfo.InvariantCulture, "[{0}]", TypeHelper.FormatInnerTypes(innerTypes)));
                }
            }
            return newType;
        }

        private static string[] GetInnerTypes(string type)
        {
            const char InnerTypeCountStart = '`';
            const char InnerTypeCountEnd = '[';
            const string AllTypesStart = "[[";
            const char SingleTypeStart = '[';
            const char SingleTypeEnd = ']';
            var innerTypes = new List<string>();
            var innerTypeCount = 0;
            var countIndex = type.IndexOf(InnerTypeCountStart);
            if (countIndex == -1) return innerTypes.ToArray();
            if (!type.Contains(AllTypesStart)) return innerTypes.ToArray();
            var innerTypeCountEnd = type.IndexOf(InnerTypeCountEnd);
            innerTypeCount = int.Parse(type.Substring(countIndex + 1, innerTypeCountEnd - countIndex - 1));
            type = type.Substring(innerTypeCountEnd + 1);
            for (var i = 0; i < innerTypeCount; i++)
            {
                var innerTypeStart = type.IndexOf(SingleTypeStart);
                var innerTypeEnd = innerTypeStart + 1;
                var openings = 1;
                while (openings > 0)
                {
                    switch (type[innerTypeEnd])
                    {
                        case SingleTypeStart:
                            openings++;
                            break;
                        case SingleTypeEnd:
                            openings--;
                            break;
                    }

                    if (openings > 0) innerTypeEnd++;
                }
                innerTypes.Add(type.Substring(innerTypeStart + 1, innerTypeEnd - innerTypeStart - 1));
                type = type.Substring(innerTypeEnd + 1);
            }
            return innerTypes.ToArray();
        }
    }

    public enum SerializationMode
    {
        Xml,
        Binary
    }

    [Serializable]
    public abstract class DataObjectBase<T> : ISerializable, INotifyPropertyChanged, IDataErrorInfo, ICloneable, IDeserializationCallback, IEditableObject
    {
        private class BackupData
        {
            private const string IsDirty = "IsDirty";
            private DataObjectBase<T> _object = null;
            private Dictionary<string, object> _propertyValuesBackup = null;
            private Dictionary<string, object> _objectValuesBackup = null;
            public BackupData(DataObjectBase<T> obj)
            {
                _object = obj;
                CreateBackup();
            }
            private void CreateBackup()
            {
                using (var stream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, _object.ConvertDictionaryToList(_object._propertyValues));
                    stream.Seek(0, 0);
                    _propertyValuesBackup = _object.ConvertListToDictionary((List<KeyValuePair<string, object>>)binaryFormatter.Deserialize(stream));
                }
                _objectValuesBackup = new Dictionary<string, object>();
                _objectValuesBackup.Add(IsDirty, _object.IsDirty);
            }
            public void RestoreBackup()
            {
                foreach (var propertyValue in _propertyValuesBackup)
                {
                    _object.SetValue(propertyValue.Key, propertyValue.Value);
                }
                _object.IsDirty = (bool)_objectValuesBackup[IsDirty];
            }
        }

        protected Dictionary<string, PropertyData> _propertyInfo = new Dictionary<string, PropertyData>();
        protected Dictionary<string, object> _propertyValues = new Dictionary<string, object>();
        private BackupData _backup;
        private Dictionary<string, string> _fieldErrors = new Dictionary<string, string>();
        private List<string> _businessErrors = new List<string>();
        private readonly SerializationInfo _serializationInfo = null;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        private bool SubscribedToEvents { get; set; }
        private bool IsDeserializedDataAvailable { get; set; }
        private bool IsDeserialized { get; set; }
        private bool IsValidated { get; set; }
        protected bool AlwaysInvokeNotifyChanged { get; set; }
        public bool HandlePropertyAndCollectionChanges { get; set; }
        public bool AutomaticallyValidateOnPropertyChanged { get; set; }
        public SerializationMode Mode { get; set; }
        public bool IsDirty { get; protected set; }
        public bool HasErrors
        {
            get { return ((_fieldErrors.Count + _businessErrors.Count) > 0); }
        }

        public int FieldErrorCount
        {
            get { return _fieldErrors.Count; }
        }

        public int BusinessRuleErrorCount
        {
            get { return _businessErrors.Count; }
        }
        protected bool DeserializationSucceeded { get; private set; }

        public static T Load(string fileName)
        {
            return Load(fileName, SerializationMode.Binary);
        }

        public static T Load(string fileName, SerializationMode mode)
        {
            using (Stream stream = new FileStream(fileName, System.IO.FileMode.Open))
            {
                return Load(stream, mode);
            }
        }

        public static T Load(Stream stream)
        {
            return Load(stream, SerializationMode.Binary);
        }

        public static T Load(Stream stream, SerializationMode mode)
        {
            object result = null;
            switch (mode)
            {
                case SerializationMode.Binary:
                    var binaryDeserializationSucceeded = false;
                    var currentStreamPosition = stream.Position;
                    try
                    {
                        var binaryFormatter = new BinaryFormatter();
                        result = binaryFormatter.Deserialize(stream);
                        binaryDeserializationSucceeded = true;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Failed to deserialize the binary object. Trying with custom binder now.{0}{0}Details:{0}{1}", Environment.NewLine, ex.ToString());
                    }
                    if (!binaryDeserializationSucceeded)
                    {
                        var customBinaryFormatter = new BinaryFormatter();
                        customBinaryFormatter.Binder = new RedirectDeserializationBinder();
                        stream.Position = currentStreamPosition;
                        result = customBinaryFormatter.Deserialize(stream);
                    }
                    break;

                case SerializationMode.Xml:
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    result = xmlSerializer.Deserialize(stream);
                    break;
            }

            if ((result != null) && (result is DataObjectBase<T>))
            {
                ((DataObjectBase<T>)result).Mode = mode;
            }
            return (T)result;
        }

        public void Save(string fileName)
        {
            Save(fileName, Mode);
        }

        public void Save(string fileName, SerializationMode mode)
        {
            var fileInfo = new FileInfo(fileName);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
            using (Stream stream = new FileStream(fileName, FileMode.Create))
            {
                Save(stream, mode);
            }
        }

        public void Save(Stream stream)
        {
            Save(stream, Mode);
        }

        public void Save(Stream stream, SerializationMode mode)
        {
            switch (mode)
            {
                case SerializationMode.Binary:
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, this);
                    break;

                case SerializationMode.Xml:
                    var xmlSerializer = new XmlSerializer(GetType());
                    xmlSerializer.Serialize(stream, this);
                    break;
            }
            IsDirty = false;
        }

        protected static PropertyData RegisterProperty(string name, Type type, object defaultValue)
        {
            if (!type.IsSerializable) throw new InvalidPropertyException(name);
            var property = new PropertyData(name, type, defaultValue);
            return property;
        }

        private void InitializeProperties()
        {
            var type = GetType();
            var fields = new List<FieldInfo>();
            while (type.BaseType != typeof(object))
            {
                var typeFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                fields.AddRange(typeFields);
                type = type.BaseType;
            }
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(PropertyData)) continue;
                var propertyValue = ((field.IsStatic) ? field.GetValue(null) : field.GetValue(this)) as PropertyData;
                if (propertyValue != null)
                {
                    InitializeProperty(propertyValue);
                }
            }

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(PropertyData)) continue;
                var propertyValue = property.GetValue(null, null) as PropertyData;
                if (propertyValue != null)
                {
                    InitializeProperty(propertyValue);
                }
            }
        }

        private void InitializeProperty(PropertyData property)
        {
            InitializeProperty(property.Name, property.Type, property.GetDefaultValue());
        }

        protected void InitializeProperty(string name, Type type, object defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new InvalidPropertyException(name);
            if (IsPropertyRegistered(name)) throw new PropertyAlreadyRegisteredException(name, GetType());
            if ((defaultValue == null) && !IsTypeNullable(type)) throw new PropertyNotNullableException(name, GetType());
            var propertyData = new PropertyData(name, type, defaultValue);
            _propertyInfo.Add(name, propertyData);
            _propertyValues.Add(name, propertyData.GetDefaultClonedValue());
        }

        public virtual void SetDefaultValues()
        {
            foreach (var propertyData in _propertyInfo)
            {
                var value = propertyData.Value.GetDefaultClonedValue();
                SetValue(propertyData.Key, value);
            }
        }

        protected bool IsPropertyRegistered(string name)
        {
            return _propertyInfo.ContainsKey(name);
        }

        protected virtual void ValidateFields()
        { }

        protected virtual void ValidateBusinessRules()
        { }

        protected void SetValue(string name, object value)
        {
            SetValue(name, value, true);
        }

        private void SetValue(string name, object value, bool notifyOnChange)
        {
            PropertyData property;
            if (!IsPropertyRegistered(name)) throw new PropertyNotRegisteredException(name, GetType());
            lock (_propertyInfo)
            {
                property = _propertyInfo[name];
            }
            if ((value == null) && !IsTypeNullable(property.Type)) throw new PropertyNotNullableException(name, GetType());
            if ((value != null) && (!property.Type.IsAssignableFrom(value.GetType()))) throw new InvalidPropertyValueException(name, property.Type, value.GetType());
            lock (_propertyValues)
            {
                var notify = false;
                if (GetValue(name) != value)
                {
                    _propertyValues[name] = value;
                    IsValidated = false;
                    IsDirty = true;
                    notify = true;
                }
                if ((PropertyChanged != null) && notifyOnChange && (AlwaysInvokeNotifyChanged || notify))
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void SetValue(PropertyData property, object value)
        {
            if (property == null) throw new NullReferenceException("Property may not be null");
            SetValue(property.Name, value);
        }

        protected object GetValue(string name)
        {
            if (!IsPropertyRegistered(name)) throw new PropertyNotRegisteredException(name, GetType());
            lock (_propertyValues)
            {
                return _propertyValues[name];
            }
        }

        protected T1 GetValue<T1>(string name)
        {
            var obj = GetValue(name);
            return ((obj != null) && (obj is T1)) ? (T1)obj : default(T1);
        }

        protected object GetValue(PropertyData property)
        {
            if (property == null) throw new NullReferenceException("Property may not be null");
            return GetValue(property.Name);
        }

        protected T1 GetValue<T1>(PropertyData property)
        {
            var obj = GetValue(property);
            return ((obj != null) && (obj is T1)) ? (T1)obj : default(T1);
        }

        public object GetDefaultValue(string name)
        {
            if (!IsPropertyRegistered(name)) throw new PropertyNotRegisteredException(name, GetType());
            lock (_propertyValues)
            {
                return _propertyInfo[name].GetDefaultValue();
            }
        }

        public T1 GetDefaultValue<T1>(string name)
        {
            var obj = GetDefaultValue(name);
            return ((obj != null) && (obj is T1)) ? (T1)obj : default(T1);
        }

        public object GetDefaultValue(PropertyData property)
        {
            if (property == null) throw new NullReferenceException("Property may not be null");
            return GetDefaultValue(property.Name);
        }

        public T1 GetDefaultValue<T1>(PropertyData property)
        {
            var obj = GetDefaultValue(property);
            return ((obj != null) && (obj is T1)) ? (T1)obj : default(T1);
        }

        public Type GetType(string name)
        {
            if (!IsPropertyRegistered(name)) throw new PropertyNotRegisteredException(name, GetType());
            lock (_propertyValues)
            {
                return _propertyInfo[name].Type;
            }
        }

        public Type GetType(PropertyData property)
        {
            return GetType(property.Name);
        }

        private bool IsTypeNullable(Type type)
        {
            if (type == null) return false;
            if (!type.IsValueType) return true;
            return Nullable.GetUnderlyingType(type) != null;
        }

        public void Validate()
        {
            if (IsValidated) return;
            _fieldErrors = new Dictionary<string, string>();
            _businessErrors = new List<string>();
            ValidateFields();
            ValidateBusinessRules();
            IsValidated = true;
        }

        public void ValidateForce()
        {
            IsValidated = false;
            Validate();
        }

        protected void SetFieldError(PropertyData property, string error)
        {
            SetFieldError(property.Name, error);
        }

        protected void SetFieldError(string property, string error)
        {
            _fieldErrors[property] = error;
        }

        protected void SetBusinessRuleError(string error)
        {
            if (_businessErrors.Contains(error)) return;
            _businessErrors.Add(error);
        }

        public void SubscribeToEvents()
        {
            if (_propertyValues == null) return;
            if (SubscribedToEvents) return;
            foreach (var property in _propertyValues)
            {
                if (property.Value == null) continue;
                var objectType = GetType(property.Key);
                var propertyChangedValue = property.Value as INotifyPropertyChanged;
                if (propertyChangedValue != null)
                {
                    propertyChangedValue.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
                }
                var collectionChangedValue = property.Value as INotifyCollectionChanged;
                if (collectionChangedValue != null)
                {
                    collectionChangedValue.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                }
            }
            SubscribedToEvents = true;
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
            IsValidated = false;
            if (!HandlePropertyAndCollectionChanges) return;
            if (PropertyChanged != null) PropertyChanged(sender, e);
            if (AutomaticallyValidateOnPropertyChanged) Validate();
        }

        public virtual void OnReloadProperties()
        {
            IsDirty = true;
            IsValidated = false;
            if (!HandlePropertyAndCollectionChanges) return;
            foreach (KeyValuePair<string, object> property in _propertyValues)
            {
                OnPropertyChanged(this, new PropertyChangedEventArgs(property.Key));
            }
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
            IsValidated = false;
            if (!HandlePropertyAndCollectionChanges) return;
            foreach (var property in _propertyValues)
            {
                if ((property.Value == null) || (property.Value != sender)) continue;
                OnPropertyChanged(this, new PropertyChangedEventArgs(property.Key));
                return;
            }
        }

        public DataObjectBase(SerializationInfo info, StreamingContext context)
        {
            List<KeyValuePair<string, object>> properties = null;
            InitializeProperties();
            DeserializationSucceeded = false;
            HandlePropertyAndCollectionChanges = true;
            AutomaticallyValidateOnPropertyChanged = true;
            Mode = SerializationMode.Binary;
            IsDirty = false;
            if (info == null)
            {
                SubscribeToEvents();
                return;
            }
            _serializationInfo = info;
            properties = (List<KeyValuePair<string, object>>)SerializationHelper.GetObject(info, "Properties", typeof(List<KeyValuePair<string, object>>), new List<KeyValuePair<string, object>>());
            GetDataFromSerializationInfoInternal(_serializationInfo);
            DeserializationSucceeded = ((properties != null) && (properties.Count > 0));
        }

        protected virtual void GetDataFromSerializationInfo(SerializationInfo info) { }

        protected virtual void GetDataFromSerializationInfoInternal(SerializationInfo info)
        {
            List<KeyValuePair<string, object>> properties;
            if (IsDeserialized) return;
            if (!IsDeserializedDataAvailable) return;
            if (info == null) return;
            try
            {
                properties = (List<KeyValuePair<string, object>>)SerializationHelper.GetObject(info, "Properties", typeof(List<KeyValuePair<string, object>>), new List<KeyValuePair<string, object>>());
                var loadedDictionary = ConvertListToDictionary(properties);
                foreach (var dictionaryItem in loadedDictionary)
                {
                    _propertyValues[dictionaryItem.Key] = dictionaryItem.Value;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An error occurred while deserializing object '{1}'.{0}{0}Details:{0}'{2}'", Environment.NewLine, GetType().Name, ex.ToString());
            }
            GetDataFromSerializationInfo(info);
            IsDeserialized = true;
            try
            {
                SubscribeToEvents();
            }
            catch (Exception)
            {
                Trace.TraceWarning("Failed to subscribe to events in the OnDeserialized method");
            }
            IsDirty = false;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var properties = ConvertDictionaryToList(_propertyValues);
            info.AddValue("Properties", properties, typeof(List<KeyValuePair<string, object>>));
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            IsDeserializedDataAvailable = true;
            GetDataFromSerializationInfoInternal(_serializationInfo);
        }

        public void OnDeserialization(object sender)
        {
            try
            {
                foreach (var property in _propertyValues)
                {
                    CallOnDeserializationCallback(property.Value);
                    var collection = property.Value as ICollection;
                    if (collection == null) continue;
                    foreach (var item in collection)
                    {
                        CallOnDeserializationCallback(item);
                    }
                }
            }
            catch (Exception)
            {
                Trace.TraceWarning("Failed to call IDeserializationCallback.OnDeserialization for child objects");
            }
        }

        private void CallOnDeserializationCallback(object obj)
        {
            if (obj == null) return;
            var propertyCallback = obj as IDeserializationCallback;
            if (propertyCallback != null)
            {
                propertyCallback.OnDeserialization(this);
            }
        }

        private List<KeyValuePair<string, object>> ConvertDictionaryToList(Dictionary<string, object> dictionary)
        {
            return dictionary.Select(dictionaryItem => new KeyValuePair<string, object>(dictionaryItem.Key, dictionaryItem.Value)).ToList();
        }

        private Dictionary<string, object> ConvertListToDictionary(List<KeyValuePair<string, object>> list)
        {
            var result = new Dictionary<string, object>();
            foreach (var listItem in list)
            {
                if (!IsPropertyRegistered(listItem.Key)) continue;
                if (listItem.Value != null)
                {
                    if ((listItem.Value is ICollection) && (listItem.Value is IDeserializationCallback))
                    {
                        var propertyDeserializationCallback = listItem.Value as IDeserializationCallback;
                        propertyDeserializationCallback.OnDeserialization(this);
                    }
                }
                result[listItem.Key] = listItem.Value;
            }
            return result;
        }

        public object Clone()
        {
            Object clone = null;
            try
            {
                var stream = new MemoryStream();
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Seek(0, 0);
                clone = formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return clone;
        }

        string IDataErrorInfo.Error
        {
            get
            {
                var error = string.Empty;
                if (!IsValidated) Validate();
                if ((_businessErrors != null) && (_businessErrors.Count > 0))
                    error = _businessErrors[0];
                return error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                var error = string.Empty;
                if (!IsValidated) Validate();
                if ((_fieldErrors != null) && _fieldErrors.ContainsKey(columnName))
                {
                    error = _fieldErrors[columnName];
                }
                return error;
            }
        }

        public void BeginEdit()
        {
            if (_backup != null)
            {
                return;
            }
            _backup = new DataObjectBase<T>.BackupData(this);
        }

        public void CancelEdit()
        {
            if (_backup == null) return;
            _backup.RestoreBackup();
            _backup = null;
        }

        public void EndEdit()
        {
            _backup = null;
        }
    }
}
