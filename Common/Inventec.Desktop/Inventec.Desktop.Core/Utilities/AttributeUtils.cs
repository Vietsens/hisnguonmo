#region License

// Created by phuongdt

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Inventec.Desktop.Core.Utilities
{
    /// <summary>
    /// Utilities class that provides a set of convenience methods for working with attributes.
    /// </summary>
    public static class AttributeUtils
    {
        /// <summary>
        /// Searches a type/method/property/field for attributes of the specified type, matching the
        /// specified filter.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <param name="inherit">True to include inherited attributes in the search.</param>
        /// <param name="filter">A filter that restricts the results of the search.</param>
        /// <returns>A list of matching attributes.</returns>
        public static List<TAttribute> GetAttributes<TAttribute>(ICustomAttributeProvider attrProvider, bool inherit, Predicate<TAttribute> filter)
            where TAttribute : Attribute
        {
            return CollectionUtils.Map<TAttribute, TAttribute, List<TAttribute>>(
                CollectionUtils.Select<TAttribute>(attrProvider.GetCustomAttributes(typeof(TAttribute), inherit), filter),
                delegate(TAttribute obj) { return obj; });
        }

        /// <summary>
        /// Searches a type/method/property/field for attributes of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <param name="inherit">True to include inherited attributes in the search.</param>
        /// <returns>A list of matching attributes.</returns>
		public static List<TAttribute> GetAttributes<TAttribute>(ICustomAttributeProvider attrProvider, bool inherit)
            where TAttribute : Attribute
        {
            return GetAttributes<TAttribute>(attrProvider, inherit, NullFilter);
        }

        /// <summary>
        /// Searches a type/method/property/field for attributes of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <returns>A list of matching attributes.</returns>
		public static List<TAttribute> GetAttributes<TAttribute>(ICustomAttributeProvider attrProvider)
            where TAttribute : Attribute
        {
            return GetAttributes<TAttribute>(attrProvider, false);
        }

        /// <summary>
        /// Searches a type/method/property/field for attributes of the specified type, returning the first match.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <param name="inherit">True to include inherited attributes in the search.</param>
        /// <param name="filter">A filter that restricts the results of the search.</param>
        /// <returns>The first matching attribute instance, or null if no matches are found.</returns>
		public static TAttribute GetAttribute<TAttribute>(ICustomAttributeProvider attrProvider, bool inherit, Predicate<TAttribute> filter)
            where TAttribute : Attribute
        {
            return CollectionUtils.SelectFirst<TAttribute>(attrProvider.GetCustomAttributes(typeof(TAttribute), inherit), filter);
        }

        /// <summary>
        /// Searches a type/method/property/field for attributes of the specified type, returning the first match.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <param name="inherit">True to include inherited attributes in the search.</param>
        /// <returns>The first matching attribute instance, or null if no matches are found.</returns>
		public static TAttribute GetAttribute<TAttribute>(ICustomAttributeProvider attrProvider, bool inherit)
            where TAttribute : Attribute
        {
            return GetAttribute<TAttribute>(attrProvider, inherit, NullFilter);
        }

        /// <summary>
        /// Searches a type/method/property/field for attributes of the specified type, returning the first match.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <returns>The first matching attribute instance, or null if no matches are found.</returns>
		public static TAttribute GetAttribute<TAttribute>(ICustomAttributeProvider attrProvider)
            where TAttribute : Attribute
        {
            return GetAttribute<TAttribute>(attrProvider, false);
        }

        /// <summary>
        /// Tests a type/method/property/field for the presence of an attribute of the specified type, and matching
        /// the specified filter.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <param name="inherit">True to include inherited attributes in the search.</param>
        /// <param name="filter">A filter that restricts the results of the search.</param>
        /// <returns>True if a match is found, otherwise false.</returns>
		public static bool HasAttribute<TAttribute>(ICustomAttributeProvider attrProvider, bool inherit, Predicate<TAttribute> filter)
            where TAttribute : Attribute
        {
            return CollectionUtils.Contains<TAttribute>(
                attrProvider.GetCustomAttributes(typeof(TAttribute), inherit),
                filter);
        }

        /// <summary>
        /// Tests a type/method/property/field for the presence of an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <param name="inherit">True to include inherited attributes in the search.</param>
        /// <returns>True if a match is found, otherwise false.</returns>
		public static bool HasAttribute<TAttribute>(ICustomAttributeProvider attrProvider, bool inherit)
            where TAttribute : Attribute
        {
            return HasAttribute<TAttribute>(attrProvider, inherit, NullFilter);
        }

        /// <summary>
        /// Tests a type/method/property/field for the presence of an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute (may also be a base class).</typeparam>
        /// <param name="attrProvider">The type/method/property/field to find attributes on.</param>
        /// <returns>True if a match is found, otherwise false.</returns>
		public static bool HasAttribute<TAttribute>(ICustomAttributeProvider attrProvider)
           where TAttribute : Attribute
        {
            return HasAttribute<TAttribute>(attrProvider, false);
        }

        private static bool NullFilter(object obj)
        {
            return true;
        }
    }
}
