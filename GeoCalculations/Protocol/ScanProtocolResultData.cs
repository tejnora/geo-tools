using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GeoBase.Utils;

namespace GeoCalculations.Protocol
{
    public static class ScanProtocolResultData
    {
        public const string RepeatedCountVariableName = "Count";
        public const string RepeatedItemVariableName = "Value";

        public enum PropertyTypes
        {
            Root,
            Node,
            Array,
            ArrayNode,
            RuntimeEvaluated,
            Method
        }

        public class PropertyContext
        {
            public PropertyContext()
            {
                Children = new List<PropertyContext>();
            }
            public PropertyContext(Type type, PropertyInfo propertyInfo, string name, PropertyTypes propertyType, ProtocolPropertyValueTypeAttribute.Types valueType = Protocol.ProtocolPropertyValueTypeAttribute.Types.Unknow)
            {
                PType = type;
                PropertyType = propertyType;
                Name = name;
                PropertyInfo = propertyInfo;
                Children = new List<PropertyContext>();
                ValueType = valueType;
            }

            public PropertyContext(Type type, MethodInfo methodInfo, string name, PropertyTypes propertyType, ProtocolPropertyValueTypeAttribute.Types valueType = Protocol.ProtocolPropertyValueTypeAttribute.Types.Unknow)
            {
                PType = type;
                PropertyType = propertyType;
                Name = name;
                MethodInfo = methodInfo;
                Children = new List<PropertyContext>();
                ValueType = valueType;
            }

            public PropertyContext(Type type, string name, PropertyTypes propertyType, ProtocolPropertyValueTypeAttribute.Types valueType = Protocol.ProtocolPropertyValueTypeAttribute.Types.Unknow)
            {
                PType = type;
                PropertyType = propertyType;
                Name = name;
                Children = new List<PropertyContext>();
                ValueType = valueType;
            }

            public Type PType { get; private set; }
            public string Name { get; private set; }
            public PropertyTypes PropertyType { get; private set; }
            public MethodInfo MethodInfo { get; private set; }
            public PropertyInfo PropertyInfo { get; private set; }
            public List<PropertyContext> Children { get; set; }
            public ProtocolPropertyValueTypeAttribute.Types ValueType { get; private set; }
        }

        static readonly HashSet<Assembly> Assemblies = new HashSet<Assembly>();
        static readonly Type ProtocolRootDataAttribute = typeof(ProtocolRootDataAttribute);
        static readonly Type ProtocolPropertyDataAttribute = typeof(ProtocolPropertyDataAttribute);
        static readonly Type ProtocolPropertyValueTypeAttribute = typeof(ProtocolPropertyValueTypeAttribute);
        static readonly Type ProtocolMethodAttribute = typeof(ProtocolMethodAttribute);
        public static IEnumerable<Type> GetCalculationResults()
        {
            if (!Assemblies.Any())
            {
                var userAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(AssemblyScanEvil.IsProbablyUserAssembly);
                foreach (var userAssembly in userAssemblies)
                {
                    Assemblies.Add(userAssembly);
                }
            }
            return FilterResultOfCalculation(Assemblies.SelectMany(a => a.GetExportedTypes()).ToList());
        }

        static IEnumerable<Type> FilterResultOfCalculation(IEnumerable<Type> types)
        {
            var typeList = new List<Type>();
            foreach (var type in types)
            {
                if (!type.GetCustomAttributes(ProtocolRootDataAttribute, true).Any()) continue;
                typeList.Add(type);
            }
            return typeList;
        }

        public static IEnumerable<Type> GetCalculationResults(Assembly assembly)
        {
            return FilterResultOfCalculation(assembly.GetExportedTypes()).ToList();
        }

        public static PropertyContext GetProperties(Type type)
        {
            var rootContext = new PropertyContext(type, "", PropertyTypes.Root);
            GetProperties(rootContext, type);
            return rootContext;
        }

        public static void GetProperties(PropertyContext parentContext, Type type)
        {
            if (GetIsListType(type))
            {
                parentContext.Children.Add(new PropertyContext(typeof(Int32), type.GetProperty("Count"), RepeatedCountVariableName, PropertyTypes.Node));
                if (IsSimpleEnumerableType(type))
                {
                    parentContext.Children.Add(new PropertyContext(type.GetGenericArguments().First(), RepeatedItemVariableName, PropertyTypes.ArrayNode));
                    return;
                }
                var genericArguments = type.GetGenericArguments();
                if (genericArguments.Count() > 1)
                    throw new ArgumentOutOfRangeException("Only one template argumen is supported.");
                type = type.GetGenericArguments().First();
            }
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var attribute = propertyInfo.GetCustomAttributes(ProtocolPropertyDataAttribute, true);
                if (attribute.Length != 1)
                    continue;
                var protocolAttribute = (ProtocolPropertyDataAttribute)attribute[0];
                attribute = propertyInfo.GetCustomAttributes(ProtocolPropertyValueTypeAttribute, true);
                var valueType = Protocol.ProtocolPropertyValueTypeAttribute.Types.Unknow;
                if (attribute.Length == 1)
                {
                    valueType = ((ProtocolPropertyValueTypeAttribute)attribute[0]).Type;
                }
                PropertyContext propContext;
                if (GetIsListType(propertyInfo.PropertyType) || !IsSimpleType(propertyInfo.PropertyType))
                {
                    propContext = new PropertyContext(propertyInfo.PropertyType, propertyInfo, protocolAttribute.Name, PropertyTypes.Array, valueType);
                    GetProperties(propContext, propertyInfo.PropertyType);
                }
                else
                {
                    propContext = new PropertyContext(propertyInfo.GetType(), propertyInfo, protocolAttribute.Name, PropertyTypes.Node, valueType);
                }
                parentContext.Children.Add(propContext);
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var methodInfo in methods)
            {
                var attribute = methodInfo.GetCustomAttributes(ProtocolMethodAttribute, true);
                if (attribute.Length != 1)
                    continue;
                var propContext = new PropertyContext(methodInfo.GetType(), methodInfo, methodInfo.Name, PropertyTypes.Method);
                parentContext.Children.Add(propContext);
            }
        }

        public static bool IsSimpleType(Type type)
        {
            return type.IsValueType || type.Name == "String";
        }

        public static bool IsSimpleEnumerableType(Type enumerableType)
        {
            var genericArguments = enumerableType.GetGenericArguments();
            if (genericArguments.Count() > 1)
                throw new ArgumentOutOfRangeException("Only one template argumen is supported.");
            var name = genericArguments.First().Name;
            return name == "String" || name == "Int32";
        }

        public static bool GetIsListType(Type type)
        {
            foreach (var intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IList<>))
                    return true;
            }
            return false;
        }
    }
}
