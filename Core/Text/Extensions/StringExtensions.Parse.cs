//--------------------------------------------------------------------------
// File:    StringExtensions.Parse.cs
// Content:	Implementation of class StringExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.ComponentModel;
using System.Globalization;
using System.Reflection;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents modern extension methods for <see cref="String"/> type optimized for .NET 8+.
/// </summary>
public static partial class StringExtensions
{
    #region ParseInvariantString and private helper methods

    /// <summary>
    /// Parses a string using ISpanParsable<T> if available, otherwise falls back to TypeConverter.
    /// </summary>
    /// <typeparam name="T">The parsing result type.</typeparam>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsing result of Type T.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>"
    public static T? ParseInvariantString<T>(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Verwende Reflection-basierte Prüfung für beide Interfaces
        return (T?)ParseInvariantString(value, typeof(T));
    }

    /// <summary>
    /// Parses a string using ISpanParsable<T> if available, otherwise falls back to TypeConverter.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <param name="type">The target type.</param>
    /// <returns>The parsed object result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> or <paramref name="type"/> is null.</exception>"
    public static object? ParseInvariantString(this string value, Type type)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(type);

        if (type.IsEnum)
        {
            return Enum.Parse(type, value, ignoreCase: true);
        }

        // For Nullable value types
        if (type.IsNullableType())
        {
            // Nullable<T> types are handled separately
            return ParseNullable(value, type);
        }

        // Priorität 1: Prüfe auf ISpanParsable<T> (wo T == type)
        var spanParsableInterface = typeof(ISpanParsable<>).MakeGenericType(type);
        if (spanParsableInterface.IsAssignableFrom(type))
        {
            return ParseWithSpanParsableReflection(value, type);
        }

        // Priorität 2: Prüfe auf IParsable<T> (wo T == type)
        var parsableInterface = typeof(IParsable<>).MakeGenericType(type);
        if (parsableInterface.IsAssignableFrom(type))
        {
            return ParseWithParsableReflection(value, type);
        }

        // Fallback: TypeConverter
        return ParseInvariantStringFallback(value, type);
    }

    private static object? ParseNullable(string value, Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            return value.ParseInvariantString(underlyingType);
        }
        return null;
    }

    private static object? ParseWithSpanParsableReflection(string value, Type type)
    {
        try
        {
            // Für ISpanParsable<T> verwenden wir einen eleganten Workaround:
            // Erstelle einen generischen Methodenaufruf zur Laufzeit
            var method = typeof(StringExtensions)
                .GetMethod(nameof(InvokeSpanParsable), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(type);

            if (method != null)
            {
                return method.Invoke(null, [value]);
            }
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            // Falls ISpanParsable<T> fehlschlägt, versuche IParsable<T> als Fallback
            return ParseWithParsableReflection(value, type);
        }

        return null;
    }

    // Hilfsmehtode für den typsicheren Aufruf von ISpanParsable<T>
    private static T? InvokeSpanParsable<T>(string value) where T : ISpanParsable<T>
    {
        try
        {
            return T.Parse(value.AsSpan(), CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            return default(T);
        }
    }

    private static object? ParseWithParsableReflection(string value, Type type)
    {
        try
        {
            // Für IParsable<T> verwenden wir ebenfalls den generischen Methodenaufruf
            var method = typeof(StringExtensions)
                .GetMethod(nameof(InvokeParsable), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(type);

            if (method != null)
            {
                return method.Invoke(null, [value]);
            }
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            return null;
        }

        return null;
    }

    // Hilfsmehtode für den typsicheren Aufruf von IParsable<T>
    private static T? InvokeParsable<T>(string value) where T : IParsable<T>
    {
        try
        {
            return T.Parse(value, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            return default(T);
        }
    }


    private static object? ParseInvariantStringFallback(string value, Type type)
    {
        try
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter?.ConvertFromInvariantString(value);
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            return null;
        }
    }

    #endregion
}
