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

    #region Is... string extensions

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

    #region SubString extensions (Right, Left, ...)

    /// <summary>
    /// Gets the leftmost characters from a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of characters to return.</param>
    /// <returns>The leftmost characters or the entire string if shorter than requested length.</returns>
    public static string? Left(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;
        if (length >= input.Length) return input;

        return input[..length];
    }

    /// <summary>
    /// Gets the rightmost characters from a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of characters to return.</param>
    /// <returns>The rightmost characters or the entire string if shorter than requested length.</returns>
    public static string? Right(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;
        if (length >= input.Length) return input;

        return input[^length..];
    }

    /// <summary>
    /// Gets the leftmost grapheme clusters (user-perceived characters) from a string.
    /// This method properly handles complex Unicode scenarios like emoji and combining characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of grapheme clusters to return.</param>
    /// <returns>The leftmost grapheme clusters.</returns>
    /// <example>
    /// <code>
    /// "Hello 👨‍👩‍👧‍👦 World".LeftGraphemes(7); // Returns "Hello 👨‍👩‍👧‍👦"
    /// "café".LeftGraphemes(3); // Returns "caf" (regardless of é representation)
    /// </code>
    /// </example>
    public static string? LeftGraphemes(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;

        var enumerator = StringInfo.GetTextElementEnumerator(input);
        var count = 0;
        var result = new StringBuilder();

        while (enumerator.MoveNext() && count < length)
        {
            result.Append(enumerator.GetTextElement());
            count++;
        }

        return result.ToString();
    }

    /// <summary>
    /// Gets the rightmost grapheme clusters (user-perceived characters) from a string.
    /// This method properly handles complex Unicode scenarios like emoji and combining characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of grapheme clusters to return.</param>
    /// <returns>The rightmost grapheme clusters.</returns>
    /// <example>
    /// <code>
    /// "Hello 👨‍👩‍👧‍👦 World".RightGraphemes(6); // Returns "👨‍👩‍👧‍👦 World"
    /// "café".RightGraphemes(2); // Returns "fé" (regardless of é representation)
    /// </code>
    /// </example>
    public static string? RightGraphemes(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;

        var textElements = new List<string>();
        var enumerator = StringInfo.GetTextElementEnumerator(input);

        while (enumerator.MoveNext())
        {
            textElements.Add(enumerator.GetTextElement());
        }

        if (length >= textElements.Count) return input;

        var startIndex = textElements.Count - length;
        return string.Concat(textElements.Skip(startIndex));
    }

    /// <summary>
    /// Clips a string to a maximum length, appending an ellipsis if necessary.
    /// </summary>
    /// <param name="text">The text to clip.</param>
    /// <param name="maxCount">The maximum string length.</param>
    /// <param name="clipText">The text to append when clipping (default: "...").</param>
    /// <returns>A string clipped to the maximum length.</returns>
    public static string Clip(this string? text, int maxCount, string clipText = "...")
    {
        if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
        if (maxCount <= 0) return string.Empty;
        if (text.Length <= maxCount) return text;

        clipText ??= "...";

        if (maxCount <= clipText.Length)
            return text[..maxCount];

        var targetLength = maxCount - clipText.Length;
        return string.Create(maxCount, (text, targetLength, clipText),
            static (span, state) =>
            {
                state.text.AsSpan(0, state.targetLength).CopyTo(span); // copy the clipped part
                state.clipText.AsSpan().CopyTo(span[state.targetLength..]); // append clip text
            });
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

    #region IndexOf Extensions - Modern Approach

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
        ArgChecker.ShouldNotBeNull(s);
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

    #region RegEx string extensions

    /// <summary>
    /// Determines if a string matches the specified regular expression pattern.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="regexPattern">The regular expression pattern.</param>
    /// <param name="options">The regex options (default: None).</param>
    /// <returns><see langword="true"/> if the string matches; otherwise, <see langword="false"/>.</returns>
    public static bool IsMatchingTo(this string? value,
        [StringSyntax(StringSyntaxAttribute.Regex)] string regexPattern,
        RegexOptions options = RegexOptions.None)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return Regex.IsMatch(value, regexPattern, options);
    }

    /// <summary>
    /// Replaces parts of a string using regular expressions.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="regexPattern">The regular expression pattern.</param>
    /// <param name="replaceValue">The replacement value.</param>
    /// <param name="options">The regex options (default: None).</param>
    /// <returns>The string with replacements applied.</returns>
    public static string ReplaceWith(this string? value,
        [StringSyntax(StringSyntaxAttribute.Regex)] string regexPattern,
        string replaceValue,
        RegexOptions options = RegexOptions.None)
    {
        if (string.IsNullOrEmpty(value))
            return value ?? string.Empty;

        return Regex.Replace(value, regexPattern, replaceValue, options);
    }

    /// <summary>
    /// Uses regular expressions to replace parts of a string.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="regexPattern">The regular expression pattern.</param>
    /// <param name="evaluator">The replacement method / lambda expression.</param>
    /// <param name="options">The regex options (default: None).</param>
    /// <returns>The newly created string</returns>
    /// <example>
    /// <code lang="cs" title="String extension method ReplaceWith example" numberLines="true" outlining="true" >
    /// string s = "12345";
    /// string replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));</code>
    /// <para>Value of <c>replaced</c> after execution = <c>" -12345- "</c>.</para>
    /// </example>
    [DebuggerStepThrough]
    public static string ReplaceWith(this string? value,
        [StringSyntax(StringSyntaxAttribute.Regex)] string regexPattern,
        MatchEvaluator evaluator,
        RegexOptions options = RegexOptions.None)
    {
        if (string.IsNullOrEmpty(value))
            return value ?? string.Empty;

        return Regex.Replace(value, regexPattern, evaluator, options);
    }

    /// <summary>
    /// Gets all matches of a regular expression pattern.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="regexPattern">The regular expression pattern.</param>
    /// <param name="options">The regex options (default: None).</param>
    /// <returns>An enumerable of match values.</returns>
    /// <example>
    /// <code lang="cs" title="String extension method GetMatchingValues example" numberLines="true" outlining="true" >
    /// string s = "12345";
    /// foreach(string number in s.GetMatchingValues(@"\d", RegexOptions.None)) 
    /// {
    ///   Console.WriteLine(number);
    /// }</code>
    /// <code title="Console Output:" numberLines="false" outlining="false" >
    /// 1
    /// 2
    /// 3
    /// 4
    /// 5</code>
    /// </example>
    public static IEnumerable<string> GetMatchingValues(this string? value,
        [StringSyntax(StringSyntaxAttribute.Regex)] string regexPattern,
        RegexOptions options = RegexOptions.None)
    {
        if (string.IsNullOrEmpty(value)) yield break;

        var matches = Regex.Matches(value, regexPattern, options);
        foreach (Match match in matches)
        {
            if (match.Success)
                yield return match.Value;
        }
    }

    #endregion

    #region Text filter extensions

    /// <summary>
    /// Removes text matching the specified filter pattern.
    /// </summary>
    /// <param name="input">The input text.</param>
    /// <param name="filter">The regex pattern to remove.</param>
    /// <returns>The input text with filter matches removed.</returns>
    public static string? FilterOutText(this string? input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string filter)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(filter))
            return input;

        return Regex.Replace(input, filter, string.Empty);
    }

    /// <summary>
    /// Keeps only text matching the specified filter pattern.
    /// </summary>
    /// <param name="input">The input text.</param>
    /// <param name="filter">The regex pattern to keep.</param>
    /// <returns>A string containing only the matching text.</returns>
    public static string? KeepFilterText(this string? input,
        [StringSyntax(StringSyntaxAttribute.Regex)] string filter)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(filter))
            return input ?? string.Empty;

        var matches = Regex.Matches(input, filter);
        if (matches.Count == 0) return string.Empty;

        var totalLength = matches.Sum(m => m.Length);
        return string.Create(totalLength, matches, static (span, matchList) =>
        {
            var position = 0;
            foreach (Match match in matchList)
            {
                match.ValueSpan.CopyTo(span[position..]);
                position += match.Length;
            }
        });
    }

    /// <summary>
    /// Keeps only alphanumeric characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A string containing only alphanumeric characters.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AlphaNumericOnly(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return AlphaNumericRegex().Matches(input).Aggregate(new StringBuilder(), (sb, match) => sb.Append(match.Value))
            .ToString();
    }

    /// <summary>
    /// Keeps only alphabetic characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A string containing only alphabetic characters.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AlphaCharactersOnly(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return AlphaCharactersRegex().Matches(input).Aggregate(new StringBuilder(), (sb, match) => sb.Append(match.Value))
            .ToString();
    }

    /// <summary>
    /// Keeps only numeric characters, optionally including decimal punctuation.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="keepNumericPunctuation">Whether to keep decimal points and commas.</param>
    /// <returns>A string containing only numeric characters.</returns>
    public static string NumericOnly(this string? input, bool keepNumericPunctuation = false)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var regex = keepNumericPunctuation ? NumericWithPunctuationRegex() : NumericOnlyRegex();
        return regex.Matches(input).Aggregate(new StringBuilder(), (sb, match) => sb.Append(match.Value))
            .ToString();
    }

    #endregion

    #region SecureString extensions

    // Obsolete: SecureString is not recommended for new development.

    #endregion

    #region String to type conversion extensions

    /// <summary>
    /// Converts a string to a byte array using UTF-8 encoding.
    /// </summary>
    /// <param name="str">The source string.</param>
    /// <returns>The UTF-8 encoded byte array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this string? str)
    {
        return Encoding.UTF8.GetBytes(str ?? string.Empty);
    }

    /// <summary>
    /// Converts a string path to a FileInfo object if the file exists.
    /// </summary>
    /// <param name="value">The file path string.</param>
    /// <returns>A FileInfo object if the file exists; otherwise, null.</returns>
    public static FileInfo? ToFileInfo(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;

        try
        {
            var fileInfo = new FileInfo(value);
            return fileInfo.Exists ? fileInfo : null;
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (PathTooLongException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    #endregion

}
