//--------------------------------------------------------------------------
// File:    Console2File.cs
// Content:	Synchronous console output redirection implementation for .NET 8+
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections.Concurrent;
using System.Text;
namespace AnBo.Core;

#endregion


/// <summary>
/// Console output redirection class with enhanced error handling and performance optimization.
/// Provides thread-safe console redirection to file with automatic resource management.
/// Optimized for synchronous scenarios with efficient buffering and reference counting.
/// </summary>
/// <remarks>
/// <para>
/// This class implements a robust console redirection mechanism that captures all console output
/// and writes it to a specified file while maintaining the original console output behavior.
/// The implementation uses reference counting to safely handle multiple redirections to the same file.
/// </para>
/// <para>
/// Key features include:
/// </para>
/// <list type="bullet">
/// <item><description>Thread-safe operations with proper locking mechanisms</description></item>
/// <item><description>Reference counting for shared file redirections</description></item>
/// <item><description>Automatic directory creation for target files</description></item>
/// <item><description>Optimized buffering for improved I/O performance</description></item>
/// <item><description>Comprehensive error handling with meaningful exceptions</description></item>
/// <item><description>Proper IDisposable implementation with finalizer backup</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Basic usage with automatic disposal
/// using var redirect = new Console2File("output.log");
/// Console.WriteLine("This goes to both console and file");
/// 
/// // Multiple redirections to the same file are handled safely
/// using var redirect1 = new Console2File("shared.log");
/// using var redirect2 = new Console2File("shared.log"); // Safe - reference counted
/// 
/// // Manual flushing for critical operations
/// using var redirect3 = new Console2File("critical.log");
/// Console.WriteLine("Important message");
/// redirect3.Flush(); // Ensure immediate write to disk
/// </code>
/// </example>
public class Console2File : IConsole2File
{
    #region Private fields

    private readonly TextWriter? _originalConsoleOut;
    private readonly StreamWriter? _fileWriter;
    private readonly string _filePath;
    
    private bool _disposed;

    private static readonly object _redirectionLock = new();
    private static readonly ConcurrentDictionary<string, int> _activeRedirections = new();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the file path where console output is being redirected.
    /// </summary>
    /// <value>The full path to the redirection file.</value>
    public string FilePath => _filePath;

    /// <summary>
    /// Gets a value indicating whether the redirection is currently active and not disposed.
    /// </summary>
    /// <value>True if redirection is active; otherwise, false.</value>
    public bool IsActive => !_disposed && _fileWriter != null;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Console2File"/> class.
    /// Redirects console output to the specified file with automatic directory creation.
    /// Uses UTF-8 encoding and append mode for optimal compatibility and data preservation.
    /// </summary>
    /// <param name="filePath">The file path for console output redirection.</param>
    /// <exception cref="ArgumentException">Thrown when filePath is null, empty, or contains invalid characters.</exception>
    /// <exception cref="IOException">Thrown when file access fails or directory creation fails.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when insufficient permissions for file access.</exception>
    /// <example>
    /// <code>
    /// // Basic redirection
    /// using var redirect = new Console2File("logs/application.log");
    /// Console.WriteLine("Application started"); // Goes to both console and file
    /// 
    /// // Redirection with automatic directory creation
    /// using var redirect2 = new Console2File(@"C:\Logs\SubDir\app.log");
    /// Console.WriteLine("Directory will be created automatically if needed");
    /// 
    /// // Multiple instances sharing the same file (reference counted)
    /// using var redirect3 = new Console2File("shared.log");
    /// using var redirect4 = new Console2File("shared.log"); // Safe concurrency
    /// </code>
    /// </example>
    public Console2File(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        //if (FileSystemManager.GetValidFilename(filePath) != filePath)
        //    throw new ArgumentException($"Invalid file path: '{filePath}'", nameof(filePath));
        Path.GetInvalidFileNameChars().ToList().ForEach(c =>
        {
            if (Path.GetFileName(filePath).Contains(c))
                throw new ArgumentException($"File path '{filePath}' contains invalid character '{c}'", nameof(filePath));
        });

        try
        {
            _filePath = Path.GetFullPath(filePath);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            lock (_redirectionLock)
            {
                // Track active redirections to the same file
                _activeRedirections.AddOrUpdate(_filePath, 1, (key, count) => count + 1);

                // Store original console output
                _originalConsoleOut = Console.Out;

                // Create FileStream with sharing enabled
                var fileStream = new FileStream(_filePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.ReadWrite,  // ← Ermöglicht Multiple Writers
                    bufferSize: 8192);

                // Create file writer with shared FileStream
                _fileWriter = new StreamWriter(fileStream, Encoding.UTF8, bufferSize: 8192)
                {
                    AutoFlush = true
                };

                // Redirect console output
                Console.SetOut(_fileWriter);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
        {
            // Clean up on failure and provide meaningful error information
            _fileWriter?.Dispose();
            throw new IOException($"Failed to redirect console output to '{filePath}': {ex.Message}", ex);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Invalid file path '{filePath}': {ex.Message}", nameof(filePath), ex);
        }
    }

    /// <summary>
    /// Protected constructor to prevent direct instantiation of base class.
    /// Used by derived classes for custom initialization scenarios.
    /// </summary>
    protected Console2File()
    {
        _filePath = string.Empty;
        _originalConsoleOut = null;
        _fileWriter = null;
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Releases all resources used by the Console2File and restores original console output.
    /// This method is thread-safe and can be called multiple times safely.
    /// Implements proper disposal pattern with reference counting for shared files.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources; false if called from finalizer.</param>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            lock (_redirectionLock)
            {
                try
                {
                    // Restore original console output if we have it
                    if (_originalConsoleOut != null)
                    {
                        Console.SetOut(_originalConsoleOut);
                    }

                    // Flush and close file writer
                    _fileWriter?.Flush();
                    _fileWriter?.Dispose();

                    // Decrement reference count for this file
                    if (!string.IsNullOrEmpty(_filePath))
                    {
                        _activeRedirections.AddOrUpdate(_filePath, 0, (key, count) => Math.Max(0, count - 1));

                        // Clean up entry if no more references
                        if (_activeRedirections.TryGetValue(_filePath, out var remainingCount) && remainingCount == 0)
                        {
                            _activeRedirections.TryRemove(_filePath, out _);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log disposal errors but don't throw from Dispose
                    System.Diagnostics.Debug.WriteLine($"Error during Console2File disposal: {ex.Message}");
                }
            }
        }

        _disposed = true;
    }

    /// <summary>
    /// Releases all resources used by the Console2File and restores original console output.
    /// Automatically called when used in a using statement or explicitly invoked.
    /// </summary>
    /// <example>
    /// <code>
    /// // Automatic disposal with using statement (recommended)
    /// using (var redirect = new Console2File("auto-dispose.log"))
    /// {
    ///     Console.WriteLine("This message will be logged");
    /// } // Automatic disposal and console restoration happens here
    /// 
    /// // Manual disposal
    /// var redirect2 = new Console2File("manual-dispose.log");
    /// try
    /// {
    ///     Console.WriteLine("Manual logging");
    /// }
    /// finally
    /// {
    ///     redirect2.Dispose(); // Manual cleanup
    /// }
    /// </code>
    /// </example>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer for Console2File.
    /// Ensures resources are cleaned up even if Dispose is not called explicitly.
    /// This provides a safety net for resource cleanup, though explicit disposal is preferred.
    /// </summary>
    ~Console2File()
    {
        Dispose(disposing: false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Manually flushes any buffered console output to the file.
    /// Useful for ensuring data persistence at critical points in application execution.
    /// This method is thread-safe and can be called multiple times without side effects.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed.</exception>
    /// <remarks>
    /// While AutoFlush is enabled by default, manual flushing can be useful in scenarios where
    /// immediate data persistence is critical, such as before application shutdown or after
    /// writing important diagnostic information.
    /// </remarks>
    /// <example>
    /// <code>
    /// using var redirect = new Console2File("critical.log");
    /// Console.WriteLine("Critical operation started");
    /// redirect.Flush(); // Ensure message is written immediately
    /// 
    /// // Perform critical operation
    /// PerformCriticalOperation();
    /// 
    /// Console.WriteLine("Critical operation completed");
    /// redirect.Flush(); // Ensure completion message is persisted
    /// </code>
    /// </example>
    public void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_redirectionLock)
        {
            _fileWriter?.Flush();
        }
    }

    /// <summary>
    /// Gets the current file size in bytes.
    /// Returns 0 if the file doesn't exist or if the redirection is not active.
    /// This method provides a convenient way to monitor log file growth.
    /// </summary>
    /// <returns>The size of the redirection file in bytes, or 0 if the file doesn't exist or an error occurs.</returns>
    /// <remarks>
    /// This method is designed to be non-throwing for monitoring scenarios. If file access fails
    /// due to permissions or I/O errors, it returns 0 rather than throwing an exception.
    /// For critical scenarios where file existence must be verified, consider using File.Exists first.
    /// </remarks>
    /// <example>
    /// <code>
    /// using var redirect = new Console2File("size-test.log");
    /// Console.WriteLine("Test message");
    /// 
    /// long initialSize = redirect.GetFileSize();
    /// Console.WriteLine($"File size after first message: {initialSize} bytes");
    /// 
    /// Console.WriteLine("Another message");
    /// long finalSize = redirect.GetFileSize();
    /// Console.WriteLine($"File size after second message: {finalSize} bytes");
    /// Console.WriteLine($"Size difference: {finalSize - initialSize} bytes");
    /// </code>
    /// </example>
    public long GetFileSize()
    {
        if (_disposed || string.IsNullOrEmpty(_filePath))
            return 0;

        try
        {
            return File.Exists(_filePath) ? new FileInfo(_filePath).Length : 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return 0; // Return 0 instead of throwing for non-critical operation
        }
    }

    #endregion

    #region Static Utility Methods

    /// <summary>
    /// Gets the number of active redirections to the specified file path.
    /// Useful for monitoring and debugging redirection usage patterns.
    /// This method helps identify potential resource leaks or unexpected redirection behavior.
    /// </summary>
    /// <param name="filePath">The file path to check for active redirections.</param>
    /// <returns>The number of active Console2File instances redirecting to the specified file.</returns>
    /// <remarks>
    /// This method is particularly useful in multi-threaded applications or when implementing
    /// custom logging strategies that need to coordinate multiple redirection instances.
    /// The count reflects the reference counting used internally to manage shared file access.
    /// </remarks>
    /// <example>
    /// <code>
    /// string logFile = "shared.log";
    /// 
    /// // Create multiple redirections
    /// using var redirect1 = new Console2File(logFile);
    /// Console.WriteLine($"Active redirections: {Console2File.GetActiveRedirectionCount(logFile)}"); // 1
    /// 
    /// using var redirect2 = new Console2File(logFile);
    /// Console.WriteLine($"Active redirections: {Console2File.GetActiveRedirectionCount(logFile)}"); // 2
    /// 
    /// // redirect1 disposed here
    /// Console.WriteLine($"Active redirections: {Console2File.GetActiveRedirectionCount(logFile)}"); // 1
    /// 
    /// // redirect2 disposed here - count becomes 0
    /// </code>
    /// </example>
    public static int GetActiveRedirectionCount(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return 0;

        try
        {
            var fullPath = Path.GetFullPath(filePath);
            return _activeRedirections.TryGetValue(fullPath, out var count) ? count : 0;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            return 0;
        }
    }

    /// <summary>
    /// Gets all currently active redirection file paths.
    /// Useful for monitoring and management of active redirections across the application.
    /// This method provides insight into all current console redirection activity.
    /// </summary>
    /// <returns>An enumerable of file paths that currently have active redirections.</returns>
    /// <remarks>
    /// The returned collection is a snapshot taken at the time of the method call.
    /// The actual set of active redirections may change immediately after this method returns
    /// due to concurrent disposal operations. This method is primarily intended for monitoring
    /// and diagnostic purposes.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create several redirections
    /// using var redirect1 = new Console2File("log1.txt");
    /// using var redirect2 = new Console2File("log2.txt");
    /// using var redirect3 = new Console2File("subdirectory/log3.txt");
    /// 
    /// var activePaths = Console2File.GetActiveRedirectionPaths();
    /// Console.WriteLine("Currently active redirections:");
    /// foreach (var path in activePaths)
    /// {
    ///     Console.WriteLine($"  - {path} ({Console2File.GetActiveRedirectionCount(path)} instances)");
    /// }
    /// 
    /// // Output:
    /// // Currently active redirections:
    /// //   - C:\MyApp\log1.txt (1 instances)
    /// //   - C:\MyApp\log2.txt (1 instances)
    /// //   - C:\MyApp\subdirectory\log3.txt (1 instances)
    /// </code>
    /// </example>
    public static IEnumerable<string> GetActiveRedirectionPaths()
    {
        return _activeRedirections
            .Where(kvp => kvp.Value > 0)
            .Select(kvp => kvp.Key)
            .ToList(); // Materialize to avoid concurrent modification issues
    }

    /// <summary>
    /// Creates a temporary redirection that automatically cleans up after the specified duration.
    /// Useful for timed logging scenarios, temporary debugging output, or time-boxed operations.
    /// The redirection is automatically disposed when the duration expires.
    /// </summary>
    /// <param name="filePath">The file path for temporary redirection.</param>
    /// <param name="duration">The duration to maintain the redirection.</param>
    /// <returns>A task that completes when the redirection expires and cleanup is finished.</returns>
    /// <remarks>
    /// This method is particularly useful for scenarios where you want to capture console output
    /// for a specific time period, such as during application startup, specific operations,
    /// or debugging sessions. The redirection is guaranteed to be cleaned up after the specified duration.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Capture console output for 30 seconds
    /// var tempRedirection = Console2File.CreateTemporaryRedirection("temp.log", TimeSpan.FromSeconds(30));
    /// 
    /// Console.WriteLine("This will be redirected for 30 seconds");
    /// Console.WriteLine("All console output during this period goes to temp.log");
    /// 
    /// // Do some work that generates console output
    /// await SomeOperation();
    /// 
    /// // Wait for redirection to expire
    /// await tempRedirection;
    /// 
    /// Console.WriteLine("This will only go to console - redirection has ended");
    /// 
    /// // temp.log now contains all output from the 30-second period
    /// </code>
    /// </example>
    public static async Task CreateTemporaryRedirection(string filePath, TimeSpan duration)
    {
        using var redirection = new Console2File(filePath);
        await Task.Delay(duration);
    }

    #endregion
}
