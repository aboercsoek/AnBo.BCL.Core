//--------------------------------------------------------------------------
// File:    RegexPatterns.cs
// Content:	Implementation of class RegexPatterns
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core;

/// Centralized collection of compiled regex patterns used throughout the string utilities.
/// This class provides performance-optimized, reusable regex patterns to avoid duplicated pattern definitions.
/// </summary>
internal static partial class RegexPatterns
{
    /// <summary>
    /// Pattern to match format string placeholders (e.g., {0}, {1:format}).
    /// Used to detect if a string contains format placeholders.
    /// </summary>
    [GeneratedRegex(@"\{0", RegexOptions.Compiled)]
    internal static partial Regex FormatString();

    /// <summary>
    /// Pattern to match alphanumeric characters (letters and digits).
    /// Used for filtering strings to contain only alphanumeric characters.
    /// </summary>
    [GeneratedRegex(@"[a-zA-Z0-9]", RegexOptions.Compiled)]
    internal static partial Regex AlphaNumeric();

    /// <summary>
    /// Pattern to match alphabetic characters only (letters).
    /// Used for filtering strings to contain only alphabetic characters.
    /// </summary>
    [GeneratedRegex(@"[a-zA-Z]", RegexOptions.Compiled)]
    internal static partial Regex AlphaCharacters();

    /// <summary>
    /// Pattern to match numeric characters with punctuation (digits, commas, periods).
    /// Used for filtering numeric strings that may contain decimal separators.
    /// </summary>
    [GeneratedRegex(@"[0-9,\.]", RegexOptions.Compiled)]
    internal static partial Regex NumericWithPunctuation();

    /// <summary>
    /// Pattern to match numeric characters only (digits).
    /// Used for filtering strings to contain only numeric digits.
    /// </summary>
    [GeneratedRegex(@"[0-9]", RegexOptions.Compiled)]
    internal static partial Regex NumericOnly();

    /// <summary>
    /// Pattern to match newline characters (\r\n, \n, \r).
    /// Used for text processing and newline normalization.
    /// </summary>
    [GeneratedRegex(@"[\r\n]+", RegexOptions.Compiled)]
    internal static partial Regex NewLine();
}
