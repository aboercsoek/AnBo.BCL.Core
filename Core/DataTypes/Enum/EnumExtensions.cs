//--------------------------------------------------------------------------
// File:    EnumExtensions.cs
// Content:	Implementation of class EnumExtensions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.ComponentModel;
using System.Text;

#endregion

namespace AnBo.Core;

/// <summary>
/// Extension methods for enum types to provide additional functionality
/// </summary>
public static class EnumExtensions
{
    #region Description Attribute Methods

    /// <summary>
    /// Gets the DisplayName attribute value from an enum value.
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The name of the Description attribute if present; otherwise the enum ToString result.</returns>
    public static string GetDescription(this Enum enumValue)
    {
        Type type = enumValue.GetType();

        if (type.HasAttribute<FlagsAttribute>() == false)
        {
            var field = type.GetField(enumValue.ToString());

            var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                             .FirstOrDefault() as DescriptionAttribute;
            return attribute?.Description ?? enumValue.ToString();
        }

        string[] enumValues = enumValue.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

        var result = new StringBuilder();
        bool firstIteration = true;

        foreach (string value in enumValues)
        {
            var field = type.GetField(value.Trim());

            var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                             .FirstOrDefault() as DescriptionAttribute;
            string name = attribute?.Description ?? enumValue.ToString();

            if (firstIteration)
            {
                result.Append(name);
                firstIteration = false;
            }
            else
            {
                result.Append(", " + name);
            }

        }

        return result.ToString();
    }

    #endregion
}
