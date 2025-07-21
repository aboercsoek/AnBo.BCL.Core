//--------------------------------------------------------------------------
// File:    BoolExtensions.cs
// Content:	Implementation of class BoolExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion


namespace AnBo.Core;

///<summary><see cref="bool"/> extension methods.</summary>
public static class BoolExtensions
{
    /// <summary>
    /// Determines whether the specified bool value is false.
    /// </summary>
    /// <param name="boolValue">The bool value.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified bool value is false; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsFalse(this bool boolValue)
    {
        return (boolValue == false);
    }

    /// <summary>
    /// Determines whether the specified bool value is true.
    /// </summary>
    /// <param name="boolValue">The bool value.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified bool value is <see langword="true"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsTrue(this bool boolValue)
    {
        return boolValue;
    }
}
