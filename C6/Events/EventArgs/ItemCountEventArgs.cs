// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace C6
{
    /// <summary>
    /// Provides data for the <see cref="ICollectionValue{T}.ItemsAdded"/>
    /// event and the <see cref="ICollectionValue{T}.ItemsRemoved"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    [DebuggerDisplay("(ItemCountEventArgs {Count} '{Item}')")] // TODO: format appropriately
    public class ItemCountEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the item added or removed from the collection.
        /// </summary>
        /// <value>The item added or removed from the collection.</value>
        public T Item { get; }


        // TODO: Validate the comments in remarks with actual implementation
        /// <summary>
        /// Gets the multiplicity with which the insertion or deletion occurred.
        /// </summary>
        /// <value>The number of times the item was inserted or removed.</value>
        /// <remarks>The multiplicity is one when only a single copy of an item
        /// was added or removed; and it may be greater than one when
        /// manipulating collections that have bag semantics, i.e.
        /// <see cref="IExtensible{T}.AllowsDuplicates"/> is <c>true</c>, and
        /// for which <see cref="IExtensible{T}.DuplicatesByCounting"/> is 
        /// <c>true</c>.</remarks>
        public int Count { get; }


        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // Item is non-null
            // Contract.Invariant(Item != null);

            // TODO: Contract size more precisely when actually used, i.e. bound the size upwards
            // Count is positive
            Contract.Invariant(Count > 0);
        }


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ItemCountEventArgs{T}"/> class for an insertion/deletion
        /// operation using an item and its multiplicity.
        /// </summary>
        /// <param name="item">The item added or removed from the collection.</param>
        /// <param name="count">The multiplicity with which the insertion or
        /// deletion occurred.</param>
        public ItemCountEventArgs(T item, int count)
        {
            // Argument must be non-null
            // Contract.Requires(item != null); // TODO: Use <ArgumentNullException>?

            // Argument must be positive
            Contract.Requires(count > 0);


            Item = item;
            Count = count;

            
            // Contract.Assume(Item != null); // Static checker shortcoming
            Contract.Assume(Count > 0); // Static checker shortcoming
        }
    }
}