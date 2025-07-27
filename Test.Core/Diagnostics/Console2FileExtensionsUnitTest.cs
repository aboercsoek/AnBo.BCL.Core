//--------------------------------------------------------------------------
// File:    Console2FileExtensionsUnitTest.cs
// Content: Unit tests for Console2FileExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Text;

namespace AnBo.Test;

[Collection("Sequential")]
public class Console2FileExtensionsUnitTest : IDisposable
{
    #region Test Setup and Teardown

    private readonly string _testDirectory;
    private readonly List<string> _createdFiles;
    private readonly TextWriter _originalConsoleOut;

    public Console2FileExtensionsUnitTest()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"Console2FileExtensionsTest_{Guid.NewGuid():N}");
        _createdFiles = new List<string>();
        _originalConsoleOut = Console.Out;

        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }
    }

    public void Dispose()
    {
        // Restore original console output
        Console.SetOut(_originalConsoleOut);

        // Clean up test files and directories
        foreach (var file in _createdFiles)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        // Clean up any rotated files
        try
        {
            if (Directory.Exists(_testDirectory))
            {
                var files = Directory.GetFiles(_testDirectory, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
                Directory.Delete(_testDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    private string GetTestFilePath(string fileName)
    {
        var path = Path.Combine(_testDirectory, fileName);
        _createdFiles.Add(path);
        return path;
    }

    private static string ReadSharedFile(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream);
        return streamReader.ReadToEnd();
    }

    #endregion

    #region CreateTimestampedPath Tests

    [Fact]
    public void CreateTimestampedPath_Should_Add_Timestamp_To_Filename()
    {
        // Arrange
        var basePath = "test.log";
        var expectedPattern = @"test-\d{8}-\d{6}\.log$";

        // Act
        var result = Console2FileExtensions.CreateTimestampedPath(basePath);

        // Assert
        result.Should().MatchRegex(expectedPattern);
        result.Should().NotBe(basePath);
    }

    [Fact]
    public void CreateTimestampedPath_Should_Handle_Custom_Format()
    {
        // Arrange
        var basePath = "app.log";
        var customFormat = "yyyy-MM-dd_HH-mm-ss";
        var expectedPattern = @"app-\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}\.log$";

        // Act
        var result = Console2FileExtensions.CreateTimestampedPath(basePath, customFormat);

        // Assert
        result.Should().MatchRegex(expectedPattern);
    }

    [Fact]
    public void CreateTimestampedPath_Should_Handle_Path_With_Directory()
    {
        // Arrange
        var basePath = Path.Combine("logs", "sub", "app.log");
        var expectedDirectoryPart = Path.Combine("logs", "sub");

        // Act
        var result = Console2FileExtensions.CreateTimestampedPath(basePath);

        // Assert
        result.Should().StartWith(expectedDirectoryPart);
        result.Should().MatchRegex(@"app-\d{8}-\d{6}\.log$");
    }

    [Fact]
    public void CreateTimestampedPath_Should_Handle_File_Without_Extension()
    {
        // Arrange
        var basePath = "logfile";

        // Act
        var result = Console2FileExtensions.CreateTimestampedPath(basePath);

        // Assert
        result.Should().MatchRegex(@"logfile-\d{8}-\d{6}$");
    }

    [Fact]
    public void CreateTimestampedPath_Should_Handle_Empty_Directory()
    {
        // Arrange
        var basePath = "test.log";

        // Act
        var result = Console2FileExtensions.CreateTimestampedPath(basePath);

        // Assert
        result.Should().MatchRegex(@"test-\d{8}-\d{6}\.log$");
        Path.GetDirectoryName(result).Should().BeEmpty();
    }

    [Fact]
    public void CreateTimestampedPath_Should_Throw_For_Null_BasePath()
    {
        // Act & Assert
        var action = () => Console2FileExtensions.CreateTimestampedPath(null!);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("basePath")
            .WithMessage("Base path cannot be null or empty.*");
    }

    [Fact]
    public void CreateTimestampedPath_Should_Throw_For_Empty_BasePath()
    {
        // Act & Assert
        var action = () => Console2FileExtensions.CreateTimestampedPath(string.Empty);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("basePath")
            .WithMessage("Base path cannot be null or empty.*");
    }

    [Fact]
    public void CreateTimestampedPath_Should_Throw_For_Whitespace_BasePath()
    {
        // Act & Assert
        var action = () => Console2FileExtensions.CreateTimestampedPath("   ");
        action.Should().Throw<ArgumentException>()
            .WithParameterName("basePath")
            .WithMessage("Base path cannot be null or empty.*");
    }

    [Fact]
    public void CreateTimestampedPath_Should_Generate_Unique_Paths()
    {
        // Arrange
        var basePath = "test.log";

        // Act
        var path1 = Console2FileExtensions.CreateTimestampedPath(basePath);
        Thread.Sleep(1100); // Ensure different timestamp (seconds precision)
        var path2 = Console2FileExtensions.CreateTimestampedPath(basePath);

        // Assert
        path1.Should().NotBe(path2);
    }

    #endregion

    #region CreateRotatingPath Tests

    [Fact]
    public void CreateRotatingPath_Should_Return_BasePath_For_NonExistent_File()
    {
        // Arrange
        var basePath = GetTestFilePath("nonexistent.log");
        var maxSize = 1024;

        // Act
        var result = Console2FileExtensions.CreateRotatingPath(basePath, maxSize);

        // Assert
        result.Should().Be(basePath);
    }

    [Fact]
    public void CreateRotatingPath_Should_Return_BasePath_For_Small_File()
    {
        // Arrange
        var basePath = GetTestFilePath("small.log");
        var content = "Small content";
        var maxSize = 1024;

        File.WriteAllText(basePath, content);

        // Act
        var result = Console2FileExtensions.CreateRotatingPath(basePath, maxSize);

        // Assert
        result.Should().Be(basePath);
        File.Exists(basePath).Should().BeTrue();
    }

    [Fact]
    public void CreateRotatingPath_Should_Rotate_Large_File()
    {
        // Arrange
        var basePath = GetTestFilePath("large.log");
        var content = new string('A', 2048); // Create large content
        var maxSize = 1024;

        File.WriteAllText(basePath, content);

        // Act
        var result = Console2FileExtensions.CreateRotatingPath(basePath, maxSize);

        // Assert
        result.Should().Be(basePath);
        
        // Original file should be moved to .1
        var rotatedFile = Path.ChangeExtension(basePath, ".1.log");
        File.Exists(rotatedFile).Should().BeTrue();
        
        // Base file should not exist or be empty after rotation
        if (File.Exists(basePath))
        {
            new FileInfo(basePath).Length.Should().Be(0);
        }
    }

    [Fact]
    public void CreateRotatingPath_Should_Handle_Multiple_Rotations()
    {
        // Arrange
        var basePath = GetTestFilePath("multi_rotate.log");
        var content = new string('B', 2048);
        var maxSize = 1024;
        var maxFiles = 3;

        // Create base file
        File.WriteAllText(basePath, content);

        // First rotation
        Console2FileExtensions.CreateRotatingPath(basePath, maxSize, maxFiles);

        // Create new content and rotate again
        File.WriteAllText(basePath, content);
        Console2FileExtensions.CreateRotatingPath(basePath, maxSize, maxFiles);

        // Act - Third rotation
        File.WriteAllText(basePath, content);
        var result = Console2FileExtensions.CreateRotatingPath(basePath, maxSize, maxFiles);

        // Assert
        result.Should().Be(basePath);
        File.Exists(Path.ChangeExtension(basePath, ".1.log")).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".2.log")).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".3.log")).Should().BeTrue();
    }

    [Fact]
    public void CreateRotatingPath_Should_Delete_Oldest_File_When_Limit_Reached()
    {
        // Arrange
        var basePath = GetTestFilePath("delete_oldest.log");
        var content = new string('C', 2048);
        var maxSize = 1024;
        var maxFiles = 2;

        // Create files manually to simulate existing rotation
        File.WriteAllText(basePath, content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".1.log"), content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".2.log"), content);

        // Act
        var result = Console2FileExtensions.CreateRotatingPath(basePath, maxSize, maxFiles);

        // Assert
        result.Should().Be(basePath);
        File.Exists(Path.ChangeExtension(basePath, ".1.log")).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".2.log")).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".3.log")).Should().BeFalse();
    }

    [Fact]
    public void CreateRotatingPath_Should_Throw_For_Null_BasePath()
    {
        // Act & Assert
        var action = () => Console2FileExtensions.CreateRotatingPath(null!, 1024);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("basePath")
            .WithMessage("Base path cannot be null or empty.*");
    }

    [Fact]
    public void CreateRotatingPath_Should_Throw_For_Empty_BasePath()
    {
        // Act & Assert
        var action = () => Console2FileExtensions.CreateRotatingPath(string.Empty, 1024);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("basePath")
            .WithMessage("Base path cannot be null or empty.*");
    }

    [Fact]
    public void CreateRotatingPath_Should_Throw_For_Zero_MaxSize()
    {
        // Arrange
        var basePath = GetTestFilePath("zero_size.log");

        // Act & Assert
        var action = () => Console2FileExtensions.CreateRotatingPath(basePath, 0);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("maxSizeBytes")
            .WithMessage("Max size must be greater than zero.*");
    }

    [Fact]
    public void CreateRotatingPath_Should_Throw_For_Negative_MaxSize()
    {
        // Arrange
        var basePath = GetTestFilePath("negative_size.log");

        // Act & Assert
        var action = () => Console2FileExtensions.CreateRotatingPath(basePath, -1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("maxSizeBytes")
            .WithMessage("Max size must be greater than zero.*");
    }

    [Fact]
    public void CreateRotatingPath_Should_Throw_For_Zero_MaxFiles()
    {
        // Arrange
        var basePath = GetTestFilePath("zero_files.log");

        // Act & Assert
        var action = () => Console2FileExtensions.CreateRotatingPath(basePath, 1024, 0);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("maxFiles")
            .WithMessage("Max files must be greater than zero.*");
    }

    [Fact]
    public void CreateRotatingPath_Should_Throw_For_Negative_MaxFiles()
    {
        // Arrange
        var basePath = GetTestFilePath("negative_files.log");

        // Act & Assert
        var action = () => Console2FileExtensions.CreateRotatingPath(basePath, 1024, -1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("maxFiles")
            .WithMessage("Max files must be greater than zero.*");
    }

    #endregion

    #region CreateTimestamped Factory Method Tests

    [Fact]
    public void CreateTimestamped_Should_Create_Console2File_With_Timestamped_Path()
    {
        // Arrange
        var basePath = GetTestFilePath("timestamped_factory.log");

        // Act
        using var redirect = Console2FileExtensions.CreateTimestamped(basePath);

        // Assert
        redirect.Should().NotBeNull();
        redirect.IsActive.Should().BeTrue();
        redirect.FilePath.Should().MatchRegex(@"timestamped_factory-\d{8}-\d{6}\.log$");
        redirect.FilePath.Should().NotBe(Path.GetFullPath(basePath));
    }

    [Fact]
    public void CreateTimestamped_Should_Create_Functional_Redirection()
    {
        // Arrange
        var basePath = GetTestFilePath("functional_timestamped.log");
        var testMessage = "Timestamped redirection test";

        // Act
        using (var redirect = Console2FileExtensions.CreateTimestamped(basePath))
        {
            Console.WriteLine(testMessage);
            redirect.Flush();
        }

        // Assert
        var timestampedFiles = Directory.GetFiles(_testDirectory, "functional_timestamped-*.log");
        timestampedFiles.Should().HaveCount(1);

        var content = File.ReadAllText(timestampedFiles[0]);
        content.Should().Contain(testMessage);
    }

    [Fact]
    public void CreateTimestamped_Should_Use_Custom_Format()
    {
        // Arrange
        var basePath = GetTestFilePath("custom_format.log");
        var customFormat = "yyyy-MM-dd";

        // Act
        using var redirect = Console2FileExtensions.CreateTimestamped(basePath, customFormat);

        // Assert
        redirect.FilePath.Should().MatchRegex(@"custom_format-\d{4}-\d{2}-\d{2}\.log$");
    }

    [Fact]
    public void CreateTimestamped_Should_Throw_For_Invalid_BasePath()
    {
        // Act & Assert
        var action = () => Console2FileExtensions.CreateTimestamped(null!);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateTimestamped_Should_Create_Directory_If_Not_Exists()
    {
        // Arrange
        var subDir = Path.Combine(_testDirectory, "sub", "nested");
        var basePath = Path.Combine(subDir, "auto_create.log");

        // Act
        using var redirect = Console2FileExtensions.CreateTimestamped(basePath);

        // Assert
        Directory.Exists(subDir).Should().BeTrue();
        redirect.IsActive.Should().BeTrue();
    }

    #endregion

    #region CreateRotating Factory Method Tests

    [Fact]
    public void CreateRotating_Should_Create_Console2File_With_Rotation()
    {
        // Arrange
        var basePath = GetTestFilePath("rotating_factory.log");
        var maxSize = 1024;

        // Act
        using var redirect = Console2FileExtensions.CreateRotating(basePath, maxSize);

        // Assert
        redirect.Should().NotBeNull();
        redirect.IsActive.Should().BeTrue();
        redirect.FilePath.Should().Be(Path.GetFullPath(basePath));
    }

    [Fact]
    public void CreateRotating_Should_Create_Functional_Redirection()
    {
        // Arrange
        var basePath = GetTestFilePath("functional_rotating.log");
        var testMessage = "Rotating redirection test";
        var maxSize = 1024;

        // Act
        using (var redirect = Console2FileExtensions.CreateRotating(basePath, maxSize))
        {
            Console.WriteLine(testMessage);
            redirect.Flush();
        }

        // Assert
        File.Exists(basePath).Should().BeTrue();
        var content = File.ReadAllText(basePath);
        content.Should().Contain(testMessage);
    }

    [Fact]
    public void CreateRotating_Should_Handle_File_Rotation()
    {
        // Arrange
        var basePath = GetTestFilePath("rotation_test.log");
        var largeContent = new string('D', 2048);
        var maxSize = 1024;

        File.WriteAllText(basePath, largeContent);

        // Act
        using var redirect = Console2FileExtensions.CreateRotating(basePath, maxSize);

        // Assert
        redirect.IsActive.Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".1.log")).Should().BeTrue();
    }

    [Fact]
    public void CreateRotating_Should_Use_Custom_MaxFiles()
    {
        // Arrange
        var basePath = GetTestFilePath("custom_max_files.log");
        var maxSize = 1024;
        var maxFiles = 5;

        // Act
        using var redirect = Console2FileExtensions.CreateRotating(basePath, maxSize, maxFiles);

        // Assert
        redirect.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CreateRotating_Should_Throw_For_Invalid_Parameters()
    {
        // Act & Assert
        var action1 = () => Console2FileExtensions.CreateRotating(null!, 1024);
        action1.Should().Throw<ArgumentException>();

        var action2 = () => Console2FileExtensions.CreateRotating("test.log", 0);
        action2.Should().Throw<ArgumentOutOfRangeException>();

        var action3 = () => Console2FileExtensions.CreateRotating("test.log", 1024, 0);
        action3.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region CreateTimestampedRotating Factory Method Tests

    [Fact]
    public void CreateTimestampedRotating_Should_Combine_Timestamping_And_Rotation()
    {
        // Arrange
        var basePath = GetTestFilePath("combined.log");
        var maxSize = 1024;
        var maxFiles = 3;

        // Act
        using var redirect = Console2FileExtensions.CreateTimestampedRotating(basePath, maxSize, maxFiles);

        // Assert
        redirect.Should().NotBeNull();
        redirect.IsActive.Should().BeTrue();
        redirect.FilePath.Should().MatchRegex(@"combined-\d{8}-\d{6}\.log$");
    }

    [Fact]
    public void CreateTimestampedRotating_Should_Create_Functional_Redirection()
    {
        // Arrange
        var basePath = GetTestFilePath("functional_combined.log");
        var testMessage = "Combined redirection test";
        var maxSize = 1024;
        var maxFiles = 3;

        // Act
        using (var redirect = Console2FileExtensions.CreateTimestampedRotating(basePath, maxSize, maxFiles))
        {
            Console.WriteLine(testMessage);
            redirect.Flush();
        }

        // Assert
        var timestampedFiles = Directory.GetFiles(_testDirectory, "functional_combined-*.log");
        timestampedFiles.Should().HaveCount(1);

        var content = File.ReadAllText(timestampedFiles[0]);
        content.Should().Contain(testMessage);
    }

    [Fact]
    public void CreateTimestampedRotating_Should_Use_Custom_Format()
    {
        // Arrange
        var basePath = GetTestFilePath("combined_custom.log");
        var maxSize = 1024;
        var maxFiles = 3;
        var customFormat = "yyyy-MM-dd_HH";

        // Act
        using var redirect = Console2FileExtensions.CreateTimestampedRotating(basePath, maxSize, maxFiles, customFormat);

        // Assert
        redirect.FilePath.Should().MatchRegex(@"combined_custom-\d{4}-\d{2}-\d{2}_\d{2}\.log$");
    }

    #endregion

    #region GetRotationInfo Tests

    [Fact]
    public void GetRotationInfo_Should_Return_Empty_For_Null_Path()
    {
        // Act
        var result = Console2FileExtensions.GetRotationInfo(null!);

        // Assert
        result.FileCount.Should().Be(0);
        result.TotalSize.Should().Be(0);
        result.OldestFile.Should().BeNull();
        result.NewestFile.Should().BeNull();
    }

    [Fact]
    public void GetRotationInfo_Should_Return_Empty_For_Empty_Path()
    {
        // Act
        var result = Console2FileExtensions.GetRotationInfo(string.Empty);

        // Assert
        result.FileCount.Should().Be(0);
        result.TotalSize.Should().Be(0);
        result.OldestFile.Should().BeNull();
        result.NewestFile.Should().BeNull();
    }

    [Fact]
    public void GetRotationInfo_Should_Return_Empty_For_NonExistent_File()
    {
        // Arrange
        var basePath = GetTestFilePath("nonexistent_info.log");

        // Act
        var result = Console2FileExtensions.GetRotationInfo(basePath);

        // Assert
        result.FileCount.Should().Be(0);
        result.TotalSize.Should().Be(0);
        result.OldestFile.Should().BeNull();
        result.NewestFile.Should().BeNull();
    }

    [Fact]
    public void GetRotationInfo_Should_Return_Info_For_Single_File()
    {
        // Arrange
        var basePath = GetTestFilePath("single_info.log");
        var content = "Single file content";
        File.WriteAllText(basePath, content);

        // Act
        var result = Console2FileExtensions.GetRotationInfo(basePath);

        // Assert
        result.FileCount.Should().Be(1);
        result.TotalSize.Should().BeGreaterThan(0);
        result.OldestFile.Should().Be(basePath);
        result.NewestFile.Should().Be(basePath);
    }

    [Fact]
    public void GetRotationInfo_Should_Return_Info_For_Multiple_Rotated_Files()
    {
        // Arrange
        var basePath = GetTestFilePath("multi_info.log");
        var content1 = "Base file content";
        var content2 = "Rotated file 1 content";
        var content3 = "Rotated file 2 content";

        File.WriteAllText(basePath, content1);
        File.WriteAllText(Path.ChangeExtension(basePath, ".1.log"), content2);
        File.WriteAllText(Path.ChangeExtension(basePath, ".2.log"), content3);

        // Act
        var result = Console2FileExtensions.GetRotationInfo(basePath);

        // Assert
        result.FileCount.Should().Be(3);
        result.TotalSize.Should().Be(content1.Length + content2.Length + content3.Length);
        result.OldestFile.Should().Be(Path.ChangeExtension(basePath, ".2.log"));
        result.NewestFile.Should().Be(basePath);
    }

    [Fact]
    public void GetRotationInfo_Should_Handle_Gaps_In_Rotation_Numbers()
    {
        // Arrange
        var basePath = GetTestFilePath("gaps_info.log");
        var content = "Content";

        File.WriteAllText(basePath, content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".1.log"), content);
        // Skip .2.log to create a gap
        File.WriteAllText(Path.ChangeExtension(basePath, ".3.log"), content);

        // Act
        var result = Console2FileExtensions.GetRotationInfo(basePath);

        // Assert
        result.FileCount.Should().Be(2); // Should stop at the gap
        result.TotalSize.Should().Be(content.Length * 2);
        result.OldestFile.Should().Be(Path.ChangeExtension(basePath, ".1.log"));
        result.NewestFile.Should().Be(basePath);
    }

    #endregion

    #region CleanupRotatedFiles Tests

    [Fact]
    public void CleanupRotatedFiles_Should_Return_Zero_For_Null_Path()
    {
        // Act
        var result = Console2FileExtensions.CleanupRotatedFiles(null!, 5);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CleanupRotatedFiles_Should_Return_Zero_For_Empty_Path()
    {
        // Act
        var result = Console2FileExtensions.CleanupRotatedFiles(string.Empty, 5);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CleanupRotatedFiles_Should_Return_Zero_For_Zero_MaxFiles()
    {
        // Arrange
        var basePath = GetTestFilePath("zero_cleanup.log");

        // Act
        var result = Console2FileExtensions.CleanupRotatedFiles(basePath, 0);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CleanupRotatedFiles_Should_Return_Zero_For_Negative_MaxFiles()
    {
        // Arrange
        var basePath = GetTestFilePath("negative_cleanup.log");

        // Act
        var result = Console2FileExtensions.CleanupRotatedFiles(basePath, -1);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CleanupRotatedFiles_Should_Delete_Files_Beyond_Limit()
    {
        // Arrange
        var basePath = GetTestFilePath("cleanup_test.log");
        var content = "Content";
        var maxFiles = 3;

        // Create files that should be kept
        File.WriteAllText(basePath, content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".1.log"), content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".2.log"), content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".3.log"), content);

        // Create files that should be deleted
        File.WriteAllText(Path.ChangeExtension(basePath, ".4.log"), content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".5.log"), content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".6.log"), content);

        // Act
        var deletedCount = Console2FileExtensions.CleanupRotatedFiles(basePath, maxFiles);

        // Assert
        deletedCount.Should().Be(3);
        
        // Verify remaining files
        File.Exists(basePath).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".1.log")).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".2.log")).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".3.log")).Should().BeTrue();
        
        // Verify deleted files
        File.Exists(Path.ChangeExtension(basePath, ".4.log")).Should().BeFalse();
        File.Exists(Path.ChangeExtension(basePath, ".5.log")).Should().BeFalse();
        File.Exists(Path.ChangeExtension(basePath, ".6.log")).Should().BeFalse();
    }

    [Fact]
    public void CleanupRotatedFiles_Should_Return_Zero_When_No_Files_To_Delete()
    {
        // Arrange
        var basePath = GetTestFilePath("no_cleanup.log");
        var content = "Content";
        var maxFiles = 5;

        File.WriteAllText(basePath, content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".1.log"), content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".2.log"), content);

        // Act
        var deletedCount = Console2FileExtensions.CleanupRotatedFiles(basePath, maxFiles);

        // Assert
        deletedCount.Should().Be(0);
    }

    [Fact]
    public void CleanupRotatedFiles_Should_Handle_Gaps_In_File_Numbers()
    {
        // Arrange
        var basePath = GetTestFilePath("gaps_cleanup.log");
        var content = "Content";
        var maxFiles = 2;

        File.WriteAllText(basePath, content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".1.log"), content);
        // Skip .2.log and .3.log to create gaps
        File.WriteAllText(Path.ChangeExtension(basePath, ".4.log"), content);
        File.WriteAllText(Path.ChangeExtension(basePath, ".5.log"), content);

        // Act
        var deletedCount = Console2FileExtensions.CleanupRotatedFiles(basePath, maxFiles);

        // Assert
        deletedCount.Should().Be(2); // Should delete .4.log and .5.log
        File.Exists(Path.ChangeExtension(basePath, ".4.log")).Should().BeFalse();
        File.Exists(Path.ChangeExtension(basePath, ".5.log")).Should().BeFalse();
    }

    [Fact]
    public void CleanupRotatedFiles_Should_Handle_IO_Exceptions_Gracefully()
    {
        // Arrange
        var basePath = GetTestFilePath("io_exception_cleanup.log");
        var content = "Content";
        var maxFiles = 1;

        File.WriteAllText(basePath, content);
        var fileToMakeReadOnly = Path.ChangeExtension(basePath, ".2.log");
        File.WriteAllText(fileToMakeReadOnly, content);
        
        try
        {
            // Make file read-only to simulate IO exception
            File.SetAttributes(fileToMakeReadOnly, FileAttributes.ReadOnly);

            // Act
            var deletedCount = Console2FileExtensions.CleanupRotatedFiles(basePath, maxFiles);

            // Assert
            deletedCount.Should().Be(0); // Should return 0 due to exception handling
        }
        finally
        {
            // Clean up
            File.SetAttributes(fileToMakeReadOnly, FileAttributes.Normal);
            File.Delete(fileToMakeReadOnly);
        }
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Full_Lifecycle_Integration_Test_With_Timestamping()
    {
        // Arrange
        var basePath = GetTestFilePath("integration_timestamp.log");
        var testMessages = new[] { "Start", "Middle", "End" };

        // Act
        using (var redirect = Console2FileExtensions.CreateTimestamped(basePath))
        {
            redirect.IsActive.Should().BeTrue();
            
            foreach (var message in testMessages)
            {
                Console.WriteLine(message);
            }
            
            redirect.Flush();
        }

        // Assert
        var timestampedFiles = Directory.GetFiles(_testDirectory, "integration_timestamp-*.log");
        timestampedFiles.Should().HaveCount(1);

        var content = File.ReadAllText(timestampedFiles[0]);
        foreach (var message in testMessages)
        {
            content.Should().Contain(message);
        }
    }

    [Fact]
    public void Full_Lifecycle_Integration_Test_With_Rotation()
    {
        // Arrange
        var basePath = GetTestFilePath("integration_rotation.log");
        var largeContent = new string('E', 2048);
        var maxSize = 1024;

        // Pre-create a large file
        File.WriteAllText(basePath, largeContent);

        // Act
        using (var redirect = Console2FileExtensions.CreateRotating(basePath, maxSize))
        {
            redirect.IsActive.Should().BeTrue();
            Console.WriteLine("New content after rotation");
            redirect.Flush();
        }

        // Assert
        File.Exists(basePath).Should().BeTrue();
        File.Exists(Path.ChangeExtension(basePath, ".1.log")).Should().BeTrue();

        var newContent = File.ReadAllText(basePath);
        newContent.Should().Contain("New content after rotation");

        var rotatedContent = File.ReadAllText(Path.ChangeExtension(basePath, ".1.log"));
        rotatedContent.Should().Contain(largeContent);
    }

    [Fact]
    public void Multiple_Extensions_Should_Work_Together()
    {
        // Arrange
        var basePath1 = GetTestFilePath("multi1.log");
        var basePath2 = GetTestFilePath("multi2.log");

        // Act
        var timestampedPath = Console2FileExtensions.CreateTimestampedPath(basePath1);
        var rotationInfo = Console2FileExtensions.GetRotationInfo(basePath2);

        using var timestamped = Console2FileExtensions.CreateTimestamped(basePath1);
        using var rotating = Console2FileExtensions.CreateRotating(basePath2, 1024);

        Console.WriteLine("Test from timestamped");
        Console.WriteLine("Test from rotating");

        timestamped.Flush();
        rotating.Flush();

        // Assert
        timestampedPath.Should().MatchRegex(@"multi1-\d{8}-\d{6}\.log$");
        rotationInfo.FileCount.Should().Be(0); // No files exist yet

        timestamped.IsActive.Should().BeTrue();
        rotating.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Extensions_Should_Work_With_Async_Operations()
    {
        // Arrange
        var basePath = GetTestFilePath("async_test.log");
        var tasks = new List<Task>();
        var testDirectory = Path.GetDirectoryName(basePath)!;
        var fileName = Path.GetFileNameWithoutExtension(basePath);
        var fileExtension = Path.GetExtension(basePath);

        // Act
        for (int i = 0; i < 5; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(() =>
            {
                var taskFileName = Path.Combine(testDirectory, $"{fileName}-{taskId}{fileExtension}");
                using (var redirect = Console2FileExtensions.CreateTimestamped(taskFileName))
                {
                    //lock (Globals.LockingObject)
                    //{
                        Console.WriteLine($"Async task {taskId} message");
                    //}
                    redirect.Flush();
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        var timestampedFiles = Directory.GetFiles(_testDirectory, "async_test*.log");
        timestampedFiles.Should().HaveCount(5);

        foreach (var file in timestampedFiles)
        {
            var content = ReadSharedFile(file);
            content.Should().MatchRegex(@"Async task \d+ message");
        }
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public void Extensions_Should_Handle_Very_Long_Paths()
    {
        // Arrange
        var longDirectory = Path.Combine(_testDirectory, new string('L', 100));
        var longFileName = new string('F', 100) + ".log";
        var longPath = Path.Combine(longDirectory, longFileName);

        Directory.CreateDirectory(longDirectory);

        // Act & Assert
        var action1 = () => Console2FileExtensions.CreateTimestampedPath(longPath);
        action1.Should().NotThrow();

        var action2 = () => Console2FileExtensions.CreateRotatingPath(longPath, 1024);
        action2.Should().NotThrow();
    }

    [Fact]
    public void Extensions_Should_Handle_Special_Characters_In_Path()
    {
        // Arrange
        var specialPath = GetTestFilePath("file with spaces & special chars.log");

        // Act & Assert
        var action1 = () => Console2FileExtensions.CreateTimestampedPath(specialPath);
        action1.Should().NotThrow();

        var action2 = () => Console2FileExtensions.CreateRotatingPath(specialPath, 1024);
        action2.Should().NotThrow();

        var action3 = () => Console2FileExtensions.GetRotationInfo(specialPath);
        action3.Should().NotThrow();

        var action4 = () => Console2FileExtensions.CleanupRotatedFiles(specialPath, 5);
        action4.Should().NotThrow();
    }

    [Fact]
    public void Extensions_Should_Handle_Unicode_Paths()
    {
        // Arrange
        var unicodePath = GetTestFilePath("测试文件.log");

        // Act & Assert
        var action1 = () => Console2FileExtensions.CreateTimestampedPath(unicodePath);
        action1.Should().NotThrow();

        var action2 = () => Console2FileExtensions.CreateRotatingPath(unicodePath, 1024);
        action2.Should().NotThrow();
    }

    [Fact]
    public void Extensions_Should_Handle_Network_Paths()
    {
        // Arrange
        var networkPath = @"\\server\share\file.log";

        // Act & Assert - These should not throw even if the path is inaccessible
        var action1 = () => Console2FileExtensions.CreateTimestampedPath(networkPath);
        action1.Should().NotThrow();

        var action2 = () => Console2FileExtensions.CreateRotatingPath(networkPath, 1024);
        action2.Should().NotThrow();

        var action3 = () => Console2FileExtensions.GetRotationInfo(networkPath);
        action3.Should().NotThrow();

        var action4 = () => Console2FileExtensions.CleanupRotatedFiles(networkPath, 5);
        action4.Should().NotThrow();
    }

    #endregion

}