using Xunit;
using FluentAssertions;
using AnBo.Core;

namespace AnBo.Test;

/// <summary>
/// Unit tests for MenuItemCommandBase abstract class
/// Tests the command pattern implementation and async behavior
/// </summary>
public class MenuItemCommandBaseTests
{
    /// <summary>
    /// Test implementation of MenuItemCommandBase for testing purposes
    /// </summary>
    private class TestMenuCommand : MenuItemCommandBase
    {
        public bool DoExecuteCalled { get; private set; }
        public bool DoExecuteAsyncCalled { get; private set; }
        public Exception? ExceptionToThrow { get; set; }
        public int ExecutionDelay { get; set; } = 0;

        public override string Text => "Test Menu Item";

        protected override void DoExecute()
        {
            DoExecuteCalled = true;

            if (ExceptionToThrow != null)
                throw ExceptionToThrow;

            if (ExecutionDelay > 0)
                Thread.Sleep(ExecutionDelay);
        }

        protected override async Task DoExecuteAsync()
        {
            DoExecuteAsyncCalled = true;

            if (ExceptionToThrow != null)
                throw ExceptionToThrow;

            if (ExecutionDelay > 0)
                await Task.Delay(ExecutionDelay);
        }

        // Reset method for test isolation
        public void Reset()
        {
            DoExecuteCalled = false;
            DoExecuteAsyncCalled = false;
            ExceptionToThrow = null;
            ExecutionDelay = 0;
        }
    }

    private readonly TestMenuCommand _testCommand;

    /// <summary>
    /// Test constructor - creates fresh test command for each test
    /// </summary>
    public MenuItemCommandBaseTests()
    {
        _testCommand = new TestMenuCommand();
    }

    [Fact]
    public void Execute_ShouldCallDoExecute()
    {
        // Act
        _testCommand.Execute();

        // Assert
        _testCommand.DoExecuteCalled.Should().BeTrue();
        _testCommand.DoExecuteAsyncCalled.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallDoExecuteAsync()
    {
        // Act
        await _testCommand.ExecuteAsync();

        // Assert
        _testCommand.DoExecuteAsyncCalled.Should().BeTrue();
        _testCommand.DoExecuteCalled.Should().BeFalse();
    }

    [Fact]
    public void Execute_WhenExceptionThrown_ShouldPropagateException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        _testCommand.ExceptionToThrow = expectedException;

        // Act & Assert
        var thrownException = Assert.Throws<InvalidOperationException>(() => _testCommand.Execute());
        thrownException.Message.Should().Be("Test exception");
    }

    [Fact]
    public async Task ExecuteAsync_WhenExceptionThrown_ShouldPropagateException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Async test exception");
        _testCommand.ExceptionToThrow = expectedException;

        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _testCommand.ExecuteAsync());
        thrownException.Message.Should().Be("Async test exception");
    }

    [Fact]
    public void Text_ShouldReturnCorrectValue()
    {
        // Act & Assert
        _testCommand.Text.Should().Be("Test Menu Item");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCompleteAsynchronously()
    {
        // Arrange
        _testCommand.ExecutionDelay = 50; // Small delay to test async behavior
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await _testCommand.ExecuteAsync();
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(40); // Allow some variance
        _testCommand.DoExecuteAsyncCalled.Should().BeTrue();
    }
}