//--------------------------------------------------------------------------
// File:    IndexValuePairUnitTest.cs
// Content:	Unit tests for IndexValuePair struct and static class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using AnBo.Core;
using FluentAssertions;

namespace AnBo.Test;

public class IndexValuePairUnitTest
{
    #region Constructor Tests

    [Fact]
    public void Constructor_With_Valid_Parameters_Should_Set_Properties()
    {
        // Arrange
        string value = "test";
        int index = 5;

        // Act
        var pair = new IndexValuePair<string>(value, index);

        // Assert
        pair.Value.Should().Be("test");
        pair.Index.Should().Be(5);
    }

    [Fact]
    public void Constructor_With_Zero_Index_Should_Set_Properties()
    {
        // Arrange
        int value = 42;
        int index = 0;

        // Act
        var pair = new IndexValuePair<int>(value, index);

        // Assert
        pair.Value.Should().Be(42);
        pair.Index.Should().Be(0);
    }

    [Fact]
    public void Constructor_With_Max_Index_Should_Set_Properties()
    {
        // Arrange
        string value = "max";
        int index = int.MaxValue;

        // Act
        var pair = new IndexValuePair<string>(value, index);

        // Assert
        pair.Value.Should().Be("max");
        pair.Index.Should().Be(int.MaxValue);
    }

    [Fact]
    public void Constructor_With_Negative_Index_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string value = "test";
        int index = -1;

        // Act & Assert
        var action = () => new IndexValuePair<string>(value, index);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_With_Null_Value_Should_Set_Null_Value()
    {
        // Arrange
        string? value = null;
        int index = 0;

        // Act
        var pair = new IndexValuePair<string?>(value, index);

        // Assert
        pair.Value.Should().BeNull();
        pair.Index.Should().Be(0);
    }

    #endregion

    #region Deconstruct Method Tests

    [Fact]
    public void Deconstruct_Should_Return_Correct_Index_And_Value()
    {
        // Arrange
        var pair = new IndexValuePair<string>("hello", 3);

        // Act
        var (index, value) = pair;

        // Assert
        index.Should().Be(3);
        value.Should().Be("hello");
    }

    [Fact]
    public void Deconstruct_With_Value_Type_Should_Return_Correct_Values()
    {
        // Arrange
        var pair = new IndexValuePair<int>(100, 7);

        // Act
        pair.Deconstruct(out int index, out int value);

        // Assert
        index.Should().Be(7);
        value.Should().Be(100);
    }

    [Fact]
    public void Deconstruct_With_Null_Value_Should_Return_Null()
    {
        // Arrange
        var pair = new IndexValuePair<string?>(null, 2);

        // Act
        var (index, value) = pair;

        // Assert
        index.Should().Be(2);
        value.Should().BeNull();
    }

    #endregion

    #region Equals Method Tests

    [Fact]
    public void Equals_With_Same_Values_Should_Return_True()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 1);
        var pair2 = new IndexValuePair<string>("test", 1);

        // Act
        var result = pair1.Equals(pair2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_With_Different_Values_Should_Return_False()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test1", 1);
        var pair2 = new IndexValuePair<string>("test2", 1);

        // Act
        var result = pair1.Equals(pair2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_With_Different_Indices_Should_Return_False()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 1);
        var pair2 = new IndexValuePair<string>("test", 2);

        // Act
        var result = pair1.Equals(pair2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_With_Both_Different_Should_Return_False()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test1", 1);
        var pair2 = new IndexValuePair<string>("test2", 2);

        // Act
        var result = pair1.Equals(pair2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_With_Null_Values_Should_Return_True()
    {
        // Arrange
        var pair1 = new IndexValuePair<string?>(null, 1);
        var pair2 = new IndexValuePair<string?>(null, 1);

        // Act
        var result = pair1.Equals(pair2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_With_One_Null_Value_Should_Return_False()
    {
        // Arrange
        var pair1 = new IndexValuePair<string?>("test", 1);
        var pair2 = new IndexValuePair<string?>(null, 1);

        // Act
        var result = pair1.Equals(pair2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_Object_With_Same_Values_Should_Return_True()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 1);
        object pair2 = new IndexValuePair<string>("test", 1);

        // Act
        var result = pair1.Equals(pair2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_Object_With_Different_Type_Should_Return_False()
    {
        // Arrange
        var pair = new IndexValuePair<string>("test", 1);
        object other = "test";

        // Act
        var result = pair.Equals(other);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_Object_With_Null_Should_Return_False()
    {
        // Arrange
        var pair = new IndexValuePair<string>("test", 1);
        object? other = null;

        // Act
        var result = pair.Equals(other);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_With_Different_Generic_Types_Should_Return_False()
    {
        // Arrange
        var stringPair = new IndexValuePair<string>("42", 1);
        var intPair = new IndexValuePair<int>(42, 1);

        // Act
        var result = stringPair.Equals((object)intPair);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetHashCode Method Tests

    [Fact]
    public void GetHashCode_With_Same_Values_Should_Return_Same_HashCode()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 1);
        var pair2 = new IndexValuePair<string>("test", 1);

        // Act
        var hash1 = pair1.GetHashCode();
        var hash2 = pair2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_With_Different_Values_Should_Return_Different_HashCodes()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test1", 1);
        var pair2 = new IndexValuePair<string>("test2", 1);

        // Act
        var hash1 = pair1.GetHashCode();
        var hash2 = pair2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GetHashCode_With_Different_Indices_Should_Return_Different_HashCodes()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 1);
        var pair2 = new IndexValuePair<string>("test", 2);

        // Act
        var hash1 = pair1.GetHashCode();
        var hash2 = pair2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GetHashCode_With_Null_Value_Should_Not_Throw()
    {
        // Arrange
        var pair = new IndexValuePair<string?>(null, 1);

        // Act & Assert
        var action = () => pair.GetHashCode();
        action.Should().NotThrow();
    }

    #endregion

    #region ToString Method Tests

    [Fact]
    public void ToString_With_String_Value_Should_Format_Correctly()
    {
        // Arrange
        var pair = new IndexValuePair<string>("hello", 3);

        // Act
        var result = pair.ToString();

        // Assert
        result.Should().Be("[3]: hello");
    }

    [Fact]
    public void ToString_With_Integer_Value_Should_Format_Correctly()
    {
        // Arrange
        var pair = new IndexValuePair<int>(42, 0);

        // Act
        var result = pair.ToString();

        // Assert
        result.Should().Be("[0]: 42");
    }

    [Fact]
    public void ToString_With_Null_Value_Should_Format_Correctly()
    {
        // Arrange
        var pair = new IndexValuePair<string?>(null, 5);

        // Act
        var result = pair.ToString();

        // Assert
        result.Should().Be("[5]: ");
    }

    [Fact]
    public void ToString_With_Large_Index_Should_Format_Correctly()
    {
        // Arrange
        var pair = new IndexValuePair<string>("test", 999999);

        // Act
        var result = pair.ToString();

        // Assert
        result.Should().Be("[999999]: test");
    }

    [Fact]
    public void ToString_With_Complex_Object_Should_Use_Object_ToString()
    {
        // Arrange
        var testObject = new TestClass { Name = "TestName", Value = 123 };
        var pair = new IndexValuePair<TestClass>(testObject, 2);

        // Act
        var result = pair.ToString();

        // Assert
        result.Should().Be("[2]: TestName:123");
    }

    #endregion

    #region Equality Operators Tests

    [Fact]
    public void EqualityOperator_With_Same_Values_Should_Return_True()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 1);
        var pair2 = new IndexValuePair<string>("test", 1);

        // Act
        var result = pair1 == pair2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_With_Different_Values_Should_Return_False()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test1", 1);
        var pair2 = new IndexValuePair<string>("test2", 1);

        // Act
        var result = pair1 == pair2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_With_Same_Values_Should_Return_False()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 1);
        var pair2 = new IndexValuePair<string>("test", 1);

        // Act
        var result = pair1 != pair2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_With_Different_Values_Should_Return_True()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test1", 1);
        var pair2 = new IndexValuePair<string>("test2", 1);

        // Act
        var result = pair1 != pair2;

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Implicit Conversion Operators Tests

    [Fact]
    public void ImplicitConversion_From_Tuple_Should_Create_Correct_Pair()
    {
        // Arrange
        var tuple = ("hello", 3);

        // Act
        IndexValuePair<string> pair = tuple;

        // Assert
        pair.Value.Should().Be("hello");
        pair.Index.Should().Be(3);
    }

    [Fact]
    public void ImplicitConversion_To_Tuple_Should_Create_Correct_Tuple()
    {
        // Arrange
        var pair = new IndexValuePair<string>("hello", 3);

        // Act
        (string Value, int Index) tuple = pair;

        // Assert
        tuple.Value.Should().Be("hello");
        tuple.Index.Should().Be(3);
    }

    [Fact]
    public void ImplicitConversion_From_Tuple_With_Value_Type_Should_Work()
    {
        // Arrange
        var tuple = (42, 0);

        // Act
        IndexValuePair<int> pair = tuple;

        // Assert
        pair.Value.Should().Be(42);
        pair.Index.Should().Be(0);
    }

    [Fact]
    public void ImplicitConversion_To_Tuple_With_Value_Type_Should_Work()
    {
        // Arrange
        var pair = new IndexValuePair<int>(42, 0);

        // Act
        (int Value, int Index) tuple = pair;

        // Assert
        tuple.Value.Should().Be(42);
        tuple.Index.Should().Be(0);
    }

    [Fact]
    public void ImplicitConversion_From_Tuple_With_Null_Value_Should_Work()
    {
        // Arrange
        var tuple = ((string?)null, 1);

        // Act
        IndexValuePair<string?> pair = tuple;

        // Assert
        pair.Value.Should().BeNull();
        pair.Index.Should().Be(1);
    }

    #endregion

    #region Static Create Method Tests

    [Fact]
    public void Create_With_Valid_Parameters_Should_Return_Correct_Pair()
    {
        // Arrange
        string value = "test";
        int index = 2;

        // Act
        var pair = IndexValuePair.Create(value, index);

        // Assert
        pair.Value.Should().Be("test");
        pair.Index.Should().Be(2);
    }

    [Fact]
    public void Create_With_Value_Type_Should_Return_Correct_Pair()
    {
        // Arrange
        int value = 42;
        int index = 5;

        // Act
        var pair = IndexValuePair.Create(value, index);

        // Assert
        pair.Value.Should().Be(42);
        pair.Index.Should().Be(5);
    }

    [Fact]
    public void Create_With_Null_Value_Should_Return_Correct_Pair()
    {
        // Arrange
        string? value = null;
        int index = 0;

        // Act
        var pair = IndexValuePair.Create(value, index);

        // Assert
        pair.Value.Should().BeNull();
        pair.Index.Should().Be(0);
    }

    [Fact]
    public void Create_With_Negative_Index_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        string value = "test";
        int index = -5;

        // Act & Assert
        var action = () => IndexValuePair.Create(value, index);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_Should_Infer_Generic_Type_From_Value()
    {
        // Arrange & Act
        var stringPair = IndexValuePair.Create("hello", 1);
        var intPair = IndexValuePair.Create(42, 2);
        var doublePair = IndexValuePair.Create(3.14, 3);

        // Assert
        stringPair.Should().BeOfType<IndexValuePair<string>>();
        stringPair.Value.Should().Be("hello");

        intPair.Should().BeOfType<IndexValuePair<int>>();
        intPair.Value.Should().Be(42);

        doublePair.Should().BeOfType<IndexValuePair<double>>();
        doublePair.Value.Should().Be(3.14);
    }

    #endregion

    #region Edge Cases and Complex Scenarios

    [Fact]
    public void Constructor_And_Create_Should_Produce_Equal_Results()
    {
        // Arrange
        string value = "test";
        int index = 1;

        // Act
        var constructorPair = new IndexValuePair<string>(value, index);
        var createPair = IndexValuePair.Create(value, index);

        // Assert
        constructorPair.Should().Be(createPair);
        constructorPair.Equals(createPair).Should().BeTrue();
        (constructorPair == createPair).Should().BeTrue();
    }

    [Fact]
    public void Multiple_Operations_Should_Work_Together()
    {
        // Arrange
        var original = IndexValuePair.Create("hello", 0);

        // Act
        var (index, value) = original;
        var tuple = (value, index + 1);
        IndexValuePair<string> newPair = tuple;
        var stringRepresentation = newPair.ToString();

        // Assert
        index.Should().Be(0);
        value.Should().Be("hello");
        newPair.Value.Should().Be("hello");
        newPair.Index.Should().Be(1);
        stringRepresentation.Should().Be("[1]: hello");
    }

    [Fact]
    public void StringArray_To_IndexedArray_With_Select_Should_Work()
    {
        var words = new[] { "Hello", "World", "C#" };
        var indexed = words.Select((word, index) => new IndexValuePair<string>(word, index));

        indexed.Should().HaveCount(3);
        // With deconstruction
        foreach (var pair in indexed)
        {
            var (index, word) = pair;
            switch (index)
            {
                case 0:
                    word.Should().Be("Hello");
                    break;
                case 1:
                    word.Should().Be("World");
                    break;
                case 2:
                    word.Should().Be("C#");
                    break;
                default:
                    throw new InvalidOperationException("Unexpected index");
            }
        }
    }

    [Fact]
    public void Pairs_With_Same_Values_Should_Have_Same_HashCode_And_Be_Equal()
    {
        // Arrange
        var pair1 = new IndexValuePair<string>("test", 3);
        var pair2 = IndexValuePair.Create("test", 3);
        (string, int) tuple = ("test", 3);
        IndexValuePair<string> pair3 = tuple;

        // Act & Assert
        pair1.Should().Be(pair2);
        pair1.Should().Be(pair3);
        pair2.Should().Be(pair3);

        pair1.GetHashCode().Should().Be(pair2.GetHashCode());
        pair1.GetHashCode().Should().Be(pair3.GetHashCode());

        (pair1 == pair2).Should().BeTrue();
        (pair1 == pair3).Should().BeTrue();
        (pair2 == pair3).Should().BeTrue();
    }

    [Fact]
    public void IEquatable_Implementation_Should_Be_Consistent_With_Operators()
    {
        // Arrange
        var pair1 = new IndexValuePair<int>(42, 5);
        var pair2 = new IndexValuePair<int>(42, 5);
        var pair3 = new IndexValuePair<int>(43, 5);

        // Act & Assert
        pair1.Equals(pair2).Should().Be(pair1 == pair2);
        pair1.Equals(pair3).Should().Be(pair1 == pair3);
        (!pair1.Equals(pair3)).Should().Be(pair1 != pair3);
    }

    [Fact]
    public void Struct_Should_Be_Value_Type()
    {
        // Arrange
        var pair = new IndexValuePair<string>("test", 1);

        // Act & Assert
        typeof(IndexValuePair<string>).IsValueType.Should().BeTrue();
        pair.GetType().IsValueType.Should().BeTrue();
    }

    [Fact]
    public void Properties_Should_Be_ReadOnly()
    {
        // Arrange
        var pair = new IndexValuePair<string>("test", 1);

        // Act & Assert
        var valueProperty = typeof(IndexValuePair<string>).GetProperty(nameof(IndexValuePair<string>.Value));
        var indexProperty = typeof(IndexValuePair<string>).GetProperty(nameof(IndexValuePair<string>.Index));

        valueProperty!.CanWrite.Should().BeFalse();
        indexProperty!.CanWrite.Should().BeFalse();

        // Values should remain constant
        pair.Value.Should().Be("test");
        pair.Index.Should().Be(1);
    }

    #endregion

    #region Test Helper Classes

    private class TestClass
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }

        public override string ToString() => $"{Name}:{Value}";
    }

    #endregion
}