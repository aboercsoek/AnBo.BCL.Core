using FluentAssertions;
using Xunit;
using AnBo.Core;
using System;

namespace AnBo.Test
{
    public class NumberFormatterUnitTest
    {
        #region ByteToBinaryString Tests

        [Fact]
        public void TestCase001_ByteToBinaryString_With_Zero_Should_Return_Zero()
        {
            // Arrange
            byte value = 0;

            // Act
            var result = NumberFormatter.ByteToBinaryString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase002_ByteToBinaryString_With_One_Should_Return_One()
        {
            // Arrange
            byte value = 1;

            // Act
            var result = NumberFormatter.ByteToBinaryString(value);

            // Assert
            result.Should().Be("1");
        }

        [Fact]
        public void TestCase003_ByteToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
        {
            // Arrange
            byte value = byte.MaxValue; // 255

            // Act
            var result = NumberFormatter.ByteToBinaryString(value);

            // Assert
            result.Should().Be("11111111");
        }

        [Fact]
        public void TestCase004_ByteToBinaryString_With_PowerOfTwo_Should_Return_Correct_Binary()
        {
            // Arrange
            byte value = 128; // 2^7

            // Act
            var result = NumberFormatter.ByteToBinaryString(value);

            // Assert
            result.Should().Be("10000000");
        }

        [Fact]
        public void TestCase005_ByteToBinaryString_With_Mixed_Bits_Should_Return_Correct_Binary()
        {
            // Arrange
            byte value = 85; // 01010101

            // Act
            var result = NumberFormatter.ByteToBinaryString(value);

            // Assert
            result.Should().Be("1010101");
        }

        #endregion

        #region Int16ToBinaryString Tests

        [Fact]
        public void TestCase006_Int16ToBinaryString_With_Zero_Should_Return_Zero()
        {
            // Arrange
            Int16 value = 0;

            // Act
            var result = NumberFormatter.Int16ToBinaryString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase007_Int16ToBinaryString_With_Positive_Value_Should_Return_Correct_Binary()
        {
            // Arrange
            Int16 value = 255;

            // Act
            var result = NumberFormatter.Int16ToBinaryString(value);

            // Assert
            result.Should().Be("11111111");
        }

        [Fact]
        public void TestCase008_Int16ToBinaryString_With_Negative_Value_Should_Return_Correct_Binary()
        {
            // Arrange
            Int16 value = -1;

            // Act
            var result = NumberFormatter.Int16ToBinaryString(value);

            // Assert
            result.Should().Be("1111111111111111");
        }

        [Fact]
        public void TestCase009_Int16ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
        {
            // Arrange
            Int16 value = Int16.MaxValue; // 32767

            // Act
            var result = NumberFormatter.Int16ToBinaryString(value);

            // Assert
            result.Should().Be("111111111111111");
        }

        [Fact]
        public void TestCase010_Int16ToBinaryString_With_MinValue_Should_Return_Correct_Binary()
        {
            // Arrange
            Int16 value = Int16.MinValue; // -32768

            // Act
            var result = NumberFormatter.Int16ToBinaryString(value);

            // Assert
            result.Should().Be("1000000000000000");
        }

        #endregion

        #region UInt16ToBinaryString Tests

        [Fact]
        public void TestCase011_UInt16ToBinaryString_With_Zero_Should_Return_Zero()
        {
            // Arrange
            UInt16 value = 0;

            // Act
            var result = NumberFormatter.UInt16ToBinaryString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase012_UInt16ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
        {
            // Arrange
            UInt16 value = UInt16.MaxValue; // 65535

            // Act
            var result = NumberFormatter.UInt16ToBinaryString(value);

            // Assert
            result.Should().Be("1111111111111111");
        }

        [Fact]
        public void TestCase013_UInt16ToBinaryString_With_PowerOfTwo_Should_Return_Correct_Binary()
        {
            // Arrange
            UInt16 value = 1024; // 2^10

            // Act
            var result = NumberFormatter.UInt16ToBinaryString(value);

            // Assert
            result.Should().Be("10000000000");
        }

        #endregion

        #region Int32ToBinaryString Tests

        [Fact]
        public void TestCase014_Int32ToBinaryString_With_Zero_Should_Return_Zero()
        {
            // Arrange
            Int32 value = 0;

            // Act
            var result = NumberFormatter.Int32ToBinaryString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase015_Int32ToBinaryString_With_Positive_Value_Should_Return_Correct_Binary()
        {
            // Arrange
            Int32 value = 255;

            // Act
            var result = NumberFormatter.Int32ToBinaryString(value);

            // Assert
            result.Should().Be("11111111");
        }

        [Fact]
        public void TestCase016_Int32ToBinaryString_With_Negative_Value_Should_Return_Correct_Binary()
        {
            // Arrange
            Int32 value = -1;

            // Act
            var result = NumberFormatter.Int32ToBinaryString(value);

            // Assert
            result.Should().Be("11111111111111111111111111111111");
        }

        [Fact]
        public void TestCase017_Int32ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
        {
            // Arrange
            Int32 value = Int32.MaxValue;

            // Act
            var result = NumberFormatter.Int32ToBinaryString(value);

            // Assert
            result.Should().Be("1111111111111111111111111111111");
        }

        [Fact]
        public void TestCase018_Int32ToBinaryString_With_MinValue_Should_Return_Correct_Binary()
        {
            // Arrange
            Int32 value = Int32.MinValue;

            // Act
            var result = NumberFormatter.Int32ToBinaryString(value);

            // Assert
            result.Should().Be("10000000000000000000000000000000");
        }

        #endregion

        #region UInt32ToBinaryString Tests

        [Fact]
        public void TestCase019_UInt32ToBinaryString_With_Zero_Should_Return_Zero()
        {
            // Arrange
            UInt32 value = 0;

            // Act
            var result = NumberFormatter.UInt32ToBinaryString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase020_UInt32ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
        {
            // Arrange
            UInt32 value = UInt32.MaxValue;

            // Act
            var result = NumberFormatter.UInt32ToBinaryString(value);

            // Assert
            result.Should().Be("11111111111111111111111111111111");
        }

        [Fact]
        public void TestCase021_UInt32ToBinaryString_With_PowerOfTwo_Should_Return_Correct_Binary()
        {
            // Arrange
            UInt32 value = 1048576; // 2^20

            // Act
            var result = NumberFormatter.UInt32ToBinaryString(value);

            // Assert
            result.Should().Be("100000000000000000000");
        }

        #endregion

        #region Int64ToBinaryString Tests

        [Fact]
        public void TestCase022_Int64ToBinaryString_With_Zero_Should_Return_Zero()
        {
            // Arrange
            Int64 value = 0;

            // Act
            var result = NumberFormatter.Int64ToBinaryString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase023_Int64ToBinaryString_With_Positive_Value_Should_Return_Correct_Binary()
        {
            // Arrange
            Int64 value = 255;

            // Act
            var result = NumberFormatter.Int64ToBinaryString(value);

            // Assert
            result.Should().Be("11111111");
        }

        [Fact]
        public void TestCase024_Int64ToBinaryString_With_Negative_Value_Should_Return_Correct_Binary()
        {
            // Arrange
            Int64 value = -1;

            // Act
            var result = NumberFormatter.Int64ToBinaryString(value);

            // Assert
            result.Should().Be("1111111111111111111111111111111111111111111111111111111111111111");
        }

        [Fact]
        public void TestCase025_Int64ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
        {
            // Arrange
            Int64 value = Int64.MaxValue;

            // Act
            var result = NumberFormatter.Int64ToBinaryString(value);

            // Assert
            result.Should().Be("111111111111111111111111111111111111111111111111111111111111111");
        }

        [Fact]
        public void TestCase026_Int64ToBinaryString_With_MinValue_Should_Return_Correct_Binary()
        {
            // Arrange
            Int64 value = Int64.MinValue;

            // Act
            var result = NumberFormatter.Int64ToBinaryString(value);

            // Assert
            result.Should().Be("1000000000000000000000000000000000000000000000000000000000000000");
        }

        #endregion

        #region UInt64ToBinaryString Tests

        [Fact]
        public void TestCase027_UInt64ToBinaryString_With_Zero_Should_Return_Zero()
        {
            // Arrange
            UInt64 value = 0;

            // Act
            var result = NumberFormatter.UInt64ToBinaryString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase028_UInt64ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
        {
            // Arrange
            UInt64 value = UInt64.MaxValue;

            // Act
            var result = NumberFormatter.UInt64ToBinaryString(value);

            // Assert
            result.Should().Be("1111111111111111111111111111111111111111111111111111111111111111");
        }

        [Fact]
        public void TestCase029_UInt64ToBinaryString_With_Large_Value_Should_Return_Correct_Binary()
        {
            // Arrange
            UInt64 value = 18446744073709551615; // UInt64.MaxValue

            // Act
            var result = NumberFormatter.UInt64ToBinaryString(value);

            // Assert
            result.Should().Be("1111111111111111111111111111111111111111111111111111111111111111");
        }

        #endregion

        #region Int16ToHexString Tests

        [Fact]
        public void TestCase030_Int16ToHexString_Single_Parameter_With_Zero_Should_Return_Zero()
        {
            // Arrange
            Int16 value = 0;

            // Act
            var result = NumberFormatter.Int16ToHexString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase031_Int16ToHexString_Single_Parameter_With_Positive_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            Int16 value = 255;

            // Act
            var result = NumberFormatter.Int16ToHexString(value);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase032_Int16ToHexString_Single_Parameter_With_Negative_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            Int16 value = -1;

            // Act
            var result = NumberFormatter.Int16ToHexString(value);

            // Assert
            result.Should().Be("ffff");
        }

        [Fact]
        public void TestCase033_Int16ToHexString_With_MinHexDigits_Should_Pad_Correctly()
        {
            // Arrange
            Int16 value = 15;
            int minHexDigits = 4;

            // Act
            var result = NumberFormatter.Int16ToHexString(value, minHexDigits);

            // Assert
            result.Should().Be("000f");
        }

        [Fact]
        public void TestCase034_Int16ToHexString_With_MinHexDigits_One_Should_Return_Minimum_Length()
        {
            // Arrange
            Int16 value = 255;
            int minHexDigits = 1;

            // Act
            var result = NumberFormatter.Int16ToHexString(value, minHexDigits);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase035_Int16ToHexString_With_ZeroX_Prefix_True_Should_Add_Prefix()
        {
            // Arrange
            Int16 value = 255;
            bool addZeroXPrefix = true;

            // Act
            var result = NumberFormatter.Int16ToHexString(value, addZeroXPrefix);

            // Assert
            result.Should().Be("0xff");
        }

        [Fact]
        public void TestCase036_Int16ToHexString_With_ZeroX_Prefix_False_Should_Not_Add_Prefix()
        {
            // Arrange
            Int16 value = 255;
            bool addZeroXPrefix = false;

            // Act
            var result = NumberFormatter.Int16ToHexString(value, addZeroXPrefix);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase037_Int16ToHexString_With_All_Parameters_Should_Format_Correctly()
        {
            // Arrange
            Int16 value = 15;
            int minHexDigits = 4;
            bool addZeroXPrefix = true;

            // Act
            var result = NumberFormatter.Int16ToHexString(value, minHexDigits, addZeroXPrefix);

            // Assert
            result.Should().Be("0x000f");
        }

        [Fact]
        public void TestCase038_Int16ToHexString_With_MaxValue_Should_Return_Correct_Hex()
        {
            // Arrange
            Int16 value = Int16.MaxValue; // 32767

            // Act
            var result = NumberFormatter.Int16ToHexString(value);

            // Assert
            result.Should().Be("7fff");
        }

        [Fact]
        public void TestCase039_Int16ToHexString_With_MinValue_Should_Return_Correct_Hex()
        {
            // Arrange
            Int16 value = Int16.MinValue; // -32768

            // Act
            var result = NumberFormatter.Int16ToHexString(value);

            // Assert
            result.Should().Be("8000");
        }

        #endregion

        #region Int32ToHexString Tests

        [Fact]
        public void TestCase040_Int32ToHexString_Single_Parameter_With_Zero_Should_Return_Zero()
        {
            // Arrange
            int value = 0;

            // Act
            var result = NumberFormatter.Int32ToHexString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase041_Int32ToHexString_Single_Parameter_With_Positive_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            int value = 255;

            // Act
            var result = NumberFormatter.Int32ToHexString(value);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase042_Int32ToHexString_Single_Parameter_With_Negative_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            int value = -1;

            // Act
            var result = NumberFormatter.Int32ToHexString(value);

            // Assert
            result.Should().Be("ffffffff");
        }

        [Fact]
        public void TestCase043_Int32ToHexString_With_MinHexDigits_Should_Pad_Correctly()
        {
            // Arrange
            int value = 15;
            int minHexDigits = 8;

            // Act
            var result = NumberFormatter.Int32ToHexString(value, minHexDigits);

            // Assert
            result.Should().Be("0000000f");
        }

        [Fact]
        public void TestCase044_Int32ToHexString_With_ZeroX_Prefix_True_Should_Add_Prefix()
        {
            // Arrange
            int value = 255;
            bool addZeroXPrefix = true;

            // Act
            var result = NumberFormatter.Int32ToHexString(value, addZeroXPrefix);

            // Assert
            result.Should().Be("0xff");
        }

        [Fact]
        public void TestCase045_Int32ToHexString_With_ZeroX_Prefix_False_Should_Not_Add_Prefix()
        {
            // Arrange
            int value = 255;
            bool addZeroXPrefix = false;

            // Act
            var result = NumberFormatter.Int32ToHexString(value, addZeroXPrefix);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase046_Int32ToHexString_With_All_Parameters_Should_Format_Correctly()
        {
            // Arrange
            int value = 15;
            int minHexDigits = 8;
            bool addZeroXPrefix = true;

            // Act
            var result = NumberFormatter.Int32ToHexString(value, minHexDigits, addZeroXPrefix);

            // Assert
            result.Should().Be("0x0000000f");
        }

        [Fact]
        public void TestCase047_Int32ToHexString_With_MaxValue_Should_Return_Correct_Hex()
        {
            // Arrange
            int value = Int32.MaxValue;

            // Act
            var result = NumberFormatter.Int32ToHexString(value);

            // Assert
            result.Should().Be("7fffffff");
        }

        [Fact]
        public void TestCase048_Int32ToHexString_With_MinValue_Should_Return_Correct_Hex()
        {
            // Arrange
            int value = Int32.MinValue;

            // Act
            var result = NumberFormatter.Int32ToHexString(value);

            // Assert
            result.Should().Be("80000000");
        }

        [Fact]
        public void TestCase049_Int32ToHexString_With_Large_Positive_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            int value = 0x12345678;

            // Act
            var result = NumberFormatter.Int32ToHexString(value);

            // Assert
            result.Should().Be("12345678");
        }

        #endregion

        #region Int64ToHexString Tests

        [Fact]
        public void TestCase050_Int64ToHexString_Single_Parameter_With_Zero_Should_Return_Zero()
        {
            // Arrange
            long value = 0;

            // Act
            var result = NumberFormatter.Int64ToHexString(value);

            // Assert
            result.Should().Be("0");
        }

        [Fact]
        public void TestCase051_Int64ToHexString_Single_Parameter_With_Positive_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            long value = 255;

            // Act
            var result = NumberFormatter.Int64ToHexString(value);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase052_Int64ToHexString_Single_Parameter_With_Negative_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            long value = -1;

            // Act
            var result = NumberFormatter.Int64ToHexString(value);

            // Assert
            result.Should().Be("ffffffffffffffff");
        }

        [Fact]
        public void TestCase053_Int64ToHexString_With_MinHexDigits_Should_Pad_Correctly()
        {
            // Arrange
            long value = 15;
            int minHexDigits = 16;

            // Act
            var result = NumberFormatter.Int64ToHexString(value, minHexDigits);

            // Assert
            result.Should().Be("000000000000000f");
        }

        [Fact]
        public void TestCase054_Int64ToHexString_With_ZeroX_Prefix_True_Should_Add_Prefix()
        {
            // Arrange
            long value = 255;
            bool addZeroXPrefix = true;

            // Act
            var result = NumberFormatter.Int64ToHexString(value, addZeroXPrefix);

            // Assert
            result.Should().Be("0xff");
        }

        [Fact]
        public void TestCase055_Int64ToHexString_With_ZeroX_Prefix_False_Should_Not_Add_Prefix()
        {
            // Arrange
            long value = 255;
            bool addZeroXPrefix = false;

            // Act
            var result = NumberFormatter.Int64ToHexString(value, addZeroXPrefix);

            // Assert
            result.Should().Be("ff");
        }

        [Fact]
        public void TestCase056_Int64ToHexString_With_All_Parameters_Should_Format_Correctly()
        {
            // Arrange
            long value = 15;
            int minHexDigits = 16;
            bool addZeroXPrefix = true;

            // Act
            var result = NumberFormatter.Int64ToHexString(value, minHexDigits, addZeroXPrefix);

            // Assert
            result.Should().Be("0x000000000000000f");
        }

        [Fact]
        public void TestCase057_Int64ToHexString_With_MaxValue_Should_Return_Correct_Hex()
        {
            // Arrange
            long value = Int64.MaxValue;

            // Act
            var result = NumberFormatter.Int64ToHexString(value);

            // Assert
            result.Should().Be("7fffffffffffffff");
        }

        [Fact]
        public void TestCase058_Int64ToHexString_With_MinValue_Should_Return_Correct_Hex()
        {
            // Arrange
            long value = Int64.MinValue;

            // Act
            var result = NumberFormatter.Int64ToHexString(value);

            // Assert
            result.Should().Be("8000000000000000");
        }

        [Fact]
        public void TestCase059_Int64ToHexString_With_Large_Positive_Value_Should_Return_Correct_Hex()
        {
            // Arrange
            long value = 0x123456789ABCDEF0;

            // Act
            var result = NumberFormatter.Int64ToHexString(value);

            // Assert
            result.Should().Be("123456789abcdef0");
        }

        [Fact]
        public void TestCase060_Int64ToHexString_With_Small_MinHexDigits_Should_Use_Actual_Length()
        {
            // Arrange
            long value = 0x123456789ABCDEF0;
            int minHexDigits = 1;

            // Act
            var result = NumberFormatter.Int64ToHexString(value, minHexDigits);

            // Assert
            result.Should().Be("123456789abcdef0");
        }

        #endregion

        #region Edge Cases and Boundary Tests

        [Fact]
        public void TestCase061_All_Binary_Methods_With_One_Should_Return_One()
        {
            // Act & Assert
            NumberFormatter.ByteToBinaryString(1).Should().Be("1");
            NumberFormatter.Int16ToBinaryString(1).Should().Be("1");
            NumberFormatter.UInt16ToBinaryString(1).Should().Be("1");
            NumberFormatter.Int32ToBinaryString(1).Should().Be("1");
            NumberFormatter.UInt32ToBinaryString(1).Should().Be("1");
            NumberFormatter.Int64ToBinaryString(1).Should().Be("1");
            NumberFormatter.UInt64ToBinaryString(1).Should().Be("1");
        }

        [Fact]
        public void TestCase062_All_Hex_Methods_With_Zero_Should_Return_Zero()
        {
            // Act & Assert
            NumberFormatter.Int16ToHexString(0).Should().Be("0");
            NumberFormatter.Int32ToHexString(0).Should().Be("0");
            NumberFormatter.Int64ToHexString(0).Should().Be("0");
        }

        [Fact]
        public void TestCase063_All_Hex_Methods_With_One_Should_Return_One()
        {
            // Act & Assert
            NumberFormatter.Int16ToHexString(1).Should().Be("1");
            NumberFormatter.Int32ToHexString(1).Should().Be("1");
            NumberFormatter.Int64ToHexString(1).Should().Be("1");
        }

        [Fact]
        public void TestCase064_Hex_Methods_With_Fifteen_Should_Return_f()
        {
            // Act & Assert
            NumberFormatter.Int16ToHexString(15).Should().Be("f");
            NumberFormatter.Int32ToHexString(15).Should().Be("f");
            NumberFormatter.Int64ToHexString(15).Should().Be("f");
        }

        [Fact]
        public void TestCase065_Hex_Methods_With_Sixteen_Should_Return_10()
        {
            // Act & Assert
            NumberFormatter.Int16ToHexString(16).Should().Be("10");
            NumberFormatter.Int32ToHexString(16).Should().Be("10");
            NumberFormatter.Int64ToHexString(16).Should().Be("10");
        }

        [Fact]
        public void TestCase066_Binary_Methods_With_Two_Should_Return_10()
        {
            // Act & Assert
            NumberFormatter.ByteToBinaryString(2).Should().Be("10");
            NumberFormatter.Int16ToBinaryString(2).Should().Be("10");
            NumberFormatter.UInt16ToBinaryString(2).Should().Be("10");
            NumberFormatter.Int32ToBinaryString(2).Should().Be("10");
            NumberFormatter.UInt32ToBinaryString(2).Should().Be("10");
            NumberFormatter.Int64ToBinaryString(2).Should().Be("10");
            NumberFormatter.UInt64ToBinaryString(2).Should().Be("10");
        }

        [Fact]
        public void TestCase067_Hex_Methods_With_Prefix_And_Zero_Should_Add_Prefix()
        {
            // Act & Assert
            NumberFormatter.Int16ToHexString(0, true).Should().Be("0x0");
            NumberFormatter.Int32ToHexString(0, true).Should().Be("0x0");
            NumberFormatter.Int64ToHexString(0, true).Should().Be("0x0");
        }

        [Fact]
        public void TestCase068_Hex_Methods_With_Large_MinHexDigits_Should_Pad_Correctly()
        {
            // Act & Assert
            NumberFormatter.Int16ToHexString(1, 4).Should().Be("0001");
            NumberFormatter.Int32ToHexString(1, 8).Should().Be("00000001");
            NumberFormatter.Int64ToHexString(1, 16).Should().Be("0000000000000001");
        }

        #endregion
    }
}