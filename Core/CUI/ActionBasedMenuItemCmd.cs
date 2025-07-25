//--------------------------------------------------------------------------
// File:    ActionBasedMenuItemCmd.cs
// Content:	Implementation of class ActionBasedMenuItemCmd
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.ComponentModel;

#endregion

namespace AnBo.Core;

///<summary>Delegate-based menu item command class</summary>
public class ActionBasedMenuItemCmd : MenuItemCommandBase
{
    private readonly Action? menuItemAction;
    private readonly Func<Task>? menuItemAsyncAction;
    private readonly string menuItemText;
    private readonly bool isAsync;


    /// <summary>
    /// Initializes a new instance of the <see cref="ActionBasedMenuItemCmd"/> class.
    /// </summary>
    /// <param name="menuItemAction">The menu item action delegate.</param>
    public ActionBasedMenuItemCmd(Action menuItemAction)
    {
        ArgumentNullException.ThrowIfNull(menuItemAction);

        this.menuItemAction = menuItemAction;
        menuItemAsyncAction = null;
        isAsync = false;
        menuItemText = GetDescriptionFromDelegate(menuItemAction.Method);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionBasedMenuItemCmd"/> class
    /// with an asynchronous action delegate.
    /// </summary>
    /// <param name="menuItemAsyncAction">The asynchronous menu item action delegate</param>
    public ActionBasedMenuItemCmd(Func<Task> menuItemAsyncAction)
    {
        ArgumentNullException.ThrowIfNull(menuItemAsyncAction);

        menuItemAction = null;
        this.menuItemAsyncAction = menuItemAsyncAction;
        isAsync = true;
        menuItemText = GetDescriptionFromDelegate(menuItemAsyncAction.Method);
    }

    /// <summary>
    /// Text to display in the menu.
    /// </summary>
    public override string Text => menuItemText;

    /// <summary>
    /// Gets a value indicating whether this menu item executes asynchronously.
    /// </summary>
    public bool IsAsync => isAsync;

    /// <summary>
    /// Execute the synchronous operation.
    /// </summary>
    protected override void DoExecute()
    {
        if (isAsync && menuItemAsyncAction != null)
        {
            // Execute async operation synchronously (blocking)
            menuItemAsyncAction().GetAwaiter().GetResult();
        }
        else if (menuItemAction != null)
        {
            menuItemAction();
        }
    }

    /// <summary>
    /// Execute the asynchronous operation.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    protected override async Task DoExecuteAsync()
    {
        if (isAsync && menuItemAsyncAction != null)
        {
            await menuItemAsyncAction();
        }
        else if (menuItemAction != null)
        {
            await Task.Run(() => menuItemAction());
        }
    }

    /// <summary>
    /// Pull the description off attributes on the delegate passed in.
    /// This only works if you pass in actual methods, but that's ok
    /// for our purposes.
    /// </summary>
    /// <param name="method">The method info to extract description from</param>
    /// <returns>Description text to display</returns>
    private string GetDescriptionFromDelegate(System.Reflection.MethodInfo method)
    {
        DescriptionAttribute? description =
            method.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .CastSequence<DescriptionAttribute>().FirstOrDefault();

        return description != null ? description.Description : "No description present";
    }
}
