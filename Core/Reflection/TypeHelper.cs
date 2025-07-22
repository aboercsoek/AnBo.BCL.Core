//--------------------------------------------------------------------------
// File:    TypeHelper.cs
// Content:	Optimized implementation of class TypeHelper
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace AnBo.Core;

/// <summary>
/// Provides optimized common type helper methods for object manipulation, cloning, and disposal.
/// </summary>
public static class TypeHelper
{
    #region DeepClone helper methods

    /// <summary>
    /// Creates a deep copy of an object using optimized JSON serialization.
    /// This method leverages the consolidated JSON serializability check for better performance and reliability.
    /// </summary>
    /// <param name="original">The object to be cloned</param>
    /// <param name="type">The type of object to be cloned. If null, uses the runtime type of the object</param>
    /// <returns>A deep copy of the original object, or null if the original was null</returns>
    /// <exception cref="InvalidOperationException">Thrown when the object cannot be cloned due to serialization errors</exception>
    /// <exception cref="ArgumentException">Thrown when the provided type is incompatible with the object</exception>
    public static object? DeepClone(object? original, Type? type = null)
    {
        if (original is null)
            return null;

        var targetType = type ?? original.GetType();

        // Validate type compatibility
        if (type is not null && !targetType.IsAssignableFrom(original.GetType()))
        {
            throw new ArgumentException(
                $"The provided type '{targetType.GetTypeName()}' is not compatible with the object type '{original.GetType().GetTypeName()}'",
                nameof(type));
        }

        // Use the consolidated serializability check
        if (!CheckJsonSerializabilityComprehensive(targetType))
        {
            throw new InvalidOperationException(
                $"Type '{targetType.GetTypeName()}' is not suitable for JSON-based deep cloning. " +
                $"Consider implementing ICloneable or using a different cloning strategy.");
        }

        try
        {
            var options = GetOrCreateJsonOptions(targetType);
            return PerformJsonClone(original, targetType, options);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"JSON serialization failed during deep clone of type '{targetType.GetTypeName()}': {ex.Message}", ex);
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException(
                $"Type '{targetType.GetTypeName()}' contains unsupported members for JSON serialization: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Unexpected error during deep clone of type '{targetType.GetTypeName()}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a deep copy of an object using optimized JSON serialization.
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

        var type = typeof(T);

        // Use the consolidated serializability check
        if (!CheckJsonSerializabilityComprehensive(type))
        {
            throw new InvalidOperationException(
                $"Type '{type.GetTypeName()}' is not suitable for JSON-based deep cloning. " +
                $"Consider implementing ICloneable or using a different cloning strategy.");
        }

        try
        {
            var options = GetOrCreateJsonOptions(type);
            return PerformJsonClone<T>(original, options);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"JSON serialization failed during deep clone of type '{type.GetTypeName()}': {ex.Message}", ex);
        }
        catch (NotSupportedException ex)
        {
            throw new InvalidOperationException(
                $"Type '{type.GetTypeName()}' contains unsupported members for JSON serialization: {ex.Message}", ex);
        }
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
    /// Gets or creates optimized JsonSerializerOptions for cloning operations with caching.
    /// </summary>
    /// <param name="type">The type for which to get options</param>
    /// <returns>JsonSerializerOptions configured for the specified type</returns>
    private static JsonSerializerOptions GetOrCreateJsonOptions(Type type)
    {
        return TypeCache.GetOrAdd($"clone_options_{type}", CreateCloneOptions);
    }

    /// <summary>
    /// Creates optimized JsonSerializerOptions for cloning operations.
    /// </summary>
    /// <returns>Configured JsonSerializerOptions for deep cloning</returns>
    private static JsonSerializerOptions CreateCloneOptions()
    {
        return new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            IncludeFields = true,
            IgnoreReadOnlyProperties = false,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            // Optimize for cloning performance
            WriteIndented = false,
            PropertyNamingPolicy = null
        };
    }

    /// <summary>
    /// Performs the actual JSON serialization/deserialization for cloning (non-generic version).
    /// </summary>
    /// <param name="original">The original object</param>
    /// <param name="type">The type of the object</param>
    /// <param name="options">JSON serializer options</param>
    /// <returns>The cloned object</returns>
    private static object? PerformJsonClone(object original, Type type, JsonSerializerOptions options)
    {
        var jsonString = JsonSerializer.Serialize(original, type, options);
        return JsonSerializer.Deserialize(jsonString, type, options);
    }

    /// <summary>
    /// Performs the actual JSON serialization/deserialization for cloning (generic version).
    /// </summary>
    /// <typeparam name="T">The type being cloned</typeparam>
    /// <param name="original">The original object</param>
    /// <param name="options">JSON serializer options</param>
    /// <returns>The cloned object</returns>
    private static T? PerformJsonClone<T>(T original, JsonSerializerOptions options)
    {
        var jsonString = JsonSerializer.Serialize(original, options);
        return JsonSerializer.Deserialize<T>(jsonString, options);
    }

    #endregion

    #endregion

    #region Dispose helper methods

    /// <summary>
    /// Safely disposes the specified object if it implements <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/>.
    /// Also handles COM objects on Windows platforms. Uses optimized core disposal logic.
    /// </summary>
    /// <param name="obj">The object that should be disposed. Can be null.</param>
    /// <remarks>
    /// This method will not throw exceptions during disposal operations.
    /// COM object release is only attempted on Windows platforms.
    /// </remarks>
    public static void SafeDispose(object? obj)
    {
        CoreDisposeAsync(obj, forceSync: true).AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Asynchronously and safely disposes the specified object if it implements <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// Uses optimized core disposal logic.
    /// </summary>
    /// <param name="obj">The object that should be disposed. Can be null.</param>
    /// <returns>A task representing the asynchronous disposal operation</returns>
    public static ValueTask SafeDisposeAsync(object? obj)
    {
        return CoreDisposeAsync(obj, forceSync: false);
    }

    /// <summary>
    /// Safely disposes all elements in a sequence that implement <see cref="IDisposable"/>.
    /// Non-disposable elements are ignored without error. Uses optimized batch processing.
    /// </summary>
    /// <param name="sequence">The sequence containing potentially disposable objects. Can be null.</param>
    /// <remarks>
    /// This method continues processing all elements even if some disposal operations fail.
    /// </remarks>
    public static void SafeDisposeAll(IEnumerable? sequence)
    {
        SafeDisposeAllCoreAsync(sequence, forceSync: true).AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Asynchronously and safely disposes all elements in a sequence that implement <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// Uses optimized batch processing with parallel execution.
    /// </summary>
    /// <param name="sequence">The sequence containing potentially disposable objects. Can be null.</param>
    /// <returns>A task representing the asynchronous disposal operation</returns>
    public static ValueTask SafeDisposeAllAsync(IEnumerable? sequence)
    {
        return SafeDisposeAllCoreAsync(sequence, forceSync: false);
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

        SafeDisposeAll(dictionary.Values);
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

        await SafeDisposeAllAsync(dictionary.Values);
    }

    /// <summary>
    /// Safely disposes both keys and values of a dictionary if they implement <see cref="IDisposable"/>.
    /// The dictionary itself is not disposed. Uses optimized batch processing.
    /// </summary>
    /// <param name="dictionary">The dictionary whose keys and values should be disposed. Can be null.</param>
    public static void SafeDisposeAllDictionaryKeysAndValues(IDictionary? dictionary)
    {
        if (dictionary is null)
            return;

        var itemsToDispose = new List<object>();
        foreach (DictionaryEntry entry in dictionary)
        {
            if (entry.Key is not null) itemsToDispose.Add(entry.Key);
            if (entry.Value is not null) itemsToDispose.Add(entry.Value);
        }

        SafeDisposeAll(itemsToDispose);
    }

    #region Private Dispose helper methods

    /// <summary>
    /// Core disposal logic that handles all disposal scenarios with optimized performance.
    /// </summary>
    /// <param name="obj">The object to dispose</param>
    /// <param name="forceSync">If true, async disposable objects will be disposed synchronously</param>
    /// <returns>ValueTask for async operations, completed task for sync operations</returns>
    private static async ValueTask CoreDisposeAsync(object? obj, bool forceSync = false)
    {
        if (obj is null) return;

        try
        {
            switch (obj)
            {
                case IAsyncDisposable asyncDisposable when !forceSync:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;

                case IAsyncDisposable asyncDisposable when forceSync:
                    // Synchronous disposal of async disposable (not recommended but sometimes necessary)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;

                default:
                    // Handle COM objects on Windows
                    await DisposeCOMObjectIfNeeded(obj);
                    break;
            }
        }
        catch
        {
            // Maintain "safe" contract by suppressing exceptions
        }
    }

    /// <summary>
    /// Handles COM object disposal on Windows platforms.
    /// </summary>
    /// <param name="obj">Potential COM object</param>
    /// <returns>Completed ValueTask</returns>
    private static ValueTask DisposeCOMObjectIfNeeded(object obj)
    {
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
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Generic batch disposal method that works for any enumerable with optimized parallel processing.
    /// </summary>
    /// <param name="items">Items to dispose</param>
    /// <param name="forceSync">Whether to force synchronous disposal</param>
    /// <returns>ValueTask representing the disposal operation</returns>
    private static async ValueTask SafeDisposeAllCoreAsync(IEnumerable? items, bool forceSync = false)
    {
        if (items is null) return;

        // Convert to list to avoid multiple enumeration and enable parallel processing
        var itemList = items.Cast<object>().Where(item => item is not null).ToList();

        if (itemList.Count == 0) return;

        try
        {
            // For small collections, process sequentially to avoid task overhead
            if (itemList.Count <= 10)
            {
                foreach (var item in itemList)
                {
                    await CoreDisposeAsync(item, forceSync);
                }
            }
            else
            {
                // For larger collections, use parallel processing for better performance
                var disposalTasks = itemList.Select(item => CoreDisposeAsync(item, forceSync).AsTask());
                await Task.WhenAll(disposalTasks);
            }
        }
        catch
        {
            // Maintain safe contract by suppressing exceptions
        }
    }

    #endregion

    #endregion

    #region Type analysis methods

    /// <summary>
    /// Determines if the type is suitable for JSON serialization with System.Text.Json.
    /// Uses comprehensive analysis and caching for optimal performance.
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
    public static bool IsJsonSerializable(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"jsonserializable_{type}",
            () => CheckJsonSerializabilityComprehensive(type));
    }

    /// <summary>
    /// Determines whether the specified type can be safely cloned using JSON serialization.
    /// Builds upon JSON serializability with additional semantic checks.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type can likely be cloned successfully, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown if the type is null</exception>
    public static bool IsCloneable(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return TypeCache.GetOrAdd($"cloneable_{type}", () => CheckCloneability(type));
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

    #region Private type analysis methods

    /// <summary>
    /// Comprehensive JSON serializability check combining all necessary validations.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="visitedTypes">Set to track visited types and prevent infinite recursion.</param>
    /// <returns>true if JSON serializable; otherwise, false.</returns>
    private static bool CheckJsonSerializabilityComprehensive(Type type, HashSet<Type>? visitedTypes = null)
    {
        // Initialize recursion protection
        visitedTypes ??= new HashSet<Type>();
        if (!visitedTypes.Add(type))
            return true; // Assume serializable for circular references (handled by ReferenceHandler.Preserve)

        try
        {
            // Basic exclusions - known problematic types
            if (IsBasicNonSerializableType(type))
                return false;

            // Primitive types and common serializable types
            if (IsKnownSerializableType(type))
                return true;

            // Nullable types - check underlying type
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
                return CheckJsonSerializabilityComprehensive(underlyingType, visitedTypes);

            // Arrays - check element type
            if (type.IsArray)
                return CheckJsonSerializabilityComprehensive(type.GetElementType()!, visitedTypes);

            // Collections - check generic arguments
            if (IsCollectionType(type))
            {
                if (type.IsGenericType)
                {
                    var genericArgs = type.GetGenericArguments();
                    return genericArgs.All(arg => CheckJsonSerializabilityComprehensive(arg, visitedTypes));
                }
                return true; // Non-generic collections are generally serializable
            }

            // Check for events (usually indicates non-serializable state)
            if (HasPublicEvents(type))
                return false;

            // Record types are generally serializable
            if (IsRecord(type))
                return true;

            // Classes need proper constructor support AND must not contain non-serializable members
            if (type.IsClass)
            {
                if (!HasValidConstructorForSerialization(type))
                    return false;

                // Additionally check if the class contains non-serializable members
                return !ContainsNonSerializableMembers(type, visitedTypes);
            }

            // Value types (structs) are generally serializable
            if (type.IsValueType)
                return !ContainsNonSerializableMembers(type, visitedTypes);

            return false;
        }
        finally
        {
            visitedTypes.Remove(type);
        }
    }

    /// <summary>
    /// Core implementation for cloneability checking.
    /// Performs additional semantic checks beyond JSON serializability.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is cloneable.</returns>
    private static bool CheckCloneability(Type type)
    {
        // First check: Must be JSON serializable
        if (!CheckJsonSerializabilityComprehensive(type))
            return false;

        // Additional semantic checks for successful cloning

        // Immutable types are effectively "cloneable" (return same instance)
        if (IsEffectivelyImmutable(type))
            return true;

        // Arrays of cloneable types are cloneable
        if (type.IsArray)
            return CheckCloneability(type.GetElementType()!);

        // Types with special clone semantics should be allowed
        if (ImplementsICloneable(type))
            return true;

        // Value types are generally safe to clone
        if (type.IsValueType)
            return true;

        // Collections with cloneable elements
        if (IsCollectionType(type) && type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            return genericArgs.All(CheckCloneability);
        }

        // Additional checks for types that might serialize but not clone properly
        if (HasProblematicCloneSemantics(type))
            return false;

        // Default: if JSON serializable, assume cloneable
        return true;
    }

    /// <summary>
    /// Checks for basic types that are known to be non-serializable.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type is known to be non-serializable.</returns>
    private static bool IsBasicNonSerializableType(Type type)
    {
        // Pointer and reference types
        if (type.IsPointer || type.IsByRef || type.IsFunctionPointer)
            return true;

        // Platform-specific integer types
        if (type == typeof(nint) || type == typeof(nuint) ||
            type == typeof(IntPtr) || type == typeof(UIntPtr))
            return true;

        // Delegate types
        if (typeof(Delegate).IsAssignableFrom(type) || type.IsSubclassOf(typeof(Delegate)))
            return true;

        // Generic EventHandler<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventHandler<>))
            return true;

        // System types that contain unmanaged resources
        Type[] unsupportedTypes =
        [
            typeof(System.Threading.Thread),
            typeof(System.IO.Stream),
            typeof(System.Runtime.InteropServices.SafeHandle),
            typeof(System.Runtime.Serialization.SerializationInfo)
        ];

        return unsupportedTypes.Any(unsupportedType => unsupportedType.IsAssignableFrom(type));
    }

    /// <summary>
    /// Checks for types that are known to be serializable.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type is known to be serializable.</returns>
    private static bool IsKnownSerializableType(Type type)
    {
        if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
            return true;

        Type[] knownSerializableTypes =
        [
            typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan),
            typeof(Guid), typeof(Uri), typeof(Version),
            typeof(decimal)
        ];

        return knownSerializableTypes.Contains(type);
    }

    /// <summary>
    /// Determines if the type is a collection type (shared helper method).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a collection type.</returns>
    private static bool IsCollectionType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }

    /// <summary>
    /// Checks if a type has public events.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type has public events.</returns>
    private static bool HasPublicEvents(Type type)
    {
        return type.GetEvents(BindingFlags.Public | BindingFlags.Instance).Any();
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

    /// <summary>
    /// Validates that a class has appropriate constructors for JSON deserialization.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type has valid constructors for serialization.</returns>
    private static bool HasValidConstructorForSerialization(Type type)
    {
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        // Parameterless constructor
        if (constructors.Any(c => c.GetParameters().Length == 0))
            return true;

        // Constructor with JsonConstructor attribute
        return constructors.Any(c => c.GetCustomAttribute<JsonConstructorAttribute>() is not null);
    }

    /// <summary>
    /// Checks if a type contains non-serializable members in its public surface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="visitedTypes">Set of visited types for recursion prevention.</param>
    /// <returns>true if the type contains non-serializable members.</returns>
    private static bool ContainsNonSerializableMembers(Type type, HashSet<Type> visitedTypes)
    {
        // Check public fields
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (IsFieldIgnored(field)) continue;
            if (!CheckJsonSerializabilityComprehensive(field.FieldType, visitedTypes))
                return true;
        }

        // Check public properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (IsPropertyIgnored(property)) continue;
            if (!property.CanRead || property.GetMethod?.IsPublic != true) continue;
            if (!CheckJsonSerializabilityComprehensive(property.PropertyType, visitedTypes))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if a field should be ignored during serialization analysis.
    /// </summary>
    /// <param name="field">The field to examine.</param>
    /// <returns>true if the field should be ignored.</returns>
    private static bool IsFieldIgnored(FieldInfo field)
    {
        return field.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any() ||
               field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Any();
    }

    /// <summary>
    /// Determines if a property should be ignored during serialization analysis.
    /// </summary>
    /// <param name="property">The property to examine.</param>
    /// <returns>true if the property should be ignored.</returns>
    private static bool IsPropertyIgnored(PropertyInfo property)
    {
        return property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any();
    }

    /// <summary>
    /// Checks if a type is effectively immutable and therefore safe to "clone" by reference.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is effectively immutable.</returns>
    private static bool IsEffectivelyImmutable(Type type)
    {
        // Known immutable types
        Type[] immutableTypes =
        [
            typeof(string), typeof(int), typeof(long), typeof(double), typeof(decimal),
            typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan), typeof(Guid),
            typeof(Uri), typeof(Version)
        ];

        return immutableTypes.Contains(type) ||
               type.IsPrimitive ||
               type.IsEnum ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                IsEffectivelyImmutable(type.GetGenericArguments()[0]));
    }

    /// <summary>
    /// Checks if a type implements ICloneable interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type implements ICloneable.</returns>
    private static bool ImplementsICloneable(Type type)
    {
        return typeof(ICloneable).IsAssignableFrom(type);
    }

    /// <summary>
    /// Checks for types that might be JSON serializable but have problematic clone semantics.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type has problematic clone semantics.</returns>
    private static bool HasProblematicCloneSemantics(Type type)
    {
        // Types that maintain internal state that shouldn't be cloned
        Type[] problematicTypes =
        [
            typeof(System.Threading.CancellationToken),        // Cancellation state
            typeof(System.Threading.CancellationTokenSource),  // Cancellation control
            typeof(System.Random),                             // Random number generator state
            typeof(System.Security.Cryptography.RandomNumberGenerator), // Crypto state
            typeof(System.Diagnostics.Stopwatch),             // Timing state
            typeof(System.Threading.Timer),                    // Timer state
        ];

        return problematicTypes.Any(pt => pt.IsAssignableFrom(type));
    }

    #endregion

    #endregion
}