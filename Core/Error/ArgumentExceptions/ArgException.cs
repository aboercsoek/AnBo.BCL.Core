//--------------------------------------------------------------------------
// File:    ArgException.cs
// Content:	Implementation of class ArgException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    ///<summary>General argument validation failed exception.</summary>
	/// <typeparam name="TValue">The Argument type.</typeparam>
    public class ArgException<TValue> : TechException
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgException{TValue}"/> class.
        /// </summary>
        /// <param name="argName">The argument name.</param>
        public ArgException(string argName)
            : base(StringResources.ErrorArgumentTemplate1Arg.SafeFormatWith(argName))
        {
            ErrorCode = 1001;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgException{TValue}"/> class.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        public ArgException(TValue argValue, string argName)
            : base(StringResources.ErrorArgumentValidationFailedTemplate2Args.SafeFormatWith(argName, argValue))
        {
            ErrorCode = 1001;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgException{TValue}"/> class.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="message">The error message.</param>
        public ArgException(TValue argValue, string argName, string message)
            : base(message.IsFormatString() ? message.SafeFormatWith(argName, argValue) : message)
        {
            ErrorCode = 1001;
        }

    }
}
