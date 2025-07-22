//--------------------------------------------------------------------------
// File:    ExceptionHelper.cs
// Content:	Implementation of class ExceptionHelper
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AnBo.Core;

/// <summary>
/// Provides common Exception helper &amp; extension methods.
/// </summary>
public static class ExceptionHelper
{

    #region Exception helper methods

    /// <summary>
    /// Determines whether the specified exception is a fatal exception.
    /// <list type="bullet">
    /// <listheader>Fatal Exceptions are:</listheader>
    /// <item><see cref="OutOfMemoryException"/></item>
    /// <item><see cref="InsufficientMemoryException"/></item>
    /// <item><see cref="ThreadAbortException"/></item>
    /// <item><see cref="AccessViolationException"/></item>
    /// <item><see cref="SEHException"/></item>
    /// <item><see cref="StackOverflowException"/></item>
    /// <item><see cref="AppDomainUnloadedException"/></item>
    /// <item><see cref="SystemException"/> with <see cref="SystemException.HResult"/> equal to -2146233036 (CriticalFinalizerObject)</item>
    /// </list>
    /// </summary>
    /// <param name="exception">The exception to check.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified exception is fatal; otherwise, <see langword="false"/>.
    /// </returns>
    /// <example>
    /// <code lang="cs" title="Exception extension method IsFatal Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_EXM_ExceptionHelper_IsFatal" />
    /// </example>
    public static bool IsFatal(this Exception? exception)
    {
        while (exception != null)
        {
            if (exception is OutOfMemoryException and not InsufficientMemoryException 
                or ThreadAbortException 
                or AccessViolationException 
                or SEHException 
                or StackOverflowException 
                or AppDomainUnloadedException
                or SystemException { HResult: -2146233036 }) // CriticalFinalizerObject
            {
                return true;
            }
            if (exception is not TypeInitializationException && exception is not TargetInvocationException)
            {
                break;
            }
            exception = exception.InnerException;
        }
        return false;
    }

    /// <summary>
    /// Builds the exception hash string by calling <see cref="RenderExceptionDetails"/> and building a MD5-Hash from the resulting string.
    /// </summary>
    /// <param name="exception">The exception to build the hash string from.</param>
    /// <returns>
    /// Returns the exception hash string.
    /// </returns>
    /// <example>
    /// <code lang="cs" title="Exception extension method GetExceptionHash Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_EXM_ExceptionHelper_GetExceptionHash" />
    /// </example>
    public static string GetExceptionHash(this Exception exception)
    {
        return GetExceptionHash(exception.RenderExceptionDetails());
    }

    /// <summary>
    /// Builds the exception string hash value by generating a MD5-Hash from the provides <paramref name="exceptionString"/>.
    /// </summary>
    /// <param name="exceptionString">The exception string to hash.</param>
    /// <returns>
    /// Returns the hash string for the provided exception string.
    /// </returns>
    internal static string GetExceptionHash(string exceptionString)
    {
        string s = FilterExceptionString(exceptionString);
        byte[] bytes = Encoding.Default.GetBytes(s);
        return Convert.ToBase64String(MD5.Create().ComputeHash(bytes));
    }


    /// <summary>
    /// Searches for an matching exception in the specified <paramref name="exception"/>. If <paramref name="exception"/> don't match, 
    /// the method will continue to search for a match in the inner exceptions.
    /// </summary>
    /// <param name="exception">The exception to search in.</param>
    /// <param name="exceptiontypeToSearchFor">The exception type to search for.</param>
    /// <param name="matchExact">if set to <see langword="true"/> the method searches for an exact match 
    /// of the specified <paramref name="exceptiontypeToSearchFor"/>, otherwise searches for an exception 
    /// type that can be assigned to <paramref name="exceptiontypeToSearchFor"/>.</param>
    /// <returns>The matching exception or <see langword="null"/> if no match was found.</returns>
    /// <example>
    /// <code lang="cs" title="Exception extension method FindMatchingException Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_EXM_ExceptionHelper_FindMatchingException" />
    /// </example>
    public static Exception? FindMatchingException(this Exception exception, Type exceptiontypeToSearchFor, bool matchExact)
    {
        if (exception == null)
            return null;

        if (exception.GetType() == typeof(CombinedException))
        {
            var combinedException = exception.AsUniversal<CombinedException>();
            foreach (var ex in combinedException!.InnerExceptions)
            {
                var filteredException = FindMatchingExceptionInternal(ex, exceptiontypeToSearchFor, matchExact);
                if (filteredException != null)
                    return ex;
            }
        }

        return FindMatchingExceptionInternal(exception, exceptiontypeToSearchFor, matchExact);
    }

    private static Exception? FindMatchingExceptionInternal(Exception exception, Type exceptiontypeToSearchFor, bool matchExact)
    {
        if (exception == null)
            return null;

        Exception? ret = exception;

        while (ret != null)
        {
            Type curType = ret.GetType();
            if (matchExact)
            {
                if (curType == exceptiontypeToSearchFor)
                    return ret;
            }
            else
            {
                if (exceptiontypeToSearchFor.IsAssignableFrom(curType))
                    return ret;
            }
            ret = ret.InnerException;
        }

        return null;
    }

    #endregion

    #region Exception formatting helper methods

    /// <summary>
    /// Builds the exception details message.
    /// </summary>
    /// <param name="exception">The exception to build the full details message from.</param>
    /// <returns>The full details exception message.</returns>
    /// <seealso cref="RenderException"/> <seealso cref="RenderExceptionSummary"/> <seealso cref="RenderExceptionMessage"/>
    /// <example>
    /// <code lang="cs" title="Exception extension method RenderExceptionDetails Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_EXM_ExceptionHelper_RenderExceptionDetails" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_Error_EXM_ExceptionHelper_RenderExceptionDetails.txt" />
    /// </example>
    public static string RenderExceptionDetails(this Exception exception)
    {
        return exception.RenderException().FullText;
    }
    // <summary>
    /// Builds the exception summary message.
    /// </summary>
    /// <param name="exception">The exception to build the summary from.</param>
    /// <returns>
    /// Returns the exception summary message (summary message is the result of <see cref="RenderExceptionMessage"/> limited to 255 characters).
    /// </returns>
    /// <seealso cref="RenderException"/> <seealso cref="RenderExceptionDetails"/> <seealso cref="RenderExceptionMessage"/>
    /// <example>
    /// <code lang="cs" title="Exception extension method RenderExceptionSummary Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_EXM_ExceptionHelper_RenderExceptionSummary" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_Error_EXM_ExceptionHelper_RenderExceptionSummary.txt" />
    /// </example>
    public static string RenderExceptionSummary(this Exception exception)
    {
        return NormalizeSummaryString(RenderExceptionMessage(exception));
    }

    /// <summary>
    /// Builds the exception message.
    /// </summary>
    /// <param name="exception">The exception to build the message from.</param>
    /// <returns>
    /// Message of the exception, into which all of the inner exceptions' messages are also included.
    /// </returns>
    /// <seealso cref="RenderException"/> <seealso cref="RenderExceptionSummary"/> <seealso cref="RenderExceptionDetails"/>
    /// <example>
    /// <code lang="cs" title="Exception extension method RenderExceptionMessage Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_EXM_ExceptionHelper_RenderExceptionMessage" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_Error_EXM_ExceptionHelper_RenderExceptionMessage.txt" />
    /// </example>
    public static string RenderExceptionMessage(this Exception exception)
    {
        string summary = GetMessageText(exception);
        MethodBase? failedMethod = GetFailedMethod(exception);
        if (failedMethod != null)
        {
            return $"at {failedMethod.DeclaringType?.Name ?? "<unknown type>"}.{failedMethod.Name} : {summary}";
        }

        return summary;
    }

    /// <summary>
    /// <para>Renders a string representation of the exception by collecting all of the data about the original exception and all of the inner/related exceptions in the tree.</para>
    /// <para>Explicitly outlines the relation between the exceptions, like which is whose inner and where they're coming from in the inner-exception tree.</para>
    /// <para>Note that <see cref="P:System.Exception.Message" /> plus <see cref="P:System.Exception.StackTrace" /> might miss the custom fields of the exception, and <see cref="M:System.Exception.ToString" /> ignores related exceptions that are not exactly inners (eg <see cref="P:System.Reflection.ReflectionTypeLoadException.LoaderExceptions" />).</para>
    /// </summary>
    /// <param name="exception">The exception to render.</param>
    /// <returns>
    /// <para>A string containing all of the meaningful messages of all the inner exceptions.</para>
    /// <para>A string with the above message plus stack traces and any other associated data.</para>
    /// </returns>
    /// <remarks>This method might be slow, but it caches the rendered data on the exception after the first call.</remarks>
    /// <seealso cref="ExceptionText"/> <seealso cref="RenderExceptionDetails"/>
    /// <example>
    /// <code lang="cs" title="Exception extension method RenderException Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_EXM_ExceptionHelper_RenderException" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_Error_EXM_ExceptionHelper_RenderException.txt" />
    /// </example>
    public static ExceptionText RenderException(this Exception exception)
    {
        string message;
        string fullText;
        string userFriendlyMessage = string.Empty;

        if (exception == null)
        {
            message = "NULL";
            fullText = "NULL";
            userFriendlyMessage = "NULL";
        }
        else
        {
            message = GetMessageText(exception);
            fullText = GetExceptionText(exception);
            if (exception is BaseException ex)
            {
                userFriendlyMessage = ex.UserFriendlyMessage ?? string.Empty;
            }
        }
        return new ExceptionText(message, fullText, userFriendlyMessage);
    }


    /// <summary>
    /// Gets the exception text in detail.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>String representation of the exception rendered by collecting all of the data about the original exception and all of the inner/related exceptions in the tree.</returns>
    /// <example>
    /// <code lang="cs" title="ExceptionHelper method GetExceptionText Example." outlining="true" source=".\Doc\examples\SampleCoreError.cs" region="Sample_Core_Error_M_ExceptionHelper_GetExceptionText" />
    /// <code title="Console Output:" source=".\Doc\examples\Sample_Core_Error_M_ExceptionHelper_GetExceptionText.txt" />
    /// </example>
    /// <seealso cref="RenderException"/> <seealso cref="RenderExceptionDetails"/>
    public static string GetExceptionText(Exception exception)
    {
        if (exception.IsNull())
            return string.Empty;

        StringBuilder sb = new StringBuilder();

        AppendUnderlined(sb, '*', "A {0} has been thrown: {1}", FormatExceptionTypeName(exception), exception.Message);

        if (exception is CombinedException combinedException)
        {
            sb.AppendLine("Combined exceptions:");
            sb.AppendLine("--------------------");
            foreach (Exception innerException in combinedException.InnerExceptions)
            {
                Exception? ex = innerException;
                int deepth = 0;
                while (ex != null)
                {
                    AppendLine(sb);
                    AppendExceptionText(sb, ex, deepth);
                    ex = ex.InnerException;
                    ++deepth;
                }
            }
        }
        else
        {
            Exception? ex = exception;
            int deepth = 0;
            while (ex != null)
            {
                AppendLine(sb);
                AppendExceptionText(sb, ex, deepth);
                ex = ex.InnerException;
                ++deepth;
            }
        }
        AppendLine(sb, "**************************************************");

        return sb.ToString();
    }

    #endregion

    #region Private Exception helper methods

    private static readonly Regex ExceptionRegex = new(@"([^\s]+\\)?([^\s\\]+)(:.+)?$", RegexOptions.Compiled | RegexOptions.Multiline);

    /// <summary>
    /// Filters the exception string.
    /// </summary>
    /// <param name="exceptionText">The exception text.</param>
    /// <returns></returns>
    private static string FilterExceptionString(string exceptionText)
    {
        if (exceptionText.IsNullOrEmptyWithTrim())
            return string.Empty;

        return ExceptionRegex.Replace(exceptionText, "$2").ToLower(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the exception message text.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>Message of the exception, into which all of the inner exceptions' messages are also included.</returns>
    private static string GetMessageText(Exception exception)
    {
        if (exception.IsNull())
            return string.Empty;

        StringBuilder sb = new StringBuilder();

        AppendLine(sb, $"{exception.Message}");

        if (exception is CombinedException combinedException)
        {
            foreach (Exception innerException in combinedException.InnerExceptions)
            {
                Exception? ex = innerException;
                //int deepth = 0;
                while (ex != null)
                {
                    AppendLine(sb, $" | {ex.Message}");
                    ex = ex.InnerException;
                    //++deepth;
                }
            }
        }
        else
        {
            Exception? ex = exception.InnerException;
            //int deepth = 0;
            while (ex != null)
            {
                AppendLine(sb, $" | {ex.Message}");
                ex = ex.InnerException;
                //++deepth;
            }
        }

        return sb.ToString();
    }

    private static MethodBase? GetFailedMethod(Exception exception)
    {
        Exception? innerException = exception;
        MethodBase? base2 = null;
        while (innerException != null)
        {
            StackFrame[] frames = new StackTrace(exception).GetFrames();
            if ((frames == null) || (frames.Length == 0))
            {
                return null;
            }
            foreach (StackFrame frame in frames)
            {
                MethodBase? method = frame.GetMethod();
                if (base2 == null)
                {
                    base2 = method;
                }
                if (method != null)
                {
                    return method;
                }
            }
            innerException = innerException.InnerException;
        }
        return base2;
    }

    private static string GetSummary(Exception exception)
    {
        return exception.RenderException().Message;
    }

    private static string NormalizeSummaryString(string summaryString)
    {
        return summaryString.Clip(255);
    }

    private static string FormatExceptionTypeName(Exception exception)
    {
        if (exception == null)
            return string.Empty;


        string? exceptionTypeFullName = exception.GetType().FullName;

        if (exceptionTypeFullName == null)
            return string.Empty;

        exceptionTypeFullName = exceptionTypeFullName.Replace("`1[[", "{");
        exceptionTypeFullName = exceptionTypeFullName.Replace("]]", "}");

        int genericTypeStartIndex = exceptionTypeFullName.IndexOf("{");

        if (genericTypeStartIndex < 1)
            return exceptionTypeFullName;

        int fullQualifiedSeperatorIndex = exceptionTypeFullName.IndexOf(",", genericTypeStartIndex);
        int genericTypeEndIndex = exceptionTypeFullName.IndexOf("}", genericTypeStartIndex);

        if (fullQualifiedSeperatorIndex > genericTypeStartIndex)
        {
            exceptionTypeFullName = exceptionTypeFullName.Remove(fullQualifiedSeperatorIndex, genericTypeEndIndex - fullQualifiedSeperatorIndex);
        }

        return exceptionTypeFullName;
    }

    private static void AppendExceptionText(StringBuilder sb, Exception exception, int deepth)
    {
        AppendUnderlined(sb, '=', "Exception #{0}", deepth.ToString());
        AppendLine(sb, $"Exception Type: {FormatExceptionTypeName(exception)}");

        if (exception.TargetSite != null)
        {
            AppendLine(sb, "TargetSite:");
            AppendLine(sb, $"\tAssembly: {exception.TargetSite.DeclaringType?.Module.Name ?? string.Empty}");
            AppendLine(sb, $"\tClass:    {exception.TargetSite.DeclaringType?.Name ?? string.Empty}");
            AppendLine(sb, $"\tMethod:   {exception.TargetSite?.ToString() ?? string.Empty}");
        }

        AppendProperties(sb, exception);
        AppendDictionary(sb, exception.Data);

        if (exception.StackTrace != null)
        {
            AppendLine(sb, "StackTrace Information:");
            AppendLine(sb, exception.StackTrace);
        }
    }

    private static void AppendProperties(StringBuilder sb, Exception exception)
    {
        try
        {
            AppendLine(sb, "Properties:");

            PropertyInfo[] api = exception.GetType().GetProperties();
            object? o;
            foreach (PropertyInfo pi in api)
            {
                try
                {
                    if ((pi.Name == "InnerException") || (pi.Name == "StackTrace") || (pi.Name == "Data"))
                        continue;
                    o = pi.GetValue(exception, null);
                    if (o == null)
                        AppendLine(sb, $"    {pi.Name}: NULL");
                    else
                        AppendLine(sb, $"    {pi.Name}: {o?.ToString() ?? string.Empty}");
                }
                catch (Exception ex)
                {
                    if (IsFatal(ex))
                        throw;

                    AppendLine(sb, $"    {pi.Name}: {ex.GetType().FullName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            if (IsFatal(ex))
                throw;
        }
    }

    private static void AppendDictionary(StringBuilder sb, IDictionary dict)
    {
        if (dict.Count == 0)
            return;
        try
        {
            object[] keyObjects = new object[dict.Count];
            string[] keys = new string[dict.Count];
            dict.Keys.CopyTo(keyObjects, 0);
            for (int i = 0; i < keyObjects.Length; ++i)
                // Replace the line with a null check to ensure no null reference assignment
                keys[i] = keyObjects[i]?.ToString() ?? string.Empty;
            Array.Sort(keys, keyObjects);

            AppendLine(sb, "Data: ");
            for (int i = 0; i < keyObjects.Length; ++i)
            {
                string key = keys[i];

                try
                {
                    object keyObject = keyObjects[i];
                    string value = StringHelper.SafeToString(dict[keyObject], "NULL");
                    AppendLine(sb, $"    {key}: {value}");
                }
                catch (Exception ex)
                {
                    if (IsFatal(ex))
                        throw;
                    AppendLine(sb, $"    {key}: {ex.GetType().FullName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            if (IsFatal(ex))
                throw;
        }
    }

    private static void AppendLine(StringBuilder sb)
    {
        sb.Append(Environment.NewLine);
    }

    private static void AppendLine(StringBuilder sb, string format, params string[] args)
    {
        if (args == null || args.Length == 0)
            sb.Append(format);
        else
            sb.AppendFormat(format, args);
        sb.Append(Environment.NewLine);
    }

    private static void AppendUnderlined(StringBuilder sb, char underline, string format, params string[] args)
    {
        int l = sb.Length;
        sb.AppendFormat(format, args);
        l = sb.Length - l;
        sb.Append(Environment.NewLine);
        sb.Append(new string(underline, l));
        sb.Append(Environment.NewLine);
    }

    #endregion

}
