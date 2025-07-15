//--------------------------------------------------------------------------
// File:    TypeExtensions.cs
// Content:	Implementation of class TypeExtensions
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml.Serialization;

#endregion

namespace AnBo.Core
{
    ///<summary>Provides extension methods for <see cref="Type"/>.</summary>
	public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether <paramref name="type"/> is a <see cref="Nullable{type}"/> type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if <paramref name="type"/> is a <see cref="Nullable{type}"/> type; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNullableType(this Type type)
        {
            //http://msdn.microsoft.com/en-us/library/ms366789.aspx
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));

        }

        /// <summary>
        /// Returns the name of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Returns the type name value.
        /// </returns>
        /// <remarks>Supports generic type names in a user friendly way without the '-signs and also resolves nested generic type names.</remarks>
        public static string GetTypeName(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type, "type");

            if (type.IsGenericType)
            {

                var argNames = type
                    .GetGenericArguments()
                    .Select(GetTypeName)
                    .ToArray();

                string args = string.Join(",", argNames);

                string typeName = type.Name;
                int index = typeName.IndexOf("`");
                typeName = typeName.Substring(0, index);

                return string.Format("{0}[of {1}]", typeName, args);
            }
            return type.Name;
        }

        /// <summary>
        /// Gets the the first field that meet the fieldName condition.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns the field info witch first match the fieldname condition.
        /// </returns>
        public static FieldInfo? GetAnyField(this Type type, string fieldName)
        {
            ArgChecker.ShouldNotBeNull(type, "type");
            ArgChecker.ShouldNotBeNullOrEmpty(fieldName, "fieldName");

            return GetFieldInfo(type, fieldName);
        }

        /// <summary>
        /// Returns the enumerable collection of all instance fields in a type (only field that are declared in that type).
        /// </summary>
        /// <param name="type">The type to get the fields from</param>
        /// <returns>The fields of the specified type.</returns>
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type, "type");

            Type? currentType = type;
            while (currentType != null)
            {
                FieldInfo[] fields =
                    currentType.GetFields(BindingFlags.Public |
                                            BindingFlags.NonPublic |
                                            BindingFlags.Instance |
                                            BindingFlags.Static |
                                            BindingFlags.IgnoreCase |
                                            BindingFlags.DeclaredOnly);
                foreach (FieldInfo fieldInfo in fields)
                {
                    yield return fieldInfo;
                }
                currentType = currentType.BaseType;
            }
        }

        /// <summary>
        /// Is this type an open generic type
        /// </summary>
        /// <returns><see langword="true"/> if the specified type is an open generic type, otherwise <see langword="false"/></returns>
        public static bool IsOpenGenericType(this Type type)
        {
            if (type == null)
                return false;

            return (type.IsGenericType && type.ContainsGenericParameters);
        }

        /// <summary>
        /// Gets the field info of a field inside Type type that matches the given fieldName.
        /// </summary>
        /// <param name="type">The type to search for the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns the first field info value that matches the fieldname.
        /// </returns>
        /// <remarks>Begins the search in the provided type and walks if not found the inheritance tree up. 
        /// Returns null if the fieldName was not found in type nor in the inheritance tree.</remarks>
        private static FieldInfo? GetFieldInfo(Type? type, string fieldName)
        {
            if (type == null)
                return null;

            if (fieldName.IsNullOrEmptyWithTrim())
                return null;

            var allFields = from f in type.GetFields(BindingFlags.Public |
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.Instance |
                                                     BindingFlags.Static |
                                                     BindingFlags.IgnoreCase |
                                                     BindingFlags.DeclaredOnly)
                            where f.Name.ToLowerInvariant() == fieldName!.ToLowerInvariant()
                            select f;

            FieldInfo? field = allFields.FirstOrDefault();
            if (field != null)
                return field;

            return GetFieldInfo(type.BaseType, fieldName);
        }

        /// <summary>
        /// Gets the default value for this reference or value type.
        /// </summary>
        public static object? GetDefaultValue(this Type type)
        {
            return type.IsValueType.IsFalse() ? null : Activator.CreateInstance(type);
        }


        /// <summary>
        /// Gets whether the <paramref name="value" /> is the default value for this reference or value type.
        /// </summary>
        public static bool IsDefaultValue(this Type type, object value)
        {
            if (ReferenceEquals(value, null))
            {
                return true;
            }
            return type.IsValueType && Equals(Activator.CreateInstance(type), value);
        }

        /// <summary>
        /// Gets whether the <paramref name="value" /> is the default value for this reference or value type, or an empty string.
        /// </summary>
        public static bool IsDefaultValueOrEmptyString(this Type type, object value)
        {
            if (ReferenceEquals(value, null))
            {
                return true;
            }

            if (type.IsValueType.IsFalse())
            {
                return ((value as string) == string.Empty);
            }
            return Equals(Activator.CreateInstance(type), value);
        }

        /// <summary>
        /// Takes the type presentation, surrounds it with quotes if it contains spaces.
        /// </summary>
        public static string QuoteIfNeeded(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type, "type");

            return type.AssemblyQualifiedName.QuoteIfNeeded();
        }


        /// <summary>
        /// Determines whether the specified type is a data contract.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified type is a data contract; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsDataContract(this Type type)
        {
            return (type == null) ? false : type.HasAttribute<DataContractAttribute>(false);
        }

        /// <summary>
        /// Determines whether the specified type has a XmlRoot attribute.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified type  has a XmlRoot attribute; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasXmlRootAttribute(this Type type)
        {
            return (type == null) ? false : type.HasAttribute<XmlRootAttribute>(false);
        }

        /// <summary>
        /// Determines whether the specified type has a XmlElement attribute.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified type has a XmlElement attribute; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasXmlElementAttribute(this Type type)
        {
            return (type == null) ? false : type.HasAttribute<XmlElementAttribute>(false);
        }

        /// <summary>
        /// Determines whether the specified type has a XmlEnum attribute.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified type has a XmlEnum attribute; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasXmlEnumAttribute(this Type type)
        {
            return (type == null) ? false : type.HasAttribute<XmlEnumAttribute>(false);
        }

        /// <summary>
        /// Determines whether the specified type has a XmlArray attribute.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified type has a XmlArray attribute; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasXmlArrayAttribute(this Type type)
        {
            return (type == null) ? false : type.HasAttribute<XmlArrayAttribute>(false);
        }

        /// <summary>
        /// Determines whether the specified type has a XmlArrayItem attribute.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified type has a XmlArrayItem attribute; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasXmlArrayItemAttribute(this Type type)
        {
            return (type == null) ? false : type.HasAttribute<XmlArrayItemAttribute>(false);
        }

        /// <summary>
        /// Determines whether the specified type has a XmlAttribute attribute.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified type has a XmlAttribute attribute; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasXmlAttributeAttribute(this Type type)
        {
            return (type == null) ? false : type.HasAttribute<XmlAttributeAttribute>(false);
        }

        // ...

        public static T? DeepClone<T>(this T original)
        {
            if (typeof(T).IsValueType == false)
            {
                if (Equals(original, default(T))) return default;
            }

            // Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(original);

            // Deserialize the JSON back to a new object
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
