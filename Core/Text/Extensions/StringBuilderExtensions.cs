//--------------------------------------------------------------------------
// File:    StringBuilderExtensions.cs
// Content:	Implementation of class StringBuilderExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

#endregion

namespace AnBo.Core
{
    /// <summary>
    /// Provides modern extension methods for <see cref="StringBuilder"/> optimized for .NET 8+.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Clears the content of the specified string builder efficiently.
        /// </summary>
        /// <param name="builder">The string builder to clear. Cannot be null.</param>
        /// <returns>The same StringBuilder instance for method chaining.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder Clear(this StringBuilder builder)
        {
            ArgChecker.ShouldNotBeNull(builder);

            builder.Length = 0;
            return builder;
        }

        /// <summary>
        /// Determines whether the specified string builder is null or empty.
        /// </summary>
        /// <param name="builder">The string builder to check.</param>
        /// <returns>
        /// <see langword="true"/> if the builder is null or has zero length; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty([NotNullWhen(false)] this StringBuilder? builder)
            => builder is null || builder.Length == 0;

        /// <summary>
        /// Determines whether the specified string builder is empty (but not null).
        /// </summary>
        /// <param name="builder">The string builder to check. Cannot be null.</param>
        /// <returns><see langword="true"/> if the builder has zero length; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgNullException">Thrown when <paramref name="builder"/> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this StringBuilder builder)
        {
            ArgChecker.ShouldNotBeNull(builder);
            return builder.Length == 0;
        }

        /// <summary>
        /// Appends the string representation of the specified value using invariant culture.
        /// </summary>
        /// <typeparam name="T">The type of the value to append.</typeparam>
        /// <param name="builder">The string builder. Cannot be null.</param>
        /// <param name="value">The value to append. Null values are ignored for reference types.</param>
        /// <returns>The same StringBuilder instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
        public static StringBuilder AppendInvariant<T>(this StringBuilder builder, T? value)
        {
            ArgChecker.ShouldNotBeNull(builder);

            return value switch
            {
                null => builder, // Skip null values
                string str => builder.Append(str),
                IFormattable formattable => builder.Append(formattable.ToString(null, CultureInfo.InvariantCulture)),
                _ => builder.Append(value.ToString())
            };
        }

        /// <summary>
        /// Appends the string representation of the specified value using invariant culture if value is formattable.
        /// or StringConversionHelper.ToInvariantString<T>.
        /// </summary>
        /// <typeparam name="T">The type of the value to append.</typeparam>
        /// <param name="builder">The string builder. Cannot be null.</param>
        /// <param name="value">The value to append. Null values are ignored for reference types.</param>
        /// <returns>The same StringBuilder instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
        public static StringBuilder AppendUseToInvariantString<T>(this StringBuilder builder, T? value)
        {
            ArgChecker.ShouldNotBeNull(builder);

            return value switch
            {
                null => builder, // Skip null values
                string str => builder.Append(str),
                IFormattable formattable => builder.Append(formattable.ToString(null, CultureInfo.InvariantCulture)),
                _ => builder.Append(value.ToInvariantString())
            };
        }

        /// <summary>
        /// Appends the specified value to the string builder.
        /// </summary>
        /// <typeparam name="T">Type of the value that should be added.</typeparam>
        /// <param name="sb">The string builder where the value should be added.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The string builder instance.</returns>
        public static StringBuilder? Append<T>(this StringBuilder sb, T value)
        {
            if (sb == null)
                return null;

            if (typeof(T).IsValueType == false)
            {
                if (value == null)
                {
                    return sb;
                }
            }

            return sb.Append(value.ToInvariantString());
        }

        /// <summary>
        /// Insert the specified text at the beginning of the string builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="text">The text to insert.</param>
        /// <returns>The string builder reference.</returns>
        public static StringBuilder Prepend(this StringBuilder builder, string text)
        {
            if ((text == null))
                return builder;

            return builder.Insert(0, text);
        }

    }
}
