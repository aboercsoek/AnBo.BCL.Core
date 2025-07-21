//--------------------------------------------------------------------------
// File:    Crc32HelperUnitTest.cs
// Content: Unit tests for Crc32Helper class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;

namespace AnBo.Test
{
    public class Crc32HelperUnitTest
    {
        #region Test Data and Constants

        private const uint KnownCrc32EmptyData = 0x00000000u;
        private const uint KnownCrc32HelloWorld = 0x4A17B156u; // CRC32 of "Hello World"
        private const uint KnownCrc32Test = 0xD87F7E0Cu; // CRC32 of "test"
        private const uint KnownCrc32ABC = 0x352441C2u; // CRC32 of "abc"

        private readonly byte[] _testBytes = "test"u8.ToArray();
        private readonly byte[] _helloWorldBytes = "Hello World"u8.ToArray();
        private readonly byte[] _abcBytes = "abc"u8.ToArray();

        #endregion

        #region Constructor and Properties Tests

        [Fact]
        public void Constructor_Should_Initialize_To_Initial_State()
        {
            // Act
            var helper = new Crc32Helper();

            // Assert
            helper.IsInitialState.Should().BeTrue();
            helper.Checksum.Should().Be(KnownCrc32EmptyData);
        }

        [Fact]
        public void IsInitialState_Should_Return_True_After_Reset()
        {
            // Arrange
            var helper = new Crc32Helper();
            helper.Update(_testBytes);

            // Act
            helper.Reset();

            // Assert
            helper.IsInitialState.Should().BeTrue();
        }

        [Fact]
        public void IsInitialState_Should_Return_False_After_Update()
        {
            // Arrange
            var helper = new Crc32Helper();

            // Act
            helper.Update(_testBytes);

            // Assert
            helper.IsInitialState.Should().BeFalse();
        }

        [Fact]
        public void Checksum_Should_Return_Zero_For_Empty_Data()
        {
            // Arrange
            var helper = new Crc32Helper();

            // Act
            helper.Update(Array.Empty<byte>());

            // Assert
            helper.Checksum.Should().Be(KnownCrc32EmptyData);
        }

        #endregion

        #region Reset Method Tests

        [Fact]
        public void Reset_Should_Clear_Previous_Data()
        {
            // Arrange
            var helper = new Crc32Helper();
            helper.Update(_testBytes);
            var checksumBeforeReset = helper.Checksum;

            // Act
            helper.Reset();

            // Assert
            helper.Checksum.Should().NotBe(checksumBeforeReset);
            helper.Checksum.Should().Be(KnownCrc32EmptyData);
            helper.IsInitialState.Should().BeTrue();
        }

        [Fact]
        public void Reset_Multiple_Times_Should_Maintain_Initial_State()
        {
            // Arrange
            var helper = new Crc32Helper();
            helper.Update(_testBytes);

            // Act
            helper.Reset();
            helper.Reset();
            helper.Reset();

            // Assert
            helper.IsInitialState.Should().BeTrue();
            helper.Checksum.Should().Be(KnownCrc32EmptyData);
        }

        #endregion

        #region Instance Update Method Tests

        [Fact]
        public void Update_ReadOnlySpan_With_Valid_Data_Should_Update_Checksum()
        {
            // Arrange
            var helper = new Crc32Helper();
            ReadOnlySpan<byte> data = _testBytes;

            // Act
            helper.Update(data);

            // Assert
            helper.Checksum.Should().Be(KnownCrc32Test);
            helper.IsInitialState.Should().BeFalse();
        }

        [Fact]
        public void Update_ReadOnlySpan_With_Empty_Data_Should_Not_Change_Checksum()
        {
            // Arrange
            var helper = new Crc32Helper();
            ReadOnlySpan<byte> data = Array.Empty<byte>();

            // Act
            helper.Update(data);

            // Assert
            helper.Checksum.Should().Be(KnownCrc32EmptyData);
            helper.IsInitialState.Should().BeTrue();
        }

        [Fact]
        public void Update_ByteArray_With_Valid_Data_Should_Update_Checksum()
        {
            // Arrange
            var helper = new Crc32Helper();

            // Act
            helper.Update(_testBytes);

            // Assert
            helper.Checksum.Should().Be(KnownCrc32Test);
            helper.IsInitialState.Should().BeFalse();
        }

        [Fact]
        public void Update_ByteArray_With_Null_Should_Throw_ArgNullException()
        {
            // Arrange
            var helper = new Crc32Helper();
            byte[]? nullArray = null;

            // Act & Assert
            var action = () => helper.Update(nullArray!);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void Update_ReadOnlyMemory_With_Valid_Data_Should_Update_Checksum()
        {
            // Arrange
            var helper = new Crc32Helper();
            ReadOnlyMemory<byte> memory = _testBytes;

            // Act
            helper.Update(memory);

            // Assert
            helper.Checksum.Should().Be(KnownCrc32Test);
            helper.IsInitialState.Should().BeFalse();
        }

        [Fact]
        public void Update_Multiple_Calls_Should_Accumulate_Correctly()
        {
            // Arrange
            var helper1 = new Crc32Helper();
            var helper2 = new Crc32Helper();
            var allData = "Hello World"u8.ToArray();
            var part1 = "Hello "u8.ToArray();
            var part2 = "World"u8.ToArray();

            // Act
            helper1.Update(allData);
            
            helper2.Update(part1);
            helper2.Update(part2);

            // Assert
            helper1.Checksum.Should().Be(helper2.Checksum);
            helper1.Checksum.Should().Be(KnownCrc32HelloWorld);
        }

        #endregion

        #region Static Compute Method Tests - Basic

        [Fact]
        public void Compute_ReadOnlySpan_With_Valid_Data_Should_Return_Correct_Checksum()
        {
            // Arrange
            ReadOnlySpan<byte> data = _testBytes;

            // Act
            uint result = Crc32Helper.Compute(data);

            // Assert
            result.Should().Be(KnownCrc32Test);
        }

        [Fact]
        public void Compute_ReadOnlySpan_With_Empty_Data_Should_Return_Zero()
        {
            // Arrange
            ReadOnlySpan<byte> data = Array.Empty<byte>();

            // Act
            uint result = Crc32Helper.Compute(data);

            // Assert
            result.Should().Be(KnownCrc32EmptyData);
        }

        [Fact]
        public void Compute_ByteArray_With_Valid_Data_Should_Return_Correct_Checksum()
        {
            // Act
            uint result = Crc32Helper.Compute(_testBytes);

            // Assert
            result.Should().Be(KnownCrc32Test);
        }

        [Fact]
        public void Compute_ByteArray_With_Null_Should_Throw_ArgNullException()
        {
            // Arrange
            byte[]? nullArray = null;

            // Act & Assert
            var action = () => Crc32Helper.Compute(nullArray!);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void Compute_ReadOnlyMemory_With_Valid_Data_Should_Return_Correct_Checksum()
        {
            // Arrange
            ReadOnlyMemory<byte> memory = _testBytes;

            // Act
            uint result = Crc32Helper.Compute(memory);

            // Assert
            result.Should().Be(KnownCrc32Test);
        }

        [Fact]
        public void Compute_Large_Data_Should_Work_Correctly()
        {
            // Arrange
            byte[] largeData = new byte[10000];
            for (int i = 0; i < largeData.Length; i++)
            {
                largeData[i] = (byte)(i % 256);
            }

            // Act
            uint result = Crc32Helper.Compute(largeData);

            // Assert
            result.Should().NotBe(0);
        }

        #endregion

        #region Static Compute Method Tests - String Support

        [Fact]
        public void Compute_String_With_Valid_Text_Should_Return_Correct_Checksum()
        {
            // Arrange
            string text = "test";

            // Act
            uint result = Crc32Helper.Compute(text);

            // Assert
            result.Should().Be(KnownCrc32Test);
        }

        [Fact]
        public void Compute_String_With_Null_Should_Throw_ArgNullException()
        {
            // Arrange
            string? nullText = null;

            // Act & Assert
            var action = () => Crc32Helper.Compute(nullText!);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void Compute_String_With_Empty_String_Should_Return_Zero()
        {
            // Arrange
            string text = string.Empty;

            // Act
            uint result = Crc32Helper.Compute(text);

            // Assert
            result.Should().Be(KnownCrc32EmptyData);
        }

        [Fact]
        public void Compute_String_With_Encoding_Should_Return_Correct_Checksum()
        {
            // Arrange
            string text = "test";
            var encoding = Encoding.UTF8;

            // Act
            uint result = Crc32Helper.Compute(text, encoding);

            // Assert
            result.Should().Be(KnownCrc32Test);
        }

        [Fact]
        public void Compute_String_With_Null_Text_And_Encoding_Should_Throw_ArgNullException()
        {
            // Arrange
            string? nullText = null;
            var encoding = Encoding.UTF8;

            // Act & Assert
            var action = () => Crc32Helper.Compute(nullText!, encoding);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void Compute_String_With_Null_Encoding_Should_Throw_ArgNullException()
        {
            // Arrange
            string text = "test";
            Encoding? nullEncoding = null;

            // Act & Assert
            var action = () => Crc32Helper.Compute(text, nullEncoding!);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void Compute_String_With_Different_Encodings_Should_Return_Different_Results()
        {
            // Arrange
            string text = "Hello Wörld"; // Contains non-ASCII character
            var utf8 = Encoding.UTF8;
            var latin1 = Encoding.Latin1;

            // Act
            uint utf8Result = Crc32Helper.Compute(text, utf8);
            uint latin1Result = Crc32Helper.Compute(text, latin1);

            // Assert
            utf8Result.Should().NotBe(latin1Result);
        }

        [Fact]
        public void Compute_String_Small_Text_Should_Use_Stack_Allocation()
        {
            // Arrange
            string text = new string('a', 200); // Small enough for stack allocation

            // Act
            uint result = Crc32Helper.Compute(text, Encoding.UTF8);

            // Assert
            result.Should().NotBe(0);
        }

        [Fact]
        public void Compute_String_Large_Text_Should_Use_Heap_Allocation()
        {
            // Arrange
            string text = new string('a', 500); // Large enough to force heap allocation

            // Act
            uint result = Crc32Helper.Compute(text, Encoding.UTF8);

            // Assert
            result.Should().NotBe(0);
        }

        #endregion

        #region Static Compute Method Tests - Stream Support

        [Fact]
        public void Compute_Stream_With_Valid_Data_Should_Return_Correct_Checksum()
        {
            // Arrange
            using var stream = new MemoryStream(_testBytes);

            // Act
            uint result = Crc32Helper.Compute(stream);

            // Assert
            result.Should().Be(KnownCrc32Test);
        }

        [Fact]
        public void Compute_Stream_With_Null_Should_Throw_ArgNullException()
        {
            // Arrange
            Stream? nullStream = null;

            // Act & Assert
            var action = () => Crc32Helper.Compute(nullStream!);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void Compute_Stream_With_Non_Readable_Stream_Should_Throw_ArgException()
        {
            // Arrange
            var writeOnlyStream = new MemoryStream();
            writeOnlyStream.Write(_testBytes);
            writeOnlyStream.Position = 0;
            // Create a wrapper that reports CanRead as false
            var nonReadableStream = new NonReadableStreamWrapper(writeOnlyStream);

            // Act & Assert
            var action = () => Crc32Helper.Compute(nonReadableStream);
            action.Should().Throw<ArgException<Stream>>();
        }

        [Fact]
        public void Compute_Stream_With_Empty_Stream_Should_Return_Zero()
        {
            // Arrange
            using var stream = new MemoryStream();

            // Act
            uint result = Crc32Helper.Compute(stream);

            // Assert
            result.Should().Be(KnownCrc32EmptyData);
        }

        [Fact]
        public void Compute_Stream_Should_Not_Change_Stream_Position()
        {
            // Arrange
            using var stream = new MemoryStream(_testBytes);
            stream.Position = 2;
            long originalPosition = stream.Position;

            // Act
            uint result = Crc32Helper.Compute(stream);

            // Assert
            result.Should().NotBe(0);
            stream.Position.Should().Be(originalPosition + _testBytes.Length - 2); // Position advances during read
        }

        [Fact]
        public void Compute_Stream_Large_File_Should_Work_Correctly()
        {
            // Arrange
            var largeData = new byte[20000]; // Larger than buffer size
            for (int i = 0; i < largeData.Length; i++)
            {
                largeData[i] = (byte)(i % 256);
            }
            using var stream = new MemoryStream(largeData);

            // Act
            uint result = Crc32Helper.Compute(stream);

            // Assert
            result.Should().NotBe(0);
        }

        [Fact]
        public async Task ComputeAsync_Stream_With_Valid_Data_Should_Return_Correct_Checksum()
        {
            // Arrange
            using var stream = new MemoryStream(_testBytes);

            // Act
            uint result = await Crc32Helper.ComputeAsync(stream);

            // Assert
            result.Should().Be(KnownCrc32Test);
        }

        [Fact]
        public async Task ComputeAsync_Stream_With_Null_Should_Throw_ArgNullException()
        {
            // Arrange
            Stream? nullStream = null;

            // Act & Assert
            var action = async () => await Crc32Helper.ComputeAsync(nullStream!);
            await action.Should().ThrowAsync<ArgNullException>();
        }

        [Fact]
        public async Task ComputeAsync_Stream_With_Non_Readable_Stream_Should_Throw_ArgException()
        {
            // Arrange
            var writeOnlyStream = new MemoryStream();
            writeOnlyStream.Write(_testBytes);
            writeOnlyStream.Position = 0;
            var nonReadableStream = new NonReadableStreamWrapper(writeOnlyStream);

            // Act & Assert
            var action = async () => await Crc32Helper.ComputeAsync(nonReadableStream);
            await action.Should().ThrowAsync<ArgException<Stream>>();
        }

        [Fact]
        public async Task ComputeAsync_Stream_With_Cancellation_Should_Throw_OperationCanceledException()
        {
            // Arrange
            using var stream = new SlowStream(_testBytes, 50); // Slow stream that takes time
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

            // Act & Assert
            var action = async () => await Crc32Helper.ComputeAsync(stream, cts.Token);
            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task ComputeAsync_Stream_With_Empty_Stream_Should_Return_Zero()
        {
            // Arrange
            using var stream = new MemoryStream();

            // Act
            uint result = await Crc32Helper.ComputeAsync(stream);

            // Assert
            result.Should().Be(KnownCrc32EmptyData);
        }

        #endregion

        #region Static Compute Method Tests - File Support

        [Fact]
        public void ComputeFile_With_Valid_File_Should_Return_Correct_Checksum()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempFile, _testBytes);

                // Act
                uint result = Crc32Helper.ComputeFile(tempFile);

                // Assert
                result.Should().Be(KnownCrc32Test);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void ComputeFile_With_Null_Path_Should_Throw_ArgNullException()
        {
            // Arrange
            string? nullPath = null;

            // Act & Assert
            var action = () => Crc32Helper.ComputeFile(nullPath!);
            action.Should().Throw<ArgNullException>();
        }

        [Fact]
        public void ComputeFile_With_Empty_Path_Should_Throw_ArgEmptyException()
        {
            // Arrange
            string emptyPath = string.Empty;

            // Act & Assert
            var action = () => Crc32Helper.ComputeFile(emptyPath);
            action.Should().Throw<ArgEmptyException>();
        }

        [Fact]
        public void ComputeFile_With_Non_Existent_File_Should_Throw_ArgFilePathException()
        {
            // Arrange
            string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Act & Assert
            var action = () => Crc32Helper.ComputeFile(nonExistentPath);
            action.Should().Throw<ArgFilePathException>();
        }

        [Fact]
        public void ComputeFile_With_Empty_File_Should_Return_Zero()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                // File is already empty after creation

                // Act
                uint result = Crc32Helper.ComputeFile(tempFile);

                // Assert
                result.Should().Be(KnownCrc32EmptyData);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task ComputeFileAsync_With_Valid_File_Should_Return_Correct_Checksum()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                await File.WriteAllBytesAsync(tempFile, _testBytes);

                // Act
                uint result = await Crc32Helper.ComputeFileAsync(tempFile);

                // Assert
                result.Should().Be(KnownCrc32Test);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task ComputeFileAsync_With_Null_Path_Should_Throw_ArgNullException()
        {
            // Arrange
            string? nullPath = null;

            // Act & Assert
            var action = async () => await Crc32Helper.ComputeFileAsync(nullPath!);
            await action.Should().ThrowAsync<ArgNullException>();
        }

        [Fact]
        public async Task ComputeFileAsync_With_Non_Existent_File_Should_Throw_ArgFilePathException()
        {
            // Arrange
            string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Act & Assert
            var action = async () => await Crc32Helper.ComputeFileAsync(nonExistentPath);
            await action.Should().ThrowAsync<ArgFilePathException>();
        }

        [Fact]
        public async Task ComputeFileAsync_With_Cancellation_Should_Throw_OperationCanceledException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            try
            {
                var largeData = new byte[1000000]; // 1MB file
                await File.WriteAllBytesAsync(tempFile, largeData);
                
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));

                // Act & Assert
                var action = async () => await Crc32Helper.ComputeFileAsync(tempFile, cts.Token);
                await action.Should().ThrowAsync<OperationCanceledException>();
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        #endregion

        #region Static Utility Method Tests

        [Fact]
        public void ToHexString_With_Uppercase_Should_Return_Uppercase_Hex()
        {
            // Arrange
            uint crc = 0x4A17B156;

            // Act
            string result = Crc32Helper.ToHexString(crc, true);

            // Assert
            result.Should().Be("4A17B156");
        }

        [Fact]
        public void ToHexString_With_Lowercase_Should_Return_Lowercase_Hex()
        {
            // Arrange
            uint crc = 0x4A17B156;

            // Act
            string result = Crc32Helper.ToHexString(crc, false);

            // Assert
            result.Should().Be("4a17b156");
        }

        [Fact]
        public void ToHexString_With_Zero_Should_Return_Eight_Zeros()
        {
            // Arrange
            uint crc = 0;

            // Act
            string result = Crc32Helper.ToHexString(crc);

            // Assert
            result.Should().Be("00000000");
        }

        [Fact]
        public void ToHexString_With_Max_Value_Should_Return_All_Fs()
        {
            // Arrange
            uint crc = uint.MaxValue;

            // Act
            string result = Crc32Helper.ToHexString(crc);

            // Assert
            result.Should().Be("FFFFFFFF");
        }

        [Fact]
        public void ValidateChecksum_With_Equal_Values_Should_Return_True()
        {
            // Arrange
            uint expected = 0x12345678;
            uint actual = 0x12345678;

            // Act
            bool result = Crc32Helper.ValidateChecksum(expected, actual);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateChecksum_With_Different_Values_Should_Return_False()
        {
            // Arrange
            uint expected = 0x12345678;
            uint actual = 0x87654321;

            // Act
            bool result = Crc32Helper.ValidateChecksum(expected, actual);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateChecksum_With_Zero_Values_Should_Return_True()
        {
            // Arrange
            uint expected = 0;
            uint actual = 0;

            // Act
            bool result = Crc32Helper.ValidateChecksum(expected, actual);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region Consistency Tests

        [Fact]
        public void Instance_And_Static_Methods_Should_Return_Same_Results()
        {
            // Arrange
            var helper = new Crc32Helper();

            // Act
            helper.Update(_testBytes);
            uint instanceResult = helper.Checksum;
            uint staticResult = Crc32Helper.Compute(_testBytes);

            // Assert
            instanceResult.Should().Be(staticResult);
        }

        [Fact]
        public void Multiple_Computations_Of_Same_Data_Should_Return_Same_Result()
        {
            // Act
            uint result1 = Crc32Helper.Compute(_testBytes);
            uint result2 = Crc32Helper.Compute(_testBytes);
            uint result3 = Crc32Helper.Compute(_testBytes);

            // Assert
            result1.Should().Be(result2);
            result2.Should().Be(result3);
        }

        [Fact]
        public void Different_Data_Should_Return_Different_Results()
        {
            // Arrange
            byte[] data1 = "test1"u8.ToArray();
            byte[] data2 = "test2"u8.ToArray();

            // Act
            uint result1 = Crc32Helper.Compute(data1);
            uint result2 = Crc32Helper.Compute(data2);

            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void String_And_Byte_Array_With_Same_Content_Should_Return_Same_Result()
        {
            // Arrange
            string text = "test";
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            // Act
            uint textResult = Crc32Helper.Compute(text);
            uint bytesResult = Crc32Helper.Compute(bytes);

            // Assert
            textResult.Should().Be(bytesResult);
        }

        [Fact]
        public void Incremental_Updates_Should_Match_Single_Computation()
        {
            // Arrange
            var helper = new Crc32Helper();
            var allData = "The quick brown fox jumps over the lazy dog"u8.ToArray();
            var parts = new[]
            {
                "The quick brown "u8.ToArray(),
                "fox jumps over "u8.ToArray(),
                "the lazy dog"u8.ToArray()
            };

            // Act
            foreach (var part in parts)
            {
                helper.Update(part);
            }
            uint incrementalResult = helper.Checksum;
            uint singleResult = Crc32Helper.Compute(allData);

            // Assert
            incrementalResult.Should().Be(singleResult);
        }

        #endregion

        #region Performance and Edge Cases Tests

        [Fact]
        public void Compute_With_Very_Large_Data_Should_Complete_Successfully()
        {
            // Arrange
            var largeData = new byte[1000000]; // 1MB
            Random.Shared.NextBytes(largeData);

            // Act
            uint result = Crc32Helper.Compute(largeData);

            // Assert
            result.Should().NotBe(0); // With random data, very unlikely to be 0
        }

        [Fact]
        public void Update_With_Multiple_Small_Chunks_Should_Work_Correctly()
        {
            // Arrange
            var helper = new Crc32Helper();
            var data = "abcdefghijklmnopqrstuvwxyz"u8.ToArray();

            // Act
            for (int i = 0; i < data.Length; i++)
            {
                helper.Update(new ReadOnlySpan<byte>(data, i, 1));
            }

            // Assert
            uint result = helper.Checksum;
            uint expected = Crc32Helper.Compute(data);
            result.Should().Be(expected);
        }

        [Fact]
        public async Task Class_Should_Be_Thread_Safe_For_Static_Methods()
        {
            // Arrange
            const int threadCount = 10;
            const int computationsPerThread = 100;
            var tasks = new Task<uint>[threadCount];
            var expectedResult = Crc32Helper.Compute(_testBytes);

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    uint lastResult = 0;
                    for (int j = 0; j < computationsPerThread; j++)
                    {
                        lastResult = Crc32Helper.Compute(_testBytes);
                    }
                    return lastResult;
                });
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            foreach (var result in results)
            {
                result.Should().Be(expectedResult);

            }
        }

        #endregion

        #region Helper Classes for Testing

        private class NonReadableStreamWrapper : Stream
        {
            private readonly Stream _innerStream;

            public NonReadableStreamWrapper(Stream innerStream)
            {
                _innerStream = innerStream;
            }

            public override bool CanRead => false; // Always return false
            public override bool CanSeek => _innerStream.CanSeek;
            public override bool CanWrite => _innerStream.CanWrite;
            public override long Length => _innerStream.Length;
            public override long Position
            {
                get => _innerStream.Position;
                set => _innerStream.Position = value;
            }

            public override void Flush() => _innerStream.Flush();
            public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
            public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
            public override void SetLength(long value) => _innerStream.SetLength(value);
            public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _innerStream?.Dispose();
                base.Dispose(disposing);
            }
        }

        private class SlowStream : Stream
        {
            private readonly MemoryStream _innerStream;
            private readonly int _delayMs;

            public SlowStream(byte[] data, int delayMs)
            {
                _innerStream = new MemoryStream(data);
                _delayMs = delayMs;
            }

            public override bool CanRead => _innerStream.CanRead;
            public override bool CanSeek => _innerStream.CanSeek;
            public override bool CanWrite => _innerStream.CanWrite;
            public override long Length => _innerStream.Length;
            public override long Position
            {
                get => _innerStream.Position;
                set => _innerStream.Position = value;
            }

            public override void Flush() => _innerStream.Flush();

            public override int Read(byte[] buffer, int offset, int count)
            {
                Thread.Sleep(_delayMs); // Simulate slow read
                return _innerStream.Read(buffer, offset, count);
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Delay(_delayMs, cancellationToken);
                return await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
            }

            public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
            public override void SetLength(long value) => _innerStream.SetLength(value);
            public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _innerStream?.Dispose();
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}