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
using System.Text;
using System.Text.Json.Serialization;

#endregion

namespace AnBo.Core
{
    /// <summary>
    /// Provides extension methods for <see cref="Type"/> with enhanced .NET 8 support.
    /// </summary>
    public static class TypeExtensions
    {
        #region Private fields and caches

        private static readonly ConcurrentDictionary<Type, bool> _nullableTypeCache = new();
        private static readonly ConcurrentDictionary<Type, bool> _jsonSerializableCache = new();
        private static readonly ConcurrentDictionary<Type, string> _typeNameCache = new();
        private static readonly ConcurrentDictionary<Type, object?> _defaultValueCache = new();

        #endregion

        #region Type name methods

        /// <summary>
        /// Takes the type presentation, surrounds it with quotes if it contains spaces.
        /// </summary>
        /// <param name="type">The type to process.</param>
        /// <returns>The assembly qualified name, quoted if necessary.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static string QuoteAssemblyQualifiedNameIfNeeded(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return type.AssemblyQualifiedName?.QuoteIfNeeded() ?? string.Empty;
        }

        /// <summary>
        /// Returns the user-friendly name of the given type, including generic type parameters.
        /// </summary>
        /// <param name="type">The type to get the name for.</param>
        /// <returns>A user-friendly type name with generic parameters in readable format.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        /// <remarks>
        /// Supports generic type names in a user-friendly way without backticks and resolves nested generic types.
        /// Example: Dictionary&lt;string,int&gt; becomes "Dictionary[of string,int]"
        /// </remarks>
        public static string GetTypeName(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            // Check cache first for performance
            return _typeNameCache.GetOrAdd(type, static t =>
            {
                // If type is not generic, return its name directly
                if (!t.IsGenericType)
                    return t.Name;

                // Handle generic types with parameters
                var sb = new StringBuilder();
                var typeName = t.Name;

                // Get the type name without the generic parameter backtick notation
                var backtickIndex = typeName.IndexOf('`');
                if (backtickIndex > 0)
                    typeName = typeName[..backtickIndex];

                sb.Append(typeName);
                sb.Append("[of ");

                // Append each generic argument
                var genericArgs = t.GetGenericArguments();
                for (var i = 0; i < genericArgs.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(GetTypeName(genericArgs[i]));
                }

                sb.Append(']');
                return sb.ToString();
            });
        }

        #endregion

        #region Field methods

        /// <summary>
        /// Gets the first field that matches the specified field name (case-insensitive).
        /// </summary>
        /// <param name="type">The type to search in.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The <see cref="FieldInfo"/> if found; otherwise, null.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> or <paramref name="fieldName"/> is null.</exception>
        /// <exception cref="ArgEmptyException">Thrown when <paramref name="fieldName"/> is empty.</exception>
        public static FieldInfo? GetAnyField(this Type type, string fieldName)
        {
            ArgChecker.ShouldNotBeNull(type);
            ArgChecker.ShouldNotBeNullOrEmpty(fieldName);

            return GetFieldInfo(type, fieldName);
        }

        /// <summary>
        /// Returns all instance and static fields from the entire inheritance hierarchy.
        /// </summary>
        /// <param name="type">The type to get fields from.</param>
        /// <returns>An enumerable of all fields in the type hierarchy.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Instance | BindingFlags.Static |
                                         BindingFlags.DeclaredOnly;

            var currentType = type;
            while (currentType is not null)
            {
                foreach (var field in currentType.GetFields(bindingFlags))
                {
                    yield return field;
                }
                currentType = currentType.BaseType;
            }
        }

        #region Private Field helper methods

        /// <summary>
        /// Gets the field info of a field inside the type hierarchy that matches the given field name.
        /// </summary>
        /// <param name="type">The type to search for the field.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>The first field info that matches the field name; otherwise, null.</returns>
        /// <remarks>
        /// Searches in the provided type and walks up the inheritance tree if not found.
        /// Uses case-insensitive comparison.
        /// </remarks>
        private static FieldInfo? GetFieldInfo(Type? type, string fieldName)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Instance | BindingFlags.Static |
                                         BindingFlags.DeclaredOnly;
            while (type is not null)
            {
                var fields = type.GetFields(bindingFlags);

                foreach (var field in fields)
                {
                    if (field.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                        return field;
                }

                type = type.BaseType;
            }

            return null;
        }

        #endregion

        #endregion

        #region Reflection extension methods

        /// <summary>
        /// Checks if the type implements the specified interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to check for.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type implements the specified interface; otherwise, false.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static bool ImplementsInterface<TInterface>(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            // if TInterface is not an interface, return false
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                return false;

            return typeof(TInterface).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the specified type has required members.
        /// </summary>
        /// <param name="type">The type to check for required members.</param>
        /// <returns>
        /// <see langword="true"/> if the type has the <see cref="RequiredMemberAttribute"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static bool HasRequiredMembers(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return type.GetCustomAttribute<RequiredMemberAttribute>() is not null;
        }

        /// <summary>
        /// Determines if the type can be instantiated (has accessible constructor and is not abstract).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type can be instantiated; otherwise, false.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static bool CanBeInstantiated(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return !type.IsAbstract &&
                   !type.IsInterface &&
                   !type.IsGenericTypeDefinition &&
                   (type == typeof(string) || type.IsValueType || (!type.IsValueType && type.GetConstructor(Type.EmptyTypes) is not null));
        }


        #endregion

        #region Generic type methods

        /// <summary>
        /// Determines whether <paramref name="type"/> is a <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type is a nullable value type; otherwise, false.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static bool IsNullableType(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return _nullableTypeCache.GetOrAdd(type, 
                static t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Gets the underlying type of a nullable type, or the type itself if not nullable.
        /// </summary>
        /// <param name="type">The type to get the underlying type for.</param>
        /// <returns>The underlying non-nullable type.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static Type GetUnderlyingType(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Determines if this type is an open generic type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the specified type is an open generic type; otherwise, false.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static bool IsOpenGenericType(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            //if (type == null)
            //    return false;

            return (type.IsGenericType && type.ContainsGenericParameters);
        }

        #endregion
        
        #region Default value methods

        /// <summary>
        /// Gets the default value for this reference or value type.
        /// </summary>
        public static object? GetDefaultValue(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return _defaultValueCache.GetOrAdd(type, 
                static t => t.IsValueType ? Activator.CreateInstance(t) : null);
        }


        /// <summary>
        /// Gets whether the <paramref name="value" /> is the default value for this reference or value type.
        /// </summary>
        /// <returns>true if the value is the default value, othewise false</returns>
        public static bool IsDefaultValue(this Type type, object? value)
        {
            ArgChecker.ShouldNotBeNull(type);

            return value switch
            {
                null => !type.IsValueType || type.IsNullableType(), // null is default for reference types an nullable value types
                string s => s == string.Empty, // empty string is default for strings
                _ when type.IsValueType => Equals(type.GetDefaultValue(), value), // compare with default instance of value type
                _ => false // otherwise, it's not default
            };
        }

        /// <summary>
        /// Determines whether the specified value is the default value for this type or an empty string.
        /// </summary>
        /// <param name="type">The type to check against.</param>
        /// <param name="value">The value to check.</param>
        /// <returns>true if the value is the default value or empty string; otherwise, false.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        public static bool IsDefaultValueOrEmptyString(this Type type, object? value)
        {
            ArgChecker.ShouldNotBeNull(type);

            return value switch
            {
                null => true,
                string s => string.IsNullOrEmpty(s),
                _ => type.IsDefaultValue(value)
            };
        }

        #endregion

        #region Serialization methods

        /// <summary>
        /// Determines if the type is suitable for JSON serialization with System.Text.Json.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type is JSON serializable; otherwise, false.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="type"/> is null.</exception>
        /// <remarks>
        /// This method performs comprehensive checks including:
        /// - Basic type compatibility
        /// - Constructor requirements
        /// - Circular reference potential
        /// - System.Text.Json attribute support
        /// </remarks>
        public static bool IsJsonSerializable(this Type type)
        {
            ArgChecker.ShouldNotBeNull(type);

            return _jsonSerializableCache.GetOrAdd(type, static t => CheckJsonSerializability(t));
        }


        #region Private JSON serializability check

        /// <summary>
        /// Performs detailed JSON serializability check for the given type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if JSON serializable; otherwise, false.</returns>
        private static bool CheckJsonSerializability(Type type)
        {
            // Basic exclusions
            if (type.IsPointer || type.IsByRef || type == typeof(nint) || type == typeof(nuint))
                return false;

            // Primitive types and common types
            if (type.IsPrimitive ||
                type == typeof(string) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid) ||
                type == typeof(Uri) ||
                type.IsEnum)
                return true;

            // Nullable types
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
                return CheckJsonSerializability(underlyingType);

            // Collections and arrays
            if (type.IsArray)
                return CheckJsonSerializability(type.GetElementType()!);

            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            {
                if (type.IsGenericType)
                {
                    var genericArgs = type.GetGenericArguments();
                    return genericArgs.All(CheckJsonSerializability);
                }
                return true; // Non-generic collections are generally serializable
            }

            // Record types (often serializable)
            if (IsRecord(type))
                return true;

            // Classes need parameterless constructor or constructor with JsonConstructor attribute
            if (type.IsClass)
            {
                var hasParameterlessConstructor = type.GetConstructor(Type.EmptyTypes) is not null;
                var hasJsonConstructor = type.GetConstructors()
                    .Any(c => c.GetCustomAttribute<JsonConstructorAttribute>() is not null);

                return hasParameterlessConstructor || hasJsonConstructor;
            }

            // Value types (structs) are generally serializable
            return type.IsValueType;
        }

        /// <summary>
        /// Determines if the type is a record type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type is a record; otherwise, false.</returns>
        private static bool IsRecord(Type type)
        {
            // Records have a synthesized method with specific characteristics
            return type.GetMethod("<Clone>$") is not null ||
                   type.GetCustomAttributes().Any(attr =>
                       attr.GetType().Name.Contains("CompilerGenerated"));
        }

        #endregion

        #endregion

        #region Deep clone methods

        /// <summary>
        /// Creates a deep copy of an object using JSON serialization
        /// </summary>
        /// <typeparam name="T">The type of object to be cloned</typeparam>
        /// <param name="original">The object to be cloned</param>
        /// <returns>A deep copy of the original object</returns>
        public static T? DeepClone<T>(this T? original)
        {
            return TypeHelper.DeepClone(original);
        }

        #endregion
    }
}
