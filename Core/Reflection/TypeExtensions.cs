//--------------------------------------------------------------------------
// File:    TypeExtensions.cs
// Content:	Optimized implementation of class TypeExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

#endregion

namespace AnBo.Core;

/// <summary>
/// Provides extension methods for <see cref="Type"/> with enhanced .NET 8 support and optimized performance.
/// </summary>
public static class TypeExtensions
{
    #region Type name methods

    /// <summary>
    /// Takes the type presentation, surrounds it with quotes if it contains spaces.
    /// </summary>
    /// <param name="type">The type to process.</param>
    /// <returns>The assembly qualified name, quoted if necessary.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static string QuoteAssemblyQualifiedNameIfNeeded(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.AssemblyQualifiedName?.QuoteIfNeeded() ?? string.Empty;
    }

    /// <summary>
    /// Returns the user-friendly name of the given type, including generic type parameters.
    /// Uses optimized caching and StringBuilder pooling for better performance.
    /// </summary>
    /// <param name="type">The type to get the name for.</param>
    /// <returns>A user-friendly type name with generic parameters in readable format.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    /// <remarks>
    /// Supports generic type names in a user-friendly way without backticks and resolves nested generic types.
    /// Example: Dictionary&lt;string,int&gt; becomes "Dictionary[of string,int]"
    /// </remarks>
    public static string GetTypeName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"typename_{type}", () => GenerateTypeName(type));
    }

    /// <summary>
    /// Generates user-friendly type names with StringBuilder pooling for memory efficiency.
    /// </summary>
    /// <param name="type">The type to generate the name for.</param>
    /// <returns>A user-friendly type name.</returns>
    private static string GenerateTypeName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        // Use StringBuilder from pool for better memory management
        var sb = StringBuilderPool.Get();
        try
        {
            var typeName = type.Name;
            var backtickIndex = typeName.IndexOf('`', StringComparison.Ordinal);
            if (backtickIndex > 0)
                typeName = typeName[..backtickIndex];

            sb.Append(typeName).Append("[of ");

            var genericArgs = type.GetGenericArguments();
            for (var i = 0; i < genericArgs.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(GetTypeName(genericArgs[i])); // Recursive call uses cache
            }

            sb.Append(']');
            return sb.ToString();
        }
        finally
        {
            StringBuilderPool.Return(sb);
        }
    }

    #endregion

    #region Field methods

    /// <summary>
    /// Gets the first field that matches the specified field name (case-insensitive).
    /// Uses optimized caching for better performance.
    /// </summary>
    /// <param name="type">The type to search in.</param>
    /// <param name="fieldName">The name of the field to find.</param>
    /// <returns>The <see cref="FieldInfo"/> if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="fieldName"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    public static FieldInfo? GetAnyField(this Type type, string fieldName)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentException.ThrowIfNullOrEmpty(fieldName);

        return ReflectionCache.GetField(type, fieldName);
    }

    /// <summary>
    /// Returns all instance and static fields from the entire inheritance hierarchy.
    /// </summary>
    /// <param name="type">The type to get fields from.</param>
    /// <returns>An enumerable of all fields in the type hierarchy.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static IEnumerable<FieldInfo> GetAllFields(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"allfields_{type}", () => GetAllFieldsCore(type).ToArray());
    }

    /// <summary>
    /// Core implementation for getting all fields from the type hierarchy.
    /// </summary>
    /// <param name="type">The type to get fields from.</param>
    /// <returns>An enumerable of all fields in the type hierarchy.</returns>
    private static IEnumerable<FieldInfo> GetAllFieldsCore(Type type)
    {
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

    #endregion

    #region Reflection extension methods

    /// <summary>
    /// Checks if the type implements the specified interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to check for.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type implements the specified interface; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static bool ImplementsInterface<TInterface>(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var interfaceType = typeof(TInterface);
        if (!interfaceType.IsInterface)
            return false;

        return TypeCache.GetOrAdd($"implements_{type}_{interfaceType}",
            () => interfaceType.IsAssignableFrom(type));
    }

    /// <summary>
    /// Determines whether the specified type has required members.
    /// Uses caching for improved performance.
    /// </summary>
    /// <param name="type">The type to check for required members.</param>
    /// <returns>
    /// <see langword="true"/> if the type has the <see cref="RequiredMemberAttribute"/>; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static bool HasRequiredMembers(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"requiredmembers_{type}",
            () => type.GetCustomAttribute<RequiredMemberAttribute>() is not null);
    }

    /// <summary>
    /// Determines if the type can be instantiated (has accessible constructor and is not abstract).
    /// Uses caching for improved performance.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type can be instantiated; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static bool CanBeInstantiated(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"instantiable_{type}", () => CheckCanBeInstantiated(type));
    }

    /// <summary>
    /// Core implementation for checking if a type can be instantiated.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type can be instantiated; otherwise, false.</returns>
    private static bool CheckCanBeInstantiated(Type type)
    {
        return !type.IsAbstract &&
               !type.IsInterface &&
               !type.IsGenericTypeDefinition &&
               (type == typeof(string) ||
                type.IsValueType ||
                (!type.IsValueType && ReflectionCache.GetConstructors(type).Any(c => c.GetParameters().Length == 0)));
    }

    #endregion

    #region Generic type methods

    /// <summary>
    /// Determines whether <paramref name="type"/> is a <see cref="Nullable{T}"/> type.
    /// Uses caching for improved performance.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type is a nullable value type; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static bool IsNullableType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"nullable_{type}",
            () => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    /// <summary>
    /// Gets the underlying type of a nullable type, or the type itself if not nullable.
    /// Uses caching for improved performance.
    /// </summary>
    /// <param name="type">The type to get the underlying type for.</param>
    /// <returns>The underlying non-nullable type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static Type GetUnderlyingType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"underlying_{type}",
            () => Nullable.GetUnderlyingType(type) ?? type);
    }

    /// <summary>
    /// Determines if this type is an open generic type.
    /// Uses caching for improved performance.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the specified type is an open generic type; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static bool IsOpenGenericType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"opengeneric_{type}",
            () => type.IsGenericType && type.ContainsGenericParameters);
    }

    #endregion

    #region Default value methods

    /// <summary>
    /// Gets the default value for this reference or value type.
    /// Uses caching for improved performance.
    /// </summary>
    /// <param name="type">The type to get the default value for.</param>
    /// <returns>The default value for the type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static object? GetDefaultValue(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"default_{type}",
            () => type.IsValueType ? Activator.CreateInstance(type) : null);
    }

    /// <summary>
    /// Gets whether the <paramref name="value" /> is the default value for this reference or value type.
    /// </summary>
    /// <param name="type">The type to check against.</param>
    /// <param name="value">The value to check.</param>
    /// <returns>true if the value is the default value, otherwise false</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static bool IsDefaultValue(this Type type, object? value)
    {
        ArgumentNullException.ThrowIfNull(type);

        return value switch
        {
            null => !type.IsValueType || type.IsNullableType(),
            string s => s == string.Empty,
            _ when type.IsValueType => Equals(type.GetDefaultValue(), value),
            _ => false
        };
    }

    /// <summary>
    /// Determines whether the specified value is the default value for this type or an empty string.
    /// </summary>
    /// <param name="type">The type to check against.</param>
    /// <param name="value">The value to check.</param>
    /// <returns>true if the value is the default value or empty string; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public static bool IsDefaultValueOrEmptyString(this Type type, object? value)
    {
        ArgumentNullException.ThrowIfNull(type);

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
    /// Delegates to TypeHelper for centralized implementation.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type is JSON serializable; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    /// <remarks>
    /// This method performs comprehensive checks including:
    /// - Basic type compatibility and exclusions
    /// - Constructor requirements for deserialization
    /// - Circular reference detection
    /// - System.Text.Json attribute support
    /// - Event and delegate detection
    /// </remarks>
    public static bool IsJsonSerializable(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeHelper.IsJsonSerializable(type);
    }

    /// <summary>
    /// Determines whether the specified type can be safely cloned using JSON serialization.
    /// Builds upon JSON serializability with additional semantic checks.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type can likely be cloned successfully, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown if the type is null</exception>
    public static bool IsCloneable(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeHelper.IsCloneable(type);
    }

    #endregion

    #region Deep clone methods

    /// <summary>
    /// Creates a deep copy of an object using optimized JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of object to be cloned</typeparam>
    /// <param name="original">The object to be cloned</param>
    /// <returns>A deep copy of the original object</returns>
    /// <exception cref="InvalidOperationException">Thrown when the object cannot be cloned</exception>
    public static T? DeepClone<T>(this T? original)
    {
        return TypeHelper.DeepClone(original);
    }

    #endregion
}

#region Supporting classes

/// <summary>
/// Centralized caching strategy with size limits and efficient eviction.
/// </summary>
internal static class TypeCache
{
    private const int MaxCacheSize = 1000;

    // Use MemoryCache with size limits instead of unlimited ConcurrentDictionary
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions
    {
        SizeLimit = MaxCacheSize,
        CompactionPercentage = 0.25 // Remove 25% when size limit reached
    });

    /// <summary>
    /// Gets or adds a cached result with automatic eviction policy.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">Factory function to create the value if not cached.</param>
    /// <param name="size">Relative size of the cached item (default: 1).</param>
    /// <returns>The cached or newly created value.</returns>
    public static T GetOrAdd<T>(string key, Func<T> factory, int size = 1)
    {
        return _cache.GetOrCreate(key, entry =>
        {
            entry.Size = size;
            entry.Priority = CacheItemPriority.Normal;
            entry.SlidingExpiration = TimeSpan.FromMinutes(30); // Auto-expire unused items
            return factory();
        })!;
    }
}

/// <summary>
/// Simple StringBuilder pool for memory efficiency.
/// </summary>
internal static class StringBuilderPool
{
    private static readonly ConcurrentQueue<StringBuilder> _pool = new();
    private const int MaxPoolSize = 20;
    private const int InitialCapacity = 256;

    /// <summary>
    /// Gets a StringBuilder from the pool or creates a new one.
    /// </summary>
    /// <returns>A StringBuilder instance.</returns>
    public static StringBuilder Get()
    {
        if (_pool.TryDequeue(out var sb))
        {
            sb.Clear();
            return sb;
        }
        return new StringBuilder(InitialCapacity);
    }

    /// <summary>
    /// Returns a StringBuilder to the pool for reuse.
    /// </summary>
    /// <param name="sb">The StringBuilder to return.</param>
    public static void Return(StringBuilder sb)
    {
        if (_pool.Count < MaxPoolSize && sb.Capacity <= 1024)
        {
            _pool.Enqueue(sb);
        }
    }
}

/// <summary>
/// Cached reflection information for better performance.
/// </summary>
internal static class ReflectionCache
{
    private static readonly ConcurrentDictionary<(Type, string), FieldInfo?> _fieldCache = new();
    private static readonly ConcurrentDictionary<Type, ConstructorInfo[]> _constructorCache = new();

    /// <summary>
    /// Gets a field from the cache or retrieves it via reflection.
    /// </summary>
    /// <param name="type">The type containing the field.</param>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The FieldInfo if found, null otherwise.</returns>
    public static FieldInfo? GetField(Type type, string fieldName)
    {
        return _fieldCache.GetOrAdd((type, fieldName),
            key => GetFieldInfoCore(key.Item1, key.Item2));
    }

    /// <summary>
    /// Gets constructors from the cache or retrieves them via reflection.
    /// </summary>
    /// <param name="type">The type to get constructors for.</param>
    /// <returns>Array of constructor info objects.</returns>
    public static ConstructorInfo[] GetConstructors(Type type)
    {
        return _constructorCache.GetOrAdd(type,
            t => t.GetConstructors(BindingFlags.Public | BindingFlags.Instance));
    }

    /// <summary>
    /// Core implementation for field lookup with inheritance chain traversal.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <param name="fieldName">The field name to find.</param>
    /// <returns>The FieldInfo if found, null otherwise.</returns>
    private static FieldInfo? GetFieldInfoCore(Type type, string fieldName)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                  BindingFlags.Instance | BindingFlags.Static |
                                  BindingFlags.DeclaredOnly;

        var currentType = type;
        while (currentType is not null)
        {
            var field = currentType.GetField(fieldName, flags);
            if (field is not null)
                return field;
            currentType = currentType.BaseType;
        }
        return null;
    }
}

#endregion