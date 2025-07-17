using AnBo.Core;
using FluentAssertions;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace AnBo.Test
{
    public class StringConversionHelperUnitTest
    {
        #region IsTypeSpan Method Tests

        [Fact]
        public void TestCase001_IsTypeSpan_With_Valid_Int_Span_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<char> span = "42".AsSpan();

            // Act
            var result = span.IsTypeSpan<int>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase002_IsTypeSpan_With_Invalid_Int_Span_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<char> span = "invalid".AsSpan();

            // Act
            var result = span.IsTypeSpan<int>();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase003_IsTypeSpan_With_Valid_Double_Span_Should_Return_True()
        {
            // Arrange
            ReadOnlySpan<char> span = "42.5".AsSpan();

            // Act
            var result = span.IsTypeSpan<double>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase004_IsTypeSpan_With_Custom_Provider_Should_Use_Provider()
        {
            // Arrange
            ReadOnlySpan<char> span = "42,5".AsSpan();
            var germanCulture = new CultureInfo("de-DE");

            // Act
            var result = span.IsTypeSpan<double>(germanCulture);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase005_IsTypeSpan_With_Null_Provider_Should_Use_InvariantCulture()
        {
            // Arrange
            ReadOnlySpan<char> span = "42.5".AsSpan();

            // Act
            var result = span.IsTypeSpan<double>(null);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase006_IsTypeSpan_With_Empty_Span_Should_Return_False()
        {
            // Arrange
            ReadOnlySpan<char> span = "".AsSpan();

            // Act
            var result = span.IsTypeSpan<int>();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase007_IsTypeSpan_With_DateTime_Should_Work()
        {
            // Arrange
            ReadOnlySpan<char> span = "2025-01-15".AsSpan();

            // Act
            var result = span.IsTypeSpan<DateTime>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase008_IsTypeSpan_With_Guid_Should_Work()
        {
            // Arrange
            var guid = Guid.NewGuid();
            ReadOnlySpan<char> span = guid.ToString().AsSpan();

            // Act
            var result = span.IsTypeSpan<Guid>();

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region IsTypeString Method Tests

        [Fact]
        public void TestCase009_IsTypeString_With_Valid_Int_String_Should_Return_True()
        {
            // Arrange
            string str = "42";

            // Act
            var result = str.IsTypeString<int>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase010_IsTypeString_With_Invalid_Int_String_Should_Return_False()
        {
            // Arrange
            string str = "invalid";

            // Act
            var result = str.IsTypeString<int>();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase011_IsTypeString_With_Null_String_Should_Return_False()
        {
            // Arrange
            string? str = null;

            // Act
            var result = str.IsTypeString<int>();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase012_IsTypeString_With_Empty_String_Should_Return_False()
        {
            // Arrange
            string str = "";

            // Act
            var result = str.IsTypeString<int>();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TestCase013_IsTypeString_With_Custom_Provider_Should_Use_Provider()
        {
            // Arrange
            string str = "42,5";
            var germanCulture = new CultureInfo("de-DE");

            // Act
            var result = str.IsTypeString<double>(germanCulture);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TestCase014_IsTypeString_With_Boolean_Should_Work()
        {
            // Arrange
            string str = "true";

            // Act
            var result = str.IsTypeString<bool>();

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region ToInvariantString Method Tests - Basic Types

        [Fact]
        public void TestCase015_ToInvariantString_With_Null_Should_Return_NullString()
        {
            // Arrange
            object? value = null;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("<null>");
        }

        [Fact]
        public void TestCase016_ToInvariantString_With_String_Should_Return_Same_String()
        {
            // Arrange
            string value = "test string";

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("test string");
        }

        [Fact]
        public void TestCase017_ToInvariantString_With_Boolean_True_Should_Return_True()
        {
            // Arrange
            bool value = true;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("True");
        }

        [Fact]
        public void TestCase018_ToInvariantString_With_Boolean_False_Should_Return_False()
        {
            // Arrange
            bool value = false;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("False");
        }

        [Fact]
        public void TestCase019_ToInvariantString_With_Integer_Should_Return_String_Representation()
        {
            // Arrange
            int value = 42;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("42");
        }

        [Fact]
        public void TestCase020_ToInvariantString_With_Double_Should_Use_InvariantCulture()
        {
            // Arrange
            double value = 42.5;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("42.5");
        }

        [Fact]
        public void TestCase021_ToInvariantString_With_DateTime_Should_Use_Default_Format()
        {
            // Arrange
            var value = new DateTime(2025, 1, 15, 14, 30, 45);

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("2025-01-15 14:30:45");
        }

        [Fact]
        public void TestCase022_ToInvariantString_With_Custom_DateTime_Format()
        {
            // Arrange
            var value = new DateTime(2025, 1, 15, 14, 30, 45);
            var options = new ToStringOptions { DateTimeFormat = "yyyy/MM/dd" };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Be("2025/01/15");
        }

        [Fact]
        public void TestCase023_ToInvariantString_With_Guid_Should_Return_String_Representation()
        {
            // Arrange
            var value = new Guid("12345678-1234-1234-1234-123456789012");

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("12345678-1234-1234-1234-123456789012");
        }

        #endregion

        #region ToInvariantString Method Tests - Collections

        [Fact]
        public void TestCase024_ToInvariantString_With_Empty_Array_Should_Return_Empty_Brackets()
        {
            // Arrange
            int[] value = [];

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("[]");
        }

        [Fact]
        public void TestCase025_ToInvariantString_With_Array_Should_Format_With_Brackets()
        {
            // Arrange
            int[] value = [1, 2, 3];

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("[1, 2, 3] (3 items)");
        }

        [Fact]
        public void TestCase026_ToInvariantString_With_Array_Should_Respect_MaxCollectionItems()
        {
            // Arrange
            int[] value = [1, 2, 3, 4, 5];
            var options = new ToStringOptions { MaxCollectionItems = 3 };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Be("[1, 2, 3, ...] (5 items)");
        }

        [Fact]
        public void TestCase027_ToInvariantString_With_Dictionary_Should_Format_With_Braces()
        {
            // Arrange
            var value = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Contain("{").And.Contain("}").And.Contain("a: 1").And.Contain("b: 2");
        }

        [Fact]
        public void TestCase028_ToInvariantString_With_Empty_Dictionary_Should_Return_Empty_Braces()
        {
            // Arrange
            var value = new Dictionary<string, int>();

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("{}");
        }

        [Fact]
        public void TestCase029_ToInvariantString_With_List_Should_Format_As_Collection()
        {
            // Arrange
            var value = new List<string> { "first", "second", "third" };

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("[first, second, third] (3 items)");
        }

        [Fact]
        public void TestCase030_ToInvariantString_With_Collection_Count_Disabled()
        {
            // Arrange
            var value = new List<int> { 1, 2, 3 };
            var options = new ToStringOptions { ShowCollectionCount = false };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Be("[1, 2, 3]");
        }

        [Fact]
        public void TestCase031_ToInvariantString_With_Custom_Separator()
        {
            // Arrange
            var value = new List<int> { 1, 2, 3 };
            var options = new ToStringOptions { CollectionSeparator = " | " };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Contain("1 | 2 | 3");
        }

        #endregion

        #region ToInvariantString Method Tests - Multidimensional Arrays

        [Fact]
        public void TestCase032_ToInvariantString_With_2D_Array_Should_Format_Correctly()
        {
            // Arrange
            int[,] value = { { 1, 2 }, { 3, 4 } };

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("[[1, 2], [3, 4]]");
        }

        [Fact]
        public void TestCase033_ToInvariantString_With_2D_Array_Show_Dimensions()
        {
            // Arrange
            int[,] value = { { 1, 2 }, { 3, 4 } };
            var options = new ToStringOptions { ShowArrayDimensions = true };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Contain("2D 2×2, 4 items");
        }

        [Fact]
        public void TestCase034_ToInvariantString_With_3D_Array_Should_Format_Correctly()
        {
            // Arrange
            int[,,] value = new int[2, 2, 2];
            value[0, 0, 0] = 1;
            value[1, 1, 1] = 8;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Contain("[").And.Contain("]");
        }

        #endregion

        #region ToInvariantString Method Tests - Nullable Types

        [Fact]
        public void TestCase035_ToInvariantString_With_Nullable_Int_Null_Should_Return_NullString()
        {
            // Arrange
            int? value = null;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("<null>");
        }

        [Fact]
        public void TestCase036_ToInvariantString_With_Nullable_Int_Value_Should_Return_Value()
        {
            // Arrange
            int? value = 42;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("42");
        }

        #endregion

        #region ToInvariantString Method Tests - Enum Types

        [Fact]
        public void TestCase037_ToInvariantString_With_Enum_Should_Return_String_Name()
        {
            // Arrange
            DayOfWeek value = DayOfWeek.Monday;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("Monday");
        }

        #endregion

        #region ToInvariantString Method Tests - Max Nesting Depth

        [Fact]
        public void TestCase038_ToInvariantString_With_Max_Depth_Reached_Should_Return_Placeholder()
        {
            // Arrange
            var options = new ToStringOptions { MaxNestingDepth = 1 };

            // Act
            var result = "test".ToInvariantString(options, currentDepth: 1);

            // Assert
            result.Should().Be("<max nesting depth reached>");
        }

        [Fact]
        public void TestCase039_ToInvariantString_With_Nested_Arrays_Should_Respect_Max_Depth()
        {
            // Arrange
            var nestedArray = new object[] { new object[] { "deep" } };
            var options = new ToStringOptions { MaxNestingDepth = 3 };

            // Act
            var result = nestedArray.ToInvariantString(options);

            // Assert
            result.Should().NotContain("<max nesting depth reached>");
        }

        #endregion

        #region ToInvariantString Method Tests - Custom Formats

        [Fact]
        public void TestCase040_ToInvariantString_With_Custom_Decimal_Format()
        {
            // Arrange
            decimal value = 123.456m;
            var options = new ToStringOptions { DecimalFormat = "F2" };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Be("123.46");
        }

        [Fact]
        public void TestCase041_ToInvariantString_With_Custom_Double_Format()
        {
            // Arrange
            double value = 123.456;
            var options = new ToStringOptions { DoubleFormat = "F1" };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Be("123.5");
        }

        [Fact]
        public void TestCase042_ToInvariantString_With_TimeSpan_Custom_Format_g()
        {
            // Arrange
            var value = new TimeSpan(1, 2, 3, 4);
            var options = new ToStringOptions { TimeSpanFormat = "g" };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Contain("1:2:03:04");
        }

        [Fact]
        public void TestCase042b_ToInvariantString_With_TimeSpan_Custom_Format_c()
        {
            // Arrange
            var value = new TimeSpan(1, 2, 3, 4);
            var options = new ToStringOptions { TimeSpanFormat = "c" };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Contain("1.02:03:04");
        }

        [Fact]
        public void TestCase043_ToInvariantString_With_DateOnly_Should_Work()
        {
            // Arrange
            var value = new DateOnly(2025, 1, 15);

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("2025-01-15");
        }

        [Fact]
        public void TestCase044_ToInvariantString_With_TimeOnly_Should_Work()
        {
            // Arrange
            var value = new TimeOnly(14, 30, 45);

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("14:30:45");
        }

        [Fact]
        public void TestCase045_ToInvariantString_With_DateTimeOffset_Should_Work()
        {
            // Arrange
            var value = new DateTimeOffset(2025, 1, 15, 14, 30, 45, TimeSpan.FromHours(2));

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Contain("2025-01-15 14:30:45").And.Contain("+02:00");
        }

        #endregion

        #region ToInvariantString Method Tests - Custom Options

        [Fact]
        public void TestCase046_ToInvariantString_With_Custom_NullString()
        {
            // Arrange
            object? value = null;
            var options = new ToStringOptions { NullString = "NULL" };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Be("NULL");
        }

        [Fact]
        public void TestCase047_ToInvariantString_With_Custom_Dictionary_Separator()
        {
            // Arrange
            var value = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
            var options = new ToStringOptions { DictionaryKeyValueSeparator = " => " };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Contain("a => 1").And.Contain("b => 2");
        }

        #endregion

        #region ParseInvariantString Method Tests - Generic Version

        [Fact]
        public void TestCase048_ParseInvariantString_Generic_With_Null_Should_Throw()
        {
            // Arrange
            string? value = null;

            // Act & Assert
            var action = () => value!.ParseInvariantString<int>();
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void TestCase049_ParseInvariantString_Generic_With_Valid_Int_Should_Return_Int()
        {
            // Arrange
            string value = "42";

            // Act
            var result = value.ParseInvariantString<int>();

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void TestCase050_ParseInvariantString_Generic_With_Valid_Double_Should_Return_Double()
        {
            // Arrange
            string value = "42.5";

            // Act
            var result = value.ParseInvariantString<double>();

            // Assert
            result.Should().Be(42.5);
        }

        [Fact]
        public void TestCase051_ParseInvariantString_Generic_With_Valid_DateTime_Should_Return_DateTime()
        {
            // Arrange
            string value = "2025-01-15";

            // Act
            var result = value.ParseInvariantString<DateTime>();

            // Assert
            result.Should().Be(new DateTime(2025, 1, 15));
        }

        [Fact]
        public void TestCase052_ParseInvariantString_Generic_With_Valid_Guid_Should_Return_Guid()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            string value = expectedGuid.ToString();

            // Act
            var result = value.ParseInvariantString<Guid>();

            // Assert
            result.Should().Be(expectedGuid);
        }

        [Fact]
        public void TestCase053_ParseInvariantString_Generic_With_Invalid_Value_Should_Return_Default()
        {
            // Arrange
            string value = "invalid";

            // Act
            var result = value.ParseInvariantString<int>();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase054_ParseInvariantString_Generic_With_Boolean_Should_Work()
        {
            // Arrange
            string value = "true";

            // Act
            var result = value.ParseInvariantString<bool>();

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region ParseInvariantString Method Tests - Type Parameter Version

        [Fact]
        public void TestCase055_ParseInvariantString_Type_With_Null_Value_Should_Throw()
        {
            // Arrange
            string? value = null;
            Type type = typeof(int);

            // Act & Assert
            var action = () => value!.ParseInvariantString(type);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void TestCase056_ParseInvariantString_Type_With_Null_Type_Should_Throw()
        {
            // Arrange
            string value = "42";
            Type? type = null;

            // Act & Assert
            var action = () => value.ParseInvariantString(type!);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void TestCase057_ParseInvariantString_Type_With_Valid_Int_Should_Return_Int()
        {
            // Arrange
            string value = "42";
            Type type = typeof(int);

            // Act
            var result = value.ParseInvariantString(type);

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void TestCase058_ParseInvariantString_Type_With_Valid_String_Should_Return_String()
        {
            // Arrange
            string value = "test";
            Type type = typeof(string);

            // Act
            var result = value.ParseInvariantString(type);

            // Assert
            result.Should().Be("test");
        }

        [Fact]
        public void TestCase059_ParseInvariantString_Type_With_Invalid_Value_Should_Return_Default()
        {
            // Arrange
            string value = "invalid";
            Type type = typeof(int);

            // Act
            var result = value.ParseInvariantString(type);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void TestCase060_ParseInvariantString_Type_With_Enum_Should_Work()
        {
            // Arrange
            string value = "Monday";
            Type type = typeof(DayOfWeek);

            // Act
            var result = value.ParseInvariantString(type);

            // Assert
            result.Should().Be(DayOfWeek.Monday);
        }

        [Fact]
        public void TestCase061_ParseInvariantString_Type_With_Custom_Type_Should_Use_TypeConverter()
        {
            // Arrange
            string value = "test";
            Type type = typeof(string);

            // Act
            var result = value.ParseInvariantString(type);

            // Assert
            result.Should().Be("test");
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public void TestCase062_ToInvariantString_With_Circular_Reference_Should_Not_Cause_Stack_Overflow()
        {
            // Arrange
            var list = new List<object>();
            list.Add(list); // Circular reference
            var options = new ToStringOptions { MaxNestingDepth = 3 };

            // Act
            var result = list.ToInvariantString(options);

            // Assert
            result.Should().NotBeNull();
            result.Should().Contain("<max nesting depth reached>");
        }

        [Fact]
        public void TestCase063_ToInvariantString_With_Very_Large_Array_Should_Truncate()
        {
            // Arrange
            var largeArray = new int[1000];
            var largeList = new List<int>();
            var largeDictionary = new Dictionary<int, string>();
            for (int i = 0; i < 1000; i++)
            {
                largeArray[i] = i;
                largeDictionary.Add(i, $"value{i}");
            }

            largeList.AddRange(largeArray);

            var options = new ToStringOptions { MaxCollectionItems = 5 };

            // Act
            var result = largeArray.ToInvariantString(options);
            var listResult = largeList.ToInvariantString(options);
            var dictResult = largeDictionary.ToInvariantString(options);

            // Assert
            result.Should().Contain("...");
            result.Should().Contain("(1000 items)");

            listResult.Should().Contain("...");
            listResult.Should().Contain("(1000 items)");

            dictResult.Should().Contain("...");
            dictResult.Should().Contain("(1000 items)");
        }

        [Fact]
        public void TestCase064_ToInvariantString_With_String_Should_Not_Be_Treated_As_Collection()
        {
            // Arrange
            string value = "hello";

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("hello");
            result.Should().NotContain("[");
            result.Should().NotContain("]");
        }

        [Fact]
        public void TestCase065_ParseInvariantString_With_Nullable_Type_Should_Work()
        {
            // Arrange
            string value = "42";
            Type type = typeof(int?);

            // Act
            var result = value.ParseInvariantString(type);

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void TestCase066_ParseInvariantString_With_Exception_Should_Return_Default()
        {
            // Arrange
            string value = "not-a-guid";
            Type type = typeof(Guid);

            // Act
            var result = value.ParseInvariantString(type);

            // Assert
            result.Should().Be(Guid.Empty);
        }

        #endregion

        #region Format String Edge Cases

        [Fact]
        public void TestCase067_ToInvariantString_With_Empty_Format_String_Should_Use_Default()
        {
            // Arrange
            decimal value = 123.456m;
            var options = new ToStringOptions { DecimalFormat = "" };

            // Act
            var result = value.ToInvariantString(options);

            // Assert
            result.Should().Be("123.456");
        }

        [Fact]
        public void TestCase068_ToInvariantString_With_Very_Long_Number_Should_Format_Correctly()
        {
            // Arrange
            long value = long.MaxValue;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be(long.MaxValue.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region Special Numeric Types

        [Fact]
        public void TestCase069_ToInvariantString_With_Float_Should_Work()
        {
            // Arrange
            float value = 123.46f;
            var options = new ToStringOptions { FloatFormat = "F1" };

            // Act
            var result1 = value.ToInvariantString();
            var result2 = value.ToInvariantString(options);

            // Assert
            result1.Should().Be("123.46");
            result2.Should().Be("123.5");
        }

        [Fact]
        public void TestCase070_ToInvariantString_With_Byte_Should_Work()
        {
            // Arrange
            byte value = 255;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("255");
        }

        [Fact]
        public void TestCase071_ToInvariantString_With_Short_Should_Work()
        {
            // Arrange
            short value = 12345;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("12345");
        }

        [Fact]
        public void TestCase072_ToInvariantString_With_UInt_Should_Work()
        {
            // Arrange
            uint value = 4294967295;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("4294967295");
        }

        [Fact]
        public void TestCase073_ToInvariantString_With_Char_Should_Work()
        {
            // Arrange
            char value = 'A';

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("A");
        }

        [Fact]
        public void TestCase074_ToInvariantString_With_Int128_Number_Should_Format_Correctly()
        {
            // Arrange
            var value = Int128.MaxValue;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be(Int128.MaxValue.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void TestCase075_ToInvariantString_With_UInt128_Number_Should_Format_Correctly()
        {
            // Arrange
            var value = UInt128.MaxValue;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be(UInt128.MaxValue.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void TestCase076_ToInvariantString_With_BigInteger_Number_Should_Format_Correctly()
        {
            // Arrange
            var zero = new BigInteger(0);          // 1 Zeichen
            var small = new BigInteger(123);          // ~3 Zeichen
            var medium1 = BigInteger.Parse("123456789012345"); // ~15 Zeichen  
            var medium2 = BigInteger.Parse("-123456789012345"); // ~15 Zeichen  
            var large = BigInteger.Pow(2, 1000);     // ~302 Zeichen (dezimal)

            // Act
            var resultZero = zero.ToInvariantString();
            var resultSmall = small.ToInvariantString();
            var resultMedium1 = medium1.ToInvariantString();
            var resultMedium2 = medium2.ToInvariantString();
            var resultLarge = large.ToInvariantString();

            // Assert
            resultZero.Should().Be(zero.ToString(CultureInfo.InvariantCulture));
            resultSmall.Should().Be(small.ToString(CultureInfo.InvariantCulture));
            resultMedium1.Should().Be(medium1.ToString(CultureInfo.InvariantCulture));
            resultMedium2.Should().Be(medium2.ToString(CultureInfo.InvariantCulture));
            resultLarge.Should().Be(large.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void TestCase077_ToInvariantString_With_Nullable_Number_Should_Format_Correctly()
        {
            // Arrange
            int? value = 42;
            // Act
            var result = value.ToInvariantString();
            // Assert
            result.Should().Be("42");
        }

        [Fact]
        public void TestCase078_ToInvariantString_With_Enum_Should_Format_Correctly()
        {
            // Arrange
            DayOfWeek value = DayOfWeek.Monday;
            // Act
            var result = value.ToInvariantString();
            // Assert
            result.Should().Be("Monday");
        }

        [Fact]
        public void TestCase079_ToInvariantString_With_Half_Should_Work()
        {
            // Arrange
            Half value = (Half)3.14;

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("3.14");
        }


        #endregion

        #region Collection Edge Cases

        [Fact]
        public void TestCase080_ToInvariantString_With_Hashtable_Should_Format_As_Dictionary()
        {
            // Arrange
            var value = new Hashtable { ["key1"] = "value1", ["key2"] = "value2" };

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Contain("{").And.Contain("}");
            result.Should().Contain("key1").And.Contain("value1");
        }

        [Fact]
        public void TestCase081_ToInvariantString_With_ArrayList_Should_Format_As_Array()
        {
            // Arrange
            var value = new ArrayList { 1, 2, 3 };

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Be("[1, 2, 3] (3 items)");
        }

        [Fact]
        public void TestCase082_ToInvariantString_With_Jagged_Array_Should_Format_Correctly()
        {
            // Arrange
            int[][] value = [[1, 2], [3, 4, 5]];

            // Act
            var result = value.ToInvariantString();

            // Assert
            result.Should().Contain("[1, 2]").And.Contain("[3, 4, 5]");
        }

        #endregion

        #region ToInvariantString Method Tests - TypeConverters

        [Fact]
        public void TestCase083_ToInvariantString_With_TypeConverter_Should_Use_Converter()
        {
            
            // Arrange
            var value = new Person("Max Mustermann", 42);
            // Act
            var result = value.ToInvariantString();
            // Assert
            result.Should().Be("[TypeConverter] Max Mustermann (42)");
        }

        // Beispiel einer benutzerdefinierten Klasse mit TypeConverter
        [TypeConverter(typeof(PersonConverter))]
        internal class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public override string ToString()
            {
                return $"Person: {Name}, {Age} Jahre alt";
            }
        }

        // Benutzerdefinierter TypeConverter für Person
        internal class PersonConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            {
                // Sagt, dass wir zu string konvertieren können
                return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
            }

            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                if (destinationType == typeof(string) && value is Person person)
                {
                    // Spezielle Formatierung durch TypeConverter
                    return $"[TypeConverter] {person.Name} ({person.Age})";
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        [Fact]
        public void TestCase084_ToInvariantString_With_ClassToString_Should_Use_ToString()
        {
            // Arrange
            var value = new SimpleProduct("Max Mustermann", 42.2m);
            // Act
            var result = value.ToInvariantString();
            // Assert
            result.Should().Be("SimpleProduct: Max Mustermann - 42.2");
        }

        // Beispiel 2: Klasse OHNE TypeConverter (fällt auf ToString() zurück)
        internal class SimpleProduct : IFormattable
        {
            public string Name { get; set; }
            public decimal Price { get; set; }

            public SimpleProduct(string name, decimal price)
            {
                Name = name;
                Price = price;
            }

            public override string ToString()
            {
                return $"SimpleProduct: {Name} - {Price}";
            }

            public string ToString(string? format, IFormatProvider? formatProvider)
            {
                return string.Format(formatProvider, "SimpleProduct: {0} - {1}", Name, Price);
            }
        }

        [Fact]
        public void TestCase085_ToInvariantString_With_Broken_TypeConverter_Should_Return_Empty()
        {
            // Arrange
            var value = new BrokenItem("42");
            // Act
            var result = value.ToInvariantString();
            // Assert
            result.Should().Be(String.Empty);
        }

        // Beispiel 3: Klasse mit TypeConverter der eine Exception wirft
        [TypeConverter(typeof(BrokenConverter))]
        internal class BrokenItem
        {
            public string Value { get; set; }

            public BrokenItem(string value)
            {
                Value = value;
            }

            public override string? ToString()
            {
                return null;
            }
        }

        internal class BrokenConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                // Simuliert eine Exception im TypeConverter
                throw new InvalidOperationException("Converter ist kaputt!");
            }
        }

        [Fact]
        public void TestCase086_ToInvariantString_With_TypeConverter_That_Cannot_Convert_To_String_Should_Return_Empty()
        {
            // Arrange
            var value = new BrokenItem2("42");
            // Act
            var result = value.ToInvariantString();
            // Assert
            result.Should().Be(String.Empty);
        }

        // Beispiel 4: Klasse mit TypeConverter nicht nach string konvertieren kann
        [TypeConverter(typeof(BrokenConverter2))]
        internal class BrokenItem2
        {
            public string Value { get; set; }

            public BrokenItem2(string value)
            {
                Value = value;
            }

            public override string? ToString()
            {
                return null;
            }
        }

        internal class BrokenConverter2 : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            {
                return destinationType == typeof(int);
            }

            public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                // Simuliert eine Exception im TypeConverter
                throw new InvalidOperationException("Converter ist kaputt!");
            }
        }

        #endregion

        #region ToInvariantString Method Tests - Multidimensional Elements

        [Fact]
        public void TestCase087_ToInvariantString_With_Multidimensional_Array_Should_Format_Correctly()
        {
            // Arrange
            int[,,] value = new int[3, 4, 2];
            var options = new ToStringOptions { ShowArrayDimensions = true, MaxNestingDepth = value.Rank };
            // Act
            var result = value.ToInvariantString(options);
            // Assert
            result.Should().NotContain("<max nesting depth reached>");
        }

        [Fact]
        public void TestCase088_ToInvariantString_With_Multidimensional_Array_With_Rank_Greater_Than_MaxDepth_Show_Max_Nesting_Messages()
        {
            // Arrange
            int[,,] value = new int[3, 4, 2];
            var options = new ToStringOptions { ShowArrayDimensions = true, MaxNestingDepth = 2 };
            // Act
            var result = value.ToInvariantString(options);
            // Assert
            result.Should().Contain("<max nesting depth reached>");
        }

        [Fact]
        public void TestCase089_ToInvariantString_With_Multidimensional_Array_And_MaxDepth_Zero_Show_Max_Nesting_Message()
        {
            // Arrange
            int[,,] value = new int[3, 4, 2];
            var options = new ToStringOptions { ShowArrayDimensions = true, MaxNestingDepth = 0 };
            // Act
            var result = value.ToInvariantString(options);
            // Assert
            result.Should().Be("<max nesting depth reached>");
        }

        [Fact]
        public void TestCase090_ToInvariantString_With_2x2_Array_And_MaxDepth_1_Show_Max_Nesting_Message()
        {
            // Arrange
            int[,] value = new int[2,2];
            var options = new ToStringOptions { MaxNestingDepth = 1 };
            // Act
            var result = value.ToInvariantString(options);
            // Assert
            result.Should().Contain("<max nesting depth reached>");
        }

        
        [Fact]
        public void TestCase091_ToInvariantString_With_2x100_Array_And_MaxCollectionItems_5_Show_3_Points()
        {
            // Arrange
            int[,] value = new int[2, 100];
            var options = new ToStringOptions { MaxCollectionItems = 5 };
            // Act
            var result = value.ToInvariantString(options);
            // Assert
            result.Should().Contain("...");
        }

        [Fact]
        public void TestCase091_ToInvariantString_With_2x10x7_Array_And_MaxCollectionItems_5_Show_3_Points()
        {
            // Arrange
            int[,,] value = new int[2, 10, 7];
            var options = new ToStringOptions { MaxCollectionItems = 5 };
            // Act
            var result = value.ToInvariantString(options);
            // Assert
            result.Should().Contain("...");
        }

        #endregion
    }
}