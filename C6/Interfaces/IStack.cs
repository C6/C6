// This file is part of the C6 Generic Collection Library for C# and CLI
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
    // TODO: decide if this should extend ICollection/IExtensible - it at least needs IsReadOnly
    /// <summary>
    /// Represents a generic last-in-first-out (LIFO) stack data structure.
    /// </summary>
    /// <typeparam name="T">The type of items in the stack.</typeparam>
    [ContractClass(typeof(IStackContract<>))]
    public interface IStack<T> : IDirectedCollectionValue<T>
    {
        // TODO: Document events!
        // Also found in IQueue<T>
        /// <summary>
        /// Gets the item at the specified index in the stack.
        /// The bottom of the stack has index <c>0</c>.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get.</param>
        /// <returns>The item at the specified index.</returns>
        [Pure]
        T this[int index] { get; }


        /// <summary>
        /// Inserts an item at the top of the stack.
        /// </summary>
        /// <param name="item">The item to push onto the stack. <c>null</c> is
        /// allowed for nullable items.</param>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
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
        void Push(T item);


        /// <summary>
        /// Removes and returns the item at the top of the stack.
        /// </summary>
        /// <returns>The item removed from the top of the stack.</returns>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and an 
        /// index of <c>coll.Count - 1</c>.
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
        T Pop();
    }



    // TODO: Ensure that the collection have the used events as active
    [ContractClassFor(typeof(IStack<>))]
    internal abstract class IStackContract<T> : IStack<T>
    {
        // ReSharper disable InvocationIsSkipped

        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public EventTypes ListenableEvents {
            get {
                // No extra Requires allowed


                // The events raised by the collection must be listenable
                Contract.Ensures(Contract.Result<EventTypes>().HasFlag(Changed | Added | Removed | Inserted | RemovedAt));


                throw new NotImplementedException();
            }
        }


        public T this[int index] {
            get {
                // Argument must be within bounds (collection must be non-empty)
                Contract.Requires(0 <= index); // TODO: Use <IndexOutOfRangeException>?
                Contract.Requires(index < Count); // TODO: Use <IndexOutOfRangeException>?


                // Result is non-null
                Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

                // Result is the same as skipping the first index items
                Contract.Ensures(Contract.Result<T>().Equals(this.Skip(index).First()));


                throw new NotImplementedException();
            }
        }


        public void Push(T item)
        {
            // Argument must be non-null if collection disallows null values
            Contract.Requires(AllowsNull || item != null); // TODO: Use <ArgumentNullException>?
            
            // Collection must be non-read-only
            Contract.Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true); // TODO: IsReadOnly is a IExtensible<T> property, which IQueue doesn't inherit from!


            // The collection becomes non-empty
            Contract.Ensures(!IsEmpty);

            // The collection will contain the item added
            Contract.Ensures(this.Contains(item)); // TODO: Use EqualityComparer?

            // Adding an item increases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) + 1);

            // Adding the item increases the number of equal items by one
            Contract.Ensures(this.Count(x => x.Equals(item)) == Contract.OldValue(this.Count(x => x.Equals(item))) + 1); // TODO: Use EqualityComparer?

            // The added item is at the end of the queue
            Contract.Ensures(this.SequenceEqual(Contract.OldValue(this.ToList()).Append(item)));

            // The item is added to the end
            Contract.Ensures(item.Equals(this.Last()));


            throw new NotImplementedException();
        }


        public T Pop()
        {
            // Collection must be non-empty
            Contract.Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?

            // Collection must be non-read-only
            Contract.Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true); // TODO: IsReadOnly is a IExtensible property, which IQueue doesn't inherit from!


            // Dequeuing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);

            // Result is non-null
            Contract.Ensures(AllowsNull || Contract.Result<T>() != null);

            // Result is the same the first items
            Contract.Ensures(Contract.Result<T>().Equals(Contract.OldValue(this.Last())));

            // Only the last item in the queue is removed
            Contract.Ensures(this.SequenceEqual(Contract.OldValue(this.Take(Count).ToList())));


            throw new NotImplementedException();
        }


        // ReSharper restore InvocationIsSkipped


        #region Non-Contract Methods

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { throw new NotImplementedException(); }
        IDirectedCollectionValue<T> IDirectedCollectionValue<T>.Backwards() { throw new NotImplementedException(); }
        public abstract EnumerationDirection Direction { get; }
        public abstract string ToString(string format, IFormatProvider formatProvider);
        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);
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

        #endregion
    }
}