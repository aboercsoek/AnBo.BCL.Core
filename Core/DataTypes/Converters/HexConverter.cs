//--------------------------------------------------------------------------
// File:    HexConverter.cs
// Content:	Implementation of a hex converter class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Globalization;
using System.Text;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Hex-string and -digit converter
	/// </summary>
	public static class HexConverter
    {
        #region HexString- & HexDigit-Methods

        /// <summary>
        /// Converts a hex digit.
        /// </summary>
        /// <exception cref="ArgException{TValue}">Is thrown if <paramref name="val"/> has imcompatible digits for hex convertian.</exception>
        /// <param name="val">The value.</param>
        /// <returns>The converted hex digit.</returns>
        public static int ConvertHexDigit(char val)
        {
            if ((val <= '9') && (val >= '0'))
            {
                return (val - '0');
            }
            if ((val >= 'a') && (val <= 'f'))
            {
                return ((val - 'a') + 0xa);
            }
            if ((val < 'A') || (val > 'F'))
            {
                throw new ArgException<char>(val, "val", "Value was out of range. Must be between '0'-'9' or 'a'-'f' or 'A'-'F'.");
            }
            return ((val - 'A') + 0xa);
        }

        /// <summary>
        /// Convert hex string to byte array.
        /// </summary>
        /// <param name="hexString">The hex string.</param>
        /// <returns>The converted byte buffer.</returns>
        /// <exception cref="ArgException{TValue}">Is thrown if <paramref name="hexString"/> is not properly formatted.</exception>"
        public static byte[] FromHexString(string hexString)
        {
            try
            {
                if (hexString.IsNullOrEmptyWithTrim())
                {
                    return [];
                }

                hexString = hexString.Replace(" ", "");

                // Remove 0x prefix if present
                if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    hexString = hexString.Substring(2);
                }

                // Convert hex string to byte array
                return Convert.FromHexString(hexString);
            }
            catch (FormatException)
            {
                throw new ArgException<string>(hexString, "hexString", "Inproperly formatted hex string");
            }

        }

        /// <summary>
        /// Returns a hexadecimal representation of an long with at least the given length.
        /// </summary>
        /// <typeparam name="T">Source type, must be byte, short, int, long, ushort or uint.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="minHexDigits">The minimum length.</param>
        /// <param name="addZeroXPrefix">Whether to add a "0x" prefix.</param>
        /// <returns>
        /// A hexadecimal representation of an long with at least the given length (hexDigits).
        /// </returns>
        public static string ToHexString<T>(T value, int minHexDigits=1, bool addZeroXPrefix = false) where T : struct //, IConvertible
        {
            if (minHexDigits < 1)
                minHexDigits = 1;

            string prefix = addZeroXPrefix ? "0x" : string.Empty;

            int maxDigits;
            switch (value)
            {
                case short s:
                    maxDigits = 4;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + Convert.ToString(s, 16).PadLeft(minHexDigits, '0');
                case int i:
                    maxDigits = 8;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + Convert.ToString(i, 16).PadLeft(minHexDigits, '0');
                case long l:
                    maxDigits = 16;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + Convert.ToString(l, 16).PadLeft(minHexDigits, '0');
                case Int128 i128:
                    maxDigits = 32;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + i128.ToString("x").PadLeft(minHexDigits, '0');
                case byte b:
                    maxDigits = 2;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + Convert.ToString(b, 16).PadLeft(minHexDigits, '0');
                case ushort us:
                    maxDigits = 4;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + Convert.ToString(us, 16).PadLeft(minHexDigits, '0');
                case uint ui:
                    maxDigits = 8;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + Convert.ToString(ui, 16).PadLeft(minHexDigits, '0');
                case ulong ul:
                    maxDigits = 16;
                    Int128 tempI128 = ul;
                    minHexDigits = Math.Min(minHexDigits, maxDigits);
                    return prefix + tempI128.ToString("x").PadLeft(minHexDigits, '0');
                default:
                    throw new ArgException<T>(value, "value", "Value must be a byte, short, int, long, ushort ,uint, ulong, Int128 type.");
            }
        }

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="buffer">The byte buffer to convert.</param>
        /// <returns>The hex string.</returns>
        public static string ToHexString(byte[] buffer)
        {
            return ToHexString(buffer, HexStringFormatOptions.None);
        }

        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="buffer">The byte buffer to convert.</param>
        /// <param name="options">The hex string format options.</param>
        /// <returns>The hex string.</returns>
        public static string ToHexString(byte[] buffer, HexStringFormatOptions options)
        {
            return ToHexString(buffer, options, ' ');
        }


        /// <summary>
        /// Convert byte array to hex string.
        /// </summary>
        /// <param name="buffer">The byte buffer to convert.</param>
        /// <param name="options">The hex string format options.</param>
        /// <param name="separator">Hex byte string separator</param>
        /// <returns>The hex string.</returns>
        public static string ToHexString(byte[] buffer, HexStringFormatOptions options, char separator)
        {
            if (buffer == null)
                return string.Empty;
            if (buffer.Length == 0)
                return string.Empty;

            int capacity;
            if ((options == HexStringFormatOptions.AddSeparatorBetweenHexBytes) || (options == HexStringFormatOptions.AddNewLineAfter16HexBytes))
                capacity = buffer.Length * 0x3;
            else
                capacity = buffer.Length * 0x2;

            var sb = new StringBuilder(capacity);
            int bufferIndex = 0x0;

            while (bufferIndex < buffer.Length)
            {
                int num = (buffer[bufferIndex] & 0xf0) >> 0x4;
                sb.Append(HexDigit(num));
                num = buffer[bufferIndex] & 0xf;
                sb.Append(HexDigit(num));
                bufferIndex++;

                if (bufferIndex < buffer.Length)
                {
                    if (options == HexStringFormatOptions.AddSeparatorBetweenHexBytes)
                        sb.Append(separator);
                    else if (options == HexStringFormatOptions.AddNewLineAfter16HexBytes)
                    {
                        if (bufferIndex % 16 == 0)
                            sb.AppendLine();
                        else
                            sb.Append(separator);
                    }
                }
            }

            return (options == HexStringFormatOptions.AddZeroXPrefix) ? "0x" + sb : sb.ToString();
        }

        private static char HexDigit(int num)
        {
            return ((num < 0xa) ? ((char)(num + 0x30)) : ((char)(num + 0x37)));
        }

        #endregion
    }
}
