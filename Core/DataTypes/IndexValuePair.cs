//--------------------------------------------------------------------------
// File:    IndexValuePair.cs
// Content:	Implementation of struct IndexValuePair
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Runtime.InteropServices;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents a value paired with its index position.
/// This structure is commonly used in enumeration scenarios where both 
/// the value and its position are needed.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <example>
/// <code>
/// var pair = new IndexValuePair&lt;string&gt;("Hello", 0);
/// Console.WriteLine($"Index: {pair.Index}, Value: {pair.Value}");
/// 
/// // Deconstruction support
/// var (index, value) = pair;
/// </code>
/// </example>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct IndexValuePair<T> : IEquatable<IndexValuePair<T>>
{
    /// <summary>
    /// Gets the zero-based index position of the value.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the value at the specified index.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexValuePair{T}"/> struct.
    /// </summary>
    /// <param name="value">The value to store.</param>
    /// <param name="index">The zero-based index of the value.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is negative.
    /// </exception>
    public IndexValuePair(T value, int index)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, int.MaxValue);

        Value = value;
        Index = index;
    }

    /// <summary>
    /// Deconstructs the pair into its index and value components.
    /// </summary>
    /// <param name="index">When this method returns, contains the index.</param>
    /// <param name="value">When this method returns, contains the value.</param>
    public void Deconstruct(out int index, out T value)
    {
        index = Index;
        value = Value;
    }

    /// <summary>
    /// Determines whether the specified <see cref="IndexValuePair{T}"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The other instance to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the specified instance is equal to this instance; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(IndexValuePair<T> other) =>
        Index == other.Index && EqualityComparer<T>.Default.Equals(Value, other.Value);

    /// <summary>
    /// Determines whether the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>
    /// <see langword="true"/> if the specified object is equal to this instance; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is IndexValuePair<T> other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(Index, Value);

    /// <summary>
    /// Returns a string representation of the index-value pair.
    /// </summary>
    /// <returns>A string in the format "[Index]: Value".</returns>
    public override string ToString() =>
        $"[{Index}]: {Value}";

    /// <summary>
    /// Determines whether two <see cref="IndexValuePair{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(IndexValuePair<T> left, IndexValuePair<T> right) =>
        left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="IndexValuePair{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(IndexValuePair<T> left, IndexValuePair<T> right) =>
        !left.Equals(right);

    /// <summary>
    /// Implicitly converts a tuple to an <see cref="IndexValuePair{T}"/>.
    /// </summary>
    /// <param name="tuple">The tuple containing value and index.</param>
    /// <returns>A new <see cref="IndexValuePair{T}"/> instance.</returns>
    public static implicit operator IndexValuePair<T>((T Value, int Index) tuple) =>
        new(tuple.Value, tuple.Index);

    /// <summary>
    /// Implicitly converts an <see cref="IndexValuePair{T}"/> to a tuple.
    /// </summary>
    /// <param name="pair">The index-value pair to convert.</param>
    /// <returns>A tuple containing the value and index.</returns>
    public static implicit operator (T Value, int Index)(IndexValuePair<T> pair) =>
        (pair.Value, pair.Index);
}

/// <summary>
/// Provides static factory methods for creating <see cref="IndexValuePair{T}"/> instances.
/// </summary>
public static class IndexValuePair
{
    /// <summary>
    /// Creates a new <see cref="IndexValuePair{T}"/> with the specified value and index.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to store.</param>
    /// <param name="index">The zero-based index of the value.</param>
    /// <returns>A new <see cref="IndexValuePair{T}"/> instance.</returns>
    public static IndexValuePair<T> Create<T>(T value, int index) =>
        new(value, index);
}
