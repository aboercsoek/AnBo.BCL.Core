//--------------------------------------------------------------------------
// File:    EnumerableExtensionsUnitTest.cs
// Content: Unit tests for EnumerableExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Collections.ObjectModel;

namespace AnBo.Test;

public class EnumerableExtensionsUnitTest
{
    #region ToObservableCollection Tests

    [Fact]
    public void ToObservableCollection_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        var action = () => source!.ToObservableCollection();
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void ToObservableCollection_With_Empty_Source_Should_Return_Empty_Collection()
    {
        // Arrange
        var source = Enumerable.Empty<int>();

        // Act
        var result = source.ToObservableCollection();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObservableCollection<int>>();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToObservableCollection_With_Items_Should_Return_Collection_With_Items()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = source.ToObservableCollection();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObservableCollection<int>>();
        result.Should().HaveCount(5);
        result.Should().ContainInOrder(1, 2, 3, 4, 5);
    }

    [Fact]
    public void ToObservableCollection_With_List_Should_Preserve_Order()
    {
        // Arrange
        var source = new List<string> { "first", "second", "third" };

        // Act
        var result = source.ToObservableCollection();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObservableCollection<string>>();
        result.Should().ContainInOrder("first", "second", "third");
    }

    #endregion

    #region SequenceEqual with Custom Comparer Tests

    [Fact]
    public void SequenceEqual_With_Null_First_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? first = null;
        var second = new[] { 1, 2, 3 };
        var comparer = new Func<int, int, bool>((x, y) => x == y);

        // Act & Assert
        var action = () => first!.SequenceEqual(second, comparer);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("first");
    }

    [Fact]
    public void SequenceEqual_With_Null_Second_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        IEnumerable<int>? second = null;
        var comparer = new Func<int, int, bool>((x, y) => x == y);

        // Act & Assert
        var action = () => first.SequenceEqual(second!, comparer);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("second");
    }

    [Fact]
    public void SequenceEqual_With_Null_Comparer_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 1, 2, 3 };
        Func<int, int, bool>? comparer = null;

        // Act & Assert
        var action = () => first.SequenceEqual(second, comparer!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("compare");
    }

    [Fact]
    public void SequenceEqual_With_Same_Reference_Should_Return_True()
    {
        // Arrange
        var sequence = new[] { 1, 2, 3 };
        var comparer = new Func<int, int, bool>((x, y) => x == y);

        // Act
        var result = sequence.SequenceEqual(sequence, comparer);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SequenceEqual_With_Equal_Sequences_Should_Return_True()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 1, 2, 3 };
        var comparer = new Func<int, int, bool>((x, y) => x == y);

        // Act
        var result = first.SequenceEqual(second, comparer);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SequenceEqual_With_Different_Sequences_Should_Return_False()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 1, 2, 4 };
        var comparer = new Func<int, int, bool>((x, y) => x == y);

        // Act
        var result = first.SequenceEqual(second, comparer);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SequenceEqual_With_Different_Lengths_Should_Return_False()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 1, 2 };
        var comparer = new Func<int, int, bool>((x, y) => x == y);

        // Act
        var result = first.SequenceEqual(second, comparer);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SequenceEqual_With_Custom_Comparer_Should_Use_Comparer()
    {
        // Arrange
        var first = new[] { "Hello", "World" };
        var second = new[] { "HELLO", "WORLD" };
        var caseInsensitiveComparer = new Func<string, string, bool>((x, y) => 
            string.Equals(x, y, StringComparison.OrdinalIgnoreCase));

        // Act
        var result = first.SequenceEqual(second, caseInsensitiveComparer);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SequenceEqual_With_Empty_Sequences_Should_Return_True()
    {
        // Arrange
        var first = Enumerable.Empty<int>();
        var second = Enumerable.Empty<int>();
        var comparer = new Func<int, int, bool>((x, y) => x == y);

        // Act
        var result = first.SequenceEqual(second, comparer);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region CanBeProvenEmptyFast Tests

    [Fact]
    public void CanBeProvenEmptyFast_With_Empty_Array_Should_Return_True()
    {
        // Arrange
        var source = new int[0];

        // Act
        var result = source.CanBeProvenEmptyFast();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanBeProvenEmptyFast_With_Empty_List_Should_Return_True()
    {
        // Arrange
        var source = new List<int>();

        // Act
        var result = source.CanBeProvenEmptyFast();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanBeProvenEmptyFast_With_Non_Empty_Array_Should_Return_False()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var result = source.CanBeProvenEmptyFast();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanBeProvenEmptyFast_With_Non_Empty_List_Should_Return_False()
    {
        // Arrange
        var source = new List<int> { 1, 2, 3 };

        // Act
        var result = source.CanBeProvenEmptyFast();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanBeProvenEmptyFast_With_LINQ_Query_Should_Return_False()
    {
        // Arrange
        var source = new[] { 1, 2, 3 }.Where(x => x > 10); // Empty but not provable fast

        // Act
        var result = source.CanBeProvenEmptyFast();

        // Assert
        result.Should().BeFalse(); // Can't be proven empty fast due to deferred execution
    }

    #endregion

    #region HasElements Tests

    [Fact]
    public void HasElements_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        var action = () => source!.HasElements();
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void HasElements_With_Empty_Array_Should_Return_False()
    {
        // Arrange
        var source = new int[0];

        // Act
        var result = source.HasElements();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasElements_With_Non_Empty_Array_Should_Return_True()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var result = source.HasElements();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasElements_With_Empty_List_Should_Return_False()
    {
        // Arrange
        var source = new List<int>();

        // Act
        var result = source.HasElements();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasElements_With_Non_Empty_List_Should_Return_True()
    {
        // Arrange
        var source = new List<int> { 42 };

        // Act
        var result = source.HasElements();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasElements_With_Single_Element_Should_Return_True()
    {
        // Arrange
        var source = new[] { 42 };

        // Act
        var result = source.HasElements();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetCountEfficiently Tests

    [Fact]
    public void GetCountEfficiently_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        var action = () => source!.GetCountEfficiently();
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void GetCountEfficiently_With_Empty_Array_Should_Return_Zero()
    {
        // Arrange
        var source = new int[0];

        // Act
        var result = source.GetCountEfficiently();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetCountEfficiently_With_Array_Should_Return_Length()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = source.GetCountEfficiently();

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void GetCountEfficiently_With_List_Should_Return_Count()
    {
        // Arrange
        var source = new List<int> { 1, 2, 3 };

        // Act
        var result = source.GetCountEfficiently();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public void GetCountEfficiently_With_LINQ_Query_Should_Enumerate_And_Count()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 }.Where(x => x % 2 == 0);

        // Act
        var result = source.GetCountEfficiently();

        // Assert
        result.Should().Be(2);
    }

    #endregion

    #region SelectManyPairs Tests

    [Fact]
    public void SelectManyPairs_With_Null_Parents_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? parents = null;
        var childSelector = new Func<int, IEnumerable<string>>(x => new[] { x.ToString() });

        // Act & Assert
        var action = () => parents!.SelectManyPairs(childSelector);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("parents");
    }

    [Fact]
    public void SelectManyPairs_With_Null_ChildSelector_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var parents = new[] { 1, 2, 3 };
        Func<int, IEnumerable<string>>? childSelector = null;

        // Act & Assert
        var action = () => parents.SelectManyPairs(childSelector!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("childSelector");
    }

    [Fact]
    public void SelectManyPairs_Should_Create_Parent_Child_Pairs()
    {
        // Arrange
        var parents = new[] { 1, 2 };
        var childSelector = new Func<int, IEnumerable<string>>(x => new[] { $"child{x}a", $"child{x}b" });

        // Act
        var result = parents.SelectManyPairs(childSelector).ToArray();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain((1, "child1a"));
        result.Should().Contain((1, "child1b"));
        result.Should().Contain((2, "child2a"));
        result.Should().Contain((2, "child2b"));
    }

    [Fact]
    public void SelectManyPairs_With_Empty_Parents_Should_Return_Empty()
    {
        // Arrange
        var parents = Enumerable.Empty<int>();
        var childSelector = new Func<int, IEnumerable<string>>(x => new[] { x.ToString() });

        // Act
        var result = parents.SelectManyPairs(childSelector);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SelectManyPairs_With_Parent_Having_No_Children_Should_Not_Include_Parent()
    {
        // Arrange
        var parents = new[] { 1, 2, 3 };
        var childSelector = new Func<int, IEnumerable<string>>(x => x == 2 ? Enumerable.Empty<string>() : new[] { x.ToString() });

        // Act
        var result = parents.SelectManyPairs(childSelector).ToArray();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain((1, "1"));
        result.Should().Contain((3, "3"));
        result.Should().NotContain(pair => pair.Item1 == 2);
    }

    #endregion

    #region Batch Tests

    [Fact]
    public void Batch_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        var action = () => source!.Batch(2).ToArray();
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void Batch_With_BatchSize_Less_Than_One_Should_Throw_ArgumentOutOfRangeException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => source.Batch(0).ToArray();
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("batchSize");
    }

    [Fact]
    public void Batch_With_Empty_Source_Should_Return_Empty()
    {
        // Arrange
        var source = Enumerable.Empty<int>();

        // Act
        var result = source.Batch(2);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Batch_Should_Create_Batches_Of_Specified_Size()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6 };

        // Act
        var result = source.Batch(2).ToArray();

        // Assert
        result.Should().HaveCount(3);
        result[0].Should().Equal(1, 2);
        result[1].Should().Equal(3, 4);
        result[2].Should().Equal(5, 6);
    }

    [Fact]
    public void Batch_With_Remainder_Should_Create_Smaller_Last_Batch()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = source.Batch(2).ToArray();

        // Assert
        result.Should().HaveCount(3);
        result[0].Should().Equal(1, 2);
        result[1].Should().Equal(3, 4);
        result[2].Should().Equal(5);
    }

    [Fact]
    public void Batch_With_BatchSize_Larger_Than_Source_Should_Return_Single_Batch()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var result = source.Batch(10).ToArray();

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Equal(1, 2, 3);
    }

    [Fact]
    public void Batch_With_BatchSize_One_Should_Create_Individual_Batches()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var result = source.Batch(1).ToArray();

        // Assert
        result.Should().HaveCount(3);
        result[0].Should().Equal(1);
        result[1].Should().Equal(2);
        result[2].Should().Equal(3);
    }

    #endregion

    #region IfThen Tests

    [Fact]
    public void IfThen_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;
        var predicate = new Func<int, bool>(x => x > 0);
        var action = new Action<int>(x => { });

        // Act & Assert
        var act = () => source!.IfThen(predicate, action);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void IfThen_With_Null_Predicate_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool>? predicate = null;
        var action = new Action<int>(x => { });

        // Act & Assert
        var act = () => source.IfThen(predicate!, action);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("ifPredicate");
    }

    [Fact]
    public void IfThen_With_Null_Action_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var predicate = new Func<int, bool>(x => x > 0);
        Action<int>? action = null;

        // Act & Assert
        var act = () => source.IfThen(predicate, action!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("thenAction");
    }

    [Fact]
    public void IfThen_Should_Execute_Action_For_Matching_Elements()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var executedValues = new List<int>();
        var predicate = new Func<int, bool>(x => x % 2 == 0);
        var action = new Action<int>(x => executedValues.Add(x));

        // Act
        var result = source.IfThen(predicate, action);

        // Assert
        result.Should().BeTrue();
        executedValues.Should().Equal(2, 4);
    }

    [Fact]
    public void IfThen_Should_Not_Execute_Action_For_Non_Matching_Elements()
    {
        // Arrange
        var source = new[] { 1, 3, 5 };
        var executedValues = new List<int>();
        var predicate = new Func<int, bool>(x => x % 2 == 0);
        var action = new Action<int>(x => executedValues.Add(x));

        // Act
        var result = source.IfThen(predicate, action);

        // Assert
        result.Should().BeTrue();
        executedValues.Should().BeEmpty();
    }

    [Fact]
    public void IfThen_Should_Return_False_When_Exception_Occurs()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var predicate = new Func<int, bool>(x => x > 0);
        var action = new Action<int>(x => throw new InvalidOperationException("Test exception"));

        // Act
        var result = source.IfThen(predicate, action);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IfThen_Should_Continue_Processing_After_Exception()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var executedValues = new List<int>();
        var predicate = new Func<int, bool>(x => x > 0);
        var action = new Action<int>(x =>
        {
            if (x == 2) throw new InvalidOperationException("Test exception");
            executedValues.Add(x);
        });

        // Act
        var result = source.IfThen(predicate, action);

        // Assert
        result.Should().BeFalse();
        executedValues.Should().Equal(1, 3);
    }

    #endregion

    #region IfThenElse Tests

    [Fact]
    public void IfThenElse_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;
        var predicate = new Func<int, bool>(x => x > 0);
        var thenAction = new Action<int>(x => { });
        var elseAction = new Action<int>(x => { });

        // Act & Assert
        var act = () => source!.IfThenElse(predicate, thenAction, elseAction);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void IfThenElse_With_Null_Predicate_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool>? predicate = null;
        var thenAction = new Action<int>(x => { });
        var elseAction = new Action<int>(x => { });

        // Act & Assert
        var act = () => source.IfThenElse(predicate!, thenAction, elseAction);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("ifPredicate");
    }

    [Fact]
    public void IfThenElse_With_Null_ThenAction_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var predicate = new Func<int, bool>(x => x > 0);
        Action<int>? thenAction = null;
        var elseAction = new Action<int>(x => { });

        // Act & Assert
        var act = () => source.IfThenElse(predicate, thenAction!, elseAction);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("thenAction");
    }

    [Fact]
    public void IfThenElse_With_Null_ElseAction_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var predicate = new Func<int, bool>(x => x > 0);
        var thenAction = new Action<int>(x => { });
        Action<int>? elseAction = null;

        // Act & Assert
        var act = () => source.IfThenElse(predicate, thenAction, elseAction!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("elseAction");
    }

    [Fact]
    public void IfThenElse_Should_Execute_Correct_Actions()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var thenExecutedValues = new List<int>();
        var elseExecutedValues = new List<int>();
        var predicate = new Func<int, bool>(x => x % 2 == 0);
        var thenAction = new Action<int>(x => thenExecutedValues.Add(x));
        var elseAction = new Action<int>(x => elseExecutedValues.Add(x));

        // Act
        var result = source.IfThenElse(predicate, thenAction, elseAction);

        // Assert
        result.Should().BeTrue();
        thenExecutedValues.Should().Equal(2, 4);
        elseExecutedValues.Should().Equal(1, 3, 5);
    }

    [Fact]
    public void IfThenElse_Should_Return_False_When_Exception_Occurs_In_ThenAction()
    {
        // Arrange
        var source = new[] { 2, 4 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);
        var thenAction = new Action<int>(x => throw new InvalidOperationException("Test exception"));
        var elseAction = new Action<int>(x => { });

        // Act
        var result = source.IfThenElse(predicate, thenAction, elseAction);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IfThenElse_Should_Return_False_When_Exception_Occurs_In_ElseAction()
    {
        // Arrange
        var source = new[] { 1, 3 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);
        var thenAction = new Action<int>(x => { });
        var elseAction = new Action<int>(x => throw new InvalidOperationException("Test exception"));

        // Act
        var result = source.IfThenElse(predicate, thenAction, elseAction);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region WithIndex Tests

    [Fact]
    public void WithIndex_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<string>? source = null;

        // Act & Assert
        var action = () => source!.WithIndex();
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void WithIndex_With_Empty_Source_Should_Return_Empty()
    {
        // Arrange
        var source = Enumerable.Empty<string>();

        // Act
        var result = source.WithIndex();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void WithIndex_Should_Create_IndexValuePairs()
    {
        // Arrange
        var source = new[] { "apple", "banana", "cherry" };

        // Act
        var result = source.WithIndex().ToArray();

        // Assert
        result.Should().HaveCount(3);
        result[0].Should().Be(IndexValuePair.Create("apple", 0));
        result[1].Should().Be(IndexValuePair.Create("banana", 1));
        result[2].Should().Be(IndexValuePair.Create("cherry", 2));
    }

    [Fact]
    public void WithIndex_Should_Work_With_Single_Element()
    {
        // Arrange
        var source = new[] { 42 };

        // Act
        var result = source.WithIndex().ToArray();

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be(IndexValuePair.Create(42, 0));
    }

    #endregion

    #region Foreach Tests

    [Fact]
    public void Foreach_With_Null_Source_Should_Not_Throw()
    {
        // Arrange
        IEnumerable<int>? source = null;
        var action = new Action<int>(x => { });

        // Act & Assert
        var act = () => source!.Foreach(action);
        act.Should().NotThrow();
    }

    [Fact]
    public void Foreach_With_Null_Action_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Action<int>? action = null;

        // Act & Assert
        var act = () => source.Foreach(action!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("action");
    }

    [Fact]
    public void Foreach_Should_Execute_Action_For_Each_Element()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var executedValues = new List<int>();
        var action = new Action<int>(x => executedValues.Add(x));

        // Act
        source.Foreach(action);

        // Assert
        executedValues.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void Foreach_With_Empty_Source_Should_Not_Execute_Action()
    {
        // Arrange
        var source = Enumerable.Empty<int>();
        var executionCount = 0;
        var action = new Action<int>(x => executionCount++);

        // Act
        source.Foreach(action);

        // Assert
        executionCount.Should().Be(0);
    }

    #endregion

    #region ForEach with Index Tests

    [Fact]
    public void ForEach_WithIndex_With_Null_Source_Should_Not_Throw()
    {
        // Arrange
        IEnumerable<int>? source = null;
        var action = new Action<int, int>((x, i) => { });

        // Act & Assert
        var act = () => source!.ForEach(action);
        act.Should().NotThrow();
    }

    [Fact]
    public void ForEach_WithIndex_With_Null_Action_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Action<int, int>? action = null;

        // Act & Assert
        var act = () => source.ForEach(action!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("action");
    }

    [Fact]
    public void ForEach_WithIndex_Should_Execute_Action_With_Correct_Indices()
    {
        // Arrange
        var source = new[] { "apple", "banana", "cherry" };
        var results = new List<(string value, int index)>();
        var action = new Action<string, int>((value, index) => results.Add((value, index)));

        // Act
        source.ForEach(action);

        // Assert
        results.Should().HaveCount(3);
        results.Should().Equal(
            ("apple", 0),
            ("banana", 1),
            ("cherry", 2)
        );
    }

    [Fact]
    public void ForEach_WithIndex_With_Empty_Source_Should_Not_Execute_Action()
    {
        // Arrange
        var source = Enumerable.Empty<string>();
        var executionCount = 0;
        var action = new Action<string, int>((value, index) => executionCount++);

        // Act
        source.ForEach(action);

        // Assert
        executionCount.Should().Be(0);
    }

    #endregion

    #region DisposeAll Tests

    [Fact]
    public void DisposeAll_With_Null_Source_Should_Not_Throw()
    {
        // Arrange
        IEnumerable<IDisposable>? source = null;

        // Act & Assert
        var action = () => source.DisposeAll();
        action.Should().NotThrow();
    }

    [Fact]
    public void DisposeAll_Should_Dispose_All_Elements()
    {
        // Arrange
        var disposables = new[]
        {
            new TestDisposable(),
            new TestDisposable(),
            new TestDisposable()
        };

        // Act
        disposables.DisposeAll();

        // Assert
        disposables.Should().AllSatisfy(d => d.IsDisposed.Should().BeTrue());
    }

    [Fact]
    public void DisposeAll_With_Null_Elements_Should_Not_Throw()
    {
        // Arrange
        var disposables = new IDisposable[] { new TestDisposable(), null!, new TestDisposable() };

        // Act & Assert
        var action = () => disposables.DisposeAll();
        action.Should().NotThrow();
    }

    [Fact]
    public void DisposeAll_With_Empty_Source_Should_Not_Throw()
    {
        // Arrange
        var source = Enumerable.Empty<IDisposable>();

        // Act & Assert
        var action = () => source.DisposeAll();
        action.Should().NotThrow();
    }

    #endregion

    #region AddRange Tests

    [Fact]
    public void AddRangeSafe_With_Null_Collection_Should_Throw_ArgumentNullException()
    {
        // Arrange
        ICollection<int>? collection = null;
        var elements = new[] { 1, 2, 3 };

        // Act & Assert
        var action = () => collection!.AddRangeSafe(elements);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("collection");
    }

    [Fact]
    public void AddRangeSafe_With_Null_Elements_Should_Not_Throw()
    {
        // Arrange
        var collection = new List<int>();
        IEnumerable<int>? elements = null;

        // Act & Assert
        var action = () => collection.AddRangeSafe(elements!);
        action.Should().NotThrow();
        collection.Should().BeEmpty();
    }

    [Fact]
    public void AddRangeSafe_With_List_Should_Use_List_AddRange()
    {
        // Arrange
        var collection = new List<int> { 1, 2 };
        var elements = new[] { 3, 4, 5 };

        // Act
        collection.AddRangeSafe(elements);

        // Assert
        collection.Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void AddRangeSafe_With_Generic_Collection_Should_Add_Elements()
    {
        // Arrange
        var collection = new HashSet<int> { 1, 2 };
        var elements = new[] { 2, 3, 4 }; // Note: 2 is duplicate for HashSet

        // Act
        collection.AddRangeSafe(elements);

        // Assert
        collection.Should().Contain([1, 2, 3, 4]);
        collection.Should().HaveCount(4); // HashSet ignores duplicates
    }

    [Fact]
    public void AddRangeSafe_With_Empty_Elements_Should_Not_Change_Collection()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };
        var elements = Enumerable.Empty<int>();

        // Act
        collection.AddRangeSafe(elements);

        // Assert
        collection.Should().Equal(1, 2, 3);
    }

    #endregion

    #region IndexOf Tests

    [Fact]
    public void IndexOf_With_Null_Source_Should_Return_Minus_One()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act
        var result = source.IndexOf(42);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void IndexOf_Should_Return_Correct_Index()
    {
        // Arrange
        var source = new[] { 10, 20, 30, 20, 40 };

        // Act
        var result = source.IndexOf(20);

        // Assert
        result.Should().Be(1); // First occurrence
    }

    [Fact]
    public void IndexOf_With_Item_Not_Found_Should_Return_Minus_One()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var result = source.IndexOf(42);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void IndexOf_With_Custom_Comparer_Should_Use_Comparer()
    {
        // Arrange
        var source = new[] { "Hello", "WORLD", "Test" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act
        var result = source.IndexOf("world", comparer);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void IndexOf_With_Empty_Source_Should_Return_Minus_One()
    {
        // Arrange
        var source = Enumerable.Empty<int>();

        // Act
        var result = source.IndexOf(42);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void IndexOf_With_Null_Item_Should_Find_Null()
    {
        // Arrange
        var source = new string?[] { "hello", null, "world" };

        // Act
        var result = source.IndexOf(null);

        // Assert
        result.Should().Be(1);
    }

    #endregion

    #region FindIndex Tests

    [Fact]
    public void FindIndex_With_Null_Source_Should_Return_Minus_One()
    {
        // Arrange
        IEnumerable<int>? source = null;
        var predicate = new Func<int, bool>(x => x > 0);

        // Act
        var result = source.FindIndex(predicate);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void FindIndex_With_Null_Predicate_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool>? predicate = null;

        // Act & Assert
        var action = () => source.FindIndex(predicate!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("predicate");
    }

    [Fact]
    public void FindIndex_Should_Return_Index_Of_First_Match()
    {
        // Arrange
        var source = new[] { 1, 3, 2, 4, 6 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = source.FindIndex(predicate);

        // Assert
        result.Should().Be(2); // Index of first even number (2)
    }

    [Fact]
    public void FindIndex_With_No_Match_Should_Return_Minus_One()
    {
        // Arrange
        var source = new[] { 1, 3, 5 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = source.FindIndex(predicate);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void FindIndex_With_Empty_Source_Should_Return_Minus_One()
    {
        // Arrange
        var source = Enumerable.Empty<int>();
        var predicate = new Func<int, bool>(x => x > 0);

        // Act
        var result = source.FindIndex(predicate);

        // Assert
        result.Should().Be(-1);
    }

    #endregion

    #region FindLastIndex with IEquatable Tests

    [Fact]
    public void FindLastIndex_IEquatable_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        var action = () => source!.FindLastIndex(42);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void FindLastIndex_IEquatable_Should_Return_Last_Index()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 2, 4 };

        // Act
        var result = source.FindLastIndex(2);

        // Assert
        result.Should().Be(3); // Last occurrence of 2
    }

    [Fact]
    public void FindLastIndex_IEquatable_With_No_Match_Should_Return_Minus_One()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var result = source.FindLastIndex(42);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void FindLastIndex_IEquatable_With_Empty_Source_Should_Return_Minus_One()
    {
        // Arrange
        var source = Enumerable.Empty<int>();

        // Act
        var result = source.FindLastIndex(42);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void FindLastIndex_IEquatable_With_Single_Match_Should_Return_Index()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };

        // Act
        var result = source.FindLastIndex(2);

        // Assert
        result.Should().Be(1);
    }

    #endregion

    #region FindLastIndex with Predicate Tests

    [Fact]
    public void FindLastIndex_Predicate_With_Null_Source_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;
        var predicate = new Func<int, bool>(x => x > 0);

        // Act & Assert
        var action = () => source!.FindLastIndex(predicate);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void FindLastIndex_Predicate_With_Null_Predicate_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool>? predicate = null;

        // Act & Assert
        var action = () => source.FindLastIndex(predicate!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("predicate");
    }

    [Fact]
    public void FindLastIndex_Predicate_Should_Return_Last_Matching_Index()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = source.FindLastIndex(predicate);

        // Assert
        result.Should().Be(5); // Index of last even number (6)
    }

    [Fact]
    public void FindLastIndex_Predicate_With_No_Match_Should_Return_Minus_One()
    {
        // Arrange
        var source = new[] { 1, 3, 5 };
        var predicate = new Func<int, bool>(x => x % 2 == 0);

        // Act
        var result = source.FindLastIndex(predicate);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void FindLastIndex_Predicate_With_Empty_Source_Should_Return_Minus_One()
    {
        // Arrange
        var source = Enumerable.Empty<int>();
        var predicate = new Func<int, bool>(x => x > 0);

        // Act
        var result = source.FindLastIndex(predicate);

        // Assert
        result.Should().Be(-1);
    }

    #endregion

    #region ToArraySafe Tests

    [Fact]
    public void ToArraySafe_With_Null_Source_Should_Return_Empty_Array()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act
        var result = source.ToArraySafe();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToArraySafe_With_Empty_Source_Should_Return_Empty_Array()
    {
        // Arrange
        var source = Enumerable.Empty<int>();

        // Act
        var result = source.ToArraySafe();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToArraySafe_With_Elements_Should_Return_Array_With_Elements()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = source.ToArraySafe();

        // Assert
        result.Should().NotBeNull();
        result.Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void ToArraySafe_With_List_Should_Return_Array_With_Same_Elements()
    {
        // Arrange
        var source = new List<string> { "apple", "banana", "cherry" };

        // Act
        var result = source.ToArraySafe();

        // Assert
        result.Should().NotBeNull();
        result.Should().Equal("apple", "banana", "cherry");
    }

    #endregion

    #region Helper Classes

    private class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public string Value { get; set; } = string.Empty;

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    #endregion
}