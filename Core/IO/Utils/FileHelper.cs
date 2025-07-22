//--------------------------------------------------------------------------
// Class:	FileHelper
// Content:	Implementation of File helper class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core
{
    /// <summary>
	/// A file and directory helper class.
	/// </summary>
	public static class FileHelper
    {

        #region Private Members

        private readonly static char[] m_Separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };

        #endregion

        #region Public constants

        // Are these constants defined anywhere in the .NET Framework?  They were
        // taken from PathTooLongException.Message, which is this:
        //
        // "The specified path, file name, or both are too long. 
        // The fully qualified file name must be less than 260 characters, 
        // and the directory name must be less than 248 characters."

        /// <summary>Maximum file name length (259).</summary>
        public const Int32 MAXIMUM_FILE_NAME_LENGTH = 259;

        /// <summary>Maximum folder name length  (247).</summary>
        public const Int32 MAXIMUM_FOLDER_NAME_LENGTH = 247;

        #endregion

        #region Public static Methods

        /// <summary>
        /// Add a slash "\" to end of input if not exists.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>input string that ends with a slash "\" char.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="input"/> is <see langword="null"/>.</exception>
        public static string AppendSlash(string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.EndsWith(@"\"))
                return input;

            return input + @"\";
        }

        /// <summary>
        /// Add a slash "\" to front of input if not exists.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>input string that starts with a slash "\" char.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="input"/> is <see langword="null"/>.</exception>
        public static string AppendFrontSlash(string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.StartsWith(@"\")) { return input; }

            return @"\" + input;
        }

        /// <summary>
        /// Get a list of files in specified path.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <param name="mask">The file mask.</param>
        /// <returns>The list of files in specified path.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is empty.</exception>
        public static List<FileInfo> GetFileInfoList(string path, string mask)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(path);
            ArgumentNullException.ThrowIfNullOrEmpty(mask);

            List<FileInfo> retVal = new List<FileInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fInfoList = dirInfo.GetFiles(mask);
            retVal.AddRange(fInfoList);
            return retVal;
        }

        /// <summary>
        /// Get a list of files in specified path.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <param name="mask">The file mask.</param>
        /// <param name="subDir">If set to <see langword="true"/> all subdirectories are also processed.</param>
        /// <returns>The list of files in specified path (files in sub directories are included if subDir is set to true.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is empty.</exception>
        public static List<FileInfo> GetFileInfoList(string path, string mask, bool subDir)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(path);
            ArgumentNullException.ThrowIfNullOrEmpty(mask);

            List<FileInfo> retVal;

            if (subDir == false)
                retVal = GetFileInfoList(path, mask);
            else
            {
                List<string> dirList = new List<string> { AppendSlash(path) };
                dirList.AddRange(GetSubDirectories(path, true));

                retVal = new List<FileInfo>();
                foreach (string dir in dirList)
                    retVal.AddRange(GetFileInfoList(dir, mask));
            }
            return retVal;
        }

        /// <summary>
        /// Get a list of files in specified path.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <param name="mask">The file mask.</param>
        /// <returns>The list of files in specified path.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is empty.</exception>
        public static List<string> GetFiles(string path, string mask)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(path);
            ArgumentNullException.ThrowIfNullOrEmpty(mask);

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fInfoList = dirInfo.GetFiles(mask);

            return fInfoList.Select(fi => Path.Combine(path, fi.Name)).ToList();
        }

        /// <summary>
        /// Get a list of files in specified path.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <param name="mask">The file mask.</param>
        /// <param name="subDir">If set to <see langword="true"/> all subdirectories are also processed.</param>
        /// <returns>The list of files in specified path (files in sub directories are included if subDir is set to true.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="path"/> or <paramref name="mask"/> is empty.</exception>
        public static List<string> GetFiles(string path, string mask, bool subDir)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(path);
            ArgumentNullException.ThrowIfNullOrEmpty(mask);

            List<string> retVal = new List<string>();

            if (subDir == false)
                retVal = GetFiles(path, mask);
            else
            {
                var dirList = new List<string> { AppendSlash(path) };
                dirList.AddRange(GetSubDirectories(path, true));

                foreach (var dir in dirList)
                    retVal.AddRange(GetFiles(dir, mask));
            }
            return retVal;
        }

        /// <summary>
        /// Get a list of subdirectories in the specified path.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>List of sub directories in the specified path.</returns>
        /// <param name="subDir">If set to <see langword="true"/> the complete subdirectory hierachy is returned.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> is <see langword="null"/>.</exception>
        public static List<string> GetSubDirectories(string path, bool subDir)
        {
            ArgumentNullException.ThrowIfNull(path);

            if (!Directory.Exists(path)) return new List<string>();

            List<string> retVal = new List<string>();

            DirectoryInfo curDirInfo = new DirectoryInfo(path);
            DirectoryInfo[] dirInCurDir = curDirInfo.GetDirectories();

            foreach (DirectoryInfo di in dirInCurDir)
            {
                string folder = AppendSlash(AppendSlash(path) + di);
                retVal.Add(folder);

                if (!subDir) continue;
                List<string> dirInSubDir = GetSubDirectories(folder, true);
                retVal.AddRange(dirInSubDir);
            }

            return retVal;
        }

        /// <summary>
        /// Determines whether the specified path is an URL.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified path is an URL; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> is <see langword="null"/>.</exception>
        public static bool IsUrl(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            return path.IndexOf("://", StringComparison.Ordinal) > 0;
        }

        /// <summary>
        /// Combines multi path segments to a single path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="paths">The path segments.</param>
        /// <returns>A string containing the combined paths.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="basePath"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Is thrown if <paramref name="basePath"/> or any item of <paramref name="paths"/> contains invalid path characters.
        /// </exception>
        public static string CombinePath(string basePath, params string[] paths)
        {
            #region PreConditions
            ArgumentNullException.ThrowIfNull(basePath);

            if (basePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException("Argument error: Contains invalid path characters (value = {1}", nameof(basePath));
            #endregion

            paths.Foreach(path =>
            {
                ArgumentNullException.ThrowIfNull(path);
                if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                {
                    throw new ArgumentException("Argument error: Contains invalid path characters.", nameof(basePath));
                }
            });

            return paths.Aggregate(basePath, Path.Combine);
        }

        /// <summary>
        /// Converts a given absolute path and a given base path to a path that leads
        /// from the base path to the absoulte path. (as a relative path)
        /// </summary>
        /// <param name="baseDirectoryPath">The base directory</param>
        /// <param name="absPath">The absolute path.</param>
        /// <returns>The path of absPath relative to baseDirectoryPath.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if <paramref name="baseDirectoryPath"/> or <paramref name="absPath"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Is thrown if <paramref name="baseDirectoryPath"/> or <paramref name="absPath"/> is empty.
        /// </exception>
        public static string GetRelativePath(string baseDirectoryPath, string absPath)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(baseDirectoryPath);
            ArgumentNullException.ThrowIfNullOrEmpty(absPath);

            if (IsUrl(absPath) || IsUrl(baseDirectoryPath))
            {
                return absPath;
            }

            baseDirectoryPath = NormalizePath(baseDirectoryPath);
            absPath = NormalizePath(absPath);

            string[] bPath = baseDirectoryPath.Split(m_Separators);
            string[] aPath = absPath.Split(m_Separators);
            int indx = 0;

            for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
            {
                if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
                    break;
            }

            if (indx == 0)
            {
                return absPath;
            }

            StringBuilder result = new StringBuilder();

            if (indx != bPath.Length)
            {
                for (int i = indx; i < bPath.Length; ++i)
                {
                    result.Append("..");
                    result.Append(Path.DirectorySeparatorChar);
                }
            }
            result.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
            return result.ToString();
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
        /// Creates the directory. CreateDirectory creates the directory only if it not exist.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> is <see langword="null"/>.</exception>
        public static void CreateDirectory(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                info.Create();
            }
        }

        /// <summary>
        /// Deletes the directory. DeleteDirectory checks if the directory exist before deleting all files an subdirectories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="path"/> is <see langword="null"/>.</exception>
        public static void DeleteDirectory(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                foreach (DirectoryInfo info2 in info.GetDirectories())
                {
                    DeleteDirectory(info2.FullName);
                }
                foreach (FileInfo info3 in info.GetFiles())
                {
                    DeleteFile(info3.FullName);
                }
                info.Delete(true);
            }
        }

        /// <summary>
        /// Deletes a file. DeleteFile checks if the file exist and removes the ReadOnly-Attribute before deleting the file.
        /// </summary>
        /// <param name="pathName">The file path.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="pathName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="pathName"/> is empty.</exception>
        public static void DeleteFile(string pathName)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(pathName);

            FileInfo info = new FileInfo(pathName);

            if (!info.Exists) return;

            info.Attributes &= ~FileAttributes.ReadOnly;
            info.Delete();
        }

        /// <summary>
        /// Determines whether a directory is empty.
        /// </summary>
        /// <param name="dirName">Name of the directory.</param>
        /// <returns>
        /// 	<see langword="true"/> if directory is empty; otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="dirName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="dirName"/> is empty.</exception>
        public static bool IsDirectoryEmpty(string dirName)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(dirName);

            FileSystemInfo[] fileSystemInfos = new DirectoryInfo(dirName).GetFileSystemInfos();
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if ((fileSystemInfos == null) || (fileSystemInfos.Length == 0x0))
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Performes a deep directory copy (copies all subdirectories and files)
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <param name="overwrite">if set to <see langword="true"/> overwrites existing files in the destination directory.</param>
        /// <exception cref="ArgumentNullException">
        ///		Is thrown if <paramref name="sourceDirectory"/> or <paramref name="destinationDirectory"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        ///		Is thrown if <paramref name="sourceDirectory"/> or <paramref name="destinationDirectory"/> is empty.</exception>
        public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(sourceDirectory);
            ArgumentNullException.ThrowIfNullOrEmpty(destinationDirectory);

            if (sourceDirectory == destinationDirectory)
                return;

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            foreach (string fileName in Directory.GetFiles(sourceDirectory))
            {
                File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
            }
            foreach (string directoryName in Directory.GetDirectories(sourceDirectory))
            {
                DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
            }
        }

        #region SearchDirectory

        /// <summary>
        /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path <paramref name="directory"/> and all subdirectories
        /// (if <paramref name="searchSubdirectories"/> is true). 
        /// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
        /// </summary>
        /// <param name="directory">The search directory.</param>
        /// <param name="filemask">The file mask used during search.</param>
        /// <param name="searchSubdirectories">If <see langword="true"/> search will also process subdirectories</param>
        /// <param name="ignoreHidden">If set to <see langword="true"/> hidden files will be ingored.</param>
        /// <returns>The search result list</returns>
        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
        {
            List<string> collection = new List<string>();
            SearchDirectory(directory, filemask, collection, searchSubdirectories, ignoreHidden);
            return collection;
        }

        /// <summary>
        /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path <paramref name="directory"/> and all subdirectories
        /// (if <paramref name="searchSubdirectories"/> is true). 
        /// Hidden files will be ignored.
        /// </summary>
        /// <param name="directory">The search directory.</param>
        /// <param name="filemask">The file mask used during search.</param>
        /// <param name="searchSubdirectories">If <see langword="true"/> search will also processes all subdirectories.</param>
        /// <returns>The search result list</returns>
        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
        {
            return SearchDirectory(directory, filemask, searchSubdirectories, true);
        }

        /// <summary>
        /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path <paramref name="directory"/> and all subdirectories.
        /// Hidden files will be ignored.
        /// </summary>
        /// <param name="directory">The search directory.</param>
        /// <param name="filemask">The file mask used during search.</param>
        /// <returns>The search result list</returns>
        public static List<string> SearchDirectory(string directory, string filemask)
        {
            return SearchDirectory(directory, filemask, true, true);
        }

        /// <summary>
        /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path <paramref name="directory"/> and all subdirectories
        /// (if <paramref name="searchSubdirectories"/> is true). 
        /// The found files are added to the ICollection{string} <paramref name="collection"/>.
        /// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
        /// </summary>
        /// <param name="directory">The search directory.</param>
        /// <param name="filemask">The file mask used during search.</param>
        /// <param name="collection">The search result collection</param>
        /// <param name="searchSubdirectories">If <see langword="true"/> search will also process subdirectories</param>
        /// <param name="ignoreHidden">If set to <see langword="true"/> hidden files will be ingored.</param>
        private static void SearchDirectory(string directory, string filemask, ICollection<string> collection, bool searchSubdirectories, bool ignoreHidden)
        {
            // If Directory.GetFiles() searches the 8.3 name as well as the full name so if the filemask is
            // "*.pdf" it will return "Template.pdf~"
            try
            {
                bool isExtMatch = Regex.IsMatch(filemask, @"^\*\..{3}$");
                string? ext = null;
                string[] file = Directory.GetFiles(directory, filemask);
                if (isExtMatch) ext = filemask.Remove(0, 1);

                foreach (string f in file)
                {
                    if (ignoreHidden && (File.GetAttributes(f) & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }
                    if (isExtMatch && Path.GetExtension(f) != ext) continue;

                    collection.Add(f);
                }

                if (searchSubdirectories)
                {
                    string[] dir = Directory.GetDirectories(directory);
                    foreach (string d in dir)
                    {
                        if (ignoreHidden && (File.GetAttributes(d) & FileAttributes.Hidden) == FileAttributes.Hidden)
                            continue;
                        SearchDirectory(d, filemask, collection, true, ignoreHidden);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore exception when access to a directory is denied.
            }
        }

        #endregion

        /// <summary>
        /// Gets the normalized version of <paramref name="fileName"/>.
        /// Slashes are replaced with backslashes, backreferences "." and ".." are 'evaluated'.
        /// </summary>
        /// <param name="fileName">The filename that should be normalized.</param>
        /// <returns>The normalized version of <paramref name="fileName"/>.</returns>
        public static string NormalizePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            int i;

            bool isWeb = false;

            for (i = 0; i < fileName.Length; i++)
            {
                if (fileName[i] == '/' || fileName[i] == '\\')
                    break;
                if (fileName[i] == ':')
                {
                    if (i > 1)
                        isWeb = true;
                    break;
                }
            }

            char outputSeparator = isWeb ? '/' : Path.DirectorySeparatorChar;

            StringBuilder result = new StringBuilder();
            if (isWeb == false && fileName.StartsWith(@"\\") || fileName.StartsWith("//"))
            {
                i = 2;
                result.Append(outputSeparator);
            }
            else
            {
                i = 0;
            }

            int segmentStartPos = i;
            for (; i <= fileName.Length; i++)
            {
                if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\')
                {
                    int segmentLength = i - segmentStartPos;
                    switch (segmentLength)
                    {
                        case 0:
                            // ignore empty segment (if not in web mode)
                            // On unix, don't ignore empty segment if i==0
                            if (isWeb || (i == 0 && Environment.OSVersion.Platform == PlatformID.Unix))
                            {
                                result.Append(outputSeparator);
                            }
                            break;
                        case 1:
                            // ignore /./ segment, but append other one-letter segments
                            if (fileName[segmentStartPos] != '.')
                            {
                                if (result.Length > 0)
                                    result.Append(outputSeparator);

                                result.Append(fileName[segmentStartPos]);
                            }
                            break;
                        case 2:
                            if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.')
                            {
                                // remove previous segment
                                int j;

                                for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--) { }

                                if (j > 0)
                                {
                                    result.Length = j;
                                }
                                break;
                            }

                            // append normal segment
                            goto default;

                        default:
                            if (result.Length > 0)
                                result.Append(outputSeparator);

                            result.Append(fileName, segmentStartPos, segmentLength);
                            break;
                    }
                    segmentStartPos = i + 1; // remember start position for next segment
                }
            }

            if (isWeb == false)
            {
                if (result.Length > 0 && result[result.Length - 1] == outputSeparator)
                {
                    result.Length -= 1;
                }
                if (result.Length == 2 && result[1] == ':')
                {
                    result.Append(outputSeparator);
                }
            }
            return result.ToString();
        }

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
        /// Gets the valid filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>A valid filename.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="filename"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="filename"/> is empty.</exception>
        public static string GetValidFilename(string filename)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(filename);

            List<char> list = new List<char>();
            list.AddRange(Path.GetInvalidPathChars());
            list.Add(Path.VolumeSeparatorChar);
            list.Add(Path.DirectorySeparatorChar);
            list.Add(Path.AltDirectorySeparatorChar);
            list.Add(Path.PathSeparator);

            filename = list.Aggregate(filename, (current, ch) => current.Replace(ch, '_'));

            if (filename.EndsWith(".", StringComparison.Ordinal))
            {
                int length = filename.Length;
                filename = filename.TrimEnd(new[] { '.' });
                int num2 = filename.Length;
                filename = filename + new string('_', length - num2);
            }

            return filename;
        }

        /// <summary>
        /// Encodes illegal characters in a file name.
        /// </summary>
        /// <param name="fileName">File name that may contain illegal characters.</param>
        /// <returns><paramref name="fileName" /> with any illegal characters hex-encoded.</returns>
        public static String EncodeIllegalFileNameChars(string fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            String sEncodedFileName = Path.GetInvalidFileNameChars()
                .Aggregate(fileName, (current, cInvalidFileNameChar) => current.Replace(cInvalidFileNameChar.ToString(), Uri.HexEscape(cInvalidFileNameChar)));

            return (sEncodedFileName);
        }

        /// <summary>
        /// Creates a date folder structure in the file system.
        /// </summary>
        /// <param name="baseFolder">The folder where the date folder structure should be created</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
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
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="baseFolder"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="baseFolder"/> is empty.</exception>
        public static void CreateDateFolder(string baseFolder, DateTime startDate, DateTime endDate)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(baseFolder);

            if (Directory.Exists(baseFolder))
            {
                if (endDate < startDate)
                    return;

                DateTime dt = startDate.Date;

                do
                {
                    string monthFolder = dt.ToString("yyyy-MM");
                    string dayFolder = dt.ToString("yyyy-MM-dd");

                    string monthPath = Path.Combine(baseFolder, monthFolder);
                    string dayPath = Path.Combine(monthFolder, dayFolder);

                    if (Directory.Exists(monthPath) == false)
                        Directory.CreateDirectory(monthPath);

                    if (Directory.Exists(dayPath) == false)
                        Directory.CreateDirectory(dayPath);

                    dt = dt.AddDays(1.0);
                } while (dt <= endDate.Date);

            }
        }

        #endregion
    }

}
