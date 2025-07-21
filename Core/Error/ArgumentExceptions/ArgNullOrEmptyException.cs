//--------------------------------------------------------------------------
// File:    ArgNullOrEmptyException.cs
// Content:	Implementation of class ArgNullOrEmptyException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

///<summary>Argument is null or empty validation exception class</summary>
public class ArgNullOrEmptyException : TechException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullOrEmptyException"/> class.
    /// </summary>
    public ArgNullOrEmptyException()
    {
        ErrorCode = 1004;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullOrEmptyException"/> class.
    /// </summary>
    /// <param name="argValue">The argument value.</param>
    /// <param name="argName">The argument name.</param>
    public ArgNullOrEmptyException(object argValue, string argName)
        : base(StringResources.ErrorShouldNotBeNullOrEmptyValidationTemplate2Args.SafeFormatWith(argName, argValue))
    {
        ErrorCode = 1004;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullOrEmptyException"/> class.
    /// </summary>
    /// <param name="argValue">The argument value.</param>
    /// <param name="argName">The argument name.</param>
    /// <param name="message">The message.</param>
    public ArgNullOrEmptyException(object argValue, string argName, string message)
        : base(message.SafeFormatWith(argName, argValue))
    {
        ErrorCode = 1004;
    }

}
