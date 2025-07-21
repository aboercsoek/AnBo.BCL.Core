//--------------------------------------------------------------------------
// File:    FilePathTooLongException.cs
// Content:	Implementation of class FilePathTooLongException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// This exception is thrown when trying to access a file with a path that is too long.
/// </summary>
public class FilePathTooLongException : TechException
{
    private const int TypeErrorCode = 1022;
    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    public FilePathTooLongException() : base(StringResources.ErrorFilePathToLong) { ErrorCode = TypeErrorCode; }

    /// <summary>
    /// Creates a new instance of this class with the specified message
    /// </summary>
    /// <param name="argValue">The argument value.</param>
    /// <param name="argName">The argument name.</param>
    public FilePathTooLongException(object argValue, string argName)
        : base(StringResources.ErrorArgFilePathToLongTemplate2Args.SafeFormatWith(argName, argValue))
    {
        ErrorCode = TypeErrorCode;
    }

    /// <summary>
    /// Creates a new instance of this class with the specified message
    /// </summary>
    /// <param name="message">The message used for the exception</param>
    public FilePathTooLongException(string message) : base(message) { ErrorCode = TypeErrorCode; }
    /// <summary>
    /// Creates a new instance of this class with the specified message and inner exception
    /// </summary>
    /// <param name="message">The message used for the exception</param>
    /// <param name="inner">The inner exception that this instance wraps.</param>
    public FilePathTooLongException(string message, Exception inner) : base(inner, message) { ErrorCode = TypeErrorCode; }
}
