//--------------------------------------------------------------------------
// File:    ActionBasedMenuItemCmd.cs
// Content:	Implementation of class ActionBasedMenuItemCmd
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.ComponentModel;
using System.Reflection;

#endregion

namespace AnBo.Core;

/// <summary>
/// Delegate-based menu item command implementation that supports both
/// synchronous and asynchronous action delegates.
/// Provides automatic text extraction from method attributes.
/// </summary>
public class ActionBasedMenuItemCmd : MenuItemCommandBase
{
    #region Private Fields

    private readonly Action? _menuItemAction;
    private readonly Func<Task>? _menuItemAsyncAction;
    private readonly string _menuItemText;
    private readonly bool _isAsync;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance with a synchronous action delegate.
    /// The menu text is automatically extracted from the method's Description attribute.
    /// </summary>
    /// <param name="menuItemAction">The synchronous menu item action delegate</param>
    /// <exception cref="ArgumentNullException">Thrown when menuItemAction is null</exception>
    public ActionBasedMenuItemCmd(Action menuItemAction)
    {
        ArgumentNullException.ThrowIfNull(menuItemAction);

        _menuItemAction = menuItemAction;
        _menuItemAsyncAction = null;
        _isAsync = false;
        _menuItemText = ExtractDescriptionFromMethod(menuItemAction.Method);
    }

    /// <summary>
    /// Initializes a new instance with an asynchronous action delegate.
    /// The menu text is automatically extracted from the method's Description attribute.
    /// </summary>
    /// <param name="menuItemAsyncAction">The asynchronous menu item action delegate</param>
    /// <exception cref="ArgumentNullException">Thrown when menuItemAsyncAction is null</exception>
    public ActionBasedMenuItemCmd(Func<Task> menuItemAsyncAction)
    {
        ArgumentNullException.ThrowIfNull(menuItemAsyncAction);

        _menuItemAction = null;
        _menuItemAsyncAction = menuItemAsyncAction;
        _isAsync = true;
        _menuItemText = ExtractDescriptionFromMethod(menuItemAsyncAction.Method);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the display text for the menu item.
    /// Text is automatically extracted from the delegate method's Description attribute.
    /// </summary>
    /// <value>The text to display in the menu interface</value>
    public override string Text => _menuItemText;

    /// <summary>
    /// Gets a value indicating whether this menu item executes asynchronously.
    /// </summary>
    /// <value>true if the menu item uses an async delegate; otherwise, false</value>
    public bool IsAsync => _isAsync;

    #endregion


    #region Protected Override Methods

    /// <summary>
    /// Executes the menu item operation synchronously.
    /// For async delegates, this method blocks until completion.
    /// </summary>
    protected override void DoExecute()
    {
        switch ((_isAsync, _menuItemAsyncAction, _menuItemAction))
        {
            case (true, not null, _):
                // Execute async operation synchronously (blocking)
                _menuItemAsyncAction().GetAwaiter().GetResult();
                break;

            case (false, _, not null):
                _menuItemAction();
                break;

            default:
                throw new InvalidOperationException("No valid action delegate configured");
        }
    }

    /// <summary>
    /// Executes the menu item operation asynchronously.
    /// Provides optimal async execution for both sync and async delegates.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    protected override async Task DoExecuteAsync()
    {
        switch ((_isAsync, _menuItemAsyncAction, _menuItemAction))
        {
            case (true, not null, _):
                await _menuItemAsyncAction().ConfigureAwait(false);
                break;

            case (false, _, not null):
                await Task.Run(_menuItemAction).ConfigureAwait(false);
                break;

            default:
                throw new InvalidOperationException("No valid action delegate configured");
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Extracts the description text from a method's DescriptionAttribute.
    /// This method uses reflection to inspect method attributes and extract
    /// user-friendly display text for menu items.
    /// </summary>
    /// <param name="method">The method info to extract description from</param>
    /// <returns>
    /// The description text if a DescriptionAttribute is found;
    /// otherwise, returns a default message
    /// </returns>
    private static string ExtractDescriptionFromMethod(MethodInfo method)
    {
        var descriptionAttribute = method
            .GetCustomAttribute<DescriptionAttribute>();

        return descriptionAttribute?.Description ?? "No description available";
    }

    #endregion
}
