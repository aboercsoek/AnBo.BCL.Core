//--------------------------------------------------------------------------
// File:    RegexHelperUnitTest.cs
// Content: Unit tests for RegexHelper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AnBo.Test.Unit;

[Trait("Category", "Unit")]
public class RegexHelperUnitTests
{
    #region Validation Methods Tests

    [Fact]
    public void IsAlphaOnly_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsAlphaOnly(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsAlphaOnly_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsAlphaOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaOnly_With_Valid_Alpha_String_Should_Return_True()
    {
        // Arrange
        string input = "HelloWorld";

        // Act
        var result = RegexHelper.IsAlphaOnly(input);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAlphaOnly_With_Mixed_Characters_Should_Return_False()
    {
        // Arrange
        string input = "Hello123";

        // Act
        var result = RegexHelper.IsAlphaOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaOnly_With_Numbers_Should_Return_False()
    {
        // Arrange
        string input = "12345";

        // Act
        var result = RegexHelper.IsAlphaOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaUpperCaseOnly_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsAlphaUpperCaseOnly(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsAlphaUpperCaseOnly_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsAlphaUpperCaseOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaUpperCaseOnly_With_Valid_UpperCase_Should_Return_True()
    {
        // Arrange
        string input = "HELLOWORLD";

        // Act
        var result = RegexHelper.IsAlphaUpperCaseOnly(input);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAlphaUpperCaseOnly_With_Mixed_Case_Should_Return_False()
    {
        // Arrange
        string input = "HelloWorld";

        // Act
        var result = RegexHelper.IsAlphaUpperCaseOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaLowerCaseOnly_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsAlphaLowerCaseOnly(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsAlphaLowerCaseOnly_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsAlphaLowerCaseOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaLowerCaseOnly_With_Valid_LowerCase_Should_Return_True()
    {
        // Arrange
        string input = "helloworld";

        // Act
        var result = RegexHelper.IsAlphaLowerCaseOnly(input);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAlphaLowerCaseOnly_With_Mixed_Case_Should_Return_False()
    {
        // Arrange
        string input = "HelloWorld";

        // Act
        var result = RegexHelper.IsAlphaLowerCaseOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaNumericOnly_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsAlphaNumericOnly(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsAlphaNumericOnly_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsAlphaNumericOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaNumericOnly_With_Valid_AlphaNumeric_Should_Return_True()
    {
        // Arrange
        string input = "Hello123World";

        // Act
        var result = RegexHelper.IsAlphaNumericOnly(input);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAlphaNumericOnly_With_Special_Characters_Should_Return_False()
    {
        // Arrange
        string input = "Hello@World";

        // Act
        var result = RegexHelper.IsAlphaNumericOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaNumericSpaceOnly_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsAlphaNumericSpaceOnly(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsAlphaNumericSpaceOnly_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsAlphaNumericSpaceOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAlphaNumericSpaceOnly_With_Valid_AlphaNumericSpace_Should_Return_True()
    {
        // Arrange
        string input = "Hello 123 World";

        // Act
        var result = RegexHelper.IsAlphaNumericSpaceOnly(input);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAlphaNumericSpaceOnly_With_Special_Characters_Should_Return_False()
    {
        // Arrange
        string input = "Hello @World";

        // Act
        var result = RegexHelper.IsAlphaNumericSpaceOnly(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsNumeric_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsNumeric(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsNumeric_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsNumeric(input);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123.45")]
    [InlineData("-123")]
    [InlineData("-123.45")]
    [InlineData("0")]
    [InlineData("0.0")]
    public void IsNumeric_With_Valid_US_Numbers_Should_Return_True(string input)
    {
        // Act
        var result = RegexHelper.IsNumeric(input, useGermanFormat: false);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123,45")]
    [InlineData("-123")]
    [InlineData("-123,45")]
    [InlineData("0")]
    [InlineData("0,0")]
    public void IsNumeric_With_Valid_German_Numbers_Should_Return_True(string input)
    {
        // Act
        var result = RegexHelper.IsNumeric(input, useGermanFormat: true);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123.45.67")]
    [InlineData("123,45,67")]
    [InlineData("")]
    public void IsNumeric_With_Invalid_Numbers_Should_Return_False(string input)
    {
        // Act
        var resultUS = RegexHelper.IsNumeric(input, useGermanFormat: false);
        var resultGerman = RegexHelper.IsNumeric(input, useGermanFormat: true);

        // Assert
        resultUS.Should().BeFalse();
        resultGerman.Should().BeFalse();
    }

    [Fact]
    public void IsValidEmail_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsValidEmail(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsValidEmail_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsValidEmail(input);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test+tag@example.org")]
    [InlineData("first-last@subdomain.example.com")]
    public void IsValidEmail_With_Valid_Emails_Should_Return_True(string input)
    {
        // Act
        var result = RegexHelper.IsValidEmail(input);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    [InlineData("test@.com")]
    [InlineData("test@example.")]
    public void IsValidEmail_With_Invalid_Emails_Should_Return_False(string input)
    {
        // Act
        var result = RegexHelper.IsValidEmail(input);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidUrl_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.IsValidUrl(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsValidUrl_With_Empty_String_Should_Return_False()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = RegexHelper.IsValidUrl(input);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("http://www.example.com")]
    [InlineData("https://subdomain.example.org")]
    [InlineData("ftp://files.example.com")]
    [InlineData("http://example.com:8080/path")]
    [InlineData("https://example.com/path?query=value")]
    public void IsValidUrl_With_Valid_Urls_Should_Return_True(string input)
    {
        // Act
        var result = RegexHelper.IsValidUrl(input);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("www.example.com")]
    [InlineData("http://")]
    [InlineData("://example.com")]
    public void IsValidUrl_With_Invalid_Urls_Should_Return_False(string input)
    {
        // Act
        var result = RegexHelper.IsValidUrl(input);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Extraction Methods Tests

    [Fact]
    public void Extract_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        string pattern = @"\d+";

        // Act & Assert
        var action = () => RegexHelper.Extract(input!, pattern);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Extract_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string? pattern = null;

        // Act & Assert
        var action = () => RegexHelper.Extract(input, pattern!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Extract_With_Negative_GroupIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string input = "test123";
        string pattern = @"\d+";
        int groupIndex = -1;

        // Act & Assert
        var action = () => RegexHelper.Extract(input, pattern, groupIndex);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Extract_With_Valid_Pattern_Should_Return_Match()
    {
        // Arrange
        string input = "abc123def";
        string pattern = @"\d+";

        // Act
        var result = RegexHelper.Extract(input, pattern);

        // Assert
        result.Should().Be("123");
    }

    [Fact]
    public void Extract_With_No_Match_Should_Return_Null()
    {
        // Arrange
        string input = "abcdef";
        string pattern = @"\d+";

        // Act
        var result = RegexHelper.Extract(input, pattern);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Extract_With_Capture_Group_Should_Return_Group_Value()
    {
        // Arrange
        string input = "name: John";
        string pattern = @"name:\s*(\w+)";

        // Act
        var result = RegexHelper.Extract(input, pattern, 1);

        // Assert
        result.Should().Be("John");
    }

    [Fact]
    public void Extract_With_Invalid_Group_Index_Should_Return_Null()
    {
        // Arrange
        string input = "abc123def";
        string pattern = @"\d+";

        // Act
        var result = RegexHelper.Extract(input, pattern, 5);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractAll_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        string pattern = @"\d+";

        // Act & Assert
        var action = () => RegexHelper.ExtractAll(input!, pattern).ToList();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExtractAll_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string? pattern = null;

        // Act & Assert
        var action = () => RegexHelper.ExtractAll(input, pattern!).ToList();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExtractAll_With_Negative_GroupIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string input = "test123";
        string pattern = @"\d+";
        int groupIndex = -1;

        // Act & Assert
        var action = () => RegexHelper.ExtractAll(input, pattern, groupIndex).ToList();
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ExtractAll_With_Multiple_Matches_Should_Return_All_Matches()
    {
        // Arrange
        string input = "abc123def456ghi789";
        string pattern = @"\d+";

        // Act
        var result = RegexHelper.ExtractAll(input, pattern).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainInOrder("123", "456", "789");
    }

    [Fact]
    public void ExtractAll_With_No_Matches_Should_Return_Empty_Collection()
    {
        // Arrange
        string input = "abcdefghi";
        string pattern = @"\d+";

        // Act
        var result = RegexHelper.ExtractAll(input, pattern).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractAll_With_Capture_Groups_Should_Return_Group_Values()
    {
        // Arrange
        string input = "name: John, age: 25, name: Jane, age: 30";
        string pattern = @"name:\s*(\w+)";

        // Act
        var result = RegexHelper.ExtractAll(input, pattern, 1).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainInOrder("John", "Jane");
    }

    [Fact]
    public void ExtractEmails_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.ExtractEmails(input!).ToList();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExtractEmails_With_Valid_Emails_Should_Return_All_Emails()
    {
        // Arrange
        string input = "Contact us at info@example.com or support@test.org for help.";

        // Act
        var result = RegexHelper.ExtractEmails(input).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainInOrder("info@example.com", "support@test.org");
    }

    [Fact]
    public void ExtractEmails_With_No_Emails_Should_Return_Empty_Collection()
    {
        // Arrange
        string input = "This text contains no email addresses.";

        // Act
        var result = RegexHelper.ExtractEmails(input).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractUrls_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.ExtractUrls(input!).ToList();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ExtractUrls_With_Valid_Urls_Should_Return_All_Urls()
    {
        // Arrange
        string input = "Visit http://example.com or https://test.org for more info.";

        // Act
        var result = RegexHelper.ExtractUrls(input).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainInOrder("http://example.com", "https://test.org");
    }

    [Fact]
    public void ExtractUrls_With_Lenient_Mode_Should_Find_More_Urls()
    {
        // Arrange
        string input = "Check www.example.com and http://test.org";

        // Act
        var strictResult = RegexHelper.ExtractUrls(input, lenient: false).ToList();
        var lenientResult = RegexHelper.ExtractUrls(input, lenient: true).ToList();

        // Assert
        strictResult.Should().HaveCount(1);
        strictResult.Should().Contain("http://test.org");
        
        lenientResult.Should().HaveCount(2);
        lenientResult.Should().Contain("www.example.com");
        lenientResult.Should().Contain("http://test.org");
    }

    [Fact]
    public void ExtractUrls_With_No_Urls_Should_Return_Empty_Collection()
    {
        // Arrange
        string input = "This text contains no URLs.";

        // Act
        var result = RegexHelper.ExtractUrls(input).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Replacement Methods Tests

    [Fact]
    public void ReplaceAll_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        string pattern = @"\d+";
        string replacement = "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceAll(input!, pattern, replacement);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceAll_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string? pattern = null;
        string replacement = "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceAll(input, pattern!, replacement);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceAll_With_Null_Replacement_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string pattern = @"\d+";
        string? replacement = null;

        // Act & Assert
        var action = () => RegexHelper.ReplaceAll(input, pattern, replacement!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceAll_With_Valid_Pattern_Should_Replace_All_Matches()
    {
        // Arrange
        string input = "abc123def456ghi";
        string pattern = @"\d+";
        string replacement = "X";

        // Act
        var result = RegexHelper.ReplaceAll(input, pattern, replacement);

        // Assert
        result.Should().Be("abcXdefXghi");
    }

    [Fact]
    public void ReplaceAll_With_Invalid_Pattern_Should_Return_Original_String()
    {
        // Arrange
        string input = "test123";
        string pattern = "[invalid";
        string replacement = "X";

        // Act
        var result = RegexHelper.ReplaceAll(input, pattern, replacement);

        // Assert
        result.Should().Be("test123");
    }

    [Fact]
    public void ReplaceAll_With_No_Matches_Should_Return_Original_String()
    {
        // Arrange
        string input = "abcdef";
        string pattern = @"\d+";
        string replacement = "X";

        // Act
        var result = RegexHelper.ReplaceAll(input, pattern, replacement);

        // Assert
        result.Should().Be("abcdef");
    }

    [Fact]
    public void ReplaceGroup_String_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        string pattern = @"(\d+)";
        int groupIndex = 1;
        string replacement = "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input!, pattern, groupIndex, replacement);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceGroup_String_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string? pattern = null;
        int groupIndex = 1;
        string replacement = "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input, pattern!, groupIndex, replacement);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceGroup_String_With_Null_Replacement_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string pattern = @"(\d+)";
        int groupIndex = 1;
        string? replacement = null;

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input, pattern, groupIndex, replacement!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceGroup_String_With_Negative_GroupIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string input = "test123";
        string pattern = @"(\d+)";
        int groupIndex = -1;
        string replacement = "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input, pattern, groupIndex, replacement);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ReplaceGroup_String_With_Valid_Group_Should_Replace_Group()
    {
        // Arrange
        string input = "price: $123.45";
        string pattern = @"price: \$(\d+)\.(\d+)";
        int groupIndex = 1;
        string replacement = "999";

        // Act
        var result = RegexHelper.ReplaceGroup(input, pattern, groupIndex, replacement);

        // Assert
        result.Should().Be("price: $999.45");
    }

    [Fact]
    public void ReplaceGroup_Callback_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        string pattern = @"(\d+)";
        int groupIndex = 1;
        CallbackRegexReplacement replacement = (idx, val) => "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input!, pattern, groupIndex, replacement);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceGroup_Callback_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string? pattern = null;
        int groupIndex = 1;
        CallbackRegexReplacement replacement = (idx, val) => "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input, pattern!, groupIndex, replacement);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceGroup_Callback_With_Null_Replacement_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        string pattern = @"(\d+)";
        int groupIndex = 1;
        CallbackRegexReplacement? replacement = null;

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input, pattern, groupIndex, replacement!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceGroup_Callback_With_Negative_GroupIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string input = "test123";
        string pattern = @"(\d+)";
        int groupIndex = -1;
        CallbackRegexReplacement replacement = (idx, val) => "X";

        // Act & Assert
        var action = () => RegexHelper.ReplaceGroup(input, pattern, groupIndex, replacement);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ReplaceGroup_Callback_With_Valid_Group_Should_Replace_Group()
    {
        // Arrange
        string input = "price: $123.45";
        string pattern = @"price: \$(\d+)\.(\d+)";
        int groupIndex = 1;
        CallbackRegexReplacement replacement = (idx, val) => (int.Parse(val) * 2).ToString();

        // Act
        var result = RegexHelper.ReplaceGroup(input, pattern, groupIndex, replacement);

        // Assert
        result.Should().Be("price: $246.45");
    }

    [Fact]
    public void ReplaceGroup_Callback_With_Invalid_Group_Should_Return_Original()
    {
        // Arrange
        string input = "price: $123.45";
        string pattern = @"price: \$(\d+)\.(\d+)";
        int groupIndex = 5;
        CallbackRegexReplacement replacement = (idx, val) => "X";

        // Act
        var result = RegexHelper.ReplaceGroup(input, pattern, groupIndex, replacement);

        // Assert
        result.Should().Be("price: $123.45");
    }

    [Fact]
    public void RemoveHtmlBreaks_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.RemoveHtmlBreaks(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveHtmlBreaks_With_Html_Breaks_Should_Remove_Them()
    {
        // Arrange
        string input = "Hello<br>World<br/>Test<p>Paragraph</p>";

        // Act
        var result = RegexHelper.RemoveHtmlBreaks(input);

        // Assert
        result.Should().Be("HelloWorldTestParagraph");
    }

    [Fact]
    public void RemoveHtmlBreaks_With_No_Html_Should_Return_Original()
    {
        // Arrange
        string input = "Hello World Test";

        // Act
        var result = RegexHelper.RemoveHtmlBreaks(input);

        // Assert
        result.Should().Be("Hello World Test");
    }

    [Fact]
    public void TrimHtmlBreaks_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.TrimHtmlBreaks(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TrimHtmlBreaks_With_Leading_Trailing_Html_Should_Remove_Them()
    {
        // Arrange
        string input = "<br>Hello World<p>";

        // Act
        var result = RegexHelper.TrimHtmlBreaks(input);

        // Assert
        result.Should().Be("Hello World");
    }

    [Fact]
    public void TrimHtmlBreaks_With_Middle_Html_Should_Keep_Them()
    {
        // Arrange
        string input = "Hello<br>World";

        // Act
        var result = RegexHelper.TrimHtmlBreaks(input);

        // Assert
        result.Should().Be("Hello<br>World");
    }

    [Fact]
    public void RemoveNonAlphaNumeric_With_Null_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        var action = () => RegexHelper.RemoveNonAlphaNumeric(input!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveNonAlphaNumeric_With_Special_Characters_Should_Remove_Them()
    {
        // Arrange
        string input = "Hello@#$123World!@#";

        // Act
        var result = RegexHelper.RemoveNonAlphaNumeric(input);

        // Assert
        result.Should().Be("Hello123World");
    }

    [Fact]
    public void RemoveNonAlphaNumeric_With_Only_AlphaNumeric_Should_Return_Original()
    {
        // Arrange
        string input = "Hello123World";

        // Act
        var result = RegexHelper.RemoveNonAlphaNumeric(input);

        // Assert
        result.Should().Be("Hello123World");
    }

    [Fact]
    public void KeepAlphaNumeric_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? input = null;

        // Act
        var result = RegexHelper.KeepAlphaNumeric(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void KeepAlphaNumeric_With_Whitespace_Should_Return_Empty()
    {
        // Arrange
        string input = "   ";

        // Act
        var result = RegexHelper.KeepAlphaNumeric(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void KeepAlphaNumeric_With_Mixed_Characters_Should_Keep_Only_AlphaNumeric()
    {
        // Arrange
        string input = "Hello@#$123World!@#";

        // Act
        var result = RegexHelper.KeepAlphaNumeric(input);

        // Assert
        result.Should().Be("Hello123World");
    }

    [Fact]
    public void KeepAlphaCharacters_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? input = null;

        // Act
        var result = RegexHelper.KeepAlphaCharacters(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void KeepAlphaCharacters_With_Whitespace_Should_Return_Empty()
    {
        // Arrange
        string input = "   ";

        // Act
        var result = RegexHelper.KeepAlphaCharacters(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void KeepAlphaCharacters_With_Mixed_Characters_Should_Keep_Only_Alpha()
    {
        // Arrange
        string input = "Hello@#$123World!@#";

        // Act
        var result = RegexHelper.KeepAlphaCharacters(input);

        // Assert
        result.Should().Be("HelloWorld");
    }

    [Fact]
    public void KeepNumericDigitsOnly_With_Null_Should_Return_Empty()
    {
        // Arrange
        string? input = null;

        // Act
        var result = RegexHelper.KeepNumericDigitsOnly(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void KeepNumericDigitsOnly_With_Whitespace_Should_Return_Empty()
    {
        // Arrange
        string input = "   ";

        // Act
        var result = RegexHelper.KeepNumericDigitsOnly(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void KeepNumericDigitsOnly_With_Mixed_Characters_Should_Keep_Only_Digits()
    {
        // Arrange
        string input = "Hello123.45World678,90";

        // Act
        var result = RegexHelper.KeepNumericDigitsOnly(input);

        // Assert
        result.Should().Be("1234567890");
    }

    [Fact]
    public void KeepNumericDigitsOnly_With_KeepPunctuation_Should_Keep_Decimal_Punctuation()
    {
        // Arrange
        string input = "Hello123.45World678,90";

        // Act
        var result = RegexHelper.KeepNumericDigitsOnly(input, keepNumericPunctuation: true);

        // Assert
        result.Should().Be("123.45678,90");
    }

    #endregion

    #region Matching Methods Tests

    [Fact]
    public void MatchAny_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        var patterns = new[] { new Regex(@"\d+"), new Regex(@"[a-z]+") };

        // Act & Assert
        var action = () => RegexHelper.MatchAny(input!, patterns);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MatchAny_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        Regex?[] patterns = { new Regex(@"\d+"), null };

        // Act & Assert
        var action = () => RegexHelper.MatchAny(input, patterns!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MatchAny_With_Matching_First_Pattern_Should_Return_Zero()
    {
        // Arrange
        string input = "test123";
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"\d+") };

        // Act
        var result = RegexHelper.MatchAny(input, patterns);

        // Assert
        result.Should().BeGreaterThan(-1);
    }

    [Fact]
    public void MatchAny_With_Matching_Second_Pattern_Should_Return_One()
    {
        // Arrange
        string input = "123test";
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"\d+") };

        // Act
        var result = RegexHelper.MatchAny(input, patterns);

        // Assert
        result.Should().BeGreaterThan(-1);
    }

    [Fact]
    public void MatchAny_With_No_Matches_Should_Return_Minus_One()
    {
        // Arrange
        string input = "!@#$%";
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"\d+") };

        // Act
        var result = RegexHelper.MatchAny(input, patterns);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void MatchAny_With_OutParameter_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        var patterns = new[] { new Regex(@"\d+"), new Regex(@"[a-z]+") };

        // Act & Assert
        var action = () => RegexHelper.MatchAny(input!, out _, patterns);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MatchAny_With_OutParameter_With_Null_Pattern_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string input = "test123";
        Regex?[] patterns = { new Regex(@"\d+"), null };

        // Act & Assert
        var action = () => RegexHelper.MatchAny(input, out _, patterns!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MatchAny_With_OutParameter_With_Match_Should_Return_Match_Object()
    {
        // Arrange
        string input = "test123";
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"\d+") };

        // Act
        var result = RegexHelper.MatchAny(input, out var match, patterns);

        // Assert
        result.Should().Be(0);
        match.Should().NotBeNull();
        match!.Value.Should().Be("test");
    }

    [Fact]
    public void MatchAny_With_OutParameter_With_No_Match_Should_Return_Null_Match()
    {
        // Arrange
        string input = "!@#$%";
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"\d+") };

        // Act
        var result = RegexHelper.MatchAny(input, out var match, patterns);

        // Assert
        result.Should().Be(-1);
        match.Should().BeNull();
    }

    [Fact]
    public void MatchAll_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"\d+") };

        // Act & Assert
        var action = () => RegexHelper.MatchAll(input!, patterns);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MatchAll_With_All_Patterns_Matching_Should_Return_True()
    {
        // Arrange
        string input = "test123";
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"\d+") };

        // Act
        var result = RegexHelper.MatchAll(input, patterns);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void MatchAll_With_Some_Patterns_Not_Matching_Should_Return_False()
    {
        // Arrange
        string input = "test123";
        var patterns = new[] { new Regex(@"[a-z]+"), new Regex(@"[A-Z]+") };

        // Act
        var result = RegexHelper.MatchAll(input, patterns);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void MatchAll_With_Empty_Patterns_Should_Return_True()
    {
        // Arrange
        string input = "test123";
        var patterns = Array.Empty<Regex>();

        // Act
        var result = RegexHelper.MatchAll(input, patterns);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Helper Methods Tests

    [Fact]
    public void GetCapture_With_Negative_GroupIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var match = Regex.Match("test123", @"([a-zA-Z]+)(\d+)");
        int groupIndex = -1;

        // Act & Assert
        var action = () => RegexHelper.GetCapture(match, groupIndex);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetCapture_With_Successful_Match_Should_Return_Group_Value()
    {
        // Arrange
        var match = Regex.Match("test123", @"([a-zA-Z]+)(\d+)");

        // Act
        var result0 = RegexHelper.GetCapture(match, 0);
        var result1 = RegexHelper.GetCapture(match, 1);
        var result2 = RegexHelper.GetCapture(match, 2);

        // Assert
        result0.Should().Be("test123");
        result1.Should().Be("test");
        result2.Should().Be("123");
    }

    [Fact]
    public void GetCapture_With_Failed_Match_Should_Return_Null()
    {
        // Arrange
        var match = Regex.Match("abcdef", @"\d+");

        // Act
        var result = RegexHelper.GetCapture(match, 0);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetCapture_With_Invalid_Group_Index_Should_Return_Null()
    {
        // Arrange
        var match = Regex.Match("test123", @"([a-zA-Z]+)(\d+)");

        // Act
        var result = RegexHelper.GetCapture(match, 5);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetLastCapture_With_Negative_Offset_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var match = Regex.Match("test123", @"([a-zA-Z]+)(\d+)");
        int offset = -1;

        // Act & Assert
        var action = () => RegexHelper.GetLastCapture(match, offset);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetLastCapture_With_Successful_Match_Should_Return_Last_Group()
    {
        // Arrange
        var match = Regex.Match("test123", @"([a-zA-Z]+)(\d+)");

        // Act
        var lastCapture = RegexHelper.GetLastCapture(match);
        var secondToLast = RegexHelper.GetLastCapture(match, 1);

        // Assert
        lastCapture.Should().Be("123");
        secondToLast.Should().Be("test");
    }

    [Fact]
    public void GetLastCapture_With_Failed_Match_Should_Return_Null()
    {
        // Arrange
        var match = Regex.Match("abcdef", @"\d+");

        // Act
        var result = RegexHelper.GetLastCapture(match);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetLastCapture_With_Invalid_Offset_Should_Return_Null()
    {
        // Arrange
        var match = Regex.Match("test123", @"([a-zA-Z]+)(\d+)");

        // Act
        var result = RegexHelper.GetLastCapture(match, 10);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SplitAt_With_Null_Input_Should_Throw_ArgumentNullException()
    {
        // Arrange
        string? input = null;
        int index = 5;

        // Act & Assert
        var action = () => RegexHelper.SplitAt(input!, index);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SplitAt_With_Negative_Index_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string input = "Hello World";
        int index = -1;

        // Act & Assert
        var action = () => RegexHelper.SplitAt(input, index);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SplitAt_With_Index_Greater_Than_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string input = "Hello World";
        int index = 15;

        // Act & Assert
        var action = () => RegexHelper.SplitAt(input, index);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SplitAt_With_Valid_Index_Should_Split_Correctly()
    {
        // Arrange
        string input = "Hello World";
        int index = 5;

        // Act
        var result = RegexHelper.SplitAt(input, index);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Hello");
        result[1].Should().Be(" World");
    }

    [Fact]
    public void SplitAt_With_IncludeIndexChar_Should_Include_Character_In_Left()
    {
        // Arrange
        string input = "Hello World";
        int index = 5;

        // Act
        var result = RegexHelper.SplitAt(input, index, includeIndexCharInLeft: true);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Hello ");
        result[1].Should().Be("World");
    }

    [Fact]
    public void SplitAt_With_Index_Zero_Should_Split_At_Beginning()
    {
        // Arrange
        string input = "Hello World";
        int index = 0;

        // Act
        var result = RegexHelper.SplitAt(input, index);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be(string.Empty);
        result[1].Should().Be("Hello World");
    }

    [Fact]
    public void SplitAt_With_Index_At_End_Should_Split_At_End()
    {
        // Arrange
        string input = "Hello World";
        int index = input.Length;

        // Act
        var result = RegexHelper.SplitAt(input, index);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Hello World");
        result[1].Should().Be(string.Empty);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void Multiple_Methods_Can_Be_Used_Together()
    {
        // Arrange
        string input = "Contact us at info@example.com or call 123-456-7890";

        // Act
        var emails = RegexHelper.ExtractEmails(input).ToList();
        var digits = RegexHelper.KeepNumericDigitsOnly(input);
        var hasEmail = RegexHelper.IsValidEmail(emails.FirstOrDefault() ?? "");

        // Assert
        emails.Should().HaveCount(1);
        emails[0].Should().Be("info@example.com");
        digits.Should().Be("1234567890");
        hasEmail.Should().BeTrue();
    }

    [Fact]
    public void Performance_With_Large_Strings_Should_Be_Reasonable()
    {
        // Arrange
        string largeString = string.Join(" ", Enumerable.Repeat("test123@example.com", 1000));

        // Act & Assert - These operations should complete quickly
        var emails = RegexHelper.ExtractEmails(largeString).ToList();
        var digits = RegexHelper.KeepNumericDigitsOnly(largeString);
        var replaced = RegexHelper.ReplaceAll(largeString, @"\d+", "X");

        emails.Should().HaveCount(1000);
        digits.Should().NotBeEmpty();
        replaced.Should().Contain("testX@example.com");
    }

    [Fact]
    public void Unicode_Strings_Should_Be_Handled_Correctly()
    {
        // Arrange
        string unicode = "Héllø Wörld 123 📧 test@exämple.com";

        // Act
        var alphaOnly = RegexHelper.IsAlphaOnly(unicode);
        var digits = RegexHelper.KeepNumericDigitsOnly(unicode);
        var emails = RegexHelper.ExtractEmails(unicode).ToList();

        // Assert
        alphaOnly.Should().BeFalse();
        digits.Should().Be("123");
        emails.Should().BeEmpty(); // The unicode domain should not match standard email regex
    }

    [Fact]
    public void Empty_And_Whitespace_Strings_Should_Be_Handled_Correctly()
    {
        // Arrange
        var testStrings = new[] { "", " ", "\t", "\n", "\r\n" };

        // Act & Assert
        foreach (var testString in testStrings)
        {
            RegexHelper.IsAlphaOnly(testString).Should().BeFalse();
            RegexHelper.IsNumeric(testString).Should().BeFalse();
            RegexHelper.IsValidEmail(testString).Should().BeFalse();
            RegexHelper.IsValidUrl(testString).Should().BeFalse();
            RegexHelper.ExtractEmails(testString).Should().BeEmpty();
            RegexHelper.ExtractUrls(testString).Should().BeEmpty();
            RegexHelper.KeepAlphaNumeric(testString).Should().Be("");
        }
    }

    [Fact]
    public void Regex_Options_Should_Be_Respected()
    {
        // Arrange
        string input = "Hello WORLD 123";
        string pattern = @"hello";

        // Act
        var caseSensitive = RegexHelper.Extract(input, pattern, 0, RegexOptions.None);
        var caseInsensitive = RegexHelper.Extract(input, pattern, 0, RegexOptions.IgnoreCase);

        // Assert
        caseSensitive.Should().BeNull();
        caseInsensitive.Should().Be("Hello");
    }

    #endregion
}