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
    /// <summary>
    /// Defines the simplest set of methods to manipulate a general generic
    /// collection.
    /// </summary>
    [ContractClass(typeof(ICollectionContract<>))]
    public interface ICollection<T> : IExtensible<T>, SCG.ICollection<T>
    {
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
        /// <param name="item">The item to locate in the collection.</param>
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
        /// <c>null</c> is allowed for nullable items, but not for the
        /// enumerable itself.
        /// </param>
        /// <returns><c>true</c> if all items in <paramref name="items"/> are
        /// in the collection; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The collection's <see cref="IExtensible{T}.EqualityComparer"/>
        /// is used to determine item equality.
        /// </remarks>
        /// <seealso cref="Contains(T)"/>
        [Pure]
        bool ContainsAll(SCG.IEnumerable<T> items);


        /// <summary>
        /// Counts the number of items in the collection equal to the specified
        /// item.
        /// </summary>
        /// <param name="item">The item to count in the collection.</param>
        /// <returns>The number of items equal to the specified item found.
        /// Returns 0 if and only if the value is not in the collection.
        /// </returns>
        /// <remarks>The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.</remarks>
        [Pure]
        int ContainsCount(T item);


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
        /// Gets the number of items contained in the collection.
        /// </summary>
        /// <value>The number of items contained in the collection.</value>
        [Pure]
        new int Count { get; }


        /// <summary>
        /// Determines whether the collection contains a specific item and 
        /// assigns it to <paramref name="item"/> if so.
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed for nullable items. If the
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
        /// <c>null</c> is allowed for nullable items. If the collection 
        /// already contains the item, the item is assigned to the parameter on
        /// return; otherwise, the item is added to the collection.</param>
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
        /// <param name="item">The item to remove from the collection.</param>
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
        /// <param name="item">The item to remove from the collection.</param>
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
        /// <param name="item">The item to remove from the collection.</param>
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
        /// from the collection. <c>null</c> is allowed for nullable items, but
        /// not for the enumerable itself.</param>
        /// <remarks>
        /// <para>
        /// If the collection has bag semantics, this means reducing the item
        /// multiplicity of each item in the collection by at most the 
        /// multiplicity of the item in <paramref name="items"/>. The
        /// collection's <see cref="IExtensible{T}.EqualityComparer"/> is used
        /// to determine item equality.
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
        /// whose items should be retained in the collection.</param>
        /// <remarks>
        /// <para>The items remaining in the collection is the intersection
        /// between the original collection and the specified collection.</para>
        /// <para>The collection's <see cref="IExtensible{T}.EqualityComparer"/>
        /// is used to determine item equality.</para>
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


        /// <summary>
        /// Returns a collection value equal to this collection with all 
        /// duplicates removed.
        /// </summary>
        /// <returns>A collection value equal to this collection without
        /// duplicates.</returns>
        /// <remarks>If the given collection allows duplicates, a new
        /// collection is created and returned; if not, the given collection is
        /// returned. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.</remarks>
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
        /// <remarks>For each item in this collection there must be one equal
        /// to it in the other collection with the same multiplicity, and vice
        /// versa. The collection's
        /// <see cref="IExtensible{T}.EqualityComparer"/> is used to determine
        /// item equality.</remarks>
        /// <seealso cref="GetUnsequencedHashCode"/>
        [Pure]
        bool UnsequencedEquals(ICollection<T> otherCollection);


        /// <summary>
        /// Determines whether the collection contains an item equal to
        /// <paramref name="item"/>, in which case that item is replaced with
        /// <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to update in the collection.</param>
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
        /// <param name="item">The item to update in the collection.</param>
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
        public bool Add(T item)
        {
            throw new NotImplementedException();
        }


        public void Clear()
        {
            throw new NotImplementedException();
        }


        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }


        public bool ContainsAll(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }


        public int ContainsCount(T item)
        {
            throw new NotImplementedException();
        }


        public Speed ContainsSpeed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }


        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public bool Find(ref T item)
        {
            throw new NotImplementedException();
        }


        public bool FindOrAdd(ref T item)
        {
            throw new NotImplementedException();
        }


        public int GetUnsequencedHashCode()
        {
            throw new NotImplementedException();
        }


        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }


        public bool Remove(T item, out T removedItem)
        {
            throw new NotImplementedException();
        }


        public bool RemoveAll(T item)
        {
            throw new NotImplementedException();
        }


        public void RemoveAll(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }


        public void RetainAll(SCG.IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }


        public ICollectionValue<T> UniqueItems()
        {
            throw new NotImplementedException();
        }


        public bool UnsequencedEquals(ICollection<T> otherCollection)
        {
            throw new NotImplementedException();
        }


        public bool Update(T item)
        {
            throw new NotImplementedException();
        }


        public bool Update(T item, out T oldItem)
        {
            throw new NotImplementedException();
        }


        public bool UpdateOrAdd(T item)
        {
            throw new NotImplementedException();
        }


        public bool UpdateOrAdd(T item, out T oldItem)
        {
            throw new NotImplementedException();
        }


        #region Non-Contract Methods

        public abstract void AddAll(SCG.IEnumerable<T> items);
        public abstract bool AllowsDuplicates { get; }
        void ICollectionValue<T>.CopyTo(T[] array, int arrayIndex) { throw new NotImplementedException(); }
        void SCG.ICollection<T>.CopyTo(T[] array, int arrayIndex) { throw new NotImplementedException(); }
        int SCG.ICollection<T>.Count { get { throw new NotImplementedException(); } }
        public abstract bool DuplicatesByCounting { get; }
        public abstract SCG.IEqualityComparer<T> EqualityComparer { get; }
        bool IExtensible<T>.IsReadOnly { get { throw new NotImplementedException(); } }
        bool SCG.ICollection<T>.IsReadOnly { get { throw new NotImplementedException(); } }
        bool SCG.ICollection<T>.Remove(T item) { throw new NotImplementedException(); }
        public abstract SCG.IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
        void SCG.ICollection<T>.Add(T item) { throw new NotImplementedException(); }
        bool IExtensible<T>.Add(T item) { throw new NotImplementedException(); }
        void SCG.ICollection<T>.Clear() { throw new NotImplementedException(); }
        public abstract T Choose();
        bool SCG.ICollection<T>.Contains(T item) { throw new NotImplementedException(); }
        int ICollectionValue<T>.Count { get { throw new NotImplementedException(); } }
        public abstract Speed CountSpeed { get; }
        public abstract bool IsEmpty { get; }
        public abstract T[] ToArray();

        #endregion
    }
}