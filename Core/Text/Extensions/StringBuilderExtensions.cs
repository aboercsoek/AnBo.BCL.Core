//--------------------------------------------------------------------------
// File:    StringBuilderExtensions.cs
// Content:	Implementation of class StringBuilderExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

#endregion

namespace AnBo.Core;

/// <summary>
/// Provides modern extension methods for <see cref="StringBuilder"/> optimized for .NET 8+.
/// </summary>
public static class StringBuilderExtensions
{
    #region Clear Methods

    /// <summary>
    /// Clears the content of the specified string builder efficiently.
    /// </summary>
    /// <param name="builder">The string builder to clear. Cannot be null.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder ClearBuilder(this StringBuilder builder)
    {
        ArgChecker.ShouldNotBeNull(builder);

        builder.Length = 0;
        return builder;
    }

    #endregion

    #region Null or Empty Checks

    /// <summary>
    /// Determines whether the specified string builder is null or empty.
    /// </summary>
    /// <param name="builder">The string builder to check.</param>
    /// <returns>
    /// <see langword="true"/> if the builder is null or has zero length; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this StringBuilder? builder)
        => builder is null || builder.Length == 0;

    /// <summary>
    /// Determines whether the specified string builder is empty (but not null).
    /// </summary>
    /// <param name="builder">The string builder to check. Cannot be null.</param>
    /// <returns><see langword="true"/> if the builder has zero length; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this StringBuilder builder)
    {
        ArgChecker.ShouldNotBeNull(builder);
        return builder.Length == 0;
    }

    #endregion

    #region Invariant Append Methods

    /// <summary>
    /// Appends the string representation of the specified value using invariant culture.
    /// </summary>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="value">The value to append. Null values are ignored for reference types.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
    public static StringBuilder AppendInvariant<T>(this StringBuilder builder, T? value)
    {
        ArgChecker.ShouldNotBeNull(builder);

        return value switch
        {
            null => builder, // Skip null values
            string str => builder.Append(str),
            IFormattable formattable => builder.Append(formattable.ToString(null, CultureInfo.InvariantCulture)),
            _ => builder.Append(value.ToString())
        };
    }

    /// <summary>
    /// Appends the string representation of the specified value using invariant culture if value is formattable.
    /// or StringConversionHelper.ToInvariantString<T>.
    /// </summary>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="value">The value to append. Null values are ignored for reference types.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
    public static StringBuilder AppendUseToInvariantString<T>(this StringBuilder builder, T? value)
    {
        ArgChecker.ShouldNotBeNull(builder);

        return value switch
        {
            null => builder, // Skip null values
            string str => builder.Append(str),
            IFormattable formattable => builder.Append(formattable.ToString(null, CultureInfo.InvariantCulture)),
            _ => builder.Append(value.ToInvariantString())
        };
    }

    #endregion

    #region Conditional Append Methods

    /// <summary>
    /// Appends the specified value only if the condition is true.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to append if condition is true.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string? value)
    {
        ArgChecker.ShouldNotBeNull(builder);

        return condition && value is not null ? builder.Append(value) : builder;
    }

    /// <summary>
    /// Appends the specified value only if the condition function returns true.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="condition">The condition function to evaluate.</param>
    /// <param name="value">The value to append if condition returns true.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="condition"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendIf(this StringBuilder builder, Func<bool> condition, string? value)
    {
        ArgChecker.ShouldNotBeNull(builder);
        ArgChecker.ShouldNotBeNull(condition);

        return condition() && value is not null ? builder.Append(value) : builder;
    }

    #endregion

    #region Prepend Methods

    /// <summary>
    /// Inserts the specified character at the beginning of the string builder.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="value">The character to insert at the beginning.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Prepend(this StringBuilder builder, char value)
    {
        ArgChecker.ShouldNotBeNull(builder);

        return builder.Insert(0, value);
    }

    /// <summary>
    /// Inserts the specified character at the beginning of the string builder.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="value">The string to insert at the beginning.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="value"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Prepend(this StringBuilder builder, string value)
    {
        ArgChecker.ShouldNotBeNull(builder);
        ArgChecker.ShouldNotBeNull(value);

        return builder.Insert(0, value);
    }

    #endregion

    #region AppendJoinCollection Methods

    /// <summary>
    /// Appends a sequence of values separated by the specified separator.
    /// </summary>
    /// <typeparam name="T">The type of values to join.</typeparam>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="separator">The separator to use between values.</param>
    /// <param name="values">The values to join and append.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> or <paramref name="values"/> is null.</exception>
    public static StringBuilder AppendJoinCollection<T>(this StringBuilder builder, string? separator, IEnumerable<T> values)
    {
        ArgChecker.ShouldNotBeNull(builder);
        ArgChecker.ShouldNotBeNull(values);

        using var enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext())
            return builder;

        // Append first element
        builder.AppendInvariant(enumerator.Current);

        // Append remaining elements with separator
        while (enumerator.MoveNext())
        {
            if (separator is not null)
                builder.Append(separator);
            builder.AppendInvariant(enumerator.Current);
        }

        return builder;
    }

    /// <summary>
    /// Appends a sequence of values separated by the specified character.
    /// </summary>
    /// <typeparam name="T">The type of values to join.</typeparam>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="separator">The separator character to use between values.</param>
    /// <param name="values">The values to join and append.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> or <paramref name="values"/> is null.</exception>
    public static StringBuilder AppendJoinCollection<T>(this StringBuilder builder, char separator, IEnumerable<T> values)
    {
        ArgChecker.ShouldNotBeNull(builder);
        ArgChecker.ShouldNotBeNull(values);

        using var enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext())
            return builder;

        // Append first element
        builder.AppendInvariant(enumerator.Current);

        // Append remaining elements with separator
        while (enumerator.MoveNext())
        {
            builder.Append(separator);
            builder.AppendInvariant(enumerator.Current);
        }

        return builder;
    }

    #endregion

    #region Conditional AppendLine Methods

    /// <summary>
    /// Appends a line terminator if the string builder is not empty.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendLineIf(this StringBuilder builder)
    {
        ArgChecker.ShouldNotBeNull(builder);
        return builder.Length > 0 ? builder.AppendLine() : builder;
    }

    /// <summary>
    /// Appends the specified value followed by a line terminator if the condition is true.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to append if condition is true.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendLineIf(this StringBuilder builder, bool condition, string? value)
    {
        ArgChecker.ShouldNotBeNull(builder);
        return condition && value is not null ? builder.AppendLine(value) : builder;
    }

    #endregion

    #region Trim Methods

    /// <summary>
    /// Removes trailing whitespace characters from the string builder.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <returns>The same StringBuilder instance for method chaining.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
    public static StringBuilder TrimEnd(this StringBuilder builder)
    {
        ArgChecker.ShouldNotBeNull(builder);

        int length = builder.Length;
        while (length > 0 && char.IsWhiteSpace(builder[length - 1]))
        {
            length--;
        }

        builder.Length = length;
        return builder;
    }

    #endregion

    #region AsSpan Methods

    /// <summary>
    /// Gets a read-only span representing the current content of the string builder.
    /// This provides efficient access without creating intermediate strings.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <returns>A read-only span of characters representing the builder's content.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this StringBuilder builder)
    {
        ArgChecker.ShouldNotBeNull(builder);
        return builder.ToString().AsSpan();
    }

    /// <summary>
    /// Gets a read-only span representing a portion of the string builder's content.
    /// </summary>
    /// <param name="builder">The string builder. Cannot be null.</param>
    /// <param name="start">The starting index.</param>
    /// <param name="length">The number of characters to include.</param>
    /// <returns>A read-only span of characters representing the specified portion.</returns>
    /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when start or length are invalid.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpan(this StringBuilder builder, int start, int length)
    {
        ArgChecker.ShouldNotBeNull(builder);
        return builder.ToString().AsSpan(start, length);
    }

    #endregion


}
