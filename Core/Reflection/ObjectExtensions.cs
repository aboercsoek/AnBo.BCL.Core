//--------------------------------------------------------------------------
// File:    ObjectExtensions.cs
// Content:	Implementation of class ObjectExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using static System.Collections.Specialized.BitVector32;

#endregion

namespace AnBo.Core;

/// <summary>
/// Fluent <see cref="object"/> and <see cref="Type"/> extensions optimized for .NET 8+.
/// Provides safe casting, type conversion, and fluent operation patterns.
/// </summary>
public static class ObjectEx
{
    #region Safe Casting Extensions

    /// <summary>
    /// Safely casts an object to a nullable value type using modern pattern matching.
    /// </summary>
    /// <typeparam name="T">The target value type to cast to</typeparam>
    /// <param name="item">The object to cast</param>
    /// <returns>The casted value or null if casting fails</returns>
    /// <example>
    /// <code>
    /// object value = 42;
    /// int? result = value.AsValueType&lt;int&gt;(); // Returns 42
    /// 
    /// object invalidValue = "text";
    /// int? nullResult = invalidValue.AsValueType&lt;int&gt;(); // Returns null
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? AsValueType<T>(this object? item) where T : struct
        => item is T value ? value : null;

    /// Safely casts an object to any type (reference or value type) using pattern matching.
    /// This is a fluent version of the 'as' keyword that works with both reference and value types.
    /// </summary>
    /// <typeparam name="T">The target type to cast to</typeparam>
    /// <param name="item">The object to cast</param>
    /// <returns>The casted value or default(T) if casting fails</returns>
    /// <example>
    /// <code>
    /// object stringValue = "Hello";
    /// string? result = stringValue.As&lt;string&gt;(); // Returns "Hello"
    /// 
    /// object numberValue = 42;
    /// int result2 = numberValue.As&lt;int&gt;(); // Returns 42
    /// 
    /// object invalidValue = "text";
    /// int result3 = invalidValue.As&lt;int&gt;(); // Returns 0 (default for int)
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? AsUniversal<T>(this object? item)
        => item is T result ? result : default;

    /// <summary>
    /// Performs a strict cast operation that throws an exception if the cast fails.
    /// This is a fluent version of explicit casting with better error messages.
    /// </summary>
    /// <typeparam name="T">The target type to cast to</typeparam>
    /// <param name="item">The object to cast (must not be null)</param>
    /// <returns>The casted value</returns>
    /// <exception cref="ArgumentNullException">Thrown when item is null</exception>
    /// <exception cref="InvalidCastException">Thrown when the cast is not possible</exception>
    /// <example>
    /// <code>
    /// object value = 42;
    /// int result = value.CastTo&lt;int&gt;(); // Returns 42
    /// 
    /// object invalidValue = "text";
    /// int result2 = invalidValue.CastTo&lt;int&gt;(); // Throws InvalidCastException
    /// </code>
    /// </example>
    public static T CastTo<T>(this object item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item is T result
            ? result
            : throw new InvalidCastException($"Cannot cast from type '{item.GetType().Name}' to '{typeof(T).Name}'");
    }

    /// <summary>
    /// Attempts to cast an object to the specified type, returning success status and result.
    /// Uses modern .NET 8 try-pattern for better performance.
    /// </summary>
    /// <typeparam name="T">The target type</typeparam>
    /// <param name="obj">The object to cast</param>
    /// <param name="result">The casted result if successful</param>
    /// <returns>true if casting succeeded; otherwise false</returns>
    /// <example>
    /// <code>
    /// object value = 42;
    /// if (value.TryCast&lt;int&gt;(out var number))
    /// {
    ///     Console.WriteLine($"Number: {number}");
    /// }
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCast<T>(this object? obj, [NotNullWhen(true)] out T? result)
    {
        if (obj is T casted)
        {
            result = casted;
            return true;
        }

        result = default;
        return false;
    }

    #endregion

    #region Sequence Casting Extensions

    /// <summary>
    /// Safely casts elements of a sequence to the target type, filtering out incompatible elements.
    /// Core implementation for all sequence casting operations.
    /// </summary>
    /// <typeparam name="TTarget">The target element type</typeparam>
    /// <param name="source">The source sequence</param>
    /// <returns>A sequence containing only successfully casted elements</returns>
    /// <example>
    /// <code>
    /// IEnumerable&lt;object&gt; mixed = new object[] { 1, "text", 2, null, 3.14 };
    /// IEnumerable&lt;int&gt; integers = mixed.CastSequenceCore&lt;int&gt;(); // Returns [1]
    /// </code>
    /// </example>
    private static IEnumerable<TTarget> CastSequenceCore<TTarget>(IEnumerable? source)
    {
        if (source is null) yield break;

        foreach (var item in source)
        {
            if (item is TTarget casted)
                yield return casted;
        }
    }

    /// <summary>
    /// Safely casts elements of a typed sequence to the target type, filtering out incompatible elements.
    /// </summary>
    /// <typeparam name="TSource">The source element type</typeparam>
    /// <typeparam name="TTarget">The target element type</typeparam>
    /// <param name="source">The source sequence</param>
    /// <returns>A sequence containing only successfully casted elements</returns>
    public static IEnumerable<TTarget> CastSequence<TSource, TTarget>(this IEnumerable<TSource?>? source)
        => CastSequenceCore<TTarget>(source);

    /// <summary>
    /// Safely casts elements of a non-generic sequence to the target type.
    /// </summary>
    /// <typeparam name="TTarget">The target element type</typeparam>
    /// <param name="source">The source sequence</param>
    /// <returns>A sequence containing only successfully casted elements</returns>
    public static IEnumerable<TTarget> CastSequence<TTarget>(this IEnumerable? source)
        => CastSequenceCore<TTarget>(source);

    /// <summary>
    /// Strictly casts all elements of a sequence to the target type.
    /// Throws an exception if any element cannot be casted.
    /// </summary>
    /// <typeparam name="TSource">The source element type</typeparam>
    /// <typeparam name="TTarget">The target element type</typeparam>
    /// <param name="source">The source sequence</param>
    /// <returns>A sequence with all elements casted to the target type</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null</exception>
    /// <exception cref="InvalidCastException">Thrown when any element cannot be casted</exception>
    public static IEnumerable<TTarget> CastSequenceStrict<TSource, TTarget>(this IEnumerable<TSource?> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        foreach (var item in source)
        {
            if (item is null)
                throw new InvalidCastException($"Cannot cast null to type '{typeof(TTarget).Name}'");

            yield return item.CastTo<TTarget>();
        }
    }

    #endregion

    #region Object Creation and Factory Methods

    /// <summary>
    /// Creates an instance of the specified type with optimized handling for common types.
    /// Uses modern .NET 8 patterns and performance optimizations.
    /// </summary>
    /// <param name="type">The type to instantiate</param>
    /// <returns>A new instance of the specified type</returns>
    /// <exception cref="ArgumentNullException">Thrown when type is null</exception>
    /// <example>
    /// <code>
    /// Type stringType = typeof(string);
    /// object instance = stringType.CreateInstance(); // Returns string.Empty
    /// 
    /// Type listType = typeof(List&lt;int&gt;);
    /// object list = listType.CreateInstance(); // Returns new List&lt;int&gt;()
    /// </code>
    /// </example>
    public static object? CreateInstance(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        // Optimized handling for common types
        return type == typeof(string)
            ? string.Empty
            : Activator.CreateInstance(type);
    }

    #endregion

    #region Fluent Operation Extensions

    /// <summary>
    /// Applies an action to an object and returns the object (fluent interface pattern).
    /// Enables method chaining for configuration and setup scenarios.
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to apply the action to</param>
    /// <param name="action">The action to apply</param>
    /// <returns>The original object after the action has been applied</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null</exception>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt;()
    ///     .With(l => l.Add(1))
    ///     .With(l => l.Add(2))
    ///     .With(l => Console.WriteLine($"List has {l.Count} items"));
    /// </code>
    /// </example>
    public static T With<T>(this T obj, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        action(obj);
        return obj;
    }

    /// <summary>
    /// Conditionally applies an action to an object based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="obj">The object to conditionally modify</param>
    /// <param name="condition">The condition to evaluate</param>
    /// <param name="action">The action to apply if condition is true</param>
    /// <returns>The original object</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null</exception>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt;()
    ///     .WithIf(DateTime.Now.Hour &gt; 12, l => l.Add(999));
    /// </code>
    /// </example>
    public static T WithIf<T>(this T obj, bool condition, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (condition)
            action(obj);

        return obj;
    }

    /// <summary>
    /// Provides a null-safe way to access nested properties using a selector function.
    /// Returns default(TResult) if any intermediate value is null.
    /// </summary>
    /// <typeparam name="T">The source type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="obj">The source object</param>
    /// <param name="selector">The selector function</param>
    /// <returns>The selected value or default if any intermediate value is null</returns>
    /// <example>
    /// <code>
    /// Person? person = GetPerson();
    /// string? city = person.SafeSelect(p => p.Address?.City);
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult? SafeSelect<T, TResult>(this T? obj, Func<T, TResult?> selector)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(selector);
        return obj is null ? default : selector(obj);
    }

    #endregion

    #region IDisposable Extensions

    /// <summary>
    /// Executes an action with a disposable object and ensures proper disposal.
    /// Uses modern using patterns for optimal performance.
    /// </summary>
    /// <typeparam name="T">The disposable type</typeparam>
    /// <param name="disposable">The disposable object</param>
    /// <param name="action">The action to execute</param>
    /// <exception cref="ArgumentNullException">Thrown when disposable or action is null</exception>
    public static void Using<T>(this T disposable, Action<T> action) where T : IDisposable
    {
        ArgumentNullException.ThrowIfNull(disposable);
        ArgumentNullException.ThrowIfNull(action);

        using (disposable)
        {
            action(disposable);
        }
    }

    /// <summary>
    /// Executes an function with a disposable object and ensures proper disposal.
    /// Uses modern using patterns for optimal performance.
    /// </summary>
    /// <typeparam name="TDisposable">The disposable type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="disposable">The disposable object</param>
    /// <param name="func">The function to execute</param>
    /// <returns>The result of the function</returns>
    /// <exception cref="ArgumentNullException">Thrown when disposable or func is null</exception>
    public static TResult Using<TDisposable, TResult>(this TDisposable disposable, Func<TDisposable, TResult> func)
        where TDisposable : IDisposable
    {
        ArgumentNullException.ThrowIfNull(disposable);
        ArgumentNullException.ThrowIfNull(func);

        using (disposable)
        {
            return func(disposable);
        }
    }

    /// <summary>
    /// Calls func(<paramref name="disposable"/>, <paramref name="funcParam2"/>) and ensures disposing of <paramref name="disposable"/> after that call.
    /// </summary>
    /// <typeparam name="TDisposable">The disposable type</typeparam>
    /// <typeparam name="TFuncParam2">Type of the second func parameter</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="disposable">The disposable object</param>
    /// <param name="funcParam2">The second func delegate argument.</param>
    /// <param name="func">The function to execute</param>
    /// <returns>The result of the function</returns>
    /// <exception cref="ArgumentNullException">Thrown when disposable or func is null</exception>
    public static TResult Using<TDisposable, TFuncParam2, TResult>(
        this TDisposable disposable, 
        TFuncParam2? funcParam2, 
        Func<TDisposable, TFuncParam2?, TResult> func) 
        where TDisposable : IDisposable
    {
        ArgumentNullException.ThrowIfNull(disposable);
        ArgumentNullException.ThrowIfNull(func);

        using (disposable)
        {
            return func(disposable, funcParam2);
        }
    }

    /// <summary>
    /// Disposes each element in a sequence that implements IDisposable.
    /// Uses modern enumeration patterns and exception handling.
    /// </summary>
    /// <typeparam name="T">The disposable element type</typeparam>
    /// <param name="sequence">The sequence of disposable objects</param>
    /// <param name="action">The action to execute for each element before disposal</param>
    /// <exception cref="ArgumentNullException">Thrown when sequence or action is null</exception>
    public static void UsingEach<T>(this IEnumerable<T> sequence, Action<T> action) where T : IDisposable
    {
        ArgumentNullException.ThrowIfNull(sequence);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in sequence)
        {
            using (item)
            {
                action(item);
            }
        }
    }

    /// <summary>
    /// Safely disposes an object if it implements IDisposable.
    /// Handles COM objects using modern interop patterns.
    /// </summary>
    /// <param name="obj">The object to dispose</param>
    public static void SafeDispose(this object? obj)
    {
        TypeHelper.SafeDispose(obj);
    }

    /// <summary>
    /// Safely disposes all elements in a sequence that implement IDisposable.
    /// </summary>
    /// <param name="sequence">The sequence containing potentially disposable objects</param>
    public static void SafeDisposeAll(this IEnumerable? sequence)
    {
        TypeHelper.SafeDisposeAll(sequence);
    }

    #endregion

    #region Null and Default Value Checking

    /// <summary>
    /// Determines whether the specified instance is null using modern null-checking patterns.
    /// Works efficiently with both reference and value types.
    /// </summary>
    /// <typeparam name="T">The type of the instance</typeparam>
    /// <param name="instance">The instance to check</param>
    /// <returns>true if the instance is null; otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull<T>([NotNullWhen(false)] this T? instance)
        => instance is null;

    /// Determines whether the specified instance is not null.
    /// </summary>
    /// <typeparam name="T">The type of the instance</typeparam>
    /// <param name="instance">The instance to check</param>
    /// <returns>true if the instance is not null; otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNull<T>([NotNullWhen(true)] this T? instance)
        => instance is not null;

    /// <summary>
    /// Efficiently determines whether a value is the default value for its type.
    /// Uses caching for improved performance with repeated type checks.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>true if the value is the default for its type; otherwise false</returns>
    /// <example>
    /// <code>
    /// int zero = 0;
    /// bool isDefault1 = zero.IsDefaultValue(); // Returns true
    /// 
    /// string empty = "";
    /// bool isDefault2 = empty.IsDefaultValue(); // Returns false (empty string != null)
    /// 
    /// string? nullString = null;
    /// bool isDefault3 = nullString.IsDefaultValue(); // Returns true
    /// </code>
    /// </example>
    public static bool IsDefaultValue(this object? value)
    {
        if (value is null) return true;

        return value.GetType().IsDefaultValue(value);
    }

    /// Determines whether a value is the default value for its type or an empty string.
    /// Optimized for string handling scenarios.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>true if the value is default or an empty string; otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefaultOrEmpty(this object? value)
    {
        if (value is null) return true;

        return value.GetType().IsDefaultValueOrEmptyString(value);
    }

    #endregion

}
