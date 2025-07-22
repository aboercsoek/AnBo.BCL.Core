//--------------------------------------------------------------------------
// File:    NumberFormatter.cs
// Content:	Implementation of class NumberFormatter
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

using System.Globalization;
using System.Numerics;

namespace AnBo.Core;

///<summary>Number to String format helper class.</summary>
public static class NumberFormatter
{
    #region ...ToBinaryString

    /// <summary>
    /// Convert an integer number to binary string.
    /// </summary>
    /// <typeparam name="T">The type of the number to convert. Must be a numeric type (byte, short, int, long, ushort, uint, ulong).</typeparam>
    /// <param name="value">The number to convert.</param>
    /// <returns>The binary string representation of the specified number.</returns>
    public static string ToBinaryString<T>(T value) where T : struct
    {
        return value switch
        {
            byte b => Convert.ToString(b, 2),
            short s => Convert.ToString(s, 2),
            int i => Convert.ToString(i, 2),
            long l => Convert.ToString(l, 2),
            ushort us => Convert.ToString(us, 2),
            uint ui => Convert.ToString(ui, 2),
            ulong ul => Convert.ToString((long)ul, 2),
            _ => throw new ArgumentException("Value must be a byte, short, int, long, ushort, uint, ulong type.", nameof(value))
        };
    }

    #endregion

    #region ...ToHexString

    /// <summary>
    /// Convert an integer value to hex string with format option.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="minHexDigits">The minimum length.</param>
    /// <param name="addZeroXPrefix">if set to <see langword="true"/> add 0x prefix.</param>
    /// <returns>The hex string representation of the specified number  with at least the given length (minHexDigits).</returns>
    public static string ToHexString<T>(T value, int minHexDigits = 1, bool addZeroXPrefix = false) where T : struct, IConvertible
    {
        return HexConverter.ToHexString(value, minHexDigits, addZeroXPrefix);
    }

    #endregion
}
