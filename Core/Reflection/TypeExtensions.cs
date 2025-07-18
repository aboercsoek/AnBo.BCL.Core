//--------------------------------------------------------------------------
// File:    TypeExtensions.cs
// Content:	Implementation of class TypeExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

#endregion

namespace AnBo.Core
{
    ///<summary>Provides extension methods for <see cref="Type"/>.</summary>
	public static class TypeExtensions
    {
        
        #region Type name methods

        /// <summary>
        /// Takes the type presentation, surrounds it with quotes if it contains spaces.
        /// </summary>
        public static string QuoteAssemblyQualifiedNameIfNeeded(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return type.AssemblyQualifiedName.QuoteIfNeeded();
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

        #endregion

        #region Field methods

        /// <summary>
        /// Gets the the first field that meet the fieldName condition.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Returns the field info witch first match the fieldname condition.
        /// </returns>
        public static FieldInfo? GetAnyField(this Type type, string? fieldName)
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
                var fields = currentType.GetFields(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.IgnoreCase |
                    BindingFlags.DeclaredOnly);

                foreach (var fieldInfo in fields)
                {
                    yield return fieldInfo;
                }

                currentType = currentType.BaseType;
            }
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

            var fields = type.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.IgnoreCase |
                BindingFlags.DeclaredOnly);

            foreach(var field in fields)
            {
                if (field.Name.AsSpan().Equals(fieldName.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return field;
                }
            }

            // If not found in the current type, check the base type
            return GetFieldInfo(type.BaseType, fieldName);
        }

        #endregion

        #region Reflection extension methods

        /// <summary>
        /// Checks if the type implements the specified interface.
        /// </summary>
        /// <param name="type"> The type to check.</param>
        /// <returns>true if the type implements the specified interface; otherwise, false.</returns>
        /// <exception cref="ArgNullException">Is thrown if <paramref name="type"/> is <see langword="null"/></exception>
        public static bool ImplementsInterface<TInterface>(this Type? type)
        {
            ArgChecker.ShouldNotBeNull(type, "type");
            return typeof(TInterface).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the specified type has required members.
        /// </summary>
        /// <param name="type">The type to check for required members.</param>
        /// <returns>
        /// <see langword="true"/> if the type has the <see cref="RequiredMemberAttribute"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasRequiredMembers(this Type? type)
        {
            if (type == null)
                return false;

            return type.GetCustomAttribute<RequiredMemberAttribute>() != null;
        }


        #endregion

        #region Generic type methods


        private static readonly ConcurrentDictionary<Type, bool> _nullableTypeCache = new();

        /// <summary>
        /// Determines whether <paramref name="type"/> is a <see cref="Nullable{type}"/> type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if <paramref name="type"/> is a <see cref="Nullable{type}"/> type; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsNullableType(this Type type)
        {
            return _nullableTypeCache.GetOrAdd(type, t =>
                t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
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

        #endregion
        
        #region Default value methods

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
        /// <returns>true if the value is the default value, othewise false</returns>
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

            if (value is string s)
            {
                return (s == string.Empty);
            }
            return Equals(Activator.CreateInstance(type), value);
        }

        #endregion

        #region Serialization methods

        /// <summary>
        /// Checks if type is JSON serializable
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>true if the type is JSON serializable, otherwise false</returns>
        public static bool IsJsonSerializable(this Type type)
        {
            // Basis-Ausschlüsse
            if (type.IsPointer || type.IsByRef)
                return false;

            // Primitive Typen und Strings sind immer serialisierbar
            if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime))
                return true;

            // Nullable-Typen
            if (Nullable.GetUnderlyingType(type) != null)
                return IsJsonSerializable(Nullable.GetUnderlyingType(type)!);

            // Collections
            if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
                return true; // Vereinfacht - sollte Element-Typ prüfen

            // Klassen brauchen parameterlosen Konstruktor
            if (type.IsClass)
                return type.GetConstructor(Type.EmptyTypes) != null;

            // Structs sind meist serialisierbar
            return type.IsValueType;
        }

        #endregion

        #region Deep clone methods

        /// <summary>
        /// Creates a deep copy of an object using JSON serialization
        /// </summary>
        /// <typeparam name="T">The type of object to be cloned</typeparam>
        /// <param name="original">The object to be cloned</param>
        /// <returns>A deep copy of the original object</returns>
        public static T? DeepClone<T>(this T original)
        {
            return TypeHelper.DeepClone(original);
        }

        #endregion
    }
}
