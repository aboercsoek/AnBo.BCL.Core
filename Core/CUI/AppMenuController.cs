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

///<summary>Console Application Menu Controller</summary>
public class AppMenuController : IEnumerable<MenuItemCommandBase>
{
    #region Private Members

    private readonly List<MenuItemCommandBase> menuItems = new List<MenuItemCommandBase>();
    private IAppMenuView menuView;

    #endregion

    #region Ctors

    /// <summary>
    /// Initializes a new application menu.
    /// </summary>
    /// <param name="appMenuHeaderText">The application menu header text.</param>
    public AppMenuController(string appMenuHeaderText)
    {
        menuView = new ConsoleAppMenuView(appMenuHeaderText ?? "Console Application Menu");
    }

    /// <summary>
    /// Initializes a new application menu.
    /// </summary>
    /// <param name="appMenuHeaderText">The application menu header text.</param>
    /// <param name="menuItemActions">The menu item actions.</param>
    public AppMenuController(string appMenuHeaderText, params Action[] menuItemActions)
        : this(appMenuHeaderText)
    {
        foreach (var menuItemAction in menuItemActions)
        {
            Add(menuItemAction);
        }
    }

    #endregion

    #region Application Menu Execution Methods

    /// <summary>
    /// Execute console application menu synchronously.
    /// </summary>
    public void Run()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Execute console application menu asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task RunAsync()
    {
        // Initialize the view with menu items texts
        menuView.InitView(this.Select(menuItem => menuItem.Text));

        while (!menuView.ShouldQuit)
        {
            menuView.DisplayMenu();
            await HandleUserSelectionAsync();
            menuView.PromptToContinue();
        }

        // User selected to quit the application
        Environment.Exit(0);
    }

    #endregion

    #region Application Menu Item Management

    /// <summary>
    /// Adds the specified menu item to the application menu.
    /// </summary>
    /// <param name="menuItem">The menu item</param>
    public void Add(MenuItemCommandBase menuItem)
    {
        if (menuItem == null)
            return;

        menuItems.Add(menuItem);
    }

    /// <summary>
    /// Adds the specified synchronous menu item action to the application menu.
    /// </summary>
    /// <param name="menuItemAction">The synchronous menu item action</param>
    public void Add(Action menuItemAction)
    {
        if (menuItemAction == null)
            return;

        menuItems.Add(new ActionBasedMenuItemCmd(menuItemAction));
    }

    /// <summary>
    /// Adds the specified asynchronous menu item action to the application menu.
    /// </summary>
    /// <param name="menuItemAsyncAction">The asynchronous menu item action</param>
    public void Add(Func<Task> menuItemAsyncAction)
    {
        if (menuItemAsyncAction == null)
            return;

        menuItems.Add(new ActionBasedMenuItemCmd(menuItemAsyncAction));
    }

    /// <summary>
    /// Returns an enumerator that iterates through the menu item collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{T}">enumerator</see> instance that can be used to iterate through the menu item collection.
    /// </returns>
    public IEnumerator<MenuItemCommandBase> GetEnumerator()
    {
        return menuItems.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handle user selection asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task HandleUserSelectionAsync()
    {
        int selectedOption = menuView.WaitForValidUserInput();
        if (selectedOption != -1)
        {
            menuView.ClearView();
            menuView.WriteMenuOperationHeader(menuItems[selectedOption].Text);
            try
            {
                // Execute async for better responsiveness
                await menuItems[selectedOption].ExecuteAsync();
            }
            catch (Exception ex)
            {
                menuView.ShowExceptionDetails(ex);
            }
        }
    }

    /// <summary>
    /// Handle user selection synchronously (legacy support).
    /// </summary>
    private void HandleUserSelection()
    {
        HandleUserSelectionAsync().GetAwaiter().GetResult();
    }

    #endregion

}
