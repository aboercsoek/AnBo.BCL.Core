//--------------------------------------------------------------------------
// File:    ArrayExtensionsUnitTest.cs
// Content: Unit tests for ArrayExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Collections.ObjectModel;

namespace AnBo.Test;

public class ArrayExtensionsUnitTest
{
    #region ToReadOnly Tests

    [Fact]
    public void ToReadOnly_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.ToReadOnly();
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void ToReadOnly_With_Empty_Array_Should_Return_Empty_ReadOnlyCollection()
    {
        // Arrange
        var array = new int[0];

        // Act
        var result = array.ToReadOnly();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ReadOnlyCollection<int>>();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToReadOnly_With_Array_Should_Return_ReadOnlyCollection_With_Same_Elements()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = array.ToReadOnly();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ReadOnlyCollection<int>>();
        result.Should().HaveCount(5);
        result.Should().ContainInOrder(1, 2, 3, 4, 5);
    }

    [Fact]
    public void ToReadOnly_Should_Wrap_Original_Array()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act
        var result = array.ToReadOnly();
        array[0] = 10; // Modify original array

        // Assert
        result[0].Should().Be(10); // ReadOnlyCollection should reflect changes
    }

    #endregion

    #region Copy Tests

    [Fact]
    public void Copy_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.Copy();
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void Copy_With_Empty_Array_Should_Return_Empty_Array()
    {
        // Arrange
        var array = new int[0];

        // Act
        var result = array.Copy();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        result.Should().NotBeSameAs(array);
    }

    [Fact]
    public void Copy_Should_Create_Independent_Copy()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = array.Copy();

        // Assert
        result.Should().NotBeSameAs(array);
        result.Should().Equal(array);
        
        // Modify original array
        array[0] = 10;
        result[0].Should().Be(1); // Copy should not be affected
    }

    [Fact]
    public void Copy_Should_Handle_Reference_Types()
    {
        // Arrange
        var array = new[] { "hello", "world", "test" };

        // Act
        var result = array.Copy();

        // Assert
        result.Should().NotBeSameAs(array);
        result.Should().Equal(array);
        result.Should().HaveCount(3);
    }

    #endregion

    #region Slice Tests

    [Fact]
    public void Slice_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.Slice(0, 1);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void Slice_With_Negative_StartIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.Slice(-1, 1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("startIndex");
    }

    [Fact]
    public void Slice_With_Negative_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.Slice(0, -1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("length");
    }

    [Fact]
    public void Slice_With_StartIndex_Greater_Than_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.Slice(4, 1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("startIndex");
    }

    [Fact]
    public void Slice_With_StartIndex_Plus_Length_Greater_Than_Array_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.Slice(2, 2);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Slice_Should_Return_Correct_Portion()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = array.Slice(1, 3);

        // Assert
        result.Should().Equal(2, 3, 4);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void Slice_With_Zero_Length_Should_Return_Empty_Array()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act
        var result = array.Slice(1, 0);

        // Assert
        result.Should().BeEmpty();
        result.Should().NotBeNull();
    }

    [Fact]
    public void Slice_Should_Handle_Full_Array()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act
        var result = array.Slice(0, 3);

        // Assert
        result.Should().Equal(array);
        result.Should().NotBeSameAs(array);
    }

    #endregion

    #region SliceFrom Tests

    [Fact]
    public void SliceFrom_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.SliceFrom(0);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void SliceFrom_With_Negative_StartIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.SliceFrom(-1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("startIndex");
    }

    [Fact]
    public void SliceFrom_With_StartIndex_Greater_Than_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.SliceFrom(4);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("startIndex");
    }

    [Fact]
    public void SliceFrom_Should_Return_Portion_From_Index_To_End()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = array.SliceFrom(2);

        // Assert
        result.Should().Equal(3, 4, 5);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void SliceFrom_With_Start_Index_Zero_Should_Return_Full_Copy()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act
        var result = array.SliceFrom(0);

        // Assert
        result.Should().Equal(array);
        result.Should().NotBeSameAs(array);
    }

    [Fact]
    public void SliceFrom_With_Start_Index_Equal_To_Length_Should_Return_Empty_Array()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act
        var result = array.SliceFrom(3);

        // Assert
        result.Should().BeEmpty();
        result.Should().NotBeNull();
    }

    #endregion

    #region Fill Tests

    [Fact]
    public void Fill_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.Fill(42);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void Fill_Should_Fill_Entire_Array()
    {
        // Arrange
        var array = new int[5];

        // Act
        array.Fill(42);

        // Assert
        array.Should().AllBeEquivalentTo(42);
        array.Should().Equal(42, 42, 42, 42, 42);
    }

    [Fact]
    public void Fill_With_Empty_Array_Should_Not_Throw()
    {
        // Arrange
        var array = new int[0];

        // Act & Assert
        var action = () => array.Fill(42);
        action.Should().NotThrow();
    }

    [Fact]
    public void Fill_Should_Handle_Reference_Types()
    {
        // Arrange
        var array = new string[3];

        // Act
        array.Fill("test");

        // Assert
        array.Should().AllBeEquivalentTo("test");
        array.Should().Equal("test", "test", "test");
    }

    [Fact]
    public void Fill_With_StartIndex_And_Count_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.Fill(42, 0, 1);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void Fill_With_Negative_StartIndex_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new int[5];

        // Act & Assert
        var action = () => array.Fill(42, -1, 1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("startIndex");
    }

    [Fact]
    public void Fill_With_Negative_Count_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new int[5];

        // Act & Assert
        var action = () => array.Fill(42, 0, -1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("count");
    }

    [Fact]
    public void Fill_With_StartIndex_Plus_Count_Greater_Than_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new int[5];

        // Act & Assert
        var action = () => array.Fill(42, 3, 3);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Fill_With_StartIndex_And_Count_Should_Fill_Specified_Range()
    {
        // Arrange
        var array = new int[5] { 1, 2, 3, 4, 5 };

        // Act
        array.Fill(99, 1, 3);

        // Assert
        array.Should().Equal(1, 99, 99, 99, 5);
    }

    [Fact]
    public void Fill_With_Zero_Count_Should_Not_Change_Array()
    {
        // Arrange
        var array = new int[3] { 1, 2, 3 };
        var expected = array.Copy();

        // Act
        array.Fill(99, 1, 0);

        // Assert
        array.Should().Equal(expected);
    }

    #endregion

    #region BinarySearch Tests

    [Fact]
    public void BinarySearch_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.BinarySearch(42);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void BinarySearch_Should_Find_Element_In_Sorted_Array()
    {
        // Arrange
        var array = new[] { 1, 3, 5, 7, 9 };

        // Act
        var result = array.BinarySearch(5);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void BinarySearch_Should_Return_Negative_For_Missing_Element()
    {
        // Arrange
        var array = new[] { 1, 3, 5, 7, 9 };

        // Act
        var result = array.BinarySearch(6);

        // Assert
        result.Should().BeLessThan(0);
        var expectedIndex = ~result;
        expectedIndex.Should().Be(3); // Where 6 should be inserted
    }

    [Fact]
    public void BinarySearch_With_Custom_Comparer_Should_Use_Comparer()
    {
        // Arrange
        var array = new[] { "apple", "BANANA", "cherry" };
        Array.Sort(array, StringComparer.OrdinalIgnoreCase);

        // Act
        var result = array.BinarySearch("banana", StringComparer.OrdinalIgnoreCase);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void BinarySearch_With_Empty_Array_Should_Return_Negative_One()
    {
        // Arrange
        var array = new int[0];

        // Act
        var result = array.BinarySearch(42);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void BinarySearch_With_Range_And_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        var action = () => array!.BinarySearch(0, 1, 42);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void BinarySearch_With_Negative_Index_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.BinarySearch(-1, 1, 2);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("index");
    }

    [Fact]
    public void BinarySearch_With_Negative_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.BinarySearch(0, -1, 2);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("length");
    }

    [Fact]
    public void BinarySearch_With_Index_Plus_Length_Greater_Than_Array_Length_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => array.BinarySearch(2, 2, 2);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void BinarySearch_With_Range_Should_Search_Only_In_Range()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = array.BinarySearch(1, 3, 3);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void BinarySearch_With_Range_Should_Not_Find_Element_Outside_Range()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = array.BinarySearch(1, 2, 4); // Search for 4 in range [1..2]

        // Assert
        result.Should().BeLessThan(0);
    }

    #endregion

    #region AllFast Tests

    [Fact]
    public void AllFast_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;
        var predicate = new Func<int, bool>(x => x > 0);

        // Act & Assert
        var action = () => array!.AllFast(predicate);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void AllFast_With_Null_Predicate_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };
        Func<int, bool>? predicate = null;

        // Act & Assert
        var action = () => array.AllFast(predicate!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("predicate");
    }

    [Fact]
    public void AllFast_With_All_Elements_Matching_Should_Return_True()
    {
        // Arrange
        var array = new[] { 2, 4, 6, 8 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = array.AllFast(predicate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void AllFast_With_Some_Elements_Not_Matching_Should_Return_False()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = array.AllFast(predicate);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AllFast_With_Empty_Array_Should_Return_True()
    {
        // Arrange
        var array = new int[0];
        var predicate = new Func<int, bool>(x => x > 0);

        // Act
        var result = array.AllFast(predicate);

        // Assert
        result.Should().BeTrue(); // All of nothing is true
    }

    [Fact]
    public void AllFast_Should_Stop_At_First_Non_Matching_Element()
    {
        // Arrange
        var array = new[] { 2, 4, 1, 6, 8 };
        var callCount = 0;
        var predicate = new Func<int, bool>(x =>
        {
            callCount++;
            return x % 2 == 0;
        });

        // Act
        var result = array.AllFast(predicate);

        // Assert
        result.Should().BeFalse();
        callCount.Should().Be(3); // Should stop after finding 1 (odd number)
    }

    #endregion

    #region AnyFast Tests

    [Fact]
    public void AnyFast_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;
        var predicate = new Func<int, bool>(x => x > 0);

        // Act & Assert
        var action = () => array!.AnyFast(predicate);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void AnyFast_With_Null_Predicate_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };
        Func<int, bool>? predicate = null;

        // Act & Assert
        var action = () => array.AnyFast(predicate!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("predicate");
    }

    [Fact]
    public void AnyFast_With_Matching_Element_Should_Return_True()
    {
        // Arrange
        var array = new[] { 1, 3, 4, 5 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = array.AnyFast(predicate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void AnyFast_With_No_Matching_Elements_Should_Return_False()
    {
        // Arrange
        var array = new[] { 1, 3, 5, 7 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = array.AnyFast(predicate);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AnyFast_With_Empty_Array_Should_Return_False()
    {
        // Arrange
        var array = new int[0];
        var predicate = new Func<int, bool>(x => x > 0);

        // Act
        var result = array.AnyFast(predicate);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AnyFast_Should_Stop_At_First_Matching_Element()
    {
        // Arrange
        var array = new[] { 1, 3, 4, 6, 8 };
        var callCount = 0;
        var predicate = new Func<int, bool>(x =>
        {
            callCount++;
            return x % 2 == 0;
        });

        // Act
        var result = array.AnyFast(predicate);

        // Assert
        result.Should().BeTrue();
        callCount.Should().Be(3); // Should stop after finding 4 (first even number)
    }

    #endregion

    #region FindIndexFast Tests

    [Fact]
    public void FindIndexFast_With_Null_Array_Should_Throw_ArgumentNullException()
    {
        // Arrange
        int[]? array = null;
        var predicate = new Func<int, bool>(x => x > 0);

        // Act & Assert
        var action = () => array!.FindIndexFast(predicate);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("array");
    }

    [Fact]
    public void FindIndexFast_With_Null_Predicate_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };
        Func<int, bool>? predicate = null;

        // Act & Assert
        var action = () => array.FindIndexFast(predicate!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("predicate");
    }

    [Fact]
    public void FindIndexFast_Should_Return_Index_Of_First_Match()
    {
        // Arrange
        var array = new[] { 1, 3, 4, 6, 5 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = array.FindIndexFast(predicate);

        // Assert
        result.Should().Be(2); // Index of first even number (4)
    }

    [Fact]
    public void FindIndexFast_With_No_Match_Should_Return_Minus_One()
    {
        // Arrange
        var array = new[] { 1, 3, 5, 7 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = array.FindIndexFast(predicate);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void FindIndexFast_With_Empty_Array_Should_Return_Minus_One()
    {
        // Arrange
        var array = new int[0];
        var predicate = new Func<int, bool>(x => x > 0);

        // Act
        var result = array.FindIndexFast(predicate);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void FindIndexFast_Should_Return_Zero_When_First_Element_Matches()
    {
        // Arrange
        var array = new[] { 2, 3, 5, 7 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = array.FindIndexFast(predicate);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void FindIndexFast_Should_Stop_At_First_Match()
    {
        // Arrange
        var array = new[] { 1, 3, 4, 6, 8 };
        var callCount = 0;
        var predicate = new Func<int, bool>(x =>
        {
            callCount++;
            return x % 2 == 0;
        });

        // Act
        var result = array.FindIndexFast(predicate);

        // Assert
        result.Should().Be(2);
        callCount.Should().Be(3); // Should stop after finding 4
    }

    [Fact]
    public void FindIndexFast_With_String_Array_Should_Work_Correctly()
    {
        // Arrange
        var array = new[] { "apple", "banana", "cherry", "date" };
        var predicate = new Func<string, bool>(x => x.StartsWith("c"));

        // Act
        var result = array.FindIndexFast(predicate);

        // Assert
        result.Should().Be(2); // Index of "cherry"
    }

    #endregion
}