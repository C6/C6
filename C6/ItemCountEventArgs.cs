// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;


namespace C6
{
    /// <summary>
    ///     Provides data for the <see cref="IListenable{T}.ItemsAdded"/> event and the
    ///     <see cref="IListenable{T}.ItemsRemoved"/> event.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    [Serializable]
    public class ItemCountEventArgs<T> : EventArgs
    {
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            // Item is non-null
            // Invariant(Item != null);

            // Count is positive
            Invariant(Count > 0); // TODO: Contract size more precisely when actually used, i.e. bound the size upwards

            // ReSharper enable InvocationIsSkipped
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemCountEventArgs{T}"/> class for an insertion/deletion operation
        ///     using an item and its multiplicity.
        /// </summary>
        /// <param name="item">
        ///     The item added or removed from the collection.
        /// </param>
        /// <param name="count">
        ///     The multiplicity with which the insertion or deletion occurred.
        /// </param>
        public ItemCountEventArgs(T item, int count)
        {
            // Argument must be non-null
            // Requires(item != null, ItemMustBeNonNull);

            // Argument must be positive
            Requires(count > 0, ArgumentMustBePositive);


            Item = item;
            Count = count;

            // Assume(Item != null); // Static checker shortcoming
            Assume(Count > 0); // Static checker shortcoming
        }

        // TODO: Validate the comments in remarks with actual implementation
        /// <summary>
        ///     Gets the multiplicity with which the insertion or deletion occurred.
        /// </summary>
        /// <value>
        ///     The number of times the item was inserted or removed.
        /// </value>
        /// <remarks>
        ///     The multiplicity is one when only a single copy of an item was added or removed; and it may be greater than one
        ///     when manipulating collections that have bag semantics, i.e. <see cref="IExtensible{T}.AllowsDuplicates"/> is
        ///     <c>true</c>, and for which <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>.
        /// </remarks>
        public int Count { get; }

        /// <summary>
        ///     Gets the item added or removed from the collection.
        /// </summary>
        /// <value>
        ///     The item added or removed from the collection.
        /// </value>
        public T Item { get; }

        public override string ToString() => $"'{Item}' {Count} {(Count == 1 ? "time" : "times")}"; // $"(ItemCountEventArgs {Count} '{Item}')"
    }
}