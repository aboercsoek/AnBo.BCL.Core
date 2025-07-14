//--------------------------------------------------------------------------
// File:    CombinedException.cs
// Content:	Implementation of class CombinedException
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// Generic exception for combining several other exceptions
	/// </summary>
    public class CombinedException : TechException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerExceptions">The inner exceptions.</param>
        public CombinedException(string message, IEnumerable<Exception> innerExceptions)
            : base(message)
        {
            ErrorCode = 1042;
            InnerExceptions = innerExceptions;
        }

        /// <summary>
        /// Gets the inner exceptions.
        /// </summary>
        /// <value>The inner exceptions.</value>
        public IEnumerable<Exception> InnerExceptions { get; protected set; }
    }
}
