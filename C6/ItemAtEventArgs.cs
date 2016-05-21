// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    /// <summary>
    ///     Provides data for the <see cref="IListenable{T}.ItemInserted"/> event and the
    ///     <see cref="IListenable{T}.ItemRemovedAt"/> event.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    [Serializable]
    public class ItemAtEventArgs<T> : EventArgs
    {
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            // Item is non-null
            // Invariant(Item != null);

            // TODO: Contract index bounds more precisely when actually used
            // Index is non-negative
            Invariant(Index >= 0);

            // ReSharper restore InvocationIsSkipped
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemAtEventArgs{T}"/> class for an insertion/deletion operation with
        ///     an item and its index.
        /// </summary>
        /// <param name="item">
        ///     The item inserted or removed from the collection.
        /// </param>
        /// <param name="index">
        ///     The index at which the item was inserted or removed.
        /// </param>
        public ItemAtEventArgs(T item, int index)
        {
            // Argument must be non-null
            // Requires(item != null, ItemMustBeNonNull);

            // Argument must be non-negative
            Requires(index >= 0, ArgumentMustBeNonNegative);


            Item = item;
            Index = index;

            // Assume(Item != null); // Static checker shortcoming
            Assume(Index >= 0); // Static checker shortcoming
        }

        /// <summary>
        ///     Gets the item inserted or removed from the collection.
        /// </summary>
        /// <value>
        ///     The item inserted or removed from the collection.
        /// </value>
        public T Item { get; }

        /// <summary>
        ///     Gets the index the item was inserted at or removed from in the collection.
        /// </summary>
        /// <value>
        ///     The index at which the item was inserted or removed.
        /// </value>
        public int Index { get; }

        public override string ToString() => $"'{Item}' at index {Index}"; // $"(ItemAtEventArgs {Index} '{Item}')"
    }
}