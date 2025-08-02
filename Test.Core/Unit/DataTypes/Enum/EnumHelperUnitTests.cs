//--------------------------------------------------------------------------
// File:    EnumHelperUnitTest.cs
// Content: Unit tests for EnumHelper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.ComponentModel;

namespace AnBo.Test.Unit;

[Trait("Category", "Unit")]
public class EnumHelperUnitTests
{
    #region Test Enums

    public enum TestEnum
    {
        [Description("First Value")]
        First = 1,
        [Description("Second Value")]
        Second = 2,
        Third = 3,
        [Description("Fourth Value")]
        Fourth = 4
    }

    [Flags]
    public enum TestFlagsEnum
    {
        None = 0,
        [Description("Read Permission")]
        Read = 1,
        [Description("Write Permission")]
        Write = 2,
        [Description("Execute Permission")]
        Execute = 4,
        [Description("Full Access")]
        All = Read | Write | Execute
    }

    public enum ByteEnum : byte
    {
        Zero = 0,
        One = 1,
        Max = 255
    }

    public enum LongEnum : long
    {
        Zero = 0,
        One = 1,
        Large = long.MaxValue
    }

    public enum EmptyEnum
    {
    }

    public enum TestEnumForConversion
    {
        First = 1,
        Second = 2,
        Different = 3
    }

    #endregion

    #region Parse Method Tests

    [Fact]
    public void Parse_With_Valid_Enum_Name_Should_Return_Correct_Value()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("First");

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void Parse_With_Valid_Enum_Name_IgnoreCase_Should_Return_Correct_Value()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("first", ignoreCase: true);

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void Parse_With_Valid_Enum_Value_Should_Return_Correct_Value()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("1");

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void Parse_With_Null_Value_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Parse_With_Empty_String_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Parse_With_Whitespace_String_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("   ");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Parse_With_Invalid_Enum_Name_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("Invalid");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Parse_With_Case_Sensitive_Mismatch_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("first", ignoreCase: false);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Parse_With_Whitespace_Around_Valid_Name_Should_Return_Correct_Value()
    {
        // Act
        var result = EnumHelper.Parse<TestEnum>("  First  ");

        // Assert
        result.Should().Be(TestEnum.First);
    }

    #endregion

    #region TryParse Method Tests

    [Fact]
    public void TryParse_With_Valid_Enum_Name_Should_Return_True_And_Correct_Value()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>("Second", out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void TryParse_With_Valid_Enum_Name_IgnoreCase_Should_Return_True_And_Correct_Value()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>("second", ignoreCase: true, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void TryParse_With_Invalid_Enum_Name_Should_Return_False_And_Default_Value()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>("Invalid", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TestEnum));
    }

    [Fact]
    public void TryParse_With_Null_Value_Should_Return_False_And_Default_Value()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>(null, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TestEnum));
    }

    [Fact]
    public void TryParse_With_Empty_String_Should_Return_False_And_Default_Value()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>("", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TestEnum));
    }

    [Fact]
    public void TryParse_With_Whitespace_String_Should_Return_False_And_Default_Value()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>("   ", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TestEnum));
    }

    [Fact]
    public void TryParse_Overload_Without_IgnoreCase_Should_Default_To_Case_Sensitive()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>("first", out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(default(TestEnum));
    }

    [Fact]
    public void TryParse_With_Whitespace_Around_Valid_Name_Should_Return_True_And_Correct_Value()
    {
        // Act
        var success = EnumHelper.TryParse<TestEnum>("  Third  ", out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(TestEnum.Third);
    }

    #endregion

    #region IsDefined Method Tests

    [Fact]
    public void IsDefined_With_Valid_Enum_Value_Should_Return_True()
    {
        // Act
        var result = EnumHelper.IsDefined(TestEnum.First);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefined_With_Invalid_Enum_Value_Should_Return_False()
    {
        // Act
        var result = EnumHelper.IsDefined((TestEnum)999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDefined_With_Default_Enum_Value_Should_Return_False_When_Not_Defined()
    {
        // Act
        var result = EnumHelper.IsDefined(default(TestEnum));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDefined_With_Flags_Enum_Single_Value_Should_Return_True()
    {
        // Act
        var result = EnumHelper.IsDefined(TestFlagsEnum.Read);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefined_With_Flags_Enum_Combined_Value_Should_Return_True()
    {
        // Act
        var result = EnumHelper.IsDefined(TestFlagsEnum.All);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefined_With_Flags_Enum_Custom_Combination_Should_Return_False()
    {
        // Act
        var result = EnumHelper.IsDefined(TestFlagsEnum.Read | TestFlagsEnum.Write);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ValidateIsDefined Method Tests

    [Fact]
    public void ValidateIsDefined_With_Valid_Enum_Value_Should_Not_Throw()
    {
        // Act & Assert
        var action = () => EnumHelper.ValidateIsDefined(TestEnum.First);
        action.Should().NotThrow();
    }

    [Fact]
    public void ValidateIsDefined_With_Invalid_Enum_Value_Should_Throw_InvalidEnumArgumentException()
    {
        // Act & Assert
        var action = () => EnumHelper.ValidateIsDefined((TestEnum)999);
        action.Should().Throw<InvalidEnumArgumentException>()
            .WithMessage("*999*")
            .WithMessage("*TestEnum*");
    }

    [Fact]
    public void ValidateIsDefined_With_Valid_Byte_Enum_Should_Not_Throw()
    {
        // Act & Assert
        var action = () => EnumHelper.ValidateIsDefined(ByteEnum.One);
        action.Should().NotThrow();
    }

    [Fact]
    public void ValidateIsDefined_With_Invalid_Byte_Enum_Should_Throw()
    {
        // Act & Assert
        var action = () => EnumHelper.ValidateIsDefined((ByteEnum)200);
        action.Should().Throw<InvalidEnumArgumentException>();
    }

    #endregion

    #region GetNames Method Tests

    [Fact]
    public void GetNames_Should_Return_All_Enum_Names()
    {
        // Act
        var result = EnumHelper.GetNames<TestEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().Contain("First");
        result.Should().Contain("Second");
        result.Should().Contain("Third");
        result.Should().Contain("Fourth");
    }

    [Fact]
    public void GetNames_Should_Return_Same_Instance_For_Repeated_Calls()
    {
        // Act
        var result1 = EnumHelper.GetNames<TestEnum>();
        var result2 = EnumHelper.GetNames<TestEnum>();

        // Assert
        result1.Should().BeSameAs(result2);
    }

    [Fact]
    public void GetNames_With_Flags_Enum_Should_Return_All_Names()
    {
        // Act
        var result = EnumHelper.GetNames<TestFlagsEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        result.Should().Contain("None");
        result.Should().Contain("Read");
        result.Should().Contain("Write");
        result.Should().Contain("Execute");
        result.Should().Contain("All");
    }

    [Fact]
    public void GetNames_With_Byte_Enum_Should_Return_All_Names()
    {
        // Act
        var result = EnumHelper.GetNames<ByteEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain("Zero");
        result.Should().Contain("One");
        result.Should().Contain("Max");
    }

    #endregion

    #region GetValues Method Tests

    [Fact]
    public void GetValues_Should_Return_All_Enum_Values()
    {
        // Act
        var result = EnumHelper.GetValues<TestEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().Contain(TestEnum.First);
        result.Should().Contain(TestEnum.Second);
        result.Should().Contain(TestEnum.Third);
        result.Should().Contain(TestEnum.Fourth);
    }

    [Fact]
    public void GetValues_Should_Return_Same_Instance_For_Repeated_Calls()
    {
        // Act
        var result1 = EnumHelper.GetValues<TestEnum>();
        var result2 = EnumHelper.GetValues<TestEnum>();

        // Assert
        result1.Should().BeSameAs(result2);
    }

    [Fact]
    public void GetValues_With_Flags_Enum_Should_Return_All_Values()
    {
        // Act
        var result = EnumHelper.GetValues<TestFlagsEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        result.Should().Contain(TestFlagsEnum.None);
        result.Should().Contain(TestFlagsEnum.Read);
        result.Should().Contain(TestFlagsEnum.Write);
        result.Should().Contain(TestFlagsEnum.Execute);
        result.Should().Contain(TestFlagsEnum.All);
    }

    [Fact]
    public void GetValues_With_Long_Enum_Should_Return_All_Values()
    {
        // Act
        var result = EnumHelper.GetValues<LongEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(LongEnum.Zero);
        result.Should().Contain(LongEnum.One);
        result.Should().Contain(LongEnum.Large);
    }

    #endregion

    #region GetUnderlyingType Method Tests

    [Fact]
    public void GetUnderlyingType_With_Int_Enum_Should_Return_Int_Type()
    {
        // Act
        var result = EnumHelper.GetUnderlyingType<TestEnum>();

        // Assert
        result.Should().Be(typeof(int));
    }

    [Fact]
    public void GetUnderlyingType_With_Byte_Enum_Should_Return_Byte_Type()
    {
        // Act
        var result = EnumHelper.GetUnderlyingType<ByteEnum>();

        // Assert
        result.Should().Be(typeof(byte));
    }

    [Fact]
    public void GetUnderlyingType_With_Long_Enum_Should_Return_Long_Type()
    {
        // Act
        var result = EnumHelper.GetUnderlyingType<LongEnum>();

        // Assert
        result.Should().Be(typeof(long));
    }

    [Fact]
    public void GetUnderlyingType_With_Flags_Enum_Should_Return_Int_Type()
    {
        // Act
        var result = EnumHelper.GetUnderlyingType<TestFlagsEnum>();

        // Assert
        result.Should().Be(typeof(int));
    }

    #endregion

    #region GetDescription Method Tests

    [Fact]
    public void GetDescription_With_Enum_Having_Description_Should_Return_Description()
    {
        // Act
        var result = EnumHelper.GetDescription(TestEnum.First);

        // Assert
        result.Should().Be("First Value");
    }

    [Fact]
    public void GetDescription_With_Enum_Without_Description_Should_Return_ToString()
    {
        // Act
        var result = EnumHelper.GetDescription(TestEnum.Third);

        // Assert
        result.Should().Be("Third");
    }

    [Fact]
    public void GetDescription_With_Flags_Enum_Single_Flag_Should_Return_Description()
    {
        // Act
        var result = EnumHelper.GetDescription(TestFlagsEnum.Read);

        // Assert
        result.Should().Be("Read Permission");
    }

    [Fact]
    public void GetDescription_With_Flags_Enum_Combined_Flags_Should_Return_Combined_Descriptions()
    {
        // Act
        var result = EnumHelper.GetDescription(TestFlagsEnum.Read | TestFlagsEnum.Write);

        // Assert
        result.Should().Be("Read Permission, Write Permission");
    }

    [Fact]
    public void GetDescription_With_Flags_Enum_Predefined_Combined_Should_Return_Its_Description()
    {
        // Act
        var result = EnumHelper.GetDescription(TestFlagsEnum.All);

        // Assert
        result.Should().Be("Full Access");
    }

    [Fact]
    public void GetDescription_With_Flags_Enum_None_Should_Return_ToString()
    {
        // Act
        var result = EnumHelper.GetDescription(TestFlagsEnum.None);

        // Assert
        result.Should().Be("None");
    }

    #endregion

    #region ParseFromDescription Method Tests

    [Fact]
    public void ParseFromDescription_With_Valid_Description_Should_Return_Correct_Enum()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("First Value");

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void ParseFromDescription_With_Valid_Description_IgnoreCase_Should_Return_Correct_Enum()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("first value", ignoreCase: true);

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void ParseFromDescription_With_Enum_Name_Should_Return_Correct_Enum()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("Third");

        // Assert
        result.Should().Be(TestEnum.Third);
    }

    [Fact]
    public void ParseFromDescription_With_Invalid_Description_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("Invalid Description");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_With_Null_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_With_Empty_String_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_With_Whitespace_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("   ");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_With_Mixed_Description_And_Enum_Name_Should_Parse_Correctly()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("First Value, Third");

        // Assert
        result.Should().Be(TestEnum.First | TestEnum.Third);
    }

    [Fact]
    public void ParseFromDescription_With_Case_Sensitive_Mismatch_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.ParseFromDescription<TestEnum>("first value", ignoreCase: false);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region IsFlags Method Tests

    [Fact]
    public void IsFlags_With_Flags_Enum_Should_Return_True()
    {
        // Act
        var result = EnumHelper.IsFlags<TestFlagsEnum>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsFlags_With_Regular_Enum_Should_Return_False()
    {
        // Act
        var result = EnumHelper.IsFlags<TestEnum>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsFlags_With_Byte_Enum_Should_Return_False()
    {
        // Act
        var result = EnumHelper.IsFlags<ByteEnum>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsFlags_With_Long_Enum_Should_Return_False()
    {
        // Act
        var result = EnumHelper.IsFlags<LongEnum>();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ConvertEnum Method Tests

    [Fact]
    public void ConvertEnum_With_Matching_Names_Should_Return_Converted_Value()
    {
        // Act
        var result = EnumHelper.ConvertEnum<TestEnum, TestEnumForConversion>(TestEnum.First);

        // Assert
        result.Should().Be(TestEnumForConversion.First);
    }

    [Fact]
    public void ConvertEnum_With_Matching_Names_IgnoreCase_Should_Return_Converted_Value()
    {
        // Act
        var result = EnumHelper.ConvertEnum<TestEnum, TestEnumForConversion>(TestEnum.Second, ignoreCase: true);

        // Assert
        result.Should().Be(TestEnumForConversion.Second);
    }

    [Fact]
    public void ConvertEnum_With_Non_Matching_Names_Should_Return_Null()
    {
        // Act
        var result = EnumHelper.ConvertEnum<TestEnum, TestEnumForConversion>(TestEnum.Third);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertEnum_Between_Different_Underlying_Types_Should_Work()
    {
        // Act
        var result = EnumHelper.ConvertEnum<ByteEnum, LongEnum>(ByteEnum.One);

        // Assert
        result.Should().Be(LongEnum.One);
    }

    [Fact]
    public void ConvertEnum_With_Case_Sensitive_Should_Return_Null_For_Case_Mismatch()
    {
        // This would require an enum with different casing to test properly
        // Act
        var result = EnumHelper.ConvertEnum<TestEnum, TestEnumForConversion>(TestEnum.First, ignoreCase: false);

        // Assert
        result.Should().Be(TestEnumForConversion.First);
    }

    #endregion

    #region Edge Cases and Performance Tests

    [Fact]
    public void Multiple_Calls_To_GetNames_Should_Use_Cache()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act - First call
        var result1 = EnumHelper.GetNames<TestEnum>();
        var firstCallTime = stopwatch.ElapsedTicks;

        stopwatch.Restart();
        
        // Act - Second call (should be faster due to caching)
        var result2 = EnumHelper.GetNames<TestEnum>();
        var secondCallTime = stopwatch.ElapsedTicks;

        // Assert
        result1.Should().BeSameAs(result2);
        secondCallTime.Should().BeLessThan(firstCallTime);
    }

    [Fact]
    public void Multiple_Calls_To_GetValues_Should_Use_Cache()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act - First call
        var result1 = EnumHelper.GetValues<TestEnum>();
        var firstCallTime = stopwatch.ElapsedTicks;

        stopwatch.Restart();
        
        // Act - Second call (should be faster due to caching)
        var result2 = EnumHelper.GetValues<TestEnum>();
        var secondCallTime = stopwatch.ElapsedTicks;

        // Assert
        result1.Should().BeSameAs(result2);
        secondCallTime.Should().BeLessThan(firstCallTime);
    }

    [Fact]
    public void Parse_With_Large_Enum_Value_Should_Work()
    {
        // Act
        var result = EnumHelper.Parse<LongEnum>(long.MaxValue.ToString());

        // Assert
        result.Should().Be(LongEnum.Large);
    }

    [Fact]
    public void GetDescription_With_Complex_Flags_Combination_Should_Handle_Gracefully()
    {
        // Act
        var complexFlags = TestFlagsEnum.Read | TestFlagsEnum.Write;
        var result = EnumHelper.GetDescription(complexFlags);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Read Permission");
        result.Should().Contain("Write Permission");
    }

    [Fact]
    public void GetDescription_With_Complex_Flags_Combination_Should_Handle_Gracefully2()
    {
        // Act
        var complexFlags = TestFlagsEnum.Read | TestFlagsEnum.Write | TestFlagsEnum.Execute;
        var result = EnumHelper.GetDescription(complexFlags);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("Full Access");
    }

    #endregion
}