//--------------------------------------------------------------------------
// File:    MonthHelper.cs
// Content:	Helper class for working with months and related enumerations
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.ComponentModel;
using System.Globalization;

#endregion

namespace AnBo.Core;

/// <summary>
/// Standard month enumeration with full names
/// </summary>
public enum Month
{
    /// <summary>January - First month of the year</summary>
    January = 1,
    /// <summary>February - Second month of the year</summary>
    February = 2,
    /// <summary>March - Third month of the year</summary>
    March = 3,
    /// <summary>April - Fourth month of the year</summary>
    April = 4,
    /// <summary>May - Fifth month of the year</summary>
    May = 5,
    /// <summary>June - Sixth month of the year</summary>
    June = 6,
    /// <summary>July - Seventh month of the year</summary>
    July = 7,
    /// <summary>August - Eighth month of the year</summary>
    August = 8,
    /// <summary>September - Ninth month of the year</summary>
    September = 9,
    /// <summary>October - Tenth month of the year</summary>
    October = 10,
    /// <summary>November - Eleventh month of the year</summary>
    November = 11,
    /// <summary>December - Twelfth month of the year</summary>
    December = 12
}


/// <summary>
/// Utility methods for working with month enumerations and conversions
/// </summary>
public static class MonthHelper
{
    #region Private Static Collections

    private static readonly Dictionary<string, Month> EnglishAbbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "jan", Month.January },
        { "feb", Month.February },
        { "mar", Month.March },
        { "apr", Month.April },
        { "may", Month.May },
        { "jun", Month.June },
        { "jul", Month.July },
        { "aug", Month.August },
        { "sep", Month.September },
        { "oct", Month.October },
        { "nov", Month.November },
        { "dec", Month.December }
    };

    private static readonly Dictionary<string, Month> GermanAbbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "jan", Month.January },
        { "feb", Month.February },
        { "mär", Month.March },
        { "mar", Month.March }, // Alternative spelling
        { "apr", Month.April },
        { "mai", Month.May },
        { "jun", Month.June },
        { "jul", Month.July },
        { "aug", Month.August },
        { "sep", Month.September },
        { "okt", Month.October },
        { "nov", Month.November },
        { "dez", Month.December }
    };

    /// <summary>
    /// Commonly used cultures for convenience
    /// </summary>
    public static readonly CultureInfo EnglishCulture = new("en-US");
    public static readonly CultureInfo GermanCulture = new("de-DE");

    #endregion

    #region Month Parsing Methods

    /// <summary>
    /// Parses a month string (abbreviation or full name) to a Month enum value.
    /// Supports both English and German abbreviations and full names.
    /// </summary>
    /// <param name="monthString">The month string to parse</param>
    /// <param name="culture">Optional culture info for localized parsing. If null, uses current culture.</param>
    /// <returns>The corresponding Month enum value</returns>
    /// <exception cref="ArgumentException">Thrown when the month string cannot be parsed</exception>
    public static Month ParseMonth(string monthString, CultureInfo? culture = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(monthString);

        if (TryParseMonth(monthString, out Month result, culture))
        {
            return result;
        }

        throw new ArgumentException($"Unable to parse month from string: '{monthString}'", nameof(monthString));
    }

    /// <summary>
    /// Attempts to parse a month string to a Month enum value.
    /// </summary>
    /// <param name="monthString">The month string to parse</param>
    /// <param name="result">The parsed Month enum value if successful</param>
    /// <param name="culture">Optional culture info for localized parsing</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParseMonth(string? monthString, out Month result, CultureInfo? culture = null)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(monthString))
        {
            return false;
        }

        culture ??= CultureInfo.CurrentCulture;
        ReadOnlySpan<char> span = monthString.AsSpan().Trim();

        // Try parsing as a number first (1-12)
        if (span.Length <= 2 && int.TryParse(span, out int monthNumber) && monthNumber >= 1 && monthNumber <= 12)
        {
            result = (Month)monthNumber;
            return true;
        }

        // Try abbreviation parsing (3+ characters)
        if (span.Length >= 3)
        {
            string abbreviation = span[..3].ToString().ToLowerInvariant();

            // Try English abbreviations first
            if (EnglishAbbreviations.TryGetValue(abbreviation, out result))
            {
                return true;
            }

            // Try German abbreviations
            if (GermanAbbreviations.TryGetValue(abbreviation, out result))
            {
                return true;
            }
        }

        // Try full month name parsing using culture-specific month names
        for (int i = 1; i <= 12; i++)
        {
            string monthName = culture.DateTimeFormat.GetMonthName(i);
            if (span.Equals(monthName.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = (Month)i;
                return true;
            }

            // Also try abbreviated month names from culture
            string abbreviatedName = culture.DateTimeFormat.GetAbbreviatedMonthName(i);
            if (span.Equals(abbreviatedName.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = (Month)i;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Parses a month string using multiple culture attempts for maximum compatibility.
    /// </summary>
    /// <param name="monthString">The month string to parse</param>
    /// <param name="result">The parsed Month enum value if successful</param>
    /// <param name="cultures">Array of cultures to try in order. If null, uses current, English, and German cultures.</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParseMonthMultiCulture(string? monthString, out Month result, params CultureInfo[] cultures)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(monthString))
        {
            return false;
        }

        // Default cultures to try if none specified
        if (cultures.Length == 0)
            cultures = [CultureInfo.CurrentCulture, EnglishCulture, GermanCulture];

        // Try each culture in order
        foreach (var culture in cultures)
        {
            if (TryParseMonth(monthString, out result, culture))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Conversion Methods

    /// <summary>
    /// Converts a Month enum to its localized full name using the specified culture.
    /// </summary>
    /// <param name="month">The month to convert</param>
    /// <param name="culture">The culture to use for localization. If null, uses current culture.</param>
    /// <returns>Localized full month name</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not a valid Month value</exception>
    public static string ToLocalizedName(Month month, CultureInfo? culture = null)
    {
        if (!Enum.IsDefined(month))
        {
            throw new ArgumentOutOfRangeException(nameof(month), month, "Invalid month value");
        }

        culture ??= CultureInfo.CurrentCulture;
        return culture.DateTimeFormat.GetMonthName((int)month);
    }

    /// <summary>
    /// Converts a Month enum to its localized abbreviation using the specified culture.
    /// </summary>
    /// <param name="month">The month to convert</param>
    /// <param name="culture">The culture to use for localization. If null, uses current culture.</param>
    /// <returns>Localized month abbreviation</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not a valid Month value</exception>
    public static string ToLocalizedAbbreviation(Month month, CultureInfo? culture = null)
    {
        if (!Enum.IsDefined(month))
        {
            throw new ArgumentOutOfRangeException(nameof(month), month, "Invalid month value");
        }

        culture ??= CultureInfo.CurrentCulture;
        return culture.DateTimeFormat.GetAbbreviatedMonthName((int)month);
    }

    /// <summary>
    /// Converts a Month enum to its English abbreviation.
    /// </summary>
    /// <param name="month">The month to convert</param>
    /// <returns>English three-letter abbreviation</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not a valid Month value</exception>
    public static string ToEnglishAbbreviation(Month month) =>
        ToLocalizedAbbreviation(month, EnglishCulture);

    /// <summary>
    /// Converts a Month enum to its German abbreviation.
    /// </summary>
    /// <param name="month">The month to convert</param>
    /// <returns>German three-letter abbreviation</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not a valid Month value</exception>
    public static string ToGermanAbbreviation(Month month) =>
        ToLocalizedAbbreviation(month, GermanCulture);

    /// <summary>
    /// Converts a Month enum to its English full name.
    /// </summary>
    /// <param name="month">The month to convert</param>
    /// <returns>English full month name</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not a valid Month value</exception>
    public static string ToEnglishName(Month month) =>
        ToLocalizedName(month, EnglishCulture);

    /// <summary>
    /// Converts a Month enum to its German full name.
    /// </summary>
    /// <param name="month">The month to convert</param>
    /// <returns>German full month name</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not a valid Month value</exception>
    public static string ToGermanName(Month month) =>
        ToLocalizedName(month, GermanCulture);

    /// <summary>
    /// Converts a Month enum to a DateTime representing the first day of that month in the specified year.
    /// </summary>
    /// <param name="month">The month</param>
    /// <param name="year">The year</param>
    /// <returns>DateTime representing the first day of the month</returns>
    public static DateTime ToDateTime(Month month, int year) =>
        new(year, (int)month, 1);

    /// <summary>
    /// Converts a Month enum to a DateOnly representing the first day of that month in the specified year.
    /// </summary>
    /// <param name="month">The month</param>
    /// <param name="year">The year</param>
    /// <returns>DateOnly representing the first day of the month</returns>
    public static DateOnly ToDateOnly(Month month, int year) =>
        new(year, (int)month, 1);

    /// <summary>
    /// Gets all months as an enumerable collection.
    /// </summary>
    /// <returns>An enumerable of all Month values</returns>
    public static IEnumerable<Month> GetAllMonths() => Enum.GetValues<Month>();

    /// <summary>
    /// Gets the number of days in the specified month and year.
    /// </summary>
    /// <param name="month">The month</param>
    /// <param name="year">The year</param>
    /// <returns>The number of days in the month</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month or year are out of valid range</exception>
    public static int GetDaysInMonth(Month month, int year)
    {
        if (!Enum.IsDefined(month))
        {
            throw new ArgumentOutOfRangeException(nameof(month), month, "Invalid month value");
        }

        return DateTime.DaysInMonth(year, (int)month);
    }

    /// <summary>
    /// Determines if the specified year is a leap year.
    /// </summary>
    /// <param name="year">The year to check</param>
    /// <returns>True if the year is a leap year, false otherwise</returns>
    public static bool IsLeapYear(int year) => DateTime.IsLeapYear(year);

    #endregion

    #region Quarter Methods

    /// <summary>
    /// Gets the quarter (1-4) for the specified month.
    /// </summary>
    /// <param name="month">The month</param>
    /// <returns>The quarter number (1-4)</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not a valid Month value</exception>
    public static int GetQuarter(Month month)
    {
        if (!Enum.IsDefined(month))
        {
            throw new ArgumentOutOfRangeException(nameof(month), month, "Invalid month value");
        }

        return ((int)month - 1) / 3 + 1;
    }

    /// <summary>
    /// Gets all months in the specified quarter.
    /// </summary>
    /// <param name="quarter">The quarter (1-4)</param>
    /// <returns>An enumerable of months in the quarter</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when quarter is not between 1 and 4</exception>
    public static IEnumerable<Month> GetMonthsInQuarter(int quarter)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(quarter, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(quarter, 4);

        int startMonth = (quarter - 1) * 3 + 1;
        return Enumerable.Range(startMonth, 3).Cast<Month>();
    }

    #endregion

    #region Month Extensions

    /// <summary>
    /// Gets the next month, wrapping to January after December.
    /// </summary>
    /// <param name="month">The current month</param>
    /// <returns>The next month</returns>
    public static Month Next(this Month month) =>
        month == Month.December ? Month.January : month + 1;

    /// <summary>
    /// Gets the previous month, wrapping to December before January.
    /// </summary>
    /// <param name="month">The current month</param>
    /// <returns>The previous month</returns>
    public static Month Previous(this Month month) =>
        month == Month.January ? Month.December : month - 1;

    /// <summary>
    /// Determines if the month is in the first half of the year (January-June).
    /// </summary>
    /// <param name="month">The month</param>
    /// <returns>True if the month is in the first half of the year</returns>
    public static bool IsFirstHalfOfYear(this Month month) => (int)month <= 6;

    /// <summary>
    /// Determines if the month is in the second half of the year (July-December).
    /// </summary>
    /// <param name="month">The month</param>
    /// <returns>True if the month is in the second half of the year</returns>
    public static bool IsSecondHalfOfYear(this Month month) => (int)month > 6;


    #endregion

}
