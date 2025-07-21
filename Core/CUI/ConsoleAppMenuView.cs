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

///<summary>The application menu view that displays the menu on the console</summary>
internal class ConsoleAppMenuView : IAppMenuView
{
    #region Private Members

    ///<summary>Separator line.</summary>
    private static readonly string Underline = new string('-', 79);

    private readonly string m_MenuHeaderText;
    private readonly List<string> m_MenuItems = new List<string>();

    private int m_CursorTop;
    private int m_MenuConsoleLeft;

    private string m_ProcessName;

    private bool m_ShouldQuit;

    #endregion

    #region Ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppMenuController"/> class.
    /// </summary>
    /// <param name="menuHeaderText">The intro text.</param>
    public ConsoleAppMenuView(string menuHeaderText)
    {
        m_ShouldQuit = false;
        m_MenuHeaderText = menuHeaderText;
        m_CursorTop = 0;
        m_MenuConsoleLeft = 0;
        m_ProcessName = string.Empty;
    }

    #endregion

    #region Public View Properties

    /// <summary>
    /// Gets a value indicating whether the user wants to quit the application.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> if user hitted ESC to quit the application; otherwise, <see langword="false"/>.
    /// </value>
    public bool ShouldQuit
    {
        get { return m_ShouldQuit; }
    }

    #endregion

    #region Public View Methods

    #region Init Console View Methods

    /// <summary>
    /// Inits the view.
    /// </summary>
    /// <param name="menuItems">The menu items.</param>
    public void InitView(IEnumerable<string> menuItems)
    {
        InitProcessInfo();
        m_MenuItems.Clear();
        m_MenuItems.AddRange(menuItems);
    }

    #endregion

    #region Console User Interface Output Methds

    /// <summary>
    /// Displays the console menu.
    /// </summary>
    public void DisplayMenu()
    {
        Console.Clear();
        int menuMaxWidth = 0;
        for (int i = 0; i < m_MenuItems.Count; ++i)
        {
            string menuItemText = @"[{0}] {1}".SafeFormatWith(Convert.ToChar(i + 65), m_MenuItems[i]);
            if (menuMaxWidth < menuItemText.Length)
                menuMaxWidth = menuItemText.Length;
        }

        m_MenuConsoleLeft = (Console.BufferWidth - menuMaxWidth) / 2;

        m_CursorTop = Console.CursorTop;
        WriteMenuHeader();

        for (int i = 0; i < m_MenuItems.Count; ++i)
        {
            WriteMenuItemCmd(i);
            m_CursorTop++;
        }

        WriteMenuFooter();
    }

    /// <summary>
    /// Clears the console view
    /// </summary>
    public void ClearView()
    {
        Console.Clear();
    }

    /// <summary>
    /// Writes the menu operation header to the console.
    /// </summary>
    /// <param name="headerText">The menu operation header text.</param>
    public void WriteMenuOperationHeader(string headerText)
    {
        ConsoleHelper.WriteLineWhite(Underline);
        ConsoleHelper.WriteLineWhite(headerText);
        ConsoleHelper.WriteLineWhite(Underline);
    }

    /// <summary>
    /// Shows the exception details on the console.
    /// </summary>
    /// <param name="exception">The exception to show.</param>
    public void ShowExceptionDetails(Exception exception)
    {
        ConsoleHelper.WriteLineRed(@"Exception type {0} was thrown.".SafeFormatWith(exception.GetType().ToString()));
        ConsoleHelper.WriteLineRed(@"Message: '{0}'".SafeFormatWith(exception.Message));
        ConsoleHelper.WriteLineRed(@"Source: '{0}'".SafeFormatWith(exception.Source));
        if (null == exception.InnerException)
        {
            ConsoleHelper.WriteLineRed(@"No Inner Exception");
        }
        else
        {
            Console.WriteLine();
            ConsoleHelper.WriteLineRed(@"Inner Exception: {0}".SafeFormatWith(exception.InnerException.ToString()));
        }
        Console.WriteLine();
    }

    #endregion

    #region Console User Input Methods

    /// <summary>
    /// Prompts to continue after menu item operation execution.
    /// </summary>
    public void PromptToContinue()
    {
        if (!m_ShouldQuit)
        {
            Console.WriteLine();
            ConsoleHelper.Write(@"Press any key to continue or [ESC] to quit...", ConsoleColor.Green);
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
            {
                m_ShouldQuit = true;
            }
        }
    }

    /// <summary>
    /// Waits for valid user menu item selection.
    /// </summary>
    /// <returns>Selected meu item index or -1 if user selected quit option.</returns>
    public int WaitForValidUserInput()
    {
        int selectedOptionIndex = -1;

        while (!m_ShouldQuit && !SelectedOptionInValidRange(selectedOptionIndex))
        {
            selectedOptionIndex = WaitForUserInput();
        }

        return selectedOptionIndex;
    }

    #endregion

    #endregion

    #region Private Members

    private int WaitForUserInput()
    {
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape)
        {
            m_ShouldQuit = true;
            return -1;
        }
        return (int)key.Key - (int)ConsoleKey.A;
    }

    private bool SelectedOptionInValidRange(int selectedOptionIndex)
    {
        return selectedOptionIndex >= 0 && selectedOptionIndex < m_MenuItems.Count;
    }

    private void InitProcessInfo()
    {
        Assembly? entryAssembly = Assembly.GetEntryAssembly();

        if ((entryAssembly.IsNull()))
            m_ProcessName = string.Empty;
        else
            m_ProcessName = entryAssembly!.GetName(false).Name + " v" + entryAssembly!.GetName(false).Version;
    }

    private void WriteMenuItemCmd(int index)
    {
        Console.CursorTop = m_CursorTop;
        Console.CursorLeft = m_MenuConsoleLeft;
        ConsoleHelper.WriteLineYellow(@"[{0}] {1}".SafeFormatWith(Convert.ToChar(index + 65), m_MenuItems[index]));
    }

    private void WriteMenuHeader()
    {
        string menuHeaderText = m_MenuHeaderText;
        int underlineConsoleLeft = (Console.BufferWidth - Underline.Length) / 2;

        Console.CursorTop = m_CursorTop;
        Console.CursorLeft = underlineConsoleLeft;
        m_CursorTop++;
        ConsoleHelper.WriteLineWhite(Underline);

        Console.CursorTop = m_CursorTop;
        Console.CursorLeft = (Console.BufferWidth - menuHeaderText.Length) / 2;
        m_CursorTop++;
        ConsoleHelper.WriteLineWhite(menuHeaderText);

        Console.CursorTop = m_CursorTop;
        Console.CursorLeft = underlineConsoleLeft;
        m_CursorTop++;
        ConsoleHelper.WriteLineWhite(Underline);
        m_CursorTop++;
    }

    private void WriteMenuFooter()
    {
        string menuFooterText = @"> Select option or [ESC] to quit...";

        m_CursorTop++;
        Console.CursorTop = m_CursorTop;
        Console.CursorLeft = (Console.BufferWidth - menuFooterText.Length) / 2;
        ConsoleHelper.Write(menuFooterText, ConsoleColor.Green);
    }

    #endregion
}
