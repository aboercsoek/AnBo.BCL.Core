//--------------------------------------------------------------------------
// File:    StringExtensions.Is.cs
// Content:	Implementation of class StringExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents modern extension methods for <see cref="String"/> type optimized for .NET 8+.
/// </summary>
public static partial class StringExtensions
{
    #region IsFormatString method

    // <summary>
    /// Determines whether a string is a format string by checking for format placeholders.
    /// </summary>
    /// <param name="text">The source string to check.</param>
    /// <returns>
    /// <see langword="true"/> if the string contains format placeholders; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFormatString([NotNullWhen(true)] this string? text)
    {
        return !string.IsNullOrEmpty(text) && FormatStringRegex().IsMatch(text);
    }

    #endregion

    #region Is Null and Empty string extensions

    // <summary>
    /// Determines whether a string is null, empty, or contains only whitespace characters.
    /// </summary>
    /// <param name="text">The source string to check.</param>
    /// <returns>
    /// <see langword="true"/> if the string is <see langword="null"/>, empty, or whitespace; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmptyWithTrim([NotNullWhen(false)] this string? text)
    {
        return string.IsNullOrWhiteSpace(text);
    }

    /// <summary>
    /// Determines whether the string is empty (but not null).
    /// </summary>
    /// <param name="text">The source string to check.</param>
    /// <returns>
    /// <see langword="true"/> if the string is empty; otherwise <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty([NotNullWhen(false)] this string? text)
    {
        // Check if the string ist empty (but not null).
        // If text ist null, the condition is null == 0, which is false.
        // Length ist checked only if text is not null.
        return text?.Length == 0;
    }

    #endregion

    #region IsEqual methods

    /// <summary>
    /// Determines whether two strings are equal, ignoring case.
    /// </summary>
    /// <param name="sourceText">The source text.</param>
    /// <param name="targetText">The target text.</param>
    /// <returns>
    /// <see langword="true"/> if the strings are equal (case-insensitive); otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualIgnoreCase(this string? sourceText, string? targetText)
    {
        return string.Equals(sourceText, targetText, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether two strings are equal using the specified comparison mode.
    /// </summary>
    /// <param name="sourceText">The source text.</param>
    /// <param name="targetText">The target text.</param>
    /// <param name="compareMode">The string comparison mode.</param>
    /// <returns>
    /// <see langword="true"/> if the strings are equal; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqual(this string? sourceText, string? targetText, StringComparison compareMode)
    {
        return string.Equals(sourceText, targetText, compareMode);
    }

    #endregion

    #region IsTypeSpan- and IsTypeString methods

    /// <summary>
    /// Determines if the specified span can be parsed as the specified type T.
    /// This version is more efficient for substring operations.
    /// </summary>
    /// <typeparam name="T">The type that implements ISpanParsable&lt;T&gt;</typeparam>
    /// <param name="span">The span to check.</param>
    /// <param name="provider">The format provider to use. If null, uses InvariantCulture.</param>
    /// <returns>
    /// 	<see langword="true"/> if the span can be parsed as type T; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsTypeSpan<T>(this ReadOnlySpan<char> span, IFormatProvider? provider = null)
        where T : ISpanParsable<T>
    {
        provider ??= CultureInfo.InvariantCulture;
        return T.TryParse(span, provider, out _);
    }

    /// <summary>
    /// Determines if the specified string can be parsed as the specified type T.
    /// </summary>
    /// <typeparam name="T">The type that implements ISpanParsable&lt;T&gt;</typeparam>
    /// <param name="str">The string to check.</param>
    /// <param name="provider">The format provider to use. If null, uses InvariantCulture.</param>
    /// <returns>
    /// 	<see langword="true"/> if the string can be parsed as type T; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsTypeString<T>(this string? str, IFormatProvider? provider = null)
        where T : ISpanParsable<T>
    {
        if (string.IsNullOrEmpty(str))
            return false;

        return str.AsSpan().IsTypeSpan<T>(provider);
    }

    #endregion

}
