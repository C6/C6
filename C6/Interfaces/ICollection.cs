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


namespace C6
{
    /// <summary>
    /// Represents an extensible generic collection from which items can also
    /// be removed.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [ContractClass(typeof(ICollectionContract<>))]
    public interface ICollection<T> : IExtensible<T>, SCG.ICollection<T>
    {
        // This is somewhat similar to the RandomAccess marker interface in Java
        /// <summary>
        /// Gets a value characterizing the asymptotic complexity of
        /// <see cref="Contains"/> proportional to collection size (worst-case
        /// or amortized as relevant).
        /// </summary>
        /// <value>A characterization of the asymptotic speed of
        /// <see cref="Contains"/> proportional to collection size.</value>
        [Pure]
        Speed ContainsSpeed { get; }

        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        /// <value>The number of items contained in the collection.</value>
        [Pure]
        new int Count { get; }

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
        /// Adds an item to the collection if possible.
        /// </summary>
        /// <param name="item">The item to add to the collection. <c>null</c> 
        /// is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/> is
        /// <c>true</c>.</param>
        /// <returns><c>true</c> if item was added;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>If the collection has set semantics, the item will be
        /// added if not already in the collection. If bag semantics, the item 
        /// will always be added. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.</para>
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
        new bool Add(T item);

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

        /// <summary>
        /// Determines whether the collection contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <returns><c>true</c> if item is found in the collection;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.</remarks>
        [Pure]
        new bool Contains(T item);

        /// <summary>
        /// Checks whether the collection contains all the items in the
        /// specified <see cref="SCG.IEnumerable{T}"/>. If the collection has
        /// bag semantics, multiplicities is taken into account.
        /// </summary>
        /// <param name="items">The specified <see cref="SCG.IEnumerable{T}"/>.
        /// The enumerable itself cannot be <c>null</c>, but its items can, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <returns><c>true</c> if all items in <paramref name="items"/> are
        /// in the collection; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// The collection's <see cref="IExtensible{T}.EqualityComparer"/>
        /// is used to determine item equality.
        /// </para>
        /// <para>
        /// If the enumerable throws an exception during enumeration, the
        /// collection remains unchanged.
        /// </para>
        /// </remarks>
        /// <seealso cref="Contains(T)"/>
        [Pure]
        bool ContainsAll(SCG.IEnumerable<T> items);

        /// <summary>
        /// Returns the item's multiplicity in the collection: the number of
        /// items in the collection equal to the specified item.
        /// </summary>
        /// <param name="item">The item to count in the collection. <c>null</c>
        /// is allowed, if <see cref="ICollectionValue{T}.AllowsNull"/> is
        /// <c>true</c>.</param>
        /// <returns>The number of items equal to the specified item found.
        /// Returns 0 if and only if the value is not in the collection.
        /// </returns>
        /// <remarks>The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.</remarks>
        [Pure]
        int ContainsCount(T item);

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
        new void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// Determines whether the collection contains a specific item and 
        /// assigns it to <paramref name="item"/> if so.
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>. If the
        /// collection contains the item, the item is assigned to the reference
        /// parameter on return.</param>
        /// <returns><c>true</c> if item is found in the collection;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.</remarks>
        /// <seealso cref="Contains"/>
        [Pure]
        bool Find(ref T item);

        /// <summary>
        /// Determines whether the collection contains a specific item and 
        /// assigns it to <paramref name="item"/> if so; otherwise, adds the
        /// item to the collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>. If the
        /// collection already contains the item, the item is assigned to the
        /// parameter on return; otherwise, the item is added to the
        /// collection.</param>
        /// <returns><c>true</c> if item was found and therefore not added;
        /// otherwise, <c>false</c> in which case the item is added.</returns>
        /// <remarks>
        /// <para>If the collection already contains the item, the item will 
        /// not be added, even if the collection has bag semantics. The
        /// collection's <see cref="IExtensible{T}.EqualityComparer"/> is used
        /// to determine item equality.</para>
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
        /// <seealso cref="Add"/>
        bool FindOrAdd(ref T item);

        // TODO: Maybe refer to static method, that can be used in contracts as well
        /// <summary>
        /// Returns the unsequenced (order-insensitive) hash code of the
        /// collection.
        /// </summary>
        /// <returns>The unsequenced hash code of the collection.</returns>
        /// <remarks>
        /// <para>
        /// The collection's unsequenced hash code is defined as the sum of a
        /// transformation of the hash codes of its items, each computed using
        /// the collection’s <see cref="IExtensible{T}.EqualityComparer"/>.
        /// </para>
        /// <para>
        /// The implementations must use a fixed transformation that allows
        /// serialization and the hash code must be cached and thus not
        /// recomputed unless the collection has changed since the last call to
        /// this method.
        /// </para>
        /// </remarks>
        /// <seealso cref="UnsequencedEquals"/>
        [Pure]
        int GetUnsequencedHashCode();

        /// <summary>
        /// Returns a new collection value whose items are
        /// <see cref="KeyValuePair{T,Int}"/> where
        /// <see cref="KeyValuePair{T,Int}.Key"/> is an item in this 
        /// collection and <see cref="KeyValuePair{TKey,TValue}.Value"/> is the
        /// multiplicity of the item in this collection: the number of times
        /// the item appears.
        /// </summary>
        /// <returns>A collection value whose items are
        /// <see cref="KeyValuePair{T,Int}"/> of this collection's items and
        /// their multiplicity.</returns>
        /// <remarks>For collections with set semantics, the multiplicity is
        /// always one; for collections with bag semantics, the multiplicity is
        /// at least one.</remarks>
        [Pure]
        ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities();

        /// <summary>
        /// Removes an occurrence of a specific item from the collection, if
        /// any.
        /// </summary>
        /// <param name="item">The item to remove from the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <returns><c>true</c> if item was successfully removed from the
        /// collection; otherwise, <c>false</c> if item is not found in
        /// the original collection.</returns>
        /// <remarks>
        /// <para>
        /// At most one item is removed, even if the collection has bag
        /// semantics. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.
        /// </para>
        /// <para>
        /// If the item is removed, it raises the following events (in that 
        /// order) with the collection as sender:
        /// <list type="bullet">
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
        new bool Remove(T item);

        /// <summary>
        /// Removes an occurrence of a specific item from the collection, if
        /// any, and assigns the removed item to
        /// <paramref name="removedItem"/>.
        /// </summary>
        /// <param name="item">The item to remove from the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <param name="removedItem">The removed item if any.</param>
        /// <returns><c>true</c> if item was successfully removed from the
        /// collection; otherwise, <c>false</c> if item is not found in
        /// the original collection.</returns>
        /// <remarks>
        /// <para>
        /// At most one item is removed, even if the collection has bag
        /// semantics. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.
        /// </para>
        /// <para>
        /// If the item is removed, it raises the following events (in that 
        /// order) with the collection as sender:
        /// <list type="bullet">
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
        bool Remove(T item, out T removedItem);

        // TODO: Reconsider rewriting event behavior documentation
        // TODO: Rename to RemoveCompletely/RemoveEquals/RemoveDuplicates/RemoveCopies and document change
        /// <summary>
        /// Removes all occurrences of a specific item from the collection, if
        /// any.
        /// </summary>
        /// <param name="item">The item to remove from the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <returns><c>true</c> if item was successfully removed from the
        /// collection; otherwise, <c>false</c> if item is not found in
        /// the original collection.</returns>
        /// <remarks>
        /// <para>
        /// This changes the multiplicity of <paramref name="item"/> in the 
        /// collection to zero, regardless of whether the collection has set or
        /// bag semantics, i.e. this guarantees that <c>coll.Contains(item)</c>
        /// returns <c>false</c> at the return of this method. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.
        /// </para>
        /// <para>
        /// If any items are removed, and the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If any items are removed, and the collection has set semantics or
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>false</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> once for each item
        /// removed (using a count of one).
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/> once at the end.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        bool RemoveAll(T item);

        /// <summary>
        /// Removes each item of the specified enumerable from the collection, 
        /// if possible, in enumeration order.
        /// </summary>
        /// <param name="items">The enumerable whose items should be removed
        /// from the collection. The enumerable itself cannot be <c>null</c>,
        /// but its items can, if <see cref="ICollectionValue{T}.AllowsNull"/>
        /// is <c>true</c>.</param>
        /// <remarks>
        /// <para>
        /// If the collection has bag semantics, this means reducing the item
        /// multiplicity of each item in the collection by at most the 
        /// multiplicity of the item in <paramref name="items"/>. The
        /// collection's <see cref="IExtensible{T}.EqualityComparer"/> is used
        /// to determine item equality.
        /// </para>
        /// <para>
        /// If the enumerable throws an exception during enumeration, the
        /// collection remains unchanged.
        /// </para>
        /// <para>
        /// If any items are removed, it raises the following events (in that 
        /// order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> once for each item
        /// removed (using a count of one).
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/> once at the end.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        void RemoveAll(SCG.IEnumerable<T> items);

        /// <summary>
        /// Removes the items of the current collection that do not exist in
        /// the specified <see cref="SCG.IEnumerable{T}"/>. If the collection
        /// has bag semantics, multiplicities is taken into account.
        /// </summary>
        /// <param name="items">The specified <see cref="SCG.IEnumerable{T}"/>
        /// whose items should be retained in the collection. The enumerable 
        /// itself cannot be <c>null</c>, but its items can, if 
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <remarks>
        /// <para>The items remaining in the collection is the intersection
        /// between the original collection and the specified collection.</para>
        /// <para>The collection's <see cref="IExtensible{T}.EqualityComparer"/>
        /// is used to determine item equality.</para>
        /// <para>
        /// If the enumerable throws an exception during enumeration, the
        /// collection remains unchanged.
        /// </para>
        /// <para>
        /// If any items are removed, it raises the following events (in that 
        /// order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> once for each item
        /// removed (using a count of one).
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/> once at the end.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        void RetainAll(SCG.IEnumerable<T> items);

        // TODO: Consider returning a read-only collection instead
        /// <summary>
        /// Returns a <see cref="ICollectionValue{T}"/> equal to this
        /// collection without duplicates.
        /// </summary>
        /// <returns>A collection value equal to this collection without
        /// duplicates.</returns>
        /// <remarks>If the collection allows duplicates, a new collection is
        /// created and returned; if not, the collection itself is returned.
        /// The collection's <see cref="IExtensible{T}.EqualityComparer"/> is
        /// used to determine item equality.</remarks>
        [Pure]
        ICollectionValue<T> UniqueItems();

        /// <summary>
        /// Compares the items in this collection to the items in the other 
        /// collection with regards to multiplicities, but without regards to
        /// sequence order.
        /// </summary>
        /// <param name="otherCollection">The collection to compare this
        /// collection to.</param>
        /// <returns><c>true</c> if the collections contain equal items;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// For each item in this collection there must be one equal
        /// to it in the other collection with the same multiplicity, and vice
        /// versa.
        /// </para>
        /// <para>
        /// The comparison uses <b>this</b> collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> to determine item
        /// equality. If the two collections use different notions of item
        /// equality, there is no guarantee that this method is symmetric, i.e.
        /// the following test is undetermined:
        /// <code>
        /// // Undetermined when coll1.EqualityComparer and coll2.EqualityComparer are not equal
        /// coll1.UnsequencedEquals(coll2) == coll2.UnsequencedEquals(coll1)
        /// </code>
        /// </para>
        /// </remarks>
        /// <seealso cref="GetUnsequencedHashCode"/>
        [Pure]
        bool UnsequencedEquals(ICollection<T> otherCollection);

        /// <summary>
        /// Determines whether the collection contains an item equal to
        /// <paramref name="item"/>, in which case that item is replaced with
        /// <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to update in the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <returns><c>true</c> if the item was found and hence updated;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// then all copies of the old item are updated; otherwise, only one 
        /// copy of <paramref name= "item"/> is updated. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the added item
        /// and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has set semantics or
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>false</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed
        /// item and a count of one.
        /// </description></item>
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
        bool Update(T item);

        /// <summary>
        /// Determines whether the collection contains an item equal to
        /// <paramref name="item"/>, in which case that item is replaced with
        /// <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to update in the collection. 
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <param name="oldItem">The removed item if any.</param>
        /// <returns><c>true</c> if the item was found and hence updated;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// then all copies of the old item are updated; otherwise, only one 
        /// copy of <paramref name= "item"/> is updated. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the added item
        /// and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has set semantics or
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>false</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed
        /// item and a count of one.
        /// </description></item>
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
        bool Update(T item, out T oldItem);

        /// <summary>
        /// Determines whether the collection contains an item equal to
        /// <paramref name="item"/>, in which case that item is replaced with
        /// <paramref name="item"/>; otherwise, the item is added.
        /// </summary>
        /// <param name="item">The item to update or add to the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <returns><c>true</c> if the item was found and hence updated;
        /// otherwise, <c>false</c> in which case the item is added.</returns>
        /// <remarks>
        /// <para>
        /// If the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// then all copies of the old item are updated; otherwise, only one 
        /// copy of <paramref name= "item"/> is updated. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the added item
        /// and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has set semantics or
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>false</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the added item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If the item is added, it raises the following events (in that
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
        bool UpdateOrAdd(T item);

        /// <summary>
        /// Determines whether the collection contains an item equal to
        /// <paramref name="item"/>, in which case that item is replaced with
        /// <paramref name="item"/>; otherwise, the item is added.
        /// </summary>
        /// <param name="item">The item to update or add to the collection.
        /// <c>null</c> is allowed, if
        /// <see cref="ICollectionValue{T}.AllowsNull"/> is <c>true</c>.
        /// </param>
        /// <param name="oldItem">The removed item if any.</param>
        /// <returns><c>true</c> if the item was found and hence updated;
        /// otherwise, <c>false</c> in which case the item is added.</returns>
        /// <remarks>
        /// <para>
        /// If the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// then all copies of the old item are updated; otherwise, only one 
        /// copy of <paramref name= "item"/> is updated. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has bag semantics and
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>true</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed 
        /// item and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the added item
        /// and a count of the item's multiplicity.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If the item is updated, and the collection has set semantics or
        /// <see cref="IExtensible{T}.DuplicatesByCounting"/> is <c>false</c>,
        /// it raises the following events (in that order) with the collection
        /// as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsRemoved"/> with the removed
        /// item and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.ItemsAdded"/> with the added item
        /// and a count of one.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If the item is added, it raises the following events (in that
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
        bool UpdateOrAdd(T item, out T oldItem);
    }


    [ContractClassFor(typeof(ICollection<>))]
    internal abstract class ICollectionContract<T> : ICollection<T>
    {
        // ReSharper disable InvocationIsSkipped

        public Speed ContainsSpeed
        {
            get
            {
                // No preconditions


                // Result is a valid enum constant
                Ensures(Enum.IsDefined(typeof(Speed), Result<Speed>()));


                return default(Speed);
            }
        }

        public int Count
        {
            get
            {
                // No additional preconditions allowed


                // No postconditions


                return default(int);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                // No additional preconditions allowed


                // No postconditions


                return default(bool);
            }
        }

        public bool Add(T item)
        {
            // No additional preconditions allowed


            // The collection will contain the item added
            Ensures(Contains(item));

            // The number of equal items increase by one
            Ensures(ContainsCount(item) == OldValue(ContainsCount(item)) + (Result<bool>() ? 1 : 0));


            return default(bool);
        }

        public void Clear()
        {
            // No additional preconditions allowed


            // No postconditions


            return;
        }

        public bool Contains(T item)
        {
            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Returns true if the collection contains the item
            Ensures(Result<bool>() == this.Contains(item, EqualityComparer));


            return default(bool);
        }


        public bool ContainsAll(SCG.IEnumerable<T> items)
        {
            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // All items must be non-null if collection disallows null values
            Requires(AllowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);


            // The collection contains the same items as items, with a multiplicity equal or greater
            Ensures(Result<bool>() == items.GroupBy(key => key, element => element, EqualityComparer).All(group => ContainsCount(group.Key) >= group.Count()));

            // Collection doesn't change if enumerator throws an exception
            EnsuresOnThrow<Exception>(this.SequenceEqual(OldValue(this.ToList())));


            return default(bool);
        }

        public int ContainsCount(T item)
        {
            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Result equals the number of items equal to item using the collection's equality comparer
            Ensures(Result<int>() == this.Count(x => EqualityComparer.Equals(x, item)));


            return default(int);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // No additional preconditions allowed


            // No postconditions


            return;
        }

        public bool Find(ref T item)
        {
            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Result is equal to Contains
            Ensures(Result<bool>() == Contains(item));

            // Ref parameter always equals itself
            Ensures(EqualityComparer.Equals(item, ValueAtReturn(out item))); // TODO: Test that this actually catches mistakes - try returning default(T)

            // If a non-value type instance is found and returned, it must come from the collection
            Ensures(typeof(T).IsValueType || !Result<bool>() || this.Contains(ValueAtReturn(out item), ComparerFactory.CreateReferenceEqualityComparer<T>())); // TODO: Test that this actually catches mistakes - try returning default(T)


            return default(bool);
        }

        public bool FindOrAdd(ref T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // The collection contains the item
            Ensures(Contains(item));

            // Result is equal to whether the collection already contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + (Result<bool>() ? 0 : 1));


            return default(bool);
        }

        public int GetUnsequencedHashCode()
        {
            return default(int);
        }

        public ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            // No preconditions


            // TODO: Ensure that the result contains the right items
            // this.GroupBy(key => key, element => element, EqualityComparer)
            //     .Select(group => new KeyValuePair<T, int>(group.Key, group.Count()))

            // Result is non-null
            Ensures(AllowsNull || ForAll(Result<ICollectionValue<KeyValuePair<T, int>>>(), pair => pair.Key != null));


            return default(ICollectionValue<KeyValuePair<T, int>>);
        }

        public bool Remove(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Returns true if the collection contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // Removing an item decreases the count by one
            Ensures(Count == OldValue(Count) - (Result<bool>() ? 1 : 0));

            // Removing the item decreases the number of equal items by one
            Ensures(this.Count(x => EqualityComparer.Equals(x, item)) == OldValue(this.Count(x => EqualityComparer.Equals(x, item))) - (Result<bool>() ? 1 : 0));


            return default(bool);
        }

        public bool Remove(T item, out T removedItem)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Returns true if the collection contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // Removing an item decreases the count by one
            Ensures(Count == OldValue(Count) - (Result<bool>() ? 1 : 0));

            // Removing the item decreases the number of equal items by one
            Ensures(this.Count(x => EqualityComparer.Equals(x, item)) == OldValue(this.Count(x => EqualityComparer.Equals(x, item))) - (Result<bool>() ? 1 : 0));

            // If an item was removed, the removed item equals the item to remove; otherwise, it equals the default value of T
            Ensures(EqualityComparer.Equals(ValueAtReturn(out removedItem), Result<bool>() ? item : default(T)));


            removedItem = default(T);
            return default(bool);
        }

        public bool RemoveAll(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Returns true if the collection contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // Removing all instances of an item decreases the count by its multiplicity
            Ensures(Count == OldValue(Count - ContainsCount(item)));

            // Removing the item decreases the number of equal items to zero
            Ensures(this.Count(x => EqualityComparer.Equals(x, item)) == 0);


            return default(bool);
        }

        public void RemoveAll(SCG.IEnumerable<T> items)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // All items must be non-null if collection disallows null values
            Requires(AllowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);


            // TODO: Write ensures

            // Collection doesn't change if enumerator throws an exception
            EnsuresOnThrow<Exception>(this.SequenceEqual(OldValue(this.ToList())));


            return;
        }

        public void RetainAll(SCG.IEnumerable<T> items)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null
            Requires(items != null, ArgumentMustBeNonNull);

            // All items must be non-null if collection disallows null values
            Requires(AllowsNull || ForAll(items, item => item != null), ItemsMustBeNonNull);


            // The collection is at most as big as the enumerable
            Ensures(Count <= (AllowsDuplicates ? items.Count() : items.Distinct(EqualityComparer).Count()));

            // The collection contains the same items as items, with a multiplicity equal or less
            Ensures(items.GroupBy(key => key, element => element, EqualityComparer).All(group => ContainsCount(group.Key) <= group.Count()));

            // Collection doesn't change if enumerator throws an exception
            EnsuresOnThrow<Exception>(this.SequenceEqual(OldValue(this.ToList())));

            // TODO: Ensure that the collection contains the right items


            return;
        }

        public ICollectionValue<T> UniqueItems()
        {
            // No preconditions


            // The result size must be equal to the number of distinct items
            Ensures(Result<ICollectionValue<T>>().Count == this.Distinct(EqualityComparer).Count());

            // TODO: Consider if this is the best solution. Maybe return read-only version/copy.
            // If the collection allows duplicates a new collection is created; otherwise, this collection is returned
            Ensures(AllowsDuplicates != ReferenceEquals(Result<ICollectionValue<T>>(), this));

            // Result is non-null
            Ensures(AllowsNull || ForAll(Result<ICollectionValue<T>>(), item => item != null));

            // Result contains the distinct items
            Ensures(Result<ICollectionValue<T>>().UnsequenceEqual(this.Distinct(EqualityComparer), EqualityComparer));


            return default(ICollectionValue<T>);
        }

        public bool UnsequencedEquals(ICollection<T> otherCollection)
        {
            // No preconditions


            // If the collections must contain a different number of (distinct) items, then they must be non-equal
            Ensures((Count == otherCollection.Count) || !Result<bool>());
            Ensures((this.Distinct(EqualityComparer).Count() == otherCollection.Distinct(EqualityComparer).Count()) || !Result<bool>());

            // Result reflects whether they are unsequenced equal
            Ensures(Result<bool>() == this.UnsequenceEqual(otherCollection, EqualityComparer));

            // If the collections have different unsequenced hash codes, then they must be non-equal
            Ensures((GetUnsequencedHashCode() == otherCollection.GetUnsequencedHashCode()) || !Result<bool>());


            return default(bool);
        }

        public bool Update(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Result is equal to whether the collection already contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // If the collection contained the item, it still does
            Ensures(OldValue(Contains(item)) == Contains(item));

            // Count remains unchanged
            Ensures(Count == OldValue(Count));

            // TODO: Make contract that ensures that the right number of items are updated based on AllowsDuplicates/DuplicatesByCounting



            return default(bool);
        }

        public bool Update(T item, out T oldItem)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // Result is equal to whether the collection already contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // If the collection contained the item, it still does
            Ensures(OldValue(Contains(item)) == Contains(item));

            // The item returned is either equal to the given item, if it was updated, or the default value of T if it was added
            Ensures(EqualityComparer.Equals(ValueAtReturn(out oldItem), Result<bool>() ? item : default(T)));

            // Count remains unchanged
            Ensures(Count == OldValue(Count));

            // TODO: Make contract that ensures that the right number of items are updated based on AllowsDuplicates/DuplicatesByCounting

            // Old value is non-null
            Ensures(!Result<bool>() || AllowsNull || ValueAtReturn(out oldItem) != null);


            oldItem = default(T);
            return default(bool);
        }

        public bool UpdateOrAdd(T item)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // The collection contains the item
            Ensures(Contains(item));

            // Result is equal to whether the collection already contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + (Result<bool>() ? 0 : 1));

            // TODO: Make contract that ensures that the right number of items are updated based on AllowsDuplicates/DuplicatesByCounting


            return default(bool);
        }

        public bool UpdateOrAdd(T item, out T oldItem)
        {
            // Collection must be non-read-only
            Requires(!IsReadOnly, CollectionMustBeNonReadOnly);

            // Collection must be non-fixed-sized
            Requires(!IsFixedSize, CollectionMustBeNonFixedSize);

            // Argument must be non-null if collection disallows null values
            Requires(AllowsNull || item != null, ItemMustBeNonNull);


            // The collection contains the item
            Ensures(Contains(item));

            // Result is equal to whether the collection already contained the item
            Ensures(Result<bool>() == OldValue(Contains(item)));

            // The item returned is either equal to the given item, if it was updated, or the default value of T if it was added
            Ensures(EqualityComparer.Equals(ValueAtReturn(out oldItem), Result<bool>() ? item : default(T)));

            // Adding an item increases the count by one
            Ensures(Count == OldValue(Count) + (Result<bool>() ? 0 : 1));

            // TODO: Make contract that ensures that the right number of items are updated based on AllowsDuplicates/DuplicatesByCounting

            // Old value is non-null
            Ensures(!Result<bool>() || AllowsNull || ValueAtReturn(out oldItem) != null);


            oldItem = default(T);
            return default(bool);
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

        #region IExtensible

        public abstract bool AllowsDuplicates { get; }
        public abstract bool DuplicatesByCounting { get; }
        public abstract SCG.IEqualityComparer<T> EqualityComparer { get; }
        public abstract bool IsFixedSize { get; }
        public abstract void AddAll(SCG.IEnumerable<T> items);

        #endregion

        #region SCG.ICollection<T>

        void SCG.ICollection<T>.Add(T item) {}

        #endregion

        #endregion
    }
}