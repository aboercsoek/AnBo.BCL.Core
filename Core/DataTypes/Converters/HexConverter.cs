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
        private static readonly string[] CachedInt32Strings;

        static HexConverter()
        {
            var strArray = new string[0x100];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = i.ToString(CultureInfo.InvariantCulture);
            }
            CachedInt32Strings = strArray;

        }

        #region HexString- & HexDigit-Methods

        /// <summary>
        /// Turns an integer into a string; independent of current culture, and more efficient (may cache strings)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string Int32ToString(int value)
        {
            return (value < CachedInt32Strings.Length) ? CachedInt32Strings[value] : value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a hex digit.
        /// </summary>
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
                throw new ArgumentException("Index was out of range. Must be between '0'-'9' or 'A'-'F'.");
            }
            return ((val - 'A') + 0xa);
        }

        /// <summary>
        /// Convert hex string to byte array.
        /// </summary>
        /// <param name="hexString">The hex string.</param>
        /// <returns>The converted byte buffer.</returns>
        public static byte[] FromHexString(string hexString)
        {
            byte[] buffer;

            if (hexString.IsNullOrEmptyWithTrim())
            {
                return new byte[0];
            }

            bool flag = false;
            int currentHexStringIndex = 0x0;
            string normalizedHexString = hexString.TrimStart();
            int length = normalizedHexString.Length;

            if (((length >= 2) && (normalizedHexString[0] == '0')) && ((normalizedHexString[1] == 'x') || (normalizedHexString[1] == 'X')))
            {
                length = normalizedHexString.Length - 2;
                currentHexStringIndex = 2;
            }

            if (length == 0)
                return new byte[0];

            if (((length % 2) != 0) && ((length % 3) != 2))
            {
                throw new ArgException<string>(hexString, "hexString", "Inproperly formatted hex string");
            }

            if ((length >= 0x3) && (normalizedHexString[currentHexStringIndex + 0x2] == ' '))
            {
                flag = true;
                buffer = new byte[(length / 0x3) + 0x1];
            }
            else
            {
                buffer = new byte[length / 0x2];
            }

            for (int i = 0x0; currentHexStringIndex < normalizedHexString.Length; i++)
            {
                int num4 = ConvertHexDigit(normalizedHexString[currentHexStringIndex]);
                int num3 = ConvertHexDigit(normalizedHexString[currentHexStringIndex + 0x1]);
                buffer[i] = (byte)(num3 | (num4 << 0x4));
                if (flag)
                {
                    currentHexStringIndex++;
                }
                currentHexStringIndex += 0x2;
            }
            return buffer;
        }

        private static char GetHexValue(int i)
        {
            if (i < 10)
            {
                return (char)(0x30 + i);
            }
            return (char)(0x61 + (i - 10));
        }

        /// <summary>
        /// Returns a hexadecimal representation of an Int16 with at least the given length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="hexDigits">The minimum length. Range[1..4].</param>
        /// <returns>
        /// A hexadecimal representation of an Int16 value with at least the given length (hexDigits).
        /// </returns>
        public static string ToHexString(Int16 value, int hexDigits)
        {
            const int maxDigits = 4;
            var charArray = new char[maxDigits];

            if (hexDigits < 1)
                hexDigits = 1;

            if (hexDigits > maxDigits)
                hexDigits = maxDigits;

            int index = maxDigits - 1;
            int endIndex = maxDigits - hexDigits;

            do
            {
                int i = value & 0x0F;
                charArray[index] = GetHexValue(i);
                if ((i != 0) && (index < endIndex))
                {
                    endIndex = index;
                }
                index--;
                value = (Int16)(value >> 4);
            } while (index >= 0);

            return new string(charArray, 0, maxDigits - (endIndex / 2));
        }

        /// <summary>
        /// Returns a hexadecimal representation of an integer with at least the given length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="hexDigits">The minimum length. Range[1..8].</param>
        /// <returns>
        /// a hexadecimal representation of an integer with at least the given length (hexDigits).
        /// </returns>
        public static string ToHexString(int value, int hexDigits)
        {
            const int maxDigits = 8;
            var charArray = new char[maxDigits];

            if (hexDigits < 1)
                hexDigits = 1;

            if (hexDigits > maxDigits)
                hexDigits = maxDigits;

            int index = maxDigits - 1;
            int endIndex = maxDigits - hexDigits;

            do
            {
                int i = value & 0x0F;
                charArray[index] = GetHexValue(i);
                if ((i != 0) && (index < endIndex))
                {
                    endIndex = index;
                }
                index--;
                value = value >> 4;
            } while (index >= 0);

            return new string(charArray, 0, maxDigits - (endIndex / 2));
        }

        /// <summary>
        /// Returns a hexadecimal representation of an long with at least the given length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="hexDigits">The minimum length. Range[1..16].</param>
        /// <returns>
        /// A hexadecimal representation of an long with at least the given length (hexDigits).
        /// </returns>
        public static string ToHexString(long value, int hexDigits)
        {
            const int maxDigits = 16;
            var charArray = new char[maxDigits];

            if (hexDigits < 1)
                hexDigits = 1;

            if (hexDigits > maxDigits)
                hexDigits = maxDigits;

            int index = maxDigits - 1;
            int endIndex = maxDigits - hexDigits;

            do
            {
                var i = (int)(value & 0x0F);
                charArray[index] = GetHexValue(i);
                if ((i != 0) && (index < endIndex))
                {
                    endIndex = index;
                }
                index--;
                value = value >> 4;
            } while (index >= 0);

            return new string(charArray, 0, maxDigits - (endIndex / 2));
        }

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="buffer">The byte buffer to convert.</param>
        /// <returns>The hex string.</returns>
        public static string ToOctetString(byte[] buffer)
        {
            if (buffer == null)
                return string.Empty;
            if (buffer.Length == 0)
                return string.Empty;

            int capacity = buffer.Length * 0x2;

            var sb = new StringBuilder(capacity);
            int bufferIndex = 0x0;

            while (bufferIndex < buffer.Length)
            {
                int num = (buffer[bufferIndex] & 0xf0) >> 0x4;
                sb.Append(HexDigit(num));
                num = buffer[bufferIndex] & 0xf;
                sb.Append(HexDigit(num));
                bufferIndex++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="buffer">The byte buffer to convert.</param>
        /// <returns>The hex string.</returns>
        public static string ToHexString(byte[] buffer)
        {
            if (buffer == null)
                return string.Empty;
            if (buffer.Length == 0)
                return string.Empty;

            int capacity = buffer.Length * 0x2;

            var sb = new StringBuilder(capacity);
            int bufferIndex = 0x0;

            while (bufferIndex < buffer.Length)
            {
                int num = (buffer[bufferIndex] & 0xf0) >> 0x4;
                sb.Append(HexDigit(num));
                num = buffer[bufferIndex] & 0xf;
                sb.Append(HexDigit(num));
                bufferIndex++;
            }

            return sb.ToString();
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
