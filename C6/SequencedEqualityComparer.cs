// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System.Diagnostics.Contracts;
using System.Linq;

using static System.Diagnostics.Contracts.Contract;

using SCG = System.Collections.Generic;


namespace C6
{
    public static class SequencedEqualityComparer
    {
        // TODO: Factor should be random, but this causes problems with serialization/deserialization!
        public const int HashFactor = 31;

        /// <summary>
        ///     Returns the sequenced (order-sensitive) hash code of the sequence.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the sequence.
        /// </typeparam>
        /// <param name="items">
        ///     The sequence whose hash code should be computed.
        /// </param>
        /// <param name="equalityComparer">
        ///     The <see cref="SCG.IEqualityComparer{T}"/> used to compute the hash code for each item, or <c>null</c> to use the
        ///     default equality comparer <see cref="SCG.EqualityComparer{T}.Default"/>.
        /// </param>
        /// <returns>
        ///     The sequenced hash code of the sequence.
        /// </returns>
        [Pure]
        public static int GetSequencedHashCode<T>(this ISequenced<T> items, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            if (items == null) {
                return 0; // TODO: Better default value?
            }

            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            var hashCode = 0; // TODO: Better intial value?

            foreach (var item in items) {
                hashCode = hashCode * HashFactor + equalityComparer.GetHashCode(item);
            }

            return hashCode;
        }

        /// <summary>
        ///     Compares the items in the two sequences with each other with regards to multiplicities and order.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the items in the sequence.
        /// </typeparam>
        /// <param name="first">
        ///     The sequence to compare the second sequence to.
        /// </param>
        /// <param name="second">
        ///     The sequence to compare the first sequence to.
        /// </param>
        /// <param name="equalityComparer">
        ///     The <see cref="SCG.IEqualityComparer{T}"/> used to comparer the items in the sequences, or <c>null</c> to use the
        ///     default equality comparer <see cref="SCG.EqualityComparer{T}.Default"/>. The sequences' own equality comparers are
        ///     completely disregarded.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the sequences contain equal items; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     The items at the same position in the two sequences are compared pairwise.
        /// </remarks>
        [Pure]
        public static bool SequencedEquals<T>(this ISequenced<T> first, ISequenced<T> second, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            Ensures(Result<bool>() == (ReferenceEquals(first, second) || (first != null && second != null && first.SequenceEqual(second, equalityComparer))));

            // Equal if reference equal - this is true for two nulls as well
            if (ReferenceEquals(first, second)) {
                return true;
            }

            if (first == null || second == null) {
                return false;
            }

            if (first.Count != second.Count) {
                return false;
            }

            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator()) {
                // Enumerators are equally long
                while (e1.MoveNext() & e2.MoveNext()) {
                    if (!equalityComparer.Equals(e1.Current, e2.Current)) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}