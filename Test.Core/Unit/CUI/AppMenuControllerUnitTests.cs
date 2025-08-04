//--------------------------------------------------------------------------
// File:    AppMenuControllerUnitTests.cs
// Content: Unit tests for AppMenuController class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using Moq;
using FluentAssertions;
using AnBo.Core;
using System.ComponentModel;

namespace AnBo.Test.Unit;

/// <summary>
/// Unit tests for AppMenuController class
/// Tests menu orchestration, view interaction, and command execution
/// </summary>
[Trait("Category", "Unit")]
public class AppMenuControllerUnitTests
{
    private readonly Mock<IAppMenuView> _mockView;
    private readonly Mock<IEnvironmentService> _mockEnvironment;
    private readonly AppMenuController _controller;
    private bool _testActionExecuted;

    /// <summary>
    /// Test constructor - sets up mocks and system under test
    /// </summary>
    public AppMenuControllerUnitTests()
    {
        _mockView = new Mock<IAppMenuView>();
        _mockEnvironment = new Mock<IEnvironmentService>();
        _controller = new AppMenuController("Test Menu", _mockEnvironment.Object);
        _controller.MenuView = _mockView.Object;
        _testActionExecuted = false;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithHeaderText_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var controller = new AppMenuController("Test Header");

        // Assert
        controller.Count.Should().Be(0);
        controller.MenuView.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHeaderText_ShouldUseDefaultHeader()
    {
        // Arrange & Act
        var controller = new AppMenuController(null);

        // Assert
        controller.MenuView.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithHeaderAndActions_ShouldAddActions()
    {
        // Arrange & Act
        var controller = new AppMenuController("Test", TestAction1, TestAction2);

        // Assert
        controller.Count.Should().Be(2);
    }

    #endregion

    #region MenuView Property Tests

    [Fact]
    public void MenuView_Set_ShouldUpdateView()
    {
        // Arrange
        var newMockView = new Mock<IAppMenuView>();

        // Act
        _controller.MenuView = newMockView.Object;

        // Assert
        _controller.MenuView.Should().Be(newMockView.Object);
    }

    [Fact]
    public void MenuView_SetNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            _controller.MenuView = null!);

        exception.ParamName.Should().Be("value");
    }

    #endregion

    #region Menu Item Management Tests

    [Fact]
    public void Add_MenuItemCommand_ShouldAddToCollection()
    {
        // Arrange
        var mockCommand = new Mock<MenuItemCommandBase>();
        mockCommand.Setup(c => c.Text).Returns("Mock Command");

        // Act
        _controller.Add(mockCommand.Object);

        // Assert
        _controller.Count.Should().Be(1);
    }

    [Fact]
    public void Add_NullMenuItemCommand_ShouldIgnore()
    {
        // Act
        _controller.Add((MenuItemCommandBase?)null);

        // Assert
        _controller.Count.Should().Be(0);
    }

    [Fact]
    public void Add_SyncAction_ShouldWrapInActionCommand()
    {
        // Act
        _controller.Add(TestAction1);

        // Assert
        _controller.Count.Should().Be(1);
    }

    [Fact]
    public void Add_AsyncAction_ShouldWrapInActionCommand()
    {
        // Act
        _controller.Add(TestAsyncAction);

        // Assert
        _controller.Count.Should().Be(1);
    }

    [Fact]
    public void Add_NullAction_ShouldIgnore()
    {
        // Act
        _controller.Add((Action?)null!);

        // Assert
        _controller.Count.Should().Be(0);
    }

    [Fact]
    public void Add_NullAsyncAction_ShouldIgnore()
    {
        // Act
        _controller.Add((Func<Task>?)null!);

        // Assert
        _controller.Count.Should().Be(0);
    }

    [Fact]
    public void Remove_ExistingMenuItem_ShouldReturnTrueAndRemove()
    {
        // Arrange
        var mockCommand = new Mock<MenuItemCommandBase>();
        mockCommand.Setup(c => c.Text).Returns("Mock Command");
        _controller.Add(mockCommand.Object);

        // Act
        var result = _controller.Remove(mockCommand.Object);

        // Assert
        result.Should().BeTrue();
        _controller.Count.Should().Be(0);
    }

    [Fact]
    public void Remove_NonExistingMenuItem_ShouldReturnFalse()
    {
        // Arrange
        var mockCommand = new Mock<MenuItemCommandBase>();

        // Act
        var result = _controller.Remove(mockCommand.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Clear_WithMenuItems_ShouldRemoveAll()
    {
        // Arrange
        _controller.Add(TestAction1);
        _controller.Add(TestAction2);

        // Act
        _controller.Clear();

        // Assert
        _controller.Count.Should().Be(0);
    }

    #endregion

    #region IEnumerable Implementation Tests

    [Fact]
    public void GetEnumerator_ShouldReturnAllMenuItems()
    {
        // Arrange
        _controller.Add(TestAction1);
        _controller.Add(TestAction2);

        // Act
        var items = _controller.ToList();

        // Assert
        items.Should().HaveCount(2);
        items.All(item => item is ActionBasedMenuItemCmd).Should().BeTrue();
    }

    [Fact]
    public void ForEach_ShouldIterateAllMenuItems()
    {
        // Arrange
        _controller.Add(TestAction1);
        _controller.Add(TestAction2);
        var count = 0;

        // Act
        foreach (var item in _controller)
        {
            count++;
            item.Should().NotBeNull();
        }

        // Assert
        count.Should().Be(2);
    }

    #endregion

    #region Run Method Tests

    [Fact]
    public void Run_WithImmediateQuit_ShouldInitializeViewAndExit()
    {
        // Arrange
        _mockView.Setup(v => v.ShouldQuit).Returns(true); // Exit immediately

        _controller.Add(TestAction1);

        // Act & Assert
        _controller.Run();

        // Verify the workflow
        _mockView.Verify(v => v.InitView(It.IsAny<IEnumerable<string>>()), Times.Once);
        _mockView.Verify(v => v.ShouldQuit, Times.Once);

        // Display and prompt should not be called since ShouldQuit is true immediately
        _mockView.Verify(v => v.DisplayMenu(), Times.Never);
        _mockView.Verify(v => v.PromptToContinue(), Times.Never);

        // Most importantly: Environment.Exit should be called
        _mockEnvironment.Verify(e => e.Exit(0), Times.Once);

    }

    [Fact]
    public void Run_WithSingleMenuInteraction_ShouldExecuteCompleteWorkflow()
    {
        var actionExecuted = false;

        _controller.Add(() => actionExecuted = true );

        // Setup view to simulate one menu interaction
        _mockView.SetupSequence(v => v.ShouldQuit)
            .Returns(false) // First call: continue
            .Returns(true); // Second call: exit

        _mockView.Setup(v => v.WaitForValidUserInput()).Returns(0); // Select first option

        _controller.Run();

        actionExecuted.Should().BeTrue();

        // Assert - Verify complete workflow execution
        // 1. Initialization
        _mockView.Verify(v => v.InitView(It.IsAny<IEnumerable<string>>()), Times.Once);

        // 2. Menu interaction
        _mockView.Verify(v => v.ShouldQuit, Times.Exactly(2));
        _mockView.Verify(v => v.DisplayMenu(), Times.Once);
        _mockView.Verify(v => v.WaitForValidUserInput(), Times.Once);
        _mockView.Verify(v => v.ClearView(), Times.Once);
        _mockView.Verify(v => v.WriteMenuOperationHeader(It.IsAny<string>()), Times.Once);
        _mockView.Verify(v => v.PromptToContinue(), Times.Once);

        // 3. Menu action execution
        actionExecuted.Should().BeTrue();

        // 4. Exit
        _mockEnvironment.Verify(e => e.Exit(0), Times.Once);
    }

    [Fact]
    public void MenuInitialization_ShouldPropagateMenuTexts()
    {
        // Arrange
        _controller.Add(TestAction1);
        _controller.Add(TestAction2);

        // Act - Get menu texts that would be passed to InitView
        var menuTexts = _controller.Select(item => item.Text).ToList();

        // Assert
        menuTexts.Should().HaveCount(2);
        menuTexts[0].Should().Be("Test Action 1");
        menuTexts[1].Should().Be("Test Action 2");
    }

    #endregion

    #region Menu Item Execution Tests

    [Fact]
    public async Task MenuItemExecution_DirectCall_ShouldExecuteCorrectly()
    {
        // Arrange
        var executionFlag = false;
        var testAction = new ActionBasedMenuItemCmd(() => executionFlag = true);
        _controller.Add(testAction);

        // Act
        var menuItem = _controller.First();
        await menuItem.ExecuteAsync();

        // Assert
        executionFlag.Should().BeTrue();
    }

    [Fact]
    public async Task MenuItemExecution_WithException_ShouldPropagateException()
    {
        // Arrange
        var exceptionAction = new ActionBasedMenuItemCmd(() =>
            throw new InvalidOperationException("Test exception"));
        _controller.Add(exceptionAction);

        // Act & Assert
        var menuItem = _controller.First();
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => menuItem.ExecuteAsync());

        exception.Message.Should().Be("Test exception");
    }

    #endregion

    #region Test Helper Methods

    /// <summary>
    /// Test action 1 with description attribute
    /// </summary>
    [Description("Test Action 1")]
    private void TestAction1()
    {
        _testActionExecuted = true;
    }

    /// <summary>
    /// Test action 2 with description attribute
    /// </summary>
    [Description("Test Action 2")]
    private void TestAction2()
    {
        _testActionExecuted = true;
    }

    /// <summary>
    /// Test async action with description attribute
    /// </summary>
    [Description("Async Test Action")]
    private async Task TestAsyncAction()
    {
        await Task.CompletedTask;
        _testActionExecuted = true;
    }

    #endregion
}

