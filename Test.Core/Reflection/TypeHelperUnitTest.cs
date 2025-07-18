using FluentAssertions;
using Xunit;
using AnBo.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AnBo.Test
{
    public class TypeHelperUnitTest
    {
        #region DeepClone Tests

        [Fact]
        public void TestCase001_DeepClone_Object_With_Null_Should_Return_Null()
        {
            // Arrange
            object? original = null;

            // Act
            var result = TypeHelper.DeepClone(original);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase002_DeepClone_Generic_With_Null_Should_Return_Default()
        {
            // Arrange
            string? original = null;

            // Act
            var result = TypeHelper.DeepClone(original);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase003_DeepClone_Object_With_Simple_Value_Should_Create_Copy()
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
        public void TestCase004_DeepClone_Generic_With_Simple_Value_Should_Create_Copy()
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
        public void TestCase005_DeepClone_Object_With_Complex_Object_Should_Create_Deep_Copy()
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
        public void TestCase006_DeepClone_Generic_With_Complex_Object_Should_Create_Deep_Copy()
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
        public void TestCase007_DeepClone_With_Value_Types_Should_Work()
        {
            // Arrange
            int original = 42;

            // Act
            var result = TypeHelper.DeepClone(original);

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void TestCase008_DeepClone_With_Collections_Should_Create_Deep_Copy()
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
        public void TestCase009_DeepClone_Object_With_Non_Serializable_Type_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var original = new NonSerializableClass();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => TypeHelper.DeepClone(original, typeof(NonSerializableClass)));
            exception.Message.Should().Contain("Fehler beim Deep Clone von Typ NonSerializableClass");
        }

        [Fact]
        public void TestCase010_DeepClone_Generic_With_Non_Serializable_Type_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var original = new NonSerializableClass();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => TypeHelper.DeepClone(original));
            exception.Message.Should().Contain("Fehler beim Deep Clone von Typ NonSerializableClass");
        }

        #endregion

        #region DisposeIfNecessary Tests

        [Fact]
        public void TestCase011_DisposeIfNecessary_With_Null_Should_Not_Throw()
        {
            // Arrange
            object? obj = null;

            // Act & Assert
            var action = () => TypeHelper.DisposeIfNecessary(obj);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase012_DisposeIfNecessary_With_IDisposable_Should_Call_Dispose()
        {
            // Arrange
            var disposable = new DisposableTestClass();

            // Act
            TypeHelper.DisposeIfNecessary(disposable);

            // Assert
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void TestCase013_DisposeIfNecessary_With_Non_IDisposable_Should_Not_Throw()
        {
            // Arrange
            var obj = new TestClass { Id = 1, Name = "Test" };

            // Act & Assert
            var action = () => TypeHelper.DisposeIfNecessary(obj);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase014_DisposeIfNecessary_With_String_Should_Not_Throw()
        {
            // Arrange
            string obj = "test string";

            // Act & Assert
            var action = () => TypeHelper.DisposeIfNecessary(obj);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase015_DisposeIfNecessary_With_Already_Disposed_Object_Should_Not_Throw()
        {
            // Arrange
            var disposable = new DisposableTestClass();
            disposable.Dispose(); // Dispose first time

            // Act & Assert
            var action = () => TypeHelper.DisposeIfNecessary(disposable);
            action.Should().NotThrow();
        }

        #endregion

        #region DisposeElementsIfNecessary Tests

        [Fact]
        public void TestCase016_DisposeElementsIfNecessary_With_Null_Should_Not_Throw()
        {
            // Arrange
            IEnumerable? enumerable = null;

            // Act & Assert
            var action = () => TypeHelper.DisposeElementsIfNecessary(enumerable);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase017_DisposeElementsIfNecessary_With_Empty_Collection_Should_Not_Throw()
        {
            // Arrange
            var enumerable = new List<object>();

            // Act & Assert
            var action = () => TypeHelper.DisposeElementsIfNecessary(enumerable);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase018_DisposeElementsIfNecessary_With_IDisposable_Elements_Should_Dispose_All()
        {
            // Arrange
            var disposables = new List<DisposableTestClass>
            {
                new DisposableTestClass(),
                new DisposableTestClass(),
                new DisposableTestClass()
            };

            // Act
            TypeHelper.DisposeElementsIfNecessary(disposables);

            // Assert
            disposables.Should().AllSatisfy(d => d.IsDisposed.Should().BeTrue());
        }

        [Fact]
        public void TestCase019_DisposeElementsIfNecessary_With_Mixed_Elements_Should_Dispose_Only_IDisposable()
        {
            // Arrange
            var disposable1 = new DisposableTestClass();
            var disposable2 = new DisposableTestClass();
            var nonDisposable = new TestClass { Id = 1, Name = "Test" };
            
            var mixed = new ArrayList { disposable1, nonDisposable, disposable2 };

            // Act
            TypeHelper.DisposeElementsIfNecessary(mixed);

            // Assert
            disposable1.IsDisposed.Should().BeTrue();
            disposable2.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void TestCase020_DisposeElementsIfNecessary_With_Non_IDisposable_Elements_Should_Not_Throw()
        {
            // Arrange
            var enumerable = new List<string> { "item1", "item2", "item3" };

            // Act & Assert
            var action = () => TypeHelper.DisposeElementsIfNecessary(enumerable);
            action.Should().NotThrow();
        }

        #endregion

        #region DisposeValuesIfNecessary Tests

        [Fact]
        public void TestCase021_DisposeValuesIfNecessary_With_Null_Should_Not_Throw()
        {
            // Arrange
            IDictionary? dict = null;

            // Act & Assert
            var action = () => TypeHelper.DisposeValuesIfNecessary(dict);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase022_DisposeValuesIfNecessary_With_Empty_Dictionary_Should_Not_Throw()
        {
            // Arrange
            var dict = new Hashtable();

            // Act & Assert
            var action = () => TypeHelper.DisposeValuesIfNecessary(dict);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase023_DisposeValuesIfNecessary_With_IDisposable_Values_Should_Dispose_All()
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
            TypeHelper.DisposeValuesIfNecessary(dict);

            // Assert
            disposable1.IsDisposed.Should().BeTrue();
            disposable2.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void TestCase024_DisposeValuesIfNecessary_With_Mixed_Values_Should_Dispose_Only_IDisposable()
        {
            // Arrange
            var disposable = new DisposableTestClass();
            var nonDisposable = new TestClass { Id = 1, Name = "Test" };
            var dict = new Hashtable
            {
                { "disposable", disposable },
                { "nonDisposable", nonDisposable },
                { "string", "test value" }
            };

            // Act
            TypeHelper.DisposeValuesIfNecessary(dict);

            // Assert
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void TestCase025_DisposeValuesIfNecessary_With_Non_IDisposable_Values_Should_Not_Throw()
        {
            // Arrange
            var dict = new Hashtable
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", 42 }
            };

            // Act & Assert
            var action = () => TypeHelper.DisposeValuesIfNecessary(dict);
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase026_DisposeValuesIfNecessary_With_Generic_Dictionary_Should_Work()
        {
            // Arrange
            var disposable1 = new DisposableTestClass();
            var disposable2 = new DisposableTestClass();
            IDictionary dict = new Dictionary<string, object>
            {
                { "key1", disposable1 },
                { "key2", disposable2 }
            };

            // Act
            TypeHelper.DisposeValuesIfNecessary(dict);

            // Assert
            disposable1.IsDisposed.Should().BeTrue();
            disposable2.IsDisposed.Should().BeTrue();
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

        public class NonSerializableClass
        {
            public IntPtr Pointer { get; set; } = new IntPtr(123);
        }

        #endregion
    }
}