//--------------------------------------------------------------------------
// File:    MonthHelperUnitTest.cs
// Content: Unit tests for MonthHelper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Globalization;

namespace AnBo.Test;

public class MonthHelperUnitTest
{
    #region ParseMonth Method Tests

    [Fact]
    public void ParseMonth_With_Null_Should_Throw_ArgumentException()
    {
        // Arrange
        string? nullMonthString = null;

        // Act & Assert
        var action = () => MonthHelper.ParseMonth(nullMonthString!);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseMonth_With_Empty_String_Should_Throw_ArgumentException()
    {
        // Arrange
        string emptyMonthString = string.Empty;

        // Act & Assert
        var action = () => MonthHelper.ParseMonth(emptyMonthString);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseMonth_With_Whitespace_Should_Throw_ArgumentException()
    {
        // Arrange
        string whitespaceMonthString = "   ";

        // Act & Assert
        var action = () => MonthHelper.ParseMonth(whitespaceMonthString);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("1", Month.January)]
    [InlineData("2", Month.February)]
    [InlineData("3", Month.March)]
    [InlineData("4", Month.April)]
    [InlineData("5", Month.May)]
    [InlineData("6", Month.June)]
    [InlineData("7", Month.July)]
    [InlineData("8", Month.August)]
    [InlineData("9", Month.September)]
    [InlineData("10", Month.October)]
    [InlineData("11", Month.November)]
    [InlineData("12", Month.December)]
    public void ParseMonth_With_Valid_Numbers_Should_Return_Correct_Month(string monthString, Month expected)
    {
        // Act
        var result = MonthHelper.ParseMonth(monthString);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("jan", Month.January)]
    [InlineData("feb", Month.February)]
    [InlineData("mar", Month.March)]
    [InlineData("apr", Month.April)]
    [InlineData("may", Month.May)]
    [InlineData("jun", Month.June)]
    [InlineData("jul", Month.July)]
    [InlineData("aug", Month.August)]
    [InlineData("sep", Month.September)]
    [InlineData("oct", Month.October)]
    [InlineData("nov", Month.November)]
    [InlineData("dec", Month.December)]
    public void ParseMonth_With_English_Abbreviations_Should_Return_Correct_Month(string monthString, Month expected)
    {
        // Act
        var result = MonthHelper.ParseMonth(monthString);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("JAN", Month.January)]
    [InlineData("FEB", Month.February)]
    [InlineData("MAR", Month.March)]
    [InlineData("APR", Month.April)]
    [InlineData("MAY", Month.May)]
    [InlineData("JUN", Month.June)]
    [InlineData("JUL", Month.July)]
    [InlineData("AUG", Month.August)]
    [InlineData("SEP", Month.September)]
    [InlineData("OCT", Month.October)]
    [InlineData("NOV", Month.November)]
    [InlineData("DEC", Month.December)]
    public void ParseMonth_With_English_Abbreviations_Uppercase_Should_Return_Correct_Month(string monthString, Month expected)
    {
        // Act
        var result = MonthHelper.ParseMonth(monthString);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("jan", Month.January)]
    [InlineData("feb", Month.February)]
    [InlineData("mär", Month.March)]
    [InlineData("mar", Month.March)]
    [InlineData("apr", Month.April)]
    [InlineData("mai", Month.May)]
    [InlineData("jun", Month.June)]
    [InlineData("jul", Month.July)]
    [InlineData("aug", Month.August)]
    [InlineData("sep", Month.September)]
    [InlineData("okt", Month.October)]
    [InlineData("nov", Month.November)]
    [InlineData("dez", Month.December)]
    public void ParseMonth_With_German_Abbreviations_Should_Return_Correct_Month(string monthString, Month expected)
    {
        // Act
        var result = MonthHelper.ParseMonth(monthString);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParseMonth_With_English_Culture_Full_Names_Should_Return_Correct_Month()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act & Assert
        MonthHelper.ParseMonth("January", culture).Should().Be(Month.January);
        MonthHelper.ParseMonth("February", culture).Should().Be(Month.February);
        MonthHelper.ParseMonth("March", culture).Should().Be(Month.March);
        MonthHelper.ParseMonth("April", culture).Should().Be(Month.April);
        MonthHelper.ParseMonth("May", culture).Should().Be(Month.May);
        MonthHelper.ParseMonth("June", culture).Should().Be(Month.June);
        MonthHelper.ParseMonth("July", culture).Should().Be(Month.July);
        MonthHelper.ParseMonth("August", culture).Should().Be(Month.August);
        MonthHelper.ParseMonth("September", culture).Should().Be(Month.September);
        MonthHelper.ParseMonth("October", culture).Should().Be(Month.October);
        MonthHelper.ParseMonth("November", culture).Should().Be(Month.November);
        MonthHelper.ParseMonth("December", culture).Should().Be(Month.December);
    }

    [Fact]
    public void ParseMonth_With_German_Culture_Full_Names_Should_Return_Correct_Month()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act & Assert
        MonthHelper.ParseMonth("Januar", culture).Should().Be(Month.January);
        MonthHelper.ParseMonth("Februar", culture).Should().Be(Month.February);
        MonthHelper.ParseMonth("März", culture).Should().Be(Month.March);
        MonthHelper.ParseMonth("April", culture).Should().Be(Month.April);
        MonthHelper.ParseMonth("Mai", culture).Should().Be(Month.May);
        MonthHelper.ParseMonth("Juni", culture).Should().Be(Month.June);
        MonthHelper.ParseMonth("Juli", culture).Should().Be(Month.July);
        MonthHelper.ParseMonth("August", culture).Should().Be(Month.August);
        MonthHelper.ParseMonth("September", culture).Should().Be(Month.September);
        MonthHelper.ParseMonth("Oktober", culture).Should().Be(Month.October);
        MonthHelper.ParseMonth("November", culture).Should().Be(Month.November);
        MonthHelper.ParseMonth("Dezember", culture).Should().Be(Month.December);
    }

    [Fact]
    public void ParseMonth_With_Mixed_Case_Should_Return_Correct_Month()
    {
        // Act & Assert
        MonthHelper.ParseMonth("JaN").Should().Be(Month.January);
        MonthHelper.ParseMonth("fEb").Should().Be(Month.February);
        MonthHelper.ParseMonth("MAR").Should().Be(Month.March);
    }

    [Fact]
    public void ParseMonth_With_Whitespace_Around_Valid_Value_Should_Return_Correct_Month()
    {
        // Act & Assert
        MonthHelper.ParseMonth("  jan  ").Should().Be(Month.January);
        MonthHelper.ParseMonth("\tfeb\t").Should().Be(Month.February);
        MonthHelper.ParseMonth("\nmar\n").Should().Be(Month.March);
    }

    [Fact]
    public void ParseMonth_With_Invalid_String_Should_Throw_ArgumentException()
    {
        // Arrange
        string invalidMonthString = "invalid";

        // Act & Assert
        var action = () => MonthHelper.ParseMonth(invalidMonthString);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Unable to parse month from string: 'invalid'*");
    }

    [Theory]
    [InlineData("0")]
    [InlineData("13")]
    [InlineData("100")]
    [InlineData("-1")]
    public void ParseMonth_With_Invalid_Numbers_Should_Throw_ArgumentException(string invalidNumber)
    {
        // Act & Assert
        var action = () => MonthHelper.ParseMonth(invalidNumber);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseMonth_With_Short_String_Should_Throw_ArgumentException()
    {
        // Arrange
        string shortString = "ab";

        // Act & Assert
        var action = () => MonthHelper.ParseMonth(shortString);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region TryParseMonth Method Tests

    [Fact]
    public void TryParseMonth_With_Null_Should_Return_False()
    {
        // Arrange
        string? nullMonthString = null;

        // Act
        var result = MonthHelper.TryParseMonth(nullMonthString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    [Fact]
    public void TryParseMonth_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string emptyMonthString = string.Empty;

        // Act
        var result = MonthHelper.TryParseMonth(emptyMonthString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    [Fact]
    public void TryParseMonth_With_Whitespace_Should_Return_False()
    {
        // Arrange
        string whitespaceMonthString = "   ";

        // Act
        var result = MonthHelper.TryParseMonth(whitespaceMonthString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    [Theory]
    [InlineData("1", Month.January)]
    [InlineData("2", Month.February)]
    [InlineData("3", Month.March)]
    [InlineData("4", Month.April)]
    [InlineData("5", Month.May)]
    [InlineData("6", Month.June)]
    [InlineData("7", Month.July)]
    [InlineData("8", Month.August)]
    [InlineData("9", Month.September)]
    [InlineData("10", Month.October)]
    [InlineData("11", Month.November)]
    [InlineData("12", Month.December)]
    public void TryParseMonth_With_Valid_Numbers_Should_Return_True_And_Correct_Month(string monthString, Month expected)
    {
        // Act
        var result = MonthHelper.TryParseMonth(monthString, out Month month);

        // Assert
        result.Should().BeTrue();
        month.Should().Be(expected);
    }

    [Theory]
    [InlineData("jan", Month.January)]
    [InlineData("feb", Month.February)]
    [InlineData("mar", Month.March)]
    [InlineData("apr", Month.April)]
    [InlineData("may", Month.May)]
    [InlineData("jun", Month.June)]
    [InlineData("jul", Month.July)]
    [InlineData("aug", Month.August)]
    [InlineData("sep", Month.September)]
    [InlineData("oct", Month.October)]
    [InlineData("nov", Month.November)]
    [InlineData("dec", Month.December)]
    public void TryParseMonth_With_English_Abbreviations_Should_Return_True_And_Correct_Month(string monthString, Month expected)
    {
        // Act
        var result = MonthHelper.TryParseMonth(monthString, out Month month);

        // Assert
        result.Should().BeTrue();
        month.Should().Be(expected);
    }

    [Theory]
    [InlineData("jan", Month.January)]
    [InlineData("feb", Month.February)]
    [InlineData("mär", Month.March)]
    [InlineData("mar", Month.March)]
    [InlineData("apr", Month.April)]
    [InlineData("mai", Month.May)]
    [InlineData("jun", Month.June)]
    [InlineData("jul", Month.July)]
    [InlineData("aug", Month.August)]
    [InlineData("sep", Month.September)]
    [InlineData("okt", Month.October)]
    [InlineData("nov", Month.November)]
    [InlineData("dez", Month.December)]
    public void TryParseMonth_With_German_Abbreviations_Should_Return_True_And_Correct_Month(string monthString, Month expected)
    {
        // Act
        var result = MonthHelper.TryParseMonth(monthString, out Month month);

        // Assert
        result.Should().BeTrue();
        month.Should().Be(expected);
    }

    [Fact]
    public void TryParseMonth_With_English_Culture_Full_Names_Should_Return_True_And_Correct_Month()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act & Assert
        MonthHelper.TryParseMonth("January", out var january, culture).Should().BeTrue();
        january.Should().Be(Month.January);

        MonthHelper.TryParseMonth("February", out var february, culture).Should().BeTrue();
        february.Should().Be(Month.February);

        MonthHelper.TryParseMonth("March", out var march, culture).Should().BeTrue();
        march.Should().Be(Month.March);

        MonthHelper.TryParseMonth("April", out var april, culture).Should().BeTrue();
        april.Should().Be(Month.April);

        MonthHelper.TryParseMonth("May", out var may, culture).Should().BeTrue();
        may.Should().Be(Month.May);

        MonthHelper.TryParseMonth("June", out var june, culture).Should().BeTrue();
        june.Should().Be(Month.June);

        MonthHelper.TryParseMonth("July", out var july, culture).Should().BeTrue();
        july.Should().Be(Month.July);

        MonthHelper.TryParseMonth("August", out var august, culture).Should().BeTrue();
        august.Should().Be(Month.August);

        MonthHelper.TryParseMonth("September", out var september, culture).Should().BeTrue();
        september.Should().Be(Month.September);

        MonthHelper.TryParseMonth("October", out var october, culture).Should().BeTrue();
        october.Should().Be(Month.October);

        MonthHelper.TryParseMonth("November", out var november, culture).Should().BeTrue();
        november.Should().Be(Month.November);

        MonthHelper.TryParseMonth("December", out var december, culture).Should().BeTrue();
        december.Should().Be(Month.December);
    }

    [Fact]
    public void TryParseMonth_With_German_Culture_Full_Names_Should_Return_True_And_Correct_Month()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act & Assert
        MonthHelper.TryParseMonth("Januar", out var january, culture).Should().BeTrue();
        january.Should().Be(Month.January);

        MonthHelper.TryParseMonth("Februar", out var february, culture).Should().BeTrue();
        february.Should().Be(Month.February);

        MonthHelper.TryParseMonth("März", out var march, culture).Should().BeTrue();
        march.Should().Be(Month.March);

        MonthHelper.TryParseMonth("April", out var april, culture).Should().BeTrue();
        april.Should().Be(Month.April);

        MonthHelper.TryParseMonth("Mai", out var may, culture).Should().BeTrue();
        may.Should().Be(Month.May);

        MonthHelper.TryParseMonth("Juni", out var june, culture).Should().BeTrue();
        june.Should().Be(Month.June);

        MonthHelper.TryParseMonth("Juli", out var july, culture).Should().BeTrue();
        july.Should().Be(Month.July);

        MonthHelper.TryParseMonth("August", out var august, culture).Should().BeTrue();
        august.Should().Be(Month.August);

        MonthHelper.TryParseMonth("September", out var september, culture).Should().BeTrue();
        september.Should().Be(Month.September);

        MonthHelper.TryParseMonth("Oktober", out var october, culture).Should().BeTrue();
        october.Should().Be(Month.October);

        MonthHelper.TryParseMonth("November", out var november, culture).Should().BeTrue();
        november.Should().Be(Month.November);

        MonthHelper.TryParseMonth("Dezember", out var december, culture).Should().BeTrue();
        december.Should().Be(Month.December);
    }

    [Fact]
    public void TryParseMonth_With_Invalid_String_Should_Return_False()
    {
        // Arrange
        string invalidMonthString = "invalid";

        // Act
        var result = MonthHelper.TryParseMonth(invalidMonthString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("13")]
    [InlineData("100")]
    [InlineData("-1")]
    public void TryParseMonth_With_Invalid_Numbers_Should_Return_False(string invalidNumber)
    {
        // Act
        var result = MonthHelper.TryParseMonth(invalidNumber, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    [Fact]
    public void TryParseMonth_With_Short_String_Should_Return_False()
    {
        // Arrange
        string shortString = "ab";

        // Act
        var result = MonthHelper.TryParseMonth(shortString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    #endregion

    #region TryParseMonthMultiCulture Method Tests

    [Fact]
    public void TryParseMonthMultiCulture_With_Null_Should_Return_False()
    {
        // Arrange
        string? nullMonthString = null;

        // Act
        var result = MonthHelper.TryParseMonthMultiCulture(nullMonthString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    [Fact]
    public void TryParseMonthMultiCulture_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string emptyMonthString = string.Empty;

        // Act
        var result = MonthHelper.TryParseMonthMultiCulture(emptyMonthString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    [Fact]
    public void TryParseMonthMultiCulture_With_English_Names_Should_Return_True()
    {
        // Act & Assert
        MonthHelper.TryParseMonthMultiCulture("January", out var january).Should().BeTrue();
        january.Should().Be(Month.January);

        MonthHelper.TryParseMonthMultiCulture("jan", out var jan).Should().BeTrue();
        jan.Should().Be(Month.January);
    }

    [Fact]
    public void TryParseMonthMultiCulture_With_German_Names_Should_Return_True()
    {
        // Act & Assert
        MonthHelper.TryParseMonthMultiCulture("dez", out var december).Should().BeTrue();
        december.Should().Be(Month.December);

        MonthHelper.TryParseMonthMultiCulture("mär", out var march).Should().BeTrue();
        march.Should().Be(Month.March);
    }

    [Fact]
    public void TryParseMonthMultiCulture_With_Custom_Cultures_Should_Use_Provided_Cultures()
    {
        // Arrange
        var frenchCulture = CultureInfo.GetCultureInfo("fr-FR");
        var cultures = new[] { frenchCulture };

        // Act & Assert
        MonthHelper.TryParseMonthMultiCulture("janvier", out var january, cultures).Should().BeTrue();
        january.Should().Be(Month.January);

        MonthHelper.TryParseMonthMultiCulture("février", out var february, cultures).Should().BeTrue();
        february.Should().Be(Month.February);

        MonthHelper.TryParseMonthMultiCulture("mars", out var march, cultures).Should().BeTrue();
        march.Should().Be(Month.March);
    }

    [Fact]
    public void TryParseMonthMultiCulture_With_Invalid_String_Should_Return_False()
    {
        // Arrange
        string invalidMonthString = "invalid";

        // Act
        var result = MonthHelper.TryParseMonthMultiCulture(invalidMonthString, out Month month);

        // Assert
        result.Should().BeFalse();
        month.Should().Be(default(Month));
    }

    #endregion

    #region Conversion Methods Tests

    [Theory]
    [InlineData(Month.January)]
    [InlineData(Month.February)]
    [InlineData(Month.March)]
    [InlineData(Month.April)]
    [InlineData(Month.May)]
    [InlineData(Month.June)]
    [InlineData(Month.July)]
    [InlineData(Month.August)]
    [InlineData(Month.September)]
    [InlineData(Month.October)]
    [InlineData(Month.November)]
    [InlineData(Month.December)]
    public void ToLocalizedName_With_Valid_Month_Should_Return_Localized_Name(Month month)
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act
        var result = MonthHelper.ToLocalizedName(month, culture);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(culture.DateTimeFormat.GetMonthName((int)month));
    }

    [Fact]
    public void ToLocalizedName_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.ToLocalizedName(invalidMonth);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("month");
    }

    [Theory]
    [InlineData(Month.January)]
    [InlineData(Month.February)]
    [InlineData(Month.March)]
    [InlineData(Month.April)]
    [InlineData(Month.May)]
    [InlineData(Month.June)]
    [InlineData(Month.July)]
    [InlineData(Month.August)]
    [InlineData(Month.September)]
    [InlineData(Month.October)]
    [InlineData(Month.November)]
    [InlineData(Month.December)]
    public void ToLocalizedAbbreviation_With_Valid_Month_Should_Return_Localized_Abbreviation(Month month)
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act
        var result = MonthHelper.ToLocalizedAbbreviation(month, culture);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(culture.DateTimeFormat.GetAbbreviatedMonthName((int)month));
    }

    [Fact]
    public void ToLocalizedAbbreviation_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.ToLocalizedAbbreviation(invalidMonth);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("month");
    }

    [Theory]
    [InlineData(Month.January, "Jan")]
    [InlineData(Month.February, "Feb")]
    [InlineData(Month.March, "Mar")]
    [InlineData(Month.April, "Apr")]
    [InlineData(Month.May, "May")]
    [InlineData(Month.June, "Jun")]
    [InlineData(Month.July, "Jul")]
    [InlineData(Month.August, "Aug")]
    [InlineData(Month.September, "Sep")]
    [InlineData(Month.October, "Oct")]
    [InlineData(Month.November, "Nov")]
    [InlineData(Month.December, "Dec")]
    public void ToEnglishAbbreviation_With_Valid_Month_Should_Return_English_Abbreviation(Month month, string expected)
    {
        // Act
        var result = MonthHelper.ToEnglishAbbreviation(month);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToEnglishAbbreviation_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.ToEnglishAbbreviation(invalidMonth);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(Month.January, "Jan")]
    [InlineData(Month.February, "Feb")]
    [InlineData(Month.March, "Mär")]
    [InlineData(Month.April, "Apr")]
    [InlineData(Month.May, "Mai")]
    [InlineData(Month.June, "Jun")]
    [InlineData(Month.July, "Jul")]
    [InlineData(Month.August, "Aug")]
    [InlineData(Month.September, "Sep")]
    [InlineData(Month.October, "Okt")]
    [InlineData(Month.November, "Nov")]
    [InlineData(Month.December, "Dez")]
    public void ToGermanAbbreviation_With_Valid_Month_Should_Return_German_Abbreviation(Month month, string expected)
    {
        // Act
        var result = MonthHelper.ToGermanAbbreviation(month);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToGermanAbbreviation_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.ToGermanAbbreviation(invalidMonth);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(Month.January, "January")]
    [InlineData(Month.February, "February")]
    [InlineData(Month.March, "March")]
    [InlineData(Month.April, "April")]
    [InlineData(Month.May, "May")]
    [InlineData(Month.June, "June")]
    [InlineData(Month.July, "July")]
    [InlineData(Month.August, "August")]
    [InlineData(Month.September, "September")]
    [InlineData(Month.October, "October")]
    [InlineData(Month.November, "November")]
    [InlineData(Month.December, "December")]
    public void ToEnglishName_With_Valid_Month_Should_Return_English_Name(Month month, string expected)
    {
        // Act
        var result = MonthHelper.ToEnglishName(month);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToEnglishName_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.ToEnglishName(invalidMonth);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(Month.January, "Januar")]
    [InlineData(Month.February, "Februar")]
    [InlineData(Month.March, "März")]
    [InlineData(Month.April, "April")]
    [InlineData(Month.May, "Mai")]
    [InlineData(Month.June, "Juni")]
    [InlineData(Month.July, "Juli")]
    [InlineData(Month.August, "August")]
    [InlineData(Month.September, "September")]
    [InlineData(Month.October, "Oktober")]
    [InlineData(Month.November, "November")]
    [InlineData(Month.December, "Dezember")]
    public void ToGermanName_With_Valid_Month_Should_Return_German_Name(Month month, string expected)
    {
        // Act
        var result = MonthHelper.ToGermanName(month);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToGermanName_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.ToGermanName(invalidMonth);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToDateTime_With_Valid_Month_And_Year_Should_Return_First_Day_Of_Month()
    {
        // Arrange
        const int year = 2025;

        // Act
        var result = MonthHelper.ToDateTime(Month.March, year);

        // Assert
        result.Should().Be(new DateTime(2025, 3, 1));
        result.Year.Should().Be(year);
        result.Month.Should().Be(3);
        result.Day.Should().Be(1);
    }

    [Fact]
    public void ToDateOnly_With_Valid_Month_And_Year_Should_Return_First_Day_Of_Month()
    {
        // Arrange
        const int year = 2025;

        // Act
        var result = MonthHelper.ToDateOnly(Month.March, year);

        // Assert
        result.Should().Be(new DateOnly(2025, 3, 1));
        result.Year.Should().Be(year);
        result.Month.Should().Be(3);
        result.Day.Should().Be(1);
    }

    [Fact]
    public void GetAllMonths_Should_Return_All_Twelve_Months()
    {
        // Act
        var result = MonthHelper.GetAllMonths().ToList();

        // Assert
        result.Should().HaveCount(12);
        result.Should().Contain(Month.January);
        result.Should().Contain(Month.February);
        result.Should().Contain(Month.March);
        result.Should().Contain(Month.April);
        result.Should().Contain(Month.May);
        result.Should().Contain(Month.June);
        result.Should().Contain(Month.July);
        result.Should().Contain(Month.August);
        result.Should().Contain(Month.September);
        result.Should().Contain(Month.October);
        result.Should().Contain(Month.November);
        result.Should().Contain(Month.December);
    }

    [Theory]
    [InlineData(Month.January, 2025, 31)]
    [InlineData(Month.February, 2025, 28)]
    [InlineData(Month.February, 2024, 29)] // Leap year
    [InlineData(Month.March, 2025, 31)]
    [InlineData(Month.April, 2025, 30)]
    [InlineData(Month.May, 2025, 31)]
    [InlineData(Month.June, 2025, 30)]
    [InlineData(Month.July, 2025, 31)]
    [InlineData(Month.August, 2025, 31)]
    [InlineData(Month.September, 2025, 30)]
    [InlineData(Month.October, 2025, 31)]
    [InlineData(Month.November, 2025, 30)]
    [InlineData(Month.December, 2025, 31)]
    public void GetDaysInMonth_With_Valid_Month_And_Year_Should_Return_Correct_Days(Month month, int year, int expectedDays)
    {
        // Act
        var result = MonthHelper.GetDaysInMonth(month, year);

        // Assert
        result.Should().Be(expectedDays);
    }

    [Fact]
    public void GetDaysInMonth_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.GetDaysInMonth(invalidMonth, 2025);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("month");
    }

    [Theory]
    [InlineData(2024, true)]  // Leap year
    [InlineData(2025, false)] // Not leap year
    [InlineData(2000, true)]  // Leap year (divisible by 400)
    [InlineData(1900, false)] // Not leap year (divisible by 100 but not 400)
    public void IsLeapYear_Should_Return_Correct_Result(int year, bool expected)
    {
        // Act
        var result = MonthHelper.IsLeapYear(year);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Quarter Methods Tests

    [Theory]
    [InlineData(Month.January, 1)]
    [InlineData(Month.February, 1)]
    [InlineData(Month.March, 1)]
    [InlineData(Month.April, 2)]
    [InlineData(Month.May, 2)]
    [InlineData(Month.June, 2)]
    [InlineData(Month.July, 3)]
    [InlineData(Month.August, 3)]
    [InlineData(Month.September, 3)]
    [InlineData(Month.October, 4)]
    [InlineData(Month.November, 4)]
    [InlineData(Month.December, 4)]
    public void GetQuarter_With_Valid_Month_Should_Return_Correct_Quarter(Month month, int expectedQuarter)
    {
        // Act
        var result = MonthHelper.GetQuarter(month);

        // Assert
        result.Should().Be(expectedQuarter);
    }

    [Fact]
    public void GetQuarter_With_Invalid_Month_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMonth = (Month)13;

        // Act & Assert
        var action = () => MonthHelper.GetQuarter(invalidMonth);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("month");
    }

    [Theory]
    [InlineData(1, new[] { Month.January, Month.February, Month.March })]
    [InlineData(2, new[] { Month.April, Month.May, Month.June })]
    [InlineData(3, new[] { Month.July, Month.August, Month.September })]
    [InlineData(4, new[] { Month.October, Month.November, Month.December })]
    public void GetMonthsInQuarter_With_Valid_Quarter_Should_Return_Correct_Months(int quarter, Month[] expectedMonths)
    {
        // Act
        var result = MonthHelper.GetMonthsInQuarter(quarter).ToArray();

        // Assert
        result.Should().BeEquivalentTo(expectedMonths);
        result.Should().HaveCount(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void GetMonthsInQuarter_With_Invalid_Quarter_Should_Throw_ArgumentOutOfRangeException(int invalidQuarter)
    {
        // Act & Assert
        var action = () => MonthHelper.GetMonthsInQuarter(invalidQuarter).ToList();
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Month Extension Methods Tests

    [Theory]
    [InlineData(Month.January, Month.February)]
    [InlineData(Month.February, Month.March)]
    [InlineData(Month.March, Month.April)]
    [InlineData(Month.April, Month.May)]
    [InlineData(Month.May, Month.June)]
    [InlineData(Month.June, Month.July)]
    [InlineData(Month.July, Month.August)]
    [InlineData(Month.August, Month.September)]
    [InlineData(Month.September, Month.October)]
    [InlineData(Month.October, Month.November)]
    [InlineData(Month.November, Month.December)]
    [InlineData(Month.December, Month.January)]
    public void Next_Should_Return_Next_Month_With_Wrapping(Month current, Month expected)
    {
        // Act
        var result = current.Next();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Month.February, Month.January)]
    [InlineData(Month.March, Month.February)]
    [InlineData(Month.April, Month.March)]
    [InlineData(Month.May, Month.April)]
    [InlineData(Month.June, Month.May)]
    [InlineData(Month.July, Month.June)]
    [InlineData(Month.August, Month.July)]
    [InlineData(Month.September, Month.August)]
    [InlineData(Month.October, Month.September)]
    [InlineData(Month.November, Month.October)]
    [InlineData(Month.December, Month.November)]
    [InlineData(Month.January, Month.December)]
    public void Previous_Should_Return_Previous_Month_With_Wrapping(Month current, Month expected)
    {
        // Act
        var result = current.Previous();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Month.January, true)]
    [InlineData(Month.February, true)]
    [InlineData(Month.March, true)]
    [InlineData(Month.April, true)]
    [InlineData(Month.May, true)]
    [InlineData(Month.June, true)]
    [InlineData(Month.July, false)]
    [InlineData(Month.August, false)]
    [InlineData(Month.September, false)]
    [InlineData(Month.October, false)]
    [InlineData(Month.November, false)]
    [InlineData(Month.December, false)]
    public void IsFirstHalfOfYear_Should_Return_Correct_Result(Month month, bool expected)
    {
        // Act
        var result = month.IsFirstHalfOfYear();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Month.January, false)]
    [InlineData(Month.February, false)]
    [InlineData(Month.March, false)]
    [InlineData(Month.April, false)]
    [InlineData(Month.May, false)]
    [InlineData(Month.June, false)]
    [InlineData(Month.July, true)]
    [InlineData(Month.August, true)]
    [InlineData(Month.September, true)]
    [InlineData(Month.October, true)]
    [InlineData(Month.November, true)]
    [InlineData(Month.December, true)]
    public void IsSecondHalfOfYear_Should_Return_Correct_Result(Month month, bool expected)
    {
        // Act
        var result = month.IsSecondHalfOfYear();

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Integration and Edge Case Tests

    [Fact]
    public void ParseMonth_And_TryParseMonth_Should_Return_Same_Results_For_Valid_Input()
    {
        // Arrange
        var testCases = new[]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12",
            "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec",
            "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC",
            "mär", "mai", "okt", "dez"
        };

        foreach (var testCase in testCases)
        {
            // Act
            var parseResult = MonthHelper.ParseMonth(testCase);
            var tryParseResult = MonthHelper.TryParseMonth(testCase, out Month month);

            // Assert
            tryParseResult.Should().BeTrue();
            month.Should().Be(parseResult);
        }
    }

    [Fact]
    public void TryParseMonth_Should_Return_False_For_Invalid_Input_While_ParseMonth_Throws()
    {
        // Arrange
        var invalidInputs = new[] { "invalid", "xyz", "123abc", "ab", "", "   ", "0", "13" };

        foreach (var invalidInput in invalidInputs)
        {
            // Act & Assert
            var tryParseResult = MonthHelper.TryParseMonth(invalidInput, out Month month);
            tryParseResult.Should().BeFalse();
            month.Should().Be(default(Month));

            if (!string.IsNullOrWhiteSpace(invalidInput))
            {
                var parseAction = () => MonthHelper.ParseMonth(invalidInput);
                parseAction.Should().Throw<ArgumentException>();
            }
        }
    }

    [Fact]
    public void Methods_Should_Handle_Unicode_Characters_In_Culture_Names()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act & Assert - German months with umlauts should work
        MonthHelper.TryParseMonth("März", out var march, culture).Should().BeTrue();
        march.Should().Be(Month.March);
    }

    [Fact]
    public void Methods_Should_Be_Case_Insensitive_For_All_Supported_Formats()
    {
        // Arrange
        var testCases = new[]
        {
            ("january", "JANUARY", "January", "JaNuArY"),
            ("jan", "JAN", "Jan", "jAn"),
            ("mär", "MÄR", "Mär", "MäR")
        };

        foreach (var (lower, upper, proper, mixed) in testCases)
        {
            // Act
            var lowerResult = MonthHelper.TryParseMonth(lower, out var lowerMonth);
            var upperResult = MonthHelper.TryParseMonth(upper, out var upperMonth);
            var properResult = MonthHelper.TryParseMonth(proper, out var properMonth);
            var mixedResult = MonthHelper.TryParseMonth(mixed, out var mixedMonth);

            // Assert
            lowerResult.Should().BeTrue();
            upperResult.Should().BeTrue();
            properResult.Should().BeTrue();
            mixedResult.Should().BeTrue();

            upperMonth.Should().Be(lowerMonth);
            properMonth.Should().Be(lowerMonth);
            mixedMonth.Should().Be(lowerMonth);
        }
    }

    [Fact]
    public void Methods_Should_Handle_Various_Whitespace_Characters()
    {
        // Arrange
        var whitespaceVariations = new[]
        {
            " jan ",
            "\tjan\t",
            "\njan\n",
            "\r\njan\r\n",
            "  \t\n jan \n\t  "
        };

        foreach (var variation in whitespaceVariations)
        {
            // Act
            var result = MonthHelper.TryParseMonth(variation, out var month);

            // Assert
            result.Should().BeTrue();
            month.Should().Be(Month.January);
        }
    }

    [Fact]
    public void Methods_Should_Prioritize_English_Over_German_Abbreviations()
    {
        // Arrange - "mar" exists in both English and German dictionaries
        string ambiguousAbbreviation = "mar";

        // Act
        var result = MonthHelper.TryParseMonth(ambiguousAbbreviation, out Month month);

        // Assert
        result.Should().BeTrue();
        month.Should().Be(Month.March); // Should match English abbreviation
    }

    [Fact]
    public void Static_Culture_Properties_Should_Be_Correctly_Initialized()
    {
        // Act & Assert
        MonthHelper.EnglishCulture.Name.Should().Be("en-US");
        MonthHelper.GermanCulture.Name.Should().Be("de-DE");
    }

    #endregion

    #region Performance and Consistency Tests

    [Fact]
    public void Methods_Should_Be_Consistent_Across_Multiple_Calls()
    {
        // Arrange
        const int iterations = 1000;
        const string testInput = "jan";
        var expectedMonth = Month.January;

        // Act & Assert
        for (int i = 0; i < iterations; i++)
        {
            var parseResult = MonthHelper.ParseMonth(testInput);
            var tryParseResult = MonthHelper.TryParseMonth(testInput, out var month);

            parseResult.Should().Be(expectedMonth);
            tryParseResult.Should().BeTrue();
            month.Should().Be(expectedMonth);
        }
    }

    [Fact]
    public void Methods_Should_Handle_Concurrent_Access()
    {
        // Arrange
        const int threadCount = 10;
        const int operationsPerThread = 100;
        var testData = new[] { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        var tasks = new List<Task>();

        // Act
        for (int t = 0; t < threadCount; t++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int i = 0; i < operationsPerThread; i++)
                {
                    var testInput = testData[i % testData.Length];
                    var parseResult = MonthHelper.ParseMonth(testInput);
                    var tryParseResult = MonthHelper.TryParseMonth(testInput, out var month);

                    // Basic consistency check
                    tryParseResult.Should().BeTrue();
                    month.Should().Be(parseResult);
                }
            }));
        }

        // Assert
        var aggregateException = Record.Exception(() => Task.WaitAll(tasks.ToArray()));
        aggregateException.Should().BeNull();
    }

    [Fact]
    public void Culture_Related_Methods_Should_Handle_Culture_Changes()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            // Set to English culture
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var englishResult = MonthHelper.ParseMonth("January");

            // Set to German culture  
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            var germanResult = MonthHelper.ParseMonth("Januar");

            // Assert
            englishResult.Should().Be(Month.January);
            germanResult.Should().Be(Month.January);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    #endregion
}