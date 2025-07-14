//--------------------------------------------------------------------------
// File:    InfrastructureException.cs
// Content:	Implementation of class InfrastructureException
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// This exception is used to mark (fatal) failures in infrastructure and system code.
	/// </summary>
    public class InfrastructureException : TechException
    {
        private const int TypeErrorCode = 1023;
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public InfrastructureException() : base(StringResources.ErrorInfrastructureOrSystem) { ErrorCode = TypeErrorCode; }
        /// <summary>
        /// Creates a new instance of this class with the specified message
        /// </summary>
        /// <param name="message">The message used for the exception</param>
        public InfrastructureException(string message) : base(message) { ErrorCode = TypeErrorCode; }
        /// <summary>
        /// Creates a new instance of this class with the specified message and inner exception
        /// </summary>
        /// <param name="cause">The inner exception that this instance wraps.</param>
        public InfrastructureException(Exception cause) : base(cause, StringResources.ErrorInfrastructureOrSystem) { ErrorCode = TypeErrorCode; }
        /// <summary>
        /// Creates a new instance of this class with the specified message and inner exception
        /// </summary>
        /// <param name="message">The message used for the exception</param>
        /// <param name="cause">The inner exception that this instance wraps.</param>
        public InfrastructureException(string message, Exception cause) : base(cause, message) { ErrorCode = TypeErrorCode; }
    }
}
