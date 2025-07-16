//--------------------------------------------------------------------------
// File:    ArgFilePathException.cs
// Content:	Implementation of class ArgFilePathException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    ///<summary>FilePath provided by Argument is not valid exception class</summary>
    public class ArgFilePathException : TechException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgFilePathException"/> class.
        /// </summary>
        public ArgFilePathException()
        {
            ErrorCode = 1006;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgFilePathException"/> class.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        public ArgFilePathException(object argValue, string argName)
            : base(StringResources.ErrorArgumentFilePathExceptionTemplate2Args.SafeFormatWith(argName, argValue))
        {
            ErrorCode = 1006;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgFilePathException"/> class.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="message">The message.</param>
        public ArgFilePathException(object argValue, string argName, string message)
            : base("{0} [Arg:{1},Value:{2}]".SafeFormatWith(message, argName, argValue))
        {
            ErrorCode = 1006;
        }

    }
}
