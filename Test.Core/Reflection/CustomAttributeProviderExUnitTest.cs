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
    public class CustomAttributeProviderExUnitTest
    {
        #region HasAttribute Tests

        [Fact]
        public void HasAttribute_With_Null_Type_Should_Should_Return_False()
        {
            // Arrange
            Type? nullType = null;

            // Act
#pragma warning disable CS8604 // Possible null reference argument.
            var result = nullType.HasAttribute<XmlRootAttribute>();
#pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasAttribute_With_Existing_Attribute_Should_Return_True()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result1 = type.HasAttribute<XmlRootAttribute>();
            var result2 = type.HasAttribute(typeof(XmlRootAttribute));

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Fact]
        public void HasAttribute_With_Non_Existing_Attribute_Should_Return_False()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result1 = type.HasAttribute<ObsoleteAttribute>();
            var result2 = type.HasAttribute(typeof(ObsoleteAttribute));

            // Assert
            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        #endregion

        #region TryGetAttribute Tests

        [Fact]
        public void TryGetAttribute_With_Null_Type_Should_Return_False()
        {
            // Arrange
            Type? nullType = null;

            // Act
            var result = nullType!.TryGetAttribute<XmlRootAttribute>(out _);
            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetAttribute_With_Null_Type_Should_Return_False()
        {
            // Arrange
            Type? nullType = null;

            // Act
            var result = nullType!.GetAttribute<XmlRootAttribute>();
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TryGetAttribute_With_Existing_Attribute_Should_Return_True_And_Attribute()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.TryGetAttribute<XmlRootAttribute>(out var attribute);

            // Assert
            result.Should().BeTrue();
            attribute.Should().NotBeNull();
            attribute!.ElementName.Should().Be("TestRoot");
        }

        [Fact]
        public void GetAttribute_With_Existing_Attribute_Should_Attribute()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var attribute = type.GetAttribute<XmlRootAttribute>();

            // Assert
            attribute.Should().NotBeNull();
            attribute!.ElementName.Should().Be("TestRoot");
        }

        [Fact]
        public void TryGetAttribute_With_Non_Existing_Attribute_Should_Return_False_And_Null()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.TryGetAttribute<ObsoleteAttribute>(out var attribute);

            // Assert
            result.Should().BeFalse();
            attribute.Should().BeNull();
        }

        [Fact]
        public void GetAttribute_With_Non_Existing_Attribute_Should_Return_Null()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var attribute = type.GetAttribute<ObsoleteAttribute>();

            // Assert
            attribute.Should().BeNull();
        }

        #endregion

        #region GetAllAttributes Tests

        [Fact]
        public void GetAllAttributes_With_Null_Type_Should_Return_Empty_Collection()
        {
            // Arrange
            Type? nullType = null;

            // Act
            var result = nullType.GetAllAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAttributes_With_Null_Type_Should_Return_Empty_Collection()
        {
            // Arrange
            Type? nullType = null;

            // Act
            var result = nullType.GetAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAllAttributes_With_Type_Having_Multiple_Attributes_Should_Return_All()
        {
            // Arrange
            var type = typeof(TestClassWithMultipleAttributes);

            // Act
            var result = type.GetAllAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(attr => attr.ElementName == "Root Base");
            result.Should().Contain(attr => attr.ElementName == "Root Class");
        }

        [Fact]
        public void GetAttributes_With_Type_Having_Multiple_Attributes_Should_Return_Only_One()
        {
            // Arrange
            var type = typeof(TestClassWithMultipleAttributes);

            // Act
            var result = type.GetAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().HaveCount(1);
            result.Should().Contain(attr => attr.ElementName == "Root Class");
        }

        [Fact]
        public void GetAllAttributes_With_Type_Having_Single_Attribute_Should_Return_Single()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.GetAllAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().HaveCount(1);
            result.First().ElementName.Should().Be("TestRoot");
        }

        [Fact]
        public void GetAttributes_With_Type_Having_Single_Attribute_Should_Return_Single()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.GetAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().HaveCount(1);
            result.First().ElementName.Should().Be("TestRoot");
        }

        [Fact]
        public void GetAllAttributes_With_Type_Having_No_Attribute_Should_Return_Empty()
        {
            // Arrange
            var type = typeof(TestClassWithFields);

            // Act
            var result = type.GetAllAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAttributes_With_Type_Having_No_Attribute_Should_Return_Empty()
        {
            // Arrange
            var type = typeof(TestClassWithFields);

            // Act
            var result = type.GetAttributes<XmlRootAttribute>().ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAllAttributes_With_Inherit_False_Should_Not_Include_Base_Attributes()
        {
            // Arrange
            var type = typeof(DerivedTestClass);

            // Act
            var result = type.GetAllAttributes<ObsoleteAttribute>(inherit: false).ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAllAttributes_With_Inherit_True_Should_Include_Base_Attributes()
        {
            // Arrange
            var type = typeof(DerivedTestClass);

            // Act
            var result = type.GetAllAttributes<ObsoleteAttribute>(inherit: true).ToList();

            // Assert
            result.Should().HaveCount(1);
            result.First().Message.Should().Be("Base class obsolete");
        }

        #endregion

        #region FindAttribute Tests

        [Fact]
        public void FindAttribute_With_Null_Type_Should_Return_Null()
        {
            // Arrange
            Type? nullType = null;

            // Act
            var result = nullType.FindAttribute<ObsoleteAttribute>(attr => attr.Message == null ? false : attr.Message.Contains("Class is"));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void FindAttribute_With_Matching_Predicate_Should_Return_Attribute()
        {
            // Arrange
            var type = typeof(TestClassWithMultipleAttributes);

            // Act
            var result = type.FindAttribute<XmlRootAttribute>(attr => attr.ElementName.Contains("Class"));

            // Assert
            result.Should().NotBeNull();
            result!.ElementName.Should().Be("Root Class");
        }

        [Fact]
        public void FindAttribute_With_Non_Matching_Predicate_Should_Return_Null()
        {
            // Arrange
            var type = typeof(TestClassWithMultipleAttributes);

            // Act
            var result = type.FindAttribute<XmlRootAttribute>(attr => attr.ElementName.Contains("NonExisting"));

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void FindAttribute_With_Type_Having_No_Attribute_Should_Return_Null()
        {
            // Arrange
            var type = typeof(TestClassWithFields);

            // Act
            var result = type.FindAttribute<ObsoleteAttribute>(attr => true);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region HasMemberAttribute Tests

        [Fact]
        public void HasMemberAttribute_With_Null_Type_Should_Return_False()
        {
            // Arrange
            Type? nullType = null;

            // Act
            var result = nullType!.HasMemberAttribute<XmlIgnoreAttribute>("FieldWithAttribute");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasMemberAttribute_With_Existing_Field_Attribute_Should_Return_True()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.HasMemberAttribute<XmlIgnoreAttribute>("FieldWithAttribute");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasMemberAttribute_With_Non_Existing_Field_Attribute_Should_Return_False()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.HasMemberAttribute<XmlIgnoreAttribute>("FieldWithoutAttribute");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasMemberAttribute_With_Non_Existing_Member_Should_Return_False()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.HasMemberAttribute<XmlIgnoreAttribute>("NonExistingMember");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region TryGetMemberAttribute Tests

        [Fact]
        public void TryGetMemberAttribute_With_Null_Type_Should_Return_False_And_Null()
        {
            // Arrange
            Type? nullType = null;

            // Act
            var result = nullType!.TryGetMemberAttribute<XmlIgnoreAttribute>("FieldWithAttribute", out var attribute);

            // Assert
            result.Should().BeFalse();
            attribute.Should().BeNull();
        }

        [Fact]
        public void TryGetMemberAttribute_With_Existing_Field_Attribute_Should_Return_True_And_Attribute()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.TryGetMemberAttribute<XmlIgnoreAttribute>("FieldWithAttribute", out var attribute);

            // Assert
            result.Should().BeTrue();
            attribute.Should().NotBeNull();
        }

        [Fact]
        public void TryGetMemberAttribute_With_Non_Existing_Field_Attribute_Should_Return_False_And_Null()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.TryGetMemberAttribute<XmlIgnoreAttribute>("FieldWithoutAttribute", out var attribute);

            // Assert
            result.Should().BeFalse();
            attribute.Should().BeNull();
        }

        [Fact]
        public void TryGetMemberAttribute_With_Non_Existing_Member_Should_Return_False_And_Null()
        {
            // Arrange
            var type = typeof(TestClassWithAttributes);

            // Act
            var result = type.TryGetMemberAttribute<XmlIgnoreAttribute>("NonExistingMember", out var attribute);

            // Assert
            result.Should().BeFalse();
            attribute.Should().BeNull();
        }

        #endregion

        #region Test Helper Classes

        [XmlRoot("TestRoot")]
        [DataContract]
        public class TestClassWithAttributes
        {
            [XmlIgnore]
            public string FieldWithAttribute = "secret";

            public string FieldWithoutAttribute = "secret";

            [DataMember(Name = "PrivateField")]
#pragma warning disable CS0414 // Field is assigned but never used
            private string privateField = "private";
#pragma warning restore CS0414 // Field is assigned but never used

            [XmlElement("TestElement")]
            public string? ElementWithAttribute { get; set; }

            public string? ElementWithoutAttribute { get; set; }

            [XmlEnum("TestEnum")]
            public DayOfWeek dayWithAttribute;

            public DayOfWeek dayWithoutAttribute;

            [XmlArray("Items")]
            [XmlArrayItem("TestItem")]
            public string[]? ItemsWithAttributes { get; set; }

            public string[]? ItemsWithoutAttributes { get; set; }

            [XmlAttribute("TestAttribute")]
            public string? NameWithAttribute { get; set; }

            public string? NameWithoutAttribute { get; set; }
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

        [XmlRoot("Root Base")]
        public class TestBaseClassWithMultipleAttributes
        {             
            public string BaseField = "base";
        }

        [XmlRoot("Root Class")]
        public class TestClassWithMultipleAttributes : TestBaseClassWithMultipleAttributes
        {
            public string TestField = "test";
        }

        public class TestClassWithFields
        {
            public string PublicField = "public";
#pragma warning disable CS0414 // Field is assigned but never used
            private string privateField = "private";
#pragma warning restore CS0414 // Field is assigned but never used
            public static string StaticField = "static";
            protected string protectedField = "protected";
        }

        #endregion
    }
}