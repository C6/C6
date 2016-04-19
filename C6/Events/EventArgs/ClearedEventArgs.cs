// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;


namespace C6
{
    /// <summary>
    ///     Provides data for the <see cref="IListenable{T}.CollectionCleared"/> event.
    /// </summary>
    [Serializable]
    public class ClearedEventArgs : EventArgs
    {
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            // ReSharper disable InvocationIsSkipped

            // Count is positive
            Invariant(Count > 0);

            // Start is only set, if a list view or index range was cleared
            Invariant(!Start.HasValue || !Full);

            // ReSharper restore InvocationIsSkipped
        }

        // TODO: Default arguments are not CLS compliant!
        // TODO: Look at FDG 8.8
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClearedEventArgs"/> class for an operation that cleared a collection
        ///     or a list view/index range using an item count and an optional start position.
        /// </summary>
        /// <param name="full">
        ///     <c>true</c> if the operation cleared a collection; <c>false</c> if the operation cleared a list view or an index
        ///     range (even if the view or range is the entire collection).
        /// </param>
        /// <param name="count">
        ///     The number of items removed by the operation.
        /// </param>
        /// <param name="start">
        ///     The position of the first item in the range deleted, when known.
        /// </param>
        public ClearedEventArgs(bool full, int count, int? start = null)
        {
            // Argument must be positive
            Requires(0 < count, ArgumentMustBePositive);

            // Start is only set, if a list view or index range was cleared
            Requires(!start.HasValue || !full); // TODO: Add user message


            Full = full;
            Count = count;
            Start = start;


            Assume(Count > 0); // Static checker shortcoming
            Assume(!Start.HasValue || !Full); // Static checker shortcoming
        }

        /// <summary>
        ///     Gets the number of items cleared by the operation.
        /// </summary>
        /// <value>
        ///     The number of items cleared by the operation.
        /// </value>
        [Pure]
        public int Count { get; }

        // TODO: Consider replacing Full with an enum instead of bool.
        /// <summary>
        ///     Gets a value indicating whether a collection was cleared, or whether a list view or an index range was cleared.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the operation cleared a collection; <c>false</c> if the operation cleared a list view or an index
        ///     range (even if the view or range is the entire collection).
        /// </value>
        [Pure]
        public bool Full { get; }

        /// <summary>
        ///     Gets the position (when known) of the first item if a list view or an index range was cleared.
        /// </summary>
        /// <value>
        ///     The index of the first item cleared, when known; otherwise, <c>null</c>.
        /// </value>
        [Pure]
        public int? Start { get; }

        public override string ToString() => $"{Count} {(Count == 1 ? "item" : "items")} {(Start.HasValue ? $" starting at index {Start.Value}" : "")} ({(Full ? "full" : "range")})"; // $"(ClearedEventArgs {Count} {Full})"
    }
}