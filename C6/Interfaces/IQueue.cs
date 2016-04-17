// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractHelperExtensions;
using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: decide if this should extend ICollection/IExtensible - it at least needs IsReadOnly
    /// <summary>
    ///     Represents a generic first-in-first-out (FIFO) queue that also supports indexing.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of items in the stack.
    /// </typeparam>
    [ContractClass(typeof(IQueueContract<>))]
    public interface IQueue<T> : IDirectedCollectionValue<T>
    {
        // Also found in IStack<T>
        /// <summary>
        ///     Gets the item at the specified index in the queue. The beginning of the queue has index zero.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index of the item to get.
        /// </param>
        /// <returns>
        ///     The item at the specified index.
        /// </returns>
        [Pure]
        T this[int index] { get; }

        /// <summary>
        ///     Removes and returns the item at the beginning of the queue.
        /// </summary>
        /// <returns>
        ///     The item that is removed from the beginning of the queue.
        /// </returns>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and an index of zero.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemsRemoved"/> with the item and a count of one.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        T Dequeue();

        /// <summary>
        ///     Adds an item to the end of the queue.
        /// </summary>
        /// <param name="item">
        ///     The item to add to the queue. <c>null</c> is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/> is
        ///     <c>true</c>.
        /// </param>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemInserted"/> with the item and an index of <c>Count</c> - 1.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemsAdded"/> with the item and a count of one.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        void Enqueue(T item);

        // TODO: Add Peek?
    }


    [ContractClassFor(typeof(IQueue<>))]
    internal abstract class IQueueContract<T> : IQueue<T>
    {
        // ReSharper disable InvocationIsSkipped

        public T this[int index]
        {
            get {
                // Argument must be within bounds (collection must be non-empty)
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);


                // Result is non-null
                Ensures(AllowsNull || Result<T>() != null);

                // Result is the same as skipping the first index items
                Ensures(Result<T>().IsSameAs(this.Skip(index).First()));


                return default(T);
            }
        }

        public T Dequeue()
        {
            // Collection must be non-empty
            Requires(!IsEmpty, CollectionMustBeNonEmpty);

            // Collection must be non-read-only
            Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true, CollectionMustBeNonReadOnly); // TODO: IsReadOnly is a IExtensible property, which IQueue doesn't inherit from!


            // Dequeuing an item decreases the count by one
            Ensures(Count == OldValue(Count) - 1);

            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Result is the same the first items
            Ensures(Result<T>().IsSameAs(OldValue(this.First())));

            // Only the first item in the queue is removed
            Ensures(this.IsSameSequenceAs(OldValue(this.Skip(1).ToList())));


            return default(T);
        }

        public void Enqueue(T item)
        {
            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);

            // Collection must be non-read-only
            Requires(!(this as IExtensible<T>)?.IsReadOnly ?? true, CollectionMustBeNonReadOnly); // TODO: IsReadOnly is a IExtensible<T> property, which IQueue doesn't inherit from!


            // The added item is at the end of the queue
            Ensures(this.IsSameSequenceAs(OldValue(ToArray()).Append(item)));

            // The collection becomes non-empty
            Ensures(!IsEmpty);

            // The collection will contain the item added
            Ensures(this.ContainsSame(item));

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + 1);

            // Adding the item increases the number of equal items by one
            Ensures(this.ContainsSameCount(item) == OldValue(this.ContainsSameCount(item)) + 1);

            // The item is added to the end
            Ensures(item.IsSameAs(this.Last()));


            return;
        }

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
        public abstract EventTypes ListenableEvents { get; }
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