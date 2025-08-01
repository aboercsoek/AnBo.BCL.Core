using System.ComponentModel;
using Xunit;
using FluentAssertions;
using AnBo.Core;

namespace AnBo.Test;

/// <summary>
/// Unit tests for ActionBasedMenuItemCmd class
/// Tests delegate-based menu command implementation
/// </summary>
public class ActionBasedMenuItemCmdTests
{
    private bool _syncActionExecuted;
    private bool _asyncActionExecuted;
    private int _executionCount;

    /// <summary>
    /// Test constructor - resets execution flags
    /// </summary>
    public ActionBasedMenuItemCmdTests()
    {
        ResetExecutionFlags();
    }

    private void ResetExecutionFlags()
    {
        _syncActionExecuted = false;
        _asyncActionExecuted = false;
        _executionCount = 0;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithSyncAction_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var command = new ActionBasedMenuItemCmd(TestSyncAction);

        // Assert
        command.IsAsync.Should().BeFalse();
        command.Text.Should().Be("Synchronous Test Action");
    }

    [Fact]
    public void Constructor_WithAsyncAction_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var command = new ActionBasedMenuItemCmd(TestAsyncAction);

        // Assert
        command.IsAsync.Should().BeTrue();
        command.Text.Should().Be("Asynchronous Test Action");
    }

    [Fact]
    public void Constructor_WithNullSyncAction_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new ActionBasedMenuItemCmd((Action)null!));

        exception.ParamName.Should().Be("menuItemAction");
    }

    [Fact]
    public void Constructor_WithNullAsyncAction_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new ActionBasedMenuItemCmd((Func<Task>)null!));

        exception.ParamName.Should().Be("menuItemAsyncAction");
    }

    #endregion

    #region Execution Tests

    [Fact]
    public void Execute_WithSyncAction_ShouldExecuteAction()
    {
        // Arrange
        var command = new ActionBasedMenuItemCmd(TestSyncAction);

        // Act
        command.Execute();

        // Assert
        _syncActionExecuted.Should().BeTrue();
        _executionCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_WithSyncAction_ShouldExecuteActionAsync()
    {
        // Arrange
        var command = new ActionBasedMenuItemCmd(TestSyncAction);

        // Act
        await command.ExecuteAsync();

        // Assert
        _syncActionExecuted.Should().BeTrue();
        _executionCount.Should().Be(1);
    }

    [Fact]
    public void Execute_WithAsyncAction_ShouldExecuteActionSynchronously()
    {
        // Arrange
        var command = new ActionBasedMenuItemCmd(TestAsyncAction);

        // Act
        command.Execute();

        // Assert
        _asyncActionExecuted.Should().BeTrue();
        _executionCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_WithAsyncAction_ShouldExecuteActionAsync()
    {
        // Arrange
        var command = new ActionBasedMenuItemCmd(TestAsyncAction);

        // Act
        await command.ExecuteAsync();

        // Assert
        _asyncActionExecuted.Should().BeTrue();
        _executionCount.Should().Be(1);
    }

    #endregion

    #region Exception Handling Tests

    [Fact]
    public void Execute_WhenSyncActionThrows_ShouldPropagateException()
    {
        // Arrange
        var command = new ActionBasedMenuItemCmd(ThrowingSyncAction);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => command.Execute());
        exception.Message.Should().Be("Sync action exception");
    }

    [Fact]
    public async Task ExecuteAsync_WhenAsyncActionThrows_ShouldPropagateException()
    {
        // Arrange
        var command = new ActionBasedMenuItemCmd(ThrowingAsyncAction);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => command.ExecuteAsync());
        exception.Message.Should().Be("Async action exception");
    }

    #endregion

    #region Text Extraction Tests

    [Fact]
    public void Text_WithMethodWithoutDescription_ShouldReturnDefaultMessage()
    {
        // Arrange & Act
        var command = new ActionBasedMenuItemCmd(ActionWithoutDescription);

        // Assert
        command.Text.Should().Be("No description available");
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task ExecuteAsync_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var command = new ActionBasedMenuItemCmd(TestAsyncActionWithDelay);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await command.ExecuteAsync();
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(40); // At least the delay
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(900); // But not too much overhead
        _asyncActionExecuted.Should().BeTrue();
    }

    #endregion

    #region Test Helper Methods

    /// <summary>
    /// Test synchronous action with description attribute
    /// </summary>
    [Description("Synchronous Test Action")]
    private void TestSyncAction()
    {
        _syncActionExecuted = true;
        _executionCount++;
    }

    /// <summary>
    /// Test asynchronous action with description attribute
    /// </summary>
    [Description("Asynchronous Test Action")]
    private async Task TestAsyncAction()
    {
        await Task.CompletedTask; // Simulate async work
        _asyncActionExecuted = true;
        _executionCount++;
    }

    /// <summary>
    /// Test async action with actual delay
    /// </summary>
    [Description("Async Action with Delay")]
    private async Task TestAsyncActionWithDelay()
    {
        await Task.Delay(50);
        _asyncActionExecuted = true;
        _executionCount++;
    }

    /// <summary>
    /// Action without description attribute for testing default behavior
    /// </summary>
    private void ActionWithoutDescription()
    {
        _syncActionExecuted = true;
    }

    /// <summary>
    /// Sync action that throws exception for testing error handling
    /// </summary>
    [Description("Throwing Sync Action")]
    private void ThrowingSyncAction()
    {
        throw new InvalidOperationException("Sync action exception");
    }

    /// <summary>
    /// Async action that throws exception for testing error handling
    /// </summary>
    [Description("Throwing Async Action")]
    private async Task ThrowingAsyncAction()
    {
        await Task.CompletedTask;
        throw new InvalidOperationException("Async action exception");
    }

    #endregion
}