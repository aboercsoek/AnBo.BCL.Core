//--------------------------------------------------------------------------
// File:    StringExtensions.SubString.cs
// Content:	Implementation of class StringExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Globalization;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// Represents modern extension methods for <see cref="String"/> type optimized for .NET 8+.
/// </summary>
public static partial class StringExtensions
{
    #region SubString extensions (Right, Left, ...)

    /// <summary>
    /// Gets the leftmost characters from a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of characters to return.</param>
    /// <returns>The leftmost characters or the entire string if shorter than requested length.</returns>
    public static string? Left(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;
        if (length >= input.Length) return input;

        return input[..length];
    }

    /// <summary>
    /// Gets the rightmost characters from a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of characters to return.</param>
    /// <returns>The rightmost characters or the entire string if shorter than requested length.</returns>
    public static string? Right(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;
        if (length >= input.Length) return input;

        return input[^length..];
    }

    /// <summary>
    /// Gets the leftmost grapheme clusters (user-perceived characters) from a string.
    /// This method properly handles complex Unicode scenarios like emoji and combining characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of grapheme clusters to return.</param>
    /// <returns>The leftmost grapheme clusters.</returns>
    /// <example>
    /// <code>
    /// "Hello 👨‍👩‍👧‍👦 World".LeftGraphemes(7); // Returns "Hello 👨‍👩‍👧‍👦"
    /// "café".LeftGraphemes(3); // Returns "caf" (regardless of é representation)
    /// </code>
    /// </example>
    public static string? LeftGraphemes(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;

        var enumerator = StringInfo.GetTextElementEnumerator(input);
        var count = 0;
        var result = new StringBuilder();

        while (enumerator.MoveNext() && count < length)
        {
            result.Append(enumerator.GetTextElement());
            count++;
        }

        return result.ToString();
    }

    /// <summary>
    /// Gets the rightmost grapheme clusters (user-perceived characters) from a string.
    /// This method properly handles complex Unicode scenarios like emoji and combining characters.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="length">The number of grapheme clusters to return.</param>
    /// <returns>The rightmost grapheme clusters.</returns>
    /// <example>
    /// <code>
    /// "Hello 👨‍👩‍👧‍👦 World".RightGraphemes(6); // Returns "👨‍👩‍👧‍👦 World"
    /// "café".RightGraphemes(2); // Returns "fé" (regardless of é representation)
    /// </code>
    /// </example>
    public static string? RightGraphemes(this string? input, int length)
    {
        if (input is null) return null;
        if (length <= 0) return string.Empty;

        var textElements = new List<string>();
        var enumerator = StringInfo.GetTextElementEnumerator(input);

        while (enumerator.MoveNext())
        {
            textElements.Add(enumerator.GetTextElement());
        }

        if (length >= textElements.Count) return input;

        var startIndex = textElements.Count - length;
        return string.Concat(textElements.Skip(startIndex));
    }

    /// <summary>
    /// Clips a string to a maximum length, appending an ellipsis if necessary.
    /// </summary>
    /// <param name="text">The text to clip.</param>
    /// <param name="maxCount">The maximum string length.</param>
    /// <param name="clipText">The text to append when clipping (default: "...").</param>
    /// <returns>A string clipped to the maximum length.</returns>
    public static string Clip(this string? text, int maxCount, string clipText = "...")
    {
        if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
        if (maxCount <= 0) return string.Empty;
        if (text.Length <= maxCount) return text;

        clipText ??= "...";

        if (maxCount <= clipText.Length)
            return text[..maxCount];

        var targetLength = maxCount - clipText.Length;
        return string.Create(maxCount, (text, targetLength, clipText),
            static (span, state) =>
            {
                state.text.AsSpan(0, state.targetLength).CopyTo(span); // copy the clipped part
                state.clipText.AsSpan().CopyTo(span[state.targetLength..]); // append clip text
            });
    }

    #endregion
}

