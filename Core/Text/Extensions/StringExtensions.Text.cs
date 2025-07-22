//--------------------------------------------------------------------------
// File:    StringExtensions.Text.cs
// Content:	Implementation of class StringExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Runtime.CompilerServices;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents modern extension methods for <see cref="String"/> type optimized for .NET 8+.
/// </summary>
public static partial class StringExtensions
{
    #region Text formatting extensions

    /// <summary>
    /// Appends a line to the existing text with proper line ending handling.
    /// </summary>
    /// <param name="text">The existing text.</param>
    /// <param name="line">The line to append.</param>
    /// <returns>The text with the appended line.</returns>
    public static string AppendLine(this string? text, string line)
    {
        var safeText = text ?? string.Empty;

        if (safeText.Length == 0 || safeText.EndsWith(Environment.NewLine))
            return safeText + line;

        return safeText + Environment.NewLine + line;
    }

    /// <summary>
    /// Converts a string to camelCase (first letter lowercase).
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The camelCase formatted string.</returns>
    public static string ToCamelCase(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
        if (value.Length == 1) return value.ToLowerInvariant();

        var span = value.AsSpan();
        var firstNonWhitespace = 0;

        // Find first non-whitespace character
        while (firstNonWhitespace < span.Length &&
               (span[firstNonWhitespace] == ' ' || span[firstNonWhitespace] == '\t'))
        {
            firstNonWhitespace++;
        }

        if (firstNonWhitespace >= span.Length) return value;

        return string.Create(value.Length, (value, firstNonWhitespace),
            static (span, state) =>
            {
                state.value.AsSpan().CopyTo(span);
                span[state.firstNonWhitespace] = char.ToLowerInvariant(span[state.firstNonWhitespace]);
            });
    }

    /// <summary>
    /// Converts a string to PascalCase (first letter uppercase).
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The PascalCase formatted string.</returns>
    public static string ToPascalCase(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
        if (value.Length == 1) return value.ToUpperInvariant();

        var span = value.AsSpan();
        var firstNonWhitespace = 0;

        // Find first non-whitespace character
        while (firstNonWhitespace < span.Length &&
               (span[firstNonWhitespace] == ' ' || span[firstNonWhitespace] == '\t'))
        {
            firstNonWhitespace++;
        }

        if (firstNonWhitespace >= span.Length) return value;

        return string.Create(value.Length, (value, firstNonWhitespace),
            static (span, state) =>
            {
                state.value.AsSpan().CopyTo(span);
                span[state.firstNonWhitespace] = char.ToUpperInvariant(span[state.firstNonWhitespace]);
            });
    }

    /// <summary>
    /// Converts a string from the source encoding to Unicode.
    /// </summary>
    /// <param name="sourceText">The source text.</param>
    /// <param name="sourceEncoding">The source encoding.</param>
    /// <returns>The Unicode-encoded string.</returns>
    public static string ToUnicodeString(this string? sourceText, Encoding? sourceEncoding)
    {
        if (string.IsNullOrEmpty(sourceText) || sourceEncoding is null)
            return sourceText ?? string.Empty;

        if (sourceEncoding.Equals(Encoding.Unicode))
            return sourceText;

        ReadOnlySpan<byte> sourceBytes = sourceEncoding.GetBytes(sourceText);
        return Encoding.Unicode.GetString(sourceBytes);
    }

    /// <summary>
    /// Ensures that a string starts with the specified prefix.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="prefix">The required prefix.</param>
    /// <returns>The string with the prefix ensured.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EnsureStartsWith(this string? value, string prefix)
    {
        value ??= string.Empty;
        return value.StartsWith(prefix) ? value : prefix + value;
    }

    /// <summary>
    /// Ensures that a string ends with the specified suffix.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="suffix">The required suffix.</param>
    /// <returns>The string with the suffix ensured.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EnsureEndsWith(this string? value, string suffix)
    {
        value ??= string.Empty;
        return value.EndsWith(suffix) ? value : value + suffix;
    }

    /// <summary>
    /// Quotes a string if it contains spaces or is null/empty.
    /// </summary>
    /// <param name="s">The string to potentially quote.</param>
    /// <returns>The quoted string if necessary.</returns>
    public static string QuoteIfNeeded(this string s)
    {
        ArgumentNullException.ThrowIfNull(s);
        //if (s is null) return "<NULL>";

        if (s.Length == 0 || s.Contains(' '))
        {
            if (s.Length > 1 && s[0] == '“' && s[^1] == '”')
                return s;
            return $"“{s}”";
        }
        return s;
    }

    /// <summary>
    /// Formats a PascalCase string as a readable sentence.
    /// </summary>
    /// <param name="value">The PascalCase string.</param>
    /// <returns>A formatted sentence.</returns>
    public static string FormatAsSentence(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        var estimatedLength = value.Length + (value.Length / 3); // Estimate for spaces
        return string.Create(estimatedLength, value, static (span, input) =>
        {
            var length = 0;
            var inputSpan = input.AsSpan();

            for (var i = 0; i < inputSpan.Length; i++)
            {
                var c = inputSpan[i];
                if (char.IsUpper(c) && i > 0)
                {
                    if (length < span.Length) span[length++] = ' ';
                }
                if (length < span.Length)
                    span[length++] = char.ToLowerInvariant(c);
                //span[length++] = i == 0 ? char.ToLowerInvariant(c) : c;
            }
        }).TrimEnd('\0');
    }

    /// <summary>
    /// Replaces null characters with their escaped representation.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A string with null characters escaped.</returns>
    public static string ReplaceNullChars(this string? input)
    {
        if (string.IsNullOrEmpty(input)) return input ?? string.Empty;

        var nullCharIndex = input.IndexOf('\0');
        if (nullCharIndex == -1) return input;

        return input.Replace("\0", "\\0");
    }

    #endregion
}
