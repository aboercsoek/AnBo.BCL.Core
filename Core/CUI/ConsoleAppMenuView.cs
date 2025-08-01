//--------------------------------------------------------------------------
// File:    ConsoleAppMenuView.cs
// Content:	Implementation of class ConsoleAppMenuView
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Reflection;

#endregion

namespace AnBo.Core;

/// <summary>
/// Console-based implementation of the application menu view.
/// Provides a text-based user interface for menu display and interaction.
/// Features centered layout, color-coded output, and keyboard navigation.
/// </summary>
public class ConsoleAppMenuView : IAppMenuView
{
    #region Private Constants and Fields

    /// <summary>
    /// Standard separator line for visual formatting
    /// </summary>
    private static readonly string Underline = new string('-', 79);

    private readonly string _menuHeaderText;
    private readonly List<string> _menuItems = [];

    private int _cursorTop;
    private int _menuConsoleLeft;

    private string _processName = string.Empty;

    private bool _shouldQuit;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the console menu view.
    /// Sets up the display parameters and process information.
    /// </summary>
    /// <param name="menuHeaderText">
    /// The header text to display at the top of the menu
    /// </param>
    public ConsoleAppMenuView(string menuHeaderText)
    {
        _menuHeaderText = menuHeaderText;
        _shouldQuit = false;
        _cursorTop = 0;
        _menuConsoleLeft = 0;
    }

    #endregion

    #region IAppMenuView Implementation

    /// <summary>
    /// Gets a value indicating whether the user wants to quit the application.
    /// This property is set when the user presses the ESC key.
    /// </summary>
    /// <value>
    /// true if the user has requested to quit; otherwise, false
    /// </value>
    public bool ShouldQuit => _shouldQuit;

    /// <summary>
    /// Initializes the view with the specified menu items.
    /// Prepares the internal state and gathers process information for display.
    /// </summary>
    /// <param name="menuItems">The collection of menu item texts to display</param>
    public void InitView(IEnumerable<string> menuItems)
    {
        InitializeProcessInfo();
        _menuItems.Clear();
        _menuItems.AddRange(menuItems);
    }

    /// <summary>
    /// Displays the complete menu interface including header, items, and footer.
    /// Automatically centers the content and calculates optimal layout.
    /// </summary>
    public void DisplayMenu()
    {
        Console.Clear();

        // Calculate the maximum width needed for proper centering
        var menuMaxWidth = CalculateMaxMenuWidth();
        _menuConsoleLeft = Math.Max(0, (Console.BufferWidth - menuMaxWidth) / 2);

        _cursorTop = Console.CursorTop;

        WriteMenuHeader();
        WriteMenuItems();
        WriteMenuFooter();
    }

    /// <summary>
    /// Clears the console display.
    /// Used to refresh the screen before displaying new content.
    /// </summary>
    public void ClearView()
    {
        Console.Clear();
    }

    /// <summary>
    /// Writes a header for the currently executing menu operation.
    /// Provides visual feedback about the active operation.
    /// </summary>
    /// <param name="headerText">The header text to display</param>
    public void WriteMenuOperationHeader(string headerText)
    {
        ConsoleHelper.WriteLineWhite(Underline);
        ConsoleHelper.WriteLineWhite(headerText);
        ConsoleHelper.WriteLineWhite(Underline);
    }

    /// <summary>
    /// Displays comprehensive exception details in a user-friendly format.
    /// Shows exception type, message, source, and inner exception information.
    /// </summary>
    /// <param name="exception">The exception to display</param>
    public void ShowExceptionDetails(Exception exception)
    {
        ConsoleHelper.WriteLineRed($"Exception type {exception.GetType()} was thrown.");
        ConsoleHelper.WriteLineRed($"Message: '{exception.Message}'");
        ConsoleHelper.WriteLineRed($"Source: '{exception.Source}'");

        if (exception.InnerException is null)
        {
            ConsoleHelper.WriteLineRed("No Inner Exception");
        }
        else
        {
            Console.WriteLine();
            ConsoleHelper.WriteLineRed($"Inner Exception: {exception.InnerException}");
        }

        Console.WriteLine();
    }



    /// <summary>
    /// Waits for and validates user menu item selection.
    /// Continues prompting until a valid selection is made or quit is requested.
    /// </summary>
    /// <returns>
    /// The selected menu item index (0-based), or -1 if quit was requested
    /// </returns>
    public int WaitForValidUserInput()
    {
        int selectedOptionIndex = -1;

        while (!_shouldQuit && !IsSelectedOptionInValidRange(selectedOptionIndex))
        {
            selectedOptionIndex = WaitForSingleKeyInput();
        }

        return selectedOptionIndex;
    }

    /// <summary>
    /// Prompts the user to continue after operation execution.
    /// Allows the user to review results before returning to the menu.
    /// </summary>
    public void PromptToContinue()
    {
        if (_shouldQuit) return;

        Console.WriteLine();
        ConsoleHelper.Write("Press any key to continue or [ESC] to quit...", ConsoleColor.Green);

        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape)
        {
            _shouldQuit = true;
        }
    }

    #endregion

    #region Private Helper Members

    /// <summary>
    /// Calculates the maximum width needed for menu display.
    /// Used for proper centering and layout calculations.
    /// </summary>
    /// <returns>The maximum width in characters</returns>
    private int CalculateMaxMenuWidth()
    {
        var maxWidth = 0;

        for (var i = 0; i < _menuItems.Count; i++)
        {
            var menuItemText = $"[{Convert.ToChar(i + 65)}] {_menuItems[i]}";
            maxWidth = Math.Max(maxWidth, menuItemText.Length);
        }

        return maxWidth;
    }

    /// <summary>
    /// Writes all menu items to the console with proper formatting.
    /// Each item is prefixed with a letter key for selection.
    /// </summary>
    private void WriteMenuItems()
    {
        for (var i = 0; i < _menuItems.Count; i++)
        {
            WriteMenuItemCommand(i);
            _cursorTop++;
        }
    }

    /// <summary>
    /// Waits for a single key press from the user.
    /// Handles the ESC key for quit functionality.
    /// </summary>
    /// <returns>
    /// The menu item index corresponding to the pressed key,
    /// or -1 if ESC was pressed
    /// </returns>
    private int WaitForSingleKeyInput()
    {
        var key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.Escape)
        {
            _shouldQuit = true;
            return -1;
        }

        // Convert key to menu item index (A=0, B=1, etc.)
        return (int)key.Key - (int)ConsoleKey.A;
    }

    /// <summary>
    /// Validates whether the selected option index is within the valid range.
    /// </summary>
    /// <param name="selectedOptionIndex">The index to validate</param>
    /// <returns>true if the index is valid; otherwise, false</returns>
    private bool IsSelectedOptionInValidRange(int selectedOptionIndex)
    {
        return selectedOptionIndex >= 0 && selectedOptionIndex < _menuItems.Count;
    }

    /// <summary>
    /// Initializes process information for display in the menu header.
    /// Extracts application name and version from the entry assembly.
    /// </summary>
    private void InitializeProcessInfo()
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null)
        {
            _processName = string.Empty;
            return;
        }

        var assemblyName = entryAssembly.GetName();
        _processName = $"{assemblyName.Name} v{assemblyName.Version}";
    }

    /// <summary>
    /// Writes a single menu item command to the console.
    /// Positions the cursor correctly and applies color formatting.
    /// </summary>
    /// <param name="index">The zero-based index of the menu item</param>
    private void WriteMenuItemCommand(int index)
    {
        Console.CursorTop = _cursorTop;
        Console.CursorLeft = _menuConsoleLeft;

        var menuChar = Convert.ToChar(index + 65);
        var menuText = $"[{menuChar}] {_menuItems[index]}";

        ConsoleHelper.WriteLineYellow(menuText);
    }

    /// <summary>
    /// Writes the menu header with proper centering and formatting.
    /// Includes the application title and decorative underlines.
    /// </summary>
    private void WriteMenuHeader()
    {
        var underlineConsoleLeft = Math.Max(0, (Console.BufferWidth - Underline.Length) / 2);
        var headerConsoleLeft = Math.Max(0, (Console.BufferWidth - _menuHeaderText.Length) / 2);

        // Top underline
        SetCursorPosition(_cursorTop++, underlineConsoleLeft);
        ConsoleHelper.WriteLineWhite(Underline);

        // Header text
        SetCursorPosition(_cursorTop++, headerConsoleLeft);
        ConsoleHelper.WriteLineWhite(_menuHeaderText);

        // Bottom underline
        SetCursorPosition(_cursorTop++, underlineConsoleLeft);
        ConsoleHelper.WriteLineWhite(Underline);

        _cursorTop++; // Add spacing
    }

    /// <summary>
    /// Writes the menu footer with user instructions.
    /// Includes selection and quit instructions.
    /// </summary>
    private void WriteMenuFooter()
    {
        const string footerText = "> Select option or [ESC] to quit...";
        var footerConsoleLeft = Math.Max(0, (Console.BufferWidth - footerText.Length) / 2);

        _cursorTop++; // Add spacing before footer
        SetCursorPosition(_cursorTop, footerConsoleLeft);
        ConsoleHelper.Write(footerText, ConsoleColor.Green);
    }

    /// <summary>
    /// Sets the console cursor position safely.
    /// Ensures the position is within valid console bounds.
    /// </summary>
    /// <param name="top">The top position (row)</param>
    /// <param name="left">The left position (column)</param>
    private static void SetCursorPosition(int top, int left)
    {
        Console.CursorTop = Math.Max(0, Math.Min(top, Console.BufferHeight - 1));
        Console.CursorLeft = Math.Max(0, Math.Min(left, Console.BufferWidth - 1));
    }

    #endregion
}
