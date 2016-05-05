// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;


namespace C6
{
    /// <summary>
    ///     Defines different types of events for collections.
    /// </summary>
    /// <remarks>
    ///     The enum type is used to report which of a collection or a dictionary's event handlers are currently active, and
    ///     which events can be listened to. The values can be combined, like flags, using the bitwise operator "or" (|) to
    ///     form other values, as seen with <see cref="Basic"/> and <see cref="All"/>.
    /// </remarks>
    /// <seealso cref="ICollectionValue{T}"/>
    [Flags]
    public enum EventTypes
    {
        /// <summary>
        ///     Corresponds to no events.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Corresponds to the <see cref="IListenable{T}.CollectionChanged"/> event.
        /// </summary>
        Changed = 1 << 0,

        /// <summary>
        ///     Corresponds to the <see cref="IListenable{T}.CollectionCleared"/> event.
        /// </summary>
        Cleared = 1 << 1,

        /// <summary>
        ///     Corresponds to the <see cref="IListenable{T}.ItemsAdded"/> event.
        /// </summary>
        Added = 1 << 2,

        /// <summary>
        ///     Corresponds to the <see cref="IListenable{T}.ItemsRemoved"/> event.
        /// </summary>
        Removed = 1 << 3,

        /// <summary>
        ///     Is the combined value of <see cref="Changed"/>, <see cref="Cleared"/>, <see cref="Added"/>, and
        ///     <see cref="Removed"/>.
        /// </summary>
        Basic = Changed | Cleared | Added | Removed,

        /// <summary>
        ///     Corresponds to the <see cref="IListenable{T}.ItemInserted"/> event.
        /// </summary>
        Inserted = 1 << 4,

        /// <summary>
        ///     Corresponds to the <see cref="IListenable{T}.ItemRemovedAt"/> event.
        /// </summary>
        RemovedAt = 1 << 5,

        /// <summary>
        ///     Is the combined value of <see cref="Inserted"/> and <see cref="RemovedAt"/>.
        /// </summary>
        Indexed = Inserted | RemovedAt,

        /// <summary>
        ///     Is the combined value of <see cref="Basic"/> and <see cref="Indexed"/>.
        /// </summary>
        All = Basic | Indexed
    }
}