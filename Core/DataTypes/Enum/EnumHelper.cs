//--------------------------------------------------------------------------
// File:    EnumHelper.cs
// Content:	Implementation of static class EnumHelper for .NET 8+
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

#endregion


namespace AnBo.Core;

/// <summary>
/// Provides static helper methods for working with enumeration types in a type-safe and performant manner.
/// This class offers caching mechanisms and modern .NET 8+ features for optimal performance.
/// </summary>
public static class EnumHelper
{
    #region Private Fields and Caching

    // Cache for enum names to avoid repeated reflection calls
    private static readonly ConcurrentDictionary<Type, string[]> _enumNamesCache = new();

    // Cache for enum values to avoid repeated reflection calls
    private static readonly ConcurrentDictionary<Type, Array> _enumValuesCache = new();

    #endregion

    #region Parsing Methods

    /// <summary>
    /// Attempts to parse the specified enumeration string into the target enum type with optional case sensitivity.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to parse to. Must be a struct and enum type.</typeparam>
    /// <param name="value">The string representation of the enum value to parse.</param>
    /// <param name="ignoreCase">True to ignore case during parsing; false to be case-sensitive.</param>
    /// <returns>The parsed enum value if successful; otherwise null.</returns>
    /// <exception cref="ArgumentException">Thrown when TEnum is not an enum type.</exception>
    /// <example>
    /// <code>
    /// var result = EnumHelper.Parse&lt;DayOfWeek&gt;("monday", ignoreCase: true);
    /// // result will be DayOfWeek.Monday
    /// </code>
    /// </example>
    public static TEnum? Parse<TEnum>(string? value, bool ignoreCase = false) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Use the modern Enum.TryParse method which is more performant
        return Enum.TryParse<TEnum>(value.AsSpan().Trim(), ignoreCase, out var result) ? result : null;
    }

    /// <summary>
    /// Attempts to parse the specified enumeration string into the target enum type.
    /// This method provides the standard TryParse pattern with an out parameter.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to parse to. Must be a struct and enum type.</typeparam>
    /// <param name="value">The string representation of the enum value to parse.</param>
    /// <param name="result">When this method returns, contains the parsed enum value if successful; otherwise, the default value.</param>
    /// <returns>True if parsing was successful; otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when TEnum is not an enum type.</exception>
    public static bool TryParse<TEnum>(string? value, [MaybeNullWhen(false)] out TEnum result) where TEnum : struct, Enum
    {
        return TryParse(value, ignoreCase: false, out result);
    }

    /// <summary>
    /// Attempts to parse the specified enumeration string into the target enum type with optional case sensitivity.
    /// This method provides the standard TryParse pattern with an out parameter.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to parse to. Must be a struct and enum type.</typeparam>
    /// <param name="value">The string representation of the enum value to parse.</param>
    /// <param name="ignoreCase">True to ignore case during parsing; false to be case-sensitive.</param>
    /// <param name="result">When this method returns, contains the parsed enum value if successful; otherwise, the default value.</param>
    /// <returns>True if parsing was successful; otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when TEnum is not an enum type.</exception>
    public static bool TryParse<TEnum>(string? value, bool ignoreCase, [MaybeNullWhen(false)] out TEnum result) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return false;
        }

        return Enum.TryParse(value.AsSpan().Trim(), ignoreCase, out result);
    }

    #endregion

    #region Validation Methods

    /// <summary>
    /// Determines whether the specified enum value is defined in the enumeration type.
    /// Uses caching for improved performance on repeated calls.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to check against.</typeparam>
    /// <param name="value">The enum value to validate.</param>
    /// <returns>True if the value is defined in the enumeration; otherwise false.</returns>
    /// <example>
    /// <code>
    /// bool isValid = EnumHelper.IsDefined(DayOfWeek.Monday); // returns true
    /// bool isInvalid = EnumHelper.IsDefined((DayOfWeek)999); // returns false
    /// </code>
    /// </example>
    public static bool IsDefined<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return Enum.IsDefined(typeof(TEnum), value);
    }

    /// <summary>
    /// Validates that the specified enum value is defined in the enumeration type.
    /// Throws an exception if the value is not defined.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to validate against.</typeparam>
    /// <param name="value">The enum value to validate.</param>
    /// <param name="argumentName">The name of the argument being validated (used in exception message).</param>
    /// <exception cref="InvalidEnumArgumentException">Thrown when the enum value is not defined in the enumeration.</exception>
    /// <example>
    /// <code>
    /// DayOfWeek dayOfWeek = DayOfWeek.Monday;
    /// var falseValue = 999;
    /// EnumHelper.ValidateIsDefined(dayOfWeek); // succeeds
    /// EnumHelper.ValidateIsDefined((DayOfWeek)falseValue); // throws exception
    /// </code>
    /// </example>
    public static void ValidateIsDefined<TEnum>(
        TEnum value, 
        [CallerArgumentExpression(nameof(value))] string? argumentName = null) 
        where TEnum : struct, Enum
    {
        if (!IsDefined(value))
        {
            throw new InvalidEnumArgumentException(argumentName, Convert.ToInt32(value), typeof(TEnum));
        }
    }

    #endregion

    #region Enum Information Methods

    /// <summary>
    /// Gets all enum names for the specified enumeration type.
    /// Results are cached for improved performance on repeated calls.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <returns>An array of strings containing all enum names.</returns>
    /// <example>
    /// <code>
    /// string[] days = EnumHelper.GetNames&lt;DayOfWeek&gt;();
    /// // returns ["Sunday", "Monday", "Tuesday", ...]
    /// </code>
    /// </example>
    public static string[] GetNames<TEnum>() where TEnum : struct, Enum
    {
        return _enumNamesCache.GetOrAdd(typeof(TEnum), static type => Enum.GetNames(type));
    }

    /// <summary>
    /// Gets all enum values for the specified enumeration type.
    /// Results are cached for improved performance on repeated calls.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <returns>An array containing all enum values.</returns>
    /// <example>
    /// <code>
    /// TEnum[] days = EnumHelper.GetValues&lt;DayOfWeek&gt;();
    /// // returns [DayOfWeek.Sunday, DayOfWeek.Monday, ...]
    /// </code>
    /// </example>
    public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
    {
        var cachedArray = _enumValuesCache.GetOrAdd(typeof(TEnum), static type => Enum.GetValues(type));
        return (TEnum[])cachedArray;
    }

    /// Gets the underlying numeric type of the specified enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <returns>The Type object representing the underlying numeric type.</returns>
    /// <example>
    /// <code>
    /// Type underlyingType = EnumHelper.GetUnderlyingType&lt;DayOfWeek&gt;();
    /// // returns typeof(int)
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetUnderlyingType<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetUnderlyingType(typeof(TEnum));
    }

    #endregion

    #region Attribute-Based Methods

    /// <summary>
    /// Gets the description for an enum value from its DescriptionAttribute.
    /// Falls back to ToString() if no DescriptionAttribute is present.
    /// Results are cached for improved performance.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The enum value to get the description for.</param>
    /// <returns>The description from DescriptionAttribute or string representation of the enum value.</returns>
    /// <example>
    /// <code>
    /// // Given: enum Status { [Description("Not Started")] NotStarted }
    /// string desc = EnumHelper.GetDescription(Status.NotStarted);
    /// // returns "Not Started"
    /// </code>
    /// </example>
    public static string GetDescription<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);

        // Handle flags enums specially
        if (IsFlags<TEnum>())
        {
            return GetFlagsDescription(enumType, value);
        }

        var fieldInfo = enumType.GetField(value.ToString()!);

        var descAttr = fieldInfo?.GetCustomAttribute<DescriptionAttribute>(inherit: false);

        return descAttr?.Description ?? value.ToString()!;
    }

    /// <summary>
    /// Attempts to parse an enum value from its description attribute.
    /// Searches through DescriptionAttribute.Description values.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="description">The description to parse.</param>
    /// <param name="ignoreCase">True to ignore case during comparison; false to be case-sensitive.</param>
    /// <returns>The enum value if found; otherwise null.</returns>
    /// <example>
    /// <code>
    /// // Given: enum Status { [Description("Not Started")] NotStarted }
    /// var result = EnumHelper.ParseFromDescription&lt;Status&gt;("Not Started");
    /// // returns Status.NotStarted
    /// </code>
    /// </example>
    public static TEnum? ParseFromDescription<TEnum>(string? description, bool ignoreCase = false) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        description = description.Trim();
        Type type = typeof(TEnum);

        string[] enumNames = EnumHelper.GetNames<TEnum>();

        foreach (string enumName in enumNames)
        {
            var field = type.GetField(enumName.ToString());

            var attribute = field?.GetCustomAttribute<DescriptionAttribute>(inherit: false);

            string desc = attribute?.Description ?? enumName;

            if (ignoreCase)
            {
                if (description.Contains(desc, StringComparison.CurrentCultureIgnoreCase) && !string.Equals(desc, enumName, StringComparison.CurrentCultureIgnoreCase))
                    description = description.Replace(desc, enumName, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                if (description.Contains(desc, StringComparison.CurrentCulture) && !string.Equals(desc, enumName, StringComparison.CurrentCulture))
                    description = description.Replace(desc, enumName, StringComparison.CurrentCulture);
            }

        }

        TEnum? enumType = EnumHelper.Parse<TEnum>(description, ignoreCase);

        return enumType;
    }

    #endregion

    #region Flags Enum Support

    /// <summary>
    /// Determines if the specified enum type is a flags enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to check.</typeparam>
    /// <returns>True if the enum type has the FlagsAttribute; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFlags<TEnum>() where TEnum : struct, Enum
    {
        return typeof(TEnum).HasAttribute<FlagsAttribute>();
    }

    ///// <summary>
    ///// Determines if the specified enum value has the specified flag set.
    ///// Only applicable to enums decorated with the FlagsAttribute.
    ///// </summary>
    ///// <typeparam name="TEnum">The flags enumeration type.</typeparam>
    ///// <param name="value">The enum value to check.</param>
    ///// <param name="flag">The flag to check for.</param>
    ///// <returns>True if the flag is set; otherwise false.</returns>
    ///// <exception cref="ArgumentException">Thrown when TEnum is not a flags enum.</exception>
    ///// <example>
    ///// <code>
    ///// var permissions = FileAccess.Read | FileAccess.Write;
    ///// bool canRead = EnumHelper.HasFlag(permissions, FileAccess.Read); // returns true
    ///// </code>
    ///// </example>
    //public static bool HasFlag<TEnum>(TEnum value, TEnum flag) where TEnum : struct, Enum
    //{
    //    if (!IsFlags<TEnum>())
    //        throw new ArgumentException($"Enum type {typeof(TEnum).Name} is not a flags enumeration.", nameof(TEnum));

    //    return value.HasFlag(flag);
    //}

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Gets the description for flags enum values by combining individual flag descriptions.
    /// </summary>
    /// <param name="enumType">The enum type.</param>
    /// <param name="enumValue">The enum value (as object).</param>
    /// <returns>Combined description string for flags enum.</returns>
    private static string GetFlagsDescription(Type enumType, object enumValue)
    {
        var enumStringValue = enumValue.ToString()!;
        var flagNames = enumStringValue.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

        if (flagNames.Length == 1)
        {
            // Single flag, get its description normally
            var fieldInfo = enumType.GetField(flagNames[0].Trim());
            if (fieldInfo == null)
                return enumStringValue;

            // Check for DescriptionAttribute (the only attribute that works on enum fields)
            var descAttr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            return descAttr?.Description ?? enumStringValue;
        }

        // Multiple flags, combine descriptions
        var descriptions = new List<string>();
        foreach (var flagName in flagNames)
        {
            var fieldInfo = enumType.GetField(flagName.Trim());
            if (fieldInfo == null)
            {
                descriptions.Add(flagName.Trim());
                continue;
            }

            var descAttr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            descriptions.Add(descAttr?.Description ?? flagName.Trim());
        }

        return string.Join(", ", descriptions);
    }

    #endregion

    #region Conversion Methods

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
    /// var result = EnumHelper.ConvertEnum&lt;OldStatus, NewStatus&gt;(OldStatus.Active);
    /// </code>
    /// </example>
    public static TTarget? ConvertEnum<TSource, TTarget>(TSource sourceValue, bool ignoreCase = false)
        where TSource : struct, Enum
        where TTarget : struct, Enum
    {
        var sourceName = sourceValue.ToString();
        return Parse<TTarget>(sourceName, ignoreCase);
    }

    #endregion

}