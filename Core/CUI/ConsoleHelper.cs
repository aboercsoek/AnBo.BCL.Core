//--------------------------------------------------------------------------
// File:    ConsoleHelper.cs
// Content:	A modern console helper implementation for .NET 8+ with
//          integrated Console2File support.
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// ConsoleHelper is a modern utility class that extends the console class with useful methods optimized for .NET 8+.
/// Provides thread-safe console operations, integrated file redirection management, and high-performance text formatting.
/// Seamlessly integrates with the Console2File ecosystem for comprehensive logging solutions.
/// </summary>
/// <remarks>
/// <para>
/// This modernized ConsoleHelper provides a comprehensive solution for console operations with built-in
/// support for file redirection, advanced formatting, and both synchronous and asynchronous logging patterns.
/// The class integrates seamlessly with the Console2File ecosystem to provide enterprise-grade logging capabilities.
/// </para>
/// <para>
/// Key features include:
/// </para>
/// <list type="bullet">
/// <item><description>Thread-safe console operations with optimized performance</description></item>
/// <item><description>Integrated Console2File redirection management</description></item>
/// <item><description>Support for both sync and async file redirection patterns</description></item>
/// <item><description>Advanced input methods with enhanced security features</description></item>
/// <item><description>Comprehensive collection and object display methods</description></item>
/// <item><description>Modern exception handling with detailed formatting</description></item>
/// <item><description>Performance-optimized string operations and color management</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Basic usage with automatic file redirection
/// ConsoleHelper.ConfigureFileRedirection("app.log", redirectionType: RedirectionType.Timestamped);
/// ConsoleHelper.WriteLineGreen("Application started");
/// 
/// // Advanced async redirection
/// await using var asyncRedirect = ConsoleHelper.CreateAsyncRedirection("server.log", 
///     maxSizeBytes: 10 * 1024 * 1024, maxFiles: 20);
/// ConsoleHelper.WriteLineBlue("Server initialized with async logging");
/// 
/// // High-performance logging with custom formatting
/// ConsoleHelper.WriteHeader("Performance Test");
/// ConsoleHelper.WriteNameValue("Transactions", 50000, ConsoleColor.Cyan, ConsoleColor.Yellow);
/// ConsoleHelper.WriteCollection("Results", performanceData, ", ");
/// </code>
/// </example>
public static class ConsoleHelper
{
    #region Private fields and redirection management

    private static readonly object _consoleLock = new();
    private static readonly ConcurrentQueue<IConsole2File> _activeRedirections = new();
    private static IConsole2File? _primaryRedirection;
    private static RedirectionConfiguration _redirectionConfig = new();

    #endregion

    #region Public Configuration Properties

    /// <summary>
    /// Gets or sets the primary redirection configuration.
    /// This configuration is used for automatic redirection management and default file operations.
    /// </summary>
    /// <value>The current redirection configuration containing file paths, rotation settings, and behavior options.</value>
    /// <example>
    /// <code>
    /// ConsoleHelper.RedirectionConfig = new RedirectionConfiguration
    /// {
    ///     BasePath = "logs/application.log",
    ///     RedirectionType = RedirectionType.TimestampedRotating,
    ///     MaxSizeBytes = 5 * 1024 * 1024,
    ///     MaxFiles = 10,
    ///     UseAsync = true
    /// };
    /// </code>
    /// </example>
    public static RedirectionConfiguration RedirectionConfig
    {
        get => _redirectionConfig;
        set => _redirectionConfig = value ?? new RedirectionConfiguration();
    }

    /// <summary>
    /// Gets a value indicating whether file redirection is currently active.
    /// </summary>
    /// <value>True if any redirection is active; otherwise, false.</value>
    public static bool IsRedirectionActive => _primaryRedirection?.IsActive == true;

    /// <summary>
    /// Gets the current primary redirection file path.
    /// </summary>
    /// <value>The file path of the primary redirection, or null if no redirection is active.</value>
    public static string? CurrentRedirectionPath => _primaryRedirection?.FilePath;

    #endregion

    #region Redirection Management Methods

    /// <summary>
    /// Configures automatic file redirection with the specified parameters.
    /// Creates and manages a redirection instance based on the provided configuration.
    /// </summary>
    /// <param name="basePath">The base file path for redirection.</param>
    /// <param name="redirectionType">The type of redirection to create (simple, timestamped, rotating, etc.).</param>
    /// <param name="maxSizeBytes">Maximum file size for rotating logs (ignored for non-rotating types).</param>
    /// <param name="maxFiles">Maximum number of backup files for rotation (ignored for non-rotating types).</param>
    /// <param name="useAsync">Whether to use async redirection for high-performance scenarios.</param>
    /// <param name="timestampFormat">Custom timestamp format for timestamped redirections.</param>
    /// <returns>The created redirection instance for manual management if needed.</returns>
    /// <exception cref="ArgumentException">Thrown when basePath is invalid.</exception>
    /// <exception cref="IOException">Thrown when redirection creation fails.</exception>
    /// <example>
    /// <code>
    /// // Simple file redirection
    /// var redirect = ConsoleHelper.ConfigureFileRedirection("app.log");
    /// ConsoleHelper.WriteLineGreen("Simple redirection active");
    /// 
    /// // Advanced timestamped rotating redirection
    /// var advancedRedirect = ConsoleHelper.ConfigureFileRedirection(
    ///     "logs/production.log",
    ///     RedirectionType.TimestampedRotating,
    ///     maxSizeBytes: 20 * 1024 * 1024,
    ///     maxFiles: 50,
    ///     useAsync: true,
    ///     timestampFormat: "yyyy-MM-dd_HH-mm-ss"
    /// );
    /// 
    /// ConsoleHelper.WriteLineBlue("Production logging with advanced features active");
    /// </code>
    /// </example>
    public static IConsole2File ConfigureFileRedirection(
        string basePath,
        RedirectionType redirectionType = RedirectionType.Simple,
        long maxSizeBytes = 10 * 1024 * 1024,
        int maxFiles = 10,
        //bool useAsync = false,
        string timestampFormat = "yyyyMMdd-HHmmss")
    {
        // Dispose existing primary redirection
        DisposePrimaryRedirection();

        // Create new redirection based on type
        _primaryRedirection = redirectionType switch
        {
            RedirectionType.Simple => new Console2File(basePath),

            RedirectionType.Timestamped => Console2FileExtensions.CreateTimestamped(basePath, timestampFormat),

            RedirectionType.Rotating => Console2FileExtensions.CreateRotating(basePath, maxSizeBytes, maxFiles),

            RedirectionType.TimestampedRotating => Console2FileExtensions.CreateTimestampedRotating(basePath, maxSizeBytes, maxFiles, timestampFormat),

            _ => throw new ArgumentException($"Unknown redirection type: {redirectionType}", nameof(redirectionType))
        };

        // Update configuration
        _redirectionConfig = new RedirectionConfiguration
        {
            BasePath = basePath,
            RedirectionType = redirectionType,
            MaxSizeBytes = maxSizeBytes,
            MaxFiles = maxFiles,
            //UseAsync = useAsync,
            TimestampFormat = timestampFormat
        };

        return _primaryRedirection;
    }

    /// <summary>
    /// Creates a temporary redirection scope that automatically cleans up after the specified duration.
    /// Useful for capturing console output during specific operations or time periods.
    /// </summary>
    /// <param name="filePath">The file path for temporary redirection.</param>
    /// <param name="duration">How long to maintain the redirection.</param>
    /// <param name="useAsync">Whether to use async redirection for better performance.</param>
    /// <returns>A task that completes when the temporary redirection expires.</returns>
    /// <example>
    /// <code>
    /// // Capture output for 30 seconds
    /// var tempTask = ConsoleHelper.CreateTemporaryRedirection("temp.log", TimeSpan.FromSeconds(30));
    /// 
    /// ConsoleHelper.WriteLineYellow("This will be captured for 30 seconds");
    /// // Do some operations...
    /// 
    /// await tempTask;
    /// ConsoleHelper.WriteLineGreen("Temporary capture completed");
    /// </code>
    /// </example>
    public static async Task CreateTemporaryRedirection(string filePath, TimeSpan duration)
    {
        await Console2File.CreateTemporaryRedirection(filePath, duration);
    }

    /// <summary>
    /// Disposes the current primary redirection and restores normal console behavior.
    /// </summary>
    public static void DisposePrimaryRedirection()
    {
        lock (_consoleLock)
        {
            if (_primaryRedirection is IAsyncDisposable asyncDisposable)
            {
                asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
            else
            {
                _primaryRedirection?.Dispose();
            }
            _primaryRedirection = null;
        }
    }

    #endregion

    #region WriteLine Methods

    /// <summary>
    /// Writes a formatted text in a specific color to the console and inserts a line break at the end.
    /// Uses high-performance string interpolation and minimizes color changes for optimal performance.
    /// The current console color is temporarily saved and restored at the end of the method.
    /// </summary>
    /// <param name="format">Format string for the object array in the args parameter</param>
    /// <param name="color">Console color for the text output</param>
    /// <param name="args">Array of objects to format</param>
    /// <exception cref="ArgumentNullException">Thrown when format is null</exception>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteLine("Processing {0} items with {1}% success rate", ConsoleColor.Green, itemCount, successRate);
    /// </code>
    /// </example>
    public static void WriteLine(string format, ConsoleColor color, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(format);
        WriteLineInternal(format.SafeFormatWith(args), color);
    }


    /// <summary>
    /// Writes a text in a specific color to the console and inserts a line break at the end.
    /// Optimized for single string output with minimal overhead.
    /// The current console color is temporarily saved and restored at the end of the method.
    /// </summary>
    /// <param name="text">Output text</param>
    /// <param name="color">Console color for the text output</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteLine("Operation completed successfully", ConsoleColor.Green);
    /// </code>
    /// </example>
    public static void WriteLine(string? text, ConsoleColor color)
    {
        if (text == null)
        {
            Console.WriteLine();
            return;
        }
        WriteLineInternal(text, color);
    }

    /// <summary>
    /// Writes an object's string representation in a specific color to the console and inserts a line break.
    /// Uses the optimized ToInvariantString extension method for consistent formatting.
    /// The current console color is temporarily saved and restored at the end of the method.
    /// </summary>
    /// <param name="value">Object to convert and output</param>
    /// <param name="color">Console color for the text output</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteLine(42.5m, ConsoleColor.Cyan);
    /// ConsoleHelper.WriteLine(DateTime.Now, ConsoleColor.Yellow);
    /// </code>
    /// </example>
    public static void WriteLine(object? value, ConsoleColor color)
    {
        WriteLineInternal(value.ToInvariantString(), color);
    }

    /// <summary>
    /// Write Exception.Message an Exception.Type to the console.
    /// If present InnerException.Message and InnnerException.Type are also written to the Console.
    /// WriteLine(Exception) uses ConsoleColor.Red to print the exception message(s).
    /// </summary>
    /// <param name="e">Exception that should be written to the console.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine.txt" />
    /// </example>
    public static void WriteLine(Exception e)
    {
        WriteLine(e, true);
    }

    /// <summary>
    /// Writes exception information to the console in red color.
    /// Includes exception type, message, and optionally the stack trace.
    /// Uses the modern ExceptionHelper for comprehensive exception formatting.
    /// </summary>
    /// <param name="exception">Exception to write to the console</param>
    /// <param name="showStackTrace">If true, includes the complete stack trace in the output</param>
    /// <example>
    /// <code>
    /// try 
    /// {
    ///     // Some operation that might throw
    /// }
    /// catch (Exception ex)
    /// {
    ///     ConsoleHelper.WriteLine(ex, showStackTrace: true);
    /// }
    /// </code>
    /// </example>
    public static void WriteLine(Exception? exception, bool showStackTrace = true)
    {
        if (exception == null) return;

        var exceptionText = showStackTrace
            ? ExceptionHelper.GetExceptionText(exception)
            : $"Exception {exception.GetType().GetTypeName()}:\nMessage: {exception.Message}\n";

        WriteLineRed(exceptionText);
    }

    #endregion

    #region Write Methods

    /// <summary>
    /// Writes a formatted text in a specific color to the console without a line break.
    /// Uses high-performance string formatting and minimizes console color operations.
    /// The current console color is temporarily saved and restored at the end of the method.
    /// </summary>
    /// <param name="format">Format string for the object array in the args parameter</param>
    /// <param name="color">Console color for the text output</param>
    /// <param name="args">Array of objects to format</param>
    /// <exception cref="ArgumentNullException">Thrown when format is null</exception>
    /// <example>
    /// <code>
    /// ConsoleHelper.Write("Progress: {0}%", ConsoleColor.Yellow, progressPercentage);
    /// </code>
    /// </example>
    public static void Write(string format, ConsoleColor color, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(format);
        WriteInternal(format.SafeFormatWith(args), color);
    }

    /// <summary>
    /// Writes an object's string representation in a specific color to the console without a line break.
    /// Uses the optimized ToInvariantString extension method for consistent formatting.
    /// The current console color is temporarily saved and restored at the end of the method.
    /// </summary>
    /// <param name="value">Object to convert and output</param>
    /// <param name="color">Console color for the text output</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.Write("Loading", ConsoleColor.Yellow);
    /// for (int i = 0; i < 3; i++)
    /// {
    ///     Thread.Sleep(500);
    ///     ConsoleHelper.Write(".", ConsoleColor.Yellow);
    /// }
    /// </code>
    /// </example>
    public static void Write(object? value, ConsoleColor color)
    {
        WriteInternal(value.ToInvariantString(), color);
    }

    #endregion

    #region Internal optimized core methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteLineInternal(string text, ConsoleColor color)
    {
        lock (_consoleLock)
        {
            WriteToConsoleWithColor(text, color, true);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteInternal(string text, ConsoleColor color)
    {
        lock (_consoleLock)
        {
            WriteToConsoleWithColor(text, color, false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteToConsoleWithColor(string text, ConsoleColor color, bool addNewLine)
    {
        var originalColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            if (addNewLine)
                Console.WriteLine(text);
            else
                Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    #endregion

    #region Special Write Methods (Header, NewLine, HR, ...)

    /// <summary>
    /// Writes a header text surrounded by horizontal lines made of '=' characters.
    /// Creates a visually prominent section header in the console output.
    /// </summary>
    /// <param name="title">Text that appears between the two horizontal lines</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteHeader("Application Started");
    /// // Output:
    /// // ===================
    /// // Application Started
    /// // ===================
    /// </code>
    /// </example>
    public static void WriteHeader(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return;

        var length = title.Length;
        HR(length, '=', ConsoleColor.White);
        WriteLine(title, ConsoleColor.White);
        HR(length, '=', ConsoleColor.White);
    }

    /// <summary>
    /// Writes a simple header with the title text and underlines it with '-' characters.
    /// Creates a clean section separator in the console output.
    /// </summary>
    /// <param name="title">Text that appears underlined</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteSimpleHeader("Configuration");
    /// // Output:
    /// //
    /// // Configuration
    /// // -------------
    /// </code>
    /// </example>
    public static void WriteSimpleHeader(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return;

        NewLine();
        WriteLine(title, ConsoleColor.White);
        HR(title.Length, '-', ConsoleColor.White);
    }


    /// <summary>
    /// Inserts a line break into the console output.
    /// Thread-safe operation that works seamlessly with file redirection.
    /// </summary>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteLine("First line", ConsoleColor.Green);
    /// ConsoleHelper.NewLine();
    /// ConsoleHelper.WriteLine("Second line after blank line", ConsoleColor.Blue);
    /// </code>
    /// </example>
    public static void NewLine()
    {
        lock (_consoleLock)
        {
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Inserts a horizontal line of 40 characters using the current console foreground color.
    /// Creates a visual separator in the console output.
    /// </summary>
    /// <example>
    /// <code>
    /// ConsoleHelper.HR();
    /// // Output: ----------------------------------------
    /// </code>
    /// </example>
    public static void HR() => HR(40, Console.ForegroundColor);

    /// <summary>
    /// Inserts a horizontal line of 40 characters in the specified color.
    /// Creates a visual separator in the console output.
    /// </summary>
    /// <param name="color">Console color for the horizontal line</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.HR(ConsoleColor.Green);
    /// // Output: ---------------------------------------- (in green)
    /// </code>
    /// </example>
    public static void HR(ConsoleColor color) => HR(40, color);

    /// <summary>
    /// Inserts a horizontal line of the specified length in the specified color.
    /// Creates a customizable visual separator in the console output.
    /// </summary>
    /// <param name="length">Width of the horizontal line</param>
    /// <param name="color">Console color for the horizontal line</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.HR(60, ConsoleColor.Cyan);
    /// // Output: ------------------------------------------------------------ (in cyan)
    /// </code>
    /// </example>
    public static void HR(int length, ConsoleColor color) => HR(length, '-', color);

    /// <summary>
    /// Inserts a horizontal line using the specified character, length, and color.
    /// Provides maximum flexibility for creating visual separators.
    /// Uses optimized string operations for performance.
    /// </summary>
    /// <param name="length">Width of the horizontal line</param>
    /// <param name="hrChar">Character used to draw the horizontal line</param>
    /// <param name="color">Console color for the horizontal line</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.HR(50, '*', ConsoleColor.Yellow);
    /// // Output: ************************************************** (in yellow)
    /// </code>
    /// </example>
    public static void HR(int length, char hrChar, ConsoleColor color)
    {
        if (length <= 0) return;

        // Use string constructor for optimal performance
        var line = new string(hrChar, length);
        WriteLine(line, color);
    }

    #endregion

    #region Input Methods

    #region ReadLine Methods

    /// <summary>
    /// Displays a prompt message in the specified color and reads a line of input from the user.
    /// Provides a clean interface for user input with colored prompts.
    /// </summary>
    /// <param name="text">Prompt text displayed before reading input</param>
    /// <param name="color">Color of the prompt text</param>
    /// <returns>User-entered text, or null if the user pressed Ctrl+C</returns>
    /// <example>
    /// <code>
    /// string? userInput = ConsoleHelper.ReadLine("Enter your name: ", ConsoleColor.Cyan);
    /// if (userInput != null)
    /// {
    ///     ConsoleHelper.WriteLineGreen($"Hello, {userInput}!");
    /// }
    /// </code>
    /// </example>
    public static string? ReadLine(string? text, ConsoleColor color)
    {
        var prompt = !string.IsNullOrEmpty(text) ? text : "Hit <ENTER> to proceed!";
        WriteLine(prompt, color);
        return Console.ReadLine();
    }

    /// <summary>
    /// Displays a prompt message and reads a line of input from the user.
    /// Uses the default console color for the prompt.
    /// </summary>
    /// <param name="text">Prompt text displayed before reading input</param>
    /// <returns>User-entered text, or null if the user pressed Ctrl+C</returns>
    /// <example>
    /// <code>
    /// string? fileName = ConsoleHelper.ReadLine("Enter file name: ");
    /// </code>
    /// </example>
    public static string? ReadLine(string? text)
    {
        var prompt = !string.IsNullOrEmpty(text) ? text : "Hit <ENTER> to proceed!";
        Console.WriteLine(prompt);
        return Console.ReadLine();
    }

    #endregion

    #region ReadKey Methods

    /// <summary>
    /// Displays a prompt message in the specified color and waits for a single key press.
    /// Provides control over whether the pressed key is displayed on screen.
    /// </summary>
    /// <param name="text">Prompt text displayed before reading the key</param>
    /// <param name="showEnteredKey">If true, the pressed key is displayed on screen</param>
    /// <param name="color">Color of the prompt text</param>
    /// <returns>Information about the key that was pressed</returns>
    /// <example>
    /// <code>
    /// ConsoleKeyInfo key = ConsoleHelper.ReadKey("Press any key to continue...", false, ConsoleColor.Yellow);
    /// if (key.Key == ConsoleKey.Escape)
    /// {
    ///     ConsoleHelper.WriteLineRed("Operation cancelled");
    /// }
    /// </code>
    /// </example>
    public static ConsoleKeyInfo ReadKey(string? text, bool showEnteredKey, ConsoleColor color)
    {
        var prompt = !string.IsNullOrEmpty(text) ? text : "Hit any key to proceed!";
        WriteLine(prompt, color);
        return Console.ReadKey(!showEnteredKey);
    }

    /// <summary>
    /// Displays a prompt message and waits for a single key press.
    /// Uses the default console color for the prompt.
    /// </summary>
    /// <param name="text">Prompt text displayed before reading the key</param>
    /// <param name="showEnteredKey">If true, the pressed key is displayed on screen</param>
    /// <returns>Information about the key that was pressed</returns>
    /// <example>
    /// <code>
    /// ConsoleKeyInfo key = ConsoleHelper.ReadKey("Continue? (Y/N)", true);
    /// </code>
    /// </example>
    public static ConsoleKeyInfo ReadKey(string? text, bool showEnteredKey)
    {
        var prompt = !string.IsNullOrEmpty(text) ? text : "Hit any key to proceed!";
        Console.WriteLine(prompt);
        return Console.ReadKey(!showEnteredKey);
    }

    #region ReadPassword Methods

    /// <summary>
    /// Reads a password and stores it in a string object.
    /// Characters are masked with the specified password character.
    /// Supports backspace for corrections and escape to cancel.
    /// Note: For enhanced security, consider using ReadPasswordSecure instead.
    /// </summary>
    /// <param name="password">String variable to store the entered password</param>
    /// <param name="text">Prompt message displayed before password input</param>
    /// <param name="passwordChar">Character displayed instead of actual password characters</param>
    /// <param name="color">Color of the prompt text and password mask characters</param>
    /// <returns>The key that ended password input (Enter or Escape)</returns>
    /// <example>
    /// <code>
    /// string password = string.Empty;
    /// ConsoleKey result = ConsoleHelper.ReadPassword(ref password, "Enter password: ", '*', ConsoleColor.Yellow);
    /// if (result == ConsoleKey.Enter && !string.IsNullOrEmpty(password))
    /// {
    ///     // Process the password
    /// }
    /// </code>
    /// </example>
    public static ConsoleKey ReadPassword(ref string password, string? text, char passwordChar, ConsoleColor color)
    {
        var passwordBuilder = new StringBuilder();
        password = string.Empty;

        if (!string.IsNullOrEmpty(text))
            WriteLine(text, color);

        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    password = passwordBuilder.ToString();
                    return ConsoleKey.Enter;

                case ConsoleKey.Escape:
                    password = string.Empty;
                    return ConsoleKey.Escape;

                case ConsoleKey.Backspace or ConsoleKey.Delete:
                    if (passwordBuilder.Length > 0)
                    {
                        passwordBuilder.Remove(passwordBuilder.Length - 1, 1);
                        Write("\b \b", color); // Erase the last character visually
                    }
                    break;

                default:
                    if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsPunctuation(keyInfo.KeyChar) || char.IsSymbol(keyInfo.KeyChar))
                    {
                        passwordBuilder.Append(keyInfo.KeyChar);
                        Write(passwordChar.ToString(), color);
                    }
                    break;
            }
        } while (true);
    }

    #endregion

    #endregion

    #endregion

    #region Colored WriteLine Convenience Methods

    /// <summary>
    /// Writes the output of value.ToString() in magenta color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineMagenta(object? value) => WriteLine(value, ConsoleColor.Magenta);

    /// <summary>
    /// Writes the output of value.ToString() in white color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineWhite(object? value) => WriteLine(value, ConsoleColor.White);

    /// <summary>
    /// Writes the output of value.ToString() in yellow color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineYellow(object? value) => WriteLine(value, ConsoleColor.Yellow);

    /// <summary>
    /// Writes the output of value.ToString() in green color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineGreen(object? value) => WriteLine(value, ConsoleColor.Green);

    /// <summary>
    /// Writes the output of value.ToString() in gray color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineGray(object? value) => WriteLine(value, ConsoleColor.Gray);

    /// <summary>
    /// Writes the output of value.ToString() in red color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineRed(object? value) => WriteLine(value, ConsoleColor.Red);

    /// <summary>
    /// Writes the output of value.ToString() in blue color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineBlue(object? value) => WriteLine(value, ConsoleColor.Blue);

    /// <summary>
    /// Writes the output of value.ToString() in cyan color.
    /// </summary>
    /// <param name="value">The output value</param>
    public static void WriteLineCyan(object? value) => WriteLine(value, ConsoleColor.Cyan);

    // Overloads with Paragraph enum support
    /// <summary>
    /// Writes the output of value.ToString() in magenta color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineMagenta(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.Magenta, addPara);

    /// <summary>
    /// Writes the output of value.ToString() in white color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineWhite(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.White, addPara);

    /// <summary>
    /// Writes the output of value.ToString() in yellow color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineYellow(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.Yellow, addPara);

    /// <summary>
    /// Writes the output of value.ToString() in green color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineGreen(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.Green, addPara);

    /// <summary>
    /// Writes the output of value.ToString() in gray color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineGray(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.Gray, addPara);

    /// <summary>
    /// Writes the output of value.ToString() in red color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineRed(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.Red, addPara);

    /// <summary>
    /// Writes the output of value.ToString() in blue color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineBlue(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.Blue, addPara);

    /// <summary>
    /// Writes the output of value.ToString() in cyan color with optional paragraph spacing.
    /// </summary>
    /// <param name="value">The output value</param>
    /// <param name="addPara">Paragraph spacing options</param>
    public static void WriteLineCyan(object? value, Paragraph? addPara) => WriteLineWithParagraph(value, ConsoleColor.Cyan, addPara);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteLineWithParagraph(object? value, ConsoleColor color, Paragraph? addPara)
    {
        addPara ??= Paragraph.Default;

        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, color);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    #endregion

    #region Collection Display Methods

    /// <summary>
    /// Writes the collection content to the console with a descriptive header.
    /// Automatically detects collection type and formats items with separators.
    /// Integrates seamlessly with file redirection when active.
    /// </summary>
    /// <param name="collectionName">Text displayed as the collection header</param>
    /// <param name="collection">The collection to display</param>
    /// <param name="separator">Separator between collection items</param>
    /// <example>
    /// <code>
    /// var numbers = new[] { 1, 2, 3, 4, 5 };
    /// ConsoleHelper.WriteCollection("Numbers", numbers, " | ");
    /// // Output: Numbers { 1 | 2 | 3 | 4 | 5 }
    /// </code>
    /// </example>
    public static void WriteCollection(string? collectionName, IEnumerable? collection, string? separator)
    {
        var header = string.IsNullOrEmpty(collectionName)
            ? $"Collection {collection?.GetType().GetTypeName()} Items = "
            : $"{collectionName.EnsureEndsWith(" ")}";

        Write(header, ConsoleColor.Green);
        WriteCollection(collection, ConsoleColor.White, separator);
    }

    /// <summary>
    /// Writes the collection content to the console in the specified color.
    /// Handles null collections gracefully and provides empty collection indicators.
    /// </summary>
    /// <param name="collection">The collection to display</param>
    /// <param name="collectionValueColor">Color used for collection values</param>
    /// <param name="separator">Separator between collection items</param>
    /// <example>
    /// <code>
    /// var items = new List&lt;string&gt; { "apple", "banana", "cherry" };
    /// ConsoleHelper.WriteCollection(items, ConsoleColor.Cyan, ", ");
    /// // Output:  { apple, banana, cherry }
    /// </code>
    /// </example>
    public static void WriteCollection(IEnumerable? collection, ConsoleColor collectionValueColor, string? separator)
    {
        if (collection == null)
        {
            WriteLine(" <null>", collectionValueColor);
            return;
        }

        separator ??= ", ";
        Write(" { ", collectionValueColor);

        bool isFirst = true;
        foreach (var item in collection)
        {
            if (!isFirst)
                Write(separator, collectionValueColor);
            else
                isFirst = false;

            Write(item.ToInvariantString(), collectionValueColor);
        }

        if (isFirst)
            Write("<empty>", collectionValueColor);

        WriteLine(" }", collectionValueColor);
    }

    /// <summary>
    /// Writes the collection content to the console with full color customization.
    /// Provides complete control over header, values, and separator colors.
    /// </summary>
    /// <param name="collectionName">The name of the collection</param>
    /// <param name="collection">The collection to display</param>
    /// <param name="separator">Separator between collection items</param>
    /// <param name="collectionNameColor">Color used for the collection name</param>
    /// <param name="collectionValueColor">Color used for collection values</param>
    /// <param name="separatorColor">Color used for separators</param>
    /// <example>
    /// <code>
    /// var config = new Dictionary&lt;string, string&gt; { {"key1", "value1"}, {"key2", "value2"} };
    /// ConsoleHelper.WriteCollection("Configuration", config, " | ", 
    ///     ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.Gray);
    /// </code>
    /// </example>
    public static void WriteCollection(string? collectionName, IEnumerable? collection, string? separator,
        ConsoleColor collectionNameColor, ConsoleColor collectionValueColor, ConsoleColor separatorColor)
    {
        var header = string.IsNullOrEmpty(collectionName)
            ? $"Collection {collection?.GetType().GetTypeName()} Items "
            : $"{collectionName.EnsureEndsWith(" ")}";

        Write(header, collectionNameColor);

        if (collection == null)
        {
            Write("= ", separatorColor);
            WriteLine("<null>", collectionValueColor);
            return;
        }

        separator ??= ", ";
        Write("= { ", separatorColor);

        bool isFirst = true;
        foreach (var item in collection)
        {
            if (!isFirst)
                Write(separator, separatorColor);
            else
                isFirst = false;

            Write(item.ToInvariantString(), collectionValueColor);
        }

        if (isFirst)
            Write("<empty>", collectionValueColor);

        WriteLine(" }", separatorColor);
    }

    #endregion

    #region Name-Value Pair Display Methods

    /// <summary>
    /// Writes a name-value pair to the console using default colors.
    /// Uses green for the name, gray for the separator, and white for the value.
    /// </summary>
    /// <param name="name">The property or variable name</param>
    /// <param name="value">The value to display</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteNameValue("UserName", "JohnDoe");
    /// ConsoleHelper.WriteNameValue("Age", 25);
    /// // Output:
    /// // UserName [String] = JohnDoe
    /// // Age [Int32] = 25
    /// </code>
    /// </example>
    public static void WriteNameValue(string name, object? value)
    {
        WriteNameValue(name, value, ConsoleColor.Green, ConsoleColor.White);
    }

    /// <summary>
    /// Writes a name-value pair to the console with custom colors for name and value.
    /// Uses gray for the separator and includes type information.
    /// </summary>
    /// <param name="name">The property or variable name</param>
    /// <param name="value">The value to display</param>
    /// <param name="nameColor">Color used for the name and type information</param>
    /// <param name="valueColor">Color used for the value</param>
    /// <example>
    /// <code>
    /// ConsoleHelper.WriteNameValue("Status", "Active", ConsoleColor.Yellow, ConsoleColor.Green);
    /// </code>
    /// </example>
    public static void WriteNameValue(string name, object? value, ConsoleColor nameColor, ConsoleColor valueColor)
    {
        WriteNameValue(name, value, nameColor, valueColor, " = ", ConsoleColor.Gray);
    }

    /// <summary>
    /// Writes a name-value pair to the console with full customization options.
    /// Supports collections with special formatting and custom separators.
    /// Automatically handles different value types including collections and null values.
    /// </summary>
    /// <param name="name">The property or variable name</param>
    /// <param name="value">The value to display</param>
    /// <param name="nameColor">Color used for the name and type information</param>
    /// <param name="valueColor">Color used for the value</param>
    /// <param name="separator">Custom separator between name and value</param>
    /// <param name="separatorColor">Color used for the separator</param>
    /// <example>
    /// <code>
    /// var items = new[] { "item1", "item2", "item3" };
    /// ConsoleHelper.WriteNameValue("Items", items, ConsoleColor.Cyan, ConsoleColor.White, " -> ", ConsoleColor.Yellow);
    /// // Output: Items [String[]] -> { item1, item2, item3 }
    /// </code>
    /// </example>
    public static void WriteNameValue(string name, object? value, ConsoleColor nameColor, ConsoleColor valueColor,
        string separator, ConsoleColor separatorColor)
    {
        if (value == null)
        {
            Write(name, nameColor);
            Write(" [null]", nameColor);
            Write(separator, separatorColor);
            WriteLine("<null>", valueColor);
            return;
        }

        // Handle collections specially
        if (value is IEnumerable enumerable and not string)
        {
            WriteCollection(name, enumerable, ", ", nameColor, valueColor, separatorColor);
            return;
        }

        // Handle regular values
        Write(name, nameColor);
        Write($" [{value.GetType().GetTypeName()}]", nameColor);
        Write(separator, separatorColor);
        WriteLine(value.ToInvariantString(), valueColor);
    }

    #endregion

    #region Redirection Status and Monitoring Methods

    /// <summary>
    /// Writes information about the current redirection status to the console.
    /// Displays active redirections, file paths, and configuration details.
    /// </summary>
    /// <example>
    /// <code>
    /// ConsoleHelper.ConfigureFileRedirection("app.log", RedirectionType.Timestamped);
    /// ConsoleHelper.WriteRedirectionStatus();
    /// // Output: detailed information about active redirection
    /// </code>
    /// </example>
    public static void WriteRedirectionStatus()
    {
        WriteSimpleHeader("Console Redirection Status");

        if (!IsRedirectionActive)
        {
            WriteLineYellow("No active redirection");
            return;
        }

        WriteNameValue("Active", IsRedirectionActive, ConsoleColor.Green, ConsoleColor.White);
        WriteNameValue("File Path", CurrentRedirectionPath ?? "<unknown>", ConsoleColor.Green, ConsoleColor.Cyan);
        WriteNameValue("Redirection Type", _redirectionConfig.RedirectionType, ConsoleColor.Green, ConsoleColor.Yellow);

        if (_redirectionConfig.RedirectionType == RedirectionType.Rotating ||
            _redirectionConfig.RedirectionType == RedirectionType.TimestampedRotating)
        {
            WriteNameValue("Max Size", $"{_redirectionConfig.MaxSizeBytes:N0} bytes", ConsoleColor.Green, ConsoleColor.White);
            WriteNameValue("Max Files", _redirectionConfig.MaxFiles, ConsoleColor.Green, ConsoleColor.White);
        }

        // Get file information if possible
        if (_primaryRedirection != null && File.Exists(_primaryRedirection.FilePath))
        {
            try
            {
                var fileInfo = new FileInfo(_primaryRedirection.FilePath);
                WriteNameValue("Current Size", $"{fileInfo.Length:N0} bytes", ConsoleColor.Green, ConsoleColor.White);
                WriteNameValue("Last Modified", fileInfo.LastWriteTime, ConsoleColor.Green, ConsoleColor.White);
            }
            catch (Exception ex)
            {
                WriteNameValue("File Info Error", ex.Message, ConsoleColor.Yellow, ConsoleColor.Red);
            }
        }
    }

    /// <summary>
    /// Writes rotation information for the current redirection if applicable.
    /// Shows details about rotated files, sizes, and retention status.
    /// </summary>
    /// <example>
    /// <code>
    /// ConsoleHelper.ConfigureFileRedirection("app.log", RedirectionType.Rotating, maxSizeBytes: 1024 * 1024);
    /// // ... generate some log data ...
    /// ConsoleHelper.WriteRotationInfo();
    /// </code>
    /// </example>
    public static void WriteRotationInfo()
    {
        if (!IsRedirectionActive || CurrentRedirectionPath == null)
        {
            WriteLineYellow("No active redirection for rotation info");
            return;
        }

        var (fileCount, totalSize, oldestFile, newestFile) = Console2FileExtensions.GetRotationInfo(CurrentRedirectionPath);

        if (fileCount == 0)
        {
            WriteLineYellow("No rotation files found");
            return;
        }

        WriteSimpleHeader("Log Rotation Information");
        WriteNameValue("Total Files", fileCount, ConsoleColor.Green, ConsoleColor.White);
        WriteNameValue("Total Size", $"{totalSize:N0} bytes", ConsoleColor.Green, ConsoleColor.White);
        WriteNameValue("Newest File", newestFile ?? "<unknown>", ConsoleColor.Green, ConsoleColor.Cyan);
        WriteNameValue("Oldest File", oldestFile ?? "<unknown>", ConsoleColor.Green, ConsoleColor.Gray);
    }

    #endregion
}
