using FluentAssertions;
using Xunit;
using AnBo.Core;
using System;

namespace AnBo.Test
{
    public class TypeOfUnitTest
    {
        [Fact]
        public void TestCase001_Boolean_Property_Should_Return_Bool_Type()
        {
            TypeOf.Boolean.Should().Be(typeof(bool));
        }

        [Fact]
        public void TestCase002_Int32_Property_Should_Return_Int_Type()
        {
            TypeOf.Int32.Should().Be(typeof(int));
        }

        [Fact]
        public void TestCase003_Int64_Property_Should_Return_Long_Type()
        {
            TypeOf.Int64.Should().Be(typeof(long));
        }

        [Fact]
        public void TestCase004_Object_Property_Should_Return_Object_Type()
        {
            TypeOf.Object.Should().Be(typeof(object));
        }

        [Fact]
        public void TestCase005_String_Property_Should_Return_String_Type()
        {
            TypeOf.String.Should().Be(typeof(string));
        }

        [Fact]
        public void TestCase006_All_Properties_Should_Not_Be_Null()
        {
            TypeOf.Boolean.Should().NotBeNull();
            TypeOf.Int32.Should().NotBeNull();
            TypeOf.Int64.Should().NotBeNull();
            TypeOf.Object.Should().NotBeNull();
            TypeOf.String.Should().NotBeNull();
        }

        [Fact]
        public void TestCase007_Properties_Should_Have_Correct_Names()
        {
            TypeOf.Boolean.Name.Should().Be("Boolean");
            TypeOf.Int32.Name.Should().Be("Int32");
            TypeOf.Int64.Name.Should().Be("Int64");
            TypeOf.Object.Name.Should().Be("Object");
            TypeOf.String.Name.Should().Be("String");
        }

        [Fact]
        public void TestCase008_Properties_Should_Have_Correct_Full_Names()
        {
            TypeOf.Boolean.FullName.Should().Be("System.Boolean");
            TypeOf.Int32.FullName.Should().Be("System.Int32");
            TypeOf.Int64.FullName.Should().Be("System.Int64");
            TypeOf.Object.FullName.Should().Be("System.Object");
            TypeOf.String.FullName.Should().Be("System.String");
        }

        [Fact]
        public void TestCase009_Properties_Should_Be_Value_Types_Or_Reference_Types_As_Expected()
        {
            TypeOf.Boolean.IsValueType.Should().BeTrue();
            TypeOf.Int32.IsValueType.Should().BeTrue();
            TypeOf.Int64.IsValueType.Should().BeTrue();
            TypeOf.Object.IsValueType.Should().BeFalse();
            TypeOf.String.IsValueType.Should().BeFalse();
        }

        [Fact]
        public void TestCase010_Properties_Should_Have_Consistent_Reference_Identity()
        {
            // Verify that multiple accesses return the same Type instance
            TypeOf.Boolean.Should().BeSameAs(TypeOf.Boolean);
            TypeOf.Int32.Should().BeSameAs(TypeOf.Int32);
            TypeOf.Int64.Should().BeSameAs(TypeOf.Int64);
            TypeOf.Object.Should().BeSameAs(TypeOf.Object);
            TypeOf.String.Should().BeSameAs(TypeOf.String);
        }
    }
}