//--------------------------------------------------------------------------
// File:    ArgEmptyException.cs
// Content:	Implementation of class ArgEmptyException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    ///<summary>Argument is empty validation exception class.</summary>
    public class ArgEmptyException : TechException
    {
        public string ParamName { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgEmptyException"/> class.
        /// </summary>
        public ArgEmptyException()
        {
            ErrorCode = 1003;
            ParamName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgEmptyException"/> class.
        /// </summary>
        /// <param name="argName">The argument name.</param>
        public ArgEmptyException(string argName)
            : base(StringResources.ErrorArgumentNotEmptyValidationTemplate1Arg.SafeFormatWith(argName))
        {
            ErrorCode = 1003;
            ParamName = argName ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgEmptyException"/> class.
        /// </summary>
        /// <param name="argName">The argument name.</param>
        /// <param name="message">The message.</param>
        public ArgEmptyException(string argName, string message)
            : base(message.IsFormatString() ? message.SafeFormatWith(argName) : message)
        {
            ErrorCode = 1003;
            ParamName = argName ?? string.Empty;
        }
    }
}
