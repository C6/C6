// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractHelperExtensions;
using static C6.Contracts.ContractMessage;

using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: Setup contracts to avoid exceptions?
    /// <summary>
    ///     Represents an indexed, sequenced generic collection where item order is determined by insertion and removal order.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the items in the collection.
    /// </typeparam>
    [ContractClass(typeof(IListContract<>))]
    public interface IList<T> : IIndexed<T>, SCG.IList<T>, IList
    {
        /// <summary>
        ///     Gets the number of items contained in the collection.
        /// </summary>
        /// <value>
        ///     The number of items contained in the collection.
        /// </value>
        [Pure]
        new int Count { get; }

        /// <summary>
        ///     Gets the first item in the list.
        /// </summary>
        /// <value>
        ///     The first item in this list.
        /// </value>
        [Pure]
        T First { get; }

        /// <summary>
        ///     Gets a value indicating whether the list has a fixed size.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the list has a fixed size; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         A list with a fixed size does not allow operations that changes the list's size.
        ///     </para>
        ///     <para>
        ///         Any list that is read-only (<see cref="IsReadOnly"/> is <c>true</c>), has a fixed size; the opposite need not
        ///         be true.
        ///     </para>
        /// </remarks>
        [Pure]
        new bool IsFixedSize { get; }

        /// <summary>
        ///     Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the collection is read-only; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     A collection that is read-only does not allow the addition or removal of items after the collection is created.
        ///     Note that read-only in this context does not indicate whether individual items of the collection can be modified.
        /// </remarks>
        [Pure]
        new bool IsReadOnly { get; }

        /// <summary>
        ///     Gets the last item in the list.
        /// </summary>
        /// <value>
        ///     The last item in the list.
        /// </value>
        [Pure]
        T Last { get; }

        // TODO: override methods to change documentation
        /// <summary>
        ///     Gets or sets a value indicating whether methods remove items from the beginning of the list.
        /// </summary>
        /// <value>
        ///     <c>true</c> if methods remove items from the beginning of the list; <c>false</c> if they remove the items from the
        ///     end of the list.
        /// </value>
        /// <remarks>
        ///     Notice that <see cref="IExtensible{T}.Add"/> and <see cref="IExtensible{T}.AddRange"/> always adds items to the end
        ///     of the list.
        /// </remarks>
        /// <seealso cref="Remove"/>
        /// <seealso cref="System.Collections.IList.Remove"/>
        /// <seealso cref="ICollection{T}.Remove(T)"/>
        /// <seealso cref="ICollection{T}.Remove(T, out T)"/>
        /// <seealso cref="ICollection{T}.RemoveRange"/>
        /// <seealso cref="ICollection{T}.RetainRange"/>
        /// <seealso cref="ICollection{T}.Update(T)"/>
        /// <seealso cref="ICollection{T}.Update(T, out T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T)"/>
        /// <seealso cref="ICollection{T}.UpdateOrAdd(T, out T)"/>
        [Pure]
        bool RemovesFromBeginning { get; set; }

        /// <summary>
        ///     Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index of the item to get or set.
        /// </param>
        /// <value>
        ///     The item at the specified index. <c>null</c> is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/> is
        ///     <c>true</c>.
        /// </value>
        /// <remarks>
        ///     The setter raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the removed item and the index.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed item and a count of one.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemInserted"/> with the inserted item and the index.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemsAdded"/> with the inserted item and a count of one.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [IndexerName("Item")]
        new T this[int index]
        {
            [Pure]
            get;
            set;
        }

        /// <summary>
        ///     Removes all items from the collection.
        /// </summary>
        /// <remarks>
        ///     If the collection is non-empty, it raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionCleared"/> as full and with count equal to the collection
        ///                 count.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        new void Clear();

        // TODO: Include index in predicate?
        // TODO: Should this be deprecated?
        // TODO: Copy docs from Enumerable.Where?
        /// <summary>
        ///     Creates a new list consisting of the items in this list satisfying a specified predicate.
        /// </summary>
        /// <param name="predicate">
        ///     A delegate that returns <c>true</c> for the items that should be included in the new list.
        /// </param>
        /// <returns>
        ///     A new list containing the items that satisfy the predicate.
        /// </returns>
        [Pure]
        IList<T> FindAll(Func<T, bool> predicate);

        /// <summary>
        ///     Searches from the beginning of the collection for the specified item and returns the zero-based index of the first
        ///     occurrence within the collection.
        /// </summary>
        /// <param name="item">
        ///     The item to locate in the collection. <c>null</c> is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/> is
        ///     <c>true</c>.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of item within the entire collection, if found; otherwise, a negative
        ///     number that is the bitwise complement of the index at which <see cref="ICollection{T}.Add"/> would put the item.
        /// </returns>
        [Pure]
        new int IndexOf(T item);

        /// <summary>
        ///     Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index at which value should be inserted.
        /// </param>
        /// <param name="item">
        ///     The item to insert into the list. <c>null</c> is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/> is
        ///     <c>true</c>.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         If <paramref name="index"/> equals the number of items in the list, then the value is appended to the end of
        ///         the list. This has the same effect as calling <see cref="ICollection{T}.Add"/>, though the events raised are
        ///         different.
        ///     </para>
        ///     <para>
        ///         When inserting, the items that follow the insertion point move down to accommodate the new item. The indices of
        ///         the items that are moved are also updated.
        ///     </para>
        ///     <para>
        ///         Raises the following events (in that order) with the collection as sender:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemInserted"/> with the inserted item and the index.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemsAdded"/> with the item and a count of one.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        new void Insert(int index, T item);

        /// <summary>
        ///     Inserts the items into the list starting at the specified index.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index at which the new items should be inserted.
        /// </param>
        /// <param name="items">
        ///     The enumerable whose items should be inserted into the list. The enumerable itself cannot be <c>null</c>, but its
        ///     items can, if <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         If the enumerable throws an exception during enumeration, the collection remains unchanged.
        ///     </para>
        ///     <para>
        ///         Raises the following events (in that order) with the collection as sender:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemInserted"/> once for each item and the index at which is was
        ///                     inserted.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemsAdded"/> once for each item added (using a count of one).
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.CollectionChanged"/> once at the end.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        void InsertRange(int index, SCG.IEnumerable<T> items);

        /// <summary>
        ///     Inserts an item at the beginning of the list.
        /// </summary>
        /// <param name="item">
        ///     The item to insert at the beginning of the list. <c>null</c> is allowed, if
        ///     <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemInserted"/> with the item and an index of zero.
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
        void InsertFirst(T item);

        /// <summary>
        ///     Inserts an item at the end of the list.
        /// </summary>
        /// <param name="item">
        ///     The item to insert at the end of the list. <c>null</c> is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/>
        ///     is <c>true</c>.
        /// </param>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemInserted"/> with the item and an index of <c>Count</c>.
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
        void InsertLast(T item);

        /// <summary>
        ///     Determines whether the list is sorted in non-descending order according to the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The default comparer <see cref="SCG.Comparer{T}.Default"/> cannot find an implementation of the
        ///     <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/> interface for type
        ///     <typeparamref name="T"/>.
        /// </exception>
        /// <returns>
        ///     <c>true</c> if the list is sorted in non-descending order; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        bool IsSorted();

        /// <summary>
        ///     Determines whether the list is sorted in non-descending order according to the specified comparer.
        /// </summary>
        /// <param name="comparer">
        ///     The <see cref="SCG.IComparer{T}"/> implementation to use when comparing items, or <c>null</c> to use the default
        ///     comparer <see cref="SCG.Comparer{T}.Default"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="comparer"/> is <c>null</c>, and the default comparer <see cref="SCG.Comparer{T}.Default"/> cannot
        ///     find an implementation of the <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/>
        ///     interface for type <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The implementation of <paramref name="comparer"/> caused an error during the sort. For example,
        ///     <paramref name="comparer"/> might not return zero when comparing an item with itself.
        /// </exception>
        /// <returns>
        ///     <c>true</c> if the list is sorted in non-descending order; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        bool IsSorted(SCG.IComparer<T> comparer);

        /// <summary>
        ///     Determines whether the list is sorted in non-descending order according to the specified
        ///     <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">
        ///     The <see cref="Comparison{T}"/> to use when comparing elements.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     The implementation of <paramref name="comparison"/> caused an error during the sort. For example,
        ///     <paramref name="comparison"/> might not return zero when comparing an item with itself.
        /// </exception>
        /// <returns>
        ///     <c>true</c> if the list is sorted in non-descending order; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        bool IsSorted(Comparison<T> comparison);

        // TODO: Deprecate?
        // TODO: Check List<T>.ConvertAll
        /// <summary>
        ///     Creates a new list consisting of the results of mapping all items in this list using the specified mapper. The new
        ///     list will use the default equality comparer for type <typeparamref name="V"/>.
        /// </summary>
        /// <typeparam name="V">
        ///     The type of the items in the new list.
        /// </typeparam>
        /// <param name="mapper">
        ///     A function that maps each item in this list to an item in the new list.
        /// </param>
        /// <returns>
        ///     An new list whose items are the results of mapping all items in this list.
        /// </returns>
        [Pure]
        IList<V> Map<V>(Func<T, V> mapper);

        // TODO: Deprecate?
        /// <summary>
        ///     Creates a new list consisting of the results of mapping all items in this list using the specified mapper. The new
        ///     list will use the specified equality comparer.
        /// </summary>
        /// <typeparam name="V">
        ///     The type of the items in the new list.
        /// </typeparam>
        /// <param name="mapper">
        ///     A function that maps each item in this list to an item in the new list.
        /// </param>
        /// <param name="equalityComparer">
        ///     The <see cref="SCG.IEqualityComparer{T}"/> to use for the new list.
        /// </param>
        /// <returns>
        ///     An new list whose items are the results of mapping all items in this list.
        /// </returns>
        [Pure]
        IList<V> Map<V>(Func<T, V> mapper, SCG.IEqualityComparer<V> equalityComparer);

        /// <summary>
        ///     Removes and returns an item from either the beginning or the end of the list, depending on the value of
        ///     <see cref="RemovesFromBeginning"/>.
        /// </summary>
        /// <returns>
        ///     The item removed from the list.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If <see cref="RemovesFromBeginning"/> is <c>true</c>, this methods removes an item from the beginning of the
        ///         list; if <see cref="RemovesFromBeginning"/> is <c>false</c>, it removes an item from the end of the list.
        ///     </para>
        ///     <para>
        ///         Raises the following events (in that order) with the collection as sender:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and an index of either zero if
        ///                     <see cref="RemovesFromBeginning"/> is <c>true</c>, or <c>Count</c> - 1 if
        ///                     <see cref="RemovesFromBeginning"/> is <c>false</c>.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed item and a count of one.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        /// <seealso cref="RemovesFromBeginning"/>
        T Remove();

        /// <summary>
        ///     Removes and returns the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index of the item to remove.
        /// </param>
        /// <returns>
        ///     The item removed from the collection.
        /// </returns>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the removed item and the index.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed item and a count of one.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        new T RemoveAt(int index);

        /// <summary>
        ///     Removes and returns an item from the beginning of the list.
        /// </summary>
        /// <returns>
        ///     The item removed from the beginning of the list.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The methods <see cref="ICollection{T}.Add"/> and <see cref="RemoveFirst"/> together behave like a
        ///         first-in-first-out queue.
        ///     </para>
        ///     <para>
        ///         Raises the following events (in that order) with the collection as sender:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and an index of zero.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed item and a count of one.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        T RemoveFirst();

        /// <summary>
        ///     Removes and returns an item from the end of the list.
        /// </summary>
        /// <returns>
        ///     The item removed from the end of the list.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The methods <see cref="ICollection{T}.Add"/> and <see cref="RemoveLast"/> together behave like a
        ///         last-in-first-out stack.
        ///     </para>
        ///     <para>
        ///         Raises the following events (in that order) with the collection as sender:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemRemovedAt"/> with the item and an index of <c>Count</c> - 1.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed item and a count of one.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        T RemoveLast();

        /// <summary>
        ///     Reverses the sequence order of the items in the list.
        /// </summary>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        void Reverse();

        /// <summary>
        ///     Randomly shuffles the items in the list.
        /// </summary>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        void Shuffle();

        /// <summary>
        ///     Shuffles the items in the list according to the specified random source.
        /// </summary>
        /// <param name="random">The random source.</param>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        void Shuffle(Random random);

        /// <summary>
        ///     Sorts the items in the list using the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The default comparer <see cref="SCG.Comparer{T}.Default"/> cannot find an implementation of the
        ///     <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/> interface for type
        ///     <typeparamref name="T"/>.
        /// </exception>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        void Sort();

        /// <summary>
        ///     Sorts the items in the list using the specified comparer.
        /// </summary>
        /// <param name="comparer">
        ///     The <see cref="SCG.IComparer{T}"/> implementation to use when comparing items, or <c>null</c> to use the default
        ///     comparer <see cref="SCG.Comparer{T}.Default"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="comparer"/> is <c>null</c>, and the default comparer <see cref="SCG.Comparer{T}.Default"/> cannot
        ///     find an implementation of the <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/>
        ///     interface for type <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The implementation of <paramref name="comparer"/> caused an error during the sort. For example,
        ///     <paramref name="comparer"/> might not return zero when comparing an item with itself.
        /// </exception>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        void Sort(SCG.IComparer<T> comparer);

        /// <summary>
        ///     Sorts the items in the list using the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">
        ///     The <see cref="Comparison{T}"/> to use when comparing elements.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     The implementation of <paramref name="comparison"/> caused an error during the sort. For example,
        ///     <paramref name="comparison"/> might not return zero when comparing an item with itself.
        /// </exception>
        /// <remarks>
        ///     Raises the following events (in that order) with the collection as sender:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <see cref="ICollectionValue{T}.CollectionChanged"/>.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        void Sort(Comparison<T> comparison);
    }


    [ContractClassFor(typeof(IList<>))]
    internal abstract class IListContract<T> : IList<T>
    {
        // ReSharper disable InvocationIsSkipped

        public int Count
        {
            get {
                // No additional preconditions allowed


                // No postconditions


                return default(int);
            }
        }

        public T First
        {
            get {
                // Collection must be non-empty
                Requires(!IsEmpty, CollectionMustBeNonEmpty);


                // Equals first item
                Ensures(Result<T>().IsSameAs(this[0]));
                Ensures(Result<T>().IsSameAs(this.First()));


                return default(T);
            }
        }

        public bool RemovesFromBeginning
        {
            get { return default(bool); }
            set {
                // Collection must be non-read-only
                Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


                // Value is updated
                Ensures(RemovesFromBeginning == value);


                return;
            }
        }

        public bool IsFixedSize
        {
            get {
                // No additional preconditions allowed


                // No postconditions


                return default(bool);
            }
        }

        public bool IsReadOnly
        {
            get {
                // No additional preconditions allowed


                // No postconditions


                return default(bool);
            }
        }

        public T Last
        {
            get {
                // Collection must be non-empty
                Requires(!IsEmpty, CollectionMustBeNonEmpty);


                // Equals first item
                Ensures(Result<T>().IsSameAs(this[Count - 1]));
                Ensures(Result<T>().IsSameAs(this.Last()));


                return default(T);
            }
        }

        public T this[int index]
        {
            get {
                // Argument must be within bounds
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);


                // Result is the same as skipping the first index items
                Ensures(Result<T>().IsSameAs(this.Skip(index).First()));


                return default(T);
            }
            set {
                // Collection must be non-read-only
                Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

                // Argument must be non-null if collection disallows null values
                Requires(AllowsNull || value != null, ArgumentMustBeNonNull);

                // Argument must be within bounds
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);

                // Collection must not already contain item if collection disallows duplicate values
                Requires(AllowsDuplicates || !Contains(value), CollectionMustAllowDuplicates);


                // Value is the same as skipping the first index items
                Ensures(value.IsSameAs(this[index]));


                return;
            }
        }

        public void Clear()
        {
            // No additional preconditions allowed


            // No postconditions


            return;
        }

        public IList<T> FindAll(Func<T, bool> predicate)
        {
            // Argument must be non-null
            Requires(predicate != null, ArgumentMustBeNonNull);


            // The result is the same as filtering this list based on the predicate
            Ensures(Result<IList<T>>().IsSameSequenceAs(this.Where(predicate)));

            // The returned list has the same type as this list
            Ensures(Result<IList<T>>().GetType() == GetType());


            return default(IList<T>);
        }

        public int IndexOf(T item)
        {
            // No additional preconditions allowed


            // Result is a valid index
            Ensures(Contains(item)
                ? 0 <= Result<int>() && Result<int>() < Count
                : ~Result<int>() == Count);


            return default(int);
        }

        public void Insert(int index, T item)
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

            // Collection must not already contain item if collection disallows duplicate values
            Requires(AllowsDuplicates || !Contains(item), CollectionMustAllowDuplicates);


            // Item is inserted at index
            Ensures(item.IsSameAs(this[index]));

            // The item is inserted into the list without replacing other items
            Ensures(this.IsSameSequenceAs(OldValue(this.Take(index).Append(item).Concat(this.Skip(index)).ToList())));


            return;
        }

        public void InsertRange(int index, SCG.IEnumerable<T> items)
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

            // Collection must not already contain the items if collection disallows duplicate values
            Requires(AllowsDuplicates || ForAll(this, item => !Contains(item)), CollectionMustAllowDuplicates);


            // The items are inserted into the list without replacing other items
            Ensures(this.IsSameSequenceAs(OldValue(this.Take(index).Concat(items).Concat(this.Skip(index)).ToList())));

            // Collection doesn't change if enumerator throws an exception
            EnsuresOnThrow<Exception>(this.IsSameSequenceAs(OldValue(ToArray())));


            return;
        }

        public void InsertFirst(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);

            // Collection must not already contain item if collection disallows duplicate values
            Requires(AllowsDuplicates || !Contains(item), CollectionMustAllowDuplicates);


            // The collection becomes non-empty
            Ensures(!IsEmpty);

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + 1);

            // The collection will contain the item added
            Ensures(Contains(item));

            // The number of equal items increase by one
            Ensures(CountDuplicates(item) == OldValue(CountDuplicates(item)) + 1);

            // The number of same items increase by one
            Ensures(this.ContainsSameCount(item) == OldValue(this.ContainsSameCount(item)) + 1);

            // The item is added to the beginning
            Ensures(item.IsSameAs(First));


            return;
        }

        public void InsertLast(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);

            // Collection must not already contain item if collection disallows duplicate values
            Requires(AllowsDuplicates || !Contains(item), CollectionMustAllowDuplicates);


            // The collection becomes non-empty
            Ensures(!IsEmpty);

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + 1);

            // The collection will contain the item added
            Ensures(Contains(item));

            // The number of equal items increase by one
            Ensures(CountDuplicates(item) == OldValue(CountDuplicates(item)) + 1);

            // The number of same items increase by one
            Ensures(this.ContainsSameCount(item) == OldValue(this.ContainsSameCount(item)) + 1);

            // The item is added to the end
            Ensures(item.IsSameAs(Last));


            return;
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
            // This would only work if the mapper is deterministic!
            // Ensures(Result<IList<V>>().SequenceEqual(this.Select(mapper)));

            // The returned list has the same type as this list
            Ensures(Result<IList<V>>().GetType() == GetType());


            return default(IList<V>);
        }

        public IList<V> Map<V>(Func<T, V> mapper, SCG.IEqualityComparer<V> equalityComparer)
        {
            // Argument must be non-null
            Requires(mapper != null, ArgumentMustBeNonNull);


            // Result is equal to mapping each item
            // This would only work if the mapper is deterministic!
            // Ensures(Result<IList<V>>().SequenceEqual(this.Select(mapper), equalityComparer));

            // Result uses equality comparer
            Ensures(Result<IList<V>>().EqualityComparer == (equalityComparer ?? SCG.EqualityComparer<V>.Default));

            // The returned list has the same type as this list
            Ensures(Result<IList<V>>().GetType() == GetType());


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
            Ensures(Result<T>().IsSameAs(OldValue(RemovesFromBeginning ? First : Last)));

            // Only the item at index is removed
            Ensures(this.IsSameSequenceAs(OldValue((RemovesFromBeginning ? this.Skip(1) : this.Take(Count - 1)).ToList())));

            // Result is non-null
            Ensures(AllowsNull || Result<T>() != null);

            // Removing an item decreases the count by one
            Ensures(Count == OldValue(Count) - 1);


            return default(T);
        }


        public T RemoveAt(int index)
        {
            // No additional preconditions allowed


            // No postconditions


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
            Ensures(Result<T>().IsSameAs(OldValue(First)));

            // Only the first item in the queue is removed
            Ensures(this.IsSameSequenceAs(OldValue(this.Skip(1).ToList())));


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
            Ensures(Result<T>().IsSameAs(OldValue(Last)));

            // Only the last item in the queue is removed
            Ensures(this.IsSameSequenceAs(OldValue(this.Take(Count - 1).ToList())));


            return default(T);
        }

        public void Reverse()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // The collection is reversed
            Ensures(this.IsSameSequenceAs(OldValue(Enumerable.Reverse(this).ToList())));


            return;
        }

        public void Shuffle()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // The collection remains the same
            Ensures(this.HasSameAs(OldValue(ToArray())));


            return;
        }

        public void Shuffle(Random random)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null
            Requires(random != null, ArgumentMustBeNonNull);


            // The collection remains the same
            Ensures(this.HasSameAs(OldValue(ToArray())));


            return;
        }

        public void Sort()
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // List becomes sorted
            Ensures(IsSorted());

            // The collection remains the same
            Ensures(this.HasSameAs(OldValue(ToArray())));


            return;
        }

        public void Sort(SCG.IComparer<T> comparer)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);


            // List becomes sorted
            Ensures(IsSorted(comparer));

            // The collection remains the same
            Ensures(this.HasSameAs(OldValue(ToArray())));


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
            Ensures(this.HasSameAs(OldValue(ToArray())));


            return;
        }

        #region Hardened Postconditions

        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public bool Add(T item)
        {
            // No additional preconditions allowed


            // Item is placed at the end
            Ensures(Last.IsSameAs(item));


            return default(bool);
        }

        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public bool AllowsDuplicates
        {
            get {
                // No additional preconditions allowed


                // Always true for lists
                Ensures(Result<bool>());


                return default(bool);
            }
        }

        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public bool IsSynchronized
        {
            get {
                // No preconditions


                // Always false
                Ensures(!Result<bool>());


                return default(bool);
            }
        }

        public int LastIndexOf(T item)
        {
            // No additional preconditions allowed


            // Result is a valid index
            Ensures(Contains(item)
                ? 0 <= Result<int>() && Result<int>() < Count
                : ~Result<int>() == Count);


            return default(int);
        }

        public void Remove(object value)
        {
            // No extra preconditions allowed


            // TODO: How do this fail when value is not of type T?
            // If an item is removed, it is removed according to RemovesFromBeginning
            Ensures(!OldValue(Contains(value)) || this.IsSameSequenceAs(OldValue(this.SkipRange(RemovesFromBeginning ? IndexOf(value) : LastIndexOf((T) value), 1).ToList()))); // TODO: Is ToList needed?


            return;
        }

        public bool Remove(T item)
        {
            // No extra preconditions allowed


            // If an item is removed, it is removed according to RemovesFromBeginning
            Ensures(!Result<bool>() || this.IsSameSequenceAs(OldValue(this.SkipRange(RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item), 1).ToList()))); // TODO: Is ToList needed?


            return default(bool);
        }

        public bool Remove(T item, out T removedItem)
        {
            // No extra preconditions allowed


            // If an item is removed, it is removed according to RemovesFromBeginning
            Ensures(!Result<bool>() || this.IsSameSequenceAs(OldValue(this.SkipRange(RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item), 1).ToList()))); // TODO: Is ToList needed?

            // The item removed is the first/last equal to item depending on the value of RemovesFromBeginning
            Ensures(!Result<bool>() || ValueAtReturn(out removedItem).IsSameAs(OldValue(this[RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)])));


            removedItem = default(T);
            return default(bool);
        }

        public bool RemoveRange(SCG.IEnumerable<T> items)
        {
            // No extra preconditions allowed


            //  
            Ensures(false); // TODO: Write contract that uses RemovesFromBeginning


            return default(bool);
        }
        public bool RemoveDuplicates(T item)
        {
            // No extra preconditions allowed
            

            // The list is the same as the old collection without item
            Ensures(this.IsSameSequenceAs(OldValue(this.Where(x => !EqualityComparer.Equals(x, item)).ToList())));


            return default(bool);
        }

        public bool RetainRange(SCG.IEnumerable<T> items)
        {
            // No extra preconditions allowed


            //  
            Ensures(false); // TODO: Write contract that uses RemovesFromBeginning


            return default(bool);
        }

        public bool Update(T item)
        {
            // No extra preconditions allowed


            // If an item is updated, it is updated according to RemovesFromBeginning
            Ensures(!Result<bool>() || this.IsSameSequenceAs(OldValue(this.Take(RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)).Append(item).Concat(this.Skip((RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)) + 1)).ToList()))); // TODO: Is ToList needed?


            return default(bool);
        }

        public bool Update(T item, out T oldItem)
        {
            // No extra preconditions allowed


            // If an item is updated, it is updated according to RemovesFromBeginning
            Ensures(!Result<bool>() || this.IsSameSequenceAs(OldValue(this.Take(RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)).Append(item).Concat(this.Skip((RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)) + 1)).ToList()))); // TODO: Is ToList needed?

            // The item removed is the first/last equal to item depending on the value of RemovesFromBeginning
            Ensures(!Result<bool>() || ValueAtReturn(out oldItem).IsSameAs(OldValue(this[RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)])));


            oldItem = default(T);
            return default(bool);
        }

        public bool UpdateOrAdd(T item)
        {
            // No extra preconditions allowed


            // If an item is updated, it is updated according to RemovesFromBeginning
            Ensures(this.IsSameSequenceAs(OldValue((Result<bool>() ? this.Take(RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)).Append(item).Concat(this.Skip((RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)) + 1)) : this.Append(item)).ToList()))); // TODO: Is ToList needed?


            return default(bool);
        }

        public bool UpdateOrAdd(T item, out T oldItem)
        {
            // No extra preconditions allowed


            // If an item is updated, it is updated according to RemovesFromBeginning
            Ensures(this.IsSameSequenceAs(OldValue((Result<bool>() ? this.Take(RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)).Append(item).Concat(this.Skip((RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)) + 1)) : this.Append(item)).ToList()))); // TODO: Is ToList needed?

            // The item removed is the first/last equal to item depending on the value of RemovesFromBeginning
            Ensures(!Result<bool>() || ValueAtReturn(out oldItem).IsSameAs(OldValue(this[RemovesFromBeginning ? IndexOf(item) : LastIndexOf(item)])));


            oldItem = default(T);
            return default(bool);
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
        public abstract bool AddRange(SCG.IEnumerable<T> items);

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
        public abstract bool Contains(T item);
        public abstract bool ContainsRange(SCG.IEnumerable<T> items);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract int CountDuplicates(T item);
        public abstract bool Find(ref T item);
        public abstract bool FindOrAdd(ref T item);
        public abstract int GetUnsequencedHashCode();
        public abstract ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities();
        public abstract ICollectionValue<T> UniqueItems();
        public abstract bool UnsequencedEquals(ICollection<T> otherCollection);

        #endregion

        #region ISequenced<T>

        public abstract int GetSequencedHashCode();
        public abstract bool SequencedEquals(ISequenced<T> otherCollection);

        #endregion

        #region IIndexed<T>

        public abstract Speed IndexingSpeed { get; }
        public abstract IDirectedCollectionValue<T> GetIndexRange(int startIndex, int count);
        public abstract void RemoveIndexRange(int startIndex, int count);

        #endregion

        #region IDisposable

        public abstract void Dispose();

        #endregion

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
        void IList.RemoveAt(int index) {}

        #endregion

        #region SCG.IList<T>

        void SCG.IList<T>.RemoveAt(int index) {}

        #endregion

        #endregion
    }
}