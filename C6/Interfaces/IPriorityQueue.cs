// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: How does Add handle duplicates?
    // TODO: Break into Min and Max priority queues?
    /// <summary>
    /// Represents a generic collection of items prioritized by a comparison
    /// (order) relation. Supports adding items and reporting or removing
    /// extremal elements.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <remarks>
    /// <para>
    /// The priority queue makes it possible to allocate an
    /// <see cref="IPriorityQueueHandle{T}"/> for an item, when adding the item
    /// to the queue. The resulting handle may be used for deleting the item
    /// efficiently even if not extremal, and for replacing the item.
    /// </para>
    /// <para>
    /// A priority queue typically only holds numeric priorities associated
    /// with some objects maintained separately in other collection objects.
    /// </para>
    /// </remarks>
    [ContractClass(typeof(IPriorityQueueContract<>))]
    public interface IPriorityQueue<T> : IExtensible<T>
    {
        /// <summary>
        /// Gets the <see cref="SCG.IComparer{T}"/> used by the priority queue.
        /// </summary>
        /// <value>The <see cref="SCG.IComparer{T}"/> used by the priority
        /// queue.</value>
        [Pure]
        SCG.IComparer<T> Comparer { get; }


        // TODO: Rename to Min
        /// <summary>
        /// Returns the current least item in the priority queue.
        /// </summary>
        /// <returns>The least item in the priority queue.</returns>
        [Pure]
        T FindMin();


        // TODO: Rename to Min
        /// <summary>
        /// Returns the current least item in the priority queue.
        /// </summary>
        /// <param name="handle">The handle associated with the least item.</param>
        /// <returns>The least item in the priority queue.</returns>
        [Pure]
        T FindMin(out IPriorityQueueHandle<T> handle);


        /// <summary>
        /// Removes and returns the least item in the priority queue.
        /// </summary>
        /// <returns>The least item in the priority queue.</returns>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the least item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        T RemoveMin();


        /// <summary>
        /// Removes and returns the least item in the priority queue.
        /// </summary>
        /// <param name="handle">The handle associated with the least item.</param>
        /// <returns>The least item in the priority queue.</returns>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the least item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        T RemoveMin(out IPriorityQueueHandle<T> handle);


        // TODO: Rename to Max
        /// <summary>
        /// Returns the current largest item in the priority queue.
        /// </summary>
        /// <returns>The current largest item in the priority queue.</returns>
        [Pure]
        T FindMax();


        // TODO: Rename to Max
        /// <summary>
        /// Returns the current largest item in the priority queue.
        /// </summary>
        /// <param name="handle">The handle associated with the largest item.</param>
        /// <returns>The current largest item in the priority queue.</returns>
        [Pure]
        T FindMax(out IPriorityQueueHandle<T> handle);


        /// <summary>
        /// Removes and returns the largest item in the priority queue.
        /// </summary>
        /// <returns>The largest item in the priority queue.</returns>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the largest
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        T RemoveMax();


        /// <summary>
        /// Removes and returns the largest item in the priority queue.
        /// </summary>
        /// <param name="handle">The handle associated with the largest item.
        /// </param>
        /// <returns>The largest item in the priority queue.</returns>
        T RemoveMax(out IPriorityQueueHandle<T> handle);


        // TODO: Exception: what if the handle is of the wrong type?
        /// <summary>
        /// Gets or sets the item with which the specified handle is
        /// associated.
        /// </summary>
        /// <value>The item with which the specified handle is/should be
        /// associated with. <c>null</c> is allowed for nullable items.</value>
        /// <param name="handle">
        /// The handle associated with the item to get or set. The handle must
        /// be associated with an item in the priority queue.
        /// </param>
        /// <returns>The specified handle's associated item.</returns>
        /// <exception cref="InvalidPriorityQueueHandleException">
        /// The handle is not associated with an item in this priority queue,
        /// or the handle is associated with an item in another priority queue.
        /// </exception>
        /// <remarks>
        /// The setter raises the following events (in that order) with the
        /// collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the old item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the new item and
        /// a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Contains(IPriorityQueueHandle{T})"/>
        /// <seealso cref="Contains(IPriorityQueueHandle{T}, out T)"/>
        /// <seealso cref="Replace"/>
        T this[IPriorityQueueHandle<T> handle] { [Pure] get; set; }


        /// <summary>
        /// Checks if the specified handle is associated with an item in the
        /// priority queue.
        /// </summary>
        /// <param name="handle">The handle associated with the item to find in
        /// the priority queue.</param>
        /// <returns><c>true</c> if the handle is associated with an item in
        /// the priority queue; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Contains(IPriorityQueueHandle{T}, out T)"/>
        /// <seealso cref="this[IPriorityQueueHandle{T}]"/>
        [Pure]
        bool Contains(IPriorityQueueHandle<T> handle);


        /// <summary>
        /// Checks if the specified handle is associated with an item in the
        /// priority queue.
        /// </summary>
        /// <param name="handle">The handle associated with the item to find in
        /// the priority queue.</param>
        /// <param name="item">The item with which the specified handle is
        /// associated, if the item is in the priority queue;
        /// otherwise, <c>default(T)</c>.</param>
        /// <returns><c>true</c> if the handle is associated with an item in
        /// the priority queue; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Contains(IPriorityQueueHandle{T})"/>
        /// <seealso cref="this[IPriorityQueueHandle{T}]"/>
        [Pure]
        bool Contains(IPriorityQueueHandle<T> handle, out T item);


        // TODO: Exception: what if the handle is of the wrong type?
        // TODO: Rename to Update?
        // TODO: Remove if this is the same as this[handle] = item?
        /// <summary>
        /// Replaces the item with which the specified handle is associated.
        /// </summary>
        /// <param name="handle">The specified handle.</param>
        /// <param name="item">The new item with which the handle should be 
        /// associated. <c>null</c> is allowed for nullable items.</param>
        /// <returns>The item that the handle was previously associated with.
        /// </returns>
        /// <exception cref="InvalidPriorityQueueHandleException">
        /// The handle is not associated with an item in this priority queue,
        /// or the handle is associated with an item in another priority queue.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This is typically used for changing the priority of a queued item.
        /// </para>
        /// <para>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the old item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the new item and
        /// a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso cref="this[IPriorityQueueHandle{T}]"/>
        T Replace(IPriorityQueueHandle<T> handle, T item);


        // TODO: Reorder parameters?
        /// <summary>
        /// Add an item to the priority queue, receiving a handle for the item 
        /// in the queue, or reusing an existing unused handle.
        /// </summary>
        /// <param name="handle">On output: a handle for the added item. 
        /// On input: null for allocating a new handle, or a currently unused handle for reuse. 
        /// A handle for reuse must be compatible with this priority queue, 
        /// by being created by a priority queue of the same runtime type, but not 
        /// necessarily the same priority queue object.</param>
        /// <param name="item">The item with which the handle should be 
        /// associated. <c>null</c> is allowed for nullable items.</param>
        /// <returns><c>true</c>.</returns>
        /// <remarks>
        /// <para>If the item is added, it raises the following events (in that 
        /// order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the item and a 
        /// count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        bool Add(ref IPriorityQueueHandle<T> handle, T item);


        /// <summary>
        /// Removes the item, with which the specified handle is associated,
        /// from the priority queue.
        /// </summary>
        /// <param name="handle">The specified handle, which will be
        /// invalidated, but reusable.</param>
        /// <returns>The item that the handle was previously associated with.
        /// </returns>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        T Remove(IPriorityQueueHandle<T> handle);
    }



    [ContractClassFor(typeof(IPriorityQueue<>))]
    internal abstract class IPriorityQueueContract<T> : IPriorityQueue<T>
    {
        // ReSharper disable InvocationIsSkipped

        public SCG.IComparer<T> Comparer {
            get {
                // No Requires


                // Result is non-null
                Contract.Ensures(Contract.Result<SCG.IComparer<T>>() != null);


                throw new NotImplementedException();
            }
        }


        public T FindMin()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are greater than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) >= 0));

            // The count remains the same
            Contract.Ensures(Count == Contract.OldValue(Count));

            // Return value is from the collection
            Contract.Ensures(this.Contains(Contract.Result<T>()));


            throw new NotImplementedException();
        }


        public T FindMin(out IPriorityQueueHandle<T> handle)
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are greater than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) >= 0));

            // Result is same as FindMin
            Contract.Ensures(Contract.Result<T>().Equals(FindMin()));

            // The handle is associated with the result
            Contract.Ensures(this[Contract.ValueAtReturn(out handle)].Equals(Contract.Result<T>()));

            // The count remains the same
            Contract.Ensures(Count == Contract.OldValue(Count));

            // Return value is from the collection
            Contract.Ensures(this.Contains(Contract.Result<T>()));

            throw new NotImplementedException();
        }


        public T RemoveMin()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are greater than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) >= 0));

            // Result is same as FindMin
            Contract.Ensures(Contract.Result<T>().Equals(Contract.OldValue(FindMin())));

            // Removing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);

            // Return value is from the collection
            Contract.Ensures(Contract.OldValue(this.Contains(Contract.Result<T>()))); // TODO: Does this work?


            throw new NotImplementedException();
        }


        public T RemoveMin(out IPriorityQueueHandle<T> handle)
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are greater than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) >= 0));

            // Result is same as FindMin
            Contract.Ensures(Contract.Result<T>().Equals(Contract.OldValue(FindMin())));

            // Removing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);

            // The handle is no longer associated with an item in the priority queue
            Contract.Ensures(!Contains(Contract.ValueAtReturn(out handle)));

            // Return value is from the collection
            Contract.Ensures(Contract.OldValue(this.Contains(Contract.Result<T>()))); // TODO: Does this work?


            throw new NotImplementedException();
        }


        public T FindMax()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are less than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) <= 0));

            // The count remains the same
            Contract.Ensures(Count == Contract.OldValue(Count));

            // Return value is from the collection
            Contract.Ensures(this.Contains(Contract.Result<T>()));

            throw new NotImplementedException();
        }


        public T FindMax(out IPriorityQueueHandle<T> handle)
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are less than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) <= 0));

            // Result is same as FindMax
            Contract.Ensures(Contract.Result<T>().Equals(FindMax()));

            // The handle is associated with the result
            Contract.Ensures(this[Contract.ValueAtReturn(out handle)].Equals(Contract.Result<T>()));

            // The count remains the same
            Contract.Ensures(Count == Contract.OldValue(Count));

            // Return value is from the collection
            Contract.Ensures(this.Contains(Contract.Result<T>()));


            throw new NotImplementedException();
        }


        public T RemoveMax()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are less than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) <= 0));

            // Result is same as FindMax
            Contract.Ensures(Contract.Result<T>().Equals(Contract.OldValue(FindMax())));

            // Removing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);

            // Return value is from the collection
            Contract.Ensures(Contract.OldValue(this.Contains(Contract.Result<T>()))); // TODO: Does this work?


            throw new NotImplementedException();
        }


        public T RemoveMax(out IPriorityQueueHandle<T> handle)
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // All items in the collection are less than or equal to the result
            Contract.Ensures(Contract.ForAll(this, item => Comparer.Compare(item, Contract.Result<T>()) <= 0));

            // Result is same as FindMax
            Contract.Ensures(Contract.Result<T>().Equals(Contract.OldValue(FindMax())));

            // Removing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);

            // The handle is no longer associated with an item in the priority queue
            Contract.Ensures(!Contains(Contract.ValueAtReturn(out handle)));

            // Return value is from the collection
            Contract.Ensures(Contract.OldValue(this.Contains(Contract.Result<T>()))); // TODO: Does this work?


            throw new NotImplementedException();
        }


        public T this[IPriorityQueueHandle<T> handle] {
            get {
                // Handle must be non-null
                Contract.Requires(handle != null);

                // Handle must be associated with item in the priority queue
                Contract.Requires(Contains(handle));


                // Result is non-null
                Contract.Ensures(AllowsNull || Contract.Result<T>() != null);
                
                // Return value is from the collection
                Contract.Ensures(this.Contains(Contract.Result<T>()));


                throw new NotImplementedException();
            }

            set {
                // Collection must be non-empty
                Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?

                // Handle must be non-null
                Contract.Requires(handle != null); // TODO: Use <ArgumentNullException>?

                // Argument must be non-null if collection disallows null values
                Contract.Requires(AllowsNull || value != null); // TODO: Use <ArgumentNullException>?
                
                // Handle must be associated with item in the priority queue
                Contract.Requires(Contains(handle)); // TODO: Use <InvalidPriorityQueueHandleException>?

                

                // The handle is associated with the result
                Contract.Ensures(this[handle].Equals(value));

                // Replacing an item does not change the count
                Contract.Ensures(Count == Contract.OldValue(Count));

                // Return value is from the collection
                Contract.Ensures(this.Contains(value));


                throw new NotImplementedException();
            }
        }


        // We know very little about handles
        public bool Contains(IPriorityQueueHandle<T> handle)
        {
            // Handle must be non-null
            Contract.Requires(handle != null);


            // No Ensures


            throw new NotImplementedException();
        }


        // We know very little about handles
        public bool Contains(IPriorityQueueHandle<T> handle, out T item)
        {
            // Handle must be non-null
            Contract.Requires(handle != null);


            // Result is equal to Contains' result
            Contract.Ensures(Contract.Result<bool>() == Contains(handle));

            // Item is non-null
            Contract.Ensures(AllowsNull || Contract.ValueAtReturn(out item) != null);


            throw new NotImplementedException();
        }


        public T Replace(IPriorityQueueHandle<T> handle, T item)
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?

            // Collection must be non-read-only
            Contract.Requires(!IsReadOnly); // TODO: Use <ReadOnlyCollectionException>?

            // Handle must be associated with item in the priority queue
            Contract.Requires(Contains(handle));

            // Argument must be non-null if collection disallows null values
            Contract.Requires(AllowsNull || item != null); // TODO: Use <ArgumentNullException>?
            

            // Count remains unchanged
            Contract.Ensures(Count == Contract.OldValue(Count));

            // The collection is non-empty
            Contract.Ensures(!IsEmpty);

            // Handle is associated with new item
            Contract.Ensures(EqualityComparer.Equals(this[handle], item));

            // Result is the old item with which the handle was associated
            Contract.Ensures(EqualityComparer.Equals(Contract.OldValue(this[handle]), Contract.Result<T>()));

            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // Return value is from the collection
            Contract.Ensures(Contract.OldValue(this.Contains(Contract.Result<T>()))); // TODO: Does this work?


            throw new NotImplementedException();
        }


        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public bool Add(T item)
        {
            // No extra Requires allowed


            // Always returns true
            Contract.Ensures(Contract.Result<bool>());


            throw new NotImplementedException();
        }


        public bool Add(ref IPriorityQueueHandle<T> handle, T item)
        {
            // Collection must be non-read-only
            Contract.Requires(!IsReadOnly); // TODO: Use <ReadOnlyCollectionException>?

            // Argument must be non-null if collection disallows null values
            Contract.Requires(AllowsNull || item != null); // TODO: Use <ArgumentNullException>?
            

            // Always returns true
            Contract.Ensures(Contract.Result<bool>());

            // The collection becomes non-empty
            Contract.Ensures(!IsEmpty);

            // The collection will contain the item added
            Contract.Ensures(this.Contains(item, EqualityComparer));

            // Adding an item increases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) + 1);

            // Adding the item increases the number of equal items by one
            Contract.Ensures(this.Count(x => EqualityComparer.Equals(x, item)) == Contract.OldValue(this.Count(x => EqualityComparer.Equals(x, item))) + 1);

            // Returned handle is non-null
            Contract.Ensures(Contract.ValueAtReturn(out handle) != null);

            // Returned handle is associated with item
            Contract.Ensures(EqualityComparer.Equals(this[Contract.ValueAtReturn(out handle)], item));


            throw new NotImplementedException();
        }


        // We do not know the actual item associated with the handle (the one that is removed)
        // nor can we check that the handle is invalidated
        public T Remove(IPriorityQueueHandle<T> handle)
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?

            // Handle must be associated with item in the priority queue
            Contract.Requires(Contains(handle));


            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // Removing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);

            // Count remains the same if an exception is thrown
            Contract.EnsuresOnThrow<InvalidPriorityQueueHandleException>(Count == Contract.OldValue(Count));

            // Count remains the same if an exception is thrown
            Contract.EnsuresOnThrow<InvalidPriorityQueueHandleException>(this.SequenceEqual(Contract.OldValue(this.ToList()), EqualityComparer));

            // Return value is from the collection
            Contract.Ensures(Contract.OldValue(this.Contains(Contract.Result<T>()))); // TODO: Does this work?


            throw new NotImplementedException();
        }


        // ReSharper restore InvocationIsSkipped


        #region Non-Contract Methods

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract string ToString(string format, IFormatProvider formatProvider);
        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);
        public abstract EventTypes ListenableEvents { get; }
        public abstract EventTypes ActiveEvents { get; }
        public abstract event EventHandler CollectionChanged;
        public abstract event EventHandler<ClearedEventArgs> CollectionCleared;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsAdded;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemInserted;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;
        public abstract bool IsEmpty { get; }
        public abstract int Count { get; }
        public abstract Speed CountSpeed { get; }
        public abstract bool AllowsNull { get; }
        public abstract T Choose();
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract T[] ToArray();
        public abstract bool IsReadOnly { get; }
        public abstract bool AllowsDuplicates { get; }
        public abstract bool DuplicatesByCounting { get; }
        public abstract SCG.IEqualityComparer<T> EqualityComparer { get; }
        public abstract void AddAll(SCG.IEnumerable<T> items);

        #endregion
    }
}