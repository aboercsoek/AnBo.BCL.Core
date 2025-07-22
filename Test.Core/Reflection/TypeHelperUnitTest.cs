using AnBo.Core;
using FluentAssertions;
using System.Collections;
using System.Text;
using System.Text.Json.Serialization;
using Xunit.Sdk;

namespace AnBo.Test;

public class TypeHelperUnitTest
{
    #region DeepClone Tests

    [Fact]
    public void DeepClone_Object_With_Null_Should_Return_Null()
    {
        // Arrange
        object? original = null;

        // Act
        var result = TypeHelper.DeepClone(original);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void DeepClone_Object_With_Null_And_Type_Should_Return_Null()
    {
        // Arrange
        string? original = null;

        // Act
        var result = TypeHelper.DeepClone(original, typeof(string));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void DeepClone_Object_With_CloneableValueAndTypeIsNull_ShouldReturnDeepClone()
    {
        // Arrange
        string original = "test";

        // Act
        var result = TypeHelper.DeepClone(original, null!);

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void DeepClone_Object_With_CloneableValueAndNotAssingableType_ShouldThroe()
    {
        // Arrange
        string original = "test";

        // Act
        var action = () => TypeHelper.DeepClone(original, typeof(int));

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*is not compatible with the object type*");
    }

    [Fact]
    public void DeepClone_Generic_With_Null_Should_Return_Default()
    {
        // Arrange
        string? original = null;

        // Act
        var result = TypeHelper.DeepClone(original);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void DeepClone_Object_With_Simple_Value_Should_Create_Copy()
    {
        // Arrange
        object original = "test string";

        // Act
        var result = TypeHelper.DeepClone(original, original.GetType());

        // Assert
        result.Should().Be("test string");
        result.Should().NotBeSameAs(original);
    }

    [Fact]
    public void DeepClone_Generic_With_Simple_Value_Should_Create_Copy()
    {
        // Arrange
        string original = "test string";

        // Act
        var result = TypeHelper.DeepClone(original);

        // Assert
        result.Should().Be("test string");
        result.Should().NotBeSameAs(original);
    }

    [Fact]
    public void DeepClone_Object_With_Complex_Object_Should_Create_Deep_Copy()
    {
        // Arrange
        var original = new TestClass
        {
            Id = 1,
            Name = "Test",
            NestedObject = new NestedTestClass { Value = "Nested" }
        };

        // Act
        var result = TypeHelper.DeepClone(original, typeof(TestClass)) as TestClass;

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(original);
        result.Id.Should().Be(original.Id);
        result.Name.Should().Be(original.Name);
        result.NestedObject.Should().NotBeSameAs(original.NestedObject);
        result.NestedObject.Value.Should().Be(original.NestedObject.Value);
    }

    [Fact]
    public void DeepClone_Generic_With_Complex_Object_Should_Create_Deep_Copy()
    {
        // Arrange
        var original = new TestClass
        {
            Id = 1,
            Name = "Test",
            NestedObject = new NestedTestClass { Value = "Nested" }
        };

        // Act
        var result = TypeHelper.DeepClone(original);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(original);
        result.Id.Should().Be(original.Id);
        result.Name.Should().Be(original.Name);
        result.NestedObject.Should().NotBeSameAs(original.NestedObject);
        result.NestedObject.Value.Should().Be(original.NestedObject.Value);
    }

    [Fact]
    public void DeepClone_With_Value_Types_Should_Work()
    {
        // Arrange
        int original = 42;

        // Act
        var result = TypeHelper.DeepClone(original);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void DeepClone_With_Collections_Should_Create_Deep_Copy()
    {
        // Arrange
        var original = new List<TestClass>
        {
            new TestClass { Id = 1, Name = "First" },
            new TestClass { Id = 2, Name = "Second" }
        };

        // Act
        var result = TypeHelper.DeepClone(original);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(original);
        result.Count.Should().Be(2);
        result[0].Should().NotBeSameAs(original[0]);
        result[0].Id.Should().Be(original[0].Id);
        result[0].Name.Should().Be(original[0].Name);
    }

    [Fact]
    public void DeepClone_With_Array_Should_Create_Deep_Copy()
    {
        // Arrange
        TestClass[] original = [
            new TestClass { Id = 1, Name = "First" },
            new TestClass { Id = 2, Name = "Second" }
        ];

        // Act
        var result = TypeHelper.DeepClone(original);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(original);
        result.Length.Should().Be(2);
        result[0].Should().NotBeSameAs(original[0]);
        result[0].Id.Should().Be(original[0].Id);
        result[0].Name.Should().Be(original[0].Name);
    }

    [Fact]
    public void DeepClone_Object_With_Non_Serializable_Type_Should_Throw_InvalidOperationException()
    {
        // Arrange
        var original = new NonSerializableClass();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => TypeHelper.DeepClone(original, typeof(NonSerializableClass)));
        exception.Message.Should().Contain("is not suitable for JSON-based deep cloning");
    }

    [Fact]
    public void DeepClone_Generic_With_Non_Serializable_Type_Should_Throw_InvalidOperationException()
    {
        // Arrange
        var original = new NonSerializableClass();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => TypeHelper.DeepClone(original));
        exception.Message.Should().Contain("is not suitable for JSON-based deep cloning");
        Console.Out.WriteLine(exception.Message);
    }

    #endregion

    #region TryDeepClone Tests

    [Fact]
    public void TryDeepClone_Generic_With_Complex_Object_Should_Create_Deep_Copy()
    {
        // Arrange
        var original = new TestClass
        {
            Id = 1,
            Name = "Test",
            NestedObject = new NestedTestClass { Value = "Nested" }
        };

        // Act
        var success = TypeHelper.TryDeepClone(original, out var result);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(original);
        result.Id.Should().Be(original.Id);
        result.Name.Should().Be(original.Name);
        result.NestedObject.Should().NotBeSameAs(original.NestedObject);
        result.NestedObject.Value.Should().Be(original.NestedObject.Value);
        success.Should().BeTrue();
    }

    [Fact]
    public void TryDeepClone_With_Value_Types_Should_Work()
    {
        // Arrange
        int original = 42;

        // Act
        var success = TypeHelper.TryDeepClone(original, out var result);

        // Assert
        result.Should().Be(42);
        success.Should().BeTrue();
    }

    [Fact]
    public void TryDeepClone_With_Collections_Should_Create_Deep_Copy()
    {
        // Arrange
        var original = new List<TestClass>
        {
            new TestClass { Id = 1, Name = "First" },
            new TestClass { Id = 2, Name = "Second" }
        };

        // Act
        var success = TypeHelper.TryDeepClone(original, out var result);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(original);
        result.Count.Should().Be(2);
        result[0].Should().NotBeSameAs(original[0]);
        result[0].Id.Should().Be(original[0].Id);
        result[0].Name.Should().Be(original[0].Name);
        success.Should().BeTrue();
    }

    [Fact]
    public void TryDeepClone_Generic_With_Non_Serializable_Type_Should_Throw_InvalidOperationException()
    {
        // Arrange
        var original = new NonSerializableClass();

        // Act & Assert
        var success = TypeHelper.TryDeepClone(original, out var result);

        // Assert
        result.Should().BeNull();
        success.Should().BeFalse();
    }

    #endregion

    #region SafeDispose Tests

    [Fact]
    public void SafeDispose_With_Null_Should_Not_Throw()
    {
        // Arrange
        object? obj = null;

        // Act & Assert
        var action = () => TypeHelper.SafeDispose(obj);
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDispose_WithDisposableObject_ShouldDisposeObject()
    {
        // Arrange
        var disposable = new DisposableTestClass();

        // Act
        TypeHelper.SafeDispose(disposable);

        // Assert
        disposable.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void SafeDispose_WithAsyncDisposableObject_ShouldDisposeObject()
    {
        // Arrange
        var asyncDisposable = new AsyncDisposableTestClass();

        // Act
        TypeHelper.SafeDispose(asyncDisposable);

        // Assert
        asyncDisposable.IsDisposed.Should().BeTrue();
        asyncDisposable.DisposalWasAsync.Should().BeTrue();
    }

    [Fact]
    public void SafeDispose_WithDualDisposableObject_ShouldPreferAsyncDisposal()
    {
        // Arrange
        var dualDisposable = new DualDisposableTestClass();

        // Act
        TypeHelper.SafeDispose(dualDisposable);

        // Assert
        dualDisposable.IsDisposed.Should().BeTrue();
        dualDisposable.DisposalWasAsync.Should().BeTrue("async disposal should be preferred");
    }

    [Fact]
    public void SafeDispose_WithNonDisposableObject_ShouldNotThrow()
    {
        // Arrange
        var nonDisposable = new NonDisposableTestClass();

        // Act
        var action = () => TypeHelper.SafeDispose(nonDisposable);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDispose_With_String_Should_Not_Throw()
    {
        // Arrange
        string obj = "test string";

        // Act & Assert
        var action = () => TypeHelper.SafeDispose(obj);
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDispose_With_Already_Disposed_Object_Should_Not_Throw()
    {
        // Arrange
        var disposable = new DisposableTestClass();
        disposable.Dispose(); // Dispose first time

        // Act & Assert
        var action = () => TypeHelper.SafeDispose(disposable);
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDispose_WithFaultyDisposable_ShouldNotThrow()
    {
        // Arrange
        var faultyDisposable = new FaultyDisposableTestClass();

        // Act
        var action = () => TypeHelper.SafeDispose(faultyDisposable);

        // Assert
        action.Should().NotThrow("SafeDispose should handle exceptions gracefully");
        faultyDisposable.IsDisposed.Should().BeFalse("disposal should have failed");
    }

    #endregion

    #region SafeDisposeAsync Tests

    [Fact]
    public async Task SafeDisposeAsync_WithNull_ShouldNotThrow()
    {
        // Arrange & Act
        var action = async () => await TypeHelper.SafeDisposeAsync(null);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SafeDisposeAsync_WithDisposableObject_ShouldDisposeObject()
    {
        // Arrange
        var disposable = new DisposableTestClass();

        // Act
        await TypeHelper.SafeDisposeAsync(disposable);

        // Assert
        disposable.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public async Task SafeDisposeAsync_WithAsyncDisposableObject_ShouldDisposeObject()
    {
        // Arrange
        var asyncDisposable = new AsyncDisposableTestClass();

        // Act
        await TypeHelper.SafeDisposeAsync(asyncDisposable);

        // Assert
        asyncDisposable.IsDisposed.Should().BeTrue();
        asyncDisposable.DisposalWasAsync.Should().BeTrue();
    }

    [Fact]
    public async Task SafeDisposeAsync_WithDualDisposableObject_ShouldPreferAsyncDisposal()
    {
        // Arrange
        var dualDisposable = new DualDisposableTestClass();

        // Act
        await TypeHelper.SafeDisposeAsync(dualDisposable);

        // Assert
        dualDisposable.IsDisposed.Should().BeTrue();
        dualDisposable.DisposalWasAsync.Should().BeTrue("async disposal should be preferred");
    }

    [Fact]
    public async Task SafeDisposeAsync_WithNonDisposableObject_ShouldNotThrow()
    {
        // Arrange
        var nonDisposable = new NonDisposableTestClass();

        // Act
        var action = async () => await TypeHelper.SafeDisposeAsync(nonDisposable);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SafeDisposeAsync_WithFaultyAsyncDisposable_ShouldNotThrow()
    {
        // Arrange
        var faultyAsyncDisposable = new FaultyAsyncDisposableTestClass();

        // Act
        var action = async () => await TypeHelper.SafeDisposeAsync(faultyAsyncDisposable);

        // Assert
        await action.Should().NotThrowAsync("SafeDisposeAsync should handle exceptions gracefully");
        faultyAsyncDisposable.IsDisposed.Should().BeFalse("disposal should have failed");
    }

    #endregion

    #region SafeDisposeAll Tests

    [Fact]
    public void SafeDisposeAll_With_Null_Should_Not_Throw()
    {
        // Arrange
        IEnumerable? enumerable = null;

        // Act
        var action = () => TypeHelper.SafeDisposeAll(enumerable);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDisposeAll_With_Empty_Collection_Should_Not_Throw()
    {
        // Arrange
        var emptySequence = new List<object>();

        // Act
        var action = () => TypeHelper.SafeDisposeAll(emptySequence);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDisposeAll_With_IDisposable_Elements_Should_Dispose_All()
    {
        // Arrange
        var disposables = new List<DisposableTestClass>
        {
            new DisposableTestClass(),
            new DisposableTestClass(),
            new DisposableTestClass()
        };

        // Act
        TypeHelper.SafeDisposeAll(disposables);

        // Assert
        disposables.Should().AllSatisfy(d => d.IsDisposed.Should().BeTrue());
    }

    [Fact]
    public void SafeDisposeAll_With_Mixed_Elements_Should_Dispose_Only_IDisposable()
    {
        // Arrange
        var disposable1 = new DisposableTestClass();
        var disposable2 = new DisposableTestClass();
        var nonDisposable = new NonDisposableTestClass();

        var sequence = new object[] { disposable1, nonDisposable, disposable2, "string", 42 };

        // Act
        TypeHelper.SafeDisposeAll(sequence);

        // Assert
        disposable1.IsDisposed.Should().BeTrue();
        disposable2.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void SafeDisposeAll_WithFaultyDisposables_ShouldContinueWithOthers()
    {
        // Arrange
        var goodDisposable = new DisposableTestClass();
        var faultyDisposable = new FaultyDisposableTestClass();
        var anotherGoodDisposable = new DisposableTestClass();
        var sequence = new object[] { goodDisposable, faultyDisposable, anotherGoodDisposable };

        // Act
        var action = () => TypeHelper.SafeDisposeAll(sequence);

        // Assert
        action.Should().NotThrow("should handle faulty disposables gracefully");
        goodDisposable.IsDisposed.Should().BeTrue();
        anotherGoodDisposable.IsDisposed.Should().BeTrue();
        faultyDisposable.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void SafeDisposeAll_WithNullElementsInSequence_ShouldSkipNulls()
    {
        // Arrange
        var disposable = new DisposableTestClass();
        var sequence = new object?[] { disposable, null, new NonDisposableTestClass(), null };

        // Act
        var action = () => TypeHelper.SafeDisposeAll(sequence);

        // Assert
        action.Should().NotThrow();
        disposable.IsDisposed.Should().BeTrue();
    }

    #endregion

    #region SafeDisposeAllAsync Tests

    [Fact]
    public async Task SafeDisposeAllAsync_WithNull_ShouldNotThrow()
    {
        // Arrange & Act
        var action = async () => await TypeHelper.SafeDisposeAllAsync(null);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SafeDisposeAllAsync_WithEmptySequence_ShouldNotThrow()
    {
        // Arrange
        var emptySequence = new List<object>();

        // Act
        var action = async () => await TypeHelper.SafeDisposeAllAsync(emptySequence);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SafeDisposeAllAsync_WithMixedObjects_ShouldDisposeOnlyDisposableOnes()
    {
        // Arrange
        var disposable = new DisposableTestClass();
        var asyncDisposable = new AsyncDisposableTestClass();
        var nonDisposable = new NonDisposableTestClass();
        var sequence = new object[] { disposable, nonDisposable, asyncDisposable };

        // Act
        await TypeHelper.SafeDisposeAllAsync(sequence);

        // Assert
        disposable.IsDisposed.Should().BeTrue();
        asyncDisposable.IsDisposed.Should().BeTrue();
        asyncDisposable.DisposalWasAsync.Should().BeTrue();
    }

    [Fact]
    public async Task SafeDisposeAllAsync_WithFaultyDisposables_ShouldContinueWithOthers()
    {
        // Arrange
        var goodDisposable = new DisposableTestClass();
        var faultyAsyncDisposable = new FaultyAsyncDisposableTestClass();
        var anotherGoodDisposable = new AsyncDisposableTestClass();
        var sequence = new object[] { goodDisposable, faultyAsyncDisposable, anotherGoodDisposable };

        // Act
        var action = async () => await TypeHelper.SafeDisposeAllAsync(sequence);

        // Assert
        await action.Should().NotThrowAsync("should handle faulty disposables gracefully");
        goodDisposable.IsDisposed.Should().BeTrue();
        anotherGoodDisposable.IsDisposed.Should().BeTrue();
        faultyAsyncDisposable.IsDisposed.Should().BeFalse();
    }

    #endregion

    #region SafeDisposeAllDictionaryValues Tests

    [Fact]
    public void SafeDisposeAllDictionaryValues_WithNull_ShouldNotThrow()
    {
        // Arrange
        IDictionary? dict = null;

        // Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryValues(dict);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDisposeAllDictionaryValues_WithEmptyDictionary_ShouldNotThrow()
    {
        // Arrange
        var emptyDictionary = new Dictionary<string, object>();

        // Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryValues(emptyDictionary);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDisposeAllDictionaryValues_WithIDisposableValues_ShouldDisposeAll()
    {
        // Arrange
        var disposable1 = new DisposableTestClass();
        var disposable2 = new DisposableTestClass();
        var dict = new Hashtable
        {
            { "key1", disposable1 },
            { "key2", disposable2 }
        };

        // Act
        TypeHelper.SafeDisposeAllDictionaryValues(dict);

        // Assert
        disposable1.IsDisposed.Should().BeTrue();
        disposable2.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void SafeDisposeAllDictionaryValues_WithMixedValues_ShouldDisposeOnlyDisposableValues()
    {
        // Arrange
        var disposable1 = new DisposableTestClass();
        var disposable2 = new DisposableTestClass();
        var nonDisposable = new NonDisposableTestClass();
        var dictionary = new Dictionary<string, object>
    {
        { "disposable1", disposable1 },
        { "nonDisposable", nonDisposable },
        { "disposable2", disposable2 },
        { "string", "test" },
        { "number", 42 }
    };

        // Act
        TypeHelper.SafeDisposeAllDictionaryValues(dictionary);

        // Assert
        disposable1.IsDisposed.Should().BeTrue();
        disposable2.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void SafeDisposeAllDictionaryValues_WithNullValues_ShouldSkipNulls()
    {
        // Arrange
        var disposable = new DisposableTestClass();
        var dictionary = new Dictionary<string, object?>
    {
        { "disposable", disposable },
        { "null1", null },
        { "null2", null }
    };

        // Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryValues(dictionary);

        // Assert
        action.Should().NotThrow();
        disposable.IsDisposed.Should().BeTrue();
    }


    [Fact]
    public void SafeDisposeAllDictionaryValues_WithFaultyDisposables_ShouldContinueWithOthers()
    {
        // Arrange
        var goodDisposable = new DisposableTestClass();
        var faultyDisposable = new FaultyDisposableTestClass();
        var anotherGoodDisposable = new DisposableTestClass();
        var dictionary = new Dictionary<string, object>
    {
        { "good1", goodDisposable },
        { "faulty", faultyDisposable },
        { "good2", anotherGoodDisposable }
    };

        // Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryValues(dictionary);

        // Assert
        action.Should().NotThrow("should handle faulty disposables gracefully");
        goodDisposable.IsDisposed.Should().BeTrue();
        anotherGoodDisposable.IsDisposed.Should().BeTrue();
        faultyDisposable.IsDisposed.Should().BeFalse();
    }

    #endregion

    #region SafeDisposeAllDictionaryValuesAsync Tests

    [Fact]
    public async Task SafeDisposeAllDictionaryValuesAsync_WithNull_ShouldNotThrow()
    {
        // Arrange & Act
        var action = async () => await TypeHelper.SafeDisposeAllDictionaryValuesAsync(null);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SafeDisposeAllDictionaryValuesAsync_WithEmptyDictionary_ShouldNotThrow()
    {
        // Arrange
        var emptyDictionary = new Dictionary<string, object>();

        // Act
        var action = async () => await TypeHelper.SafeDisposeAllDictionaryValuesAsync(emptyDictionary);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SafeDisposeAllDictionaryValuesAsync_WithMixedValues_ShouldDisposeOnlyDisposableValues()
    {
        // Arrange
        var disposable = new DisposableTestClass();
        var asyncDisposable = new AsyncDisposableTestClass();
        var nonDisposable = new NonDisposableTestClass();
        var dictionary = new Dictionary<string, object>
    {
        { "disposable", disposable },
        { "asyncDisposable", asyncDisposable },
        { "nonDisposable", nonDisposable }
    };

        // Act
        await TypeHelper.SafeDisposeAllDictionaryValuesAsync(dictionary);

        // Assert
        disposable.IsDisposed.Should().BeTrue();
        asyncDisposable.IsDisposed.Should().BeTrue();
        asyncDisposable.DisposalWasAsync.Should().BeTrue();
    }

    [Fact]
    public async Task SafeDisposeAllDictionaryValuesAsync_WithFaultyDisposables_ShouldContinueWithOthers()
    {
        // Arrange
        var goodDisposable = new DisposableTestClass();
        var faultyAsyncDisposable = new FaultyAsyncDisposableTestClass();
        var anotherGoodDisposable = new AsyncDisposableTestClass();
        var dictionary = new Dictionary<string, object>
    {
        { "good1", goodDisposable },
        { "faulty", faultyAsyncDisposable },
        { "good2", anotherGoodDisposable }
    };

        // Act
        var action = async () => await TypeHelper.SafeDisposeAllDictionaryValuesAsync(dictionary);

        // Assert
        await action.Should().NotThrowAsync("should handle faulty disposables gracefully");
        goodDisposable.IsDisposed.Should().BeTrue();
        anotherGoodDisposable.IsDisposed.Should().BeTrue();
        faultyAsyncDisposable.IsDisposed.Should().BeFalse();
    }

    #endregion

    #region SafeDisposeAllDictionaryKeysAndValues Tests

    [Fact]
    public void SafeDisposeAllDictionaryKeysAndValues_WithNull_ShouldNotThrow()
    {
        // Arrange & Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryKeysAndValues(null);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDisposeAllDictionaryKeysAndValues_WithEmptyDictionary_ShouldNotThrow()
    {
        // Arrange
        var emptyDictionary = new Dictionary<string, object>();

        // Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryKeysAndValues(emptyDictionary);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void SafeDisposeAllDictionaryKeysAndValues_WithDisposableKeysAndValues_ShouldDisposeBoth()
    {
        // Arrange
        var disposableKey1 = new DisposableTestClass();
        var disposableKey2 = new DisposableTestClass();
        var disposableValue1 = new DisposableTestClass();
        var disposableValue2 = new DisposableTestClass();
        var nonDisposableKey = "stringKey";
        var nonDisposableValue = new NonDisposableTestClass();

        var dictionary = new Dictionary<object, object>
    {
        { disposableKey1, disposableValue1 },
        { nonDisposableKey, disposableValue2 },
        { disposableKey2, nonDisposableValue }
    };

        // Act
        TypeHelper.SafeDisposeAllDictionaryKeysAndValues(dictionary);

        // Assert
        disposableKey1.IsDisposed.Should().BeTrue("disposable key should be disposed");
        disposableKey2.IsDisposed.Should().BeTrue("disposable key should be disposed");
        disposableValue1.IsDisposed.Should().BeTrue("disposable value should be disposed");
        disposableValue2.IsDisposed.Should().BeTrue("disposable value should be disposed");
    }

    [Fact]
    public void SafeDisposeAllDictionaryKeysAndValues_WithNullKeysAndValues_ShouldSkipNulls()
    {
        // Arrange
        var disposableKey1 = new DisposableTestClass();
        var disposableKey2 = new DisposableTestClass();
        var disposableValue = new DisposableTestClass();
        var nonDisposableKey1 = "stringKey1";
        var nonDisposableKey2 = "stringKey2";
        var dictionary = new Dictionary<object, object?>
        {
            { disposableKey1, disposableValue },
            { nonDisposableKey1, disposableValue },
            { disposableKey2, null },
            { nonDisposableKey2, null }
        };

        // Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryKeysAndValues(dictionary);

        // Assert
        action.Should().NotThrow();
        disposableKey1.IsDisposed.Should().BeTrue();
        disposableKey2.IsDisposed.Should().BeTrue();
        disposableValue.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void SafeDisposeAllDictionaryKeysAndValues_WithFaultyDisposables_ShouldContinueWithOthers()
    {
        // Arrange
        var goodDisposableKey = new DisposableTestClass();
        var faultyDisposableKey = new FaultyDisposableTestClass();
        var goodDisposableValue = new DisposableTestClass();
        var faultyDisposableValue = new FaultyDisposableTestClass();

        var dictionary = new Dictionary<object, object>
    {
        { goodDisposableKey, goodDisposableValue },
        { faultyDisposableKey, faultyDisposableValue }
    };

        // Act
        var action = () => TypeHelper.SafeDisposeAllDictionaryKeysAndValues(dictionary);

        // Assert
        action.Should().NotThrow("should handle faulty disposables gracefully");
        goodDisposableKey.IsDisposed.Should().BeTrue();
        goodDisposableValue.IsDisposed.Should().BeTrue();
        faultyDisposableKey.IsDisposed.Should().BeFalse();
        faultyDisposableValue.IsDisposed.Should().BeFalse();
    }

    #endregion

    #region Integration Tests for SafeDisposeAll and SafeDisposeAllAsync with Large Collections

    [Fact]
    public void SafeDisposeAll_WithLargeCollection_ShouldHandleEfficiently()
    {
        // Arrange
        const int itemCount = 1000;
        var disposables = Enumerable.Range(0, itemCount)
            .Select(_ => new DisposableTestClass())
            .Cast<object>()
            .ToList();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        TypeHelper.SafeDisposeAll(disposables);
        stopwatch.Stop();

        // Assert
        disposables.Cast<DisposableTestClass>()
            .All(d => d.IsDisposed)
            .Should().BeTrue("all disposables should be disposed");

        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000,
            "disposal of 1000 objects should complete within reasonable time");
    }

    [Fact]
    public async Task SafeDisposeAllAsync_WithLargeCollection_ShouldHandleEfficiently()
    {
        // Arrange
        const int itemCount = 1000;
        var disposables = Enumerable.Range(0, itemCount)
            .Select(_ => new AsyncDisposableTestClass())
            .Cast<object>()
            .ToList();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await TypeHelper.SafeDisposeAllAsync(disposables);
        stopwatch.Stop();

        // Assert
        disposables.Cast<AsyncDisposableTestClass>()
            .All(d => d.IsDisposed)
            .Should().BeTrue("all async disposables should be disposed");

        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000,
            "async disposal of 1000 objects should complete within reasonable time");
    }

    #endregion

    #region IsCloneable(Type) Tests

    [Fact]
    public void IsCloneable_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var action = () => TypeHelper.IsCloneable((Type)null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("type");
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(double))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(byte))]
    [InlineData(typeof(long))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(TimeSpan))]
    [InlineData(typeof(Guid))]
    public void IsCloneable_WithValueTypes_ShouldReturnTrue(Type valueType)
    {
        // Act
        var result = TypeHelper.IsCloneable(valueType);

        // Assert
        result.Should().BeTrue($"value type '{valueType.Name}' should be cloneable");
    }

    [Theory]
    [InlineData(typeof(int?))]
    [InlineData(typeof(double?))]
    [InlineData(typeof(bool?))]
    [InlineData(typeof(DateTime?))]
    [InlineData(typeof(Guid?))]
    public void IsCloneable_WithNullableValueTypes_ShouldReturnTrue(Type nullableType)
    {
        // Act
        var result = TypeHelper.IsCloneable(nullableType);

        // Assert
        result.Should().BeTrue($"nullable value type '{nullableType.Name}' should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithCustomStruct_ShouldReturnTrue1()
    {
        // Act
        var result = TypeHelper.IsCloneable(typeof(CloneableStruct));

        // Assert
        result.Should().BeTrue("custom struct should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithString_ShouldReturnTrue()
    {
        // Act
        var result = TypeHelper.IsCloneable(typeof(string));

        // Assert
        result.Should().BeTrue("string should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithSimpleClass_ShouldReturnTrue()
    {
        // Act
        var result = TypeHelper.IsCloneable(typeof(CloneableTestClass));

        // Assert
        result.Should().BeTrue("simple class with basic properties should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithDisposableClass_ShouldReturnTrue()
    {
        // Act
        var result = TypeHelper.IsCloneable(typeof(DisposableTestClass));

        // Assert
        result.Should().BeTrue("disposable class should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithAsyncDisposableClass_ShouldReturnTrue()
    {
        // Act
        var result = TypeHelper.IsCloneable(typeof(AsyncDisposableTestClass));

        // Assert
        result.Should().BeTrue("async disposable class should be cloneable");
    }

    [Theory]
    [InlineData(typeof(Action))]
    [InlineData(typeof(Func<int>))]
    [InlineData(typeof(Func<int, string>))]
    [InlineData(typeof(EventHandler))]
    [InlineData(typeof(EventHandler<string>))]
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(MulticastDelegate))]
    public void IsCloneable_WithDelegateTypes_ShouldReturnFalse(Type delegateType)
    {
        // Act
        var result = TypeHelper.IsCloneable(delegateType);

        // Assert
        result.Should().BeFalse($"delegate type '{delegateType.Name}' should not be cloneable");
    }

    [Theory]
    [InlineData(typeof(IntPtr))]
    [InlineData(typeof(UIntPtr))]
    public void IsCloneable_WithPointerTypes_ShouldReturnFalse(Type pointerType)
    {
        // Act
        var result = TypeHelper.IsCloneable(pointerType);

        // Assert
        result.Should().BeFalse($"pointer type '{pointerType.Name}' should not be cloneable");
    }

    [Theory]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(string[]))]
    [InlineData(typeof(CloneableTestClass[]))]
    [InlineData(typeof(DateTime[]))]
    public void IsCloneable_WithArraysOfCloneableTypes_ShouldReturnTrue(Type arrayType)
    {
        // Act
        var result = TypeHelper.IsCloneable(arrayType);

        // Assert
        result.Should().BeTrue($"array of cloneable type '{arrayType.Name}' should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithArraysOfCloneableTypes_2_ShouldReturnTrue()
    {
        // Arrange
        TestClass[] items = [
            new TestClass { Id = 1, Name = "First" },
            new TestClass { Id = 2, Name = "Second" }
        ];

        // Act
        var result = TypeHelper.IsCloneable(items);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(typeof(Action[]))]
    [InlineData(typeof(Func<int>[]))]
    //[InlineData(typeof(IntPtr[]))]
    public void IsCloneable_WithArraysOfNonCloneableTypes_ShouldReturnFalse(Type arrayType)
    {
        // Act
        var result = TypeHelper.IsCloneable(arrayType);

        // Assert
        result.Should().BeFalse($"array of non-cloneable type '{arrayType.Name}' should not be cloneable");
    }

    [Theory]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(List<string>))]
    [InlineData(typeof(List<CloneableTestClass>))]
    [InlineData(typeof(Dictionary<string, int>))]
    [InlineData(typeof(Dictionary<string, CloneableTestClass>))]
    [InlineData(typeof(HashSet<string>))]
    [InlineData(typeof(Queue<int>))]
    [InlineData(typeof(Stack<string>))]
    public void IsCloneable_WithGenericCollectionsOfCloneableTypes_ShouldReturnTrue(Type collectionType)
    {
        // Act
        var result = TypeHelper.IsCloneable(collectionType);

        // Assert
        result.Should().BeTrue($"generic collection '{collectionType.Name}' with cloneable types should be cloneable");
    }

    [Theory]
    [InlineData(typeof(List<Action>))]
    [InlineData(typeof(Dictionary<string, Func<int>>))]
    [InlineData(typeof(HashSet<Func<int, int>>))]
    public void IsCloneable_WithGenericCollectionsOfNonCloneableTypes_ShouldReturnFalse(Type collectionType)
    {
        // Act
        var result = TypeHelper.IsCloneable(collectionType);

        // Assert
        result.Should().BeFalse($"generic collection '{collectionType.Name}' with non-cloneable types should not be cloneable");
    }

    [Fact]
    public void IsCloneable_WithNestedGenericTypes_ShouldEvaluateInnerTypes()
    {
        // Arrange
        var cloneableNestedType = typeof(List<List<string>>);
        var nonCloneableNestedType = typeof(List<List<Action>>);

        // Act
        var cloneableResult = TypeHelper.IsCloneable(cloneableNestedType);
        var nonCloneableResult = TypeHelper.IsCloneable(nonCloneableNestedType);

        // Assert
        cloneableResult.Should().BeTrue("nested generic with cloneable inner types should be cloneable");
        nonCloneableResult.Should().BeFalse("nested generic with non-cloneable inner types should not be cloneable");
    }

    [Fact]
    public void IsCloneable_WithComplexGenericType_ShouldEvaluateAllGenericArguments()
    {
        // Arrange
        var complexCloneableType = typeof(Dictionary<string, List<CloneableTestClass>>);
        var complexNonCloneableType = typeof(Dictionary<string, List<Action>>);

        // Act
        var cloneableResult = TypeHelper.IsCloneable(complexCloneableType);
        var nonCloneableResult = TypeHelper.IsCloneable(complexNonCloneableType);

        // Assert
        cloneableResult.Should().BeTrue("complex generic with all cloneable types should be cloneable");
        nonCloneableResult.Should().BeFalse("complex generic with any non-cloneable type should not be cloneable");
    }

    [Theory]
    [InlineData(typeof(IEnumerable<int>))]
    [InlineData(typeof(ICollection<string>))]
    [InlineData(typeof(IList<CloneableTestClass>))]
    [InlineData(typeof(IDictionary<string, int>))]
    public void IsCloneable_WithGenericInterfaces_ShouldReturnTrue(Type interfaceType)
    {
        // Act
        var result = TypeHelper.IsCloneable(interfaceType);

        // Assert
        result.Should().BeTrue($"generic interface '{interfaceType.Name}' with cloneable types should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithNonGenericIEnumerable_ShouldReturnTrue()
    {
        // Act
        var result = TypeHelper.IsCloneable(typeof(IEnumerable));

        // Assert
        result.Should().BeTrue("non-generic IEnumerable should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithRecord_ShouldReturnTrue()
    {
        // Act
        var result = TypeHelper.IsCloneable(typeof(TestRecord));

        // Assert
        result.Should().BeTrue("Records should be cloneable");
    }

    #endregion

    #region IsCloneable(object) Tests

    [Fact]
    public void IsCloneable_WithNullObject_ShouldReturnTrue()
    {
        // Act
        var result = TypeHelper.IsCloneable((object?)null);

        // Assert
        result.Should().BeTrue("null object should be considered cloneable");
    }

    [Theory]
    [InlineData(42)]
    [InlineData(3.14)]
    [InlineData(true)]
    [InlineData('A')]
    public void IsCloneable_WithValueTypeObjects_ShouldReturnTrue(object valueObj)
    {
        // Act
        var result = TypeHelper.IsCloneable(valueObj);

        // Assert
        result.Should().BeTrue($"value type object '{valueObj}' should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithStringObject_ShouldReturnTrue()
    {
        // Arrange
        var stringObj = "Hello World";

        // Act
        var result = TypeHelper.IsCloneable(stringObj);

        // Assert
        result.Should().BeTrue("string object should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithSimpleClassObject_ShouldReturnTrue()
    {
        // Arrange
        var obj = new CloneableTestClass
        {
            Name = "Test",
            Age = 25,
            CreatedAt = DateTime.Now
        };

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeTrue("simple class object should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithDisposableObject_ShouldReturnTrue()
    {
        // Arrange
        var obj = new DisposableTestClass();

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeTrue("disposable object should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithAsyncDisposableObject_ShouldReturnTrue()
    {
        // Arrange
        var obj = new AsyncDisposableTestClass();

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeTrue("async disposable object should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithDualDisposableObject_ShouldReturnTrue()
    {
        // Arrange
        var obj = new DualDisposableTestClass();

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeTrue("dual disposable object should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithNonDisposableObject_ShouldReturnTrue()
    {
        // Arrange
        var obj = new NonDisposableTestClass { Value = "Test" };

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeTrue("non-disposable object should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithObjectContainingDelegate_ShouldReturnFalse()
    {
        // Arrange
        var obj = new ClassWithDelegate
        {
            Name = "Test",
            OnAction = () => { },
            Converter = x => x.ToString()
        };

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeFalse("object containing delegates should not be cloneable");
    }

    [Fact]
    public void IsCloneable_WithObjectContainingEvent_ShouldReturnFalse()
    {
        // Arrange
        var obj = new ClassWithEvent { Name = "Test" };

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeFalse("object containing events should not be cloneable");
    }

    [Fact]
    public void IsCloneable_WithObjectContainingIntPtr_ShouldReturnFalse()
    {
        // Arrange
        var obj = new ClassWithIntPtr
        {
            Name = "Test",
            Handle = new IntPtr(123),
            UHandle = new UIntPtr(456)
        };

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeFalse("object containing IntPtr should not be cloneable");
    }

    [Fact]
    public void IsCloneable_WithArrayOfCloneableObjects_ShouldReturnTrue()
    {
        // Arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = TypeHelper.IsCloneable(array);

        // Assert
        result.Should().BeTrue("array of cloneable objects should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithListOfCloneableObjects_ShouldReturnTrue()
    {
        // Arrange
        var list = new List<string> { "one", "two", "three" };

        // Act
        var result = TypeHelper.IsCloneable(list);

        // Assert
        result.Should().BeTrue("list of cloneable objects should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithDictionaryOfCloneableObjects_ShouldReturnTrue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>
    {
        { "one", 1 },
        { "two", 2 },
        { "three", 3 }
    };

        // Act
        var result = TypeHelper.IsCloneable(dictionary);

        // Assert
        result.Should().BeTrue("dictionary of cloneable objects should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithNestedCloneableObject_ShouldReturnTrue()
    {
        // Arrange
        var obj = new NestedCloneableClass
        {
            Name = "Parent",
            NestedObject = new CloneableTestClass { Name = "Child", Age = 10 },
            NestedList = new List<CloneableTestClass>
        {
            new() { Name = "Item1", Age = 1 },
            new() { Name = "Item2", Age = 2 }
        }
        };

        // Act
        var result = TypeHelper.IsCloneable(obj);

        // Assert
        result.Should().BeTrue("object with nested cloneable objects should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithCircularReferenceObject_ShouldReturnTrue()
    {
        // Arrange
        var parent = new CircularReferenceClass { Name = "Parent" };
        var child = new CircularReferenceClass { Name = "Child", Parent = parent };
        parent.Children = new List<CircularReferenceClass> { child };

        // Act
        var result = TypeHelper.IsCloneable(parent);

        // Assert
        result.Should().BeTrue("object with potential circular references should still be considered cloneable");
    }

    [Fact]
    public void IsCloneable_WithCustomStruct_ShouldReturnTrue2()
    {
        // Arrange
        var structObj = new CloneableStruct
        {
            X = 10,
            Y = 20,
            Label = "Point"
        };

        // Act
        var result = TypeHelper.IsCloneable(structObj);

        // Assert
        result.Should().BeTrue("custom struct should be cloneable");
    }

    [Fact]
    public void IsCloneable_WithStructContainingDelegate_ShouldReturnFalse()
    {
        // Arrange
        var structObj = new StructWithDelegate
        {
            Value = 42,
            Callback = () => { }
        };

        // Act
        var result = TypeHelper.IsCloneable(structObj);

        // Assert
        result.Should().BeFalse("struct containing delegate should not be cloneable");
    }

    #endregion

    #region Edge Cases and Complex Scenarios

    [Fact]
    public void IsCloneable_WithGenericClassInstances_ShouldEvaluateGenericArguments()
    {
        // Arrange
        var cloneableGeneric = new GenericTestClass<string>
        {
            Value = "test",
            Items = new List<string> { "item1", "item2" }
        };
        var nonCloneableGeneric = new GenericTestClass<Action>
        {
            Value = () => { },
            Items = new List<Action> { () => { } }
        };

        // Act
        var cloneableResult = TypeHelper.IsCloneable(cloneableGeneric);
        var nonCloneableResult = TypeHelper.IsCloneable(nonCloneableGeneric);

        // Assert
        cloneableResult.Should().BeTrue("generic class with cloneable type arguments should be cloneable");
        nonCloneableResult.Should().BeFalse("generic class with non-cloneable type arguments should not be cloneable");
    }

    [Fact]
    public void IsCloneable_WithMixedCollection_ShouldEvaluateElementTypes()
    {
        // Arrange
        var cloneableCollection = new List<object> { "string", 42, new CloneableTestClass() };
        var nonCloneableCollection = new List<object> { "string", 42, new Action(() => { }) };

        // Act
        var cloneableResult = TypeHelper.IsCloneable(cloneableCollection);
        var nonCloneableResult = TypeHelper.IsCloneable(nonCloneableCollection);

        // Assert
        cloneableResult.Should().BeTrue("collection with cloneable runtime types should be cloneable");
        nonCloneableResult.Should().BeTrue("object collection should be considered cloneable based on declared type");
    }

    [Theory]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(TimeSpan))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Uri))]
    public void IsCloneable_WithCommonFrameworkTypes_ShouldReturnExpectedResults(Type frameworkType)
    {
        // Act
        var result = TypeHelper.IsCloneable(frameworkType);

        // Assert
        result.Should().BeTrue($"common framework type '{frameworkType.Name}' should be cloneable");
    }

    [Fact]
    public void IsCloneable_PerformanceTest_ShouldHandleLargeNumberOfCalls()
    {
        // Arrange
        var types = new[]
        {
        typeof(int), typeof(string), typeof(List<int>), typeof(Dictionary<string, object>),
        typeof(CloneableTestClass), typeof(Action), typeof(IntPtr), typeof(DateTime)
    };

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            foreach (var type in types)
            {
                TypeHelper.IsCloneable(type);
            }
        }
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000,
            "IsCloneable should handle 80,000 calls efficiently");
    }

    #endregion

    #region Integration with IsCloneable and DeepClone

    [Fact]
    public void IsCloneable_ConsistencyWithDeepClone_ShouldMatchActualCloneability()
    {
        // Arrange
        var cloneableObject = new CloneableTestClass { Name = "Test", Age = 25 };
        var problematicObject = new ClassWithDelegate { Name = "Test", OnAction = () => { } };

        // Act
        var cloneableIsCloneable = TypeHelper.IsCloneable(cloneableObject);
        var problematicIsCloneable = TypeHelper.IsCloneable(problematicObject);

        var cloneableCanClone = TypeHelper.TryDeepClone(cloneableObject, out _);
        var problematicCanClone = TypeHelper.TryDeepClone(problematicObject, out _);

        // Assert
        cloneableIsCloneable.Should().BeTrue("cloneable object should be identified as cloneable");
        cloneableCanClone.Should().BeTrue("cloneable object should actually clone successfully");

        problematicIsCloneable.Should().BeFalse("problematic object should be identified as non-cloneable");
        problematicCanClone.Should().BeFalse("problematic object should fail to clone");
    }

    [Fact]
    public void IsCloneable_WithVariousCollectionTypes_ShouldEvaluateCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
        (Collection: (object)new int[] { 1, 2, 3 }, Expected: true, Name: "int array"),
        (Collection: new List<string> { "a", "b" }, Expected: true, Name: "string list"),
        (Collection: new Dictionary<string, int> { { "key", 1 } }, Expected: true, Name: "string-int dictionary"),
        (Collection: new HashSet<DateTime> { DateTime.Now }, Expected: true, Name: "DateTime hashset"),
        (Collection: new Queue<CloneableTestClass>(), Expected: true, Name: "cloneable class queue"),
        (Collection: new Stack<NonDisposableTestClass>(), Expected: true, Name: "non-disposable class stack")
    };

        foreach (var testCase in testCases)
        {
            // Act
            var result = TypeHelper.IsCloneable(testCase.Collection);

            // Assert
            result.Should().Be(testCase.Expected, $"{testCase.Name} cloneability should be {testCase.Expected}");
        }
    }

    #endregion

    #region Test Helper Classes

    public class TestClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public NestedTestClass? NestedObject { get; set; }
    }

    public class NestedTestClass
    {
        public string? Value { get; set; }
    }

    public class DisposableTestClass : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public record TestRecord(string Name, int Value);

    /// <summary>
    /// Test class that implements IAsyncDisposable for async disposal testing
    /// </summary>
    public class AsyncDisposableTestClass : IAsyncDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool DisposalWasAsync { get; private set; }

        public ValueTask DisposeAsync()
        {
            IsDisposed = true;
            DisposalWasAsync = true;
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Test class that implements both IDisposable and IAsyncDisposable
    /// </summary>
    public class DualDisposableTestClass : IDisposable, IAsyncDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool DisposalWasAsync { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
            DisposalWasAsync = false;
        }

        public ValueTask DisposeAsync()
        {
            IsDisposed = true;
            DisposalWasAsync = true;
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Test class that throws an exception during disposal
    /// </summary>
    public class FaultyDisposableTestClass : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool ThrowOnDispose { get; set; } = true;

        public void Dispose()
        {
            if (ThrowOnDispose)
                throw new InvalidOperationException("Disposal failed");

            IsDisposed = true;
        }
    }

    /// <summary>
    /// Test class that throws an exception during async disposal
    /// </summary>
    public class FaultyAsyncDisposableTestClass : IAsyncDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool ThrowOnDispose { get; set; } = true;

        public ValueTask DisposeAsync()
        {
            if (ThrowOnDispose)
                throw new InvalidOperationException("Async disposal failed");

            IsDisposed = true;
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Non-disposable test class for negative testing
    /// </summary>
    public class NonDisposableTestClass
    {
        public string Value { get; set; } = "Test";
    }

    public class NonSerializableClass
    {
        public IntPtr Pointer { get; set; } = new IntPtr(123);
    }

    /// <summary>
    /// Simple cloneable test class with basic properties
    /// </summary>
    public class CloneableTestClass
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Test class with JsonIgnore attribute
    /// </summary>
    public class JsonIgnoredTestClass
    {
        [JsonIgnore]
        public string Value { get; set; } = "Test";
    }

    /// <summary>
    /// Test class containing delegates (non-serializable)
    /// </summary>
    public class ClassWithDelegate
    {
        public string Name { get; set; } = "Test";
        public Action? OnAction { get; set; }
        public Func<int, string>? Converter { get; set; }
    }

    /// <summary>
    /// Test class containing events (non-serializable)
    /// </summary>
    public class ClassWithEvent
    {
        public string Name { get; set; } = "Test";
        public event EventHandler? SomethingHappened;
#pragma warning disable CS0067 // Event is never used
        public event EventHandler<string>? GenericEvent;
#pragma warning restore CS0067 // Event is never used

        protected virtual void OnSomethingHappened()
        {
            SomethingHappened?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Test class with IntPtr (non-serializable)
    /// </summary>
    public class ClassWithIntPtr
    {
        public string Name { get; set; } = "Test";
        public IntPtr Handle { get; set; }
        public UIntPtr UHandle { get; set; }
    }

    /// <summary>
    /// Generic test class for testing generic collections
    /// </summary>
    public class GenericTestClass<T>
    {
        public T? Value { get; set; }
        public List<T>? Items { get; set; }
    }

    /// <summary>
    /// Test class with nested cloneable objects
    /// </summary>
    public class NestedCloneableClass
    {
        public string? Name { get; set; }
        public CloneableTestClass? NestedObject { get; set; }
        public List<CloneableTestClass>? NestedList { get; set; }
    }

    /// <summary>
    /// Test class with circular reference potential
    /// </summary>
    public class CircularReferenceClass
    {
        public string? Name { get; set; }
        public CircularReferenceClass? Parent { get; set; }
        public List<CircularReferenceClass>? Children { get; set; }
    }

    /// <summary>
    /// Custom struct for value type testing
    /// </summary>
    public struct CloneableStruct
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Label { get; set; }
    }

    /// <summary>
    /// Struct with non-cloneable members
    /// </summary>
    public struct StructWithDelegate
    {
        public int Value { get; set; }
        public Action? Callback { get; set; }
    }

    #endregion
}