// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: Setup contracts to avoid exceptions?
    /// <summary>
    /// Represents an indexed, sequenced generic collection where item order is
    /// determined by insertion and removal order.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [ContractClass(typeof(IListContract<>))]
    public interface IList<T> : IIndexed<T>, SCG.IList<T>, IList
    {
        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        /// <value>The number of items contained in the collection.</value>
        [Pure]
        new int Count { get; }

        /// <summary>
        /// Gets the first item in the list.
        /// </summary>
        /// <value>The first item in this list.</value>
        [Pure]
        T First { get; }

        // TODO: Better name?
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Remove"/>
        /// removes an item from the beginning or from the end of the list.
        /// </summary>
        /// <value><c>true</c> if <see cref="Remove"/> removes an item from the
        /// beginning of the list; <c>false</c> if it removes an item from the
        /// end.</value>
        /// <remarks>
        /// <see cref="ICollection{T}.Add"/> always adds items to the end of
        /// the list.
        /// </remarks>
        /// <seealso cref="Remove"/>
        [Pure]
        bool IsFifo { get; set; }

        /// <summary>
        /// Gets a value indicating whether the list has a fixed size.
        /// </summary>
        /// <value><c>true</c> if the list has a fixed size;
        /// otherwise, <c>false</c>.</value>
        /// <remarks>
        /// <para>
        /// A list with a fixed size does not allow operations that changes the
        /// list's size.
        /// </para>
        /// <para>
        /// Any list that is read-only (<see cref="IsReadOnly"/> is
        /// <c>true</c>), has a fixed size; the opposite need not be true.
        /// </para>
        /// </remarks>
        [Pure]
        new bool IsFixedSize { get; }

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
        new bool IsReadOnly { get; }

        /// <summary>
        /// Gets the last item in the list.
        /// </summary>
        /// <value>The last item in the list.</value>
        [Pure]
        T Last { get; }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get or set.</param>
        /// <value>The item at the specified index.</value>
        /// <remarks>
        /// The setter raises the following events (in that order) with the
        /// collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the removed 
        /// item and the index.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemInserted"/> with the inserted 
        /// item and the index.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the inserted item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        [IndexerName("Item")]
        new T this[int index]
        {
            [Pure]
            get;
            set;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <remarks>
        /// If the collection is non-empty, it raises the following events (in
        /// that order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionCleared"/> as full and 
        /// with count equal to the collection count.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        new void Clear();

        // TODO: Include index in predicate?
        // TODO: Should this be deprecated?
        // TODO: Copy docs from Enumerable.Where?
        /// <summary>
        /// Creates a new list consisting of the items in this list satisfying
        /// a specified predicate.
        /// </summary>
        /// <param name="predicate">A delegate that returns <c>true</c> for the
        /// items that should be included in the new list.</param>
        /// <returns>A new list containing the items that satisfy the
        /// predicate.</returns>
        [Pure]
        IList<T> FindAll(Func<T, bool> predicate);

        /// <summary>
        /// Searches from the beginning of the collection for the specified
        /// item and returns the zero-based index of the first occurrence
        /// within the collection. 
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns>The zero-based index of the first occurrence of item
        /// within the entire collection, if found; otherwise, a negative
        /// number that is the bitwise complement of the index at which
        /// <see cref="ICollection{T}.Add"/> would put the item.</returns>
        [Pure]
        new int IndexOf(T item);

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be
        /// inserted.</param>
        /// <param name="item">The item to insert into the list.</param>
        /// <returns><c>true</c> if item was added;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="index"/> equals the number of items in the list,
        /// then value is appended to the end of the list. This has the same
        /// effect as calling <see cref="ICollection{T}.Add"/>, though the
        /// events raised are different.
        /// </para>
        /// <para>
        /// When inserting, the items that follow the insertion point move down
        /// to accommodate the new item. The indices of the items that are
        /// moved are also updated.
        /// </para>
        /// <para>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemInserted"/> with the inserted 
        /// item and the index.
        /// </description></item>
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
        new bool Insert(int index, T item);

        /// <summary>
        /// Inserts the items of a collection into the list starting at the
        /// specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new items
        /// should be inserted.</param>
        /// <param name="items">The enumerable whose items should be inserted
        /// into the list. The enumerable itself cannot be <c>null</c>, but it
        /// can contain items that are null, if type <typeparamref name="T"/>
        /// is a reference type and the collection allows it.</param>
        /// <remarks>
        /// If any items are added, it raises the following events (in 
        /// that order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemInserted"/> once for each item
        /// and the index at which is was inserted.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> once for each item 
        /// added (using a count of one).
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/> once at the
        /// end.
        /// </description></item>
        /// </list>
        /// </remarks>
        void InsertAll(int index, SCG.IEnumerable<T> items);

        /// <summary>
        /// Inserts an item at the beginning of the list.
        /// </summary>
        /// <param name="item">The item to insert at the beginning of the list.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns><c>true</c> if item was inserted;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the item is inserted, it raises the following events (in that
        /// order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemInserted"/> with the item and an 
        /// index of <c>0</c>.
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
        bool InsertFirst(T item);

        /// <summary>
        /// Inserts an item at the end of the list.
        /// </summary>
        /// <param name="item">The item to insert at the end of the list.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns><c>true</c> if item was inserted;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the item is inserted, it raises the following events (in that
        /// order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemInserted"/> with the item and an 
        /// index of <c>Count</c>.
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
        bool InsertLast(T item);

        /// <summary>
        /// Determines whether the list is sorted in non-descending order
        /// according to the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">The default comparer
        /// <see cref="SCG.Comparer{T}.Default"/> cannot find an implementation
        /// of the <see cref="IComparable{T}"/> generic interface or the
        /// <see cref="IComparable"/> interface for type
        /// <typeparamref name="T"/>.</exception>
        /// <returns><c>true</c> if the list is sorted in non-descending order;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        bool IsSorted();

        /// <summary>
        /// Determines whether the list is sorted in non-descending order
        /// according to the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="SCG.IComparer{T}"/>
        /// implementation to use when comparing items, or <c>null</c> to use
        /// the default comparer <see cref="SCG.Comparer{T}.Default"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is <c>null</c>, and the default
        /// comparer <see cref="SCG.Comparer{T}.Default"/> cannot find an
        /// implementation of the <see cref="IComparable{T}"/> generic
        /// interface or the <see cref="IComparable"/> interface for type
        /// <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentException">The implementation of
        /// <paramref name="comparer"/> caused an error during the sort. For
        /// example, <paramref name="comparer"/> might not return 0 when
        /// comparing an item with itself.</exception>
        /// <returns><c>true</c> if the list is sorted in non-descending order;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        bool IsSorted(SCG.IComparer<T> comparer);

        /// <summary>
        /// Determines whether the list is sorted in non-descending order
        /// according to the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use
        /// when comparing elements.</param>
        /// <exception cref="ArgumentException">The implementation of 
        /// <paramref name="comparison"/> caused an error during the sort. For
        /// example, <paramref name="comparison"/> might not return 0 when
        /// comparing an item with itself.</exception>
        /// <returns><c>true</c> if the list is sorted in non-descending order;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        bool IsSorted(Comparison<T> comparison);

        // TODO: Deprecate?
        /// <summary>
        /// Creates a new list consisting of the results of mapping all items
        /// in this list using the specified mapper. The new list will use the
        /// default equality comparer for type <typeparamref name="V"/>.
        /// </summary>
        /// <typeparam name="V">The type of the items in the new list.
        /// </typeparam>
        /// <param name="mapper">A function that maps each item in this list to
        /// an item in the new list.
        /// </param>
        /// <returns>An new list whose items are the results of mapping all
        /// items in this list.</returns>
        [Pure]
        IList<V> Map<V>(Func<T, V> mapper);

        // TODO: Deprecate?
        /// <summary>
        /// Creates a new list consisting of the results of mapping all items
        /// in this list using the specified mapper. The new list will use the
        /// specified equality comparer.
        /// </summary>
        /// <typeparam name="V">The type of the items in the new list.
        /// </typeparam>
        /// <param name="mapper">A function that maps each item in this list to
        /// an item in the new list.
        /// </param>
        /// <param name="equalityComparer">The
        /// <see cref="SCG.IEqualityComparer{T}"/> to use for the new list.</param>
        /// <returns>An new list whose items are the results of mapping all
        /// items in this list.</returns>
        [Pure]
        IList<V> Map<V>(Func<T, V> mapper, SCG.IEqualityComparer<V> equalityComparer);

        /// <summary>
        /// Removes and returns an item from either the beginning or the end of
        /// the list, depending on the value of <see cref="IsFifo"/>.
        /// </summary>
        /// <returns>The item removed from the list.</returns>
        /// <remarks>
        /// <para>
        /// If <see cref="IsFifo"/> is <c>true</c>, this methods removes an 
        /// item from the beginning of the list; if <see cref="IsFifo"/> is
        /// <c>false</c>, it removes an item from the end of the list.
        /// </para>
        /// <para>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and
        /// an index of either <c>0</c> if <see cref="IsFifo"/> is <c>true</c>,
        /// or <c>Count - 1</c> if <see cref="IsFifo"/> is <c>false</c>.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso cref="IsFifo"/>
        T Remove();

        /// <summary>
        /// Removes and returns the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <returns>The item removed from the collection.</returns>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the removed 
        /// item and the index.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        new T RemoveAt(int index);

        /// <summary>
        /// Removes and returns an item from the beginning of the list.
        /// </summary>
        /// <returns>The item removed from the beginning of the list.</returns>
        /// <remarks>
        /// <para>
        /// The methods <see cref="ICollection{T}.Add"/> and
        /// <see cref="RemoveFirst"/> together behave like a first-in-first-out
        /// queue.
        /// </para>
        /// <para>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and
        /// an index of zero.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        T RemoveFirst();

        /// <summary>
        /// Removes and returns an item from the end of the list.
        /// </summary>
        /// <returns>The item removed from the end of the list.</returns>
        /// <remarks>
        /// <para>
        /// The methods <see cref="ICollection{T}.Add"/> and
        /// <see cref="RemoveLast"/> together behave like a last-in-first-out
        /// stack. 
        /// </para>
        /// <para>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and
        /// an index of <c>Count - 1</c>.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        T RemoveLast();

        /// <summary>
        /// Reverses the sequence order of the items in the list.
        /// </summary>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void Reverse();

        /// <summary>
        /// Randomly shuffles the items in the list.
        /// </summary>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void Shuffle();

        /// <summary>
        /// Shuffles the items in the list according to the specified random
        /// source.
        /// </summary>
        /// <param name="random">The random source.</param>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void Shuffle(Random random);

        /// <summary>
        /// Sorts the items in the list using the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">The default comparer
        /// <see cref="SCG.Comparer{T}.Default"/> cannot find an implementation
        /// of the <see cref="IComparable{T}"/> generic interface or the
        /// <see cref="IComparable"/> interface for type
        /// <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void Sort();

        /// <summary>
        /// Sorts the items in the list using the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="SCG.IComparer{T}"/>
        /// implementation to use when comparing items, or <c>null</c> to use
        /// the default comparer <see cref="SCG.Comparer{T}.Default"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is <c>null</c>, and the default
        /// comparer <see cref="SCG.Comparer{T}.Default"/> cannot find an
        /// implementation of the <see cref="IComparable{T}"/> generic
        /// interface or the <see cref="IComparable"/> interface for type
        /// <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentException">The implementation of
        /// <paramref name="comparer"/> caused an error during the sort. For
        /// example, <paramref name="comparer"/> might not return 0 when
        /// comparing an item with itself.</exception>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void Sort(SCG.IComparer<T> comparer);

        /// <summary>
        /// Sorts the items in the list using the specified
        /// <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use
        /// when comparing elements.</param>
        /// <exception cref="ArgumentException">The implementation of 
        /// <paramref name="comparison"/> caused an error during the sort. For
        /// example, <paramref name="comparison"/> might not return 0 when
        /// comparing an item with itself.</exception>
        /// <remarks>
        /// Raises the following events (in that order) with the collection as
        /// sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void Sort(Comparison<T> comparison);
    }


    [ContractClassFor(typeof(IList<>))]
    internal abstract class IListContract<T> : IList<T>
    {
        // ReSharper disable InvocationIsSkipped

        // Contracts are copied from IIndexed<T>.Count. Keep both updated!
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

        public T First
        {
            get
            {
                // Collection must be non-empty
                Requires(!IsEmpty, CollectionMustBeNonEmpty);


                // Equals first item
                Ensures(Result<T>().Equals(this[0]));
                Ensures(Result<T>().Equals(this.First()));


                return default(T);
            }
        }

        public bool IsFifo
        {
            get { return default(bool); }
            set
            {
                // Collection must be non-read-only
                Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


                // Value is updated
                Ensures(IsFifo == value);


                return;
            }
        }

        // Contracts are copied from IExtensible<T>.IsReadOnly. Keep both updated!
        public bool IsFixedSize
        {
            get
            {
                // No preconditions


                // Read-only list has fixed size
                Ensures(!IsReadOnly || Result<bool>());


                return default(bool);
            }
        }

        // Contracts are copied from ICollection<T>.IsReadOnly. Keep both updated!
        public bool IsReadOnly
        {
            get { return default(bool); }
        }

        public T Last
        {
            get
            {
                // Collection must be non-empty
                Requires(!IsEmpty, CollectionMustBeNonEmpty);


                // Equals first item
                Ensures(Result<T>().Equals(this[Count - 1]));
                Ensures(Result<T>().Equals(this.Last()));


                return default(T);
            }
        }

        public T this[int index]
        {
            get
            {
                // Argument must be within bounds
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);


                // Result is the same as skipping the first index items
                Ensures(Result<T>().Equals(this.Skip(index).First()));


                return default(T);
            }
            set
            {
                // Collection must be non-read-only
                Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

                // Argument must be non-null if collection disallows null values
                Requires(AllowsNull || value != null, ArgumentMustBeNonNull);

                // Argument must be within bounds
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);

                // Collection must not already contain item if collection disallows duplicate values
                Requires(AllowsDuplicates || !Contains(value), CollectionMustAllowDuplicates); // TODO: Behavior mismatch with Insert methods


                // Value is the same as skipping the first index items
                Ensures(value.Equals(this.Skip(index).First()));


                return;
            }
        }

        // Contracts are copied from ICollection<T>.Clear. Keep both updated!
        public void Clear()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);


            // The collection becomes empty
            Ensures(IsEmpty);
            Ensures(Count == 0);
            Ensures(!this.Any());


            return;
        }

        public IList<T> FindAll(Func<T, bool> predicate)
        {
            // Argument must be non-null
            Requires(predicate != null, ArgumentMustBeNonNull);


            // The result is equal to filtering this list based on the predicate
            Ensures(Result<IList<T>>().SequenceEqual(this.Where(predicate)));


            return default(IList<T>);
        }

        // Contracts are copied from IIndexed<T>.IndexOf. Keep both updated!
        public int IndexOf(T item)
        {
            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Result is a valid index
            Ensures(Contains(item)
                ? 0 <= Result<int>() && Result<int>() < Count
                : 0 <= ~Result<int>() && ~Result<int>() <= Count);

            // Item at index equals item
            Ensures(Result<int>() < 0 || EqualityComparer.Equals(item, this[Result<int>()]));

            // No item before index equals item
            Ensures(Result<int>() < 0 || !this.Take(Result<int>()).Contains(item, EqualityComparer));


            return default(int);
        }

        public bool Insert(int index, T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be within bounds
            Requires(0 <= index, ArgumentMustBeWithinBounds);
            Requires(index <= Count, ArgumentMustBeWithinBounds);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Returns true if bag semantic, otherwise the opposite of whether the collection already contained the item
            Ensures(AllowsDuplicates ? Result<bool>() : !OldValue(this.Contains(item, EqualityComparer)));

            // Item is inserted at index
            Ensures(item.Equals(this[index]));

            // The item is inserted into the list without replacing other items
            Ensures(this.SequenceEqual(OldValue(this.Take(index).Append(item).Concat(this.Skip(index)).ToList())));


            return default(bool);
        }

        public void InsertAll(int index, SCG.IEnumerable<T> items)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be within bounds
            Requires(0 <= index, ArgumentMustBeWithinBounds);
            Requires(index < Count, ArgumentMustBeWithinBounds);

            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);


            // TODO: Ensures


            return;
        }

        public bool InsertFirst(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Returns true if bag semantic, otherwise the opposite of whether the collection already contained the item
            Ensures(AllowsDuplicates ? Result<bool>() : OldValue(!this.Contains(item, EqualityComparer)));

            // The collection becomes non-empty
            Ensures(!IsEmpty);

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + 1);

            // Adding the item increases the number of equal items by one
            Ensures(this.Count(x => EqualityComparer.Equals(x, item)) == OldValue(this.Count(x => EqualityComparer.Equals(x, item))) + 1);

            // The collection will contain the item added
            Ensures(Contains(item));

            // The number of equal items increase by one
            Ensures(ContainsCount(item) == OldValue(ContainsCount(item)) + 1);

            // The item is added to the beginning
            Ensures(item.Equals(First));


            return default(bool);
        }

        public bool InsertLast(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Returns true if bag semantic, otherwise the opposite of whether the collection already contained the item
            Ensures(AllowsDuplicates ? Result<bool>() : OldValue(!this.Contains(item, EqualityComparer)));

            // The collection becomes non-empty
            Ensures(!IsEmpty);

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + 1);

            // Adding the item increases the number of equal items by one
            Ensures(this.Count(x => EqualityComparer.Equals(x, item)) == OldValue(this.Count(x => EqualityComparer.Equals(x, item))) + 1);

            // The collection will contain the item added
            Ensures(Contains(item));

            // The number of equal items increase by one
            Ensures(ContainsCount(item) == OldValue(ContainsCount(item)) + 1);

            // The item is added to the end
            Ensures(item.Equals(Last));


            return default(bool);
        }

        public bool IsSorted()
        {
            // No preconditions


            // True if sorted
            Ensures(Result<bool>() == Extensions.IsSorted(this));


            return default(bool);
        }

        public bool IsSorted(SCG.IComparer<T> comparer)
        {
            // No preconditions


            // True if sorted
            Ensures(Result<bool>() == Extensions.IsSorted(this, comparer));


            return default(bool);
        }

        public bool IsSorted(Comparison<T> comparison)
        {
            // Argument must be non-null
            Requires(comparison != null, ArgumentMustBeNonNull);


            // True if sorted
            Ensures(Result<bool>() == Extensions.IsSorted(this, comparison));


            return default(bool);
        }

        public IList<V> Map<V>(Func<T, V> mapper)
        {
            // Argument must be non-null
            Requires(mapper != null, ArgumentMustBeNonNull);


            // Result is equal to mapping each item
            Ensures(Result<IList<V>>().SequenceEqual(this.Select(mapper)));


            return default(IList<V>);
        }

        public IList<V> Map<V>(Func<T, V> mapper, SCG.IEqualityComparer<V> equalityComparer)
        {
            // Argument must be non-null
            Requires(mapper != null, ArgumentMustBeNonNull);


            // Result is equal to mapping each item
            Ensures(Result<IList<V>>().SequenceEqual(this.Select(mapper), equalityComparer)); // TODO: Does this always work? What if unique objects are created?

            // Result uses equality comparer
            Ensures(Result<IList<V>>().EqualityComparer == (equalityComparer ?? SCG.EqualityComparer<V>.Default));


            return default(IList<V>);
        }

        public T Remove()
        {
            // Collection must be non-empty
            Requires(!IsEmpty, CollectionMustBeNonEmpty);

            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);


            // Result is the item previously first/last
            Ensures(Result<T>().Equals(OldValue(IsFifo ? First : Last)));

            // Only the item at index is removed
            Ensures(this.SequenceEqual(OldValue((IsFifo ? this.Skip(1) : this.Take(Count - 1)).ToList())));

            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Removing an item decreases the count by one
            Ensures(Count == OldValue(Count) - 1);


            return default(T);
        }


        // Contracts are copied from IIndexed<T>.RemoveAt. Keep both updated!
        public T RemoveAt(int index)
        {
            // Argument must be within bounds (collection must be non-empty)
            Requires(0 <= index, ArgumentMustBeWithinBounds);
            Requires(index < Count, ArgumentMustBeWithinBounds);

            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);


            // Result is the item previously at the specified index
            Ensures(Result<T>().Equals(OldValue(this[index])));

            // Only the item at index is removed
            Ensures(this.SequenceEqual(OldValue(this.SkipRange(index, 1).ToList())));

            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Removing an item decreases the count by one
            Ensures(Count == OldValue(Count) - 1);


            return default(T);
        }

        public T RemoveFirst()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Collection must be non-empty
            Requires(!IsEmpty, CollectionMustBeNonEmpty);


            // Dequeuing an item decreases the count by one
            Ensures(Count == OldValue(Count) - 1);

            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Result is the same the first items
            Ensures(Result<T>().Equals(OldValue(First)));

            // Only the first item in the queue is removed
            Ensures(this.SequenceEqual(OldValue(this.Skip(1).ToList())));


            return default(T);
        }

        public T RemoveLast()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Collection must be non-empty
            Requires(!IsEmpty, CollectionMustBeNonEmpty);


            // Dequeuing an item decreases the count by one
            Ensures(Count == OldValue(Count) - 1);

            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Result is the same the first items
            Ensures(Result<T>().Equals(OldValue(Last)));

            // Only the last item in the queue is removed
            Ensures(this.SequenceEqual(OldValue(this.Take(Count - 1).ToList())));


            return default(T);
        }

        public void Reverse()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // The collection is reversed
            Ensures(this.SequenceEqual(OldValue(Enumerable.Reverse(this).ToList()))); // Uses the items' equality comparer and not the collection's


            return;
        }

        public void Shuffle()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // The collection remains the same
            Ensures(this.UnsequenceEqual(OldValue(ToArray()))); // Uses the items' equality comparer and not the collection's


            return;
        }

        public void Shuffle(Random random)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null
            Requires(random != null, ArgumentMustBeNonNull);


            // The collection remains the same
            Ensures(this.UnsequenceEqual(OldValue(ToArray()))); // Uses the items' equality comparer and not the collection's


            return;
        }

        public void Sort()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // List becomes sorted
            Ensures(IsSorted());

            // The collection remains the same
            Ensures(this.UnsequenceEqual(OldValue(ToArray()))); // Uses the items' equality comparer and not the collection's


            return;
        }

        public void Sort(SCG.IComparer<T> comparer)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // List becomes sorted
            Ensures(IsSorted(comparer));

            // The collection remains the same
            Ensures(this.UnsequenceEqual(OldValue(ToArray()))); // Uses the items' equality comparer and not the collection's


            return;
        }

        public void Sort(Comparison<T> comparison)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null
            Requires(comparison != null, ArgumentMustBeNonNull);


            // List becomes sorted
            Ensures(IsSorted(comparison));

            // The collection remains the same
            Ensures(this.UnsequenceEqual(OldValue(ToArray()))); // Uses the items' equality comparer and not the collection's


            return;
        }

        #region Hardened Postconditions

        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public bool AllowsDuplicates
        {
            get
            {
                // No additional preconditions allowed


                // Always true for lists
                Ensures(Result<bool>());


                return default(bool);
            }
        }

        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public bool IsSynchronized
        {
            get
            {
                // No preconditions


                // Always false
                Ensures(!Result<bool>());


                return default(bool);
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
        public abstract Speed CountSpeed { get; }
        public abstract bool IsEmpty { get; }
        public abstract EventTypes ListenableEvents { get; }
        public abstract T Choose();
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

        #region IExtensible

        public abstract bool DuplicatesByCounting { get; }
        public abstract SCG.IEqualityComparer<T> EqualityComparer { get; }
        public abstract void AddAll(SCG.IEnumerable<T> items);

        #endregion

        #region SC.ICollection

        public abstract object SyncRoot { get; }
        public abstract void CopyTo(Array array, int index);

        #endregion

        #region SCG.ICollection<T>

        void SCG.ICollection<T>.Add(T item) {}
        void SCG.IList<T>.Insert(int index, T item) {}

        #endregion

        #region ICollection<T>

        public abstract Speed ContainsSpeed { get; }
        public abstract bool Add(T item);
        public abstract bool Contains(T item);
        public abstract bool ContainsAll(SCG.IEnumerable<T> items);
        public abstract int ContainsCount(T item);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract bool Find(ref T item);
        public abstract bool FindOrAdd(ref T item);
        public abstract int GetUnsequencedHashCode();
        public abstract ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities();
        public abstract bool Remove(T item);
        public abstract bool Remove(T item, out T removedItem);
        public abstract bool RemoveAll(T item);
        public abstract void RemoveAll(SCG.IEnumerable<T> items);
        public abstract void RetainAll(SCG.IEnumerable<T> items);
        public abstract ICollectionValue<T> UniqueItems();
        public abstract bool UnsequencedEquals(ICollection<T> otherCollection);
        public abstract bool Update(T item);
        public abstract bool Update(T item, out T oldItem);
        public abstract bool UpdateOrAdd(T item);
        public abstract bool UpdateOrAdd(T item, out T oldItem);

        #endregion

        #region ISequenced<T>

        public abstract int GetSequencedHashCode();
        public abstract bool SequencedEquals(ISequenced<T> otherCollection);

        #endregion

        #region IIndexed<T>

        public abstract Speed IndexingSpeed { get; }
        public abstract IDirectedCollectionValue<T> GetIndexRange(int startIndex, int count);
        public abstract int LastIndexOf(T item);
        public abstract void RemoveIndexRange(int startIndex, int count);

        #endregion

        /*#region IDisposable

        public abstract void Dispose();

        #endregion*/

        #region SC.IList

        object IList.this[int index]
        {
            get { return default(object); }
            set { }
        }

        public abstract int Add(object value);
        public abstract bool Contains(object value);
        public abstract int IndexOf(object value);
        public abstract void Insert(int index, object value);
        public abstract void Remove(object value);
        void IList.RemoveAt(int index) {}

        #endregion

        #region SCG.IList<T>

        void SCG.IList<T>.RemoveAt(int index) {}

        #endregion

        #endregion
    }
}