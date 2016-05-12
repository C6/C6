// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using C6.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Appends the specified element at the end of the <see cref="SCG.IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements the <see cref="SCG.IEnumerable{T}"/> contains.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The enumerable to which the item should be appended.
        /// </param>
        /// <param name="item">The item to append.</param>
        /// <returns>
        ///     An <see cref="SCG.IEnumerable{T}"/>, enumerating first the items in the existing enumerable, then the item.
        /// </returns>
        [Pure]
        public static SCG.IEnumerable<T> Append<T>(this SCG.IEnumerable<T> enumerable, T item)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            // Requires(item != null, ItemMustBeNonNull);

            // Result is non-null
            Ensures(Result<SCG.IEnumerable<T>>() != null);

            #endregion

            foreach (var t in enumerable) {
                yield return t;
            }

            yield return item;
        }

        /// <summary>
        ///     Determines whether the <see cref="SCG.IEnumerable{T}"/> is empty.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the array.
        /// </typeparam>
        /// <param name="enumerable">
        ///     The <see cref="SCG.IEnumerable{T}"/> to check for emptiness.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <see cref="SCG.IEnumerable{T}"/> is empty; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsEmpty<T>(this SCG.IEnumerable<T> enumerable)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);


            // Returns true if Count is zero, otherwise false
            Ensures(Result<bool>() != enumerable.Any());

            #endregion

            var collectionValue = enumerable as ICollectionValue<T>;
            if (collectionValue != null) {
                return collectionValue.IsEmpty;
            }

            var collection = enumerable as SCG.ICollection<T>;
            if (collection != null) {
                return collection.Count == 0;
            }

            return !enumerable.Any();
        }

        /// <summary>
        ///     Determines whether the <see cref="Array"/> is empty.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the array.
        /// </typeparam>
        /// <param name="array">
        ///     The <see cref="Array"/> to check for emptiness.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Array"/> is empty; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsEmpty<T>(this T[] array)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);


            // Returns true if Count is zero, otherwise false
            Ensures(Result<bool>() != array.Any());

            #endregion

            return array.Length == 0;
        }

        /// <summary>
        ///     Determines whether an enumerable contains an element that satisfies a specified condition. Determines whether any
        ///     element of a sequence satisfies a condition.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of <paramref name="enumerable"/>.
        /// </typeparam>
        /// <param name="enumerable">
        ///     An <see cref="SCG.IEnumerable{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// <param name="item">
        ///     The first element that satisfies the specified condition; otherwise, the default value for type
        ///     <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if an element in the enumerable satisfies the condition; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method returns the same as
        ///         <see cref="Enumerable.Any{TSource}(SCG.IEnumerable{TSource}, Func{TSource, bool})"/>, while at the same time
        ///         returning the same value as
        ///         <see cref="Enumerable.FirstOrDefault{TSource}(SCG.IEnumerable{TSource}, Func{TSource, bool})"/>, in
        ///         <paramref name="item"/>.
        ///     </para>
        ///     <para>
        ///         Both <paramref name="enumerable"/> and <paramref name="predicate"/> must be deterministic.
        ///     </para>
        /// </remarks>
        [Pure]
        public static bool Find<T>(this SCG.IEnumerable<T> enumerable, Func<T, bool> predicate, out T item)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(predicate != null, ArgumentMustBeNonNull);


            // Returns true if p(x) returned true for some item x; otherwise false
            Ensures(Result<bool>() == enumerable.Any(predicate));

            // Result item equals the first (or default) item satisfying the predicate
            Ensures(ValueAtReturn(out item).IsSameAs(enumerable.FirstOrDefault(predicate)));

            #endregion

            bool result;

            // Use Enumerable.Where() to be able to retrieve value for result
            using (var enumerator = enumerable.Where(predicate).GetEnumerator()) {
                item = (result = enumerator.MoveNext()) ? enumerator.Current : default(T);
            }

            return result;
        }

        /// <summary>
        ///     Returns the position of the first item that satisfies a specified condition, if any.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of <paramref name="collection"/>.
        /// </typeparam>
        /// <param name="collection">
        ///     An <see cref="IIndexed{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        ///     A function to test each item for a condition.
        /// </param>
        /// <returns>
        ///     The index of the first item that satisfies the condition; otherwise, -1.
        /// </returns>
        [Pure]
        public static int FindIndex<T>(this IIndexed<T> collection, Func<T, bool> predicate)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(collection != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(predicate != null, ArgumentMustBeNonNull);

            // Result is a valid index
            Ensures(Result<int>() == (collection.Any(predicate) ? collection.Count(predicate) : -1));

            #endregion

            var index = 0;

            foreach (var item in collection) {
                if (predicate(item)) {
                    return index;
                }
                index++;
            }

            return -1;
        }

        /// <summary>
        ///     Returns the position of the last item that satisfies a specified condition, if any.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of <paramref name="collection"/>.
        /// </typeparam>
        /// <param name="collection">
        ///     An <see cref="IIndexed{T}"/> to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        ///     A function to test each item for a condition.
        /// </param>
        /// <returns>
        ///     The index of the last item that satisfies the condition; otherwise, -1.
        /// </returns>
        [Pure]
        public static int FindLastIndex<T>(this IIndexed<T> collection, Func<T, bool> predicate)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(collection != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(predicate != null, ArgumentMustBeNonNull);

            // Result is a valid index
            Ensures(Result<int>() == (collection.Any(predicate) ? collection.Count - collection.Backwards().Count(predicate) - 1 : -1));

            #endregion

            var index = collection.Count - 1;

            foreach (var item in collection.Backwards()) {
                if (predicate(item)) {
                    return index;
                }
                index--;
            }

            return -1;
        }

        // TODO: Test
        /// <summary>
        ///     Determines whether all pairs of consecutive elements of a sequence satisfy a condition.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements of <paramref name="enumerable"/>.
        /// </typeparam>
        /// <param name="enumerable">
        ///     An <see cref="SCG.IEnumerable{T}"/> that contains the elements to apply the predicate to.
        /// </param>
        /// <param name="predicate">
        ///     A function to test each pair of consecutive elements for a condition.
        /// </param>
        /// <returns>
        ///     <c>true</c> if every pair of consecutive elements of the enumerable sequence passes the test in the specified
        ///     predicate, or if the sequence is empty; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool AllConsecutiveElements<T>(this SCG.IEnumerable<T> enumerable, Func<T, T, bool> predicate)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(predicate != null, ArgumentMustBeNonNull);

            #endregion

            using (var enumerator = enumerable.GetEnumerator()) {
                if (enumerator.MoveNext()) {
                    var previous = enumerator.Current;

                    while (enumerator.MoveNext()) {
                        var current = enumerator.Current;

                        if (!predicate(previous, current)) {
                            return false;
                        }

                        previous = current;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether the enumerable is sorted in non-descending order according to the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The default comparer <see cref="SCG.Comparer{T}.Default"/> cannot find an implementation of the
        ///     <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/> interface for type
        ///     <typeparamref name="T"/>.
        /// </exception>
        /// <returns>
        ///     <c>true</c> if the enumerable is sorted in non-descending order; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsSorted<T>(this SCG.IEnumerable<T> enumerable)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            #endregion

            return enumerable.IsSorted(SCG.Comparer<T>.Default);
        }

        /// <summary>
        ///     Determines whether the enumerable is sorted in non-descending order according to the specified comparer.
        /// </summary>
        /// <param name="comparer">
        ///     The <see cref="SCG.IComparer{T}"/> implementation to use when comparing items, or <c>null</c> to use the default
        ///     comparer <see cref="SCG.Comparer{T}.Default"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="comparer"/> is <c>null</c>, and the default comparer <see cref="SCG.Comparer{T}.Default"/> cannot
        ///     find an implementation of the <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/>
        ///     interface for type <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The implementation of <paramref name="comparer"/> caused an error during the sort. For example,
        ///     <paramref name="comparer"/> might not return zero when comparing an item with itself.
        /// </exception>
        /// <returns>
        ///     <c>true</c> if the enumerable is sorted in non-descending order; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsSorted<T>(this SCG.IEnumerable<T> enumerable, SCG.IComparer<T> comparer)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            #endregion

            if (comparer == null) {
                comparer = SCG.Comparer<T>.Default;
            }

            return enumerable.AllConsecutiveElements((x, y) => comparer.Compare(x, y) <= 0);
        }

        /// <summary>
        ///     Determines whether the enumerable is sorted in non-descending order according to the specified
        ///     <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">
        ///     The <see cref="Comparison{T}"/> to use when comparing elements.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     The implementation of <paramref name="comparison"/> caused an error during the sort. For example,
        ///     <paramref name="comparison"/> might not return zero when comparing an item with itself.
        /// </exception>
        /// <returns>
        ///     <c>true</c> if the enumerable is sorted in non-descending order; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsSorted<T>(this SCG.IEnumerable<T> enumerable, Comparison<T> comparison)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            // Argument must be non-null
            Requires(comparison != null, ArgumentMustBeNonNull);

            #endregion

            return enumerable.AllConsecutiveElements((x, y) => comparison(x, y) <= 0);
        }

        // TODO: Test
        /// <summary>
        ///     Shuffles the elements in the array.
        /// </summary>
        /// <param name="array">The array to shuffle.</param>
        public static void Shuffle<T>(this T[] array)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);

            #endregion

            Shuffle(array, new Random());
        }

        // TODO: Test
        /// <summary>
        ///     Shuffles the elements in the list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this SCG.IList<T> list)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(list != null, ArgumentMustBeNonNull);

            #endregion

            Shuffle(list, new Random());
        }

        // TODO: Test
        /// <summary>
        ///     Shuffles the elements in the array according to the specified random source.
        /// </summary>
        /// <param name="array">The array to shuffle.</param>
        /// <param name="random">The random source.</param>
        public static void Shuffle<T>(this T[] array, Random random)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);

            // The elements are the same
            Ensures(array.HasSameAs(OldValue(array.ToList())));

            #endregion

            array.Shuffle(0, array.Length, random);
        }

        public static void Shuffle<T>(this T[] array, int startIndex, int count, Random random = null)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);


            // The elements are the same
            Ensures(array.HasSameAs(OldValue(array.ToList())));

            // The elements before do not change order
            Ensures(array.Take(startIndex).IsSameSequenceAs(OldValue(array.Take(startIndex).ToList())));

            // The elements after do not change order
            Ensures(array.Skip(startIndex + count).IsSameSequenceAs(OldValue(array.Skip(startIndex + count).ToList())));

            #endregion

            if (random == null) {
                random = new Random(); // TODO: Use C5.Random?
            }
            var n = count;

            while (--n > 0) {
                array.Swap(startIndex + random.Next(n + 1), startIndex + n);
            }
        }

        // TODO: Test
        /// <summary>
        ///     Shuffles the items in the list according to the specified random source.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="random">The random source.</param>
        public static void Shuffle<T>(this SCG.IList<T> list, Random random)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(list != null, ArgumentMustBeNonNull);

            // List must be non-read-only
            Requires(!list.IsReadOnly, CollectionMustBeNonReadOnly);

            // The elements are the same
            Ensures(list.HasSameAs(OldValue(list.ToList())));

            #endregion

            if (random == null) {
                random = new Random(); // TODO: Use C5.Random?
            }

            var n = list.Count;
            while (--n > 0) {
                list.Swap(random.Next(n + 1), n);
            }
        }

        // TODO: Test?
        /// <summary>
        ///     Swaps the elements at the specified indices in an array.
        /// </summary>
        /// <param name="array">
        ///     The array in which to swap the elements.
        /// </param>
        /// <param name="i">
        ///     The index of the first element.
        /// </param>
        /// <param name="j">
        ///     The index of the second element.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the elements in the array.
        /// </typeparam>
        public static void Swap<T>(this T[] array, int i, int j)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);

            // Argument must be within bounds
            Requires(0 <= i, ArgumentMustBeWithinBounds);
            Requires(i < array.Length, ArgumentMustBeWithinBounds);

            // Argument must be within bounds
            Requires(0 <= j, ArgumentMustBeWithinBounds);
            Requires(j < array.Length, ArgumentMustBeWithinBounds);


            // The values are swapped
            Ensures(Equals(array[i], OldValue(array[j])));
            Ensures(Equals(array[j], OldValue(array[i])));

            #endregion

            if (i != j) {
                var element = array[i];
                array[i] = array[j];
                array[j] = element;
            }
        }

        // TODO: Test?
        /// <summary>
        ///     Swaps the elements at the specified indices in a list.
        /// </summary>
        /// <param name="list">
        ///     The list in which to swap the elements.
        /// </param>
        /// <param name="i">
        ///     The index of the first element.
        /// </param>
        /// <param name="j">
        ///     The index of the second element.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the elements in the list.
        /// </typeparam>
        public static void Swap<T>(this SCG.IList<T> list, int i, int j)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(list != null, ArgumentMustBeNonNull);

            // List must be non-read-only
            Requires(!list.IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be within bounds
            Requires(0 <= i, ArgumentMustBeWithinBounds);
            Requires(i < list.Count, ArgumentMustBeWithinBounds);

            // Argument must be within bounds
            Requires(0 <= j, ArgumentMustBeWithinBounds);
            Requires(j < list.Count, ArgumentMustBeWithinBounds);


            // The values are swapped
            Ensures(Equals(list[i], OldValue(list[j])));
            Ensures(Equals(list[j], OldValue(list[i])));

            #endregion

            if (i != j) {
                var element = list[i];
                list[i] = list[j];
                list[j] = element;
            }
        }
    }
}