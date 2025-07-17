//--------------------------------------------------------------------------
// File:    ToStringOptions.cs
// Content:	Implementation of class ToStringOptions
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

#endregion

namespace AnBo.Core
{
    /// <summary>
    /// Provides configuration options for customizing string conversion behavior.
    /// This class defines settings that control how objects are converted to their string representations,
    /// including handling of null values, collections, and formatting options.
    /// </summary>
    public class ToStringOptions
    {
        /// <summary>
        /// Gets the default instance of <see cref="ToStringOptions"/> with standard configuration values.
        /// This instance uses default settings suitable for most string conversion scenarios.
        /// </summary>
        public static readonly ToStringOptions Default = new();

        /// <summary>
        /// Gets or sets the string representation used for null values.
        /// </summary>
        /// <value>
        /// A string that represents null values. The default value is "&lt;null&gt;".
        /// </value>
        public string NullString { get; set; } = "<null>";

        /// <summary>
        /// Gets or sets the maximum number of items to include when converting collections to strings.
        /// </summary>
        /// <value>
        /// The maximum number of collection items to display. The default value is 100.
        /// If a collection contains more items than this limit, only the first items up to this limit will be included.
        /// </value>
        public int MaxCollectionItems { get; set; } = 100;

        /// <summary>
        /// Gets or sets a value indicating whether to show the total count of items in collections.
        /// </summary>
        /// <value>
        /// <c>true</c> if the collection count should be displayed; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        public bool ShowCollectionCount { get; set; } = true;

        /// <summary>
        /// Gets or sets the separator string used between collection items.
        /// </summary>
        /// <value>
        /// A string used to separate individual items in collection representations.
        /// The default value is ", " (comma followed by space).
        /// </value>
        public string CollectionSeparator { get; set; } = ", ";

        /// <summary>
        /// Gets or sets the separator string used between dictionary keys and values.
        /// </summary>
        /// <value>
        /// A string used to separate keys from values in dictionary representations.
        /// The default value is ": " (colon followed by space).
        /// </value>
        public string DictionaryKeyValueSeparator { get; set; } = ": ";

        /// <summary>
        /// Gets or sets a value indicating whether to display array dimensions in the string representation.
        /// </summary>
        /// <value>
        /// <c>true</c> if array dimensions should be included in the output; otherwise, <c>false</c>.
        /// The default value is <c>false</c>. When enabled, multidimensional arrays will show their dimension information.
        /// </value>
        public bool ShowArrayDimensions { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum depth for nested object conversion to prevent infinite recursion.
        /// </summary>
        /// <value>
        /// The maximum nesting depth allowed during string conversion. The default value is 10.
        /// When this depth is exceeded, further nested objects will be represented with a placeholder
        /// to prevent stack overflow and infinite loops in circular object references.
        /// </value>
        public int MaxNestingDepth { get; set; } = 10;

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="DateTime"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom date and time format string. The default value is "yyyy-MM-dd HH:mm:ss".
        /// Uses the standard .NET DateTime format strings.
        /// </value>
        /// <seealso cref="DateTime.ToString(string)"/>
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="DateTimeOffset"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom date and time offset format string. The default value is "yyyy-MM-dd HH:mm:ss zzz".
        /// Uses the standard .NET DateTime format strings.
        /// </value>
        /// <seealso cref="DateTimeOffset.ToString(string)"/>   
        public string DateTimeOffsetFormat { get; set; } = "yyyy-MM-dd HH:mm:ss zzz";

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="DateOnly"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom date format string. The default value is "yyyy-MM-dd".
        /// Uses the standard .NET DateOnly format strings.
        /// </value>
        /// <seealso cref="DateOnly.ToString(string)"/>
        public string DateOnlyFormat { get; set; } = "yyyy-MM-dd";

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="TimeOnly"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom time format string. The default value is "HH:mm:ss".
        /// Uses the standard .NET TimeOnly format strings.
        /// </value>
        /// <seealso cref="TimeOnly.ToString(string)"/>
        public string TimeOnlyFormat { get; set; } = "HH:mm:ss";

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="TimeSpan"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom TimeSpan format string. The default value is "c" (constant format).
        /// The constant format displays time spans in the format [-][d.]hh:mm:ss[.fffffff].
        /// </value>
        /// <seealso cref="TimeSpan.ToString(string)"/>
        public string TimeSpanFormat { get; set; } = "c"; // Constant format

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="decimal"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom numeric format string. The default value is an empty string,
        /// which uses the default decimal formatting behavior.
        /// </value>
        /// <seealso cref="decimal.ToString(string)"/>
        public string DecimalFormat { get; set; } = ""; // Empty = Standard

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="double"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom numeric format string. The default value is an empty string,
        /// which uses the default double formatting behavior.
        /// </value>
        /// <seealso cref="double.ToString(string)"/>
        public string DoubleFormat { get; set; } = ""; // Empty = Standard

        /// <summary>
        /// Gets or sets the format string used for converting <see cref="float"/> values to strings.
        /// </summary>
        /// <value>
        /// A standard or custom numeric format string. The default value is an empty string,
        /// which uses the default float formatting behavior.
        /// </value>
        /// <seealso cref="float.ToString(string)"/>
        public string FloatFormat { get; set; } = ""; // Empty = Standard
    }
}
