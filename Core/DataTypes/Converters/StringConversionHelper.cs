//--------------------------------------------------------------------------
// File:    StringConversionHelper.cs
// Content:	Implementation of a string conversion helper class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Common conversion tasks such as parsing string values into various types.
	/// </summary>
	public static class StringConversionHelper
    {
        #region Is...-Methods

        /// <summary>
        /// Determines if the specified string contains a Int16 value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a Int16 value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsInt16String(string str)
        {
            Int16 trash;
            return Int16.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a Int32 value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a Int32 value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsInt32String(string str)
        {
            Int32 trash;
            return Int32.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a Int64 value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a Int64 value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsInt64String(string str)
        {
            Int64 trash;
            return Int64.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a double value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a double value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsDoubleString(string str)
        {
            double trash;
            return double.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a decimal value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a decimal value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsDecimalString(string str)
        {
            decimal trash;
            return decimal.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a float value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a float value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFloatString(string str)
        {
            float trash;
            return float.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a bool value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a bool value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsBooleanString(string str)
        {
            bool trash;
            return bool.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a byte value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a byte value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsByteString(string str)
        {
            byte trash;
            return byte.TryParse(str, out trash);
        }

        /// <summary>
        /// Determines if the specified string contains a TimeSpan value.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>
        /// 	<see langword="true"/> if the string contains a TimeSpan value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsTimeSpanString(string str)
        {
            TimeSpan trash;
            return DateTimeHelper.TryParseTimeSpan(str, out trash);
        }


        #endregion

        #region ToString-Methods

        /// <summary>
        /// Int32 to binary string format.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string ToBinaryString(int number)
        {
            return Convert.ToString(number, 2);
        }

        /// <summary>
        /// Int32 to hex string format.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string ToHexString(int number)
        {
            return ToHexString(number, false);
        }

        /// <summary>
        /// Int32 to hex string format.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string ToHexString(int number, bool addZeroXPrefix)
        {
            string result = addZeroXPrefix ? "0x" : "";
            result += Convert.ToString(number, 16);
            return result;
        }

        /// <summary>
        /// Converts the value into a string by using a TypeConverter for type of val, if available.
        /// </summary>
        /// <param name="val">Value to convert into an string</param>
        /// <returns>Converted object string.</returns>
        public static string ToInvariantString(this object? val)
        {
            return ToInvariantString(val, "<null>");
        }

        /// <summary>
        /// Converts the value into a string by using a TypeConverter for type of val, if available.
        /// </summary>
        /// <param name="val">Value to convert into an string</param>
        /// <param name="nullString">null value string result.</param>
        /// <returns>Converted object string.</returns>
        public static string ToInvariantString(this object? val, string nullString)
        {
            if (val == null)
                return nullString.SafeString();

            if (val is string s)
            {
                return s;
            }

            try
            {
                TypeConverter c = TypeDescriptor.GetConverter(val);

                string? result = null;
                if (c != null)
                    result = c.ConvertToInvariantString(val);

                return result ?? val.ToString() ?? nullString.SafeString();
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;
            }

            return nullString.SafeString();
        }

        /// <summary>
        /// Converts the value to string using a TypeConverter for Type T if available.
        /// </summary>
        //public static string? ToString<T>(T val)
        //{
        //    if (val is string s)
        //    {
        //        return string.IsNullOrEmpty(s) ? "<null>" : s;
        //    }
        //    try
        //    {
        //        TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
        //        string? str = c.ConvertToInvariantString(val);
        //        //return string.IsNullOrEmpty(str) ? val.ToString() : str;
        //        return str ?? val?.ToString() ?? string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.IsFatal())
        //            throw;
        //    }
        //    // If no TypeConverter is available, just return the ToString() result
        //    return val.ToString();

        //}

        /// <summary>
        /// Converts the value to string using a TypeConverter for Type T if available.
        /// </summary>
        public static string ToString<T>(T? val)
        {
            if (val is null)
                return "<null>";

            if (val is string s)
                return string.IsNullOrEmpty(s) ? "<null>" : s;

            // Für häufige Typen optimierte Pfade
            return val switch
            {
                int i => i.ToString(CultureInfo.InvariantCulture),
                long l => l.ToString(CultureInfo.InvariantCulture),
                double d => d.ToString(CultureInfo.InvariantCulture),
                float f => f.ToString(CultureInfo.InvariantCulture),
                bool b => b.ToString(CultureInfo.InvariantCulture),
                DateTime dt => dt.ToString(CultureInfo.InvariantCulture),
                Guid g => g.ToString(),
                _ => TryConvertWithTypeConverter(val) ?? val.ToString() ?? string.Empty
            };
        }

        // Hilfsmethode für primitive Typen
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryConvertPrimitive<T>(T val, [NotNullWhen(true)] out string? result)
        {
            result = val switch
            {
                int i => i.ToString(CultureInfo.InvariantCulture),
                long l => l.ToString(CultureInfo.InvariantCulture),
                double d => d.ToString(CultureInfo.InvariantCulture),
                float f => f.ToString(CultureInfo.InvariantCulture),
                bool b => b.ToString(CultureInfo.InvariantCulture),
                DateTime dt => dt.ToString(CultureInfo.InvariantCulture),
                Guid g => g.ToString(),
                _ => null
            };

            return result is not null;
        }

        // Place TypeConverter logic in a pivate method
        private static string? TryConvertWithTypeConverter<T>(T val)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.CanConvertTo(typeof(string)))
                {
                    return converter.ConvertToInvariantString(val);
                }
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                // Exception wird geschluckt, Fallback wird verwendet
            }

            return null;
        }

        /// <summary>
        /// Modern string conversion using ISpanFormattable for .NET 8 and later.
        /// </summary>
        /// <param name="val">The value to convert.</param>
        /// <returns>A string representation of the value.</returns>
        public static string ToStringSpan<T>(T? val) where T : ISpanFormattable
        {
            if (val is null)
                return "<null>";

            if (val is string s)
                return string.IsNullOrEmpty(s) ? "<null>" : s;

            // Für ISpanFormattable Typen optimierte Konvertierung
            Span<char> buffer = stackalloc char[256];
            if (val.TryFormat(buffer, out int charsWritten, ReadOnlySpan<char>.Empty, CultureInfo.InvariantCulture))
            {
                return buffer[..charsWritten].ToString();
            }

            return val.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Wraps the <see cref="T:System.ComponentModel.TypeConverter" />.
        /// </summary>
        /// <typeparam name="T">The parsing result type.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsing result of Type T.</returns>
        public static T? ParseInvariantString<T>(this string value)
        {
            ArgChecker.ShouldNotBeNull(value);

            return (T?)ParseInvariantString(value, typeof(T));
        }


        /// <summary>
        /// Wraps the <see cref="T:System.ComponentModel.TypeConverter" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The parsed object result.</returns>
        public static object? ParseInvariantString(this string value, Type type)
        {
            ArgChecker.ShouldNotBeNull(value);
            ArgChecker.ShouldNotBeNull(type);

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter?.ConvertFromInvariantString(value);
        }

        #endregion
    }
}
