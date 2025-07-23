//--------------------------------------------------------------------------
// File:    EnumExtensionsUnitTest.cs
// Content: Unit tests for EnumExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.ComponentModel;

namespace AnBo.Test;

public class EnumExtensionsUnitTest
{
    #region Test Enums

    // Regular enum for testing
    private enum TestEnum
    {
        [Description("First Value")]
        First = 1,
        
        [Description("Second Value")]
        Second = 2,
        
        Third = 3, // No description attribute
        
        [Description("Fourth Value")]
        Fourth = 4
    }

    // Flags enum for testing
    [Flags]
    private enum TestFlags
    {
        None = 0,
        
        [Description("Read Permission")]
        Read = 1,
        
        [Description("Write Permission")]
        Write = 2,
        
        [Description("Execute Permission")]
        Execute = 4,
        
        All = Read | Write | Execute
    }

    // Simple enum without descriptions
    private enum SimpleEnum
    {
        Value1,
        Value2,
        Value3
    }

    // Enum for conversion testing
    private enum TargetEnum
    {
        First,
        Second,
        Third,
        Different
    }

    #endregion

    #region GetDescription Tests

    [Fact]
    public void GetDescription_With_Description_Attribute_Should_Return_Description()
    {
        // Act
        var result = TestEnum.First.GetDescription();

        // Assert
        result.Should().Be("First Value");
    }

    [Fact]
    public void GetDescription_Without_Description_Attribute_Should_Return_ToString()
    {
        // Act
        var result = TestEnum.Third.GetDescription();

        // Assert
        result.Should().Be("Third");
    }

    [Fact]
    public void GetDescription_With_Flags_Enum_Should_Return_Combined_Descriptions()
    {
        // Act
        var result = (TestFlags.Read | TestFlags.Write).GetDescription();

        // Assert
        result.Should().Be("Read Permission, Write Permission");
    }

    [Fact]
    public void GetDescription_With_Single_Flag_Should_Return_Single_Description()
    {
        // Act
        var result = TestFlags.Execute.GetDescription();

        // Assert
        result.Should().Be("Execute Permission");
    }

    [Fact]
    public void GetDescription_With_Simple_Enum_Should_Return_ToString()
    {
        // Act
        var result = SimpleEnum.Value1.GetDescription();

        // Assert
        result.Should().Be("Value1");
    }

    #endregion

    #region ParseAsEnum Tests

    [Fact]
    public void ParseAsEnum_With_Valid_Name_Should_Return_Enum_Value()
    {
        // Act
        var result = "First".ParseAsEnum<TestEnum>();

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void ParseAsEnum_With_Valid_Name_Ignore_Case_Should_Return_Enum_Value()
    {
        // Act
        var result = "first".ParseAsEnum<TestEnum>(ignoreCase: true);

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void ParseAsEnum_With_Invalid_Name_Should_Return_Null()
    {
        // Act
        var result = "Invalid".ParseAsEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseAsEnum_With_Null_String_Should_Return_Null()
    {
        // Act
        var result = ((string?)null).ParseAsEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseAsEnum_With_Empty_String_Should_Return_Null()
    {
        // Act
        var result = string.Empty.ParseAsEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseAsEnum_With_Whitespace_Should_Return_Null()
    {
        // Act
        var result = "   ".ParseAsEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseAsEnum_With_Numeric_String_Should_Return_Enum_Value()
    {
        // Act
        var result = "2".ParseAsEnum<TestEnum>();

        // Assert
        result.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void ParseAsEnum_Case_Sensitive_With_Wrong_Case_Should_Return_Null()
    {
        // Act
        var result = "first".ParseAsEnum<TestEnum>(ignoreCase: false);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region ParseFromDescription Tests

    [Fact]
    public void ParseFromDescription_With_Valid_Description_Should_Return_Enum_Value()
    {
        // Act
        var result = "First Value".ParseFromDescription<TestEnum>();

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void ParseFromDescription_With_Valid_Description_Ignore_Case_Should_Return_Enum_Value()
    {
        // Act
        var result = "first value".ParseFromDescription<TestEnum>(ignoreCase: true);

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void ParseFromDescription_With_Invalid_Description_Should_Return_Null()
    {
        // Act
        var result = "Invalid Description".ParseFromDescription<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_With_Enum_Name_Should_Return_Enum_Value()
    {
        // Act
        var result = "Third".ParseFromDescription<TestEnum>();

        // Assert
        result.Should().Be(TestEnum.Third);
    }

    [Fact]
    public void ParseFromDescription_With_Null_String_Should_Return_Null()
    {
        // Act
        var result = ((string?)null).ParseFromDescription<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_With_Empty_String_Should_Return_Null()
    {
        // Act
        var result = string.Empty.ParseFromDescription<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_With_Whitespace_Should_Return_Null()
    {
        // Act
        var result = "   ".ParseFromDescription<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromDescription_Case_Sensitive_With_Wrong_Case_Should_Return_Null()
    {
        // Act
        var result = "first value".ParseFromDescription<TestEnum>(ignoreCase: false);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region IsFlags Tests

    [Fact]
    public void IsFlags_With_Flags_Enum_Should_Return_True()
    {
        // Act
        var result = TestFlags.Read.IsFlags();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsFlags_With_Regular_Enum_Should_Return_False()
    {
        // Act
        var result = TestEnum.First.IsFlags();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsFlags_With_Simple_Enum_Should_Return_False()
    {
        // Act
        var result = SimpleEnum.Value1.IsFlags();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetFlags Tests

    [Fact]
    public void GetFlags_With_Single_Flag_Should_Return_Single_Flag()
    {
        // Act
        var result = TestFlags.Read.GetFlags().ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(TestFlags.Read);
    }

    [Fact]
    public void GetFlags_With_Multiple_Flags_Should_Return_All_Set_Flags()
    {
        // Act
        var result = (TestFlags.Read | TestFlags.Write).GetFlags().ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(TestFlags.Read);
        result.Should().Contain(TestFlags.Write);
        result.Should().NotContain(TestFlags.Execute);
    }

    [Fact]
    public void GetFlags_With_All_Flags_Should_Return_All_Individual_Flags()
    {
        // Act
        var result = TestFlags.All.GetFlags().ToList();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(TestFlags.Read);
        result.Should().Contain(TestFlags.Write);
        result.Should().Contain(TestFlags.Execute);
        result.Should().Contain(TestFlags.All);
    }

    [Fact]
    public void GetFlags_With_None_Flag_Should_Return_Empty()
    {
        // Act
        var result = TestFlags.None.GetFlags().ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetFlags_With_Non_Flags_Enum_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => TestEnum.First.GetFlags().ToList();
        action.Should().Throw<ArgumentException>()
            .WithMessage("*not a flags enumeration*")
            .And.ParamName.Should().Be("TEnum");
    }

    #endregion

    #region AddFlag Tests

    [Fact]
    public void AddFlag_With_New_Flag_Should_Add_Flag()
    {
        // Act
        var result = TestFlags.Read.AddFlag(TestFlags.Write);

        // Assert
        result.Should().Be(TestFlags.Read | TestFlags.Write);
        result.HasFlag(TestFlags.Read).Should().BeTrue();
        result.HasFlag(TestFlags.Write).Should().BeTrue();
    }

    [Fact]
    public void AddFlag_With_Existing_Flag_Should_Keep_Both_Flags()
    {
        // Arrange
        var initial = TestFlags.Read | TestFlags.Write;

        // Act
        var result = initial.AddFlag(TestFlags.Read);

        // Assert
        result.Should().Be(TestFlags.Read | TestFlags.Write);
        result.HasFlag(TestFlags.Read).Should().BeTrue();
        result.HasFlag(TestFlags.Write).Should().BeTrue();
    }

    [Fact]
    public void AddFlag_With_None_Should_Add_Flag()
    {
        // Act
        var result = TestFlags.None.AddFlag(TestFlags.Execute);

        // Assert
        result.Should().Be(TestFlags.Execute);
        result.HasFlag(TestFlags.Execute).Should().BeTrue();
    }

    [Fact]
    public void AddFlag_With_Non_Flags_Enum_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => TestEnum.First.AddFlag(TestEnum.Second);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*not a flags enumeration*")
            .And.ParamName.Should().Be("TEnum");
    }

    #endregion

    #region RemoveFlag Tests

    [Fact]
    public void RemoveFlag_With_Existing_Flag_Should_Remove_Flag()
    {
        // Arrange
        var initial = TestFlags.Read | TestFlags.Write;

        // Act
        var result = initial.RemoveFlag(TestFlags.Write);

        // Assert
        result.Should().Be(TestFlags.Read);
        result.HasFlag(TestFlags.Read).Should().BeTrue();
        result.HasFlag(TestFlags.Write).Should().BeFalse();
    }

    [Fact]
    public void RemoveFlag_With_Non_Existing_Flag_Should_Not_Change_Value()
    {
        // Act
        var result = TestFlags.Read.RemoveFlag(TestFlags.Write);

        // Assert
        result.Should().Be(TestFlags.Read);
        result.HasFlag(TestFlags.Read).Should().BeTrue();
        result.HasFlag(TestFlags.Write).Should().BeFalse();
    }

    [Fact]
    public void RemoveFlag_All_Flags_Should_Result_In_None()
    {
        // Arrange
        var initial = TestFlags.Read | TestFlags.Write | TestFlags.Execute;

        // Act
        var result = initial.RemoveFlag(TestFlags.Read)
                           .RemoveFlag(TestFlags.Write)
                           .RemoveFlag(TestFlags.Execute);

        // Assert
        result.Should().Be(TestFlags.None);
    }

    [Fact]
    public void RemoveFlag_With_Non_Flags_Enum_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => TestEnum.First.RemoveFlag(TestEnum.Second);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*not a flags enumeration*")
            .And.ParamName.Should().Be("TEnum");
    }

    #endregion

    #region ToggleFlag Tests

    [Fact]
    public void ToggleFlag_With_Existing_Flag_Should_Remove_Flag()
    {
        // Arrange
        var initial = TestFlags.Read | TestFlags.Write;

        // Act
        var result = initial.ToggleFlag(TestFlags.Read);

        // Assert
        result.Should().Be(TestFlags.Write);
        result.HasFlag(TestFlags.Read).Should().BeFalse();
        result.HasFlag(TestFlags.Write).Should().BeTrue();
    }

    [Fact]
    public void ToggleFlag_With_Non_Existing_Flag_Should_Add_Flag()
    {
        // Act
        var result = TestFlags.Read.ToggleFlag(TestFlags.Write);

        // Assert
        result.Should().Be(TestFlags.Read | TestFlags.Write);
        result.HasFlag(TestFlags.Read).Should().BeTrue();
        result.HasFlag(TestFlags.Write).Should().BeTrue();
    }

    [Fact]
    public void ToggleFlag_Multiple_Times_Should_Return_To_Original()
    {
        // Arrange
        var initial = TestFlags.Read;

        // Act
        var result = initial.ToggleFlag(TestFlags.Write)
                           .ToggleFlag(TestFlags.Write);

        // Assert
        result.Should().Be(initial);
    }

    [Fact]
    public void ToggleFlag_With_Non_Flags_Enum_Should_Throw_ArgumentException()
    {
        // Act & Assert
        var action = () => TestEnum.First.ToggleFlag(TestEnum.Second);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*not a flags enumeration*")
            .And.ParamName.Should().Be("TEnum");
    }

    #endregion

    #region ConvertTo Tests

    [Fact]
    public void ConvertTo_With_Matching_Name_Should_Return_Target_Enum()
    {
        // Act
        var result = TestEnum.First.ConvertTo<TestEnum, TargetEnum>();

        // Assert
        result.Should().Be(TargetEnum.First);
    }

    [Fact]
    public void ConvertTo_With_Non_Matching_Name_Should_Return_Null()
    {
        // Act
        var result = TestEnum.Fourth.ConvertTo<TestEnum, TargetEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertTo_With_Ignore_Case_Should_Work()
    {
        // Act
        var result = TestEnum.First.ConvertTo<TestEnum, TargetEnum>(ignoreCase: true);

        // Assert
        result.Should().Be(TargetEnum.First);
    }

    [Fact]
    public void ConvertTo_Same_Type_Should_Return_Same_Value()
    {
        // Act
        var result = TestEnum.Second.ConvertTo<TestEnum, TestEnum>();

        // Assert
        result.Should().Be(TestEnum.Second);
    }

    #endregion

    #region ToJsonString Tests

    [Fact]
    public void ToJsonString_With_Description_Should_Return_Description()
    {
        // Act
        var result = TestEnum.First.ToJsonString(useDescription: true);

        // Assert
        result.Should().Be("First Value");
    }

    [Fact]
    public void ToJsonString_Without_Description_Should_Return_Name()
    {
        // Act
        var result = TestEnum.First.ToJsonString(useDescription: false);

        // Assert
        result.Should().Be("First");
    }

    [Fact]
    public void ToJsonString_Default_Should_Use_Description()
    {
        // Act
        var result = TestEnum.Second.ToJsonString();

        // Assert
        result.Should().Be("Second Value");
    }

    [Fact]
    public void ToJsonString_Without_Description_Attribute_Should_Return_Name()
    {
        // Act
        var result = TestEnum.Third.ToJsonString();

        // Assert
        result.Should().Be("Third");
    }

    #endregion

    #region GetNumericValue Tests

    [Fact]
    public void GetNumericValue_Should_Return_Underlying_Value_As_Int()
    {
        // Act
        var result = TestEnum.Second.GetNumericValue<TestEnum, int>();

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void GetNumericValue_Should_Return_Underlying_Value_As_Long()
    {
        // Act
        var result = TestEnum.Fourth.GetNumericValue<TestEnum, long>();

        // Assert
        result.Should().Be(4L);
    }

    [Fact]
    public void GetNumericValue_Should_Return_Underlying_Value_As_Byte()
    {
        // Act
        var result = TestEnum.First.GetNumericValue<TestEnum, byte>();

        // Assert
        result.Should().Be((byte)1);
    }

    [Fact]
    public void GetNumericValue_With_Zero_Based_Enum_Should_Work()
    {
        // Act
        var result = SimpleEnum.Value1.GetNumericValue<SimpleEnum, int>();

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region GetAllValues Tests

    [Fact]
    public void GetAllValues_Should_Return_All_Enum_Values()
    {
        // Act
        var result = TestEnum.First.GetAllValues();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(TestEnum.First);
        result.Should().Contain(TestEnum.Second);
        result.Should().Contain(TestEnum.Third);
        result.Should().Contain(TestEnum.Fourth);
    }

    [Fact]
    public void GetAllValues_Should_Return_Same_Result_For_Different_Instances()
    {
        // Act
        var result1 = TestEnum.First.GetAllValues();
        var result2 = TestEnum.Third.GetAllValues();

        // Assert
        result1.Should().BeEquivalentTo(result2);
    }

    [Fact]
    public void GetAllValues_With_Simple_Enum_Should_Return_All_Values()
    {
        // Act
        var result = SimpleEnum.Value1.GetAllValues();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(SimpleEnum.Value1);
        result.Should().Contain(SimpleEnum.Value2);
        result.Should().Contain(SimpleEnum.Value3);
    }

    #endregion

    #region GetAllNames Tests

    [Fact]
    public void GetAllNames_Should_Return_All_Enum_Names()
    {
        // Act
        var result = TestEnum.First.GetAllNames();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain("First");
        result.Should().Contain("Second");
        result.Should().Contain("Third");
        result.Should().Contain("Fourth");
    }

    [Fact]
    public void GetAllNames_Should_Return_Same_Result_For_Different_Instances()
    {
        // Act
        var result1 = TestEnum.First.GetAllNames();
        var result2 = TestEnum.Second.GetAllNames();

        // Assert
        result1.Should().BeEquivalentTo(result2);
    }

    [Fact]
    public void GetAllNames_With_Simple_Enum_Should_Return_All_Names()
    {
        // Act
        var result = SimpleEnum.Value2.GetAllNames();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain("Value1");
        result.Should().Contain("Value2");
        result.Should().Contain("Value3");
    }

    #endregion

    #region IsDefault Tests

    [Fact]
    public void IsDefault_With_Default_Value_Should_Return_True()
    {
        // Act
        var result = SimpleEnum.Value1.IsDefault(); // Value1 = 0 (default)

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefault_With_Non_Default_Value_Should_Return_False()
    {
        // Act
        var result = TestEnum.First.IsDefault(); // First = 1 (not default)

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDefault_With_Explicit_Default_Enum_Should_Return_True()
    {
        // Arrange
        var defaultValue = default(TestEnum); // Should be 0

        // Act
        var result = defaultValue.IsDefault();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefault_With_Flags_None_Should_Return_True()
    {
        // Act
        var result = TestFlags.None.IsDefault(); // None = 0 (default)

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetNext Tests

    [Fact]
    public void GetNext_Should_Return_Next_Enum_Value()
    {
        // Act
        var result = TestEnum.First.GetNext();

        // Assert
        result.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void GetNext_With_Last_Value_Should_Wrap_To_First()
    {
        // Act
        var result = TestEnum.Fourth.GetNext();

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void GetNext_With_Simple_Enum_Should_Work()
    {
        // Act
        var result = SimpleEnum.Value1.GetNext();

        // Assert
        result.Should().Be(SimpleEnum.Value2);
    }

    [Fact]
    public void GetNext_Multiple_Times_Should_Cycle_Through_All_Values()
    {
        // Arrange
        var current = SimpleEnum.Value1;

        // Act & Assert
        current = current.GetNext();
        current.Should().Be(SimpleEnum.Value2);

        current = current.GetNext();
        current.Should().Be(SimpleEnum.Value3);

        current = current.GetNext();
        current.Should().Be(SimpleEnum.Value1); // Wrapped around
    }

    #endregion

    #region GetPrevious Tests

    [Fact]
    public void GetPrevious_Should_Return_Previous_Enum_Value()
    {
        // Act
        var result = TestEnum.Second.GetPrevious();

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void GetPrevious_With_First_Value_Should_Wrap_To_Last()
    {
        // Act
        var result = TestEnum.First.GetPrevious();

        // Assert
        result.Should().Be(TestEnum.Fourth);
    }

    [Fact]
    public void GetPrevious_With_Simple_Enum_Should_Work()
    {
        // Act
        var result = SimpleEnum.Value2.GetPrevious();

        // Assert
        result.Should().Be(SimpleEnum.Value1);
    }

    [Fact]
    public void GetPrevious_Multiple_Times_Should_Cycle_Through_All_Values()
    {
        // Arrange
        var current = SimpleEnum.Value3;

        // Act & Assert
        current = current.GetPrevious();
        current.Should().Be(SimpleEnum.Value2);

        current = current.GetPrevious();
        current.Should().Be(SimpleEnum.Value1);

        current = current.GetPrevious();
        current.Should().Be(SimpleEnum.Value3); // Wrapped around
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void GetNext_And_GetPrevious_Should_Be_Inverse_Operations()
    {
        // Arrange
        var original = TestEnum.Second;

        // Act
        var next = original.GetNext();
        var backToPrevious = next.GetPrevious();

        // Assert
        backToPrevious.Should().Be(original);
    }

    [Fact]
    public void Flags_Operations_Should_Work_Together()
    {
        // Arrange
        var initial = TestFlags.None;

        // Act
        var withRead = initial.AddFlag(TestFlags.Read);
        var withReadWrite = withRead.AddFlag(TestFlags.Write);
        var toggledExecute = withReadWrite.ToggleFlag(TestFlags.Execute);
        var removedRead = toggledExecute.RemoveFlag(TestFlags.Read);

        // Assert
        removedRead.Should().Be(TestFlags.Write | TestFlags.Execute);
        removedRead.HasFlag(TestFlags.Write).Should().BeTrue();
        removedRead.HasFlag(TestFlags.Execute).Should().BeTrue();
        removedRead.HasFlag(TestFlags.Read).Should().BeFalse();
    }

    [Fact]
    public void Parse_And_ToJsonString_Should_Be_Compatible()
    {
        // Arrange
        var original = TestEnum.Second;
        var description = original.GetDescription();

        // Act
        var parsed = description.ParseFromDescription<TestEnum>();
        var jsonString = original.ToJsonString();

        // Assert
        parsed.Should().Be(original);
        jsonString.Should().Be(description);
    }

    [Fact]
    public void GetAllValues_And_Navigation_Should_Be_Consistent()
    {
        // Arrange
        var allValues = TestEnum.First.GetAllValues();

        // Act & Assert
        for (int i = 0; i < allValues.Length; i++)
        {
            var current = allValues[i];
            var next = current.GetNext();
            var expectedNext = allValues[(i + 1) % allValues.Length];
            
            next.Should().Be(expectedNext);
        }
    }

    [Fact]
    public void Conversion_Between_Similar_Enums_Should_Work()
    {
        // Act
        var converted = TestEnum.First.ConvertTo<TestEnum, TargetEnum>();
        var backConverted = converted?.ToString().ParseAsEnum<TestEnum>();

        // Assert
        converted.Should().Be(TargetEnum.First);
        backConverted.Should().Be(TestEnum.First);
    }

    #endregion
}