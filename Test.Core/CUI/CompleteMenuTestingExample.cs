using AnBo.Core;
using AnBo.Test.Helpers;
using FluentAssertions;
using Moq;
using System.ComponentModel;
using Xunit;

namespace AnBo.Test.Examples;

/// <summary>
/// Comprehensive example demonstrating complete menu testing scenarios
/// Shows real-world usage patterns and advanced testing techniques
/// </summary>
public class CompleteMenuTestingExample
{
    private bool _customerViewExecuted;
    private bool _reportGeneratedExecuted;
    private string _lastProcessedData = string.Empty;

    /// <summary>
    /// Example: Testing a complete customer management menu
    /// Demonstrates realistic business scenario testing focusing on menu composition and execution
    /// </summary>
    [Fact]
    public async Task CustomerManagementMenu_CompleteWorkflow_ShouldExecuteCorrectly()
    {
        // Arrange - Build a realistic menu using the test builder
        var (controller, mockView) = new TestMenuControllerBuilder("Customer Management System")
            .WithSyncAction("View Customers", ViewCustomers)
            .WithAsyncAction("Generate Report", GenerateReportAsync)
            .WithSyncAction("Export Data", ExportData)
            .WithThrowingAction("Simulate Error", new InvalidOperationException("Database connection failed"))
            .Build();

        // Act - Test the menu composition and individual item execution
        controller.Count.Should().Be(4);

        // Execute menu workflow directly (bypassing RunAsync which contains Environment.Exit)
        var menuItems = controller.ToList();

        await menuItems[0].ExecuteAsync(); // View Customers
        await menuItems[1].ExecuteAsync(); // Generate Report
        await menuItems[2].ExecuteAsync(); // Export Data

        // Assert - Verify all operations were executed correctly
        _customerViewExecuted.Should().BeTrue();
        _reportGeneratedExecuted.Should().BeTrue();
        _lastProcessedData.Should().Be("Data exported successfully");

        // Verify menu structure
        controller.ShouldHaveValidMenuTexts();

        // Verify menu items have correct descriptions
        var menuTexts = controller.Select(item => item.Text).ToArray();
        menuTexts[0].Should().Be("View Customers");
        menuTexts[1].Should().Be("Generate Report");
        menuTexts[2].Should().Be("Export Data");
        menuTexts[3].Should().Be("Simulate Error Action");
    }

    /// <summary>
    /// Example: Testing exception handling in menu operations
    /// Shows comprehensive error scenario testing
    /// </summary>
    [Fact]
    public void MenuExceptionHandling_VariousExceptionTypes_ShouldHandleGracefully()
    {
        // Arrange - Create menu items that throw different exceptions
        var exceptionCommands = MenuTestDataFactory.CreateExceptionTestCommands().ToList();
        var controller = new AppMenuController("Exception Test Menu");

        foreach (var command in exceptionCommands)
        {
            controller.Add(command);
        }

        // Act & Assert - Test each exception scenario
        var menuItems = controller.ToList();

        // Test ArgumentException handling
        var argumentExceptionAction = () => menuItems[0].Execute();
        argumentExceptionAction.Should().Throw<ArgumentException>()
            .WithMessage("Test argument exception");

        // Test InvalidOperationException handling  
        var operationExceptionAction = () => menuItems[1].Execute();
        operationExceptionAction.Should().Throw<InvalidOperationException>()
            .WithMessage("Test operation exception");

        // Test Custom exception handling
        var customExceptionAction = () => menuItems[2].Execute();
        customExceptionAction.Should().Throw<CustomTestException>()
            .WithMessage("Custom test exception");
    }

    /// <summary>
    /// Example: Performance testing for menu operations
    /// Demonstrates testing of async operations and timing
    /// </summary>
    [Fact]
    public async Task MenuPerformance_AsyncOperations_ShouldCompleteWithinExpectedTime()
    {
        // Arrange
        const int expectedMaxExecutionTimeMs = 500;
        var performanceTestCommands = new[]
        {
            new SpyMenuCommand("Fast Operation", () => Task.Delay(50)),
            new SpyMenuCommand("Medium Operation", () => Task.Delay(100)),
            new SpyMenuCommand("Slow Operation", () => Task.Delay(200))
        };

        var controller = new AppMenuController("Performance Test Menu");
        foreach (var command in performanceTestCommands)
        {
            controller.Add(command);
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act - Execute all operations concurrently
        var executionTasks = controller.Select(item => item.ExecuteAsync()).ToArray();
        await Task.WhenAll(executionTasks);

        stopwatch.Stop();

        // Assert - Verify performance characteristics
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(expectedMaxExecutionTimeMs);

        // Verify all spy commands were executed
        foreach (var spyCommand in performanceTestCommands)
        {
            spyCommand.ExecuteAsyncCalled.Should().BeTrue();
            spyCommand.ExecuteCount.Should().Be(1);
        }
    }

    /// <summary>
    /// Example: Testing menu state management and isolation
    /// Ensures menu operations don't interfere with each other
    /// </summary>
    [Fact]
    public void MenuStateIsolation_MultipleExecutions_ShouldMaintainIndependentState()
    {
        // Arrange
        var spyCommands = TestExtensions.CreateSpyCommands(3).ToArray();
        var controller = new AppMenuController("State Isolation Test");

        foreach (var command in spyCommands)
        {
            controller.Add(command);
        }

        // Act - Execute commands multiple times in different orders
        var menuItems = controller.ToList();

        // First execution sequence: 0, 1, 2
        menuItems[0].Execute();
        menuItems[1].Execute();
        menuItems[2].Execute();

        // Second execution sequence: 2, 0, 1
        menuItems[2].Execute();
        menuItems[0].Execute();
        menuItems[1].Execute();

        // Assert - Verify each command maintains its own execution count
        spyCommands[0].ExecuteCount.Should().Be(2);
        spyCommands[1].ExecuteCount.Should().Be(2);
        spyCommands[2].ExecuteCount.Should().Be(2);

        // Verify state isolation - each command should have been called independently
        foreach (var spy in spyCommands)
        {
            spy.ExecuteCalled.Should().BeTrue();
            spy.DoExecuteCalled.Should().BeTrue();
        }
    }

    /// <summary>
    /// Example: Integration test with realistic data processing
    /// Tests menu with actual business logic simulation
    /// </summary>
    [Fact]
    public async Task DataProcessingMenu_RealWorldScenario_ShouldProcessDataCorrectly()
    {
        // Arrange - Simulate a data processing application
        var testData = new List<string> { "Record1", "Record2", "Record3" };
        var processedRecords = new List<string>();

        var dataMenu = new AppMenuController("Data Processing Menu");

        // Add realistic data processing operations
        dataMenu.Add(new ActionBasedMenuItemCmd(() =>
        {
            // Simulate data loading
            _lastProcessedData = $"Loaded {testData.Count} records";
        }));

        dataMenu.Add(new ActionBasedMenuItemCmd(async () =>
        {
            // Simulate async data processing
            await Task.Delay(10); // Simulate processing time
            processedRecords.AddRange(testData.Select(r => $"Processed_{r}"));
            _lastProcessedData = $"Processed {processedRecords.Count} records";
        }));

        dataMenu.Add(new ActionBasedMenuItemCmd(() =>
        {
            // Simulate data validation
            var validRecords = processedRecords.Where(r => r.StartsWith("Processed_")).Count();
            _lastProcessedData = $"Validated {validRecords} records";
        }));

        // Act - Execute the complete data processing workflow
        var menuItems = dataMenu.ToList();

        menuItems[0].Execute(); // Load data
        await menuItems[1].ExecuteAsync(); // Process data
        menuItems[2].Execute(); // Validate data

        // Assert - Verify the complete workflow
        processedRecords.Should().HaveCount(3);
        processedRecords.Should().AllSatisfy(record =>
            record.Should().StartWith("Processed_"));

        _lastProcessedData.Should().Be("Validated 3 records");
        dataMenu.Count.Should().Be(3);
    }

    /// <summary>
    /// Example: Testing menu view integration patterns
    /// Shows how to test menu-view interaction without RunAsync
    /// </summary>
    [Fact]
    public void MenuViewIntegration_ConfigurationAndSetup_ShouldWorkCorrectly()
    {
        // Arrange
        var controller = new AppMenuController("View Integration Test");
        var mockView = new Mock<IAppMenuView>();

        controller.Add(ViewCustomers);
        controller.Add(GenerateReportAsync);
        controller.Add(ExportData);

        // Act - Replace view and test configuration
        controller.MenuView = mockView.Object;

        // Simulate what would happen in InitView
        var menuTexts = controller.Select(item => item.Text).ToArray();

        // Assert - Verify view integration setup
        controller.MenuView.Should().Be(mockView.Object);
        menuTexts.Should().HaveCount(3);
        menuTexts[0].Should().Be("View Customers");
        menuTexts[1].Should().Be("Generate Report");
        menuTexts[2].Should().Be("Export Data");

        // Verify the mock view can be configured (demonstrates testability)
        mockView.Setup(v => v.ShouldQuit).Returns(true);
        mockView.Object.ShouldQuit.Should().BeTrue();
    }

    /// <summary>
    /// Example: Testing error propagation through menu system
    /// Demonstrates comprehensive error handling testing
    /// </summary>
    [Fact]
    public async Task MenuErrorPropagation_ThroughoutSystem_ShouldMaintainIntegrity()
    {
        // Arrange
        var controller = new AppMenuController("Error Propagation Test");
        var errorMessages = new List<string>();

        // Add menu items with different error scenarios
        controller.Add(() => throw new ArgumentException("Invalid argument"));
        controller.Add(async () =>
        {
            await Task.Delay(1);
            throw new InvalidOperationException("Operation failed");
        });
        controller.Add(() => throw new CustomTestException("Custom error"));

        var menuItems = controller.ToList();

        // Act & Assert - Test error propagation for each menu item

        // Test sync exception
        var syncException = Assert.Throws<ArgumentException>(() => menuItems[0].Execute());
        syncException.Message.Should().Be("Invalid argument");

        // Test async exception
        var asyncException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => menuItems[1].ExecuteAsync());
        asyncException.Message.Should().Be("Operation failed");

        // Test custom exception
        var customException = Assert.Throws<CustomTestException>(() => menuItems[2].Execute());
        customException.Message.Should().Be("Custom error");

        // Verify menu structure remains intact after exceptions
        controller.Count.Should().Be(3);
        controller.Should().AllSatisfy(item => item.Should().NotBeNull());
    }

    #region Test Helper Methods

    /// <summary>
    /// Test method: Simulates viewing customer data
    /// </summary>
    [Description("View Customers")]
    private void ViewCustomers()
    {
        _customerViewExecuted = true;
        _lastProcessedData = "Customer list displayed";
    }

    /// <summary>
    /// Test method: Simulates async report generation
    /// </summary>
    [Description("Generate Report")]
    private async Task GenerateReportAsync()
    {
        await Task.Delay(10); // Simulate report generation time
        _reportGeneratedExecuted = true;
        _lastProcessedData = "Report generated successfully";
    }

    /// <summary>
    /// Test method: Simulates data export operation
    /// </summary>
    [Description("Export Data")]
    private void ExportData()
    {
        _lastProcessedData = "Data exported successfully";
    }

    #endregion
}

/// <summary>
/// Edge case testing for menu system
/// Tests boundary conditions and error scenarios
/// </summary>
public class MenuEdgeCaseTests
{
    [Fact]
    public void EmptyMenu_ShouldHandleGracefully()
    {
        // Arrange & Act
        var emptyController = new AppMenuController("Empty Menu");

        // Assert
        emptyController.Count.Should().Be(0);
        emptyController.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MenuWithInvalidHeaderText_ShouldHandleGracefully(string? headerText)
    {
        // Act & Assert - Should not throw
        var controller = new AppMenuController(headerText);
        controller.Should().NotBeNull();
    }

    [Fact]
    public void MenuWithVeryLongTexts_ShouldHandleCorrectly()
    {
        // Arrange
        var longText = new string('A', 1000); // Very long text
        var controller = new AppMenuController("Long Text Test");

        // Act
        controller.Add(() => { /* Long text action */ });

        // Assert
        controller.Count.Should().Be(1);
        var menuItem = controller.First();
        menuItem.Should().NotBeNull();
    }

    [Fact]
    public void MenuWithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var controller = new AppMenuController("Special Characters: äöü ñ 中文 🎯");

        // Act & Assert - Should not throw
        controller.Add(SpecialCharacterAction);
        controller.Count.Should().Be(1);
    }

    [Fact]
    public void ConcurrentMenuOperations_ShouldBeThreadSafe()
    {
        // Arrange
        var controller = new AppMenuController("Concurrency Test");
        var executionCount = 0;
        var lockObject = new object();

        Action incrementAction = () =>
        {
            lock (lockObject)
            {
                executionCount++;
            }
        };

        // Add multiple menu items
        for (int i = 0; i < 10; i++)
        {
            controller.Add(incrementAction);
        }

        // Act - Execute all menu items concurrently
        var tasks = controller.Select(item => Task.Run(() => item.Execute())).ToArray();
        Task.WaitAll(tasks);

        // Assert
        executionCount.Should().Be(10);
    }

    /// <summary>
    /// Test action with special characters in description
    /// </summary>
    [Description("Special Characters: äöü ñ 中文 🎯")]
    private static void SpecialCharacterAction()
    {
        // Test action with special characters
    }
}

/// <summary>
/// Memory and resource management tests for menu system
/// Ensures proper cleanup and resource handling
/// </summary>
public class MenuResourceManagementTests
{
    [Fact]
    public void MenuWithManyItems_ShouldNotLeakMemory()
    {
        // Arrange
        const int numberOfItems = 1000;
        var controller = new AppMenuController("Memory Test");

        // Act - Add many menu items
        for (int i = 0; i < numberOfItems; i++)
        {
            var localIndex = i; // Capture for closure
            controller.Add(() => { var result = localIndex * 2; });
        }

        // Assert
        controller.Count.Should().Be(numberOfItems);

        // Cleanup test
        controller.Clear();
        controller.Count.Should().Be(0);
    }

    [Fact]
    public void MenuItemDisposal_WithDisposableResources_ShouldCleanupCorrectly()
    {
        // Arrange
        var disposableResource = new TestDisposableResource();
        var controller = new AppMenuController("Disposal Test");

        // Act
        controller.Add(() => disposableResource.DoWork());

        var menuItem = controller.First();
        menuItem.Execute();

        // Manually dispose resource (in real scenario, this would be automatic)
        disposableResource.Dispose();

        // Assert
        disposableResource.IsDisposed.Should().BeTrue();
        disposableResource.WorkExecuted.Should().BeTrue();
    }

    /// <summary>
    /// Test disposable resource for resource management testing
    /// </summary>
    private class TestDisposableResource : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool WorkExecuted { get; private set; }

        /// <summary>
        /// Simulates doing work with the resource
        /// </summary>
        public void DoWork()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(TestDisposableResource));

            WorkExecuted = true;
        }

        /// <summary>
        /// Disposes the test resource
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}

/// <summary>
/// Behavioral tests for menu system
/// Tests the overall behavior and user experience aspects
/// </summary>
public class MenuBehaviorTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(26)] // Maximum for A-Z keys
    public void MenuWithVariousItemCounts_ShouldHandleCorrectly(int itemCount)
    {
        // Arrange
        var controller = new AppMenuController($"Menu with {itemCount} items");

        // Act - Add the specified number of items
        for (int i = 0; i < itemCount; i++)
        {
            var itemIndex = i; // Capture for closure
            controller.Add(() => { var result = itemIndex; });
        }

        // Assert
        controller.Count.Should().Be(itemCount);
        controller.Should().HaveCount(itemCount);

        // Verify all items are accessible
        var itemsList = controller.ToList();
        itemsList.Should().HaveCount(itemCount);
        itemsList.Should().AllSatisfy(item => item.Should().NotBeNull());
    }

    [Fact]
    public void MenuItemExecution_ShouldMaintainExecutionOrder()
    {
        // Arrange
        var executionOrder = new List<int>();
        var controller = new AppMenuController("Execution Order Test");

        // Add menu items that record their execution order
        for (int i = 0; i < 5; i++)
        {
            var itemIndex = i;
            controller.Add(() => executionOrder.Add(itemIndex));
        }

        // Act - Execute items in specific order
        var menuItems = controller.ToList();
        menuItems[2].Execute(); // Execute item 2 first
        menuItems[0].Execute(); // Then item 0
        menuItems[4].Execute(); // Then item 4
        menuItems[1].Execute(); // Then item 1
        menuItems[3].Execute(); // Finally item 3

        // Assert - Verify execution order matches our sequence
        executionOrder.Should().Equal(2, 0, 4, 1, 3);
    }

    [Fact]
    public async Task AsyncMenuExecution_MixedSyncAndAsync_ShouldHandleCorrectly()
    {
        // Arrange
        var executionResults = new List<string>();
        var controller = new AppMenuController("Mixed Execution Test");

        // Add mix of sync and async operations
        controller.Add(() => executionResults.Add("Sync1"));
        controller.Add(async () =>
        {
            await Task.Delay(10);
            executionResults.Add("Async1");
        });
        controller.Add(() => executionResults.Add("Sync2"));
        controller.Add(async () =>
        {
            await Task.Delay(5);
            executionResults.Add("Async2");
        });

        // Act - Execute all items asynchronously
        var menuItems = controller.ToList();
        var executionTasks = menuItems.Select(item => item.ExecuteAsync()).ToArray();
        await Task.WhenAll(executionTasks);

        // Assert - All operations should have completed
        executionResults.Should().HaveCount(4);
        executionResults.Should().Contain("Sync1", "Async1", "Sync2", "Async2");
    }

    [Fact]
    public void MenuModification_AfterInitialization_ShouldUpdateCorrectly()
    {
        // Arrange
        var controller = new AppMenuController("Dynamic Menu Test");
        controller.Add(() => { /* Initial action */ });

        var initialCount = controller.Count;

        // Act - Modify menu after initialization
        controller.Add(() => { /* Added action */ });
        var afterAddCount = controller.Count;

        var firstItem = controller.First();
        controller.Remove(firstItem);
        var afterRemoveCount = controller.Count;

        controller.Clear();
        var afterClearCount = controller.Count;

        // Assert - Verify all modifications worked correctly
        initialCount.Should().Be(1);
        afterAddCount.Should().Be(2);
        afterRemoveCount.Should().Be(1);
        afterClearCount.Should().Be(0);
    }
}