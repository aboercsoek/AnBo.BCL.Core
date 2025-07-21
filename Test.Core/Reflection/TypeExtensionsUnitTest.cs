//--------------------------------------------------------------------------
// File:    TypeExtensionsUnitTest.cs
// Content:	Unit tests for TypeExtensions class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Numerics;
using System.Text.Json.Serialization;

namespace AnBo.Test;

public class TypeExtensionsUnitTest
{
    #region QuoteAssemblyQualifiedNameIfNeeded Tests

    [Fact]
    public void QuoteAssemblyQualifiedNameIfNeeded_With_Null_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        var exception = Assert.Throws<ArgNullException>(() => nullType!.QuoteAssemblyQualifiedNameIfNeeded());
        exception.ParamName.Should().Be("type");
    }

    [Fact]
    public void QuoteAssemblyQualifiedNameIfNeeded_With_Simple_Type_Should_Return_Unquoted_Name()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = type.QuoteAssemblyQualifiedNameIfNeeded();

        // Assert
        result.Should().Contain(type.AssemblyQualifiedName);
        result.Should().StartWith("“");
        result.Should().EndWith("”");
    }

    [Fact]
    public void QuoteAssemblyQualifiedNameIfNeeded_With_Generic_Type_Should_Handle_Correctly()
    {
        // Arrange
        var type = typeof(List<string>);

        // Act
        var result = type.QuoteAssemblyQualifiedNameIfNeeded();

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region GetTypeName Tests

    [Fact]
    public void GetTypeName_With_Null_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        var exception = Assert.Throws<ArgNullException>(() => nullType!.GetTypeName());
        exception.ParamName.Should().Be("type");
    }

    [Fact]
    public void GetTypeName_With_Simple_Type_Should_Return_Simple_Name()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = type.GetTypeName();

        // Assert
        result.Should().Be("String");
    }

    [Fact]
    public void GetTypeName_With_Generic_Type_Should_Return_Friendly_Name()
    {
        // Arrange
        var type = typeof(List<string>);

        // Act
        var result = type.GetTypeName();

        // Assert
        result.Should().Be("List[of String]");
    }

    [Fact]
    public void GetTypeName_With_Multiple_Generic_Arguments_Should_Return_Friendly_Name()
    {
        // Arrange
        var type = typeof(Dictionary<string, int>);

        // Act
        var result = type.GetTypeName();

        // Assert
        result.Should().Be("Dictionary[of String,Int32]");
    }

    [Fact]
    public void GetTypeName_With_Nested_Generic_Type_Should_Return_Friendly_Name()
    {
        // Arrange
        var type = typeof(List<Dictionary<string, int>>);

        // Act
        var result = type.GetTypeName();

        // Assert
        result.Should().Be("List[of Dictionary[of String,Int32]]");
    }

    #endregion

    #region GetAnyField Tests

    [Fact]
    public void GetAnyField_With_Null_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        var exception = Assert.Throws<ArgNullException>(() => nullType!.GetAnyField("fieldName"));
        exception.ParamName.Should().Be("type");
    }

    [Fact]
    public void GetAnyField_With_Null_FieldName_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act & Assert
        var exception = Assert.Throws<ArgNullException>(() => type.GetAnyField(null!));
        exception.ParamName.Should().Be("fieldName");
    }

    [Fact]
    public void GetAnyField_With_Empty_FieldName_Should_Throw_ArgumentException()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act & Assert
        var exception = Assert.Throws<ArgEmptyException>(() => type.GetAnyField(""));
        exception.ParamName.Should().Be("fieldName");
    }

    [Fact]
    public void GetAnyField_With_Existing_Public_Field_Should_Return_FieldInfo()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act
        var result = type.GetAnyField("PublicField");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("PublicField");
        result.FieldType.Should().Be(typeof(string));
    }

    [Fact]
    public void GetAnyField_With_Existing_Private_Field_Should_Return_FieldInfo()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act
        var result = type.GetAnyField("privateField");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("privateField");
    }

    [Fact]
    public void GetAnyField_With_Non_Existing_Field_Should_Return_Null()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act
        var result = type.GetAnyField("NonExistingField");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetAnyField_With_Case_Insensitive_Match_Should_Return_FieldInfo()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act
        var result = type.GetAnyField("PUBLICFIELD");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("PublicField");
    }

    [Fact]
    public void GetAnyField_With_Inherited_Field_Should_Return_FieldInfo()
    {
        // Arrange
        var type = typeof(DerivedTestClass);

        // Act
        var result = type.GetAnyField("baseField");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("baseField");
    }

    #endregion

    #region GetAllFields Tests

    [Fact]
    public void GetAllFields_With_Null_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        var exception = Assert.Throws<ArgNullException>(() => nullType!.GetAllFields().ToList());
        exception.ParamName.Should().Be("type");
    }

    [Fact]
    public void GetAllFields_With_Simple_Class_Should_Return_All_Fields()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act
        var result = type.GetAllFields().ToList();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain(f => f.Name == "PublicField");
        result.Should().Contain(f => f.Name == "privateField");
        result.Should().Contain(f => f.Name == "StaticField");
    }

    [Fact]
    public void GetAllFields_With_Derived_Class_Should_Return_All_Fields_Including_Base()
    {
        // Arrange
        var type = typeof(DerivedTestClass);

        // Act
        var result = type.GetAllFields().ToList();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain(f => f.Name == "derivedField");
        result.Should().Contain(f => f.Name == "baseField");
    }

    [Fact]
    public void GetAllFields_With_Object_Type_Should_Return_Empty()
    {
        // Arrange
        var type = typeof(object);

        // Act
        var result = type.GetAllFields().ToList();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region ImplementsInterface Tests

    [Fact]
    public void ImplementsInterface_With_Null_Type_Should_Throw_ArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        var exception = Assert.Throws<ArgNullException>(() => nullType!.ImplementsInterface<IDisposable>());
        exception.ParamName.Should().Be("type");
    }

    [Fact]
    public void ImplementsInterface_With_No_Interface_Type_Should_Return_False()
    {
        // Arrange
        var type = typeof(TestClassImplementingIDisposable);

        // Act
        var result = type.ImplementsInterface<List<int>>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ImplementsInterface_With_Type_Implementing_Interface_Should_Return_True()
    {
        // Arrange
        var type = typeof(TestClassImplementingIDisposable);

        // Act
        var result = type.ImplementsInterface<IDisposable>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ImplementsInterface_With_Type_Not_Implementing_Interface_Should_Return_False()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act
        var result = type.ImplementsInterface<IDisposable>();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ImplementsInterface_With_Interface_Type_Should_Return_True()
    {
        // Arrange
        var type = typeof(IDisposable);

        // Act
        var result = type.ImplementsInterface<IDisposable>();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ImplementsInterface_With_Generic_Interface_Should_Return_True()
    {
        // Arrange
        var type = typeof(List<string>);

        // Act
        var result = type.ImplementsInterface<IEnumerable<string>>();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region HasRequiredMembers Tests

    [Fact]
    public void HasRequiredMembers_With_Null_Type_Should_Return_False()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        var action = () => nullType!.HasRequiredMembers();
        action.Should().Throw<ArgNullException>();
    }

    [Fact]
    public void HasRequiredMembers_With_Type_Having_Required_Members_Should_Return_True()
    {
        // Arrange
        var type = typeof(TestClassWithRequiredMembers);

        // Act
        var result = type.HasRequiredMembers();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasRequiredMembers_With_Type_Without_Required_Members_Should_Return_False()
    {
        // Arrange
        var type = typeof(TestClassWithFields);

        // Act
        var result = type.HasRequiredMembers();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsNullableType Tests

    [Fact]
    public void IsNullableType_With_Nullable_Int_Should_Return_True()
    {
        // Arrange
        var type = typeof(int?);

        // Act
        var result = type.IsNullableType();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullableType_With_Nullable_DateTime_Should_Return_True()
    {
        // Arrange
        var type = typeof(DateTime?);

        // Act
        var result = type.IsNullableType();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullableType_With_Non_Nullable_Int_Should_Return_False()
    {
        // Arrange
        var type = typeof(int);

        // Act
        var result = type.IsNullableType();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsNullableType_With_Reference_Type_Should_Return_False()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = type.IsNullableType();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsNullableType_With_Generic_List_Should_Return_False()
    {
        // Arrange
        var type = typeof(List<int>);

        // Act
        var result = type.IsNullableType();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsOpenGenericType Tests

    [Fact]
    public void IsOpenGenericType_With_Null_Type_Should_Return_False()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        var action = () => nullType!.IsOpenGenericType();
        action.Should().Throw<ArgNullException>();
    }

    [Fact]
    public void IsOpenGenericType_With_Open_Generic_List_Should_Return_True()
    {
        // Arrange
        var type = typeof(List<>);

        // Act
        var result = type.IsOpenGenericType();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsOpenGenericType_With_Closed_Generic_List_Should_Return_False()
    {
        // Arrange
        var type = typeof(List<int>);

        // Act
        var result = type.IsOpenGenericType();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsOpenGenericType_With_Non_Generic_Type_Should_Return_False()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = type.IsOpenGenericType();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsOpenGenericType_With_Open_Generic_Dictionary_Should_Return_True()
    {
        // Arrange
        var type = typeof(Dictionary<,>);

        // Act
        var result = type.IsOpenGenericType();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetDefaultValue Tests

    [Fact]
    public void GetDefaultValue_With_Reference_Type_Should_Return_Null()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = type.GetDefaultValue();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetDefaultValue_With_Value_Type_Int_Should_Return_Zero()
    {
        // Arrange
        var type = typeof(int);

        // Act
        var result = type.GetDefaultValue();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetDefaultValue_With_Value_Type_Bool_Should_Return_False()
    {
        // Arrange
        var type = typeof(bool);

        // Act
        var result = type.GetDefaultValue();

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void GetDefaultValue_With_DateTime_Should_Return_MinValue()
    {
        // Arrange
        var type = typeof(DateTime);

        // Act
        var result = type.GetDefaultValue();

        // Assert
        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void GetDefaultValue_With_Nullable_Type_Should_Return_Null()
    {
        // Arrange
        var type = typeof(int?);

        // Act
        var result = type.GetDefaultValue();

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region IsDefaultValue Tests

    [Fact]
    public void IsDefaultValue_With_Null_Value_Should_Return_True()
    {
        // Arrange
        var type = typeof(string);
        object? value = null;

        // Act
        var result = type.IsDefaultValue(value!);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefaultValue_With_Nullable_Type_And_Null_Value_Should_Return_True()
    {
        // Arrange
        var type = typeof(int?);
        int? value = null;

        // Act
        var result = type.IsDefaultValue(value!);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefaultValue_With_Default_Int_Should_Return_True()
    {
        // Arrange
        var type = typeof(int);
        var value = 0;

        // Act
        var result = type.IsDefaultValue(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefaultValue_With_Non_Default_Int_Should_Return_False()
    {
        // Arrange
        var type = typeof(int);
        var value = 42;

        // Act
        var result = type.IsDefaultValue(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDefaultValue_With_Default_Bool_Should_Return_True()
    {
        // Arrange
        var type = typeof(bool);
        var value = false;

        // Act
        var result = type.IsDefaultValue(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefaultValue_With_Non_Default_Bool_Should_Return_False()
    {
        // Arrange
        var type = typeof(bool);
        var value = true;

        // Act
        var result = type.IsDefaultValue(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDefaultValue_With_Non_Null_String_Should_Return_False()
    {
        // Arrange
        var type = typeof(string);
        var value = "test";

        // Act
        var result = type.IsDefaultValue(value);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsDefaultValueOrEmptyString Tests

    [Fact]
    public void IsDefaultValueOrEmptyString_With_Null_Should_Return_True()
    {
        // Arrange
        var type = typeof(string);
        object? value = null;

        // Act
        var result = type.IsDefaultValueOrEmptyString(value!);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefaultValueOrEmptyString_With_Empty_String_Should_Return_True()
    {
        // Arrange
        var type = typeof(string);
        var value = "";

        // Act
        var result = type.IsDefaultValueOrEmptyString(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefaultValueOrEmptyString_With_Non_Empty_String_Should_Return_False()
    {
        // Arrange
        var type = typeof(string);
        var value = "test";

        // Act
        var result = type.IsDefaultValueOrEmptyString(value);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDefaultValueOrEmptyString_With_Default_Value_Type_Should_Return_True()
    {
        // Arrange
        var type = typeof(int);
        var value = 0;

        // Act
        var result = type.IsDefaultValueOrEmptyString(value);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefaultValueOrEmptyString_With_Non_Default_Value_Type_Should_Return_False()
    {
        // Arrange
        var type = typeof(int);
        var value = 42;

        // Act
        var result = type.IsDefaultValueOrEmptyString(value);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsJsonSerializable Tests

    [Fact]
    public void IsJsonSerializable_With_Simple_Value_Type_Should_Return_True()
    {
        // Arrange
        var type = typeof(int);

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsJsonSerializable_With_String_Type_Should_Return_True()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsJsonSerializable_With_Class_Having_Parameterless_Constructor_Should_Return_True()
    {
        // Arrange
        var type = typeof(TestClassWithParameterlessConstructor);

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsJsonSerializable_With_Class_Without_Parameterless_Constructor_Should_Return_False()
    {
        // Arrange
        var type = typeof(TestClassWithoutParameterlessConstructor);

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsJsonSerializable_With_Pointer_Type_Should_Return_False()
    {
        // Arrange
        var type = typeof(int).MakePointerType();

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsJsonSerializable_With_ByRef_Type_Should_Return_False()
    {
        // Arrange
        var type = typeof(int).MakeByRefType();

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsJsonSerializable_With_Nullable_Int32_Type_Should_Return_True()
    {
        // Arrange
        var type = typeof(int?);

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsJsonSerializable_With_Int32_Array_Type_Should_Return_True()
    {
        // Arrange
        var type = typeof(int[]);

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsJsonSerializable_With_Struct_Type_Should_Return_True()
    {
        // Arrange
        var type = typeof(BigInteger);

        // Act
        var result = type.IsJsonSerializable();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region DeepClone Tests

    [Fact]
    public void DeepClone_With_Null_Should_Return_Null()
    {
        // Arrange
        string? original = null;

        // Act
        var result = original.DeepClone();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void DeepClone_With_Simple_Object_Should_Create_Copy()
    {
        // Arrange
        var original = "test string";

        // Act
        var result = original.DeepClone();

        // Assert
        result.Should().Be("test string");
        result.Should().NotBeSameAs(original);
    }

    [Fact]
    public void DeepClone_With_Complex_Object_Should_Create_Deep_Copy()
    {
        // Arrange
        var original = new TestClassForCloning
        {
            Id = 1,
            Name = "Test",
            NestedObject = new NestedTestClassForCloning { Value = "Nested" }
        };

        // Act
        var result = original.DeepClone();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(original);
        result!.Id.Should().Be(original.Id);
        result.Name.Should().Be(original.Name);
        result.NestedObject.Should().NotBeSameAs(original.NestedObject);
        result.NestedObject!.Value.Should().Be(original.NestedObject.Value);
    }

    [Fact]
    public void DeepClone_With_Value_Type_Should_Work()
    {
        // Arrange
        int original = 42;

        // Act
        var result = original.DeepClone();

        // Assert
        result.Should().Be(42);
    }

    #endregion

    #region CanBeInstantiated Tests

    [Fact]
    public void CanBeInstantiated_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act
        Action act = () => nullType!.CanBeInstantiated();

        // Assert
        act.Should().Throw<ArgNullException>();
    }

    [Fact]
    public void CanBeInstantiated_WithConcreteClassWithParameterlessConstructor_ShouldReturnTrue()
    {
        // Arrange
        var type = typeof(TestClassWithParameterlessConstructor);

        // Act
        var result = type.CanBeInstantiated();

        // Assert
        result.Should().BeTrue("because concrete classes with parameterless constructors can be instantiated");
    }

    [Fact]
    public void CanBeInstantiated_WithConcreteClassWithoutParameterlessConstructor_ShouldReturnFalse()
    {
        // Arrange
        var type = typeof(TestClassWithoutParameterlessConstructor);

        // Act
        var result = type.CanBeInstantiated();

        // Assert
        result.Should().BeFalse("because classes without parameterless constructors cannot be instantiated via reflection");
    }

    [Fact]
    public void CanBeInstantiated_WithAbstractClass_ShouldReturnFalse()
    {
        // Arrange
        var type = typeof(AbstractTestClass);

        // Act
        var result = type.CanBeInstantiated();

        // Assert
        result.Should().BeFalse("because abstract classes cannot be instantiated");
    }

    [Fact]
    public void CanBeInstantiated_WithInterface_ShouldReturnFalse()
    {
        // Arrange
        var type = typeof(ITestInterface);

        // Act
        var result = type.CanBeInstantiated();

        // Assert
        result.Should().BeFalse("because interfaces cannot be instantiated");
    }

    [Fact]
    public void CanBeInstantiated_WithGenericTypeDefinition_ShouldReturnFalse()
    {
        // Arrange
        var type = typeof(List<>);

        // Act
        var result = type.CanBeInstantiated();

        // Assert
        result.Should().BeFalse("because generic type definitions cannot be instantiated");
    }

    [Fact]
    public void CanBeInstantiated_WithClosedGenericType_ShouldReturnTrue()
    {
        // Arrange
        var type = typeof(List<string>);

        // Act
        var result = type.CanBeInstantiated();

        // Assert
        result.Should().BeTrue("because closed generic types with parameterless constructors can be instantiated");
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(TestStruct))]
    public void CanBeInstantiated_WithValueTypesAndString_ShouldReturnTrue(Type type)
    {
        // Act
        var result = type.CanBeInstantiated();

        // Assert
        result.Should().BeTrue("because value types and string have implicit parameterless constructors");
    }

    #endregion

    #region GetUnderlyingType Tests

    [Fact]
    public void GetUnderlyingType_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act
        Action act = () => nullType!.GetUnderlyingType();

        // Assert
        act.Should().Throw<ArgNullException>();
    }

    [Theory]
    [InlineData(typeof(int?), typeof(int))]
    [InlineData(typeof(bool?), typeof(bool))]
    [InlineData(typeof(DateTime?), typeof(DateTime))]
    [InlineData(typeof(decimal?), typeof(decimal))]
    public void GetUnderlyingType_WithNullableTypes_ShouldReturnUnderlyingType(Type nullableType, Type expectedUnderlyingType)
    {
        // Act
        var result = nullableType.GetUnderlyingType();

        // Assert
        result.Should().Be(expectedUnderlyingType,
            "because nullable types should return their underlying non-nullable type");
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(TestClassWithFields))]
    public void GetUnderlyingType_WithNonNullableTypes_ShouldReturnSameType(Type type)
    {
        // Act
        var result = type.GetUnderlyingType();

        // Assert
        result.Should().Be(type,
            "because non-nullable types should return themselves");
    }

    #endregion

    #region Test Helper Classes

    public class TestClassWithFields
    {
        public string PublicField = "public";
#pragma warning disable CS0414 // Field is assigned but never used
        private string privateField = "private";
#pragma warning restore CS0414 // Field is assigned but never used
        public static string StaticField = "static";
        protected string protectedField = "protected";
    }

    [Obsolete("Base class obsolete")]
    public class BaseTestClass
    {
        protected string baseField = "base";
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public class DerivedTestClass : BaseTestClass
#pragma warning restore CS0618 // Type or member is obsolete
    {
        public string derivedField = "derived";
    }

    public class TestClassForCloning
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public NestedTestClassForCloning? NestedObject { get; set; }
    }

    public class NestedTestClassForCloning
    {
        public string? Value { get; set; }
    }

    public class TestClassImplementingIDisposable : IDisposable
    {
        public void Dispose()
        {
            // Implementation not needed for testing
        }
    }

    public class TestClassWithRequiredMembers
    {
        public required string RequiredProperty { get; set; }
    }

    public class TestClassWithParameterlessConstructor
    {
        public string Name { get; set; } = "";

        public TestClassWithParameterlessConstructor()
        {
        }

        public TestClassWithParameterlessConstructor(string name)
        {
            Name = name;
        }
    }

    public class TestClassWithoutParameterlessConstructor
    {
        public string Name { get; set; }

        public TestClassWithoutParameterlessConstructor(string name)
        {
            Name = name;
        }
    }

    // Additional test classes for specific scenarios
    public abstract class AbstractTestClass
    {
        public string Name { get; set; } = "";
    }

    public interface ITestInterface
    {
        void TestMethod();
    }

    public class TestClassWithJsonConstructor
    {
        public string Name { get; set; }
        public int Value { get; set; }

        [JsonConstructor]
        public TestClassWithJsonConstructor(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    public record TestRecord(string Name, int Value);

    public struct TestStruct
    {
        public int Value { get; set; }
    }

    #endregion
}