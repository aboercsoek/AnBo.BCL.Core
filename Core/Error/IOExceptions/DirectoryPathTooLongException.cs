//--------------------------------------------------------------------------
// File:    DirectoryPathTooLongException.cs
// Content:	Implementation of class DirectoryPathTooLongException
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// This exception is thrown when trying to access a file directory with a path that is too long.
	/// </summary>
    public class DirectoryPathTooLongException : TechException
    {
        private const int TypeErrorCode = 1021;
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public DirectoryPathTooLongException() : base(StringResources.ErrorDirectoryPathToLong) { ErrorCode = TypeErrorCode; }

        /// <summary>
        /// Creates a new instance of this class with the specified message
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        public DirectoryPathTooLongException(object argValue, string argName)
            : base(StringResources.ErrorArgDirectoryPathToLongTemplate2Args.SafeFormatWith(argName, argValue))
        {
            ErrorCode = TypeErrorCode;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified message
        /// </summary>
        /// <param name="message">The message used for the exception</param>
        public DirectoryPathTooLongException(string message) : base(message) { ErrorCode = TypeErrorCode; }
        /// <summary>
        /// Creates a new instance of this class with the specified message and inner exception
        /// </summary>
        /// <param name="message">The message used for the exception</param>
        /// <param name="inner">The inner exception that this instance wraps.</param>
        public DirectoryPathTooLongException(string message, Exception inner) : base(inner, message) { ErrorCode = TypeErrorCode; }
    }
}
