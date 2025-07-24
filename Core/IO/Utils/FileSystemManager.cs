//--------------------------------------------------------------------------
// Class:	FileSystemManager
// Content:	Modern implementation of file system operations for .NET 8+
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core;

/// <summary>
/// A comprehensive file system manager optimized for .NET 8+.
/// Provides both synchronous and asynchronous operations for files, directories, and paths
/// with proper cancellation support, error handling, and modern performance optimizations.
/// </summary>
public static partial class FileSystemManager
{
    #region Private Members

    private static readonly char[] PathSeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

    /// <summary>
    /// Compiled regex for 3-character extension matching
    /// </summary>
    [GeneratedRegex(@"^\*\..{3}$", RegexOptions.Compiled)]
    private static partial Regex ExtensionPattern();

    #endregion

    #region Path Operations

    /// <summary>
    /// Adds a trailing directory separator to the input string if it doesn't already exist.
    /// Uses the platform-appropriate separator character.
    /// </summary>
    /// <param name="input">The input path string.</param>
    /// <returns>The input string with a trailing directory separator.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
    public static string EnsureTrailingSlash(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.EndsWith(Path.DirectorySeparatorChar) || input.EndsWith(Path.AltDirectorySeparatorChar)
            ? input
            : input + Path.DirectorySeparatorChar;
    }

    /// <summary>
    /// Adds a leading directory separator to the input string if it doesn't already exist.
    /// Uses the platform-appropriate separator character.
    /// </summary>
    /// <param name="input">The input path string.</param>
    /// <returns>The input string with a leading directory separator.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
    public static string EnsureLeadingSlash(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.StartsWith(Path.DirectorySeparatorChar) || input.StartsWith(Path.AltDirectorySeparatorChar)
            ? input
            : Path.DirectorySeparatorChar + input;
    }

    /// <summary>
    /// Combines baseDirectoryPath with relPath and normalizes the resulting path.
    /// </summary>
    /// <param name="baseDirectoryPath">The base directory</param>
    /// <param name="relPath">The relative path.</param>
    /// <returns>The absolute path of relPath.</returns>
    /// <exception cref="ArgumentNullException">
    /// Is thrown if <paramref name="baseDirectoryPath"/> or <paramref name="relPath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Is thrown if <paramref name="baseDirectoryPath"/> is empty.
    /// </exception>
    public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(baseDirectoryPath);
        ArgumentNullException.ThrowIfNull(relPath);

        return NormalizePath(Path.Combine(baseDirectoryPath, relPath));
    }

    /// <summary>
    /// Normalizes a file path by resolving relative components and using consistent separators.
    /// This method is a wrapper around Path.GetFullPath with additional path handling.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path.</returns>
    /// <exception cref="ArgumentException">Thrown when the path is invalid.</exception>
    public static string NormalizePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return path ?? string.Empty;

        try
        {
            return Path.GetFullPath(path);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            // Fallback for edge cases - use original logic
            return NormalizePathFallback(path);
        }
    }

    /// <summary>
    /// Fallback normalization method for edge cases where Path.GetFullPath fails.
    /// </summary>
    /// <param name="fileName">The filename to normalize.</param>
    /// <returns>The normalized path.</returns>
    private static string NormalizePathFallback(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return fileName;

        var separator = Path.DirectorySeparatorChar;
        var result = new StringBuilder();

        var segments = fileName.Split(['/', '\\'], StringSplitOptions.None);
        var normalizedSegments = new List<string>();

        foreach (var segment in segments)
        {
            switch (segment)
            {
                case "":
                    // Handle empty segments (e.g., leading/trailing slashes)
                    if (normalizedSegments.Count == 0 && fileName.StartsWith(separator))
                        normalizedSegments.Add("");
                    break;
                case ".":
                    // Skip current directory references
                    break;
                case "..":
                    // Handle parent directory references
                    if (normalizedSegments.Count > 0 && normalizedSegments[^1] != "..")
                        normalizedSegments.RemoveAt(normalizedSegments.Count - 1);
                    else if (!Path.IsPathRooted(fileName))
                        normalizedSegments.Add("..");
                    break;
                default:
                    // Add valid segments
                    normalizedSegments.Add(segment);
                    break;
            }
        }

        return string.Join(separator.ToString(), normalizedSegments);
    }

    /// <summary>
    /// Determines whether two file paths refer to the same location after normalization.
    /// </summary>
    /// <param name="path1">The first path.</param>
    /// <param name="path2">The second path.</param>
    /// <returns>True if the paths are equal after normalization; otherwise, false.</returns>
    public static bool ArePathsEqual(string? path1, string? path2)
    {
        if (path1 == path2) return true;
        if (path1 is null || path2 is null) return false;

        return string.Equals(
            NormalizePath(path1),
            NormalizePath(path2),
            StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region File Name Utilities

    /// <summary>
    /// Determines whether fileName1 and fileName2 are equal after normalization of both filenames.
    /// </summary>
    /// <param name="fileName1">The filename.</param>
    /// <param name="fileName2">The filename.</param>
    /// <returns>
    /// 	<see langword="true"/> if the filenames are equal after normalization; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsEqualFileName(string fileName1, string fileName2)
    {
        return string.Equals(NormalizePath(fileName1),
                             NormalizePath(fileName2),
                             StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a valid filename by replacing invalid characters with underscores.
    /// </summary>
    /// <param name="filename">The filename to validate.</param>
    /// <param name="replacement">The character to replace invalid characters with (default: '_').</param>
    /// <returns>A valid filename.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null.</exception>
    public static string GetValidFilename(string filename, char replacement = '_')
    {
        ArgumentNullException.ThrowIfNull(filename);

        if (string.IsNullOrWhiteSpace(filename))
            return "unnamed";

        var invalidChars = Path.GetInvalidFileNameChars();
        var result = new StringBuilder(filename.Length);

        foreach (var c in filename)
        {
            result.Append(Array.IndexOf(invalidChars, c) >= 0 ? replacement : c);
        }

        // Handle filenames ending with periods (not allowed on Windows)
        var validName = result.ToString().TrimEnd('.');
        if (string.IsNullOrEmpty(validName))
            return "unnamed";

        // Add underscores for removed periods
        var periodCount = result.Length - validName.Length;
        return validName + new string(replacement, periodCount);
    }

    /// URL-encodes illegal filename characters.
    /// </summary>
    /// <param name="filename">The filename that may contain illegal characters.</param>
    /// <returns>The filename with illegal characters URL-encoded.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filename"/> is null.</exception>
    public static string UrlEncodeFilename(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        var invalidChars = Path.GetInvalidFileNameChars();
        var result = new StringBuilder(filename.Length * 2); // Estimate for encoding

        foreach (var c in filename)
        {
            if (Array.IndexOf(invalidChars, c) >= 0)
            {
                result.Append(Uri.HexEscape(c));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    #endregion

    #region File Operations

    /// <summary>
    /// Gets file information for all files matching the specified pattern in the given directory.
    /// </summary>
    /// <param name="directoryPath">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern (e.g., "*.txt").</param>
    /// <param name="includeSubdirectories">Whether to include subdirectories in the search.</param>
    /// <returns>A list of FileInfo objects for matching files.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory doesn't exist.</exception>
    public static List<FileInfo> GetFiles(string directoryPath, string searchPattern, bool includeSubdirectories = false)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);
        ArgumentNullException.ThrowIfNull(searchPattern);

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return [.. directory.GetFiles(searchPattern, searchOption)];
    }

    /// <summary>
    /// Asynchronously enumerates files matching the specified pattern in the given directory.
    /// This is more memory-efficient for large directories.
    /// </summary>
    /// <param name="directoryPath">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern (e.g., "*.txt").</param>
    /// <param name="includeSubdirectories">Whether to include subdirectories in the search.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of FileInfo objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory doesn't exist.</exception>
    public static async IAsyncEnumerable<FileInfo> GetFilesAsync(
        string directoryPath,
        string searchPattern,
        bool includeSubdirectories = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);
        ArgumentNullException.ThrowIfNull(searchPattern);

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        await Task.Yield(); // Make method truly async

        foreach (var file in directory.EnumerateFiles(searchPattern, searchOption))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return file;
        }
    }

    /// <summary>
    /// Gets file paths for all files matching the specified pattern in the given directory.
    /// </summary>
    /// <param name="directoryPath">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern (e.g., "*.txt").</param>
    /// <param name="includeSubdirectories">Whether to include subdirectories in the search.</param>
    /// <returns>A list of file paths.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory doesn't exist.</exception>
    public static List<string> GetFilePaths(string directoryPath, string searchPattern, bool includeSubdirectories = false)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);
        ArgumentNullException.ThrowIfNull(searchPattern);

        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return [.. Directory.EnumerateFiles(directoryPath, searchPattern, searchOption)];
    }

    /// <summary>
    /// Asynchronously enumerates files matching the specified pattern in the given directory.
    /// This is more memory-efficient for large directories.
    /// </summary>
    /// <param name="directoryPath">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern (e.g., "*.txt").</param>
    /// <param name="includeSubdirectories">Whether to include subdirectories in the search.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of file paths.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory doesn't exist.</exception>
    public static async IAsyncEnumerable<string> GetFilePathsAsync(
        string directoryPath,
        string searchPattern,
        bool includeSubdirectories = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);
        ArgumentNullException.ThrowIfNull(searchPattern);

        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        await Task.Yield(); // Make method truly async

        foreach (var file in Directory.EnumerateFiles(directoryPath, searchPattern, searchOption))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return file;
        }
    }

    /// <summary>
    /// Safely deletes a file, removing read-only attributes if necessary.
    /// </summary>
    /// <param name="filePath">The path to the file to delete.</param>
    /// <param name="throwOnError">Whether to throw exceptions on error.</param>
    /// <returns>True if the file was successfully deleted or didn't exist; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null.</exception>
    public static bool TryDeleteFile(string filePath, bool throwOnError = false)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        try
        {
            if (!File.Exists(filePath))
                return true;

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.IsReadOnly)
                fileInfo.IsReadOnly = false;

            File.Delete(filePath);
            return true;
        }
        catch when (!throwOnError)
        {
            return false;
        }
    }

    /// <summary>
    /// Asynchronously and safely deletes a file, removing read-only attributes if necessary.
    /// </summary>
    /// <param name="filePath">The path to the file to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the file was successfully deleted or didn't exist; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null.</exception>
    public static async Task<bool> TryDeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        try
        {
            await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                    return;

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.IsReadOnly)
                    fileInfo.IsReadOnly = false;

                File.Delete(filePath);
            }, cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Directory Operations

    /// <summary>
    /// Gets all subdirectories in the specified path.
    /// </summary>
    /// <param name="directoryPath">The directory path to search.</param>
    /// <param name="includeSubdirectories">Whether to recursively include subdirectories.</param>
    /// <returns>A list of subdirectory paths.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null.</exception>
    public static List<string> GetSubdirectories(string directoryPath, bool includeSubdirectories = false)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        if (!Directory.Exists(directoryPath))
            return [];

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return [.. Directory.EnumerateDirectories(directoryPath, "*", searchOption)];
    }

    /// <summary>
    /// Ensures that a directory exists, creating it if necessary.
    /// </summary>
    /// <param name="directoryPath">The directory path to ensure exists.</param>
    /// <returns>True if the directory exists or was created successfully; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null.</exception>
    public static bool EnsureDirectoryExists(string directoryPath)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        try
        {
            Directory.CreateDirectory(directoryPath);
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Determines whether a directory is empty (contains no files or subdirectories).
    /// </summary>
    /// <param name="directoryPath">The directory path to check.</param>
    /// <returns>True if the directory is empty; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory doesn't exist.</exception>
    public static bool IsDirectoryEmpty(string directoryPath)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        // EnumerateFileSystemInfos() returns both files AND subdirectories
        // A directory is empty only if it contains neither files nor subdirectories
        return !directory.EnumerateFileSystemInfos().Any();
    }

    /// <summary>
    /// Recursively deletes a directory and all its contents.
    /// </summary>
    /// <param name="directoryPath">The directory path to delete.</param>
    /// <param name="throwOnError">Whether to throw exceptions on error.</param>
    /// <returns>True if the directory was successfully deleted or didn't exist; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null.</exception>
    public static bool TryDeleteDirectory(string directoryPath, bool throwOnError = false)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        try
        {
            if (!Directory.Exists(directoryPath))
                return true;

            Directory.Delete(directoryPath, recursive: true);
            return true;
        }
        catch when (!throwOnError)
        {
            return false;
        }
    }

    /// <summary>
    /// Asynchronously and safely deletes recursively a directory and all its contents.
    /// </summary>
    /// <param name="directoryPath">The directory path to delete.</param>
    /// <param name="throwOnError">Whether to throw exceptions on error.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the directory was successfully deleted or didn't exist; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null.</exception>
    public static async Task<bool> TryDeleteDirectoryAsync(
        string directoryPath,
        bool throwOnError = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        try
        {
            return await Task.Run(() =>
            {
                if (!Directory.Exists(directoryPath))
                    return true;

                Directory.Delete(directoryPath, recursive: true);
                return true;
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch when (!throwOnError)
        {
            return false;
        }
    }

    /// <summary>
    /// Performs a deep copy of a directory structure.
    /// </summary>
    /// <param name="sourceDirectory">The source directory path.</param>
    /// <param name="destinationDirectory">The destination directory path.</param>
    /// <param name="overwrite">Whether to overwrite existing files.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the source directory doesn't exist.</exception>
    public static async Task CopyDirectoryAsync(
        string sourceDirectory,
        string destinationDirectory,
        bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceDirectory);
        ArgumentNullException.ThrowIfNull(destinationDirectory);

        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");

        if (ArePathsEqual(sourceDirectory, destinationDirectory))
            return;

        await Task.Run(() =>
        {
            EnsureDirectoryExists(destinationDirectory);

            // Copy files
            Parallel.ForEach(
                Directory.EnumerateFiles(sourceDirectory),
                new ParallelOptions { CancellationToken = cancellationToken },
                (filePath, parallelLoopState) =>
                {
                    try
                    {
                        // Check for cancellation before starting file copy
                        cancellationToken.ThrowIfCancellationRequested();

                        var fileName = Path.GetFileName(filePath);
                        var destFilePath = Path.Combine(destinationDirectory, fileName);

                        // For large files, use a copy method that supports periodic cancellation checks
                        var fileInfo = new FileInfo(filePath);
                        if (fileInfo.Length > 10 * 1024 * 1024) // Files larger than 10MB
                        {
                            CopyLargeFileWithCancellation(filePath, destFilePath, overwrite, cancellationToken);
                        }
                        else
                        {
                            File.Copy(filePath, destFilePath, overwrite);
                        }

                        // Check for cancellation after completing file copy
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        // Stop processing new iterations when cancelled
                        parallelLoopState.Stop();
                        throw;
                    }
                });

            // Copy subdirectories
            var tasks = Directory.EnumerateDirectories(sourceDirectory) // IEnumerable<string>
                .Select(dirPath =>                                      // Transform each string
                {
                    var dirName = Path.GetFileName(dirPath);
                    var destDirPath = Path.Combine(destinationDirectory, dirName);
                    // Return a Task
                    return CopyDirectoryAsync(dirPath, destDirPath, overwrite, cancellationToken);
                }); // Result: IEnumerable<Task>

            Task.WaitAll([.. tasks], cancellationToken);
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Copies a large file with periodic cancellation checks for better responsiveness.
    /// </summary>
    /// <param name="sourceFile">The source file path.</param>
    /// <param name="destFile">The destination file path.</param>
    /// <param name="overwrite">Whether to overwrite existing files.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    private static void CopyLargeFileWithCancellation(
        string sourceFile,
        string destFile,
        bool overwrite,
        CancellationToken cancellationToken)
    {
        const int bufferSize = 64 * 1024; // 64KB buffer for efficient I/O
        const int checkInterval = 1024 * 1024; // Check cancellation every 1MB

        using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var destStream = new FileStream(destFile,
            overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write, FileShare.None);

        var buffer = new byte[bufferSize];
        long copiedBytes = 0;
        int bytesRead;

        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            destStream.Write(buffer, 0, bytesRead);
            copiedBytes += bytesRead;

            // Check for cancellation periodically during large file copy
            if (copiedBytes % checkInterval == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }

    /// <summary>
    /// Creates a date-based folder structure for organizing files by date.
    /// </summary>
    /// <param name="baseDirectory">The base directory where date folders will be created.</param>
    /// <param name="startDate">The start date for folder creation.</param>
    /// <param name="endDate">The end date for folder creation.</param>
    /// <param name="createDayFolders">Whether to create individual day folders within month folders.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseDirectory"/> is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the base directory doesn't exist.</exception>
    /// <remarks>
    /// <para>Date folder structure example:</para><pre>
    ///  + 2009-08
    ///    + 2009-08-28
    ///    + 2009-08-29
    ///    + 2009-08-30
    ///    + 2009-08-31
    ///  + 2009-09
    ///    + 2009-09-01
    ///    + 2009-09-02
    ///    + ...
    /// </pre></remarks>
    public static void CreateDateFolders(
        string baseDirectory,
        DateTime startDate,
        DateTime endDate,
        bool createDayFolders = true)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);

        if (!Directory.Exists(baseDirectory))
            throw new DirectoryNotFoundException($"Base directory not found: {baseDirectory}");

        if (endDate < startDate)
            return;

        var currentDate = startDate.Date;
        var processedMonths = new HashSet<string>();

        while (currentDate <= endDate.Date)
        {
            var monthFolder = currentDate.ToString("yyyy-MM");
            var monthPath = Path.Combine(baseDirectory, monthFolder);

            // Create month folder only once
            if (processedMonths.Add(monthFolder))
            {
                EnsureDirectoryExists(monthPath);
            }

            // Create day folder if requested
            if (createDayFolders)
            {
                var dayFolder = currentDate.ToString("yyyy-MM-dd");
                var dayPath = Path.Combine(monthPath, dayFolder);
                EnsureDirectoryExists(dayPath);
            }

            currentDate = currentDate.AddDays(1);
        }
    }

    /// <summary>
    /// Creates a date-based folder structure for organizing files by date.
    /// </summary>
    /// <param name="baseDirectory">The base directory where date folders will be created.</param>
    /// <param name="startDate">The start date for folder creation.</param>
    /// <param name="endDate">The end date for folder creation.</param>
    /// <param name="createDayFolders">Whether to create individual day folders within month folders.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseDirectory"/> is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the base directory doesn't exist.</exception>
    public static async Task CreateDateFoldersAsync(
        string baseDirectory,
        DateTime startDate,
        DateTime endDate,
        bool createDayFolders = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);

        if (!Directory.Exists(baseDirectory))
            throw new DirectoryNotFoundException($"Base directory not found: {baseDirectory}");

        if (endDate < startDate)
            return;

        await Task.Run(() =>
        {
            var currentDate = startDate.Date;
            var processedMonths = new HashSet<string>();

            while (currentDate <= endDate.Date)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var monthFolder = currentDate.ToString("yyyy-MM");
                var monthPath = Path.Combine(baseDirectory, monthFolder);

                // Create month folder only once
                if (processedMonths.Add(monthFolder))
                {
                    EnsureDirectoryExists(monthPath);
                }

                // Create day folder if requested
                if (createDayFolders)
                {
                    var dayFolder = currentDate.ToString("yyyy-MM-dd");
                    var dayPath = Path.Combine(monthPath, dayFolder);
                    EnsureDirectoryExists(dayPath);
                }

                currentDate = currentDate.AddDays(1);
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Search Directory Operations

    /// <summary>
    /// Searches for files matching the specified pattern, with options for hidden files and subdirectories.
    /// </summary>
    /// <param name="directoryPath">The directory to search.</param>
    /// <param name="searchPattern">The file pattern to match.</param>
    /// <param name="includeSubdirectories">Whether to search subdirectories.</param>
    /// <param name="includeHidden">Whether to include hidden files.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of matching file paths.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    public static async Task<List<string>> SearchFilesAsync(
        string directoryPath,
        string searchPattern,
        bool includeSubdirectories = true,
        bool includeHidden = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);
        ArgumentNullException.ThrowIfNull(searchPattern);

        var results = new List<string>();

        await foreach (var file in GetFilesAsync(directoryPath, searchPattern, includeSubdirectories, cancellationToken))
        {
            if (!includeHidden && file.Attributes.HasFlag(FileAttributes.Hidden))
                continue;

            results.Add(file.FullName);
        }

        return results;
    }

    // cancellationToken.ThrowIfCancellationRequested();

    /// <summary>
    /// Searches for files matching multiple patterns simultaneously.
    /// </summary>
    /// <param name="directoryPath">The directory to search.</param>
    /// <param name="searchPatterns">The file patterns to match.</param>
    /// <param name="includeSubdirectories">Whether to search subdirectories.</param>
    /// <param name="includeHidden">Whether to include hidden files.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of matching file paths.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    public static async Task<List<string>> SearchFilesAsync(
        string directoryPath,
        IEnumerable<string> searchPatterns,
        bool includeSubdirectories = true,
        bool includeHidden = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);
        ArgumentNullException.ThrowIfNull(searchPatterns);

        var tasks = searchPatterns.Select(pattern =>
            SearchFilesAsync(directoryPath, pattern, includeSubdirectories, includeHidden, cancellationToken));

        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        return results.SelectMany(r => r).Distinct().ToList();
    }

    #endregion

    #region Directory Size Operations

    /// <summary>
    /// Gets the size of a directory including all subdirectories.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <param name="includeSubdirectories">Whether to include subdirectories in the calculation.</param>
    /// <returns>The total size in bytes.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory doesn't exist.</exception>
    public static long GetDirectorySize(string directoryPath, bool includeSubdirectories = true)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return directory.EnumerateFiles("*", searchOption).Sum(file => file.Length);
    }


    /// <summary>
    /// Gets the size of a directory including all subdirectories.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <param name="includeSubdirectories">Whether to include subdirectories in the calculation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The total size in bytes.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory doesn't exist.</exception>
    public static async Task<long> GetDirectorySizeAsync(
        string directoryPath,
        bool includeSubdirectories = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(directoryPath);

        return await Task.Run(() =>
        {
            var directory = new DirectoryInfo(directoryPath);
            if (!directory.Exists)
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            long totalSize = 0;
            int fileCount = 0;

            foreach (var file in directory.EnumerateFiles("*", searchOption))
            {
                // Check for cancellation periodically
                if (fileCount % 100 == 0) // Every 100 files
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                try
                {
                    totalSize += file.Length;
                    fileCount++;
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip files we can't access, but continue with others
                    continue;
                }
                catch (FileNotFoundException)
                {
                    // File might have been deleted during enumeration
                    continue;
                }
            }

            return totalSize;

        }, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
