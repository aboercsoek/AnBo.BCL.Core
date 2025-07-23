//--------------------------------------------------------------------------
// File:    StringExtensions.RegEx.cs
// Content:	Implementation of class StringExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    #endregion

}
