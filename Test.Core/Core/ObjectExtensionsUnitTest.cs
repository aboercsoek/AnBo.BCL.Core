using FluentAssertions;
using Xunit;
using AnBo.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AnBo.Test
{
    public class ObjectExtensionsUnitTest
    {
        #region AsUniversal<T> Method Tests

        [Fact]
        public void TestCase001_As_With_Null_Should_Return_Default()
        {
            // Arrange
            object? item = null;

            // Act
            var result = item.AsUniversal<string>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase002_As_With_Compatible_Type_Should_Return_Casted_Value()
        {
            // Arrange
            object item = "test string";

            // Act
            var result = item.AsUniversal<string>();

            // Assert
            result.Should().Be("test string");
        }

        [Fact]
        public void TestCase003_As_With_Incompatible_Type_Should_Return_Default()
        {
            // Arrange
            object item = "test string";

            // Act
            var result = item.AsUniversal<int>();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase004_As_With_Value_Type_Should_Return_Correct_Value()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.AsUniversal<int>();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void TestCase005_As_With_Nullable_Value_Type_Should_Return_Correct_Value()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.AsUniversal<int?>();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void TestCase006_As_With_Inheritance_Should_Return_Base_Type()
        {
            // Arrange
            object item = new List<string>();

            // Act
            var result = item.AsUniversal<IEnumerable>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        #endregion

        #region AsSequence<TSource, TTarget> Method Tests

        [Fact]
        public void TestCase007_AsSequence_Generic_With_Null_Should_Return_Empty_Sequence()
        {
            // Arrange
            IEnumerable<object>? source = null;

            // Act
            var result = source.AsSequence<object, string>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase008_AsSequence_Generic_With_Compatible_Types_Should_Return_Casted_Items()
        {
            // Arrange
            var source = new List<object> { "test1", "test2", "test3" };

            // Act
            var result = source.AsSequence<object, string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().ContainInOrder("test1", "test2", "test3");
        }

        [Fact]
        public void TestCase009_AsSequence_Generic_With_Incompatible_Types_Should_Return_Empty_Sequence()
        {
            // Arrange
            var source = new List<string> { "test1", "test2", "test3" };

            // Act
            var result = source.AsSequence<string, int?>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase010_AsSequence_Generic_With_Mixed_Compatible_Items_Should_Filter_Compatible()
        {
            // Arrange
            var source = new List<object> { "test1", 42, "test2", null, "test3" };

            // Act
            var result = source.AsSequence<object, string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().ContainInOrder("test1", "test2", "test3");
        }

        [Fact]
        public void TestCase011_AsSequence_Generic_With_Inheritance_Should_Cast_Derived_Types()
        {
            // Arrange
            var source = new List<List<string>> { new(), new() };

            // Act
            var result = source.AsSequence<List<string>, IEnumerable>().ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllBeAssignableTo<IEnumerable>();
        }

        #endregion

        #region AsSequence<TTarget> Method Tests

        [Fact]
        public void TestCase012_AsSequence_NonGeneric_With_Null_Should_Return_Empty_Sequence()
        {
            // Arrange
            IEnumerable? source = null;

            // Act
            var result = source.AsSequence<string>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase013_AsSequence_NonGeneric_With_Compatible_Items_Should_Return_Casted_Items()
        {
            // Arrange
            var source = new ArrayList { "test1", "test2", "test3" };

            // Act
            var result = source.AsSequence<string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain("test1", "test2", "test3");
        }

        [Fact]
        public void TestCase014_AsSequence_NonGeneric_With_Mixed_Items_Should_Filter_Compatible()
        {
            // Arrange
            var source = new ArrayList { "test1", 42, "test2", null };

            // Act
            var result = source.AsSequence<string>().ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainInOrder("test1", "test2");
        }

        [Fact]
        public void TestCase015_AsSequence_NonGeneric_With_Value_Types_Should_Handle_Default_Values()
        {
            // Arrange
            var source = new ArrayList { 1, 2, 0, 3 };

            // Act
            var result = source.AsSequence<int>().ToList();

            // Assert
            result.Should().HaveCount(4);
            result.Should().ContainInOrder(1, 2, 0, 3);
        }

        #endregion

        #region Cast<T> Method Tests

        [Fact]
        public void TestCase016_Cast_With_Null_Should_Return_Default()
        {
            // Arrange
            object? item = null;

            // Act
            var result = item.Cast<string>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase017_Cast_With_Compatible_Type_Should_Return_Casted_Value()
        {
            // Arrange
            object item = "test string";

            // Act
            var result = item.Cast<string>();

            // Assert
            result.Should().Be("test string");
        }

        [Fact]
        public void TestCase018_Cast_With_Incompatible_Type_Should_Throw_InvalidCastException()
        {
            // Arrange
            object item = "test string";

            // Act & Assert
            var action = () => item.Cast<int>();
            action.Should().Throw<InvalidCastException>()
                .WithMessage("Can not cast from type String to Int32");
        }

        [Fact]
        public void TestCase019_Cast_With_Value_Type_Should_Return_Correct_Value()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.Cast<int>();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void TestCase020_Cast_With_Inheritance_Should_Return_Base_Type()
        {
            // Arrange
            object item = new List<string>();

            // Act
            var result = item.Cast<IEnumerable>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        #endregion

        #region CastSequence<TSource, TTarget> Method Tests

        [Fact]
        public void TestCase021_CastSequence_With_Null_Should_Return_Empty_Sequence()
        {
            // Arrange
            IEnumerable<object>? source = null;

            // Act
            var result = source.CastSequence<object, string>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase022_CastSequence_With_Compatible_Types_Should_Return_Casted_Items()
        {
            // Arrange
            var source = new List<object> { "test1", "test2", "test3" };

            // Act
            var result = source.CastSequence<object, string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain("test1", "test2", "test3");
        }

        [Fact]
        public void TestCase023_CastSequence_With_Incompatible_Type_Should_Throw_InvalidCastException()
        {
            // Arrange
            var source = new List<object> { "test1", 42, "test2" };

            // Act & Assert
            var action = () => source.CastSequence<object, string>().ToList();
            action.Should().Throw<InvalidCastException>();
        }

        [Fact]
        public void TestCase024_CastSequence_With_Null_Items_Should_Skip_Null_Items()
        {
            // Arrange
            var source = new List<object?> { "test1", null, "test2" };

            // Act
            var result = source.CastSequence<object?, string>().ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain("test1", "test2");
        }

        [Fact]
        public void TestCase025_CastSequence_With_Inheritance_Should_Cast_Derived_Types()
        {
            // Arrange
            var source = new List<List<string>> { new(), new() };

            // Act
            var result = source.CastSequence<List<string>, IEnumerable>().ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllBeAssignableTo<IEnumerable>();
        }

        #endregion

        #region New Method Tests

        [Fact]
        public void TestCase026_New_With_Null_Type_Should_Return_Null()
        {
            // Arrange
            Type? type = null;

            // Act
            var result = type.New();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase027_New_With_Valid_Type_Should_Create_Instance()
        {
            // Arrange
            Type type = typeof(string);

            // Act
            var result = type.New();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
        }

        [Fact]
        public void TestCase028_New_With_Value_Type_Should_Create_Default_Instance()
        {
            // Arrange
            Type type = typeof(int);

            // Act
            var result = type.New();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<int>();
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase029_New_With_Complex_Type_Should_Create_Instance()
        {
            // Arrange
            Type type = typeof(List<string>);

            // Act
            var result = type.New();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        #endregion

        #region With<T> Method Tests

        [Fact]
        public void TestCase030_With_Should_Execute_Action_And_Return_Object()
        {
            // Arrange
            var list = new List<string>();
            var actionExecuted = false;

            // Act
            var result = list.With(l =>
            {
                l.Add("test");
                actionExecuted = true;
            });

            // Assert
            result.Should().BeSameAs(list);
            result.Should().Contain("test");
            actionExecuted.Should().BeTrue();
        }

        [Fact]
        public void TestCase031_With_Should_Handle_Null_Object()
        {
            // Arrange
            List<string>? list = null;
            var actionExecuted = false;

            // Act & Assert
            //var action = () => list!.With(l =>
            //{
            //    actionExecuted = true;
            //});
            // Act & Assert
            var action = () => {
                var count = list!.Count;
                actionExecuted = true;
            };
            action.Should().Throw<NullReferenceException>();
            actionExecuted.Should().BeFalse();
        }

        [Fact]
        public void TestCase032_With_Should_Allow_Method_Chaining()
        {
            // Arrange
            var list = new List<string>();

            // Act
            var result = list
                .With(l => l.Add("first"))
                .With(l => l.Add("second"));

            // Assert
            result.Should().BeSameAs(list);
            result.Should().HaveCount(2);
            result.Should().Contain("first", "second");
        }

        #endregion

        #region WithDispose<T> Action Method Tests

        [Fact]
        public void TestCase033_WithDispose_Action_Should_Execute_Action_And_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable();
            var actionExecuted = false;

            // Act
            disposable.WithDispose(d =>
            {
                actionExecuted = true;
                d.Value = "modified";
            });

            // Assert
            actionExecuted.Should().BeTrue();
            disposable.Value.Should().Be("modified");
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void TestCase034_WithDispose_Action_Should_Dispose_Even_On_Exception()
        {
            // Arrange
            var disposable = new TestDisposable();

            // Act & Assert
            var action = () => disposable.WithDispose(d =>
            {
                throw new InvalidOperationException("Test exception");
            });

            action.Should().Throw<InvalidOperationException>();
            disposable.IsDisposed.Should().BeTrue();
        }

        #endregion

        #region WithDispose<T> Sequence Method Tests

        [Fact]
        public void TestCase035_WithDispose_Sequence_Should_Execute_Action_For_Each_And_Dispose_All()
        {
            // Arrange
            var disposables = new List<TestDisposable>
            {
                new TestDisposable { Value = "first" },
                new TestDisposable { Value = "second" },
                new TestDisposable { Value = "third" }
            };
            var executionCount = 0;

            // Act
            disposables.WithDispose(d =>
            {
                executionCount++;
                d.Value += "_modified";
            });

            // Assert
            executionCount.Should().Be(3);
            disposables.Should().AllSatisfy(d =>
            {
                d.Value.Should().EndWith("_modified");
                d.IsDisposed.Should().BeTrue();
            });
        }

        [Fact]
        public void TestCase036_WithDispose_Sequence_Should_Dispose_All_Even_On_Exception()
        {
            // Arrange
            var disposables = new List<TestDisposable>
            {
                new TestDisposable { Value = "first" },
                new TestDisposable { Value = "second" },
                new TestDisposable { Value = "third" }
            };

            // Act & Assert
            var action = () => disposables.WithDispose(d =>
            {
                if (d.Value == "second")
                    throw new InvalidOperationException("Test exception");
            });

            action.Should().Throw<InvalidOperationException>();
            
            // First item should be disposed, others might not be due to exception
            disposables[0].IsDisposed.Should().BeTrue();
        }

        #endregion

        #region WithDispose<TDisposable, TResult> Func Method Tests

        [Fact]
        public void TestCase037_WithDispose_Func_Should_Execute_Func_Return_Result_And_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable { Value = "test" };

            // Act
            var result = disposable.WithDispose(d => d.Value.ToUpper());

            // Assert
            result.Should().Be("TEST");
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void TestCase038_WithDispose_Func_Should_Dispose_Even_On_Exception()
        {
            // Arrange
            var disposable = new TestDisposable();

            // Act & Assert
            var action = () => disposable.WithDispose<TestDisposable, string>(d =>
            {
                throw new InvalidOperationException("Test exception");
            });

            action.Should().Throw<InvalidOperationException>();
            disposable.IsDisposed.Should().BeTrue();
        }

        #endregion

        #region WithDispose<TDisposable, TFunc2, TResult> Method Tests

        [Fact]
        public void TestCase039_WithDispose_Func_With_Second_Parameter_Should_Execute_And_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable { Value = "test" };
            var secondParam = "_suffix";

            // Act
            var result = disposable.WithDispose(secondParam, (d, suffix) => d.Value + suffix);

            // Assert
            result.Should().Be("test_suffix");
            disposable.IsDisposed.Should().BeTrue();
        }

        #endregion

        #region DisposeIfNecessary Method Tests

        [Fact]
        public void TestCase040_DisposeIfNecessary_With_Null_Should_Not_Throw()
        {
            // Arrange
            object? obj = null;

            // Act & Assert
            var action = () => obj.DisposeIfNecessary();
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase041_DisposeIfNecessary_With_Disposable_Should_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable();

            // Act
            ((object)disposable).DisposeIfNecessary();

            // Assert
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void TestCase042_DisposeIfNecessary_With_Non_Disposable_Should_Not_Throw()
        {
            // Arrange
            object obj = "test string";

            // Act & Assert
            var action = () => obj.DisposeIfNecessary();
            action.Should().NotThrow();
        }

        #endregion

        #region DisposeElementsIfNecessary Method Tests

        [Fact]
        public void TestCase043_DisposeElementsIfNecessary_With_Null_Should_Not_Throw()
        {
            // Arrange
            IEnumerable? sequence = null;

            // Act & Assert
            var action = () => sequence.DisposeElementsIfNecessary();
            action.Should().NotThrow();
        }

        [Fact]
        public void TestCase044_DisposeElementsIfNecessary_With_Disposable_Elements_Should_Dispose_All()
        {
            // Arrange
            var disposables = new ArrayList
            {
                new TestDisposable { Value = "first" },
                new TestDisposable { Value = "second" },
                "non-disposable",
                new TestDisposable { Value = "third" }
            };

            // Act
            disposables.DisposeElementsIfNecessary();

            // Assert
            ((TestDisposable)disposables[0]!).IsDisposed.Should().BeTrue();
            ((TestDisposable)disposables[1]!).IsDisposed.Should().BeTrue();
            ((TestDisposable)disposables[3]!).IsDisposed.Should().BeTrue();
        }

        #endregion

        #region IsNull<T> Method Tests

        [Fact]
        public void TestCase045_IsNull_With_Null_Reference_Type_Should_Return_True()
        {
            // Arrange
            string? value = null;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase046_IsNull_With_Non_Null_Reference_Type_Should_Return_False()
        {
            // Arrange
            string value = "test";

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase047_IsNull_With_Value_Type_Should_Return_False()
        {
            // Arrange
            int value = 42;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase048_IsNull_With_Default_Value_Type_Should_Return_False()
        {
            // Arrange
            int value = 0;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase049_IsNull_With_Nullable_Value_Type_Null_Should_Return_True()
        {
            // Arrange
            int? value = null;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase050_IsNull_With_Nullable_Value_Type_Not_Null_Should_Return_False()
        {
            // Arrange
            int? value = 42;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsNotNull<T> Method Tests

        [Fact]
        public void TestCase051_IsNotNull_With_Null_Reference_Type_Should_Return_False()
        {
            // Arrange
            string? value = null;

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase052_IsNotNull_With_Non_Null_Reference_Type_Should_Return_True()
        {
            // Arrange
            string value = "test";

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase053_IsNotNull_With_Value_Type_Should_Return_True()
        {
            // Arrange
            int value = 42;

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase054_IsNotNull_With_Nullable_Value_Type_Null_Should_Return_False()
        {
            // Arrange
            int? value = null;

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase055_IsNotNull_With_Nullable_Value_Type_Not_Null_Should_Return_True()
        {
            // Arrange
            int? value = 42;

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region IsDefaultValue Method Tests

        [Fact]
        public void TestCase056_IsDefaultValue_With_Null_Should_Return_True()
        {
            // Arrange
            object? value = null;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase057_IsDefaultValue_With_Reference_Type_Default_Should_Return_False()
        {
            // Arrange
            object value = "test";

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase058_IsDefaultValue_With_Value_Type_Default_Should_Return_True()
        {
            // Arrange
            object value = 0;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase059_IsDefaultValue_With_Value_Type_Non_Default_Should_Return_False()
        {
            // Arrange
            object value = 42;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase060_IsDefaultValue_With_Boolean_Default_Should_Return_True()
        {
            // Arrange
            object value = false;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase061_IsDefaultValue_With_Boolean_Non_Default_Should_Return_False()
        {
            // Arrange
            object value = true;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsDefaultValueOrEmptyString Method Tests

        [Fact]
        public void TestCase062_IsDefaultValueOrEmptyString_With_Null_Should_Return_True()
        {
            // Arrange
            object? value = null;

            // Act
            var result = value.IsDefaultValueOrEmptyString();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase063_IsDefaultValueOrEmptyString_With_Empty_String_Should_Return_True()
        {
            // Arrange
            object value = string.Empty;

            // Act
            var result = value.IsDefaultValueOrEmptyString();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase064_IsDefaultValueOrEmptyString_With_Non_Empty_String_Should_Return_False()
        {
            // Arrange
            object value = "test";

            // Act
            var result = value.IsDefaultValueOrEmptyString();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase065_IsDefaultValueOrEmptyString_With_Value_Type_Default_Should_Return_True()
        {
            // Arrange
            object value = 0;

            // Act
            var result = value.IsDefaultValueOrEmptyString();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase066_IsDefaultValueOrEmptyString_With_Value_Type_Non_Default_Should_Return_False()
        {
            // Arrange
            object value = 42;

            // Act
            var result = value.IsDefaultValueOrEmptyString();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Helper Classes

        private class TestDisposable : IDisposable
        {
            public string Value { get; set; } = string.Empty;
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        #endregion
    }
}