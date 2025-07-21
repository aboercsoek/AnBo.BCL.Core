//--------------------------------------------------------------------------
// File:    InvalidTypeCastException.cs
// Content:	Implementation of class InvalidTypeCastException
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
using System.Xml.Serialization;

#endregion

namespace AnBo.Core;

/// <summary>
/// This exception is thrown when error occurs during type casting.
/// </summary>
public class InvalidTypeCastException : TechException
{
    private const int TypeErrorCode = 1018;
    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    public InvalidTypeCastException() { ErrorCode = TypeErrorCode; }
    /// <summary>
    /// Creates a new instance of this class with the specified message
    /// </summary>
    /// <param name="message">The message used for the exception</param>
    public InvalidTypeCastException(string message) : base(message) { ErrorCode = TypeErrorCode; }
    /// <summary>
    /// Creates a new instance of this class with the specified message and inner exception
    /// </summary>
    /// <param name="message">The message used for the exception</param>
    /// <param name="inner">The inner exception that this instance wraps.</param>
    public InvalidTypeCastException(string message, Exception inner) : base(inner, message) { ErrorCode = TypeErrorCode; }

}
