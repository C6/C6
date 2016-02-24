// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

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
    }


    // TODO: Ensure that the collection have the used events as active
    [ContractClassFor(typeof(IStack<>))]
    internal abstract class IStackContract<T> : IStack<T>
    {
        // ReSharper disable InvocationIsSkipped

        public T this[int index]
        {
            get
            {
                // Argument must be within bounds (collection must be non-empty)
                Requires(0 <= index); // TODO: Use <IndexOutOfRangeException>?
                Requires(index < Count); // TODO: Use <IndexOutOfRangeException>?


                // Result is non-null
                Ensures(AllowsNull || Result<T>() != null);

                // Result is the same as skipping the first index items
                Ensures(Result<T>().Equals(this.Skip(index).First()));


                return default(T);
            }
        }

        public T Pop()
        {
            // Collection must be non-empty
            Requires(!IsEmpty); // TODO: Use <NoSuchItemException>?

            // Collection must be non-read-only
            Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true); // TODO: IsReadOnly is a IExtensible property, which IQueue doesn't inherit from!


            // Dequeuing an item decreases the count by one
            Ensures(Count == OldValue(Count) - 1);

            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Result is the same the first items
            Ensures(Result<T>().Equals(OldValue(this.Last())));

            // Only the last item in the queue is removed
            Ensures(this.SequenceEqual(OldValue(this.Take(Count - 1).ToList())));


            return default(T);
        }

        public void Push(T item)
        {
            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null); // TODO: Use <ArgumentNullException>?

            // Collection must be non-read-only
            Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true); // TODO: IsReadOnly is a IExtensible<T> property, which IQueue doesn't inherit from!


            // The collection becomes non-empty
            Ensures(!IsEmpty);

            // The collection will contain the item added
            Ensures(this.Contains(item)); // TODO: Use EqualityComparer?

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + 1);

            // Adding the item increases the number of equal items by one
            Ensures(this.Count(x => x.Equals(item)) == OldValue(this.Count(x => x.Equals(item))) + 1); // TODO: Use EqualityComparer?

            // The added item is at the end of the queue
            Ensures(this.SequenceEqual(OldValue(this.ToList()).Append(item)));

            // The item is added to the end
            Ensures(item.Equals(this.Last()));


            return;
        }

        #region Hardened Postconditions

        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public EventTypes ListenableEvents
        {
            get
            {
                // No extra Requires allowed


                // The events raised by the collection must be listenable
                Ensures(Result<EventTypes>().HasFlag(Changed | Added | Removed | Inserted | RemovedAt));


                return default(EventTypes);
            }
        }

        #endregion

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        #region SCG.IEnumerable<T>

        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IShowable

        public abstract string ToString(string format, IFormatProvider formatProvider);
        public abstract bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);

        #endregion

        #region ICollectionValue<T>

        public abstract EventTypes ActiveEvents { get; }
        public abstract bool AllowsNull { get; }
        public abstract int Count { get; }
        public abstract Speed CountSpeed { get; }
        public abstract bool IsEmpty { get; }
        public abstract T Choose();
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract T[] ToArray();
        public abstract event EventHandler CollectionChanged;
        public abstract event EventHandler<ClearedEventArgs> CollectionCleared;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemInserted;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsAdded;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;

        #endregion

        #region IDirectedEnumerable<T>

        public abstract EnumerationDirection Direction { get; }
        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() => default(IDirectedEnumerable<T>);

        #endregion

        #region IDirectedCollectionValue<T>

        public abstract IDirectedCollectionValue<T> Backwards();

        #endregion

        #endregion
    }
}