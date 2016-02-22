// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

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
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?
            // Argument must be non-null
            // Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Result is non-null
            Ensures(Result<SCG.IEnumerable<T>>() != null);


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
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?


            // Returns true if Count is zero, otherwise false
            Ensures(Result<bool>() != enumerable.Any());


            return (enumerable as ICollectionValue<T>)?.IsEmpty ?? !enumerable.Any();
        }


        [Pure]
        public static bool Find<T>(this SCG.IEnumerable<T> enumerable, Func<T, bool> predicate, out T item)
        {
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            Requires(predicate != null); // TODO: Use <ArgumentNullException>?


            // Returns true if p(x) returned true for some item x; otherwise false
            Ensures(Result<bool>() == enumerable.Any(predicate));

            // Result item equals the first (or default) item satisfying the predicate
            Ensures(ValueAtReturn(out item).Equals(enumerable.FirstOrDefault(predicate)));


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
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            Requires(predicate != null); // TODO: Use <ArgumentNullException>?


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
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?

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
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?

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
            // Argument must be non-null
            Requires(enumerable != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-null
            Requires(comparison != null); // TODO: Use <ArgumentNullException>?

            return enumerable.AllConsecutiveElements((x, y) => comparison(x, y) <= 0);
        }
    }
}