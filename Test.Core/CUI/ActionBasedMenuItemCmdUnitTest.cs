//--------------------------------------------------------------------------
// File:    ActionBasedMenuItemCmdUnitTest.cs
// Content: Unit tests for ActionBasedMenuItemCmd class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.ComponentModel;
using System.Reflection;

namespace AnBo.Test;

[Collection("Sequential")]
public class ActionBasedMenuItemCmdUnitTest
{
    #region Constructor Tests

    [Fact]
    public void Constructor_With_Synchronous_Action_Should_Initialize_Correctly()
    {
        // Arrange
        var wasCalled = false;
        Action testAction = () => wasCalled = true;

        // Act
        var menuItem = new ActionBasedMenuItemCmd(testAction);

        // Assert
        menuItem.Should().NotBeNull();
        menuItem.IsAsync.Should().BeFalse();
        menuItem.Text.Should().Be("No description available");
    }

    [Fact]
    public void Constructor_With_Asynchronous_Action_Should_Initialize_Correctly()
    {
        // Arrange
        var wasCalled = false;
        Func<Task> testAsyncAction = async () =>
        {
            await Task.Delay(1);
            wasCalled = true;
        };

        // Act
        var menuItem = new ActionBasedMenuItemCmd(testAsyncAction);

        // Assert
        menuItem.Should().NotBeNull();
        menuItem.IsAsync.Should().BeTrue();
        menuItem.Text.Should().Be("No description available");
    }

    [Fact]
    public void Constructor_With_Null_Synchronous_Action_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => new ActionBasedMenuItemCmd((Action)null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("menuItemAction");
    }

    [Fact]
    public void Constructor_With_Null_Asynchronous_Action_Should_Throw_ArgumentNullException()
    {
        // Act & Assert
        var action = () => new ActionBasedMenuItemCmd((Func<Task>)null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("menuItemAsyncAction");
    }

    [Fact]
    public void Constructor_With_Described_Synchronous_Method_Should_Extract_Description()
    {
        // Act
        var menuItem = new ActionBasedMenuItemCmd(TestMethodWithDescription);

        // Assert
        menuItem.Text.Should().Be("Test method with description");
        menuItem.IsAsync.Should().BeFalse();
    }

    [Fact]
    public void Constructor_With_Described_Asynchronous_Method_Should_Extract_Description()
    {
        // Act
        var menuItem = new ActionBasedMenuItemCmd(TestAsyncMethodWithDescription);

        // Assert
        menuItem.Text.Should().Be("Test async method with description");
        menuItem.IsAsync.Should().BeTrue();
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Text_Property_Should_Return_Extracted_Description()
    {
        // Arrange
        var menuItem = new ActionBasedMenuItemCmd(TestMethodWithDescription);

        // Act & Assert
        menuItem.Text.Should().Be("Test method with description");
    }

    [Fact]
    public void Text_Property_Should_Return_Default_For_Undescribed_Method()
    {
        // Arrange
        Action undescribedAction = () => { };
        var menuItem = new ActionBasedMenuItemCmd(undescribedAction);

        // Act & Assert
        menuItem.Text.Should().Be("No description available");
    }

    [Fact]
    public void IsAsync_Property_Should_Return_False_For_Synchronous_Action()
    {
        // Arrange
        var menuItem = new ActionBasedMenuItemCmd(() => { });

        // Act & Assert
        menuItem.IsAsync.Should().BeFalse();
    }

    [Fact]
    public void IsAsync_Property_Should_Return_True_For_Asynchronous_Action()
    {
        // Arrange
        var menuItem = new ActionBasedMenuItemCmd(async () => await Task.Delay(1));

        // Act & Assert
        menuItem.IsAsync.Should().BeTrue();
    }

    #endregion

    #region DoExecute Tests

    [Fact]
    public void DoExecute_With_Synchronous_Action_Should_Execute_Action()
    {
        // Arrange
        var wasCalled = false;
        var menuItem = new ActionBasedMenuItemCmd(() => wasCalled = true);

        // Act
        menuItem.Execute();

        // Assert
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void DoExecute_With_Asynchronous_Action_Should_Execute_Action_Synchronously()
    {
        // Arrange
        var wasCalled = false;
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            await Task.Delay(10);
            wasCalled = true;
        });

        // Act
        menuItem.Execute();

        // Assert
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void DoExecute_With_Exception_In_Synchronous_Action_Should_Propagate_Exception()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var menuItem = new ActionBasedMenuItemCmd(() => throw expectedException);

        // Act & Assert
        var action = () => menuItem.Execute();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Test exception");
    }

    [Fact]
    public void DoExecute_With_Exception_In_Asynchronous_Action_Should_Propagate_Exception()
    {
        // Arrange
        var expectedException = new ArgumentException("Async test exception");
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            await Task.Delay(1);
            throw expectedException;
        });

        // Act & Assert
        var action = () => menuItem.Execute();
        action.Should().Throw<ArgumentException>()
            .WithMessage("Async test exception");
    }

    #endregion

    #region DoExecuteAsync Tests

    [Fact]
    public async Task DoExecuteAsync_With_Synchronous_Action_Should_Execute_Action_Asynchronously()
    {
        // Arrange
        var wasCalled = false;
        var menuItem = new ActionBasedMenuItemCmd(() => wasCalled = true);

        // Act
        await menuItem.ExecuteAsync();

        // Assert
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task DoExecuteAsync_With_Asynchronous_Action_Should_Execute_Action_Asynchronously()
    {
        // Arrange
        var wasCalled = false;
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            await Task.Delay(10);
            wasCalled = true;
        });

        // Act
        await menuItem.ExecuteAsync();

        // Assert
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task DoExecuteAsync_With_Exception_In_Synchronous_Action_Should_Propagate_Exception()
    {
        // Arrange
        var expectedException = new NotSupportedException("Sync async test exception");
        var menuItem = new ActionBasedMenuItemCmd(() => throw expectedException);

        // Act & Assert
        var action = async () => await menuItem.ExecuteAsync();
        await action.Should().ThrowAsync<NotSupportedException>()
            .WithMessage("Sync async test exception");
    }

    [Fact]
    public async Task DoExecuteAsync_With_Exception_In_Asynchronous_Action_Should_Propagate_Exception()
    {
        // Arrange
        var expectedException = new TimeoutException("Async async test exception");
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            await Task.Delay(1);
            throw expectedException;
        });

        // Act & Assert
        var action = async () => await menuItem.ExecuteAsync();
        await action.Should().ThrowAsync<TimeoutException>()
            .WithMessage("Async async test exception");
    }

    [Fact]
    public async Task DoExecuteAsync_Should_Use_ConfigureAwait_False()
    {
        // Arrange
        var taskStarted = false;
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            taskStarted = true;
            await Task.Delay(10);
        });

        // Act
        var task = menuItem.ExecuteAsync();
        
        // Assert - Task should start immediately and not deadlock
        await task;
        taskStarted.Should().BeTrue();
    }

    #endregion

    #region ExtractDescriptionFromMethod Tests

    [Fact]
    public void ExtractDescriptionFromMethod_Should_Return_Description_When_Attribute_Present()
    {
        // Arrange
        var methodInfo = typeof(ActionBasedMenuItemCmdUnitTest)
            .GetMethod(nameof(TestMethodWithDescription), BindingFlags.NonPublic | BindingFlags.Instance)!;

        // Act
        var result = CallExtractDescriptionFromMethod(methodInfo);

        // Assert
        result.Should().Be("Test method with description");
    }

    [Fact]
    public void ExtractDescriptionFromMethod_Should_Return_Default_When_No_Attribute()
    {
        // Arrange
        Action testAction = () => { };
        var methodInfo = testAction.Method;

        // Act
        var result = CallExtractDescriptionFromMethod(methodInfo);

        // Assert
        result.Should().Be("No description available");
    }

    [Fact]
    public void ExtractDescriptionFromMethod_Should_Handle_Empty_Description_Attribute()
    {
        // Arrange
        var methodInfo = typeof(ActionBasedMenuItemCmdUnitTest)
            .GetMethod(nameof(TestMethodWithEmptyDescription), BindingFlags.NonPublic | BindingFlags.Instance)!;

        // Act
        var result = CallExtractDescriptionFromMethod(methodInfo);

        // Assert
        result.Should().Be("");
    }

    [Fact]
    public void ExtractDescriptionFromMethod_Should_Handle_Null_Description_Attribute()
    {
        // Arrange
        var methodInfo = typeof(ActionBasedMenuItemCmdUnitTest)
            .GetMethod(nameof(TestMethodWithNullDescription), BindingFlags.NonPublic | BindingFlags.Instance)!;

        // Act
        var result = CallExtractDescriptionFromMethod(methodInfo);

        // Assert
        result.Should().Be("No description available");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Complete_Workflow_With_Synchronous_Action_Should_Work_End_To_End()
    {
        // Arrange
        var executionOrder = new List<string>();
        var menuItem = new ActionBasedMenuItemCmd(() => executionOrder.Add("executed"));

        // Act
        var text = menuItem.Text;
        var isAsync = menuItem.IsAsync;
        menuItem.Execute();

        // Assert
        text.Should().Be("No description available");
        isAsync.Should().BeFalse();
        executionOrder.Should().ContainSingle().Which.Should().Be("executed");
    }

    [Fact]
    public async Task Complete_Workflow_With_Asynchronous_Action_Should_Work_End_To_End()
    {
        // Arrange
        var executionOrder = new List<string>();
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            await Task.Delay(1);
            executionOrder.Add("async executed");
        });

        // Act
        var text = menuItem.Text;
        var isAsync = menuItem.IsAsync;
        await menuItem.ExecuteAsync();

        // Assert
        text.Should().Be("No description available");
        isAsync.Should().BeTrue();
        executionOrder.Should().ContainSingle().Which.Should().Be("async executed");
    }

    [Fact]
    public void MenuItem_With_Complex_Synchronous_Operation_Should_Execute_Correctly()
    {
        // Arrange
        var results = new List<int>();
        var menuItem = new ActionBasedMenuItemCmd(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                results.Add(i * 2);
            }
        });

        // Act
        menuItem.Execute();

        // Assert
        results.Should().Equal(0, 2, 4, 6, 8);
    }

    [Fact]
    public async Task MenuItem_With_Complex_Asynchronous_Operation_Should_Execute_Correctly()
    {
        // Arrange
        var results = new List<string>();
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1);
                results.Add($"async-{i}");
            }
        });

        // Act
        await menuItem.ExecuteAsync();

        // Assert
        results.Should().Equal("async-0", "async-1", "async-2");
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void MenuItem_Should_Handle_Action_That_Modifies_Static_State()
    {
        // Arrange
        StaticTestState.Value = 0;
        var menuItem = new ActionBasedMenuItemCmd(() => StaticTestState.Value = 42);

        // Act
        menuItem.Execute();

        // Assert
        StaticTestState.Value.Should().Be(42);

        // Cleanup
        StaticTestState.Value = 0;
    }

    [Fact]
    public async Task MenuItem_Should_Handle_Async_Action_That_Returns_Task()
    {
        // Arrange
        var completed = false;
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            await Task.Run(() => Thread.Sleep(10));
            completed = true;
        });

        // Act
        await menuItem.ExecuteAsync();

        // Assert
        completed.Should().BeTrue();
    }

    [Fact]
    public void MenuItem_Should_Handle_Action_With_Capturing_Variables()
    {
        // Arrange
        var capturedValue = "initial";
        var menuItem = new ActionBasedMenuItemCmd(() => capturedValue = "modified");

        // Act
        menuItem.Execute();

        // Assert
        capturedValue.Should().Be("modified");
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public async Task MenuItem_Should_Be_Thread_Safe_For_Multiple_Executions()
    {
        // Arrange
        var executionCount = 0;
        var menuItem = new ActionBasedMenuItemCmd(() => Interlocked.Increment(ref executionCount));
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => menuItem.Execute()));
        }

        await Task.WhenAll(tasks);

        // Assert
        executionCount.Should().Be(10);
    }

    [Fact]
    public async Task Async_MenuItem_Should_Be_Thread_Safe_For_Multiple_Executions()
    {
        // Arrange
        var executionCount = 0;
        var menuItem = new ActionBasedMenuItemCmd(async () =>
        {
            await Task.Delay(1);
            Interlocked.Increment(ref executionCount);
        });
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(menuItem.ExecuteAsync());
        }

        await Task.WhenAll(tasks);

        // Assert
        executionCount.Should().Be(10);
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void Synchronous_Execution_Should_Complete_Quickly()
    {
        // Arrange
        var menuItem = new ActionBasedMenuItemCmd(() => Thread.Sleep(1));
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        menuItem.Execute();
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should complete reasonably quickly
    }

    [Fact]
    public async Task Asynchronous_Execution_Should_Complete_Efficiently()
    {
        // Arrange
        var menuItem = new ActionBasedMenuItemCmd(async () => await Task.Delay(10));
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await menuItem.ExecuteAsync();
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should complete reasonably quickly
    }

    #endregion

    #region Helper Methods and Test Data

    [Description("Test method with description")]
    private void TestMethodWithDescription()
    {
        // Test method for description extraction
    }

    [Description("Test async method with description")]
    private async Task TestAsyncMethodWithDescription()
    {
        await Task.Delay(1);
    }

    [Description("")]
    private void TestMethodWithEmptyDescription()
    {
        // Test method with empty description
    }

    [Description(null!)]
    private void TestMethodWithNullDescription()
    {
        // Test method with null description
    }

    private static string CallExtractDescriptionFromMethod(MethodInfo method)
    {
        // Use reflection to call the private static method
        var extractMethod = typeof(ActionBasedMenuItemCmd)
            .GetMethod("ExtractDescriptionFromMethod", BindingFlags.NonPublic | BindingFlags.Static);

        return (string)extractMethod!.Invoke(null, new object[] { method })!;
    }

    private static class StaticTestState
    {
        public static int Value { get; set; }
    }

    #endregion
}