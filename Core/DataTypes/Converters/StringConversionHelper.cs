//--------------------------------------------------------------------------
// File:    StringConversionHelper.cs
// Content:	Implementation of a string conversion helper class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Reflection;

#endregion

namespace AnBo.Core;

/// <summary>
/// Common conversion tasks such as parsing string values into various types.
/// </summary>
public static class StringConversionHelper
{
    #region IsTypeSpan- and IsTypeString methods

    /// <summary>
    /// Determines if the specified span can be parsed as the specified type T.
    /// This version is more efficient for substring operations.
    /// </summary>
    /// <typeparam name="T">The type that implements ISpanParsable&lt;T&gt;</typeparam>
    /// <param name="span">The span to check.</param>
    /// <param name="provider">The format provider to use. If null, uses InvariantCulture.</param>
    /// <returns>
    /// 	<see langword="true"/> if the span can be parsed as type T; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsTypeSpan<T>(this ReadOnlySpan<char> span, IFormatProvider? provider = null)
        where T : ISpanParsable<T>
    {
        provider ??= CultureInfo.InvariantCulture;
        return T.TryParse(span, provider, out _);
    }

    /// <summary>
    /// Determines if the specified string can be parsed as the specified type T.
    /// </summary>
    /// <typeparam name="T">The type that implements ISpanParsable&lt;T&gt;</typeparam>
    /// <param name="str">The string to check.</param>
    /// <param name="provider">The format provider to use. If null, uses InvariantCulture.</param>
    /// <returns>
    /// 	<see langword="true"/> if the string can be parsed as type T; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsTypeString<T>(this string? str, IFormatProvider? provider = null)
        where T : ISpanParsable<T>
    {
        if (string.IsNullOrEmpty(str))
            return false;

        return str.AsSpan().IsTypeSpan<T>(provider);
    }

    #endregion

    #region ToInvariantString and private helper methods

    /// <summary>
    /// Converts the specified value to its string representation using invariant culture formatting.
    /// This method provides optimized conversion paths for common types and supports customizable formatting options.
    /// </summary>
    /// <typeparam name="T">The type of the value to convert.</typeparam>
    /// <param name="val">The value to convert to a string representation. Can be null.</param>
    /// <param name="options">
    /// Optional formatting options that control the conversion behavior. If null, <see cref="ToStringOptions.Default"/> is used.
    /// These options specify formatting for dates, numbers, collections, and null values.
    /// </param>
    /// <param name="currentDepth">
    /// The current recursion depth for nested object conversion. Used internally to prevent infinite recursion
    /// and stack overflow in circular object references. When this depth reaches <see cref="ToStringOptions.MaxNestingDepth"/>,
    /// further nested objects will be represented with a placeholder string.
    /// </param>
    /// <returns>
    /// A string representation of the specified value. Returns the configured null string (default "&lt;null&gt;") 
    /// if the value is null. For collections, returns a formatted representation with brackets and optional item counts.
    /// All conversions use invariant culture to ensure consistent formatting regardless of system locale.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method uses optimized conversion paths for performance:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Primitive numeric types (byte, short, int, long, etc.) use direct ToString with invariant culture</description></item>
    /// <item><description>Floating-point types (double, float, decimal) support custom formatting via options</description></item>
    /// <item><description>Date/time types (DateTime, DateOnly, TimeOnly, TimeSpan) use configurable format strings</description></item>
    /// <item><description>Collections (arrays, IEnumerable, IDictionary) are formatted with brackets and separators</description></item>
    /// <item><description>Types implementing ISpanFormattable use stack-allocated buffers for efficient formatting</description></item>
    /// <item><description>Nullable types are unwrapped and converted recursively</description></item>
    /// <item><description>Enum types use their string representation</description></item>
    /// <item><description>Other types fall back to TypeConverter or ToString()</description></item>
    /// </list>
    /// <para>
    /// Collection formatting includes optional item count display and truncation when exceeding the maximum item limit.
    /// Multidimensional arrays are supported with recursive formatting.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic usage
    /// int number = 42;
    /// string result = number.ToInvariantString(); // "42"
    /// 
    /// // With custom options
    /// var options = new ToStringOptions { DateTimeFormat = "yyyy-MM-dd" };
    /// DateTime date = DateTime.Now;
    /// string formatted = date.ToInvariantString(options); // "2025-07-17"
    /// 
    /// // Collection formatting
    /// int[] array = { 1, 2, 3 };
    /// string arrayStr = array.ToInvariantString(); // "[1, 2, 3] (3 items)"
    /// </code>
    /// </example>
    /// <seealso cref="ToStringOptions"/>
    /// <seealso cref="CultureInfo.InvariantCulture"/>
    /// <seealso cref="ISpanFormattable"/>
    /// <seealso cref="TypeDescriptor.GetConverter(Type)"/>
    public static string ToInvariantString<T>(this T? val, ToStringOptions? options = null, int currentDepth = 0)
    {
        options ??= ToStringOptions.Default;

        // MaxNestingDepth-Schutz auf oberster Ebene
        if (currentDepth >= options.MaxNestingDepth)
            return "<max nesting depth reached>";

        if (val is null)
            return options.NullString;

        if (val is string s)
            return s;

        // Check if the value implements ISpanFormattable for optimized formatting (for .NET 6+)
        if (val is ISpanFormattable spanFormattable)
        {
            return FormatUsingSpan(spanFormattable, options);
        }


        // For all other types, use a switch expression for concise handling
        return val switch

        {
            bool b => b.ToString(CultureInfo.InvariantCulture),

            // Collections specific handling
            Array array => FormatArray(array, options, currentDepth),

            IDictionary dict => FormatDictionary(dict, options, currentDepth),

            //IEnumerable enumerable when ShouldFormatAsCollection(enumerable)
            IEnumerable enumerable => FormatEnumerable(enumerable, options, currentDepth),

            //_ => TryConvertWithTypeConverter(val) ?? Convert.ToString(val, CultureInfo.InvariantCulture) ?? string.Empty
            _ => TryConvertWithTypeConverter(val) ?? string.Empty
        };
    }

    // Place TypeConverter logic in a pivate method
    private static string? TryConvertWithTypeConverter<T>(T val)
    {
        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertTo(typeof(string)))
            {
                return converter.ConvertToInvariantString(val);
            }
        }
        catch (Exception ex) when (!ex.IsFatal())
        {
            // Exception wird geschluckt, Fallback wird verwendet
        }

        return null;
    }

    private static string FormatEnumerable(IEnumerable enumerable, ToStringOptions options, int currentDepth = 0)
    {
        var items = new List<string>();
        int count = 0;

        foreach (var item in enumerable)
        {
            if (count >= options.MaxCollectionItems)
            {
                items.Add("...");
                break;
            }
            items.Add(item.ToInvariantString(options, currentDepth + 1));
            count++;
        }

        var result = $"[{string.Join(options.CollectionSeparator, items)}]";

        if (options.ShowCollectionCount && enumerable is ICollection collection)
        {
            result += $" ({collection.Count} items)";
        }

        return result;
    }

    //private static bool ShouldFormatAsCollection(IEnumerable enumerable)
    //{
    //    // Strings sind auch IEnumerable, aber wir wollen sie nicht als Collection behandeln
    //    return enumerable is not string;
    //}

    private static string FormatArray(Array array, ToStringOptions options, int currentDepth = 0)
    {
        if (array.Length == 0) return "[]";

        // Mehrdimensionale Arrays mit Depth
        if (array.Rank > 1)
        {
            return FormatMultidimensionalArray(array, options, currentDepth);
        }

        // Eindimensionale Arrays
        var items = new List<string>();
        int processedCount = 0;

        for (int i = 0; i < array.Length; i++)
        {
            if (processedCount >= options.MaxCollectionItems)
            {
                items.Add("...");
                break;
            }

            var value = array.GetValue(i);
            items.Add(value.ToInvariantString(options, currentDepth + 1));
            processedCount++;
        }

        var result = $"[{string.Join(options.CollectionSeparator, items)}]";

        if (options.ShowCollectionCount)
        {
            result += $" ({array.Length} items)";
        }

        return result;
    }

    private static string FormatDictionary(IDictionary dict, ToStringOptions options, int currentDepth = 0)
    {
        if (dict.Count == 0) return "{}";

        var pairs = new List<string>();
        int processedCount = 0;

        foreach (DictionaryEntry entry in dict)
        {
            if (processedCount >= options.MaxCollectionItems)
            {
                pairs.Add("...");
                break;
            }

            var key = entry.Key.ToInvariantString(options, currentDepth + 1);
            var value = entry.Value.ToInvariantString(options, currentDepth + 1);
            pairs.Add($"{key}{options.DictionaryKeyValueSeparator}{value}");
            processedCount++;
        }

        var result = $"{{{string.Join(options.CollectionSeparator, pairs)}}}";

        if (options.ShowCollectionCount)
        {
            result += $" ({dict.Count} items)";
        }

        return result;
    }

    // Bonus: Mehrdimensionale Arrays
    private static string FormatMultidimensionalArray(Array array, ToStringOptions options, int currentDepth = 0)
    {
        var dimensions = new int[array.Rank];
        for (int i = 0; i < array.Rank; i++)
        {
            dimensions[i] = array.GetLength(i);
        }

        var result = FormatMultidimensionalArrayRecursive(array, dimensions, new int[array.Rank], 0, options, currentDepth);

        // ShowArrayDimensions: Erweiterte Dimensionsinfo
        if (options.ShowArrayDimensions)
        {
            var dimensionInfo = string.Join("×", dimensions);
            var totalElements = dimensions.Aggregate(1, (acc, dim) => acc * dim);
            var rankInfo = array.Rank > 1 ? $"{array.Rank}D " : "";
            result += $" ({rankInfo}{dimensionInfo}, {totalElements} items)";
        }

        return result;
    }

    private static string FormatMultidimensionalArrayRecursive(Array array, int[] dimensions, int[] indices, int currentDimension, ToStringOptions options, int currentDepth)
    {
        // MaxNestingDepth: Schutz vor zu tiefer Verschachtelung
        if (currentDepth >= options.MaxNestingDepth)
        {
            return "<max nesting depth reached>";
        }

        if (currentDimension == dimensions.Length - 1)
        {
            // Letzte Dimension - sammle die Werte
            var items = new List<string>();
            int processedCount = 0;

            for (int i = 0; i < dimensions[currentDimension]; i++)
            {
                if (processedCount >= options.MaxCollectionItems)
                {
                    items.Add("...");
                    break;
                }

                indices[currentDimension] = i;
                var value = array.GetValue(indices);

                // Rekursive Konvertierung mit erhöhter Tiefe
                string valueString;
                if (value is Array nestedArray)
                {
                    // Verschachtelte Arrays (z.B. jagged arrays)
                    valueString = nestedArray.ToInvariantString(options, currentDepth + 1);
                }
                else
                {
                    valueString = value.ToInvariantString(options);
                }

                items.Add(valueString);
                processedCount++;
            }


            return $"[{string.Join(options.CollectionSeparator, items)}]";
        }
        else
        {
            // Rekursive Dimension
            var subArrays = new List<string>();
            int processedCount = 0;


            for (int i = 0; i < dimensions[currentDimension]; i++)
            {
                if (processedCount >= options.MaxCollectionItems)
                {
                    subArrays.Add("...");
                    break;
                }

                indices[currentDimension] = i;
                var subArray = FormatMultidimensionalArrayRecursive(
                    array,
                    dimensions,
                    indices,
                    currentDimension + 1,
                    options,
                    currentDepth + 1  // Tiefe erhöhen
                );
                subArrays.Add(subArray);
                processedCount++;
            }

            return $"[{string.Join(options.CollectionSeparator, subArrays)}]";
        }
    }

    // Span-Formatierung
    private static string FormatUsingSpan(ISpanFormattable spanFormattable, ToStringOptions options)
    {
        // Format-String basierend auf Typ bestimmen
        var format = GetFormatString(spanFormattable, options);

        int bufferSize = GetOptimalBufferSize(spanFormattable);

        if (bufferSize <= 256)
        {
            Span<char> buffer = stackalloc char[bufferSize];
            if (spanFormattable.TryFormat(buffer, out int charsWritten, format, CultureInfo.InvariantCulture))
            {
                return buffer[..charsWritten].ToString();
            }
        }
        else
        {
            var buffer = new char[bufferSize];
            if (spanFormattable.TryFormat(buffer, out int charsWritten, format, CultureInfo.InvariantCulture))
            {
                return new string(buffer, 0, charsWritten);
            }
        }

        return spanFormattable.ToString() ?? string.Empty;
    }

    private static ReadOnlySpan<char> GetFormatString(ISpanFormattable spanFormattable, ToStringOptions options)
    {
        return spanFormattable switch
        {
            DateTime => options.DateTimeFormat.AsSpan(),
            DateTimeOffset => options.DateTimeOffsetFormat.AsSpan(),
            DateOnly => options.DateOnlyFormat.AsSpan(),
            TimeOnly => options.TimeOnlyFormat.AsSpan(),
            TimeSpan => options.TimeSpanFormat.AsSpan(),
            decimal when !string.IsNullOrEmpty(options.DecimalFormat) => options.DecimalFormat.AsSpan(),
            double when !string.IsNullOrEmpty(options.DoubleFormat) => options.DoubleFormat.AsSpan(),
            float when !string.IsNullOrEmpty(options.FloatFormat) => options.FloatFormat.AsSpan(),
            _ => ReadOnlySpan<char>.Empty
        };
    }

    // Intelligente Puffergrößen-Bestimmung
    private static int GetOptimalBufferSize(ISpanFormattable spanFormattable)
    {
        // BigInteger kann sehr groß sein, daher kein fester Puffer
        if (spanFormattable is BigInteger bi)
        {
            if (bi.IsZero) return 1;
            // Logarithmische Schätzung: log10(2^bits) ≈ bits * 0.301
            int bits = (int)Math.Ceiling(BigInteger.Log(BigInteger.Abs(bi), 2));
            int digits = (int)Math.Ceiling(bits * 0.3010299957); // log10(2)

            int estimatedSize = digits + (bi.Sign < 0 ? 1 : 0); // +1 für Minuszeichen

            estimatedSize = Math.Max(estimatedSize * 2, 64); // Mindestens 64, doppelte Schätzung
            return estimatedSize;
        }

        // Für andere Typen eine feste Puffergröße basierend auf den erwarteten Maximalwerten
        return spanFormattable switch
        {
            byte or sbyte => 4,           // "-128"
            short or ushort => 6,         // "-32768"
            int or uint => 12,            // "-2147483648"
            long or ulong => 21,          // "-9223372036854775808"
            Half => 20,                   // "6.103515625E-05", "Infinity"
            float => 25,                  // Wissenschaftliche Notation
            double => 35,                 // Wissenschaftliche Notation
            decimal => 32,                // Maximale Dezimalstellen
            DateTime => 20,               // "yyyy-MM-dd HH:mm:ss"
            DateTimeOffset => 35,         // "yyyy-MM-dd HH:mm:ss.fffffff zzz" Mit Zeitzone
            TimeSpan => 20,               // "[-]d.hh:mm:ss.fffffff"
            Guid => 36,                   // "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            Int128 => 50,                 // Sehr große Zahlen
            UInt128 => 50,                // Sehr große Zahlen
            _ => 256                      // Sicherheitspuffer
        };
    }

    #endregion

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
            var method = typeof(StringConversionHelper)
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
            var method = typeof(StringConversionHelper)
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
