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
    /// Represents a sequenced collection whose items are accessible by index.
    /// </summary>
    [ContractClass(typeof(IIndexedContract<>))]
    public interface IIndexed<T> : ISequenced<T>, SCG.IReadOnlyList<T>
    {
        /// <summary>
        /// Gets the number of items contained in the collection.
        /// </summary>
        /// <value>The number of items contained in the collection.</value>
        [Pure]
        new int Count { get; }


        /// <summary>
        /// Returns an <see cref="IDirectedCollectionValue{T}"/> containing 
        /// the items in the specified index range of this collection.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count">The number of items in the
        ///     <see cref="IDirectedCollectionValue{T}"/>.</param>
        /// <returns>An <see cref="IDirectedCollectionValue{T}"/> containing 
        /// the items in the specified index range of this collection.
        /// </returns>
        /// <remarks>
        /// This is useful for enumerating an index range, either forwards or 
        /// backwards. Often used together with <see cref="IndexOf"/>. The
        /// forwards enumerator is equal to
        /// <c>coll.Skip(startIndex).Take(count)</c>, but potentially much
        /// faster.
        /// </remarks>
        [Pure]
        IDirectedCollectionValue<T> GetIndexRange(int startIndex, int count);


        /// <summary>
        /// Gets a value characterizing the asymptotic complexity of
        /// <see cref="SCG.IReadOnlyList{T}.this"/> proportional to collection
        /// size (worst-case or amortized as relevant).
        /// </summary>
        /// <value>A characterization of the asymptotic speed of
        /// <see cref="SCG.IReadOnlyList{T}.this"/> proportional to collection
        /// size.</value>
        [Pure]
        Speed IndexingSpeed { get; }


        // TODO: Move from IIndexed to IIndexedSorted? Introduce extension method for IIndexed?
        /// <summary>
        /// Searches from the beginning of the collection for the specified
        /// item and returns the zero-based index of the first occurrence
        /// within the collection. 
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns>The zero-based index of the first occurrence of item
        /// within the entire collection, if found; otherwise, the one's
        /// complement of the index at which <see cref="ICollection{T}.Add"/>
        /// would put the item.</returns>
        [Pure]
        int IndexOf(T item);


        // TODO: Two's complement?!
        // TODO: Move from IIndexed to IIndexedSorted? Introduce extension method for IIndexed?
        /// <summary>
        /// Searches from the end of the collection for the specified
        /// item and returns the zero-based index of the first occurrence
        /// within the collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.
        /// <c>null</c> is allowed for nullable items.</param>
        /// <returns>The zero-based index of the last occurrence of item within
        /// the entire collection, if found; otherwise, the one's complement of
        /// the index at which <see cref="ICollection{T}.Add"/> would put the
        /// item.</returns>
        [Pure]
        int LastIndexOf(T item);


        /// <summary>
        /// Removes the item at the specified index of the collection.
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
        T RemoveAt(int index);


        /// <summary>
        /// Remove all items in the specified index range.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count">The number of items to remove.</param>
        /// <remarks>
        /// If the cleared index range is non-empty, it raises the following
        /// events (in that order) with the collection as sender:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionCleared"/> as non-full and 
        /// with count equal to <paramref name="count"/>.
        /// </description></item>
        /// <item><description>
        /// <see cref="ICollectionValue{T}.CollectionChanged"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        void RemoveIndexRange(int startIndex, int count);
    }



    [ContractClassFor(typeof(IIndexed<>))]
    internal abstract class IIndexedContract<T> : IIndexed<T>
    {
        // ReSharper disable InvocationIsSkipped

        public IDirectedCollectionValue<T> GetIndexRange(int startIndex, int count)
        {
            // Argument must be within bounds
            Contract.Requires(0 <= startIndex); // TODO: Use <ArgumentOutOfRangeException>?
            Contract.Requires(startIndex + count <= Count); // TODO: Use <ArgumentOutOfRangeException>?

            // Argument must be non-negative
            Contract.Requires(0 <= count);


            // Result has the same count
            Contract.Ensures(Contract.Result<IDirectedCollectionValue<T>>().Count == count);

            // Result equals subrange
            Contract.Ensures(Contract.Result<IDirectedCollectionValue<T>>().SequenceEqual(this.Skip(startIndex).Take(count)));


            throw new NotImplementedException();
        }


        public Speed IndexingSpeed {
            get {
                // No Requires


                // Result is a valid enum constant
                Contract.Ensures(Enum.IsDefined(typeof(Speed), Contract.Result<Speed>()));


                throw new NotImplementedException();
            }
        }


        public int IndexOf(T item)
        {
            // No Requires


            // Result is a valid index
            Contract.Ensures(Contains(item)
                ? 0 <= Contract.Result<int>() && Contract.Result<int>() < Count
                : 0 <= ~Contract.Result<int>() && ~Contract.Result<int>() <= Count);

            // Item at index equals item
            Contract.Ensures(Contract.Result<int>() < 0 || EqualityComparer.Equals(item, this[Contract.Result<int>()]));

            // No item before index equals item
            Contract.Ensures(Contract.Result<int>() < 0 || !this.Take(Contract.Result<int>()).Contains(item, EqualityComparer));


            throw new NotImplementedException();
        }


        public int LastIndexOf(T item)
        {
            // No Requires


            // Result is a valid index
            Contract.Ensures(Contains(item)
                ? 0 <= Contract.Result<int>() && Contract.Result<int>() < Count
                : 0 <= ~Contract.Result<int>() && ~Contract.Result<int>() <= Count);

            // Item at index equals item
            Contract.Ensures(Contract.Result<int>() < 0 || EqualityComparer.Equals(item, this[Contract.Result<int>()]));

            // No item after index equals item
            Contract.Ensures(Contract.Result<int>() < 0 || !this.Skip(Contract.Result<int>() + 1).Contains(item, EqualityComparer));


            throw new NotImplementedException();
        }


        public T RemoveAt(int index)
        {
            // Argument must be within bounds (collection must be non-empty)
            Contract.Requires(0 <= index); // TODO: Use <IndexOutOfRangeException>?
            Contract.Requires(index < Count); // TODO: Use <IndexOutOfRangeException>?


            // Result is the item previously at the specified index
            Contract.Ensures(Contract.Result<T>().Equals(Contract.OldValue(this[index])));

            // Only the item at index is removed
            // TODO: Contract.Ensures(this.SequenceEqual(Contract.OldValue(this.SkipRange(index, 1).ToList())));

            // Removing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);


            throw new NotImplementedException();
        }


        public void RemoveIndexRange(int startIndex, int count)
        {
            // Argument must be within bounds (collection must be non-empty)
            Contract.Requires(0 <= startIndex); // TODO: Use <IndexOutOfRangeException>?
            Contract.Requires(startIndex + count < Count); // TODO: Use <IndexOutOfRangeException>?

            // Argument must be non-negative
            Contract.Requires(0 <= count);


            // Only the items in the index range are removed
            // TODO: Contract.Ensures(this.SequenceEqual(Contract.OldValue(this.SkipRange(startIndex, count).ToList())));

            // Removing an item decreases the count by one
            Contract.Ensures(Count == Contract.OldValue(Count) - count);


            throw new NotImplementedException();
        }


        // Static checker shortcoming: https://github.com/Microsoft/CodeContracts/issues/331
        public T this[int index] {
            get {
                // No extra Requires allowed


                // Result is item at index
                Contract.Ensures(Contract.Result<T>().Equals(this.Skip(index).First()));


                throw new NotImplementedException();
            }
        }


        // ReSharper restore InvocationIsSkipped


        #region Non-Contract Methods

        public abstract bool Add(T item);
        public abstract void Clear();
        public abstract bool Contains(T item);
        public abstract void CopyTo(T[] array, int arrayIndex);
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
        public abstract int Count { get; }
        public abstract bool Find(ref T item);
        public abstract bool FindOrAdd(ref T item);
        public abstract int GetUnsequencedHashCode();
        public abstract bool IsReadOnly { get; }
        public abstract bool ContainsAll(SCG.IEnumerable<T> items);
        public abstract int ContainsCount(T item);
        public abstract Speed ContainsSpeed { get; }
        public abstract void AddAll(SCG.IEnumerable<T> items);
        public abstract bool AllowsDuplicates { get; }
        void ICollectionValue<T>.CopyTo(T[] array, int arrayIndex) { throw new NotImplementedException(); }
        void SCG.ICollection<T>.CopyTo(T[] array, int arrayIndex) { throw new NotImplementedException(); }
        int SCG.ICollection<T>.Count { get { throw new NotImplementedException(); } }
        public abstract bool DuplicatesByCounting { get; }
        public abstract SCG.IEqualityComparer<T> EqualityComparer { get; }
        bool IExtensible<T>.IsReadOnly { get { throw new NotImplementedException(); } }
        bool SCG.ICollection<T>.IsReadOnly { get { throw new NotImplementedException(); } }
        public abstract ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities();
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
        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { throw new NotImplementedException(); }
        IDirectedCollectionValue<T> IDirectedCollectionValue<T>.Backwards() { throw new NotImplementedException(); }
        public abstract EnumerationDirection Direction { get; }
        public abstract int GetSequencedHashCode();
        public abstract bool SequencedEquals(ISequenced<T> otherCollection);

        #endregion
    }
}