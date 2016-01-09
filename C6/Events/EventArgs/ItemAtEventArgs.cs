// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace C6
{
    /// <summary>
    /// Provides data for the <see cref="ICollectionValue{T}.ItemInserted"/>
    /// event and the <see cref="ICollectionValue{T}.ItemRemovedAt"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    [DebuggerDisplay("(ItemAtEventArgs {Index} '{Item}')")] // TODO: format appropriately
    public class ItemAtEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the item inserted or removed from the collection.
        /// </summary>
        /// <value>The item inserted or removed from the collection.</value>
        public T Item { get; }

        
        /// <summary>
        /// Gets the index the item was inserted at or removed from in the
        /// collection.
        /// </summary>
        /// <value>The index at which the item was inserted or removed.</value>
        public int Index { get; }


        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // Item is non-null
            Contract.Invariant(Item != null);

            // TODO: Contract index bounds more precisely when actually used
            // Index is non-negative
            Contract.Invariant(Index >= 0);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ItemAtEventArgs{T}"/>
        /// class for an insertion/deletion operation with an item and its
        /// index.
        /// </summary>
        /// <param name="item">The item inserted or removed from the collection.</param>
        /// <param name="index">The index at which the item was inserted or removed.</param>
        public ItemAtEventArgs(T item, int index)
        {
            // Argument must be non-null
            Contract.Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Argument must be non-negative
            Contract.Requires(index >= 0); // TODO: Use <ArgumentOutOfRangeException>?


            Item = item;
            Index = index;

            
            Contract.Assume(Item != null); // Static checker shortcoming
            Contract.Assume(Index >= 0); // Static checker shortcoming
        }
    }
}