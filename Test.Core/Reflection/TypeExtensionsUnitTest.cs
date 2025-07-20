//--------------------------------------------------------------------------
// File:    TypeExtensionsUnitTest.cs
// Content:	Unit tests for TypeExtensions class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Numerics;

namespace AnBo.Test
{
    public class TypeExtensionsUnitTest
    {
        #region QuoteAssemblyQualifiedNameIfNeeded Tests

        [Fact]
        public void TestCase001_QuoteAssemblyQualifiedNameIfNeeded_With_Null_Type_Should_Throw_ArgumentNullException()
        {
            // Arrange
            Type? nullType = null;

            // Act & Assert
            var exception = Assert.Throws<ArgNullException>(() => nullType!.QuoteAssemblyQualifiedNameIfNeeded());
            exception.ParamName.Should().Be("type");
        }

        [Fact]
        public void TestCase002_QuoteAssemblyQualifiedNameIfNeeded_With_Simple_Type_Should_Return_Unquoted_Name()
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
        public void TestCase003_QuoteAssemblyQualifiedNameIfNeeded_With_Generic_Type_Should_Handle_Correctly()
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
        public void TestCase004_GetTypeName_With_Null_Type_Should_Throw_ArgumentNullException()
        {
            // Arrange
            Type? nullType = null;

            // Act & Assert
            var exception = Assert.Throws<ArgNullException>(() => nullType!.GetTypeName());
            exception.ParamName.Should().Be("type");
        }

        [Fact]
        public void TestCase005_GetTypeName_With_Simple_Type_Should_Return_Simple_Name()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.GetTypeName();

            // Assert
            result.Should().Be("String");
        }

        [Fact]
        public void TestCase006_GetTypeName_With_Generic_Type_Should_Return_Friendly_Name()
        {
            // Arrange
            var type = typeof(List<string>);

            // Act
            var result = type.GetTypeName();

            // Assert
            result.Should().Be("List[of String]");
        }

        [Fact]
        public void TestCase007_GetTypeName_With_Multiple_Generic_Arguments_Should_Return_Friendly_Name()
        {
            // Arrange
            var type = typeof(Dictionary<string, int>);

            // Act
            var result = type.GetTypeName();

            // Assert
            result.Should().Be("Dictionary[of String,Int32]");
        }

        [Fact]
        public void TestCase008_GetTypeName_With_Nested_Generic_Type_Should_Return_Friendly_Name()
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
        public void TestCase009_GetAnyField_With_Null_Type_Should_Throw_ArgumentNullException()
        {
            // Arrange
            Type? nullType = null;

            // Act & Assert
            var exception = Assert.Throws<ArgNullException>(() => nullType!.GetAnyField("fieldName"));
            exception.ParamName.Should().Be("type");
        }

        [Fact]
        public void TestCase010_GetAnyField_With_Null_FieldName_Should_Throw_ArgumentNullException()
        {
            // Arrange
            var type = typeof(TestClassWithFields);

            // Act & Assert
            var exception = Assert.Throws<ArgNullException>(() => type.GetAnyField(null!));
            exception.ParamName.Should().Be("fieldName");
        }

        [Fact]
        public void TestCase011_GetAnyField_With_Empty_FieldName_Should_Throw_ArgumentException()
        {
            // Arrange
            var type = typeof(TestClassWithFields);

            // Act & Assert
            var exception = Assert.Throws<ArgEmptyException>(() => type.GetAnyField(""));
            exception.ParamName.Should().Be("fieldName");
        }

        [Fact]
        public void TestCase012_GetAnyField_With_Existing_Public_Field_Should_Return_FieldInfo()
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
        public void TestCase013_GetAnyField_With_Existing_Private_Field_Should_Return_FieldInfo()
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
        public void TestCase014_GetAnyField_With_Non_Existing_Field_Should_Return_Null()
        {
            // Arrange
            var type = typeof(TestClassWithFields);

            // Act
            var result = type.GetAnyField("NonExistingField");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase015_GetAnyField_With_Case_Insensitive_Match_Should_Return_FieldInfo()
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
        public void TestCase016_GetAnyField_With_Inherited_Field_Should_Return_FieldInfo()
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
        public void TestCase017_GetAllFields_With_Null_Type_Should_Throw_ArgumentNullException()
        {
            // Arrange
            Type? nullType = null;

            // Act & Assert
            var exception = Assert.Throws<ArgNullException>(() => nullType!.GetAllFields().ToList());
            exception.ParamName.Should().Be("type");
        }

        [Fact]
        public void TestCase018_GetAllFields_With_Simple_Class_Should_Return_All_Fields()
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
        public void TestCase019_GetAllFields_With_Derived_Class_Should_Return_All_Fields_Including_Base()
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
        public void TestCase020_GetAllFields_With_Object_Type_Should_Return_Empty()
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
        public void TestCase021_ImplementsInterface_With_Null_Type_Should_Throw_ArgumentNullException()
        {
            // Arrange
            Type? nullType = null;

            // Act & Assert
            var exception = Assert.Throws<ArgNullException>(() => nullType!.ImplementsInterface<IDisposable>());
            exception.ParamName.Should().Be("type");
        }

        [Fact]
        public void TestCase022_ImplementsInterface_With_Type_Implementing_Interface_Should_Return_True()
        {
            // Arrange
            var type = typeof(TestClassImplementingIDisposable);

            // Act
            var result = type.ImplementsInterface<IDisposable>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase023_ImplementsInterface_With_Type_Not_Implementing_Interface_Should_Return_False()
        {
            // Arrange
            var type = typeof(TestClassWithFields);

            // Act
            var result = type.ImplementsInterface<IDisposable>();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase024_ImplementsInterface_With_Interface_Type_Should_Return_True()
        {
            // Arrange
            var type = typeof(IDisposable);

            // Act
            var result = type.ImplementsInterface<IDisposable>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase025_ImplementsInterface_With_Generic_Interface_Should_Return_True()
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
        public void TestCase026_HasRequiredMembers_With_Null_Type_Should_Return_False()
        {
            // Arrange
            Type? nullType = null;

            // Act & Assert
            var action = () => nullType!.HasRequiredMembers();
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void TestCase027_HasRequiredMembers_With_Type_Having_Required_Members_Should_Return_True()
        {
            // Arrange
            var type = typeof(TestClassWithRequiredMembers);

            // Act
            var result = type.HasRequiredMembers();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase028_HasRequiredMembers_With_Type_Without_Required_Members_Should_Return_False()
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
        public void TestCase029_IsNullableType_With_Nullable_Int_Should_Return_True()
        {
            // Arrange
            var type = typeof(int?);

            // Act
            var result = type.IsNullableType();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase030_IsNullableType_With_Nullable_DateTime_Should_Return_True()
        {
            // Arrange
            var type = typeof(DateTime?);

            // Act
            var result = type.IsNullableType();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase031_IsNullableType_With_Non_Nullable_Int_Should_Return_False()
        {
            // Arrange
            var type = typeof(int);

            // Act
            var result = type.IsNullableType();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase032_IsNullableType_With_Reference_Type_Should_Return_False()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.IsNullableType();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase033_IsNullableType_With_Generic_List_Should_Return_False()
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
        public void TestCase034_IsOpenGenericType_With_Null_Type_Should_Return_False()
        {
            // Arrange
            Type? nullType = null;

            // Act & Assert
            var action = () => nullType!.IsOpenGenericType();
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void TestCase035_IsOpenGenericType_With_Open_Generic_List_Should_Return_True()
        {
            // Arrange
            var type = typeof(List<>);

            // Act
            var result = type.IsOpenGenericType();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase036_IsOpenGenericType_With_Closed_Generic_List_Should_Return_False()
        {
            // Arrange
            var type = typeof(List<int>);

            // Act
            var result = type.IsOpenGenericType();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase037_IsOpenGenericType_With_Non_Generic_Type_Should_Return_False()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.IsOpenGenericType();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase038_IsOpenGenericType_With_Open_Generic_Dictionary_Should_Return_True()
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
        public void TestCase039_GetDefaultValue_With_Reference_Type_Should_Return_Null()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.GetDefaultValue();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase040_GetDefaultValue_With_Value_Type_Int_Should_Return_Zero()
        {
            // Arrange
            var type = typeof(int);

            // Act
            var result = type.GetDefaultValue();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase041_GetDefaultValue_With_Value_Type_Bool_Should_Return_False()
        {
            // Arrange
            var type = typeof(bool);

            // Act
            var result = type.GetDefaultValue();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void TestCase042_GetDefaultValue_With_DateTime_Should_Return_MinValue()
        {
            // Arrange
            var type = typeof(DateTime);

            // Act
            var result = type.GetDefaultValue();

            // Assert
            result.Should().Be(default(DateTime));
        }

        [Fact]
        public void TestCase043_GetDefaultValue_With_Nullable_Type_Should_Return_Null()
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
        public void TestCase044_IsDefaultValue_With_Null_Value_Should_Return_True()
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
        public void TestCase045_IsDefaultValue_With_Default_Int_Should_Return_True()
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
        public void TestCase046_IsDefaultValue_With_Non_Default_Int_Should_Return_False()
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
        public void TestCase047_IsDefaultValue_With_Default_Bool_Should_Return_True()
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
        public void TestCase048_IsDefaultValue_With_Non_Default_Bool_Should_Return_False()
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
        public void TestCase049_IsDefaultValue_With_Non_Null_String_Should_Return_False()
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
        public void TestCase050_IsDefaultValueOrEmptyString_With_Null_Should_Return_True()
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
        public void TestCase051_IsDefaultValueOrEmptyString_With_Empty_String_Should_Return_True()
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
        public void TestCase052_IsDefaultValueOrEmptyString_With_Non_Empty_String_Should_Return_False()
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
        public void TestCase053_IsDefaultValueOrEmptyString_With_Default_Value_Type_Should_Return_True()
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
        public void TestCase054_IsDefaultValueOrEmptyString_With_Non_Default_Value_Type_Should_Return_False()
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
        public void TestCase055_IsJsonSerializable_With_Simple_Value_Type_Should_Return_True()
        {
            // Arrange
            var type = typeof(int);

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase056_IsJsonSerializable_With_String_Type_Should_Return_True()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase057_IsJsonSerializable_With_Class_Having_Parameterless_Constructor_Should_Return_True()
        {
            // Arrange
            var type = typeof(TestClassWithParameterlessConstructor);

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase058_IsJsonSerializable_With_Class_Without_Parameterless_Constructor_Should_Return_False()
        {
            // Arrange
            var type = typeof(TestClassWithoutParameterlessConstructor);

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase059_IsJsonSerializable_With_Pointer_Type_Should_Return_False()
        {
            // Arrange
            var type = typeof(int).MakePointerType();

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase060_IsJsonSerializable_With_ByRef_Type_Should_Return_False()
        {
            // Arrange
            var type = typeof(int).MakeByRefType();

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase061_IsJsonSerializable_With_Nullable_Int32_Type_Should_Return_True()
        {
            // Arrange
            var type = typeof(int?);

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase062_IsJsonSerializable_With_Int32_Array_Type_Should_Return_True()
        {
            // Arrange
            var type = typeof(int[]);

            // Act
            var result = type.IsJsonSerializable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase063_IsJsonSerializable_With_Struct_Type_Should_Return_True()
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
        public void TestCase064_DeepClone_With_Null_Should_Return_Null()
        {
            // Arrange
            string? original = null;

            // Act
            var result = original.DeepClone();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TestCase065_DeepClone_With_Simple_Object_Should_Create_Copy()
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
        public void TestCase066_DeepClone_With_Complex_Object_Should_Create_Deep_Copy()
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
        public void TestCase067_DeepClone_With_Value_Type_Should_Work()
        {
            // Arrange
            int original = 42;

            // Act
            var result = original.DeepClone();

            // Assert
            result.Should().Be(42);
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

        #endregion
    }
}