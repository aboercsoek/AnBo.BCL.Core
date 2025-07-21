//--------------------------------------------------------------------------
// File:    TypeHelper.cs
// Content:	Implementation of class TypeHelper
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace AnBo.Core;

/// <summary>
/// Provides common type helper methods for object manipulation, cloning, and disposal.
/// </summary>
public static class TypeHelper
{
    #region Private fields and configuration

    // Thread-safe cache for JsonSerializerOptions per type
    private static readonly ConcurrentDictionary<Type, JsonSerializerOptions> _jsonOptionsCache = new();

    // Default configuration for JSON serialization
    private static readonly JsonSerializerOptions _defaultJsonOptions = CreateDefaultJsonOptions();

    /// <summary>
    /// Creates the default JSON serializer options for deep cloning operations.
    /// </summary>
    /// <returns>Configured JsonSerializerOptions instance</returns>
    private static JsonSerializerOptions CreateDefaultJsonOptions()
    {
        return new JsonSerializerOptions
        {
            // Allows reference cycles and preserves object references
            ReferenceHandler = ReferenceHandler.Preserve,

            // Includes fields (not just properties)
            IncludeFields = true,

            // Does not ignore read-only properties
            IgnoreReadOnlyProperties = false,

            // Handles numbers as strings when necessary
            NumberHandling = JsonNumberHandling.AllowReadingFromString,

            // Allows comments in JSON
            ReadCommentHandling = JsonCommentHandling.Skip,

            // Handles unknown properties
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,

            // Configures case sensitivity
            PropertyNamingPolicy = null,

            // Allows trailing commas
            AllowTrailingCommas = true,

            // Improves performance
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };
    }

    #endregion

    #region DeepClone helper methods

    /// <summary>
    /// Creates a deep copy of an object using JSON serialization.
    /// This method is suitable for most serializable objects but may not work with
    /// objects containing delegates, events, or other non-serializable members.
    /// </summary>
    /// <param name="original">The object to be cloned</param>
    /// <param name="type">The type of object to be cloned. If null, uses the runtime type of the object</param>
    /// <returns>A deep copy of the original object, or null if the original was null</returns>
    /// <exception cref="InvalidOperationException">Thrown when the object cannot be cloned due to serialization errors</exception>
    /// <exception cref="ArgException">Thrown when the provided type is incompatible with the object</exception>
    public static object? DeepClone(object? original, Type type)
    {
        if (original is null)
            return null;

        var targetType = type ?? original.GetType();

        // Validate type compatibility
        if (type is not null && !targetType.IsAssignableFrom(original.GetType()))
        {
            throw new ArgException<Type>(type,
                nameof(type),
                $"The provided type '{targetType.Name}' is not compatible with the object type '{original.GetType().Name}'");

        }

        // Check if the type is known to be non-serializable
        if (IsKnownNonSerializableType(targetType))
        {
            throw new InvalidOperationException(
                $"Type '{targetType.Name}' is known to be non-serializable and cannot be deep cloned using JSON serialization");
        }

        try
        {
            var options = GetOrCreateJsonOptions(targetType);

            // Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(original, targetType, options);

            // Deserialize back to the original type
            return JsonSerializer.Deserialize(jsonString, targetType, options);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Failed to deep clone object of type '{targetType.Name}' due to JSON serialization error: {ex.Message}", ex);
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException(
                $"Type '{targetType.Name}' contains members that are not supported for JSON serialization: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Unexpected error during deep clone of type '{targetType.Name}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a deep copy of an object using JSON serialization.
    /// This is a generic version that provides type safety and better performance.
    /// </summary>
    /// <typeparam name="T">The type of object to be cloned</typeparam>
    /// <param name="original">The object to be cloned</param>
    /// <returns>A deep copy of the original object, or default(T) if the original was null/default</returns>
    /// <exception cref="InvalidOperationException">Thrown when the object cannot be cloned due to serialization errors</exception>
    public static T? DeepClone<T>(T? original)
    {
        // Handle value types and null references efficiently
        if (original is null)
            return default(T);

        if (typeof(T).IsValueType && EqualityComparer<T>.Default.Equals(original, default(T)))
            return default(T);

        return (T?)DeepClone(original, typeof(T));
    }

    /// <summary>
    /// Attempts to create a deep copy of an object, returning a success indicator and the cloned object.
    /// </summary>
    /// <typeparam name="T">The type of object to be cloned</typeparam>
    /// <param name="original">The object to be cloned</param>
    /// <param name="clone">The cloned object if successful, default(T) otherwise</param>
    /// <returns>True if cloning was successful, false otherwise</returns>
    public static bool TryDeepClone<T>(T? original, out T? clone)
    {
        try
        {
            clone = DeepClone(original);
            return true;
        }
        catch
        {
            clone = default(T);
            return false;
        }
    }

    #region Private DeepClone helper methods

    /// <summary>
    /// Gets or creates cached JsonSerializerOptions for a specific type to improve performance.
    /// </summary>
    /// <param name="type">The type for which to get options</param>
    /// <returns>JsonSerializerOptions configured for the specified type</returns>
    private static JsonSerializerOptions GetOrCreateJsonOptions(Type type)
    {
        return _jsonOptionsCache.GetOrAdd(type, _ => _defaultJsonOptions);
    }

    /// <summary>
    /// Checks if a type contains elements that make it unsuitable for serialization.
    /// This method performs a comprehensive analysis of the type structure including
    /// fields, properties, and known problematic type patterns.
    /// </summary>
    /// <param name="type">The type to analyze for serialization compatibility</param>
    /// <param name="visitedTypes">Optional set to track visited types and prevent infinite recursion</param>
    /// <returns>True if the type contains non-serializable elements, false otherwise</returns>
    private static bool IsKnownNonSerializableType(Type type, HashSet<Type>? visitedTypes = null)
    {
        // Initialize visited types collection for recursion protection
        visitedTypes ??= new HashSet<Type>();

        // Prevent infinite recursion with circular references
        if (!visitedTypes.Add(type))
            return false;

        try
        {
            // Check if type itself is a known problematic type
            if (IsProblematicType(type))
                return true;

            // Check if type has events (usually indicates non-serializable state)
            if (HasEvents(type))
                return true;

            // Recursively check all fields
            if (HasNonSerializableFields(type, visitedTypes))
                return true;

            // Recursively check all properties
            if (HasNonSerializableProperties(type, visitedTypes))
                return true;

            return false;
        }
        finally
        {
            visitedTypes.Remove(type);
        }
    }

    /// <summary>
    /// Determines if a type is inherently problematic for serialization.
    /// Checks against known delegate types, pointer types, and generic event handlers.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type is known to be problematic for serialization</returns>
    private static bool IsProblematicType(Type type)
    {
        // Define known problematic types - consider making this static readonly for better performance
        Type[] problematicTypes =
        [
            typeof(Delegate),
            typeof(MulticastDelegate),
            typeof(EventHandler),
            typeof(IntPtr),
            typeof(UIntPtr),
            typeof(System.Threading.Thread), // Threads cannot be serialized
            typeof(System.IO.Stream),        // Streams contain unmanaged resources
            typeof(System.Runtime.InteropServices.SafeHandle) // Safe handles wrap unmanaged resources
        ];

        // Check direct type inheritance
        if (problematicTypes.Any(pt => pt.IsAssignableFrom(type)))
            return true;

        // Check if type is a delegate subclass
        if (type.IsSubclassOf(typeof(Delegate)))
            return true;

        // Check for generic EventHandler<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventHandler<>))
            return true;

        // Check for function pointers (C# 9.0+)
        if (type.IsFunctionPointer)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if a type declares any events, which typically indicate non-serializable state.
    /// </summary>
    /// <param name="type">The type to examine</param>
    /// <returns>True if the type has public instance events</returns>
    private static bool HasEvents(Type type)
    {
        return type.GetEvents(BindingFlags.Public | BindingFlags.Instance).Any();
    }

    /// <summary>
    /// Examines all public instance fields to determine if any are non-serializable.
    /// Respects JsonIgnore and NonSerialized attributes.
    /// </summary>
    /// <param name="type">The type containing fields to check</param>
    /// <param name="visitedTypes">Collection of already visited types for recursion prevention</param>
    /// <returns>True if any field is determined to be non-serializable</returns>
    private static bool HasNonSerializableFields(Type type, HashSet<Type> visitedTypes)
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // Skip fields explicitly marked as ignored
            if (IsFieldIgnored(field))
                continue;

            // Recursively check field type
            if (IsKnownNonSerializableType(field.FieldType, visitedTypes))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Examines all public instance properties to determine if any are non-serializable.
    /// Respects JsonIgnore and NonSerialized attributes.
    /// </summary>
    /// <param name="type">The type containing properties to check</param>
    /// <param name="visitedTypes">Collection of already visited types for recursion prevention</param>
    /// <returns>True if any property is determined to be non-serializable</returns>
    private static bool HasNonSerializableProperties(Type type, HashSet<Type> visitedTypes)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Skip properties explicitly marked as ignored
            if (IsPropertyIgnored(property))
                continue;

            // Skip properties without public getter (cannot be serialized anyway)
            if (!property.CanRead || property.GetMethod?.IsPublic != true)
                continue;

            // Recursively check property type
            if (IsKnownNonSerializableType(property.PropertyType, visitedTypes))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if a field should be ignored during serialization analysis.
    /// Checks for JsonIgnore and NonSerialized attributes.
    /// </summary>
    /// <param name="field">The field to examine</param>
    /// <returns>True if the field should be ignored for serialization purposes</returns>
    private static bool IsFieldIgnored(FieldInfo field)
    {
        return field.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any() ||
               field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Any();
    }

    /// <summary>
    /// Determines if a property should be ignored during serialization analysis.
    /// Checks for JsonIgnore attribute (NonSerialized is not applicable to properties).
    /// </summary>
    /// <param name="property">The property to examine</param>
    /// <returns>True if the property should be ignored for serialization purposes</returns>
    private static bool IsPropertyIgnored(PropertyInfo property)
    {
        return property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any();
        // Note: NonSerializedAttribute is not valid on properties, only on fields
    }

    #endregion

    #endregion

    #region Dispose helper methods

    /// <summary>
    /// Safely disposes the specified object if it implements <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/>.
    /// Also handles COM objects on Windows platforms.
    /// </summary>
    /// <param name="obj">The object that should be disposed. Can be null.</param>
    /// <remarks>
    /// This method will not throw exceptions during disposal operations.
    /// COM object release is only attempted on Windows platforms.
    /// </remarks>
    public static void SafeDispose(object? obj)
    {
        if (obj is null)
            return;

        try
        {
            switch (obj)
            {
                case IAsyncDisposable asyncDisposable:
                    // For async disposable, we can only call DisposeAsync().AsTask().Wait()
                    // but this should be used carefully to avoid deadlocks
                    // In most cases, prefer the async version SafeDisposeAsync
                    asyncDisposable.DisposeAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;

                default:
                    // Handle COM objects (Windows only)
                    if (OperatingSystem.IsWindows() && Marshal.IsComObject(obj))
                    {
                        try
                        {
                            Marshal.ReleaseComObject(obj);
                        }
                        catch
                        {
                            // Silently ignore COM release errors as recommended
                        }
                    }
                    break;
            }
        }
        catch
        {
            // Silently ignore disposal errors to maintain the "safe" contract
        }
    }

    /// <summary>
    /// Asynchronously and safely disposes the specified object if it implements <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="obj">The object that should be disposed. Can be null.</param>
    /// <returns>A task representing the asynchronous disposal operation</returns>
    public static async ValueTask SafeDisposeAsync(object? obj)
    {
        if (obj is null)
            return;

        try
        {
            switch (obj)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;

                default:
                    // Handle COM objects (Windows only)
                    if (OperatingSystem.IsWindows() && Marshal.IsComObject(obj))
                    {
                        try
                        {
                            Marshal.ReleaseComObject(obj);
                        }
                        catch
                        {
                            // Silently ignore COM release errors
                        }
                    }
                    break;
            }
        }
        catch
        {
            // Silently ignore disposal errors to maintain the "safe" contract
        }
    }

    /// <summary>
    /// Safely disposes all elements in a sequence that implement <see cref="IDisposable"/>.
    /// Non-disposable elements are ignored without error.
    /// </summary>
    /// <param name="sequence">The sequence containing potentially disposable objects. Can be null.</param>
    /// <remarks>
    /// This method continues processing all elements even if some disposal operations fail.
    /// </remarks>
    public static void SafeDisposeAll(IEnumerable? sequence)
    {
        if (sequence is null) return;

        foreach (var item in sequence)
        {
            SafeDispose(item);
        }
    }

    /// <summary>
    /// Asynchronously and safely disposes all elements in a sequence that implement <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="sequence">The sequence containing potentially disposable objects. Can be null.</param>
    /// <returns>A task representing the asynchronous disposal operation</returns>
    public static async ValueTask SafeDisposeAllAsync(IEnumerable? sequence)
    {
        if (sequence is null)
            return;

        foreach (var item in sequence)
        {
            await SafeDisposeAsync(item).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Safely disposes the values of a dictionary if they implement <see cref="IDisposable"/>.
    /// The dictionary itself and its keys are not disposed.
    /// </summary>
    /// <param name="dictionary">The dictionary whose values should be disposed. Can be null.</param>
    /// <remarks>
    /// This method continues processing all values even if some disposal operations fail.
    /// Only the values are disposed, not the dictionary instance itself or its keys.
    /// </remarks>
    [DebuggerStepThrough]
    public static void SafeDisposeAllDictionaryValues(IDictionary? dictionary)
    {
        if (dictionary is null)
            return;

        foreach (var value in dictionary.Values)
        {
            SafeDispose(value);
        }
    }

    /// <summary>
    /// Asynchronously and safely disposes the values of a dictionary if they implement <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="dictionary">The dictionary whose values should be disposed. Can be null.</param>
    /// <returns>A task representing the asynchronous disposal operation</returns>
    [DebuggerStepThrough]
    public static async ValueTask SafeDisposeAllDictionaryValuesAsync(IDictionary? dictionary)
    {
        if (dictionary is null)
            return;

        foreach (var value in dictionary.Values)
        {
            await SafeDisposeAsync(value).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Safely disposes both keys and values of a dictionary if they implement <see cref="IDisposable"/>.
    /// The dictionary itself is not disposed.
    /// </summary>
    /// <param name="dictionary">The dictionary whose keys and values should be disposed. Can be null.</param>
    public static void SafeDisposeAllDictionaryKeysAndValues(IDictionary? dictionary)
    {
        if (dictionary is null)
            return;

        foreach (DictionaryEntry entry in dictionary)
        {
            SafeDispose(entry.Key);
            SafeDispose(entry.Value);
        }
    }

    #endregion

    #region Type checking helper methods

    /// <summary>
    /// Determines whether the specified type can be safely cloned using JSON serialization.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type can likely be cloned successfully, false otherwise</returns>
    /// <exception cref="ArgNullException">Thrown if the type is null</exception>"
    public static bool IsCloneable(Type type)
    {
        ArgNullException.ThrowIfNull(type);

        // Value types are generally cloneable
        if (type.IsPrimitive && type != typeof(IntPtr) && type != typeof(UIntPtr))
            return true;

        // String is cloneable (though it's immutable)
        if (type == typeof(string))
            return true;

        // Arrays of cloneable types are cloneable
        if (type.IsArray)
            return IsCloneable(type.GetElementType()!);

        // Check for known non-serializable types
        if (IsKnownNonSerializableType(type))
            return false;

        // Value types are generally cloneable
        if (type.IsValueType)
            return true;

        // Collections are generally cloneable if their element types are
        if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            return genericArgs.All(IsCloneable);
        }

        // For other reference types, assume they're cloneable unless proven otherwise
        return true;
    }

    /// <summary>
    /// Determines whether the specified object can be safely cloned.
    /// </summary>
    /// <param name="obj">The object to check. Can be null.</param>
    /// <returns>True if the object can likely be cloned successfully, false otherwise</returns>
    public static bool IsCloneable(object? obj)
    {
        return obj is null || IsCloneable(obj.GetType());
    }

    #endregion

}
