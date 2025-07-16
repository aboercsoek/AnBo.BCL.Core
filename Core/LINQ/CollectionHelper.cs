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

namespace AnBo.Core
{
    ///<summary>Collection Helper Methods</summary>
	public static class CollectionHelper
    {
        /// <summary>
        /// Converts all the items of type T in collection to a new collection of type U according to converter
        /// </summary>
        /// <typeparam name="TSource">The source collection item type.</typeparam>
        /// <typeparam name="TTarget">The target collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="converter">The converter that converts source collection item of type T into traget items of type U</param>
        /// <returns>The target collection with items of type U</returns>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="collection"/> or <paramref name="converter"/> is null.</exception>
        public static IEnumerable<TTarget> ConvertAll<TSource, TTarget>(IEnumerable<TSource> collection, Converter<TSource, TTarget> converter)
        {
            #region PreConditions

            if (collection == null)
                throw new ArgumentNullException("collection");

            if (converter == null)
                throw new ArgumentNullException("converter");

            #endregion

            return collection.Select(item => converter(item));
        }

        /// <summary>
        /// Finds all the items in collection that satisfy the predicate match. Same as LINQ's Where
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="match">The predicate that defines the match condition.</param>
        /// <returns>All items that meet the predicate condition.</returns>
        /// <exception cref="ArgumentNullException">Is thrown if either <paramref name="collection"/> or <paramref name="match"/> is null.</exception>
        public static IEnumerable<T> FindAll<T>(IEnumerable<T> collection, Predicate<T> match)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            return collection.Where(item => match(item));
        }

        /// <summary>
        /// Finds all the items in collection1 that are not in collection2
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection1">The source collection.</param>
        /// <param name="collection2">The complement check collection</param>
        /// <returns>All items in collection1 that are not in collection2.</returns>
        public static IEnumerable<T> Complement<T>(IEnumerable<T> collection1, IEnumerable<T> collection2) where T : IEquatable<T>
        {
            return collection1.Where(item => collection2.Contains(item) == false);
        }

        /// <summary>
        /// Find the index of the first occurrence of item in collection
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="value">The value to find the index for.</param>
        /// <returns>The index of the first occurrence of item in collection or -1 if item was not found in the given collection.</returns>
        public static int FindIndex<T>(IEnumerable<T> collection, T value) where T : IEquatable<T>
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            int index = 0;

            foreach (T item in collection)
            {
                if (item.Equals(value) == false)
                {
                    index++;
                }
                else
                {
                    return index;
                }
            }
            return -1;
        }


        /// <summary>
        /// Find the index of the last occurrence of item in collection
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="value">The value to find the last index for.</param>
        /// <returns>Index of the last occurrence of item in collection or -1 if item was not found in the collection.</returns>
        public static int FindLastIndex<T>(IEnumerable<T> collection, T value) where T : IEquatable<T>
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            int last = -1 + collection.Count(item => item.Equals(value));

            return last;
        }

        /// <summary>
        ///  Performs an action on every item in collection
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="action">The action to perform on eatch collection item</param>
        public static void ForEach<T>(IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            foreach (T item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        /// Sorts the collection
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <returns>The sorted input collection.</returns>
        public static IEnumerable<T> Sort<T>(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            List<T> list = new List<T>(collection);
            list.Sort();

            return list;
        }

        /// <summary>
        /// Converts iterator to an array
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="iterator"></param>
        /// <returns>The collection behind iterator as array.</returns>
        public static T[] ToArray<T>(IEnumerator<T> iterator)
        {
            if (iterator == null)
            {
                throw new ArgumentNullException("iterator");
            }
            List<T> list = new List<T>();

            while (iterator.MoveNext())
            {
                list.Add(iterator.Current);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Converts the items in collection to an array
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="count">Initial size for optimization</param>
        /// <returns>The converted array of type T witch contains the elements of the collection.</returns>
        public static T[] ToArray<T>(IEnumerable<T> collection, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            return ToArray(collection.GetEnumerator(), count);
        }

        /// <summary>
        /// Converts the items in iterator to an array
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="iterator">The collection iterator.</param>
        /// <param name="count">Initial size for optimization</param>
        /// <returns>Items retrieved during iterator walk trough as array</returns>
        public static T[] ToArray<T>(IEnumerator<T> iterator, int count)
        {
            if (iterator == null)
            {
                throw new ArgumentNullException("iterator");
            }
            List<T> list = new List<T>(count);

            while (iterator.MoveNext())
            {
                list.Add(iterator.Current);
            }

            return list.ToArray();
        }
        /// <summary>
        /// Converts the items in collection to an array of type U according to the converter
        /// </summary>
        /// <typeparam name="TSource">The source collection item type.</typeparam>
        /// <typeparam name="TTarget">The trarget collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="converter">The converter used to convert the input collection items.</param>
        /// <returns>Converted items collection as array.</returns>
        public static TTarget[] ToArray<TSource, TTarget>(IEnumerable<TSource> collection, Converter<TSource, TTarget> converter)
        {
            int count = collection.Count();
            return ToArray(collection, converter, count);
        }

        /// <summary>
        /// Converts the items in collection to an array  of type U according to the converter
        /// </summary>
        /// <typeparam name="TSource">The source collection item type.</typeparam>
        /// <typeparam name="TTarget">The trarget collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="converter">The converter used to convert the input collection items.</param>
        /// <param name="count">Initial size for optimization</param>
        /// <returns>Converted items collection as array.</returns>
        public static TTarget[] ToArray<TSource, TTarget>(IEnumerable<TSource> collection, Converter<TSource, TTarget> converter, int count)
        {
            List<TTarget> list = new List<TTarget>(count);

            list.AddRange(collection.Select(t => converter(t)));

            return list.ToArray();
        }

        /// <summary>
        /// Converts a IEnumerable collection into a list.
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <returns>New created list with input collection items as content.</returns>
        public static IList<T> ToList<T>(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return new List<T>(collection);
        }

        /// <summary>
        /// Converts all the items in the object-based collection of the type T to a new array of type U according to converter
        /// </summary>
        /// <typeparam name="TSource">The source collection item type.</typeparam>
        /// <typeparam name="TTarget">The target array item type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="converter">The object type converter.</param>
        /// <returns>The converted source collection as array.</returns>
        public static TTarget[] UnsafeToArray<TSource, TTarget>(IEnumerable collection, Converter<TSource, TTarget> converter)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            IEnumerable<TTarget> newCollection = UnsafeConvertAll(collection, converter);
            return newCollection.ToArray();
        }

        /// <summary>
        /// Converts all the items in the object-based iterator of the type T to a new array of type U according to converter
        /// </summary>
        /// <typeparam name="TSource">The source collection item type.</typeparam>
        /// <typeparam name="TTarget">The target array item type.</typeparam>
        /// <param name="iterator">The source collection iterator.</param>
        /// <param name="converter">The object type converter.</param>
        /// <returns>The converted source collection returned by iterator as array.</returns>
        public static TTarget[] UnsafeToArray<TSource, TTarget>(IEnumerator iterator, Converter<TSource, TTarget> converter)
        {
            if (iterator == null)
            {
                throw new ArgumentNullException("iterator");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            IEnumerator<TTarget> newIterator = UnsafeConvertAll(iterator, converter);
            return ToArray(newIterator);
        }

        /// <summary>
        /// Converts an object collection into a strongly typed array.
        /// </summary>
        /// <typeparam name="T">The result array type.</typeparam>
        /// <param name="collection">The source collection.</param>
        /// <returns>The stronly typed array.</returns>
        public static T[] UnsafeToArray<T>(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            IEnumerator iterator = collection.GetEnumerator();

            using (iterator as IDisposable)
            {
                return UnsafeToArray<T>(iterator);
            }
        }
        /// <summary>
        /// Converts an object collection iterator into a strongly typed array.
        /// </summary>
        /// <typeparam name="T">The result array type.</typeparam>
        /// <param name="iterator">The object collection iterator.</param>
        /// <returns>The stronly typed array.</returns>
        public static T[] UnsafeToArray<T>(IEnumerator iterator)
        {
            if (iterator == null)
            {
                throw new ArgumentNullException("iterator");
            }

            Converter<object, T> innerConverter = item => (T)item;

            return UnsafeToArray(iterator, innerConverter);
        }

        /// <summary>
        /// Converts all the items of type T in the object-based collection to a new collection of type U according to converter
        /// </summary>
        /// <typeparam name="TSource">The source collection item type.</typeparam>
        /// <typeparam name="TTarget">The target array item type.</typeparam>
        /// <param name="collection">The source object collection.</param>
        /// <param name="converter">The object type converter.</param>
        /// <returns>The converted source collection as strongly typed collection.</returns>
        public static IEnumerable<TTarget> UnsafeConvertAll<TSource, TTarget>(IEnumerable collection, Converter<TSource, TTarget> converter)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return collection.AsSequence<TSource>().Select(sourceItem => converter(sourceItem));
        }
        /// <summary>
        /// Converts all the items of type T in the object-based IEnumerator to a new collection of type U according to converter
        /// </summary>
        /// <typeparam name="TSource">The source collection item type.</typeparam>
        /// <typeparam name="TTarget">The target array item type.</typeparam>
        /// <param name="iterator">The source collection iterator.</param>
        /// <param name="converter">The object type converter.</param>
        /// <returns>The converted source collection returned by source iterator as strongly typed collection.</returns>
        public static IEnumerator<TTarget> UnsafeConvertAll<TSource, TTarget>(IEnumerator iterator, Converter<TSource, TTarget> converter)
        {
            if (iterator == null)
            {
                throw new ArgumentNullException("iterator");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            while (iterator.MoveNext())
            {
                yield return converter((TSource)iterator.Current);
            }
        }

        /// <summary>
        /// Copies the key collection of a Dictionary to an array
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">Dictionary whose key collection should be copied.</param>
        /// <returns>An array of the dictionary's keys.</returns>
        /// <remarks>
        /// <para>{keywords:dictionary,array,dictionary keys,toarray}</para>
        /// <para>{role:converter}</para>
        /// </remarks>
        public static TKey[] DictionaryKeysToArray<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            //Contract.Requires<ArgNullException>(dictionary != null, "dictionary");
            ArgChecker.ShouldNotBeNull(dictionary, "dictionary");

            return (dictionary.Keys.ToArray());
        }

        /// <summary>
        /// Copies the value collection of a Dictionary to an array.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">Dictionary whose value collection should be copied.</param>
        /// <returns>An array of the dictionary's values.</returns>
        /// <remarks>
        /// <para>{keywords:dictionary,array,dictionary values,toarray}</para>
        /// <para>{role:converter}</para>
        /// </remarks>
        public static TValue[] DictionaryValuesToArray<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            Debug.Assert(dictionary != null);

            return (dictionary.Values.ToArray());
        }
    }
}
