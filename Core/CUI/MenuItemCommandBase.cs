//--------------------------------------------------------------------------
// File:    MenuItemCommandBase.cs
// Content:	Implementation of class MenuItemCommandBase
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Abstract base class that encapsulates the behavior of a single menu item
/// and provides both synchronous and asynchronous execution capabilities.
/// Implements the Command pattern for menu operations.
/// </summary>
/// Abstract base class that encapsulates the behavior of a single menu item
/// and provides both synchronous and asynchronous execution capabilities.
/// Implements the Command pattern for menu operations.
/// </summary>
public abstract class MenuItemCommandBase
{
    #region Public Methods

    /// <summary>
    /// Executes the menu option synchronously.
    /// This method delegates to the abstract DoExecute implementation.
    /// </summary>
    public virtual void Execute()
    {
        DoExecute();
    }

    /// <summary>
    /// Executes the menu option asynchronously with optimal performance.
    /// This method delegates to the virtual DoExecuteAsync implementation.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public virtual async Task ExecuteAsync()
    {
        await DoExecuteAsync().ConfigureAwait(false);
    }

    #endregion

    #region Abstract and Virtual Members

    /// <summary>
    /// Gets the display text for the menu item.
    /// Must be implemented by derived classes to provide menu item description.
    /// </summary>
    /// <value>The text to display in the menu interface</value>
    public abstract string Text { get; }

    /// <summary>
    /// Executes the menu item operation synchronously.
    /// Must be implemented by derived classes to define the actual operation logic.
    /// </summary>
    protected abstract void DoExecute();

    /// <summary>
    /// Executes the menu item operation asynchronously.
    /// Default implementation wraps the synchronous execution in a task.
    /// Override this method in derived classes for true asynchronous operations.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    protected virtual async Task DoExecuteAsync()
    {
        await Task.Run(DoExecute).ConfigureAwait(false);
    }

    #endregion
}
