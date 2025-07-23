//--------------------------------------------------------------------------
// File:    RegexHelper.cs
// Content:	Implementation of Regex helper class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core;

/// <summary>
/// Delegate for regex replacement callbacks that allows custom replacement logic.
/// </summary>
/// <param name="capturedIndex">The index of the captured group</param>
/// <param name="capturedValue">The value of the captured group</param>
/// <returns>The replacement string for the captured value</returns>
public delegate string CallbackRegexReplacement(int capturedIndex, string capturedValue);

/// <summary>
/// Modern regex helper class providing comprehensive string pattern matching and manipulation utilities.
/// Optimized for .NET 8+ with source-generated regex patterns for maximum performance.
/// </summary>
public static class RegexHelper
{
    #region Validation Methods

    /// <summary>
    /// Validates if the input string contains only alphabetic characters.
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <returns>True if input contains only letters (a-z, A-Z), false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsAlphaOnly(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && RegexPatterns.AlphaOnly().IsMatch(input);
    }

    /// <summary>
    /// Validates if the input string contains only uppercase alphabetic characters.
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <returns>True if input contains only uppercase letters (A-Z), false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsAlphaUpperCaseOnly(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && RegexPatterns.AlphaUpperCaseOnly().IsMatch(input);
    }

    /// <summary>
    /// Validates if the input string contains only lowercase alphabetic characters.
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <returns>True if input contains only lowercase letters (a-z), false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsAlphaLowerCaseOnly(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && RegexPatterns.AlphaLowerCaseOnly().IsMatch(input);
    }

    /// <summary>
    /// Validates if the input string contains only alphanumeric characters.
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <returns>True if input contains only letters and digits, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsAlphaNumericOnly(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && RegexPatterns.AlphaNumericOnly().IsMatch(input);
    }

    /// <summary>
    /// Validates if the input string contains only alphanumeric characters and spaces.
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <returns>True if input contains only letters, digits, and spaces, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsAlphaNumericSpaceOnly(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && RegexPatterns.AlphaNumericSpaceOnly().IsMatch(input);
    }

    /// <summary>
    /// Validates if the input string represents a valid numeric value (including decimals and negative numbers).
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <param name="useGermanFormat">If true, uses comma as decimal separator; otherwise uses period</param>
    /// <returns>True if input represents a valid number, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsNumeric(string input, bool useGermanFormat = false)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && (useGermanFormat
            ? RegexPatterns.NumericGerman().IsMatch(input)
            : RegexPatterns.Numeric().IsMatch(input));
    }

    /// <summary>
    /// Validates if the input string represents a valid email address.
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <returns>True if input is a valid email format, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsValidEmail(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && RegexPatterns.Email().IsMatch(input);
    }

    /// <summary>
    /// Validates if the input string represents a valid URL.
    /// </summary>
    /// <param name="input">The string to validate</param>
    /// <returns>True if input is a valid URL format, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool IsValidUrl(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > 0 && RegexPatterns.Url().IsMatch(input);
    }

    #endregion

    #region Extraction Methods

    /// <summary>
    /// Extracts the value of a specific capture group from the first match.
    /// </summary>
    /// <param name="input">The input string to search in</param>
    /// <param name="pattern">The regex pattern to match</param>
    /// <param name="groupIndex">The capture group index (0 = entire match, 1+ = numbered groups)</param>
    /// <param name="options">Optional regex options to apply</param>
    /// <returns>The captured value or null if no match found or group doesn't exist</returns>
    /// <exception cref="ArgumentNullException">Thrown when input or pattern is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when groupIndex is negative</exception>
    public static string? Extract(string input, string pattern, int groupIndex = 0, RegexOptions options = RegexOptions.None)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfNegative(groupIndex);

        var match = Regex.Match(input, pattern, options);
        return GetCapture(match, groupIndex);
    }

    /// <summary>
    /// Extracts all matches of a specific capture group from the input string.
    /// </summary>
    /// <param name="input">The input string to search in</param>
    /// <param name="pattern">The regex pattern to match</param>
    /// <param name="groupIndex">The capture group index (0 = entire match, 1+ = numbered groups)</param>
    /// <param name="options">Optional regex options to apply</param>
    /// <returns>Collection of all captured values for the specified group</returns>
    /// <exception cref="ArgumentNullException">Thrown when input or pattern is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when groupIndex is negative</exception>
    public static IEnumerable<string> ExtractAll(string input, string pattern, int groupIndex = 0, RegexOptions options = RegexOptions.None)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentOutOfRangeException.ThrowIfNegative(groupIndex);

        var matches = Regex.Matches(input, pattern, options);
        foreach (Match match in matches)
        {
            var capture = GetCapture(match, groupIndex);
            if (capture != null)
                yield return capture;
        }
    }

    /// <summary>
    /// Extracts all email addresses found in the input string.
    /// </summary>
    /// <param name="input">The input string to search in</param>
    /// <returns>Collection of all email addresses found</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static IEnumerable<string> ExtractEmails(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RegexPatterns.EmailOnly().Matches(input).Select(m => m.Value);
    }

    /// <summary>
    /// Extracts all URLs found in the input string.
    /// </summary>
    /// <param name="input">The input string to search in</param>
    /// <param name="lenient">If true, uses lenient URL matching that doesn't require protocol</param>
    /// <returns>Collection of all URLs found</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static IEnumerable<string> ExtractUrls(string input, bool lenient = false)
    {
        ArgumentNullException.ThrowIfNull(input);
        var pattern = lenient ? RegexPatterns.UriLenient() : RegexPatterns.Uri();
        return pattern.Matches(input).Select(m => m.Value);
    }

    #endregion

    #region Replacement Methods

    /// <summary>
    /// Replaces all matches of a pattern with a static replacement string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="pattern">The regex pattern to match</param>
    /// <param name="replacement">The replacement string</param>
    /// <param name="options">Optional regex options to apply</param>
    /// <returns>String with all matches replaced</returns>
    /// <exception cref="ArgumentNullException">Thrown when input, pattern, or replacement is null</exception>
    public static string ReplaceAll(string input, string pattern, string replacement, RegexOptions options = RegexOptions.None)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentNullException.ThrowIfNull(replacement);

        try
        {
            return Regex.Replace(input, pattern, replacement, options);
        }
        catch (ArgumentException)
        {
            // Invalid regex pattern - return original string
            return input;
        }
    }

    /// <summary>
    /// Replaces matches of a specific capture group with a replacement string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="pattern">The regex pattern to match</param>
    /// <param name="groupIndex">The capture group index to replace</param>
    /// <param name="replacement">The replacement string</param>
    /// <param name="options">Optional regex options to apply</param>
    /// <returns>String with specified capture groups replaced</returns>
    /// <exception cref="ArgumentNullException">Thrown when input, pattern, or replacement is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when groupIndex is negative</exception>
    public static string ReplaceGroup(string input, string pattern, int groupIndex, string replacement, RegexOptions options = RegexOptions.None)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentNullException.ThrowIfNull(replacement);
        ArgumentOutOfRangeException.ThrowIfNegative(groupIndex);

        return ReplaceGroup(input, pattern, groupIndex, new StaticStringReplacer(replacement).Replace, options);
    }

    /// <summary>
    /// Replaces matches of a specific capture group using a callback function.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="pattern">The regex pattern to match</param>
    /// <param name="groupIndex">The capture group index to replace</param>
    /// <param name="replacement">The callback function that generates replacement strings</param>
    /// <param name="options">Optional regex options to apply</param>
    /// <returns>String with specified capture groups replaced</returns>
    /// <exception cref="ArgumentNullException">Thrown when input, pattern, or replacement is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when groupIndex is negative</exception>
    public static string ReplaceGroup(string input, string pattern, int groupIndex, CallbackRegexReplacement replacement, RegexOptions options = RegexOptions.None)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(pattern);
        ArgumentNullException.ThrowIfNull(replacement);
        ArgumentOutOfRangeException.ThrowIfNegative(groupIndex);

        return Regex.Replace(input, pattern, match =>
        {
            if (match.Groups.Count > groupIndex)
            {
                var group = match.Groups[groupIndex];
                var replacementValue = replacement(groupIndex, group.Value);
                return match.Value.Replace(group.Value, replacementValue);
            }
            return match.Value;
        }, options);
    }

    /// <summary>
    /// Removes all HTML break tags and paragraph tags from the input string.
    /// </summary>
    /// <param name="input">The input string containing HTML</param>
    /// <returns>String with HTML break and paragraph tags removed</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static string RemoveHtmlBreaks(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RegexPatterns.HtmlBreakOrParagraph().Replace(input, string.Empty);
    }

    /// <summary>
    /// Trims HTML break and paragraph tags from the beginning and end of the string.
    /// </summary>
    /// <param name="input">The input string containing HTML</param>
    /// <returns>String with leading and trailing HTML tags removed</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static string TrimHtmlBreaks(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RegexPatterns.HtmlBreakOrParagraphTrim().Replace(input, string.Empty);
    }

    /// <summary>
    /// Removes all non-alphanumeric characters from the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>String containing only letters and digits</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static string RemoveNonAlphaNumeric(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return RegexPatterns.NonWordDigit().Replace(input, string.Empty);
    }

    /// <summary>
    /// Keeps only alphanumeric characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A string containing only alphanumeric characters.</returns>
    public static string KeepAlphaNumeric(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return RegexPatterns.AlphaNumeric().Matches(input).Aggregate(new StringBuilder(), (sb, match) => sb.Append(match.Value))
            .ToString();
    }

    /// <summary>
    /// Keeps only alphabetic characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A string containing only alphabetic characters.</returns>
    public static string KeepAlphaCharacters(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return RegexPatterns.AlphaCharacters().Matches(input).Aggregate(new StringBuilder(), (sb, match) => sb.Append(match.Value))
            .ToString();
    }

    /// <summary>
    /// Keeps only numeric characters, optionally including decimal punctuation.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="keepNumericPunctuation">Whether to keep decimal points and commas.</param>
    /// <returns>A string containing only numeric characters.</returns>
    public static string KeepNumericDigitsOnly(string? input, bool keepNumericPunctuation = false)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var regex = keepNumericPunctuation ? RegexPatterns.NumericWithPunctuation() : RegexPatterns.NumericDigitsOnly();
        return regex.Matches(input).Aggregate(new StringBuilder(), (sb, match) => sb.Append(match.Value))
            .ToString();
    }

    #endregion

    #region Matching Methods

    /// <summary>
    /// Checks if the input matches any of the provided regex patterns.
    /// </summary>
    /// <param name="input">The input string to test</param>
    /// <param name="patterns">Array of regex patterns to test against</param>
    /// <returns>Index of the first matching pattern, or -1 if no matches found</returns>
    /// <exception cref="ArgumentNullException">Thrown when input or patterns is null</exception>
    public static int MatchAny(string input, params Regex[] patterns)
    {
        return MatchAny(input, out _, patterns);
    }

    /// <summary>
    /// Checks if the input matches any of the provided regex patterns and returns the successful match.
    /// </summary>
    /// <param name="input">The input string to test</param>
    /// <param name="successfulMatch">The first successful match result, or null if no matches</param>
    /// <param name="patterns">Array of regex patterns to test against</param>
    /// <returns>Index of the first matching pattern, or -1 if no matches found</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null, or a patterns item is null</exception>
    public static int MatchAny(string input, out Match? successfulMatch, params Regex[] patterns)
    {
        ArgumentNullException.ThrowIfNull(input);

        successfulMatch = null;

        if (patterns.Any(pattern => pattern == null))
            throw new ArgumentNullException(nameof(patterns), "One or more patterns are null.");

        for (int i = 0; i < patterns.Length; i++)
        {
            var match = patterns[i].Match(input);
            if (match.Success)
            {
                successfulMatch = match;
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Checks if the input matches all of the provided regex patterns.
    /// </summary>
    /// <param name="input">The input string to test</param>
    /// <param name="patterns">Array of regex patterns that must all match</param>
    /// <returns>True if input matches all patterns, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    public static bool MatchAll(string input, params Regex[] patterns)
    {
        ArgumentNullException.ThrowIfNull(input);

        return patterns.All(pattern => pattern.IsMatch(input));
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Safely extracts a capture group value from a regex match.
    /// </summary>
    /// <param name="match">The regex match result</param>
    /// <param name="groupIndex">The capture group index to extract</param>
    /// <returns>The capture group value or null if match failed or group doesn't exist</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when groupIndex is negative</exception>
    public static string? GetCapture(Match match, int groupIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(groupIndex);

        return match.Success && match.Groups.Count > groupIndex
            ? match.Groups[groupIndex].Value
            : null;
    }

    /// <summary>
    /// Gets the Nth last capture group from a regex match.
    /// </summary>
    /// <param name="match">The regex match result</param>
    /// <param name="offset">Offset from the last group (0 = last, 1 = second-to-last, etc.) (default is 0)</param>
    /// <returns>The value of the specified capture group or null if match failed or offset invalid</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when offset is negative</exception>
    public static string? GetLastCapture(Match match, int offset = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        return match.Success && match.Groups.Count > offset
            ? GetCapture(match, match.Groups.Count - 1 - offset)
            : null;
    }

    /// <summary>
    /// Splits a string at the specified index position.
    /// </summary>
    /// <param name="input">The input string to split</param>
    /// <param name="index">The zero-based index where to split</param>
    /// <param name="includeIndexCharInLeft">If true, includes the character at index in the left part</param>
    /// <returns>Array with two elements: [0] = left part, [1] = right part</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of bounds</exception>
    public static string[] SplitAt(string input, int index, bool includeIndexCharInLeft = false)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, input.Length);

        int splitIndex = includeIndexCharInLeft ? index + 1 : index;
        return [
            input[..splitIndex],
            input[splitIndex..]
        ];
    }

    #endregion

    #region Nested type: StaticStringReplacer

    private class StaticStringReplacer(string replacement)
    {
        private readonly string replacement = replacement;

        public string Replace(int capturedIndex, string capturedValue) => replacement;
    }

    #endregion
}
