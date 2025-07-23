//--------------------------------------------------------------------------
// File:    DayOfWeekHelper.cs
// Content:	Implementation of class DayOfWeekHelper
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

using System.Globalization;

namespace AnBo.Core;

/// <summary>
/// Utility methods for working with DayOfWeek enumerations
/// </summary>
public static class DayOfWeekHelper
{
    #region Private Static Collections

    private static readonly Dictionary<string, DayOfWeek> EnglishDayOfWeekAbbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "mon", DayOfWeek.Monday },
        { "tue", DayOfWeek.Tuesday },
        { "wed", DayOfWeek.Wednesday },
        { "thu", DayOfWeek.Thursday },
        { "fri", DayOfWeek.Friday },
        { "sat", DayOfWeek.Saturday },
        { "sun", DayOfWeek.Sunday }
    };

    private static readonly Dictionary<string, DayOfWeek> GermanDayOfWeekAbbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "mon", DayOfWeek.Monday },
        { "die", DayOfWeek.Tuesday },
        { "mit", DayOfWeek.Wednesday },
        { "don", DayOfWeek.Thursday },
        { "fre", DayOfWeek.Friday },
        { "sam", DayOfWeek.Saturday },
        { "son", DayOfWeek.Sunday }
    };

    #endregion

    #region Day of Week Parsing Methods

    /// <summary>
    /// Parses a day of week string (abbreviation or full name) to a DayOfWeek enum value.
    /// </summary>
    /// <param name="dayString">The day string to parse</param>
    /// <param name="culture">Optional culture info for localized parsing</param>
    /// <returns>The corresponding DayOfWeek enum value</returns>
    /// <exception cref="ArgumentException">Thrown when the day string cannot be parsed</exception>
    public static DayOfWeek ParseDayOfWeek(string dayString, CultureInfo? culture = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dayString);

        if (TryParseDayOfWeek(dayString, out DayOfWeek result, culture))
        {
            return result;
        }

        throw new ArgumentException($"Unable to parse day of week from string: '{dayString}'", nameof(dayString));
    }

    /// <summary>
    /// Attempts to parse a day of week string to a DayOfWeek enum value.
    /// </summary>
    /// <param name="dayString">The day string to parse</param>
    /// <param name="result">The parsed DayOfWeek enum value if successful</param>
    /// <param name="culture">Optional culture info for localized parsing</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParseDayOfWeek(string? dayString, out DayOfWeek result, CultureInfo? culture = null)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(dayString))
        {
            return false;
        }

        culture ??= CultureInfo.CurrentCulture;
        ReadOnlySpan<char> span = dayString.AsSpan().Trim();

        // Try abbreviation parsing (3+ characters)
        if (span.Length >= 3)
        {
            string abbreviation = span[..3].ToString().ToLowerInvariant();

            // Try English abbreviations first
            if (EnglishDayOfWeekAbbreviations.TryGetValue(abbreviation, out result))
            {
                return true;
            }

            // Try German abbreviations
            if (GermanDayOfWeekAbbreviations.TryGetValue(abbreviation, out result))
            {
                return true;
            }
        }

        // Try full day name parsing using culture-specific day names
        for (int i = 0; i < 7; i++)
        {
            var dayOfWeek = (DayOfWeek)i;
            string dayName = culture.DateTimeFormat.GetDayName(dayOfWeek);
            if (span.Equals(dayName.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = dayOfWeek;
                return true;
            }

            // Also try abbreviated day names from culture
            string abbreviatedName = culture.DateTimeFormat.GetAbbreviatedDayName(dayOfWeek);
            if (span.Equals(abbreviatedName.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = dayOfWeek;
                return true;
            }
        }

        return false;
    }

    #endregion
}
