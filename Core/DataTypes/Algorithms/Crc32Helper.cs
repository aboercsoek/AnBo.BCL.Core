//--------------------------------------------------------------------------
// File:    Crc32Helper.cs
// Content:	Implementation of a Crc32Helper helper class
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Buffers;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// A high-performance utility class to compute CRC32 (Cyclic Redundancy Check, 32 Bit) checksums.
/// Uses the standard IEEE 802.3 CRC32 polynomial (0xEDB88320).
/// This implementation is optimized for .NET 8+ and supports modern APIs.
/// Infos about CRC32:
/// CRC32 is commonly used for error-checking in data transmission and storage.
/// The checksum is computed using a fixed polynom and the input data, which can be a byte array, span, memory segment, or stream.
/// If the data ist transfered together with the checksum, the checksum can be used to verify the integrity of the data.
/// </summary>
public sealed class Crc32Helper
{
    #region Private Constants

    // CRC32 lookup table for polynomial 0xEDB88320 (256 entries)
    private static readonly uint[] Crc32Table =
    {
        0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f,
        0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988,
        0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91, 0x1db71064, 0x6ab020f2,
        0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
        0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
        0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
        0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c,
        0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
        0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423,
        0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
        0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190, 0x01db7106,
        0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
        0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d,
        0x91646c97, 0xe6635c01, 0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e,
        0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
        0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
        0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7,
        0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0,
        0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa,
        0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
        0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81,
        0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a,
        0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683, 0xe3630b12, 0x94643b84,
        0x0d6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
        0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
        0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc,
        0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8, 0xa1d1937e,
        0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
        0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55,
        0x316e8eef, 0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
        0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28,
        0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
        0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 0x9c0906a9, 0xeb0e363f,
        0x72076785, 0x05005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38,
        0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,
        0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
        0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69,
        0x616bffd3, 0x166ccf45, 0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2,
        0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc,
        0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
        0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693,
        0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
        0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
    };

    // Interal constant for the initial CRC value
    private const uint InitialCrc = 0xFFFFFFFF; // Inverted starting value for CRC32

    private const int BufferSize = 8192; // 8KB buffer for file operations

    #endregion

    #region Private Fields

    private uint currentCrc = InitialCrc;

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Updates the CRC value with a single byte.
    /// </summary>
    /// <param name="value">The byte value to process</param>
    /// <param name="crc">The current CRC value</param>
    /// <returns>The updated CRC value</returns>
    private static uint UpdateCrc(byte value, uint crc)
    {
        // This is the core of the CRC32 calculation.
        return Crc32Table[(crc ^ value) & 0xFF] ^ (crc >> 8);
    }

    /// <summary>
    /// Computes CRC32 for a span of bytes with an initial CRC value.
    /// </summary>
    /// <param name="data">The data to process</param>
    /// <param name="initialCrc">The initial CRC value</param>
    /// <returns>The computed CRC value</returns>
    private static uint ComputeCore(ReadOnlySpan<byte> data, uint initialCrc = InitialCrc)
    {
        uint crc = initialCrc;

        foreach (byte b in data)
        {
            crc = UpdateCrc(b, crc);
        }

        return crc;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the current CRC32 checksum value.
    /// </summary>
    /// <value>The current checksum as an unsigned 32-bit integer</value>
    public uint Checksum => ~currentCrc; // Invert the CRC value to get the final checksum (as per CRC32 specification)

    /// <summary>
    /// Gets a value indicating whether the current instance has been reset to initial state.
    /// </summary>
    public bool IsInitialState => currentCrc == InitialCrc;

    #endregion

    #region Public Instance Methods

    /// <summary>
    /// Resets the CRC calculation to its initial state.
    /// </summary>
    public void Reset()
    {
        currentCrc = InitialCrc;
    }

    /// <summary>
    /// Updates the CRC with additional data from a byte span. Uses a internal current CRC value.
    /// Use <see cref="Checksum"/> property to get the final CRC value.
    /// </summary>
    /// <param name="data">The data to add to the CRC calculation</param>
    public void Update(ReadOnlySpan<byte> data)
    {
        currentCrc = ComputeCore(data, currentCrc);
    }

    /// <summary>
    /// Updates the CRC with additional data from a byte array. Uses a internal current CRC value.
    /// Use <see cref="Checksum"/> property to get the final CRC value.
    /// </summary>
    /// <param name="data">The data to add to the CRC calculation. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null</exception>
    public void Update(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        Update(data.AsSpan());
    }

    /// <summary>
    /// Updates the CRC with additional data from a memory segment. Uses a internal current CRC value.
    /// Use <see cref="Checksum"/> property to get the final CRC value.
    /// </summary>
    /// <param name="data">The data to add to the CRC calculation</param>
    public void Update(ReadOnlyMemory<byte> data)
    {
        Update(data.Span);
    }

    #endregion

    #region Public Static Methods - Basic Compute

    /// <summary>
    /// Computes the CRC32 checksum for a byte span.
    /// </summary>
    /// <param name="data">The data to compute the checksum for</param>
    /// <returns>The computed CRC32 checksum</returns>
    public static uint Compute(ReadOnlySpan<byte> data)
    {
        return ~ComputeCore(data);
    }

    /// <summary>
    /// Computes the CRC32 checksum for a byte array.
    /// </summary>
    /// <param name="data">The data to compute the checksum for. Must not be null.</param>
    /// <returns>The computed CRC32 checksum</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null</exception>
    public static uint Compute(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return Compute(data.AsSpan());
    }

    /// <summary>
    /// Computes the CRC32 checksum for a memory segment.
    /// </summary>
    /// <param name="data">The data to compute the checksum for</param>
    /// <returns>The computed CRC32 checksum</returns>
    public static uint Compute(ReadOnlyMemory<byte> data)
    {
        return Compute(data.Span);
    }

    #endregion

    #region Public Static Methods - String Support

    /// <summary>
    /// Computes the CRC32 checksum for a string using UTF-8 encoding.
    /// </summary>
    /// <param name="text">The string to compute the checksum for. Must not be null.</param>
    /// <returns>The computed CRC32 checksum</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null</exception>
    public static uint Compute(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return Compute(text, Encoding.UTF8);
    }

    /// <summary>
    /// Computes the CRC32 checksum for a string using the specified encoding.
    /// </summary>
    /// <param name="text">The string to compute the checksum for. Must not be null.</param>
    /// <param name="encoding">The encoding to use for converting the string to bytes. Must not be null.</param>
    /// <returns>The computed CRC32 checksum</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> or <paramref name="encoding"/> is null</exception>
    public static uint Compute(string text, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(encoding);

        // For small strings, use stack allocation to avoid heap allocation
        if (text.Length <= 256)
        {
            Span<byte> buffer = stackalloc byte[encoding.GetMaxByteCount(text.Length)];
            int bytesWritten = encoding.GetBytes(text, buffer);
            return Compute(buffer[..bytesWritten]);
        }

        // For larger strings, use the encoding's GetBytes method
        byte[] bytes = encoding.GetBytes(text);
        return Compute(bytes);
    }

    #endregion

    #region Public Static Methods - Stream Support

    /// <summary>
    /// Computes the CRC32 checksum for data from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from. Must not be null and must be readable.</param>
    /// <returns>The computed CRC32 checksum</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown when the stream is not readable</exception>
    public static uint Compute(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable", nameof(stream));
        }

        uint crc = InitialCrc;
        // Use a rented buffer to avoid frequent allocations
        byte[] buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

        try
        {
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                crc = ComputeCore(buffer.AsSpan(0, bytesRead), crc);
            }
        }
        finally
        {
            // Return the buffer to the pool to avoid memory leaks
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return ~crc;
    }

    /// <summary>
    /// Asynchronously computes the CRC32 checksum for data from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from. Must not be null and must be readable.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A task representing the asynchronous operation with the computed CRC32 checksum as result</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown when the stream is not readable</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled</exception>
    public static async Task<uint> ComputeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream); ;

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable", nameof(stream));
        }

        uint crc = InitialCrc;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
            {
                crc = ComputeCore(buffer.AsSpan(0, bytesRead), crc);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return ~crc;
    }

    #endregion

    #region Public Static Methods - File Support

    /// <summary>
    /// Computes the CRC32 checksum for a file.
    /// </summary>
    /// <param name="filePath">The path to the file. Must not be null or empty.</param>
    /// <returns>The computed CRC32 checksum</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> empty</exception>
    /// <exception cref="ArgumentException">Thrown when the file does not exist</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the file is denied</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs</exception>
    public static uint ComputeFile(string filePath)
    {
        ArgChecker.ShouldBeExistingFile(filePath);

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Compute(fileStream);
    }

    /// <summary>
    /// Asynchronously computes the CRC32 checksum for a file.
    /// </summary>
    /// <param name="filePath">The path to the file. Must not be null or empty.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A task representing the asynchronous operation with the computed CRC32 checksum as result</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> empty</exception>
    /// <exception cref="ArgumentException">Thrown when the file does not exist</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the file is denied</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled</exception>
    public static async Task<uint> ComputeFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgChecker.ShouldBeExistingFile(filePath);

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await ComputeAsync(fileStream, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Public Static Methods - Utility

    /// <summary>
    /// Converts a CRC32 value to its hexadecimal string representation.
    /// </summary>
    /// <param name="crc">The CRC32 value to convert</param>
    /// <param name="upperCase">True to use uppercase letters, false for lowercase</param>
    /// <returns>The hexadecimal string representation of the CRC32 value</returns>
    public static string ToHexString(uint crc, bool upperCase = true)
    {
        return crc.ToString(upperCase ? "X8" : "x8");
    }

    /// <summary>
    /// Validates whether two CRC32 values are equal.
    /// This method provides a clear semantic meaning for checksum validation.
    /// </summary>
    /// <param name="expected">The expected CRC32 value</param>
    /// <param name="actual">The actual CRC32 value</param>
    /// <returns>True if the CRC32 values match, false otherwise</returns>
    public static bool ValidateChecksum(uint expected, uint actual)
    {
        return expected == actual;
    }

    #endregion
}
