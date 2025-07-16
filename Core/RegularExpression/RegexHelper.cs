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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// The Regex replacement callback delegate.
	/// </summary>
	/// <param name="capturedIndex">The current captured index.</param>
	/// <param name="capturedValue">The current captured value.</param>
	/// <returns>The value that replaces the captured value.</returns>
	public delegate string CallbackRegexReplacement(int capturedIndex, string capturedValue);

    /// <summary>
    /// Common regular expressions.
    /// </summary>
    public static class RegexHelper
    {
        #region Regex Constants

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Email = new Regex(RegexExpressionStrings.EmailExpression, RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Url = new Regex(RegexExpressionStrings.UrlExpression, RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex NonWordDigitRegex = new Regex(@"[^a-zA-Z0-9]+", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Uri = new Regex(@"\w+://" + RegexExpressionStrings.UriChars, RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex UriLenient = new Regex(@"(\w+://)?" + RegexExpressionStrings.UriChars, RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static Regex HtmlBreak = new Regex(RegexExpressionStrings.HtmlBreakExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static Regex HtmlBreakOrParagraph = new Regex(RegexExpressionStrings.HtmlBreakOrParagraphExpression,
                                                             RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static Regex HtmlBreakOrParagraphTrim =
            new Regex(
                string.Format("({0})|({1})", RegexExpressionStrings.HtmlBreakOrParagraphTrimLeftExpression, RegexExpressionStrings.HtmlBreakOrParagraphTrimRightExpression),
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static Regex HtmlParagraph = new Regex(RegexExpressionStrings.HtmlParagraphExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Gets the capture. Group number 0 is the entire match. Group
        /// number 1 is the first matched group from the left, and so on.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <returns>The capture (matching group content) at the specified group number, 
        /// or null if match.Success is false, or groupNumber is greater or equal group count.</returns>
        public static string? GetCapture(Match match, int groupNumber)
        {
            if (match.Success && match.Groups.Count > groupNumber)
            {
                return match.Groups[groupNumber].ToString();
            }

            return null;
        }

        /// <summary>
        /// Gets the last capture.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The last capture of the match.</returns>
        public static string? GetLastCapture(Match match)
        {
            return GetLastCapture(match, 0);
        }

        /// <summary>
        /// Gets the last Nth capture, specified by <paramref name="offset"/>.
        /// If <paramref name="offset"/> is 0, then this will return the last
        /// capture. If it is 1, then this will return the second-to-last
        /// capture and so on.
        /// </summary>
        /// <param name="match">The Regex match result.</param>
        /// <param name="offset">The Nth last capture. If 0, then this will return the last
        /// capture. If it is 1, then this will return the second-to-last
        /// capture and so on.</param>
        /// <returns>The last capture of the match.</returns>
        public static string? GetLastCapture(Match match, int offset)
        {
            return GetCapture(match, match.Groups.Count - 1 - offset);
        }

        /// <summary>
        /// Matches any.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="regexs">The regexs.</param>
        /// <returns>The <paramref name="regexs"/> index of the first successful match or -1 if <paramref name="input"/> does not match at all.</returns>
        public static int MatchAny(string input, params Regex[] regexs)
        {
            return MatchAny(input, out _, regexs);
        }

        /// <summary>
        /// Checks if <paramref name="input"/> matches any regex expressions
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="successfulMatch">[Out] Contains the successful match or null if MatchAny returns -1.</param>
        /// <param name="regexs">The regexs.</param>
        /// <returns>The <paramref name="regexs"/> index of the first successful match or -1 if <paramref name="input"/> does not match at all.</returns>
        public static int MatchAny(string input, out Match? successfulMatch, params Regex[] regexs)
        {
            successfulMatch = null;
            if (regexs != null)
            {
                for (int i = 0; i < regexs.Length; i++)
                {
                    Match m = regexs[i].Match(input);
                    if (!m.Success) continue;
                    successfulMatch = m;
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if <paramref name="input"/> matches all regex expressions
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="regexs">The regexs.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="input"/> matches all <paramref name="regexs"/>, otherwise <see langword="false"/></returns>
        public static bool MatchAll(string input, params Regex[] regexs)
        {
            bool matchAllResult = true;
            if (regexs != null)
            {
                matchAllResult = regexs.Select(t => t.Match(input)).All(m => m.Success);
            }
            return matchAllResult;
        }

        /// <summary>
        /// Returns the capture value of a specific capture index.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="regularExpression">The regular expression.</param>
        /// <param name="captureIndex">The capture index to return.</param>
        /// <returns>The capture value of a specific capture index or <see langword="null"/> if no match was found.</returns>
        /// <remarks>Capture index number 0 is the entire match. capture index 1 is the first matched group from the left, and so on.</remarks>
        public static string? Extract(string input, string regularExpression, int captureIndex)
        {
            Match m = Regex.Match(input, regularExpression);
            if (m.Success)
            {
                return GetCapture(m, captureIndex);
            }
            return null;
        }

        /// <summary>
        /// Returns the capture value of a specific capture index.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="regularExpression">The regular expression.</param>
        /// <param name="captureIndex">The capture index to return.</param>
        /// <param name="options">The regex options that should be used.</param>
        /// <returns>The capture value of a specific capture index or <see langword="null"/> if no match was found.</returns>
        /// <remarks>Capture index number 0 is the entire match. capture index 1 is the first matched group from the left, and so on.</remarks>
        public static string? Extract(string input, string regularExpression, int captureIndex, RegexOptions options)
        {
            Match m = Regex.Match(input, regularExpression, options);
            if (m.Success)
            {
                return GetCapture(m, captureIndex);
            }
            return null;
        }

        /// <summary>
        /// Replace matches with replace expression.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="regularExpression">The regular expression.</param>
        /// <param name="replaceExpression">The replacement expression.</param>
        /// <returns>The RegEx replace result string.</returns>
        /// <example>
        /// <para>Input:  "TestColumn.Value"</para>
        /// <para>Output: "TestColumn.Value as TestColumn"</para>
        /// <code>
        /// string result = RegexUtilities.ReplaceAll("TestColumn.Value", @"^(?&lt;ValueExp&gt;[\s]*(?&lt;ValueName&gt;[\S]*)\.Value)", "${ValueExp} as ${ValueName}");
        /// </code>
        /// </example>
        public static string ReplaceAll(string input, string regularExpression, string replaceExpression)
        {
            string resultString = string.Empty;
            try
            {
                Regex regexObj = new Regex(regularExpression, RegexOptions.Multiline);
                resultString = regexObj.Replace(input, replaceExpression);
            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
            }
            return resultString;
        }

        /// <summary>
        /// Replaces the capture value of a specific capture index in all matching groups with the specified static replacement.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="regularExpression">The regular expression.</param>
        /// <param name="captureIndex">The capture index that should be replaced.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <returns>The RegEx replaced input string.</returns>
        public static string Replace(string input, string regularExpression, int captureIndex, string replacement)
        {
            return Replace(input, regularExpression, captureIndex, new StaticStringReplacer(replacement).Replace);
        }

        /// <summary>
        /// Replaces the capture value of a specific capture index in all matching groups with the value returned by the replacemant delegate.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="regularExpression">The regular expression.</param>
        /// <param name="captureIndex">The capture index to replace.</param>
        /// <param name="replacement">The replacement delegate.</param>
        /// <returns>The RegEx replaced input string.</returns>
        public static string Replace(string input, string regularExpression, int captureIndex, CallbackRegexReplacement replacement)
        {
            Match m = Regex.Match(input, regularExpression);
            StringBuilder result = new StringBuilder(input.Length);
            while (m.Success)
            {
                Group g = m.Groups[captureIndex];
                string[] pieces = StringHelper.SplitOn(input, g.Index, false);
                result.Append(pieces[0]);
                result.Append(replacement(captureIndex, g.ToString()));
                input = pieces[1].Substring(g.Length);

                // Get the next match
                m = Regex.Match(input, regularExpression);
            }
            result.Append(input);
            return result.ToString();
        }

        /// <summary>
        /// Remove all non word characters from the input string
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>Output string where all non word characters have been removed.</returns>
        public static string RemoveNonWordCharacters(string input)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(input))
            {
                result = NonWordDigitRegex.Replace(input, "");
            }
            return result;
        }

        #endregion

        #region Nested type: StaticStringReplacer

        private class StaticStringReplacer
        {
            private string m_Replacement;

            public StaticStringReplacer(string replacement)
            {
                m_Replacement = replacement;
            }

            public string Replace(int capturedIndex, string capturedValue)
            {
                return m_Replacement;
            }
        }

        #endregion
    }
}
