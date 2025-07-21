//--------------------------------------------------------------------------
// File:    ConsoleHelper.cs
// Content:	A command line parser implementation.
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Security;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// ConsoleHelper is a utility class that extends the console class with some usefull methods
/// </summary>
public static class ConsoleHelper
{
    #region Public Redirect to file fields

    private static string? outputFileName;

    /// <summary>
    /// Output file name used to redirect console output to file.
    /// </summary>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper OutputFileName Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_F_ConsoleHelper_OutputFileName" />
    /// </example>
    public static string? OutputFileName { get => outputFileName; set => outputFileName = value; }

    #endregion

    #region WriteLine Methods

    /// <summary>
    /// Writes a text in a specific color to the console and inserts 
    /// line break at the end of the output string.
    /// The current console color is temporarily saved and is restored 
    /// at the end of the method.
    /// </summary>
    /// <param name="format">Format string for object array in parameter arg</param>
    /// <param name="color">Console color</param>
    /// <param name="args">Array of objects</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine.txt" />
    /// </example>
    public static void WriteLine(string format, ConsoleColor color, params object[] args)
    {
        if (format == null)
            return;

        if (!string.IsNullOrEmpty(OutputFileName))
        {
            using (new Console2File(OutputFileName))
            {
                Console.WriteLine(format.SafeFormatWith(args));
            }
        }
        var tmpColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(format.SafeFormatWith(args));
        Console.ForegroundColor = tmpColor;
    }


    /// <summary>
    /// Writes a text in a specific color to the console and inserts 
    /// a line break at the end of the output string.
    /// The current console color is temporarily saved and is restored 
    /// at the end of the method.
    /// </summary>
    /// <param name="text">Output text</param>
    /// <param name="color">Console color</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine.txt" />
    /// </example>
    public static void WriteLine(string text, ConsoleColor color)
    {
        if (text == null)
        {
            Console.WriteLine();
            return;
        }

        if (!string.IsNullOrEmpty(OutputFileName))
        {
            using (new Console2File(OutputFileName))
            {
                Console.WriteLine(text);
            }
        }
        var tmpColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = tmpColor;
    }

    /// <summary>
    /// Writes a text in a specific color to the console and inserts 
    /// a line break at the end of the output string.
    /// The current console color is temporarily saved and is restored 
    /// at the end of the method.
    /// </summary>
    /// <param name="value">Output text</param>
    /// <param name="color">Console color</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine.txt" />
    /// </example>
    public static void WriteLine(object value, ConsoleColor color)
    {
        if (!string.IsNullOrEmpty(OutputFileName))
        {
            using (new Console2File(OutputFileName))
            {
                Console.WriteLine(value.ToInvariantString());
            }
        }
        var tmpColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(value.ToInvariantString());
        Console.ForegroundColor = tmpColor;
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
    /// Write Exception.Message an Exception.Type to the console.
    /// If present InnerException.Message and InnnerException.Type are also written to the Console.
    /// WriteLine(Exception) uses ConsoleColor.Red to print the exception message(s).
    /// </summary>
    /// <param name="e">Exception that should be written to the console.</param>
    /// <param name="showStackTrace">If true shows also the stack trace.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine.txt" />
    /// </example>
    public static void WriteLine(Exception e, bool showStackTrace)
    {
        if (e == null) return;

        WriteLineRed(showStackTrace
                        ? ExceptionHelper.GetExceptionText(e)
                        : "Exception {0}:\nMessage: {1}\n".SafeFormatWith(e.GetType().GetTypeName(), e.Message));
    }

    #endregion

    #region Write Methods

    /// <summary>
    /// Writes a text in a specific color to the console.
    /// The current console color is temporarily saved and is restored 
    /// at the end of the method.
    /// </summary>
    /// <param name="format">Format string for object array in parameter arg</param>
    /// <param name="color">Console color</param>
    /// <param name="args">Array of objects</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine.txt" />
    /// </example>
    public static void Write(string format, ConsoleColor color, params object[] args)
    {
        if (format == null)
            return;

        if (!string.IsNullOrEmpty(OutputFileName))
        {
            using (new Console2File(OutputFileName))
            {
                Console.Write(format.SafeFormatWith(args));
            }
        }
        var tmpColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(format.SafeFormatWith(args));
        Console.ForegroundColor = tmpColor;
    }

    /// <summary>
    /// Writes a text in a specific color to the console.
    /// The current console color is temporarily saved and is restored 
    /// at the end of the method.
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="color">Console color</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteAndWriteLine.txt" />
    /// </example>
    public static void Write(object value, ConsoleColor color)
    {
        if (!string.IsNullOrEmpty(OutputFileName))
        {
            using (new Console2File(OutputFileName))
            {
                Console.Write(value.ToInvariantString());
            }
        }

        var tmpColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(value.ToInvariantString());
        Console.ForegroundColor = tmpColor;
    }

    #endregion

    #region Special Write Methods (Header, NewLine, HR, ...)

    /// <summary>
    /// Writes the value of titel between lines of '=' characters
    /// </summary>
    /// <param name="title">Text that appears between the two horizontal lines</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Special Write Methods Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_SpecialWrite" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_SpecialWrite.txt" />
    /// </example>
    public static void WriteHeader(string title)
    {
        if (title.IsNullOrEmptyWithTrim())
            return;
        HR(title.Length, '=', ConsoleColor.White);
        Write(title, ConsoleColor.White);
        HR(title.Length, '=', ConsoleColor.White);
    }

    /// <summary>
    /// Writes the value of titel and underlines the title with '-' characters
    /// </summary>
    /// <param name="title">Text that appears underlined</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Special Write Methods Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_SpecialWrite" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_SpecialWrite.txt" />
    /// </example>
    public static void WriteSimpleHeader(string title)
    {
        if (title.IsNullOrEmptyWithTrim())
            return;
        NewLine();
        Write(title, ConsoleColor.White);
        HR(title.Length, '-', ConsoleColor.White);
    }


    /// <summary>
    /// Inserts a line break.
    /// </summary>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Special Write Methods Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_SpecialWrite" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_SpecialWrite.txt" />
    /// </example>
    public static void NewLine()
    {
        if (!string.IsNullOrEmpty(OutputFileName))
        {
            using (new Console2File(OutputFileName))
            {
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }

    /// <summary>Inserts a horizontal line.</summary>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Special Write Methods Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_SpecialWrite" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_SpecialWrite.txt" />
    /// </example>
    // ReSharper disable InconsistentNaming
    public static void HR()
    // ReSharper restore InconsistentNaming
    {
        HR(40, Console.ForegroundColor);
    }

    /// <summary>Inserts a horizontal line.</summary>
    /// <param name="color">Console color</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Special Write Methods Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_SpecialWrite" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_SpecialWrite.txt" />
    /// </example>
    // ReSharper disable InconsistentNaming
    public static void HR(ConsoleColor color)
    // ReSharper restore InconsistentNaming
    {
        HR(40, color);
    }

    /// <summary>Inserts a horizontal line.</summary>
    /// <param name="length">Width of the horizontal line</param>
    /// <param name="color">Console color</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Special Write Methods Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_SpecialWrite" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_SpecialWrite.txt" />
    /// </example>
    // ReSharper disable InconsistentNaming
    public static void HR(int length, ConsoleColor color)
    // ReSharper restore InconsistentNaming
    {
        if (length <= 0)
            return;

        HR(length, '-', color);
    }

    /// <summary>Inserts a horizontal line.</summary>
    /// <param name="length">Width of the horizontal line</param>
    /// <param name="hrChar">Character that is used to write the horizontal line</param>
    /// <param name="color">Console color</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Special Write Methods Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_SpecialWrite" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_SpecialWrite.txt" />
    /// </example>
    // ReSharper disable InconsistentNaming
    public static void HR(int length, char hrChar, ConsoleColor color)
    // ReSharper restore InconsistentNaming
    {
        if (length <= 0)
            return;

        var tmpColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        StringBuilder sb = new StringBuilder(length);
        sb.AppendLine();
        for (int i = 0; i < length; i++)
            sb.Append(hrChar);
        string line = sb.ToString();
        Console.WriteLine(line);
        Console.ForegroundColor = tmpColor;
        if (!string.IsNullOrEmpty(OutputFileName))
        {
            using (new Console2File(OutputFileName))
            {
                Console.WriteLine(line);
            }
        }
    }

    #endregion

    #region ReadLine Methods

    /// <summary>
    /// Calls Console.Writeline(text) in the color provided by the parameter color.
    /// After that the method calls Console.Readline.
    /// </summary>
    /// <param name="text">Text that is written before ReadLine</param>
    /// <param name="color">Color of the written Text</param>
    /// <returns>User entered Text during Console.ReadLine.</returns>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper ReadLine Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_ReadLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_ReadLine.txt" />
    /// </example>
    /// <remarks>
    /// If parameter text is null or empty the method writes
    /// "Hit &lt;ENTER&gt; to proceed!" to the console.<br/>
    /// </remarks>
    public static string? ReadLine(string text, ConsoleColor color)
    {
        WriteLine(!string.IsNullOrEmpty(text) ? text : "Hit <ENTER> to proceed!", color);
        return Console.ReadLine();
    }

    /// <summary>
    /// Calls Console.Writeline(text). After that the method calls Console.Readline.
    /// </summary>
    /// <param name="text">Text that is written before ReadLine</param>
    /// <returns>User entered Text during Console.ReadLine.</returns>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper ReadLine Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_ReadLine" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_ReadLine.txt" />
    /// </example>
    /// <remarks>
    /// If parameter text is null or empty the method writes
    /// "Hit &lt;ENTER&gt; to proceed!" to the console.<br/>
    /// </remarks>
    public static string? ReadLine(string text)
    {
        Console.WriteLine(!string.IsNullOrEmpty(text) ? text : @"Hit <ENTER> to proceed!");
        return Console.ReadLine();
    }

    #endregion

    #region ReadKey Methods

    /// <summary>
    /// Calls Console.Writeline(text). After that the method calls Console.ReadKey.
    /// Text is written in the ConsoleColor that is provided by the parameter color.
    /// </summary>
    /// <param name="text">Text that is written before ReadKey</param>
    /// <param name="showEnteredKey">If true the pressed key is written to the console.</param>
    /// <param name="color">Color of the written Text</param>
    /// <returns>Key information of the key that was pressed.</returns>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper ReadKey Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_ReadKey" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_ReadKey.txt" />
    /// </example>
    /// <remarks>
    /// If parameter text is null or empty the method writes
    /// "Hit any key to proceed!" to the console.<br/>
    /// </remarks>
    public static ConsoleKeyInfo ReadKey(string text, bool showEnteredKey, ConsoleColor color)
    {
        WriteLine(!string.IsNullOrEmpty(text) ? text : "Hit any key to proceed!", color);
        return Console.ReadKey(!showEnteredKey);
    }

    /// <summary>
    /// Calls Console.Writeline(text). After that the method calls Console.ReadKey.
    /// </summary>
    /// <param name="text">Text that is written before ReadKey</param>
    /// <param name="showEnteredKey">If true the pressed key is written to the console.</param>
    /// <returns>Key information of the key that was pressed.</returns>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper ReadKey Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_ReadKey" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_ReadKey.txt" />
    /// </example>
    /// <remarks>
    /// If parameter text is null or empty the method writes
    /// "Hit any key to proceed!" to the console.<br/>
    /// </remarks>
    public static ConsoleKeyInfo ReadKey(string text, bool showEnteredKey)
    {
        Console.WriteLine(!string.IsNullOrEmpty(text) ? text : @"Hit any key to proceed!");
        return Console.ReadKey(!showEnteredKey);
    }

    /// <summary>
    /// Reads a password and stores it in given SecureString object.
    /// </summary>
    /// <param name="password">Enterted password</param>
    /// <param name="text">Message text that is written before the password is read</param>
    /// <param name="passwordChar">Character that is printed instead of clear password characters</param>
    /// <param name="color">Color of text und entered password</param>
    /// <returns>Key that was entered (Escape, Enter)</returns>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper ReadPassword Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_ReadPassword" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_ReadPassword.txt" />
    /// </example>
    public static ConsoleKey ReadPasswordSecure(ref SecureString password, string text, char passwordChar, ConsoleColor color)
    {
        password.Clear();

        if (!string.IsNullOrEmpty(text))
            WriteLine(text, color);

        ConsoleKeyInfo cki;
        bool endRead = false;
        do
        {
            cki = Console.ReadKey(true);
            if (cki.Key == ConsoleKey.Enter)
            {
                endRead = true;
            }
            else if (cki.Key == ConsoleKey.Escape)
            {
                password.Clear();
                endRead = true;
            }
            else if (char.IsLetterOrDigit(cki.KeyChar))
            {
                password.AppendChar(cki.KeyChar);
                Write("{0}", color, passwordChar);
            }
            else if ((cki.Key == ConsoleKey.Delete) || (cki.Key == ConsoleKey.Backspace))
            {
                if (password.Length > 0)
                {
                    password.RemoveAt(password.Length - 1);
                }
            }
        } while (endRead == false);

        return cki.Key;
    }

    /// <summary>
    /// Reads a password and stores it in given string object.
    /// </summary>
    /// <param name="password">Enterted password</param>
    /// <param name="text">Message text that is written before the password is read</param>
    /// <param name="passwordChar">Character that is printed instead of clear password characters</param>
    /// <param name="color">Color of text und entered password</param>
    /// <returns>Key that was entered (Escape, Enter)</returns>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper ReadPassword Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_ReadPassword" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_ReadPassword.txt" />
    /// </example>
    public static ConsoleKey ReadPassword(ref string password, string text, char passwordChar, ConsoleColor color)
    {
        StringBuilder sb = new StringBuilder();
        password = string.Empty;

        if (!string.IsNullOrEmpty(text))
            WriteLine(text, color);

        ConsoleKeyInfo cki;
        bool endRead = false;
        do
        {
            cki = Console.ReadKey(true);
            if (cki.Key == ConsoleKey.Enter)
            {
                password = sb.ToString();
                endRead = true;
            }
            else if (cki.Key == ConsoleKey.Escape)
            {
                password = string.Empty;
                endRead = true;
            }
            else if (char.IsLetterOrDigit(cki.KeyChar))
            {
                sb.Append(cki.KeyChar);
                Write("{0}", color, passwordChar);
            }
            else if ((cki.Key == ConsoleKey.Delete) || (cki.Key == ConsoleKey.Backspace))
            {
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
        } while (endRead == false);

        return cki.Key;
    }

    #endregion

    #region WriteLine<Color> Methods

    /// <summary>
    /// Writes the output of value.ToString() in magenta.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineMagenta(object value)
    {
        WriteLine(value, ConsoleColor.Magenta);
    }

    /// <summary>
    /// Writes the output of value.ToString() in white.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineWhite(object value)
    {
        WriteLine(value, ConsoleColor.White);
    }


    /// <summary>
    /// Writes the output of value.ToString() in yellow.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineYellow(object value)
    {
        WriteLine(value, ConsoleColor.Yellow);
    }

    /// <summary>
    /// Writes the output of value.ToString() in green.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineGreen(object value)
    {
        WriteLine(value, ConsoleColor.Green);
    }

    /// <summary>
    /// Writes the output of value.ToString() in gray.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineGray(object value)
    {
        WriteLine(value, ConsoleColor.Gray);
    }

    /// <summary>
    /// Writes the output of value.ToString() in red.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineRed(object value)
    {
        WriteLine(value, ConsoleColor.Red);
    }

    /// <summary>
    /// Writes the output of value.ToString() in blue.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineBlue(object value)
    {
        WriteLine(value, ConsoleColor.Blue);
    }

    /// <summary>
    /// Writes the output of value.ToString() in <see cref="F:System.ConsoleColor.Cyan"/>.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineCyan(object value)
    {
        WriteLine(value, ConsoleColor.Cyan);
    }

    /// <summary>
    /// Writes the output of value.ToString() in magenta.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineMagenta(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.Magenta);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    /// <summary>
    /// Writes the output of value.ToString() in white.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineWhite(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.White);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }


    /// <summary>
    /// Writes the output of value.ToString() in yellow.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineYellow(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.Yellow);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    /// <summary>
    /// Writes the output of value.ToString() in green.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineGreen(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.Green);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    /// <summary>
    /// Writes the output of value.ToString() in gray.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineGray(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.Gray);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    /// <summary>
    /// Writes the output of value.ToString() in red.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineRed(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.Red);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    /// <summary>
    /// Writes the output of value.ToString() in blue.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineBlue(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.Blue);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    /// <summary>
    /// Writes the output of value.ToString() in cyan.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <param name="addPara">Add additional paragraphs</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper Write and WriteLine Examples" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteColor" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteColor.txt" />
    /// </example>
    public static void WriteLineCyan(object value, Paragraph addPara)
    {
        if ((addPara & Paragraph.AddBefore) == Paragraph.AddBefore)
            NewLine();

        WriteLine(value, ConsoleColor.Cyan);

        if ((addPara & Paragraph.AddAfter) == Paragraph.AddAfter)
            NewLine();
    }

    #endregion

    #region WriteCollection Methods


    /// <summary>
    /// Writes the collection content to the console.
    /// </summary>
    /// <param name="collectionName">Text in the first line.</param>
    /// <param name="collection">The collection.</param>
    /// <param name="separator">The separator.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper WriteCollection Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteCollection" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteCollection.txt" />
    /// </example>
    public static void WriteCollection(string collectionName, IEnumerable collection, string separator)
    {
        Write(string.IsNullOrEmpty(collectionName) ?
                                "Collection {0} Items = ".SafeFormatWith(collection.GetType().GetTypeName()) :
                                collectionName.EnsureEndsWith(" "),
                                ConsoleColor.Green);

        WriteCollection(collection, ConsoleColor.White, separator);
    }

    /// <summary>
    /// Writes the collection content to the console.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="collectionValueColor">Color of the collection values</param>
    /// <param name="separator">The separator.</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper WriteCollection Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteCollection" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteCollection.txt" />
    /// </example>
    public static void WriteCollection(IEnumerable collection, ConsoleColor collectionValueColor, string separator)
    {
        if (collection == null)
        {
            WriteLine(" <null>", collectionValueColor);
            return;
        }

        if (string.IsNullOrEmpty(separator))
            separator = ", ";

        bool firstLoop = true;
        Write(" { ", collectionValueColor);
        foreach (var o in collection)
        {
            if (firstLoop)
                firstLoop = false;
            else
                Write(separator, collectionValueColor);

            Write(o.ToInvariantString(), collectionValueColor);

        }
        if (firstLoop)
            Write("<empty>", collectionValueColor);

        Write(" }\n", collectionValueColor);

    }


    /// <summary>
    /// Writes the collection content to the console.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="collection">The collection.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="collectionNameColor">Color of the collection name</param>
    /// <param name="collectionValueColor">Color of the collection values</param>
    /// <param name="separatorColor">ConsoleColor used for separator</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper WriteCollection Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteCollection" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteCollection.txt" />
    /// </example>
    public static void WriteCollection(string collectionName, IEnumerable? collection, string separator, ConsoleColor collectionNameColor, ConsoleColor collectionValueColor, ConsoleColor separatorColor)
    {
        Write(string.IsNullOrEmpty(collectionName) ?
                    "Collection {0} Items = ".SafeFormatWith(collection?.GetType().GetTypeName()) :
                    collectionName.EnsureEndsWith(" "),
                    collectionNameColor);

        if (collection == null)
        {
            Write("= ", separatorColor);
            WriteLine("<null>", collectionValueColor);
            return;
        }

        if (string.IsNullOrEmpty(separator))
            separator = ", ";

        bool firstLoop = true;
        Write("= { ", separatorColor);
        foreach (var o in collection)
        {
            if (firstLoop)
                firstLoop = false;
            else
                Write(separator, separatorColor);

            Write(o.ToInvariantString(), collectionValueColor);

        }
        if (firstLoop)
            Write("<empty>", collectionValueColor);

        WriteLine(" }", separatorColor);

    }
    #endregion

    #region WriteNameValue Methods

    /// <summary>
    /// Writes a name value pair to the console
    /// </summary>
    /// <param name="name">The name</param>
    /// <param name="value">The value</param>
    /// <remarks>
    /// Writes 'name = value'. 
    /// ConsoleColor.Green is used for name. 
    /// ConsoleColor.Gray is used for equal sign.
    /// ConsoleColor.White is used for value.
    /// </remarks>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper WriteNameValue Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteNameValue" />
    /// </example>
    public static void WriteNameValue(string name, object value)
    {
        WriteNameValue(name, value, ConsoleColor.Green, ConsoleColor.White);
    }

    /// <summary>
    /// Writes a name value pair to the console
    /// </summary>
    /// <param name="name">The name</param>
    /// <param name="value">The value</param>
    /// <param name="nameColor">ConsoleColor used for name</param>
    /// <param name="valueColor">ConsoleColor used for value</param>
    /// <remarks>Uses equal sign as separator. ConsoleColor.Green is used for equal sign.</remarks>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper WriteNameValue Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteNameValue" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteNameValue.txt" />
    /// </example>
    public static void WriteNameValue(string name, object value, ConsoleColor nameColor, ConsoleColor valueColor)
    {
        WriteNameValue(name, value, nameColor, valueColor, " = ", ConsoleColor.Gray);
    }

    /// <summary>
    ///  Writes a name value pair to the console
    /// </summary>
    /// <param name="name">The name</param>
    /// <param name="value">The value</param>
    /// <param name="nameColor">ConsoleColor used for name</param>
    /// <param name="valueColor">ConsoleColor used for value</param>
    /// <param name="separator">Separator of name and value</param>
    /// <param name="separatorColor">ConsoleColor used for separator</param>
    /// <example>
    /// <code lang="cs" title="ConsoleHelper WriteNameValue Example" numberLines="true" outlining="true" source=".\Doc\examples\SampleCoreCuiConsoleHelper.cs" region="Sample_Core_CUI_M_ConsoleHelper_WriteNameValue" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_CUI_M_ConsoleHelper_WriteNameValue.txt" />
    /// </example>
    public static void WriteNameValue(string name, object value, ConsoleColor nameColor, ConsoleColor valueColor, string separator, ConsoleColor separatorColor)
    {
        if (value.AsUniversal<string>().IsNotNull())
        {
            Write(name, nameColor);
            Write(" [{0}]".SafeFormatWith(value.GetType().GetTypeName()), nameColor);

            Write(string.IsNullOrEmpty(separator) ? " = " : separator, separatorColor);
            WriteLine(value.ToInvariantString(), valueColor);
        }
        else if (value.AsUniversal<IEnumerable>().IsNotNull())
        {
            WriteCollection(name, value.AsUniversal<IEnumerable>(), ", ", nameColor, valueColor, separatorColor);
        }
        else
        {
            Write(name, nameColor);
            Write(" [{0}]".SafeFormatWith(value.GetType().GetTypeName()), nameColor);

            Write(string.IsNullOrEmpty(separator) ? " = " : separator, separatorColor);
            WriteLine(value.ToInvariantString(), valueColor);
        }

    }

    #endregion

}
