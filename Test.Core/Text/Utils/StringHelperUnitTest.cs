//--------------------------------------------------------------------------
// File:    StringHelperUnitTest.cs
// Content: Unit tests for StringHelper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Globalization;
using System.Text;

namespace AnBo.Test;

public class StringHelperUnitTest
{
    #region SafeToString Method Tests

    [Fact]
    public void SafeToString_With_Null_Object_Should_Return_Empty_String()
    {
        // Arrange
        object? obj = null;

        // Act
        var result = StringHelper.SafeToString(obj);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void SafeToString_With_Valid_Object_Should_Return_String_Representation()
    {
        // Arrange
        var obj = 42;

        // Act
        var result = StringHelper.SafeToString(obj);

        // Assert
        result.Should().Be("42");
    }

    [Fact]
    public void SafeToString_With_String_Object_Should_Return_Same_String()
    {
        // Arrange
        var obj = "test string";

        // Act
        var result = StringHelper.SafeToString(obj);

        // Assert
        result.Should().Be("test string");
    }

    [Fact]
    public void SafeToString_With_Complex_Object_Should_Return_String_Representation()
    {
        // Arrange
        var obj = new DateTime(2025, 1, 15);

        // Act
        var result = StringHelper.SafeToString(obj);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SafeToString_With_DefaultValue_And_Null_Should_Return_DefaultValue_String()
    {
        // Arrange
        object? obj = null;
        string defaultValue = "default";

        // Act
        var result = StringHelper.SafeToString(obj, defaultValue);

        // Assert
        result.Should().Be("default");
    }

    [Fact]
    public void SafeToString_With_DefaultValue_And_Valid_Object_Should_Return_Object_String()
    {
        // Arrange
        var obj = 123;
        string defaultValue = "default";

        // Act
        var result = StringHelper.SafeToString(obj, defaultValue);

        // Assert
        result.Should().Be("123");
    }

    [Fact]
    public void SafeToString_With_DefaultValue_Null_Should_Return_Empty_String()
    {
        // Arrange
        object? obj = null;
        string? defaultValue = null;

        // Act
        var result = StringHelper.SafeToString(obj, defaultValue!);

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion

    #region SafeFormat Method Tests

    [Fact]
    public void SafeFormat_With_Null_Format_Should_Return_Empty_String()
    {
        // Arrange
        string? format = null;
        object[] parameters = { 1, 2, 3 };

        // Act
        var result = StringHelper.SafeFormat(format, parameters);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void SafeFormat_With_Empty_Format_Should_Return_Empty_String()
    {
        // Arrange
        string format = string.Empty;
        object[] parameters = { 1, 2, 3 };

        // Act
        var result = StringHelper.SafeFormat(format, parameters);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void SafeFormat_With_Valid_Format_And_Parameters_Should_Return_Formatted_String()
    {
        // Arrange
        string format = "Value: {0}, Name: {1}";
        object[] parameters = { 42, "Test" };

        // Act
        var result = StringHelper.SafeFormat(format, parameters);

        // Assert
        result.Should().Be("Value: 42, Name: Test");
    }

    [Fact]
    public void SafeFormat_With_No_Parameters_Should_Return_Original_Format()
    {
        // Arrange
        string format = "No parameters here";
        object[] parameters = { };

        // Act
        var result = StringHelper.SafeFormat(format, parameters);

        // Assert
        result.Should().Be("No parameters here");
    }

    [Fact]
    public void SafeFormat_With_Invalid_Format_Should_Return_Error_Information()
    {
        // Arrange
        string format = "Invalid format {0} {1} {2}";
        object[] parameters = { "only_one" };

        // Act
        var result = StringHelper.SafeFormat(format, parameters);

        // Assert
        result.Should().Contain("Exception occurred during formatting");
    }

    [Fact]
    public void SafeFormat_With_Null_Parameters_Should_Handle_Gracefully()
    {
        // Arrange
        string format = "Test {0}";
        object?[] parameters = { null };

        // Act
        var result = StringHelper.SafeFormat(format, parameters);

        // Assert
        result.Should().Be("Test ");
    }

    #endregion

    #region SafeAppendFormat Method Tests

    [Fact]
    public void SafeAppendFormat_With_Null_StringBuilder_Should_Not_Throw()
    {
        // Arrange
        StringBuilder? sb = null;
        string format = "test {0}";
        object[] args = { 1 };

        // Act
        Action action = () => StringHelper.SafeAppendFormat(sb!, format, args);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeAppendFormat_With_Valid_StringBuilder_Should_Append_Formatted_Text()
    {
        // Arrange
        var sb = new StringBuilder("Start: ");
        string format = "Value {0}";
        object[] args = { 42 };

        // Act
        StringHelper.SafeAppendFormat(sb, format, args);

        // Assert
        sb.ToString().Should().Be("Start: Value 42");
    }

    [Fact]
    public void SafeAppendFormat_With_Empty_Format_Should_Not_Append()
    {
        // Arrange
        var sb = new StringBuilder("Start");
        string format = string.Empty;
        object[] args = { };

        // Act
        StringHelper.SafeAppendFormat(sb, format, args);

        // Assert
        sb.ToString().Should().Be("Start");
    }

    [Fact]
    public void SafeAppendFormat_With_No_Args_Should_Append_Format_String()
    {
        // Arrange
        var sb = new StringBuilder("Start: ");
        string format = "No args";
        object[] args = { };

        // Act
        StringHelper.SafeAppendFormat(sb, format, args);

        // Assert
        sb.ToString().Should().Be("Start: No args");
    }

    #endregion

    #region StringCollectionToMultiLine Method Tests

    [Fact]
    public void StringCollectionToMultiLine_With_Null_Collection_Should_Return_Empty_String()
    {
        // Arrange
        IEnumerable<string>? collection = null;

        // Act
        var result = StringHelper.StringCollectionToMultiLine(collection!);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void StringCollectionToMultiLine_With_Empty_Collection_Should_Return_Empty_String()
    {
        // Arrange
        var collection = new List<string>();

        // Act
        var result = StringHelper.StringCollectionToMultiLine(collection);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void StringCollectionToMultiLine_With_Single_Item_Should_Return_Item()
    {
        // Arrange
        var collection = new List<string> { "single item" };

        // Act
        var result = StringHelper.StringCollectionToMultiLine(collection);

        // Assert
        result.Should().Be("single item");
    }

    [Fact]
    public void StringCollectionToMultiLine_With_Multiple_Items_Should_Return_Joined_String()
    {
        // Arrange
        var collection = new List<string> { "line1", "line2", "line3" };

        // Act
        var result = StringHelper.StringCollectionToMultiLine(collection);

        // Assert
        result.Should().Be($"line1{Environment.NewLine}line2{Environment.NewLine}line3");
    }

    [Fact]
    public void StringCollectionToMultiLine_With_Null_Items_Should_Handle_Gracefully()
    {
        // Arrange
        var collection = new List<string?> { "line1", null, "line3" };

        // Act
        var result = StringHelper.StringCollectionToMultiLine(collection!);

        // Assert
        result.Should().Contain("line1");
        result.Should().Contain("line3");
    }

    [Fact]
    public void StringCollectionToMultiLine_Array_With_Null_Array_Should_Return_Empty_String()
    {
        // Arrange
        string[]? array = null;

        // Act
        var result = StringHelper.StringCollectionToMultiLine(array);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void StringCollectionToMultiLine_Empty_Array_Should_Return_Empty_String()
    {
        // Arrange
        string[] array = [];

        // Act
        var result = StringHelper.StringCollectionToMultiLine(array);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void StringCollectionToMultiLine_Array_With_Multiple_Items_Should_Return_Joined_String()
    {
        // Arrange
        string[] array = ["item1", "item2", "item3"];

        // Act
        var result = StringHelper.StringCollectionToMultiLine(array);

        // Assert
        result.Should().Be($"item1{Environment.NewLine}item2{Environment.NewLine}item3");
    }

    [Fact]
    public void StringCollectionToMultiLine_Array_With_Single_Item_Should_Return_Item()
    {
        // Arrange
        string[] array = ["single item"];

        // Act
        var result = StringHelper.StringCollectionToMultiLine(array);

        // Assert
        result.Should().Be("single item");
    }

    #endregion

    #region Join Method Tests

    [Fact]
    public void Join_With_Null_Separator_Should_Throw_ArgNullException()
    {
        // Arrange
        string? separator = null;
        int[] items = [1, 2, 3];

        // Act & Assert
        Action action = () => StringHelper.Join(separator!, items);
        action.Should().Throw<ArgNullException>();
    }

    [Fact]
    public void Join_With_Valid_Items_Should_Return_Joined_String()
    {
        // Arrange
        string separator = ", ";
        int[] items = { 1, 2, 3 };

        // Act
        var result = StringHelper.Join(separator, items);

        // Assert
        result.Should().Be("1, 2, 3");
    }

    [Fact]
    public void Join_With_Empty_Items_Should_Return_Empty_String()
    {
        // Arrange
        string separator = ", ";
        int[] items = { };

        // Act
        var result = StringHelper.Join(separator, items);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Join_With_IEnumerable_And_Null_Items_Should_Throw_ArgNullException()
    {
        // Arrange
        string separator = ", ";
        IEnumerable<int>? items = null;

        // Act & Assert
        Action action = () => StringHelper.Join(separator, items!);
        action.Should().Throw<ArgNullException>();
    }

    [Fact]
    public void Join_With_IEnumerable_And_Converter_Should_Use_Converter()
    {
        // Arrange
        string separator = " | ";
        var items = new List<int> { 1, 2, 3 };
        Func<int, string> converter = x => $"#{x}";

        // Act
        var result = StringHelper.Join(separator, items, converter);

        // Assert
        result.Should().Be("#1 | #2 | #3");
    }

    [Fact]
    public void Join_With_Null_Converter_Should_Use_Default_ToString()
    {
        // Arrange
        string separator = "-";
        var items = new List<int> { 10, 20, 30 };

        // Act
        var result = StringHelper.Join(separator, items, null);

        // Assert
        result.Should().Be("10-20-30");
    }

    #endregion

    #region Byte Array Conversion Method Tests

    [Fact]
    public void GetBytesFromString_With_Null_String_Should_Return_Empty_Array()
    {
        // Arrange
        string? str = null;

        // Act
        var result = StringHelper.GetBytesFromString(str);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetBytesFromString_With_Empty_String_Should_Return_Empty_Array()
    {
        // Arrange
        string str = string.Empty;

        // Act
        var result = StringHelper.GetBytesFromString(str);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetBytesFromString_With_Valid_String_Should_Return_Byte_Array()
    {
        // Arrange
        string str = "Hello";

        // Act
        var result = StringHelper.GetBytesFromString(str);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void GetStringFromBytes_With_Null_Array_Should_Throw_ArgNullException()
    {
        // Arrange
        byte[]? data = null;

        // Act & Assert
        Action action = () => StringHelper.GetStringFromBytes(data!);
        action.Should().Throw<ArgNullException>();
    }

    [Fact]
    public void GetStringFromBytes_With_Valid_Array_Should_Return_String()
    {
        // Arrange
        string original = "Test String";
        byte[] data = StringHelper.GetBytesFromString(original);

        // Act
        var result = StringHelper.GetStringFromBytes(data);

        // Assert
        result.Should().Be(original);
    }

    [Fact]
    public void GetStringFromBytes_With_Empty_Array_Should_Return_Empty_String()
    {
        // Arrange
        byte[] data = { };

        // Act
        var result = StringHelper.GetStringFromBytes(data);

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion

    #region Number Formatting Method Tests

    [Fact]
    public void PadIntegerZerosLeft_Should_Pad_With_Zeros()
    {
        // Arrange
        int value = 42;
        int length = 5;

        // Act
        var result = StringHelper.PadIntegerZerosLeft(value, length);

        // Assert
        result.Should().Be("00042");
    }

    [Fact]
    public void PadIntegerZerosLeft_With_Length_Less_Than_Value_Length_Should_Return_Value_String()
    {
        // Arrange
        int value = 12345;
        int length = 3;

        // Act
        var result = StringHelper.PadIntegerZerosLeft(value, length);

        // Assert
        result.Should().Be("12345");
    }

    [Fact]
    public void PadIntegerZerosLeft_With_Zero_Length_Should_Return_Single_Character()
    {
        // Arrange
        int value = 5;
        int length = 0;

        // Act
        var result = StringHelper.PadIntegerZerosLeft(value, length);

        // Assert
        result.Should().Be("5");
    }

    [Fact]
    public void PadIntegerLeft_Should_Pad_With_Specified_Character()
    {
        // Arrange
        int value = 42;
        int length = 5;
        char paddingChar = '*';

        // Act
        var result = StringHelper.PadIntegerLeft(value, length, paddingChar);

        // Assert
        result.Should().Be("***42");
    }

    [Fact]
    public void PadIntegerLeft_With_Default_Padding_Should_Use_Space()
    {
        // Arrange
        int value = 123;
        int length = 6;

        // Act
        var result = StringHelper.PadIntegerLeft(value, length);

        // Assert
        result.Should().Be("   123");
    }

    [Fact]
    public void PadIntegerRight_Should_Pad_With_Specified_Character()
    {
        // Arrange
        int value = 42;
        int length = 5;
        char paddingChar = '*';

        // Act
        var result = StringHelper.PadIntegerRight(value, length, paddingChar);

        // Assert
        result.Should().Be("42***");
    }

    [Fact]
    public void ToStringWithLeading_Should_Format_Based_On_Max_Value()
    {
        // Arrange
        int value = 5;
        int maxValue = 1000;

        // Act
        var result = StringHelper.ToStringWithLeading(value, maxValue);

        // Assert
        result.Should().Be("0005");
    }

    [Fact]
    public void ToStringWithLeading_With_Value_Greater_Than_Max_Should_Return_Value_String()
    {
        // Arrange
        int value = 2000;
        int maxValue = 1000;

        // Act
        var result = StringHelper.ToStringWithLeading(value, maxValue);

        // Assert
        result.Should().Be("2000");
    }

    [Fact]
    public void ToStringWithLeading_With_Custom_Culture_Should_Use_Culture()
    {
        // Arrange
        int value = 123;
        int maxValue = 10000;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = StringHelper.ToStringWithLeading(value, maxValue, culture);

        // Assert
        result.Should().Be("00123");
    }

    #endregion

    #region Character Removal Method Tests

    [Fact]
    public void RemoveCharacters_With_Null_String_Should_Return_Empty_String()
    {
        // Arrange
        string? str = null;
        char[] chars = { 'a', 'b' };

        // Act
        var result = StringHelper.RemoveCharacters(str, chars);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void RemoveCharacters_With_Empty_String_Should_Return_Empty_String()
    {
        // Arrange
        string str = string.Empty;
        char[] chars = { 'a', 'b' };

        // Act
        var result = StringHelper.RemoveCharacters(str, chars);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void RemoveCharacters_With_Valid_String_Should_Remove_Specified_Characters()
    {
        // Arrange
        string str = "hello world";
        char[] chars = { 'l', 'o' };

        // Act
        var result = StringHelper.RemoveCharacters(str, chars);

        // Assert
        result.Should().Be("he wrd");
    }

    [Fact]
    public void RemoveCharacters_With_No_Characters_To_Remove_Should_Return_Original_String()
    {
        // Arrange
        string str = "hello world";
        char[] chars = { };

        // Act
        var result = StringHelper.RemoveCharacters(str, chars);

        // Assert
        result.Should().Be("hello world");
    }

    [Fact]
    public void RemoveCharacters_With_Characters_Not_In_String_Should_Return_Original_String()
    {
        // Arrange
        string str = "hello";
        char[] chars = { 'x', 'y', 'z' };

        // Act
        var result = StringHelper.RemoveCharacters(str, chars);

        // Assert
        result.Should().Be("hello");
    }

    [Fact]
    public void RemoveCharactersInverse_With_Null_String_Should_Return_Empty_String()
    {
        // Arrange
        string? str = null;
        char[] chars = { 'a', 'b' };

        // Act
        var result = StringHelper.RemoveCharactersInverse(str, chars);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void RemoveCharactersInverse_With_Valid_String_Should_Keep_Only_Specified_Characters()
    {
        // Arrange
        string str = "hello world 123";
        char[] chars = { 'h', 'e', 'l', 'o' };

        // Act
        var result = StringHelper.RemoveCharactersInverse(str, chars);

        // Assert
        result.Should().Be("hellool");
    }

    [Fact]
    public void RemoveCharactersInverse_With_No_Characters_To_Keep_Should_Return_Empty_String()
    {
        // Arrange
        string str = "hello world";
        char[] chars = { };

        // Act
        var result = StringHelper.RemoveCharactersInverse(str, chars);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void RemoveCharactersInverse_With_Characters_Not_In_String_Should_Return_Empty_String()
    {
        // Arrange
        string str = "hello";
        char[] chars = { 'x', 'y', 'z' };

        // Act
        var result = StringHelper.RemoveCharactersInverse(str, chars);

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion

    #region Random String Generation Method Tests

    [Fact]
    public void RandomString_With_Zero_Size_Should_Return_Empty_String()
    {
        // Act
        var result = StringHelper.RandomString(0);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void RandomString_With_Positive_Size_Should_Return_String_Of_Correct_Length()
    {
        // Arrange
        int size = 10;

        // Act
        var result = StringHelper.RandomString(size);

        // Assert
        result.Should().HaveLength(size);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void RandomString_With_LowerCase_True_Should_Return_Lowercase_String()
    {
        // Arrange
        int size = 100;

        // Act
        var result = StringHelper.RandomString(size, true);

        // Assert
        result.Should().HaveLength(size);
        result.Should().Match(s => s.All(char.IsLower));
    }

    [Fact]
    public void RandomString_With_LowerCase_False_Should_Return_Mixed_Case_String()
    {
        // Arrange
        int size = 100;

        // Act
        var result = StringHelper.RandomString(size, false);

        // Assert
        result.Should().HaveLength(size);
        result.Should().Match(s => s.All(char.IsLetter));
    }

    [Fact]
    public void RandomString_With_Negative_Size_Should_Throw_ArgOutOfRangeException()
    {
        // Arrange
        int size = -1;

        // Act & Assert
        Action action = () => StringHelper.RandomString(size);
        action.Should().Throw<ArgOutOfRangeException<int>>();
    }

    [Fact]
    public void RandomString_With_Size_Too_Large_Should_Throw_ArgOutOfRangeException()
    {
        // Arrange
        int size = 5000; // Greater than 4096

        // Act & Assert
        Action action = () => StringHelper.RandomString(size);
        action.Should().Throw<ArgOutOfRangeException<int>>();
    }

    [Fact]
    public void RandomString_Multiple_Calls_Should_Return_Different_Strings()
    {
        // Arrange
        int size = 20;

        // Act
        var result1 = StringHelper.RandomString(size);
        var result2 = StringHelper.RandomString(size);

        // Assert
        result1.Should().NotBe(result2);
    }

    #endregion

    #region CRC32 Calculation Method Tests

    [Fact]
    public void CalculateCrc32_With_Null_String_Should_Return_Zero()
    {
        // Arrange
        string? str = null;

        // Act
        var result = StringHelper.CalculateCrc32(str!);

        // Assert
        result.Should().Be(0u);
    }

    [Fact]
    public void CalculateCrc32_With_Empty_String_Should_Return_Zero()
    {
        // Arrange
        string str = string.Empty;

        // Act
        var result = StringHelper.CalculateCrc32(str);

        // Assert
        result.Should().Be(0u);
    }

    [Fact]
    public void CalculateCrc32_With_Valid_String_Should_Return_Non_Zero()
    {
        // Arrange
        string str = "Hello World";

        // Act
        var result = StringHelper.CalculateCrc32(str);

        // Assert
        result.Should().NotBe(0u);
    }

    [Fact]
    public void CalculateCrc32_With_Same_String_Should_Return_Same_Value()
    {
        // Arrange
        string str = "test string";

        // Act
        var result1 = StringHelper.CalculateCrc32(str);
        var result2 = StringHelper.CalculateCrc32(str);

        // Assert
        result1.Should().Be(result2);
    }

    [Fact]
    public void CalculateCrc32_With_Different_Strings_Should_Return_Different_Values()
    {
        // Arrange
        string str1 = "string1";
        string str2 = "string2";

        // Act
        var result1 = StringHelper.CalculateCrc32(str1);
        var result2 = StringHelper.CalculateCrc32(str2);

        // Assert
        result1.Should().NotBe(result2);
    }

    #endregion

    #region HTML Utilities Method Tests

    [Fact]
    public void ReplaceNewLineWithHtmlBr_With_Null_String_Should_Return_Empty_String()
    {
        // Arrange
        string? text = null;

        // Act
        var result = StringHelper.ReplaceNewLineWithHtmlBr(text!);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ReplaceNewLineWithHtmlBr_With_Empty_String_Should_Return_Empty_String()
    {
        // Arrange
        string text = string.Empty;

        // Act
        var result = StringHelper.ReplaceNewLineWithHtmlBr(text);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ReplaceNewLineWithHtmlBr_With_CRLF_Should_Replace_With_Br()
    {
        // Arrange
        string text = "line1\r\nline2";

        // Act
        var result = StringHelper.ReplaceNewLineWithHtmlBr(text);

        // Assert
        result.Should().Be("line1<br />line2");
    }

    [Fact]
    public void ReplaceNewLineWithHtmlBr_With_LF_Should_Replace_With_Br()
    {
        // Arrange
        string text = "line1\nline2";

        // Act
        var result = StringHelper.ReplaceNewLineWithHtmlBr(text);

        // Assert
        result.Should().Be("line1<br />line2");
    }

    [Fact]
    public void ReplaceNewLineWithHtmlBr_With_Multiple_NewLines_Should_Replace_All()
    {
        // Arrange
        string text = "line1\r\nline2\nline3\r\nline4";

        // Act
        var result = StringHelper.ReplaceNewLineWithHtmlBr(text);

        // Assert
        result.Should().Be("line1<br />line2<br />line3<br />line4");
    }

    #endregion

    #region String Splitting Method Tests

    [Fact]
    public void SplitOn_With_Null_String_Should_Return_Two_Empty_Strings()
    {
        // Arrange
        string? str = null;
        int index = 0;
        bool includeIndexInFirstPortion = true;

        // Act
        var result = StringHelper.SplitOn(str, index, includeIndexInFirstPortion);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be(string.Empty);
        result[1].Should().Be(string.Empty);
    }

    [Fact]
    public void SplitOn_With_Empty_String_Should_Return_Two_Empty_Strings()
    {
        // Arrange
        string str = string.Empty;
        int index = 0;
        bool includeIndexInFirstPortion = true;

        // Act
        var result = StringHelper.SplitOn(str, index, includeIndexInFirstPortion);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be(string.Empty);
        result[1].Should().Be(string.Empty);
    }

    [Fact]
    public void SplitOn_With_Valid_String_And_Include_Index_True_Should_Split_Correctly()
    {
        // Arrange
        string str = "Hello World";
        int index = 5; // Space character
        bool includeIndexInFirstPortion = true;

        // Act
        var result = StringHelper.SplitOn(str, index, includeIndexInFirstPortion);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Hello ");
        result[1].Should().Be("World");
    }

    [Fact]
    public void SplitOn_With_Valid_String_And_Include_Index_False_Should_Split_Correctly()
    {
        // Arrange
        string str = "Hello World";
        int index = 5; // Space character
        bool includeIndexInFirstPortion = false;

        // Act
        var result = StringHelper.SplitOn(str, index, includeIndexInFirstPortion);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Hello");
        result[1].Should().Be(" World");
    }

    [Fact]
    public void SplitOn_With_Index_Out_Of_Range_Should_Return_Original_And_Empty()
    {
        // Arrange
        string str = "Hello";
        int index = 10; // Out of range
        bool includeIndexInFirstPortion = true;

        // Act
        var result = StringHelper.SplitOn(str, index, includeIndexInFirstPortion);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Hello");
        result[1].Should().Be(string.Empty);
    }

    [Fact]
    public void SplitOn_With_Negative_Index_Should_Return_Original_And_Empty()
    {
        // Arrange
        string str = "Hello";
        int index = -1;
        bool includeIndexInFirstPortion = true;

        // Act
        var result = StringHelper.SplitOn(str, index, includeIndexInFirstPortion);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Hello");
        result[1].Should().Be(string.Empty);
    }

    [Fact]
    public void SquareChunk_With_Null_String_Should_Return_Single_Empty_String()
    {
        // Arrange
        string? value = null;
        char[] separators = { ' ', ',' };

        // Act
        var result = StringHelper.SquareChunk(value, separators);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be(string.Empty);
    }

    [Fact]
    public void SquareChunk_With_Empty_String_Should_Return_Single_Empty_String()
    {
        // Arrange
        string value = string.Empty;
        char[] separators = { ' ', ',' };

        // Act
        var result = StringHelper.SquareChunk(value, separators);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be(string.Empty);
    }

    [Fact]
    public void SquareChunk_With_Short_String_Should_Return_Single_Chunk()
    {
        // Arrange
        string value = "short";
        char[] separators = { ' ' };

        // Act
        var result = StringHelper.SquareChunk(value, separators);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be("short");
    }

    [Fact]
    public void SquareChunk_With_Long_String_And_Separators_Should_Return_Multiple_Chunks()
    {
        // Arrange
        string value = "This is a long string with multiple words";
        char[] separators = { ' ' };

        // Act
        var result = StringHelper.SquareChunk(value, separators);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(chunk => chunk.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void SquareChunk_With_Null_Separators_Should_Handle_Gracefully()
    {
        // Arrange
        string value = "test string";
        char[]? separators = null;

        // Act
        var result = StringHelper.SquareChunk(value, separators!);

        // Assert
        result.Should().NotBeEmpty();
    }

    #endregion

    #region Static Members Tests

    [Fact]
    public void DefaultQuoteSensitiveChars_Should_Contain_Quote_Character()
    {
        // Act & Assert
        StringHelper.DefaultQuoteSensitiveChars.Should().Contain('"');
        StringHelper.DefaultQuoteSensitiveChars.Should().HaveCount(1);
    }

    #endregion

    #region JoinParams Method Tests

    [Fact]
    public void JoinParams_With_Null_Separator_Should_Throw_ArgNullException()
    {
        // Arrange
        string? separator = null;
        int[] items = { 1, 2, 3 };

        // Act & Assert
        Action action = () => StringHelper.JoinParams(separator!, items);
        action.Should().Throw<ArgNullException>();
    }

    [Fact]
    public void JoinParams_With_Valid_Items_Should_Return_Joined_String()
    {
        // Arrange
        string separator = ", ";

        // Act
        var result = StringHelper.JoinParams(separator, 1, 2, 3);

        // Assert
        result.Should().Be("1, 2, 3");
    }

    [Fact]
    public void JoinParams_With_No_Items_Should_Return_Empty_String()
    {
        // Arrange
        string separator = ", ";

        // Act
        var result = StringHelper.JoinParams(separator);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void JoinParams_With_Single_Item_Should_Return_Item_String()
    {
        // Arrange
        string separator = ", ";

        // Act
        var result = StringHelper.JoinParams(separator, "single");

        // Assert
        result.Should().Be("single");
    }

    [Fact]
    public void JoinParams_With_Null_Items_Should_Handle_Gracefully()
    {
        // Arrange
        string separator = " | ";

        // Act
        var result = StringHelper.JoinParams(separator, "test", null, "value");

        // Assert
        result.Should().Be("test |  | value");
    }

    [Fact]
    public void JoinParams_With_Different_Types_Should_Use_ToString()
    {
        // Arrange
        string separator = " - ";

        // Act
        var result = StringHelper.JoinParams(separator, 42, "text", true, DateTime.Parse("2025-01-15"));

        // Assert
        result.Should().Contain("42");
        result.Should().Contain("text");
        result.Should().Contain("True");
        result.Should().Contain("2025");
        result.Should().Contain(" - ");
    }

    [Fact]
    public void JoinParams_With_Empty_Separator_Should_Concatenate_Items()
    {
        // Arrange
        string separator = string.Empty;

        // Act
        var result = StringHelper.JoinParams(separator, "A", "B", "C");

        // Assert
        result.Should().Be("ABC");
    }

    [Fact]
    public void JoinParams_With_Complex_Objects_Should_Use_ToString()
    {
        // Arrange
        string separator = " <> ";
        var list = new List<string> { "item1", "item2" };
        var dict = new Dictionary<string, int> { { "key", 1 } };

        // Act
        var result = StringHelper.JoinParams(separator, list, dict);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(" <> ");
    }

    [Fact]
    public void JoinParams_With_Large_Number_Of_Items_Should_Handle_Efficiently()
    {
        // Arrange
        string separator = ", ";
        var items = Enumerable.Range(1, 100).Cast<object>().ToArray();

        // Act
        var result = StringHelper.JoinParams(separator, items);

        // Assert
        result.Should().StartWith("1, 2, 3");
        result.Should().EndWith("98, 99, 100");
        result.Should().Contain("50");
    }

    [Fact]
    public void JoinParams_With_Whitespace_Separator_Should_Format_Correctly()
    {
        // Arrange
        string separator = "   ";

        // Act
        var result = StringHelper.JoinParams(separator, "word1", "word2", "word3");

        // Assert
        result.Should().Be("word1   word2   word3");
    }

    [Fact]
    public void JoinParams_With_Special_Characters_Should_Handle_Correctly()
    {
        // Arrange
        string separator = " | ";

        // Act
        var result = StringHelper.JoinParams(separator, "text with spaces", "text\nwith\nnewlines", "text\twith\ttabs");

        // Assert
        result.Should().Contain("text with spaces");
        result.Should().Contain("text\nwith\nnewlines");
        result.Should().Contain("text\twith\ttabs");
        result.Should().Contain(" | ");
    }

    [Fact]
    public void JoinParams_Performance_Should_Be_Reasonable()
    {
        // Arrange
        string separator = ",";
        var items = Enumerable.Range(1, 1000).Cast<object>().ToArray();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = StringHelper.JoinParams(separator, items);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNullOrEmpty();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Performance check
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void SafeFormat_With_Complex_Scenario_Should_Handle_Gracefully()
    {
        // Arrange
        string format = "User: {0}, Age: {1}, Date: {2}";
        object?[] parameters = { null, 25, DateTime.Now };

        // Act
        var result = StringHelper.SafeFormat(format, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("User: ");
        result.Should().Contain("Age: 25");
    }

    [Fact]
    public void Multiple_Character_Operations_Should_Work_Together()
    {
        // Arrange
        string original = "Hello World! 123";
        char[] removeChars = { ' ', '!' };
        char[] keepChars = { 'H', 'e', 'l', 'o', 'W', 'r', 'd' };

        // Act
        var removed = StringHelper.RemoveCharacters(original, removeChars);
        var kept = StringHelper.RemoveCharactersInverse(original, keepChars);

        // Assert
        removed.Should().Be("HelloWorld123");
        kept.Should().Be("HelloWorld");
    }

    [Fact]
    public void ByteArray_Roundtrip_Should_Preserve_Original_String()
    {
        // Arrange
        string original = "Test string with üñíçødé characters";

        // Act
        byte[] bytes = StringHelper.GetBytesFromString(original);
        string restored = StringHelper.GetStringFromBytes(bytes);

        // Assert
        restored.Should().Be(original);
    }

    [Fact]
    public void Number_Formatting_With_Different_Values_Should_Work_Consistently()
    {
        // Arrange & Act
        var result1 = StringHelper.PadIntegerZerosLeft(1, 3);
        var result2 = StringHelper.PadIntegerZerosLeft(10, 3);
        var result3 = StringHelper.PadIntegerZerosLeft(100, 3);

        // Assert
        result1.Should().Be("001");
        result2.Should().Be("010");
        result3.Should().Be("100");
    }

    [Fact]
    public void Join_Methods_Should_Handle_Various_Types()
    {
        // Arrange
        var stringList = new List<string> { "a", "b", "c" };
        var intList = new List<int> { 1, 2, 3 };

        // Act
        var stringResult = StringHelper.Join(", ", stringList);
        var intResult = StringHelper.Join(" | ", intList);

        // Assert
        stringResult.Should().Be("a, b, c");
        intResult.Should().Be("1 | 2 | 3");
    }

    [Fact]
    public void Random_String_Generation_Should_Be_Consistent_With_Parameters()
    {
        // Act
        var lowercase = StringHelper.RandomString(50, true);
        var mixedcase = StringHelper.RandomString(50, false);

        // Assert
        lowercase.Should().HaveLength(50);
        mixedcase.Should().HaveLength(50);
        lowercase.Should().Match(s => s.All(char.IsLower));
        mixedcase.Should().Match(s => s.All(char.IsLetter));
    }

    [Fact]
    public void Class_Structure_Should_Be_Static_And_Partial()
    {
        // Act & Assert
        typeof(StringHelper).IsAbstract.Should().BeTrue();
        typeof(StringHelper).IsSealed.Should().BeTrue();
        typeof(StringHelper).IsPublic.Should().BeTrue();
    }

    #endregion
}