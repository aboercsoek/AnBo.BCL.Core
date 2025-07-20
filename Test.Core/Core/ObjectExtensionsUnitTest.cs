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
        public void AsUniversal_With_Null_Should_Return_Default()
        {
            // Arrange
            object? item = null;

            // Act
            var result = item.AsUniversal<string>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void AsUniversal_With_Compatible_Type_Should_Return_Casted_Value()
        {
            // Arrange
            object item = "test string";

            // Act
            var result = item.AsUniversal<string>();

            // Assert
            result.Should().Be("test string");
        }

        [Fact]
        public void AsUniversal_With_Incompatible_Type_Should_Return_Default()
        {
            // Arrange
            object item = "test string";

            // Act
            int? result = item.AsUniversal<int>();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void AsUniversal_With_Incompatible_Type_Should_Return_Null()
        {
            // Arrange
            object item = "test string";

            // Act
            var result = item.AsValueType<int>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void AsUniversal_With_Value_Type_Should_Return_Correct_Value()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.AsUniversal<int>();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void AsUniversal_With_Nullable_Value_Type_Should_Return_Correct_Value()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.AsUniversal<int?>();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void AsUniversal_With_Nullable_Value_Type_Should_Return_Null()
        {
            // Arrange
            object? item = null;

            // Act
            var result = item.AsUniversal<int?>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void As_With_Nullable_Value_Type_Should_Return_Null()
        {
            // Arrange
            object? item = "test string";

            // Act
            var result = item.AsUniversal<int?>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void As_With_Inheritance_Should_Return_Base_Type()
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

        #region CastSequence<TSource, TTarget> Method Tests

        [Fact]
        public void CastSequence_Generic_With_Null_Should_Return_Empty_Sequence()
        {
            // Arrange
            IEnumerable<object>? source = null;

            // Act
#pragma warning disable CS8604 // Possible null reference argument.
            var result = source.CastSequence<object, string>().ToList();
#pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void CastSequence_Generic_With_Compatible_Types_Should_Return_Casted_Items()
        {
            // Arrange
            var source = new List<object> { "test1", "test2", "test3" };

            // Act
            var result = source.CastSequence<object, string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().ContainInOrder("test1", "test2", "test3");
        }

        [Fact]
        public void CastSequence_Generic_With_Incompatible_Types_Should_Return_Empty_Sequence()
        {
            // Arrange
            var source = new List<string> { "test1", "test2", "test3" };

            // Act
            var result = source.CastSequence<string, int?>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void CastSequence_Generic_With_Mixed_Compatible_Items_Should_Filter_Compatible()
        {
            // Arrange
            var source = new List<object?> { "test1", 42, "test2", null, "test3" };

            // Act
            var result = source.CastSequence<object?, string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().ContainInOrder("test1", "test2", "test3");
        }

        [Fact]
        public void CastSequence_Generic_With_Inheritance_Should_Cast_Derived_Types()
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

        #region CastSequence<TTarget> Method Tests

        [Fact]
        public void CastSequence_NonGeneric_With_Null_Should_Return_Empty_Sequence()
        {
            // Arrange
            IEnumerable? source = null;

            // Act
#pragma warning disable CS8604 // Possible null reference argument.
            var result = source.CastSequence<string>().ToList();
#pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void CastSequence_NonGeneric_With_Compatible_Items_Should_Return_Casted_Items()
        {
            // Arrange
            var source = new ArrayList { "test1", "test2", "test3" };

            // Act
            var result = source.CastSequence<string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain("test1", "test2", "test3");
        }

        [Fact]
        public void CastSequence_NonGeneric_With_Mixed_Items_Should_Filter_Compatible()
        {
            // Arrange
            var source = new ArrayList { "test1", 42, "test2", null };

            // Act
            var result = source.CastSequence<string>().ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainInOrder("test1", "test2");
        }

        [Fact]
        public void CastSequence_NonGeneric_With_Value_Types_Should_Handle_Default_Values()
        {
            // Arrange
            var source = new ArrayList { 1, 2, 0, 3 };

            // Act
            var result = source.CastSequence<int>().ToList();

            // Assert
            result.Should().HaveCount(4);
            result.Should().ContainInOrder(1, 2, 0, 3);
        }

        #endregion

        #region CastTo<T> Method Tests

        [Fact]
        public void Cast_With_Null_Should_Return_Default()
        {
            // Arrange
            object? item = null;

            // Act & Assert
            var action = () => item!.CastTo<string>();
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void CastTo_With_Compatible_Type_Should_Return_Casted_Value()
        {
            // Arrange
            object item = "test string";

            // Act
            var result = item.CastTo<string>();

            // Assert
            result.Should().Be("test string");
        }

        [Fact]
        public void CastTo_With_Incompatible_Type_Should_Throw_InvalidCastException()
        {
            // Arrange
            object item = "test string";

            // Act & Assert
            var action = () => item.CastTo<int>();
            action.Should().Throw<InvalidCastException>()
                .WithMessage("Cannot cast from type 'String' to 'Int32'");
        }

        [Fact]
        public void CastTo_With_Value_Type_Should_Return_Correct_Value()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.CastTo<int>();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void CastTo_With_Inheritance_Should_Return_Base_Type()
        {
            // Arrange
            object item = new List<string>();

            // Act
            var result = item.CastTo<IEnumerable>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        #endregion

        #region CastSequenceStrict<TSource, TTarget> Method Tests

        [Fact]
        public void CastSequenceStrict_With_Null_Should_Return_Empty_Sequence()
        {
            // Arrange
            IEnumerable<object>? source = null;

            // Act & Assert
            var action = () => source!.CastSequenceStrict<object, string>().ToList();
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void CastSequenceStrict_With_Compatible_Types_Should_Return_Casted_Items()
        {
            // Arrange
            var source = new List<object> { "test1", "test2", "test3" };

            // Act
            var result = source.CastSequenceStrict<object, string>().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain("test1", "test2", "test3");
        }

        [Fact]
        public void CastSequenceStrict_With_Incompatible_Type_Should_Throw_InvalidCastException()
        {
            // Arrange
            var source = new List<object> { "test1", 42, "test2" };

            // Act & Assert
            var action = () => source.CastSequenceStrict<object, string>().ToList();
            action.Should().Throw<InvalidCastException>();
        }

        [Fact]
        public void CastSequenceStrict_With_Null_Items_Should_Skip_Null_Items()
        {
            // Arrange
            var source = new List<object?> { "test1", null, "test2" };

            // Act & Assert
            var action = () => source!.CastSequenceStrict<object, string>().ToList();
            action.Should().Throw<InvalidCastException>();
        }

        [Fact]
        public void CastSequenceStrict_With_Inheritance_Should_Cast_Derived_Types()
        {
            // Arrange
            var source = new List<List<string>> { new(), new() };

            // Act
            var result = source.CastSequenceStrict<List<string>, IEnumerable>().ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllBeAssignableTo<IEnumerable>();
        }

        #endregion

        #region CreateInstance Method Tests

        [Fact]
        public void CreateInstance_With_Null_Type_Should_Return_Null()
        {
            // Arrange
            Type? type = null;

            // Act & Assert
            var action = () => type!.CreateInstance();
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void CreateInstance_With_Valid_Type_Should_Create_Instance()
        {
            // Arrange
            Type type = typeof(string);

            // Act
            var result = type.CreateInstance();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
        }

        [Fact]
        public void CreateInstance_With_Value_Type_Should_Create_Default_Instance()
        {
            // Arrange
            Type type = typeof(int);

            // Act
            var result = type.CreateInstance();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<int>();
            result.Should().Be(0);
        }

        [Fact]
        public void New_With_Complex_Type_Should_Create_Instance()
        {
            // Arrange
            Type type = typeof(List<string>);

            // Act
            var result = type.CreateInstance();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        #endregion

        #region With<T> and WithIf<T> Method Tests

        [Fact]
        public void With_Should_Execute_Action_And_Return_Object()
        {
            // Arrange
            var list = new List<string>();
            var actionExecuted = false;

            // Act
            var result = list.With(l =>
            {
                l!.Add("test");
                actionExecuted = true;
            });

            // Assert
            result.Should().BeSameAs(list);
            result.Should().Contain("test");
            actionExecuted.Should().BeTrue();
        }

        [Fact]
        public void WithIf_Should_Execute_Action_If_Condition_Is_True()
        {
            // Arrange
            var list = new List<string>();
            var actionExecuted = false;

            // Act
            var result = list.WithIf(false, l =>
            {
                l!.Add("test");
                actionExecuted = true;
            });

            // Assert
            result.Should().BeSameAs(list);
            result.Should().HaveCount(0);
            actionExecuted.Should().BeFalse();
        }

        [Fact]
        public void WithIf_Should_Not_Execute_Action_If_Condition_Is_False()
        {
            // Arrange
            var list = new List<string>();
            var actionExecuted = false;

            // Act
            var result = list.WithIf(true, l =>
            {
                l!.Add("test");
                actionExecuted = true;
            });

            // Assert
            result.Should().BeSameAs(list);
            result.Should().Contain("test");
            actionExecuted.Should().BeTrue();
        }

        [Fact]
        public void With_Should_Allow_Null_Object()
        {
            // Arrange
            List<string>? list = null;
            var actionExecuted = false;

            // Act & Assert
            var result = list.With((value) =>
            {
                var count = value is not null ? value!.Count : -1;
                actionExecuted = true;
            });

            // Assert
            result.Should().BeNull();
            actionExecuted.Should().BeTrue();
        }

        [Fact]
        public void With_Should_Allow_Method_Chaining()
        {
            // Arrange
            var list = new List<string>();

            // Act
            var result = list
                .With(l => l!.Add("first"))
                .With(l => l!.Add("second"));

            // Assert
            result.Should().BeSameAs(list);
            result.Should().HaveCount(2);
            result.Should().Contain("first", "second");
        }

        #endregion

        #region Using<T> Action and Func Method Tests

        [Fact]
        public void Using_Action_Should_Execute_Action_And_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable();
            var actionExecuted = false;

            // Act
            disposable.Using(d =>
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
        public void Using_Action_Should_Dispose_Even_On_Exception()
        {
            // Arrange
            var disposable = new TestDisposable();

            // Act & Assert
            var action = () => disposable.Using(d =>
            {
                throw new InvalidOperationException("Test exception");
            });

            action.Should().Throw<InvalidOperationException>();
            disposable.IsDisposed.Should().BeTrue();
        }

        #endregion

        #region UsingEach<T> Sequence Method Tests

        [Fact]
        public void UsingEach_Sequence_Should_Execute_Action_For_Each_And_Dispose_All()
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
            disposables.UsingEach(d =>
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
        public void UsingEach_Sequence_Should_Dispose_All_Even_On_Exception()
        {
            // Arrange
            var disposables = new List<TestDisposable>
            {
                new TestDisposable { Value = "first" },
                new TestDisposable { Value = "second" },
                new TestDisposable { Value = "third" }
            };

            // Act & Assert
            var action = () => disposables.UsingEach(d =>
            {
                if (d.Value == "second")
                    throw new InvalidOperationException("Test exception");
            });

            action.Should().Throw<InvalidOperationException>();
            
            // First item should be disposed, others might not be due to exception
            disposables[0].IsDisposed.Should().BeTrue();
        }

        #endregion

        #region Using<TDisposable, TResult> Func Method Tests

        [Fact]
        public void Using_Func_Should_Execute_Func_Return_Result_And_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable { Value = "test" };

            // Act
            var result = disposable.Using(d => d.Value.ToUpper());

            // Assert
            result.Should().Be("TEST");
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void Using_Func_Should_Dispose_Even_On_Exception()
        {
            // Arrange
            var disposable = new TestDisposable();
            var funcExecuted = false;
            string? result = null;

            // Act
            var action = () => result = disposable.Using(d =>
            {
                throw new InvalidOperationException("Test exception");
#pragma warning disable CS0162 // Unreachable code detected
                funcExecuted = true;
                d.Value = "modified";
                return d.Value;
#pragma warning restore CS0162 // Unreachable code detected
            });

            // Assert
            action.Should().Throw<InvalidOperationException>();
            funcExecuted.Should().BeFalse();
            disposable.IsDisposed.Should().BeTrue();
            result.Should().BeNull();
        }

        #endregion

        #region Using<TDisposable, TFunc2, TResult> Method Tests

        [Fact]
        public void Using_Func_With_Second_Parameter_Should_Execute_And_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable { Value = "test" };
            var secondParam = "_suffix";

            // Act
            var result = disposable.Using(secondParam, (d, suffix) => d.Value + suffix);

            // Assert
            result.Should().Be("test_suffix");
            disposable.IsDisposed.Should().BeTrue();
        }

        #endregion

        #region SafeDispose Method Tests

        [Fact]
        public void DisposeIfNecessary_With_Null_Should_Not_Throw()
        {
            // Arrange
            object? obj = null;

            // Act & Assert
            var action = () => obj!.SafeDispose();
            action.Should().NotThrow();
        }

        [Fact]
        public void SafeDispose_With_Disposable_Should_Dispose()
        {
            // Arrange
            var disposable = new TestDisposable();

            // Act
            ((object)disposable).SafeDispose();

            // Assert
            disposable.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void SafeDispose_With_Non_Disposable_Should_Not_Throw()
        {
            // Arrange
            object obj = "test string";

            // Act & Assert
            var action = () => obj.SafeDispose();
            action.Should().NotThrow();
        }

        #endregion

        #region SafeDisposeAll Method Tests

        [Fact]
        public void SafeDisposeAll_With_Null_Should_Not_Throw()
        {
            // Arrange
            IEnumerable? sequence = null;

            // Act & Assert
            var action = () => sequence!.SafeDisposeAll();
            action.Should().NotThrow();
        }

        [Fact]
        public void SafeDisposeAll_With_Disposable_Elements_Should_Dispose_All()
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
            disposables.SafeDisposeAll();

            // Assert
            ((TestDisposable)disposables[0]!).IsDisposed.Should().BeTrue();
            ((TestDisposable)disposables[1]!).IsDisposed.Should().BeTrue();
            ((TestDisposable)disposables[3]!).IsDisposed.Should().BeTrue();
        }

        #endregion

        #region IsNull<T> Method Tests

        [Fact]
        public void IsNull_With_Null_Reference_Type_Should_Return_True()
        {
            // Arrange
            string? value = null;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNull_With_Non_Null_Reference_Type_Should_Return_False()
        {
            // Arrange
            string value = "test";

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsNull_With_Value_Type_Should_Return_False()
        {
            // Arrange
            int value = 42;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsNull_With_Default_Value_Type_Should_Return_False()
        {
            // Arrange
            int value = 0;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsNull_With_Nullable_Value_Type_Null_Should_Return_True()
        {
            // Arrange
            int? value = null;

            // Act
            var result = value.IsNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNull_With_Nullable_Value_Type_Not_Null_Should_Return_False()
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
        public void IsNotNull_With_Null_Reference_Type_Should_Return_False()
        {
            // Arrange
            string? value = null;

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsNotNull_With_Non_Null_Reference_Type_Should_Return_True()
        {
            // Arrange
            string value = "test";

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNotNull_With_Value_Type_Should_Return_True()
        {
            // Arrange
            int value = 42;

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNotNull_With_Nullable_Value_Type_Null_Should_Return_False()
        {
            // Arrange
            int? value = null;

            // Act
            var result = value.IsNotNull();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsNotNull_With_Nullable_Value_Type_Not_Null_Should_Return_True()
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
        public void IsDefaultValue_With_Null_Should_Return_True()
        {
            // Arrange
            object? value = null;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDefaultValue_With_Reference_Type_Default_Should_Return_False()
        {
            // Arrange
            object value = "test";

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsDefaultValue_With_Value_Type_Default_Should_Return_True()
        {
            // Arrange
            object value = 0;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDefaultValue_With_Value_Type_Non_Default_Should_Return_False()
        {
            // Arrange
            object value = 42;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsDefaultValue_With_Boolean_Default_Should_Return_True()
        {
            // Arrange
            object value = false;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDefaultValue_With_Boolean_Non_Default_Should_Return_False()
        {
            // Arrange
            object value = true;

            // Act
            var result = value.IsDefaultValue();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsDefaultOrEmpty Method Tests

        [Fact]
        public void IsDefaultOrEmpty_With_Null_Should_Return_True()
        {
            // Arrange
            object? value = null;

            // Act
            var result = value.IsDefaultOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDefaultOrEmpty_With_Empty_String_Should_Return_True()
        {
            // Arrange
            object value = string.Empty;

            // Act
            var result = value.IsDefaultOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDefaultOrEmpty_With_Non_Empty_String_Should_Return_False()
        {
            // Arrange
            object value = "test";

            // Act
            var result = value.IsDefaultOrEmpty();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsDefaultOrEmpty_With_Value_Type_Default_Should_Return_True()
        {
            // Arrange
            object value = 0;

            // Act
            var result = value.IsDefaultOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsDefaultOrEmpty_With_Value_Type_Non_Default_Should_Return_False()
        {
            // Arrange
            object value = 42;

            // Act
            var result = value.IsDefaultOrEmpty();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region TryCast Tests

        #region TryCast Happy Path Tests

        [Fact]
        public void TryCast_WithValidCast_ShouldReturnTrueAndCorrectResult()
        {
            // Arrange
            object stringValue = "Hello World";

            // Act
            bool success = stringValue.TryCast<string>(out var result);

            // Assert
            success.Should().BeTrue();
            result.Should().Be("Hello World");
        }

        [Fact]
        public void TryCast_WithValueType_ShouldReturnTrueAndCorrectResult()
        {
            // Arrange
            object intValue = 42;

            // Act
            bool success = intValue.TryCast<int>(out var result);

            // Assert
            success.Should().BeTrue();
            result.Should().Be(42);
        }

        [Fact]
        public void TryCast_WithInheritance_ShouldWorkCorrectly()
        {
            // Arrange
            object derivedObj = new DerivedClass { Name = "TestDerived" };

            // Act
            bool successBase = derivedObj.TryCast<BaseClass>(out var baseResult);
            bool successDerived = derivedObj.TryCast<DerivedClass>(out var derivedResult);

            // Assert
            successBase.Should().BeTrue();
            baseResult.Should().NotBeNull();
            baseResult!.Name.Should().Be("TestDerived");

            successDerived.Should().BeTrue();
            derivedResult.Should().NotBeNull();
            derivedResult!.DerivedProperty.Should().Be(42);
        }

        [Fact]
        public void TryCast_WithBoxedValueType_ShouldWorkCorrectly()
        {
            // Arrange
            object boxedDouble = 3.14;

            // Act
            bool success = boxedDouble.TryCast<double>(out var result);

            // Assert
            success.Should().BeTrue();
            result.Should().Be(3.14);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TryCast_WithBoolean_ShouldWorkCorrectly(bool expectedValue)
        {
            // Arrange
            object boolValue = expectedValue;

            // Act
            bool success = boolValue.TryCast<bool>(out var result);

            // Assert
            success.Should().BeTrue();
            result.Should().Be(expectedValue);
        }

        #endregion

        #region TryCast Failure Cases

        [Fact]
        public void TryCast_WithInvalidCast_ShouldReturnFalseAndDefaultResult()
        {
            // Arrange
            object stringValue = "Not a number";

            // Act
            bool success = stringValue.TryCast<int>(out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(default(int)); // Should be 0
        }

        [Fact]
        public void TryCast_WithNullObject_ShouldReturnFalseAndDefaultResult()
        {
            // Arrange
            object? nullValue = null;

            // Act
            bool success = nullValue.TryCast<string>(out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryCast_WithUnrelatedTypes_ShouldReturnFalse()
        {
            // Arrange
            object unrelatedObj = new UnrelatedClass();

            // Act
            bool success = unrelatedObj.TryCast<DerivedClass>(out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryCast_WithWrongValueType_ShouldReturnFalse()
        {
            // Arrange
            object intValue = 42;

            // Act
            bool success = intValue.TryCast<double>(out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().Be(0.0); // Default double value
        }

        [Fact]
        public void TryCast_WithBaseToDeriveCast_ShouldReturnFalseWhenNotActuallyDerived()
        {
            // Arrange
            object baseObj = new BaseClass { Name = "ActualBase" };

            // Act
            bool success = baseObj.TryCast<DerivedClass>(out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        #endregion

        #region TryCast Edge Cases

        [Fact]
        public void TryCast_WithNullableValueType_ShouldWorkCorrectly()
        {
            // Arrange
            object nullableInt = (int?)42;

            // Act
            bool successNullable = nullableInt.TryCast<int?>(out var nullableResult);
            bool successInt = nullableInt.TryCast<int>(out var intResult);

            // Assert
            successNullable.Should().BeTrue();
            nullableResult.Should().Be(42);

            successInt.Should().BeTrue();
            intResult.Should().Be(42);
        }

        [Fact]
        public void TryCast_WithInterfaceCast_ShouldWorkCorrectly()
        {
            // Arrange
            object list = new List<string> { "item1", "item2" };

            // Act
            bool success = list.TryCast<IList<string>>(out var result);

            // Assert
            success.Should().BeTrue();
            result.Should().NotBeNull();
            result!.Count.Should().Be(2);
        }

        #endregion

        #endregion

        #region SafeSelect Tests

        #region SafeSelect Happy Path Tests

        [Fact]
        public void SafeSelect_WithValidObject_ShouldReturnCorrectResult()
        {
            // Arrange
            var person = new Person { Name = "John Doe", Age = 30 };

            // Act
            var name = person.SafeSelect(p => p.Name);
            var age = person.SafeSelect(p => p.Age);

            // Assert
            name.Should().Be("John Doe");
            age.Should().Be(30);
        }

        [Fact]
        public void SafeSelect_WithNestedPropertyAccess_ShouldReturnCorrectResult()
        {
            // Arrange
            var person = new Person
            {
                Name = "Jane Doe",
                Address = new Address
                {
                    City = "Berlin",
                    Country = new Country { Name = "Germany", Code = "DE" }
                }
            };

            // Act
            var city = person.SafeSelect(p => p.Address?.City);
            var countryName = person.SafeSelect(p => p.Address?.Country?.Name);
            var countryCode = person.SafeSelect(p => p.Address?.Country?.Code);

            // Assert
            city.Should().Be("Berlin");
            countryName.Should().Be("Germany");
            countryCode.Should().Be("DE");
        }

        [Fact]
        public void SafeSelect_WithComplexSelector_ShouldReturnCorrectResult()
        {
            // Arrange
            var person = new Person
            {
                Name = "John",
                Phones = new List<Phone>
                {
                    new Phone { Number = "123-456-7890", Type = PhoneType.Mobile },
                    new Phone { Number = "098-765-4321", Type = PhoneType.Home }
                }
            };

            // Act
            var phoneCount = person.SafeSelect(p => p.Phones?.Count);
            var firstPhoneNumber = person.SafeSelect(p => p.Phones?.FirstOrDefault()?.Number);
            var hasWorkPhone = person.SafeSelect(p => p.Phones?.Any(ph => ph.Type == PhoneType.Work));

            // Assert
            phoneCount.Should().Be(2);
            firstPhoneNumber.Should().Be("123-456-7890");
            hasWorkPhone.Should().Be(false);
        }

        [Fact]
        public void SafeSelect_WithValueTypeResult_ShouldReturnCorrectValue()
        {
            // Arrange
            var person = new Person { Age = 25 };

            // Act
            var age = person.SafeSelect(p => p.Age);
            var ageDoubled = person.SafeSelect(p => p.Age * 2);

            // Assert
            age.Should().Be(25);
            ageDoubled.Should().Be(50);
        }

        #endregion

        #region SafeSelect Null Handling Tests

        [Fact]
        public void SafeSelect_WithNullObject_ShouldReturnDefault()
        {
            // Arrange
            Person? nullPerson = null;

            // Act
            var name = nullPerson.SafeSelect(p => p.Name);
            var age = nullPerson.SafeSelect(p => p.Age);

            // Assert
            name.Should().BeNull();
            age.Should().Be(0); // Default for int
        }

        [Fact]
        public void SafeSelect_WithNullNestedProperty_ShouldReturnDefault()
        {
            // Arrange
            var person = new Person { Name = "John" }; // Address is null

            // Act
            var city = person.SafeSelect(p => p.Address?.City);
            var zipCode = person.SafeSelect(p => p.Address?.ZipCode);
            var countryName = person.SafeSelect(p => p.Address?.Country?.Name);

            // Assert
            city.Should().BeNull();
            zipCode.Should().BeNull(); // Nullable because of null propagation
            countryName.Should().BeNull();
        }

        [Fact]
        public void SafeSelect_WithPartiallyNullPath_ShouldReturnDefault()
        {
            // Arrange
            var person = new Person
            {
                Name = "Jane",
                Address = new Address { City = "Munich" } // Country is null
            };

            // Act
            var city = person.SafeSelect(p => p.Address?.City);
            var countryName = person.SafeSelect(p => p.Address?.Country?.Name);

            // Assert
            city.Should().Be("Munich");
            countryName.Should().BeNull();
        }

        [Fact]
        public void SafeSelect_WithNullCollection_ShouldReturnDefault()
        {
            // Arrange
            var person = new Person { Name = "Bob" }; // Phones is null

            // Act
            var phoneCount = person.SafeSelect(p => p.Phones?.Count);
            var firstPhone = person.SafeSelect(p => p.Phones?.FirstOrDefault());

            // Assert
            phoneCount.Should().BeNull();
            firstPhone.Should().BeNull();
        }

        #endregion

        #region SafeSelect Exception Handling Tests

        [Fact]
        public void SafeSelect_WithNullSelector_ShouldThrowArgumentNullException()
        {
            // Arrange
            var person = new Person { Name = "Test" };
            Func<Person, string?>? nullSelector = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                person.SafeSelect(nullSelector!));

            exception.ParamName.Should().Be("selector");
        }

        [Fact]
        public void SafeSelect_WhenSelectorThrowsException_ShouldPropagateException()
        {
            // Arrange
            var person = new Person { Name = "Test" };
            var expectedException = new InvalidOperationException("Selector failed");

            // Act & Assert
            var thrownException = Assert.Throws<InvalidOperationException>(() =>
                person.SafeSelect<Person, string>(p => throw expectedException));

            thrownException.Should().BeSameAs(expectedException);
        }

        #endregion

        #region SafeSelect Edge Cases

        [Fact]
        public void SafeSelect_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var person = new Person { Name = "" };

            // Act
            var name = person.SafeSelect(p => p.Name);
            var nameLength = person.SafeSelect(p => p.Name?.Length);

            // Assert
            name.Should().Be("");
            nameLength.Should().Be(0);
        }

        [Fact]
        public void SafeSelect_WithComplexTransformation_ShouldWorkCorrectly()
        {
            // Arrange
            var person = new Person
            {
                Name = "John Doe",
                Address = new Address { City = "Berlin", ZipCode = 12345 }
            };

            // Act
            var addressSummary = person.SafeSelect(p =>
                p.Address != null
                    ? $"{p.Address.City} ({p.Address.ZipCode})"
                    : "No address");

            var fullInfo = person.SafeSelect(p => new
            {
                PersonName = p.Name,
                Location = p.Address?.City ?? "Unknown",
                HasAddress = p.Address != null
            });

            // Assert
            addressSummary.Should().Be("Berlin (12345)");
            fullInfo.Should().NotBeNull();
            fullInfo!.PersonName.Should().Be("John Doe");
            fullInfo.Location.Should().Be("Berlin");
            fullInfo.HasAddress.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("John")]
        [InlineData("A very long name with special characters הצü")]
        public void SafeSelect_WithVariousStringValues_ShouldHandleCorrectly(string? nameValue)
        {
            // Arrange
            var person = new Person { Name = nameValue };

            // Act
            var name = person.SafeSelect(p => p.Name);
            var nameUpper = person.SafeSelect(p => p.Name?.ToUpper());
            var nameLength = person.SafeSelect(p => p.Name?.Length);

            // Assert
            name.Should().Be(nameValue);
            nameUpper.Should().Be(nameValue?.ToUpper());
            nameLength.Should().Be(nameValue?.Length);
        }

        [Fact]
        public void SafeSelect_WithRecursiveStructure_ShouldWorkCorrectly()
        {
            // Arrange
            var person = new Person
            {
                Name = "Test",
                Address = new Address
                {
                    Country = new Country
                    {
                        Name = "Germany"
                    }
                }
            };

            // Act
            var deepProperty = person.SafeSelect(p =>
                p.Address?.Country?.Name?.ToUpper()?.Substring(0, 3));

            // Assert
            deepProperty.Should().Be("GER");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void SafeSelect_CombinedWithTryCast_ShouldWorkTogether()
        {
            // Arrange
            object personObj = new Person
            {
                Name = "Integration Test",
                Address = new Address { City = "Hamburg" }
            };

            // Act
            bool castSuccess = personObj.TryCast<Person>(out var person);
            var city = person.SafeSelect(p => p.Address?.City);

            // Assert
            castSuccess.Should().BeTrue();
            city.Should().Be("Hamburg");
        }

        [Fact]
        public void SafeSelect_WithMultipleChainingCalls_ShouldWorkCorrectly()
        {
            // Arrange
            var person = new Person
            {
                Name = "Chain Test",
                Address = new Address
                {
                    City = "Berlin",
                    Country = new Country { Code = "DE" }
                }
            };

            // Act
            var result = person
                .SafeSelect(p => p.Address)
                ?.SafeSelect(a => a.Country)
                ?.SafeSelect(c => c.Code);

            // Assert
            result.Should().Be("DE");
        }

        #endregion

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

        /// <summary>
        /// Test class hierarchy for inheritance testing
        /// </summary>
        private class BaseClass
        {
            public virtual string Name { get; set; } = "Base";
        }

        private class DerivedClass : BaseClass
        {
            public override string Name { get; set; } = "Derived";
            public int DerivedProperty { get; set; } = 42;
        }

        private class UnrelatedClass
        {
            public string Value { get; set; } = "Unrelated";
        }

        /// <summary>
        /// Test classes for SafeSelect navigation testing
        /// </summary>
        private class Person
        {
            public string? Name { get; set; }
            public Address? Address { get; set; }
            public List<Phone>? Phones { get; set; }
            public int Age { get; set; }
        }

        private class Address
        {
            public string? Street { get; set; }
            public string? City { get; set; }
            public Country? Country { get; set; }
            public int ZipCode { get; set; }
        }

        private class Country
        {
            public string? Name { get; set; }
            public string? Code { get; set; }
        }

        private class Phone
        {
            public string? Number { get; set; }
            public PhoneType Type { get; set; }
        }

        private enum PhoneType
        {
            Mobile,
            Home,
            Work
        }


        #endregion
    }
}