// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System.Diagnostics.Contracts;

using SCG = System.Collections.Generic;



namespace C6
{
    /// <summary>
    /// Represents a sequenced collection whose items are accessible by index.
    /// </summary>
    public interface IIndexed<T> : ISequenced<T>, SCG.IReadOnlyList<T>
    {
        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        /// <value>The number of items contained in the collection.</value>
        [Pure]
        new int Count { get; }


        /// <summary>
        /// Returns an <see cref="IDirectedCollectionValue{T}"/> containing 
        /// the items in the specified index range of this collection.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count">The number of items in the
        /// <see cref="IDirectedCollectionValue{T}"/>.</param>
        /// <returns>An <see cref="IDirectedCollectionValue{T}"/> containing 
        /// the items in the specified index range of this collection.
        /// </returns>
        /// <remarks>
        /// This is useful for enumerating an index range, either forwards or 
        /// backwards. Often used together with <see cref="IndexOf"/>. The
        /// forwards enumerator is equal to
        /// <c>coll.Skip(startIndex).Take(count)</c>, but potentially much
        /// faster.
        /// </remarks>
        [Pure]
        IDirectedCollectionValue<T> GetIndexRange(int startIndex, int count);


        /// <summary>
        /// Gets a value characterizing the asymptotic complexity of
        /// <see cref="SCG.IReadOnlyList{T}.this"/> proportional to collection
        /// size (worst-case or amortized as relevant).
        /// </summary>
        /// <value>A characterization of the asymptotic speed of
        /// <see cref="SCG.IReadOnlyList{T}.this"/> proportional to collection
        /// size.</value>
        [Pure]
        Speed IndexingSpeed { get; }

        
        // TODO: Move from IIndexed to IIndexedSorted? Introduce extension method for IIndexed?
        /// <summary>
        /// Searches from the beginning of the collection for the specified
        /// item and returns the zero-based index of the first occurrence
        /// within the collection. 
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns>The zero-based index of the first occurrence of item
        /// within the entire collection, if found; otherwise, the one's
        /// complement of the index at which <see cref="ICollection{T}.Add"/>
        /// would put the item.</returns>
        [Pure]
        int IndexOf(T item);


        // TODO: Two's complement?!
        // TODO: Move from IIndexed to IIndexedSorted? Introduce extension method for IIndexed?
        /// <summary>
        /// Searches from the end of the collection for the specified
        /// item and returns the zero-based index of the first occurrence
        /// within the collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns>The zero-based index of the last occurrence of item within
        /// the entire collection, if found; otherwise, the one's complement of
        /// the index at which <see cref="ICollection{T}.Add"/> would put the
        /// item.</returns>
        [Pure]
        int LastIndexOf(T item);


        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <returns>The item removed from the collection.</returns>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the removed 
        /// item and the index.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        T RemoveAt(int index);


        /// <summary>
        /// Remove all items in the specified index range.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count">The number of items to remove.</param>
        /// <remarks>
        /// If the cleared index range is non-empty, it raises the following
        /// events (in that order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionCleared"/> as non-full and 
        /// with count equal to <paramref name="count"/>.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void RemoveIndexRange(int startIndex, int count);
    }
}
