//--------------------------------------------------------------------------
// File:    BooleanBoxes.cs
// Content:	Implementation of class BooleanBoxes
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives
using System;
using System.Linq;
#endregion

namespace AnBo.Core
{
    ///<summary>BooleanBoxes optimizes performance for boxing boolean values, by holding two boxed intances for <c>true</c> and <c>false</c> values.</summary>
    public static class BooleanBoxes
    {
        /// <summary>Boxed boolean instance, for <c>false</c> value.</summary>
        public static readonly object FalseBox = false;
        /// <summary>Boxed boolean instance, for <c>true</c> value.</summary>
        public static readonly object TrueBox = true;

        /// <summary>
        /// Performance optimized boxing of bool values.
        /// </summary>
        /// <param name="value">boolen value to box.</param>
        /// <returns>Returns <see cref="TrueBox"/> if <paramref name="value"/> is <c>true</c>, 
        /// or <see cref="FalseBox"/> if <paramref name="value"/> is <c>false</c>.</returns>
        public static object Box(bool value)
        {
            return value ? TrueBox : FalseBox;
        }
    }

}