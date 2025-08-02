//--------------------------------------------------------------------------
// File:    AppMenuControllerIntegrationTests.cs
// Content:	Integration tests for class AppMenuController
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;
using Moq;
using System.ComponentModel;

namespace AnBo.Test.Integration;

/// <summary>
/// Integration tests for AppMenuController with real view
/// Tests the complete workflow without excessive mocking
/// </summary>
[Trait("Category", "Integration")]
public class AppMenuControllerIntegrationTests
{
    [Fact]
    public void AppMenuController_WithRealView_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var controller = new AppMenuController("Integration Test Menu");
        controller.Add(() => Console.WriteLine("Test"));

        // Assert
        controller.Count.Should().Be(1);
        controller.MenuView.Should().BeOfType<ConsoleAppMenuView>();
    }

    [Fact]
    public void MenuItems_ShouldHaveCorrectTextFromDescriptionAttributes()
    {
        // Arrange
        var controller = new AppMenuController("Test Menu");

        // Act
        controller.Add(TestMenuAction);
        var menuItem = controller.First();

        // Assert
        menuItem.Text.Should().Be("Integration Test Menu Action");
    }

    [Fact]
    public async Task MenuExecution_CompleteWorkflow_ShouldWork()
    {
        // Arrange
        var controller = new AppMenuController("Workflow Test");
        var results = new List<string>();

        controller.Add(() => results.Add("Step1"));
        controller.Add(async () =>
        {
            await Task.Delay(1);
            results.Add("Step2");
        });
        controller.Add(() => results.Add("Step3"));

        // Act - Execute all menu items in sequence
        foreach (var menuItem in controller)
        {
            await menuItem.ExecuteAsync();
        }

        // Assert
        results.Should().Equal("Step1", "Step2", "Step3");
    }

    [Fact]
    public void MenuController_ErrorHandling_ShouldPropagateExceptions()
    {
        // Arrange
        var controller = new AppMenuController("Error Test Menu");
        controller.Add(() => throw new ArgumentException("Test error"));

        // Act & Assert
        var menuItem = controller.First();
        var exception = Assert.Throws<ArgumentException>(() => menuItem.Execute());
        exception.Message.Should().Be("Test error");
    }

    [Fact]
    public void MenuView_Replacement_ShouldWorkCorrectly()
    {
        // Arrange
        var controller = new AppMenuController("View Test");
        var mockView = new Mock<IAppMenuView>();
        mockView.Setup(v => v.ShouldQuit).Returns(false);

        var originalView = controller.MenuView;

        // Act
        controller.MenuView = mockView.Object;

        // Assert
        controller.MenuView.Should().Be(mockView.Object);
        controller.MenuView.Should().NotBe(originalView);
    }

    [Fact]
    public void MenuController_WithRealComponents_ShouldWorkCorrectly()
    {
        // Arrange - Use real ConsoleAppMenuView for integration testing
        var realController = new AppMenuController("Integration Test Menu");
        var executionCount = 0;

        realController.Add(() => executionCount++);
        realController.Add(async () =>
        {
            await Task.Delay(1);
            executionCount++;
        });

        // Act - Test direct menu item execution
        var menuItems = realController.ToList();

        // Execute sync item
        menuItems[0].Execute();

        // Execute async item
        menuItems[1].ExecuteAsync().GetAwaiter().GetResult();

        // Assert
        executionCount.Should().Be(2);
        realController.Count.Should().Be(2);
        realController.MenuView.Should().BeOfType<ConsoleAppMenuView>();
    }

    /// <summary>
    /// Test menu action for integration testing with proper description
    /// </summary>
    [Description("Integration Test Menu Action")]
    private static void TestMenuAction()
    {
        // Test action implementation
    }
}
