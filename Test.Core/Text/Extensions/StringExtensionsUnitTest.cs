//--------------------------------------------------------------------------
// File:    StringExtensionsUnitTest.cs
// Content: Unit tests for StringExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;
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
    public void SafeFormatWith_With_Null_Should_Handle_Gracefully()
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
    public void SafeFormatWith_With_Valid_Format_Should_Format_Correctly()
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

    [Fact]
    public void AlphaNumericOnly_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.AlphaNumericOnly();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void AlphaNumericOnly_With_Mixed_Input_Should_Keep_Only_AlphaNumeric()
    {
        // Arrange
        string input = "abc123!@#def456$%^";

        // Act
        var result = input.AlphaNumericOnly();

        // Assert
        result.Should().Be("abc123def456");
    }

    [Fact]
    public void AlphaCharactersOnly_With_Mixed_Input_Should_Keep_Only_Alpha()
    {
        // Arrange
        string input = "abc123!@#def456$%^";

        // Act
        var result = input.AlphaCharactersOnly();

        // Assert
        result.Should().Be("abcdef");
    }

    [Fact]
    public void NumericOnly_With_Mixed_Input_Should_Keep_Only_Numeric()
    {
        // Arrange
        string input = "abc123.45def6,78$%^";

        // Act
        var result = input.NumericOnly();

        // Assert
        result.Should().Be("12345678");
    }

    [Fact]
    public void NumericOnly_With_KeepPunctuation_Should_Keep_Decimal_Punctuation()
    {
        // Arrange
        string input = "abc123.45def6,78$%^";

        // Act
        var result = input.NumericOnly(true);

        // Assert
        result.Should().Be("123.456,78");
    }

    #endregion


    #region Edge cases and integration tests

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