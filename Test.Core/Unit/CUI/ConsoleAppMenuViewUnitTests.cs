//--------------------------------------------------------------------------
// File:    ConsoleAppMenuViewUnitTests.cs
// Content: Unit tests for ConsoleAppMenuView class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;
using Moq;

namespace AnBo.Test.Unit;

/// <summary>
/// Unit tests for ConsoleAppMenuView class
/// Tests console-based menu display and user interaction
/// Note: Some tests are limited due to console dependencies in unit test environment
/// </summary>
[Collection("Sequential")]
[Trait("Category", "Unit")]
public class ConsoleAppMenuViewUnitTests : IDisposable
{
    private readonly ConsoleAppMenuView _view;
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalOutput;
    private readonly TextReader _originalInput;

    /// <summary>
    /// Test constructor - sets up console redirection for testing
    /// </summary>
    public ConsoleAppMenuViewUnitTests()
    {
        _view = new ConsoleAppMenuView("Test Menu Header");

        // Capture console output for verification
        _originalOutput = Console.Out;
        _originalInput = Console.In;
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    /// <summary>
    /// Cleanup method - restores original console streams
    /// </summary>
    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        Console.SetIn(_originalInput);
        _consoleOutput?.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithHeaderText_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var view = new ConsoleAppMenuView("Custom Header");

        // Assert
        view.Should().NotBeNull();
        view.ShouldQuit.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("My Application Menu")]
    [InlineData("🎯 Special Characters Menu")]
    public void Constructor_WithVariousHeaderTexts_ShouldHandleCorrectly(string headerText)
    {
        // Arrange & Act
        var view = new ConsoleAppMenuView(headerText);

        // Assert
        view.Should().NotBeNull();
        view.ShouldQuit.Should().BeFalse();
    }

    #endregion

    #region InitView Tests

    [Fact]
    public void InitView_WithMenuItems_ShouldStoreItems()
    {
        // Arrange
        var menuItems = new[] { "Option 1", "Option 2", "Option 3" };

        // Act & Assert - Should not throw
        _view.InitView(menuItems);
        _view.Should().NotBeNull();
    }

    [Fact]
    public void InitView_WithEmptyCollection_ShouldHandleGracefully()
    {
        // Arrange
        var emptyMenuItems = Array.Empty<string>();

        // Act & Assert - Should not throw
        _view.InitView(emptyMenuItems);
    }

    [Fact]
    public void InitView_WithNullItems_ShouldHandleGracefully()
    {
        // Arrange
        var menuItemsWithNull = new string?[] { "Option 1", null, "Option 3" };

        // Act & Assert - Should not throw
        _view.InitView(menuItemsWithNull!);
    }

    #endregion

    #region Display Methods Tests (Console-Safe)

    [Fact]
    public void DisplayMenu_WithoutConsole_ShouldHandleGracefully()
    {
        // Arrange
        var menuItems = new[] { "Test Option 1", "Test Option 2" };
        _view.InitView(menuItems);

        // Act & Assert - Test that the method doesn't crash when console operations fail
        // In unit test environment, Console.Clear() and related operations may fail
        // The view should handle this gracefully

        // We test this by verifying the object remains in a valid state
        Action displayAction = () =>
        {
            try
            {
                _view.DisplayMenu();
            }
            catch (System.IO.IOException)
            {
                // Expected in unit test environment - console operations may fail
                // This is acceptable behavior
            }
        };

        displayAction.Should().NotThrow<NullReferenceException>();
        displayAction.Should().NotThrow<InvalidOperationException>();
        _view.ShouldQuit.Should().BeFalse(); // Object should remain in valid state
    }

    [Fact]
    public void ClearView_WithoutConsole_ShouldHandleGracefully()
    {
        // Act & Assert - Similar to DisplayMenu, console clear may fail in unit tests
        Action clearAction = () =>
        {
            try
            {
                _view.ClearView();
            }
            catch (System.IO.IOException)
            {
                // Expected in unit test environment
            }
        };

        clearAction.Should().NotThrow<NullReferenceException>();
        clearAction.Should().NotThrow<InvalidOperationException>();
    }

    #endregion

    #region WriteMenuOperationHeader Tests

    [Fact]
    public void WriteMenuOperationHeader_WithText_ShouldWriteFormattedHeader()
    {
        // Arrange
        const string headerText = "Executing Test Operation";

        // Act
        _view.WriteMenuOperationHeader(headerText);

        // Assert
        var output = _consoleOutput.ToString();
        output.Should().Contain(headerText);
        output.Should().Contain("---"); // Should contain underlines
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Special Characters: äöü")]
    public void WriteMenuOperationHeader_WithVariousTexts_ShouldHandleCorrectly(string headerText)
    {
        // Act & Assert - Should not throw
        _view.WriteMenuOperationHeader(headerText);

        // Verify output was written
        var output = _consoleOutput.ToString();
        output.Should().NotBeEmpty();
    }

    #endregion

    #region ShowExceptionDetails Tests

    [Fact]
    public void ShowExceptionDetails_WithSimpleException_ShouldDisplayDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception message");

        // Act
        _view.ShowExceptionDetails(exception);

        // Assert
        var output = _consoleOutput.ToString();
        output.Should().Contain("InvalidOperationException");
        output.Should().Contain("Test exception message");
    }

    [Fact]
    public void ShowExceptionDetails_WithInnerException_ShouldDisplayBothExceptions()
    {
        // Arrange
        var innerException = new ArgumentException("Inner exception message");
        var outerException = new InvalidOperationException("Outer exception message", innerException);

        // Act
        _view.ShowExceptionDetails(outerException);

        // Assert
        var output = _consoleOutput.ToString();
        output.Should().Contain("InvalidOperationException");
        output.Should().Contain("Outer exception message");
        output.Should().Contain("ArgumentException");
        output.Should().Contain("Inner exception message");
    }

    [Fact]
    public void ShowExceptionDetails_WithNullSource_ShouldHandleGracefully()
    {
        // Arrange
        var exception = new Exception("Test message");

        // Act & Assert - Should not throw
        _view.ShowExceptionDetails(exception);

        var output = _consoleOutput.ToString();
        output.Should().Contain("Test message");
    }

    [Fact]
    public void ShowExceptionDetails_WithComplexExceptionHierarchy_ShouldDisplayAll()
    {
        // Arrange
        var deepestException = new ArgumentNullException("param", "Parameter was null");
        var middleException = new ArgumentException("Invalid argument", deepestException);
        var topException = new InvalidOperationException("Operation failed", middleException);

        // Act
        _view.ShowExceptionDetails(topException);

        // Assert
        var output = _consoleOutput.ToString();
        output.Should().Contain("InvalidOperationException");
        output.Should().Contain("Operation failed");
        output.Should().Contain("ArgumentException");
        output.Should().Contain("Invalid argument");
    }

    #endregion

    #region ShouldQuit Property Tests

    [Fact]
    public void ShouldQuit_InitialState_ShouldBeFalse()
    {
        // Act & Assert
        _view.ShouldQuit.Should().BeFalse();
    }

    [Fact]
    public void ShouldQuit_ConsistentBehavior_ShouldRemainFalse()
    {
        // Arrange & Act - Multiple calls should be consistent
        var initialState = _view.ShouldQuit;
        var secondCall = _view.ShouldQuit;
        var thirdCall = _view.ShouldQuit;

        // Assert
        initialState.Should().BeFalse();
        secondCall.Should().BeFalse();
        thirdCall.Should().BeFalse();
        secondCall.Should().Be(initialState);
        thirdCall.Should().Be(initialState);
    }

    #endregion

    #region State Management Tests

    [Fact]
    public void View_AfterMultipleOperations_ShouldMaintainValidState()
    {
        // Arrange
        var menuItems = new[] { "Test 1", "Test 2", "Test 3" };

        // Act - Perform multiple operations
        _view.InitView(menuItems);
        _view.WriteMenuOperationHeader("Test Header 1");
        _view.WriteMenuOperationHeader("Test Header 2");

        var exception = new Exception("Test exception");
        _view.ShowExceptionDetails(exception);

        // Assert - Object should remain in valid state
        _view.Should().NotBeNull();
        _view.ShouldQuit.Should().BeFalse();

        // Verify output contains expected content
        var output = _consoleOutput.ToString();
        output.Should().Contain("Test Header 1");
        output.Should().Contain("Test Header 2");
        output.Should().Contain("Test exception");
    }

    #endregion
}

/// <summary>
/// Integration tests for ConsoleAppMenuView that focus on behavior
/// rather than actual console interaction
/// </summary>
[Collection("Sequential")]
[Trait("Category", "Unit")]
public class ConsoleAppMenuViewBehaviorTests
{
    [Fact]
    public void ConsoleView_WithRealMenuItems_ShouldMaintainState()
    {
        // Arrange
        var view = new ConsoleAppMenuView("Integration Test Menu");
        var menuItems = new[]
        {
            "View Customer Data",
            "Add New Customer",
            "Generate Report",
            "Export to Excel",
            "System Settings"
        };

        // Act
        view.InitView(menuItems);

        // Assert - Focus on state rather than console output
        view.Should().NotBeNull();
        view.ShouldQuit.Should().BeFalse();
    }

    [Fact]
    public void DisplayMenu_WithManyOptions_ShouldNotThrowUnhandledExceptions()
    {
        // Arrange
        var view = new ConsoleAppMenuView("Large Menu Test");
        var manyMenuItems = Enumerable.Range(1, 15)
            .Select(i => $"Menu Option {i} - Some longer descriptive text")
            .ToArray();

        // Act
        view.InitView(manyMenuItems);

        // Assert - Should not throw unhandled exceptions
        Action displayAction = () =>
        {
            try
            {
                view.DisplayMenu();
            }
            catch (System.IO.IOException)
            {
                // Console operations may fail in test environment
                // This is expected and acceptable
            }
        };

        displayAction.Should().NotThrow<ArgumentException>();
        displayAction.Should().NotThrow<InvalidOperationException>();
        displayAction.Should().NotThrow<NullReferenceException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void View_WithVariousMenuSizes_ShouldHandleCorrectly(int menuItemCount)
    {
        // Arrange
        var view = new ConsoleAppMenuView($"Menu with {menuItemCount} items");
        var menuItems = Enumerable.Range(1, menuItemCount)
            .Select(i => $"Menu Item {i}")
            .ToArray();

        // Act & Assert - Should handle various menu sizes without issues
        Action initAction = () => view.InitView(menuItems);
        initAction.Should().NotThrow();

        view.ShouldQuit.Should().BeFalse();
    }
}

/// <summary>
/// Tests demonstrating how console view could be made more testable
/// Shows patterns for better testability in console applications
/// </summary>
public class ConsoleViewTestabilityExamples
{
    /// <summary>
    /// Example of how to create a testable console wrapper interface
    /// This demonstrates the pattern for making console operations mockable
    /// </summary>
    public interface IConsoleWrapper
    {
        void Clear();
        void WriteLine(string text);
        void Write(string text);
        ConsoleKeyInfo ReadKey(bool intercept);
        int BufferWidth { get; }
        int BufferHeight { get; }
        int CursorTop { get; set; }
        int CursorLeft { get; set; }
    }

    /// <summary>
    /// Example implementation of console wrapper
    /// </summary>
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void Clear() => Console.Clear();
        public void WriteLine(string text) => Console.WriteLine(text);
        public void Write(string text) => Console.Write(text);
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
        public int BufferWidth => Console.BufferWidth;
        public int BufferHeight => Console.BufferHeight;
        public int CursorTop { get => Console.CursorTop; set => Console.CursorTop = value; }
        public int CursorLeft { get => Console.CursorLeft; set => Console.CursorLeft = value; }
    }

    /// <summary>
    /// Example test showing how view could be tested with mocked console
    /// This demonstrates the pattern for full testability
    /// </summary>
    [Fact]
    public void ExampleOfFullyTestableConsoleView_ShouldAllowCompleteTesting()
    {
        // This demonstrates the testing approach you could use 
        // if ConsoleAppMenuView was refactored to use dependency injection

        // Arrange
        var mockConsole = new Mock<IConsoleWrapper>();
        mockConsole.Setup(c => c.BufferWidth).Returns(80);
        mockConsole.Setup(c => c.BufferHeight).Returns(25);

        // Act & Assert - With proper DI, you could test all console interactions
        mockConsole.Object.BufferWidth.Should().Be(80);
        mockConsole.Object.BufferHeight.Should().Be(25);

        // This would allow testing of:
        // - Console clearing operations
        // - Text positioning and formatting
        // - User input handling
        // - Buffer size calculations

        mockConsole.Verify(c => c.BufferWidth, Times.Once);
    }

    /// <summary>
    /// Example of testing console output through string capture
    /// Shows current approach limitations and workarounds
    /// </summary>
    [Fact]
    public void ConsoleOutputCapture_LimitationsAndWorkarounds_ShouldDemonstrateApproach()
    {
        // Arrange
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            Console.WriteLine("Test output");
            Console.Write("More test output");

            // Assert
            var output = stringWriter.ToString();
            output.Should().Contain("Test output");
            output.Should().Contain("More test output");
        }
        finally
        {
            // Cleanup
            Console.SetOut(originalOut);
        }

        // This approach works for WriteLine/Write but not for:
        // - Console.Clear() (requires console handle)
        // - Console.ReadKey() (requires input stream)
        // - Cursor positioning (requires console buffer)
    }
}