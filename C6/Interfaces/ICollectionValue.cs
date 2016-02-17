﻿// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using SCG = System.Collections.Generic;

using static C6.EventTypes;


namespace C6
{
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
        /// Gets a bit flag indicating the collection's subscribable events.
        /// </summary>
        /// <value>
        /// The bit flag indicating the collection's subscribable events.
        /// </value>
        [Pure]
        EventTypes ListenableEvents { get; }


        /// <summary>
        /// Gets a bit flag indicating the collection's currently subscribed
        /// events.
        /// </summary>
        /// <value>
        /// The bit flag indicating the collection's currently subscribed events.
        /// </value>
        [Pure]
        EventTypes ActiveEvents { get; }


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
        /// <seealso cref="EventTypes.Changed"/>
        /// <seealso cref="ICollection{T}.Add"/>
        /// <seealso cref="ICollection{T}.Clear"/>
        /// <seealso cref="ICollection{T}.FindOrAdd"/>
        /// <seealso cref="ICollection{T}.Remove(T)"/>
        /// <seealso cref="ICollection{T}.Remove(T, out T)"/>
        /// <seealso cref="ICollection{T}.RemoveAll(T)"/>
        /// <seealso cref="ICollection{T}.RemoveAll(SCG.IEnumerable{T})"/>
        /// <seealso cref="ICollection{T}.RetainAll"/>
        /// <seealso cref="ICollection{T}.Update(T)"/>
        /// <seealso cref="ICollection{T}.Update(T, out T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T, out T)"/>
        /// <seealso cref="IExtensible{T}.Add"/>
        /// <seealso cref="IExtensible{T}.AddAll"/>
        /// <seealso cref="IIndexed{T}.RemoveAt"/>
        /// <seealso cref="IIndexed{T}.RemoveIndexRange"/>
        /// <seealso cref="IPriorityQueue{T}.Contains(IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.Contains(IPriorityQueueHandle{T}, out T)"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.this"/>
        /// <seealso cref="IPriorityQueue{T}.Remove"/>
        /// <seealso cref="IPriorityQueue{T}.Replace"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Pop"/>
        /// <seealso cref="IStack{T}.Push"/>
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
        /// <seealso cref="EventTypes.Cleared"/>
        /// <seealso cref="ICollection{T}.Clear"/>
        /// <seealso cref="IIndexed{T}.RemoveIndexRange"/>
        event EventHandler<ClearedEventArgs> CollectionCleared;


        // TODO: When an item is inserted into a list, both ItemInserted and ItemsAdded events will be fired.
        /// <summary>
        /// Occurs when an item is added to the collection.
        /// </summary>
        /// <remarks>
        /// The event is raised after an item has been added to the collection 
        /// and the collection in an internally consistent state, and before
        /// the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.Added"/>
        /// <seealso cref="ICollection{T}.Add"/>
        /// <seealso cref="ICollection{T}.FindOrAdd"/>
        /// <seealso cref="ICollection{T}.Update(T)"/>
        /// <seealso cref="ICollection{T}.Update(T, out T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T, out T)"/>
        /// <seealso cref="IExtensible{T}.Add"/>
        /// <seealso cref="IExtensible{T}.AddAll"/>
        /// <seealso cref="IPriorityQueue{T}.Replace"/>
        /// <seealso cref="IPriorityQueue{T}.this[IPriorityQueueHandle{T}]"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Push"/>
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
        /// <seealso cref="EventTypes.Removed"/>
        /// <seealso cref="ICollection{T}.Remove(T)"/>
        /// <seealso cref="ICollection{T}.Remove(T, out T)"/>
        /// <seealso cref="ICollection{T}.RemoveAll(T)"/>
        /// <seealso cref="ICollection{T}.RemoveAll(SCG.IEnumerable{T})"/>
        /// <seealso cref="ICollection{T}.RetainAll"/>
        /// <seealso cref="ICollection{T}.Update(T)"/>
        /// <seealso cref="ICollection{T}.Update(T, out T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T, out T)"/>
        /// <seealso cref="IIndexed{T}.RemoveAt"/>
        /// <seealso cref="IPriorityQueue{T}.Remove"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMax(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin()"/>
        /// <seealso cref="IPriorityQueue{T}.RemoveMin(out IPriorityQueueHandle{T})"/>
        /// <seealso cref="IPriorityQueue{T}.Replace"/>
        /// <seealso cref="IPriorityQueue{T}.this"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        /// <seealso cref="IStack{T}.Pop"/>
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
        /// <seealso cref="EventTypes.Inserted"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Push"/>
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
        /// <seealso cref="EventTypes.RemovedAt"/>
        /// <seealso cref="IIndexed{T}.RemoveAt"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        /// <seealso cref="IStack{T}.Pop"/>
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
        /// Gets a value indicating whether the collection allows <c>null</c>
        /// items.
        /// </summary>
        /// <value><c>true</c> if the collection allows <c>null</c> items;
        /// otherwise, <c>false</c>.</value>
        /// <remarks>
        /// <para>
        /// If the collection disallows <c>null</c> items, none of the items in
        /// the collection can be <c>null</c>: adding or inserting a
        /// <c>null</c> item will result in an error, and any property or
        /// method returning an item from the collection is guaranteed not to
        /// return <c>null</c>. If the collection allows <c>null</c> items, the
        /// collection user must check for <c>null</c>.
        /// </para>
        /// <para>
        /// <see cref="AllowsNull"/> does not reflect whether the collection 
        /// actually contains any <c>null</c> items.
        /// </para>
        /// <para>
        /// If <typeparamref name="T"/> is a value type, then
        /// <see cref="AllowsNull"/> is always <c>false</c>.
        /// </para>
        /// </remarks>
        [Pure]
        bool AllowsNull { get; }


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
        // ReSharper disable InvocationIsSkipped

        public EventTypes ListenableEvents {
            get {
                // No Requires


                // The listenable events must exist
                Contract.Ensures(All.HasFlag(Contract.Result<EventTypes>()));


                throw new NotImplementedException();
            }
        }


        public EventTypes ActiveEvents {
            get {
                // No Requires


                // The active events must exist
                Contract.Ensures(All.HasFlag(Contract.Result<EventTypes>()));

                // The active events must be listenable
                Contract.Ensures(ListenableEvents.HasFlag(Contract.Result<EventTypes>()));

                // TODO: Check this matches the actual active events.


                throw new NotImplementedException();
            }
        }


        public event EventHandler CollectionChanged {
            add {
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
            remove {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Changed)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ClearedEventArgs> CollectionCleared {
            add {
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
            remove {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Cleared)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded {
            add {
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
            remove {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Added)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved {
            add {
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
            remove {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Removed)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemAtEventArgs<T>> ItemInserted {
            add {
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
            remove {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(Inserted)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }


        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt {
            add {
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
            remove {
                // Value must be non-null
                Contract.Requires(value != null);

                // Event is listenable
                Contract.Requires(ListenableEvents.HasFlag(RemovedAt)); // TODO: Use <UnlistenableEventException>?


                // No Ensures


                throw new NotImplementedException();
            }
        }

        public bool IsEmpty {
            get {
                // No Requires


                // Returns true if Count is zero, otherwise false
                Contract.Ensures(Contract.Result<bool>() == (Count == 0));

                // Returns true if the enumerator is empty, otherwise false
                Contract.Ensures(Contract.Result<bool>() != this.Any());


                throw new NotImplementedException();
            }
        }


        // Contracts are copied to ICollection<T>.Count. Keep both updated!
        public int Count {
            get {
                // No Requires


                // Returns a non-negative number
                Contract.Ensures(Contract.Result<int>() >= 0);

                // Returns the same as the number of items in the enumerator
                Contract.Ensures(Contract.Result<int>() == this.Count());


                throw new NotImplementedException();
            }
        }


        public Speed CountSpeed {
            get {
                // No Requires


                // Result is a valid enum constant
                Contract.Ensures(Enum.IsDefined(typeof(Speed), Contract.Result<Speed>()));


                throw new NotImplementedException();
            }
        }

        public bool AllowsNull
        {
            get
            {
                // No Requires


                // Value types must return false
                Contract.Ensures(!typeof(T).IsValueType || !Contract.Result<bool>());


                throw new NotImplementedException();
            }
        }

        public T Choose()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // Return value is from the collection
            Contract.Ensures(this.Contains(Contract.Result<T>()));


            throw new NotImplementedException();
        }


        // Contracts are copied to ICollection.CopyTo. Keep both updated!
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Argument must be non-null
            Contract.Requires(array != null); // TODO: Use <ArgumentNullException>?

            // Argument must be within bounds
            Contract.Requires(0 <= arrayIndex); // TODO: Use <ArgumentOutOfRangeException>?
            Contract.Requires(arrayIndex + Count <= array.Length); // TODO: Use <ArgumentOutOfRangeException>?

            // Array contains the collection's items in enumeration order from arrayIndex
            Contract.Ensures(Enumerable.SequenceEqual(Enumerable.Skip(array, arrayIndex), this));


            throw new NotImplementedException();
        }


        public T[] ToArray()
        {
            // No Requires


            // Result is non-null
            Contract.Ensures(Contract.Result<T[]>() != null);

            // Result contains the collection's items in enumeration order
            Contract.Ensures(Enumerable.SequenceEqual(Contract.Result<T[]>(), this));


            throw new NotImplementedException();
        }


        // ReSharper restore InvocationIsSkipped


        #region Non-Contract Methods

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);
        public abstract string ToString(string format, IFormatProvider formatProvider);

        #endregion
    }
}