using FluentAssertions;
using AnBo.Core;

namespace AnBo.Test;

public class NumberFormatterUnitTest
{
    #region Byte ToBinaryString Tests

    [Fact]
    public void ByteToBinaryString_With_Zero_Should_Return_Zero()
    {
        // Arrange
        byte value = 0;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void ByteToBinaryString_With_One_Should_Return_One()
    {
        // Arrange
        byte value = 1;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1");
    }

    [Fact]
    public void ByteToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
    {
        // Arrange
        byte value = byte.MaxValue; // 255

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("11111111");
    }

    [Fact]
    public void ByteToBinaryString_With_PowerOfTwo_Should_Return_Correct_Binary()
    {
        // Arrange
        byte value = 128; // 2^7

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("10000000");
    }

    [Fact]
    public void ByteToBinaryString_With_Mixed_Bits_Should_Return_Correct_Binary()
    {
        // Arrange
        byte value = 85; // 01010101

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1010101");
    }

    #endregion

    #region Int16 ToBinaryString Tests

    [Fact]
    public void Int16ToBinaryString_With_Zero_Should_Return_Zero()
    {
        // Arrange
        Int16 value = 0;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void Int16ToBinaryString_With_Positive_Value_Should_Return_Correct_Binary()
    {
        // Arrange
        Int16 value = 255;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("11111111");
    }

    [Fact]
    public void Int16ToBinaryString_With_Negative_Value_Should_Return_Correct_Binary()
    {
        // Arrange
        Int16 value = -1;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1111111111111111");
    }

    [Fact]
    public void Int16ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
    {
        // Arrange
        Int16 value = Int16.MaxValue; // 32767

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("111111111111111");
    }

    [Fact]
    public void Int16ToBinaryString_With_MinValue_Should_Return_Correct_Binary()
    {
        // Arrange
        Int16 value = Int16.MinValue; // -32768

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1000000000000000");
    }

    #endregion

    #region UInt16 ToBinaryString Tests

    [Fact]
    public void UInt16ToBinaryString_With_Zero_Should_Return_Zero()
    {
        // Arrange
        UInt16 value = 0;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void UInt16ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
    {
        // Arrange
        UInt16 value = UInt16.MaxValue; // 65535

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1111111111111111");
    }

    [Fact]
    public void UInt16ToBinaryString_With_PowerOfTwo_Should_Return_Correct_Binary()
    {
        // Arrange
        UInt16 value = 1024; // 2^10

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("10000000000");
    }

    #endregion

    #region Int32 ToBinaryString Tests

    [Fact]
    public void Int32ToBinaryString_With_Zero_Should_Return_Zero()
    {
        // Arrange
        Int32 value = 0;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void Int32ToBinaryString_With_Positive_Value_Should_Return_Correct_Binary()
    {
        // Arrange
        Int32 value = 255;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("11111111");
    }

    [Fact]
    public void Int32ToBinaryString_With_Negative_Value_Should_Return_Correct_Binary()
    {
        // Arrange
        Int32 value = -1;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("11111111111111111111111111111111");
    }

    [Fact]
    public void Int32ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
    {
        // Arrange
        Int32 value = Int32.MaxValue;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1111111111111111111111111111111");
    }

    [Fact]
    public void Int32ToBinaryString_With_MinValue_Should_Return_Correct_Binary()
    {
        // Arrange
        Int32 value = Int32.MinValue;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("10000000000000000000000000000000");
    }

    #endregion

    #region UInt32 ToBinaryString Tests

    [Fact]
    public void UInt32ToBinaryString_With_Zero_Should_Return_Zero()
    {
        // Arrange
        UInt32 value = 0;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void UInt32ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
    {
        // Arrange
        UInt32 value = UInt32.MaxValue;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("11111111111111111111111111111111");
    }

    [Fact]
    public void UInt32ToBinaryString_With_PowerOfTwo_Should_Return_Correct_Binary()
    {
        // Arrange
        UInt32 value = 1048576; // 2^20

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("100000000000000000000");
    }

    #endregion

    #region Int64 ToBinaryString Tests

    [Fact]
    public void Int64ToBinaryString_With_Zero_Should_Return_Zero()
    {
        // Arrange
        Int64 value = 0;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void Int64ToBinaryString_With_Positive_Value_Should_Return_Correct_Binary()
    {
        // Arrange
        Int64 value = 255;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("11111111");
    }

    [Fact]
    public void Int64ToBinaryString_With_Negative_Value_Should_Return_Correct_Binary()
    {
        // Arrange
        Int64 value = -1;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1111111111111111111111111111111111111111111111111111111111111111");
    }

    [Fact]
    public void Int64ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
    {
        // Arrange
        Int64 value = Int64.MaxValue;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("111111111111111111111111111111111111111111111111111111111111111");
    }

    [Fact]
    public void Int64ToBinaryString_With_MinValue_Should_Return_Correct_Binary()
    {
        // Arrange
        Int64 value = Int64.MinValue;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1000000000000000000000000000000000000000000000000000000000000000");
    }

    #endregion

    #region UInt64 ToBinaryString Tests

    [Fact]
    public void UInt64ToBinaryString_With_Zero_Should_Return_Zero()
    {
        // Arrange
        UInt64 value = 0;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void UInt64ToBinaryString_With_MaxValue_Should_Return_Correct_Binary()
    {
        // Arrange
        UInt64 value = UInt64.MaxValue;

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1111111111111111111111111111111111111111111111111111111111111111");
    }

    [Fact]
    public void UInt64ToBinaryString_With_Large_Value_Should_Return_Correct_Binary()
    {
        // Arrange
        UInt64 value = 18446744073709551615; // UInt64.MaxValue

        // Act
        var result = NumberFormatter.ToBinaryString(value);

        // Assert
        result.Should().Be("1111111111111111111111111111111111111111111111111111111111111111");
    }

    #endregion

    #region Int16 ToHexString Tests

    [Fact]
    public void Int16ToHexString_Single_Parameter_With_Zero_Should_Return_Zero()
    {
        // Arrange
        Int16 value = 0;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void Int16ToHexString_Single_Parameter_With_Positive_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        Int16 value = 255;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("ff");
    }

    [Fact]
    public void Int16ToHexString_Single_Parameter_With_Negative_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        Int16 value = -1;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("ffff");
    }

    [Fact]
    public void Int16ToHexString_With_MinHexDigits_Should_Pad_Correctly()
    {
        // Arrange
        Int16 value = 15;
        int minHexDigits = 4;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits);

        // Assert
        result.Should().Be("000f");
    }

    [Fact]
    public void Int16ToHexString_With_MinHexDigits_One_Should_Return_Minimum_Length()
    {
        // Arrange
        Int16 value = 255;
        int minHexDigits = 1;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits);

        // Assert
        result.Should().Be("ff");
    }

    [Fact]
    public void Int16ToHexString_With_ZeroX_Prefix_True_Should_Add_Prefix()
    {
        // Arrange
        Int16 value = 255;
        int minHexDigits = 1;
        bool addZeroXPrefix = true;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("0xff");
    }

    [Fact]
    public void Int16ToHexString_With_ZeroX_Prefix_False_Should_Not_Add_Prefix()
    {
        // Arrange
        Int16 value = 255;
        int minHexDigits = 1;
        bool addZeroXPrefix = false;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("ff");
    }

    [Fact]
    public void Int16ToHexString_With_All_Parameters_Should_Format_Correctly()
    {
        // Arrange
        Int16 value = 15;
        int minHexDigits = 4;
        bool addZeroXPrefix = true;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("0x000f");
    }

    [Fact]
    public void Int16ToHexString_With_MaxValue_Should_Return_Correct_Hex()
    {
        // Arrange
        Int16 value = Int16.MaxValue; // 32767

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("7fff");
    }

    [Fact]
    public void Int16ToHexString_With_MinValue_Should_Return_Correct_Hex()
    {
        // Arrange
        Int16 value = Int16.MinValue; // -32768

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("8000");
    }

    #endregion

    #region Int32 ToHexString Tests

    [Fact]
    public void Int32ToHexString_Single_Parameter_With_Zero_Should_Return_Zero()
    {
        // Arrange
        int value = 0;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void Int32ToHexString_Single_Parameter_With_Positive_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        int value = 255;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("ff");
    }

    [Fact]
    public void Int32ToHexString_Single_Parameter_With_Negative_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        int value = -1;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("ffffffff");
    }

    [Fact]
    public void Int32ToHexString_With_MinHexDigits_Should_Pad_Correctly()
    {
        // Arrange
        int value = 15;
        int minHexDigits = 8;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits);

        // Assert
        result.Should().Be("0000000f");
    }

    [Fact]
    public void Int32ToHexString_With_ZeroX_Prefix_True_Should_Add_Prefix()
    {
        // Arrange
        int value = 255;
        int minHexDigits = 1;
        bool addZeroXPrefix = true;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("0xff");
    }

    [Fact]
    public void Int32ToHexString_With_ZeroX_Prefix_False_Should_Not_Add_Prefix()
    {
        // Arrange
        int value = 255;
        int minHexDigits = 1;
        bool addZeroXPrefix = false;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("ff");
    }

    [Fact]
    public void Int32ToHexString_With_All_Parameters_Should_Format_Correctly()
    {
        // Arrange
        int value = 15;
        int minHexDigits = 8;
        bool addZeroXPrefix = true;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("0x0000000f");
    }

    [Fact]
    public void Int32ToHexString_With_MaxValue_Should_Return_Correct_Hex()
    {
        // Arrange
        int value = Int32.MaxValue;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("7fffffff");
    }

    [Fact]
    public void Int32ToHexString_With_MinValue_Should_Return_Correct_Hex()
    {
        // Arrange
        int value = Int32.MinValue;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("80000000");
    }

    [Fact]
    public void Int32ToHexString_With_Large_Positive_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        int value = 0x12345678;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("12345678");
    }

    #endregion

    #region Int64 ToHexString Tests

    [Fact]
    public void Int64ToHexString_Single_Parameter_With_Zero_Should_Return_Zero()
    {
        // Arrange
        long value = 0;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("0");
    }

    [Fact]
    public void Int64ToHexString_Single_Parameter_With_Positive_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        long value = 255;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("ff");
    }

    [Fact]
    public void Int64ToHexString_Single_Parameter_With_Negative_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        long value = -1;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("ffffffffffffffff");
    }

    [Fact]
    public void Int64ToHexString_With_MinHexDigits_Should_Pad_Correctly()
    {
        // Arrange
        long value = 15;
        int minHexDigits = 16;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits);

        // Assert
        result.Should().Be("000000000000000f");
    }

    [Fact]
    public void Int64ToHexString_With_ZeroX_Prefix_True_Should_Add_Prefix()
    {
        // Arrange
        long value = 255;
        int minHexDigits = 1;
        bool addZeroXPrefix = true;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("0xff");
    }

    [Fact]
    public void Int64ToHexString_With_ZeroX_Prefix_False_Should_Not_Add_Prefix()
    {
        // Arrange
        long value = 255;
        int minHexDigits = 1;
        bool addZeroXPrefix = false;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("ff");
    }

    [Fact]
    public void Int64ToHexString_With_All_Parameters_Should_Format_Correctly()
    {
        // Arrange
        long value = 15;
        int minHexDigits = 16;
        bool addZeroXPrefix = true;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits, addZeroXPrefix);

        // Assert
        result.Should().Be("0x000000000000000f");
    }

    [Fact]
    public void Int64ToHexString_With_MaxValue_Should_Return_Correct_Hex()
    {
        // Arrange
        long value = Int64.MaxValue;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("7fffffffffffffff");
    }

    [Fact]
    public void Int64ToHexString_With_MinValue_Should_Return_Correct_Hex()
    {
        // Arrange
        long value = Int64.MinValue;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("8000000000000000");
    }

    [Fact]
    public void Int64ToHexString_With_Large_Positive_Value_Should_Return_Correct_Hex()
    {
        // Arrange
        long value = 0x123456789ABCDEF0;

        // Act
        var result = NumberFormatter.ToHexString(value);

        // Assert
        result.Should().Be("123456789abcdef0");
    }

    [Fact]
    public void Int64ToHexString_With_Small_MinHexDigits_Should_Use_Actual_Length()
    {
        // Arrange
        long value = 0x123456789ABCDEF0;
        int minHexDigits = 1;

        // Act
        var result = NumberFormatter.ToHexString(value, minHexDigits);

        // Assert
        result.Should().Be("123456789abcdef0");
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public void All_Binary_Methods_With_One_Should_Return_One()
    {
        // Act & Assert
        NumberFormatter.ToBinaryString((byte)1).Should().Be("1");
        NumberFormatter.ToBinaryString((short)1).Should().Be("1");
        NumberFormatter.ToBinaryString((ushort)1).Should().Be("1");
        NumberFormatter.ToBinaryString((int)1).Should().Be("1");
        NumberFormatter.ToBinaryString((uint)1).Should().Be("1");
        NumberFormatter.ToBinaryString((long)1).Should().Be("1");
        NumberFormatter.ToBinaryString((ulong)1).Should().Be("1");
    }

    [Fact]
    public void All_Hex_Methods_With_Zero_Should_Return_Zero()
    {
        // Act & Assert
        NumberFormatter.ToHexString((short)0).Should().Be("0");
        NumberFormatter.ToHexString((int)0).Should().Be("0");
        NumberFormatter.ToHexString((long)0).Should().Be("0");
    }

    [Fact]
    public void All_Hex_Methods_With_One_Should_Return_One()
    {
        // Act & Assert
        NumberFormatter.ToHexString((short)1).Should().Be("1");
        NumberFormatter.ToHexString((int)1).Should().Be("1");
        NumberFormatter.ToHexString((long)1).Should().Be("1");
    }

    [Fact]
    public void Hex_Methods_With_Fifteen_Should_Return_f()
    {
        // Act & Assert
        NumberFormatter.ToHexString((short)15).Should().Be("f");
        NumberFormatter.ToHexString((int)15).Should().Be("f");
        NumberFormatter.ToHexString((long)15).Should().Be("f");
    }

    [Fact]
    public void Hex_Methods_With_Sixteen_Should_Return_10()
    {
        // Act & Assert
        NumberFormatter.ToHexString((short)16).Should().Be("10");
        NumberFormatter.ToHexString((int)16).Should().Be("10");
        NumberFormatter.ToHexString((long)16).Should().Be("10");
    }

    [Fact]
    public void Binary_Methods_With_Two_Should_Return_10()
    {
        // Act & Assert
        NumberFormatter.ToBinaryString((byte)2).Should().Be("10");
        NumberFormatter.ToBinaryString((short)2).Should().Be("10");
        NumberFormatter.ToBinaryString((ushort)2).Should().Be("10");
        NumberFormatter.ToBinaryString((int)2).Should().Be("10");
        NumberFormatter.ToBinaryString((uint)2).Should().Be("10");
        NumberFormatter.ToBinaryString((long)2).Should().Be("10");
        NumberFormatter.ToBinaryString((ulong)2).Should().Be("10");
    }

    [Fact]
    public void Hex_Methods_With_Prefix_And_Zero_Should_Add_Prefix()
    {
        // Act & Assert
        NumberFormatter.ToHexString<Int16>(value: 0, addZeroXPrefix: true).Should().Be("0x0");
        NumberFormatter.ToHexString<Int32>(value: 0, addZeroXPrefix: true).Should().Be("0x0");
        NumberFormatter.ToHexString<Int64>(value: 0, addZeroXPrefix: true).Should().Be("0x0");
    }

    [Fact]
    public void Hex_Methods_With_Large_MinHexDigits_Should_Pad_Correctly()
    {
        // Act & Assert
        NumberFormatter.ToHexString<Int16>(1, 4).Should().Be("0001");
        NumberFormatter.ToHexString<Int32>(1, 8).Should().Be("00000001");
        NumberFormatter.ToHexString<Int64>(1, 16).Should().Be("0000000000000001");
    }

    #endregion

    #region Test Cases for Invalid Inputs

    [Fact]
    public void ToBinaryString_Called_With_Int128_Should_Thow_ArgException()
    {   // Arrange
        Int128 i128 = 0;
        // Act & Assert
        var action = () => NumberFormatter.ToBinaryString(i128);
        action.Should().Throw<ArgException<Int128>>()
            .WithMessage("Value must be a byte, short, int, long, ushort, uint, ulong type.");
    }


    #endregion
}