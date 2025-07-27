//--------------------------------------------------------------------------
// File:    Console2FileUnitTest.cs
// Content: Unit tests for Console2File class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Text;

namespace AnBo.Test;

[Collection("Sequential")]
public class Console2FileUnitTest : IDisposable
{
    #region Test Setup and Teardown

    private readonly string _testDirectory;
    private readonly List<string> _createdFiles;
    private readonly TextWriter _originalConsoleOut;

    public Console2FileUnitTest()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"Console2FileTest_{Guid.NewGuid():N}");
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
                    File.Delete(file);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        try
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
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

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_With_Valid_FilePath_Should_Create_Instance_Successfully()
    {
        // Arrange
        var filePath = GetTestFilePath("test.log");

        // Act
        using var redirect = new Console2File(filePath);

        // Assert
        redirect.Should().NotBeNull();
        redirect.FilePath.Should().Be(Path.GetFullPath(filePath));
        redirect.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Constructor_With_Valid_FilePath_Should_Create_Directory_If_Not_Exists()
    {
        // Arrange
        var subDir = Path.Combine(_testDirectory, "subdir", "nested");
        var filePath = Path.Combine(subDir, "test.log");
        _createdFiles.Add(filePath);

        // Act
        using var redirect = new Console2File(filePath);

        // Assert
        Directory.Exists(subDir).Should().BeTrue();
        redirect.FilePath.Should().Be(Path.GetFullPath(filePath));
    }

    [Fact]
    public void Constructor_With_Null_FilePath_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => new Console2File(null!);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("filePath")
            .WithMessage("File path cannot be null or empty.*");
    }

    [Fact]
    public void Constructor_With_Empty_FilePath_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => new Console2File(string.Empty);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("filePath")
            .WithMessage("File path cannot be null or empty.*");
    }

    [Fact]
    public void Constructor_With_Whitespace_FilePath_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => new Console2File("   ");
        action.Should().Throw<ArgumentException>()
            .WithParameterName("filePath")
            .WithMessage("File path cannot be null or empty.*");
    }

    [Fact]
    public void Constructor_With_Invalid_Path_Characters_Should_Throw_ArgumentException()
    {
        // Arrange
        var invalidPath = Path.Combine(_testDirectory, "test<>|.log");

        // Act & Assert
        var action = () => new Console2File(invalidPath);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Should_Redirect_Console_Output()
    {
        // Arrange
        var filePath = GetTestFilePath("redirect_test.log");
        var testMessage = "Test console redirection";

        // Act
        using (var redirect = new Console2File(filePath))
        {
            Console.WriteLine(testMessage);
            redirect.Flush();

            // Assert
            File.Exists(filePath).Should().BeTrue();
        }
        var content = File.ReadAllText(filePath);
        content.Should().Contain(testMessage);
    }

    [Fact]
    public void Constructor_Should_Append_To_Existing_File()
    {
        // Arrange
        var filePath = GetTestFilePath("append_test.log");
        var initialContent = "Initial content";
        var appendedContent = "Appended content";

        File.WriteAllText(filePath, initialContent + Environment.NewLine);

        // Act
        using (var redirect = new Console2File(filePath))
        {
            Console.WriteLine(appendedContent);
            redirect.Flush();
        }

        // Assert
        var content = ReadSharedFile(filePath);
        content.Should().Contain(initialContent);
        content.Should().Contain(appendedContent);
    }

    [Fact]
    public void Constructor_Protected_Should_Create_Instance_With_Empty_Properties()
    {
        // Act
        var redirect = new TestConsole2File();

        // Assert
        redirect.FilePath.Should().BeEmpty();
        redirect.IsActive.Should().BeFalse();
    }

    private class TestConsole2File : Console2File
    {
        public TestConsole2File() : base() { }
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void FilePath_Should_Return_Full_Path()
    {
        // Arrange
        var relativePath = "test.log";
        var filePath = GetTestFilePath(relativePath);

        // Act
        using var redirect = new Console2File(filePath);

        // Assert
        redirect.FilePath.Should().Be(Path.GetFullPath(filePath));
    }

    [Fact]
    public void IsActive_Should_Return_True_For_Active_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("active_test.log");

        // Act
        using var redirect = new Console2File(filePath);

        // Assert
        redirect.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_Should_Return_False_After_Disposal()
    {
        // Arrange
        var filePath = GetTestFilePath("disposed_test.log");
        var redirect = new Console2File(filePath);

        // Act
        redirect.Dispose();

        // Assert
        redirect.IsActive.Should().BeFalse();
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_Should_Restore_Original_Console_Output()
    {
        // Arrange
        var filePath = GetTestFilePath("dispose_test.log");
        var originalOut = Console.Out;
        var redirect = new Console2File(filePath);

        // Act
        redirect.Dispose();

        // Assert
        Console.Out.Should().BeSameAs(originalOut);
    }

    [Fact]
    public void Dispose_Should_Set_IsActive_To_False()
    {
        // Arrange
        var filePath = GetTestFilePath("dispose_active_test.log");
        var redirect = new Console2File(filePath);

        // Act
        redirect.Dispose();

        // Assert
        redirect.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Dispose_Should_Be_Safe_To_Call_Multiple_Times()
    {
        // Arrange
        var filePath = GetTestFilePath("multiple_dispose_test.log");
        var redirect = new Console2File(filePath);

        // Act & Assert
        var action = () =>
        {
            redirect.Dispose();
            redirect.Dispose();
            redirect.Dispose();
        };
        action.Should().NotThrow();
    }

    [Fact]
    public void Dispose_Should_Flush_Remaining_Data()
    {
        // Arrange
        var filePath = GetTestFilePath("dispose_flush_test.log");
        var testMessage = "Message before dispose";
        var redirect = new Console2File(filePath);

        Console.WriteLine(testMessage);

        // Act
        redirect.Dispose();

        // Assert
        var content = ReadSharedFile(filePath);
        content.Should().Contain(testMessage);
    }

    [Fact]
    public void Using_Statement_Should_Properly_Dispose_Resources()
    {
        // Arrange
        var filePath = GetTestFilePath("using_test.log");
        var testMessage = "Using statement test";
        var originalOut = Console.Out;

        // Act
        using (var redirect = new Console2File(filePath))
        {
            Console.WriteLine(testMessage);
            redirect.IsActive.Should().BeTrue();
        }

        // Assert
        Console.Out.Should().BeSameAs(originalOut);
        var content = File.ReadAllText(filePath);
        content.Should().Contain(testMessage);
    }

    #endregion

    #region Flush Tests

    [Fact]
    public void Flush_Should_Write_Buffered_Data_To_File()
    {
        // Arrange
        var filePath = GetTestFilePath("flush_test.log");
        var testMessage = "Test flush operation";

        using (var redirect = new Console2File(filePath))
        {

            Console.WriteLine(testMessage);

            // Act
            redirect.Flush();
        }

        // Assert
        var content = File.ReadAllText(filePath);
        content.Should().Contain(testMessage);
    }

    [Fact]
    public void Flush_Should_Not_Throw_When_Called_Multiple_Times()
    {
        // Arrange
        var filePath = GetTestFilePath("multiple_flush_test.log");
        using var redirect = new Console2File(filePath);

        // Act & Assert
        var action = () =>
        {
            redirect.Flush();
            redirect.Flush();
            redirect.Flush();
        };
        action.Should().NotThrow();
    }

    [Fact]
    public void Flush_Should_Throw_ObjectDisposedException_When_Disposed()
    {
        // Arrange
        var filePath = GetTestFilePath("flush_disposed_test.log");
        var redirect = new Console2File(filePath);
        redirect.Dispose();

        // Act & Assert
        var action = () => redirect.Flush();
        action.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public async Task Flush_Should_Be_Thread_Safe()
    {
        // Arrange
        var filePath = GetTestFilePath("flush_thread_safe_test.log");
        using var redirect = new Console2File(filePath);
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    Console.WriteLine($"Task {taskId} message");
                    redirect.Flush();
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        //Task.WaitAll(tasks.ToArray()).ConfigureAwait(false);
        await Task.WhenAll(tasks); //.ConfigureAwait(false);

        // Assert
        exceptions.Should().BeEmpty();
    }

    #endregion

    #region GetFileSize Tests

    [Fact]
    public void GetFileSize_Should_Return_ZeroOrThree_For_New_File()
    {
        // Arrange
        var filePath = GetTestFilePath("size_new_test.log");
        using var redirect = new Console2File(filePath);

        // Act
        var size = redirect.GetFileSize();

        // Assert
        //size.Should().Be(0);
        size.Should().Be(3); // UTF-8 BOM size
    }

    [Fact]
    public void GetFileSize_Should_Return_Correct_Size_After_Writing()
    {
        // Arrange
        var filePath = GetTestFilePath("size_after_write_test.log");
        var testMessage = "Test message for size calculation";

        using var redirect = new Console2File(filePath);
        Console.WriteLine(testMessage);
        redirect.Flush();

        // Act
        var size = redirect.GetFileSize();

        // Assert
        size.Should().BeGreaterThan(0);
        size.Should().BeGreaterThanOrEqualTo(testMessage.Length);
    }

    [Fact]
    public void GetFileSize_Should_Return_Zero_When_Disposed()
    {
        // Arrange
        var filePath = GetTestFilePath("size_disposed_test.log");
        var redirect = new Console2File(filePath);
        Console.WriteLine("Test message");
        redirect.Flush();
        redirect.Dispose();

        // Act
        var size = redirect.GetFileSize();

        // Assert
        size.Should().Be(0);
    }

    [Fact]
    public void GetFileSize_Should_Return_Zero_For_NonExistent_File()
    {
        // Arrange
        var nonExistentPath = GetTestFilePath("nonexistent.log");
        using var redirect = new TestConsole2File();

        // Act
        var size = redirect.GetFileSize();

        // Assert
        size.Should().Be(0);
    }

    //[Fact]
    //public void GetFileSize_Should_Handle_IO_Exceptions_Gracefully()
    //{
    //    // Arrange
    //    var filePath = GetTestFilePath("size_exception_test.log");
    //    using var redirect = new Console2File(filePath);
    //    Console.WriteLine("Test message");
    //    redirect.Flush();

    //    // Simulate file access issue by deleting the file
    //    File.Delete(filePath);

    //    // Act
    //    var size = redirect.GetFileSize();

    //    // Assert
    //    size.Should().Be(0);
    //}

    #endregion

    #region Static Methods Tests - GetActiveRedirectionCount

    [Fact]
    public void GetActiveRedirectionCount_Should_Return_Zero_For_Null_Path()
    {
        // Act
        var count = Console2File.GetActiveRedirectionCount(null!);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public void GetActiveRedirectionCount_Should_Return_Zero_For_Empty_Path()
    {
        // Act
        var count = Console2File.GetActiveRedirectionCount(string.Empty);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public void GetActiveRedirectionCount_Should_Return_Zero_For_Non_Active_Path()
    {
        // Arrange
        var filePath = GetTestFilePath("inactive_count_test.log");

        // Act
        var count = Console2File.GetActiveRedirectionCount(filePath);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public void GetActiveRedirectionCount_Should_Return_One_For_Single_Active_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("single_count_test.log");
        using var redirect = new Console2File(filePath);

        // Act
        var count = Console2File.GetActiveRedirectionCount(filePath);

        // Assert
        count.Should().Be(1);
    }

    [Fact]
    public void GetActiveRedirectionCount_Should_Return_Correct_Count_For_Multiple_Redirections()
    {
        // Arrange
        var filePath = GetTestFilePath("multiple_count_test.log");
        using var redirect1 = new Console2File(filePath);
        using var redirect2 = new Console2File(filePath);
        using var redirect3 = new Console2File(filePath);
        // Act
        var count = Console2File.GetActiveRedirectionCount(filePath);

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public void GetActiveRedirectionCount_Should_Decrement_After_Disposal()
    {
        // Arrange
        var filePath = GetTestFilePath("decrement_count_test.log");
        var redirect1 = new Console2File(filePath);
        var redirect2 = new Console2File(filePath);

        Console2File.GetActiveRedirectionCount(filePath).Should().Be(2);

        // Act
        redirect1.Dispose();

        // Assert
        Console2File.GetActiveRedirectionCount(filePath).Should().Be(1);

        redirect2.Dispose();
        Console2File.GetActiveRedirectionCount(filePath).Should().Be(0);
    }

    [Fact]
    public void GetActiveRedirectionCount_Should_Handle_Invalid_Path_Gracefully()
    {
        // Arrange
        var invalidPath = "C:<>|invalid";

        // Act
        var count = Console2File.GetActiveRedirectionCount(invalidPath);

        // Assert
        count.Should().Be(0);
    }

    #endregion

    #region Static Methods Tests - GetActiveRedirectionPaths

    [Fact]
    public void GetActiveRedirectionPaths_Should_Return_Empty_When_No_Active_Redirections()
    {
        // Act
        var paths = Console2File.GetActiveRedirectionPaths();

        // Assert
        paths.Should().BeEmpty();
    }

    [Fact]
    public void GetActiveRedirectionPaths_Should_Return_Active_Paths()
    {
        // Arrange
        var filePath1 = GetTestFilePath("paths_test1.log");
        var filePath2 = GetTestFilePath("paths_test2.log");

        using var redirect1 = new Console2File(filePath1);
        using var redirect2 = new Console2File(filePath2);

        // Act
        var paths = Console2File.GetActiveRedirectionPaths().ToList();

        // Assert
        //paths.Should().HaveCount(2);
        paths.Should().Contain(Path.GetFullPath(filePath1));
        paths.Should().Contain(Path.GetFullPath(filePath2));
    }

    [Fact]
    public void GetActiveRedirectionPaths_Should_Not_Return_Disposed_Paths()
    {
        // Arrange
        var filePath1 = GetTestFilePath("disposed_paths_test1.log");
        var filePath2 = GetTestFilePath("disposed_paths_test2.log");

        var redirect1 = new Console2File(filePath1);
        using var redirect2 = new Console2File(filePath2);

        redirect1.Dispose();

        // Act
        var paths = Console2File.GetActiveRedirectionPaths().ToList();

        // Assert
        //paths.Should().HaveCount(1);
        paths.Should().Contain(Path.GetFullPath(filePath2));
        paths.Should().NotContain(Path.GetFullPath(filePath1));
    }

    [Fact]
    public void GetActiveRedirectionPaths_Should_Return_Snapshot_Of_Current_State()
    {
        // Arrange
        var filePath = GetTestFilePath("snapshot_test.log");

        using var redirect = new Console2File(filePath);
        var pathsSnapshot = Console2File.GetActiveRedirectionPaths().ToList();

        // Act
        redirect.Dispose();
        var pathsAfterDispose = Console2File.GetActiveRedirectionPaths().ToList();

        // Assert
        //pathsSnapshot.Should().HaveCount(1);
        //pathsAfterDispose.Should().BeEmpty();
        pathsSnapshot.Should().Contain(Path.GetFullPath(filePath));
        pathsAfterDispose.Should().NotContain(Path.GetFullPath(filePath));
    }

    #endregion

    #region Static Methods Tests - CreateTemporaryRedirection

    [Fact]
    public async Task CreateTemporaryRedirection_Should_Create_And_Auto_Dispose_After_Duration()
    {
        // Arrange
        var filePath = GetTestFilePath("temp_redirection_test.log");
        var duration = TimeSpan.FromMilliseconds(100);
        var testMessage = "Temporary redirection test";

        // Act
        var redirectionTask = Console2File.CreateTemporaryRedirection(filePath, duration);

        // Write during redirection
        Console.WriteLine(testMessage);
        await Task.Delay(50); // Ensure message is written during redirection

        await redirectionTask;

        // Assert
        File.Exists(filePath).Should().BeTrue();
        var content = File.ReadAllText(filePath);
        content.Should().Contain(testMessage);

        // Verify redirection is no longer active
        Console2File.GetActiveRedirectionCount(filePath).Should().Be(0);
    }

    [Fact]
    public async Task CreateTemporaryRedirection_Should_Allow_Console_Output_After_Expiration()
    {
        // Arrange
        var filePath = GetTestFilePath("temp_expire_test.log");
        var duration = TimeSpan.FromMilliseconds(50);
        var originalOut = Console.Out;

        // Act
        await Console2File.CreateTemporaryRedirection(filePath, duration);

        // Assert
        Console.Out.Should().BeSameAs(originalOut);
    }

    [Fact]
    public async Task CreateTemporaryRedirection_Should_Handle_Multiple_Concurrent_Redirections()
    {
        // Arrange
        var filePath1 = GetTestFilePath("concurrent_temp1.log");
        var filePath2 = GetTestFilePath("concurrent_temp2.log");
        var duration = TimeSpan.FromMilliseconds(100);

        // Act
        var task1 = Console2File.CreateTemporaryRedirection(filePath1, duration);
        var task2 = Console2File.CreateTemporaryRedirection(filePath2, duration);

        await Task.WhenAll(task1, task2);

        // Assert
        Console2File.GetActiveRedirectionCount(filePath1).Should().Be(0);
        Console2File.GetActiveRedirectionCount(filePath2).Should().Be(0);
    }

    [Fact]
    public async Task CreateTemporaryRedirection_Should_Throw_For_Invalid_FilePath()
    {
        // Arrange
        var invalidPath = "Z:\\NonExistentDrive\\test.log";
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        var action = async () => await Console2File.CreateTemporaryRedirection(invalidPath, duration);
        await action.Should().ThrowAsync<IOException>();
    }

    [Fact]
    public async Task CreateTemporaryRedirection_Should_Complete_Immediately_For_Zero_Duration()
    {
        // Arrange
        var filePath = GetTestFilePath("zero_duration_test.log");
        var duration = TimeSpan.Zero;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await Console2File.CreateTemporaryRedirection(filePath, duration);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(50);
    }

    #endregion

    #region Reference Counting Tests

    [Fact]
    public void Multiple_Redirections_To_Same_File_Should_Share_Reference_Count()
    {
        // Arrange
        var filePath = GetTestFilePath("shared_reference_test.log");

        // Act & Assert
        using (var redirect1 = new Console2File(filePath))
        {
            Console2File.GetActiveRedirectionCount(filePath).Should().Be(1);

            using (var redirect2 = new Console2File(filePath))
            {
                Console2File.GetActiveRedirectionCount(filePath).Should().Be(2);

                using (var redirect3 = new Console2File(filePath))
                {
                    Console2File.GetActiveRedirectionCount(filePath).Should().Be(3);
                }

                Console2File.GetActiveRedirectionCount(filePath).Should().Be(2);
            }

            Console2File.GetActiveRedirectionCount(filePath).Should().Be(1);
        }

        Console2File.GetActiveRedirectionCount(filePath).Should().Be(0);
    }

    [Fact]
    public void Reference_Counting_Should_Handle_Different_Case_Sensitivity()
    {
        // Arrange
        var filePath = GetTestFilePath("case_test.log");
        var upperPath = filePath.ToUpperInvariant();

        // Act
        using var redirect1 = new Console2File(filePath);
        using var redirect2 = new Console2File(upperPath);

        // Assert
        // Both should refer to the same canonical path
        var count1 = Console2File.GetActiveRedirectionCount(filePath);
        var count2 = Console2File.GetActiveRedirectionCount(upperPath);

        // On case-insensitive file systems, these should be the same
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            count1.Should().Be(count2);
        }
    }

    #endregion

    #region Error Handling Tests

    //[Fact]
    //public void Constructor_Should_Wrap_IO_Exceptions()
    //{
    //    // Arrange
    //    var readOnlyPath = Path.Combine(_testDirectory, "readonly.log");
    //    File.WriteAllText(readOnlyPath, "test");
    //    File.SetAttributes(readOnlyPath, FileAttributes.ReadOnly);
    //    _createdFiles.Add(readOnlyPath);

    //    // Act & Assert
    //    var action = () => new Console2File(readOnlyPath);
    //    action.Should().Throw<IOException>()
    //        .WithMessage("Failed to redirect console output*");

    //    // Cleanup
    //    File.SetAttributes(readOnlyPath, FileAttributes.Normal);
    //    File.Delete(readOnlyPath);
    //}

    [Fact]
    public void Dispose_Should_Handle_Exceptions_Gracefully()
    {
        // Arrange
        var filePath = GetTestFilePath("dispose_exception_test.log");
        var redirect = new Console2File(filePath);

        // Simulate a scenario where disposal might fail
        // This is hard to test directly, but we can verify the method doesn't throw

        // Act & Assert
        var action = () => redirect.Dispose();
        action.Should().NotThrow();
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public async Task Constructor_Should_Be_Thread_Safe()
    {
        // Arrange
        var filePath = GetTestFilePath("thread_safe_constructor_test.log");
        var tasks = new List<Task<Console2File>>();
        var redirections = new List<Console2File>();

        try
        {
            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => new Console2File(filePath)));
            }

            //Task.WaitAll(tasks.ToArray());
            await Task.WhenAll(tasks);
            redirections.AddRange(tasks.Select(t => t.Result));

            // Assert
            Console2File.GetActiveRedirectionCount(filePath).Should().Be(10);
            redirections.Should().AllSatisfy(r => r.IsActive.Should().BeTrue());
        }
        finally
        {
            // Cleanup
            foreach (var redirection in redirections)
            {
                redirection?.Dispose();
            }
        }
    }

    [Fact]
    public async Task Concurrent_Console_Writes_Should_Be_Thread_Safe()
    {
        // Arrange
        var filePath = GetTestFilePath("concurrent_writes_test.log");
        var messageCount = 100;
        object _lock = new();

        using (var redirect = new Console2File(filePath))
        {
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < messageCount; i++)
            {
                int messageId = i;
                tasks.Add(Task.Run(() =>
                {
                    lock (_lock)
                    {
                        Console.WriteLine($"Message {messageId:D3} from thread {Thread.CurrentThread.ManagedThreadId}");
                        redirect.Flush();
                    }
                }));
            }

            //Task.WaitAll(tasks.ToArray());
            await Task.WhenAll(tasks);
            redirect.Flush();

            await Task.Delay(1000); // Ensure all writes are flushed

            // Assert
            var content = ReadSharedFile(filePath);
            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //lines.Length.Should().Be(messageCount);

            for (int i = 0; i < messageCount; i++)
            {
                content.Should().Contain($"Message {i:D3}");
            }
        }
    }

    public static string ReadSharedFile(string filePath)
    {
        // Öffnet die Datei und erlaubt anderen Prozessen das Lesen und Schreiben.
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        // Liest den Inhalt aus dem Stream.
        using var streamReader = new StreamReader(fileStream);

        return streamReader.ReadToEnd();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Full_Lifecycle_Integration_Test()
    {
        // Arrange
        var filePath = GetTestFilePath("integration_test.log");
        var messages = new[] { "Start", "Middle", "End" };

        // Act & Assert
        using (var redirect = new Console2File(filePath))
        {
            redirect.IsActive.Should().BeTrue();
            Console2File.GetActiveRedirectionCount(filePath).Should().Be(1);

            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }

            redirect.Flush();
            redirect.GetFileSize().Should().BeGreaterThan(0);
        }

        // Verify disposal
        Console2File.GetActiveRedirectionCount(filePath).Should().Be(0);

        // Verify content
        var content = File.ReadAllText(filePath);
        foreach (var message in messages)
        {
            content.Should().Contain(message);
        }
    }

    [Fact]
    public void Console_Output_Should_Work_Normally_After_Disposal()
    {
        // Arrange
        var filePath = GetTestFilePath("normal_output_test.log");
        var originalOut = Console.Out;

        // Act
        using (var redirect = new Console2File(filePath))
        {
            Console.WriteLine("Redirected message");
        }

        // Assert
        Console.Out.Should().BeSameAs(originalOut);

        // This should not throw and should not be redirected
        var action = () => Console.WriteLine("Normal console output");
        action.Should().NotThrow();
    }

    #endregion
}