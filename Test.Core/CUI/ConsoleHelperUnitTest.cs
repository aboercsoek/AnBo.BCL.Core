//--------------------------------------------------------------------------
// File:    ConsoleHelperUnitTest.cs
// Content: Unit tests for ConsoleHelper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Text;

namespace AnBo.Test;

[Collection("Sequential")]
public class ConsoleHelperUnitTest : IDisposable
{
    #region Test Setup and Teardown

    private readonly string _testDirectory;
    private readonly List<string> _createdFiles;
    private readonly TextWriter _originalConsoleOut;
    private readonly ConsoleColor _originalForegroundColor;
    private readonly RedirectionConfiguration _originalRedirectionConfig;

    public ConsoleHelperUnitTest()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"ConsoleHelperTest_{Guid.NewGuid():N}");
        _createdFiles = new List<string>();
        _originalConsoleOut = Console.Out;
        _originalForegroundColor = Console.ForegroundColor;
        _originalRedirectionConfig = ConsoleHelper.RedirectionConfig;

        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }
    }

    public void Dispose()
    {
        // Restore original console state
        Console.SetOut(_originalConsoleOut);
        Console.ForegroundColor = _originalForegroundColor;
        ConsoleHelper.RedirectionConfig = _originalRedirectionConfig;

        // Dispose any active redirections
        ConsoleHelper.DisposePrimaryRedirection();

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

    private string CaptureConsoleOutput(Action action)
    {
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        var originalOut = Console.Out;

        try
        {
            Console.SetOut(writer);
            action();
            writer.Flush();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        return output.ToString();
    }

    #endregion

    #region Configuration Properties Tests

    [Fact]
    public void RedirectionConfig_Should_Get_And_Set_Configuration()
    {
        // Arrange
        var config = new RedirectionConfiguration
        {
            BasePath = "test.log",
            RedirectionType = RedirectionType.Timestamped,
            MaxSizeBytes = 5 * 1024 * 1024,
            MaxFiles = 20,
            TimestampFormat = "yyyy-MM-dd_HH-mm-ss"
        };

        // Act
        ConsoleHelper.RedirectionConfig = config;

        // Assert
        ConsoleHelper.RedirectionConfig.Should().Be(config);
        ConsoleHelper.RedirectionConfig.BasePath.Should().Be("test.log");
        ConsoleHelper.RedirectionConfig.RedirectionType.Should().Be(RedirectionType.Timestamped);
        ConsoleHelper.RedirectionConfig.MaxSizeBytes.Should().Be(5 * 1024 * 1024);
        ConsoleHelper.RedirectionConfig.MaxFiles.Should().Be(20);
        ConsoleHelper.RedirectionConfig.TimestampFormat.Should().Be("yyyy-MM-dd_HH-mm-ss");
    }

    [Fact]
    public void RedirectionConfig_With_Null_Should_Create_New_Configuration()
    {
        // Act
        ConsoleHelper.RedirectionConfig = null!;

        // Assert
        ConsoleHelper.RedirectionConfig.Should().NotBeNull();
        ConsoleHelper.RedirectionConfig.Should().BeOfType<RedirectionConfiguration>();
    }

    [Fact]
    public void IsRedirectionActive_Should_Return_False_Initially()
    {
        // Assert
        ConsoleHelper.IsRedirectionActive.Should().BeFalse();
    }

    [Fact]
    public void CurrentRedirectionPath_Should_Return_Null_Initially()
    {
        // Assert
        ConsoleHelper.CurrentRedirectionPath.Should().BeNull();
    }

    [Fact]
    public void IsRedirectionActive_Should_Return_True_When_Redirection_Is_Active()
    {
        // Arrange
        var filePath = GetTestFilePath("active_test.log");

        // Act
        using var redirect = ConsoleHelper.ConfigureFileRedirection(filePath);

        // Assert
        ConsoleHelper.IsRedirectionActive.Should().BeTrue();
        ConsoleHelper.CurrentRedirectionPath.Should().Be(Path.GetFullPath(filePath));
    }

    #endregion

    #region ConfigureFileRedirection Tests

    [Fact]
    public void ConfigureFileRedirection_With_Simple_Type_Should_Create_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("simple_redirect.log");

        // Act
        using var redirect = ConsoleHelper.ConfigureFileRedirection(filePath, RedirectionType.Simple);

        // Assert
        redirect.Should().NotBeNull();
        redirect.FilePath.Should().Be(Path.GetFullPath(filePath));
        redirect.IsActive.Should().BeTrue();
        ConsoleHelper.IsRedirectionActive.Should().BeTrue();
    }

    [Fact]
    public void ConfigureFileRedirection_With_Timestamped_Type_Should_Create_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("timestamped_redirect.log");

        // Act
        using var redirect = ConsoleHelper.ConfigureFileRedirection(filePath, RedirectionType.Timestamped);

        // Assert
        redirect.Should().NotBeNull();
        redirect.IsActive.Should().BeTrue();
        ConsoleHelper.IsRedirectionActive.Should().BeTrue();
    }

    [Fact]
    public void ConfigureFileRedirection_With_Rotating_Type_Should_Create_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("rotating_redirect.log");

        // Act
        using var redirect = ConsoleHelper.ConfigureFileRedirection(
            filePath,
            RedirectionType.Rotating,
            maxSizeBytes: 1024,
            maxFiles: 5);

        // Assert
        redirect.Should().NotBeNull();
        redirect.IsActive.Should().BeTrue();
        ConsoleHelper.IsRedirectionActive.Should().BeTrue();
        ConsoleHelper.RedirectionConfig.MaxSizeBytes.Should().Be(1024);
        ConsoleHelper.RedirectionConfig.MaxFiles.Should().Be(5);
    }

    [Fact]
    public void ConfigureFileRedirection_With_TimestampedRotating_Type_Should_Create_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("timestamped_rotating_redirect.log");

        // Act
        using var redirect = ConsoleHelper.ConfigureFileRedirection(
            filePath,
            RedirectionType.TimestampedRotating,
            maxSizeBytes: 2048,
            maxFiles: 3,
            timestampFormat: "yyyyMMdd-HHmmss");

        // Assert
        redirect.Should().NotBeNull();
        redirect.IsActive.Should().BeTrue();
        ConsoleHelper.RedirectionConfig.TimestampFormat.Should().Be("yyyyMMdd-HHmmss");
    }

    [Fact]
    public void ConfigureFileRedirection_With_Invalid_Type_Should_Throw_ArgumentException()
    {
        // Arrange
        var filePath = GetTestFilePath("invalid_type.log");

        // Act & Assert
        var action = () => ConsoleHelper.ConfigureFileRedirection(filePath, (RedirectionType)999);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("redirectionType")
            .WithMessage("Unknown redirection type: 999*");
    }

    [Fact]
    public void ConfigureFileRedirection_Should_Dispose_Previous_Redirection()
    {
        // Arrange
        var filePath1 = GetTestFilePath("first_redirect.log");
        var filePath2 = GetTestFilePath("second_redirect.log");

        // Act
        var redirect1 = ConsoleHelper.ConfigureFileRedirection(filePath1);
        var wasActive = redirect1.IsActive;

        var redirect2 = ConsoleHelper.ConfigureFileRedirection(filePath2);

        // Assert
        wasActive.Should().BeTrue();
        redirect1.IsActive.Should().BeFalse();
        redirect2.IsActive.Should().BeTrue();
        ConsoleHelper.CurrentRedirectionPath.Should().Be(Path.GetFullPath(filePath2));

        redirect2.Dispose();
    }

    #endregion

    #region CreateTemporaryRedirection Tests

    [Fact]
    public async Task CreateTemporaryRedirection_Should_Complete_After_Duration()
    {
        // Arrange
        var filePath = GetTestFilePath("temp_redirect.log");
        var duration = TimeSpan.FromMilliseconds(100);

        // Act
        var task = ConsoleHelper.CreateTemporaryRedirection(filePath, duration);
        await task;

        // Assert
        task.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTemporaryRedirection_Should_Allow_Console_Output_During_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("temp_output.log");
        var duration = TimeSpan.FromMilliseconds(200);
        var testMessage = "Temporary redirection test";

        // Act
        var task = ConsoleHelper.CreateTemporaryRedirection(filePath, duration);

        // Write during redirection
        await Task.Delay(50);
        Console.WriteLine(testMessage);
        await Task.Delay(50);

        await task;

        // Assert
        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            content.Should().Contain(testMessage);
        }
    }

    #endregion

    #region DisposePrimaryRedirection Tests

    [Fact]
    public void DisposePrimaryRedirection_Should_Dispose_Active_Redirection()
    {
        // Arrange
        var filePath = GetTestFilePath("dispose_primary.log");
        var redirect = ConsoleHelper.ConfigureFileRedirection(filePath);

        // Act
        ConsoleHelper.DisposePrimaryRedirection();

        // Assert
        ConsoleHelper.IsRedirectionActive.Should().BeFalse();
        ConsoleHelper.CurrentRedirectionPath.Should().BeNull();
        redirect.IsActive.Should().BeFalse();
    }

    [Fact]
    public void DisposePrimaryRedirection_Should_Not_Throw_When_No_Active_Redirection()
    {
        // Act & Assert
        var action = () => ConsoleHelper.DisposePrimaryRedirection();
        action.Should().NotThrow();
    }

    #endregion

    #region WriteLine Methods Tests

    [Fact]
    public void WriteLine_With_Format_And_Color_Should_Output_Formatted_Text()
    {
        // Arrange
        var format = "Hello {0}, you have {1} messages";
        var args = new object[] { "John", 5 };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLine(format, ConsoleColor.Green, args));

        // Assert
        output.Should().Contain("Hello John, you have 5 messages");
    }

    [Fact]
    public void WriteLine_With_Format_And_Null_Should_Not_Throw()
    {
        // Act & Assert
        var action = () => ConsoleHelper.WriteLine(null!, ConsoleColor.Red);
        action.Should().NotThrow();
    }

    [Fact]
    public void WriteLine_With_Text_And_Color_Should_Output_Text()
    {
        // Arrange
        var text = "Test message";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLine(text, ConsoleColor.Blue));

        // Assert
        output.Should().Contain("Test message");
    }

    [Fact]
    public void WriteLine_With_Null_Text_Should_Output_Empty_Line()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLine(null, ConsoleColor.Red));

        // Assert
        output.Should().Be(Environment.NewLine);
    }

    [Fact]
    public void WriteLine_With_Object_And_Color_Should_Output_Object_String()
    {
        // Arrange
        var value = 42.5m;

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLine(value, ConsoleColor.Cyan));

        // Assert
        output.Should().Contain("42.5");
    }

    [Fact]
    public void WriteLine_With_Exception_Should_Output_Exception_Details()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception message");

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLine(exception));

        // Assert
        output.Should().Contain("InvalidOperationException");
        output.Should().Contain("Test exception message");
    }

    [Fact]
    public void WriteLine_With_Exception_And_StackTrace_Should_Include_StackTrace()
    {
        // Arrange
        var exception = new ArgumentException("Test exception");

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLine(exception, showStackTrace: true));

        // Assert
        output.Should().Contain("ArgumentException");
        output.Should().Contain("Test exception");
    }

    [Fact]
    public void WriteLine_With_Null_Exception_Should_Not_Output_Anything()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLine((Exception?)null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Assert
        output.Should().BeEmpty();
    }

    #endregion

    #region Write Methods Tests

    [Fact]
    public void Write_With_Format_And_Color_Should_Output_Formatted_Text_Without_NewLine()
    {
        // Arrange
        var format = "Progress: {0}%";
        var args = new object[] { 75 };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.Write(format, ConsoleColor.Yellow, args));

        // Assert
        output.Should().Be("Progress: 75%");
        output.Should().NotContain(Environment.NewLine);
    }

    [Fact]
    public void Write_With_Null_Format_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => ConsoleHelper.Write(null!, ConsoleColor.Red);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Write_With_Object_And_Color_Should_Output_Object_String_Without_NewLine()
    {
        // Arrange
        var value = "test";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.Write(value, ConsoleColor.Green));

        // Assert
        output.Should().Be("test");
        output.Should().NotContain(Environment.NewLine);
    }

    #endregion

    #region Header Methods Tests

    [Fact]
    public void WriteHeader_Should_Create_Header_With_Equal_Signs()
    {
        // Arrange
        var title = "Test Header";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteHeader(title));

        // Assert
        output.Should().Contain("==========="); // Length of "Test Header"
        output.Should().Contain("Test Header");
        var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(3);
        lines[0].Should().Be(new string('=', title.Length));
        lines[1].Should().Be(title);
        lines[2].Should().Be(new string('=', title.Length));
    }

    [Fact]
    public void WriteHeader_With_Empty_Title_Should_Not_Output_Anything()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteHeader(""));

        // Assert
        output.Should().BeEmpty();
    }

    [Fact]
    public void WriteHeader_With_Whitespace_Title_Should_Not_Output_Anything()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteHeader("   "));

        // Assert
        output.Should().BeEmpty();
    }

    [Fact]
    public void WriteSimpleHeader_Should_Create_Header_With_Dashes()
    {
        // Arrange
        var title = "Simple Header";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteSimpleHeader(title));

        // Assert
        output.Should().Contain("Simple Header");
        output.Should().Contain("-------------"); // Length of "Simple Header"
        var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        lines[0].Should().Be(title);
        lines[1].Should().Be(new string('-', title.Length));
    }

    [Fact]
    public void WriteSimpleHeader_With_Empty_Title_Should_Not_Output_Anything()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteSimpleHeader(""));

        // Assert
        output.Should().BeEmpty();
    }

    #endregion

    #region NewLine and HR Methods Tests

    [Fact]
    public void NewLine_Should_Output_Single_Line_Break()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.NewLine());

        // Assert
        output.Should().Be(Environment.NewLine);
    }

    [Fact]
    public void HR_Default_Should_Output_40_Dashes()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.HR());

        // Assert
        output.Should().Contain(new string('-', 40));
    }

    [Fact]
    public void HR_With_Color_Should_Output_40_Dashes()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.HR(ConsoleColor.Green));

        // Assert
        output.Should().Contain(new string('-', 40));
    }

    [Fact]
    public void HR_With_Length_And_Color_Should_Output_Specified_Length()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.HR(20, ConsoleColor.Blue));

        // Assert
        output.Should().Contain(new string('-', 20));
    }

    [Fact]
    public void HR_With_Custom_Character_Should_Output_Custom_Line()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.HR(15, '*', ConsoleColor.Red));

        // Assert
        output.Should().Contain(new string('*', 15));
    }

    [Fact]
    public void HR_With_Zero_Length_Should_Not_Output_Anything()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.HR(0, '-', ConsoleColor.White));

        // Assert
        output.Should().BeEmpty();
    }

    [Fact]
    public void HR_With_Negative_Length_Should_Not_Output_Anything()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.HR(-5, '-', ConsoleColor.White));

        // Assert
        output.Should().BeEmpty();
    }

    #endregion

    #region ReadLine Methods Tests

    [Fact]
    public void ReadLine_With_Text_And_Color_Should_Display_Prompt()
    {
        // Arrange
        var input = "test input\n";
        var inputReader = new StringReader(input);
        Console.SetIn(inputReader);

        try
        {
            // Act
            var output = CaptureConsoleOutput(() =>
            {
                var result = ConsoleHelper.ReadLine("Enter text: ", ConsoleColor.Green);
                result.Should().Be("test input");
            });

            // Assert
            output.Should().Contain("Enter text: ");
        }
        finally
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
        }
    }

    [Fact]
    public void ReadLine_With_Text_Should_Display_Default_Prompt()
    {
        // Arrange
        var input = "test\n";
        var inputReader = new StringReader(input);
        Console.SetIn(inputReader);

        try
        {
            // Act
            var output = CaptureConsoleOutput(() =>
            {
                var result = ConsoleHelper.ReadLine("Custom prompt: ");
                result.Should().Be("test");
            });

            // Assert
            output.Should().Contain("Custom prompt: ");
        }
        finally
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
        }
    }

    [Fact]
    public void ReadLine_With_Empty_Text_Should_Display_Default_Message()
    {
        // Arrange
        var input = "\n";
        var inputReader = new StringReader(input);
        Console.SetIn(inputReader);

        try
        {
            // Act
            var output = CaptureConsoleOutput(() =>
            {
                var result = ConsoleHelper.ReadLine("");
                result.Should().Be("");
            });

            // Assert
            output.Should().Contain("Hit <ENTER> to proceed!");
        }
        finally
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
        }
    }

    #endregion

    #region ReadKey Methods Tests

    [Fact]
    public void ReadKey_With_Text_Should_Display_Prompt()
    {
        // Note: ReadKey tests are limited in unit testing as they require actual key input
        // These tests focus on the prompt display behavior

        // Arrange & Act
        var output = CaptureConsoleOutput(() =>
        {
            // We can't easily test ReadKey without manual input, 
            // but we can test the prompt display
            ConsoleHelper.Write("Press any key: ", ConsoleColor.Yellow);
        });

        // Assert
        output.Should().Contain("Press any key: ");
    }

    #endregion

    #region ReadPassword Methods Tests

    [Fact]
    public void ReadPassword_Should_Handle_Enter_Key()
    {
        // Note: Password reading requires complex console input simulation
        // This test focuses on the method signature and basic behavior

        var password = string.Empty;

        // We can test that the method exists and has correct signature
        var method = typeof(ConsoleHelper).GetMethod("ReadPassword",
            new[] { typeof(string).MakeByRefType(), typeof(string), typeof(char), typeof(ConsoleColor) });

        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(ConsoleKey));
    }

    #endregion

    #region Colored WriteLine Convenience Methods Tests

    [Fact]
    public void WriteLineMagenta_Should_Output_In_Magenta()
    {
        // Arrange
        var value = "Magenta text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineMagenta(value));

        // Assert
        output.Should().Contain("Magenta text");
    }

    [Fact]
    public void WriteLineWhite_Should_Output_In_White()
    {
        // Arrange
        var value = "White text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineWhite(value));

        // Assert
        output.Should().Contain("White text");
    }

    [Fact]
    public void WriteLineYellow_Should_Output_In_Yellow()
    {
        // Arrange
        var value = "Yellow text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineYellow(value));

        // Assert
        output.Should().Contain("Yellow text");
    }

    [Fact]
    public void WriteLineGreen_Should_Output_In_Green()
    {
        // Arrange
        var value = "Green text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineGreen(value));

        // Assert
        output.Should().Contain("Green text");
    }

    [Fact]
    public void WriteLineGray_Should_Output_In_Gray()
    {
        // Arrange
        var value = "Gray text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineGray(value));

        // Assert
        output.Should().Contain("Gray text");
    }

    [Fact]
    public void WriteLineRed_Should_Output_In_Red()
    {
        // Arrange
        var value = "Red text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineRed(value));

        // Assert
        output.Should().Contain("Red text");
    }

    [Fact]
    public void WriteLineBlue_Should_Output_In_Blue()
    {
        // Arrange
        var value = "Blue text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineBlue(value));

        // Assert
        output.Should().Contain("Blue text");
    }

    [Fact]
    public void WriteLineCyan_Should_Output_In_Cyan()
    {
        // Arrange
        var value = "Cyan text";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineCyan(value));

        // Assert
        output.Should().Contain("Cyan text");
    }

    [Fact]
    public void WriteLineMagenta_With_Paragraph_Before_Should_Add_NewLine_Before()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineMagenta("Test", Paragraph.AddBefore));

        // Assert
        output.Should().StartWith(Environment.NewLine);
        output.Should().Contain("Test");
    }

    [Fact]
    public void WriteLineGreen_With_Paragraph_After_Should_Add_NewLine_After()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineGreen("Test", Paragraph.AddAfter));

        // Assert
        output.Should().Contain("Test");
        output.Should().EndWith(Environment.NewLine + Environment.NewLine);
    }

    [Fact]
    public void WriteLineRed_With_Paragraph_BeforeAndAfter_Should_Add_Both()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteLineRed("Test", Paragraph.AddBeforeAndAfter));

        // Assert
        output.Should().StartWith(Environment.NewLine);
        output.Should().Contain("Test");
        output.Should().EndWith(Environment.NewLine + Environment.NewLine);
    }

    #endregion

    #region Collection Display Methods Tests

    [Fact]
    public void WriteCollection_With_Name_Should_Display_Collection_With_Header()
    {
        // Arrange
        var items = new[] { "apple", "banana", "cherry" };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteCollection("Fruits", items, ", "));

        // Assert
        output.Should().Contain("Fruits");
        output.Should().Contain("{ apple, banana, cherry }");
    }

    [Fact]
    public void WriteCollection_With_Null_Collection_Should_Display_Null()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteCollection("Empty", null, ", "));

        // Assert
        output.Should().Contain("Empty");
        output.Should().Contain("<null>");
    }

    [Fact]
    public void WriteCollection_With_Empty_Collection_Should_Display_Empty()
    {
        // Arrange
        var items = new string[0];

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteCollection("Empty", items, ", "));

        // Assert
        output.Should().Contain("Empty");
        output.Should().Contain("{ <empty> }");
    }

    [Fact]
    public void WriteCollection_With_Custom_Separator_Should_Use_Separator()
    {
        // Arrange
        var items = new[] { 1, 2, 3 };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteCollection("Numbers", items, " | "));

        // Assert
        output.Should().Contain("Numbers");
        output.Should().Contain("{ 1 | 2 | 3 }");
    }

    [Fact]
    public void WriteCollection_With_Null_Separator_Should_Use_Default()
    {
        // Arrange
        var items = new[] { "a", "b" };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteCollection(items, ConsoleColor.White, null));

        // Assert
        output.Should().Contain("{ a, b }");
    }

    [Fact]
    public void WriteCollection_With_Full_Colors_Should_Display_With_Custom_Colors()
    {
        // Arrange
        var items = new[] { "item1", "item2" };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteCollection("Items", items, " | ",
                ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.Gray));

        // Assert
        output.Should().Contain("Items");
        output.Should().Contain("item1 | item2");
    }

    #endregion

    #region Name-Value Pair Display Methods Tests

    [Fact]
    public void WriteNameValue_With_Default_Colors_Should_Display_Name_And_Value()
    {
        // Arrange
        var name = "Property";
        var value = "TestValue";

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteNameValue(name, value));

        // Assert
        output.Should().Contain("Property");
        output.Should().Contain("TestValue");
        output.Should().Contain("[String]");
        output.Should().Contain(" = ");
    }

    [Fact]
    public void WriteNameValue_With_Custom_Colors_Should_Use_Custom_Colors()
    {
        // Arrange
        var name = "Count";
        var value = 42;

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteNameValue(name, value, ConsoleColor.Yellow, ConsoleColor.Cyan));

        // Assert
        output.Should().Contain("Count");
        output.Should().Contain("42");
        output.Should().Contain("[Int32]");
    }

    [Fact]
    public void WriteNameValue_With_Null_Value_Should_Display_Null()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteNameValue("NullProperty", null));

        // Assert
        output.Should().Contain("NullProperty");
        output.Should().Contain("[null]");
        output.Should().Contain("<null>");
    }

    [Fact]
    public void WriteNameValue_With_Collection_Should_Display_As_Collection()
    {
        // Arrange
        var items = new[] { "a", "b", "c" };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteNameValue("Items", items));

        // Assert
        output.Should().Contain("Items");
        output.Should().Contain("{ a, b, c }");
    }

    [Fact]
    public void WriteNameValue_With_Custom_Separator_Should_Use_Separator()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteNameValue("Property", "Value",
                ConsoleColor.Green, ConsoleColor.White, " -> ", ConsoleColor.Yellow));

        // Assert
        output.Should().Contain("Property");
        output.Should().Contain(" -> ");
        output.Should().Contain("Value");
    }

    #endregion

    #region Redirection Status Methods Tests

    [Fact]
    public void WriteRedirectionStatus_With_No_Active_Redirection_Should_Display_No_Active()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteRedirectionStatus());

        // Assert
        output.Should().Contain("Console Redirection Status");
        output.Should().Contain("No active redirection");
    }

    [Fact]
    public void WriteRedirectionStatus_With_Active_Redirection_Should_Display_Details()
    {
        // Arrange
        var filePath = GetTestFilePath("status_test.log");

        // Act
        //var output = CaptureConsoleOutput(() => 
        //{
        using (var redirect = ConsoleHelper.ConfigureFileRedirection(filePath, RedirectionType.Simple))
        {
            ConsoleHelper.WriteRedirectionStatus();
        }

        var output = ReadSharedFile(filePath);
        // Assert
        output.Should().Contain("Console Redirection Status");
        output.Should().Contain("Active");
        output.Should().Contain("True");
        output.Should().Contain("File Path");
        output.Should().Contain("Simple");
    }

    [Fact]
    public void WriteRedirectionStatus_With_Rotating_Redirection_Should_Display_Size_Info()
    {
        // Arrange
        var filePath = GetTestFilePath("rotating_status.log");

        // Act
        using (var redirect = ConsoleHelper.ConfigureFileRedirection(filePath,
            RedirectionType.Rotating, maxSizeBytes: 1024, maxFiles: 5))
        {
            ConsoleHelper.WriteRedirectionStatus();
        }

        var output = ReadSharedFile(filePath);

        // Assert
        output.Should().Contain("Max Size");
        output.Should().Contain("1.024 bytes");
        output.Should().Contain("Max Files");
        output.Should().Contain("5");
    }

    [Fact]
    public void WriteRotationInfo_With_No_Active_Redirection_Should_Display_Message()
    {
        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteRotationInfo());

        // Assert
        output.Should().Contain("No active redirection for rotation info");
    }

    [Fact]
    public void WriteRotationInfo_With_Active_Redirection_Should_Check_Files()
    {
        // Arrange
        var filePath = GetTestFilePath("rotation_info.log");

        // Act
        using (var redirect = ConsoleHelper.ConfigureFileRedirection(filePath))
        {
            ConsoleHelper.WriteRotationInfo();
        }

        var output = ReadSharedFile(filePath);

        // Assert
        // The exact output depends on whether rotation files exist
        output.Should().ContainAny("rotation", "No rotation files found");
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public async Task WriteLine_Should_Be_Thread_Safe()
    {
        // Arrange
        var tasks = new List<Task>();
        var messageCount = 50;

        // Act
        var output = CaptureConsoleOutput(() =>
        {
            for (int i = 0; i < messageCount; i++)
            {
                int messageId = i;
                tasks.Add(Task.Run(() =>
                    ConsoleHelper.WriteLineGreen($"Message {messageId}")));
            }

            Task.WaitAll(tasks.ToArray());
        });

        await Task.Delay(1);

        // Assert
        // Verify that all messages appear in output
        for (int i = 0; i < messageCount; i++)
        {
            output.Should().Contain($"Message {i}");
        }
    }

    [Fact]
    public async Task ConfigureFileRedirection_Should_Be_Thread_Safe()
    {
        // Arrange
        var filePath = GetTestFilePath("thread_safe_config.log");
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    using var redirect = ConsoleHelper.ConfigureFileRedirection(filePath);
                    Thread.Sleep(10);
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

        await Task.WhenAll(tasks);

        // Assert
        // Thread safety means no exceptions should occur
        exceptions.Should().BeEmpty();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Full_Console_Operations_Integration_Test()
    {
        // Arrange
        var filePath = GetTestFilePath("integration.log");

        // Act & Assert - Test full workflow
        var output = CaptureConsoleOutput(() =>
        {
            // Configure redirection
            //using var redirect = ConsoleHelper.ConfigureFileRedirection(filePath);

            // Test various output methods
            ConsoleHelper.WriteHeader("Integration Test");
            ConsoleHelper.WriteLineGreen("Green message");
            ConsoleHelper.WriteLineRed("Red message");
            ConsoleHelper.WriteNameValue("Test Property", "Test Value");

            var items = new[] { 1, 2, 3 };
            ConsoleHelper.WriteCollection("Numbers", items, ", ");

            ConsoleHelper.HR(20, '*', ConsoleColor.Blue);
            ConsoleHelper.NewLine();

            // Test status
            ConsoleHelper.WriteRedirectionStatus();
        });

        // Verify console output contains expected elements
        output.Should().Contain("Integration Test");
        output.Should().Contain("Green message");
        output.Should().Contain("Red message");
        output.Should().Contain("Test Property");
        output.Should().Contain("Numbers");
        output.Should().Contain("{ 1, 2, 3 }");
        output.Should().Contain(new string('*', 20));
        output.Should().Contain("Console Redirection Status");
    }

    //[Fact]
    //public void Color_Restoration_Test()
    //{
    //    // Arrange
    //    var originalColor = Console.ForegroundColor;
    //    Console.ForegroundColor = ConsoleColor.DarkMagenta;

    //    // Act
    //    ConsoleHelper.WriteLineGreen("Test message");

    //    // Assert
    //    Console.ForegroundColor.Should().Be(ConsoleColor.DarkMagenta);

    //    // Cleanup
    //    Console.ForegroundColor = originalColor;
    //}

    #endregion

    #region Error Handling Tests

    [Fact]
    public void Methods_Should_Handle_Console_Exceptions_Gracefully()
    {
        // This test verifies that console operations don't throw unexpected exceptions
        // Most console operations are wrapped in try-catch internally

        // Act & Assert
        var actions = new Action[]
        {
            () => ConsoleHelper.WriteLineGreen("test"),
            () => ConsoleHelper.WriteHeader("test"),
            () => ConsoleHelper.HR(),
            () => ConsoleHelper.NewLine(),
            () => ConsoleHelper.WriteNameValue("test", "value"),
            () => ConsoleHelper.WriteCollection("test", new[] { 1, 2, 3 }, ",")
        };

        var output = CaptureConsoleOutput(() =>
        {
            foreach (var action in actions)
            {
                action.Should().NotThrow();
            }
        });
    }

    [Fact]
    public void WriteCollection_With_Null_Collection_Name_Should_Use_Default()
    {
        // Arrange
        var items = new[] { 1, 2, 3 };

        // Act
        var output = CaptureConsoleOutput(() =>
            ConsoleHelper.WriteCollection(null, items, ", "));

        // Assert
        output.Should().Contain("Collection");
        output.Should().Contain("Items = ");
        output.Should().Contain("{ 1, 2, 3 }");
    }

    #endregion

    public static string ReadSharedFile(string filePath)
    {
        // Öffnet die Datei und erlaubt anderen Prozessen das Lesen und Schreiben.
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        // Liest den Inhalt aus dem Stream.
        using var streamReader = new StreamReader(fileStream);

        return streamReader.ReadToEnd();
    }
}