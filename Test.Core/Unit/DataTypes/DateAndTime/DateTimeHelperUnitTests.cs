//--------------------------------------------------------------------------
// File:    DateTimeHelperUnitTest.cs
// Content: Unit tests for DateTimeHelper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Data.SqlTypes;

namespace AnBo.Test.Unit;

[Trait("Category", "Unit")]
public class DateTimeHelperUnitTests
{
    #region Test Data and Constants

    private static readonly DateTime TestDateTime = new(2025, 7, 23, 14, 30, 45, 123);
    private static readonly DateTime TestDate = new(2025, 7, 23);
    private const string ValidDateTimeString = "2025-07-23T14:30:45.123";
    private const string ValidDateString = "2025-07-23";
    private const string FileSortableDateTime = "20250723T143045123";
    private const string FileSortableDateTimeNoMs = "20250723T143045";
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    #endregion

    #region Create Method Tests

    [Fact]
    public void Create_With_Valid_DateTime_String_Should_Return_Correct_DateTime()
    {
        // Act
        var result = DateTimeHelper.Create(ValidDateTimeString);

        // Assert
        result.Should().Be(TestDateTime);
    }

    [Fact]
    public void Create_With_Valid_Date_String_Should_Return_Date_Only()
    {
        // Act
        var result = DateTimeHelper.Create(ValidDateString);

        // Assert
        result.Date.Should().Be(TestDate);
    }

    [Fact]
    public void Create_With_Various_DateTime_Formats_Should_Parse_Correctly()
    {
        // Arrange
        var testCases = new[]
        {
            ("2025-07-23T14:30:45.123", new DateTime(2025, 7, 23, 14, 30, 45, 123)),
            ("20250723T143045123", new DateTime(2025, 7, 23, 14, 30, 45, 123)),
            ("2025-07-23T14:30:45", new DateTime(2025, 7, 23, 14, 30, 45)),
            ("20250723T143045", new DateTime(2025, 7, 23, 14, 30, 45)),
            ("20250723143045", new DateTime(2025, 7, 23, 14, 30, 45))
        };

        foreach (var (input, expected) in testCases)
        {
            // Act
            var result = DateTimeHelper.Create(input);

            // Assert
            result.Should().Be(expected, $"Input: {input}");
        }
    }

    [Fact]
    public void Create_With_Various_Date_Formats_Should_Parse_Correctly()
    {
        // Arrange
        var testCases = new[]
        {
            ("2025-07-23", new DateTime(2025, 7, 23)),
            ("23.07.2025", new DateTime(2025, 7, 23)),
            ("07/23/2025", new DateTime(2025, 7, 23)),
            ("23.07.25", new DateTime(2025, 7, 23)),
            ("23Jul25", new DateTime(2025, 7, 23)),
            ("23Jul2025", new DateTime(2025, 7, 23))
        };

        foreach (var (input, expected) in testCases)
        {
            // Act
            var result = DateTimeHelper.Create(input);

            // Assert
            result.Date.Should().Be(expected.Date, $"Input: {input}");
        }
    }

    [Fact]
    public void Create_With_Null_Should_Throw_ArgumentException()
    {
        // Arrange
        string? nullValue = null;

        // Act & Assert
        var action = () => DateTimeHelper.Create(nullValue!);
        action.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void Create_With_Empty_String_Should_Throw_ArgumentException()
    {
        // Arrange
        string emptyValue = string.Empty;

        // Act & Assert
        var action = () => DateTimeHelper.Create(emptyValue);
        action.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void Create_With_Whitespace_Should_Throw_ArgumentException()
    {
        // Arrange
        string whitespaceValue = "   ";

        // Act & Assert
        var action = () => DateTimeHelper.Create(whitespaceValue);
        action.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void Create_With_Invalid_Format_Should_Throw_ArgumentException()
    {
        // Arrange
        string invalidValue = "invalid-datetime";

        // Act & Assert
        var action = () => DateTimeHelper.Create(invalidValue);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("value")
            .WithMessage("*Unable to parse DateTime from value*");
    }

    #endregion

    #region TryCreate Method Tests

    [Fact]
    public void TryCreate_With_Valid_DateTime_String_Should_Return_True_And_Correct_DateTime()
    {
        // Act
        var success = DateTimeHelper.TryCreate(ValidDateTimeString, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TestDateTime);
    }

    [Fact]
    public void TryCreate_With_Valid_Date_String_Should_Return_True_And_Date_Only()
    {
        // Act
        var success = DateTimeHelper.TryCreate(ValidDateString, out var result);

        // Assert
        success.Should().BeTrue();
        result.Date.Should().Be(TestDate);
    }

    [Fact]
    public void TryCreate_With_Null_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryCreate(null, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryCreate_With_Empty_String_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryCreate(string.Empty, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryCreate_With_Whitespace_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryCreate("   ", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryCreate_With_Invalid_Format_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryCreate("invalid-datetime", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryCreate_With_Trimmed_Input_Should_Work_Correctly()
    {
        // Arrange
        string paddedInput = $"  {ValidDateTimeString}  ";

        // Act
        var success = DateTimeHelper.TryCreate(paddedInput, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TestDateTime);
    }

    #endregion

    #region CreateWithDayPart Method Tests

    [Fact]
    public void CreateWithDayPart_With_BeginOfDay_Should_Return_Midnight()
    {
        // Act
        var result = DateTimeHelper.CreateWithDayPart(ValidDateString, DateTimeHelper.DayPart.BeginOfDay);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 0, 0, 0));
    }

    [Fact]
    public void CreateWithDayPart_With_HighNoon_Should_Return_Noon()
    {
        // Act
        var result = DateTimeHelper.CreateWithDayPart(ValidDateString, DateTimeHelper.DayPart.HighNoon);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 12, 0, 0));
    }

    [Fact]
    public void CreateWithDayPart_With_EndOfDay_Should_Return_End_Of_Day()
    {
        // Act
        var result = DateTimeHelper.CreateWithDayPart(ValidDateString, DateTimeHelper.DayPart.EndOfDay);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 23, 59, 59, 999).AddTicks(9999));
    }

    [Fact]
    public void CreateWithDayPart_With_Null_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.CreateWithDayPart(null!, DateTimeHelper.DayPart.BeginOfDay);
        action.Should().Throw<ArgumentException>().WithParameterName("dateString");
    }

    [Fact]
    public void CreateWithDayPart_With_Invalid_Date_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.CreateWithDayPart("invalid-date", DateTimeHelper.DayPart.BeginOfDay);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("dateString")
            .WithMessage("*Unable to parse date from value*");
    }

    #endregion

    #region TryCreateWithDayPart Method Tests

    [Fact]
    public void TryCreateWithDayPart_With_Valid_Date_Should_Return_True_And_Correct_Time()
    {
        // Act
        var success = DateTimeHelper.TryCreateWithDayPart(ValidDateString, DateTimeHelper.DayPart.HighNoon, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(new DateTime(2025, 7, 23, 12, 0, 0));
    }

    [Fact]
    public void TryCreateWithDayPart_With_Invalid_Date_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryCreateWithDayPart("invalid-date", DateTimeHelper.DayPart.BeginOfDay, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryCreateWithDayPart_With_Null_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryCreateWithDayPart(null, DateTimeHelper.DayPart.BeginOfDay, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    #endregion

    #region ToIso8601DateTime Method Tests

    [Fact]
    public void ToIso8601DateTime_Should_Return_ISO8601_Format()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 125, DateTimeKind.Utc);

        // Act
        var result = DateTimeHelper.ToIso8601DateTime(dateTime);

        // Assert
        result.Should().Be("2025-07-23T14:30:45.125Z");
    }

    [Fact]
    public void ToIso8601DateTime_With_Local_Time_Should_Convert_To_UTC()
    {
        // Arrange
        var localDateTime = new DateTime(2025, 7, 23, 14, 30, 45, 125, DateTimeKind.Local);

        // Act
        var result = DateTimeHelper.ToIso8601DateTime(localDateTime);

        // Assert
        result.Should().EndWith("Z");
        result.Should().StartWith("2025-07-23T");
    }

    #endregion

    #region ToSqlServerPrecision Method Tests

    [Fact]
    public void ToSqlServerPrecision_Should_Round_To_SqlDateTime_Precision()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 125); 

        // Act
        var result = DateTimeHelper.ToSqlServerPrecision(dateTime); // Rounds milliseconds to nearest .000 .003 .007

        // Assert
        var expected = new SqlDateTime(dateTime).Value;
        result.Should().Be(expected);
        result.Should().NotBe(dateTime); // Should be different due to precision rounding
    }

    [Fact]
    public void ToSqlServerPrecision_Should_Throw_On_DateTime_MinValue()
    {
        // Arrange
        var minDateTime = DateTime.MinValue;

        // Act & Assert - Should not throw
        var action1 = () => DateTimeHelper.ToSqlServerPrecision(minDateTime);

        action1.Should().Throw<SqlTypeException>();
    }

    [Fact]
    public void ToSqlServerPrecision_Should_Handle_Edge_Cases()
    {
        // Arrange
        var minDateTime = new DateTime(1753, 1, 1, 12, 0, 0);
        var maxDateTime = DateTime.MaxValue;

        // Act & Assert - Should not throw
        var action1 = () => DateTimeHelper.ToSqlServerPrecision(minDateTime);
        var action2 = () => DateTimeHelper.ToSqlServerPrecision(maxDateTime);

        action1.Should().NotThrow();
        action2.Should().NotThrow();
    }

    #endregion

    #region ToFileSortableString Method Tests

    [Fact]
    public void ToFileSortableString_Without_Milliseconds_Should_Return_Correct_Format()
    {
        // Act
        var result = DateTimeHelper.ToFileSortableString(TestDateTime, false);

        // Assert
        result.Should().Be("20250723T143045");
    }

    [Fact]
    public void ToFileSortableString_With_Milliseconds_Should_Return_Correct_Format()
    {
        // Act
        var result = DateTimeHelper.ToFileSortableString(TestDateTime, true);

        // Assert
        result.Should().Be("20250723T143045123");
    }

    [Fact]
    public void ToFileSortableString_Default_Should_Not_Include_Milliseconds()
    {
        // Act
        var result = DateTimeHelper.ToFileSortableString(TestDateTime);

        // Assert
        result.Should().Be("20250723T143045");
        result.Length.Should().Be(15);
    }

    [Fact]
    public void ToFileSortableString_Should_Be_Sortable()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            new DateTime(2024, 6, 15),
            new DateTime(2026, 3, 20)
        };

        // Act
        var sortedStrings = dates.Select(d => DateTimeHelper.ToFileSortableString(d)).OrderBy(s => s).ToArray();
        var expectedOrder = dates.OrderBy(d => d).Select(d => DateTimeHelper.ToFileSortableString(d)).ToArray();

        // Assert
        sortedStrings.Should().BeEquivalentTo(expectedOrder, options => options.WithStrictOrdering());
    }

    #endregion

    #region FromFileSortableString Method Tests

    [Fact]
    public void FromFileSortableString_With_Milliseconds_Should_Parse_Correctly()
    {
        // Act
        var result = DateTimeHelper.FromFileSortableString(FileSortableDateTime);

        // Assert
        result.Should().Be(TestDateTime);
    }

    [Fact]
    public void FromFileSortableString_Without_Milliseconds_Should_Parse_Correctly()
    {
        // Act
        var result = DateTimeHelper.FromFileSortableString(FileSortableDateTimeNoMs);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 14, 30, 45));
    }

    [Fact]
    public void FromFileSortableString_With_Null_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.FromFileSortableString(null!);
        action.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void FromFileSortableString_With_Invalid_Format_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.FromFileSortableString("invalid-format");
        action.Should().Throw<ArgumentException>()
            .WithParameterName("value")
            .WithMessage("*Invalid file-sortable DateTime format*");
    }

    [Fact]
    public void FromFileSortableString_Roundtrip_Should_Preserve_DateTime()
    {
        // Arrange
        var original = TestDateTime;

        // Act
        var sortableString = DateTimeHelper.ToFileSortableString(original, true);
        var roundtrip = DateTimeHelper.FromFileSortableString(sortableString);

        // Assert
        roundtrip.Should().Be(original);
    }

    #endregion

    #region TryFromFileSortableString Method Tests

    [Fact]
    public void TryFromFileSortableString_With_Valid_Format_Should_Return_True()
    {
        // Act
        var success = DateTimeHelper.TryFromFileSortableString(FileSortableDateTime, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TestDateTime);
    }

    [Fact]
    public void TryFromFileSortableString_With_Invalid_Format_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryFromFileSortableString("invalid-format", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryFromFileSortableString_With_Wrong_Length_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryFromFileSortableString("20250723T14", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryFromFileSortableString_With_Null_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryFromFileSortableString(null, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void TryFromFileSortableString_With_Whitespace_Should_Handle_Trimming()
    {
        // Arrange
        var paddedInput = $"  {FileSortableDateTime}  ";

        // Act
        var success = DateTimeHelper.TryFromFileSortableString(paddedInput, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TestDateTime);
    }

    #endregion

    #region DOS DateTime Conversion Tests

    [Fact]
    public void FromDosDateTime_With_Valid_Values_Should_Convert_Correctly()
    {
        // Arrange
        ushort dosDate = 0x5AF7; // July 23, 2025 (encoded)
        ushort dosTime = 0x734B; // 14:30:22 (encoded, seconds are divided by 2)

        // Act
        var result = DateTimeHelper.FromDosDateTime(dosDate, dosTime);

        // Assert
        result.Year.Should().Be(2025);
        result.Month.Should().Be(7);
        result.Day.Should().Be(23);
        result.Hour.Should().Be(14);
        result.Minute.Should().Be(26);
        result.Second.Should().Be(22);
    }

    [Fact]
    public void FromDosDateTime_With_Invalid_Date_Should_Return_Default_Date()
    {
        // Arrange
        ushort dosDate = 0xFFFF; // Invalid date
        ushort dosTime = 0x0000;

        // Act
        var result = DateTimeHelper.FromDosDateTime(dosDate, dosTime);

        // Assert
        result.Should().Be(new DateTime(1980, 1, 1));
    }

    [Fact]
    public void FromDosDateTime_With_Invalid_Time_Should_Return_Midnight()
    {
        // Arrange
        ushort dosDate = 0x32F7; // Valid date
        ushort dosTime = 0xFFFF; // Invalid time

        // Act
        var result = DateTimeHelper.FromDosDateTime(dosDate, dosTime);

        // Assert
        result.Hour.Should().Be(0);
        result.Minute.Should().Be(0);
        result.Second.Should().Be(0);
    }

    [Fact]
    public void FromDosDateTime_With_Zero_Date_Should_Return_Default_Date()
    {
        // Arrange
        ushort dosDate = 0x0000; // Zero date (invalid)
        ushort dosTime = 0x0000;

        // Act
        var result = DateTimeHelper.FromDosDateTime(dosDate, dosTime);

        // Assert
        result.Should().Be(new DateTime(1980, 1, 1));
    }

    [Fact]
    public void FromDosDateTime_With_Combined_Timestamp_Should_Work_Correctly()
    {
        // Arrange
        uint dosTimestamp = 0x5AF7734B; // 0x32F7734B; // Combined date and time

        // Act
        var result = DateTimeHelper.FromDosDateTime(dosTimestamp);

        // Assert
        result.Year.Should().Be(2025);
        result.Month.Should().Be(7);
        result.Day.Should().Be(23);
    }

    [Fact]
    public void FromDosDateTime_Should_Handle_Exception_Gracefully()
    {
        // Arrange - Values that might cause DateTime constructor to throw
        ushort dosDate = 0x7FFF; // Max year + invalid month/day
        ushort dosTime = 0x7FFF; // Max values

        // Act
        var result = DateTimeHelper.FromDosDateTime(dosDate, dosTime);

        // Assert
        result.Should().Be(new DateTime(1980, 1, 1)); // Fallback value
    }

    #endregion

    #region Unix Timestamp Conversion Tests

    [Fact]
    public void ToUnixTimestamp_Should_Convert_DateTime_To_Unix_Seconds()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, DateTimeKind.Utc);

        // Act
        var result = DateTimeHelper.ToUnixTimestamp(dateTime);

        // Assert
        result.Should().BeGreaterThan(0);
        // July 23, 2025 should be well into the future from Unix epoch
        result.Should().BeGreaterThan(1753142400); // Approximately 2025
    }

    [Fact]
    public void FromUnixTimestamp_Should_Convert_Unix_Seconds_To_DateTime()
    {
        // Arrange
        long unixTimestamp = 1753142445; // Some future timestamp

        // Act
        var result = DateTimeHelper.FromUnixTimestamp(unixTimestamp);

        // Assert
        result.Should().BeAfter(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void Unix_Timestamp_Roundtrip_Should_Preserve_DateTime()
    {
        // Arrange
        var original = new DateTime(2025, 7, 23, 14, 30, 45, DateTimeKind.Utc);

        // Act
        var timestamp = DateTimeHelper.ToUnixTimestamp(original);
        var roundtrip = DateTimeHelper.FromUnixTimestamp(timestamp);

        // Assert
        roundtrip.Should().BeCloseTo(original.ToLocalTime(), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ToUnixTimestampMilliseconds_Should_Convert_DateTime_To_Unix_Milliseconds()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 123, DateTimeKind.Utc);

        // Act
        var result = DateTimeHelper.ToUnixTimestampMilliseconds(dateTime);

        // Assert
        result.Should().BeGreaterThan(0);
        // Should be 1000 times larger than seconds timestamp
        var secondsTimestamp = DateTimeHelper.ToUnixTimestamp(dateTime);
        result.Should().BeGreaterThan(secondsTimestamp * 1000);
    }

    [Fact]
    public void FromUnixTimestampMilliseconds_Should_Convert_Unix_Milliseconds_To_DateTime()
    {
        // Arrange
        long unixTimestampMs = 1753142445123; // Some future timestamp with milliseconds

        // Act
        var result = DateTimeHelper.FromUnixTimestampMilliseconds(unixTimestampMs);

        // Assert
        result.Should().BeAfter(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void Unix_Timestamp_Milliseconds_Roundtrip_Should_Preserve_DateTime()
    {
        // Arrange
        var original = new DateTime(2025, 7, 23, 14, 30, 45, 123, DateTimeKind.Utc);

        // Act
        var timestamp = DateTimeHelper.ToUnixTimestampMilliseconds(original);
        var roundtrip = DateTimeHelper.FromUnixTimestampMilliseconds(timestamp);

        // Assert
        roundtrip.Should().BeCloseTo(original.ToLocalTime(), TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void Unix_Epoch_Should_Return_Zero_Timestamp()
    {
        // Arrange
        var epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = DateTimeHelper.ToUnixTimestamp(epochUtc);

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region TimeSpan Helper Methods Tests

    [Fact]
    public void Abs_With_Positive_TimeSpan_Should_Return_Same_Value()
    {
        // Arrange
        var timeSpan = TimeSpan.FromHours(5);

        // Act
        var result = DateTimeHelper.Abs(timeSpan);

        // Assert
        result.Should().Be(timeSpan);
    }

    [Fact]
    public void Abs_With_Negative_TimeSpan_Should_Return_Positive_Value()
    {
        // Arrange
        var timeSpan = TimeSpan.FromHours(-5);

        // Act
        var result = DateTimeHelper.Abs(timeSpan);

        // Assert
        result.Should().Be(TimeSpan.FromHours(5));
    }

    [Fact]
    public void Abs_With_Zero_Should_Return_Zero()
    {
        // Act
        var result = DateTimeHelper.Abs(TimeSpan.Zero);

        // Assert
        result.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void IsNegative_With_Negative_TimeSpan_Should_Return_True()
    {
        // Arrange
        var timeSpan = TimeSpan.FromMinutes(-30);

        // Act
        var result = DateTimeHelper.IsNegative(timeSpan);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNegative_With_Positive_TimeSpan_Should_Return_False()
    {
        // Arrange
        var timeSpan = TimeSpan.FromMinutes(30);

        // Act
        var result = DateTimeHelper.IsNegative(timeSpan);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsNegative_With_Zero_Should_Return_False()
    {
        // Act
        var result = DateTimeHelper.IsNegative(TimeSpan.Zero);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ConvertTimeSpanToDouble Method Tests

    [Fact]
    public void ConvertTimeSpanToDouble_With_TimeSpan_Object_Should_Return_TotalMilliseconds()
    {
        // Arrange
        var timeSpan = TimeSpan.FromMinutes(5);

        // Act
        var result = DateTimeHelper.ConvertTimeSpanToDouble(timeSpan);

        // Assert
        result.Should().Be(timeSpan.TotalMilliseconds);
        result.Should().Be(300000); // 5 minutes = 300,000 milliseconds
    }

    [Fact]
    public void ConvertTimeSpanToDouble_With_String_Should_Parse_And_Return_TotalMilliseconds()
    {
        // Arrange
        string timeSpanString = "00:05:00"; // 5 minutes

        // Act
        var result = DateTimeHelper.ConvertTimeSpanToDouble(timeSpanString);

        // Assert
        result.Should().Be(300000); // 5 minutes = 300,000 milliseconds
    }

    [Fact]
    public void ConvertTimeSpanToDouble_With_Zero_Should_Return_Zero()
    {
        // Act
        var result = DateTimeHelper.ConvertTimeSpanToDouble(TimeSpan.Zero);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ConvertTimeSpanToDouble_With_Negative_TimeSpan_Should_Return_Negative_Value()
    {
        // Arrange
        var timeSpan = TimeSpan.FromMinutes(-5);

        // Act
        var result = DateTimeHelper.ConvertTimeSpanToDouble(timeSpan);

        // Assert
        result.Should().Be(-300000);
    }

    #endregion

    #region ParseTimeSpan Method Tests

    [Fact]
    public void ParseTimeSpan_With_Valid_TimeSpan_String_Should_Parse_Correctly()
    {
        // Arrange
        string timeSpanString = "01:30:45"; // 1 hour, 30 minutes, 45 seconds

        // Act
        var result = DateTimeHelper.ParseTimeSpan(timeSpanString);

        // Assert
        result.Should().Be(new TimeSpan(1, 30, 45));
    }

    [Fact]
    public void ParseTimeSpan_With_Component_None_Should_Parse_As_Is()
    {
        // Arrange
        string timeSpanString = "02:15:30";

        // Act
        var result = DateTimeHelper.ParseTimeSpan(timeSpanString, DateTimeHelper.TimeSpanComponent.None);

        // Assert
        result.Should().Be(new TimeSpan(2, 15, 30));
    }

    [Fact]
    public void ParseTimeSpan_With_Component_Hours_Should_Assume_Hours()
    {
        // Arrange
        string timeSpanString = "5"; // Should be interpreted as 5 hours

        // Act
        var result = DateTimeHelper.ParseTimeSpan(timeSpanString, DateTimeHelper.TimeSpanComponent.Hours);

        // Assert
        result.Should().Be(TimeSpan.FromHours(5));
    }

    [Fact]
    public void ParseTimeSpan_With_Component_Minutes_Should_Assume_Minutes()
    {
        // Arrange
        string timeSpanString = "30"; // Should be interpreted as 30 minutes

        // Act
        var result = DateTimeHelper.ParseTimeSpan(timeSpanString, DateTimeHelper.TimeSpanComponent.Minutes);

        // Assert
        result.Should().Be(TimeSpan.FromMinutes(30));
    }

    [Fact]
    public void ParseTimeSpan_With_Component_Seconds_Should_Assume_Seconds()
    {
        // Arrange
        string timeSpanString = "45"; // Should be interpreted as 45 seconds

        // Act
        var result = DateTimeHelper.ParseTimeSpan(timeSpanString, DateTimeHelper.TimeSpanComponent.Seconds);

        // Assert
        result.Should().Be(TimeSpan.FromSeconds(45));
    }

    [Fact]
    public void ParseTimeSpan_With_Component_Days_Should_Assume_Days()
    {
        // Arrange
        string timeSpanString = "2"; // Should be interpreted as 2 days

        // Act
        var result = DateTimeHelper.ParseTimeSpan(timeSpanString, DateTimeHelper.TimeSpanComponent.Days);

        // Assert
        result.Should().Be(TimeSpan.FromDays(2));
    }

    [Fact]
    public void ParseTimeSpan_With_Null_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.ParseTimeSpan(null!);
        action.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void ParseTimeSpan_With_Invalid_Format_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.ParseTimeSpan("invalid-timespan");
        action.Should().Throw<ArgumentException>()
            .WithParameterName("value")
            .WithMessage("*Unable to parse TimeSpan from value*");
    }

    [Fact]
    public void ParseTimeSpan_With_Positive_Sign_Should_Parse_Correctly()
    {
        // Arrange
        string timeSpanString = "+02:30:00";

        // Act
        var result = DateTimeHelper.ParseTimeSpan(timeSpanString);

        // Assert
        result.Should().Be(new TimeSpan(2, 30, 0));
    }

    #endregion

    #region TryParseTimeSpan Method Tests

    [Fact]
    public void TryParseTimeSpan_With_Valid_String_Should_Return_True()
    {
        // Arrange
        string timeSpanString = "01:30:45";

        // Act
        var success = DateTimeHelper.TryParseTimeSpan(timeSpanString, DateTimeHelper.TimeSpanComponent.None, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(new TimeSpan(1, 30, 45));
    }

    [Fact]
    public void TryParseTimeSpan_With_Invalid_String_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryParseTimeSpan("invalid", DateTimeHelper.TimeSpanComponent.None, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TimeSpan));
    }

    [Fact]
    public void TryParseTimeSpan_With_Null_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryParseTimeSpan(null, DateTimeHelper.TimeSpanComponent.None, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TimeSpan));
    }

    [Fact]
    public void TryParseTimeSpan_With_Whitespace_Should_Return_False()
    {
        // Act
        var success = DateTimeHelper.TryParseTimeSpan("   ", DateTimeHelper.TimeSpanComponent.None, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TimeSpan));
    }

    [Fact]
    public void TryParseTimeSpan_With_Component_Assumption_Should_Work()
    {
        // Act
        var success = DateTimeHelper.TryParseTimeSpan("120", DateTimeHelper.TimeSpanComponent.Seconds, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TimeSpan.FromSeconds(120));
    }

    #endregion

    #region FormatWithSign Method Tests

    [Fact]
    public void FormatWithSign_With_Positive_TimeSpan_Should_Add_Plus_Sign()
    {
        // Arrange
        var timeSpan = TimeSpan.FromHours(5);

        // Act
        var result = DateTimeHelper.FormatWithSign(timeSpan);

        // Assert
        result.Should().StartWith("+");
        result.Should().Be("+05:00:00");
    }

    [Fact]
    public void FormatWithSign_With_Negative_TimeSpan_Should_Keep_Minus_Sign()
    {
        // Arrange
        var timeSpan = TimeSpan.FromHours(-5);

        // Act
        var result = DateTimeHelper.FormatWithSign(timeSpan);

        // Assert
        result.Should().StartWith("-");
        result.Should().Be("-05:00:00");
    }

    [Fact]
    public void FormatWithSign_With_Zero_Should_Add_Plus_Sign()
    {
        // Act
        var result = DateTimeHelper.FormatWithSign(TimeSpan.Zero);

        // Assert
        result.Should().StartWith("+");
        result.Should().Be("+00:00:00");
    }

    [Fact]
    public void FormatWithSign_Nullable_With_Valid_TimeSpan_Should_Format_Correctly()
    {
        // Arrange
        TimeSpan? timeSpan = TimeSpan.FromMinutes(30);

        // Act
        var result = DateTimeHelper.FormatWithSign(timeSpan);

        // Assert
        result.Should().Be("+00:30:00");
    }

    [Fact]
    public void FormatWithSign_Nullable_With_Null_Should_Return_Null()
    {
        // Arrange
        TimeSpan? timeSpan = null;

        // Act
        var result = DateTimeHelper.FormatWithSign(timeSpan);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetLastDayOfMonth Method Tests

    [Fact]
    public void GetLastDayOfMonth_With_Valid_Year_And_Month_Should_Return_Last_Day()
    {
        // Act
        var result = DateTimeHelper.GetLastDayOfMonth(2025, 7);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 31));
    }

    [Fact]
    public void GetLastDayOfMonth_With_February_Leap_Year_Should_Return_29th()
    {
        // Act
        var result = DateTimeHelper.GetLastDayOfMonth(2024, 2); // 2024 is a leap year

        // Assert
        result.Should().Be(new DateTime(2024, 2, 29));
    }

    [Fact]
    public void GetLastDayOfMonth_With_February_Non_Leap_Year_Should_Return_28th()
    {
        // Act
        var result = DateTimeHelper.GetLastDayOfMonth(2025, 2); // 2025 is not a leap year

        // Assert
        result.Should().Be(new DateTime(2025, 2, 28));
    }

    [Fact]
    public void GetLastDayOfMonth_With_Invalid_Year_Should_Throw_ArgumentOutOfRangeException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.GetLastDayOfMonth(0, 1);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetLastDayOfMonth_With_Invalid_Month_Low_Should_Throw_ArgumentOutOfRangeException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.GetLastDayOfMonth(2025, 0);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetLastDayOfMonth_With_Invalid_Month_High_Should_Throw_ArgumentOutOfRangeException()
    {
        // Act & Assert
        var action = () => DateTimeHelper.GetLastDayOfMonth(2025, 13);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetLastDayOfMonth_Should_Work_For_All_Months()
    {
        // Arrange
        var expectedDays = new[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        for (int month = 1; month <= 12; month++)
        {
            // Act
            var result = DateTimeHelper.GetLastDayOfMonth(2025, month);

            // Assert
            result.Day.Should().Be(expectedDays[month - 1], $"Month {month} should have {expectedDays[month - 1]} days");
        }
    }

    #endregion

    #region Truncate Method Tests

    [Fact]
    public void Truncate_To_Hour_Should_Remove_Minutes_Seconds_Milliseconds()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 123);
        var precision = TimeSpan.FromHours(1);

        // Act
        var result = DateTimeHelper.Truncate(dateTime, precision);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 14, 0, 0));
    }

    [Fact]
    public void Truncate_To_Minute_Should_Remove_Seconds_Milliseconds()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 123);
        var precision = TimeSpan.FromMinutes(1);

        // Act
        var result = DateTimeHelper.Truncate(dateTime, precision);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 14, 30, 0));
    }

    [Fact]
    public void Truncate_To_Second_Should_Remove_Milliseconds()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 123);
        var precision = TimeSpan.FromSeconds(1);

        // Act
        var result = DateTimeHelper.Truncate(dateTime, precision);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 14, 30, 45));
    }

    [Fact]
    public void Truncate_To_Day_Should_Remove_Time_Components()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 123);
        var precision = TimeSpan.FromDays(1);

        // Act
        var result = DateTimeHelper.Truncate(dateTime, precision);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 0, 0, 0));
    }

    [Fact]
    public void Truncate_Should_Preserve_DateTimeKind()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 30, 45, 123, DateTimeKind.Utc);
        var precision = TimeSpan.FromHours(1);

        // Act
        var result = DateTimeHelper.Truncate(dateTime, precision);

        // Assert
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Truncate_With_Zero_Precision_Should_Throw_ArgumentException()
    {
        // Arrange
        var dateTime = DateTime.Now;
        var precision = TimeSpan.Zero;

        // Act & Assert
        var action = () => DateTimeHelper.Truncate(dateTime, precision);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("precision")
            .WithMessage("*Precision must be positive*");
    }

    [Fact]
    public void Truncate_With_Negative_Precision_Should_Throw_ArgumentException()
    {
        // Arrange
        var dateTime = DateTime.Now;
        var precision = TimeSpan.FromMinutes(-1);

        // Act & Assert
        var action = () => DateTimeHelper.Truncate(dateTime, precision);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("precision")
            .WithMessage("*Precision must be positive*");
    }

    [Fact]
    public void Truncate_With_Custom_Precision_Should_Work_Correctly()
    {
        // Arrange
        var dateTime = new DateTime(2025, 7, 23, 14, 37, 45, 123);
        var precision = TimeSpan.FromMinutes(15); // 15-minute intervals

        // Act
        var result = DateTimeHelper.Truncate(dateTime, precision);

        // Assert
        result.Should().Be(new DateTime(2025, 7, 23, 14, 30, 0)); // Truncated to nearest 15-minute interval
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void Create_And_ToFileSortableString_Roundtrip_Should_Work()
    {
        // Arrange
        var originalString = "2025-07-23T14:30:45.123";

        // Act
        var dateTime = DateTimeHelper.Create(originalString);
        var fileSortable = DateTimeHelper.ToFileSortableString(dateTime, true);
        var backToDateTime = DateTimeHelper.FromFileSortableString(fileSortable);

        // Assert
        backToDateTime.Should().Be(dateTime);
    }

    [Fact]
    public void All_Day_Parts_Should_Be_Within_Same_Day()
    {
        // Arrange
        var dateString = "2025-07-23";

        // Act
        var beginOfDay = DateTimeHelper.CreateWithDayPart(dateString, DateTimeHelper.DayPart.BeginOfDay);
        var highNoon = DateTimeHelper.CreateWithDayPart(dateString, DateTimeHelper.DayPart.HighNoon);
        var endOfDay = DateTimeHelper.CreateWithDayPart(dateString, DateTimeHelper.DayPart.EndOfDay);

        // Assert
        beginOfDay.Date.Should().Be(highNoon.Date);
        highNoon.Date.Should().Be(endOfDay.Date);
        beginOfDay.Should().BeBefore(highNoon);
        highNoon.Should().BeBefore(endOfDay);
    }

    [Fact]
    public void TimeSpan_Parsing_With_Different_Components_Should_Produce_Different_Results()
    {
        // Arrange
        var value = "30";

        // Act
        var asSeconds = DateTimeHelper.ParseTimeSpan(value, DateTimeHelper.TimeSpanComponent.Seconds);
        var asMinutes = DateTimeHelper.ParseTimeSpan(value, DateTimeHelper.TimeSpanComponent.Minutes);
        var asHours = DateTimeHelper.ParseTimeSpan(value, DateTimeHelper.TimeSpanComponent.Hours);

        // Assert
        asSeconds.Should().NotBe(asMinutes);
        asMinutes.Should().NotBe(asHours);
        asSeconds.Should().Be(TimeSpan.FromSeconds(30));
        asMinutes.Should().Be(TimeSpan.FromMinutes(30));
        asHours.Should().Be(TimeSpan.FromHours(30));
    }

    [Fact]
    public void Multiple_DateTime_Formats_Should_Parse_To_Same_Result()
    {
        // Arrange
        var formats = new[]
        {
            "2025-07-23T14:30:45",
            "20250723T143045",
            "20250723143045"
        };
        var expected = new DateTime(2025, 7, 23, 14, 30, 45);

        foreach (var format in formats)
        {
            // Act
            var result = DateTimeHelper.Create(format);

            // Assert
            result.Should().Be(expected, $"Format: {format}");
        }
    }

    [Fact]
    public void SqlServer_Precision_Should_Be_Different_From_Original_For_High_Precision_DateTime()
    {
        // Arrange
        var highPrecisionDateTime = new DateTime(2025, 7, 23, 14, 30, 45, 123).AddTicks(4567);

        // Act
        var sqlPrecision = DateTimeHelper.ToSqlServerPrecision(highPrecisionDateTime);

        // Assert
        sqlPrecision.Should().NotBe(highPrecisionDateTime);
        // SQL Server DateTime has lower precision, so some ticks should be lost
        sqlPrecision.Should().BeCloseTo(highPrecisionDateTime, TimeSpan.FromMilliseconds(10));
    }

    #endregion
}