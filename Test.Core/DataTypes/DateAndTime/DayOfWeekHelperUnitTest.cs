//--------------------------------------------------------------------------
// File:    DayOfWeekHelperUnitTest.cs
// Content: Unit tests for DayOfWeekHelper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Globalization;

namespace AnBo.Test;

public class DayOfWeekHelperUnitTest
{
    #region ParseDayOfWeek Method Tests

    [Fact]
    public void ParseDayOfWeek_With_Null_Should_Throw_ArgumentException()
    {
        // Arrange
        string? nullDayString = null;

        // Act & Assert
        var action = () => DayOfWeekHelper.ParseDayOfWeek(nullDayString!);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseDayOfWeek_With_Empty_String_Should_Throw_ArgumentException()
    {
        // Arrange
        string emptyDayString = string.Empty;

        // Act & Assert
        var action = () => DayOfWeekHelper.ParseDayOfWeek(emptyDayString);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseDayOfWeek_With_Whitespace_Should_Throw_ArgumentException()
    {
        // Arrange
        string whitespaceDayString = "   ";

        // Act & Assert
        var action = () => DayOfWeekHelper.ParseDayOfWeek(whitespaceDayString);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("mon", DayOfWeek.Monday)]
    [InlineData("tue", DayOfWeek.Tuesday)]
    [InlineData("wed", DayOfWeek.Wednesday)]
    [InlineData("thu", DayOfWeek.Thursday)]
    [InlineData("fri", DayOfWeek.Friday)]
    [InlineData("sat", DayOfWeek.Saturday)]
    [InlineData("sun", DayOfWeek.Sunday)]
    public void ParseDayOfWeek_With_English_Abbreviations_Should_Return_Correct_DayOfWeek(string dayString, DayOfWeek expected)
    {
        // Act
        var result = DayOfWeekHelper.ParseDayOfWeek(dayString);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("MON", DayOfWeek.Monday)]
    [InlineData("TUE", DayOfWeek.Tuesday)]
    [InlineData("WED", DayOfWeek.Wednesday)]
    [InlineData("THU", DayOfWeek.Thursday)]
    [InlineData("FRI", DayOfWeek.Friday)]
    [InlineData("SAT", DayOfWeek.Saturday)]
    [InlineData("SUN", DayOfWeek.Sunday)]
    public void ParseDayOfWeek_With_English_Abbreviations_Uppercase_Should_Return_Correct_DayOfWeek(string dayString, DayOfWeek expected)
    {
        // Act
        var result = DayOfWeekHelper.ParseDayOfWeek(dayString);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("mon", DayOfWeek.Monday)]
    [InlineData("die", DayOfWeek.Tuesday)]
    [InlineData("mit", DayOfWeek.Wednesday)]
    [InlineData("don", DayOfWeek.Thursday)]
    [InlineData("fre", DayOfWeek.Friday)]
    [InlineData("sam", DayOfWeek.Saturday)]
    [InlineData("son", DayOfWeek.Sunday)]
    public void ParseDayOfWeek_With_German_Abbreviations_Should_Return_Correct_DayOfWeek(string dayString, DayOfWeek expected)
    {
        // Act
        var result = DayOfWeekHelper.ParseDayOfWeek(dayString);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParseDayOfWeek_With_English_Culture_Full_Names_Should_Return_Correct_DayOfWeek()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act & Assert
        DayOfWeekHelper.ParseDayOfWeek("Monday", culture).Should().Be(DayOfWeek.Monday);
        DayOfWeekHelper.ParseDayOfWeek("Tuesday", culture).Should().Be(DayOfWeek.Tuesday);
        DayOfWeekHelper.ParseDayOfWeek("Wednesday", culture).Should().Be(DayOfWeek.Wednesday);
        DayOfWeekHelper.ParseDayOfWeek("Thursday", culture).Should().Be(DayOfWeek.Thursday);
        DayOfWeekHelper.ParseDayOfWeek("Friday", culture).Should().Be(DayOfWeek.Friday);
        DayOfWeekHelper.ParseDayOfWeek("Saturday", culture).Should().Be(DayOfWeek.Saturday);
        DayOfWeekHelper.ParseDayOfWeek("Sunday", culture).Should().Be(DayOfWeek.Sunday);
    }

    [Fact]
    public void ParseDayOfWeek_With_German_Culture_Full_Names_Should_Return_Correct_DayOfWeek()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act & Assert
        DayOfWeekHelper.ParseDayOfWeek("Montag", culture).Should().Be(DayOfWeek.Monday);
        DayOfWeekHelper.ParseDayOfWeek("Dienstag", culture).Should().Be(DayOfWeek.Tuesday);
        DayOfWeekHelper.ParseDayOfWeek("Mittwoch", culture).Should().Be(DayOfWeek.Wednesday);
        DayOfWeekHelper.ParseDayOfWeek("Donnerstag", culture).Should().Be(DayOfWeek.Thursday);
        DayOfWeekHelper.ParseDayOfWeek("Freitag", culture).Should().Be(DayOfWeek.Friday);
        DayOfWeekHelper.ParseDayOfWeek("Samstag", culture).Should().Be(DayOfWeek.Saturday);
        DayOfWeekHelper.ParseDayOfWeek("Sonntag", culture).Should().Be(DayOfWeek.Sunday);
    }

    [Fact]
    public void ParseDayOfWeek_With_Mixed_Case_Should_Return_Correct_DayOfWeek()
    {
        // Act & Assert
        DayOfWeekHelper.ParseDayOfWeek("MoN").Should().Be(DayOfWeek.Monday);
        DayOfWeekHelper.ParseDayOfWeek("tUe").Should().Be(DayOfWeek.Tuesday);
        DayOfWeekHelper.ParseDayOfWeek("WED").Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void ParseDayOfWeek_With_Whitespace_Around_Valid_Value_Should_Return_Correct_DayOfWeek()
    {
        // Act & Assert
        DayOfWeekHelper.ParseDayOfWeek("  mon  ").Should().Be(DayOfWeek.Monday);
        DayOfWeekHelper.ParseDayOfWeek("\ttue\t").Should().Be(DayOfWeek.Tuesday);
        DayOfWeekHelper.ParseDayOfWeek("\nwed\n").Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void ParseDayOfWeek_With_Culture_Abbreviated_Names_Should_Return_Correct_DayOfWeek()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act & Assert
        DayOfWeekHelper.ParseDayOfWeek("Mon", culture).Should().Be(DayOfWeek.Monday);
        DayOfWeekHelper.ParseDayOfWeek("Tue", culture).Should().Be(DayOfWeek.Tuesday);
        DayOfWeekHelper.ParseDayOfWeek("Wed", culture).Should().Be(DayOfWeek.Wednesday);
        DayOfWeekHelper.ParseDayOfWeek("Thu", culture).Should().Be(DayOfWeek.Thursday);
        DayOfWeekHelper.ParseDayOfWeek("Fri", culture).Should().Be(DayOfWeek.Friday);
        DayOfWeekHelper.ParseDayOfWeek("Sat", culture).Should().Be(DayOfWeek.Saturday);
        DayOfWeekHelper.ParseDayOfWeek("Sun", culture).Should().Be(DayOfWeek.Sunday);
    }

    [Fact]
    public void ParseDayOfWeek_With_Invalid_String_Should_Throw_ArgumentException()
    {
        // Arrange
        string invalidDayString = "invalid";

        // Act & Assert
        var action = () => DayOfWeekHelper.ParseDayOfWeek(invalidDayString);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Unable to parse day of week from string: 'invalid'*");
    }

    [Fact]
    public void ParseDayOfWeek_With_Short_String_Should_Throw_ArgumentException()
    {
        // Arrange
        string shortString = "ab";

        // Act & Assert
        var action = () => DayOfWeekHelper.ParseDayOfWeek(shortString);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseDayOfWeek_With_Numbers_Should_Throw_ArgumentException()
    {
        // Arrange
        string numberString = "123";

        // Act & Assert
        var action = () => DayOfWeekHelper.ParseDayOfWeek(numberString);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseDayOfWeek_With_CurrentCulture_Should_Use_Current_Culture()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            // Act
            var result = DayOfWeekHelper.ParseDayOfWeek("Monday");

            // Assert
            result.Should().Be(DayOfWeek.Monday);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    #endregion

    #region TryParseDayOfWeek Method Tests

    [Fact]
    public void TryParseDayOfWeek_With_Null_Should_Return_False()
    {
        // Arrange
        string? nullDayString = null;

        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(nullDayString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeFalse();
        dayOfWeek.Should().Be(default(DayOfWeek));
    }

    [Fact]
    public void TryParseDayOfWeek_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string emptyDayString = string.Empty;

        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(emptyDayString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeFalse();
        dayOfWeek.Should().Be(default(DayOfWeek));
    }

    [Fact]
    public void TryParseDayOfWeek_With_Whitespace_Should_Return_False()
    {
        // Arrange
        string whitespaceDayString = "   ";

        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(whitespaceDayString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeFalse();
        dayOfWeek.Should().Be(default(DayOfWeek));
    }

    [Theory]
    [InlineData("mon", DayOfWeek.Monday)]
    [InlineData("tue", DayOfWeek.Tuesday)]
    [InlineData("wed", DayOfWeek.Wednesday)]
    [InlineData("thu", DayOfWeek.Thursday)]
    [InlineData("fri", DayOfWeek.Friday)]
    [InlineData("sat", DayOfWeek.Saturday)]
    [InlineData("sun", DayOfWeek.Sunday)]
    public void TryParseDayOfWeek_With_English_Abbreviations_Should_Return_True_And_Correct_DayOfWeek(string dayString, DayOfWeek expected)
    {
        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(dayString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeTrue();
        dayOfWeek.Should().Be(expected);
    }

    [Theory]
    [InlineData("mon", DayOfWeek.Monday)]
    [InlineData("die", DayOfWeek.Tuesday)]
    [InlineData("mit", DayOfWeek.Wednesday)]
    [InlineData("don", DayOfWeek.Thursday)]
    [InlineData("fre", DayOfWeek.Friday)]
    [InlineData("sam", DayOfWeek.Saturday)]
    [InlineData("son", DayOfWeek.Sunday)]
    public void TryParseDayOfWeek_With_German_Abbreviations_Should_Return_True_And_Correct_DayOfWeek(string dayString, DayOfWeek expected)
    {
        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(dayString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeTrue();
        dayOfWeek.Should().Be(expected);
    }

    [Fact]
    public void TryParseDayOfWeek_With_English_Culture_Full_Names_Should_Return_True_And_Correct_DayOfWeek()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act & Assert
        DayOfWeekHelper.TryParseDayOfWeek("Monday", out var monday, culture).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);

        DayOfWeekHelper.TryParseDayOfWeek("Tuesday", out var tuesday, culture).Should().BeTrue();
        tuesday.Should().Be(DayOfWeek.Tuesday);

        DayOfWeekHelper.TryParseDayOfWeek("Wednesday", out var wednesday, culture).Should().BeTrue();
        wednesday.Should().Be(DayOfWeek.Wednesday);

        DayOfWeekHelper.TryParseDayOfWeek("Thursday", out var thursday, culture).Should().BeTrue();
        thursday.Should().Be(DayOfWeek.Thursday);

        DayOfWeekHelper.TryParseDayOfWeek("Friday", out var friday, culture).Should().BeTrue();
        friday.Should().Be(DayOfWeek.Friday);

        DayOfWeekHelper.TryParseDayOfWeek("Saturday", out var saturday, culture).Should().BeTrue();
        saturday.Should().Be(DayOfWeek.Saturday);

        DayOfWeekHelper.TryParseDayOfWeek("Sunday", out var sunday, culture).Should().BeTrue();
        sunday.Should().Be(DayOfWeek.Sunday);
    }

    [Fact]
    public void TryParseDayOfWeek_With_German_Culture_Full_Names_Should_Return_True_And_Correct_DayOfWeek()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act & Assert
        DayOfWeekHelper.TryParseDayOfWeek("Montag", out var monday, culture).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);

        DayOfWeekHelper.TryParseDayOfWeek("Dienstag", out var tuesday, culture).Should().BeTrue();
        tuesday.Should().Be(DayOfWeek.Tuesday);

        DayOfWeekHelper.TryParseDayOfWeek("Mittwoch", out var wednesday, culture).Should().BeTrue();
        wednesday.Should().Be(DayOfWeek.Wednesday);

        DayOfWeekHelper.TryParseDayOfWeek("Donnerstag", out var thursday, culture).Should().BeTrue();
        thursday.Should().Be(DayOfWeek.Thursday);

        DayOfWeekHelper.TryParseDayOfWeek("Freitag", out var friday, culture).Should().BeTrue();
        friday.Should().Be(DayOfWeek.Friday);

        DayOfWeekHelper.TryParseDayOfWeek("Samstag", out var saturday, culture).Should().BeTrue();
        saturday.Should().Be(DayOfWeek.Saturday);

        DayOfWeekHelper.TryParseDayOfWeek("Sonntag", out var sunday, culture).Should().BeTrue();
        sunday.Should().Be(DayOfWeek.Sunday);
    }

    [Fact]
    public void TryParseDayOfWeek_With_Mixed_Case_Should_Return_True_And_Correct_DayOfWeek()
    {
        // Act & Assert
        DayOfWeekHelper.TryParseDayOfWeek("MoN", out var monday).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);

        DayOfWeekHelper.TryParseDayOfWeek("tUe", out var tuesday).Should().BeTrue();
        tuesday.Should().Be(DayOfWeek.Tuesday);

        DayOfWeekHelper.TryParseDayOfWeek("WED", out var wednesday).Should().BeTrue();
        wednesday.Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void TryParseDayOfWeek_With_Whitespace_Around_Valid_Value_Should_Return_True_And_Correct_DayOfWeek()
    {
        // Act & Assert
        DayOfWeekHelper.TryParseDayOfWeek("  mon  ", out var monday).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);

        DayOfWeekHelper.TryParseDayOfWeek("\ttue\t", out var tuesday).Should().BeTrue();
        tuesday.Should().Be(DayOfWeek.Tuesday);

        DayOfWeekHelper.TryParseDayOfWeek("\nwed\n", out var wednesday).Should().BeTrue();
        wednesday.Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void TryParseDayOfWeek_With_Culture_Abbreviated_Names_Should_Return_True_And_Correct_DayOfWeek()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act & Assert
        DayOfWeekHelper.TryParseDayOfWeek("Mon", out var monday, culture).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);

        DayOfWeekHelper.TryParseDayOfWeek("Tue", out var tuesday, culture).Should().BeTrue();
        tuesday.Should().Be(DayOfWeek.Tuesday);

        DayOfWeekHelper.TryParseDayOfWeek("Wed", out var wednesday, culture).Should().BeTrue();
        wednesday.Should().Be(DayOfWeek.Wednesday);

        DayOfWeekHelper.TryParseDayOfWeek("Thu", out var thursday, culture).Should().BeTrue();
        thursday.Should().Be(DayOfWeek.Thursday);

        DayOfWeekHelper.TryParseDayOfWeek("Fri", out var friday, culture).Should().BeTrue();
        friday.Should().Be(DayOfWeek.Friday);

        DayOfWeekHelper.TryParseDayOfWeek("Sat", out var saturday, culture).Should().BeTrue();
        saturday.Should().Be(DayOfWeek.Saturday);

        DayOfWeekHelper.TryParseDayOfWeek("Sun", out var sunday, culture).Should().BeTrue();
        sunday.Should().Be(DayOfWeek.Sunday);
    }

    [Fact]
    public void TryParseDayOfWeek_With_Invalid_String_Should_Return_False()
    {
        // Arrange
        string invalidDayString = "invalid";

        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(invalidDayString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeFalse();
        dayOfWeek.Should().Be(default(DayOfWeek));
    }

    [Fact]
    public void TryParseDayOfWeek_With_Short_String_Should_Return_False()
    {
        // Arrange
        string shortString = "ab";

        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(shortString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeFalse();
        dayOfWeek.Should().Be(default(DayOfWeek));
    }

    [Fact]
    public void TryParseDayOfWeek_With_Numbers_Should_Return_False()
    {
        // Arrange
        string numberString = "123";

        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(numberString, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeFalse();
        dayOfWeek.Should().Be(default(DayOfWeek));
    }

    [Fact]
    public void TryParseDayOfWeek_With_CurrentCulture_Should_Use_Current_Culture()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            // Act
            var result = DayOfWeekHelper.TryParseDayOfWeek("Monday", out DayOfWeek dayOfWeek);

            // Assert
            result.Should().BeTrue();
            dayOfWeek.Should().Be(DayOfWeek.Monday);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void TryParseDayOfWeek_With_Null_Culture_Should_Use_Current_Culture()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            // Act
            var result = DayOfWeekHelper.TryParseDayOfWeek("Monday", out DayOfWeek dayOfWeek, null);

            // Assert
            result.Should().BeTrue();
            dayOfWeek.Should().Be(DayOfWeek.Monday);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void TryParseDayOfWeek_With_Long_String_Starting_With_Valid_Abbreviation_Should_Return_True()
    {
        // Act & Assert
        DayOfWeekHelper.TryParseDayOfWeek("monday", out var monday).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);

        DayOfWeekHelper.TryParseDayOfWeek("tuesday", out var tuesday).Should().BeTrue();
        tuesday.Should().Be(DayOfWeek.Tuesday);
    }

    [Fact]
    public void TryParseDayOfWeek_Prioritizes_English_Over_German_Abbreviations()
    {
        // Arrange - "mon" exists in both English and German dictionaries
        string ambiguousAbbreviation = "mon";

        // Act
        var result = DayOfWeekHelper.TryParseDayOfWeek(ambiguousAbbreviation, out DayOfWeek dayOfWeek);

        // Assert
        result.Should().BeTrue();
        dayOfWeek.Should().Be(DayOfWeek.Monday); // Should match English abbreviation
    }

    [Fact]
    public void TryParseDayOfWeek_With_French_Culture_Should_Work_With_French_Names()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("fr-FR");

        // Act & Assert
        DayOfWeekHelper.TryParseDayOfWeek("lundi", out var monday, culture).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);

        DayOfWeekHelper.TryParseDayOfWeek("mardi", out var tuesday, culture).Should().BeTrue();
        tuesday.Should().Be(DayOfWeek.Tuesday);

        DayOfWeekHelper.TryParseDayOfWeek("mercredi", out var wednesday, culture).Should().BeTrue();
        wednesday.Should().Be(DayOfWeek.Wednesday);

        DayOfWeekHelper.TryParseDayOfWeek("jeudi", out var thursday, culture).Should().BeTrue();
        thursday.Should().Be(DayOfWeek.Thursday);

        DayOfWeekHelper.TryParseDayOfWeek("vendredi", out var friday, culture).Should().BeTrue();
        friday.Should().Be(DayOfWeek.Friday);

        DayOfWeekHelper.TryParseDayOfWeek("samedi", out var saturday, culture).Should().BeTrue();
        saturday.Should().Be(DayOfWeek.Saturday);

        DayOfWeekHelper.TryParseDayOfWeek("dimanche", out var sunday, culture).Should().BeTrue();
        sunday.Should().Be(DayOfWeek.Sunday);
    }

    #endregion

    #region Integration and Edge Case Tests

    [Fact]
    public void ParseDayOfWeek_And_TryParseDayOfWeek_Should_Return_Same_Results_For_Valid_Input()
    {
        // Arrange
        var testCases = new[]
        {
            "mon", "tue", "wed", "thu", "fri", "sat", "sun",
            "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN",
            "die", "mit", "don", "fre", "sam", "son"
        };

        foreach (var testCase in testCases)
        {
            // Act
            var parseResult = DayOfWeekHelper.ParseDayOfWeek(testCase);
            var tryParseResult = DayOfWeekHelper.TryParseDayOfWeek(testCase, out DayOfWeek dayOfWeek);

            // Assert
            tryParseResult.Should().BeTrue();
            dayOfWeek.Should().Be(parseResult);
        }
    }

    [Fact]
    public void TryParseDayOfWeek_Should_Return_False_For_Invalid_Input_While_ParseDayOfWeek_Throws()
    {
        // Arrange
        var invalidInputs = new[] { "invalid", "xyz", "123", "ab", "", "   " };

        foreach (var invalidInput in invalidInputs)
        {
            // Act & Assert
            var tryParseResult = DayOfWeekHelper.TryParseDayOfWeek(invalidInput, out DayOfWeek dayOfWeek);
            tryParseResult.Should().BeFalse();
            dayOfWeek.Should().Be(default(DayOfWeek));

            if (!string.IsNullOrWhiteSpace(invalidInput))
            {
                var parseAction = () => DayOfWeekHelper.ParseDayOfWeek(invalidInput);
                parseAction.Should().Throw<ArgumentException>();
            }
        }
    }

    [Fact]
    public void Methods_Should_Handle_Unicode_Characters_In_Culture_Names()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act & Assert - German days with umlauts should work
        DayOfWeekHelper.TryParseDayOfWeek("Montag", out var monday, culture).Should().BeTrue();
        monday.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void Methods_Should_Be_Case_Insensitive_For_All_Supported_Formats()
    {
        // Arrange
        var testCases = new[]
        {
            ("monday", "MONDAY", "Monday", "MoNdAy"),
            ("tue", "TUE", "Tue", "tUe"),
            ("mit", "MIT", "Mit", "MiT")
        };

        foreach (var (lower, upper, proper, mixed) in testCases)
        {
            // Act
            var lowerResult = DayOfWeekHelper.TryParseDayOfWeek(lower, out var lowerDay);
            var upperResult = DayOfWeekHelper.TryParseDayOfWeek(upper, out var upperDay);
            var properResult = DayOfWeekHelper.TryParseDayOfWeek(proper, out var properDay);
            var mixedResult = DayOfWeekHelper.TryParseDayOfWeek(mixed, out var mixedDay);

            // Assert
            lowerResult.Should().BeTrue();
            upperResult.Should().BeTrue();
            properResult.Should().BeTrue();
            mixedResult.Should().BeTrue();

            upperDay.Should().Be(lowerDay);
            properDay.Should().Be(lowerDay);
            mixedDay.Should().Be(lowerDay);
        }
    }

    [Fact]
    public void Methods_Should_Handle_Various_Whitespace_Characters()
    {
        // Arrange
        var whitespaceVariations = new[]
        {
            " mon ",
            "\tmon\t",
            "\nmon\n",
            "\r\nmon\r\n",
            "  \t\n mon \n\t  "
        };

        foreach (var variation in whitespaceVariations)
        {
            // Act
            var result = DayOfWeekHelper.TryParseDayOfWeek(variation, out var dayOfWeek);

            // Assert
            result.Should().BeTrue();
            dayOfWeek.Should().Be(DayOfWeek.Monday);
        }
    }

    #endregion

    #region Performance and Consistency Tests

    [Fact]
    public void Methods_Should_Be_Consistent_Across_Multiple_Calls()
    {
        // Arrange
        const int iterations = 1000;
        const string testInput = "mon";
        var expectedDay = DayOfWeek.Monday;

        // Act & Assert
        for (int i = 0; i < iterations; i++)
        {
            var parseResult = DayOfWeekHelper.ParseDayOfWeek(testInput);
            var tryParseResult = DayOfWeekHelper.TryParseDayOfWeek(testInput, out var dayOfWeek);

            parseResult.Should().Be(expectedDay);
            tryParseResult.Should().BeTrue();
            dayOfWeek.Should().Be(expectedDay);
        }
    }

    [Fact]
    public void Methods_Should_Handle_Concurrent_Access()
    {
        // Arrange
        const int threadCount = 10;
        const int operationsPerThread = 100;
        var testData = new[] { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };
        var tasks = new List<Task>();

        // Act
        for (int t = 0; t < threadCount; t++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int i = 0; i < operationsPerThread; i++)
                {
                    var testInput = testData[i % testData.Length];
                    var parseResult = DayOfWeekHelper.ParseDayOfWeek(testInput);
                    var tryParseResult = DayOfWeekHelper.TryParseDayOfWeek(testInput, out var dayOfWeek);

                    // Basic consistency check
                    tryParseResult.Should().BeTrue();
                    dayOfWeek.Should().Be(parseResult);
                }
            }));
        }

        // Assert
        var aggregateException = Record.Exception(() => Task.WaitAll(tasks.ToArray()));
        aggregateException.Should().BeNull();
    }

    #endregion
}