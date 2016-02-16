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
    // This is mainly the intersection of the main stream generic collection
    // interfaces and the priority queue interface, ICollection<T> and IPriorityQueue<T>.
    /// <summary>
    /// Represents a generic collection to which one may add items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [ContractClass(typeof(IExtensibleContract<>))]
    public interface IExtensible<T> : ICollectionValue<T>
    {
        // TODO: Move to ICollectionValue?
        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <value><c>true</c> if the collection is read-only;
        /// otherwise, <c>false</c>.</value>
        /// <remarks>A collection that is read-only does not allow the addition
        /// or removal of items after the collection is created. Note that 
        /// read-only in this context does not indicate whether individual 
        /// items of the collection can be modified.</remarks>
        [Pure]
        bool IsReadOnly { get; }


        // TODO: Which one does it use, when there is a IComparer as well?!
        /// <summary>
        /// Gets a value indicating whether the collection allows duplicates.
        /// </summary>
        /// <value><c>true</c> if the collection allows duplicates;
        /// otherwise, <c>false</c>.</value>
        /// <remarks>
        /// <c>true</c> if the collection has bag semantics and may contain two
        /// items that are duplicate, i.e. equal by the collection's comparer
        /// or equality comparer. Otherwise <c>false</c>, in which case the
        /// collection has set semantics.
        /// </remarks>
        [Pure]
        bool AllowsDuplicates { get; }


        /// <summary>
        /// Gets a value indicating whether the collection only stores an item
        /// once and keeps track of duplicates using a counter.
        /// </summary>
        /// <value><c>true</c> if only one representative of a group of equal 
        /// items is kept in the collection together with a counter;
        /// <c>false</c> if each item is stored explicitly.</value>
        /// <remarks>Is by convention always <c>true</c> for collections with
        /// set semantics.</remarks>
        [Pure]
        bool DuplicatesByCounting { get; }

        /*
        // TODO: Add this? Update contracts on methods.
        // TODO: Move to ICollectionValue?
        /// <summary>
        /// Gets a value indicating whether the collection allows <c>null</c> 
        /// items.
        /// </summary>
        /// <value><c>true</c>, if the collection allows items that are
        /// <c>null</c>; otherwise, <c>false</c>.</value>
        [Pure]
        bool AllowsNull { get; }
        */


        // TODO: wonder where the right position of this is. And the semantics. Should at least be in the same class as AllowsDuplicates!
        // TODO: Could the result be null?
        /// <summary>
        /// Gets the <see cref="SCG.IEqualityComparer{T}"/> used by the collection.
        /// </summary>
        /// <value>The <see cref="SCG.IEqualityComparer{T}"/> used by the collection.</value>
        [Pure]
        SCG.IEqualityComparer<T> EqualityComparer { get; }


        // TODO: Should we allow/disallow null values generally? Seems only to be a problem with hash-based collections.
        /// <summary>
        /// Adds an item to the collection if possible.
        /// </summary>
        /// <param name="item">The item to add to the collection.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns><c>true</c> if item was added;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>If the collection has set semantics, the item will be
        /// added if not already in the collection. If bag semantics, the item 
        /// will always be added. The collection's
        /// <see cref="EqualityComparer"/> is used to determine item equality.
        /// </para>
        /// <para>If the item is added, it raises the following events (in that 
        /// order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the added item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        bool Add(T item);


        // TODO: Rename to AddRange?
        /// <summary>
        /// Adds each item of the specified enumerable to the collection, if 
        /// possible, in enumeration order.
        /// </summary>
        /// <param name="items">The enumerable whose items should be added to
        /// the collection. <c>null</c> is allowed for nullable items, but not 
        /// for the enumerable itself.</param>
        /// <remarks>
        /// <para>If the collection has set semantics, each item will be added
        /// if not already in the collection. If bag semantics, the items will
        /// always be added. The collection's <see cref="EqualityComparer"/> is
        /// used to determine item equality.</para>
        /// <para>This is equivalent to
        /// <c>foreach (var item in coll) { coll.Add(item); }</c>, but might be
        /// more efficient and it only raises the event 
        /// <see cref="ICollectionValue{T}.CollectionChanged"/> once.</para>
        /// <para>If any items are added, it raises the following events (in 
        /// that order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> once for each item 
        /// added (using a count of one).
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/> once at the end.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        void AddAll(SCG.IEnumerable<T> items);
    }



    [ContractClassFor(typeof(IExtensible<>))]
    internal abstract class IExtensibleContract<T> : IExtensible<T>
    {
        // ReSharper disable InvocationIsSkipped

        // Contracts are copied to ICollection<T>.IsReadOnly. Keep both updated!
        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public bool AllowsDuplicates {
            get {
                // A set only contains distinct items // TODO: Is this the right place to put it?
                Contract.Ensures(Contract.Result<bool>() || Count == this.Distinct(EqualityComparer).Count());


                throw new NotImplementedException();
            }
        }


        public bool DuplicatesByCounting {
            get {
                // No Requires


                // True by convention for collections with set semantics
                Contract.Ensures(AllowsDuplicates || Contract.Result<bool>()); // TODO: Replace with Contract.Requires(AllowsDuplicates)? Update documentation accordingly!


                throw new NotImplementedException();
            }
        }


        public SCG.IEqualityComparer<T> EqualityComparer {
            get {
                // No Requires


                // Result is non-null
                Contract.Ensures(Contract.Result<SCG.IEqualityComparer<T>>() != null);


                throw new NotImplementedException();
            }
        }


        // Contracts are copied to ICollection<T>.Add. Keep both updated!
        public bool Add(T item)
        {
            // Collection must be non-read-only
            Contract.Requires(!IsReadOnly); // TODO: Use <ReadOnlyCollectionException>?

            // Argument must be non-null
            // Contract.Requires(AllowsNull || item != null); // TODO: Use <ArgumentNullException>?


            // Returns true if bag semantic, otherwise the opposite of whether the collection already contained the item
            Contract.Ensures(AllowsDuplicates ? Contract.Result<bool>() : !Contract.OldValue(this.Contains(item, EqualityComparer)));

            // The collection becomes non-empty
            Contract.Ensures(!IsEmpty);

            // The collection will contain the item added
            Contract.Ensures(this.Contains(item, EqualityComparer));

            // Adding an item increases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) + (Contract.Result<bool>() ? 1 : 0));

            // Adding the item increases the number of equal items by one
            Contract.Ensures(this.Count(x => EqualityComparer.Equals(x, item)) == Contract.OldValue(this.Count(x => EqualityComparer.Equals(x, item))) + (Contract.Result<bool>() ? 1 : 0));


            throw new NotImplementedException();
        }


        public void AddAll(SCG.IEnumerable<T> items)
        {
            // Collection must be non-read-only
            Contract.Requires(!IsReadOnly); // TODO: Use <ReadOnlyCollectionException>?

            // Argument must be non-null
            Contract.Requires(items != null); // TODO: Use <ArgumentNullException>?

            // All items must be non-null
            // Contract.Requires(AllowsNull || Contract.ForAll(items, item => item != null)); // TODO: Use <ArgumentNullException>?


            // The collection becomes non-empty
            Contract.Ensures(!IsEmpty);

            // The collection will contain the items added
            Contract.Ensures(Contract.ForAll(items, item => this.Contains(item, EqualityComparer)));

            // Count can never decrement
            Contract.Ensures(items.Any() ? Count <= Contract.OldValue(Count) : Count == Contract.OldValue(Count));

            // TODO: Make more exact check of added items


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
        public abstract T Choose();
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract T[] ToArray();

        #endregion
    }
}