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

/// <summary>
/// Centralized collection of source-generated regex patterns optimized for .NET 8+.
/// All patterns are compiled at build-time for maximum performance and provide
/// type-safe access to commonly used regular expressions.
/// </summary>
internal static partial class RegexPatterns
{
    #region Character Validation Patterns

    /// <summary>
    /// Matches strings containing only alphabetic characters (a-z, A-Z).
    /// Equivalent to: ^[a-zA-Z]*$
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z]+$")]
    internal static partial Regex AlphaOnly();

    /// <summary>
    /// Matches strings containing only uppercase alphabetic characters (A-Z).
    /// Equivalent to: ^[A-Z]*$
    /// </summary>
    [GeneratedRegex(@"^[A-Z]+$")]
    internal static partial Regex AlphaUpperCaseOnly();

    /// <summary>
    /// Matches strings containing only lowercase alphabetic characters (a-z).
    /// Equivalent to: ^[a-z]*$
    /// </summary>
    [GeneratedRegex(@"^[a-z]+$")]
    internal static partial Regex AlphaLowerCaseOnly();

    /// <summary>
    /// Matches strings containing only alphanumeric characters (letters and digits).
    /// Equivalent to: ^[a-zA-Z0-9]*$
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z0-9]+$")]
    internal static partial Regex AlphaNumericOnly();

    /// <summary>
    /// Matches strings containing only alphanumeric characters and spaces.
    /// Equivalent to: ^[a-zA-Z0-9 ]*$
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z0-9 ]+$")]
    internal static partial Regex AlphaNumericSpaceOnly();

    /// <summary>
    /// Matches strings containing alphanumeric characters, spaces, and dashes.
    /// Equivalent to: ^[a-zA-Z0-9 \-]*$
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z0-9 \-]+$")]
    internal static partial Regex AlphaNumericSpaceDashOnly();

    /// <summary>
    /// Matches strings containing alphanumeric characters, spaces, dashes, and underscores.
    /// Equivalent to: ^[a-zA-Z0-9 \-_]*$
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z0-9 \-_]+$")]
    internal static partial Regex AlphaNumericSpaceDashUnderscoreOnly();

    /// <summary>
    /// Matches strings containing alphanumeric characters, spaces, dashes, underscores, and periods.
    /// Equivalent to: ^[a-zA-Z0-9\. \-_]*$
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z0-9\. \-_]+$")]
    internal static partial Regex AlphaNumericSpaceDashUnderscorePeriodOnly();

    #endregion

    #region Numeric Patterns

    /// <summary>
    /// Matches numeric strings with optional decimal point and negative sign (US format).
    /// Equivalent to: ^\-?[0-9]*\.?[0-9]*$
    /// </summary>
    [GeneratedRegex(@"^\-?[0-9]+\.?[0-9]*$")]
    internal static partial Regex Numeric();

    /// <summary>
    /// Matches numeric strings with optional decimal comma and negative sign (German format).
    /// Equivalent to: ^\-?[0-9]*,?[0-9]*$
    /// </summary>
    [GeneratedRegex(@"^\-?[0-9]+,?[0-9]*$")]
    internal static partial Regex NumericGerman();

    /// <summary>
    /// Matches numeric characters with common punctuation (digits, commas, periods).
    /// Used for filtering numeric content that may contain separators.
    /// </summary>
    [GeneratedRegex(@"[0-9,\.]")]
    internal static partial Regex NumericWithPunctuation();

    /// <summary>
    /// Matches only numeric digits (0-9).
    /// Used for extracting pure numeric content.
    /// </summary>
    [GeneratedRegex(@"[0-9]")]
    internal static partial Regex NumericDigitsOnly();

    #endregion

    #region Communication Patterns

    /// <summary>
    /// Matches email addresses with comprehensive validation.
    /// Supports common email formats including special characters and international domains.
    /// Equivalent to: ^([0-9a-zA-Z]+[-._+&amp;])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$
    /// </summary>
    [GeneratedRegex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.IgnoreCase)]
    internal static partial Regex Email();

    /// <summary>
    /// Matches email addresses with comprehensive validation.
    /// Supports common email formats including special characters and international domains.
    /// Equivalent to: ([0-9a-zA-Z]+[-._+&amp;])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}
    /// Used for extracting email addresses.
    /// </summary>
    [GeneratedRegex(@"([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}", RegexOptions.IgnoreCase)]
    internal static partial Regex EmailOnly();

    /// <summary>
    /// Matches HTTP and FTP URLs with comprehensive validation.
    /// Supports various URL formats including ports, paths, and query parameters.
    /// Equivalent to: ^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_=]*)?$
    /// </summary>
    [GeneratedRegex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$", RegexOptions.IgnoreCase)]
    internal static partial Regex Url();

    /// <summary>
    /// Matches HTTP and FTP URLs with comprehensive validation.
    /// Supports various URL formats including ports, paths, and query parameters.
    /// Equivalent to: ^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_=]*)?$
    /// Used for extracting urls.
    /// </summary>
    [GeneratedRegex(@"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?", RegexOptions.IgnoreCase)]
    internal static partial Regex UrlOnly();

    /// <summary>
    /// Matches URI patterns with protocol requirement.
    /// Used for finding complete URIs in text.
    /// </summary>
    [GeneratedRegex(@"\w+://[^\s)<>\]}!([]+")]
    internal static partial Regex Uri();

    /// <summary>
    /// Matches URI patterns with optional protocol (lenient matching).
    /// Used for finding potential URIs that may not include protocol.
    /// </summary>
    [GeneratedRegex(@"\b((https?://)?(www\.)?[a-zA-Z0-9\-]+\.[a-zA-Z]{2,}(\/\S*)?)\b")]
    internal static partial Regex UriLenient();

    #endregion

    #region HTML Patterns

    /// <summary>
    /// Matches HTML break tags (&lt;br&gt;, &lt;br/&gt;, &lt;br /&gt;) with flexible spacing.
    /// Equivalent to: &lt;\s*br\s*/?\s*&gt;
    /// </summary>
    [GeneratedRegex(@"<\s*br\s*/?\s*>", RegexOptions.IgnoreCase)]
    internal static partial Regex HtmlBreak();

    /// <summary>
    /// Matches HTML break and paragraph tags with flexible spacing.
    /// Includes &lt;br&gt;, &lt;p&gt;, and their variations.
    /// Equivalent to: &lt;\s*([bp]r?)\s*/?\s*&gt;
    /// </summary>
    [GeneratedRegex(@"<\/?\s*(br|p)\s*\/?>", RegexOptions.IgnoreCase)]
    internal static partial Regex HtmlBreakOrParagraph();

    /// <summary>
    /// Matches HTML break and paragraph tags at the beginning or end of strings.
    /// Used for trimming unwanted HTML tags from string boundaries.
    /// </summary>
    [GeneratedRegex(@"(^<\/?\s*(br|p)\s*\/?>)|(<\/?\s*(br|p)\s*\/?>$)", RegexOptions.IgnoreCase)]
    internal static partial Regex HtmlBreakOrParagraphTrim();

    /// <summary>
    /// Matches HTML paragraph tags (&lt;p&gt;, &lt;/;p&gt) with flexible spacing.
    /// Equivalent to: &lt;\s*p\s*/?\s*&gt;
    /// </summary>
    [GeneratedRegex(@"<\/?\s*p\s*?>", RegexOptions.IgnoreCase)]
    internal static partial Regex HtmlParagraph();

    #endregion

    #region Text Processing Patterns

    /// <summary>
    /// Matches format string placeholders (e.g., {0}, {1:format}).
    /// Used to detect if a string contains format placeholders.
    /// </summary>
    [GeneratedRegex(@"\{0")]
    internal static partial Regex FormatString();

    /// <summary>
    /// Matches alphabetic characters only (letters).
    /// Used for filtering strings to contain only alphabetic characters.
    /// </summary>
    [GeneratedRegex(@"[a-zA-Z]")]
    internal static partial Regex AlphaCharacters();

    /// <summary>
    /// Matches alphanumeric characters (letters and digits).
    /// Used for filtering strings to contain only alphanumeric characters.
    /// </summary>
    [GeneratedRegex(@"[a-zA-Z0-9]")]
    internal static partial Regex AlphaNumeric();

    /// <summary>
    /// Matches newline characters (\r\n, \n, \r).
    /// Used for text processing and newline normalization.
    /// </summary>
    [GeneratedRegex(@"[\r\n]+")]
    internal static partial Regex NewLine();

    /// <summary>
    /// Matches non-word and non-digit characters.
    /// Used for removing special characters from strings.
    /// Equivalent to: [^a-zA-Z0-9]+
    /// </summary>
    [GeneratedRegex(@"[^a-zA-Z0-9]+")]
    internal static partial Regex NonWordDigit();

    /// <summary>
    /// Matches whitespace characters (spaces, tabs, newlines).
    /// Used for whitespace processing and normalization.
    /// </summary>
    [GeneratedRegex(@"\s+")]
    internal static partial Regex Whitespace();

    /// <summary>
    /// Matches consecutive spaces (2 or more).
    /// Used for normalizing multiple spaces to single spaces.
    /// </summary>
    [GeneratedRegex(@" {2,}")]
    internal static partial Regex MultipleSpaces();

    #endregion

    #region Specialized Patterns

    /// <summary>
    /// Matches German postal codes (5 digits).
    /// Equivalent to: ^[0-9]{5}$
    /// </summary>
    [GeneratedRegex(@"^[0-9]{5}$")]
    internal static partial Regex GermanPostalCode();

    /// <summary>
    /// Matches US postal codes (ZIP codes) in both 5-digit and 9-digit formats.
    /// Supports formats: 12345 and 12345-6789
    /// Equivalent to: ^[0-9]{5}(-[0-9]{4})?$
    /// </summary>
    [GeneratedRegex(@"^[0-9]{5}(-[0-9]{4})?$")]
    internal static partial Regex UsPostalCode();

    /// <summary>
    /// Matches phone numbers in various international formats.
    /// Supports formats with country codes, area codes, and different separators.
    /// </summary>
    [GeneratedRegex(@"^(\+?[1-9]\d{0,3})?[-.\s]?(\(?\d{1,4}\)?)?[-.\s]?\d{1,4}[-.\s]?\d{1,9}$")]
    internal static partial Regex PhoneNumber();

    /// <summary>
    /// Matches credit card numbers (removes spaces and dashes for validation).
    /// Supports major card formats (Visa, MasterCard, American Express, etc.).
    /// </summary>
    [GeneratedRegex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13}|3[0-9]{13}|6(?:011|5[0-9]{2})[0-9]{12})$")]
    internal static partial Regex CreditCard();

    /// <summary>
    /// Matches IPv4 addresses with proper range validation.
    /// Validates that each octet is between 0-255.
    /// </summary>
    [GeneratedRegex(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
    internal static partial Regex IPv4Address();

    /// <summary>
    /// Matches hexadecimal color codes (#RGB or #RRGGBB format).
    /// Supports both 3-digit and 6-digit hex color formats.
    /// </summary>
    [GeneratedRegex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
    internal static partial Regex HexColor();

    /// <summary>
    /// Matches file extensions (e.g., .txt, .pdf, .docx).
    /// Extracts the extension part including the dot.
    /// </summary>
    [GeneratedRegex(@"\.[a-zA-Z0-9]+$", RegexOptions.IgnoreCase)]
    internal static partial Regex FileExtension();

    /// <summary>
    /// Matches version numbers in semantic versioning format (e.g., 1.2.3, 2.0.0-alpha).
    /// Supports major.minor.patch with optional pre-release identifiers.
    /// </summary>
    [GeneratedRegex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")]
    internal static partial Regex SemanticVersion();

    #endregion

    #region Legacy Support (for migration from RegexExpressionStrings)

    ///// <summary>
    ///// Legacy support: matches strings containing only alphabetic characters.
    ///// Use AlphaOnly() instead for new code.
    ///// </summary>
    //[GeneratedRegex(@"^[a-zA-Z]*$")]
    //internal static partial Regex AlphaExpression();

    ///// <summary>
    ///// Legacy support: matches numeric strings (US format).
    ///// Use Numeric() instead for new code.
    ///// </summary>
    //[GeneratedRegex(@"^\-?[0-9]*\.?[0-9]*$")]
    //internal static partial Regex NumericExpression();

    ///// <summary>
    ///// Legacy support: matches email addresses.
    ///// Use Email() instead for new code.
    ///// </summary>
    //[GeneratedRegex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.IgnoreCase)]
    //internal static partial Regex EmailExpression();

    ///// <summary>
    ///// Legacy support: matches URLs.
    ///// Use Url() instead for new code.
    ///// </summary>
    //[GeneratedRegex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$", RegexOptions.IgnoreCase)]
    //internal static partial Regex UrlExpression();

    #endregion
}
