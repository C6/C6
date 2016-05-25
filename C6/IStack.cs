// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractHelperExtensions;
using static C6.Contracts.ContractMessage;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: decide if this should extend ICollection/IExtensible - it at least needs IsReadOnly
    /// <summary>
    ///     Represents a generic last-in-first-out (LIFO) stack that also supports indexing.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of items in the stack.
    /// </typeparam>
    [ContractClass(typeof(IStackContract<>))]
    public interface IStack<T> : IDirectedCollectionValue<T>, IListenable<T>
    {
        // Also found in IQueue<T>
        /// <summary>
        ///     Gets the item at the specified index in the stack. The bottom of the stack has index zero.
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
        ///     Removes and returns the item at the top of the stack.
        /// </summary>
        /// <returns>
        ///     The item removed from the top of the stack.
        /// </returns>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="IListenable{T}.ItemRemovedAt"/> with the item and an index of <c>Count</c> - 1.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="IListenable{T}.ItemsRemoved"/> with the item and a count of one.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="IListenable{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        T Pop();

        /// <summary>
        ///     Inserts an item at the top of the stack.
        /// </summary>
        /// <param name="item">
        ///     The item to push onto the stack. <c>null</c> is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/> is
        ///     <c>true</c>.
        /// </param>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="IListenable{T}.ItemInserted"/> with the item and an index of <c>Count</c> - 1.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="IListenable{T}.ItemsAdded"/> with the item and a count of one.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="IListenable{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
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
            get {
                // Argument must be within bounds
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);


                // Result is non-null
                Ensures(AllowsNull || Result<T>() != null);

                // Result is the same as skipping the first index items
                Ensures(Result<T>().IsSameAs(this.ElementAt(index)));


                return default(T);
            }
        }

        public T Pop()
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
            Ensures(Result<T>().IsSameAs(OldValue(this.Last())));

            // Only the last item in the queue is removed
            Ensures(this.IsSameSequenceAs(OldValue(this.Take(Count - 1).ToList())));


            return default(T);
        }

        public void Push(T item)
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
            Ensures(this.CountSame(item) == OldValue(this.CountSame(item)) + 1);

            // The item is added to the end
            Ensures(item.IsSameAs(this.Last()));
            

            return;
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

        #region IListenable<T>

        public abstract EventTypes ActiveEvents { get; }
        public abstract EventTypes ListenableEvents { get; }
        public abstract event EventHandler CollectionChanged;
        public abstract event EventHandler<ClearedEventArgs> CollectionCleared;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemInserted;
        public abstract event EventHandler<ItemAtEventArgs<T>> ItemRemovedAt;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsAdded;
        public abstract event EventHandler<ItemCountEventArgs<T>> ItemsRemoved;

        #endregion

        #region IDirectedCollectionValue<T>

        public abstract EnumerationDirection Direction { get; }
        public abstract IDirectedCollectionValue<T> Backwards();

        #endregion

        #endregion
    }
}