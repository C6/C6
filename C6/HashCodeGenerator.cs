// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Diagnostics.Contracts;

using SCG = System.Collections.Generic;


namespace C6
{
    public static class HashCodeGenerator
    {
        private const uint H1 = 1529784657;
        private const uint H2 = 2912831877;
        private const uint H3 = 1118771817;

        /// <summary>
        /// Returns the unsequenced (order-insensitive) hash code of the
        /// enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
        /// <param name="items">The enumerable whose hash code should be computed.</param>
        /// <param name="equalityComparer">The <see cref="SCG.IEqualityComparer{T}"/>
        /// used to compute the hash code for each item.</param>
        /// <returns>The unsequenced hash code of the enumerable.</returns>
        [Pure]
        public static int GetUnsequencedHashCode<T>(this SCG.IEnumerable<T> items, SCG.IEqualityComparer<T> equalityComparer = null)
        {
            if (equalityComparer == null) {
                equalityComparer = SCG.EqualityComparer<T>.Default;
            }

            var h = 0;

            // Does not use Linq.Sum() as it throws an exception if it overflows
            foreach (var item in items) {
                var hashCode = (uint) equalityComparer.GetHashCode(item);

                // TODO: the three odd factors should be random, but this causes problems with serialization/deserialization!
                // We need at least three products, as two is too few
                h += (int) ((hashCode*H1 + 1) ^ (hashCode*H2) ^ (hashCode*H3 + 2));
            }

            return h;
        }
    }
}