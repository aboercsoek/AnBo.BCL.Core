//--------------------------------------------------------------------------
// File:    InvalidOperationRequestException.cs
// Content:	Implementation of class InvalidOperationRequestException
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Thrown when an operation request is denyed due to the state of the object.
	/// </summary>
    public class InvalidOperationRequestException : TechException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidOperationRequestException"/> class.
        /// </summary>
        public InvalidOperationRequestException()
        {
            ErrorCode = 1011;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidOperationRequestException"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public InvalidOperationRequestException(string operation)
            : base(StringResources.ErrorInvalidOperationRequestTemplate1Arg.SafeFormatWith(operation))
        {
            ErrorCode = 1011;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidOperationRequestException"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="message">The error message.</param>
        public InvalidOperationRequestException(string operation, string message)
            : base(StringResources.ErrorInvalidOperationRequestTemplate1Arg.SafeFormatWith(operation) + "\n" + message.SafeString())
        {
            ErrorCode = 1011;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidOperationRequestException"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="inner">The inner exception.</param>
        public InvalidOperationRequestException(string operation, Exception inner)
            : base(inner, StringResources.ErrorInvalidOperationRequestTemplate1Arg.SafeFormatWith(operation))
        {
            ErrorCode = 1011;
        }

    }
}
