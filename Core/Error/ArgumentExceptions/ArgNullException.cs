//--------------------------------------------------------------------------
// File:    ArgNullException.cs
// Content:	Implementation of class ArgNullException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AnBo.Core;

///<summary>Argument is null validation exception class.</summary>
public class ArgNullException : TechException
{
    public string ParamName { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullException"/> class.
    /// </summary>
    public ArgNullException()
    {
        ErrorCode = 1002;
        ParamName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullException"/> class.
    /// </summary>
    /// <param name="argName">The argument name.</param>
    public ArgNullException(string? argName)
        : base(StringResources.ErrorShouldNotBeNullValidationTemplate1Arg.SafeFormatWith(argName))
    {
        ErrorCode = 1002;
        ParamName = argName ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullException"/> class.
    /// </summary>
    /// <param name="argName">The argument name.</param>
    /// <param name="message">The message.</param>
    public ArgNullException(string argName, string message)
        : base(message.IsFormatString() ? message.SafeFormatWith(argName) : message)
    {
        ErrorCode = 1002;
        ParamName = argName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="formatParameters">The format parameters.</param>
    public ArgNullException(string message, params object[] formatParameters)
        : base((formatParameters != null && formatParameters.Length > 0 && message.IsFormatString()) ? message.SafeFormatWith(formatParameters) : message)
    {
        ErrorCode = 1002;
        ParamName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgNullException"/> class.
    /// </summary>
    /// <param name="inner">The inner.</param>
    /// <param name="message">The message.</param>
    /// <param name="formatParameters">The format parameters.</param>
    public ArgNullException(Exception inner, string message, params object[] formatParameters)
        : base(
            formatParameters != null && formatParameters.Length > 0 ? message.SafeFormatWith(formatParameters) : message, inner)
    {
        ErrorCode = 1002;
        ParamName = string.Empty;
    }

    /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            Throw(paramName);
        }
    }

    [DoesNotReturn]
    internal static void Throw(string? paramName) =>
       throw new ArgNullException(paramName);
}
