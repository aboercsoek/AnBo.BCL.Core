//--------------------------------------------------------------------------
// File:    Console2FileExtensions.cs
// Content:	Extension methods for enhanced Console2File functionality
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Extension methods for enhanced Console2File functionality.
/// Provides additional utility methods for common redirection scenarios including
/// timestamped logging, log rotation, and convenient factory methods.
/// </summary>
/// <remarks>
/// <para>
/// This static class extends the basic Console2File functionality with commonly needed
/// features that simplify log management and file handling scenarios. The methods are
/// designed to work seamlessly with both synchronous and asynchronous console redirection.
/// </para>
/// <para>
/// Key functionality includes:
/// </para>
/// <list type="bullet">
/// <item><description>Timestamped file path generation for unique log files</description></item>
/// <item><description>Log rotation based on file size limits</description></item>
/// <item><description>Factory methods for common redirection patterns</description></item>
/// <item><description>Thread-safe operations for concurrent scenarios</description></item>
/// <item><description>Comprehensive error handling with graceful fallbacks</description></item>
/// </list>
/// </remarks>
public static class Console2FileExtensions
{
    #region Path Generation Methods

    /// <summary>
    /// Creates a timestamped log file name based on the provided base path.
    /// Useful for creating unique log files for each application run or session.
    /// The timestamp is inserted before the file extension to maintain file type association.
    /// </summary>
    /// <param name="basePath">The base file path without timestamp.</param>
    /// <param name="format">Optional timestamp format (default: "yyyyMMdd-HHmmss").</param>
    /// <returns>A timestamped file path with the format: basename-timestamp.extension</returns>
    /// <exception cref="ArgumentException">Thrown when basePath is null, empty, or whitespace.</exception>
    /// <remarks>
    /// <para>
    /// This method generates unique file names by incorporating the current date and time
    /// into the file path. This is particularly useful for:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Creating separate log files for each application session</description></item>
    /// <item><description>Preventing log file conflicts in multi-instance scenarios</description></item>
    /// <item><description>Organizing logs chronologically</description></item>
    /// <item><description>Automatic log archiving by timestamp</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic timestamped path generation
    /// string timestampedPath = Console2FileExtensions.CreateTimestampedPath("logs/app.log");
    /// // Result: "logs/app-20250726-143052.log"
    /// 
    /// using var redirect = new Console2File(timestampedPath);
    /// Console.WriteLine("This goes to a timestamped log file");
    /// 
    /// // Custom timestamp format
    /// string customPath = Console2FileExtensions.CreateTimestampedPath("logs/app.log", "yyyy-MM-dd_HH-mm-ss");
    /// // Result: "logs/app-2025-07-26_14-30-52.log"
    /// 
    /// // Different file types work automatically
    /// string txtPath = Console2FileExtensions.CreateTimestampedPath("output.txt");
    /// string csvPath = Console2FileExtensions.CreateTimestampedPath("data.csv", "yyyyMMdd");
    /// // Results: "output-20250726-143052.txt", "data-20250726.csv"
    /// 
    /// // Handling subdirectories
    /// string subDirPath = Console2FileExtensions.CreateTimestampedPath(@"logs\sub\application.log");
    /// // Result: "logs\sub\application-20250726-143052.log"
    /// </code>
    /// </example>
    public static string CreateTimestampedPath(string basePath, string format = "yyyyMMdd-HHmmss")
    {
        if (string.IsNullOrWhiteSpace(basePath))
            throw new ArgumentException("Base path cannot be null or empty.", nameof(basePath));

        var directory = Path.GetDirectoryName(basePath) ?? string.Empty;
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(basePath);
        var extension = Path.GetExtension(basePath);
        var timestamp = DateTime.Now.ToString(format);

        var timestampedFileName = $"{fileNameWithoutExt}-{timestamp}{extension}";
        return Path.Combine(directory, timestampedFileName);
    }

    /// <summary>
    /// Creates a rotating log file path based on file size limits.
    /// Automatically generates numbered backup files when size limit is reached.
    /// Implements a rolling file strategy with configurable retention policies.
    /// </summary>
    /// <param name="basePath">The base file path for rotation.</param>
    /// <param name="maxSizeBytes">Maximum file size in bytes before rotation occurs.</param>
    /// <param name="maxFiles">Maximum number of rotated files to keep (default: 10).</param>
    /// <returns>The appropriate log file path for current writing operations.</returns>
    /// <exception cref="ArgumentException">Thrown when basePath is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when maxSizeBytes or maxFiles is less than or equal to zero.</exception>
    /// <remarks>
    /// <para>
    /// This method implements a log rotation strategy that helps manage disk space by:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Checking if the current log file exceeds the size limit</description></item>
    /// <item><description>Rotating existing files to numbered backups (app.log → app.1.log)</description></item>
    /// <item><description>Removing the oldest backup when the file limit is reached</description></item>
    /// <item><description>Creating a fresh log file for new content</description></item>
    /// </list>
    /// <para>
    /// The rotation pattern follows the format: basename.N.extension, where N increases
    /// with age (1 = newest backup, maxFiles = oldest backup).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic log rotation - rotate when file exceeds 1MB, keep 5 backups
    /// string rotatedPath = Console2FileExtensions.CreateRotatingPath("logs/app.log", 1024 * 1024, 5);
    /// using var redirect = new Console2File(rotatedPath);
    /// 
    /// // Rotation creates files like:
    /// // app.log (current)
    /// // app.1.log (previous)
    /// // app.2.log (older)
    /// // app.3.log (older)
    /// // app.4.log (older)
    /// // app.5.log (oldest - will be deleted on next rotation)
    /// 
    /// // High-frequency logging with smaller rotation size
    /// string highFreqPath = Console2FileExtensions.CreateRotatingPath("debug.log", 512 * 1024, 3);
    /// using var debugRedirect = new Console2File(highFreqPath);
    /// 
    /// // Large application logs with extensive history
    /// string largePath = Console2FileExtensions.CreateRotatingPath("application.log", 10 * 1024 * 1024, 20);
    /// using var appRedirect = new Console2File(largePath);
    /// 
    /// // Check if rotation occurred
    /// long currentSize = redirect.GetFileSize();
    /// if (currentSize < 1024 * 1024) // Less than max size
    /// {
    ///     Console.WriteLine("Rotation occurred - fresh log file");
    /// }
    /// </code>
    /// </example>
    public static string CreateRotatingPath(string basePath, long maxSizeBytes, int maxFiles = 10)
    {
        if (string.IsNullOrWhiteSpace(basePath))
            throw new ArgumentException("Base path cannot be null or empty.", nameof(basePath));
        if (maxSizeBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxSizeBytes), "Max size must be greater than zero.");
        if (maxFiles <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxFiles), "Max files must be greater than zero.");

        // Check if base file exists and needs rotation
        if (!File.Exists(basePath))
            return basePath;

        var fileInfo = new FileInfo(basePath);
        if (fileInfo.Length < maxSizeBytes)
            return basePath;

        // Perform rotation
        RotateLogFiles(basePath, maxFiles);
        return basePath;
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Rotates existing log files by renaming them with incremental numbers.
    /// Internal helper method for log rotation functionality that handles the file system operations.
    /// </summary>
    /// <param name="basePath">The base file path to rotate.</param>
    /// <param name="maxFiles">Maximum number of files to keep in rotation.</param>
    /// <remarks>
    /// This method performs the actual file system operations for log rotation:
    /// <list type="number">
    /// <item><description>Remove the oldest file (basename.maxFiles.ext) if it exists</description></item>
    /// <item><description>Rename files in reverse order (N → N+1) to make room</description></item>
    /// <item><description>Move the current file to the first backup position (basename → basename.1.ext)</description></item>
    /// </list>
    /// Error handling is designed to be non-destructive - if rotation fails, logging continues with the existing file.
    /// </remarks>
    private static void RotateLogFiles(string basePath, int maxFiles)
    {
        var directory = Path.GetDirectoryName(basePath) ?? string.Empty;
        var fileName = Path.GetFileNameWithoutExtension(basePath);
        var extension = Path.GetExtension(basePath);

        try
        {
            // Remove the oldest file if we're at the limit
            var oldestFile = Path.Combine(directory, $"{fileName}.{maxFiles}{extension}");
            if (File.Exists(oldestFile))
            {
                File.Delete(oldestFile);
            }

            // Rotate existing files (move N to N+1)
            for (int i = maxFiles - 1; i >= 1; i--)
            {
                var currentFile = Path.Combine(directory, $"{fileName}.{i}{extension}");
                var nextFile = Path.Combine(directory, $"{fileName}.{i + 1}{extension}");

                if (File.Exists(currentFile))
                {
                    if (File.Exists(nextFile))
                        File.Delete(nextFile);
                    File.Move(currentFile, nextFile);
                }
            }

            // Move the base file to .1
            var firstRotatedFile = Path.Combine(directory, $"{fileName}.1{extension}");
            if (File.Exists(firstRotatedFile))
                File.Delete(firstRotatedFile);
            File.Move(basePath, firstRotatedFile);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // If rotation fails, continue with the original file
            System.Diagnostics.Debug.WriteLine($"Log rotation failed for {basePath}: {ex.Message}");
        }
    }

    #endregion

    #region Factory Methods for Console2File

    /// <summary>
    /// Creates a Console2File instance with automatic timestamping.
    /// Convenience method for common timestamped logging scenarios that combines
    /// path generation and redirection creation in a single operation.
    /// </summary>
    /// <param name="basePath">The base file path for timestamped logging.</param>
    /// <param name="timestampFormat">Optional timestamp format (default: "yyyyMMdd-HHmmss").</param>
    /// <returns>A new Console2File instance with timestamped file path.</returns>
    /// <exception cref="ArgumentException">Thrown when basePath is invalid.</exception>
    /// <exception cref="IOException">Thrown when file creation fails.</exception>
    /// <remarks>
    /// This factory method simplifies the common pattern of creating timestamped log files
    /// by combining path generation and redirection creation. It's equivalent to calling
    /// CreateTimestampedPath followed by the Console2File constructor.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Simple timestamped logging
    /// using var redirect = Console2FileExtensions.CreateTimestamped("logs/session.log");
    /// Console.WriteLine("Session started with automatic timestamping");
    /// // Creates file like: logs/session-20250726-143052.log
    /// 
    /// // Custom timestamp format
    /// using var customRedirect = Console2FileExtensions.CreateTimestamped("logs/app.log", "yyyy-MM-dd");
    /// Console.WriteLine("Daily log with custom format");
    /// // Creates file like: logs/app-2025-07-26.log
    /// 
    /// // Multiple timestamped sessions
    /// using var session1 = Console2FileExtensions.CreateTimestamped("sessions/user.log");
    /// using var session2 = Console2FileExtensions.CreateTimestamped("sessions/admin.log");
    /// // Creates unique files for each session automatically
    /// 
    /// // Error handling
    /// try
    /// {
    ///     using var redirect = Console2FileExtensions.CreateTimestamped(@"Z:\InvalidDrive\test.log");
    /// }
    /// catch (IOException ex)
    /// {
    ///     Console.WriteLine($"Failed to create timestamped redirection: {ex.Message}");
    /// }
    /// </code>
    /// </example>
    public static Console2File CreateTimestamped(string basePath, string timestampFormat = "yyyyMMdd-HHmmss")
    {
        var timestampedPath = CreateTimestampedPath(basePath, timestampFormat);
        return new Console2File(timestampedPath);
    }

    /// <summary>
    /// Creates a Console2File instance with automatic log rotation.
    /// Convenience method that combines rotation path logic with redirection creation
    /// for applications that need automatic log file management.
    /// </summary>
    /// <param name="basePath">The base file path for rotating logs.</param>
    /// <param name="maxSizeBytes">Maximum file size before rotation occurs.</param>
    /// <param name="maxFiles">Maximum number of backup files to maintain (default: 10).</param>
    /// <returns>A new Console2File instance with rotation-managed file path.</returns>
    /// <exception cref="ArgumentException">Thrown when basePath is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when size or file limits are invalid.</exception>
    /// <exception cref="IOException">Thrown when file creation fails.</exception>
    /// <remarks>
    /// This factory method provides an easy way to implement log rotation without manual
    /// file management. The rotation occurs automatically when the current log file
    /// exceeds the specified size limit, making it ideal for long-running applications.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create rotating log with 1MB limit and 5 backup files
    /// using var rotate = Console2FileExtensions.CreateRotating("app.log", 1024 * 1024, 5);
    /// for (int i = 0; i < 10000; i++)
    /// {
    ///     Console.WriteLine($"Log entry {i}: {DateTime.Now}");
    ///     // Rotation happens automatically when size limit is reached
    /// }
    /// 
    /// // High-frequency debug logging with smaller files
    /// using var debug = Console2FileExtensions.CreateRotating("debug.log", 512 * 1024, 3);
    /// Console.WriteLine("Debug logging with automatic rotation");
    /// 
    /// // Production logging with larger files and more history
    /// using var prod = Console2FileExtensions.CreateRotating("production.log", 10 * 1024 * 1024, 20);
    /// Console.WriteLine("Production logging with extensive backup history");
    /// </code>
    /// </example>
    public static Console2File CreateRotating(string basePath, long maxSizeBytes, int maxFiles = 10)
    {
        var rotatingPath = CreateRotatingPath(basePath, maxSizeBytes, maxFiles);
        return new Console2File(rotatingPath);
    }

    #endregion

    #region Advanced Factory Methods

    /// <summary>
    /// Creates a Console2File instance with both timestamping and rotation.
    /// Combines the benefits of unique timestamped files with size-based rotation
    /// for comprehensive log management in complex applications.
    /// </summary>
    /// <param name="basePath">The base file path for the combined logging strategy.</param>
    /// <param name="maxSizeBytes">Maximum file size before rotation occurs.</param>
    /// <param name="maxFiles">Maximum number of backup files to maintain.</param>
    /// <param name="timestampFormat">Timestamp format for the base file name.</param>
    /// <returns>A new Console2File instance with both timestamping and rotation.</returns>
    /// <remarks>
    /// This advanced factory method creates a logging strategy where each session gets
    /// a unique timestamped file, and if that file grows too large, it gets rotated.
    /// This provides both session separation and size management.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Combined timestamping and rotation
    /// using var combined = Console2FileExtensions.CreateTimestampedRotating("logs/app.log", 2 * 1024 * 1024, 5);
    /// // Creates: logs/app-20250726-143052.log with rotation when it exceeds 2MB
    /// 
    /// Console.WriteLine("Starting application with advanced logging");
    /// // If this session generates more than 2MB of logs, rotation will occur
    /// // producing files like: app-20250726-143052.1.log, app-20250726-143052.2.log, etc.
    /// </code>
    /// </example>
    public static Console2File CreateTimestampedRotating(string basePath, long maxSizeBytes, int maxFiles, string timestampFormat = "yyyyMMdd-HHmmss")
    {
        var timestampedPath = CreateTimestampedPath(basePath, timestampFormat);
        var rotatingPath = CreateRotatingPath(timestampedPath, maxSizeBytes, maxFiles);
        return new Console2File(rotatingPath);
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets information about log rotation status for a given base path.
    /// Provides insights into the current state of log rotation including
    /// file sizes, rotation counts, and disk usage patterns.
    /// </summary>
    /// <param name="basePath">The base path to analyze for rotation information.</param>
    /// <returns>A tuple containing rotation statistics and file information.</returns>
    /// <remarks>
    /// This method helps monitor and troubleshoot log rotation behavior by providing
    /// comprehensive information about the current state of rotated log files.
    /// </remarks>
    /// <example>
    /// <code>
    /// var (fileCount, totalSize, oldestFile, newestFile) = Console2FileExtensions.GetRotationInfo("app.log");
    /// Console.WriteLine($"Rotation Status:");
    /// Console.WriteLine($"  Files: {fileCount}");
    /// Console.WriteLine($"  Total Size: {totalSize:N0} bytes");
    /// Console.WriteLine($"  Oldest: {oldestFile}");
    /// Console.WriteLine($"  Newest: {newestFile}");
    /// </code>
    /// </example>
    public static (int FileCount, long TotalSize, string? OldestFile, string? NewestFile) GetRotationInfo(string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
            return (0, 0, null, null);

        try
        {
            var directory = Path.GetDirectoryName(basePath) ?? string.Empty;
            var fileName = Path.GetFileNameWithoutExtension(basePath);
            var extension = Path.GetExtension(basePath);
            var baseFileName = Path.GetFileName(basePath);

            var files = new List<(string Path, int Number, long Size)>();

            // Check base file
            if (File.Exists(basePath))
            {
                var info = new FileInfo(basePath);
                files.Add((basePath, 0, info.Length));
            }

            // Check rotated files
            for (int i = 1; i <= 100; i++) // Check up to 100 rotated files
            {
                var rotatedFile = Path.Combine(directory, $"{fileName}.{i}{extension}");
                if (File.Exists(rotatedFile))
                {
                    var info = new FileInfo(rotatedFile);
                    files.Add((rotatedFile, i, info.Length));
                }
                else
                {
                    break; // No more rotated files
                }
            }

            if (files.Count == 0)
                return (0, 0, null, null);

            var totalSize = files.Sum(f => f.Size);
            var oldestFile = files.OrderByDescending(f => f.Number).First().Path;
            var newestFile = files.OrderBy(f => f.Number).First().Path;

            return (files.Count, totalSize, oldestFile, newestFile);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return (0, 0, null, null);
        }
    }

    /// <summary>
    /// Cleans up old rotated log files beyond the specified retention policy.
    /// Useful for maintenance operations and disk space management.
    /// </summary>
    /// <param name="basePath">The base path of the log files to clean up.</param>
    /// <param name="maxFiles">Maximum number of files to retain.</param>
    /// <returns>The number of files that were deleted during cleanup.</returns>
    /// <remarks>
    /// This utility method helps maintain log file retention policies by removing
    /// files that exceed the specified limits. It's useful for periodic maintenance
    /// tasks and automated cleanup operations.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Clean up old log files, keeping only the newest 10
    /// int deletedCount = Console2FileExtensions.CleanupRotatedFiles("app.log", 10);
    /// Console.WriteLine($"Cleanup completed: {deletedCount} old files removed");
    /// 
    /// // Scheduled cleanup task
    /// var timer = new Timer(async _ =>
    /// {
    ///     int cleaned = Console2FileExtensions.CleanupRotatedFiles("production.log", 50);
    ///     if (cleaned > 0)
    ///         Console.WriteLine($"Automated cleanup: {cleaned} files removed");
    /// }, null, TimeSpan.Zero, TimeSpan.FromHours(24));
    /// </code>
    /// </example>
    public static int CleanupRotatedFiles(string basePath, int maxFiles)
    {
        if (string.IsNullOrWhiteSpace(basePath) || maxFiles <= 0)
            return 0;

        try
        {
            var directory = Path.GetDirectoryName(basePath) ?? string.Empty;
            var fileName = Path.GetFileNameWithoutExtension(basePath);
            var extension = Path.GetExtension(basePath);

            int deletedCount = 0;

            // Find and delete files beyond the retention limit
            for (int i = maxFiles + 1; i <= 1000; i++) // Check up to 1000 files
            {
                var fileToDelete = Path.Combine(directory, $"{fileName}.{i}{extension}");
                if (File.Exists(fileToDelete))
                {
                    File.Delete(fileToDelete);
                    deletedCount++;
                }
                //else
                //{
                //    break; // No more files to delete
                //}
            }

            return deletedCount;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return 0;
        }
    }

    #endregion
}
