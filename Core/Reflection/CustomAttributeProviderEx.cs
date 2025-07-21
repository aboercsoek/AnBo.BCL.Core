//--------------------------------------------------------------------------
// File:    CustomAttributeProviderEx.cs
// Content:	Implementation of class CustomAttributeProviderEx
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AnBo.Core;

///<summary>Custom attributes extension methods</summary>
public static class CustomAttributeProviderEx
{
    #region HasAttribute methods

    /// <summary>
    /// Determines whether the specified reflection type has the custom attribute of type T.
    /// </summary>
    /// <param name="self">The custom attribute provider.</param>
    /// <param name="attributeType">Type of the the custom attribute.</param>
    /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified reflection type has the custom attribute of type attributeType; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool HasAttribute(this ICustomAttributeProvider self, Type attributeType, bool inherit = true)
    {
        if (self == null)
        {
            return false;
        }
        return self.IsDefined(attributeType, inherit: inherit);
    }

    /// <summary>
    /// Checks if the type has an attribute of the specified type <typeparamref name="TAttribute"/> defined on it.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to search for</typeparam>
    /// <param name="type">The type to check</param>
    /// <returns>true if the attribute exists, false otherwise</returns>
    public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider self, bool inherit = true)
        where TAttribute : Attribute
    {
        return self.HasAttribute(typeof(TAttribute), inherit);
    }

    #endregion

    #region Get- und TryGetAttribute methods

    /// <summary>
    /// Returns the custom attribute of type T defined on this member.
    /// </summary>
    /// <typeparam name="T">Type of the the custom attribute</typeparam>
    /// <param name="self">The custom attribute provider.</param>
    /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
    /// <returns>
    /// 	The first custom attribute of type T defined on this member or <see langword="null" /> if no custom attribute of type T is defined on this member.
    /// </returns>
    public static TAttribute? GetAttribute<TAttribute>(this ICustomAttributeProvider self, bool inherit = true)
        where TAttribute : Attribute
    {
        return (self == null) ? default : (TAttribute?)self.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();
    }

    /// <summary>
    /// Tries to get an attribute of the specified type <typeparamref name="TAttribute"/> from the type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to search for</typeparam>
    /// <param name="self">The custom attribute provider.</param>
    /// <param name="attribute">The found attribute (null if not present)</param>
    /// <returns>true if the attribute exists, false otherwise</returns>
    public static bool TryGetAttribute<TAttribute>(this ICustomAttributeProvider self, out TAttribute? attribute, bool inherit = true)
        where TAttribute : Attribute
    {
        if (self == null)
        {
            attribute = default;
            return false;
        }

        attribute = (TAttribute?)self.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();
        return attribute != null;
    }

    #endregion

    #region GetAttributes and GetAllAttributes methods

    /// <summary>
    /// Returns the custom attributes of type T defined on this member.
    /// </summary>
    /// <typeparam name="T">Type of the the custom attribute</typeparam>
    /// <param name="self">The custom attribute provider.</param>
    /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
    /// <returns>
    /// 	The custom attributes of type T defined on this member.
    /// </returns>
    public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider? self, bool inherit = true)
    {
        return (self == null) ? [] : self.GetCustomAttributes(typeof(T), inherit).Select(attr => (T)attr);
    }

    /// <summary>
    /// Gets all attributes of the specified type <typeparamref name="TAttribute"/> from the type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to search for</typeparam>
    /// <param name="self">The type to check</param>
    /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events</param>
    /// <returns>An enumerable collection of attributes of type <typeparamref name="TAttribute"/>, or an empty collection if no attributes are found</returns>
    public static IEnumerable<TAttribute> GetAllAttributes<TAttribute>(this Type? self, bool inherit = true)
        where TAttribute : Attribute
    {
        if (self == null)
            return Enumerable.Empty<TAttribute>();

        var attributes = new List<TAttribute>();

        // Wie ein Aufzug: Etage für Etage nach oben
        while (self != null)
        {
            attributes.AddRange(self.GetCustomAttributes<TAttribute>(inherit: false));
            if (inherit == false)
                break;
            self = self.BaseType;
        }

        return attributes;
    }

    #endregion

    #region FindAttribute methods

    /// <summary>
    /// Finds the first attribute of the specified type <typeparamref name="TAttribute"/> that matches the given predicate.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to search for</typeparam>
    /// <param name="self">The type to check for attributes</param>
    /// <param name="predicate">A function to test each attribute for a condition</param>
    /// <returns>The first attribute that matches the predicate, or null if no matching attribute is found or if type is null</returns>
    public static TAttribute? FindAttribute<TAttribute>(this Type? self, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        if (self == null)
            return null;

        return self.GetAllAttributes<TAttribute>().FirstOrDefault(predicate);
    }

    #endregion

    #region Type Member Attribute extension methods

    /// <summary>
    /// Checks if a type member (property, field, method, etc.) has a specific attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to search for</typeparam>
    /// <param name="type">The type to check</param>
    /// <returns>true if the attribute exists, false otherwise</returns>
    public static bool HasMemberAttribute<TAttribute>(this Type type, string memberName) where TAttribute : Attribute
    {
        if (type == null)
            return false;

        var memberInfo = type.GetMember(memberName).FirstOrDefault();

        return memberInfo == null ? false : memberInfo.IsDefined(typeof(TAttribute), inherit: false);
    }

    /// <summary>
    /// Checks if a type member (property, field, method, etc.) has a specific attribute and tries to get it.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to search for</typeparam>
    /// <param name="type">The type to check</param>
    /// <param name="attribute">The found attribute (null if not found)</param>
    /// <returns>true if the attribute exists, false otherwise</returns>
    public static bool TryGetMemberAttribute<TAttribute>(this Type type, string memberName, out TAttribute? attribute)
        where TAttribute : Attribute
    {
        if (type == null)
        {
            attribute = default!;
            return false;
        }

        var memberInfo = type.GetMember(memberName).FirstOrDefault();

        if (memberInfo == null)
        {
            attribute = default!;
            return false;
        }

        attribute = memberInfo.GetCustomAttribute<TAttribute>(inherit: false);

        return attribute != null;
    }

    #endregion

}
