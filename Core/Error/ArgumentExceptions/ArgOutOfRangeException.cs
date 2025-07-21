//--------------------------------------------------------------------------
// File:    ArgOutOfRangeException.cs
// Content:	Implementation of class ArgOutOfRangeException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

///<summary>Argument out of range validation exception class.</summary>
	/// <typeparam name="TValue">The Argument type.</typeparam>
public class ArgOutOfRangeException<TValue> : TechException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgOutOfRangeException{TValue}"/> class.
    /// </summary>
    public ArgOutOfRangeException()
    {
        ErrorCode = 1005;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgOutOfRangeException{TValue}"/> class.
    /// </summary>
    /// <param name="argValue">The argument value.</param>
    /// <param name="argName">The argument name.</param>
    public ArgOutOfRangeException(TValue argValue, string argName)
        : base(StringResources.ErrorArgumentOutOfRangeValidationTemplate4Args.SafeFormatWith(argName, argValue))
    {
        ErrorCode = 1005;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgOutOfRangeException{TValue}"/> class.
    /// </summary>
    /// <param name="argValue">The argument value.</param>
    /// <param name="argName">The argument name.</param>
    /// <param name="validMinValue">Valid min value.</param>
    /// <param name="validMaxValue">Valid max value.</param>
    public ArgOutOfRangeException(TValue argValue, string argName, TValue validMinValue, TValue validMaxValue)
        : base(StringResources.ErrorArgumentOutOfRangeValidationWithRangeTemplate4Args.SafeFormatWith(argName, argValue, validMinValue, validMaxValue))
    {
        ErrorCode = 1005;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgOutOfRangeException{TValue}"/> class.
    /// </summary>
    /// <param name="argValue">The argument value.</param>
    /// <param name="argName">The argument name.</param>
    /// <param name="message">The message template.</param>
    public ArgOutOfRangeException(TValue argValue, string argName, string message)
        : base(StringHelper.SafeFormat(message, argName, argValue))
    {
        ErrorCode = 1005;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TechException"/> class.
    /// </summary>
    /// <param name="argValue">The argument value.</param>
    /// <param name="argName">The argument name.</param>
    /// <param name="inner">The inner exception.</param>
    /// <param name="message">The message template.</param>
    public ArgOutOfRangeException(TValue argValue, string argName, Exception inner, string message)
        : base(StringHelper.SafeFormat(message, argName, argValue, inner))
    {
        ErrorCode = 1005;
    }
}
