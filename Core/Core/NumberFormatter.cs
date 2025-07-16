//--------------------------------------------------------------------------
// File:    NumberFormatter.cs
// Content:	Implementation of class NumberFormatter
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    ///<summary>Number to String format helper class.</summary>
	public static class NumberFormatter
    {
        #region ...ToBinaryString

        /// <summary>
        /// Convert Byte to binary string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string ByteToBinaryString(byte value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// Convert Int16 to binary string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string Int16ToBinaryString(Int16 value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// Convert UInt16 to binary string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string UInt16ToBinaryString(UInt16 value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// Convert Int32 to binary string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string Int32ToBinaryString(Int32 value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// Convert UInt32 to binary string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string UInt32ToBinaryString(UInt32 value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// Convert Int64 to binary string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string Int64ToBinaryString(Int64 value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// Convert UInt64 to binary string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The binary string representation of the specified number.</returns>
        public static string UInt64ToBinaryString(UInt64 value)
        {
            return Convert.ToString((Int64)value, 2);
        }

        #endregion

        #region Int16ToHexString

        /// <summary>
        /// Convert Int16 value to hex string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string Int16ToHexString(Int16 value)
        {
            return HexConverter.ToHexString(value, 1);
        }

        /// <summary>
        /// Convert Int16 value to hex string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="minHexDigits">The minimum length. Range[1..8].</param>
        /// <returns>The hex string representation of the specified number  with at least the given length (minHexDigits).</returns>
        public static string Int16ToHexString(Int16 value, int minHexDigits)
        {
            return HexConverter.ToHexString(value, minHexDigits);
        }

        /// <summary>
        /// Convert Int16 value to hex string with format option.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string Int16ToHexString(Int16 value, bool addZeroXPrefix)
        {
            string result = addZeroXPrefix ? "0x" : "";
            return result + HexConverter.ToHexString(value, 1);
        }

        /// <summary>
        /// Convert Int16 value to hex string with format option.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="minHexDigits">The minimum length. Range[1..8].</param>
        /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
        /// <returns>The hex string representation of the specified number  with at least the given length (minHexDigits).</returns>
        public static string Int16ToHexString(Int16 value, int minHexDigits, bool addZeroXPrefix)
        {
            string result = addZeroXPrefix ? "0x" : "";
            return result + HexConverter.ToHexString(value, minHexDigits);
        }

        #endregion

        #region Int32ToHexString

        /// <summary>
        /// Convert Int32 value to hex string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string Int32ToHexString(int value)
        {
            return HexConverter.ToHexString(value, 1);
        }

        /// <summary>
        /// Convert Int32 value to hex string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="minHexDigits">The minimum length. Range[1..8].</param>
        /// <returns>The hex string representation of the specified number  with at least the given length (minHexDigits).</returns>
        public static string Int32ToHexString(int value, int minHexDigits)
        {
            return HexConverter.ToHexString(value, minHexDigits);
        }

        /// <summary>
        /// Convert Int32 value to hex string with format option.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string Int32ToHexString(int value, bool addZeroXPrefix)
        {
            string result = addZeroXPrefix ? "0x" : "";
            return result + HexConverter.ToHexString(value, 1);
        }

        /// <summary>
        /// Convert Int32 value to hex string with format option.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="minHexDigits">The minimum length. Range[1..8].</param>
        /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
        /// <returns>The hex string representation of the specified number  with at least the given length (minHexDigits).</returns>
        public static string Int32ToHexString(int value, int minHexDigits, bool addZeroXPrefix)
        {
            string result = addZeroXPrefix ? "0x" : "";
            return result + HexConverter.ToHexString(value, minHexDigits);
        }

        #endregion

        #region Int64ToHexString

        /// <summary>
        /// Convert Int64 value to hex string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string Int64ToHexString(long value)
        {
            return HexConverter.ToHexString(value, 1);
        }

        /// <summary>
        /// Convert Int64 value to hex string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="minHexDigits">The minimum length. Range[1..16].</param>
        /// <returns>The hex string representation of the specified number  with at least the given length (minHexDigits).</returns>
        public static string Int64ToHexString(long value, int minHexDigits)
        {
            return HexConverter.ToHexString(value, minHexDigits);
        }

        /// <summary>
        /// Convert Int64 value to hex string with format option.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
        /// <returns>The hex string representation of the specified number.</returns>
        public static string Int64ToHexString(long value, bool addZeroXPrefix)
        {
            string result = addZeroXPrefix ? "0x" : "";
            return result + HexConverter.ToHexString(value, 1);
        }

        /// <summary>
        /// Convert Int64 value to hex string with format option.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="minHexDigits">The minimum length. Range[1..16].</param>
        /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
        /// <returns>The hex string representation of the specified number  with at least the given length (minHexDigits).</returns>
        public static string Int64ToHexString(long value, int minHexDigits, bool addZeroXPrefix)
        {
            string result = addZeroXPrefix ? "0x" : "";
            return result + HexConverter.ToHexString(value, minHexDigits);
        }

        #endregion
    }
}
