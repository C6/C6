// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System.Diagnostics.Contracts;
using System.Linq;

using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: Rewrite documentation based on hash code solution.
    /// <summary>
    /// Represents an editable collection maintaining a definite sequence order
    /// of its items.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface must compute the hash code and 
    /// equality exactly as prescribed in the method definitions in order to be
    /// consistent with other collection classes implementing this interface.
    /// </remarks>
    public interface ISequenced<T> : ICollection<T>, IDirectedCollectionValue<T>
    {
        // TODO: Consider how this should be implemented/documented. Maybe use a static helper class.
        /// <summary>
        /// Returns the sequenced (order-sensitive) hash code of the collection.
        /// </summary>
        /// <returns>The sequenced hash code of the collection.</returns>
        /// <remarks>
        /// The hash code is defined as <c>h(...h(h(h(x1),x2),x3),...,xn)</c>
        /// for <c>h(a,b)=CONSTANT*a+b</c> and the x's the hash codes of the
        /// items of this collection.
        /// </remarks>
        [Pure]
        int GetSequencedHashCode();


        /// <summary>
        /// Compares the items in this collection to the items in the other 
        /// collection with regards to multiplicities and sequence order.
        /// </summary>
        /// <param name="otherCollection">The collection to compare this
        /// collection to.</param>
        /// <returns><c>true</c> if the collections contain equal items in the
        /// same order; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// Enumeration of the collections must yield equal items, place for
        /// place. The comparison uses <b>this</b> collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> to determine item
        /// equality. If the two collections use different notions of item
        /// equality, there is no guarantee that this method is symmetric, i.e.
        /// the following test is undetermined:
        /// <code>
        /// // Undetermined when coll1.EqualityComparer and coll2.EqualityComparer are not equal
        /// coll1.SequencedEquals(coll2) == coll2.SequencedEquals(coll1)
        /// </code>
        /// </para>
        /// <para>This method is equivalent to
        /// <c>Enumerable.SequenceEqual(coll1, coll2, coll1.EqualityComparer)</c>,
        /// but might be more efficient.</para>
        /// </remarks>
        /// <seealso cref="GetSequencedHashCode"/>
        /// <seealso cref="Enumerable.SequenceEqual{T}(SCG.IEnumerable{T}, SCG.IEnumerable{T}, SCG.IEqualityComparer{T})"/>
        [Pure]
        bool SequencedEquals(ISequenced<T> otherCollection);
    }
}
