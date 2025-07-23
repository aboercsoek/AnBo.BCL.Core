//--------------------------------------------------------------------------
// File:    EnumExtensions.cs
// Content:	Implementation of class EnumExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Runtime.CompilerServices;

#endregion

namespace AnBo.Core;

/// <summary>
/// Provides extension methods for enum types to enhance functionality and usability.
/// These methods offer a fluent API for working with enumerations in modern .NET applications.
/// </summary>
public static class EnumExtensions
{   
    #region Description Attribute Methods

    /// <summary>
    /// Gets the DisplayName attribute value from an enum value.
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The name of the Description attribute if present; otherwise the enum ToString result.</returns>
    public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        return EnumHelper.GetDescription(enumValue);
    }

    #endregion

    #region String Parsing Extensions

    /// <summary>
    /// Attempts to parse this string as an enum value of the specified type with optional case sensitivity.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to parse to. Must be a struct and enum type.</typeparam>
    /// <param name="value">The string to parse as an enum value.</param>
    /// <param name="ignoreCase">True to ignore case during parsing; false to be case-sensitive.</param>
    /// <returns>The parsed enum value if successful; otherwise null.</returns>
    /// <example>
    /// <code>
    /// var result = "monday".ParseAsEnum&lt;DayOfWeek&gt;(ignoreCase: true);
    /// // result will be DayOfWeek.Monday
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum? ParseAsEnum<TEnum>(this string? value, bool ignoreCase = false) where TEnum : struct, Enum
    {
        return EnumHelper.Parse<TEnum>(value, ignoreCase);
    }

    /// <summary>
    /// Parse a specific display name enumeration string.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value to parse.</param>
    /// <param name="ignoreCase">if set to <see langword="true"/> ignore case while parsing.</param>
    /// <returns>The enumeration object, or null if parsing failed.</returns>
    public static TEnum? ParseFromDescription<TEnum>(this string value, bool ignoreCase = false) where TEnum : struct, Enum
    {
        return EnumHelper.ParseFromDescription<TEnum>(value, ignoreCase);
    }

    #endregion

    #region Flags Enum Extensions

    /// <summary>
    /// Determines if this enum type is a flags enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The enum value (used for type inference).</param>
    /// <returns>True if the enum type has the FlagsAttribute; otherwise false.</returns>
    /// <example>
    /// <code>
    /// bool isFlags = FileAccess.Read.IsFlags(); // returns true if FileAccess has [Flags]
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFlags<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return EnumHelper.IsFlags<TEnum>();
    }

    /// <summary>
    /// Gets all individual flags that are set in this flags enum value.
    /// Only applicable to enums decorated with the FlagsAttribute.
    /// </summary>
    /// <typeparam name="TEnum">The flags enumeration type.</typeparam>
    /// <param name="value">The flags enum value to decompose.</param>
    /// <returns>An enumerable of individual flag values that are set.</returns>
    /// <exception cref="ArgumentException">Thrown when TEnum is not a flags enum.</exception>
    /// <example>
    /// <code>
    /// var permissions = FileAccess.Read | FileAccess.Write;
    /// var flags = permissions.GetFlags().ToArray();
    /// // returns [FileAccess.Read, FileAccess.Write]
    /// </code>
    /// </example>
    public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        if (!value.IsFlags())
            throw new ArgumentException($"Enum type {typeof(TEnum).Name} is not a flags enumeration.", nameof(TEnum));
        
        //if (value.Equals(default(TEnum)))
        //    yield return default(TEnum);

        foreach (var enumValue in EnumHelper.GetValues<TEnum>())
        {
            if (value.HasFlag(enumValue) && !enumValue.Equals(default(TEnum)))
                yield return enumValue;
        }
    }

    /// <summary>
    /// Adds the specified flag to this flags enum value.
    /// Only applicable to enums decorated with the FlagsAttribute.
    /// </summary>
    /// <typeparam name="TEnum">The flags enumeration type.</typeparam>
    /// <param name="value">The original flags enum value.</param>
    /// <param name="flag">The flag to add.</param>
    /// <returns>A new enum value with the specified flag added.</returns>
    /// <exception cref="ArgumentException">Thrown when TEnum is not a flags enum.</exception>
    /// <example>
    /// <code>
    /// var permissions = FileAccess.Read;
    /// var newPermissions = permissions.AddFlag(FileAccess.Write);
    /// // newPermissions = FileAccess.Read | FileAccess.Write
    /// </code>
    /// </example>
    public static TEnum AddFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum
    {
        if (!value.IsFlags())
            throw new ArgumentException($"Enum type {typeof(TEnum).Name} is not a flags enumeration.", nameof(TEnum));

        var result = Convert.ToUInt64(value) | Convert.ToUInt64(flag);
        return (TEnum)Enum.ToObject(typeof(TEnum), result);
    }

    /// <summary>
    /// Removes the specified flag from this flags enum value.
    /// Only applicable to enums decorated with the FlagsAttribute.
    /// </summary>
    /// <typeparam name="TEnum">The flags enumeration type.</typeparam>
    /// <param name="value">The original flags enum value.</param>
    /// <param name="flag">The flag to remove.</param>
    /// <returns>A new enum value with the specified flag removed.</returns>
    /// <exception cref="ArgumentException">Thrown when TEnum is not a flags enum.</exception>
    /// <example>
    /// <code>
    /// var permissions = FileAccess.Read | FileAccess.Write;
    /// var newPermissions = permissions.RemoveFlag(FileAccess.Write);
    /// // newPermissions = FileAccess.Read
    /// </code>
    /// </example>
    public static TEnum RemoveFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum
    {
        if (!value.IsFlags())
            throw new ArgumentException($"Enum type {typeof(TEnum).Name} is not a flags enumeration.", nameof(TEnum));

        var result = Convert.ToUInt64(value) & ~Convert.ToUInt64(flag);
        return (TEnum)Enum.ToObject(typeof(TEnum), result);
    }

    /// <summary>
    /// Toggles the specified flag in this flags enum value.
    /// If the flag is present, it's removed; if not present, it's added.
    /// Only applicable to enums decorate
    /// <summary>
    /// Toggles the specified flag in this flags enum value.
    /// If the flag is present, it's removed; if not present, it's added.
    /// Only applicable to enums decorated with the FlagsAttribute.
    /// </summary>
    /// <typeparam name="TEnum">The flags enumeration type.</typeparam>
    /// <param name="value">The original flags enum value.</param>
    /// <param name="flag">The flag to toggle.</param>
    /// <returns>A new enum value with the specified flag toggled.</returns>
    /// <exception cref="ArgumentException">Thrown when TEnum is not a flags enum.</exception>
    /// <example>
    /// <code>
    /// var permissions = FileAccess.Read;
    /// var toggled1 = permissions.ToggleFlag(FileAccess.Write); // adds Write
    /// // toggled1 = FileAccess.Read | FileAccess.Write
    /// var toggled2 = toggled1.ToggleFlag(FileAccess.Read); // removes Read  
    /// // toggled2 = FileAccess.Write
    /// </code>
    /// </example>
    public static TEnum ToggleFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum
    {
        if (!value.IsFlags())
            throw new ArgumentException($"Enum type {typeof(TEnum).Name} is not a flags enumeration.", nameof(TEnum));

        return value.HasFlag(flag) ? value.RemoveFlag(flag) : value.AddFlag(flag);
    }

    #endregion

    #region Conversion Extensions

    /// <summary>
    /// Converts an enum value from one enum type to another enum type by name matching.
    /// Useful for converting between similar enums in different assemblies or versions.
    /// </summary>
    /// <typeparam name="TSource">The source enumeration type.</typeparam>
    /// <typeparam name="TTarget">The target enumeration type.</typeparam>
    /// <param name="sourceValue">The source enum value to convert.</param>
    /// <param name="ignoreCase">True to ignore case during name matching; false to be case-sensitive.</param>
    /// <returns>The converted enum value if successful; otherwise null.</returns>
    /// <example>
    /// <code>
    /// // Convert between similar enums
    /// var result = OldStatus.Active.ConvertTo&lt;NewStatus&gt;();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTarget? ConvertTo<TSource, TTarget>(this TSource sourceValue, bool ignoreCase = false)
        where TSource : struct, Enum
        where TTarget : struct, Enum
    {
        return EnumHelper.ConvertEnum<TSource, TTarget>(sourceValue, ignoreCase);
    }

    #endregion

    #region Serialization Extensions

    /// <summary>
    /// Converts this enum value to a JSON-friendly string representation.
    /// Uses the description if available, otherwise the enum name.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="useDescription">True to use DescriptionAttribute if available; false to use enum name.</param>
    /// <returns>A JSON-friendly string representation of the enum value.</returns>
    /// <example>
    /// <code>
    /// // Given: enum Status { [Description("Not Started")] NotStarted }
    /// string json = Status.NotStarted.ToJsonString(useDescription: true);
    /// // returns "Not Started"
    /// </code>
    /// </example>
    public static string ToJsonString<TEnum>(this TEnum value, bool useDescription = true) where TEnum : struct, Enum
    {
        return useDescription ? value.GetDescription() : value.ToString();
    }

    /// <summary>
    /// Gets the underlying numeric value of this enum as the specified numeric type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <typeparam name="TNumeric">The target numeric type (byte, int, long, etc.).</typeparam>
    /// <param name="value">The enum value to convert.</param>
    /// <returns>The underlying numeric value converted to the specified type.</returns>
    /// <example>
    /// <code>
    /// int numericValue = DayOfWeek.Monday.GetNumericValue&lt;int&gt;();
    /// // returns 1 (assuming Monday = 1)
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNumeric GetNumericValue<TEnum, TNumeric>(this TEnum value)
        where TEnum : struct, Enum
        where TNumeric : struct, IConvertible
    {
        return (TNumeric)Convert.ChangeType(value, typeof(TNumeric));
    }

    #endregion

    #region Utility Extensions

    /// <summary>
    /// Gets all enum values of the same type as this enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The enum value (used for type inference).</param>
    /// <returns>An array containing all enum values of this type.</returns>
    /// <example>
    /// <code>
    /// var allDays = DayOfWeek.Monday.GetAllValues();
    /// // returns [Sunday, Monday, Tuesday, ...]
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum[] GetAllValues<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return EnumHelper.GetValues<TEnum>();
    }

    /// <summary>
    /// Gets all enum names of the same type as this enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The enum value (used for type inference).</param>
    /// <returns>An array containing all enum names of this type.</returns>
    /// <example>
    /// <code>
    /// var allNames = DayOfWeek.Monday.GetAllNames();
    /// // returns ["Sunday", "Monday", "Tuesday", ...]
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] GetAllNames<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return EnumHelper.GetNames<TEnum>();
    }

    /// <summary>
    /// Determines if this enum value is the default (first) value of its enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The enum value to check.</param>
    /// <returns>True if the value is the default enum value; otherwise false.</returns>
    /// <example>
    /// <code>
    /// bool isDefault = DayOfWeek.Sunday.IsDefault(); // returns true if Sunday = 0
    /// </code>
    /// </example>
    public static bool IsDefault<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return value.Equals(default(TEnum));
    }

    /// <summary>
    /// Gets the next enum value in the sequence, wrapping around to the first value if at the end.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The current enum value.</param>
    /// <returns>The next enum value in the sequence.</returns>
    /// <example>
    /// <code>
    /// var nextDay = DayOfWeek.Monday.GetNext();
    /// // returns DayOfWeek.Tuesday
    /// var wrapped = DayOfWeek.Saturday.GetNext();
    /// // returns DayOfWeek.Sunday (wraps around)
    /// </code>
    /// </example>
    public static TEnum GetNext<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        var values = EnumHelper.GetValues<TEnum>();
        var currentIndex = Array.IndexOf(values, value);
        var nextIndex = (currentIndex + 1) % values.Length;
        return values[nextIndex];
    }

    /// <summary>
    /// Gets the previous enum value in the sequence, wrapping around to the last value if at the beginning.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The current enum value.</param>
    /// <returns>The previous enum value in the sequence.</returns>
    /// <example>
    /// <code>
    /// var prevDay = DayOfWeek.Tuesday.GetPrevious();
    /// // returns DayOfWeek.Monday
    /// var wrapped = DayOfWeek.Sunday.GetPrevious();
    /// // returns DayOfWeek.Saturday (wraps around)
    /// </code>
    /// </example>
    public static TEnum GetPrevious<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        var values = EnumHelper.GetValues<TEnum>();
        var currentIndex = Array.IndexOf(values, value);
        var prevIndex = (currentIndex - 1 + values.Length) % values.Length;
        return values[prevIndex];
    }

    #endregion

}
