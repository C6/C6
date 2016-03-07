// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

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
    public interface ICollectionValue<T> : SCG.IEnumerable<T> // TODO: Add IShowable again
    {
        /// <summary>
        /// Gets a bit flag indicating the collection's currently subscribed
        /// events.
        /// </summary>
        /// <value>
        /// The bit flag indicating the collection's currently subscribed events.
        /// </value>
        /// <seealso cref="ListenableEvents"/>
        [Pure]
        EventTypes ActiveEvents { get; }

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
        /// Gets a value indicating whether the collection is empty.
        /// </summary>
        /// <value><c>true</c> if the collection is empty;
        /// otherwise, <c>false</c>.</value>
        [Pure]
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a bit flag indicating the collection's subscribable events.
        /// </summary>
        /// <value>
        /// The bit flag indicating the collection's subscribable events.
        /// </value>
        /// <seealso cref="ActiveEvents"/>
        [Pure]
        EventTypes ListenableEvents { get; }

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

        /// <summary>
        /// Occurs when the collection has changed.
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
        /// <seealso cref="IList{T}.this"/>
        /// <seealso cref="IList{T}.Clear"/>
        /// <seealso cref="IList{T}.Insert(int,T)"/>
        /// <seealso cref="IList{T}.InsertAll"/>
        /// <seealso cref="IList{T}.InsertFirst"/>
        /// <seealso cref="IList{T}.InsertLast"/>
        /// <seealso cref="IList{T}.Remove()"/>
        /// <seealso cref="IList{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.RemoveFirst"/>
        /// <seealso cref="IList{T}.RemoveLast"/>
        /// <seealso cref="IList{T}.Reverse"/>
        /// <seealso cref="IList{T}.Shuffle()"/>
        /// <seealso cref="IList{T}.Shuffle(Random)"/>
        /// <seealso cref="IList{T}.Sort()"/>
        /// <seealso cref="IList{T}.Sort(SCG.IComparer{T})"/>
        /// <seealso cref="IList{T}.Sort(Comparison{T})"/>
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

        /// <summary>
        /// Occurs when (part of) the collection has been cleared.
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
        /// <seealso cref="IList{T}.Clear"/>
        event EventHandler<ClearedEventArgs> CollectionCleared;

        /// <summary>
        /// Occurs when an item was inserted at a specific position in the
        /// collection.
        /// </summary>
        /// <remarks>
        /// The event is raised after an item has been inserted into the
        /// collection and the collection in an internally consistent state,
        /// and before the corresponding <see cref="CollectionChanged"/> event
        /// is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.Inserted"/>
        /// <seealso cref="IList{T}.this"/>
        /// <seealso cref="IList{T}.Insert(int,T)"/>
        /// <seealso cref="IList{T}.InsertAll"/>
        /// <seealso cref="IList{T}.InsertFirst"/>
        /// <seealso cref="IList{T}.InsertLast"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Push"/>
        event EventHandler<ItemAtEventArgs<T>> ItemInserted;

        /// <summary>
        /// Occurs when an item was removed from a specific position in the
        /// collection.
        /// </summary>
        /// <remarks>
        /// The event is raised after an item has been removed from the collection 
        /// and the collection in an internally consistent state, and before
        /// the corresponding <see cref="CollectionChanged"/> event is raised.
        /// </remarks>
        /// <seealso cref="EventTypes.RemovedAt"/>
        /// <seealso cref="IIndexed{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.this"/>
        /// <seealso cref="IList{T}.Remove()"/>
        /// <seealso cref="IList{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.RemoveFirst"/>
        /// <seealso cref="IList{T}.RemoveLast"/>
        /// <seealso cref="IQueue{T}.Dequeue"/>
        /// <seealso cref="IStack{T}.Pop"/>
        event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;

        /// <summary>
        /// Occurs when an item was added to the collection.
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
        /// <seealso cref="IList{T}.this"/>
        /// <seealso cref="IList{T}.Insert(int,T)"/>
        /// <seealso cref="IList{T}.InsertAll"/>
        /// <seealso cref="IList{T}.InsertFirst"/>
        /// <seealso cref="IList{T}.InsertLast"/>
        /// <seealso cref="IPriorityQueue{T}.Replace"/>
        /// <seealso cref="IPriorityQueue{T}.this[IPriorityQueueHandle{T}]"/>
        /// <seealso cref="IQueue{T}.Enqueue"/>
        /// <seealso cref="IStack{T}.Push"/>
        event EventHandler<ItemCountEventArgs<T>> ItemsAdded;

        /// <summary>
        /// Occurs when an item was removed from the collection.
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
        /// <seealso cref="IList{T}.this"/>
        /// <seealso cref="IList{T}.Remove()"/>
        /// <seealso cref="IList{T}.RemoveAt"/>
        /// <seealso cref="IList{T}.RemoveFirst"/>
        /// <seealso cref="IList{T}.RemoveLast"/>
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
    }


    // TODO: Add contracts on the events to ensure they are thrown http://stackoverflow.com/questions/34591107/writing-code-contracts-on-methods-throwing-events
    [ContractClassFor(typeof(ICollectionValue<>))]
    internal abstract class ICollectionValueContract<T> : ICollectionValue<T>
    {
        // ReSharper disable InvocationIsSkipped

        public EventTypes ActiveEvents
        {
            get
            {
                // No preconditions


                // The active events must exist
                Ensures(All.HasFlag(Result<EventTypes>()));

                // The active events must be listenable
                Ensures(ListenableEvents.HasFlag(Result<EventTypes>()));

                // TODO: Check this matches the actual active events.


                return default(EventTypes);
            }
        }

        public bool AllowsNull
        {
            get
            {
                // No preconditions


                // Value types must return false
                Ensures(!typeof(T).IsValueType || !Result<bool>());


                return default(bool);
            }
        }

        // Contracts are copied to ICollection<T>.Count. Keep both updated!
        public int Count
        {
            get
            {
                // No preconditions


                // Returns a non-negative number
                Ensures(Result<int>() >= 0);

                // Returns the same as the number of items in the enumerator
                Ensures(Result<int>() == this.Count());


                return default(int);
            }
        }

        public Speed CountSpeed
        {
            get
            {
                // No preconditions


                // Result is a valid enum constant
                Ensures(Enum.IsDefined(typeof(Speed), Result<Speed>()));


                return default(Speed);
            }
        }

        public bool IsEmpty
        {
            get
            {
                // No preconditions


                // Returns true if Count is zero, otherwise false
                Ensures(Result<bool>() == (Count == 0));

                // Returns true if the enumerator is empty, otherwise false
                Ensures(Result<bool>() != this.Any());


                return default(bool);
            }
        }

        public EventTypes ListenableEvents
        {
            get
            {
                // No preconditions


                // The listenable events must exist
                Ensures(All.HasFlag(Result<EventTypes>()));


                return default(EventTypes);
            }
        }

        public T Choose()
        {
            // Collection must be non-empty
            Requires(!IsEmpty, CollectionMustBeNonEmpty);


            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Return value is from the collection
            Ensures(this.Contains(Result<T>()));


            return default(T);
        }

        // Contracts are copied to ICollection.CopyTo. Keep both updated!
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Argument must be non-null
            Requires(array != null, ArgumentMustBeNonNull);

            // Argument must be within bounds
            Requires(0 <= arrayIndex, ArgumentMustBeWithinBounds);
            Requires(arrayIndex + Count <= array.Length, ArgumentMustBeWithinBounds);


            // Array contains the collection's items in enumeration order from arrayIndex
            Ensures(array.Skip(arrayIndex).Take(Count).SequenceEqual(this));

            // The rest of the array is unchanged
            Ensures(OldValue(array.Take(arrayIndex).ToList()).SequenceEqual(array.Take(arrayIndex)));
            Ensures(OldValue(array.Skip(arrayIndex + Count).ToList()).SequenceEqual(array.Skip(arrayIndex + Count)));


            return;
        }

        public T[] ToArray()
        {
            // No preconditions


            // Result is non-null
            Ensures(Result<T[]>() != null);

            // Result contains the collection's items in enumeration order
            Ensures(Result<T[]>().SequenceEqual(this));


            return default(T[]);
        }

        public event EventHandler CollectionChanged
        {
            add
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Changed), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Changed));

                // No other events became active
                Ensures(ActiveEvents == (OldValue(ActiveEvents) | Changed));


                return;
            }
            remove
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Changed), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ClearedEventArgs> CollectionCleared
        {
            add
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Cleared), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Cleared));

                // No other events became active
                Ensures(ActiveEvents == (OldValue(ActiveEvents) | Cleared));


                return;
            }
            remove
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Cleared), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemInserted
        {
            add
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Inserted), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Inserted));

                // No other events became active
                Ensures(ActiveEvents == (OldValue(ActiveEvents) | Inserted));


                return;
            }
            remove
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Inserted), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt
        {
            add
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(RemovedAt), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(RemovedAt));

                // No other events became active
                Ensures(ActiveEvents == (OldValue(ActiveEvents) | RemovedAt));


                return;
            }
            remove
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(RemovedAt), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsAdded
        {
            add
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Added), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Added));

                // No other events became active
                Ensures(ActiveEvents == (OldValue(ActiveEvents) | Added));


                return;
            }
            remove
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Added), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        public event EventHandler<ItemCountEventArgs<T>> ItemsRemoved
        {
            add
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Removed), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // Event is active
                Ensures(ActiveEvents.HasFlag(Removed));

                // No other events became active
                Ensures(ActiveEvents == (OldValue(ActiveEvents) | Removed));


                return;
            }
            remove
            {
                // Event is listenable
                Requires(ListenableEvents.HasFlag(Removed), EventMustBeListenable);

                // Value must be non-null
                Requires(value != null, ArgumentMustBeNonNull);


                // No postconditions


                return;
            }
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        #region SCG.IEnumerable<T>
        
        // TODO: Ensure that no item is null if AllowsNull is false?
        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IShowable

        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);
        public abstract string ToString(string format, IFormatProvider formatProvider);

        #endregion

        #endregion
    }
}