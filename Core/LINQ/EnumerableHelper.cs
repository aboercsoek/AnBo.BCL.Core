//--------------------------------------------------------------------------
// File:    CollectionHelper.cs
// Content:	Implementation of class CollectionHelper
// Author:	Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------
#region Using directives

using System.Collections;
using System.Diagnostics;

#endregion

namespace AnBo.Core;

/*
--------------------------------------------------------------------------
 Info:
 All methods in this class are now implemented as extension methods
 in EnumerableExtensions or are redundant with LINQ methods.
--------------------------------------------------------------------------
*/

/*
      Removed redundant methods:
        ConvertAll - Identical to LINQ Select
        FindAll - Identical to LINQ Where
        Complement - Identical to LINQ Except
        FindIndex - Already available in EnumerableExtensions
        ForEach - Redundant to extension method
        Sort - Identical to LINQ OrderBy
        ToList - Already available in LINQ
        DictionaryKeysToArray/DictionaryValuesToArray - Can be accessed directly with .Keys.ToArray()
        Unsafe UnsafeToArray methods - Replaced with type-safe alternatives

    Methods that were implemented as extension methods and moved to EnumerableExtensions:
        FindLastIndex - Moved to EnumerableExtensions

*/

//public static class EnumerableHelper
//{

    

//}
