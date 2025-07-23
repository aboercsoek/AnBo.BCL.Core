//--------------------------------------------------------------------------
// File:    DateTimeHelper.cs
// Content:	Implementation of class DateTimeUtlities
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;

#endregion

namespace AnBo.Core;

/// <summary>
/// Methods to help in date and time manipulation.
/// </summary>
public static class DateTimeHelper
{
    #region Constants and Static Fields

    /// <summary>
    /// Common DateTime format patterns supported by the parser
    /// </summary>
    private static readonly string[] CommonDateTimeFormats = [
        "yyyyMMdd'T'HHmmssfff",
        "yyyy-MM-dd'T'HH:mm:ss.fff",
        "yyyy-MM-dd'T'HHmmssfff",
        "yyyyMMddHHmmssfff",
        "yyyyMMdd'T'HHmmss",
        "yyyy-MM-dd'T'HH:mm:ss",
        "yyyy-MM-dd'T'HHmmss",
        "yyyyMMddHHmmss",
        "dd.MM.yyyy HH:mm:ss",
        "MM/dd/yyyy HH:mm:ss"
    ];

    /// <summary>
    /// Common Date format patterns supported by the parser
    /// </summary>
    private static readonly string[] CommonDateFormats = [
        "yyyy-MM-dd",
        "dd.MM.yyyy",
        "MM/dd/yyyy",
        "dd.MM.yy",
        "ddMMMyy",
        "ddMMMyyyy",
        "dd MMM yy",
        "dd MMM yyyy"
    ];

    /// <summary>
    /// Unix epoch start time (January 1, 1970, 00:00:00 UTC)
    /// </summary>
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    #endregion

    #region Enumerations

    /// <summary>
    /// Enumeration for day part specification
    /// </summary>
    public enum DayPart
    {
        /// <summary>Beginning of day = 00:00:00</summary>
        BeginOfDay,
        /// <summary>High Noon = 12:00:00</summary>
        HighNoon,
        /// <summary>End of day = 23:59:59.999</summary>
        EndOfDay
    }

    /// <summary>
    /// TimeSpan component assumption for parsing strings without delimiters
    /// </summary>
    public enum TimeSpanComponent
    {
        /// <summary>No assumption - parse as-is</summary>
        None,
        /// <summary>Assume the value represents days</summary>
        Days,
        /// <summary>Assume the value represents hours</summary>
        Hours,
        /// <summary>Assume the value represents minutes</summary>
        Minutes,
        /// <summary>Assume the value represents seconds</summary>
        Seconds
    }

    #endregion

    #region DateTime Parsing and Creation

    /// <summary>
    /// Attempts to parse a DateTime string into a DateTime object using various common formats.
    /// </summary>
    /// <param name="value">The DateTime string to parse</param>
    /// <returns>The parsed DateTime object</returns>
    /// <exception cref="ArgumentException">Thrown when the value cannot be parsed</exception>
    /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
    public static DateTime Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (TryCreate(value, out DateTime result))
        {
            return result;
        }

        throw new ArgumentException($"Unable to parse DateTime from value: '{value}'", nameof(value));
    }

    /// <summary>
    /// Attempts to parse a DateTime string into a DateTime object using various common formats.
    /// </summary>
    /// <param name="value">The DateTime string to parse</param>
    /// <param name="result">The parsed DateTime object if successful</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryCreate(string? value, out DateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        ReadOnlySpan<char> span = value.AsSpan().Trim();

        // Try DateTime formats first (longer strings)
        if (span.Length >= 14)
        {
            foreach (string format in CommonDateTimeFormats)
            {
                if (DateTime.TryParseExact(span, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                {
                    return true;
                }

                // TO-DO: Consider using InvariantCulture for international formats
            }
        }
        else
        {
            // Try Date formats
            foreach (string format in CommonDateFormats)
            {
                if (DateTime.TryParseExact(span, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                {
                    result = result.Date;
                    return true;
                }

                // Also try with InvariantCulture for international formats
                if (DateTime.TryParseExact(span, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    result = result.Date;
                    return true;
                }
            }
        }

        // Fall back to standard parsing
        return DateTime.TryParse(value, out result);
    }

    /// <summary>
    /// Creates a DateTime from a date string and sets the time to a specific part of the day.
    /// </summary>
    /// <param name="dateString">The date string to parse</param>
    /// <param name="dayPart">The part of the day to set</param>
    /// <returns>DateTime with the specified day part</returns>
    /// <exception cref="ArgumentException">Thrown when dateString cannot be parsed</exception>
    public static DateTime CreateWithDayPart(string dateString, DayPart dayPart)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dateString);

        if (!TryCreate(dateString, out DateTime date))
        {
            throw new ArgumentException($"Unable to parse date from value: '{dateString}'", nameof(dateString));
        }

        return SetDayPart(date.Date, dayPart);
    }

    /// <summary>
    /// Attempts to create a DateTime from a date string and sets the time to a specific part of the day.
    /// </summary>
    /// <param name="dateString">The date string to parse</param>
    /// <param name="dayPart">The part of the day to set</param>
    /// <param name="result">The resulting DateTime if successful</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool TryCreateWithDayPart(string? dateString, DayPart dayPart, out DateTime result)
    {
        result = default;

        if (!TryCreate(dateString, out DateTime date))
        {
            return false;
        }

        result = SetDayPart(date.Date, dayPart);
        return true;
    }

    /// <summary>
    /// Sets the time component of a DateTime to represent a specific part of the day.
    /// </summary>
    /// <param name="date">The date to modify</param>
    /// <param name="dayPart">The part of the day to set</param>
    /// <returns>DateTime with the specified time component</returns>
    private static DateTime SetDayPart(DateTime date, DayPart dayPart) => dayPart switch
    {
        DayPart.BeginOfDay => date.Date,
        DayPart.HighNoon => date.Date.AddHours(12),
        DayPart.EndOfDay => date.Date.AddDays(1).AddTicks(-1), // 23:59:59.9999999
        _ => date.Date
    };

    #endregion

    #region DateTime Formatting

    /// <summary>
    /// Converts a DateTime object into a ISO 8601 conform date time string.
    /// </summary>
    /// <param name="value">DateTime object to convert.</param>
    /// <returns>ISO 8601 DateTime string</returns>
    public static string ToIso8601DateTime(DateTime value) =>
       XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);

    /// <summary>
    /// Converts a DateTime value into a <see cref="DateTime"/> value with a precision of a <see cref="SqlDateTime">SQL Server DateTime type</see>.
    /// </summary>
    /// <param name="value"><see cref="DateTime"/> object that should be converted</param>
    /// <returns>DateTime object with <see cref="SqlDateTime">SQL Server DateTime</see> precision</returns>
    public static DateTime ToSqlServerPrecision(DateTime value) =>
        new SqlDateTime(value).Value;

    /// <summary>
    /// Converts a DateTime object to a file-sortable string format.
    /// </summary>
    /// <param name="value">The DateTime to convert</param>
    /// <param name="includeMilliseconds">Whether to include milliseconds in the output</param>
    /// <returns>File-sortable DateTime string (yyyyMMddTHHmmss or yyyyMMddTHHmmssfff)</returns>
    public static string ToFileSortableString(DateTime value, bool includeMilliseconds = false)
    {
        string format = includeMilliseconds ? "yyyyMMdd'T'HHmmssfff" : "yyyyMMdd'T'HHmmss";
        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses a file-sortable DateTime string back to a DateTime object.
    /// </summary>
    /// <param name="value">The file-sortable DateTime string</param>
    /// <returns>The parsed DateTime object</returns>
    /// <exception cref="ArgumentException">Thrown when the string format is invalid</exception>
    public static DateTime FromFileSortableString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (TryFromFileSortableString(value, out DateTime result))
        {
            return result;
        }

        throw new ArgumentException($"Invalid file-sortable DateTime format: '{value}'", nameof(value));
    }

    /// <summary>
    /// Attempts to parse a file-sortable DateTime string back to a DateTime object.
    /// </summary>
    /// <param name="value">The file-sortable DateTime string</param>
    /// <param name="result">The parsed DateTime object if successful</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryFromFileSortableString(string? value, out DateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        ReadOnlySpan<char> span = value.AsSpan().Trim();

        return span.Length switch
        {
            15 => DateTime.TryParseExact(span, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result),
            18 => DateTime.TryParseExact(span, "yyyyMMdd'T'HHmmssfff", CultureInfo.InvariantCulture, DateTimeStyles.None, out result),
            _ => false
        };
    }

    #endregion

    #region DOS DateTime Conversion

    /// <summary>
    /// Converts DOS date and time values to a DateTime object.
    /// </summary>
    /// <param name="dosDate">DOS date value</param>
    /// <param name="dosTime">DOS time value</param>
    /// <returns>The converted DateTime object</returns>
    public static DateTime FromDosDateTime(ushort dosDate, ushort dosTime)
    {
        /*
         DOS date format:
            Bits	Content	                Value Range
            0-4	    Day of the Month	    1-31
            5-8	    Month	                1-12 (1 = January, 2 = February, ...)
            9-15	Year (Offset from 1980)	0-127 (is 1980-2107)

            15 14 13 12 11 10 9   8  7  6  5    4  3  2  1  0
            Y  Y  Y  Y  Y  Y  Y   M  M  M  M    D  D  D  D  D
            \-----------------/   \-------/    \-----------/
                    Year            Month           Day

        DOS time format:
            Bits    Content Value Range Note
            0-4     Seconds 0-29        Value is multiplied by 2
            5-10    Minutes 0-59
            11-15   Hours   0-23

            15 14 13 12 11   10  9  8  7  6  5   4  3  2  1  0
             H  H  H  H  H   M   M  M  M  M  M   S  S  S  S  S
            \-------------/  \---------------/   \-----------/
                 Hours            Minutes          Seconds/2
        */
        try
        {
            int year = (dosDate >> 9) + 1980;
            int month = (dosDate >> 5) & 0xF;
            int day = dosDate & 0x1F;
            int hour = dosTime >> 11;
            int minute = (dosTime >> 5) & 0x3F;
            int second = (dosTime & 0x1F) * 2;

            // Handle invalid values
            if (dosDate == 0xFFFF || month == 0 || day == 0)
            {
                year = 1980;
                month = 1;
                day = 1;
            }

            if (dosTime == 0xFFFF)
            {
                hour = minute = second = 0;
            }

            return new DateTime(year, month, day, hour, minute, second);
        }
        catch
        {
            return new DateTime(1980, 1, 1);
        }
    }

    /// <summary>
    /// Converts a DOS timestamp to a DateTime object.
    /// </summary>
    /// <param name="dosTimestamp">The DOS timestamp</param>
    /// <returns>The converted DateTime object</returns>
    public static DateTime FromDosDateTime(uint dosTimestamp) =>
        FromDosDateTime((ushort)(dosTimestamp >> 16), (ushort)(dosTimestamp & 0xFFFF));

    #endregion

    #region Unix Timestamp Conversion

    /// <summary>
    /// Converts a DateTime to a Unix timestamp (seconds since Unix epoch).
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns>Unix timestamp as a long</returns>
    public static long ToUnixTimestamp(DateTime dateTime) =>
        ((DateTimeOffset)dateTime.ToUniversalTime()).ToUnixTimeSeconds();

    /// <summary>
    /// Converts a Unix timestamp to a DateTime.
    /// </summary>
    /// <param name="unixTimestamp">The Unix timestamp</param>
    /// <returns>The corresponding DateTime in local time</returns>
    public static DateTime FromUnixTimestamp(long unixTimestamp) =>
        DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).ToLocalTime().DateTime;

    /// <summary>
    /// Converts a DateTime to a Unix timestamp in milliseconds.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert</param>
    /// <returns>Unix timestamp in milliseconds as a long</returns>
    public static long ToUnixTimestampMilliseconds(DateTime dateTime) =>
        ((DateTimeOffset)dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();

    /// <summary>
    /// Converts a Unix timestamp in milliseconds to a DateTime.
    /// </summary>
    /// <param name="unixTimestampMs">The Unix timestamp in milliseconds</param>
    /// <returns>The corresponding DateTime in local time</returns>
    public static DateTime FromUnixTimestampMilliseconds(long unixTimestampMs) =>
        DateTimeOffset.FromUnixTimeMilliseconds(unixTimestampMs).ToLocalTime().DateTime;

    #endregion

    #region TimeSpan Helper Methods

    /// <summary>
    /// Returns the absolute value of a TimeSpan.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan</param>
    /// <returns>The absolute TimeSpan value</returns>
    public static TimeSpan Abs(TimeSpan timeSpan) => timeSpan.Duration();

    /// <summary>
    /// Determines if a TimeSpan is negative.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to check</param>
    /// <returns>True if the TimeSpan is negative, false otherwise</returns>
    public static bool IsNegative(TimeSpan timeSpan) => timeSpan < TimeSpan.Zero;

    /// <summary>
    /// Converts the time span to double.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns></returns>
    public static double ConvertTimeSpanToDouble(string timeSpan)
    {
        return ConvertTimeSpanToDouble(ParseTimeSpan(timeSpan));
    }

    /// <summary>
    /// Converts the time span to integer.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns></returns>
    public static double ConvertTimeSpanToDouble(TimeSpan timeSpan)
    {
        return timeSpan.TotalMilliseconds;
    }

    /// <summary>
    /// Attempts to parse a TimeSpan string with optional component assumption.
    /// </summary>
    /// <param name="value">The string to parse</param>
    /// <param name="component">The component assumption for strings without delimiters</param>
    /// <param name="result">The parsed TimeSpan if successful</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParseTimeSpan(string? value, TimeSpanComponent component, out TimeSpan result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }
        
        // Handle positive sign prefix
        if (value.Length > 1 && value[0] == '+')
        {
            value = value[1..];
        }

        switch (component)
        {
            case TimeSpanComponent.Seconds:
                result = TimeSpan.FromSeconds(value.ParseInvariantString<int>());
                return true;
            case TimeSpanComponent.Minutes:
                result = TimeSpan.FromMinutes(value.ParseInvariantString<int>());
                return true;
            case TimeSpanComponent.Hours:
                result = TimeSpan.FromHours(value.ParseInvariantString<int>());
                return true;
            case TimeSpanComponent.Days:
                result = TimeSpan.FromDays(value.ParseInvariantString<int>());
                return true;
            case TimeSpanComponent.None:
                break;
        }

        return TimeSpan.TryParse(value, out result);
    }

    /// <summary>
    /// Parses a TimeSpan string with optional component assumption.
    /// </summary>
    /// <param name="value">The string to parse</param>
    /// <param name="component">The component assumption for strings without delimiters</param>
    /// <returns>The parsed TimeSpan</returns>
    /// <exception cref="ArgumentException">Thrown when the value cannot be parsed</exception>
    public static TimeSpan ParseTimeSpan(string value, TimeSpanComponent component = TimeSpanComponent.None)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (TryParseTimeSpan(value, component, out TimeSpan result))
        {
            return result;
        }

        throw new ArgumentException($"Unable to parse TimeSpan from value: '{value}'", nameof(value));
    }

    /// <summary>
    /// Formats a TimeSpan with an explicit sign indicator.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to format</param>
    /// <returns>A string representation with explicit positive or negative sign</returns>
    public static string FormatWithSign(TimeSpan timeSpan) =>
        (timeSpan >= TimeSpan.Zero ? "+" : "") + timeSpan;

    /// <summary>
    /// Formats a nullable TimeSpan with an explicit sign indicator.
    /// </summary>
    /// <param name="timeSpan">The nullable TimeSpan to format</param>
    /// <returns>A string representation with explicit sign, or null if the input is null</returns>
    public static string? FormatWithSign(TimeSpan? timeSpan) =>
        timeSpan?.Let(FormatWithSign);

    /// <summary>
    /// Extension method to enable fluent syntax for nullable operations
    /// </summary>
    internal static TResult Let<T, TResult>(this T value, Func<T, TResult> func) => func(value);

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the last day of the specified month and year.
    /// </summary>
    /// <param name="year">The year</param>
    /// <param name="month">The month (1-12)</param>
    /// <returns>The last day of the month as a DateTime</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when year or month are out of valid range</exception>
    public static DateTime GetLastDayOfMonth(int year, int month)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(year, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(month, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(month, 12);

        int daysInMonth = DateTime.DaysInMonth(year, month);
        return new DateTime(year, month, daysInMonth);
    }

    /// <summary>
    /// Truncates a DateTime to the specified precision.
    /// </summary>
    /// <param name="dateTime">The DateTime to truncate</param>
    /// <param name="precision">The TimeSpan representing the precision</param>
    /// <returns>The truncated DateTime</returns>
    public static DateTime Truncate(DateTime dateTime, TimeSpan precision)
    {
        if (precision <= TimeSpan.Zero)
        {
            throw new ArgumentException("Precision must be positive", nameof(precision));
        }

        long ticks = dateTime.Ticks / precision.Ticks * precision.Ticks;
        return new DateTime(ticks, dateTime.Kind);
    }

    #endregion

    #region Obsolete Methods

    ///// <summary>
    ///// Gets the last day of the specified year\month combination.
    ///// </summary>
    ///// <param name="year">The year.</param>
    ///// <param name="month">The month.</param>
    ///// <returns>Last day of the specified year\month combination.</returns>
    ///// <exception cref="ArgumentOutOfRangeException">Is thrown if <paramref name="year"/> or <paramref name="month"/> are out of range.</exception>
    ///// <remarks>Uses the <see cref="GregorianCalendar"/> to determine the last day.</remarks>
    //public static DateTime GetLastDay(int year, int month)
    //{
    //    ArgumentOutOfRangeException.ThrowIfLessThan(year, 0);
    //    ArgumentOutOfRangeException.ThrowIfLessThan(month, 1);
    //    ArgumentOutOfRangeException.ThrowIfGreaterThan(month, 12);

    //    // Start at the last day of the month, until we get to the day of the week
    //    // we were looking for
    //    DateTime lastDay = new DateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
    //    return lastDay.Date;
    //}

    #endregion

}
