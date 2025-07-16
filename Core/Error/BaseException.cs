//--------------------------------------------------------------------------
// File:    BaseException.cs
// Content:	Implementation of class BaseException
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// This simply extends the <see cref="Exception"/> class
	/// by adding a variable length parameter list in the basic
	/// constructor which takes the exception message, and then
	/// apply string.Format if necessary, which is an incredibly
	/// common expectation when throwing exceptions, and should have been
	/// part of the base exception class.
	/// </summary>
    public class BaseException : Exception
    {

        /// <summary>Key for userfriendly error message in Exception.Data</summary>
        private const string USER_FRIENDLY_MESSAGE = "UserFriendlyMessage";

        /// <summary>Set or get the userfriendly error message</summary>
        public string? UserFriendlyMessage
        {
            get => !Data.Contains(USER_FRIENDLY_MESSAGE) ? string.Empty : Data[USER_FRIENDLY_MESSAGE] as string;

            set => Data[USER_FRIENDLY_MESSAGE] = value ?? string.Empty;
        }

        private int m_ErrorCode = 1;

        /// <summary>Liefert den Fehlercode, der zu einer Exception hinterlegt ist.
        /// Default ist 1, das kann in speziellen abgeleiteten Klassen überschrieben werden.
        /// </summary>
        public int ErrorCode
        {
            get => m_ErrorCode; set => m_ErrorCode = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exception"/> class.
        /// </summary>
        public BaseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="formatParameters">The format parameters.</param>
        public BaseException(string message, params object[] formatParameters)
            : base(formatParameters != null && formatParameters.Length > 0 ? message.SafeFormatWith(formatParameters) : message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="message">The message.</param>
        /// <param name="formatParameters">The format parameters.</param>
        public BaseException(Exception inner, string message, params object[] formatParameters)
            : base(
                formatParameters != null && formatParameters.Length > 0 ? message.SafeFormatWith(formatParameters) : message, inner)
        {
        }
    }
}
