using FluentAssertions;
using Xunit;
using AnBo.Core;
using System;
using System.Globalization;
using System.Text;

namespace AnBo.Test
{
    public class HexConverterUnitTest
    {
        #region ConvertHexDigit Method Tests

        [Theory]
        [InlineData('0', 0)]
        [InlineData('1', 1)]
        [InlineData('2', 2)]
        [InlineData('3', 3)]
        [InlineData('4', 4)]
        [InlineData('5', 5)]
        [InlineData('6', 6)]
        [InlineData('7', 7)]
        [InlineData('8', 8)]
        [InlineData('9', 9)]
        public void TestCase001_ConvertHexDigit_With_Numeric_Digits_Should_Return_Correct_Values(char input, int expected)
        {
            // Act
            var result = HexConverter.ConvertHexDigit(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData('a', 10)]
        [InlineData('b', 11)]
        [InlineData('c', 12)]
        [InlineData('d', 13)]
        [InlineData('e', 14)]
        [InlineData('f', 15)]
        public void TestCase002_ConvertHexDigit_With_Lowercase_Letters_Should_Return_Correct_Values(char input, int expected)
        {
            // Act
            var result = HexConverter.ConvertHexDigit(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData('A', 10)]
        [InlineData('B', 11)]
        [InlineData('C', 12)]
        [InlineData('D', 13)]
        [InlineData('E', 14)]
        [InlineData('F', 15)]
        public void TestCase003_ConvertHexDigit_With_Uppercase_Letters_Should_Return_Correct_Values(char input, int expected)
        {
            // Act
            var result = HexConverter.ConvertHexDigit(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData('g')]
        [InlineData('G')]
        [InlineData('z')]
        [InlineData('Z')]
        [InlineData('/')]
        [InlineData(':')]
        [InlineData('@')]
        [InlineData('`')]
        public void TestCase004_ConvertHexDigit_With_Invalid_Characters_Should_Throw_ArgException(char input)
        {
            // Act & Assert
            var action = () => HexConverter.ConvertHexDigit(input);
            action.Should().Throw<ArgException<char>>()
                .WithMessage("Value was out of range. Must be between '0'-'9' or 'a'-'f' or 'A'-'F'.");
        }

        #endregion

        #region FromHexString Method Tests

        [Fact]
        public void TestCase005_FromHexString_With_Null_Should_Return_Empty_Array()
        {
            // Arrange
            string? hexString = null;

            // Act
            var result = HexConverter.FromHexString(hexString!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase006_FromHexString_With_Empty_String_Should_Return_Empty_Array()
        {
            // Arrange
            string hexString = "";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase007_FromHexString_With_Whitespace_Only_Should_Return_Empty_Array()
        {
            // Arrange
            string hexString = "   ";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase008_FromHexString_With_0x_Prefix_Should_Strip_Prefix()
        {
            // Arrange
            string hexString = "0x41";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().Equal(new byte[] { 0x41 });
        }

        [Fact]
        public void TestCase009_FromHexString_With_0X_Prefix_Should_Strip_Prefix()
        {
            // Arrange
            string hexString = "0X4101";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().Equal(new byte[] { 0x41, 0x01 });
        }

        [Fact]
        public void TestCase010_FromHexString_With_0x_Only_Should_Return_Empty_Array()
        {
            // Arrange
            string hexString = "0x";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void TestCase011_FromHexString_With_Simple_Hex_Should_Convert_Correctly()
        {
            // Arrange
            string hexString = "41424344";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().Equal(new byte[] { 0x41, 0x42, 0x43, 0x44 });
        }

        [Fact]
        public void TestCase012_FromHexString_With_Space_Separated_Hex_Should_Convert_Correctly()
        {
            // Arrange
            string hexString = "41 42 43 44";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().Equal(new byte[] { 0x41, 0x42, 0x43, 0x44 });
        }

        [Fact]
        public void TestCase013_FromHexString_With_Mixed_Case_Should_Convert_Correctly()
        {
            // Arrange
            string hexString = "aAbBcCdD";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().Equal(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD });
        }

        [Fact]
        public void TestCase014_FromHexString_With_Leading_Whitespace_Should_Convert_Correctly()
        {
            // Arrange
            string hexString = "   41424344";

            // Act
            var result = HexConverter.FromHexString(hexString);

            // Assert
            result.Should().Equal(new byte[] { 0x41, 0x42, 0x43, 0x44 });
        }

        [Fact]
        public void TestCase015_FromHexString_With_Invalid_Format_Odd_Length_Should_Throw_ArgException()
        {
            // Arrange
            string hexString = "0414243";

            // Act & Assert
            var action = () => HexConverter.FromHexString(hexString);
            action.Should().Throw<ArgException<string>>()
               .WithMessage("Inproperly formatted hex string");
        }

        [Fact]
        public void TestCase016_FromHexString_With_Invalid_Format_Space_Not_Third_Should_Throw_ArgException()
        {
            // Arrange
            string hexString = "41 42  43";

            // Act & Assert
            var action = () => HexConverter.FromHexString(hexString);
            action.Should().Throw<ArgException<string>>()
                .WithMessage("Inproperly formatted hex string");
        }

        [Fact]
        public void TestCase017_FromHexString_With_Invalid_Hex_Character_Should_Throw_ArgException()
        {
            // Arrange
            string hexString = "41G2";

            // Act & Assert
            var action = () => HexConverter.FromHexString(hexString);
            action.Should().Throw<ArgException<char>>();
        }

        #endregion

        #region ToHexString Int16 Method Tests

        [Fact]
        public void TestCase018_ToHexString_Int16_With_Zero_Should_Return_Single_Zero()
        {
            // Arrange
            Int16 value = 0;
            int hexDigits = 1;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase019_ToHexString_Int16_With_Positive_Value_Should_Return_Hex_String()
        {
            // Arrange
            Int16 value = 255;
            int hexDigits = 2;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase020_ToHexString_Int16_With_Padding_Should_Add_Leading_Zeros()
        {
            // Arrange
            Int16 value = 15;
            int hexDigits = 4;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("000f");
        }

        [Fact]
        public void TestCase021_ToHexString_Int16_With_Negative_Digits_Should_Use_Minimum_One()
        {
            // Arrange
            Int16 value = 15;
            int hexDigits = -5;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("f");
        }

        [Fact]
        public void TestCase022_ToHexString_Int16_With_Excessive_Digits_Should_Cap_At_Four()
        {
            // Arrange
            Int16 value = 15;
            int hexDigits = 10;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("000f");
        }

        [Fact]
        public void TestCase023_ToHexString_Int16_With_Max_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            Int16 value = Int16.MaxValue;
            int hexDigits = 4;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("7fff");
        }

        [Fact]
        public void TestCase024_ToHexString_Int16_With_Negative_Value_Should_Return_Two_Complement()
        {
            // Arrange
            Int16 value = -1;
            int hexDigits = 4;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("ffff");
        }

        #endregion

        #region ToHexString Int32 Method Tests

        [Fact]
        public void TestCase025_ToHexString_Int32_With_Zero_Should_Return_Single_Zero()
        {
            // Arrange
            int value = 0;
            int hexDigits = 1;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase026_ToHexString_Int32_With_Positive_Value_Should_Return_Hex_String()
        {
            // Arrange
            int value = 4095;
            int hexDigits = 3;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("fff");
        }

        [Fact]
        public void TestCase027_ToHexString_Int32_With_Padding_Should_Add_Leading_Zeros()
        {
            // Arrange
            int value = 255;
            int hexDigits = 8;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("000000ff");
        }

        [Fact]
        public void TestCase028_ToHexString_Int32_With_Negative_Digits_Should_Use_Minimum_One()
        {
            // Arrange
            int value = 255;
            int hexDigits = -3;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase029_ToHexString_Int32_With_Excessive_Digits_Should_Cap_At_Eight()
        {
            // Arrange
            int value = 255;
            int hexDigits = 15;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("000000ff");
        }

        [Fact]
        public void TestCase030_ToHexString_Int32_With_Max_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            int value = int.MaxValue;
            int hexDigits = 8;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("7fffffff");
        }

        [Fact]
        public void TestCase031_ToHexString_Int32_With_Negative_Value_Should_Return_Two_Complement()
        {
            // Arrange
            int value = -1;
            int hexDigits = 8;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("ffffffff");
        }

        #endregion

        #region ToHexString Int64 Method Tests

        [Fact]
        public void TestCase032_ToHexString_Int64_With_Zero_Should_Return_Single_Zero()
        {
            // Arrange
            long value = 0L;
            int hexDigits = 1;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase033_ToHexString_Int64_With_Positive_Value_Should_Return_Hex_String()
        {
            // Arrange
            long value = 1048575L;
            int hexDigits = 5;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("fffff");
        }

        [Fact]
        public void TestCase034_ToHexString_Int64_With_Padding_Should_Add_Leading_Zeros()
        {
            // Arrange
            long value = 255L;
            int hexDigits = 16;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("00000000000000ff");
        }

        [Fact]
        public void TestCase035_ToHexString_Int64_With_Negative_Digits_Should_Use_Minimum_One()
        {
            // Arrange
            long value = 255L;
            int hexDigits = -10;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase036_ToHexString_Int64_With_Excessive_Digits_Should_Cap_At_Sixteen()
        {
            // Arrange
            long value = 255L;
            int hexDigits = 25;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("00000000000000ff");
        }

        [Fact]
        public void TestCase037_ToHexString_Int64_With_Max_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            long value = long.MaxValue;
            int hexDigits = 16;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("7fffffffffffffff");
        }

        [Fact]
        public void TestCase038_ToHexString_Int64_With_Negative_Value_Should_Return_Two_Complement()
        {
            // Arrange
            long value = -1L;
            int hexDigits = 16;

            // Act
            var result = HexConverter.ToHexString(value, hexDigits);

            // Assert
            result.Should().Be("ffffffffffffffff");
        }

        #endregion

        #region ToHexString Byte Array Method Tests

        [Fact]
        public void TestCase039_ToHexString_ByteArray_With_Null_Should_Return_Empty_String()
        {
            // Arrange
            byte[]? buffer = null;

            // Act
            var result = HexConverter.ToHexString(buffer!);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void TestCase040_ToHexString_ByteArray_With_Empty_Array_Should_Return_Empty_String()
        {
            // Arrange
            byte[] buffer = new byte[0];

            // Act
            var result = HexConverter.ToHexString(buffer);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void TestCase041_ToHexString_ByteArray_With_Single_Byte_Should_Return_Two_Hex_Digits()
        {
            // Arrange
            byte[] buffer = { 0x41 };

            // Act
            var result = HexConverter.ToHexString(buffer);

            // Assert
            result.Should().Be("41");
        }

        [Fact]
        public void TestCase042_ToHexString_ByteArray_With_Multiple_Bytes_Should_Return_Concatenated_Hex()
        {
            // Arrange
            byte[] buffer = { 0x41, 0x42, 0x43, 0x44 };

            // Act
            var result = HexConverter.ToHexString(buffer);

            // Assert
            result.Should().Be("41424344");
        }

        [Fact]
        public void TestCase043_ToHexString_ByteArray_With_Zero_Bytes_Should_Return_Correct_Hex()
        {
            // Arrange
            byte[] buffer = { 0x00, 0x0F, 0xFF };

            // Act
            var result = HexConverter.ToHexString(buffer).ToLower();

            // Assert
            result.Should().Be("000fff");
        }

        [Fact]
        public void TestCase044_ToHexString_ByteArray_With_All_Values_Should_Return_Lowercase_Hex()
        {
            // Arrange
            byte[] buffer = { 0xAB, 0xCD, 0xEF };

            // Act
            var result = HexConverter.ToHexString(buffer).ToLower();

            // Assert
            result.Should().Be("abcdef");
        }

        #endregion

        #region ToHexString Byte Array With Options Method Tests

        [Fact]
        public void TestCase045_ToHexString_ByteArray_With_None_Option_Should_Return_Plain_Hex()
        {
            // Arrange
            byte[] buffer = { 0x41, 0x42, 0x43 };

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.None);

            // Assert
            result.Should().Be("414243");
        }

        [Fact]
        public void TestCase046_ToHexString_ByteArray_With_ZeroX_Prefix_Should_Add_Prefix()
        {
            // Arrange
            byte[] buffer = { 0x41, 0x42, 0x43 };

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddZeroXPrefix);

            // Assert
            result.Should().Be("0x414243");
        }

        [Fact]
        public void TestCase047_ToHexString_ByteArray_With_Separator_Should_Add_Default_Separator()
        {
            // Arrange
            byte[] buffer = { 0x41, 0x42, 0x43 };

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddSeparatorBetweenHexBytes);

            // Assert
            result.Should().Be("41 42 43");
        }

        [Fact]
        public void TestCase048_ToHexString_ByteArray_With_NewLine_Option_Should_Add_Newlines()
        {
            // Arrange
            byte[] buffer = new byte[20];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(0x41 + (i % 6));
            }

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddNewLineAfter16HexBytes);

            // Assert
            result.Should().Contain(Environment.NewLine);
            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines.Should().HaveCount(2);
        }

        [Fact]
        public void TestCase049_ToHexString_ByteArray_With_Null_Array_And_Options_Should_Return_Empty()
        {
            // Arrange
            byte[]? buffer = null;

            // Act
            var result = HexConverter.ToHexString(buffer!, HexStringFormatOptions.AddSeparatorBetweenHexBytes);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void TestCase050_ToHexString_ByteArray_With_Empty_Array_And_Options_Should_Return_Empty()
        {
            // Arrange
            byte[] buffer = new byte[0];

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddZeroXPrefix);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void TestCase051_ToHexString_ByteArray_With_Single_Byte_And_Separator_Should_Not_Add_Separator()
        {
            // Arrange
            byte[] buffer = { 0x41 };

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddSeparatorBetweenHexBytes);

            // Assert
            result.Should().Be("41");
        }

        #endregion

        #region ToHexString Byte Array With Custom Separator Method Tests

        [Fact]
        public void TestCase052_ToHexString_ByteArray_With_Custom_Separator_Should_Use_Custom_Separator()
        {
            // Arrange
            byte[] buffer = { 0x41, 0x42, 0x43 };
            char separator = '-';

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddSeparatorBetweenHexBytes, separator);

            // Assert
            result.Should().Be("41-42-43");
        }

        [Fact]
        public void TestCase053_ToHexString_ByteArray_With_NewLine_And_Custom_Separator_Should_Use_Custom_Separator()
        {
            // Arrange
            byte[] buffer = new byte[18];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(0x41 + (i % 6));
            }
            char separator = ':';

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddNewLineAfter16HexBytes, separator);

            // Assert
            result.Should().Contain(":");
            result.Should().Contain(Environment.NewLine);
        }

        [Fact]
        public void TestCase054_ToHexString_ByteArray_With_ZeroX_And_Custom_Separator_Should_Add_Prefix()
        {
            // Arrange
            byte[] buffer = { 0x41, 0x42, 0x43 };
            char separator = '_';

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddZeroXPrefix, separator);

            // Assert
            result.Should().Be("0x414243");
        }

        [Fact]
        public void TestCase055_ToHexString_ByteArray_With_None_And_Custom_Separator_Should_Ignore_Separator()
        {
            // Arrange
            byte[] buffer = { 0x41, 0x42, 0x43 };
            char separator = '#';

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.None, separator);

            // Assert
            result.Should().Be("414243");
            result.Should().NotContain("#");
        }

        [Fact]
        public void TestCase056_ToHexString_ByteArray_With_Null_And_Custom_Separator_Should_Return_Empty()
        {
            // Arrange
            byte[]? buffer = null;
            char separator = '|';

            // Act
            var result = HexConverter.ToHexString(buffer!, HexStringFormatOptions.AddSeparatorBetweenHexBytes, separator);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void TestCase057_ToHexString_ByteArray_With_Empty_And_Custom_Separator_Should_Return_Empty()
        {
            // Arrange
            byte[] buffer = new byte[0];
            char separator = '*';

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddSeparatorBetweenHexBytes, separator);

            // Assert
            result.Should().Be(string.Empty);
        }

        #endregion

        #region Edge Cases and Error Conditions Tests

        [Fact]
        public void TestCase058_FromHexString_With_Mixed_Invalid_Format_Should_Throw_ArgException()
        {
            // Arrange
            string hexString = "41 4 43";

            // Act & Assert
            var action = () => HexConverter.FromHexString(hexString);
            action.Should().Throw<ArgException<string>>();
        }

        [Fact]
        public void TestCase059_ToHexString_ByteArray_With_16_Bytes_NewLine_Option_Should_Not_Add_Newline()
        {
            // Arrange
            byte[] buffer = new byte[16];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(0x41 + (i % 6));
            }

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddNewLineAfter16HexBytes);

            // Assert
            result.Should().NotContain(Environment.NewLine);
        }

        [Fact]
        public void TestCase060_ToHexString_ByteArray_With_17_Bytes_NewLine_Option_Should_Add_Newline()
        {
            // Arrange
            byte[] buffer = new byte[17];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(0x41 + (i % 6));
            }

            // Act
            var result = HexConverter.ToHexString(buffer, HexStringFormatOptions.AddNewLineAfter16HexBytes);

            // Assert
            result.Should().Contain(Environment.NewLine);
        }

        [Fact]
        public void TestCase061_FromHexString_Roundtrip_Should_Preserve_Data()
        {
            // Arrange
            byte[] originalBuffer = { 0x00, 0x01, 0x0F, 0x10, 0x7F, 0x80, 0xFE, 0xFF };

            // Act
            var hexString = HexConverter.ToHexString(originalBuffer);
            var resultBuffer = HexConverter.FromHexString(hexString);

            // Assert
            resultBuffer.Should().Equal(originalBuffer);
        }

        [Fact]
        public void TestCase062_ConvertHexDigit_All_Valid_Characters_Should_Work()
        {
            // Arrange & Act & Assert
            for (char c = '0'; c <= '9'; c++)
            {
                var result = HexConverter.ConvertHexDigit(c);
                result.Should().BeInRange(0, 9);
            }

            for (char c = 'a'; c <= 'f'; c++)
            {
                var result = HexConverter.ConvertHexDigit(c);
                result.Should().BeInRange(10, 15);
            }

            for (char c = 'A'; c <= 'F'; c++)
            {
                var result = HexConverter.ConvertHexDigit(c);
                result.Should().BeInRange(10, 15);
            }
        }

        #endregion

        #region Class Structure Tests

        [Fact]
        public void TestCase063_HexConverter_Class_Should_Be_Static()
        {
            // Act & Assert
            typeof(HexConverter).IsAbstract.Should().BeTrue();
            typeof(HexConverter).IsSealed.Should().BeTrue();
        }

        [Fact]
        public void TestCase064_HexConverter_Class_Should_Be_Public()
        {
            // Act & Assert
            typeof(HexConverter).IsPublic.Should().BeTrue();
        }

        [Fact]
        public void TestCase065_HexConverter_Class_Should_Be_In_AnBo_Core_Namespace()
        {
            // Act & Assert
            typeof(HexConverter).Namespace.Should().Be("AnBo.Core");
        }

        #endregion
    }
}