//--------------------------------------------------------------------------
// File:    FileSystemManagerUnitTest.cs
// Content: Unit tests for FileSystemManager class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using Xunit;
using AnBo.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnBo.Test;

public class FileSystemManagerUnitTest : IDisposable
{
    #region Test Setup and Cleanup

    private readonly string _testDirectory;
    private readonly List<string> _createdDirectories = new();
    private readonly List<string> _createdFiles = new();

    public FileSystemManagerUnitTest()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileSystemManagerTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDirectory);
        _createdDirectories.Add(_testDirectory);
    }

    public void Dispose()
    {
        // Clean up created files
        foreach (var file in _createdFiles.Where(File.Exists))
        {
            try
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.IsReadOnly)
                    fileInfo.IsReadOnly = false;
                File.Delete(file);
            }
            catch { /* Ignore cleanup errors */ }
        }

        // Clean up created directories
        foreach (var dir in _createdDirectories.Where(Directory.Exists).OrderByDescending(d => d.Length))
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    private string CreateTestFile(string fileName, string content = "test content")
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        File.WriteAllText(filePath, content);
        _createdFiles.Add(filePath);
        return filePath;
    }

    private string CreateTestDirectory(string dirName)
    {
        var dirPath = Path.Combine(_testDirectory, dirName);
        Directory.CreateDirectory(dirPath);
        _createdDirectories.Add(dirPath);
        return dirPath;
    }

    #endregion

    #region Path Operations Tests

    [Fact]
    public void EnsureTrailingSlash_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.EnsureTrailingSlash(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void EnsureTrailingSlash_With_Path_Without_Trailing_Slash_Should_Add_Slash()
    {
        // Arrange
        var input = "C:\\test";

        // Act
        var result = FileSystemManager.EnsureTrailingSlash(input);

        // Assert
        result.Should().Be($"C:\\test{Path.DirectorySeparatorChar}");
    }

    [Fact]
    public void EnsureTrailingSlash_With_Path_With_Trailing_Slash_Should_Return_Same()
    {
        // Arrange
        var input = $"C:\\test{Path.DirectorySeparatorChar}";

        // Act
        var result = FileSystemManager.EnsureTrailingSlash(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void EnsureTrailingSlash_With_Alt_Directory_Separator_Should_Return_Same()
    {
        // Arrange
        var input = $"C:/test{Path.AltDirectorySeparatorChar}";

        // Act
        var result = FileSystemManager.EnsureTrailingSlash(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void EnsureLeadingSlash_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.EnsureLeadingSlash(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void EnsureLeadingSlash_With_Path_Without_Leading_Slash_Should_Add_Slash()
    {
        // Arrange
        var input = "test\\path";

        // Act
        var result = FileSystemManager.EnsureLeadingSlash(input);

        // Assert
        result.Should().Be($"{Path.DirectorySeparatorChar}test\\path");
    }

    [Fact]
    public void EnsureLeadingSlash_With_Path_With_Leading_Slash_Should_Return_Same()
    {
        // Arrange
        var input = $"{Path.DirectorySeparatorChar}test\\path";

        // Act
        var result = FileSystemManager.EnsureLeadingSlash(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void GetAbsolutePath_With_Null_BaseDirectory_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetAbsolutePath(null!, "relative");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetAbsolutePath_With_Empty_BaseDirectory_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetAbsolutePath("", "relative");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetAbsolutePath_With_Null_RelativePath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetAbsolutePath("C:\\base", null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetAbsolutePath_With_Valid_Paths_Should_Return_Combined_Path()
    {
        // Arrange
        var basePath = "C:\\base";
        var relativePath = "sub\\file.txt";

        // Act
        var result = FileSystemManager.GetAbsolutePath(basePath, relativePath);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("base");
        result.Should().Contain("sub");
        result.Should().Contain("file.txt");
    }

    [Fact]
    public void NormalizePath_With_Null_Path_Should_Return_Empty_String()
    {
        // Act
        var result = FileSystemManager.NormalizePath(null);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void NormalizePath_With_Empty_Path_Should_Return_Empty_String()
    {
        // Act
        var result = FileSystemManager.NormalizePath("");

        // Assert
        result.Should().Be("");
    }

    [Fact]
    public void NormalizePath_With_Valid_Path_Should_Return_Normalized_Path()
    {
        // Arrange
        var path = "C:\\test\\..\\normalized";

        // Act
        var result = FileSystemManager.NormalizePath(path);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotContain("..");
    }

    [Fact]
    public void ArePathsEqual_With_Both_Null_Should_Return_True()
    {
        // Act
        var result = FileSystemManager.ArePathsEqual(null, null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ArePathsEqual_With_One_Null_Should_Return_False()
    {
        // Act
        var result1 = FileSystemManager.ArePathsEqual(null, "path");
        var result2 = FileSystemManager.ArePathsEqual("path", null);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Fact]
    public void ArePathsEqual_With_Same_Paths_Should_Return_True()
    {
        // Arrange
        var path1 = "C:\\test\\path";
        var path2 = "C:\\test\\path";

        // Act
        var result = FileSystemManager.ArePathsEqual(path1, path2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ArePathsEqual_With_Different_Case_Should_Return_True()
    {
        // Arrange
        var path1 = "C:\\TEST\\path";
        var path2 = "C:\\test\\PATH";

        // Act
        var result = FileSystemManager.ArePathsEqual(path1, path2);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region File Name Utilities Tests

    [Fact]
    public void IsEqualFileName_With_Same_Normalized_Paths_Should_Return_True()
    {
        // Arrange
        var fileName1 = "C:\\test\\file.txt";
        var fileName2 = "C:\\TEST\\FILE.TXT";

        // Act
        var result = FileSystemManager.IsEqualFileName(fileName1, fileName2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEqualFileName_With_Different_Paths_Should_Return_False()
    {
        // Arrange
        var fileName1 = "C:\\test\\file1.txt";
        var fileName2 = "C:\\test\\file2.txt";

        // Act
        var result = FileSystemManager.IsEqualFileName(fileName1, fileName2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetValidFilename_With_Null_Filename_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetValidFilename(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetValidFilename_With_Empty_Filename_Should_Return_Unnamed()
    {
        // Act
        var result = FileSystemManager.GetValidFilename("");

        // Assert
        result.Should().Be("unnamed");
    }

    [Fact]
    public void GetValidFilename_With_Whitespace_Filename_Should_Return_Unnamed()
    {
        // Act
        var result = FileSystemManager.GetValidFilename("   ");

        // Assert
        result.Should().Be("unnamed");
    }

    [Fact]
    public void GetValidFilename_With_Invalid_Characters_Should_Replace_With_Underscore()
    {
        // Arrange
        var filename = "test<>|file.txt";

        // Act
        var result = FileSystemManager.GetValidFilename(filename);

        // Assert
        result.Should().NotContain("<");
        result.Should().NotContain(">");
        result.Should().NotContain("|");
        result.Should().Contain("_");
    }

    [Fact]
    public void GetValidFilename_With_Custom_Replacement_Should_Use_Custom_Character()
    {
        // Arrange
        var filename = "test<file.txt";
        var replacement = '-';

        // Act
        var result = FileSystemManager.GetValidFilename(filename, replacement);

        // Assert
        result.Should().NotContain("<");
        result.Should().Contain("-");
    }

    [Fact]
    public void GetValidFilename_With_Trailing_Periods_Should_Remove_And_Replace()
    {
        // Arrange
        var filename = "test...";

        // Act
        var result = FileSystemManager.GetValidFilename(filename);

        // Assert
        result.Should().Be("test___");
    }

    [Fact]
    public void UrlEncodeFilename_With_Null_Filename_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.UrlEncodeFilename(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UrlEncodeFilename_With_Valid_Filename_Should_Return_Same()
    {
        // Arrange
        var filename = "validfile.txt";

        // Act
        var result = FileSystemManager.UrlEncodeFilename(filename);

        // Assert
        result.Should().Be(filename);
    }

    [Fact]
    public void UrlEncodeFilename_With_Invalid_Characters_Should_Encode()
    {
        // Arrange
        var filename = "test<file>.txt";

        // Act
        var result = FileSystemManager.UrlEncodeFilename(filename);

        // Assert
        result.Should().NotContain("<");
        result.Should().NotContain(">");
        result.Should().Contain("%");
    }

    #endregion

    #region File Operations Tests

    [Fact]
    public void GetFiles_With_Null_DirectoryPath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetFiles(null!, "*.txt");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetFiles_With_Null_SearchPattern_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetFiles(_testDirectory, null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetFiles_With_NonExistent_Directory_Should_Throw_DirectoryNotFoundException()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act & Assert
        var action = () => FileSystemManager.GetFiles(nonExistentDir, "*.txt");
        action.Should().Throw<DirectoryNotFoundException>();
    }

    [Fact]
    public void GetFiles_With_Valid_Directory_Should_Return_Matching_Files()
    {
        // Arrange
        CreateTestFile("test1.txt");
        CreateTestFile("test2.txt");
        CreateTestFile("test3.doc");

        // Act
        var result = FileSystemManager.GetFiles(_testDirectory, "*.txt");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(f => f.Extension == ".txt");
    }

    [Fact]
    public void GetFiles_With_Subdirectories_Should_Respect_IncludeSubdirectories_Flag()
    {
        // Arrange
        var subDir = CreateTestDirectory("subdir");
        CreateTestFile("root.txt");
        CreateTestFile(Path.Combine("subdir", "sub.txt"));

        // Act
        var resultTopOnly = FileSystemManager.GetFiles(_testDirectory, "*.txt", false);
        var resultWithSubs = FileSystemManager.GetFiles(_testDirectory, "*.txt", true);

        // Assert
        resultTopOnly.Should().HaveCount(1);
        resultWithSubs.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetFilesAsync_With_Valid_Directory_Should_Return_Matching_Files()
    {
        // Arrange
        CreateTestFile("async1.txt");
        CreateTestFile("async2.txt");

        // Act
        var result = new List<FileInfo>();
        await foreach (var file in FileSystemManager.GetFilesAsync(_testDirectory, "*.txt"))
        {
            result.Add(file);
        }

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(f => f.Extension == ".txt");
    }

    [Fact]
    public async Task GetFilesAsync_With_Cancellation_Should_Respect_CancellationToken()
    {
        // Arrange
        CreateTestFile("cancel1.txt");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var action = async () =>
        {
            await foreach (var file in FileSystemManager.GetFilesAsync(_testDirectory, "*.txt", cancellationToken: cts.Token))
            {
                // Should not reach here
            }
        };

        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void GetFilePaths_With_Valid_Directory_Should_Return_File_Paths()
    {
        // Arrange
        CreateTestFile("path1.txt");
        CreateTestFile("path2.txt");

        // Act
        var result = FileSystemManager.GetFilePaths(_testDirectory, "*.txt");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(path => path.EndsWith(".txt"));
    }

    [Fact]
    public async Task GetFilePathsAsync_With_Valid_Directory_Should_Return_File_Paths()
    {
        // Arrange
        CreateTestFile("pathAsync1.txt");
        CreateTestFile("pathAsync2.txt");

        // Act
        var result = new List<string>();
        await foreach (var path in FileSystemManager.GetFilePathsAsync(_testDirectory, "*.txt"))
        {
            result.Add(path);
        }

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(path => path.EndsWith(".txt"));
    }

    [Fact]
    public void TryDeleteFile_With_Null_FilePath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.TryDeleteFile(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryDeleteFile_With_NonExistent_File_Should_Return_True()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

        // Act
        var result = FileSystemManager.TryDeleteFile(nonExistentFile);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TryDeleteFile_With_Existing_File_Should_Delete_And_Return_True()
    {
        // Arrange
        var filePath = CreateTestFile("delete.txt");

        // Act
        var result = FileSystemManager.TryDeleteFile(filePath);

        // Assert
        result.Should().BeTrue();
        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public void TryDeleteFile_With_ReadOnly_File_Should_Remove_ReadOnly_And_Delete()
    {
        // Arrange
        var filePath = CreateTestFile("readonly.txt");
        var fileInfo = new FileInfo(filePath);
        fileInfo.IsReadOnly = true;

        // Act
        var result = FileSystemManager.TryDeleteFile(filePath);

        // Assert
        result.Should().BeTrue();
        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public async Task TryDeleteFileAsync_With_Existing_File_Should_Delete_And_Return_True()
    {
        // Arrange
        var filePath = CreateTestFile("deleteAsync.txt");

        // Act
        var result = await FileSystemManager.TryDeleteFileAsync(filePath);

        // Assert
        result.Should().BeTrue();
        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public async Task TryDeleteFileAsync_With_Cancellation_Should_Throw_OperationCanceledException()
    {
        // Arrange
        var filePath = CreateTestFile("cancelDelete.txt");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var action = async () => await FileSystemManager.TryDeleteFileAsync(filePath, cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region Directory Operations Tests

    [Fact]
    public void GetSubdirectories_With_Null_DirectoryPath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetSubdirectories(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetSubdirectories_With_NonExistent_Directory_Should_Return_Empty_List()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act
        var result = FileSystemManager.GetSubdirectories(nonExistentDir);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetSubdirectories_With_Valid_Directory_Should_Return_Subdirectories()
    {
        // Arrange
        CreateTestDirectory("sub1");
        CreateTestDirectory("sub2");

        // Act
        var result = FileSystemManager.GetSubdirectories(_testDirectory);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(dir => dir.Contains("sub1"));
        result.Should().Contain(dir => dir.Contains("sub2"));
    }

    [Fact]
    public void EnsureDirectoryExists_With_Null_DirectoryPath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.EnsureDirectoryExists(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void EnsureDirectoryExists_With_NonExistent_Directory_Should_Create_And_Return_True()
    {
        // Arrange
        var newDir = Path.Combine(_testDirectory, "newdir");

        // Act
        var result = FileSystemManager.EnsureDirectoryExists(newDir);

        // Assert
        result.Should().BeTrue();
        Directory.Exists(newDir).Should().BeTrue();
        _createdDirectories.Add(newDir);
    }

    [Fact]
    public void EnsureDirectoryExists_With_Existing_Directory_Should_Return_True()
    {
        // Act
        var result = FileSystemManager.EnsureDirectoryExists(_testDirectory);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDirectoryEmpty_With_Null_DirectoryPath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.IsDirectoryEmpty(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsDirectoryEmpty_With_NonExistent_Directory_Should_Throw_DirectoryNotFoundException()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act & Assert
        var action = () => FileSystemManager.IsDirectoryEmpty(nonExistentDir);
        action.Should().Throw<DirectoryNotFoundException>();
    }

    [Fact]
    public void IsDirectoryEmpty_With_Empty_Directory_Should_Return_True()
    {
        // Arrange
        var emptyDir = CreateTestDirectory("empty");

        // Act
        var result = FileSystemManager.IsDirectoryEmpty(emptyDir);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDirectoryEmpty_With_Directory_Containing_Files_Should_Return_False()
    {
        // Arrange
        CreateTestFile("notempty.txt");

        // Act
        var result = FileSystemManager.IsDirectoryEmpty(_testDirectory);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDirectoryEmpty_With_Directory_Containing_Subdirectories_Should_Return_False()
    {
        // Arrange
        var parentDir = CreateTestDirectory("parent");
        var subDir = Path.Combine(parentDir, "child");
        Directory.CreateDirectory(subDir);
        _createdDirectories.Add(subDir);

        // Act
        var result = FileSystemManager.IsDirectoryEmpty(parentDir);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TryDeleteDirectory_With_Null_DirectoryPath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.TryDeleteDirectory(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryDeleteDirectory_With_NonExistent_Directory_Should_Return_True()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act
        var result = FileSystemManager.TryDeleteDirectory(nonExistentDir);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TryDeleteDirectory_With_Existing_Directory_Should_Delete_And_Return_True()
    {
        // Arrange
        var dirToDelete = CreateTestDirectory("todelete");
        CreateTestFile(Path.Combine("todelete", "file.txt"));

        // Act
        var result = FileSystemManager.TryDeleteDirectory(dirToDelete);

        // Assert
        result.Should().BeTrue();
        Directory.Exists(dirToDelete).Should().BeFalse();
    }

    [Fact]
    public async Task TryDeleteDirectoryAsync_With_Existing_Directory_Should_Delete_And_Return_True()
    {
        // Arrange
        var dirToDelete = CreateTestDirectory("todeleteAsync");
        CreateTestFile(Path.Combine("todeleteAsync", "file.txt"));

        // Act
        var result = await FileSystemManager.TryDeleteDirectoryAsync(dirToDelete);

        // Assert
        result.Should().BeTrue();
        Directory.Exists(dirToDelete).Should().BeFalse();
    }

    [Fact]
    public async Task CopyDirectoryAsync_With_Null_SourceDirectory_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = async () => await FileSystemManager.CopyDirectoryAsync(null!, "dest");
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CopyDirectoryAsync_With_Null_DestinationDirectory_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = async () => await FileSystemManager.CopyDirectoryAsync(_testDirectory, null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CopyDirectoryAsync_With_NonExistent_Source_Should_Throw_DirectoryNotFoundException()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");
        var destDir = Path.Combine(_testDirectory, "dest");

        // Act & Assert
        var action = async () => await FileSystemManager.CopyDirectoryAsync(nonExistentDir, destDir);
        await action.Should().ThrowAsync<DirectoryNotFoundException>();
    }

    [Fact]
    public async Task CopyDirectoryAsync_With_Same_Source_And_Destination_Should_Return_Without_Error()
    {
        // Act & Assert
        var action = async () => await FileSystemManager.CopyDirectoryAsync(_testDirectory, _testDirectory);
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CopyDirectoryAsync_Should_Copy_Files_And_Subdirectories()
    {
        // Arrange
        var sourceDir = CreateTestDirectory("source");
        var subDir = Path.Combine(sourceDir, "subdir");
        Directory.CreateDirectory(subDir);
        _createdDirectories.Add(subDir);
        
        var sourceFile = Path.Combine(sourceDir, "file.txt");
        var subFile = Path.Combine(subDir, "subfile.txt");
        File.WriteAllText(sourceFile, "content");
        File.WriteAllText(subFile, "subcontent");
        _createdFiles.Add(sourceFile);
        _createdFiles.Add(subFile);

        var destDir = Path.Combine(_testDirectory, "destination");

        // Act
        await FileSystemManager.CopyDirectoryAsync(sourceDir, destDir);

        // Assert
        Directory.Exists(destDir).Should().BeTrue();
        File.Exists(Path.Combine(destDir, "file.txt")).Should().BeTrue();
        Directory.Exists(Path.Combine(destDir, "subdir")).Should().BeTrue();
        File.Exists(Path.Combine(destDir, "subdir", "subfile.txt")).Should().BeTrue();

        _createdDirectories.Add(destDir);
        _createdDirectories.Add(Path.Combine(destDir, "subdir"));
        _createdFiles.Add(Path.Combine(destDir, "file.txt"));
        _createdFiles.Add(Path.Combine(destDir, "subdir", "subfile.txt"));
    }

    [Fact]
    public void CreateDateFolders_With_Null_BaseDirectory_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.CreateDateFolders(null!, DateTime.Now, DateTime.Now);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateDateFolders_With_NonExistent_BaseDirectory_Should_Throw_DirectoryNotFoundException()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act & Assert
        var action = () => FileSystemManager.CreateDateFolders(nonExistentDir, DateTime.Now, DateTime.Now);
        action.Should().Throw<DirectoryNotFoundException>();
    }

    [Fact]
    public void CreateDateFolders_With_EndDate_Before_StartDate_Should_Return_Without_Creating_Folders()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 15);
        var endDate = new DateTime(2025, 1, 10);

        // Act
        FileSystemManager.CreateDateFolders(_testDirectory, startDate, endDate);

        // Assert - Should not create any date folders
        var subdirs = Directory.GetDirectories(_testDirectory);
        subdirs.Should().NotContain(dir => dir.Contains("2025-01"));
    }

    [Fact]
    public void CreateDateFolders_Should_Create_Month_And_Day_Folders()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 15);
        var endDate = new DateTime(2025, 1, 17);

        // Act
        FileSystemManager.CreateDateFolders(_testDirectory, startDate, endDate);

        // Assert
        var monthDir = Path.Combine(_testDirectory, "2025-01");
        Directory.Exists(monthDir).Should().BeTrue();
        Directory.Exists(Path.Combine(monthDir, "2025-01-15")).Should().BeTrue();
        Directory.Exists(Path.Combine(monthDir, "2025-01-16")).Should().BeTrue();
        Directory.Exists(Path.Combine(monthDir, "2025-01-17")).Should().BeTrue();

        _createdDirectories.Add(monthDir);
        _createdDirectories.Add(Path.Combine(monthDir, "2025-01-15"));
        _createdDirectories.Add(Path.Combine(monthDir, "2025-01-16"));
        _createdDirectories.Add(Path.Combine(monthDir, "2025-01-17"));
    }

    [Fact]
    public void CreateDateFolders_Without_Day_Folders_Should_Create_Only_Month_Folders()
    {
        // Arrange
        var startDate = new DateTime(2025, 2, 15);
        var endDate = new DateTime(2025, 2, 17);

        // Act
        FileSystemManager.CreateDateFolders(_testDirectory, startDate, endDate, createDayFolders: false);

        // Assert
        var monthDir = Path.Combine(_testDirectory, "2025-02");
        Directory.Exists(monthDir).Should().BeTrue();
        Directory.GetDirectories(monthDir).Should().BeEmpty();

        _createdDirectories.Add(monthDir);
    }

    [Fact]
    public async Task CreateDateFoldersAsync_Should_Create_Folders_Asynchronously()
    {
        // Arrange
        var startDate = new DateTime(2025, 3, 15);
        var endDate = new DateTime(2025, 3, 16);

        // Act
        await FileSystemManager.CreateDateFoldersAsync(_testDirectory, startDate, endDate);

        // Assert
        var monthDir = Path.Combine(_testDirectory, "2025-03");
        Directory.Exists(monthDir).Should().BeTrue();
        Directory.Exists(Path.Combine(monthDir, "2025-03-15")).Should().BeTrue();
        Directory.Exists(Path.Combine(monthDir, "2025-03-16")).Should().BeTrue();

        _createdDirectories.Add(monthDir);
        _createdDirectories.Add(Path.Combine(monthDir, "2025-03-15"));
        _createdDirectories.Add(Path.Combine(monthDir, "2025-03-16"));
    }

    #endregion

    #region Search Directory Operations Tests

    [Fact]
    public async Task SearchFilesAsync_With_Null_DirectoryPath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = async () => await FileSystemManager.SearchFilesAsync(null!, "*.txt");
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SearchFilesAsync_With_Null_SearchPattern_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = async () => await FileSystemManager.SearchFilesAsync(_testDirectory, searchPattern: null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SearchFilesAsync_Should_Find_Matching_Files()
    {
        // Arrange
        CreateTestFile("search1.txt");
        CreateTestFile("search2.doc");
        var subDir = CreateTestDirectory("searchsub");
        CreateTestFile(Path.Combine("searchsub", "search3.txt"));

        // Act
        var result = await FileSystemManager.SearchFilesAsync(_testDirectory, "*.txt");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(path => path.EndsWith(".txt"));
    }

    [Fact]
    public async Task SearchFilesAsync_With_Multiple_Patterns_Should_Find_All_Matching_Files()
    {
        // Arrange
        CreateTestFile("multi1.txt");
        CreateTestFile("multi2.doc");
        CreateTestFile("multi3.pdf");
        var patterns = new[] { "*.txt", "*.doc" };

        // Act
        var result = await FileSystemManager.SearchFilesAsync(_testDirectory, patterns);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(path => path.EndsWith(".txt"));
        result.Should().Contain(path => path.EndsWith(".doc"));
    }

    [Fact]
    public async Task SearchFilesAsync_With_Hidden_Files_Should_Respect_IncludeHidden_Flag()
    {
        // Arrange
        var hiddenFile = CreateTestFile("hidden.txt");
        var fileInfo = new FileInfo(hiddenFile);
        fileInfo.Attributes |= FileAttributes.Hidden;

        CreateTestFile("visible.txt");

        // Act
        var resultWithoutHidden = await FileSystemManager.SearchFilesAsync(_testDirectory, "*.txt", includeHidden: false);
        var resultWithHidden = await FileSystemManager.SearchFilesAsync(_testDirectory, "*.txt", includeHidden: true);

        // Assert
        resultWithoutHidden.Should().HaveCount(1);
        resultWithHidden.Should().HaveCount(2);
    }

    #endregion

    #region Directory Size Operations Tests

    [Fact]
    public void GetDirectorySize_With_Null_DirectoryPath_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => FileSystemManager.GetDirectorySize(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetDirectorySize_With_NonExistent_Directory_Should_Throw_DirectoryNotFoundException()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act & Assert
        var action = () => FileSystemManager.GetDirectorySize(nonExistentDir);
        action.Should().Throw<DirectoryNotFoundException>();
    }

    [Fact]
    public void GetDirectorySize_With_Empty_Directory_Should_Return_Zero()
    {
        // Arrange
        var emptyDir = CreateTestDirectory("empty");

        // Act
        var result = FileSystemManager.GetDirectorySize(emptyDir);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetDirectorySize_With_Files_Should_Return_Total_Size()
    {
        // Arrange
        var sizeDir = CreateTestDirectory("sizetest");
        var file1 = Path.Combine(sizeDir, "file1.txt");
        var file2 = Path.Combine(sizeDir, "file2.txt");
        File.WriteAllText(file1, "12345"); // 5 bytes
        File.WriteAllText(file2, "123456789"); // 9 bytes
        _createdFiles.Add(file1);
        _createdFiles.Add(file2);

        // Act
        var result = FileSystemManager.GetDirectorySize(sizeDir);

        // Assert
        result.Should().Be(14);
    }

    [Fact]
    public void GetDirectorySize_Without_Subdirectories_Should_Not_Include_Subdirectory_Files()
    {
        // Arrange
        var sizeDir = CreateTestDirectory("sizetest2");
        var subDir = Path.Combine(sizeDir, "sub");
        Directory.CreateDirectory(subDir);
        _createdDirectories.Add(subDir);

        var rootFile = Path.Combine(sizeDir, "root.txt");
        var subFile = Path.Combine(subDir, "sub.txt");
        File.WriteAllText(rootFile, "12345"); // 5 bytes
        File.WriteAllText(subFile, "123456789"); // 9 bytes
        _createdFiles.Add(rootFile);
        _createdFiles.Add(subFile);

        // Act
        var result = FileSystemManager.GetDirectorySize(sizeDir, includeSubdirectories: false);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public async Task GetDirectorySizeAsync_Should_Return_Correct_Size()
    {
        // Arrange
        var sizeDir = CreateTestDirectory("asyncsizetest");
        var file = Path.Combine(sizeDir, "async.txt");
        File.WriteAllText(file, "async content"); // 13 bytes
        _createdFiles.Add(file);

        // Act
        var result = await FileSystemManager.GetDirectorySizeAsync(sizeDir);

        // Assert
        result.Should().Be(13);
    }

    [Fact]
    public async Task GetDirectorySizeAsync_With_Cancellation_Should_Throw_OperationCanceledException()
    {
        // Arrange
        var sizeDir = CreateTestDirectory("cancelsizetest");
        CreateTestFile(Path.Combine("cancelsizetest", "file.txt"));
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var action = async () => await FileSystemManager.GetDirectorySizeAsync(sizeDir, cancellationToken: cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion
}