//--------------------------------------------------------------------------
// File:    ExceptionText.cs
// Content:	Implementation of class ExceptionText
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Result of rendering an exception.
/// </summary>
[Serializable]
public class ExceptionText
{
    /// <summary>
    /// <para>String representation of the exception rendered by collecting all of the data about the original exception and all of the inner/related exceptions in the tree.</para>
    /// <para>A more detailed and well-organized counterpart for <see cref="T:System.Exception" />'s <see cref="M:System.Exception.ToString" /> method.</para>
    /// </summary>
    public readonly string FullText;
    /// <summary>
    /// <para>Message of the exception, into which all of the inner exceptions' messages are also included.</para>
    /// <para>A more detailed counterpart for <see cref="T:System.Exception" />'s <see cref="P:System.Exception.Message" /> property.</para>
    /// </summary>
    public readonly string Message;
    /// <summary>
    /// <para>User friendly message of the exception.</para>
    /// </summary>
    public readonly string UserFriendlyMessage;


    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionText"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="fullText">The full text of the exception.</param>
    /// <param name="userFriendlyMessage">The user friendly exception message.</param>
    public ExceptionText(string message, string fullText, string userFriendlyMessage)
    {
        ArgChecker.ShouldNotBeNull(message);
        ArgChecker.ShouldNotBeNull(fullText);

        Message = message;
        FullText = fullText;
        UserFriendlyMessage = userFriendlyMessage ?? string.Empty;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return Message;
    }
}
