using System.ComponentModel;
using AnBo.Core;
using Moq;

namespace AnBo.Test.Helpers;

/// <summary>
/// Builder pattern for creating test menu controllers with fluent API
/// Simplifies test setup and makes tests more readable
/// </summary>
public class TestMenuControllerBuilder
{
    private readonly AppMenuController _controller;
    private readonly Mock<IAppMenuView> _mockView;

    /// <summary>
    /// Initializes a new test menu controller builder
    /// </summary>
    /// <param name="headerText">The header text for the menu</param>
    public TestMenuControllerBuilder(string headerText = "Test Menu")
    {
        _controller = new AppMenuController(headerText);
        _mockView = new Mock<IAppMenuView>();
        _controller.MenuView = _mockView.Object;
    }

    /// <summary>
    /// Adds a synchronous test action to the menu
    /// </summary>
    /// <param name="actionName">Name for the action (used in description)</param>
    /// <param name="action">The action to execute</param>
    /// <returns>This builder for method chaining</returns>
    public TestMenuControllerBuilder WithSyncAction(string actionName, Action action)
    {
        // Create a test command with the specified name and action
        var testCommand = new TestMenuCommand(actionName, action);
        _controller.Add(testCommand);
        return this;
    }

    /// <summary>
    /// Adds an asynchronous test action to the menu
    /// </summary>
    /// <param name="actionName">Name for the action (used in description)</param>
    /// <param name="asyncAction">The async action to execute</param>
    /// <returns>This builder for method chaining</returns>
    public TestMenuControllerBuilder WithAsyncAction(string actionName, Func<Task> asyncAction)
    {
        var testCommand = new TestMenuCommand(actionName, asyncAction);
        _controller.Add(testCommand);
        return this;
    }

    /// <summary>
    /// Adds a test action that throws an exception
    /// </summary>
    /// <param name="actionName">Name for the action</param>
    /// <param name="exception">The exception to throw</param>
    /// <returns>This builder for method chaining</returns>
    public TestMenuControllerBuilder WithThrowingAction(string actionName, Exception exception)
    {
        Action throwingAction = () => throw exception;
        var testCommand = new TestMenuCommand($"{actionName} Action", throwingAction);
        _controller.Add(testCommand);
        return this;
    }

    /// <summary>
    /// Configures the mock view to quit immediately
    /// </summary>
    /// <returns>This builder for method chaining</returns>
    public TestMenuControllerBuilder WithImmediateQuit()
    {
        _mockView.Setup(v => v.ShouldQuit).Returns(true);
        return this;
    }

    /// <summary>
    /// Configures the mock view with specific user input sequence
    /// </summary>
    /// <param name="userInputs">Sequence of user menu selections</param>
    /// <returns>This builder for method chaining</returns>
    public TestMenuControllerBuilder WithUserInputSequence(params int[] userInputs)
    {
        var sequence = _mockView.SetupSequence(v => v.WaitForValidUserInput());
        foreach (var input in userInputs)
        {
            sequence.Returns(input);
        }
        return this;
    }

    /// <summary>
    /// Builds the configured controller and mock view
    /// </summary>
    /// <returns>Tuple containing the controller and mock view</returns>
    public (AppMenuController Controller, Mock<IAppMenuView> MockView) Build()
    {
        return (_controller, _mockView);
    }
}

/// <summary>
/// Test-specific implementation of MenuItemCommandBase for builder pattern
/// Allows dynamic creation of menu commands with custom text and actions
/// </summary>
internal class TestMenuCommand : MenuItemCommandBase
{
    private readonly string _text;
    private readonly Action? _syncAction;
    private readonly Func<Task>? _asyncAction;
    private readonly bool _isAsync;

    /// <summary>
    /// Initializes a test menu command with synchronous action
    /// </summary>
    /// <param name="text">Display text for the command</param>
    /// <param name="syncAction">Synchronous action to execute</param>
    public TestMenuCommand(string text, Action syncAction)
    {
        _text = text;
        _syncAction = syncAction;
        _isAsync = false;
    }

    /// <summary>
    /// Initializes a test menu command with asynchronous action
    /// </summary>
    /// <param name="text">Display text for the command</param>
    /// <param name="asyncAction">Asynchronous action to execute</param>
    public TestMenuCommand(string text, Func<Task> asyncAction)
    {
        _text = text;
        _asyncAction = asyncAction;
        _isAsync = true;
    }

    /// <summary>
    /// Gets the display text for this menu command
    /// </summary>
    public override string Text => _text;

    /// <summary>
    /// Executes the menu command synchronously
    /// </summary>
    protected override void DoExecute()
    {
        if (_isAsync && _asyncAction != null)
        {
            // Execute async action synchronously
            _asyncAction().GetAwaiter().GetResult();
        }
        else if (!_isAsync && _syncAction != null)
        {
            _syncAction();
        }
        else
        {
            throw new InvalidOperationException("No valid action configured for test command");
        }
    }

    /// <summary>
    /// Executes the menu command asynchronously
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    protected override async Task DoExecuteAsync()
    {
        if (_isAsync && _asyncAction != null)
        {
            await _asyncAction().ConfigureAwait(false);
        }
        else if (!_isAsync && _syncAction != null)
        {
            await Task.Run(_syncAction).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException("No valid action configured for test command");
        }
    }
}

/// <summary>
/// Spy implementation of MenuItemCommandBase for testing
/// Records all method calls for verification in tests
/// </summary>
public class SpyMenuCommand : MenuItemCommandBase
{
    private readonly string _text;
    private readonly Action? _syncAction;
    private readonly Func<Task>? _asyncAction;

    public bool ExecuteCalled { get; private set; }
    public bool ExecuteAsyncCalled { get; private set; }
    public bool DoExecuteCalled { get; private set; }
    public bool DoExecuteAsyncCalled { get; private set; }
    public int ExecuteCount { get; private set; }
    public Exception? ExceptionToThrow { get; set; }

    /// <summary>
    /// Initializes a spy command with synchronous action
    /// </summary>
    /// <param name="text">Display text for the command</param>
    /// <param name="syncAction">Optional synchronous action</param>
    public SpyMenuCommand(string text, Action? syncAction = null)
    {
        _text = text;
        _syncAction = syncAction;
    }

    /// <summary>
    /// Initializes a spy command with asynchronous action
    /// </summary>
    /// <param name="text">Display text for the command</param>
    /// <param name="asyncAction">Asynchronous action</param>
    public SpyMenuCommand(string text, Func<Task> asyncAction)
    {
        _text = text;
        _asyncAction = asyncAction;
    }

    public override string Text => _text;

    public override void Execute()
    {
        ExecuteCalled = true;
        ExecuteCount++;
        base.Execute();
    }

    public override async Task ExecuteAsync()
    {
        ExecuteAsyncCalled = true;
        ExecuteCount++;
        await base.ExecuteAsync();
    }

    protected override void DoExecute()
    {
        DoExecuteCalled = true;

        if (ExceptionToThrow != null)
            throw ExceptionToThrow;

        _syncAction?.Invoke();
    }

    protected override async Task DoExecuteAsync()
    {
        DoExecuteAsyncCalled = true;

        if (ExceptionToThrow != null)
            throw ExceptionToThrow;

        if (_asyncAction != null)
        {
            await _asyncAction();
        }
        else
        {
            await base.DoExecuteAsync();
        }
    }

    /// <summary>
    /// Resets all spy counters and flags
    /// </summary>
    public void Reset()
    {
        ExecuteCalled = false;
        ExecuteAsyncCalled = false;
        DoExecuteCalled = false;
        DoExecuteAsyncCalled = false;
        ExecuteCount = 0;
        ExceptionToThrow = null;
    }
}

/// <summary>
/// Test data factory for creating various menu scenarios
/// Provides common test data configurations
/// </summary>
public static class MenuTestDataFactory
{
    /// <summary>
    /// Creates a collection of test menu actions with proper menu commands
    /// </summary>
    /// <returns>Collection of test menu commands</returns>
    public static IEnumerable<MenuItemCommandBase> CreateTestMenuCommands()
    {
        yield return new TestMenuCommand("View Customer List", () => { });
        yield return new TestMenuCommand("Add New Customer", () => { });
        yield return new TestMenuCommand("Generate Report", () => { });
        yield return new TestMenuCommand("Export Data", () => { });
    }

    /// <summary>
    /// Creates a collection of test async menu commands
    /// </summary>
    /// <returns>Collection of async test menu commands</returns>
    public static IEnumerable<MenuItemCommandBase> CreateTestAsyncMenuCommands()
    {
        yield return new TestMenuCommand("Load Data Async", async () => await Task.Delay(10));
        yield return new TestMenuCommand("Save Data Async", async () => await Task.Delay(10));
        yield return new TestMenuCommand("Process Async", async () => await Task.Delay(10));
    }

    /// <summary>
    /// Creates test menu items with various exception scenarios
    /// </summary>
    /// <returns>Collection of exception-throwing menu commands</returns>
    public static IEnumerable<MenuItemCommandBase> CreateExceptionTestCommands()
    {
        yield return new SpyMenuCommand("Throws ArgumentException")
        {
            ExceptionToThrow = new ArgumentException("Test argument exception")
        };

        yield return new SpyMenuCommand("Throws InvalidOperationException")
        {
            ExceptionToThrow = new InvalidOperationException("Test operation exception")
        };

        yield return new SpyMenuCommand("Throws Custom Exception")
        {
            ExceptionToThrow = new CustomTestException("Custom test exception")
        };
    }

    /// <summary>
    /// Creates performance test commands with varying execution times
    /// </summary>
    /// <returns>Collection of performance test commands</returns>
    public static IEnumerable<SpyMenuCommand> CreatePerformanceTestCommands()
    {
        yield return new SpyMenuCommand("Fast Operation", () => Task.Delay(10));
        yield return new SpyMenuCommand("Medium Operation", () => Task.Delay(50));
        yield return new SpyMenuCommand("Slow Operation", () => Task.Delay(100));
    }

    /// <summary>
    /// Creates menu commands for stress testing scenarios
    /// </summary>
    /// <param name="count">Number of commands to create</param>
    /// <returns>Collection of stress test commands</returns>
    public static IEnumerable<MenuItemCommandBase> CreateStressTestCommands(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var index = i; // Capture for closure
            yield return new TestMenuCommand($"Stress Test Command {index + 1}",
                () => { var result = index * index; });
        }
    }

    /// <summary>
    /// Creates menu commands that simulate various business operations
    /// </summary>
    /// <returns>Collection of business simulation commands</returns>
    public static IEnumerable<MenuItemCommandBase> CreateBusinessSimulationCommands()
    {
        yield return new TestMenuCommand("Process Orders",
            async () => await Task.Delay(50)); // Simulate order processing

        yield return new TestMenuCommand("Update Inventory",
            () => Thread.Sleep(25)); // Simulate inventory update

        yield return new TestMenuCommand("Generate Sales Report",
            async () => await Task.Delay(75)); // Simulate report generation

        yield return new TestMenuCommand("Backup Database",
            async () => await Task.Delay(100)); // Simulate database backup
    }
}

/// <summary>
/// Custom exception for testing exception handling scenarios
/// </summary>
public class CustomTestException : Exception
{
    /// <summary>
    /// Initializes a new custom test exception
    /// </summary>
    /// <param name="message">Exception message</param>
    public CustomTestException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new custom test exception with inner exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public CustomTestException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Extension methods for test assertions and common test operations
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Verifies that a menu controller contains the expected number of items
    /// </summary>
    /// <param name="controller">The menu controller to verify</param>
    /// <param name="expectedCount">Expected number of menu items</param>
    public static void ShouldHaveMenuItemCount(this AppMenuController controller, int expectedCount)
    {
        if (controller.Count != expectedCount)
        {
            throw new Xunit.Sdk.XunitException(
                $"Expected menu to have {expectedCount} items, but found {controller.Count}");
        }
    }

    /// <summary>
    /// Verifies that all menu items have non-empty text
    /// </summary>
    /// <param name="controller">The menu controller to verify</param>
    public static void ShouldHaveValidMenuTexts(this AppMenuController controller)
    {
        foreach (var menuItem in controller)
        {
            if (string.IsNullOrWhiteSpace(menuItem.Text))
            {
                throw new Xunit.Sdk.XunitException(
                    "Found menu item with empty or null text");
            }
        }
    }

    /// <summary>
    /// Verifies that menu items contain specific text patterns
    /// </summary>
    /// <param name="controller">The menu controller to verify</param>
    /// <param name="expectedTexts">Expected text patterns</param>
    public static void ShouldContainMenuTexts(this AppMenuController controller, params string[] expectedTexts)
    {
        var actualTexts = controller.Select(item => item.Text).ToArray();

        foreach (var expectedText in expectedTexts)
        {
            if (!actualTexts.Any(text => text.Contains(expectedText)))
            {
                throw new Xunit.Sdk.XunitException(
                    $"Expected to find menu item containing '{expectedText}', but it was not found");
            }
        }
    }

    /// <summary>
    /// Creates a collection of spy commands for testing
    /// </summary>
    /// <param name="count">Number of spy commands to create</param>
    /// <returns>Collection of spy commands</returns>
    public static IEnumerable<SpyMenuCommand> CreateSpyCommands(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new SpyMenuCommand($"Spy Command {i + 1}");
        }
    }

    /// <summary>
    /// Executes all menu items in a controller sequentially
    /// </summary>
    /// <param name="controller">The menu controller</param>
    /// <returns>Task representing the async execution</returns>
    public static async Task ExecuteAllMenuItemsAsync(this AppMenuController controller)
    {
        foreach (var menuItem in controller)
        {
            await menuItem.ExecuteAsync();
        }
    }

    /// <summary>
    /// Executes all menu items in a controller concurrently
    /// </summary>
    /// <param name="controller">The menu controller</param>
    /// <returns>Task representing the concurrent execution</returns>
    public static async Task ExecuteAllMenuItemsConcurrentlyAsync(this AppMenuController controller)
    {
        var tasks = controller.Select(item => item.ExecuteAsync()).ToArray();
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Measures the execution time of all menu items
    /// </summary>
    /// <param name="controller">The menu controller</param>
    /// <returns>Tuple containing execution time and results</returns>
    public static async Task<(TimeSpan ExecutionTime, int ItemsExecuted)> MeasureExecutionTimeAsync(
        this AppMenuController controller)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await controller.ExecuteAllMenuItemsAsync();

        stopwatch.Stop();

        return (stopwatch.Elapsed, controller.Count);
    }
}