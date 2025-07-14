//--------------------------------------------------------------------------
// File:    BusinessException.cs
// Content:	Implementation of class BusinessException
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    ///<summary>Base Exception class for all business errors.</summary>
    public class BusinessException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class.
        /// </summary>
        public BusinessException()
        {
            ErrorCode = 2000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="formatParameters">The format parameters.</param>
        public BusinessException(string message, params object[] formatParameters)
            : base(formatParameters != null && formatParameters.Length > 0 ? message.SafeFormatWith(formatParameters) : message)
        {
            ErrorCode = 2000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="message">The message.</param>
        /// <param name="formatParameters">The format parameters.</param>
        public BusinessException(Exception inner, string message, params object[] formatParameters)
            : base(inner,
                formatParameters != null && formatParameters.Length > 0 ? message.SafeFormatWith(formatParameters) : message)
        {
            ErrorCode = 2000;
        }

    }
}
