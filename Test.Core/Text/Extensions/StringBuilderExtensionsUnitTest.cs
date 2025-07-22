//--------------------------------------------------------------------------
// File:    StringBuilderExtensionsUnitTest.cs
// Content: Unit tests for StringBuilderExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;
using System.Globalization;
using System.Text;

namespace AnBo.Test;

public class StringBuilderExtensionsUnitTest
{
    #region Clear Method Tests

    [Fact]
    public void Clear_With_Non_Empty_StringBuilder_Should_Clear_Content_And_Return_Same_Instance()
    {
        // Arrange
        var builder = new StringBuilder("Hello World");

        // Act
        var result = builder.ClearBuilder();

        // Assert
        result.Should().BeSameAs(builder);
        result.Length.Should().Be(0);
        result.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void Clear_With_Empty_StringBuilder_Should_Return_Same_Instance()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.ClearBuilder();

        // Assert
        result.Should().BeSameAs(builder);
        result.Length.Should().Be(0);
    }

    [Fact]
    public void Clear_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.ClearBuilder();
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region IsNullOrEmpty Method Tests

    [Fact]
    public void IsNullOrEmpty_With_Null_StringBuilder_Should_Return_True()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act
        var result = builder.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_With_Empty_StringBuilder_Should_Return_True()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_With_Non_Empty_StringBuilder_Should_Return_False()
    {
        // Arrange
        var builder = new StringBuilder("Hello");

        // Act
        var result = builder.IsNullOrEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsNullOrEmpty_With_StringBuilder_Containing_Whitespace_Should_Return_False()
    {
        // Arrange
        var builder = new StringBuilder("   ");

        // Act
        var result = builder.IsNullOrEmpty();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsEmpty Method Tests

    [Fact]
    public void IsEmpty_With_Empty_StringBuilder_Should_Return_True()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.IsEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_With_Non_Empty_StringBuilder_Should_Return_False()
    {
        // Arrange
        var builder = new StringBuilder("Hello");

        // Act
        var result = builder.IsEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.IsEmpty();
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region AppendInvariant Method Tests

    [Fact]
    public void AppendInvariant_With_String_Should_Append_String()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendInvariant("Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void AppendInvariant_With_Null_String_Should_Skip_Null()
    {
        // Arrange
        var builder = new StringBuilder("Start");

        // Act
        var result = builder.AppendInvariant<string?>(null);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Start");
    }

    [Fact]
    public void AppendInvariant_With_IFormattable_Should_Use_InvariantCulture()
    {
        // Arrange
        var builder = new StringBuilder();
        var value = 42.5;

        // Act
        var result = builder.AppendInvariant(value);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("42.5");
    }

    [Fact]
    public void AppendInvariant_With_Non_Formattable_Object_Should_Use_ToString()
    {
        // Arrange
        var builder = new StringBuilder();
        var customObject = new { Name = "Test", Value = 42 };

        // Act
        var result = builder.AppendInvariant(customObject);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Contain("Name").And.Contain("Test").And.Contain("Value").And.Contain("42");
    }

    [Fact]
    public void AppendInvariant_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.AppendInvariant("test");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendInvariant_With_DateTime_Should_Use_InvariantCulture()
    {
        // Arrange
        var builder = new StringBuilder();
        var dateTime = new DateTime(2025, 7, 20, 14, 30, 45);

        // Act
        var result = builder.AppendInvariant(dateTime);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(dateTime.ToString(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void AppendInvariant_With_Integer_Should_Append_Correctly()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendInvariant(12345);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("12345");
    }

    #endregion

    #region AppendUseToInvariantString Method Tests

    [Fact]
    public void AppendUseToInvariantString_With_String_Should_Append_String()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendUseToInvariantString("Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void AppendUseToInvariantString_With_Null_String_Should_Skip_Null()
    {
        // Arrange
        var builder = new StringBuilder("Start");

        // Act
        var result = builder.AppendUseToInvariantString<string?>(null);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Start");
    }

    [Fact]
    public void AppendUseToInvariantString_With_IFormattable_Should_Use_InvariantCulture()
    {
        // Arrange
        var builder = new StringBuilder();
        var value = 42.5;

        // Act
        var result = builder.AppendUseToInvariantString(value);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("42.5");
    }

    [Fact]
    public void AppendUseToInvariantString_With_Non_Formattable_Object_Should_Use_ToInvariantString()
    {
        // Arrange
        var builder = new StringBuilder();
        var customObject = new { Name = "Test", Value = 42 };

        // Act
        var result = builder.AppendUseToInvariantString(customObject);

        // Assert
        result.Should().BeSameAs(builder);
        var expectedString = customObject.ToInvariantString();
        result.ToString().Should().Be(expectedString);
    }

    [Fact]
    public void AppendUseToInvariantString_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.AppendUseToInvariantString("test");
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region AppendIf Method Tests

    [Fact]
    public void AppendIf_With_True_Condition_Should_Append_Value()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIf(true, "Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void AppendIf_With_False_Condition_Should_Not_Append_Value()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIf(false, "Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void AppendIf_With_True_Condition_And_Null_Value_Should_Not_Append()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendIf(true, null);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void AppendIf_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.AppendIf(true, "test");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendIf_With_Function_Condition_True_Should_Append_Value()
    {
        // Arrange
        var builder = new StringBuilder();
        Func<bool> condition = () => true;

        // Act
        var result = builder.AppendIf(condition, "Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void AppendIf_With_Function_Condition_False_Should_Not_Append_Value()
    {
        // Arrange
        var builder = new StringBuilder();
        Func<bool> condition = () => false;

        // Act
        var result = builder.AppendIf(condition, "Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void AppendIf_With_Null_Function_Condition_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var builder = new StringBuilder();
        Func<bool>? condition = null;

        // Act & Assert
        var action = () => builder.AppendIf(condition!, "test");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendIf_With_Function_Condition_And_Null_Value_Should_Not_Append()
    {
        // Arrange
        var builder = new StringBuilder();
        Func<bool> condition = () => true;

        // Act
        var result = builder.AppendIf(condition, null);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    #endregion

    #region Prepend Method Tests

    [Fact]
    public void Prepend_Char_Should_Insert_At_Beginning()
    {
        // Arrange
        var builder = new StringBuilder("World");

        // Act
        var result = builder.Prepend('H');

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("HWorld");
    }

    [Fact]
    public void Prepend_Char_To_Empty_StringBuilder_Should_Add_Char()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.Prepend('A');

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("A");
    }

    [Fact]
    public void Prepend_String_Should_Insert_At_Beginning()
    {
        // Arrange
        var builder = new StringBuilder("World");

        // Act
        var result = builder.Prepend("Hello ");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello World");
    }

    [Fact]
    public void Prepend_String_To_Empty_StringBuilder_Should_Add_String()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.Prepend("Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void Prepend_Char_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.Prepend('H');
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Prepend_String_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.Prepend("Hello");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Prepend_String_With_Null_Value_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var builder = new StringBuilder("World");

        // Act & Assert
        var action = () => builder.Prepend((string)null!);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region AppendJoin Method Tests

    [Fact]
    public void AppendJoinCollection_String_Separator_With_Valid_Values_Should_Join_Correctly()
    {
        // Arrange
        var builder = new StringBuilder();
        var values = new[] { 1, 2, 3 };

        // Act
        var result = builder.AppendJoinCollection(", ", values);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("1, 2, 3");
    }

    [Fact]
    public void AppendJoinCollection_String_Separator_With_Empty_Collection_Should_Not_Append()
    {
        // Arrange
        var builder = new StringBuilder("Start");
        var values = Array.Empty<int>();

        // Act
        var result = builder.AppendJoinCollection(", ", values);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Start");
    }

    [Fact]
    public void AppendJoinCollection_String_Separator_With_Single_Value_Should_Append_Without_Separator()
    {
        // Arrange
        var builder = new StringBuilder();
        var values = new[] { "Hello" };

        // Act
        var result = builder.AppendJoinCollection(", ", values);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void AppendJoinCollection_String_Separator_With_Null_Separator_Should_Join_Without_Separator()
    {
        // Arrange
        var builder = new StringBuilder();
        var values = new[] { "A", "B", "C" };

        // Act
        var result = builder.AppendJoinCollection((string?)null, values);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("ABC");
    }

    [Fact]
    public void AppendJoinCollection_Char_Separator_With_Valid_Values_Should_Join_Correctly()
    {
        // Arrange
        var builder = new StringBuilder();
        var values = new[] { 1, 2, 3 };

        // Act
        var result = builder.AppendJoinCollection(',', values);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("1,2,3");
    }

    [Fact]
    public void AppendJoinCollection_Char_Separator_With_Empty_Collection_Should_Not_Append()
    {
        // Arrange
        var builder = new StringBuilder("Start");
        var values = Array.Empty<int>();

        // Act
        var result = builder.AppendJoinCollection(',', values);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Start");
    }

    [Fact]
    public void AppendJoinCollection_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;
        var values = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => builder!.AppendJoinCollection(", ", values);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendJoinCollection_With_Null_Values_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var builder = new StringBuilder();
        IEnumerable<int>? values = null;

        // Act & Assert
        var action = () => builder.AppendJoinCollection(", ", values!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendJoinCollection_With_Mixed_Types_Should_Format_Using_AppendInvariant()
    {
        // Arrange
        var builder = new StringBuilder();
        var values = new object[] { 1, 2.5, "test", true };

        // Act
        var result = builder.AppendJoinCollection("; ", values);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("1; 2.5; test; True");
    }

    #endregion

    #region AppendLineIf Method Tests

    [Fact]
    public void AppendLineIf_With_Non_Empty_StringBuilder_Should_Append_Line()
    {
        // Arrange
        var builder = new StringBuilder("Hello");

        // Act
        var result = builder.AppendLineIf();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be($"Hello{Environment.NewLine}");
    }

    [Fact]
    public void AppendLineIf_With_Empty_StringBuilder_Should_Not_Append_Line()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendLineIf();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void AppendLineIf_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.AppendLineIf();
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendLineIf_With_Condition_True_Should_Append_Line_With_Value()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendLineIf(true, "Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be($"Hello{Environment.NewLine}");
    }

    [Fact]
    public void AppendLineIf_With_Condition_False_Should_Not_Append()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendLineIf(false, "Hello");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void AppendLineIf_With_Condition_True_And_Null_Value_Should_Not_Append()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AppendLineIf(true, null);

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    #endregion

    #region TrimEnd Method Tests

    [Fact]
    public void TrimEnd_With_Trailing_Spaces_Should_Remove_Spaces()
    {
        // Arrange
        var builder = new StringBuilder("Hello   ");

        // Act
        var result = builder.TrimEnd();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void TrimEnd_With_Trailing_Mixed_Whitespace_Should_Remove_All_Whitespace()
    {
        // Arrange
        var builder = new StringBuilder("Hello \t\n\r ");

        // Act
        var result = builder.TrimEnd();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void TrimEnd_With_No_Trailing_Whitespace_Should_Not_Change()
    {
        // Arrange
        var builder = new StringBuilder("Hello");

        // Act
        var result = builder.TrimEnd();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void TrimEnd_With_Only_Whitespace_Should_Result_In_Empty()
    {
        // Arrange
        var builder = new StringBuilder("   \t\n   ");

        // Act
        var result = builder.TrimEnd();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
        result.Length.Should().Be(0);
    }

    [Fact]
    public void TrimEnd_With_Empty_StringBuilder_Should_Remain_Empty()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.TrimEnd();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void TrimEnd_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        StringBuilder? builder = null;

        // Act & Assert
        var action = () => builder!.TrimEnd();
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region AsSpan Method Tests

    [Fact]
    public void AsSpan_With_Non_Empty_StringBuilder_Should_Return_Correct_Span()
    {
        // Arrange
        var builder = new StringBuilder("Hello World");

        // Act
        var result = builder.AsSpan();

        // Assert
        result.Length.Should().Be(11);
        result.ToString().Should().Be("Hello World");
    }

    [Fact]
    public void AsSpan_With_Empty_StringBuilder_Should_Return_Empty_Span()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder.AsSpan();

        // Assert
        result.Length.Should().Be(0);
        result.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void AsSpan_With_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        //StringBuilder? builder = null;

        // Act & Assert
        var action = HelperMethod;
        action.Should().Throw<ArgumentNullException>();
    }

    private static void HelperMethod()
    {
        // Arrange
        StringBuilder? builder = null;
        builder!.AsSpan();
    }

    [Fact]
    public void AsSpan_With_Start_And_Length_Should_Return_Correct_Substring_Span()
    {
        // Arrange
        var builder = new StringBuilder("Hello World");

        // Act
        var result = builder.AsSpan(6, 5);

        // Assert
        result.Length.Should().Be(5);
        result.ToString().Should().Be("World");
    }

    [Fact]
    public void AsSpan_With_Start_Zero_And_Full_Length_Should_Return_Full_Span()
    {
        // Arrange
        var builder = new StringBuilder("Hello");

        // Act
        var result = builder.AsSpan(0, 5);

        // Assert
        result.Length.Should().Be(5);
        result.ToString().Should().Be("Hello");
    }

    [Fact]
    public void AsSpan_With_Invalid_Start_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        //var builder = new StringBuilder("Hello");

        // Act & Assert
        var action = AsSpan_HelperMethod1;
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static void AsSpan_HelperMethod1()
    {
        // Arrange
        var builder = new StringBuilder("Hello");
        builder.AsSpan(-1, 2);
    }


    [Fact]
    public void AsSpan_With_Invalid_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        //var builder = new StringBuilder("Hello");

        // Act & Assert
        var action = AsSpan_HelperMethod2;
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static void AsSpan_HelperMethod2()
    {
        // Arrange
        var builder = new StringBuilder("Hello");
        builder.AsSpan(0, 10);
    }

    [Fact]
    public void AsSpan_With_Start_And_Length_And_Null_StringBuilder_Should_Throw_ArgumentNullException()
    {
        // Arrange
        //StringBuilder? builder = null;

        // Act & Assert
        var action = AsSpan_HelperMethod3;
        action.Should().Throw<ArgumentNullException>();
    }

    private static void AsSpan_HelperMethod3()
    {
        // Arrange
        StringBuilder? builder = null;
        builder!.AsSpan(0, 5);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Method_Chaining_Should_Work_Correctly()
    {
        // Arrange
        var builder = new StringBuilder();

        // Act
        var result = builder
            .Append("Start")
            .AppendIf(true, " Middle")
            .AppendInvariant(42)
            .Prepend(">> ")
            .TrimEnd();

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be(">> Start Middle42");
    }

    [Fact]
    public void Complex_AppendJoinCollection_With_Different_Types_Should_Format_Correctly()
    {
        // Arrange
        var builder = new StringBuilder("Values: ");
        var values = new object[] { 1, 2.5, DateTime.Parse("2025-07-20"), true, "test" };

        // Act
        var result = builder.AppendJoinCollection(" | ", values);

        // Assert
        result.Should().BeSameAs(builder);
        var content = result.ToString();
        content.Should().StartWith("Values: ");
        content.Should().Contain("1 | 2.5");
        content.Should().Contain("True");
        content.Should().Contain("test");
    }

    [Fact]
    public void Clear_And_Build_New_Content_Should_Work()
    {
        // Arrange
        var builder = new StringBuilder("Old content");

        // Act
        var result = builder
            .Clear()
            .Append("New ")
            .AppendInvariant("content")
            .AppendLineIf()
            .Append("Second line");

        // Assert
        result.Should().BeSameAs(builder);
        result.ToString().Should().Be($"New content{Environment.NewLine}Second line");
    }

    #endregion
}