//--------------------------------------------------------------------------
// File:    ArrayExtensions.cs
// Content:	Implementation of Extensions class ArrayExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#endregion

/*
 * INFORMATION about the refactoring of ArrayExtensions für .NET 8+:
 * - Removed all LINQ duplicates: ConvertAll, SkipWhile, Take, TakeWhile, Skip, Concat, Reverse, Union, Except, Intersect, Distinct, Where, Sort
 * 
 * Added new methods for .NET 8+:
 * - Copy()/Slice()/SliceFrom() - Effiziente Array-Kopieroperationen mit Array.Copy
 * - Fill()-Überladungen - Nutzt .NET's Array.Fill für optimale Performance
 * - BinarySearch()-Überladungen - Effiziente O(log n) Suche in sortierten Arrays
 * - AllFast()/AnyFast()/FindIndexFast() - Performance-optimierte Varianten ohne LINQ-Overhead
 * - AsSpan()-Überladungen - Moderne Span-basierte High-Performance-Operationen
*/

namespace AnBo.Core;

/// <summary>
/// Represents extension methods for arrays optimized for .NET 8.
/// </summary>
public static class ArrayExtensions
{
    #region ToReadOnly Extensions

    /// <summary>
    /// Returns a <see ref="ReadOnlyCollection{T}">read-only wrapper</see> for the specified array.
    /// </summary>
    /// <typeparam name="T">Array type</typeparam>
    /// <param name="array">Source array of type T</param>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> of the source array.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    public static ReadOnlyCollection<T> ToReadOnly<T>(this T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        return new ReadOnlyCollection<T>(array);
    }

    #endregion

    #region Copy Extension Methods

    /// <summary>
    /// Creates a copy of the array with efficient memory allocation.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="source">The source array to copy.</param>
    /// <returns>A new array containing the same elements as the source.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <remarks>
    /// This method uses Array.Copy for optimal performance when creating array copies.
    /// Prefer this over LINQ ToArray() when you specifically need to copy an existing array.
    /// </remarks>
    public static T[] Copy<T>(this T[] source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new T[source.Length];
        Array.Copy(source, result, source.Length);
        return result;
    }

    /// <summary>
    /// Creates a copy of a portion of the array efficiently.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="source">The source array to copy from.</param>
    /// <param name="startIndex">The starting index in the source array.</param>
    /// <param name="length">The number of elements to copy.</param>
    /// <returns>A new array containing the specified portion of the source array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="startIndex"/> or <paramref name="length"/> is invalid.
    /// </exception>
    /// <remarks>
    /// This method provides efficient array slicing using Array.Copy.
    /// More efficient than LINQ Skip().Take().ToArray() for arrays.
    /// </remarks>
    public static T[] Slice<T>(this T[] source, int startIndex, int length)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, source.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + length, source.Length);

        var result = new T[length];
        Array.Copy(source, startIndex, result, 0, length);
        return result;
    }

    /// <summary>
    /// Creates a copy of the array from a starting index to the end.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="source">The source array to copy from.</param>
    /// <param name="startIndex">The starting index in the source array.</param>
    /// <returns>A new array containing elements from the start index to the end.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="startIndex"/> is invalid.
    /// </exception>
    /// <remarks>
    /// This method provides efficient array slicing from a start index to the end.
    /// More efficient than LINQ Skip().ToArray() for arrays.
    /// </remarks>
    public static T[] SliceFrom<T>(this T[] source, int startIndex)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, source.Length);

        int length = source.Length - startIndex;
        return source.Slice(startIndex, length);
    }

    #endregion

    #region Fill Extension Methods

    /// <summary>
    /// Efficiently fills an array with a specified value.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The array to fill.</param>
    /// <param name="value">The value to fill the array with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <remarks>
    /// This method uses Array.Fill (available in .NET Core 2.1+) for optimal performance.
    /// Modifies the original array in-place.
    /// </remarks>
    public static void Fill<T>(this T[] array, T value)
    {
        ArgumentNullException.ThrowIfNull(array);
        Array.Fill(array, value);
    }

    /// <summary>
    /// Efficiently fills a portion of an array with a specified value.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The array to fill.</param>
    /// <param name="value">The value to fill the array with.</param>
    /// <param name="startIndex">The starting index to begin filling.</param>
    /// <param name="count">The number of elements to fill.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="startIndex"/> or <paramref name="count"/> is invalid.
    /// </exception>
    /// <remarks>
    /// This method uses Array.Fill for optimal performance.
    /// Modifies the original array in-place.
    /// </remarks>
    public static void Fill<T>(this T[] array, T value, int startIndex, int count)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, array.Length);

        Array.Fill(array, value, startIndex, count);
    }

    #endregion

    #region BinarySearch Extension Methods

    /// <summary>
    /// Efficiently searches for an element in a sorted array using binary search with a custom comparer.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The sorted array to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <param name="comparer">The comparer to use for comparing elements.</param>
    /// <returns>
    /// The index of the value if found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than the value.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <remarks>
    /// The array must be sorted according to the comparer for this method to work correctly.
    /// Uses Array.BinarySearch for O(log n) performance.
    /// </remarks>
    public static int BinarySearch<T>(this T[] array, T value, IComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(array);
        return Array.BinarySearch(array, value, comparer);
    }

    /// <summary>
    /// Efficiently searches for an element in a portion of a sorted array using binary search.
    /// </summary>
    /// <typeparam name="T">The array element type that implements <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="array">The sorted array to search.</param>
    /// <param name="index">The starting index of the range to search.</param>
    /// <param name="length">The length of the range to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <param name="comparer">The comparer to use for comparing elements.</param>
    /// <returns>
    /// The index of the value if found; otherwise, a negative number that is the bitwise complement
    /// of the index of the next element that is larger than the value.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> or <paramref name="length"/> is invalid.
    /// </exception>
    /// <remarks>
    /// The specified range must be sorted in ascending order for this method to work correctly.
    /// Uses Array.BinarySearch for O(log n) performance.
    /// </remarks>
    public static int BinarySearch<T>(this T[] array, int index, int length, T value, IComparer<T>? comparer = null)
        where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index + length, array.Length);

        return Array.BinarySearch(array, index, length, value, comparer);
    }

    #endregion

    #region Fast LINQ-like Methods

    /// <summary>
    /// Efficiently determines whether all elements in the array satisfy a condition without LINQ overhead.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The array to check.</param>
    /// <param name="predicate">The condition to test each element against.</param>
    /// <returns><c>true</c> if all elements satisfy the condition; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <remarks>
    /// This method provides optimized all-checking for arrays with early termination.
    /// Uses direct array indexing for better performance than LINQ All() on large arrays.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllFast<T>(this T[] array, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < array.Length; i++)
        {
            if (!predicate(array[i]))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Efficiently determines whether any element in the array satisfies a condition without LINQ overhead.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The array to check.</param>
    /// <param name="predicate">The condition to test each element against.</param>
    /// <returns><c>true</c> if any element satisfies the condition; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <remarks>
    /// This method provides optimized any-checking for arrays with early termination.
    /// Uses direct array indexing for better performance than LINQ Any() on large arrays.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyFast<T>(this T[] array, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < array.Length; i++)
        {
            if (predicate(array[i]))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Efficiently finds the first index where a condition is met without LINQ overhead.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The array to search.</param>
    /// <param name="predicate">The condition to test each element against.</param>
    /// <returns>The index of the first matching element, or -1 if no match is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <remarks>
    /// This method provides optimized index finding for arrays.
    /// Uses direct array indexing for better performance than LINQ-based solutions on large arrays.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FindIndexFast<T>(this T[] array, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < array.Length; i++)
        {
            if (predicate(array[i]))
                return i;
        }
        return -1;
    }

    #endregion

}
