//--------------------------------------------------------------------------
// File:    IConsole2File.cs
// Content:	Interface for console redirection implementations
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
    /// Interface for console redirection implementations.
    /// Provides a common contract for both synchronous and asynchronous file redirection.
    /// Enables consistent interaction with different console redirection strategies.
    /// </summary>
    /// <remarks>
    /// This interface defines the core functionality that all console redirection implementations must provide.
    /// It serves as the foundation for both simple file redirection and advanced async scenarios.
    /// Implementations should ensure thread-safety and proper resource management.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Using the interface for polymorphic behavior
    /// IConsole2File CreateRedirection(bool useAsync, string filePath)
    /// {
    ///     return useAsync 
    ///         ? new AsyncConsole2File(filePath) 
    ///         : new Console2File(filePath);
    /// }
    /// 
    /// using var redirection = CreateRedirection(false, "output.log");
    /// Console.WriteLine($"Redirecting to: {redirection.FilePath}");
    /// </code>
    /// </example>
    public interface IConsole2File : IDisposable
    {
        /// <summary>
        /// Gets the file path where console output is being redirected.
        /// This path represents the target destination for all console output during the redirection period.
        /// </summary>
        /// <value>
        /// The full path to the redirection file. This value is set during construction and remains 
        /// constant throughout the lifetime of the redirection instance.
        /// </value>
        /// <example>
        /// <code>
        /// using var redirect = new Console2File("logs/application.log");
        /// Console.WriteLine($"Output redirected to: {redirect.FilePath}");
        /// // Output: Output redirected to: C:\MyApp\logs\application.log
        /// </code>
        /// </example>
        string FilePath { get; }

        /// <summary>
        /// Gets a value indicating whether the redirection is currently active and operational.
        /// A redirection is considered active when it has successfully initialized and has not been disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the redirection is active and console output is being written to the file; 
        /// otherwise, <c>false</c> if the redirection has been disposed or failed to initialize.
        /// </value>
        /// <remarks>
        /// This property is useful for monitoring the state of redirection instances, especially in 
        /// scenarios where multiple redirections might be managed or when implementing retry logic.
        /// Once a redirection becomes inactive (disposed), it cannot be reactivated.
        /// </remarks>
        /// <example>
        /// <code>
        /// var redirect = new Console2File("temp.log");
        /// Console.WriteLine($"Redirection active: {redirect.IsActive}"); // true
        /// 
        /// redirect.Dispose();
        /// Console.WriteLine($"Redirection active: {redirect.IsActive}"); // false
        /// </code>
        /// </example>
        bool IsActive { get; }
    }
}
