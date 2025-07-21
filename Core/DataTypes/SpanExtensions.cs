//--------------------------------------------------------------------------
// File:    SpanExtensions.cs
// Content:	Implementation of class SpanExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

///<summary>TODO: Description of class SpanExtensions</summary>
/// <summary>
/// Extension methods for Span&lt;T&gt; and ReadOnlySpan&lt;T&gt; to provide LINQ-like functionality
/// while maintaining high performance characteristics.
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    /// Counts the number of elements in the span that satisfy the specified condition.
    /// This method provides LINQ-like Count functionality for Span&lt;T&gt; with optimal performance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to count elements in.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The number of elements that satisfy the condition.</returns>
    /// <exception cref="ArgumentNullException">Thrown when predicate is null.</exception>
    public static int Count<T>(this Span<T> span, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        int count = 0;

        // Use ref iteration for maximum performance
        ref T reference = ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span);

        for (int i = 0; i < span.Length; i++)
        {
            if (predicate(System.Runtime.CompilerServices.Unsafe.Add(ref reference, i)))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Counts the number of elements in the read-only span that satisfy the specified condition.
    /// This method provides LINQ-like Count functionality for ReadOnlySpan&lt;T&gt; with optimal performance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The read-only span to count elements in.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The number of elements that satisfy the condition.</returns>
    /// <exception cref="ArgumentNullException">Thrown when predicate is null.</exception>
    public static int Count<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        int count = 0;

        // Use ref iteration for maximum performance
        ref T reference = ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span);

        for (int i = 0; i < span.Length; i++)
        {
            if (predicate(System.Runtime.CompilerServices.Unsafe.Add(ref reference, i)))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Alternative implementation using simple indexing (slightly less optimal but more readable).
    /// Counts the number of elements in the span that satisfy the specified condition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to count elements in.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The number of elements that satisfy the condition.</returns>
    /// <exception cref="ArgumentNullException">Thrown when predicate is null.</exception>
    public static int CountSimple<T>(this Span<T> span, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        int count = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (predicate(span[i]))
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Alternative implementation for ReadOnlySpan using simple indexing.
    /// Counts the number of elements in the read-only span that satisfy the specified condition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The read-only span to count elements in.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The number of elements that satisfy the condition.</returns>
    /// <exception cref="ArgumentNullException">Thrown when predicate is null.</exception>
    public static int CountSimple<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        int count = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (predicate(span[i]))
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Additional useful extension: Any method for Span&lt;T&gt;.
    /// Determines whether any element in the span satisfies the specified condition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to check.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>true if any element satisfies the condition; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when predicate is null.</exception>
    public static bool Any<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < span.Length; i++)
        {
            if (predicate(span[i]))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Additional useful extension: All method for Span&lt;T&gt;.
    /// Determines whether all elements in the span satisfy the specified condition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to check.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>true if all elements satisfy the condition; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when predicate is null.</exception>
    public static bool All<T>(this ReadOnlySpan<T> span, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < span.Length; i++)
        {
            if (!predicate(span[i]))
            {
                return false;
            }
        }
        return true;
    }
}
