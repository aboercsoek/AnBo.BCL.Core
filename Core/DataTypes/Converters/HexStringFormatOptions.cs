//--------------------------------------------------------------------------
// File:    HexStringFormatOptions.cs
// Content:	Implementation of a hex converter options enum
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// ToHexString format options
/// </summary>
public enum HexStringFormatOptions
{
    /// <summary>
    /// No format options
    /// </summary>
    None,
    /// <summary>
    /// Add 0x as prefix to the hex string
    /// </summary>
    AddZeroXPrefix,
    /// <summary>
    /// Insert a separator between the hex byte strings (default is space).
    /// </summary>
    AddSeparatorBetweenHexBytes,
    /// <summary>
    /// Insert a Space between the hex byte strings and add a new line every 16 hex byte strings.
    /// </summary>
    AddNewLineAfter16HexBytes,
}
