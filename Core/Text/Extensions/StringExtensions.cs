//--------------------------------------------------------------------------
// File:    StringExtensions.cs
// Content:	Implementation of class StringExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents modern extension methods for <see cref="String"/> type optimized for .NET 8+.
/// </summary>
public static partial class StringExtensions
{
    #region Cached Regex Patterns

    [GeneratedRegex(@"\{0", RegexOptions.Compiled)]
    private static partial Regex FormatStringRegex();

    [GeneratedRegex(@"[a-zA-Z0-9]", RegexOptions.Compiled)]
    private static partial Regex AlphaNumericRegex();

    [GeneratedRegex(@"[a-zA-Z]", RegexOptions.Compiled)]
    private static partial Regex AlphaCharactersRegex();

    [GeneratedRegex(@"[0-9,\.]", RegexOptions.Compiled)]
    private static partial Regex NumericWithPunctuationRegex();

    [GeneratedRegex(@"[0-9]", RegexOptions.Compiled)]
    private static partial Regex NumericOnlyRegex();

    #endregion

    #region Safe string extensions

    /// <summary>
    /// Gets the length of a string safely, returning -1 for null strings.
    /// </summary>
    /// <param name="text">The source string.</param>
    /// <returns>The string length, or -1 if the string is <see langword="null"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SafeLength(this string? text) => text?.Length ?? -1;

    /// <summary>
    /// Returns a safe string representation, using a default value for null strings.
    /// </summary>
    /// <param name="text">The source string.</param>
    /// <param name="defaultValue">The default value to use if the string is null.</param>
    /// <returns>The string value or the default value if null.</returns>
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SafeString(this string? text, string defaultValue = "")
        => text ?? defaultValue;

    /// <summary>
    /// Formats the string value with the <paramref name="parameters"/> and returns the result.
    /// </summary>
    /// <param name="value">The source string.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The format string result.</returns>
    [return: NotNull]
    [DebuggerStepThrough]
    public static string SafeFormatWith(this string? value, params object?[] parameters)
    {
        return StringHelper.SafeFormat(value, parameters);
    }

    #endregion

    #region Join extensions

    /// <summary>
    /// Joins the elements of a sequence into a single string using the specified separator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="items">The items to join.</param>
    /// <param name="separator">The separator to use between items.</param>
    /// <returns>A string containing all items separated by the separator.</returns>
    public static string Join<T>(this IEnumerable<T> items, string separator = ", ")
    {
        return StringHelper.Join(separator, items);
    }


    #endregion

    #region IndexOf Extensions

    /// <summary>
    /// Finds the first occurrence of a substring starting from the specified index.
    /// This method provides additional safety checks and consistent behavior.
    /// </summary>
    /// <param name="source">The string to search in.</param>
    /// <param name="value">The substring to search for.</param>
    /// <param name="startIndex">The start index of the search.</param>
    /// <param name="comparisonType">The type of comparison to perform.</param>
    /// <returns>The first index of the occurrence, or -1 if not found.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello World Hello";
    /// int index = text.SafeIndexOf("Hello", 6); // Returns 12 (second "Hello")
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SafeIndexOf(this string? source, string? value, int startIndex = 0,
        StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value) ||
            startIndex < 0 || startIndex >= source.Length)
            return -1;

        // Use native optimized method with proper comparison
        return source.IndexOf(value, startIndex, comparisonType);
    }

    /// <summary>
    /// Finds the last occurrence of a substring in the string.
    /// </summary>
    /// <param name="source">The string to search in.</param>
    /// <param name="value">The substring to search for.</param>
    /// <param name="startIndex">The start index to search backwards from (optional).</param>
    /// <param name="comparisonType">The type of comparison to perform.</param>
    /// <returns>The last index of the occurrence, or -1 if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SafeLastIndexOf(this string? source, string? value, int? startIndex = null,
        StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value)) return -1;

        return startIndex.HasValue
            ? source.LastIndexOf(value, startIndex.Value, comparisonType)
            : source.LastIndexOf(value, comparisonType);
    }

    #endregion

}
