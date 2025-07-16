//--------------------------------------------------------------------------
// File:    TechException.cs
// Content:	Implementation of class TechException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    ///<summary>Base Exception class for all technical errors.</summary>
    public class TechException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TechException"/> class.
        /// </summary>
        public TechException()
        {
            ErrorCode = 1000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TechException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="formatParameters">The format parameters.</param>
        public TechException(string message, params object[] formatParameters)
            : base(formatParameters != null && formatParameters.Length > 0 ? message.SafeFormatWith(formatParameters) : message)
        {
            ErrorCode = 1000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TechException"/> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="message">The message.</param>
        /// <param name="formatParameters">The format parameters.</param>
        public TechException(Exception inner, string message, params object[] formatParameters)
            : base(inner,
                formatParameters != null && formatParameters.Length > 0 ? message.SafeFormatWith(formatParameters) : message)
        {
            ErrorCode = 1000;
        }
    }
}
