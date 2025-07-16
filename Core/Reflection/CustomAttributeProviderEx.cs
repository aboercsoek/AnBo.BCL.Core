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

namespace AnBo.Core
{
    ///<summary>TODO: Description of class CustomAttributeProviderEx</summary>
	public static class CustomAttributeProviderEx
    {
        /// <summary>
        /// Determines whether the specified reflection type has the custom attribute of type T.
        /// </summary>
        /// <typeparam name="T">Type of the the custom attribute</typeparam>
        /// <param name="self">The custom attribute provider.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified reflection type has the custom attribute of type T; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider self)
        {
            return self.HasAttribute(typeof(T), true);
        }

        /// <summary>
        /// Determines whether the specified reflection type has the custom attribute of type T.
        /// </summary>
        /// <typeparam name="T">Type of the the custom attribute</typeparam>
        /// <param name="self">The custom attribute provider.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified reflection type has the custom attribute of type T; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider self, bool inherit)
        {
            return self.HasAttribute(typeof(T), inherit);
        }

        /// <summary>
        /// Determines whether the specified reflection type has the custom attribute of type T.
        /// </summary>
        /// <param name="self">The custom attribute provider.</param>
        /// <param name="attributeType">Type of the the custom attribute.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified reflection type has the custom attribute of type attributeType; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasAttribute(this ICustomAttributeProvider self, Type attributeType)
        {
            return self.HasAttribute(attributeType, true);
        }

        /// <summary>
        /// Determines whether the specified reflection type has the custom attribute of type T.
        /// </summary>
        /// <param name="self">The custom attribute provider.</param>
        /// <param name="attributeType">Type of the the custom attribute.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>
        /// 	<see langword="true"/> if the specified reflection type has the custom attribute of type attributeType; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool HasAttribute(this ICustomAttributeProvider self, Type attributeType, bool inherit)
        {
            return (self == null) ? false : self.GetCustomAttributes(attributeType, inherit).Length > 0;
        }



        /// <summary>
        /// Returns the custom attribute of type T defined on this member.
        /// </summary>
        /// <typeparam name="T">Type of the the custom attribute</typeparam>
        /// <param name="self">The custom attribute provider.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>
        /// 	The first custom attribute of type T defined on this member or <see langword="null" /> if no custom attribute of type T is defined on this member.
        /// </returns>
        public static T? GetAttribute<T>(this ICustomAttributeProvider self, bool inherit)
        {
            return (self == null) ? default : (T?)self.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        /// <summary>
        /// Returns the custom attributes of type T defined on this member.
        /// </summary>
        /// <typeparam name="T">Type of the the custom attribute</typeparam>
        /// <param name="self">The custom attribute provider.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>
        /// 	The custom attributes of type T defined on this member.
        /// </returns>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider self, bool inherit)
        {

            return (self == null) ? [] : self.GetCustomAttributes(typeof(T), inherit).Select(attr => (T)attr);
        }

        /// <summary>
        /// Gets the custom attribute from the type hierarchy.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Returns the custom attribute from the type hierarchy as tuple where item1 contains the custom attribute
        /// and item2 contains the type in the type hierachy where the custom attribute was found.
        /// </returns>
        public static Tuple<TAttribute?, Type?> GetCustomAttributeFromHierarchy<TAttribute>(this Type type) where TAttribute : Attribute
        {
            ArgChecker.ShouldNotBeNull(type);

            if (type.GetCustomAttributes(typeof(TAttribute), true).Length < 1)
            {
                return new Tuple<TAttribute?, Type?>(default, null);
            }

            while (true)
            {
                object[] customAttributes = type.GetCustomAttributes(typeof(TAttribute), false);
                if (customAttributes.Length < 1)
                {
                    var tempType = type.BaseType;
                    type = tempType ?? throw new InvalidOperationRequestException("GetCustomAttributeFromHierarchy",
                         "Types were found in hierarchy, but not for any particular type from it");

                    if (type == TypeOf.Object)
                    {
                        throw new InvalidOperationRequestException("GetCustomAttributeFromHierarchy",
                            "Types were found in hierarchy, but not for any particular type from it");
                    }
                }
                else
                {
                    if (customAttributes.Length > 1)
                    {
                        throw new InvalidOperationRequestException("GetCustomAttributeFromHierarchy",
                            "Type {0} from hierarchy has multiple attributes of type {1}, which is not allowed.".SafeFormatWith(
                            type.ToString().QuoteIfNeeded(), typeof(TAttribute).QuoteIfNeeded()));
                    }
                    return new Tuple<TAttribute?, Type?>((TAttribute)customAttributes[0], type);
                }
                
            }
        }



    }
}
