//--------------------------------------------------------------------------
// File:    ArgFilePathException.cs
// Content:	Implementation of class ArgFilePathException
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    ///<summary>FilePath provided by Argument is not valid exception class</summary>
    public class ArgDirectoryPathException : TechException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgDirectoryPathException"/> class.
        /// </summary>
        public ArgDirectoryPathException()
        {
            ErrorCode = 1007;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgDirectoryPathException"/> class.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        public ArgDirectoryPathException(object argValue, string argName)
            : base(StringResources.ErrorArgumentDirectoryPathExceptionTemplate2Args.SafeFormatWith(argName, argValue))
        {
            ErrorCode = 1007;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgDirectoryPathException"/> class.
        /// </summary>
        /// <param name="argValue">The argument value.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="message">The message.</param>
        public ArgDirectoryPathException(object argValue, string argName, string message)
            : base("{0} [Arg:{1},Value:{2}]".SafeFormatWith(message, argName, argValue))
        {
            ErrorCode = 1007;
        }
    }
}
