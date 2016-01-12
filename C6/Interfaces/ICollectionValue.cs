// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using SCG = System.Collections.Generic;
using static C6.EventType;

namespace C6
{
    // TODO: Document events with <seealso/>
    /// <summary>
    /// Represents a generic, observable collection that may be enumerated and
    /// can answer efficiently how many items it contains.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <remarks>
    /// Like <see cref="SCG.IEnumerable{T}"/>, the interface does not prescribe
    /// any operations to initialize or update the collection. Its main usage
    /// is to be the return type of query operations on generic collection.
    /// </remarks>
    [ContractClass(typeof(ICollectionValueContract<>))]
    public interface ICollectionValue<T> : SCG.IEnumerable<T>, IShowable
    {
        /// <summary>
        /// Gets a bitmap flag indicating the collection's subscribable events.
        /// </summary>
        /// <value>
        /// The bitmap indicating the collection's subscribable events.
        /// </value>
        [Pure]
        EventType ListenableEvents { get; }


        /// <summary>
        /// Gets a bitmap flag indicating the collection's currently subscribed
        /// events.
        /// </summary>
        /// <value>
        /// The bitmap indicating the collection's currently subscribed events.
        /// </value>
        [Pure]
        EventType ActiveEvents { get; }


        /// <summary>
        /// Occurs when the collection is changed.
        /// </summary>
        /// <remarks>
        /// <para>The event is raised after an operation on the collection has 
        /// changed its contents and the collection in an internally consistent
        /// state. Any operation that changes the collection must raise
        /// <see cref="CollectionChanged"/> as its last event.</para>
        /// <para> Normally, a multi-operation like 
        /// <see cref="IExtensible{T}.AddAll"/> will only raise one
        /// <see cref="CollectionChanged"/> event.
        /// </para>
        /// </remarks>
        /// <seealso cref="EventType.Changed"/>
        /// <seealso cref="IExtensible{T}.Add"/>
        /// <seealso cref="IExtensible{T}.AddAll"/>
        /// <seealso cref="IStack{T}.Push"/>
        /// <seealso cref="IStack{T}.Pop"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        event EventHandler CollectionChanged;


        // TODO: Document different scenarios (8.8.5)
        /// <summary>
        /// Occurs when the collection is cleared.
        /// </summary>
        /// <remarks>
        /// The event is raised after the collection (or a part of it) is 
        /// cleared and the collection in an internally consistent state, and
        /// before the corresponding <see cref="CollectionChanged"/> event is
        /// raised.
        /// </remarks>
        /// <seealso cref="EventType.Cleared"/>
        event EventHandler<ClearedEventArgs> CollectionCleared;


        // TODO: an Update operation will fire an ItemsRemoved and an ItemsAdded event.
        // TODO: When an item is inserted into a list, both ItemInserted and ItemsAdded events will be fired.
        /// <summary>
        /// Occurs when an item is added to the collection.
        /// </summary>
        /// <remarks>
        /// The event is raised after an item has been added to the collection 
        /// and the collection in an internally consistent state, and before
        /// the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventType.Added"/>
        /// <seealso cref="IExtensible{T}.Add"/>
        /// <seealso cref="IExtensible{T}.AddAll"/>
        /// <seealso cref="IStack{T}.Push"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        event EventHandler<ItemCountEventArgs<T>> ItemsAdded;


        /// <summary>
        /// Occurs when an item is removed from the collection.
        /// </summary>
        /// <remarks>
        /// The event is raised after an item has been removed from the
        /// collection and the collection in an internally consistent state,
        /// and before the corresponding <see cref="CollectionChanged"/> event
        /// is raised.
        /// </remarks>
        /// <seealso cref="EventType.Removed"/>
        /// <seealso cref="IStack{T}.Pop"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;


        /// <summary>
        /// Occurs when an item is inserted at a specific position in the
        /// collection.
        /// </summary>
        /// <remarks>
        /// The event is raised after an item has been inserted into the
        /// collection and the collection in an internally consistent state,
        /// and before the corresponding <see cref="CollectionChanged"/> event
        /// is raised.
        /// </remarks>
        /// <seealso cref="EventType.Inserted"/>
        /// <seealso cref="IStack{T}.Push"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        event EventHandler<ItemAtEventArgs<T>> ItemInserted;


        /// <summary>
        /// Occurs when an item is removed from a specific position in the
        /// collection.
        /// </summary>
        /// <remarks>
        /// The event is raised after an item has been removed from the collection 
        /// and the collection in an internally consistent state, and before
        /// the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventType.RemovedAt"/>
        /// <seealso cref="IStack{T}.Pop"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;


        /// <summary>
        /// Gets a value indicating whether the collection is empty.
        /// </summary>
        /// <value><c>true</c> if the collection is empty;
        /// otherwise, <c>false</c>.</value>
        [Pure]
        bool IsEmpty { get; }


        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        /// <value>The number of items contained in the collection.</value>
        [Pure]
        int Count { get; }


        /// <summary>
        /// Gets a value characterizing the asymptotic complexity of
        /// <see cref="Count"/> proportional to collection size (worst-case or
        /// amortized as relevant).
        /// </summary>
        /// <value>A characterization of the asymptotic speed of
        /// <see cref="Count"/> proportional to collection size.</value>
        [Pure]
        Speed CountSpeed { get; }


        /// <summary>
        /// Returns some item from the collection.
        /// </summary>
        /// <returns>Some item in the collection.</returns>
        /// <remarks>
        /// Implementations must assure that the item returned may be 
        /// efficiently removed. However, it is not required that repeated 
        /// calls give the same result.
        /// </remarks>
        [Pure]
        T Choose();


        /// <summary>
        /// Copies the items of the collection to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is
        /// the destination of the items copied from the collection. The
        /// <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based arrayIndex in array at 
        /// which copying begins.</param>
        [Pure]
        void CopyTo(T[] array, int arrayIndex);


        /// <summary>
        /// Creates an array from the collection in the same order as the enumerator would output them.
        /// </summary>
        /// <returns>An array that contains the items from the collection.</returns>
        [Pure]
        T[] ToArray();
    }



    // TODO: Add contracts on the events to ensure they are thrown http://stackoverflow.com/questions/34591107/writing-code-contracts-on-methods-throwing-events
    [ContractClassFor(typeof(ICollectionValue<>))]
    internal abstract class ICollectionValueContract<T> : ICollectionValue<T>
    {
        public EventType ListenableEvents
        {
            get
            {
                // No Requires


                // The listenable events must exist
                Contract.Ensures(All.HasFlag(Contract.Result<EventType>()));


                throw new NotImplementedException();
            }
        }


        public EventType ActiveEvents
        {
            get
            {
                // No Requires


                // The active events must exist
                Contract.Ensures(All.HasFlag(Contract.Result<EventType>()));

                // The active events must be listenable
                Contract.Ensures(ListenableEvents.HasFlag(Contract.Result<EventType>()));

                // TODO: Check this matches the actual active events.


                throw new NotImplementedException();
            }
        }


        public event EventHandler CollectionChanged
        {
            add
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Changed)); // TODO: Use <UnlistenableEventException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Changed));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Changed));


                throw new NotImplementedException();
            }
            remove
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Changed)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ClearedEventArgs> CollectionCleared
        {
            add
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Cleared)); // TODO: Use <UnlistenableEventException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Cleared));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Cleared));


                throw new NotImplementedException();
            }
            remove
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Cleared)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded
        {
            add
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Added)); // TODO: Use <UnlistenableEventException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Added));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Added));


                throw new NotImplementedException();
            }
            remove
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Added)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved
        {
            add
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Removed)); // TODO: Use <UnlistenableEventException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Removed));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Removed));


                throw new NotImplementedException();
            }
            remove
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Removed)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemAtEventArgs<T>> ItemInserted
        {
            add
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Inserted)); // TODO: Use <UnlistenableEventException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(Inserted));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | Inserted));


                throw new NotImplementedException();
            }
            remove
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Inserted)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt
        {
            add
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(RemovedAt)); // TODO: Use <UnlistenableEventException>?


                // Event is active
                Contract.Ensures(ActiveEvents.HasFlag(RemovedAt));

                // No other events became active
                Contract.Ensures(ActiveEvents == (Contract.OldValue(ActiveEvents) | RemovedAt));


                throw new NotImplementedException();
            }
            remove
            {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(RemovedAt)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }

        public bool IsEmpty
        {
            get
            {
                // No Requires

                
                // Returns true if Count is zero, otherwise false
                Contract.Ensures(Contract.Result<bool>() == (Count == 0));

                // Returns true if the enumerator is empty, otherwise false
                Contract.Ensures(Contract.Result<bool>() != this.Any());


                throw new NotImplementedException();
            }
        }


        public int Count
        {
            get
            {
                // No Requires


                // Returns a non-negative number
                Contract.Ensures(Contract.Result<int>() >= 0);

                // Returns the same as the number of items in the enumerator
                Contract.Ensures(Contract.Result<int>() == Enumerable.Count(this));


                throw new NotImplementedException();
            }
        }


        public Speed CountSpeed
        {
            get
            {
                // No Requires


                // Result is a valid enum constant
                Contract.Ensures(Enum.IsDefined(typeof(Speed), Contract.Result<Speed>()));


                throw new NotImplementedException();
            }
        }


        public T Choose()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is never null
            // Contract.Ensures(Contract.Result<T>() != null);

            // Return value is from the collection
            Contract.Ensures(this.Contains(Contract.Result<T>()));


            throw new NotImplementedException();
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            // Argument must be non-null
            Contract.Requires(array != null); // TODO: Use <ArgumentNullException>?

            // Argument must be within bounds
            Contract.Requires(0 <= arrayIndex && arrayIndex + Count <= array.Length); // TODO: Use <ArgumentOutOfRangeException>?


            // Array contains the collection's items in enumeration order from arrayIndex
            Contract.Ensures(Enumerable.SequenceEqual(Enumerable.Skip(array, arrayIndex), this));


            throw new NotImplementedException();
        }


        public T[] ToArray()
        {
            // No Requires


            // Result is never null
            Contract.Ensures(Contract.Result<T[]>() != null);

            // Result contains the collection's items in enumeration order
            Contract.Ensures(Enumerable.SequenceEqual(Contract.Result<T[]>(), this));


            throw new NotImplementedException();
        }

        #region Non-Contract Methods

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public abstract bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider);
        public abstract string ToString(string format, IFormatProvider formatProvider);

        #endregion
    }
}