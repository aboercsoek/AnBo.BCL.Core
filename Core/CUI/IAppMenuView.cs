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
/// Defines the contract for application menu view implementations.
/// This interface abstracts the presentation layer from the menu logic,
/// allowing for different UI implementations (console, GUI, web, etc.).
/// </summary>
public interface IAppMenuView
{
    #region Properties

    /// <summary>
    /// Gets a value indicating whether the user wants to quit the application.
    /// This property is checked by the menu controller to determine when to exit.
    /// </summary>
    /// <value>
    /// true if the user has requested to quit the application (e.g., pressed ESC);
    /// otherwise, false
    /// </value>
    bool ShouldQuit { get; }

    #endregion

    #region Initialization Methods

    /// <summary>
    /// Initializes the view with the provided menu items.
    /// This method prepares the view for displaying the menu options.
    /// </summary>
    /// <param name="menuItems">
    /// A collection of menu item text strings to be displayed
    /// </param>
    void InitView(IEnumerable<string> menuItems);

    #endregion

    #region Display Methods

    /// <summary>
    /// Displays the complete menu interface to the user.
    /// This includes headers, menu options, and user instruction prompts.
    /// </summary>
    void DisplayMenu();

    /// <summary>
    /// Clears the current view content.
    /// Used to refresh the display or prepare for new content.
    /// </summary>
    void ClearView();

    /// <summary>
    /// Displays a header for the currently executing menu operation.
    /// This provides user feedback about which operation is running.
    /// </summary>
    /// <param name="headerText">The header text to display</param>
    void WriteMenuOperationHeader(string headerText);

    /// <summary>
    /// Displays detailed exception information to the user.
    /// This method handles error presentation in a user-friendly format.
    /// </summary>
    /// <param name="exception">The exception details to display</param>
    void ShowExceptionDetails(Exception exception);

    #endregion

    #region User Interaction Methods

    /// <summary>
    /// Waits for and validates user menu item selection.
    /// This method handles input validation and user interaction logic.
    /// </summary>
    /// <returns>
    /// The selected menu item index (0-based), or -1 if the user chose to quit
    /// </returns>
    int WaitForValidUserInput();

    /// <summary>
    /// Prompts the user to continue after menu operation execution.
    /// This provides a pause mechanism and allows users to review results.
    /// </summary>
    void PromptToContinue();

    #endregion
}
