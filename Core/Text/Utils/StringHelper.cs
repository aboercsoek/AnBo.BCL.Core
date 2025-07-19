//--------------------------------------------------------------------------
// File:    StringHelper.cs
// Content:	Implementation of String helper class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// String manipulation and generation methods, as well as string array manipulation.
	/// </summary>
	public static class StringHelper
    {
        #region Private and Public Static Members

        /// <summary>
        /// Char array with default quote char (").
        /// </summary>
        public static readonly char[] DefaultQuoteSensitiveChars = new[] { '\"' };

        private static Random m_Random;

        #endregion

        #region Static Ctor

        /// <summary>
        /// Static ctor
        /// </summary>
        static StringHelper()
        {
            m_Random = new Random(unchecked((int)DateTime.UtcNow.Ticks));
        }

        #endregion

        #region Safe ToString methods

        /// <summary>
        /// Safe ToString-Operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>If <paramref name="obj"/> is null String.Empty; otherwise obj.ToString().</returns>
        [DebuggerStepThrough]
        public static string SafeToString(object obj)
        {
            try
            {
                return (obj == null) ? String.Empty : obj.ToInvariantString();
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;
            }
            return String.Empty;
        }

        /// <summary>
        /// Safe ToString-Operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>If <paramref name="obj"/> is <see langword="null"/> the safe ToString value of <paramref name="defaultValue"/>; otherwise the value of obj.ToString().</returns>
        [DebuggerStepThrough]
        public static string SafeToString(object? obj, string defaultValue)
        {
            try
            {
                return (obj == null) ? SafeToString(defaultValue) : obj.ToInvariantString();
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;
            }

            return SafeToString(defaultValue);
        }

        #endregion

        #region Safe string formating methods

        /// <summary>
        /// Formats the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        [return: NotNull]
        [DebuggerStepThrough]
        public static string SafeFormat(string? format, params object?[] args)
        {
            if (format == null)
                return String.Empty;

            if (args == null || args.Length == 0)
                return format;

            try
            {
                //return String.Format(format, args);
                return String.Format(format, args.Select(arg => arg.ToInvariantString()).ToArray());
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;

                StringBuilder sb = new StringBuilder();
                FormatFallback(ex, sb, format, args);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Appends the format.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        [DebuggerStepThrough]
        public static void SafeAppendFormat(StringBuilder sb, string format, params object[] args)
        {
            if (sb == null)
                return;

            if (String.IsNullOrEmpty(format))
                return;

            if (args == null || args.Length == 0)
            {
                sb.Append(format);
                return;
            }

            try
            {
                sb.Append(SafeFormat(format, args));
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;

                FormatFallback(ex, sb, format, args);
            }
        }

        /// <summary>
        /// Format fallback method.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="sb">The sb.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        [DebuggerStepThrough]
        private static void FormatFallback(Exception ex, StringBuilder sb, string format, params object?[] args)
        {
            if (sb == null)
                return;

            sb.Append("*** Exception occured during formatting: ");

            if (ex != null)
            {
                sb.Append(ex.GetType().FullName).Append(": ")
                  .Append(ex.Message).Append(Environment.NewLine);
            }
            else
            {
                sb.Append(Environment.NewLine);
            }

            sb.Append("SafeFormat: '").Append(format.SafeString("<null>")).Append("'").Append(Environment.NewLine);

            if (args == null)
                sb.Append("args: <null>").Append(Environment.NewLine);
            else
            {
                for (int i = 0; i < args.Length; ++i)
                    sb.Append("arg #")
                      .Append(i).Append(": '")
                      .Append(SafeToString(args[i], "<null>")).Append("'")
                      .Append(Environment.NewLine);
            }
        }

        #endregion

        #region String List to Multiline String

        /// <summary>
        /// Converts a string list into a multi line string. Puts \n between the lines
        /// </summary>
        /// <param name="strList">The string list</param>
        /// <returns>The multi line string</returns>
        [DebuggerStepThrough]
        public static string StringList2MultiLine(ICollection<string> strList)
        {
            if (strList == null)
                return string.Empty;
            if (strList.Count == 1)
                return strList.First().SafeString();

            StringBuilder sb = new StringBuilder();
            int maxCount = strList.Count;
            int currentCount = 1;
            foreach (string item in strList)
            {
                if (currentCount < maxCount)
                    sb.AppendLine(item.SafeString());
                else
                    sb.Append(item.SafeString());
                currentCount++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a string array into a multi line string. Puts \n between the lines
        /// </summary>
        /// <param name="strArray">The string array</param>
        /// <returns>The multi line string</returns>
        [DebuggerStepThrough]
        public static string StringArray2MultiLine(string[] strArray)
        {
            if (strArray == null)
                return string.Empty;
            if (strArray.Length == 1)
                return strArray[0].SafeString();

            StringBuilder sb = new StringBuilder();
            int maxCount = strArray.Length;
            int currentCount = 1;
            foreach (string item in strArray)
            {
                if (currentCount < maxCount)
                    sb.AppendLine(item.SafeString());
                else
                    sb.Append(item.SafeString());
                currentCount++;
            }
            return sb.ToString();
        }

        #endregion

        #region Join Methods

        /// <summary>
        /// Concatenates a specified separator System.String between each element of
        /// <paramref name="collection"/>, yielding a single concatenated string.
        /// </summary>
        /// <param name="collection">Collection of strings.</param>
        /// <param name="separator">A System.String.</param>
        /// <returns>A System.String consisting of the elements of value interspersed with the separator string.</returns>
        public static string Join(this IEnumerable<string> collection, string separator)
        {
            #region PreConditions

            ArgChecker.ShouldNotBeNull(collection);
            ArgChecker.ShouldNotBeNull(separator);

            #endregion

            return string.Join(separator, collection.ToArray());
        }

        /// <summary>
        /// Joins the variable char-array chars to a string. 
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="chars">The chars.</param>
        /// <returns></returns>
        public static string? Join(string separator, params char[] chars)
        {
            string? result = null;

            if (chars != null)
            {
                int l = chars.Length;
                for (int i = 0; i < l; i++)
                {
                    if (i > 0)
                    {
                        result += separator;
                    }
                    result += chars[i];
                }
            }

            return result;
        }


        /// <summary>
        /// Joins the specified items using the default appender.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string Join<T>(string separator, params T[] items)
        {
            return Join(separator, items, (sb, item) => sb.Append<T>(item));
        }

        /// <summary>
        /// Joins array of Type T values using the specified separator.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="items">The items.</param>
        /// <param name="appender">The appender (Convert type T object to string an append the result to a given StringBuilder).</param>
        /// <returns>The joined items.</returns>
        public static string Join<T>(string separator, T[] items, Action<StringBuilder, T> appender)
        {
            if (items.Length == 0)
            {
                return string.Empty;
            }
            if (separator == null)
            {
                separator = string.Empty;
            }
            StringBuilder builder = new StringBuilder(items.Length * (separator.Length + 10));
            bool flag = true;
            foreach (T local in items)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(separator);
                }
                appender(builder, local);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Joins the specified items using a custom
        /// appender.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="items">The items.</param>
        /// <param name="appender">The appender.</param>
        /// <returns></returns>
        public static string Join<T>(string separator, ICollection<T> items, Action<StringBuilder, T> appender)
        {
            if (items.Count == 0)
            {
                return string.Empty;
            }
            if (separator == null)
            {
                separator = string.Empty;
            }
            StringBuilder builder = new StringBuilder(items.Count * (separator.Length + 10));
            bool flag = true;
            foreach (T local in items)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(separator);
                }
                appender(builder, local);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Joins sequence of Type T items using the specified separator.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="items">The items.</param>
        /// <param name="appender">The appender (Convert type T object to string an append the result to a given StringBuilder).</param>
        /// <returns>The joined items.</returns>
        public static string Join<T>(string separator, IEnumerable<T> items, Action<StringBuilder, T> appender)
        {
            if (separator == null)
            {
                separator = string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            bool firstIteration = true;
            foreach (T local in items)
            {
                if (firstIteration)
                {
                    firstIteration = false;
                }
                else
                {
                    builder.Append(separator);
                }

                appender(builder, local);
            }
            return builder.ToString();
        }

        #endregion

        #region Byte-Array convertion to and from string

        /// <summary>
        /// Gets the bytes from string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The characters of the string as a sequence of bytes.</returns>
        public static byte[] GetBytesFromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new byte[0];

            // Strings in .NET are always UTF16
            return Encoding.Unicode.GetBytes(str);
        }

        /// <summary>
        /// Gets the string from bytes.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The decoding string result.</returns>
        public static string GetStringFromBytes(byte[] data)
        {
            // Strings in .NET are always UTF16
            return Encoding.Unicode.GetString(data);
        }

        #endregion

        #region Padding Methods

        /// <summary>
        /// Returns a string of length <paramref name="length"/> with 0's padded to the left, if necessary.
        /// </summary>
        /// <param name="val">The padding value.</param>
        /// <param name="length">The padding length.</param>
        /// <returns>Returns a string of length <paramref name="length"/> with 0's padded to the left, if necessary.</returns>
        public static string PadIntegerLeft(int val, int length)
        {
            return PadIntegerLeft(val, length, '0');
        }

        /// <summary>
        /// Pads the integer left.
        /// </summary>
        /// <param name="val">The padding value.</param>
        /// <param name="length">The padding length.</param>
        /// <param name="pad">The padding char.</param>
        /// <returns>The the padding left string result.</returns>
        public static string PadIntegerLeft(int val, int length, char pad)
        {
            string result = val.ToString();
            while (result.Length < length)
            {
                result = pad + result;
            }
            return result;
        }

        /// <summary>
        /// Returns a string of length <paramref name="length"/> with
        /// 0's padded to the right, if necessary.
        /// </summary>
        /// <param name="val">The padding value.</param>
        /// <param name="length">The padding length.</param>
        /// <returns>The the padding right string result.</returns>
        public static string PadIntegerRight(int val, int length)
        {
            return PadIntegerRight(val, length, '0');
        }

        /// <summary>
        /// Pads the integer right.
        /// </summary>
        /// <param name="val">The value to pad.</param>
        /// <param name="length">The padding length.</param>
        /// <param name="pad">The padding char.</param>
        /// <returns>The the padding right string result.</returns>
        public static string PadIntegerRight(int val, int length, char pad)
        {
            string result = val.ToString();
            while (result.Length < length)
            {
                result += pad;
            }
            return result;
        }

        #endregion

        #region Remove Chars Methods

        /// <summary>
        /// Removes all characters passed in from the string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string RemoveCharacters(string str, params char[] chars)
        {
            if (chars != null)
            {
                str = Regex.Replace(str, "[" + new string(chars) + "]+", "");
            }
            return str;
        }

        /// <summary>
        /// Remove all characters that are not in the passed in array from the string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string RemoveCharactersInverse(string str, params char[] chars)
        {
            if (chars != null)
            {
                str = Regex.Replace(str, "[^" + new string(chars) + "]+", "");
            }
            return str;
        }

        #endregion


        /// <summary>
        /// Returns a string of length <paramref name="size"/> filled
        /// with random ASCII characters in the range A-Z, a-z. If <paramref name="lowerCase"/>
        /// is <see langword="true"/>, then the range is only a-z.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="lowerCase">if set to <see langword="true"/> [lower case].</param>
        /// <returns>The generated random string.</returns>
        /// <exception cref="ArgOutOfRangeException{TValue}">Is thrown if <paramref name="size"/> is less than 0 or greater than 4096.</exception>
        public static string RandomString(int size, bool lowerCase)
        {
            ArgChecker.ShouldBeInRange(size, 0, 4096);

            StringBuilder builder = new StringBuilder(size);
            int low = 65; // 'A'
            int high = 91; // 'Z' + 1
            if (lowerCase)
            {
                low = 97; // 'a';
                high = 123; // 'z' + 1
            }
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(m_Random.Next(low, high));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Calculates the CRC32.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>CRC32 value of the string.</returns>
        public static uint CalculateCrc32(string str)
        {
            return Crc32Helper.Compute(str);
        }

        /// <summary>
        /// Replaces NewLine character with HTML br element.
        /// </summary>
        /// <param name="text">String to convert.</param>
        /// <returns>Converted string</returns>
        public static string ReplaceNewLineWithHtmlBr(string text)
        {
            string result = text;

            if (!string.IsNullOrEmpty(text))
            {
                result = text.Replace("\r\n", "<br />").Replace("\n", "<br />");
            }
            return result;
        }


        /// <summary>
        /// Formats the <paramref name="value" /> to a string, adding leading zeros so that all of the numbers up to <paramref name="maxvalue" />, inclusively, had the same number of characters in their string representation when formatted thru this function.
        /// </summary>
        public static string ToStringWithLeading(int value, int maxvalue, CultureInfo culture)
        {
            if (value >= maxvalue)
            {
                return value.ToString(culture);
            }
            return value.ToString(string.Format("D{0}", ((int)Math.Floor(Math.Log((double)maxvalue, 10.0))) + 1), culture);
        }

        /// <summary>
        /// Splits <paramref name="str"/> based on the index. The first element
        /// is the left portion, and the second element
        /// is the right portion. The character at index <paramref name="index"/>
        /// is either included at the end of the left portion, or at the
        /// beginning of the right portion, depending on <paramref name="isIndexInFirstPortion"/>
        /// The return result is never null, and the elements
        /// are never null, so one of the elements may be an empty string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="index">The index.</param>
        /// <param name="isIndexInFirstPortion">if set to <see langword="true"/> [is index in first portion].</param>
        /// <returns>The Split-Operation string-Array result.</returns>
        public static string[] SplitOn(string str, int index, bool isIndexInFirstPortion)
        {
            string one, two;
            if (index == -1)
            {
                one = str;
                two = "";
            }
            else
            {
                if (index == 0)
                {
                    if (isIndexInFirstPortion)
                    {
                        one = str[0].ToString();
                        two = str.Substring(1);
                    }
                    else
                    {
                        one = "";
                        two = str;
                    }
                }
                else if (index == str.Length - 1)
                {
                    if (isIndexInFirstPortion)
                    {
                        one = str;
                        two = "";
                    }
                    else
                    {
                        one = str.Substring(0, str.Length - 1);
                        two = str[str.Length - 1].ToString();
                    }
                }
                else
                {
                    one = str.Substring(0, isIndexInFirstPortion ? index + 1 : index);
                    two = str.Substring(isIndexInFirstPortion ? index + 1 : index);
                }
            }

            return new[] { one, two };
        }

        /// <summary>
        /// Splits a string in order to create a square of lines
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static string[] SquareChunk(string value, params char[] separators)
        {
            if (value.Length == 0)
            {
                return new string[] { string.Empty };
            }
            List<int> list = new List<int>();
            int num2 = (int)Math.Sqrt((double)value.Length);
            int num3 = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (separators.Contains<char>(ch))
                {
                    num2 = Math.Max(num2, i - num3);
                    list.Add(num3 = i);
                }
            }
            num2 = Math.Max(num2, 1);
            list.Add(value.Length);
            List<string> list2 = new List<string>();
            int startIndex = 0;
            for (int j = 0; j < list.Count; j++)
            {
                int num7 = list[j];
                int length = num7 - startIndex;
                if (length >= num2)
                {
                    list2.Add(value.Substring(startIndex, length));
                    startIndex = num7;
                }
            }
            if (startIndex < value.Length)
            {
                list2.Add(value.Substring(startIndex));
            }
            return list2.ToArray();
        }
    }
}
