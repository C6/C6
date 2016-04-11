// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Diagnostics.Contracts;

using C6.Contracts;

using static System.Diagnostics.Contracts.Contract;

using SCG = System.Collections.Generic;


namespace C6
{
    public static class UnsequencedEqualityComparer
    {
        // TODO: the three odd factors should be random, but this causes problems with serialization/deserialization!
        private const uint H1 = 1529784657;
        private const uint H2 = 2912831877;
        private const uint H3 = 1118771817;

        /// <summary>
        /// Returns the unsequenced (order-insensitive) hash code of the collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="collection">The collection whose hash code should be computed.</param>
        /// <param name="equalityComparer">The <see cref="SCG.IEqualityComparer{T}"/>
        /// used to compute the hash code for each item, or <c>null</c> to use
        /// the default equality comparer <see cref="SCG.EqualityComparer{T}.Default"/>.
        /// The collection's own equality comparer is completely disregarded.</param>
        /// <returns>The unsequenced hash code of the collection.</returns>
        [Pure]
        public static int GetUnsequencedHashCode<T>(this SCG.ICollection<T> collection, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            if (collection == null) {
                return 0; // TODO: Better default value? H1/H2/H3?
            }

            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            var hashCode = 0; // TODO: Better intial value?

            // Does not use Linq.Sum() as it throws an exception if it overflows
            foreach (var item in collection) {
                var h = (uint) equalityComparer.GetHashCode(item);

                // We need at least three products, as two is too few
                hashCode += (int) ((h * H1 + 1) ^ (h * H2) ^ (h * H3 + 2));
            }

            return hashCode;
        }

        /// <summary>
        /// Compares the items in the two collections with each other 
        /// with regards to multiplicities, but without regards to sequence order.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collections.</typeparam>
        /// <param name="first">The collection to compare the second
        /// collection to.</param>
        /// <param name="second">The collection to compare the first
        /// collection to.</param>
        /// <param name="equalityComparer">The <see cref="SCG.IEqualityComparer{T}"/>
        /// used to comparer the items in the collections, or <c>null</c> to use
        /// the default equality comparer <see cref="SCG.EqualityComparer{T}.Default"/>.
        /// The collections' own equality comparers are completely disregarded.
        /// </param>
        /// <returns><c>true</c> if the collections contain equal items;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// For each item in this collection there must be one equal
        /// to it in the other collection with the same multiplicity, and vice
        /// versa.
        /// </remarks>
        [Pure]
        public static bool UnsequencedEquals<T>(this ICollection<T> first, ICollection<T> second, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            Ensures(Result<bool>() == first.UnsequenceEqual(second, equalityComparer));

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

            // To avoid an O(n^2) algorithm, we make an auxiliary dictionary to hold the count of items
            var dictionary = new SCG.Dictionary<T, int>(equalityComparer); // TODO: Use C6 version (HashBag<T>)

            foreach (var item in second) {
                int count;
                if (dictionary.TryGetValue(item, out count)) {
                    // Dictionary already contained item, so we increment count with one
                    dictionary[item] = count + 1;
                }
                else {
                    dictionary.Add(item, 1);
                }
            }

            foreach (var item in first) {
                int count;
                if (dictionary.TryGetValue(item, out count) && count > 0) {
                    dictionary[item] = count - 1;
                }
                else {
                    return false;
                }
            }

            return true;
        }
    }
}