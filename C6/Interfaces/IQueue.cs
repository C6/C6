// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using SCG = System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace C6
{
    /// <summary>
    /// Represents a generic first-in-first-out (FIFO) queue data structure.
    /// </summary>
    /// <typeparam name="T">The type of items in the stack.</typeparam>
    [ContractClass(typeof(IQueueContract<>))]
    public interface IQueue<T> : IDirectedCollectionValue<T>
    {
        // TODO: Also found in IStack<T>
        /// <summary>
        /// Gets the item at the specified index in the queue.
        /// The beginning of the queue has index <c>0</c>.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get.</param>
        /// <returns>The item at the specified index.</returns>
        [Pure]
        T this[int index] { get; }


        /// <summary>
        /// Adds an item to the end of the queue.
        /// </summary>
        /// <param name="item">The item to add to the queue.</param>
        /// <remarks>
        /// It raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemInserted"/> with the item and an 
        /// index of <c>coll.Count - 1</c>.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the item and a 
        /// count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void Enqueue(T item);


        /// <summary>
        /// Removes and returns the item at the beginning of the queue.
        /// </summary>
        /// <returns>
        /// The item that is removed from the beginning of the queue.
        /// </returns>
        /// <remarks>
        /// It raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and an 
        /// index of <c>0</c>.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the item and a 
        /// count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        T Dequeue();

        // TODO: Add Peek?
    }



    [ContractClassFor(typeof(IQueue<>))]
    internal abstract class IQueueContract<T> : IQueue<T>
    {
        /*
        // TODO: Not allowed: seems like an error with the statick checker: https://github.com/Microsoft/CodeContracts/issues/331
        public EventTypes ListenableEvents
        {
            get
            {
                // The events raised by the collection must be listenable
                Contract.Ensures(Contract.Result<EventTypes>().HasFlag(Changed | Added | Removed | Inserted | RemovedAt));


                throw new NotImplementedException();
            }
        }
        */


        public T this[int index]
        {
            get
            {
                // Argument must be within bounds (collection must be non-empty)
                Contract.Requires(0 <= index && index < Count); // TODO: Use <IndexOutOfRangeException>?


                // Result is non-null
                // Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

                // Result is the same as skipping the first index items
                Contract.Ensures(Contract.Result<T>().Equals(this.Skip(index).First()));


                throw new NotImplementedException();
            }
        }


        public void Enqueue(T item)
        {
            // Argument must be non-null
            // Contract.Requires(AllowsNull || item != null); // TODO: Use <ArgumentNullException>?

            // Collection must be non-read-only
            Contract.Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true); // TODO: IsReadOnly is a IExtensible<T> property, which IQueue doesn't inherit from!


            // Adding an item makes the collection non-empty
            Contract.Ensures(!IsEmpty);

            // The collection will contain the item added
            Contract.Ensures(this.Contains(item)); // TODO: Use EqualityComparer?

            // Adding an item increments the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) + 1);

            // Adding the item increments the number of equal items by one
            Contract.Ensures(this.Count(x => x.Equals(item)) == Contract.OldValue(this.Count(x => x.Equals(item))) + 1); // TODO: Use EqualityComparer?

            // The added item is at the end of the queue
            Contract.Ensures(this.SequenceEqual(Contract.OldValue(this.ToArray()).Append(item)));

            // The item is added to the end
            Contract.Ensures(item.Equals(this.Last()));


            throw new NotImplementedException();
        }


        public T Dequeue()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?

            // Collection must be non-read-only
            Contract.Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true); // TODO: IsReadOnly is a IExtensible property, which IQueue doesn't inherit from!


            // Dequeuing an item decrements the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);

            // Result is non-null
            // Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // Result is the same the first items
            Contract.Ensures(Contract.Result<T>().Equals(Contract.OldValue(this.First())));

            // Only the first item in the queue is removed
            Contract.Ensures(Enumerable.SequenceEqual(this, Contract.OldValue(this.Skip(1))));


            throw new NotImplementedException();
        }


        #region Non-Contract Methods

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { throw new NotImplementedException(); }
        IDirectedCollectionValue<T> IDirectedCollectionValue<T>.Backwards() { throw new NotImplementedException(); }
        public abstract EnumerationDirection Direction { get; }
        public abstract string ToString(string format, IFormatProvider formatProvider);
        public abstract bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider);
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
        public abstract T Choose();
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract T[] ToArray();

        #endregion
    }
}