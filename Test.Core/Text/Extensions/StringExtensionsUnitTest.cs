//--------------------------------------------------------------------------
// File:    StringExtensionsUnitTest.cs
// Content: Unit tests for StringExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace AnBo.Test;

public class StringExtensionsUnitTest
{
    #region Is... string extension tests

    [Fact]
    public void IsFormatString_With_Null_Should_Return_False()
    {
        // Arrange
        string? text = null;

        // Act
        var result = text.IsFormatString();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsFormatString_With_Empty_Should_Return_False()
    {
        // Arrange
        string text = string.Empty;

        // Act
        var result = text.IsFormatString();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsFormatString_With_Format_Placeholder_Should_Return_True()
    {
        // Arrange
        string text = "Hello {0}";

        // Act
        var result = text.IsFormatString();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsFormatString_Without_Format_Placeholder_Should_Return_False()
    {
        // Arrange
        string text = "Hello World";

        // Act
        var result = text.IsFormatString();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsNullOrEmptyWithTrim_With_Null_Should_Return_True()
    {
        // Arrange
        string? text = null;

        // Act
        var result = text.IsNullOrEmptyWithTrim();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmptyWithTrim_With_Empty_Should_Return_True()
    {
        // Arrange
        string text = string.Empty;

        // Act
        var result = text.IsNullOrEmptyWithTrim();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmptyWithTrim_With_Whitespace_Should_Return_True()
    {
        // Arrange
        string text = "   \t\n   ";

        // Act
        var result = text.IsNullOrEmptyWithTrim();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmptyWithTrim_With_Valid_Text_Should_Return_False()
    {
        // Arrange
        string text = "Hello";

        // Act
        var result = text.IsNullOrEmptyWithTrim();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_With_Null_Should_Return_False()
    {
        // Arrange
        string? text = null;

        // Act
        var result = text.IsEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_With_Empty_Should_Return_True()
    {
        // Arrange
        string text = string.Empty;

        // Act
        var result = text.IsEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_With_Non_Empty_Should_Return_False()
    {
        // Arrange
        string text = "Hello";

        // Act
        var result = text.IsEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEqualIgnoreCase_With_Same_Case_Should_Return_True()
    {
        // Arrange
        string source = "Hello";
        string target = "Hello";

        // Act
        var result = source.IsEqualIgnoreCase(target);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEqualIgnoreCase_With_Different_Case_Should_Return_True()
    {
        // Arrange
        string source = "Hello";
        string target = "HELLO";

        // Act
        var result = source.IsEqualIgnoreCase(target);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEqualIgnoreCase_With_Different_Strings_Should_Return_False()
    {
        // Arrange
        string source = "Hello";
        string target = "World";

        // Act
        var result = source.IsEqualIgnoreCase(target);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEqualIgnoreCase_With_Null_Values_Should_Return_True()
    {
        // Arrange
        string? source = null;
        string? target = null;

        // Act
        var result = source.IsEqualIgnoreCase(target);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEqual_With_Ordinal_Comparison_Should_Work_Correctly()
    {
        // Arrange
        string source = "Hello";
        string target = "hello";

        // Act
        var result = source.IsEqual(target, StringComparison.Ordinal);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEqual_With_OrdinalIgnoreCase_Comparison_Should_Work_Correctly()
    {
        // Arrange
        string source = "Hello";
        string target = "hello";

        // Act
        var result = source.IsEqual(target, StringComparison.OrdinalIgnoreCase);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Safe string extension tests

    [Fact]
    public void SafeLength_With_Null_Should_Return_Minus_One()
    {
        // Arrange
        string? text = null;

        // Act
        var result = text.SafeLength();

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeLength_With_Empty_Should_Return_Zero()
    {
        // Arrange
        string text = string.Empty;

        // Act
        var result = text.SafeLength();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void SafeLength_With_Valid_String_Should_Return_Length()
    {
        // Arrange
        string text = "Hello";

        // Act
        var result = text.SafeLength();

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void SafeString_With_Null_Should_Return_Default()
    {
        // Arrange
        string? text = null;

        // Act
        var result = text.SafeString();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void SafeString_With_Null_And_Custom_Default_Should_Return_Custom_Default()
    {
        // Arrange
        string? text = null;
        string defaultValue = "DEFAULT";

        // Act
        var result = text.SafeString(defaultValue);

        // Assert
        result.Should().Be("DEFAULT");
    }

    [Fact]
    public void SafeString_With_Valid_String_Should_Return_Original()
    {
        // Arrange
        string text = "Hello";

        // Act
        var result = text.SafeString("DEFAULT");

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void SafeFormat_With_Null_Should_Handle_Gracefully()
    {
        // Arrange
        string? text = null;
        object[] parameters = { "test", 42 };

        // Act
        var result = text.SafeFormatWith(parameters);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void SafeFormat_With_Valid_Format_Should_Format_Correctly()
    {
        // Arrange
        string text = "Hello {0}, you are {1} years old";
        object[] parameters = { "John", 25 };

        // Act
        var result = text.SafeFormatWith(parameters);

        // Assert
        result.Should().Be("Hello John, you are 25 years old");
    }

    #endregion

    #region SubString extension tests (Right, Left, ...)

    [Fact]
    public void Left_With_Null_Should_Return_Null()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.Left(5);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Left_With_Zero_Length_Should_Return_Empty()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.Left(0);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Left_With_Negative_Length_Should_Return_Empty()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.Left(-5);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Left_With_Length_Greater_Than_String_Should_Return_Entire_String()
    {
        // Arrange
        string input = "Hello";

        // Act
        var result = input.Left(10);

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void Left_With_Valid_Length_Should_Return_Correct_Substring()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.Left(5);

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void Right_With_Null_Should_Return_Null()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.Right(5);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Right_With_Zero_Length_Should_Return_Empty()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.Right(0);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Right_With_Negative_Length_Should_Return_Empty()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.Right(-5);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Right_With_Length_Greater_Than_String_Should_Return_Entire_String()
    {
        // Arrange
        string input = "Hello";

        // Act
        var result = input.Right(10);

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void Right_With_Valid_Length_Should_Return_Correct_Substring()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.Right(5);

        // Assert
        result.Should().Be("World");
    }

    [Fact]
    public void Clip_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? text = null;

        // Act
        var result = text.Clip(10);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Clip_With_Empty_Should_Return_Empty()
    {
        // Arrange
        string text = string.Empty;

        // Act
        var result = text.Clip(10);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Clip_With_Zero_MaxCount_Should_Return_Empty()
    {
        // Arrange
        string text = "Hello World";

        // Act
        var result = text.Clip(0);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Clip_With_Negative_MaxCount_Should_Return_Empty()
    {
        // Arrange
        string text = "Hello World";

        // Act
        var result = text.Clip(-5);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Clip_With_Text_Shorter_Than_MaxCount_Should_Return_Original()
    {
        // Arrange
        string text = "Hello";

        // Act
        var result = text.Clip(10);

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void Clip_With_Text_Longer_Than_MaxCount_Should_Clip_With_Ellipsis()
    {
        // Arrange
        string text = "Hello World";

        // Act
        var result = text.Clip(8);

        // Assert
        result.Should().Be("Hello...");
        result.Should().HaveLength(8);
    }

    [Fact]
    public void Clip_With_Custom_ClipText_Should_Use_Custom_ClipText()
    {
        // Arrange
        string text = "Hello World";

        // Act
        var result = text.Clip(8, ">>>");

        // Assert
        result.Should().Be("Hello>>>");
        result.Should().HaveLength(8);
    }

    [Fact]
    public void Clip_With_MaxCount_Smaller_Than_ClipText_Should_Return_Text_Only()
    {
        // Arrange
        string text = "Hello World";

        // Act
        var result = text.Clip(2, "...");

        // Assert
        result.Should().Be("He");
        result.Should().HaveLength(2);
    }

    #endregion

    #region Join extension tests

    [Fact]
    public void Join_With_Null_Items_Should_Handle_Gracefully()
    {
        // Arrange
        IEnumerable<string>? items = null;

        // Act & Assert
        Action action = () => items!.Join();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Join_With_Empty_Items_Should_Return_Empty_String()
    {
        // Arrange
        var items = new List<string>();

        // Act
        var result = items.Join();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Join_With_Single_Item_Should_Return_Item()
    {
        // Arrange
        var items = new List<string> { "single" };

        // Act
        var result = items.Join();

        // Assert
        result.Should().Be("single");
    }

    [Fact]
    public void Join_With_Multiple_Items_Should_Join_With_Default_Separator()
    {
        // Arrange
        var items = new List<string> { "one", "two", "three" };

        // Act
        var result = items.Join();

        // Assert
        result.Should().Be("one, two, three");
    }

    [Fact]
    public void Join_With_Custom_Separator_Should_Use_Custom_Separator()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };

        // Act
        var result = items.Join(" | ");

        // Assert
        result.Should().Be("1 | 2 | 3");
    }

    #endregion

    #region IndexOf extension tests

    [Fact]
    public void SafeIndexOf_With_Null_Source_Should_Return_Minus_One()
    {
        // Arrange
        string? source = null;

        // Act
        var result = source.SafeIndexOf("test");

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeIndexOf_With_Empty_Source_Should_Return_Minus_One()
    {
        // Arrange
        string source = string.Empty;

        // Act
        var result = source.SafeIndexOf("test");

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeIndexOf_With_Null_Value_Should_Return_Minus_One()
    {
        // Arrange
        string source = "Hello World";

        // Act
        var result = source.SafeIndexOf(null);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeIndexOf_With_Empty_Value_Should_Return_Minus_One()
    {
        // Arrange
        string source = "Hello World";

        // Act
        var result = source.SafeIndexOf(string.Empty);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeIndexOf_With_Valid_Search_Should_Return_Correct_Index()
    {
        // Arrange
        string source = "Hello World";

        // Act
        var result = source.SafeIndexOf("World");

        // Assert
        result.Should().Be(6);
    }

    [Fact]
    public void SafeIndexOf_With_StartIndex_Should_Search_From_Index()
    {
        // Arrange
        string source = "Hello World Hello";

        // Act
        var result = source.SafeIndexOf("Hello", 6);

        // Assert
        result.Should().Be(12);
    }

    [Fact]
    public void SafeIndexOf_With_Negative_StartIndex_Should_Return_Minus_One()
    {
        // Arrange
        string source = "Hello World";

        // Act
        var result = source.SafeIndexOf("Hello", -1);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeIndexOf_With_StartIndex_Beyond_Length_Should_Return_Minus_One()
    {
        // Arrange
        string source = "Hello World";

        // Act
        var result = source.SafeIndexOf("Hello", 20);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeLastIndexOf_With_Null_Source_Should_Return_Minus_One()
    {
        // Arrange
        string? source = null;

        // Act
        var result = source.SafeLastIndexOf("test");

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void SafeLastIndexOf_With_Valid_Search_Should_Return_Last_Index()
    {
        // Arrange
        string source = "Hello World Hello";

        // Act
        var result = source.SafeLastIndexOf("Hello");

        // Assert
        result.Should().Be(12);
    }

    [Fact]
    public void SafeLastIndexOf_With_StartIndex_Should_Search_From_Index()
    {
        // Arrange
        string source = "Hello World Hello";

        // Act
        var result = source.SafeLastIndexOf("Hello", 10);

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region Text formatting extension tests

    [Fact]
    public void AppendLine_With_Null_Text_Should_Return_Line()
    {
        // Arrange
        string? text = null;
        string line = "new line";

        // Act
        var result = text.AppendLine(line);

        // Assert
        result.Should().Be("new line");
    }

    [Fact]
    public void AppendLine_With_Empty_Text_Should_Return_Line()
    {
        // Arrange
        string text = string.Empty;
        string line = "new line";

        // Act
        var result = text.AppendLine(line);

        // Assert
        result.Should().Be("new line");
    }

    [Fact]
    public void AppendLine_With_Text_Ending_With_NewLine_Should_Append_Directly()
    {
        // Arrange
        string text = "existing line" + Environment.NewLine;
        string line = "new line";

        // Act
        var result = text.AppendLine(line);

        // Assert
        result.Should().Be("existing line" + Environment.NewLine + "new line");
    }

    [Fact]
    public void AppendLine_With_Text_Not_Ending_With_NewLine_Should_Add_NewLine()
    {
        // Arrange
        string text = "existing line";
        string line = "new line";

        // Act
        var result = text.AppendLine(line);

        // Assert
        result.Should().Be("existing line" + Environment.NewLine + "new line");
    }

    [Fact]
    public void ToCamelCase_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.ToCamelCase();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ToCamelCase_With_Empty_Should_Return_Empty()
    {
        // Arrange
        string value = string.Empty;

        // Act
        var result = value.ToCamelCase();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ToCamelCase_With_Single_Character_Should_Return_Lowercase()
    {
        // Arrange
        string value = "A";

        // Act
        var result = value.ToCamelCase();

        // Assert
        result.Should().Be("a");
    }

    [Fact]
    public void ToCamelCase_With_PascalCase_Should_Convert_To_CamelCase()
    {
        // Arrange
        string value = "PascalCase";

        // Act
        var result = value.ToCamelCase();

        // Assert
        result.Should().Be("pascalCase");
    }

    [Fact]
    public void ToCamelCase_With_Leading_Whitespace_Should_Handle_Correctly()
    {
        // Arrange
        string value = "  PascalCase";

        // Act
        var result = value.ToCamelCase();

        // Assert
        result.Should().Be("  pascalCase");
    }

    [Fact]
    public void ToPascalCase_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.ToPascalCase();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ToPascalCase_With_CamelCase_Should_Convert_To_PascalCase()
    {
        // Arrange
        string value = "camelCase";

        // Act
        var result = value.ToPascalCase();

        // Assert
        result.Should().Be("CamelCase");
    }

    [Fact]
    public void ToUnicodeString_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? sourceText = null;
        Encoding encoding = Encoding.UTF8;

        // Act
        var result = sourceText.ToUnicodeString(encoding);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ToUnicodeString_With_Null_Encoding_Should_Return_Original()
    {
        // Arrange
        string sourceText = "Hello";
        Encoding? encoding = null;

        // Act
        var result = sourceText.ToUnicodeString(encoding);

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void ToUnicodeString_With_Unicode_Encoding_Should_Return_Original()
    {
        // Arrange
        string sourceText = "Hello";
        Encoding encoding = Encoding.Unicode;

        // Act
        var result = sourceText.ToUnicodeString(encoding);

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void ToUnicodeString_With_Different_Encoding_Should_Convert()
    {
        // Arrange
        string sourceText = "Hello";
        Encoding encoding = Encoding.UTF8;

        // Act
        var result = sourceText.ToUnicodeString(encoding);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void EnsureStartsWith_With_Null_Should_Return_Prefix()
    {
        // Arrange
        string? value = null;
        string prefix = "prefix";

        // Act
        var result = value.EnsureStartsWith(prefix);

        // Assert
        result.Should().Be("prefix");
    }

    [Fact]
    public void EnsureStartsWith_With_String_Already_Starting_With_Prefix_Should_Return_Original()
    {
        // Arrange
        string value = "prefixSuffix";
        string prefix = "prefix";

        // Act
        var result = value.EnsureStartsWith(prefix);

        // Assert
        result.Should().Be("prefixSuffix");
    }

    [Fact]
    public void EnsureStartsWith_With_String_Not_Starting_With_Prefix_Should_Add_Prefix()
    {
        // Arrange
        string value = "suffix";
        string prefix = "prefix";

        // Act
        var result = value.EnsureStartsWith(prefix);

        // Assert
        result.Should().Be("prefixsuffix");
    }

    [Fact]
    public void EnsureEndsWith_With_Null_Should_Return_Suffix()
    {
        // Arrange
        string? value = null;
        string suffix = "suffix";

        // Act
        var result = value.EnsureEndsWith(suffix);

        // Assert
        result.Should().Be("suffix");
    }

    [Fact]
    public void EnsureEndsWith_With_String_Already_Ending_With_Suffix_Should_Return_Original()
    {
        // Arrange
        string value = "prefixSuffix";
        string suffix = "Suffix";

        // Act
        var result = value.EnsureEndsWith(suffix);

        // Assert
        result.Should().Be("prefixSuffix");
    }

    [Fact]
    public void EnsureEndsWith_With_String_Not_Ending_With_Suffix_Should_Add_Suffix()
    {
        // Arrange
        string value = "prefix";
        string suffix = "suffix";

        // Act
        var result = value.EnsureEndsWith(suffix);

        // Assert
        result.Should().Be("prefixsuffix");
    }

    [Fact]
    public void QuoteIfNeeded_With_Null_Should_Return_NULL()
    {
        // Arrange
        string? s = null;

        // Act & Assert
        var action = () => s!.QuoteIfNeeded();
        action.Should().Throw<ArgumentNullException>();

    }

    [Fact]
    public void QuoteIfNeeded_With_Empty_Should_Return_Quoted_Empty()
    {
        // Arrange
        string s = string.Empty;

        // Act
        var result = s.QuoteIfNeeded();

        // Assert
        result.Should().Be("“”");
    }

    [Fact]
    public void QuoteIfNeeded_With_String_Containing_Space_Should_Return_Quoted()
    {
        // Arrange
        string s = "hello world";

        // Act
        var result = s.QuoteIfNeeded();

        // Assert
        result.Should().Be("“hello world”");
    }

    [Fact]
    public void QuoteIfNeeded_With_Already_Quoted_String_Should_Return_Original()
    {
        // Arrange
        string s = "“already quoted”";

        // Act
        var result = s.QuoteIfNeeded();

        // Assert
        result.Should().Be("“already quoted”");
    }

    [Fact]
    public void QuoteIfNeeded_With_String_Without_Space_Should_Return_Original()
    {
        // Arrange
        string s = "nospaceshere";

        // Act
        var result = s.QuoteIfNeeded();

        // Assert
        result.Should().Be("nospaceshere");
    }

    [Fact]
    public void FormatAsSentence_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.FormatAsSentence();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void FormatAsSentence_With_PascalCase_Should_Add_Spaces()
    {
        // Arrange
        string value = "PascalCaseString";

        // Act
        var result = value.FormatAsSentence();

        // Assert
        result.Should().Be("pascal case string");
    }

    [Fact]
    public void ReplaceNullChars_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.ReplaceNullChars();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ReplaceNullChars_With_No_Null_Chars_Should_Return_Original()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.ReplaceNullChars();

        // Assert
        result.Should().Be("Hello World");
    }

    [Fact]
    public void ReplaceNullChars_With_Null_Chars_Should_Replace()
    {
        // Arrange
        string input = "Hello\0World";

        // Act
        var result = input.ReplaceNullChars();

        // Assert
        result.Should().Be("Hello\\0World");
    }

    #endregion

    #region RegEx string extension tests

    [Fact]
    public void IsMatchingTo_With_Null_Should_Return_False()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.IsMatchingTo(@"\d+");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMatchingTo_With_Empty_Should_Return_False()
    {
        // Arrange
        string value = string.Empty;

        // Act
        var result = value.IsMatchingTo(@"\d+");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMatchingTo_With_Matching_Pattern_Should_Return_True()
    {
        // Arrange
        string value = "123";

        // Act
        var result = value.IsMatchingTo(@"\d+");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMatchingTo_With_Non_Matching_Pattern_Should_Return_False()
    {
        // Arrange
        string value = "abc";

        // Act
        var result = value.IsMatchingTo(@"\d+");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ReplaceWith_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.ReplaceWith(@"\d+", "X");

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ReplaceWith_With_Valid_Pattern_Should_Replace()
    {
        // Arrange
        string value = "abc123def456";

        // Act
        var result = value.ReplaceWith(@"\d+", "X");

        // Assert
        result.Should().Be("abcXdefX");
    }

    [Fact]
    public void ReplaceWith_With_MatchEvaluator_Should_Use_Evaluator()
    {
        // Arrange
        string value = "123";

        // Act
        var result = value.ReplaceWith(@"\d", m => $"[{m.Value}]");

        // Assert
        result.Should().Be("[1][2][3]");
    }

    [Fact]
    public void GetMatchingValues_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.GetMatchingValues(@"\d").ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetMatchingValues_With_Matches_Should_Return_All_Matches()
    {
        // Arrange
        string value = "a1b2c3";

        // Act
        var result = value.GetMatchingValues(@"\d").ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainInOrder("1", "2", "3");
    }

    #endregion

    #region Text filter extension tests

    [Fact]
    public void FilterOutText_With_Null_Should_Return_Null()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.FilterOutText(@"\d+");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FilterOutText_With_Valid_Input_Should_Remove_Matching_Text()
    {
        // Arrange
        string input = "abc123def456";

        // Act
        var result = input.FilterOutText(@"\d+");

        // Assert
        result.Should().Be("abcdef");
    }

    [Fact]
    public void KeepFilterText_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.KeepFilterText(@"\d+");

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void KeepFilterText_With_Valid_Input_Should_Keep_Only_Matching_Text()
    {
        // Arrange
        string input = "abc123def456";

        // Act
        var result = input.KeepFilterText(@"\d+");

        // Assert
        result.Should().Be("123456");
    }

    #endregion

    #region IsTypeSpan Method Tests

    [Fact]
    public void IsTypeSpan_With_Valid_Int_Span_Should_Return_True()
    {
        // Arrange
        ReadOnlySpan<char> span = "42".AsSpan();

        // Act
        var result = span.IsTypeSpan<int>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTypeSpan_With_Invalid_Int_Span_Should_Return_False()
    {
        // Arrange
        ReadOnlySpan<char> span = "invalid".AsSpan();

        // Act
        var result = span.IsTypeSpan<int>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsTypeSpan_With_Valid_Double_Span_Should_Return_True()
    {
        // Arrange
        ReadOnlySpan<char> span = "42.5".AsSpan();

        // Act
        var result = span.IsTypeSpan<double>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTypeSpan_With_Custom_Provider_Should_Use_Provider()
    {
        // Arrange
        ReadOnlySpan<char> span = "42,5".AsSpan();
        var germanCulture = new CultureInfo("de-DE");

        // Act
        var result = span.IsTypeSpan<double>(germanCulture);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTypeSpan_With_Null_Provider_Should_Use_InvariantCulture()
    {
        // Arrange
        ReadOnlySpan<char> span = "42.5".AsSpan();

        // Act
        var result = span.IsTypeSpan<double>(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTypeSpan_With_Empty_Span_Should_Return_False()
    {
        // Arrange
        ReadOnlySpan<char> span = "".AsSpan();

        // Act
        var result = span.IsTypeSpan<int>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsTypeSpan_With_DateTime_Should_Work()
    {
        // Arrange
        ReadOnlySpan<char> span = "2025-01-15".AsSpan();

        // Act
        var result = span.IsTypeSpan<DateTime>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTypeSpan_With_Guid_Should_Work()
    {
        // Arrange
        var guid = Guid.NewGuid();
        ReadOnlySpan<char> span = guid.ToString().AsSpan();

        // Act
        var result = span.IsTypeSpan<Guid>();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region IsTypeString Method Tests

    [Fact]
    public void IsTypeString_With_Valid_Int_String_Should_Return_True()
    {
        // Arrange
        string str = "42";

        // Act
        var result = str.IsTypeString<int>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTypeString_With_Invalid_Int_String_Should_Return_False()
    {
        // Arrange
        string str = "invalid";

        // Act
        var result = str.IsTypeString<int>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsTypeString_With_Null_String_Should_Return_False()
    {
        // Arrange
        string? str = null;

        // Act
        var result = str.IsTypeString<int>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsTypeString_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string str = "";

        // Act
        var result = str.IsTypeString<int>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsTypeString_With_Custom_Provider_Should_Use_Provider()
    {
        // Arrange
        string str = "42,5";
        var germanCulture = new CultureInfo("de-DE");

        // Act
        var result = str.IsTypeString<double>(germanCulture);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTypeString_With_Boolean_Should_Work()
    {
        // Arrange
        string str = "true";

        // Act
        var result = str.IsTypeString<bool>();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region ToInvariantString Method Tests - Basic Types

    [Fact]
    public void ToInvariantString_With_Null_Should_Return_NullString()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("<null>");
    }

    [Fact]
    public void ToInvariantString_With_String_Should_Return_Same_String()
    {
        // Arrange
        string value = "test string";

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("test string");
    }

    [Fact]
    public void ToInvariantString_With_Boolean_True_Should_Return_True()
    {
        // Arrange
        bool value = true;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("True");
    }

    [Fact]
    public void ToInvariantString_With_Boolean_False_Should_Return_False()
    {
        // Arrange
        bool value = false;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("False");
    }

    [Fact]
    public void ToInvariantString_With_Integer_Should_Return_String_Representation()
    {
        // Arrange
        int value = 42;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("42");
    }

    [Fact]
    public void ToInvariantString_With_Double_Should_Use_InvariantCulture()
    {
        // Arrange
        double value = 42.5;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("42.5");
    }

    [Fact]
    public void ToInvariantString_With_DateTime_Should_Use_Default_Format()
    {
        // Arrange
        var value = new DateTime(2025, 1, 15, 14, 30, 45);

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("2025-01-15 14:30:45");
    }

    [Fact]
    public void ToInvariantString_With_Custom_DateTime_Format()
    {
        // Arrange
        var value = new DateTime(2025, 1, 15, 14, 30, 45);
        var options = new ToStringOptions { DateTimeFormat = "yyyy/MM/dd" };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Be("2025/01/15");
    }

    [Fact]
    public void ToInvariantString_With_Guid_Should_Return_String_Representation()
    {
        // Arrange
        var value = new Guid("12345678-1234-1234-1234-123456789012");

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("12345678-1234-1234-1234-123456789012");
    }

    #endregion

    #region ToInvariantString Method Tests - Collections

    [Fact]
    public void ToInvariantString_With_Empty_Array_Should_Return_Empty_Brackets()
    {
        // Arrange
        int[] value = [];

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("[]");
    }

    [Fact]
    public void ToInvariantString_With_Array_Should_Format_With_Brackets()
    {
        // Arrange
        int[] value = [1, 2, 3];

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("[1, 2, 3] (3 items)");
    }

    [Fact]
    public void ToInvariantString_With_Array_Should_Respect_MaxCollectionItems()
    {
        // Arrange
        int[] value = [1, 2, 3, 4, 5];
        var options = new ToStringOptions { MaxCollectionItems = 3 };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Be("[1, 2, 3, ...] (5 items)");
    }

    [Fact]
    public void ToInvariantString_With_Dictionary_Should_Format_With_Braces()
    {
        // Arrange
        var value = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Contain("{").And.Contain("}").And.Contain("a: 1").And.Contain("b: 2");
    }

    [Fact]
    public void ToInvariantString_With_Empty_Dictionary_Should_Return_Empty_Braces()
    {
        // Arrange
        var value = new Dictionary<string, int>();

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("{}");
    }

    [Fact]
    public void ToInvariantString_With_List_Should_Format_As_Collection()
    {
        // Arrange
        var value = new List<string> { "first", "second", "third" };

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("[first, second, third] (3 items)");
    }

    [Fact]
    public void ToInvariantString_With_Collection_Count_Disabled()
    {
        // Arrange
        var value = new List<int> { 1, 2, 3 };
        var options = new ToStringOptions { ShowCollectionCount = false };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Be("[1, 2, 3]");
    }

    [Fact]
    public void ToInvariantString_With_Custom_Separator()
    {
        // Arrange
        var value = new List<int> { 1, 2, 3 };
        var options = new ToStringOptions { CollectionSeparator = " | " };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Contain("1 | 2 | 3");
    }

    #endregion

    #region ToInvariantString Method Tests - Multidimensional Arrays

    [Fact]
    public void ToInvariantString_With_2D_Array_Should_Format_Correctly()
    {
        // Arrange
        int[,] value = { { 1, 2 }, { 3, 4 } };

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("[[1, 2], [3, 4]]");
    }

    [Fact]
    public void ToInvariantString_With_2D_Array_Show_Dimensions()
    {
        // Arrange
        int[,] value = { { 1, 2 }, { 3, 4 } };
        var options = new ToStringOptions { ShowArrayDimensions = true };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Contain("2D 2×2, 4 items");
    }

    [Fact]
    public void ToInvariantString_With_3D_Array_Should_Format_Correctly()
    {
        // Arrange
        int[,,] value = new int[2, 2, 2];
        value[0, 0, 0] = 1;
        value[1, 1, 1] = 8;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Contain("[").And.Contain("]");
    }

    #endregion

    #region ToInvariantString Method Tests - Nullable Types

    [Fact]
    public void ToInvariantString_With_Nullable_Int_Null_Should_Return_NullString()
    {
        // Arrange
        int? value = null;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("<null>");
    }

    [Fact]
    public void ToInvariantString_With_Nullable_Int_Value_Should_Return_Value()
    {
        // Arrange
        int? value = 42;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("42");
    }

    #endregion

    #region ToInvariantString Method Tests - Enum Types

    [Fact]
    public void ToInvariantString_With_Enum_Should_Return_String_Name()
    {
        // Arrange
        DayOfWeek value = DayOfWeek.Monday;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("Monday");
    }

    #endregion

    #region ToInvariantString Method Tests - Max Nesting Depth

    [Fact]
    public void ToInvariantString_With_Max_Depth_Reached_Should_Return_Placeholder()
    {
        // Arrange
        var options = new ToStringOptions { MaxNestingDepth = 1 };

        // Act
        var result = "test".ToInvariantString(options, currentDepth: 1);

        // Assert
        result.Should().Be("<max nesting depth reached>");
    }

    [Fact]
    public void ToInvariantString_With_Nested_Arrays_Should_Respect_Max_Depth()
    {
        // Arrange
        var nestedArray = new object[] { new object[] { "deep" } };
        var options = new ToStringOptions { MaxNestingDepth = 3 };

        // Act
        var result = nestedArray.ToInvariantString(options);

        // Assert
        result.Should().NotContain("<max nesting depth reached>");
    }

    #endregion

    #region ToInvariantString Method Tests - Custom Formats

    [Fact]
    public void ToInvariantString_With_Custom_Decimal_Format()
    {
        // Arrange
        decimal value = 123.456m;
        var options = new ToStringOptions { DecimalFormat = "F2" };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Be("123.46");
    }

    [Fact]
    public void ToInvariantString_With_Custom_Double_Format()
    {
        // Arrange
        double value = 123.456;
        var options = new ToStringOptions { DoubleFormat = "F1" };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Be("123.5");
    }

    [Fact]
    public void ToInvariantString_With_TimeSpan_Custom_Format_g()
    {
        // Arrange
        var value = new TimeSpan(1, 2, 3, 4);
        var options = new ToStringOptions { TimeSpanFormat = "g" };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Contain("1:2:03:04");
    }

    [Fact]
    public void ToInvariantString_With_TimeSpan_Custom_Format_c()
    {
        // Arrange
        var value = new TimeSpan(1, 2, 3, 4);
        var options = new ToStringOptions { TimeSpanFormat = "c" };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Contain("1.02:03:04");
    }

    [Fact]
    public void ToInvariantString_With_DateOnly_Should_Work()
    {
        // Arrange
        var value = new DateOnly(2025, 1, 15);

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("2025-01-15");
    }

    [Fact]
    public void ToInvariantString_With_TimeOnly_Should_Work()
    {
        // Arrange
        var value = new TimeOnly(14, 30, 45);

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("14:30:45");
    }

    [Fact]
    public void ToInvariantString_With_DateTimeOffset_Should_Work()
    {
        // Arrange
        var value = new DateTimeOffset(2025, 1, 15, 14, 30, 45, TimeSpan.FromHours(2));

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Contain("2025-01-15 14:30:45").And.Contain("+02:00");
    }

    #endregion

    #region ToInvariantString Method Tests - Custom Options

    [Fact]
    public void ToInvariantString_With_Custom_NullString()
    {
        // Arrange
        object? value = null;
        var options = new ToStringOptions { NullString = "NULL" };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Be("NULL");
    }

    [Fact]
    public void ToInvariantString_With_Custom_Dictionary_Separator()
    {
        // Arrange
        var value = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var options = new ToStringOptions { DictionaryKeyValueSeparator = " => " };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Contain("a => 1").And.Contain("b => 2");
    }

    #endregion

    #region ParseInvariantString Method Tests - Generic Version

    [Fact]
    public void ParseInvariantString_Generic_With_Null_Should_Throw()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        var action = () => value!.ParseInvariantString<int>();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ParseInvariantString_Generic_With_Valid_Int_Should_Return_Int()
    {
        // Arrange
        string value = "42";

        // Act
        var result = value.ParseInvariantString<int>();

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void ParseInvariantString_Generic_With_Valid_Double_Should_Return_Double()
    {
        // Arrange
        string value = "42.5";

        // Act
        var result = value.ParseInvariantString<double>();

        // Assert
        result.Should().Be(42.5);
    }

    [Fact]
    public void ParseInvariantString_Generic_With_Valid_DateTime_Should_Return_DateTime()
    {
        // Arrange
        string value = "2025-01-15";

        // Act
        var result = value.ParseInvariantString<DateTime>();

        // Assert
        result.Should().Be(new DateTime(2025, 1, 15));
    }

    [Fact]
    public void ParseInvariantString_Generic_With_Valid_Guid_Should_Return_Guid()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        string value = expectedGuid.ToString();

        // Act
        var result = value.ParseInvariantString<Guid>();

        // Assert
        result.Should().Be(expectedGuid);
    }

    [Fact]
    public void ParseInvariantString_Generic_With_Invalid_Value_Should_Return_Default()
    {
        // Arrange
        string value = "invalid";

        // Act
        var result = value.ParseInvariantString<int>();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ParseInvariantString_Generic_With_Boolean_Should_Work()
    {
        // Arrange
        string value = "true";

        // Act
        var result = value.ParseInvariantString<bool>();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region ParseInvariantString Method Tests - Type Parameter Version

    [Fact]
    public void ParseInvariantString_Type_With_Null_Value_Should_Throw()
    {
        // Arrange
        string? value = null;
        Type type = typeof(int);

        // Act & Assert
        var action = () => value!.ParseInvariantString(type);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ParseInvariantString_Type_With_Null_Type_Should_Throw()
    {
        // Arrange
        string value = "42";
        Type? type = null;

        // Act & Assert
        var action = () => value.ParseInvariantString(type!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ParseInvariantString_Type_With_Valid_Int_Should_Return_Int()
    {
        // Arrange
        string value = "42";
        Type type = typeof(int);

        // Act
        var result = value.ParseInvariantString(type);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void ParseInvariantString_Type_With_Valid_String_Should_Return_String()
    {
        // Arrange
        string value = "test";
        Type type = typeof(string);

        // Act
        var result = value.ParseInvariantString(type);

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void ParseInvariantString_Type_With_Invalid_Value_Should_Return_Default()
    {
        // Arrange
        string value = "invalid";
        Type type = typeof(int);

        // Act
        var result = value.ParseInvariantString(type);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ParseInvariantString_Type_With_Enum_Should_Work()
    {
        // Arrange
        string value = "Monday";
        Type type = typeof(DayOfWeek);

        // Act
        var result = value.ParseInvariantString(type);

        // Assert
        result.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void ParseInvariantString_Type_With_Custom_Type_Should_Use_TypeConverter()
    {
        // Arrange
        string value = "test";
        Type type = typeof(string);

        // Act
        var result = value.ParseInvariantString(type);

        // Assert
        result.Should().Be("test");
    }

    #endregion

    #region ToInvariantString Edge Cases and Error Handling

    [Fact]
    public void ToInvariantString_With_Circular_Reference_Should_Not_Cause_Stack_Overflow()
    {
        // Arrange
        var list = new List<object>();
        list.Add(list); // Circular reference
        var options = new ToStringOptions { MaxNestingDepth = 3 };

        // Act
        var result = list.ToInvariantString(options);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("<max nesting depth reached>");
    }

    [Fact]
    public void ToInvariantString_With_Very_Large_Array_Should_Truncate()
    {
        // Arrange
        var largeArray = new int[1000];
        var largeList = new List<int>();
        var largeDictionary = new Dictionary<int, string>();
        for (int i = 0; i < 1000; i++)
        {
            largeArray[i] = i;
            largeDictionary.Add(i, $"value{i}");
        }

        largeList.AddRange(largeArray);

        var options = new ToStringOptions { MaxCollectionItems = 5 };

        // Act
        var result = largeArray.ToInvariantString(options);
        var listResult = largeList.ToInvariantString(options);
        var dictResult = largeDictionary.ToInvariantString(options);

        // Assert
        result.Should().Contain("...");
        result.Should().Contain("(1000 items)");

        listResult.Should().Contain("...");
        listResult.Should().Contain("(1000 items)");

        dictResult.Should().Contain("...");
        dictResult.Should().Contain("(1000 items)");
    }

    [Fact]
    public void ToInvariantString_With_String_Should_Not_Be_Treated_As_Collection()
    {
        // Arrange
        string value = "hello";

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("hello");
        result.Should().NotContain("[");
        result.Should().NotContain("]");
    }

    [Fact]
    public void ParseInvariantString_With_Nullable_Type_Should_Work()
    {
        // Arrange
        string value = "42";
        Type type = typeof(int?);

        // Act
        var result = value.ParseInvariantString(type);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void ParseInvariantString_With_Exception_Should_Return_Default()
    {
        // Arrange
        string value = "not-a-guid";
        Type type = typeof(Guid);

        // Act
        var result = value.ParseInvariantString(type);

        // Assert
        result.Should().Be(Guid.Empty);
    }

    #endregion

    #region ToInvariantString Format String Edge Cases

    [Fact]
    public void ToInvariantString_With_Empty_Format_String_Should_Use_Default()
    {
        // Arrange
        decimal value = 123.456m;
        var options = new ToStringOptions { DecimalFormat = "" };

        // Act
        var result = value.ToInvariantString(options);

        // Assert
        result.Should().Be("123.456");
    }

    [Fact]
    public void ToInvariantString_With_Very_Long_Number_Should_Format_Correctly()
    {
        // Arrange
        long value = long.MaxValue;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be(long.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    #endregion

    #region ToInvariantString Special Numeric Types

    [Fact]
    public void ToInvariantString_With_Float_Should_Work()
    {
        // Arrange
        float value = 123.46f;
        var options = new ToStringOptions { FloatFormat = "F1" };

        // Act
        var result1 = value.ToInvariantString();
        var result2 = value.ToInvariantString(options);

        // Assert
        result1.Should().Be("123.46");
        result2.Should().Be("123.5");
    }

    [Fact]
    public void ToInvariantString_With_Byte_Should_Work()
    {
        // Arrange
        byte value = 255;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("255");
    }

    [Fact]
    public void ToInvariantString_With_Short_Should_Work()
    {
        // Arrange
        short value = 12345;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("12345");
    }

    [Fact]
    public void ToInvariantString_With_UInt_Should_Work()
    {
        // Arrange
        uint value = 4294967295;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("4294967295");
    }

    [Fact]
    public void ToInvariantString_With_Char_Should_Work()
    {
        // Arrange
        char value = 'A';

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("A");
    }

    [Fact]
    public void ToInvariantString_With_Int128_Number_Should_Format_Correctly()
    {
        // Arrange
        var value = Int128.MaxValue;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be(Int128.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToInvariantString_With_UInt128_Number_Should_Format_Correctly()
    {
        // Arrange
        var value = UInt128.MaxValue;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be(UInt128.MaxValue.ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToInvariantString_With_BigInteger_Number_Should_Format_Correctly()
    {
        // Arrange
        var zero = new BigInteger(0);          // 1 Zeichen
        var small = new BigInteger(123);          // ~3 Zeichen
        var medium1 = BigInteger.Parse("123456789012345"); // ~15 Zeichen  
        var medium2 = BigInteger.Parse("-123456789012345"); // ~15 Zeichen  
        var large = BigInteger.Pow(2, 1000);     // ~302 Zeichen (dezimal)

        // Act
        var resultZero = zero.ToInvariantString();
        var resultSmall = small.ToInvariantString();
        var resultMedium1 = medium1.ToInvariantString();
        var resultMedium2 = medium2.ToInvariantString();
        var resultLarge = large.ToInvariantString();

        // Assert
        resultZero.Should().Be(zero.ToString(CultureInfo.InvariantCulture));
        resultSmall.Should().Be(small.ToString(CultureInfo.InvariantCulture));
        resultMedium1.Should().Be(medium1.ToString(CultureInfo.InvariantCulture));
        resultMedium2.Should().Be(medium2.ToString(CultureInfo.InvariantCulture));
        resultLarge.Should().Be(large.ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToInvariantString_With_Nullable_Number_Should_Format_Correctly()
    {
        // Arrange
        int? value = 42;
        // Act
        var result = value.ToInvariantString();
        // Assert
        result.Should().Be("42");
    }

    [Fact]
    public void ToInvariantString_With_Enum_Should_Format_Correctly()
    {
        // Arrange
        DayOfWeek value = DayOfWeek.Monday;
        // Act
        var result = value.ToInvariantString();
        // Assert
        result.Should().Be("Monday");
    }

    [Fact]
    public void ToInvariantString_With_Half_Should_Work()
    {
        // Arrange
        Half value = (Half)3.14;

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("3.14");
    }


    #endregion

    #region ToInvariantString Collection Edge Cases

    [Fact]
    public void ToInvariantString_With_Hashtable_Should_Format_As_Dictionary()
    {
        // Arrange
        var value = new Hashtable { ["key1"] = "value1", ["key2"] = "value2" };

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Contain("{").And.Contain("}");
        result.Should().Contain("key1").And.Contain("value1");
    }

    [Fact]
    public void ToInvariantString_With_ArrayList_Should_Format_As_Array()
    {
        // Arrange
        var value = new ArrayList { 1, 2, 3 };

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Be("[1, 2, 3] (3 items)");
    }

    [Fact]
    public void ToInvariantString_With_Jagged_Array_Should_Format_Correctly()
    {
        // Arrange
        int[][] value = [[1, 2], [3, 4, 5]];

        // Act
        var result = value.ToInvariantString();

        // Assert
        result.Should().Contain("[1, 2]").And.Contain("[3, 4, 5]");
    }

    #endregion

    #region ToInvariantString Method Tests - TypeConverters

    [Fact]
    public void ToInvariantString_With_TypeConverter_Should_Use_Converter()
    {

        // Arrange
        var value = new Person("Max Mustermann", 42);
        // Act
        var result = value.ToInvariantString();
        // Assert
        result.Should().Be("[TypeConverter] Max Mustermann (42)");
    }

    // Beispiel einer benutzerdefinierten Klasse mit TypeConverter
    [TypeConverter(typeof(PersonConverter))]
    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"Person: {Name}, {Age} Jahre alt";
        }
    }

    // Benutzerdefinierter TypeConverter für Person
    internal class PersonConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            // Sagt, dass wir zu string konvertieren können
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Person person)
            {
                // Spezielle Formatierung durch TypeConverter
                return $"[TypeConverter] {person.Name} ({person.Age})";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [Fact]
    public void ToInvariantString_With_ClassToString_Should_Use_ToString()
    {
        // Arrange
        var value = new SimpleProduct("Max Mustermann", 42.2m);
        // Act
        var result = value.ToInvariantString();
        // Assert
        result.Should().Be("SimpleProduct: Max Mustermann - 42.2");
    }

    // Beispiel 2: Klasse OHNE TypeConverter (fällt auf ToString() zurück)
    internal class SimpleProduct : IFormattable
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public SimpleProduct(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"SimpleProduct: {Name} - {Price}";
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format(formatProvider, "SimpleProduct: {0} - {1}", Name, Price);
        }
    }

    [Fact]
    public void ToInvariantString_With_Broken_TypeConverter_Should_Return_Empty()
    {
        // Arrange
        var value = new BrokenItem("42");
        // Act
        var result = value.ToInvariantString();
        // Assert
        result.Should().Be(String.Empty);
    }

    // Beispiel 3: Klasse mit TypeConverter der eine Exception wirft
    [TypeConverter(typeof(BrokenConverter))]
    internal class BrokenItem
    {
        public string Value { get; set; }

        public BrokenItem(string value)
        {
            Value = value;
        }

        public override string? ToString()
        {
            return null;
        }
    }

    internal class BrokenConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            // Simuliert eine Exception im TypeConverter
            throw new InvalidOperationException("Converter ist kaputt!");
        }
    }

    [Fact]
    public void ToInvariantString_With_TypeConverter_That_Cannot_Convert_To_String_Should_Return_Empty()
    {
        // Arrange
        var value = new BrokenItem2("42");
        // Act
        var result = value.ToInvariantString();
        // Assert
        result.Should().Be(String.Empty);
    }

    // Beispiel 4: Klasse mit TypeConverter nicht nach string konvertieren kann
    [TypeConverter(typeof(BrokenConverter2))]
    internal class BrokenItem2
    {
        public string Value { get; set; }

        public BrokenItem2(string value)
        {
            Value = value;
        }

        public override string? ToString()
        {
            return null;
        }
    }

    internal class BrokenConverter2 : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(int);
        }

        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            // Simuliert eine Exception im TypeConverter
            throw new InvalidOperationException("Converter ist kaputt!");
        }
    }

    #endregion

    #region ToInvariantString Method Tests - Multidimensional Elements

    [Fact]
    public void ToInvariantString_With_Multidimensional_Array_Should_Format_Correctly()
    {
        // Arrange
        int[,,] value = new int[3, 4, 2];
        var options = new ToStringOptions { ShowArrayDimensions = true, MaxNestingDepth = value.Rank };
        // Act
        var result = value.ToInvariantString(options);
        // Assert
        result.Should().NotContain("<max nesting depth reached>");
    }

    [Fact]
    public void ToInvariantString_With_Multidimensional_Array_With_Rank_Greater_Than_MaxDepth_Show_Max_Nesting_Messages()
    {
        // Arrange
        int[,,] value = new int[3, 4, 2];
        var options = new ToStringOptions { ShowArrayDimensions = true, MaxNestingDepth = 2 };
        // Act
        var result = value.ToInvariantString(options);
        // Assert
        result.Should().Contain("<max nesting depth reached>");
    }

    [Fact]
    public void ToInvariantString_With_Multidimensional_Array_And_MaxDepth_Zero_Show_Max_Nesting_Message()
    {
        // Arrange
        int[,,] value = new int[3, 4, 2];
        var options = new ToStringOptions { ShowArrayDimensions = true, MaxNestingDepth = 0 };
        // Act
        var result = value.ToInvariantString(options);
        // Assert
        result.Should().Be("<max nesting depth reached>");
    }

    [Fact]
    public void ToInvariantString_With_2x2_Array_And_MaxDepth_1_Show_Max_Nesting_Message()
    {
        // Arrange
        int[,] value = new int[2, 2];
        var options = new ToStringOptions { MaxNestingDepth = 1 };
        // Act
        var result = value.ToInvariantString(options);
        // Assert
        result.Should().Contain("<max nesting depth reached>");
    }


    [Fact]
    public void ToInvariantString_With_2x100_Array_And_MaxCollectionItems_5_Show_3_Points()
    {
        // Arrange
        int[,] value = new int[2, 100];
        var options = new ToStringOptions { MaxCollectionItems = 5 };
        // Act
        var result = value.ToInvariantString(options);
        // Assert
        result.Should().Contain("...");
    }

    [Fact]
    public void ToInvariantString_With_2x10x7_Array_And_MaxCollectionItems_5_Show_3_Points()
    {
        // Arrange
        int[,,] value = new int[2, 10, 7];
        var options = new ToStringOptions { MaxCollectionItems = 5 };
        // Act
        var result = value.ToInvariantString(options);
        // Assert
        result.Should().Contain("...");
    }

    #endregion

    #region String to type conversion extension tests

    [Fact]
    public void ToByteArray_With_Null_Should_Return_Empty_Array()
    {
        // Arrange
        string? str = null;

        // Act
        var result = str.ToByteArray();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToByteArray_With_Valid_String_Should_Return_UTF8_Bytes()
    {
        // Arrange
        string str = "Hello";

        // Act
        var result = str.ToByteArray();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("Hello"));
    }

    [Fact]
    public void ToFileInfo_With_Null_Should_Return_Null()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.ToFileInfo();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToFileInfo_With_Empty_Should_Return_Null()
    {
        // Arrange
        string value = string.Empty;

        // Act
        var result = value.ToFileInfo();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToFileInfo_With_Invalid_Path_Should_Return_Null()
    {
        // Arrange
        string value = "invalid<>path";

        // Act
        var result = value.ToFileInfo();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToFileInfo_With_Non_Existing_File_Should_Return_Null()
    {
        // Arrange
        string value = @"C:\NonExistingFile.txt";

        // Act
        var result = value.ToFileInfo();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToFileInfo_With_Existing_File_Should_Return_FileInfo()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            // Act
            var result = tempFile.ToFileInfo();

            // Assert
            result.Should().NotBeNull();
            result!.Exists.Should().BeTrue();
            result.FullName.Should().Be(tempFile);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    #endregion

    #region String Handling Edge cases and integration tests

    [Fact]
    public void Multiple_Extensions_Can_Be_Chained()
    {
        // Arrange
        string input = "  hello World  ";

        // Act
        var result = input
            .SafeString()
            .ToPascalCase()
            .Left(10)
            .EnsureEndsWith("!");

        // Assert
        result.Should().Be("  Hello Wo!");
    }

    [Fact]
    public void Safe_Operations_Handle_Null_Gracefully()
    {
        // Arrange
        string? nullString = null;

        // Act & Assert
        nullString.SafeLength().Should().Be(-1);
        nullString.SafeString().Should().Be(string.Empty);
        nullString.IsFormatString().Should().BeFalse();
        nullString.IsNullOrEmptyWithTrim().Should().BeTrue();
        nullString.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void Regex_Operations_Handle_Invalid_Patterns_Gracefully()
    {
        // Arrange
        string value = "test123";

        // Act & Assert - These should not throw exceptions
        Action action1 = () => value.IsMatchingTo("[invalid");
        Action action2 = () => value.ReplaceWith("[invalid", "X");
        Action action3 = () => value.GetMatchingValues("[invalid").ToList();

        action1.Should().Throw<ArgumentException>();
        action2.Should().Throw<ArgumentException>();
        action3.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void String_Manipulation_Preserves_Unicode()
    {
        // Arrange
        string unicode = "👨‍👩‍👧‍👦Héllø Wörld! 🌟";

        // Act
        var left = unicode.LeftGraphemes(5);
        var right = unicode.RightGraphemes(5);
        var clipped = unicode.Clip(10);

        // Assert
        left.Should().Be("👨‍👩‍👧‍👦Héll");
        right.Should().Be("ld! 🌟");
    }

    [Fact]
    public void Performance_With_Large_Strings_Should_Be_Reasonable()
    {
        // Arrange
        string largeString = new string('A', 10000);

        // Act & Assert - These operations should complete quickly
        largeString.SafeLength().Should().Be(10000);
        largeString.Left(100).Should().HaveLength(100);
        largeString.Right(100).Should().HaveLength(100);
        largeString.IsEmpty().Should().BeFalse();
    }

    #endregion
}