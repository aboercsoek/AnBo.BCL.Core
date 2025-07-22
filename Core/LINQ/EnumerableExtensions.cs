//--------------------------------------------------------------------------
// File:    EnumerableExtensions.cs
// Content:	Implementation of class EnumerableExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents extension methods for <see cref="IEnumerable{T}"/> types.
/// </summary>
public static class EnumerableExtensions
{
    #region To... extension methods

    /// <summary>
    /// Converts a sequence of type <see cref="IEnumerable{T}"/> 
    /// to type <see cref="ObservableCollection{T}"/>. 
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="source">The sequence to convert.</param>
    /// <returns>
    /// The conversion result as <see cref="ObservableCollection{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <remarks>ObservableCollection are useful for data binding in WPF and other XAML-based frameworks.</remarks>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new ObservableCollection<T>(source);
    }

    #endregion

    #region Equality extension methods

    /// <summary>
    /// Determines whether two sequences are equal by comparing count and elements using a custom comparison function.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequences.</typeparam>
    /// <param name="first">First sequence to compare.</param>
    /// <param name="second">Second sequence to compare.</param>
    /// <param name="compare">The comparison function to use for element comparison.</param>
    /// <returns>
    /// <c>true</c> if sequences are equal in length and all corresponding elements are equal
    /// according to the comparison function; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> compare)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(compare);

        if (ReferenceEquals(first, second))
            return true;

        // Try to get counts efficiently
        if (first.TryGetNonEnumeratedCount(out int firstCount) &&
            second.TryGetNonEnumeratedCount(out int secondCount) &&
            firstCount != secondCount)
        {
            return false;
        }

        // Enumerators are disposable, so we use 'using' statements to ensure they are disposed of properly.
        using var firstEnumerator = first.GetEnumerator();
        using var secondEnumerator = second.GetEnumerator();

        while (true)
        {
            bool firstHasNext = firstEnumerator.MoveNext();
            bool secondHasNext = secondEnumerator.MoveNext();

            if (firstHasNext != secondHasNext)
                return false;

            if (!firstHasNext)
                return true;

            if (!compare(firstEnumerator.Current, secondEnumerator.Current))
                return false;
        }
    }

    #endregion

    #region Element existence and counting extension methods

    /// <summary>
    /// Checks if the collection can be proven empty without enumeration.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection to check.</param>
    /// <returns>
    /// <c>true</c> if the collection can be proven empty without enumeration; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This uses .NET 8's TryGetNonEnumeratedCount for efficient empty checking without triggering enumeration.
    /// Useful for performance-sensitive scenarios where you need to avoid deferred execution costs.
    /// </remarks>
    public static bool CanBeProvenEmptyFast<T>(this IEnumerable<T> source)
        => source.TryGetNonEnumeratedCount(out int count) && count == 0;

    /// <summary>
    /// Efficiently determines if any element exists in a sequence without fully enumerating it when possible.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>
    /// <c>true</c> if the sequence contains at least one element; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <remarks>
    /// This method uses .NET 8's TryGetNonEnumeratedCount for optimal performance when possible.
    /// For collections with known counts, this avoids enumeration overhead.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasElements<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        // Fast path: use count if available without enumeration
        if (source.TryGetNonEnumeratedCount(out int count))
            return count > 0;

        // Fallback: check if we can get the first element
        return source.Any();
    }

    /// <summary>
    /// Gets the count of elements efficiently, avoiding enumeration when possible.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>The number of elements in the sequence.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <remarks>
    /// This method uses .NET 8's TryGetNonEnumeratedCount for optimal performance when possible.
    /// Only falls back to full enumeration if the count cannot be determined efficiently.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCountEfficiently<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        // Fast path: use count if available without enumeration
        if (source.TryGetNonEnumeratedCount(out int count))
            return count;

        // Fallback: enumerate to count
        return source.Count();
    }

    #endregion

    #region Pairing and batching extension methods

    /// <summary>
    /// Creates parent-child tuple pairs from a collection and a child selector function.
    /// </summary>
    /// <typeparam name="TParent">The type of parent elements.</typeparam>
    /// <typeparam name="TChild">The type of child elements.</typeparam>
    /// <param name="parents">The parent collection.</param>
    /// <param name="childSelector">Function to select children for each parent.</param>
    /// <returns>An enumerable of (parent, child) tuples.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IEnumerable<(TParent, TChild)> SelectManyPairs<TParent, TChild>(
        this IEnumerable<TParent> parents,
        Func<TParent, IEnumerable<TChild>> childSelector)
    {
        ArgumentNullException.ThrowIfNull(parents);
        ArgumentNullException.ThrowIfNull(childSelector);


        return from parent in parents
               from child in childSelector(parent)
               select (parent, child);
    }

    /// <summary>
    /// Creates a memory-efficient batch processor for large sequences.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An enumerable of batches, where each batch is an array of elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="batchSize"/> is less than 1.</exception>
    /// <remarks>
    /// This method is useful for processing large sequences in chunks to manage memory usage
    /// or implement batch processing patterns efficiently.
    /// </remarks>
    public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(batchSize, 1);

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var batch = new List<T>(batchSize) { enumerator.Current };

            // Fill the rest of the batch
            for (int i = 1; i < batchSize && enumerator.MoveNext(); i++)
            {
                batch.Add(enumerator.Current);
            }

            yield return batch.ToArray();
        }
    }

    #endregion

    #region Conditional execution extension methods

    /// <summary>
    /// Executes conditional logic on each element in the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="IfPredicate">The condition predicate.</param>
    /// <param name="thenAction">The action to execute when predicate is true.</param>
    /// <returns>
    /// <c>true</c> if no exceptions were thrown during execution; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <remarks>
    /// This provides functional programming style conditional execution over collections.
    /// Unlike standard LINQ, this method performs side effects and handles exceptions gracefully.
    /// </remarks>
    public static bool IfThen<T>(this IEnumerable<T> source, Func<T, bool> ifPredicate, Action<T> thenAction)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(ifPredicate);
        ArgumentNullException.ThrowIfNull(thenAction);

        bool success = true;

        foreach (T element in source)
        {
            try
            {
                if (ifPredicate(element))
                    thenAction(element);
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                success = false;
            }
        }

        return success;
    }

    /// <summary>
    /// Executes conditional logic with else branch on each element in the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="ifPredicate">The condition predicate.</param>
    /// <param name="thenAction">The action to execute when predicate is true.</param>
    /// <param name="elseAction">The action to execute when predicate is false.</param>
    /// <returns>
    /// <c>true</c> if no exceptions were thrown during execution; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <remarks>
    /// This provides functional programming style conditional execution with else branches over collections.
    /// Unlike standard LINQ, this method performs side effects and handles exceptions gracefully.
    /// </remarks>
    public static bool IfThenElse<T>(
        this IEnumerable<T> source,
        Func<T, bool> ifPredicate,
        Action<T> thenAction,
        Action<T> elseAction)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(ifPredicate);
        ArgumentNullException.ThrowIfNull(thenAction);
        ArgumentNullException.ThrowIfNull(elseAction);

        bool success = true;

        foreach (T element in source)
        {
            try
            {
                if (ifPredicate(element))
                    thenAction(element);
                else
                    elseAction(element);

            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                success = false;
            }
        }

        return success;
    }

    #endregion

    // --------------------------------------------------------------------------------------------------------
    // ToHashSet has been removed, because it is available in .NET Core 2.0 and later.
    // --------------------------------------------------------------------------------------------------------

    #region Indexing and iteration extension methods

    /// <summary>
    /// Enumerates a sequence with indices, creating IndexValuePair elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>An enumerable of of <see cref="IndexValuePair{T}"/> items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <remarks>If you prefer using tuple, consider using Select((value, index) => (value, index)) instead.</remarks>
    public static IEnumerable<IndexValuePair<T>> WithIndex<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Select((item, index) => IndexValuePair.Create(item, index));
    }

    /// <summary>
    /// Executes an action on each element in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="action">The action to execute on each element.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is null.</exception>
    /// <remarks>
    /// If <paramref name="source"/> is null, the method returns without throwing an exception.
    /// </remarks>
    public static void Foreach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            return; // Don't throw an error is source items are null. Just exit the method.

        ArgumentNullException.ThrowIfNull(action);

        foreach (T element in source)
        {
            action(element);
        }
    }

    /// <summary>
    /// Executes an action on each element in the sequence, providing the current index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="action">The action to execute on each element and its index.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is null.</exception>
    /// <remarks>
    /// If <paramref name="source"/> is null, the method returns without throwing an exception.
    /// </remarks>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        if (source == null)
            return; // Don't throw an error is source items are null. Just exit the method.

        ArgumentNullException.ThrowIfNull(action);

        int index = 0;
        foreach (T element in source)
        {
            action(element, index++);
        }
    }

    #endregion

    // --------------------------------------------------------------------------------------------------------
    // As or Cast methods are available through the OfType extension methods in .NET 3.5 and .NET 5+ and later.
    // --------------------------------------------------------------------------------------------------------

    // --------------------------------------------------------------------------------------------------------
    // ConvertAll has been removed. .NET provides Select methods
    // --------------------------------------------------------------------------------------------------------

    // --------------------------------------------------------------------------------------------------------
    // Sort and Complement have been removed. .NET provides Order, OrderDescending, and Except methods
    // --------------------------------------------------------------------------------------------------------

    #region Disposal and collection extension methods

    /// <summary>
    /// Disposes all disposable elements in the sequence.
    /// </summary>
    /// <typeparam name="T">The element type, which must implement <see cref="IDisposable"/>.</typeparam>
    /// <param name="source">The sequence of disposable elements.</param>
    public static void DisposeAll<T>(this IEnumerable<T>? source)
        where T : IDisposable
    {
        if (source is null)
            return;

        foreach (T element in source)
            element?.Dispose();
    }

    /// <summary>
    /// Adds multiple elements to a collection.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="collection">The target collection.</param>
    /// <param name="elements">The elements to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is null.</exception>
    public static void AddRangeSafe<T>(this ICollection<T> collection, IEnumerable<T>? elements)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (elements is null)
            return;

        // Optimize for known collection types
        switch (collection)
        {
            case List<T> list:
                list.AddRange(elements);
                return;
            default:
                foreach (T element in elements)
                    collection.Add(element);
                break;
        }
    }

    #endregion

    #region Indexing and searching extension methods

    /// <summary>
    /// Finds the index of an element in the sequence using a custom equality comparer.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="item">The item to find.</param>
    /// <param name="comparer">Optional equality comparer.</param>
    /// <returns>The zero-based index of the item, or -1 if not found.</returns>
    /// <remarks>
    /// Unlike LINQ's FirstOrDefault with index, this method returns the actual index position
    /// and allows custom equality comparers for complex scenarios.
    /// </remarks>
    public static int IndexOf<T>(
        this IEnumerable<T>? source,
        T item,
        IEqualityComparer<T>? comparer = null)
    {
        if (source is null)
            return -1;

        comparer ??= EqualityComparer<T>.Default;
        int index = 0;

        foreach (T element in source)
        {
            if (comparer.Equals(element, item))
                return index;
            index++;
        }

        return -1;
    }

    /// <summary>
    /// Finds the index of the first element matching a predicate.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>The zero-based index of the first matching element, or -1 if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
    /// <remarks>
    /// This complements LINQ's Where().FirstOrDefault() pattern by returning the index position
    /// instead of the element itself.
    /// </remarks>
    public static int FindIndex<T>(this IEnumerable<T>? source, Func<T, bool> predicate)
    {
        if (source is null)
            return -1;

        ArgumentNullException.ThrowIfNull(predicate);

        int index = 0;
        foreach (T element in source)
        {
            if (predicate(element))
                return index;
            index++;
        }

        return -1;
    }

    /// <summary>
    /// Finds the index of the last occurrence of an item in a sequence.
    /// </summary>
    /// <typeparam name="T">The element type that implements <see cref="IEquatable{T}"/>.</typeparam>
    /// <param name="source">The source sequence to search.</param>
    /// <param name="value">The value to find the last index for.</param>
    /// <returns>
    /// The zero-based index of the last occurrence of the item, or -1 if not found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <remarks>
    /// This method provides efficient last-index searching that LINQ doesn't offer directly.
    /// For better performance with large sequences, consider using indexed collections like arrays or lists.
    /// </remarks>
    public static int FindLastIndex<T>(this IEnumerable<T> source, T value)
        where T : IEquatable<T>
    {
        ArgumentNullException.ThrowIfNull(source);

        int lastIndex = -1;
        int currentIndex = 0;

        foreach (T item in source)
        {
            if (item.Equals(value))
                lastIndex = currentIndex;
            currentIndex++;
        }

        return lastIndex;
    }

    /// <summary>
    /// Finds the index of the last occurrence of an item matching a predicate.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence to search.</param>
    /// <param name="predicate">The predicate to match elements.</param>
    /// <returns>
    /// The zero-based index of the last matching element, or -1 if no match is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <remarks>
    /// This complements LINQ's Where().LastOrDefault() by returning the index position
    /// instead of the element itself.
    /// </remarks>
    public static int FindLastIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        int lastIndex = -1;
        int currentIndex = 0;

        foreach (T item in source)
        {
            if (predicate(item))
                lastIndex = currentIndex;
            currentIndex++;
        }

        return lastIndex;
    }

    #endregion

    #region Null-safe collection conversion methods

    /// <summary>
    /// Converts a potentially null enumerable to an array safely.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source enumerable.</param>
    /// <returns>An array containing the elements, or an empty array if source is null.</returns>
    /// <remarks>
    /// This provides null-safe array conversion using modern collection expressions.
    /// Useful in scenarios where you need to ensure a non-null array result.
    /// </remarks>
    public static T[] ToArraySafe<T>(this IEnumerable<T>? source)
        => source?.ToArray() ?? [];

    #endregion
}
