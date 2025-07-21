//--------------------------------------------------------------------------
// File:    IAppMenuView.cs
// Content:	Application Menu View Contract.
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core;

/// <summary>
/// Application Menu View Contract
/// </summary>
public interface IAppMenuView
{
    /// <summary>
    /// Gets a value indicating whether the user wants to quit the application.
    /// </summary>
    /// <value>
    ///   <c>true</c> if user hitted ESC to quit the application; otherwise, <c>false</c>.
    /// </value>
    bool ShouldQuit { get; }

    /// <summary>
    /// Inits the view.
    /// </summary>
    /// <param name="menuItems">The menu items.</param>
    void InitView(IEnumerable<string> menuItems);

    /// <summary>
    /// Displays the menu.
    /// </summary>
    void DisplayMenu();

    /// <summary>
    /// Clears the view.
    /// </summary>
    void ClearView();

    /// <summary>
    /// Writes the menu operation header.
    /// </summary>
    /// <param name="headerText">The menu operation header text.</param>
    void WriteMenuOperationHeader(string headerText);

    /// <summary>
    /// Shows the exception details.
    /// </summary>
    /// <param name="exception">The exception to show.</param>
    void ShowExceptionDetails(Exception exception);

    /// <summary>
    /// Waits for valid user menu item selection.
    /// </summary>
    /// <returns>Selected meu item index or -1 if user selected quit option.</returns>
    int WaitForValidUserInput();

    /// <summary>
    /// Prompts to continue after menu item operation execution.
    /// </summary>
    void PromptToContinue();
}
