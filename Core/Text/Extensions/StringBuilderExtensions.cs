//--------------------------------------------------------------------------
// File:    StringBuilderExtensions.cs
// Content:	Implementation of class StringBuilderExtensions
// Author:	Andreas Börcsök
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core
{
    ///<summary>Represents extension methods for <see cref="StringBuilder"/> type.</summary>
	public static class StringBuilderExtensions
    {
        /// <summary>
        /// Clears the content of the specified string builder.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        public static void Clear(this StringBuilder stringBuilder)
        {
            if (stringBuilder != null)
                stringBuilder.Length = 0;
        }

        /// <summary>
        /// Determines whether the specified string builder is empty.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified builder is empty; otherwise, <see langword="false"/> (if builder is <see langword="null"/> <see langword="false"/> is returned).
        /// </returns>
        public static bool IsEmpty(this StringBuilder builder)
        {
            return (builder == null) ? false : (builder.Length == 0);
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
