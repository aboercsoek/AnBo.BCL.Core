//--------------------------------------------------------------------------
// File:    TypeOf.cs
// Content:	Implementation of class TypeOf
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives
#endregion

namespace AnBo.Core;

///<summary>Common Type provider class</summary>
public class TypeOf
{
    /// <summary>
    /// Bool Type
    /// </summary>
    public static readonly Type Boolean = typeof(bool);
    /// <summary>
    /// Int32 (int) Type
    /// </summary>
    public static readonly Type Int32 = typeof(int);
    /// <summary>
    /// Int64 (long) Type
    /// </summary>
    public static readonly Type Int64 = typeof(long);
    /// <summary>
    /// Object Type
    /// </summary>
    public static readonly Type Object = typeof(object);
    /// <summary>
    /// String Type
    /// </summary>
    public static readonly Type String = typeof(string);

}
