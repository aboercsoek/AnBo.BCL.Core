//--------------------------------------------------------------------------
// File:    StringExtensions.To.cs
// Content:	Implementation of class StringExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents modern extension methods for <see cref="String"/> type optimized for .NET 8+.
/// </summary>
public static partial class StringExtensions
{
    #region String to type conversion extensions

    /// <summary>
    /// Converts a string to a byte array using UTF-8 encoding.
    /// </summary>
    /// <param name="str">The source string.</param>
    /// <returns>The UTF-8 encoded byte array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this string? str)
    {
        return StringHelper.GetBytesFromString(str, Encoding.UTF8);
    }

    /// <summary>
    /// Converts a string path to a FileInfo object if the file exists.
    /// </summary>
    /// <param name="value">The file path string.</param>
    /// <returns>A FileInfo object if the file exists; otherwise, null.</returns>
    public static FileInfo? ToFileInfo(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;

        try
        {
            var fileInfo = new FileInfo(value);
            return fileInfo.Exists ? fileInfo : null;
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (PathTooLongException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    #endregion
}
