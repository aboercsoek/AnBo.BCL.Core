//--------------------------------------------------------------------------
// File:    AppMenuController.cs
// Content:	Implementation of class AppMenuController
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core;

/// <summary>
/// Console Application Menu Controller that orchestrates menu display and execution.
/// Implements the Controller pattern to separate menu logic from presentation.
/// Supports both synchronous and asynchronous menu operations.
/// </summary>
public class AppMenuController : IEnumerable<MenuItemCommandBase>
{
    #region Private Fields

    private readonly List<MenuItemCommandBase> _menuItems = [];
    private IAppMenuView _menuView;
    private readonly IEnvironmentService _environmentService;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new application menu controller with the specified header text.
    /// Uses the default console view implementation.
    /// </summary>
    /// <param name="appMenuHeaderText">
    /// The header text to display at the top of the menu.
    /// If null, a default header will be used.
    /// </param>
    /// <param name="environmentService">Environment service for exit operations</param>
    public AppMenuController(string? appMenuHeaderText, IEnvironmentService? environmentService = null)
    {
        _menuView = new ConsoleAppMenuView(appMenuHeaderText ?? "Console Application Menu");
        _environmentService = environmentService ?? new EnvironmentService();
    }

    /// <summary>
    /// Initializes a new application menu controller with header text and initial menu actions.
    /// This constructor provides a convenient way to set up a menu with predefined actions.
    /// </summary>
    /// <param name="appMenuHeaderText">
    /// The header text to display at the top of the menu
    /// </param>
    /// <param name="menuItemActions">
    /// Variable number of action delegates to add as menu items
    /// </param>
    public AppMenuController(string appMenuHeaderText, params Action[] menuItemActions)
        : this(appMenuHeaderText)
    {
        foreach (var menuItemAction in menuItemActions)
        {
            Add(menuItemAction);
        }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the menu view implementation.
    /// Allows for dependency injection of different view implementations.
    /// </summary>
    /// <value>The current menu view instance</value>
    /// <exception cref="ArgumentNullException">Is thrown if set value is null</exception>
    public IAppMenuView MenuView
    {
        get => _menuView;
        set => _menuView = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the number of menu items currently registered.
    /// </summary>
    /// <value>The total count of menu items</value>
    public int Count => _menuItems.Count;

    #endregion

    #region Application Menu Execution Methods

    /// <summary>
    /// Executes the console application menu synchronously.
    /// This method blocks until the user chooses to quit the application.
    /// </summary>
    public void Run()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executes the console application menu asynchronously.
    /// Provides the main menu loop with proper async/await support.
    /// </summary>
    /// <returns>A task representing the asynchronous menu execution</returns>
    public async Task RunAsync()
    {
        // Initialize the view with menu items texts
        _menuView.InitView(this.Select(menuItem => menuItem.Text));

        // Main menu loop - continue until user chooses to quit
        while (!_menuView.ShouldQuit)
        {
            _menuView.DisplayMenu();
            await HandleUserSelectionAsync().ConfigureAwait(false);
            _menuView.PromptToContinue();
        }

        // User selected to quit the application
        _environmentService.Exit(0);
    }

    #endregion

    #region Menu Item Management Methods

    /// <summary>
    /// Adds a menu item command to the application menu.
    /// Provides type-safe menu item registration.
    /// </summary>
    /// <param name="menuItem">
    /// The menu item command to add. Null values are ignored.
    /// </param>
    public void Add(MenuItemCommandBase? menuItem)
    {
        if (menuItem is null)
            return;

        _menuItems.Add(menuItem);
    }

    /// <summary>
    /// Adds a synchronous action delegate as a menu item.
    /// The action will be wrapped in an ActionBasedMenuItemCmd automatically.
    /// </summary>
    /// <param name="menuItemAction">
    /// The synchronous action to execute. Null values are ignored.
    /// </param>
    public void Add(Action menuItemAction)
    {
        if (menuItemAction is null)
            return;

        _menuItems.Add(new ActionBasedMenuItemCmd(menuItemAction));
    }

    /// <summary>
    /// Adds an asynchronous function delegate as a menu item.
    /// The function will be wrapped in an ActionBasedMenuItemCmd automatically.
    /// </summary>
    /// <param name="menuItemAsyncAction">
    /// The asynchronous function to execute. Null values are ignored.
    /// </param>
    public void Add(Func<Task> menuItemAsyncAction)
    {
        if (menuItemAsyncAction is null)
            return;

        _menuItems.Add(new ActionBasedMenuItemCmd(menuItemAsyncAction));
    }
    /// <summary>
    /// Removes all menu items from the controller.
    /// Useful for dynamic menu rebuilding scenarios.
    /// </summary>
    public void Clear()
    {
        _menuItems.Clear();
    }

    /// <summary>
    /// Removes a specific menu item from the controller.
    /// </summary>
    /// <param name="menuItem">The menu item to remove</param>
    /// <returns>true if the item was found and removed; otherwise, false</returns>
    public bool Remove(MenuItemCommandBase menuItem)
    {
        return _menuItems.Remove(menuItem);
    }

    #endregion

    #region IEnumerable Implementation

    /// <summary>
    /// Returns an enumerator that iterates through the menu item collection.
    /// Enables foreach loops and LINQ operations on the menu controller.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the menu items
    /// </returns>
    public IEnumerator<MenuItemCommandBase> GetEnumerator()
    {
        return _menuItems.GetEnumerator();
    }

    /// <summary>
    /// Returns a non-generic enumerator for the menu item collection.
    /// Required for IEnumerable interface compatibility.
    /// </summary>
    /// <returns>A non-generic enumerator for the menu items</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Handles user menu selection and executes the chosen operation asynchronously.
    /// This method encapsulates the selection validation, execution, and error handling logic.
    /// </summary>
    /// <returns>A task representing the asynchronous selection handling</returns>
    private async Task HandleUserSelectionAsync()
    {
        int selectedOption = _menuView.WaitForValidUserInput();

        if (selectedOption == -1 || selectedOption >= _menuItems.Count)
        {
            return; // Invalid selection or quit requested
        }

        var selectedMenuItem = _menuItems[selectedOption];

        _menuView.ClearView();
        _menuView.WriteMenuOperationHeader(selectedMenuItem.Text);
        try
        {
            // Execute the selected menu item asynchronously for better responsiveness
            await selectedMenuItem.ExecuteAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Display comprehensive error information to the user
            _menuView.ShowExceptionDetails(ex);
        }
    }

    #endregion

}
