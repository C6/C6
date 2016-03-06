// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    public static class Extensions
    {
        /// <summary>
        /// Appends the specified element at the end of the
        /// <see cref="SCG.IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements the
        /// <see cref="SCG.IEnumerable{T}"/> contains.</typeparam>
        /// <param name="enumerable">The enumerable to which the item should be
        /// appended.</param>
        /// <param name="item">The item to append.</param>
        /// <returns>An <see cref="SCG.IEnumerable{T}"/>, enumerating first the
        /// items in the existing enumerable, then the item.</returns>
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
        /// Gets a value indicating whether the
        /// <see cref="SCG.IEnumerable{T}"/> is empty.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="SCG.IEnumerable{T}"/> is empty;
        /// otherwise, <c>false</c>.
        /// </value>
        [Pure]
        public static bool IsEmpty<T>(this SCG.IEnumerable<T> enumerable)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);


            // Returns true if Count is zero, otherwise false
            Ensures(Result<bool>() != enumerable.Any());

            #endregion

            return (enumerable as ICollectionValue<T>)?.IsEmpty ?? !enumerable.Any();
        }


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
            Ensures(ValueAtReturn(out item).Equals(enumerable.FirstOrDefault(predicate)));

            #endregion

            bool result;

            using (var enumerator = enumerable.Where(predicate).GetEnumerator()) {
                // ReSharper disable once AssignmentInConditionalExpression
                item = (result = enumerator.MoveNext()) ? enumerator.Current : default(T);
            }

            return result;
        }


        // TODO: Implement and document
        [Pure]
        public static int FindIndex<T>(this IIndexed<T> collection, Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }


        // TODO: Implement and document
        [Pure]
        public static int FindLastIndex<T>(this IIndexed<T> collection, Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        // TODO: Test
        /// <summary>
        /// Determines whether all pairs of consecutive elements of a sequence
        /// satisfy a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements of
        /// <paramref name="enumerable"/>.</typeparam>
        /// <param name="enumerable">An <see cref="SCG.IEnumerable{T}"/> that
        /// contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each pair of consecutive
        /// elements for a condition.</param>
        /// <returns><c>true</c> if every pair of consecutive elements of the
        /// enumerable sequence passes the test in the specified predicate, or
        /// if the sequence is empty; otherwise, <c>false</c>.</returns>
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
        /// Determines whether the enumerable is sorted in non-descending order
        /// according to the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">The default comparer
        /// <see cref="SCG.Comparer{T}.Default"/> cannot find an implementation
        /// of the <see cref="IComparable{T}"/> generic interface or the
        /// <see cref="IComparable"/> interface for type
        /// <typeparamref name="T"/>.</exception>
        /// <returns><c>true</c> if the enumerable is sorted in non-descending
        /// order; otherwise, <c>false</c>.</returns>
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
        /// Determines whether the enumerable is sorted in non-descending order
        /// according to the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="SCG.IComparer{T}"/>
        /// implementation to use when comparing items, or <c>null</c> to use
        /// the default comparer <see cref="SCG.Comparer{T}.Default"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is <c>null</c>, and the default
        /// comparer <see cref="SCG.Comparer{T}.Default"/> cannot find an
        /// implementation of the <see cref="IComparable{T}"/> generic
        /// interface or the <see cref="IComparable"/> interface for type
        /// <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentException">The implementation of
        /// <paramref name="comparer"/> caused an error during the sort. For
        /// example, <paramref name="comparer"/> might not return 0 when
        /// comparing an item with itself.</exception>
        /// <returns><c>true</c> if the enumerable is sorted in non-descending
        /// order; otherwise, <c>false</c>.</returns>
        [Pure]
        public static bool IsSorted<T>(this SCG.IEnumerable<T> enumerable, SCG.IComparer<T> comparer)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(enumerable != null, ArgumentMustBeNonNull);

            #endregion

            comparer = comparer ?? SCG.Comparer<T>.Default;

            return enumerable.AllConsecutiveElements((x, y) => comparer.Compare(x, y) <= 0);
        }

        /// <summary>
        /// Determines whether the enumerable is sorted in non-descending order
        /// according to the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use
        /// when comparing elements.</param>
        /// <exception cref="ArgumentException">The implementation of 
        /// <paramref name="comparison"/> caused an error during the sort. For
        /// example, <paramref name="comparison"/> might not return 0 when
        /// comparing an item with itself.</exception>
        /// <returns><c>true</c> if the enumerable is sorted in non-descending
        /// order; otherwise, <c>false</c>.</returns>
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
        /// Shuffles the elements in the array.
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
        // TODO: IList extensions?
        /// <summary>
        /// Shuffles the elements in the list.
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
        /// Shuffles the items in the array according to the specified random
        /// source.
        /// </summary>
        /// <param name="array">The array to shuffle.</param>
        /// <param name="random">The random source.</param>
        public static void Shuffle<T>(this T[] array, Random random)
        {
            #region Code Contracts

            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);

            #endregion

            random = random ?? new Random(); // TODO: Use C5.Random?
            var n = array.Length;

            while (--n > 0) {
                array.Swap(random.Next(n + 1), n);
            }
        }

        // TODO: Test
        // TODO: IList extensions?
        /// <summary>
        /// Shuffles the items in the list according to the specified random
        /// source.
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

            #endregion

            random = random ?? new Random(); // TODO: Use C5.Random?
            var n = list.Count;

            while (--n > 0) {
                list.Swap(random.Next(n + 1), n);
            }
        }

        // TODO: Test?
        /// <summary>
        /// Swaps the elements at the specified indices in an array.
        /// </summary>
        /// <param name="array">The array in which to swap the elements.</param>
        /// <param name="i">The index of the first element.</param>
        /// <param name="j">The index of the second element.</param>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
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
        // TODO: IList extensions?
        /// <summary>
        /// Swaps the elements at the specified indices in a list.
        /// </summary>
        /// <param name="list">The list in which to swap the elements.</param>
        /// <param name="i">The index of the first element.</param>
        /// <param name="j">The index of the second element.</param>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
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