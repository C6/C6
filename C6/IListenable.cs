// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;
using static C6.EventTypes;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6
{
    /// <summary>
    ///     Represents a generic, listenable collection value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    [ContractClass(typeof(IListenableContract<>))]
    public interface IListenable<T> : ICollectionValue<T>
    {
        /// <summary>
        ///     Gets a bit flag indicating the collection's currently subscribed events.
        /// </summary>
        /// <value>
        ///     The bit flag indicating the collection's currently subscribed events.
        /// </value>
        /// <seealso cref="ListenableEvents"/>
        [Pure]
        EventTypes ActiveEvents { get; }

        /// <summary>
        ///     Gets a bit flag indicating the collection's subscribable events.
        /// </summary>
        /// <value>
        ///     The bit flag indicating the collection's subscribable events.
        /// </value>
        /// <seealso cref="ActiveEvents"/>
        [Pure]
        EventTypes ListenableEvents { get; }

        /// <summary>
        ///     Occurs when the collection has changed.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The event is raised after an operation on the collection has changed its contents and the collection in an
        ///         internally consistent state. Any operation that changes the collection must raise
        ///         <see cref="CollectionChanged"/> as its last event.
        ///     </para>
        ///     <para>
        ///         Normally, a multi-operation like <see cref="IExtensible{T}.AddRange"/> will only raise one
        ///         <see cref="CollectionChanged"/> event.
        ///     </para>
        /// </remarks>
        /// <seealso cref="EventTypes.Changed"/>
        /// <seealso cref="ICollection{T}.Add"/>
        /// <seealso cref="ICollection{T}.Clear"/>
        /// <seealso cref="ICollection{T}.FindOrAdd"/>
        /// <seealso cref="ICollection{T}.Remove(T)"/>
        /// <seealso cref="ICollection{T}.Remove(T, out T)"/>
        /// <seealso cref="ICollection{T}.RemoveRange"/>
        /// <seealso cref="ICollection{T}.RemoveDuplicates"/>
        /// <seealso cref="ICollection{T}.RetainRange"/>
        /// <seealso cref="ICollection{T}.Update(T)"/>
        /// <seealso cref="ICollection{T}.Update(T, out T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T, out T)"/>
        /// <seealso cref="IExtensible{T}.Add"/>
        /// <seealso cref="IExtensible{T}.AddRange"/>
        /// <seealso cref="IIndexed{T}.RemoveAt"/>
        /// <seealso cref="IIndexed{T}.RemoveIndexRange"/>
        /// <seealso cref="IList{T}.this "/>
        /// <seealso cref="IList{T}.Clear"/>
        /// <seealso cref="IList{T}.Insert(int, T)"/>
        /// <seealso cref="IList{T}.InsertRange"/>
        /// <seealso cref="IList{T}.InsertFirst"/>
        /// <seealso cref="IList{T}.InsertLast"/>
        /// <seealso cref="IList{T}.Remove(T)"/>
        /// <seealso cref="IList{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.RemoveFirst"/>
        /// <seealso cref="IList{T}.RemoveLast"/>
        /// <seealso cref="IList{T}.Reverse"/>
        /// <seealso cref="IList{T}.Shuffle()"/>
        /// <seealso cref="IList{T}.Shuffle(Random)"/>
        /// <seealso cref="IList{T}.Sort()"/>
        /// <seealso cref="IList{T}.Sort(SCG.IComparer{T})"/>
        /// <seealso cref="IList{T}.Sort(Comparison{T})"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.this "/>
        /// <seealso cref="IPriorityQueue{T}.Remove"/>
        /// <seealso cref="IPriorityQueue{T}.Replace"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Pop"/>
        /// <seealso cref="IStack{T}.Push"/>
        event EventHandler CollectionChanged;

        /// <summary>
        ///     Occurs when (part of) the collection has been cleared.
        /// </summary>
        /// <remarks>
        ///     The event is raised after the collection (or a part of it) is cleared, when the collection in an internally
        ///     consistent state, but before the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.Cleared"/>
        /// <seealso cref="ICollection{T}.Clear"/>
        /// <seealso cref="IIndexed{T}.RemoveIndexRange"/>
        /// <seealso cref="IList{T}.Clear"/>
        event EventHandler<ClearedEventArgs> CollectionCleared;

        /// <summary>
        ///     Occurs when an item was inserted at a specific position in the collection.
        /// </summary>
        /// <remarks>
        ///     The event is raised after an item has been inserted into the collection, when the collection in an internally
        ///     consistent state, but before the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.Inserted"/>
        /// <seealso cref="IList{T}.this "/>
        /// <seealso cref="IList{T}.Insert(int, T)"/>
        /// <seealso cref="IList{T}.InsertRange"/>
        /// <seealso cref="IList{T}.InsertFirst"/>
        /// <seealso cref="IList{T}.InsertLast"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Push"/>
        event EventHandler<ItemAtEventArgs<T>> ItemInserted;

        /// <summary>
        ///     Occurs when an item was removed from a specific position in the collection.
        /// </summary>
        /// <remarks>
        ///     The event is raised after an item has been removed from the collection, when the collection in an internally
        ///     consistent state, but before the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.RemovedAt"/>
        /// <seealso cref="IIndexed{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.this "/>
        /// <seealso cref="IList{T}.Remove(T)"/>
        /// <seealso cref="IList{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.RemoveFirst"/>
        /// <seealso cref="IList{T}.RemoveLast"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        /// <seealso cref="IStack{T}.Pop"/>
        event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;

        /// <summary>
        ///     Occurs when an item was added to the collection.
        /// </summary>
        /// <remarks>
        ///     The event is raised after an item has been added to the collection, when the collection in an internally consistent
        ///     state, but before the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.Added"/>
        /// <seealso cref="ICollection{T}.Add"/>
        /// <seealso cref="ICollection{T}.FindOrAdd"/>
        /// <seealso cref="ICollection{T}.Update(T)"/>
        /// <seealso cref="ICollection{T}.Update(T, out T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T, out T)"/>
        /// <seealso cref="IExtensible{T}.Add"/>
        /// <seealso cref="IExtensible{T}.AddRange"/>
        /// <seealso cref="IList{T}.this "/>
        /// <seealso cref="IList{T}.Insert(int, T)"/>
        /// <seealso cref="IList{T}.InsertRange"/>
        /// <seealso cref="IList{T}.InsertFirst"/>
        /// <seealso cref="IList{T}.InsertLast"/>
        /// <seealso cref="IPriorityQueue{T}.Replace"/>
        /// <seealso cref="IPriorityQueue{T}.this[IPriorityQueueHandle{T}]"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Push"/>
        event EventHandler<ItemCountEventArgs<T>> ItemsAdded;

        /// <summary>
        ///     Occurs when an item was removed from the collection.
        /// </summary>
        /// <remarks>
        ///     The event is raised after an item has been removed from the collection, when the collection in an internally
        ///     consistent state, but before the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.Removed"/>
        /// <seealso cref="ICollection{T}.Remove(T)"/>
        /// <seealso cref="ICollection{T}.Remove(T, out T)"/>
        /// <seealso cref="ICollection{T}.RemoveRange"/>
        /// <seealso cref="ICollection{T}.RemoveDuplicates"/>
        /// <seealso cref="ICollection{T}.RetainRange"/>
        /// <seealso cref="ICollection{T}.Update(T)"/>
        /// <seealso cref="ICollection{T}.Update(T, out T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T, out T)"/>
        /// <seealso cref="IIndexed{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.this "/>
        /// <seealso cref="IList{T}.Remove(T)"/>
        /// <seealso cref="IList{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.RemoveFirst"/>
        /// <seealso cref="IList{T}.RemoveLast"/>
        /// <seealso cref="IPriorityQueue{T}.Remove"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.Replace"/>
        /// <seealso cref="IPriorityQueue{T}.this "/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        /// <seealso cref="IStack{T}.Pop"/>
        event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;
    }


    // TODO: Add contracts on the events to ensure they are thrown http://stackoverflow.com/questions/34591107/writing-code-contracts-on-methods-throwing-events
    [ContractClassFor(typeof(IListenable<>))]
    internal abstract class IListenableContract<T> : IListenable<T>
    {
        // ReSharper disable InvocationIsSkipped

        public EventTypes ActiveEvents
        {
            get {
                // No preconditions


                // The active events must exist
                Ensures(All.HasFlag(Result<EventTypes>()));

                // The active events must be listenable
                Ensures(ListenableEvents.HasFlag(Result<EventTypes>()));

                // TODO: Check this matches the actual active events.


                return default(EventTypes);
            }
        }

        public EventTypes ListenableEvents
        {
            get {
                // No preconditions


                // The listenable events must exist
                Ensures(All.HasFlag(Result<EventTypes>()));


                return default(EventTypes);
            }
        }

        public event EventHandler CollectionChanged
        {
            add {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Changed), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Changed));

                // No other events became active
                Ensures(ActiveEvents == OldValue(ActiveEvents | Changed));


                return;
            }
            remove {
                // Event is active
                Requires(ActiveEvents.HasFlag(Changed), EventMustBeActive);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ClearedEventArgs> CollectionCleared
        {
            add {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Cleared), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Cleared));

                // No other events became active
                Ensures(ActiveEvents == OldValue(ActiveEvents | Cleared));


                return;
            }
            remove {
                // Event is active
                Requires(ActiveEvents.HasFlag(Cleared), EventMustBeActive);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemInserted
        {
            add {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Inserted), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Inserted));

                // No other events became active
                Ensures(ActiveEvents == OldValue(ActiveEvents | Inserted));


                return;
            }
            remove {
                // Event is active
                Requires(ActiveEvents.HasFlag(Inserted), EventMustBeActive);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt
        {
            add {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(RemovedAt), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(RemovedAt));

                // No other events became active
                Ensures(ActiveEvents == OldValue(ActiveEvents | RemovedAt));


                return;
            }
            remove {
                // Event is active
                Requires(ActiveEvents.HasFlag(RemovedAt), EventMustBeActive);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded
        {
            add {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Added), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Added));

                // No other events became active
                Ensures(ActiveEvents == OldValue(ActiveEvents | Added));


                return;
            }
            remove {
                // Event is active
                Requires(ActiveEvents.HasFlag(Added), EventMustBeActive);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved
        {
            add {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Removed), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Removed));

                // No other events became active
                Ensures(ActiveEvents == OldValue(ActiveEvents | Removed));


                return;
            }
            remove {
                // Event is active
                Requires(ActiveEvents.HasFlag(Removed), EventMustBeActive);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        #region SCG.IEnumerable<T>

        public abstract SCG.IEnumerator<T> GetEnumerator();
        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IShowable

        public abstract string ToString(string format, IFormatProvider formatProvider);
        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);

        #endregion

        #region ICollectionValue<T>

        public abstract bool AllowsNull { get; }
        public abstract int Count { get; }
        public abstract Speed CountSpeed { get; }
        public abstract bool IsEmpty { get; }
        public abstract T Choose();
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract T[] ToArray();

        #endregion

        #endregion
    }
}