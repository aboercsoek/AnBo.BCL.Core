//--------------------------------------------------------------------------
// File:    StringHelper.cs
// Content:	Implementation of String helper class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// String manipulation and generation methods, as well as string array manipulation.
/// Modernized for .NET 8+ with improved performance and nullable reference types.
/// </summary>
public static partial class StringHelper
{
    #region Private and Public Static Members

    /// <summary>
    /// Char array with default quote char (").
    /// </summary>
    public static readonly char[] DefaultQuoteSensitiveChars = ['"'];

    /// <summary>
    /// Thread-safe random number generator for string generation.
    /// </summary>
    private static readonly ThreadLocal<Random> ThreadLocalRandom = new(() => new Random());

    /// <summary>
    /// Compiled regex for removing characters (performance optimization).
    /// </summary>
    //[GeneratedRegex(@"[\r\n]+")]
    //private static partial Regex NewLineRegex();

    #endregion

    #region Safe ToString methods

    /// <summary>
    /// Safe ToString-Operation with fallback value.
    /// </summary>
    /// <param name="obj">The object to convert to string. Can be null.</param>
    /// <param name="defaultValue">The default value to use if obj is null or conversion fails. (default: empty string)</param>
    /// <returns>If <paramref name="obj"/> is null or conversion fails, returns the safe ToString value of <paramref name="defaultValue"/>; otherwise the value of obj.ToString().</returns>
    [return: NotNull]
    [DebuggerStepThrough]
    public static string SafeToString(object? obj, string defaultValue = "")
    {
        defaultValue ??= string.Empty;
        var options = new ToStringOptions();
        options.NullString = defaultValue;

        try
        {
            return obj?.ToInvariantString(options) ?? defaultValue;
        }
        catch (Exception ex) when (!ex.IsFatal())
        {
            return defaultValue;
        }
    }

    #endregion

    #region Safe string formating methods

    /// <summary>
    /// Safely formats a string with the specified format and arguments.
    /// Provides detailed error information if formatting fails.
    /// </summary>
    /// <param name="format">The format string. Can be null.</param>
    /// <param name="parameters">The parameter for formatting.</param>
    /// <returns>The formatted string or error information if formatting fails.</returns>
    [return: NotNull]
    [DebuggerStepThrough]
    public static string SafeFormat(string? format, params object?[] parameters)
    {
        if (string.IsNullOrEmpty(format) || parameters.Length == 0)
            return format ?? string.Empty;

        try
        {
            // Use string interpolation-friendly approach
            //var safeArgs = args.Select(arg => SafeToStrin(arg)).ToArray();
            //return string.Format(CultureInfo.InvariantCulture, format, safeArgs);
            return string.Format(format, parameters);
        }
        catch (Exception ex) when (!ex.IsFatal())
        {
            return CreateFormatFallback(ex, format, parameters);
        }
    }

    /// <summary>
    /// Safely appends formatted text to a StringBuilder.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="format">The format string.</param>
    /// <param name="args">The arguments for formatting.</param>
    [DebuggerStepThrough]
    public static void SafeAppendFormat(StringBuilder sb, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(sb);
        //if (sb is null || string.IsNullOrEmpty(format))
        //    return;

        if (args.Length == 0)
        {
            sb.Append(SafeToString(format));
            return;
        }

        try
        {
            sb.Append(StringHelper.SafeFormat(format, args));
        }
        catch (Exception ex) when (!ex.IsFatal())
        {
            sb.Append(CreateFormatFallback(ex, format, args));
        }
    }

    /// <summary>
    /// Creates fallback formatting information when string formatting fails.
    /// </summary>
    /// <param name="ex">The exception that occurred during formatting.</param>
    /// <param name="format">The original format string.</param>
    /// <param name="args">The original arguments.</param>
    /// <returns>A detailed error message with format and argument information.</returns>
    [DebuggerStepThrough]
    private static string CreateFormatFallback(Exception ex, string? format, params object?[] args)
    {
        var sb = new StringBuilder();
        sb.AppendLine("*** Exception occurred during formatting:");

        if (ex is not null)
        {
            sb.AppendLine($"{ex.GetType().FullName}: {ex.Message}");
        }

        sb.AppendLine($"SafeFormat: '{format.SafeString("<null>")}'");

        for (int i = 0; i < args.Length; ++i)
        {
            sb.AppendLine($"arg #{i}: '{SafeToString(args[i], "<null>")}'");
        }

        return sb.ToString();
    }

    #endregion

    #region String Collection to Multiline String

    /// <summary>
    /// Converts a string collection into a multi-line string with newlines between elements.
    /// </summary>
    /// <param name="strCollection">The string collection to convert.</param>
    /// <returns>The multi-line string representation.</returns>
    [DebuggerStepThrough]
    public static string StringCollectionToMultiLine(IEnumerable<string> strCollection)
    {
        if (strCollection is null)
            return string.Empty;

        var items = strCollection.ToArray();
        return items.Length switch
        {
            0 => string.Empty,
            1 => items[0].SafeString(),
            _ => string.Join(Environment.NewLine, items.Select(item => item.SafeString()))
        };
    }

    /// <summary>
    /// Converts a string array into a multi-line string with newlines between elements.
    /// </summary>
    /// <param name="strArray">The string array to convert.</param>
    /// <returns>The multi-line string representation.</returns>
    [DebuggerStepThrough]
    public static string StringCollectionToMultiLine(string[]? strArray)
    {
        if (strArray is null)
            return string.Empty;

        return strArray.Length switch
        {
            0 => string.Empty,
            1 => strArray[0].SafeString(),
            _ => string.Join(Environment.NewLine, strArray.Select(item => item.SafeString()))
        };
    }

    #endregion

    #region Join Methods

    /// <summary>
    /// Joins items of type T using the default string representation.
    /// </summary>
    /// <typeparam name="T">The type of items to join.</typeparam>
    /// <param name="separator">The separator to use between items.</param>
    /// <param name="items">The items to join.</param>
    /// <returns>The joined string representation of all items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when separator is null.</exception>
    public static string JoinParams(string separator, params object?[] items)
    {
        ArgumentNullException.ThrowIfNull(separator);

        return string.Join(separator, items.Select(item => SafeToString(item)));
    }

    /// <summary>
    /// Joins items using a custom string converter function.
    /// </summary>
    /// <typeparam name="T">The type of items to join.</typeparam>
    /// <param name="separator">The separator to use between items.</param>
    /// <param name="items">The items to join.</param>
    /// <param name="converter">Function to convert each item to its string representation.</param>
    /// <returns>The joined string using the custom converter.</returns>
    /// <exception cref="ArgumentNullException">Thrown when separator or items is null.</exception>
    public static string Join<T>(string separator, IEnumerable<T> items, Func<T, string>? converter = null)
    {
        ArgumentNullException.ThrowIfNull(separator);
        ArgumentNullException.ThrowIfNull(items);

        return (converter is null) ?
            string.Join(separator, items.Select(item => SafeToString(item))) :
            string.Join(separator, items.Select(converter));
    }

    #endregion

    #region Byte-Array convertion to and from string

    /// <summary>
    /// Converts a string to its byte representation, using the specified enconding (default is UTF-16).
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <param name="encoding">Optional encoding to use for conversion. If null, defaults to UTF-16 (Unicode).</param>
    /// <returns>The byte array representation of the string.</returns>
    public static byte[] GetBytesFromString(string? str, Encoding? encoding = null)
    {
        if (string.IsNullOrEmpty(str))
            return [];

        encoding ??= Encoding.Unicode; // Default to UTF-16 encoding
        
        return encoding.GetBytes(str);
    }

    /// <summary>
    /// Converts a UTF-16 byte array back to a string.
    /// </summary>
    /// <param name="data">The byte array to convert.</param>
    /// <param name="encoding">Optional encoding to use for conversion. If null, defaults to UTF-16 (Unicode).</param>
    /// <returns>The decoded string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    public static string GetStringFromBytes(byte[] data, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(data);

        encoding ??= Encoding.Unicode; // Default to UTF-16 encoding
       
        return encoding.GetString(data);
    }

    #endregion

    #region Number Formatting

    /// <summary>
    /// Returns a string representation of an integer with leading zeros to reach the specified length.
    /// </summary>
    /// <param name="value">The integer value to format.</param>
    /// <param name="length">The desired total length of the resulting string.</param>
    /// <returns>A string with leading zeros if necessary to reach the specified length.</returns>
    public static string PadIntegerZerosLeft(int value, int length)
    {
        //return PadIntegerLeft(val, length, '0');
        return value.ToString($"D{Math.Max(length, 1)}");
    }

    /// <summary>
    /// Returns a string representation of an integer with the specified padding character on the left.
    /// </summary>
    /// <param name="value">The integer value to format.</param>
    /// <param name="length">The desired total length of the resulting string.</param>
    /// <param name="paddingChar">The character to use for padding. (default = ' ')</param>
    /// <returns>A string with left padding to reach the specified length.</returns>
    public static string PadIntegerLeft(int value, int length, char paddingChar = ' ')
    {
        return value.ToString().PadLeft(length, paddingChar);
    }

    // <summary>
    /// Returns a string representation of an integer with the specified padding character on the right.
    /// </summary>
    /// <param name="value">The integer value to format.</param>
    /// <param name="length">The desired total length of the resulting string.</param>
    /// <param name="paddingChar">The character to use for padding.</param>
    /// <returns>A string with right padding to reach the specified length.</returns>
    public static string PadIntegerRight(int value, int length, char paddingChar = ' ')
    {
        return value.ToString().PadRight(length, paddingChar);
    }

    /// <summary>
    /// Formats an integer with leading zeros based on the maximum value's digit count.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="maxValue">The maximum value that determines the padding length.</param>
    /// <param name="culture">The culture to use for formatting.</param>
    /// <returns>The formatted string with appropriate leading zeros.</returns>
    public static string ToStringWithLeading(int value, int maxValue, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.InvariantCulture;

        if (value >= maxValue)
            return value.ToString(culture);

        int digits = (int)Math.Floor(Math.Log10(maxValue)) + 1;
        return value.ToString($"D{digits}", culture);
    }

    #endregion

    #region Character Removal Methods

    /// <summary>
    /// Removes all specified characters from the input string.
    /// </summary>
    /// <param name="str">The input string to process.</param>
    /// <param name="chars">The characters to remove from the string.</param>
    /// <returns>The string with all specified characters removed.</returns>
    public static string RemoveCharacters(string? str, params char[] chars)
    {
        if (string.IsNullOrEmpty(str) || chars.Length == 0)
            return str ?? string.Empty;

        // string.Create is used for performance optimization
        // to avoid unnecessary allocations and copying.
        // It allows us to create a new string directly in the target span.
        //
        // Create initializes a new string with the specified length, and then
        // calls the callback function with a span to the new created string.
        //
        // The length of the new string is determined by counting all the characters
        // in the original string that are not in the chars array.
        return string.Create(str.Length, (str, chars), static (span, state) =>
        {
            // Unpack the state tuple (source string and characters to remove)
            var (source, charsToRemove) = state;
            // Use a span to source string for efficient character iteration
            var sourceSpan = source.AsSpan();
            // Write index for the new string
            int writeIndex = 0;

            // Iterate through each character in the source span
            foreach (char c in sourceSpan)
            {
                if (!charsToRemove.Contains(c))
                {
                    // If the character is not in the chars array, write it to the new span
                    span[writeIndex++] = c;
                }
            }

            // Fill remaining positions (won't be used in final string)
            span[writeIndex..].Clear();
        })[..GetLengthAfterRemoval(str, chars)]; // Range operator is used to slice the string to the correct length after removal.
    }

    /// <summary>
    /// Removes all characters that are NOT in the specified array from the string.
    /// </summary>
    /// <param name="str">The input string to process.</param>
    /// <param name="chars">The characters to keep in the string.</param>
    /// <returns>The string with only the specified characters remaining.</returns>
    public static string RemoveCharactersInverse(string? str, params char[] chars)
    {
        if (string.IsNullOrEmpty(str))
            return str ?? string.Empty;

        if (chars.Length == 0)
            return string.Empty;

        // string.Create is used for performance optimization
        // to avoid unnecessary allocations and copying.
        // It allows us to create a new string directly in the target span.
        //
        // Create initializes a new string with the specified length, and then
        // calls the callback function with a span to the new created string.
        //
        // The length of the new string is determined by counting all the characters
        // in the original string that are in the chars array.
        return string.Create(str.Length, (str, chars), static (span, state) =>
        {
            // Unpack the state tuple (source string and characters to remove)
            var (source, charsToKeep) = state;
            // Use a span to source string for efficient character iteration
            var sourceSpan = source.AsSpan();
            // Write index for the new string
            int writeIndex = 0;

            // Iterate through each character in the source span
            foreach (char c in sourceSpan)
            {
                if (charsToKeep.Contains(c))
                {
                    // If the character is in the chars array, write it to the new span
                    span[writeIndex++] = c;
                }
            }

            // Fill remaining positions (won't be used in final string)
            span[writeIndex..].Clear();
        })[..GetLengthAfterInverseRemoval(str, chars)]; // Range operator is used to slice the string to the correct length after inverse removal.
    }

    /// <summary>
    /// Helper method to calculate the length after character removal.
    /// </summary>
    /// <param name="str">The source string.</param>
    /// <param name="chars">Characters to remove.</param>
    /// <returns>The length of the string after removal.</returns>
    private static int GetLengthAfterRemoval(string str, char[] chars)
    {
        return str.AsSpan().Count(c => !chars.Contains(c));
    }

    /// <summary>
    /// Helper method to calculate the length after inverse character removal.
    /// </summary>
    /// <param name="str">The source string.</param>
    /// <param name="chars">Characters to keep.</param>
    /// <returns>The length of the string after inverse removal.</returns>
    private static int GetLengthAfterInverseRemoval(string str, char[] chars)
    {
        return str.AsSpan().Count(c => chars.Contains(c));
    }

    #endregion

    #region Random String Generation

    /// <summary>
    /// Generates a random string of the specified size using ASCII characters.
    /// </summary>
    /// <param name="size">The length of the string to generate.</param>
    /// <param name="lowerCase">If true, uses only lowercase letters (a-z); otherwise uses both upper and lowercase (A-Z, a-z).</param>
    /// <returns>A randomly generated string of the specified length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when size is less than 0 or greater than 4096.</exception>
    public static string RandomString(int size, bool lowerCase = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(size, 4096);


        if (size == 0)
            return string.Empty;

        var random = ThreadLocalRandom.Value!; // Thread-safe random instance

        // Use string.Create for performance optimization
        // This method allows us to create a new string directly in the target span
        // and fill it with random characters based on the specified size and case.
        // Callback function is executed with a span of the new string,
        // and a state tuple containing the random instance and case preference.
        return string.Create(size, (random, lowerCase), static (span, state) =>
        {
            // Unpack the state tuple
            var (rng, useLowerCase) = state;
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";

            var chars = useLowerCase ? lowerChars : upperChars + lowerChars;

            for (int i = 0; i < span.Length; i++)
            {
                // Fill the span with random characters from the selected character set
                span[i] = chars[rng.Next(chars.Length)];
            }
        });
    }

    #endregion

    #region CRC32 Calculation

    // <summary>
    /// Calculates the CRC32 checksum for the specified string.
    /// </summary>
    /// <param name="str">The string to calculate CRC32 for.</param>
    /// <returns>The CRC32 checksum value.</returns>
    public static uint CalculateCrc32(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        return Crc32Helper.Compute(str);
    }

    #endregion

    #region HTML Utilities

    /// <summary>
    /// Replaces newline characters with HTML &lt;br /&gt; elements.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>The text with newlines replaced by HTML break elements.</returns>
    public static string ReplaceNewLineWithHtmlBr(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.Replace("\r\n", "<br />", StringComparison.Ordinal)
                  .Replace("\n", "<br />", StringComparison.Ordinal);
    }

    #endregion

    #region String Splitting

    /// <summary>
    /// Splits a string at the specified index into two parts.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <param name="index">The index at which to split.</param>
    /// <param name="includeIndexInFirstPortion">If true, includes the character at index in the first portion; otherwise in the second.</param>
    /// <returns>An array with two elements: the left and right portions of the split.</returns>
    public static string[] SplitOn(string? str, int index, bool includeIndexInFirstPortion)
    {
        if (string.IsNullOrEmpty(str))
            return [string.Empty, string.Empty];

        if (index < 0 || index >= str.Length)
            return [str, string.Empty];

        int splitPoint = includeIndexInFirstPortion ? index + 1 : index;

        return [
            str[..splitPoint],
            str[splitPoint..]
        ];
    }

    /// <summary>
    /// Splits a string into chunks to create a square-like arrangement based on separators.
    /// </summary>
    /// <param name="value">The string to split into chunks.</param>
    /// <param name="separators">The separator characters to consider for splitting.</param>
    /// <returns>An array of string chunks arranged in a square-like pattern.</returns>
    public static string[] SquareChunk(string? value, params char[] separators)
    {
        if (string.IsNullOrEmpty(value))
            return [string.Empty];

        separators ??= [];

        var separatorPositions = new List<int>();
        int targetChunkSize = (int)Math.Sqrt(value.Length);
        int lastSeparator = 0;

        // Find separator positions and adjust target chunk size
        for (int i = 0; i < value.Length; i++)
        {
            if (separators.Contains(value[i]))
            {
                targetChunkSize = Math.Max(targetChunkSize, i - lastSeparator);
                separatorPositions.Add(lastSeparator = i);
            }
        }

        targetChunkSize = Math.Max(targetChunkSize, 1);
        separatorPositions.Add(value.Length);

        var chunks = new List<string>();
        int startIndex = 0;

        foreach (int endIndex in separatorPositions)
        {
            int length = endIndex - startIndex;
            if (length >= targetChunkSize)
            {
                chunks.Add(value[startIndex..endIndex]);
                startIndex = endIndex;
            }
        }

        if (startIndex < value.Length)
            chunks.Add(value[startIndex..]);

        return chunks.ToArray();
    }

    #endregion
}
