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
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core
{
    ///<summary>Represents extension methods for <see cref="String"/> type.</summary>
	public static class StringExtensions
    {

        #region Is... string extensions

        /// <summary>
        /// Determines whether a string is a format string.
        /// </summary>
        /// <param name="text">The source string.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the string is a format string; otherwise, <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsFormatString(this string? text)
        {
            return !string.IsNullOrEmpty(text) && text.Contains("{0");
        }

        /// <summary>
        /// Determines whether a string is null or empty after trimming.
        /// </summary>
        /// <param name="text">The source string.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the string is <see langword="null"/> or empty after applying <see cref="M:System.String.Trim"/>; otherwise, <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsNullOrEmptyWithTrim(this string? text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            return text.Trim().Length == 0;
        }

        /// <summary>
        /// Determines whether the string is empty.
        /// </summary>
        /// <param name="text">The source string.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the string is empty; otherwise (including string is <see langword="null"/>) <see langword="false"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsEmpty(this string text)
        {
            if (text == null)
                return false;

            return (text.Length == 0);
        }

        /// <summary>
        /// Determines whether the two strings are equal irgnoring the case.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="targetText">The target text.</param>
        /// <returns>
        /// 	<see langword="true"/> if the two strings are equal irgnoring the case; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsEqualIgnoreCase(this string sourceText, string targetText)
        {
            return IsEqual(sourceText, targetText, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the two strings are equal using the provided compare mode.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="targetText">The target text.</param>
        /// <param name="compareMode">The string compare mode to use.</param>
        /// <returns>
        /// 	<see langword="true"/> if the two strings are equal using the provided compare mode; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsEqual(this string sourceText, string targetText, StringComparison compareMode)
        {
            if (sourceText.IsNull() && targetText.IsNull())
                return true;

            if (sourceText.IsEmpty() && targetText.IsEmpty())
                return true;

            return string.Equals(sourceText, targetText, compareMode);
        }


        #endregion

        #region Safe string extensions

        /// <summary>
        /// Safe Length operation.
        /// </summary>
        /// <param name="text">The source string.</param>
        /// <returns>The string length, or -1 if string value is <see langword="null"/>.</returns>
        [DebuggerStepThrough]
        public static int SafeLength(this string text)
        {
            return (text == null) ? -1 : text.Length;
        }

        /// <summary>
        /// Safe ToString operation.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>If string is <see langword="null"/> <see cref="F:System.String.Empty"/>; otherwise the string value.</returns>
        [return: NotNull]
        [DebuggerStepThrough]
        public static string SafeString(this string text)
        {
            return text ?? String.Empty;
        }

        /// <summary>
        /// Safe ToString operation.
        /// </summary>
        /// <param name="text">The source string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Returns the <see cref="SafeString(string)"/> value of <paramref name="defaultValue"/>, if the string is <see langword="null"/>; otherwise the string value.
        /// </returns>
        [return: NotNull]
        [DebuggerStepThrough]
        public static string SafeString(this string text, string defaultValue)
        {
            return text ?? defaultValue.SafeString();
        }

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
        /// Gets the first x number of characters from the left hand side
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">x number of characters to return</param>
        /// <returns>The resulting string</returns>
        public static string? Left(string input, int length)
        {
            if (input.IsNull())
                return null;
            if (input.Length <= length)
                return input;
            if (length <= 0)
                return string.Empty;

            return input[length..];
        }

        /// <summary>
        /// Gets the last x number of characters from the right hand side
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">x number of characters to return</param>
        /// <returns>The resulting string</returns>
        public static string? Right(string input, int length)
        {
            if (input.IsNull())
                return null;
            if (input.Length <= length)
                return input;
            if (length <= 0)
                return string.Empty;

            return input.Substring(input.Length - length, length);
        }

        /// <summary>
        /// Returns a string at most <paramref name="maxCount"/> long. If <paramref name="maxCount"/> is reached, ... is appended. 
        /// </summary>
        /// <param name="text">The text to clip.</param>
        /// <param name="maxCount">Max string length.</param>
        /// <returns>A string at most <paramref name="maxCount"/> long. If <paramref name="maxCount"/> is reached, ... is appended.</returns>
        [DebuggerStepThrough]
        public static string Clip(this string text, int maxCount)
        {
            return Clip(text, maxCount, "...");
        }

        /// <summary>
        /// Returns a string at most <paramref name="maxCount"/> long. If <paramref name="maxCount"/> is reached, <paramref name="clipText"/> is appended. 
        /// </summary>
        /// <param name="text">The text to clip.</param>
        /// <param name="maxCount">Max string length.</param>
        /// <param name="clipText">The clipping text.</param>
        /// <returns>A string at most <paramref name="maxCount"/> long. If <paramref name="maxCount"/> is reached, <paramref name="clipText"/> is appended.</returns>
        [DebuggerStepThrough]
        public static string Clip(this string text, int maxCount, string clipText)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string ellipseText = clipText ?? "...";

            if (maxCount <= 0)
                return string.Empty;

            if (text.Length > maxCount)
            {
                if (maxCount <= ellipseText.Length)
                    return text.Substring(0, maxCount);

                return (text.Substring(0, maxCount - ellipseText.Length) + ellipseText);
            }
            return text;
        }

        #endregion

        #region Join extensions

        /// <summary>
        /// String array to text (retruns string.Empty if stringArray is null or contains no items).
        /// </summary>
        /// <param name="stringArray">The string array.</param>
        /// <returns>Joined string array items separeted by ,</returns>
        public static string Join(this string[] stringArray)
        {
            if ((stringArray != null) && (stringArray.Length != 0))
            {
                return string.Join(",", stringArray);
            }
            return string.Empty;
        }

        /// <summary>
        /// String sequence to text (retruns string.Empty if stringSequence is null).
        /// </summary>
        /// <param name="stringSequence">The string sequence.</param>
        /// <returns>Joined string sequence items separeted by ,</returns>
        public static string Join(this IEnumerable<string> stringSequence)
        {
            if (stringSequence == null)
                return string.Empty;

            var stringArray = stringSequence.ToArray();

            return stringArray.Length == 0 ? string.Empty : string.Join(",", stringArray);
        }

        /// <summary>
        /// Joins the specified items using the default appender.
        /// </summary>
        /// <typeparam name="T">The item type of the <paramref name="items"/> collection.</typeparam>
        /// <param name="separator">The separator.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string Join<T>(string separator, ICollection<T> items)
        {
            return StringHelper.Join(separator, items, (sb, item) => sb.Append<T>(item));
        }

        /// <summary>
        /// Joins the specified items using the default appender.
        /// </summary>
        /// <typeparam name="T">The item type of the <paramref name="items"/> sequence.</typeparam>
        /// <param name="separator">The separator.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string Join<T>(string separator, IEnumerable<T> items)
        {
            return StringHelper.Join(separator, items, (sb, item) => sb.Append<T>(item));
        }


        #endregion

        #region IndexOf Extensions

        /// <summary>
        /// Returns the last index of the occurence of <paramref name="c"/>. -1 if not found; uses ordinal string comparison.
        /// </summary>
        /// <param name="source">The string to search in.</param>
        /// <param name="c">The char to search for.</param>
        /// <param name="start">The start index of the search.</param>
        /// <returns>Last index of the occurence of <paramref name="c"/>, or -1 if not found.</returns>
        public static int IndexOf(this string source, char c, int start)
        {
            for (int i = start; i < source.Length; i++)
            {
                if (source[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the last index of the occurence of <paramref name="sub"/>. -1 if not found; uses ordinal string comparison.
        /// </summary>
        /// <param name="source">The string to search in.</param>
        /// <param name="sub">The string to search for.</param>
        /// <param name="start">The start index of the search.</param>
        /// <returns>Last index of the occurence of <paramref name="sub"/> or -1 if not found.</returns>
        public static int IndexOf(this string source, string sub, int start)
        {
            if (source.IsNull() || sub.IsNull())
                return -1;

            if (sub.Length > source.Length)
                return -1;

            if (start < 0)
                start = 0;

            if (sub.Length == 0)
                return start;

            char ch = sub[0];
            int end = source.Length - sub.Length;
            for (int i = start; i <= end; i++)
            {
                if (source[i] != ch) continue;

                bool foundOtherInValue = true;
                for (var j = 1; j < sub.Length; j++)
                {
                    if (source[i + j] != sub[j])
                    {
                        foundOtherInValue = false;
                        break;
                    }
                }

                if (foundOtherInValue)
                    return i;
            }
            return -1;
        }

        #endregion

        #region Text formatting extensions

        /// <summary>
        /// Appends the line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="line">The line.</param>
        /// <returns>string where line was appended to text, adding Enviroment.NewLine before adding line string.</returns>
        [DebuggerStepThrough]
        public static string AppendLine(this string text, string line)
        {
            string s = text.SafeString();
            return ((((s.Length == 0) || s.EndsWith(Environment.NewLine)) ? s : (s + Environment.NewLine)) + line);
        }

        /// <summary>
        /// Convert the string value into a camel case formatted string (First letter is lower case).
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>
        /// Camel case formatted string.
        /// </returns>
        /// <example>
        /// <code lang="cs" title="String extension method ToCamelCase example" numberLines="true" outlining="true" >
        /// string text1 = "Test";
        /// string text2 = "\tTest";
        /// string text3 = "   Test";
        /// text1 = text1.ToCamelCase();
        /// text2 = text2.ToCamelCase();
        /// text3 = text3.ToCamelCase();</code>
        /// <para>After code execution <c>text1</c> value is <c>"test"</c>, <c>text2</c> value is <c>"\ttest"</c>, <c>text3</c> value is <c>"   test"</c></para>
        /// </example>
        [DebuggerStepThrough]
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length == 1)
                return value.ToLower();

            char[] inputChars = value.ToCharArray();
            for (int x = 0; x < inputChars.Length; ++x)
            {
                if (inputChars[x] == ' ' || inputChars[x] == '\t') continue;
                inputChars[x] = char.ToLowerInvariant(inputChars[x]);
                break;
            }
            return new string(inputChars);
        }

        /// <summary>
        /// Convert the string value into a pascal case formatted string (First letter is upper case).
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>
        /// Pascal case formatted string.
        /// </returns>
        /// <example>
        /// <code lang="cs" title="String extension method ToPascalCase example" numberLines="true" outlining="true" >
        /// string text1 = "test";
        /// string text2 = "\ttest";
        /// string text3 = "   test";
        /// text1 = text1.ToPascalCase();
        /// text2 = text2.ToPascalCase();
        /// text3 = text3.ToPascalCase();</code>
        /// <para>After code execution <c>text1</c> value is <c>"Test"</c>, <c>text2</c> value is <c>"\tTest"</c>, <c>text3</c> value is <c>"   Test"</c></para>
        /// </example>
        [DebuggerStepThrough]
        public static string ToPascalCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length == 1)
                return value.ToUpper();

            char[] inputChars = value.ToCharArray();
            for (int x = 0; x < inputChars.Length; ++x)
            {
                if (inputChars[x] == ' ' || inputChars[x] == '\t') continue;
                inputChars[x] = char.ToUpperInvariant(inputChars[x]);
                break;
            }
            return new string(inputChars);

        }

        /// <summary>
        /// Convert a string from it's source encoding represantion to a Unicode encoded string.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="sourceEncoding">The source encoding.</param>
        /// <returns>
        /// The Unicode encoded string.
        /// </returns>
        public static string ToUnicodeString(this string sourceText, Encoding sourceEncoding)
        {
            if (String.IsNullOrEmpty(sourceText))
                return sourceText;
            if (sourceEncoding == null)
                return sourceText;
            if (sourceEncoding.EncodingName == Encoding.Unicode.EncodingName)
                return sourceText;

            Encoding unicodeEncoding = Encoding.Unicode;
            byte[] sourceBytes = sourceEncoding.GetBytes(sourceText);
            string unicodeString = unicodeEncoding.GetString(sourceBytes);
            return unicodeString;
        }

        /// <summary>
        /// Ensures that a string starts with a given prefix.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="prefix">The prefix value to check for.</param>
        /// <returns>The string value including the prefix</returns>
        /// <example>
        /// <code lang="cs" title="String extension method EnsureStartsWith example" numberLines="true" outlining="true" >
        /// string extension1 = "txt";
        /// string extension2 = ".txt";
        /// string fileName = "Test";
        /// 
        /// string fileName1 = string.Concat(fileName, extension1.EnsureStartsWith("."));
        /// string fileName2 = string.Concat(fileName, extension2.EnsureStartsWith("."));</code>
        /// <para>After code execution <c>fileName1</c> value and <c>fileName2</c> value is <c>"Test.txt"</c></para>
        /// </example>
        [DebuggerStepThrough]
        public static string EnsureStartsWith(this string value, string prefix)
        {
            if (value.StartsWith(prefix)) return value;
            return string.Concat(prefix, value);
        }

        /// <summary>
        /// Ensures that a string ends with a given suffix.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="suffix">The suffix value to check for.</param>
        /// <returns>The string value including the suffix</returns>
        /// <example>
        /// <code lang="cs" title="String extension method EnsureEndsWith example" numberLines="true" outlining="true" >
        /// string url1 = "http://www.smartexpert.boercsoek.de";
        /// string url2 = "http://www.smartexpert.boercsoek.de/";
        /// url1 = url1.EnsureEndsWith("/");
        /// url2 = url2.EnsureEndsWith("/");</code>
        /// <para>After code execution <c>url1</c> value and <c>url2</c> value is <c>"http://www.smartexpert.boercsoek.de/"</c></para>
        /// </example>
        [DebuggerStepThrough]
        public static string EnsureEndsWith(this string value, string suffix)
        {
            if (value.EndsWith(suffix)) return value;
            return string.Concat(value, suffix);
        }

        /// <summary>
        /// If the string contains spaces, surrounds it with quotes.
        /// </summary>
        /// <returns>The quoted string if quoted is needed (containing spaces).</returns>
        public static string QuoteIfNeeded(this string? s)
        {
            if (s == null)
            {
                return "<NULL>";
            }
            if ((s.Length != 0) && !s.Contains(" "))
            {
                return s;
            }
            if (((s.Length > 0) && (s[0] == '“')) && (s[s.Length - 1] == '”'))
            {
                return s;
            }
            return ('“' + s + '”');
        }



        /// <summary>
        /// Takes a capitalized word and turns it into a sequence of word:
        /// MyName -&gt; my name
        /// </summary>
        /// <param name="value">The string that should be formatted as sentence.</param>
        /// <returns>The formatted string result.</returns>
        public static string FormatAsSentence(this string value)
        {
            if (value.Length == 0)
            {
                return string.Empty;
            }
            char[] chArray = new char[value.Length * 2];
            int length = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        chArray[length++] = ' ';
                    }
                    chArray[length++] = char.ToLowerInvariant(c);
                }
                else
                {
                    chArray[length++] = c;
                }
            }
            return new string(chArray, 0, length);
        }

        /// <summary>
        /// In a string, replaces null characters ('\0') with "\\0".
        /// </summary>
        /// <returns>A string where all null characters are replaced.</returns>
        public static string ReplaceNullChars(this string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            int length = input.Length;
            bool foundNullChar = false;
            for (int i = 0; i < length; i++)
            {
                if (input[i] == '\0')
                {
                    foundNullChar = true;
                    break;
                }
            }
            if (!foundNullChar)
            {
                return input;
            }

            char[] chArray = new char[length * 2];
            int buildIndex = 0;
            for (int j = 0; j < length; j++)
            {
                if (input[j] == '\0')
                {
                    chArray[buildIndex++] = '\\';
                    chArray[buildIndex++] = '0';
                }
                else
                {
                    chArray[buildIndex++] = input[j];
                }
            }
            return new string(chArray, 0, buildIndex);
        }

        #endregion

        #region RegEx string extensions

        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <returns>
        /// 	<see langword="true"/> if the value is matching to the specified pattern; otherwise, <see langword="false"/>.
        /// </returns>
        /// <example>
        /// <code lang="cs" title="String extension method IsMathingTo example" numberLines="true" outlining="true" >
        /// string s = "12345";
        /// bool isMatching = s.IsMatchingTo(@"^\d+$");</code>
        /// <para>Value of <c>isMatching</c> after execution = <c>true</c>.</para>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern)
        {
            return IsMatchingTo(value, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>
        /// 	<see langword="true"/> if the value is matching to the specified pattern; otherwise, <see langword="false"/>.
        /// </returns>
        /// <example>
        /// <code lang="cs" title="String extension method IsMathingTo example" numberLines="true" outlining="true" >
        /// string s = "12345";
        /// bool isMatching = s.IsMatchingTo(@"^\d+$", RegexOptions.None);</code>
        /// <para>Value of <c>isMatching</c> after execution = <c>true</c>.</para>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.IsMatch(value, regexPattern, options);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="replaceValue">The replacement value.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code lang="cs" title="String extension method ReplaceWith example" numberLines="true" outlining="true" >
        /// string s = "12345";
        /// string replaced = s.ReplaceWith(@"\d", "&lt;number&gt;"+s+"&lt;/number&gt;");</code>
        /// <para>Value of <c>replaced</c> after execution = <c>"&lt;number&gt;12345&lt;/number&gt;"</c>.</para>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue)
        {
            return ReplaceWith(value, regexPattern, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="replaceValue">The replacement value.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code lang="cs" title="String extension method ReplaceWith example" numberLines="true" outlining="true" >
        /// string s = "12345";
        /// string replaced = s.ReplaceWith(@"\d", "&lt;number&gt;"+s+"&lt;/number&gt;", RegexOptions.None);</code>
        /// <para>Value of <c>replaced</c> after execution = <c>"&lt;number&gt;12345&lt;/number&gt;"</c>.</para>
        /// </example>
        [DebuggerStepThrough]
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue, RegexOptions options)
        {
            return Regex.Replace(value, regexPattern, replaceValue, options);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code lang="cs" title="String extension method ReplaceWith example" numberLines="true" outlining="true" >
        /// string s = "12345";
        /// string replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));</code>
        /// <para>Value of <c>replaced</c> after execution = <c>" -12345- "</c>.</para>
        /// </example>
        [DebuggerStepThrough]
        public static string ReplaceWith(this string value, string regexPattern, MatchEvaluator evaluator)
        {
            return ReplaceWith(value, regexPattern, RegexOptions.None, evaluator);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <param name="evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code lang="cs" title="String extension method ReplaceWith example" numberLines="true" outlining="true" >
        /// string s = "12345";
        /// string replaced = s.ReplaceWith(@"\d", RegexOptions.None, m => string.Concat(" -", m.Value, "- "));</code>
        /// <para>Value of <c>replaced</c> after execution = <c>" -12345- "</c>.</para>
        /// </example>
        [DebuggerStepThrough]
        public static string ReplaceWith(this string value, string regexPattern, RegexOptions options, MatchEvaluator evaluator)
        {
            return Regex.Replace(value, regexPattern, evaluator, options);
        }

        /// <summary>
        /// Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <returns>A collection of all matches</returns>
        [DebuggerStepThrough]
        public static MatchCollection GetMatches(this string value, string regexPattern)
        {
            return GetMatches(value, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>A collection of all matches</returns>
        [DebuggerStepThrough]
        public static MatchCollection GetMatches(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.Matches(value, regexPattern, options);
        }

        /// <summary>
        /// Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <returns>An enumeration of matching strings</returns>
        /// <example>
        /// <code lang="cs" title="String extension method GetMatchingValues example" numberLines="true" outlining="true" >
        /// string s = "12345";
        /// foreach(string number in s.GetMatchingValues(@"\d")) 
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
        [DebuggerStepThrough]
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern)
        {
            return GetMatchingValues(value, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="regexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>An enumeration of matching strings</returns>
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
        [DebuggerStepThrough]
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern, RegexOptions options)
        {
            return from match in GetMatches(value, regexPattern, options).UnsafeToArray<Match>()
                   where match.Success
                   select match.Value;
        }


        #endregion

        #region Text filter extensions

        /// <summary>
        /// Removes the filter text from the input.
        /// </summary>
        /// <param name="input">Input text</param>
        /// <param name="filter">Regex expression of text to filter out</param>
        /// <returns>The input text minus the filter text.</returns>
        public static string? FilterOutText(this string input, string filter)
        {
            if (input.IsNull())
                return null;
            if (filter.IsNull())
                return input;

            Regex tempRegex = new Regex(filter);
            return tempRegex.Replace(input, "");
        }

        /// <summary>
        /// Removes everything that is not in the filter text from the input.
        /// </summary>
        /// <param name="input">Input text</param>
        /// <param name="filter">Regex expression of text to keep</param>
        /// <returns>The input text minus everything not in the filter text.</returns>
        public static string? KeepFilterText(this string input, string filter)
        {
            if (input.IsNull())
                return null;
            if (filter.IsNull())
                return input;

            Regex tempRegex = new Regex(filter);
            MatchCollection collection = tempRegex.Matches(input);
            StringBuilder builder = new StringBuilder();
            foreach (Match match in collection)
            {
                builder.Append(match.Value);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Keeps only alphanumeric characters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>the string only containing alphanumeric characters</returns>
        public static string? AlphaNumericOnly(this string input)
        {
            if (input.IsNullOrEmptyWithTrim())
                return string.Empty;

            return KeepFilterText(input, "[a-zA-Z0-9]");
        }

        /// <summary>
        /// Keeps only alpha characters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>the string only containing alpha characters</returns>
        public static string? AlphaCharactersOnly(this string input)
        {
            if (input.IsNullOrEmptyWithTrim())
                return string.Empty;

            return KeepFilterText(input, "[a-zA-Z]");

        }

        /// <summary>
        /// Keeps only numeric characters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="keepNumericPunctuation">Determines if decimal places should be kept</param>
        /// <returns>the string only containing numeric characters</returns>
        public static string? NumericOnly(this string input, bool keepNumericPunctuation)
        {
            if (input.IsNullOrEmptyWithTrim())
                return string.Empty;

            return KeepFilterText(input, keepNumericPunctuation ? @"[0-9,\.]" : "[0-9]");
        }

        #endregion

        #region SecureString extensions

        /// <summary>
        /// Convert <see cref="String"/> to type <see cref="System.Security.SecureString"/>.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>
        /// The convertion result (instance of target type <see cref="System.Security.SecureString"/>).
        /// </returns>
        public static SecureString? ToSecureString(this string source)
        {
            if (source == null)
                return null;

            SecureString secureString = new SecureString();

            foreach (char key in source)
                secureString.AppendChar(key);

            secureString.MakeReadOnly();

            return secureString;
        }

        /// <summary>
        /// Convert <see cref="System.Security.SecureString"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="secureString">The secure string to convert.</param>
        /// <returns>
        /// The converted string.
        /// </returns>
        public static string? ToUnsecureString(this SecureString secureString)
        {
            if (secureString == null)
                return null;

            IntPtr ptrBstr = Marshal.SecureStringToBSTR(secureString);
            string str = Marshal.PtrToStringBSTR(ptrBstr);
            Marshal.ZeroFreeBSTR(ptrBstr);
            return str;
        }

        #endregion

        #region String to type conversion extensions

        /// <summary>
        /// Gets the bytes from string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The characters of the string as a sequence of bytes.</returns>
        public static byte[] ToByteArray(this string str)
        {
            return StringHelper.GetBytesFromString(str);
        }

        /// <summary>
        /// Convert the string value into a to type <see cref="System.IO.FileInfo"/> object.
        /// </summary>
        /// <param name="value">The current string value.</param>
        /// <returns>
        /// The convertion result (instance of target type <see cref="System.IO.FileInfo"/>, or <see langword="null"/> if file does not exist.).
        /// </returns>
        [DebuggerStepThrough]
        public static FileInfo? ToFileInfo(this string value)
        {
            if (string.IsNullOrEmpty(value) || File.Exists(value) == false)
                return null;

            return new FileInfo(value);
        }

        #endregion

    }
}
